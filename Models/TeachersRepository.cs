using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;

namespace Models
{
    public class TeachersRepository : Repository<Teacher>
    {
		public override int Add(Teacher teacher)
		{
			teacher.Code = Teacher.GenerateCode();
			return base.Add(teacher);
		}
		public List<int> Years()
		{
			List<int> Years = new List<int>();
			foreach (Teacher teacher in ToList().OrderBy(m => m.Year))
			{
				if (Years.IndexOf(teacher.Year) == -1)
				{
					Years.Add(teacher.Year);
				}
			}
			return Years;
		}
		public SelectList ToSelectList()
		{
			return SelectListUtilities<Teacher>.Convert(ToList().OrderBy(m => m.Code));
		}
		private void UpdateAllocation(Teacher teacher, List<int> coursesId)
		{
			teacher.DeleteNextSessionAllocations();
			if (coursesId != null && coursesId.Count > 0)
			{
				foreach (var courseId in coursesId)
				{
					DB.Allocations.Add(teacher.Id, courseId);
				}
			}
		}
		private void DeleteAllocations(Teacher teacher)
		{
			foreach (var course in teacher.Courses)
			{
				DB.Allocations.Delete(teacher.Id, course.Id);
			}
		}

		public bool Update(Teacher teacher, List<int> coursesId)
		{
			BeginTransaction();
			base.Update(teacher);
			UpdateAllocation(teacher, coursesId);
			EndTransaction();
			return true;
		}
		public override bool Delete(int Id)
		{
			BeginTransaction();
			Teacher teacher = Get(Id);
			if (teacher != null)
			{
				DeleteAllocations(teacher);
				base.Delete(Id);
			}
			EndTransaction();
			return true;
		}
	}
}