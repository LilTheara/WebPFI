using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class CoursesRepository : Repository<Course>
    {
        public SelectList ToSelectList()
        {
            return SelectListUtilities<Course>.Convert(ToList().OrderBy(c => c.Code), "Caption");
        }

        public bool Update(Course course, List<int> SelectedStudents)
        {
            BeginTransaction();
            base.Update(course);
            course.UpdateRegistrations(SelectedStudents);
            EndTransaction();
            return true;
        }
    }
}