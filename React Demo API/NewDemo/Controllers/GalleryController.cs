using NewDemo.Database;
using NewDemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NewDemo.Controllers
{
    [CustomeAuthenticationFilter]
    public class GalleryController : ApiController
    {
        Database.DB record = new Database.DB();
        [System.Web.Http.HttpPost]
        public DataSet Post(string command, string intSchool_id, string intAcademic_id)
        {
            Gallery gallery = new Gallery();
            gallery.intSchool_id = Convert.ToInt32(intSchool_id);
            gallery.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.GallerDetails(command, gallery);
            return ds;
        }
        [System.Web.Http.HttpPost]
        public DataSet Post(string command, string intSchool_id, string intAcademic_id, string GalleryId)
        {
            Gallery gallery = new Gallery();
            gallery.intSchool_id = Convert.ToInt32(intSchool_id);
            gallery.intAcademic_id = Convert.ToInt32(intAcademic_id);
            gallery.intGallery_id = Convert.ToInt32(GalleryId);
            DataSet ds = record.GallerDetails(command, gallery);
            return ds;
        }
        [System.Web.Http.HttpPost]
        public DataSet Post(string command, string EventName,string intInserted_by,string InsertedIP, string intSchool_id, string intAcademic_id)
        {
            Gallery gallery = new Gallery();
            gallery.intSchool_id = Convert.ToInt32(intSchool_id);
            gallery.intAcademic_id = Convert.ToInt32(intAcademic_id);
            gallery.EventName = EventName;
            gallery.intInserted_by = intInserted_by;
            gallery.InsertedIP = InsertedIP;
            DataSet ds = record.GallerDetails(command, gallery);
            return ds;
        }

    }
}
