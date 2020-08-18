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
    public class ForgetPasswordController : ApiController
    {
        Database.DB record = new Database.DB();
        //Validate User ID
        public DataSet Get(String command, string UserID)
        {
            ForgetPassword forgetPassword = new ForgetPassword();
            forgetPassword.UserID = Convert.ToInt32(UserID);
            DataSet ds = record.ValidateUserID(command, UserID);
            return ds;
        }
        // Forget Password
        public DataSet Get(string command, string UserID, string mobile_number)
        {
            ForgetPassword forgetPassword = new ForgetPassword();
            forgetPassword.UserID = Convert.ToInt32(UserID);
            forgetPassword.mobile_no = Convert.ToString(mobile_number);
            DataSet ds = record.ForgetPassword(command, forgetPassword);
            return ds;
        }

        //Validate OTP While forget password
        public DataSet Get(string command, string UserID, string mobile_number, string OTP)
        {
            ForgetPassword forgetPassword = new ForgetPassword();
            forgetPassword.UserID = Convert.ToInt32(UserID);
            forgetPassword.mobile_no = Convert.ToString(mobile_number);
            forgetPassword.OTP = Convert.ToString(OTP);
            DataSet ds = record.ForgetPassOTPValidate(command, forgetPassword);
            return ds;
        }
        //Change Password
        public DataSet Get(string command, int UserID, string mobile_number, string Password)
        {
            ForgetPassword forgetPassword = new ForgetPassword();
            forgetPassword.UserID = Convert.ToInt32(UserID);
            forgetPassword.mobile_no = Convert.ToString(mobile_number);
            forgetPassword.Password = Convert.ToString(Password);
            DataSet ds = record.ChangePassword(command, forgetPassword);
            return ds;
        }
        [CustomeAuthenticationFilter]
        public HttpResponseMessage Put(string command, ForgetPassword forgetPassword)
        {
            try
            {
                string Message = "";
                //DataSet ds = record.UpdatePassword("UpdatePassword", forgetPassword);
                //var message = Request.CreateResponse(HttpStatusCode.OK, "Password Change Successful..");
                //return message;
               string usertype= forgetPassword.intusertypeId.ToString();
                string userid= forgetPassword.intuserId.ToString();
                int result = 0;
                result = record.UpdatePassword("UpdatePassword", forgetPassword);
                if (result > 0)
                {
                    bool Logout=  record.LogOutFromAllDevices(usertype, userid);
                     var message = Request.CreateResponse(HttpStatusCode.OK, "Password Change Successful..");
                    return message;
                }
                else
                {
                    var message = Request.CreateResponse(HttpStatusCode.OK, "Password Not Changed..");
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
