using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace Models
{
	public class RegistrationsRepository : Repository<Registration>
	{
		public void DeleteByCourseId(int courseId)
		{
			List<Registration> list = ToList().Where(l => l.CourseId == courseId).ToList().Copy();
			list.ForEach(l => Delete(l.Id));
		}
		public void DeleteByStudentId(int studentId)
		{
			List<Registration> list = ToList().Where(l => l.StudentId == studentId).ToList().Copy();
			list.ForEach(l => Delete(l.Id));
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