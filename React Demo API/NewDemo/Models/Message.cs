using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewDemo.Models
{
    public class Message
    {
        public int intSchool_id { get; set; }
        public int intAcademic_id { get; set; }
        public string intUserType_id { get; set; }

        public string intUser_id { get; set; }
        public string intStandard_id { get; set; }
        public string intDivision_id { get; set; }

        public string intDepartment_id { get; set; }
        public string Mobile_number { get; set; }

        public string message { get; set; }
        public string messagetitle { get; set; }
        public string messagecount { get; set; }
        public string insertedby { get; set; }
        public string IP { get; set; }

        public string command { get; set; }
        public string MessageLanguage { get; set; }
        public string Status { get; set; }
        public string Msg_id { get; set; }
        public string PageIndex { get; set; }
        public int id { get; set; }
        public string UserName { get; set; }
    }
}