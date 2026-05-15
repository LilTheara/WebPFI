using DAL;
using Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Registrar.Models
{
    public class Course : Record
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int Session { get; set; }

        [JsonIgnore]
        public string Caption => Code + " " + Title;

        [JsonIgnore]
        public List<Registration> Registrations =>
            DB.Registrations.ToList().Where(r => r.CourseId == Id).ToList();

        [JsonIgnore]
        public List<Registration> NextSessionRegistrations =>
            DB.Registrations.ToList().Where(r => r.CourseId == Id && r.isNextSession).ToList();

        [JsonIgnore]
        public List<Student> Students
        {
            get
            {
                var students = new List<Student>();
                foreach (var registration in Registrations.OrderBy(r => r.Student.Code))
                {
                    students.Add(registration.Student);
                }
                return students;
            }
        }

        [JsonIgnore]
        public List<Student> NextSessionStudents
        {
            get
            {
                var students = new List<Student>();
                foreach (var registration in NextSessionRegistrations.OrderBy(r => r.Student.Code))
                {
                    students.Add(registration.Student);
                }
                return students;
            }
        }

        [JsonIgnore]
        public SelectList StudentsSelectList =>
            SelectListUtilities<Student>.Convert(Students, "Caption");

        [JsonIgnore]
        public SelectList NextSessionStudentsToSelectList =>
            SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");

        public override bool IsValid()
        {
            if (!HasRequiredLength(Code, 1)) return false;
            if (!HasRequiredLength(Title, 1)) return false;
            if (Session < 1 || Session > 6) return false;
            if (DB.Courses.ToList().Where(c => c.Code == Code && c.Id != Id).Any()) return false;
            return true;
        }

        public void DeleteAllRegistrations()
        {
            foreach (Registration registration in Registrations)
                DB.Registrations.Delete(registration.Id);
        }

        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration registration in NextSessionRegistrations)
                DB.Registrations.Delete(registration.Id);
        }

        public void UpdateRegistrations(List<int> selectedStudentsId)
        {
            DeleteNextSessionRegistrations();

            if (selectedStudentsId != null)
            {
                foreach (int studentId in selectedStudentsId)
                    DB.Registrations.Add(new Registration { StudentId = studentId, CourseId = Id });
            }
        }
    }
}