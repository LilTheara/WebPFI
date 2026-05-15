using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using Models;
using static Controllers.AccessControl;

namespace Controllers
{

	[UserAccess(Access.View)]
	public class StudentsController : Controller
	{
		private void InitSessionVariables()
		{
			// Session is a dictionary that hold keys values specific to a session
			// Each user of this web application have their own Session
			// A Session has a default time out of 20 minutes, after time out it is cleared

			//if (Session["CurrentMediaId"] == null) Session["CurrentMediaId"] = 0;
			//if (Session["CurrentMediaTitle"] == null) Session["CurrentMediaTitle"] = "";
			if (Session["Search"] == null) Session["Search"] = false;
			if (Session["SearchString"] == null) Session["SearchString"] = "";
			if (Session["SelectedYear"] == null) Session["SelectedYear"] = 0;
			//if (Session["Categories"] == null) Session["Categories"] = DB.Medias.MediasCategories();
			//if (Session["SortByTitle"] == null) Session["SortByTitle"] = true;
			//if (Session["MediaSortBy"] == null) Session["MediaSortBy"] = MediaSortBy.PublishDate;
			//if (Session["SortAscending"] == null) Session["SortAscending"] = false;
			//if (Session["SelectedOwnerId"] == null) Session["SelectedOwnerId"] = 0;

			ValidateSelectedYear();

			// paging handling
			if (Session["pageNum"] == null) Session["pageNum"] = 1;
			if (Session["firstPageSize"] == null) Session["firstPageSize"] = 20;
			if (Session["pageSize"] == null) Session["pageSize"] = 3;
			if (Session["EndOfStudents"] == null) Session["EndOfStudents"] = false;
			if (Session["StudentsYearsList"] == null) Session["StudentsYearsList"] = DB.Students.Years();
			if (Session["RegistrationsYearsList"] == null) Session["RegistrationsYearsList"] = DB.Registrations.Years();
		}
		private List<Student> _getItems(int index, int nbItems)
		{
			try
			{
				IEnumerable<Student> result = null;

				InitSessionVariables();

				bool search = (bool)Session["Search"];
				string searchString = (string)Session["SearchString"];

				result = DB.Students.ToList();

				if (search)
				{
					result = result.Where(c => (c.LastName.ToLower() + c.FirstName.ToLower()).Contains(searchString));
					int SelectedYear = (int)Session["SelectedYear"];
					if(SelectedYear != 0)
						result = result.Where(y=>y.Year == SelectedYear);
				}

				if (result.Count() < nbItems + index)
				{
					nbItems = result.Count() - index;
					Session["EndOfStudents"] = true;
				}
				return result.Skip(index).Take(nbItems).ToList();
			}
			catch (System.Exception ex)
			{
				return null;
			}
		}
		private void ResetStudentsPaging()
		{
			Session["pageNum"] = 1;
			Session["EndOfStudents"] = false;
		}
		private void ValidateSelectedYear()
		{
			if (Session["SelectedYear"] != null)
			{
				var selectedYear = (int)Session["SelectedYear"];
				var Students = DB.Students.ToList().Where(c => c.Year == selectedYear);
				if (Students.Count() == 0)
					Session["SelectedYear"] = 0;
			}
		}
		public ActionResult ToggleSearch()
		{
			ResetStudentsPaging();
			if (Session["Search"] == null) Session["Search"] = false;
			Session["Search"] = !(bool)Session["Search"];
			return RedirectToAction("List");
		}
		public ActionResult SetSearchString(string value)
		{
			ResetStudentsPaging();
			Session["SearchString"] = value.ToLower();
			return RedirectToAction("List");
		}
		public ActionResult SetSearchYear(int value)
		{
			ResetStudentsPaging();
			Session["SelectedYear"] = value;
			return RedirectToAction("List");
		}
		public ActionResult List()
		{
			return View();
		}
		public ActionResult Details(int id)
		{
			Session["CurrentStudentId"] = id;
			Student student = DB.Students.Get(id);
			Session["UserCanEditCurrentStudent"] = false;
			if (student != null)
			{
				Session["CurrentStudentName"] = student.FullName;
				Session["UserCanEditCurrentStudent"] = Models.User.ConnectedUser.Access >= Access.Write || Models.User.ConnectedUser.IsAdmin;
				return View(student);
			}
			return RedirectToAction("List");
		}
		[UserAccess(Models.Access.Write)]
		public ActionResult Delete()
		{
			int id = Session["CurrentStudentId"] != null ? (int)Session["CurrentStudentId"] : 0;
			if (id != 0)
			{
				DB.Students.Delete(id);
			}
			return RedirectToAction("List");
		}
		[UserAccess(Access.Write)]
		public ActionResult Create()
		{
			return View(new Student());
		}

