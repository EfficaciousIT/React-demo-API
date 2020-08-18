﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewDemo.Models
{
    public class Vacation
    {
        public int intVacation_id { get; set; }
        public string vchVacation_name { get; set; }
        public int intRole_Id { get; set; } 
        public string standards { get; set; }
        public int intStandard_id { get; set; }
        public string dtFromDate { get; set; }
        public string dtToDate { get; set; }
        public int intSchool_id { get; set; }
        public int intInsertedBy { get; set; }
        public string vchInsertedIp { get; set; }
        public int intUpdatedBy { get; set; }
        public string vchUpdatedIp { get; set; }
        public int intDeletedBy { get; set; }
        public string vchDeletedIp { get; set; }
        public string Description { get; set; }
        public int intNoOfDay { get; set; }
        public int intAcademic_id { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
        public string orderby { get; set; }
    }
}