using NewDemo.Database;
using NewDemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace NewDemo.Controllers
{
    [CustomeAuthenticationFilter]
    public class OTPValidateController : ApiController
    {
        NewDemo.Database.DB record = new NewDemo.Database.DB();
        public DataSet Get(string command, string intUserType_id, string mobile_number, int intuser_id)
        {
            OTPValidate OTPvalidate = new OTPValidate();
            OTPvalidate.intuser_type = Convert.ToInt32(intUserType_id);
            OTPvalidate.mobile_number = Convert.ToString(mobile_number);
            OTPvalidate.intuser_id = Convert.ToString(intuser_id);
            DataSet ds = record.OTPValidate(command, OTPvalidate);
            return ds;
        }
        public DataSet Get(string command, string intUserType_id, int intuser_id, string mobile_number,string OTP)
        {
            OTPValidate OTPvalidate = new OTPValidate();
            OTPvalidate.intuser_type = Convert.ToInt32(intUserType_id);
            OTPvalidate.intuser_id = Convert.ToString(intuser_id);
            OTPvalidate.mobile_number = Convert.ToString(mobile_number);
            OTPvalidate.OTP = Convert.ToString(OTP);
            DataSet ds = record.ValidatingOTP(command, OTPvalidate);
            return ds;
        }

        // For Email
        public DataSet Get(string command, string intUserType_id, string EmailId, string  intuser_id)
        {
            OTPValidate OTPvalidate = new OTPValidate();
            OTPvalidate.intuser_type = Convert.ToInt32(intUserType_id);
            OTPvalidate.EmailId = Convert.ToString(EmailId);
            OTPvalidate.intuser_id = Convert.ToString(intuser_id);
            DataSet ds = record.EmailValidation(command, OTPvalidate);
            return ds;
        }

        public DataSet Get(string command, string intUserType_id, string  intuser_id, string EmailId, string EmailOTP)
        {
            OTPValidate OTPvalidate = new OTPValidate();
            OTPvalidate.intuser_type = Convert.ToInt32(intUserType_id);
            OTPvalidate.intuser_id = Convert.ToString(intuser_id);
            OTPvalidate.EmailId = Convert.ToString(EmailId);
            OTPvalidate.EmailOTP = Convert.ToString(EmailOTP);
            DataSet ds = record.ValidatingEmailOTP(command, OTPvalidate);
            return ds;
        }


        // Forget Password
        //public DataSet Get(string command, string mobile_number)
        //{
        //    DataSet ds = record.ForgetPassword(command, mobile_number);
        //    return ds;
        //}

        ////Validate OTP While forget password
        //public DataSet Get(string command, string mobile_number, string OTP)
        //{
        //    DataSet ds = record.ForgetPassOTPValidate(command, mobile_number, OTP);
        //    return ds;
        //}
    }
}