		[HttpPost]
		[UserAccess(Access.Write)]
		[ValidateAntiForgeryToken()]
		public ActionResult Create(Student Student)
		{
			if (Student.IsValid())
			{
				DB.Students.Add(Student);
				DB.Events.Add("Create", Student.FullName);
				return RedirectToAction("List");
			}
			DB.Events.Add("Illegal Create Student");
			return Redirect("/Accounts/Login?message=Erreur de creation de Student!&success=false");
		}
		[UserAccess(Access.Write)]
		public ActionResult Edit()
		{
			int id = Session["CurrentStudentId"] != null ? (int)Session["CurrentStudentId"] : 0;
			if (id != 0)
			{
				Student student = DB.Students.Get(id);
				if (student != null)
				{
					if (Models.User.ConnectedUser.Access >= Access.Write || Models.User.ConnectedUser.IsAdmin)
						return View(student);
				}
			}
			return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
		}

		[UserAccess(Access.Write)]
		[HttpPost]
		[ValidateAntiForgeryToken()]
		public ActionResult Edit(Student student, List<int> SelectedCourses)
		{
			int id = Session["CurrentStudentId"] != null ? (int)Session["CurrentStudentId"] : 0;

			Student storedStudent = DB.Students.Get(id);
			if (storedStudent != null)
			{
				student.Id = id;

				if (student.IsValid())
				{
					DB.Students.Update(student, SelectedCourses);
					return RedirectToAction("Details/" + id);
				}
			}
			DB.Events.Add("Illegal Edit Student");
			return Redirect("/Accounts/Login?message=Erreur de modification de Student!&success=false");
		}
		public JsonResult CheckConflict(string Email)
		{
			int id = Session["CurrentStudentId"] != null ? (int)Session["CurrentStudentId"] : 0;
			// Response json value true if name is used in other Medias than the current Media
			return Json(DB.Students.ToList().Where(c => c.Email == Email && c.Id != id).Any(),
						JsonRequestBehavior.AllowGet /* must have for CORS verification by client browser */);
		}
		public ActionResult GetStudents(bool forceRefresh = false)
		{
			try
			{
				if (DB.Students.HasChanged || forceRefresh)
				{
					Session["StudentsYearsList"] = DB.Students.Years();
					InitSessionVariables();
					int pageNum = (int)Session["pageNum"];
					int pageSize = (int)Session["pageSize"];
					int firstPageSize = (int)Session["firstPageSize"];
					return PartialView(_getItems(0, pageNum > 1 ? (pageNum - 1) * pageSize + firstPageSize : firstPageSize));
				}
				return null;

			}
			catch (System.Exception ex)
			{
				return Content("Erreur interne " + ex.Message, "text/html");
			}
		}
		public ActionResult GetStudentDetails(bool forceRefresh = false)
		{
			try
			{
				Session["RegistrationsYearsList"] = DB.Registrations.Years();
				InitSessionVariables();

				int studentId = (int)Session["CurrentStudentId"];
				Student student = DB.Students.Get(studentId);
				if (DB.Students.HasChanged || forceRefresh)
				{
					return PartialView(student);
				}
				return null;
			}
			catch (System.Exception ex)
			{
				return Content("Erreur interne" + ex.Message, "text/html");
			}
		}
		public ActionResult GetYearsList()
		{
			try
			{
				InitSessionVariables();
				bool search = (bool)Session["Search"];

				if (search)
				{
					var years = DB.Students.Years();
					ViewBag.SelectedYear = (int)Session["SelectedYear"];
					return PartialView(years);
				}
				return null;
			}
			catch (System.Exception ex)
			{
				return Content("Erreur interne " + ex.Message, "text/html");
			}
		}

		public ActionResult SetYear()
		{
			ViewBag.Year = NextSession.Year;
			ViewBag.Session = NextSession.ValidSessions.Contains(1) ? "Automne" : "Hiver";
			return View();
		}

		[HttpPost]
		public ActionResult SetYear(int year, string session)
		{
			NextSession.CurrentDate = new DateTime(year, (session == "Automne" ? 8 : 1), 15);
			return RedirectToAction("List");
		}
	}
}