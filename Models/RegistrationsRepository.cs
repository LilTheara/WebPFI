using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace Models
{
	public class RegistrationsRepository : Repository<Registration>
	{
		public bool Delete(int studentId, int courseId)
		{
			Registration registration = DB.Registrations.ToList().Where(c => c.StudentId == studentId && c.CourseId == courseId).FirstOrDefault();
			if (registration != null)
			{
				return base.Delete(registration.Id);
			}
			return false;
		}
		public int Add(int studentId, int courseId)
		{
			Registration registration = new Registration { StudentId = studentId, CourseId = courseId };
			return base.Add(registration);
		}
		public List<int> Years()
		{
			List<int> Years = new List<int>();
			foreach (Registration registration in ToList().OrderBy(m => m.Year))
			{
				if (Years.IndexOf(registration.Year) == -1)
				{
					Years.Add(registration.Year);
				}
			}
			return Years;
		}
	}
}