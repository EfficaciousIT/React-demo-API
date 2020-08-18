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
    public class HolidayController : ApiController
    {
        Database.DB record = new Database.DB();
        [System.Web.Http.HttpPost]
        public DataSet Post(string command, string intSchool_id, string intAcademic_id, string orderby)
        {
            Holiday holiday = new Holiday();
            holiday.intSchool_id = Convert.ToInt32(intSchool_id);
            holiday.intAcademic_id = Convert.ToInt32(intAcademic_id);
            holiday.orderby = orderby;
            DataSet ds = record.HolidayList(command, holiday);
            return ds;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post(string command, string vchHoliday_name, string dtFromDate, string dtToDate, string Description, string intInsertedBy, string vchInsertedIp, string intSchool_id, string intAcademic_id)
        {
            Holiday holiday = new Holiday();
            holiday.vchHoliday_name = Convert.ToString(vchHoliday_name);
            holiday.dtFromDate = Convert.ToString(dtFromDate);
            holiday.dtToDate = Convert.ToString(dtToDate);
            holiday.Description = Convert.ToString(Description);
            holiday.intInsertedBy = Convert.ToInt32(intInsertedBy);
            holiday.vchInsertedIp = Convert.ToString(vchInsertedIp);
            //holiday.intHoliday_Type_Id = Convert.ToInt32(intHoliday_Type_Id);
            holiday.intSchool_id = Convert.ToInt32(intSchool_id);
            holiday.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.HolidayList(command, holiday);
            // return ds;
            if (ds != null)
            {
                var message = Request.CreateResponse(HttpStatusCode.Created, "Record inserted Successfully .....");
                return message;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Error occured.....");
            }
        }
        [System.Web.Http.HttpPost]
        public DataSet Post(string command, int intHoliday_id, string intSchool_id, string intAcademic_id)
        {
            Holiday holiday = new Holiday();
            holiday.intHoliday_id = Convert.ToInt32(intHoliday_id);
            holiday.intSchool_id = Convert.ToInt32(intSchool_id);
            holiday.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.HolidayList(command, holiday);
            return ds;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage UpdateHoliday(string command, string intHoliday_id, string vchHoliday_name, string dtFromDate, string dtToDate, string Description, string intUpdatedBy, string vchUpdatedIp, string intSchool_id, string intAcademic_id)
        {
            Holiday holiday = new Holiday();
            holiday.intHoliday_id = Convert.ToInt32(intHoliday_id);
            holiday.vchHoliday_name = Convert.ToString(vchHoliday_name);
            holiday.dtFromDate = Convert.ToString(dtFromDate);
            holiday.dtToDate = Convert.ToString(dtToDate);
            holiday.Description = Convert.ToString(Description);
            holiday.intUpdatedBy = Convert.ToInt32(intUpdatedBy);
            holiday.vchUpdatedIp = Convert.ToString(vchUpdatedIp);
            // holiday.intHoliday_Type_Id = Convert.ToInt32(intHoliday_Type_Id);
            holiday.intSchool_id = Convert.ToInt32(intSchool_id);
            holiday.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.HolidayList(command, holiday);
            // return ds;
            if (ds != null)
            {
                var message = Request.CreateResponse(HttpStatusCode.Created, "Record Updated Successfully .....");
                return message;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Error occured.....");
            }
        }
        [HttpPost]
        public DataSet Delete(string command, string intHolidayId)
        {
            Holiday holiday = new Holiday();
            holiday.intHoliday_id = Convert.ToInt32(intHolidayId);
            DataSet ds = record.HolidayDelete(command, holiday);
            return ds;
        }
    }
}
