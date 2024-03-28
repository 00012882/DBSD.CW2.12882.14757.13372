using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DBSD.CW2._12882._14757._13372.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("LastName")]
        public string LastName { get; set; }

        [DisplayName("Phone")]
        public string Phone { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("HireDate")]
        public DateTime HireDate { get; set; }

        [Ignore]
        public byte[] EmployeeImage { get; set; }
        public bool FullTimeEmployee { get; set; }
    }
}