using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using Models;
using Newtonsoft.Json;

namespace Models
{
    public class Teacher : Record
    {
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
        public string Avatar { get; set; }
		[JsonIgnore] public string FullName => LastName + " " + FirstName;
		[JsonIgnore] public string Caption => Code + " " + LastName + " " + FirstName;
		[JsonIgnore] public int Year => !string.IsNullOrEmpty(Code) ? int.Parse(Code.Substring(0, 4)) : 0;
		[JsonIgnore] public List<Allocation> Allocations => DB.Allocations.ToList().Where(r => r.TeacherId == Id).ToList();
		[JsonIgnore] public List<Allocation> NextSessionAllocations => DB.Allocations.ToList().Where(r => r.TeacherId == Id && r.isNextSession).ToList();
		[JsonIgnore]
		public List<Course> Courses
		{
			get
			{
				var courses = new List<Course>();
				foreach (var allocation in Allocations.OrderBy(r => r.Course.Code))
				{
					courses.Add(allocation.Course);
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
				foreach (var allocation in NextSessionAllocations.OrderBy(r => r.Course.Code))
				{
					courses.Add(allocation.Course);
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
			return true;
		}
		public void DeleteAllAllocations()
		{
			foreach (Allocation allocation in Allocations)
				DB.Allocations.Delete(allocation.Id);
		}
		public void DeleteNextSessionAllocations()
		{
			foreach (Allocation allocation in NextSessionAllocations)
				DB.Allocations.Delete(allocation.Id);
		}
		public void UpdateAllocations(List<int> selectedCoursesId)
		{
			DeleteNextSessionAllocations();
			if (selectedCoursesId != null)
				foreach (int courseId in selectedCoursesId)
					DB.Allocations.Add(new Allocation { TeacherId = Id, CourseId = courseId });
		}
		public static string GenerateCode()
		{
			Random rdn = new Random();
			string tempCode = "0";
			int num;
			do
			{
				num = rdn.Next(00000, 100000);
				tempCode = "CLG-420-" + num.ToString();
			} while (DB.Students.ToList().Any(c => c.Code == tempCode));
			return tempCode;
		}
	}
}