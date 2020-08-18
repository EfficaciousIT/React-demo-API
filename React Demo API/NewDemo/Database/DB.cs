using NewDemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace NewDemo.Database
{
    public class DB
    {
        string constr = System.Configuration.ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;
        string SqlString = "";


        string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

        public DataSet OTPValidate(string command, OTPValidate OTPvalidate)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_ValidateMobileNo";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", command);
                    com.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                    com.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "OTPValidate");


                    //Check how many times OTP send
                    SqlCommand com10 = new SqlCommand("usp_forgetpassword", con);
                    com10.CommandType = CommandType.StoredProcedure;
                    com10.Parameters.AddWithValue("@command", "CountSendOTP");
                    com10.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                    com10.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    SqlDataAdapter da5 = new SqlDataAdapter(com10);
                    DataSet ds5 = new DataSet();
                    da5.Fill(ds5, "CountSendOTP");

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds5.Tables[0].Rows.Count < 2)
                        {
                            String query1 = "select intvalid_flg as intvalid_flg  from tblOTPValidate where intUser_id='" + OTPvalidate.intuser_id + "' and intUserType_id=1  and intvalid_flg=1  order by dtSendOTP desc";
                            SqlCommand com5 = new SqlCommand(query, con);
                            int rec = com5.ExecuteNonQuery();
                            if (rec <= 0)
                            {
                                string sRandomOTP = GenerateRandomOTP(4, saAllowedCharacters);
                                // Past here SMS link
                                string Message = "Your OTP Number is " + sRandomOTP;
                                //string Uname = "Arohan School";
                                //string URL = "http://alerts.justnsms.com/api/web2sms.php?workingkey=A1f82622e8e28d6c8e63ebc1543439e25&sender=AROHAN&to=" + OTPvalidate.mobile_number + "&message=" + Message + "&format=json&custom=1,2&flash=0&unicode=1";
                               // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + OTPvalidate.mobile_number + "&msgtype=UNI&message=" + Message + "&response=Y";
                                string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + OTPvalidate.mobile_number + "&msgtype=TXT&message=" + Message + "&response=Y";

                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                StreamReader sr = new StreamReader(resp.GetResponseStream());
                                string results = sr.ReadToEnd();

                                SqlCommand com1 = new SqlCommand("usp_ValidateMobileNo", con);
                                com1.CommandType = CommandType.StoredProcedure;
                                com1.Parameters.AddWithValue("@command", "InsertOTPRecord");
                                com1.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                                com1.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                                com1.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                                com1.Parameters.AddWithValue("@OTP", sRandomOTP);
                                com1.Parameters.AddWithValue("@intvalid_flg", 0);

                                int i = com1.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        if (ds5.Tables[0].Rows.Count < 2)
                        {
                            SqlCommand com1 = new SqlCommand("usp_ValidateMobileNo", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "InsertOTPRecord");
                            com1.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                            com1.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                            com1.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                            com1.Parameters.AddWithValue("@intvalid_flg", 3);

                            int i = com1.ExecuteNonQuery();
                        }
                    }

                    SqlCommand com2 = new SqlCommand("usp_ValidateMobileNo", con);
                    com2.CommandType = CommandType.StoredProcedure;
                    com2.Parameters.AddWithValue("@command", "ReturnValue");
                    com2.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com2.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                    com2.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);

                    SqlDataAdapter da1 = new SqlDataAdapter(com2);
                    DataSet dsObj = new DataSet();
                    da1.Fill(dsObj, "Status");

                    if (ds5.Tables[0].Rows.Count > 2)
                    {
                        dsObj.Tables["Status"].Rows.Clear();
                        DataRow dr = dsObj.Tables["Status"].NewRow(); //Create New Row
                        dr["intvalid_flg"] = "4";   //4 means number OTP Limit Reached              // Set Column Value
                        dsObj.Tables["Status"].Rows.InsertAt(dr, 0); // InsertAt specified position
                    }

                    return (dsObj);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        //public  DataSet AddNoticeboard(string command, Noticeboard noticeboard)
        //{
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        try
        //        {
        //            String query = "usp_ValidateMobileNo";
        //            SqlCommand com = new SqlCommand(query, con);
        //            con.Open();
        //            com.CommandType = CommandType.StoredProcedure;
        //            SqlDataAdapter da = new SqlDataAdapter(com);
        //            DataSet ds = new DataSet();
        //            da.Fill(ds, "AddNoticeboard");
        //            return (ds);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            con.Close();
        //            con.Dispose();
        //        }
        //    }
        //}

        //public DataSet AttendanceDetail(string command, Attendance attendance)
        //{
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        try
        //        {
        //            String query = "";

        //            if (command == "fillRole")
        //            {
        //                query = "usp_usermaster";
        //            }
        //            else if (attendance.intRole_id == 1 || command == "SelectStudent")
        //            {
        //                query = "usp_mob_Attendance";
        //            }

        //            else if (command == "selectAllteachr" || command == "selectAllStaff")
        //            {
        //                query = "usp_TeacherTransaction";
        //            }
        //            SqlCommand com = new SqlCommand(query, con);
        //            con.Open();
        //            com.CommandType = CommandType.StoredProcedure;

        //            if (command == "fillRole")
        //            {
        //                com.Parameters.AddWithValue("@command", command);
        //                com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
        //            }

        //            else if (command == "selectAllteachr" || command == "selectAllStaff")
        //            {
        //                com.Parameters.AddWithValue("@type", command);
        //                com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
        //            }

        //            else if (attendance.intRole_id == 1)
        //            {
        //                if (command == "SelectStudent")
        //                {
        //                    com.Parameters.AddWithValue("@command", command);
        //                    com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
        //                    com.Parameters.AddWithValue("@intDivision_id", attendance.intDivision_id);
        //                    com.Parameters.AddWithValue("@intAcademic_id", attendance.intAcademic_id);
        //                }
        //                else
        //                {
        //                    com.Parameters.AddWithValue("@command", command);
        //                    com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
        //                }

        //            }



        //            SqlDataAdapter da = new SqlDataAdapter(com);
        //            DataSet ds = new DataSet();
        //            da.Fill(ds, "AttendanceDetail");
        //            return (ds);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            con.Close();
        //            con.Dispose();
        //        }
        //    }
        //}

        public DataSet GallerDetails(string command, Gallery gallery)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_gallery";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    if (command == "Select")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intAcademic_id", gallery.intAcademic_id);
                    }
                    else if (command == "SelectGallery")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intAcademic_id", gallery.intAcademic_id);
                        com.Parameters.AddWithValue("@intSchool_id", gallery.intSchool_id);
                        com.Parameters.AddWithValue("@GalleryId", gallery.intGallery_id);
                    }
                    else if(command == "AddAlbum")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intAcademic_id", gallery.intAcademic_id);
                        com.Parameters.AddWithValue("@intSchool_id", gallery.intSchool_id);
                        com.Parameters.AddWithValue("@EventName", gallery.EventName);
                        com.Parameters.AddWithValue("@intInserted_by",gallery.intInserted_by);
                        com.Parameters.AddWithValue("@InseretIP", gallery.InsertedIP);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "GalleryDetails");
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet GalleryDetail(string extension, string EventDescription, string Folder_id)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_gallery";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "Insert");
                    com.Parameters.AddWithValue("@FileName", extension);
                    com.Parameters.AddWithValue("@EventDescription", EventDescription);
                    com.Parameters.AddWithValue("@Uploadedfrom", "Mobile");
                    com.Parameters.AddWithValue("@Filetype", "Gallery");
                    com.Parameters.AddWithValue("@GalleryId", Folder_id);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "GalleryDetail");
                    return (ds);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet Gallerydelete(string id, string filename)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_gallery";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "Delete");
                    com.Parameters.AddWithValue("@id", id);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "GalleryDetail");
                    var filePath = HttpContext.Current.Server.MapPath("~/image/" + filename);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        public DataSet SendMessage(Message message)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    DataSet ds = new DataSet();
                    //if (message.command == "insertMessage")
                    //{
                    //    string Mobile_Number = "";
                    //    string msg = "";
                    //    Mobile_Number = message.Mobile_number;
                    //    msg = message.message;

                    //    //string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=TXT&message=" + msg + "&response=Y";
                    //    string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                    //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                    //    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    //    StreamReader sr = new StreamReader(resp.GetResponseStream());
                    //    string results = sr.ReadToEnd();

                    //    String query = "usp_Message";
                    //    SqlCommand com = new SqlCommand(query, con);
                    //    con.Open();
                    //    com.CommandType = CommandType.StoredProcedure;
                    //    com.Parameters.AddWithValue("@command", message.command);
                    //    com.Parameters.AddWithValue("@intUserType_id", message.intUserType_id);
                    //    com.Parameters.AddWithValue("@intStandard_id", message.intStandard_id);
                    //    com.Parameters.AddWithValue("@intDivision_id", message.intDivision_id);
                    //    com.Parameters.AddWithValue("@intDepartment_id", message.intDepartment_id);
                    //    com.Parameters.AddWithValue("@intUser_id", message.intUser_id);
                    //    com.Parameters.AddWithValue("@MobileNo", message.Mobile_number);
                    //    com.Parameters.AddWithValue("@vchMessage", message.message);
                    //    com.Parameters.AddWithValue("@vchmessagecount", message.messagecount);
                    //    com.Parameters.AddWithValue("@intInserted_by", message.insertedby);
                    //    com.Parameters.AddWithValue("@InsertIP", message.IP);
                    //    com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                    //    com.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                    //    com.Parameters.AddWithValue("@Result", results);

                    //    int i = com.ExecuteNonQuery();

                    //    String query1 = "usp_Message";
                    //    SqlCommand com1 = new SqlCommand(query1, con);
                    //    com1.CommandType = CommandType.StoredProcedure;
                    //    com1.Parameters.AddWithValue("@command", "ReturnValue");
                    //    com1.Parameters.AddWithValue("@Result", results);
                    //    SqlDataAdapter da = new SqlDataAdapter(com1);
                    //    da.Fill(ds, "SendMessages");
                    //    // return (ds);
                    //}

                    if (message.command == "MessagesDetails")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@msg_id", message.Msg_id);
                        com.Parameters.AddWithValue("@PageIndex", message.PageIndex);
                        com.Parameters.AddWithValue("@User_name", message.UserName);
                        com.Parameters.AddWithValue("@PageSize", "10");
                        com.Parameters.AddWithValue("@RecordCount", "");
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds);
                        ds.Tables[0].TableName = "Message";
                        ds.Tables[1].TableName = "SendMessages";
                        //return (ds);
                    }
                    if (message.command == "UserWiseMessages")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intUser_id", message.intUser_id);
                        com.Parameters.AddWithValue("@intUserType_id", message.intUserType_id);
                        //com.Parameters.AddWithValue("@PageIndex", message.PageIndex);
                        //com.Parameters.AddWithValue("@PageSize", "10");
                        //com.Parameters.AddWithValue("@RecordCount", "");
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds, "SendMessages");
                        //return (ds);
                    }
                    else if (message.command == "MessageDetails")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@vchMessage", message.message);
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds, "SendMessages");
                    }

                    else if (message.command == "ListOfRoles")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds, "SendMessages");
                    }
                    else if (message.command == "ListOfStandards")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds, "SendMessages");
                    }
                    else if (message.command == "ListOfDivisions")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intStandard_id",message.intStandard_id);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds, "SendMessages");
                    }
                    else if (message.command == "ListOfStudentsDivisionWise")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intStandard_id", message.intStandard_id);
                        com.Parameters.AddWithValue("@intDivision_id", message.intDivision_id);
                        com.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);

                        da.Fill(ds, "SendMessages");
                    }
                    else if (message.command == "ListOfTeachers")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        com.Parameters.AddWithValue("@intDepartment_id", message.intDepartment_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "ListOfTeachers");
                    }
                    else if (message.command == "ListOfStaff")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        com.Parameters.AddWithValue("@intDepartment_id", message.intDepartment_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "ListOfStaff");
                    }
                   else if (message.command == "ListOfDepartments")
                    {

                        String queryList = "usp_Message";
                        SqlCommand com = new SqlCommand(queryList, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        com.Parameters.AddWithValue("@intUserType_id", message.intUserType_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "Departments");
                    }
                    else if (message.command == "ReadMsg")
                    {
                        String queryList = "usp_Message";
                        SqlCommand com = new SqlCommand(queryList, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intUser_id", message.intUser_id);
                        com.Parameters.AddWithValue("@intUserType_id", message.intUserType_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "IsReadMsg");
                    }
                    else if (message.command == "ListOfMessages")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                        com.Parameters.AddWithValue("@Status", message.Status);
                        com.Parameters.AddWithValue("@intUserType_id", message.intUserType_id);
                        com.Parameters.AddWithValue("@PageIndex", message.PageIndex);
                        com.Parameters.AddWithValue("@PageSize", "10");
                        com.Parameters.AddWithValue("@RecordCount", "");
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "SendMessages");
                    }
                    else if (message.command == "EditMessage")
                    {
                        String query = "usp_Message";
                        SqlCommand com = new SqlCommand(query, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                        com.Parameters.AddWithValue("@Msg_id", message.Msg_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "SendMessages");
                    }
                    int resendrows = 0;
                    if (message.command == "Resend" || message.command == "ResendAll")
                    {
                        String queryList = "usp_Message";
                        SqlCommand com8 = new SqlCommand(queryList, con);
                        con.Open();
                        com8.CommandType = CommandType.StoredProcedure;
                        com8.Parameters.AddWithValue("@command", message.command);
                        com8.Parameters.AddWithValue("@msg_id", message.Msg_id);
                        if (message.command == "Resend")
                        {
                            com8.Parameters.AddWithValue("@id", message.id);
                        }
                        SqlDataAdapter da8 = new SqlDataAdapter(com8);
                        DataSet dsObj = new DataSet();
                        da8.Fill(dsObj, "ResendMessageDetails");
                        for (int k = 0; dsObj.Tables["ResendMessageDetails"].Rows.Count > k; k++)
                        {
                            string Mobile_Number = Convert.ToString(dsObj.Tables["ResendMessageDetails"].Rows[k]["MobileNumber"]);
                            string msg = "";
                            msg = Convert.ToString(dsObj.Tables["ResendMessageDetails"].Rows[k]["vchMessage"]);
                           // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                            string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=TXT&message=" + msg + "&response=Y";

                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                            StreamReader sr = new StreamReader(resp.GetResponseStream());
                            string results = sr.ReadToEnd();
                           // string results = "0";
                            String query = "usp_Message";
                            SqlCommand com4 = new SqlCommand(query, con);
                            com4.CommandType = CommandType.StoredProcedure;
                            com4.Parameters.AddWithValue("@command", "Update_Details");
                            com4.Parameters.AddWithValue("@Result", results);
                            com4.Parameters.AddWithValue("@id", dsObj.Tables["ResendMessageDetails"].Rows[k]["id"]);
                            int i = com4.ExecuteNonQuery();
                            ds.Clear();
                            if (i == 1)
                                resendrows += 1;
                        }
                        if (Convert.ToInt32(dsObj.Tables["ResendMessageDetails1"].Rows[0]["count"]) == resendrows)
                        {
                            String queryList1 = "usp_Message";
                            SqlCommand com9 = new SqlCommand(queryList1, con);
                            com9.CommandType = CommandType.StoredProcedure;
                            com9.Parameters.AddWithValue("@command", "UpdateMessageMaster");
                            com9.Parameters.AddWithValue("@msg_id", message.Msg_id);
                            SqlDataAdapter da9 = new SqlDataAdapter(com9);
                            DataSet dsObj1 = new DataSet();
                            da9.Fill(dsObj1);
                        }
                        DataTable ds1 = ds.Tables.Add("SendMessages");
                        ds1.Columns.Add("Result", typeof(Int32));
                        DataRow dr = ds.Tables["SendMessages"].NewRow();
                        dr["Result"] = resendrows;
                        ds.Tables["SendMessages"].Rows.InsertAt(dr, 0);
                    }
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet DeleteMessage(Message message)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    DataSet ds = new DataSet();
                    if (message.command == "DeleteMessage")
                    {

                        String queryList = "usp_Message";
                        SqlCommand com = new SqlCommand(queryList, con);
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@command", message.command);
                        com.Parameters.AddWithValue("@msg_id", message.Msg_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "SendMessages");
                    }
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        //public DataSet ForgetPassOTPValidate(string command, string mobile_number, string OTP)
        //{
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        try
        //        {
        //            String query = "usp_forgetpassword";
        //            SqlCommand com = new SqlCommand(query, con);
        //            con.Open();
        //            com.CommandType = CommandType.StoredProcedure;
        //            com.Parameters.AddWithValue("@command", "RecentOTP");
        //            com.Parameters.AddWithValue("@mobile_number", mobile_number);
        //            SqlDataAdapter da = new SqlDataAdapter(com);
        //            DataSet ds = new DataSet();
        //            da.Fill(ds, "OTPValidate");
        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                string ReceivedOTP = Convert.ToString(ds.Tables[0].Rows[0]["OTP"]);

        //                if (OTP == ReceivedOTP)
        //                {
        //                    SqlCommand com1 = new SqlCommand(query, con);
        //                    com1.CommandType = CommandType.StoredProcedure;
        //                    com1.Parameters.AddWithValue("@command", "UpdateValidFlg");
        //                    com1.Parameters.AddWithValue("@mobile_number", mobile_number);
        //                    com1.Parameters.AddWithValue("@OTP", OTP);
        //                    com1.Parameters.AddWithValue("@intvalid_flg", 1);

        //                    int i = com1.ExecuteNonQuery();

        //                }
        //                else
        //                {
        //                    SqlCommand com1 = new SqlCommand(query, con);
        //                    com1.CommandType = CommandType.StoredProcedure;
        //                    com1.Parameters.AddWithValue("@command", "UpdateNotValidFlg");
        //                    com1.Parameters.AddWithValue("@mobile_number", mobile_number);
        //                    com1.Parameters.AddWithValue("@intvalid_flg", 2);

        //                    int i = com1.ExecuteNonQuery();

        //                }
        //            }


        //            SqlCommand com2 = new SqlCommand("usp_ValidateMobileNo", con);
        //            com2.CommandType = CommandType.StoredProcedure;
        //            com2.Parameters.AddWithValue("@command", "OTPReturnValue");
        //            com2.Parameters.AddWithValue("@mobile_number", mobile_number);
        //            //com2.Parameters.AddWithValue("@OTP", OTPvalidate.OTP);

        //            SqlDataAdapter da1 = new SqlDataAdapter(com2);
        //            DataSet dsObj = new DataSet();
        //            da1.Fill(dsObj, "Status");

        //            return (dsObj);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            con.Close();
        //            con.Dispose();
        //        }
        //    }
        //}
        //public DataSet ForgetPassword(string command, string mobile_number)
        //{
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        try
        //        {
        //            string intUserType_id = "";
        //            string intUser_id = "";
        //            String query = "usp_forgetpassword";
        //            SqlCommand com = new SqlCommand(query, con);
        //            con.Open();
        //            com.CommandType = CommandType.StoredProcedure;
        //            com.Parameters.AddWithValue("@command", "IsNumberRegistered");
        //            com.Parameters.AddWithValue("@mobile_number", mobile_number);
        //            SqlDataAdapter da = new SqlDataAdapter(com);
        //            DataSet ds = new DataSet();
        //            da.Fill(ds, "OTPValidate");
        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                intUserType_id = Convert.ToString(ds.Tables[0].Rows[0]["intUserType_id"]);
        //                intUser_id = Convert.ToString(ds.Tables[0].Rows[0]["intUser_id"]);

        //                string sRandomOTP = GenerateRandomOTP(4, saAllowedCharacters);
        //                // Past here SMS link

        //                string Message = "Your OTP Number is " + sRandomOTP;
        //                // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=GurukulAratiG&pass=_nwGE1$5&senderid=GURUKL&dest_mobileno=" + OTPvalidate.mobile_number + "&msgtype=UNI&message=" + Message + "&response=Y";
        //                string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + mobile_number + "&msgtype=TXT&message=" + Message + "&response=Y";

        //                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
        //                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        //                StreamReader sr = new StreamReader(resp.GetResponseStream());
        //                string results = sr.ReadToEnd();

        //                SqlCommand com1 = new SqlCommand("usp_forgetpassword", con);
        //                com1.CommandType = CommandType.StoredProcedure;
        //                com1.Parameters.AddWithValue("@command", "InsertOTP");
        //                com1.Parameters.AddWithValue("@intUserType_id", intUserType_id);
        //                com1.Parameters.AddWithValue("@mobile_number", mobile_number);
        //                com1.Parameters.AddWithValue("@intUser_id", intUser_id);
        //                com1.Parameters.AddWithValue("@OTP", sRandomOTP);
        //                com1.Parameters.AddWithValue("@intvalid_flg", 0);

        //                int i = com1.ExecuteNonQuery();
        //            }

        //            SqlCommand com2 = new SqlCommand("usp_forgetpassword", con);
        //            com2.CommandType = CommandType.StoredProcedure;
        //            com2.Parameters.AddWithValue("@command", "ReturnValue");
        //            com2.Parameters.AddWithValue("@mobile_number", mobile_number);

        //            SqlDataAdapter da1 = new SqlDataAdapter(com2);
        //            DataSet dsObj = new DataSet();
        //            da1.Fill(dsObj, "Status");

        //            return (dsObj);

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            con.Close();
        //            con.Dispose();
        //        }
        //    }
        //}


        public DataSet ForgetPassOTPValidate(string command, ForgetPassword forgetPassword)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_forgetpassword";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();

                    //For Getting UserId and UserTypeID
                    //Getting UserID and UserType_id
                    string usertype = Convert.ToString(forgetPassword.UserID).Substring(0, 1);
                    string intUserType_id = Convert.ToString(usertype);
                    string user_id = Convert.ToString(forgetPassword.UserID).Substring(5, 4);
                    string intUser_id = Convert.ToString(user_id);
                    if (Convert.ToString(intUserType_id) == "1")
                    {
                        command = "student";
                    }
                    else if (Convert.ToString(intUserType_id) == "3")
                    {
                        command = "teacher";
                    }
                    else if (Convert.ToString(intUserType_id) == "4")
                    {
                        command = "staff";
                    }
                    else if (Convert.ToString(intUserType_id) == "5")
                    {
                        command = "admin";
                    }

                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "RecentOTP");
                    com.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    com.Parameters.AddWithValue("@intUser_id", intUser_id);
                    com.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "OTPValidate");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string ReceivedOTP = Convert.ToString(ds.Tables[0].Rows[0]["OTP"]);

                        if (Convert.ToString(forgetPassword.OTP) == ReceivedOTP)
                        {
                            SqlCommand com1 = new SqlCommand(query, con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "UpdateValidFlg");
                            com1.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                            com1.Parameters.AddWithValue("@OTP", forgetPassword.OTP);
                            com1.Parameters.AddWithValue("@intvalid_flg", 1);

                            int i = com1.ExecuteNonQuery();

                        }
                        else
                        {
                            SqlCommand com1 = new SqlCommand(query, con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "UpdateNotValidFlg");
                            com1.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                            com1.Parameters.AddWithValue("@intvalid_flg", 2);

                            int i = com1.ExecuteNonQuery();

                        }
                    }


                    SqlCommand com2 = new SqlCommand("usp_forgetpassword", con);
                    com2.CommandType = CommandType.StoredProcedure;
                    com2.Parameters.AddWithValue("@command", "ReturnValue");
                    com2.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    com2.Parameters.AddWithValue("@intUser_id", intUser_id);
                    com2.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                    //com2.Parameters.AddWithValue("@OTP", OTPvalidate.OTP);

                    SqlDataAdapter da1 = new SqlDataAdapter(com2);
                    DataSet dsObj = new DataSet();
                    da1.Fill(dsObj, "Status");

                    return (dsObj);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet ForgetPassword(string command, ForgetPassword forgetPassword)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    string intUserType_id = "";
                    string intUser_id = "";

                    //Getting UserID and UserType_id
                    string usertype = Convert.ToString(forgetPassword.UserID).Substring(0, 1);
                    intUserType_id = Convert.ToString(usertype);
                    string user_id = Convert.ToString(forgetPassword.UserID).Substring(5, 4);
                    intUser_id = Convert.ToString(user_id);
                    if (Convert.ToString(intUserType_id) == "1")
                    {
                        command = "student";
                    }
                    else if (Convert.ToString(intUserType_id) == "3")
                    {
                        command = "teacher";
                    }
                    else if (Convert.ToString(intUserType_id) == "4")
                    {
                        command = "staff";
                    }
                    else if (Convert.ToString(intUserType_id) == "5")
                    {
                        command = "admin";
                    }


                    String query = "usp_forgetpassword";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "IsNumberRegistered");
                    com.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    com.Parameters.AddWithValue("@intUser_id", intUser_id);
                    com.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "OTPValidate");


                    //Check how many times OTP send
                    SqlCommand com5 = new SqlCommand("usp_forgetpassword", con);
                    com5.CommandType = CommandType.StoredProcedure;
                    com5.Parameters.AddWithValue("@command", "CountSendOTP");
                    com5.Parameters.AddWithValue("@intUser_id", intUser_id);
                    com5.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    SqlDataAdapter da5 = new SqlDataAdapter(com5);
                    DataSet ds5 = new DataSet();
                    da5.Fill(ds5, "CountSendOTP");



                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        intUserType_id = Convert.ToString(ds.Tables[0].Rows[0]["intUserType_id"]);
                        intUser_id = Convert.ToString(ds.Tables[0].Rows[0]["intUser_id"]);

                        if (ds5.Tables[0].Rows.Count < 2)
                        {

                            string sRandomOTP = GenerateRandomOTP(4, saAllowedCharacters);
                            // Past here SMS link

                            string Message = "Your OTP Number is " + sRandomOTP;
                            // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=GurukulAratiG&pass=_nwGE1$5&senderid=GURUKL&dest_mobileno=" + OTPvalidate.mobile_number + "&msgtype=UNI&message=" + Message + "&response=Y";
                            // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + forgetPassword.mobile_no + "&msgtype=TXT&message=" + Message + "&response=Y";
                           // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + forgetPassword.mobile_no + "&msgtype=UNI&message=" + Message + "&response=Y";
                            string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + forgetPassword.mobile_no + "&msgtype=TXT&message=" + Message + "&response=Y";

                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                            StreamReader sr = new StreamReader(resp.GetResponseStream());
                            string results = sr.ReadToEnd();

                            SqlCommand com1 = new SqlCommand("usp_forgetpassword", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "InsertOTP");
                            com1.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                            com1.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                            com1.Parameters.AddWithValue("@intUser_id", intUser_id);
                            com1.Parameters.AddWithValue("@OTP", sRandomOTP);
                            com1.Parameters.AddWithValue("@intvalid_flg", 0);

                            int i = com1.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        if (ds5.Tables[0].Rows.Count < 2)
                        {

                            //Check Number is exists or not in our master 
                            SqlCommand com1 = new SqlCommand(query, con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "CheckNumberExistsORNot");
                            com1.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                            com1.Parameters.AddWithValue("@intUser_id", intUser_id);
                            com1.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                            SqlDataAdapter da2 = new SqlDataAdapter(com1);
                            DataSet ds1 = new DataSet();
                            da2.Fill(ds1, "OTPValidate");

                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                //intUserType_id = Convert.ToString(ds.Tables[0].Rows[0]["intUserType_id"]);
                                //intUser_id = Convert.ToString(ds.Tables[0].Rows[0]["intUser_id"]);

                                string sRandomOTP = GenerateRandomOTP(4, saAllowedCharacters);
                                // Past here SMS link

                                string Message = "Your OTP Number is " + sRandomOTP;
                                // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=GurukulAratiG&pass=_nwGE1$5&senderid=GURUKL&dest_mobileno=" + OTPvalidate.mobile_number + "&msgtype=UNI&message=" + Message + "&response=Y";
                                 string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + forgetPassword.mobile_no + "&msgtype=TXT&message=" + Message + "&response=Y";
                                //string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + forgetPassword.mobile_no + "&msgtype=UNI&message=" + Message + "&response=Y";

                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                StreamReader sr = new StreamReader(resp.GetResponseStream());
                                string results = sr.ReadToEnd();

                                SqlCommand com3 = new SqlCommand("usp_forgetpassword", con);
                                com3.CommandType = CommandType.StoredProcedure;
                                com3.Parameters.AddWithValue("@command", "InsertOTP");
                                com3.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                                com3.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);
                                com3.Parameters.AddWithValue("@intUser_id", intUser_id);
                                com3.Parameters.AddWithValue("@OTP", sRandomOTP);
                                com3.Parameters.AddWithValue("@intvalid_flg", 0);

                                int i = com3.ExecuteNonQuery();
                            }
                        }

                    }
                    SqlCommand com2 = new SqlCommand("usp_forgetpassword", con);
                    com2.CommandType = CommandType.StoredProcedure;
                    com2.Parameters.AddWithValue("@command", "ReturnValue");
                    com2.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    com2.Parameters.AddWithValue("@intUser_id", intUser_id);
                    com2.Parameters.AddWithValue("@mobile_number", forgetPassword.mobile_no);

                    SqlDataAdapter da1 = new SqlDataAdapter(com2);
                    DataSet dsObj = new DataSet();
                    da1.Fill(dsObj, "Status");

                    if (ds5.Tables[0].Rows.Count >= 2)
                    {
                        dsObj.Tables["Status"].Rows.Clear();
                        DataRow dr = dsObj.Tables["Status"].NewRow(); //Create New Row
                        dr["intvalid_flg"] = "4";   //4 means OTP Limit Reached              // Set Column Value
                        dsObj.Tables["Status"].Rows.InsertAt(dr, 0); // InsertAt specified position
                    }

                    //if entered mobile number is not registerd against enterd user id
                    else if(dsObj.Tables["Status"].Rows.Count == 0)
                    {
                        DataRow dr = dsObj.Tables["Status"].NewRow(); //Create New Row
                        dr["intvalid_flg"] = "3";   //3 means number is not registred            // Set Column Value
                        dsObj.Tables["Status"].Rows.InsertAt(dr, 0); // InsertAt specified position
                    }

                    return (dsObj);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet ValidateUserID(string command, string UserID)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_forgetpassword";
                    SqlCommand com = new SqlCommand(query, con);
                    string usertype = UserID.Substring(0, 1);
                    string intUserType_id = Convert.ToString(usertype);
                    string user_id = UserID.Substring(5, 4);
                    string intUser_id = Convert.ToString(user_id);
                    if (Convert.ToString(intUserType_id) == "1")
                    {
                        command = "student";
                    }
                    else if (Convert.ToString(intUserType_id) == "3")
                    {
                        command = "teacher";
                    }
                    else if (Convert.ToString(intUserType_id) == "4")
                    {
                        command = "staff";
                    }
                    else if (Convert.ToString(intUserType_id) == "5")
                    {
                        command = "admin";
                    }
                    con.Open();

                    SqlCommand com1 = new SqlCommand("usp_forgetpassword", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    com1.Parameters.AddWithValue("@command", "CheckUserID");
                    com1.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    com1.Parameters.AddWithValue("@vchUser_name", UserID);
                    SqlDataAdapter da1 = new SqlDataAdapter(com1);
                    DataSet ds = new DataSet();
                    da1.Fill(ds, "UserName");
                    //if entered use name is incorrect
                    if (ds.Tables["UserName"].Rows.Count == 0)
                    {
                        DataRow dr = ds.Tables["UserName"].NewRow(); //Create New Row
                        dr["UserName"] = "False";             // Set Column Value
                        ds.Tables["UserName"].Rows.InsertAt(dr, 0); // InsertAt specified position
                    }

                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "ValidUserId");
                    com.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    com.Parameters.AddWithValue("@intUser_id", intUser_id);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    //DataSet ds = new DataSet();
                    da.Fill(ds, "ValidUserID");

                    //if Mobile number is not verified
                    if (ds.Tables["ValidUserID"].Rows.Count == 0)
                    {
                        DataRow dr = ds.Tables["ValidUserID"].NewRow(); //Create New Row
                        dr["Mobile_number"] = "False";             // Set Column Value
                        ds.Tables["ValidUserID"].Rows.InsertAt(dr, 0); // InsertAt specified position
                    }

                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet ChangePassword(string command, ForgetPassword forgetPassword)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_forgetpassword";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    //Getting UserID and UserType_id
                    string usertype = Convert.ToString(forgetPassword.UserID).Substring(0, 1);
                    string intUserType_id = Convert.ToString(usertype);
                    string user_id = Convert.ToString(forgetPassword.UserID).Substring(5, 4);
                    string intUser_id = Convert.ToString(user_id);
                    if (Convert.ToString(intUserType_id) == "1")
                    {
                        command = "student";
                    }
                    else if (Convert.ToString(intUserType_id) == "3")
                    {
                        command = "teacher";
                    }
                    else if (Convert.ToString(intUserType_id) == "4")
                    {
                        command = "staff";
                    }
                    else if (Convert.ToString(intUserType_id) == "5")
                    {
                        command = "admin";
                    }
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "ChangePassword");
                    com.Parameters.AddWithValue("@intUserType_id", intUserType_id);
                    com.Parameters.AddWithValue("@intUser_id", intUser_id);
                    com.Parameters.AddWithValue("@vchUser_name", forgetPassword.UserID);
                    com.Parameters.AddWithValue("@vchPassword", forgetPassword.Password);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "PasswordChanged");
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public int UpdatePassword(string command, ForgetPassword forgetPassword)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_forgetpassword";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "UpdatePassword");
                    com.Parameters.AddWithValue("@intUserType_id", forgetPassword.intusertypeId);
                    com.Parameters.AddWithValue("@intUser_id", forgetPassword.intuserId);
                    com.Parameters.AddWithValue("@vchOldPassword", forgetPassword.oldpassword);
                    com.Parameters.AddWithValue("@vchPassword", forgetPassword.Password);
                    int count = Convert.ToInt32(com.ExecuteNonQuery());
                    //SqlDataAdapter da = new SqlDataAdapter(com);
                    //DataSet ds = new DataSet();
                    //da.Fill(ds, "PasswordChanged");
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        public DataSet AttendanceDetail(string command, Attendance attendance)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "";
                    query = "usp_Mob_attendance";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    if (command == "fillRole")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
                        com.Parameters.AddWithValue("@intUserType_id", attendance.intUserType_id);
                    }

                    else if (command == "GetList")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
                        com.Parameters.AddWithValue("@intUserType_id", attendance.intUserType_id);
                        com.Parameters.AddWithValue("@intRole_id", attendance.intRole_id);
                        if (attendance.intUserType_id == 3)
                        {
                            com.Parameters.AddWithValue("@intUser_id", attendance.intUser_id);
                        }
                    }
                    else if (command == "SelectStudent")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
                        com.Parameters.AddWithValue("@intDivision_id", attendance.intDivision_id);
                        com.Parameters.AddWithValue("@intAcademic_id", attendance.intAcademic_id);
                    }

                    else if (command == "GetAttendance")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
                        com.Parameters.AddWithValue("@intUserType_id", attendance.intUserType_id);
                        com.Parameters.AddWithValue("@intUser_id", attendance.intUser_id);
                        com.Parameters.AddWithValue("@intAcademic_id", attendance.intAcademic_id);
                    }
                    else if (command == "selectstudattendance")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intSchool_id", attendance.intSchool_id);
                        com.Parameters.AddWithValue("@intUserType_id", attendance.intUserType_id);
                        com.Parameters.AddWithValue("@intPerson_id", attendance.intPerson_id);
                        com.Parameters.AddWithValue("@intRole_id", attendance.intRole_id);
                        com.Parameters.AddWithValue("@intAcademic_id", attendance.intAcademic_id);
                    }
                    else if (command == "Insert")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intUserType_id", attendance.intRole_id);
                        com.Parameters.AddWithValue("@intUser_id", attendance.intPerson_id);
                        com.Parameters.AddWithValue("@dtDate", attendance.dtDate);
                        com.Parameters.AddWithValue("@status", attendance.status);
                        com.Parameters.AddWithValue("@intschool_id", attendance.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", attendance.intAcademic_id);
                        com.Parameters.AddWithValue("@intstanderd_id", attendance.intStandard_id);
                        com.Parameters.AddWithValue("@intdivision_id", attendance.intDivision_id);
                    }
                    else if (command == "InsertTeacher")
                    {
                        com.Parameters.AddWithValue("@command", command);
                        com.Parameters.AddWithValue("@intUserType_id", attendance.intRole_id);
                        com.Parameters.AddWithValue("@intUser_id", attendance.intPerson_id);
                        com.Parameters.AddWithValue("@dtDate", attendance.dtDate);
                        com.Parameters.AddWithValue("@status", attendance.status);
                        com.Parameters.AddWithValue("@intSchool_Id", attendance.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", attendance.intAcademic_id);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "AttendanceDetail");
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        public DataSet LoginDetails(string command, Login Login)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_Mob_usermaster";
                    SqlCommand com = new SqlCommand(query, con);
                    string usertype = Login.vchUser_name.Substring(0, 1); 
                    string intUserType_id =Convert.ToString(usertype);

                    string inuserid = Login.vchUser_name.Substring(5, 4);

                    //School Id
                    string schoolID = Login.vchUser_name.Substring(1, 2);
                    string intSchool_id = Convert.ToString(schoolID);

                    if (Convert.ToString(intUserType_id) == "1")
                    {
                        command = "student";
                    }
                    else if (Convert.ToString(intUserType_id) == "3")
                    {
                        command = "teacher";
                    }
                    else if (Convert.ToString(intUserType_id) == "4")
                    {
                        command = "staff";
                    }
                    else if (Convert.ToString(intUserType_id) == "5")
                    {
                        command = "admin";
                    }
                    string academicyear="";
                    SqlCommand com1 = new SqlCommand("select top 1 intAcademic_id from tblAcademicYear order by intAcademic_id  desc", con);
                    SqlDataReader dr1;
                    con.Open();
                    dr1 = com1.ExecuteReader();
                    while (dr1.Read())
                    {
                        academicyear = dr1[0].ToString();
                    }
                    con.Close();

                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", command);
                    com.Parameters.AddWithValue("@usertype", intUserType_id);
                    com.Parameters.AddWithValue("@username", Login.vchUser_name);
                    com.Parameters.AddWithValue("@intschool_id", intSchool_id);
                    com.Parameters.AddWithValue("@intAcademic_id", academicyear);
                    com.Parameters.AddWithValue("@password", Login.vchPassword);
                    com.Parameters.AddWithValue("@inuserid", inuserid);
                    com.Parameters.AddWithValue("@DeviceId", Login.deviceId);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "LoginDetails");

                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet JwtTokenLog(string command, Login Login)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_jwttoken";
                    SqlCommand com = new SqlCommand(query, con);
                    string usertype = Login.vchUser_name.Substring(0, 1);
                    string intUserType_id = Convert.ToString(usertype);

                    string inuserid = Login.vchUser_name.Substring(5, 4);

                    //School Id
                    string schoolID = Login.vchUser_name.Substring(1, 2);
                    string intSchool_id = Convert.ToString(schoolID);

                    string dtfromdate = Convert.ToDateTime(Login.jwtTokenIssueddt).ToString("MM/dd/yyyy");
                    Login.jwtTokenIssueddt = dtfromdate.Replace("-", "/");

                    string dtTodate = Convert.ToDateTime(Login.jwtTokenExpdt).ToString("MM/dd/yyyy");
                    Login.jwtTokenExpdt = dtTodate.Replace("-", "/");

                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", command);
                    com.Parameters.AddWithValue("@intUsertype_id", intUserType_id);
                    com.Parameters.AddWithValue("@intUser_id", inuserid);
                    com.Parameters.AddWithValue("@jwt_token", Login.jwtToken);
                    com.Parameters.AddWithValue("@dtCreate_date", Login.jwtTokenIssueddt);
                    com.Parameters.AddWithValue("@dtExpire_date", Login.jwtTokenExpdt);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "LoginDetails");

                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public bool LogOutFromAllDevices(string intUserType_id, string inuserid)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_jwttoken";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "LogOutFromAllDevices");
                    com.Parameters.AddWithValue("@intUsertype_id", intUserType_id);
                    com.Parameters.AddWithValue("@intUser_id", inuserid);
                    int count = 0;
                       count= Convert.ToInt32(com.ExecuteNonQuery());

                    bool Result = count == 0 ? false : true;

                    return Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet LogoutUser(string command, Login Login)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_jwttoken";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "LogoutUser");
                    com.Parameters.AddWithValue("@intUsertype_id", Login.intUserType_id);
                    com.Parameters.AddWithValue("@intUser_id", Login.intUser_id);
                    com.Parameters.AddWithValue("@jwt_token", Login.jwtToken);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "LogoutDetails");
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }


        public DataSet UserDetail(string command, Login Login)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_UserDetails";
                    SqlCommand com = new SqlCommand(query, con);
                    string usertype = Login.vchUser_name.Substring(0, 1);
                    string intUserType_id = Convert.ToString(usertype);

                    string inuserid = Login.vchUser_name.Substring(5, 4);

                    //School Id
                    string schoolID = Login.vchUser_name.Substring(1, 2);
                    string intSchool_id = Convert.ToString(schoolID);

                    if (Convert.ToString(intUserType_id) == "1")
                    {
                        command = "student";
                    }
                    else if (Convert.ToString(intUserType_id) == "3")
                    {
                        command = "teacher";
                    }
                    else if (Convert.ToString(intUserType_id) == "4")
                    {
                        command = "staff";
                    }
                    else if (Convert.ToString(intUserType_id) == "5")
                    {
                        command = "admin";
                    }
                    string academicyear = "";
                    SqlCommand com1 = new SqlCommand("select top 1 intAcademic_id from tblAcademicYear order by intAcademic_id  desc", con);
                    SqlDataReader dr1;
                    con.Open();
                    dr1 = com1.ExecuteReader();
                    while (dr1.Read())
                    {
                        academicyear = dr1[0].ToString();
                    }
                    con.Close();

                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", command);
                    com.Parameters.AddWithValue("@username", Login.vchUser_name);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "UserDetails");

                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet AddNoticeboard(string command, Noticeboard noticeboard)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    //
                    String query1 = "usp_Noticeboard";
                    SqlCommand com1 = new SqlCommand(query1, con);
                    com1.CommandType = CommandType.StoredProcedure;
                    DataSet ds1 = new DataSet();
                    string dtfromdate = Convert.ToDateTime(noticeboard.dtIssue_date).ToString("MM/dd/yyyy");
                    noticeboard.dtIssue_date = dtfromdate.Replace("-", "/");

                    string dtenddate = Convert.ToDateTime(noticeboard.dtEnd_date).ToString("MM/dd/yyyy");
                    noticeboard.dtEnd_date = dtenddate.Replace("-", "/");
                    com1.Parameters.AddWithValue("@type", "NewDemoInsertNoticeboard");
                    com1.Parameters.AddWithValue("@dtIssue_date", noticeboard.dtIssue_date);
                    com1.Parameters.AddWithValue("@dtEnd_date", noticeboard.dtEnd_date);
                    com1.Parameters.AddWithValue("@vchSubject", noticeboard.vchSubject);
                    com1.Parameters.AddWithValue("@vchNotice", noticeboard.vchNotice);
                    com1.Parameters.AddWithValue("@intSchool_id", noticeboard.intSchool_id);
                    com1.Parameters.AddWithValue("@intInserted_by", noticeboard.intInserted_by);
                    com1.Parameters.AddWithValue("@InsertIP", noticeboard.InsertIP);
                    com1.Parameters.AddWithValue("@intAcademic_id", noticeboard.intAcademic_id);
                    com1.Parameters.AddWithValue("@Image_Name", noticeboard.ImageName);
                    com1.Parameters.AddWithValue("@PDF_Name", noticeboard.PDFName);
                    com1.Parameters.AddWithValue("@vchURL",noticeboard.Link);
                    com1.Parameters.AddWithValue("@IsTeacherVisible", noticeboard.visibleForTeacher);
                    com1.Parameters.AddWithValue("@InsertedFrom","App");
                    //SqlDataAdapter da1 = new SqlDataAdapter(com1);
                    //da1.Fill(ds1, "AddNoticeboard");
                    con.Open();
                    com1.ExecuteReader();
                    con.Close();

                    String query = "usp_Noticeboard";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    DataSet ds = new DataSet();

                    if(noticeboard.intUserType_id == 1)
                    { 
                    string standards = noticeboard.intStandard_id;
                    string[] values = standards.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Trim();
                        com.Parameters.Clear();
                        com.Parameters.AddWithValue("@type", "NewDemoInsertNoticeboardDetails");
                        com.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                        com.Parameters.AddWithValue("@intStandard_id", values[i]);
                        com.Parameters.AddWithValue("@DIV", noticeboard.intDivision_id);
                        com.Parameters.AddWithValue("@intUser_id", noticeboard.intUser_id);
                        com.Parameters.AddWithValue("@intDepartment_id", noticeboard.intDepartment_id);

                        com.Parameters.AddWithValue("@intSchool_id", noticeboard.intSchool_id);
                        com.Parameters.AddWithValue("@intInserted_by", noticeboard.intInserted_by);
                        com.Parameters.AddWithValue("@InsertIP", noticeboard.InsertIP);
                        com.Parameters.AddWithValue("@intAcademic_id", noticeboard.intAcademic_id);

                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "AddNoticeboard");
                    }
                    }
                    else
                    { 
                    string departments = noticeboard.intDepartment_id;
                    string[] department = departments.Split(',');
                        for (int i = 0; i < department.Length; i++)
                        {
                            department[i] = department[i].Trim();
                            com.Parameters.Clear();
                            com.Parameters.AddWithValue("@type", "NewDemoInsertNoticeboardDetails");
                            com.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                            com.Parameters.AddWithValue("@intStandard_id", noticeboard.intStandard_id);
                            com.Parameters.AddWithValue("@DIV", noticeboard.intDivision_id);
                            com.Parameters.AddWithValue("@intUser_id", noticeboard.intUser_id);
                            com.Parameters.AddWithValue("@intDepartment_id", department[i]);

                            com.Parameters.AddWithValue("@intSchool_id", noticeboard.intSchool_id);
                            com.Parameters.AddWithValue("@intInserted_by", noticeboard.intInserted_by);
                            com.Parameters.AddWithValue("@InsertIP", noticeboard.InsertIP);
                            com.Parameters.AddWithValue("@intAcademic_id", noticeboard.intAcademic_id);

                            SqlDataAdapter da = new SqlDataAdapter(com);
                            da.Fill(ds, "AddNoticeboard");
                        }
                    }

                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet EditNoticeboard(string command, Noticeboard noticeboard)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query1 = "usp_Noticeboard";
                    SqlCommand com1 = new SqlCommand(query1, con);
                    com1.CommandType = CommandType.StoredProcedure;
                    DataSet ds1 = new DataSet();
                    string dtfromdate = Convert.ToDateTime(noticeboard.dtIssue_date).ToString("MM/dd/yyyy");
                    noticeboard.dtIssue_date = dtfromdate.Replace("-", "/");

                    string dtenddate = Convert.ToDateTime(noticeboard.dtEnd_date).ToString("MM/dd/yyyy");
                    noticeboard.dtEnd_date = dtenddate.Replace("-", "/");
                    com1.Parameters.AddWithValue("@type", "NewDemoInsertNoticeboardEdit");
                    com1.Parameters.AddWithValue("@dtIssue_date", noticeboard.dtIssue_date);
                    com1.Parameters.AddWithValue("@dtEnd_date", noticeboard.dtEnd_date);
                    com1.Parameters.AddWithValue("@vchSubject", noticeboard.vchSubject);
                    com1.Parameters.AddWithValue("@vchNotice", noticeboard.vchNotice);
                    com1.Parameters.AddWithValue("@intUpdate_by", noticeboard.intInserted_by);
                    com1.Parameters.AddWithValue("@UpdateIP", noticeboard.InsertIP);
                    com1.Parameters.AddWithValue("@intNotice_id", noticeboard.intNotice_id);
                    com1.Parameters.AddWithValue("@Image_Name", noticeboard.ImageName);
                    com1.Parameters.AddWithValue("@PDF_Name", noticeboard.PDFName);
                    com1.Parameters.AddWithValue("@vchURL", noticeboard.Link);
                    com1.Parameters.AddWithValue("@IsTeacherVisible", noticeboard.visibleForTeacher);
                    com1.Parameters.AddWithValue("@InsertedFrom","App");
                    con.Open();
                    com1.ExecuteReader();
                    con.Close();
                    con.Open();
                    SqlCommand com2 = new SqlCommand("select int_id from tblnoticeboard where intNotice_id='" + noticeboard.intNotice_id + "'", con);
                    SqlDataAdapter da = new SqlDataAdapter(com2);
                    DataSet ds = new DataSet();
                    da.Fill(ds);                    //dsObj.Tables[0].Rows.Count 
                    int ExistingRecords = (int)ds.Tables[0].Rows.Count;

                    string standards = noticeboard.intStandard_id;

                    string[] values = standards.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Trim();
                    }
                    string[] departments = noticeboard.intDepartment_id.Split(',');

                    con.Close();

                    if(noticeboard.intUserType_id == 1)
                    { 

                    if (ExistingRecords < values.Length)
                    {
                        for (int j = 0; j < ExistingRecords; j++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                            con.Open();
                            string check = "";
                            check = values[j].Trim();
                            String query3 = "update tblnoticeboard set intUserType_id='" + noticeboard.intUserType_id + "',intStandard_id='" + values[j] + "',intDivision_id='" + noticeboard.intDivision_id + "', intUser_id = '" + noticeboard.intUser_id + "',intDepartment_id = '" + noticeboard.intDepartment_id + "', intSchool_id = '" + noticeboard.intSchool_id + "', intActive_flg =1  where int_Id = '" + Intid + "'";
                            SqlCommand com3 = new SqlCommand(query3, con);
                            //com1.CommandType = CommandType.StoredProcedure;
                            //DataSet ds3 = new DataSet();
                            //com3.Parameters.Clear();
                            //com3.Parameters.AddWithValue("@type", "NewDemoInsertNoticeboardDetailsEdit");
                            //com3.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                            //com3.Parameters.AddWithValue("@intStandard_id", values[j]);
                            //com3.Parameters.AddWithValue("@DIV", noticeboard.intDivision_id);
                            //com3.Parameters.AddWithValue("@intUser_id", noticeboard.intUser_id);
                            //com3.Parameters.AddWithValue("@intDepartment_id", noticeboard.intDepartment_id);
                            //com3.Parameters.AddWithValue("@intSchool_id", noticeboard.intSchool_id);
                            //com3.Parameters.AddWithValue("@intActive_flg", "1");
                            ////com3.Parameters.AddWithValue("@intNotice_id", intNotice_id);
                            //com3.Parameters.AddWithValue("@int_id", Intid);
                            int result = com3.ExecuteNonQuery();
                            con.Close();
                            //SqlDataAdapter da3 = new SqlDataAdapter(com3);
                            //DataSet ds4 = new DataSet();
                            //da3.Fill(ds4, "NoticeboardDetails");
                            // return (ds4);
                        }

                        int count = values.Length - ExistingRecords;
                        for (int i = ExistingRecords; i < values.Length; i++)
                        {
                            string check = "";
                            check = values[i].Trim();
                            con.Open();
                            String query4 = "usp_Noticeboard";
                            SqlCommand com4 = new SqlCommand(query4, con);
                            com4.CommandType = CommandType.StoredProcedure;
                            com4.Parameters.Clear();
                            com4.Parameters.AddWithValue("@type", "NewDemoInsertNotice");
                            com4.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                            com4.Parameters.AddWithValue("@intStandard_id", values[i]);
                            com4.Parameters.AddWithValue("@DIV", noticeboard.intDivision_id);
                            com4.Parameters.AddWithValue("@intUser_id", noticeboard.intUser_id);
                            com4.Parameters.AddWithValue("@intDepartment_id", noticeboard.intDepartment_id);

                            com4.Parameters.AddWithValue("@intSchool_id", noticeboard.intSchool_id);
                            com4.Parameters.AddWithValue("@intNotice_id", noticeboard.intNotice_id);
                            com4.Parameters.AddWithValue("@intAcademic_id", noticeboard.intAcademic_id);
                            int result = com4.ExecuteNonQuery();
                            con.Close();
                        }

                    }

                    else if (ExistingRecords == values.Length)
                    {
                        for (int j = 0; j < ExistingRecords; j++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                            con.Open();
                            string check = "";
                            check = values[j].Trim();
                            String query3 = "update tblnoticeboard set intUserType_id='" + noticeboard.intUserType_id + "',intStandard_id='" + values[j] + "',intDivision_id='" + noticeboard.intDivision_id + "', intUser_id = '" + noticeboard.intUser_id + "',intDepartment_id = '" + noticeboard.intDepartment_id + "', intSchool_id = '" + noticeboard.intSchool_id + "', intActive_flg =1  where int_Id = '" + Intid + "'";
                            SqlCommand com3 = new SqlCommand(query3, con);
                            int result = com3.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                    else if (ExistingRecords > values.Length)
                    {
                        for (int j = 0; j < values.Length; j++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                            con.Open();
                            string check = "";
                            check = values[j].Trim();
                            String query3 = "update tblnoticeboard set intUserType_id='" + noticeboard.intUserType_id + "',intStandard_id='" + values[j] + "',intDivision_id='" + noticeboard.intDivision_id + "', intUser_id = '" + noticeboard.intUser_id + "',intDepartment_id = '" + noticeboard.intDepartment_id + "', intSchool_id = '" + noticeboard.intSchool_id + "', intActive_flg =1  where int_Id = '" + Intid + "'";
                            SqlCommand com3 = new SqlCommand(query3, con);
                            int result = com3.ExecuteNonQuery();
                            con.Close();
                            //SqlDataAdapter da3 = new SqlDataAdapter(com3);
                            //DataSet ds4 = new DataSet();
                            //da3.Fill(ds4, "NoticeboardDetails");
                            // return (ds4);
                        }

                        int count = ExistingRecords - values.Length;
                        for (int i = values.Length; i < ExistingRecords; i++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[i]["int_id"]);
                            con.Open();
                            //string check = "";
                            //check = values[i].Trim();
                            String query4 = "update tblnoticeboard set intActive_flg =0  where int_Id = '" + Intid + "'";
                            SqlCommand com4 = new SqlCommand(query4, con);
                            //com4.CommandType = CommandType.StoredProcedure;
                            //com4.Parameters.Clear();
                            //com4.Parameters.AddWithValue("@type", "NewDemoInsertNotice");
                            //com4.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                            //com4.Parameters.AddWithValue("@intStandard_id", values[i]);
                            //com4.Parameters.AddWithValue("@DIV", noticeboard.intDivision_id);
                            //com4.Parameters.AddWithValue("@intUser_id", noticeboard.intUser_id);
                            //com4.Parameters.AddWithValue("@intDepartment_id", noticeboard.intDepartment_id);

                            //com4.Parameters.AddWithValue("@intSchool_id", noticeboard.intSchool_id);
                            //com4.Parameters.AddWithValue("@intNotice_id", intNotice_id);
                            //com4.Parameters.AddWithValue("@intAcademic_id", noticeboard.intAcademic_id);
                            int result = com4.ExecuteNonQuery();
                            con.Close();
                        }

                    }
                    }
                    else
                    {

                        if (ExistingRecords < departments.Length)
                        {
                            for (int j = 0; j < ExistingRecords; j++)
                            {
                                string Intid = "";
                                Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                                con.Open();
                                String query3 = "update tblnoticeboard set intUserType_id='" + noticeboard.intUserType_id + "',intStandard_id='" + noticeboard.intStandard_id + "',intDivision_id='" + noticeboard.intDivision_id + "', intUser_id = '" + noticeboard.intUser_id + "',intDepartment_id = '" + departments[j] + "', intSchool_id = '" + noticeboard.intSchool_id + "', intActive_flg =1  where int_Id = '" + Intid + "'";
                                SqlCommand com3 = new SqlCommand(query3, con);
                                int result = com3.ExecuteNonQuery();
                                con.Close();
                            }
                            for (int i = ExistingRecords; i < departments.Length; i++)
                            {
                                con.Open();
                                String query4 = "usp_Noticeboard";
                                SqlCommand com4 = new SqlCommand(query4, con);
                                com4.CommandType = CommandType.StoredProcedure;
                                com4.Parameters.Clear();
                                com4.Parameters.AddWithValue("@type", "NewDemoInsertNotice");
                                com4.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                                com4.Parameters.AddWithValue("@intStandard_id", noticeboard.intStandard_id);
                                com4.Parameters.AddWithValue("@DIV", noticeboard.intDivision_id);
                                com4.Parameters.AddWithValue("@intUser_id", noticeboard.intUser_id);
                                com4.Parameters.AddWithValue("@intDepartment_id", departments[i]);

                                com4.Parameters.AddWithValue("@intSchool_id", noticeboard.intSchool_id);
                                com4.Parameters.AddWithValue("@intNotice_id", noticeboard.intNotice_id);
                                com4.Parameters.AddWithValue("@intAcademic_id", noticeboard.intAcademic_id);
                                int result = com4.ExecuteNonQuery();
                                con.Close();
                            }

                        }

                        else if (ExistingRecords == departments.Length)
                        {
                            for (int j = 0; j < ExistingRecords; j++)
                            {
                                string Intid = "";
                                Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                                con.Open();
                                String query3 = "update tblnoticeboard set intUserType_id='" + noticeboard.intUserType_id + "',intStandard_id='" + noticeboard.intStandard_id + "',intDivision_id='" + noticeboard.intDivision_id + "', intUser_id = '" + noticeboard.intUser_id + "',intDepartment_id = '" + departments[j] + "', intSchool_id = '" + noticeboard.intSchool_id + "', intActive_flg =1  where int_Id = '" + Intid + "'";
                                SqlCommand com3 = new SqlCommand(query3, con);
                                int result = com3.ExecuteNonQuery();
                                con.Close();
                            }
                        }

                        else if (ExistingRecords > departments.Length)
                        {
                            for (int j = 0; j < departments.Length; j++)
                            {
                                string Intid = "";
                                Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                                con.Open();
                                String query3 = "update tblnoticeboard set intUserType_id='" + noticeboard.intUserType_id + "',intStandard_id='" + noticeboard.intStandard_id + "',intDivision_id='" + noticeboard.intDivision_id + "', intUser_id = '" + noticeboard.intUser_id + "',intDepartment_id = '" + departments[j] + "', intSchool_id = '" + noticeboard.intSchool_id + "', intActive_flg =1  where int_Id = '" + Intid + "'";
                                SqlCommand com3 = new SqlCommand(query3, con);
                                int result = com3.ExecuteNonQuery();
                                con.Close();
                            }
                            
                            for (int i = departments.Length; i < ExistingRecords; i++)
                            {
                                string Intid = "";
                                Intid = Convert.ToString(ds.Tables[0].Rows[i]["int_id"]);
                                con.Open();
                                String query4 = "update tblnoticeboard set intActive_flg =0  where int_Id = '" + Intid + "'";
                                SqlCommand com4 = new SqlCommand(query4, con);
                                int result = com4.ExecuteNonQuery();
                                con.Close();
                            }

                        }

                    }

                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // con.Close();
                    //con.Dispose();
                }
            }
        }
        public DataSet NoticeboardDetails(string command, Noticeboard noticeboard)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {

                    String query = "";
                    //query = "usp_NewAdminDashboard";
                    query = "usp_Noticeboard";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    if (noticeboard.intUserType_id == 5)
                    {
                        com.Parameters.AddWithValue("@type", command);
                        com.Parameters.AddWithValue("@intSchool_Id", noticeboard.intSchool_id);
                        if (command == "NewDemoActiveNotice" || command == "NewDemoInactiveNotice" || command == "NewDemoUpcomingNotice")
                        {
                            com.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                            com.Parameters.AddWithValue("@Pageindex", noticeboard.pageindex);
                            com.Parameters.AddWithValue("@PageSize", "10");
                            if (noticeboard.filterBy == "All")
                                com.Parameters.AddWithValue("@intUsertype_id", "0,1,3,4,5");
                            else
                                com.Parameters.AddWithValue("@intUsertype_id", noticeboard.filterBy);
                        }
                        else if (command == "NewDemoselectDataWithID")
                        {
                            com.Parameters.AddWithValue("@SchoolId", noticeboard.intSchool_id);
                            com.Parameters.AddWithValue("@intNotice_id", noticeboard.intNotice_id);
                        }
                        else if (command == "InactivateNotice")
                        {
                            com.Parameters.AddWithValue("@intNotice_id", noticeboard.intNotice_id);
                        }
                    }
                    else
                    {
                        com.Parameters.AddWithValue("@Pageindex", noticeboard.pageindex);
                        com.Parameters.AddWithValue("@PageSize", "10");
                        if (command == "NewDemoNotice")
                        {
                            // com.Parameters.AddWithValue("@type", "NewDemoNotice");
                            com.Parameters.AddWithValue("@type", command);
                            com.Parameters.AddWithValue("@SchoolId", noticeboard.intSchool_id);
                            com.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                            com.Parameters.AddWithValue("@intDepartment_id", noticeboard.intDepartment_id);
                            com.Parameters.AddWithValue("@filterby",noticeboard.filterBy);
                            if (noticeboard.intUserType_id == 3)
                                com.Parameters.AddWithValue("@intStandard_id","0");
                        }
                        else if (command == "NewDemoNoticeForStudent")
                        {
                            com.Parameters.AddWithValue("@type", command);
                            com.Parameters.AddWithValue("@SchoolId", noticeboard.intSchool_id);
                            com.Parameters.AddWithValue("@UserTypeId", noticeboard.intUserType_id);
                            com.Parameters.AddWithValue("@intDepartment_id", noticeboard.intDepartment_id);
                            com.Parameters.AddWithValue("@intStandard_id", noticeboard.intStandard_id);
                        }
                    }
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "NoticeboardDetails");
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet DeleteNotice(string command, Noticeboard noticeboard)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {

                    String query = "";
                    query = "usp_Noticeboard";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@type", "DeleteNotice");
                        com.Parameters.AddWithValue("@Delete_IP", noticeboard.InsertIP);
                        com.Parameters.AddWithValue("@intDelete_by", noticeboard.intUser_id);
                        com.Parameters.AddWithValue("@intNotice_id",noticeboard.intNotice_id);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "NoticeboardDetails");
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        private string GenerateRandomOTP(int iOTPLength, string[] saAllowedCharacters)

        {

            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < iOTPLength; i++)

            {

                int p = rand.Next(0, saAllowedCharacters.Length);

                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                sOTP += sTempChars;

            }

            return sOTP;

        }

        public DataSet ValidatingOTP(string command, OTPValidate OTPvalidate)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {

                    String query = "usp_ValidateMobileNo";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "ValidateOTP");
                    com.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                    com.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "OTPValidate");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string ReceivedOTP = Convert.ToString(ds.Tables[0].Rows[0]["OTP"]);

                        if (OTPvalidate.OTP == ReceivedOTP)
                        {
                            SqlCommand com1 = new SqlCommand(query, con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "UpdateValidFlg");
                            com1.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                            com1.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                            com1.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                            com1.Parameters.AddWithValue("@OTP", OTPvalidate.OTP);
                            com1.Parameters.AddWithValue("@intvalid_flg", 1);

                            int i = com1.ExecuteNonQuery();

                        }
                        else
                        {
                            SqlCommand com1 = new SqlCommand(query, con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "UpdateNotValidFlg");
                            com1.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                            com1.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                            com1.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                            com1.Parameters.AddWithValue("@intvalid_flg", 2);

                            int i = com1.ExecuteNonQuery();

                        }
                    }


                    SqlCommand com2 = new SqlCommand("usp_ValidateMobileNo", con);
                    com2.CommandType = CommandType.StoredProcedure;
                    com2.Parameters.AddWithValue("@command", "OTPReturnValue");
                    com2.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com2.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                    com2.Parameters.AddWithValue("@mobile_number", OTPvalidate.mobile_number);
                    com2.Parameters.AddWithValue("@OTP", OTPvalidate.OTP);

                    SqlDataAdapter da1 = new SqlDataAdapter(com2);
                    DataSet dsObj = new DataSet();
                    da1.Fill(dsObj, "Status");

                    return (dsObj);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }

        }
        public DataSet EmailValidation(string command, OTPValidate OTPvalidate)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    string sRandomOTP = GenerateRandomOTP(4, saAllowedCharacters);

                    MailMessage mail2 = new MailMessage();
                    mail2.To.Add(OTPvalidate.EmailId);
                    mail2.From = new MailAddress("enquiry@efficacious.co.in", "Efficacious India Limited");
                    mail2.Subject = "Testing OTP";
                    string body2 = sRandomOTP+ " OTP";
                    mail2.Body = body2;
                    mail2.IsBodyHtml = true;
                    SmtpClient smtp2 = new SmtpClient();
                    smtp2.Host = "mail.efficacious.co.in";
                    smtp2.Port = 25;
                    smtp2.UseDefaultCredentials = true;
                    smtp2.Credentials = new System.Net.NetworkCredential("enquiry@efficacious.co.in", "effi@2019");
                    smtp2.EnableSsl = false;
                    smtp2.Send(mail2);

                    SqlCommand com1 = new SqlCommand("usp_ValidateMobileNo", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    com1.Parameters.AddWithValue("@command", "InsertEmailOTPRecord");
                    com1.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com1.Parameters.AddWithValue("@vchEmail_id", OTPvalidate.EmailId);
                    com1.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                    com1.Parameters.AddWithValue("@EmailOTP", sRandomOTP);
                    com1.Parameters.AddWithValue("@bitvalid_flg", 0);

                    int i = com1.ExecuteNonQuery();


                    SqlCommand com2 = new SqlCommand("usp_ValidateMobileNo", con);
                    com2.CommandType = CommandType.StoredProcedure;
                    com2.Parameters.AddWithValue("@command", "ReturnEmailValue");
                    com2.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com2.Parameters.AddWithValue("@vchEmail_id", OTPvalidate.EmailId);
                    com2.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);

                    SqlDataAdapter da1 = new SqlDataAdapter(com2);
                    DataSet dsObj = new DataSet();
                    da1.Fill(dsObj, "Status");

                    return (dsObj);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        public DataSet ValidatingEmailOTP(string command, OTPValidate OTPvalidate)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {

                    String query = "usp_ValidateMobileNo";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "ValidateEmailOTP");
                    com.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                    com.Parameters.AddWithValue("@vchEmail_id", OTPvalidate.EmailId);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "OTPValidate");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string ReceivedOTP = Convert.ToString(ds.Tables[0].Rows[0]["EmailOTP"]);

                        if (OTPvalidate.EmailOTP == ReceivedOTP)
                        {
                            SqlCommand com1 = new SqlCommand(query, con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "UpdateEmailValidFlg");
                            com1.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                            com1.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                            com1.Parameters.AddWithValue("@vchEmail_id", OTPvalidate.EmailId);
                            com1.Parameters.AddWithValue("@EmailOTP", OTPvalidate.EmailOTP);
                            com1.Parameters.AddWithValue("@bitvalid_flg", 1);

                            int i = com1.ExecuteNonQuery();

                        }
                        else
                        {
                            SqlCommand com1 = new SqlCommand(query, con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "UpdateEmailNotValidFlg");
                            com1.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                            com1.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                            com1.Parameters.AddWithValue("@vchEmail_id", OTPvalidate.EmailId);
                            com1.Parameters.AddWithValue("@bitvalid_flg", 2);

                            int i = com1.ExecuteNonQuery();

                        }
                    }


                    SqlCommand com2 = new SqlCommand("usp_ValidateMobileNo", con);
                    com2.CommandType = CommandType.StoredProcedure;
                    com2.Parameters.AddWithValue("@command", "EmailOTPReturnValue");
                    com2.Parameters.AddWithValue("@intUserType_id", OTPvalidate.intuser_type);
                    com2.Parameters.AddWithValue("@intUser_id", OTPvalidate.intuser_id);
                    com2.Parameters.AddWithValue("@vchEmail_id", OTPvalidate.EmailId);
                    

                    SqlDataAdapter da1 = new SqlDataAdapter(com2);
                    DataSet dsObj = new DataSet();
                    da1.Fill(dsObj, "Status");

                    return (dsObj);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }

        }

        public DataSet HolidayList(string command, Holiday holiday)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    string query = "usp_setHolidayList";
                    SqlCommand com = new SqlCommand(query, con);
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    if (command == "ListOfHolidays")
                    {
                        com.Parameters.AddWithValue("@type", command);
                        com.Parameters.AddWithValue("@intSchool_id", holiday.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", holiday.intAcademic_id);
                        com.Parameters.AddWithValue("@orderby", holiday.orderby);
                    }
                    else if (command == "InsertUpdate")
                    {
                        com.Parameters.AddWithValue("@type", command);
                        com.Parameters.AddWithValue("@vchHoliday_name", holiday.vchHoliday_name);
                        com.Parameters.AddWithValue("@dtFromDate", holiday.dtFromDate);
                        com.Parameters.AddWithValue("@dtToDate", holiday.dtToDate);
                        com.Parameters.AddWithValue("@Description", holiday.Description);
                        com.Parameters.AddWithValue("@intInsertedBy", holiday.intInsertedBy);
                        com.Parameters.AddWithValue("@vchInsertedIp", holiday.vchInsertedIp);
                        //com.Parameters.AddWithValue("@intHoliday_Type_Id", holiday.intHoliday_Type_Id);
                        com.Parameters.AddWithValue("@intSchool_id", holiday.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", holiday.intAcademic_id);
                    }
                    else if (command == "FilldataEdit")
                    {
                        com.Parameters.AddWithValue("@type", command);
                        com.Parameters.AddWithValue("@intHoliday_id", holiday.intHoliday_id);
                        com.Parameters.AddWithValue("@intSchool_id", holiday.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", holiday.intAcademic_id);
                    }
                    else if (command == "UpdateHoliday")
                    {
                        com.Parameters.AddWithValue("@type", command);
                        com.Parameters.AddWithValue("@intHoliday_id", holiday.intHoliday_id);
                        com.Parameters.AddWithValue("@vchHoliday_name", holiday.vchHoliday_name);
                        com.Parameters.AddWithValue("@dtFromDate", holiday.dtFromDate);
                        com.Parameters.AddWithValue("@dtToDate", holiday.dtToDate); 
                        com.Parameters.AddWithValue("@Description", holiday.Description);
                        com.Parameters.AddWithValue("@intUpdatedBy", holiday.intUpdatedBy);
                        com.Parameters.AddWithValue("@vchUpdatedIp", holiday.vchUpdatedIp);
                        // com.Parameters.AddWithValue("@intHoliday_Type_Id", holiday.intHoliday_Type_Id);
                        com.Parameters.AddWithValue("@intSchool_id", holiday.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", holiday.intAcademic_id);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "Holiday");
                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet HolidayDelete(string command, Holiday holiday)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_setHolidayList";
                    SqlCommand com = new SqlCommand(query, con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@type", "DeleteHoliday");
                    com.Parameters.AddWithValue("@intHoliday_id", holiday.intHoliday_id);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "Delete");
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet VacationList(string command, Vacation vacation)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    string query = "usp_setVacationList";
                    SqlCommand com = new SqlCommand(query, con);
                    DataSet ds = new DataSet();
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    if (command == "InsertVacation")
                    {
                        string dtfromdate = Convert.ToDateTime(vacation.dtFromDate).ToString("MM/dd/yyyy");
                        vacation.dtFromDate = dtfromdate.Replace("-", "/");

                        string dtenddate = Convert.ToDateTime(vacation.dtToDate).ToString("MM/dd/yyyy");
                        vacation.dtToDate = dtenddate.Replace("-", "/");

                        SqlCommand com1 = new SqlCommand(query, con);
                        com1.CommandType = CommandType.StoredProcedure;
                        com1.Parameters.AddWithValue("@type", command);
                        //com1.Parameters.AddWithValue("@vchVacation_name", vacation.vchVacation_name);
                        com1.Parameters.Add("@vchVacation_name", SqlDbType.NVarChar).Value = vacation.vchVacation_name;
                        com1.Parameters.AddWithValue("@dtFromDate", vacation.dtFromDate);
                        com1.Parameters.AddWithValue("@dtToDate", vacation.dtToDate);
                        com1.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                        com1.Parameters.AddWithValue("@intInsertedBy", vacation.intInsertedBy);
                        com1.Parameters.AddWithValue("@vchInsertedIp", vacation.vchInsertedIp);
                        com1.Parameters.AddWithValue("@Description",vacation.Description);
                        com1.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                        com1.ExecuteReader();
                        con.Close();
                        con.Open();

                        string StringStandards = vacation.standards;
                        string[] values = StringStandards.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = values[i].Trim();
                            com.Parameters.Clear();
                            com.Parameters.AddWithValue("@type", "InsertVacationDetails");
                            com.Parameters.AddWithValue("@vchVacation_name", vacation.vchVacation_name);
                            com.Parameters.AddWithValue("@intStandard_id", values[i]);
                            com.Parameters.AddWithValue("@dtFromDate", vacation.dtFromDate);
                            com.Parameters.AddWithValue("@dtToDate", vacation.dtToDate);
                            com.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                            com.Parameters.AddWithValue("@intInsertedBy", vacation.intInsertedBy);
                            com.Parameters.AddWithValue("@vchInsertedIp", vacation.vchInsertedIp);
                            com.Parameters.AddWithValue("@Description", vacation.Description);
                            com.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                            SqlDataAdapter da = new SqlDataAdapter(com);
                            da.Fill(ds, "Vacation");
                        }
                    }
                    else if (command == "VacationList")
                    {
                        com.Parameters.AddWithValue("@type", "VacationList");
                        com.Parameters.AddWithValue("@standard", vacation.standards);
                        com.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                        com.Parameters.AddWithValue("@orderby", vacation.orderby);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "Vacation");
                    }
                    else if (command == "EditVacation")
                    {
                        com.Parameters.AddWithValue("@type", "EditVacation");
                        com.Parameters.AddWithValue("@intVacation_id", vacation.intVacation_id);
                        com.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "Vacation");
                    }
                    else if (command == "StudentVacation")
                    {
                        com.Parameters.AddWithValue("@type", "StudentVacation");
                        com.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                        com.Parameters.AddWithValue("@intstandard_id", vacation.intStandard_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "Vacation");
                    }
                    else if (command == "ListOfStandards")
                    {
                        com.Parameters.AddWithValue("@type", "ListOfStandards");
                        com.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds, "Standards");
                    }

                    else if(command == "CalendarMonthData") 
                    {
                        com.Parameters.AddWithValue("@type", "CalendarMonthData");
                        com.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                        com.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                        com.Parameters.AddWithValue("@standard", vacation.standards);
                        com.Parameters.AddWithValue("@month", vacation.Month);
                        com.Parameters.AddWithValue("@year", vacation.Year);
                        com.Parameters.AddWithValue("@orderby", vacation.orderby);
                        SqlDataAdapter da = new SqlDataAdapter(com);
                        da.Fill(ds);
                        ds.Tables[0].TableName = "Holiday";
                        ds.Tables[1].TableName = "Vacation";
                    }

                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet VacationEdit(string command, Vacation vacation)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query1 = "usp_setVacationList";
                    SqlCommand com1 = new SqlCommand(query1, con);
                    com1.CommandType = CommandType.StoredProcedure;
                    DataSet ds1 = new DataSet();
                    string dtfromdate = Convert.ToDateTime(vacation.dtFromDate).ToString("MM/dd/yyyy");
                    vacation.dtFromDate = dtfromdate.Replace("-", "/");

                    string dtenddate = Convert.ToDateTime(vacation.dtToDate).ToString("MM/dd/yyyy");
                    vacation.dtToDate = dtenddate.Replace("-", "/");
                    com1.Parameters.AddWithValue("@type", "EditVacation_master");
                    com1.Parameters.AddWithValue("@vchVacation_name", vacation.vchVacation_name);
                    com1.Parameters.AddWithValue("@dtFromDate", vacation.dtFromDate);
                    com1.Parameters.AddWithValue("@dtToDate", vacation.dtToDate);
                    com1.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                    com1.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                    com1.Parameters.AddWithValue("@intUpdatedBy", vacation.intUpdatedBy);
                    com1.Parameters.AddWithValue("@vchUpdatedIp", vacation.vchUpdatedIp);
                    com1.Parameters.AddWithValue("@Description", vacation.Description);
                    com1.Parameters.AddWithValue("@intvacation_id", vacation.intVacation_id);
                    con.Open();
                    com1.ExecuteReader();
                    con.Close();
                    con.Open();
                    SqlCommand com2 = new SqlCommand("select int_id from tblVacation where intVacation_id='" + vacation.intVacation_id + "'", con);
                    SqlDataAdapter da = new SqlDataAdapter(com2);
                    DataSet ds = new DataSet();
                    da.Fill(ds);                    //dsObj.Tables[0].Rows.Count 
                    int ExistingRecords = (int)ds.Tables[0].Rows.Count;

                    string standards = vacation.standards;

                    string[] values = standards.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Trim();
                    }
                    con.Close();
                    if (ExistingRecords < values.Length)
                    {
                        for (int j = 0; j < ExistingRecords; j++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                            con.Open();
                            string check = "";
                            check = values[j].Trim();
                            String query3 = "update tblVacation set intStandard_id='" + values[j] + "' , intUpdatedBy='" + vacation.intUpdatedBy + "' , dtUpdatedDt='" + DateTime.Now.ToString("MM/dd/yyyy") + "' , vchUpdatedIp='" + vacation.vchUpdatedIp + "' , bitActiveFlg =1  where int_Id = '" + Intid + "'";
                            SqlCommand com3 = new SqlCommand(query3, con);

                            int result = com3.ExecuteNonQuery();
                            con.Close();

                        }

                        int count = values.Length - ExistingRecords;
                        for (int i = ExistingRecords; i < values.Length; i++)
                        {
                            string check = "";
                            check = values[i].Trim();
                            con.Open();
                            String query4 = "usp_setVacationList";
                            SqlCommand com4 = new SqlCommand(query4, con);
                            com4.CommandType = CommandType.StoredProcedure;
                            com4.Parameters.Clear();
                            com4.Parameters.AddWithValue("@type", "InsertVacationDetailsEdit");
                            com4.Parameters.AddWithValue("@vchVacation_name", vacation.vchVacation_name);
                            com4.Parameters.AddWithValue("@intStandard_id", values[i]);
                            com4.Parameters.AddWithValue("@dtFromDate", vacation.dtFromDate);
                            com4.Parameters.AddWithValue("@dtToDate", vacation.dtToDate);
                            com4.Parameters.AddWithValue("@intSchool_id", vacation.intSchool_id);
                            com4.Parameters.AddWithValue("@intInsertedBy", vacation.intInsertedBy);
                            com4.Parameters.AddWithValue("@vchInsertedIp", vacation.vchInsertedIp);
                            com4.Parameters.AddWithValue("@Description", vacation.Description);
                            com4.Parameters.AddWithValue("@intAcademic_id", vacation.intAcademic_id);
                            com4.Parameters.AddWithValue("@intvacation_id", vacation.intVacation_id);
                            int result = com4.ExecuteNonQuery();
                            con.Close();
                        }

                    }

                    else if (ExistingRecords == values.Length)
                    {
                        for (int j = 0; j < ExistingRecords; j++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                            con.Open();
                            string check = "";
                            check = values[j].Trim();
                            String query3 = "update tblVacation set intStandard_id='" + values[j] + "'  , intUpdatedBy='" + vacation.intUpdatedBy + "' , dtUpdatedDt='" + DateTime.Now.ToString("MM/dd/yyyy") + "' , vchUpdatedIp='" + vacation.vchUpdatedIp + "' , bitActiveFlg =1  where int_Id = '" + Intid + "'";
                            SqlCommand com3 = new SqlCommand(query3, con);
                            int result = com3.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                    else if (ExistingRecords > values.Length)
                    {
                        for (int j = 0; j < values.Length; j++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[j]["int_id"]);
                            con.Open();
                            string check = "";
                            check = values[j].Trim();
                            String query3 = "update tblVacation set intStandard_id='" + values[j] + "' , intUpdatedBy='" + vacation.intUpdatedBy + "' , dtUpdatedDt='" + DateTime.Now.ToString("MM/dd/yyyy") + "' , vchUpdatedIp='" + vacation.vchUpdatedIp + "' , bitActiveFlg =1  where int_Id = '" + Intid + "'";
                            SqlCommand com3 = new SqlCommand(query3, con);
                            int result = com3.ExecuteNonQuery();
                            con.Close();

                        }

                        int count = ExistingRecords - values.Length;
                        for (int i = values.Length; i < ExistingRecords; i++)
                        {
                            string Intid = "";
                            Intid = Convert.ToString(ds.Tables[0].Rows[i]["int_id"]);
                            con.Open();
                            String query4 = "update tblVacation set bitActiveFlg =0  where int_Id = '" + Intid + "'";
                            SqlCommand com4 = new SqlCommand(query4, con);
                            int result = com4.ExecuteNonQuery();
                            con.Close();
                        }

                    }

                    return (ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet VacationDelete(string command, Vacation vacation)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    String query = "usp_setVacationList";
                    SqlCommand com = new SqlCommand(query, con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@type", "DeleteVacation");
                    com.Parameters.AddWithValue("@intVacation_id", vacation.intVacation_id);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds,"Delete");
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public DataSet SendMessageTest(Message message)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    DataSet ds = new DataSet();
                    if (message.command != "Multiple")
                    {
                        if (message.Status != "Draft")
                        {
                            message.Status = "Inprogress";
                        }

                        string msg_id = null;

                        String queryList = "usp_Message";
                        SqlCommand com5 = new SqlCommand(queryList, con);
                        con.Open();
                        com5.CommandType = CommandType.StoredProcedure;
                        com5.Parameters.AddWithValue("@command", message.command);
                        com5.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                        com5.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);

                        if (message.command == "ListOfTeachers" || message.command == "ListOfStaff")
                        {
                            string usertype_id = null;
                            usertype_id = message.command == "ListOfTeachers" ? "3" : "4";
                            SqlCommand sqlCommand = new SqlCommand(queryList, con);
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlCommand.Parameters.AddWithValue("@command", "ListOfDepartments");
                            sqlCommand.Parameters.AddWithValue("@intUserType_id", usertype_id);
                            sqlCommand.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = sqlCommand;
                            DataSet dataSet = new DataSet();
                            da.Fill(dataSet);
                            string department_id = null;
                            string deptid = null;
                            for (int a = 0; a < dataSet.Tables[0].Rows.Count; a++)
                            {
                                department_id = Convert.ToString(dataSet.Tables[0].Rows[a]["intDepartment"]);
                                deptid += department_id + ",";
                            }
                            com5.Parameters.AddWithValue("@intDepartment_id", deptid);
                        }

                        // For selected students,teachers or staff
                        if (message.command == "ListOfSelectedStudents" || message.command == "ListOfSelectedTeachers" || message.command == "ListOfSelectedStaffs")
                        {
                            com5.Parameters.AddWithValue("@intStandard_id", message.intStandard_id);
                            com5.Parameters.AddWithValue("@intDivision_id", message.intDivision_id);
                            com5.Parameters.AddWithValue("@intUser_id", message.intUser_id);
                        }
                        //for Sending  All Standards
                        else if (message.command == "ListOfStudentsStandardWise")
                        {
                            com5.Parameters.AddWithValue("@intStandard_id", message.intStandard_id);
                        }
                        // For sending selected divisions
                        else if (message.command == "ListOfStudentsDivisionWise")
                        {
                            com5.Parameters.AddWithValue("@intStandard_id", message.intStandard_id);
                            com5.Parameters.AddWithValue("@intDivision_id", message.intDivision_id);
                        }

                        SqlDataAdapter da5 = new SqlDataAdapter(com5);
                        DataSet dsObj = new DataSet();
                        da5.Fill(dsObj, "AllStandardList");

                        //Add columns to the dataset for teacher and staff
                        if (message.command == "ListOfTeachers" || message.command == "ListOfStaff")
                        {
                            //Change Column name
                            if (message.command == "ListOfTeachers")
                                dsObj.Tables["AllStandardList"].Columns["intteacher_id"].ColumnName = "intStudent_id";

                            //Change Column name
                            if (message.command == "ListOfStaff")
                                dsObj.Tables["AllStandardList"].Columns["intstaff_id"].ColumnName = "intStudent_id";

                            //Change Column name
                            dsObj.Tables["AllStandardList"].Columns["intMobileNo"].ColumnName = "intBusAlert1";

                            //Add intstanderd_id column to the dataset
                            System.Data.DataColumn newColumn = new System.Data.DataColumn("intstanderd_id", typeof(System.Int32));
                            newColumn.DefaultValue = "0";
                            dsObj.Tables["AllStandardList"].Columns.Add(newColumn);

                            //Add intDivision_id column to the dataset
                            System.Data.DataColumn newColumn1 = new System.Data.DataColumn("intDivision_id", typeof(System.Int32));
                            newColumn1.DefaultValue = "0";
                            dsObj.Tables["AllStandardList"].Columns.Add(newColumn1);
                        }

                        if (message.Msg_id == null)
                        {
                            //insert message into tblmsg_master 
                            SqlCommand com1 = new SqlCommand("usp_Message", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "insertMsg_Master");
                            com1.Parameters.AddWithValue("@MessageTitle", message.messagetitle);
                            com1.Parameters.AddWithValue("@vchMessage", message.message);
                            com1.Parameters.AddWithValue("@MessageCount", message.messagecount);
                            com1.Parameters.AddWithValue("@insertIP", message.IP);
                            com1.Parameters.AddWithValue("@intSchool_id", message.intSchool_id);
                            com1.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                            com1.Parameters.AddWithValue("@Status", message.Status);
                            com1.Parameters.AddWithValue("@MessageLanguage", message.MessageLanguage);
                            SqlDataAdapter da2 = new SqlDataAdapter(com1);
                            da2.Fill(dsObj, "Msg_id");
                        }
                        else
                        {
                            SqlCommand com1 = new SqlCommand("usp_Message", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "Delete_MsgDetails");
                            com1.Parameters.AddWithValue("@Msg_id", message.Msg_id);

                            com1.Parameters.AddWithValue("@MessageTitle", message.messagetitle);
                            com1.Parameters.AddWithValue("@vchMessage", message.message);
                            com1.Parameters.AddWithValue("@MessageCount", message.messagecount);
                            com1.Parameters.AddWithValue("@insertIP", message.IP);
                            com1.Parameters.AddWithValue("@MessageLanguage", message.MessageLanguage);

                            SqlDataAdapter da2 = new SqlDataAdapter(com1);
                            da2.Fill(dsObj, "Msg_id");
                        }
                        int Sendmsg = 0;

                        for (int j = 0; dsObj.Tables["AllStandardList"].Rows.Count > j; j++)
                        {
                            string Mobile_Number = Convert.ToString(dsObj.Tables["AllStandardList"].Rows[j]["intBusAlert1"]);
                            string msg = "";
                            //Mobile_Number = message.Mobile_number;
                            msg = message.message;

                           // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=TXT&message=" + msg + "&response=Y";
                           //// string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                           // HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                           // HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                           // StreamReader sr = new StreamReader(resp.GetResponseStream());
                           // string results = sr.ReadToEnd();

                            string results = "0";
                            String query = "usp_Message";
                            SqlCommand com = new SqlCommand(query, con);
                            //con.Open();
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@command", "insertMsg_details");
                            com.Parameters.AddWithValue("@intUserType_id", message.intUserType_id);
                            com.Parameters.AddWithValue("@intStandard_id", dsObj.Tables["AllStandardList"].Rows[j]["intstanderd_id"]);
                            com.Parameters.AddWithValue("@intDivision_id", dsObj.Tables["AllStandardList"].Rows[j]["intDivision_id"]);
                            com.Parameters.AddWithValue("@intDepartment_id", dsObj.Tables["AllStandardList"].Rows[j]["intDepartment_id"]);
                            com.Parameters.AddWithValue("@intUser_id", dsObj.Tables["AllStandardList"].Rows[j]["intStudent_id"]);
                            com.Parameters.AddWithValue("@MobileNo", dsObj.Tables["AllStandardList"].Rows[j]["intBusAlert1"]);
                            com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                            com.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                            com.Parameters.AddWithValue("@Result", results);
                            com.Parameters.AddWithValue("@Msg_status", message.Status);
                            msg_id = Convert.ToString(dsObj.Tables["Msg_id"].Rows[0]["Msg_id"]);
                            com.Parameters.AddWithValue("@Msg_id", msg_id);

                            int i = com.ExecuteNonQuery();
                            if (i == 1)
                                Sendmsg += 1;
                            //String query1 = "usp_Message";
                            //SqlCommand com2 = new SqlCommand(query1, con);
                            //com2.CommandType = CommandType.StoredProcedure;
                            //com2.Parameters.AddWithValue("@command", "ReturnValue");
                            //com2.Parameters.AddWithValue("@Result", results);
                            //SqlDataAdapter da = new SqlDataAdapter(com);
                            //da.Fill(dsObj);
                            // return (ds);
                        }
                        int updaterows = 0;
                        if (message.Status != "Draft")
                        {

                            SqlCommand com3 = new SqlCommand("usp_Message", con);
                            com3.CommandType = CommandType.StoredProcedure;
                            com3.Parameters.AddWithValue("@command", "Update_master");
                            com3.Parameters.AddWithValue("@Msg_id", msg_id);
                            SqlDataAdapter da3 = new SqlDataAdapter(com3);
                            da3.Fill(dsObj, "MessageDetails");
                            for (int j = 0; dsObj.Tables["MessageDetails"].Rows.Count > j; j++)
                            {
                                string Mobile_Number = Convert.ToString(dsObj.Tables["MessageDetails"].Rows[j]["MobileNumber"]);
                                string msg = "";
                                msg = message.message;
                                string URL = "";
                                if (message.MessageLanguage == "0")
                                {
                                    URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=TXT&message=" + msg + "&response=Y";
                                }
                                else
                                {
                                    URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                                }
                                // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                StreamReader sr = new StreamReader(resp.GetResponseStream());
                                string results = sr.ReadToEnd();
                                //string results = "0";
                                string id = Convert.ToString(dsObj.Tables["MessageDetails"].Rows[j]["id"]);
                                String query = "usp_Message";
                                SqlCommand com4 = new SqlCommand(query, con);
                                com4.CommandType = CommandType.StoredProcedure;
                                com4.Parameters.AddWithValue("@command", "Update_Details");
                                com4.Parameters.AddWithValue("@Result", results);
                                com4.Parameters.AddWithValue("@id", id);
                                int i = com4.ExecuteNonQuery();
                                if (i == 1)
                                    updaterows += 1;
                            }
                        }
                        if (message.Status == "Draft")
                        {
                            DataTable ds1 = ds.Tables.Add("SendMessages");
                            ds1.Columns.Add("Result", typeof(Int32));
                            DataRow dr = ds.Tables["SendMessages"].NewRow();
                            dr["Result"] = Sendmsg;
                            ds.Tables["SendMessages"].Rows.InsertAt(dr, 0);
                        }
                        else
                        {
                            DataTable ds1 = ds.Tables.Add("SendMessages");
                            ds1.Columns.Add("Result", typeof(Int32));
                            DataRow dr = ds.Tables["SendMessages"].NewRow();
                            dr["Result"] = updaterows;
                            ds.Tables["SendMessages"].Rows.InsertAt(dr, 0);
                        }
                    }
                    else
                    {
                        if (message.Status != "Draft")
                        {
                            message.Status = "Inprogress";
                        }

                        string msg_id = null;

                        string[] values = message.Mobile_number.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = values[i].Trim();
                        }

                        DataSet dsObj = new DataSet();

                        if (message.Msg_id == null)
                        {
                            //insert message into tblmsg_master 
                            SqlCommand com1 = new SqlCommand("usp_Message", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "insertMsg_Master");
                            com1.Parameters.AddWithValue("@MessageTitle", message.messagetitle);
                            com1.Parameters.AddWithValue("@vchMessage", message.message);
                            com1.Parameters.AddWithValue("@MessageCount", message.messagecount);
                            com1.Parameters.AddWithValue("@insertIP", message.IP);
                            com1.Parameters.AddWithValue("@intSchool_id", message.intSchool_id);
                            com1.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                            com1.Parameters.AddWithValue("@Status", message.Status);
                            com1.Parameters.AddWithValue("@MessageLanguage", message.MessageLanguage);
                            SqlDataAdapter da2 = new SqlDataAdapter(com1);
                            da2.Fill(dsObj, "Msg_id");
                        }
                        else
                        {
                            SqlCommand com1 = new SqlCommand("usp_Message", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            com1.Parameters.AddWithValue("@command", "Delete_MsgDetails");
                            com1.Parameters.AddWithValue("@MessageTitle", message.messagetitle);
                            com1.Parameters.AddWithValue("@vchMessage", message.message);
                            com1.Parameters.AddWithValue("@MessageCount", message.messagecount);
                            com1.Parameters.AddWithValue("@InsertIP", message.IP);
                            com1.Parameters.AddWithValue("@MessageLanguage", message.MessageLanguage);
                            com1.Parameters.AddWithValue("@Msg_id", message.Msg_id);
                            SqlDataAdapter da2 = new SqlDataAdapter(com1);
                            da2.Fill(dsObj, "Msg_id");
                        }
                        int Sendmsg = 0;
                        con.Open();
                        for (int j = 0; values.Count() > j; j++)
                        {
                            string Mobile_Number = values[j].Trim();
                            string msg = "";
                            msg = message.message;

                            //string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=TXT&message=" + msg + "&response=Y";
                            ////string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                            //HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                            //StreamReader sr = new StreamReader(resp.GetResponseStream());
                            //string results = sr.ReadToEnd();

                            string results = "0";
                            String query = "usp_Message";
                            SqlCommand com = new SqlCommand(query, con);
                            //con.Open();
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@command", "insertMsg_details");
                            com.Parameters.AddWithValue("@intUserType_id", "0");
                            com.Parameters.AddWithValue("@intStandard_id", "0");
                            com.Parameters.AddWithValue("@intDivision_id", "0");
                            com.Parameters.AddWithValue("@intDepartment_id", "0");
                            com.Parameters.AddWithValue("@intUser_id", "0");
                            com.Parameters.AddWithValue("@MobileNo", values[j]);
                            com.Parameters.AddWithValue("@intSchool_Id", message.intSchool_id);
                            com.Parameters.AddWithValue("@intAcademic_id", message.intAcademic_id);
                            com.Parameters.AddWithValue("@Result", results);
                            com.Parameters.AddWithValue("@Msg_status", message.Status);
                            msg_id = Convert.ToString(dsObj.Tables["Msg_id"].Rows[0]["Msg_id"]);
                            com.Parameters.AddWithValue("@Msg_id", msg_id);

                            int i = com.ExecuteNonQuery();
                            if (i == 1)
                                Sendmsg += 1;
                        }
                        int updaterows = 0;
                        if (message.Status != "Draft")
                        {

                            SqlCommand com3 = new SqlCommand("usp_Message", con);
                            com3.CommandType = CommandType.StoredProcedure;
                            com3.Parameters.AddWithValue("@command", "Update_master");
                            com3.Parameters.AddWithValue("@Msg_id", msg_id);
                            SqlDataAdapter da3 = new SqlDataAdapter(com3);
                            da3.Fill(dsObj, "MessageDetails");
                            for (int j = 0; dsObj.Tables["MessageDetails"].Rows.Count > j; j++)
                            {
                                string Mobile_Number = Convert.ToString(dsObj.Tables["MessageDetails"].Rows[j]["MobileNumber"]);
                                string msg = "";
                                msg = message.message;
                                string URL = "";
                                if (message.MessageLanguage == "0")
                                {
                                    URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=TXT&message=" + msg + "&response=Y";
                                }
                                else
                                {
                                     URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Central%20Model%20school&pass=$b5@ZtX3&senderid=CMSBKP&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                                }
                                
                               // string URL = "http://www.smsjust.com/sms/user/urlsms.php?username=Arohan School&pass=XH9@xy0-&senderid=AROHAN&dest_mobileno=" + Mobile_Number + "&msgtype=UNI&message=" + msg + "&response=Y";
                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                StreamReader sr = new StreamReader(resp.GetResponseStream());
                                string results = sr.ReadToEnd();
                               // string results = "0";
                                string id = Convert.ToString(dsObj.Tables["MessageDetails"].Rows[j]["id"]);
                                String query = "usp_Message";
                                SqlCommand com4 = new SqlCommand(query, con);
                                com4.CommandType = CommandType.StoredProcedure;
                                com4.Parameters.AddWithValue("@command", "Update_Details");
                                com4.Parameters.AddWithValue("@Result", results);
                                com4.Parameters.AddWithValue("@id", id);
                                int i = com4.ExecuteNonQuery();
                                if (i == 1)
                                    updaterows += 1;
                            }
                        }
                        if (message.Status == "Draft")
                        {
                            DataTable ds1 = ds.Tables.Add("SendMessages");
                            ds1.Columns.Add("Result", typeof(Int32));
                            DataRow dr = ds.Tables["SendMessages"].NewRow();
                            dr["Result"] = Sendmsg;
                            ds.Tables["SendMessages"].Rows.InsertAt(dr, 0);
                        }
                        else
                        {
                            DataTable ds1 = ds.Tables.Add("SendMessages");
                            ds1.Columns.Add("Result", typeof(Int32));
                            DataRow dr = ds.Tables["SendMessages"].NewRow();
                            dr["Result"] = updaterows;
                            ds.Tables["SendMessages"].Rows.InsertAt(dr, 0);
                        }
                    }
                    return (ds);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
    }
}
