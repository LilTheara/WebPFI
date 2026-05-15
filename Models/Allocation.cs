using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using Models;
using Newtonsoft.Json;

namespace Models
{
	public class Allocation : Record
	{
		public Allocation()
		{
			Year = NextSession.Year;
		}
		public int TeacherId { get; set; }
		public int CourseId { get; set; }
		public int Year { get; set; }
		[JsonIgnore]
		public Teacher Teacher => DB.Teachers.Get(TeacherId);
		[JsonIgnore]
		public Course Course => DB.Courses.Get(CourseId);
		[JsonIgnore]
		public bool isNextSession => Year == NextSession.Year && NextSession.ValidSessions.Contains(Course.Session);
	}
}