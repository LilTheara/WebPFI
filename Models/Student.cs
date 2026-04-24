using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL;

namespace Models
{
	public class Student : Record
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Code { get; set; }
		public DateTime BirthDate { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
	}
}