using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewDemo.Models
{
    public class Noticeboard
    {
        //public int intUserType_id { get; set; }
        //public string[] intStandard_id { get; set; }
        //public string[] intDepartment_id { get; set; }
        //public int intTeacher_id { get; set; }
        //public string dtIssue_date { get; set; }
        //public string dtEnd_date { get; set; }
        //public string vchSubject { get; set; }
        //public string vchNotice { get; set; }
        //public int intInserted_by { get; set; }
        //public string InsertIP { get; set; }
        //public int intSchool_id { get; set; }

        //public int intUser_id { get; set; }
        //public int intAcademic_id { get; set; }
        //public string[] intDivision_id { get; set; }
        public int intUserType_id { get; set; }
        public string intStandard_id { get; set; }
        public string intDepartment_id { get; set; }
        // public int intTeacher_id { get; set; }
        public string dtIssue_date { get; set; }
        public string dtEnd_date { get; set; }
        public string vchSubject { get; set; }
        public string vchNotice { get; set; }
        public int intInserted_by { get; set; }
        public string InsertIP { get; set; }
        public int intSchool_id { get; set; }

        public int intUser_id { get; set; }
        public int intAcademic_id { get; set; }
        public string intDivision_id { get; set; }
        public string ImageName { get; set; }
        public int intNotice_id { get; set; }

        public string image { get; set; }
        public int pageindex { get; set; }
        public string filterBy { get; set; }

        public string PDFName { get; set; }
        public string pdf { get; set; }
        public string Link { get; set; }
        public int visibleForTeacher { get; set; }

    }
}