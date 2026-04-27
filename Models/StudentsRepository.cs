using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
	}
}