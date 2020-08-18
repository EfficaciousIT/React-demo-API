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
    public class VacationController : ApiController
    {
        Database.DB record = new Database.DB();

        [System.Web.Http.HttpPost]
        public DataSet Post(string command, string intSchool_id, string intAcademic_id)
        {
            Vacation vacation = new Vacation();
            vacation.intSchool_id = Convert.ToInt32(intSchool_id);
            vacation.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.VacationList(command, vacation);
            return ds;
        }

        [System.Web.Http.HttpPost]
        public DataSet Post(string command, string standards, string intSchool_id, string intAcademic_id, string orderby)
        {
            Vacation vacation = new Vacation();
            vacation.standards = standards;
            vacation.intSchool_id = Convert.ToInt32(intSchool_id);
            vacation.intAcademic_id = Convert.ToInt32(intAcademic_id);
            vacation.orderby = orderby;
            DataSet ds = record.VacationList(command, vacation);
            return ds;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddVacation(string command, string vchVacation_name, string standards, string dtFromDate, string dtToDate, string intSchool_id, string intInsertedBy, string vchInsertedIp, string Description, string intAcademic_id)
        {
            Vacation vacation = new Vacation();
            vacation.vchVacation_name = vchVacation_name;
            vacation.standards = Convert.ToString(standards);
            //vacation.ToStandard = Convert.ToInt32(ToStandard);            
            string dateString = Convert.ToDateTime(dtFromDate).ToString("dd/MM/yyyy");
            vacation.dtFromDate = dateString.Replace("-", "/");
            string Todate = Convert.ToDateTime(dtToDate).ToString("dd/MM/yyyy");
            vacation.dtToDate = Todate.Replace("-", "/");
            //vacation.dtFromDate = dtFromDate;
            //vacation.dtToDate = dtToDate;
            vacation.intSchool_id = Convert.ToInt32(intSchool_id);
            vacation.intInsertedBy = Convert.ToInt32(intInsertedBy);
            vacation.vchInsertedIp = vchInsertedIp;
            vacation.Description = Description;
            vacation.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.VacationList(command, vacation);
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
        public HttpResponseMessage EditVacation(string command, string intVacation_id, string vchVacation_name, string standards, string dtFromDate, string dtToDate, string intSchool_id, string intUpdatedBy, string vchUpdatedIp, string Description, string intAcademic_id)
        {
            Vacation vacation = new Vacation();
            vacation.intVacation_id = Convert.ToInt32(intVacation_id);
            vacation.vchVacation_name = vchVacation_name;
            vacation.standards = Convert.ToString(standards);
            string dateString = Convert.ToDateTime(dtFromDate).ToString("dd/MM/yyyy");
            vacation.dtFromDate = dateString.Replace("-", "/");
            string Todate = Convert.ToDateTime(dtToDate).ToString("dd/MM/yyyy");
            vacation.dtToDate = Todate.Replace("-", "/");
            vacation.intSchool_id = Convert.ToInt32(intSchool_id);
            vacation.intUpdatedBy = Convert.ToInt32(intUpdatedBy);
            vacation.vchUpdatedIp = vchUpdatedIp;
            vacation.Description = Description;
            vacation.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.VacationEdit(command, vacation);
            // return ds;
            if (ds != null)
            {
                var message = Request.CreateResponse(HttpStatusCode.Created, "Record Updated Successfully.....");
                return message;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Error occured.....");
            }
        }

        [System.Web.Http.HttpPost]
        public DataSet Post(string command, string intStandard_id, int intSchool_id, string intAcademic_id)
        {
            Vacation vacation = new Vacation();
            vacation.intStandard_id = Convert.ToInt32(intStandard_id);
            vacation.intSchool_id = Convert.ToInt32(intSchool_id);
            vacation.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.VacationList(command, vacation);
            return ds;
        }

        public DataSet Calendar(string command, string intSchool_id, string intAcademic_id, string standards, string month, string year, string orderby)
        {
            Vacation vacation = new Vacation();
            vacation.Month = Convert.ToInt32(month);
            vacation.Year = Convert.ToInt32(year);
            vacation.standards = standards;
            vacation.intSchool_id = Convert.ToInt32(intSchool_id);
            vacation.intAcademic_id = Convert.ToInt32(intAcademic_id);
            vacation.orderby = orderby;
            DataSet ds = record.VacationList(command, vacation);
            return ds;
        }

        [System.Web.Http.HttpPost]
        public DataSet Post(string command, int intVacationId, string intSchool_id, string intAcademic_id)
        {
            Vacation vacation = new Vacation();
            vacation.intVacation_id = Convert.ToInt32(intVacationId);
            vacation.intSchool_id = Convert.ToInt32(intSchool_id);
            vacation.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.VacationList(command, vacation);
            return ds;
        }

        [System.Web.Http.HttpPost]
        public DataSet Delete(string command, string intVacationId)
        {
            Vacation vacation = new Vacation();
            vacation.intVacation_id = Convert.ToInt32(intVacationId);
            DataSet ds = record.VacationDelete(command, vacation);
            return ds;
        }
    }
}
