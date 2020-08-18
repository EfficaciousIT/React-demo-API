using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewDemo.Models
{
    public class Login
    {
        public string vchFCMToken { get; set; }
        public string vchEmail { get; set; }
        public int intUser_id { get; set; }
        public int intUserType_id { get; set; }
        public int intStudent_id { get; set; }
        public string vchStudentFirst_name { get; set; }
        public string vchStudentLast_name { get; set; }
        public string name { get; set; }
        public string std { get; set; }
        public int intstandard_id { get; set; }
        public int intDivision_id { get; set; }
        public string vchStandard_name { get; set; }
        public string vchDivisionName { get; set; }
        public string vchUser_name { get; set; }
        public string vchPassword { get; set; }
        public int intAcademic_id { get; set; }
        public int intTeacher_id { get; set; }
        public string vchFirst_name { get; set; }
        public string vchLast_name { get; set; }
        public int intStaff_id { get; set; }
        public int intAdmin_id { get; set; }
        public int intPrincipal_id { get; set; }
        public int intManager_id { get; set; }
        public int intSchool_id { get; set; }
        public string deviceId { get; set; }
        public string jwtToken { get; set; }
        public string jwtTokenIssueddt { get; set; }
        public string jwtTokenExpdt { get; set; }
    }
}