using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrar.Models
{
    public class Course : Record
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int Session { get; set; }
    }
}