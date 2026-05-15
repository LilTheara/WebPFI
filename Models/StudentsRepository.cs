using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;

namespace Models
{
	public class StudentsRepository : Repository<Student>
	{
		public List<int> Years()
		{
			List<int> Years = new List<int>();
			foreach (Student student in ToList().OrderBy(m => m.Year))
			{
				if (Years.IndexOf(student.Year) == -1)
				{
					Years.Add(student.Year);
				}
			}
			return Years;
		}
		public SelectList ToSelectList()
		{
			return SelectListUtilities<Student>.Convert(ToList().OrderBy(m => m.Code));
		}
		private void UpdateRegistration(Student student, List<int> coursesId)
		{
			DeleteRegistrations(student);
			if (coursesId != null && coursesId.Count > 0)
			{
				foreach (var courseId in coursesId)
				{
					DB.Registrations.Add(student.Id, courseId);
				}
			}
		}
		private void DeleteRegistrations(Student student)
		{
			foreach (var course in student.Courses)
			{
				DB.Registrations.Delete(student.Id, course.Id);
			}
		}
		
		public bool Update(Student student, List<int> coursesId)
		{
			BeginTransaction();
			base.Update(student);
			UpdateRegistration(student, coursesId);
			EndTransaction();
			return true;
		}
		public override bool Delete(int Id)
		{
			BeginTransaction();
			Student student = Get(Id);
			if (student != null)
			{
				DeleteRegistrations(student);
				base.Delete(Id);
			}
			EndTransaction();
			return true;
		}
	}
}