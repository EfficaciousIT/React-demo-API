using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewDemo.Models
{
    public class Attendance
    {
        public int intSchool_id { get; set; }
        public int intAcademic_id { get; set; }
        public int intUserType_id { get; set; }

        public int intUser_id { get; set; }
        public int intStandard_id { get; set; }
        public int intDivision_id { get; set; }
        public DateTime Dtdate { get; set; }
        public string status { get; set; }
        public int intRole_id { get; set; }
        public int intPerson_id { get; set; }
        public string dtDate { get; set; }
    }
}