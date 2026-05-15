using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrar.Models
{
    public class Teacher : Record
    {
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Phone { get; set; }
        public string Avatar { get; set; }
    }
}