using DAL;
using Models;
using Registrar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Controllers.AccessControl;


namespace Registrar.Controllers
{

    [UserAccess(Access.View)]
    public class CoursesController : Controller
    {
        private void InitSessionVariables()
        {
            if (Session["CurrentCourseId"] == null) Session["CurrentCourseId"] = 0;
            if (Session["CurrentCourseTitle"] == null) Session["CurrentCourseTitle"] = "";
            if (Session["Search"] == null) Session["Search"] = false;
            if (Session["SearchString"] == null) Session["SearchString"] = "";
        }

        private void ResetCurrentCourseInfo()
        {
            Session["CurrentCourseId"] = 0;
            Session["CurrentCourseTitle"] = "";
        }

        public ActionResult List()
        {
            ResetCurrentCourseInfo();
            return View();
        }

        public ActionResult GetCourses(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                if (DB.Courses.HasChanged || forceRefresh)
                {
                    IEnumerable<Course> courses = DB.Courses.ToList();

                    bool search = (bool)Session["Search"];
                    string searchString = ((string)Session["SearchString"]).ToLower();

                    if (search && searchString != "")
                    {
                        courses = courses.Where(c =>
                            c.Code.ToLower().Contains(searchString) ||
                            c.Title.ToLower().Contains(searchString));
                    }

                    courses = courses.OrderBy(c => c.Session).ThenBy(c => c.Code);

                    return PartialView(courses);
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

            Course course = DB.Courses.Get(id);

            if (course != null)
            {
                Session["CurrentCourseId"] = id;
                Session["CurrentCourseTitle"] = course.Title;
                return View(course);
            }

            return RedirectToAction("List");
        }

        public ActionResult GetCourseDetails(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = Session["CurrentCourseId"] != null ? (int)Session["CurrentCourseId"] : 0;
                Course course = DB.Courses.Get(id);

                if (course != null && (DB.Courses.HasChanged || forceRefresh))
                {
                    return PartialView(course);
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
        [UserAccess(Access.Write)]
        public ActionResult Edit()
        {
            int id = Session["CurrentCourseId"] != null ? (int)Session["CurrentCourseId"] : 0;

            if (id != 0)
            {
                Course course = DB.Courses.Get(id);
                if (course != null)
                    return View(course);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [UserAccess(Access.Write)]
        public ActionResult Edit(Course course, List<int> SelectedStudents)
        {
            int id = Session["CurrentCourseId"] != null ? (int)Session["CurrentCourseId"] : 0;

            Course storedCourse = DB.Courses.Get(id);

            if (storedCourse != null)
            {
                course.Id = id;

                if (course.IsValid())
                {
                    DB.Courses.Update(course, SelectedStudents);
                    return RedirectToAction("Details/" + id);
                }
            }

            return Redirect("/Accounts/Login?message=Erreur de modification de Course!&success=false");
        }
        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            return View(new Course());
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Course course)
        {
            DB.Courses.Add(course);
            return RedirectToAction("List");
        }

    }
}