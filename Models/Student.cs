using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using Newtonsoft.Json;
using Registrar.Models;

namespace Models
{
	public class Student : Record
	{	
		public string Code { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime BirthDate { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		[JsonIgnore] public string FullName => LastName + " " + FirstName;
		[JsonIgnore] public string Caption => Code + " " + LastName + " " + FirstName;
		[JsonIgnore] public int Year => !string.IsNullOrEmpty(Code) ? int.Parse(Code.Substring(0, 4)) : 0;
		[JsonIgnore] public List<Registration> Registrations => DB.Registrations.ToList().Where(r => r.StudentId == Id).ToList();
		[JsonIgnore] public List<Registration> NextSessionRegistrations => DB.Registrations.ToList().Where(r => r.StudentId == Id && r.isNextSession).ToList();
		[JsonIgnore]
		public List<Course> Courses
		{
			get {
				var courses = new List<Course>();
				foreach (var registration in Registrations.OrderBy(r => r.Course.Code))
				{
					courses.Add(registration.Course);
				}
				return courses;
			}
		}
		[JsonIgnore]
		public List<Course> NextSessionCourses
		{
			get
			{
				var courses = new List<Course>();
				foreach (var registration in NextSessionRegistrations.OrderBy(r => r.Course.Code))
				{
					courses.Add(registration.Course);
				}
				return courses;
			}
		}
		public SelectList CoursesToSeleclist()
		{
			return SelectListUtilities<Course>.Convert(Courses, "Caption");
		}
		[JsonIgnore]
		public SelectList NextSessionCoursesToSelectList => SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");
		public override bool IsValid()
		{
			if (!HasRequiredLength(FirstName, 1)) return false;
			if (!HasRequiredLength(LastName, 1)) return false;
			if (!IsEmail(Email)) return false;
			return true;
		}
		public void DeleteAllRegistrations(){
			foreach (Registration registration in Registrations)
				DB.Registrations.Delete(registration.Id);
		}
		public void DeleteNextSessionRegistrations() {
			foreach (Registration registration in NextSessionRegistrations)
				DB.Registrations.Delete(registration.Id);
		}
		public void UpdateRegistrations(List<int> selectedCoursesId)
		{
			DeleteNextSessionRegistrations();
			if(selectedCoursesId != null)
				foreach (int courseId in selectedCoursesId)
					DB.Registrations.Add(new Registration { StudentId = Id, CourseId = courseId });
		}
		public static string GenerateCode()
		{
			Random rdn = new Random();
			string tempCode = "0";
			int num;
			do
			{
				num = rdn.Next(000000, 1000000);
				tempCode = NextSession.Year.ToString() + num.ToString();
			} while (DB.Students.ToList().Any(c => c.Code == tempCode));
			return tempCode;
		}
	}
}