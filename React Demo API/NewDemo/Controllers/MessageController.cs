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
    public class MessageController : ApiController
    {
        Database.DB record = new Database.DB();
        public DataSet Get(string command)
        {
            Message message = new Message();
            message.command = command;
            DataSet ds = record.SendMessage(message);
            return ds;
        }

        public DataSet GetList(string command, string intSchool_id, string intUserType_id)
        {
            Message message = new Message();
            message.command = command;
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            message.intUserType_id = intUserType_id;
            DataSet ds = record.SendMessage(message);
            return ds;
        }

        //public DataSet IsRead(string command, string intUserType_id,string intUser_id)
        //{
        //    Message message = new Message();
        //    message.command = command;
        //    message.intUserType_id = intUserType_id;
        //    message.intUser_id = intUser_id;
        //    DataSet ds = record.SendMessage(message);
        //    return ds;
        //}
        [HttpPost]
        public DataSet Delete(string command, string Msg_id)
        {
            Message message = new Message();
            message.command = command;
            message.Msg_id = Msg_id;
            DataSet ds = record.DeleteMessage(message);
            return ds;
        }
        public DataSet Get(string command, string Msg_id, int PageIndex, string UserName)
        {
            Message message = new Message();
            message.command = command;
            message.Msg_id =Convert.ToString(Msg_id);
            message.PageIndex =Convert.ToString(PageIndex);
            message.UserName = UserName;
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, string Msg_id, string id)
        {
            Message message = new Message();
            message.command = command;
            message.Msg_id = Msg_id;
            message.id = Convert.ToInt32(id);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, int intUser_id, string intUserType_id)
        {
            Message message = new Message();
            message.command = command;
            message.intUser_id = Convert.ToString(intUser_id);
            message.intUserType_id = Convert.ToString(intUserType_id);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command,  string intDepartment_id,int intSchool_id)
        {
            Message message = new Message();
            message.command = command;
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            message.intDepartment_id = Convert.ToString(intDepartment_id);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, int intSchool_id)
        {
            Message message = new Message();
            message.command = command;
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, int intSchool_id,int intStandard_id,string intAcademic_id)
        {
            Message message = new Message();
            message.command = command;
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            message.intStandard_id =Convert.ToString(intStandard_id);
            message.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, int intSchool_id, int intStandard_id,string intDivision_id,string intAcademic_id)
        {
            Message message = new Message();
            message.command = command;
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            message.intStandard_id =Convert.ToString(intStandard_id);
            message.intDivision_id = Convert.ToString(intDivision_id);
            message.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, string intSchool_id, string intAcademic_id, string Status)
        {
            Message message = new Message();
            message.command = command;
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            message.intAcademic_id = Convert.ToInt32(intAcademic_id);
            message.Status = Status;
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, string intSchool_id, string intAcademic_id, string Status, string PageIndex, string usertype_id)
        {
            Message message = new Message();
            message.command = command;
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            message.intAcademic_id = Convert.ToInt32(intAcademic_id);
            message.Status = Status;
            message.Status = Status;
            if (usertype_id == "All")
                usertype_id = "0,1,2,3,4,5";
            message.intUserType_id = Convert.ToString(usertype_id);
            message.PageIndex = Convert.ToString(PageIndex);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        public DataSet Get(string command, int Msg_id, string intSchool_id, string intAcademic_id)
        {
            Message message = new Message();
            message.command = command;
            message.Msg_id = Convert.ToString(Msg_id);
            message.intSchool_id = Convert.ToInt32(intSchool_id);
            message.intAcademic_id = Convert.ToInt32(intAcademic_id);
            DataSet ds = record.SendMessage(message);
            return ds;
        }
        [HttpPost]
        public DataSet Getmy([FromBody]Message message)
        {
            DataSet ds = record.SendMessageTest(message);
            return ds;
        }
    }
}
