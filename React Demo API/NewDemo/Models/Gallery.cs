using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewDemo.Models
{
    public class Gallery
    {
        public int intSchool_id { get; set; }
        public int intAcademic_id { get; set; }
        public int intUserType_id { get; set; }

        public int intUser_id { get; set; }
        public int intGallery_id { get; set; }

        public string EventName { get; set; }
        public string intInserted_by { get; set; }
        public string InsertedIP { get; set; }
    }
}