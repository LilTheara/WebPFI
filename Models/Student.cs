using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using Newtonsoft.Json;

namespace Models
{
	public class Student : Record
	{
		public string Code { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime BirthDate { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		[JsonIgnore] public string FullName => LastName + " " + FirstName;
		[JsonIgnore] public string Caption => Code + " " + LastName + " " + FirstName;
		[JsonIgnore] public int Year => int.Parse(Code.Substring(0, 4));
		//[JsonIgnore] public SelectList StudentsSelectList => SelectListUtilities<Student>.Convert(Students, "Caption");
		//[JsonIgnore]
		//public SelectList NextSessionStudentsToSelectList => SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");
	}
}