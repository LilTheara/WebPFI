using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using Models;

namespace Controllers
{
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
			//if (Session["SelectedCategory"] == null) Session["SelectedCategory"] = "";
			//if (Session["Categories"] == null) Session["Categories"] = DB.Medias.MediasCategories();
			//if (Session["SortByTitle"] == null) Session["SortByTitle"] = true;
			//if (Session["MediaSortBy"] == null) Session["MediaSortBy"] = MediaSortBy.PublishDate;
			//if (Session["SortAscending"] == null) Session["SortAscending"] = false;
			//if (Session["SelectedOwnerId"] == null) Session["SelectedOwnerId"] = 0;

			//ValidateSelectedCategory();

			// paging handling
			if (Session["pageNum"] == null) Session["pageNum"] = 1;
			if (Session["firstPageSize"] == null) Session["firstPageSize"] = 20;
			if (Session["pageSize"] == null) Session["pageSize"] = 3;
			if (Session["EndOfMedias"] == null) Session["EndOfMedias"] = false;
			if (Session["StudentsYearsList"] == null) Session["StudentsYearsList"] = DB.Students.Years();
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
					result = result.Where(c => (c.LastName.ToLower() + c.FirstName.ToLower()).Contains(searchString));

				if (result.Count() < nbItems + index)
				{
					nbItems = result.Count() - index;
					Session["EndOfMedias"] = true;
				}
				return result.Skip(index).Take(nbItems).ToList();
			}
			catch (System.Exception ex)
			{
				return null;
			}
		}

		public ActionResult List()
		{
			return View();
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
		/*
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
			return RedirectToAction("Index");
		}
		*/
	}
}