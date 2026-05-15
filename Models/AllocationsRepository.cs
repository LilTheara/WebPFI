using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using Models;

namespace Models
{
	public class AllocationsRepository : Repository<Allocation>
	{
		public bool Delete(int teacherId, int courseId)
		{
			Allocation allocation = DB.Allocations.ToList().Where(c => c.TeacherId == teacherId && c.CourseId == courseId).FirstOrDefault();
			if (allocation != null)
			{
				return base.Delete(allocation.Id);
			}
			return false;
		}
		public int Add(int teacherId, int courseId)
		{
			Allocation allocation = new Allocation { TeacherId = teacherId, CourseId = courseId };
			return base.Add(allocation);
		}
		public List<int> Years()
		{
			List<int> Years = new List<int>();
			foreach (Allocation allocation in ToList().OrderBy(m => m.Year))
			{
				if (Years.IndexOf(allocation.Year) == -1)
				{
					Years.Add(allocation.Year);
				}
			}
			return Years;
		}
	}
}