using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using Models;

namespace Registrar.Models
{
    public class CoursesRepository : Repository<Course>
    {
		public SelectList ToSelectList()
		{
			return SelectListUtilities<Course>.Convert(ToList().OrderBy(m => m.Code));
		}
		private void UpdateRegistration(Course course, List<int> studentsId)
		{
			DeleteRegistrations(course);
			if (studentsId != null && studentsId.Count > 0)
			{
				foreach (var studentId in studentsId)
				{
					DB.Registrations.Add(studentId, course.Id);
				}
			}
		}
		private void DeleteRegistrations(Course course)
		{
			foreach (var student in course.Students)
			{
				DB.Registrations.Delete(student.Id, course.Id);
			}
		}
		public bool Update(Course course, List<int> studentsId)
		{
			BeginTransaction();
			base.Update(course);
			UpdateRegistration(course, studentsId);
			EndTransaction();
			return true;
		}
	}
}