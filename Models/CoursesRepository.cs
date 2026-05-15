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
	}
}