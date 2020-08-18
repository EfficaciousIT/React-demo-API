using NewDemo.Database;
using NewDemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace NewDemo.Controllers
{
    [CustomeAuthenticationFilter]
    public class AttendanceController : ApiController
    {
        NewDemo.Database.DB record = new NewDemo.Database.DB();
        public DataSet Get(string command, string intUserType_id, string intUser_id, string intSchool_id, string intAcademic_id)
        {
            Attendance attendance = new Attendance();
            attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
            attendance.intSchool_id = Convert.ToInt32(intSchool_id);
            attendance.intUserType_id = Convert.ToInt32(intUserType_id);
            attendance.intUser_id = Convert.ToInt32(intUser_id);
            DataSet ds = record.AttendanceDetail(command, attendance);
            return ds;
        }
        public DataSet Get(string command, string intUserType_id, string intUser_id, string intSchool_id, string intAcademic_id, string intRole_id)
        {
            Attendance attendance = new Attendance();
            attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
            attendance.intSchool_id = Convert.ToInt32(intSchool_id);
            attendance.intUserType_id = Convert.ToInt32(intUserType_id);
            attendance.intUser_id = Convert.ToInt32(intUser_id);
            attendance.intRole_id = Convert.ToInt32(intRole_id);
            DataSet ds = record.AttendanceDetail(command, attendance);
            return ds;
        }
        public DataSet Get(string command, string intUserType_id, string intUser_id, string intSchool_id, string intAcademic_id, string intRole_id, string intDivision_id)
        {
            Attendance attendance = new Attendance();
            attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
            attendance.intSchool_id = Convert.ToInt32(intSchool_id);
            attendance.intUserType_id = Convert.ToInt32(intUserType_id);
            attendance.intUser_id = Convert.ToInt32(intUser_id);
            attendance.intRole_id = Convert.ToInt32(intRole_id);
            attendance.intDivision_id = Convert.ToInt32(intDivision_id);
            DataSet ds = record.AttendanceDetail(command, attendance);
            return ds;
        }
        public DataSet Get(string command, string intUserType_id, string intUser_id, string intSchool_id, string intAcademic_id, int intRole_id, string intperson_id)
        {
            Attendance attendance = new Attendance();
            attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
            attendance.intSchool_id = Convert.ToInt32(intSchool_id);
            attendance.intUserType_id = Convert.ToInt32(intUserType_id);
            attendance.intPerson_id = Convert.ToInt32(intperson_id);
            attendance.intRole_id = Convert.ToInt32(intRole_id);
            DataSet ds = record.AttendanceDetail(command, attendance);
            return ds;
        }
        public DataSet Get(string command, string intRole_id, string intPerson_id, string dtDate, string status, string intSchool_id, string intAcademic_id, string intStandard_id, string intDivision_id)
        {
            Attendance attendance = new Attendance();
            attendance.intRole_id = Convert.ToInt32(intRole_id);
            attendance.intPerson_id = Convert.ToInt32(intPerson_id);
            attendance.dtDate = Convert.ToString(dtDate);
            attendance.status = Convert.ToString(status);
            attendance.intSchool_id = Convert.ToInt32(intSchool_id);
            attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
            attendance.intStandard_id = Convert.ToInt32(intStandard_id);
            attendance.intDivision_id = Convert.ToInt32(intDivision_id);
            //    attendance.intRole_id = Convert.ToInt32(intRole_id);
            DataSet ds = record.AttendanceDetail(command, attendance);
            return ds;
        }
        public DataSet Get(string command, int intRole_id, string intPerson_id, string dtDate, string status, string intSchool_id, string intAcademic_id)
        {
            Attendance attendance = new Attendance();
            attendance.intRole_id = Convert.ToInt32(intRole_id);
            attendance.intPerson_id = Convert.ToInt32(intPerson_id);
            attendance.dtDate = Convert.ToString(dtDate);
            attendance.status = Convert.ToString(status);
            attendance.intSchool_id = Convert.ToInt32(intSchool_id);
            attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
            attendance.intRole_id = Convert.ToInt32(intRole_id);
            DataSet ds = record.AttendanceDetail(command, attendance);
            return ds;
        }
        public HttpResponseMessage Post(string command, int intRole_id, string intPerson_id, string dtDate, string status, string intSchool_id, string intAcademic_id, string intStandard_id, string intDivision_id)
        {
            try
            {
                Attendance attendance = new Attendance();
                attendance.intRole_id = Convert.ToInt32(intRole_id);
                attendance.intPerson_id = Convert.ToInt32(intPerson_id);
                attendance.dtDate = Convert.ToString(dtDate);
                attendance.status = Convert.ToString(status);
                attendance.intSchool_id = Convert.ToInt32(intSchool_id);
                attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
                attendance.intStandard_id = Convert.ToInt32(intStandard_id);
                attendance.intDivision_id = Convert.ToInt32(intDivision_id);
                DataSet ds = record.AttendanceDetail(command, attendance);
                var message = Request.CreateResponse(HttpStatusCode.Created);
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        public HttpResponseMessage Post(string command, int intRole_id, string intPerson_id, string dtDate, string status, string intSchool_id, string intAcademic_id)
        {
            try
            {
                Attendance attendance = new Attendance();
                attendance.intRole_id = Convert.ToInt32(intRole_id);
                attendance.intPerson_id = Convert.ToInt32(intPerson_id);
                attendance.dtDate = Convert.ToString(dtDate);
                attendance.status = Convert.ToString(status);
                attendance.intSchool_id = Convert.ToInt32(intSchool_id);
                attendance.intAcademic_id = Convert.ToInt32(intAcademic_id);
                DataSet ds = record.AttendanceDetail(command, attendance);
                var message = Request.CreateResponse(HttpStatusCode.Created);
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}