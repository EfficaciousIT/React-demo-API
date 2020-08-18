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
    public class LoginController : ApiController
    {
        Database.DB record = new Database.DB();
        public DataSet Get(string command, string vchUser_name, string vchPassword, string intSchool_id,string deviceId)
        {
            Login login = new Login();
            
            login.vchUser_name = vchUser_name;
            login.vchPassword = vchPassword;
            login.deviceId = deviceId;
            
            login.intSchool_id = Convert.ToInt32(intSchool_id);
            DataSet ds = record.LoginDetails(command, login);


            //check the column exits or not 
            DataColumnCollection col = ds.Tables[0].Columns;
            if (!col.Contains("Token"))
                ds.Tables[0].Columns.Add("Token");

            if (ds.Tables[0].Rows.Count > 0)
            {
                //string Message = TokenManager.GenerateToken(login.vchUser_name);
                //ds.Tables[0].Rows[0]["Token"] = Message;
                List<string> TokenDetails = TokenManager.GenerateToken(login.vchUser_name);
                login.jwtToken = TokenDetails[0];
                login.jwtTokenIssueddt = TokenDetails[1];
                login.jwtTokenExpdt = TokenDetails[2];
                record.JwtTokenLog("insertLogin", login);
                ds.Tables[0].Rows[0]["Token"] = login.jwtToken;
            }

            return ds;
        }

        [CustomeAuthenticationFilter]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Get(string command, Login login)
        {
            try
            {
                string Message = "";
                DataSet ds = record.LogoutUser("LogoutUser", login);
                var message = Request.CreateResponse(HttpStatusCode.OK, "Log Out Successful..");
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }


        }

        public DataSet UserDetail(string command, string username)
        {
            Login login = new Login();

            login.vchUser_name = username;

            DataSet ds = record.UserDetail(command, login);
            return ds;
        }
    }
}