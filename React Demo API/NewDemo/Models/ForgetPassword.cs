using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewDemo.Models
{
    public class ForgetPassword
    {
        public int UserID { get; set; }
        public string mobile_no { get; set; }
        public string OTP { get; set; }
        public string Password { get; set; }
        public int intuserId { get; set; }
        public int intusertypeId { get; set; }
        public string oldpassword { get; set; }
    }
}