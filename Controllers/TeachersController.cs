using DAL;
using Models;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace Controllers
{
    [UserAccess(Access.View)]
    public class TeachersController : Controller
    {
        private void InitSessionVariables()
        {
            if (Session["CurrentTeacherId"] == null) Session["CurrentTeacherId"] = 0;
            if (Session["CurrentTeacherName"] == null) Session["CurrentTeacherName"] = "";
            if (Session["Search"] == null) Session["Search"] = false;
            if (Session["SearchString"] == null) Session["SearchString"] = "";
        }

        private void ResetCurrentTeacherInfo()
        {
            Session["CurrentTeacherId"] = 0;
            Session["CurrentTeacherName"] = "";
        }

        public ActionResult List()
        {
            ResetCurrentTeacherInfo();
            return View();
        }

        public ActionResult GetTeachers(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                if (DB.Teachers.HasChanged || forceRefresh)
                {
                    IEnumerable<Teacher> teachers = DB.Teachers.ToList();

                    bool search = (bool)Session["Search"];
                    string searchString = ((string)Session["SearchString"]).ToLower();

                    if (search && searchString != "")
                    {
                        teachers = teachers.Where(t =>
                            t.Code.ToLower().Contains(searchString) ||
                            t.FirstName.ToLower().Contains(searchString) ||
                            t.LastName.ToLower().Contains(searchString));
                    }

                    teachers = teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName);

                    return PartialView(teachers);
                }

                return null;
            }
            catch (System.Exception ex)
            {
                return Content("Erreur interne " + ex.Message, "text/html");
            }
        }

        public ActionResult Details(int id)
        {
            InitSessionVariables();

            Teacher teacher = DB.Teachers.Get(id);

            if (teacher != null)
            {
                Session["CurrentTeacherId"] = id;
                Session["CurrentTeacherName"] = teacher.LastName + " " + teacher.FirstName;
                return View(teacher);
            }

            return RedirectToAction("List");
        }
		public JsonResult CheckConflict(string Email)
		{
			int id = Session["CurrentTeacherId"] != null ? (int)Session["CurrentTeacherId"] : 0;
			// Response json value true if name is used in other Medias than the current Media
			return Json(DB.Teachers.ToList().Where(c => c.Email == Email && c.Id != id).Any(),
						JsonRequestBehavior.AllowGet /* must have for CORS verification by client browser */);
		}
		public ActionResult GetTeacherDetails(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = Session["CurrentTeacherId"] != null ? (int)Session["CurrentTeacherId"] : 0;
                Teacher teacher = DB.Teachers.Get(id);

                if (teacher != null && (DB.Teachers.HasChanged || forceRefresh))
                {
                    return PartialView(teacher);
                }

                return null;
            }
            catch (System.Exception ex)
            {
                return Content("Erreur interne " + ex.Message, "text/html");
            }
        }

        public ActionResult ToggleSearch()
        {
            InitSessionVariables();
            Session["Search"] = !(bool)Session["Search"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string value)
        {
            InitSessionVariables();
            Session["SearchString"] = value == null ? "" : value.ToLower();
            return RedirectToAction("List");
        }

		[UserAccess(Models.Access.Write)]
		public ActionResult Delete()
		{
			int id = Session["CurrentTeacherId"] != null ? (int)Session["CurrentTeacherId"] : 0;
			if (id != 0)
			{
				DB.Teachers.Delete(id);
			}
			return RedirectToAction("List");
		}
		[UserAccess(Access.Write)]
		public ActionResult Create()
		{
			return View(new Teacher());
		}

		[HttpPost]
		[UserAccess(Access.Write)]
		[ValidateAntiForgeryToken()]
		public ActionResult Create(Teacher teacher)
		{
			if (teacher.IsValid())
			{
				DB.Teachers.Add(teacher);
				DB.Events.Add("Create", teacher.FullName);
				return RedirectToAction("List");
			}
			DB.Events.Add("Illegal Create Teacher");
			return Redirect("/Accounts/Login?message=Erreur de creation de Teacher!&success=false");
		}
		[UserAccess(Access.Write)]
		public ActionResult Edit()
		{
			int id = Session["CurrentTeacherId"] != null ? (int)Session["CurrentTeacherId"] : 0;
			Teacher teacher = DB.Teachers.Get(id);
			if (teacher != null)
			{
				if (Models.User.ConnectedUser.Access >= Access.Write || Models.User.ConnectedUser.IsAdmin)
					return View(teacher);
			}

			return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
		}

		[UserAccess(Access.Write)]
		[HttpPost]
		[ValidateAntiForgeryToken()]
		public ActionResult Edit(Teacher teacher, List<int> SelectedCourses)
		{
			int id = Session["CurrentTeacherId"] != null ? (int)Session["CurrentTeacherId"] : 0;

			Teacher storedTeacher = DB.Teachers.Get(id);
			if (storedTeacher != null)
			{
				teacher.Id = id;

				if (teacher.IsValid())
				{
					DB.Teachers.Update(teacher, SelectedCourses);
					return RedirectToAction("Details/" + id);
				}
			}
			DB.Events.Add("Illegal Edit Teacher");
			return Redirect("/Accounts/Login?message=Erreur de modification de Teacher!&success=false");
		}
	}
}