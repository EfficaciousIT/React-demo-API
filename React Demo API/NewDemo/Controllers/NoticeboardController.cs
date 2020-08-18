using NewDemo.Database;
using NewDemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace NewDemo.Controllers
{
    [CustomeAuthenticationFilter]
    public class NoticeboardController : ApiController
    {
        Database.DB record = new Database.DB();
        public DataSet Get(string command, string intUserType_id, string intSchool_id, int pageindex, string filterBy)
        {
            Noticeboard noticeboard = new Noticeboard();
            noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
            noticeboard.intSchool_id = Convert.ToInt32(intSchool_id);
            noticeboard.pageindex = Convert.ToInt32(pageindex);
            noticeboard.filterBy = filterBy;
            DataSet ds = record.NoticeboardDetails(command, noticeboard);
            return ds;
        }
        public DataSet get(string command, string intUserType_id, string intNotice_id)
        {
            Noticeboard noticeboard = new Noticeboard();
            noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
            noticeboard.intNotice_id = Convert.ToInt32(intNotice_id);
            DataSet ds = record.NoticeboardDetails(command, noticeboard);
            return ds;
        }
        public DataSet Get(string command, string intUserType_id, string intStandard_id, string intDepartment_id, string intSchool_id, int pageindex)
        {
            Noticeboard noticeboard = new Noticeboard();
            noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
            noticeboard.intStandard_id = intStandard_id;
            noticeboard.intDepartment_id = intDepartment_id;
            noticeboard.intSchool_id = Convert.ToInt32(intSchool_id);
            noticeboard.pageindex = Convert.ToInt32(pageindex);
            DataSet ds = record.NoticeboardDetails(command, noticeboard);
            return ds;
        }

        public DataSet Get(string command, string intUserType_id, string intDepartment_id, string intSchool_id,int pageindex,string filterBy)
        {
            Noticeboard noticeboard = new Noticeboard();
            noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
            noticeboard.intDepartment_id = intDepartment_id;
            noticeboard.intSchool_id = Convert.ToInt32(intSchool_id);
            noticeboard.pageindex = Convert.ToInt32(pageindex);
            noticeboard.filterBy = filterBy;
            DataSet ds = record.NoticeboardDetails(command, noticeboard);
            return ds;
        }

        //public HttpResponseMessage Post(string command, string intUserType_id, string intSchool_id, string[] intStandard_id, string[] intdepartment_id, string intUser_id, string dtIssue_date, string dtEnd_date, string vchSubject, string vchNotice, string intInserted_by, string insert_IP, string intAcademic_id, string[] intDivision_id)
        //{
        //    try
        //    { 
        //    Noticeboard noticeboard = new Noticeboard();
        //    noticeboard.intSchool_id = Convert.ToInt32(intSchool_id);
        //    noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
        //    noticeboard.intStandard_id = intStandard_id;
        //    noticeboard.intDivision_id = intDivision_id;
        //    noticeboard.intDepartment_id = intdepartment_id;
        //    noticeboard.intUser_id = Convert.ToInt32(intUser_id);
        //    noticeboard.dtIssue_date = Convert.ToString(dtIssue_date);
        //    noticeboard.dtEnd_date = Convert.ToString(dtEnd_date);
        //    noticeboard.vchSubject = Convert.ToString(vchSubject);
        //    noticeboard.vchNotice = Convert.ToString(vchNotice);
        //    noticeboard.intInserted_by = Convert.ToInt32(intInserted_by);
        //    noticeboard.InsertIP = Convert.ToString(insert_IP);
        //    noticeboard.intAcademic_id = Convert.ToInt32(intAcademic_id);
        //    DataSet ds = record.AddNoticeboard(command, noticeboard);
        //    var message = Request.CreateResponse(HttpStatusCode.Created);
        //    return message;
        //    }
        //    // return ds;
        //     catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
        //    }
        //}

        //[System.Web.Http.ActionName("MyMethod")]
        //[System.Web.Http.HttpPost]
        //public HttpResponseMessage MyMethod([FromBody] Noticeboard objNoticeboard)
        //{
        //    try
        //    {
        //        //Noticeboard noticeboard = new Noticeboard();
        //        //noticeboard.intSchool_id = intSchool_id;
        //        //noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
        //        //noticeboard.intStandard_id = intStandard_id;
        //        //noticeboard.intDivision_id = intDivision_id;
        //        //noticeboard.intDepartment_id = intdepartment_id;
        //        //noticeboard.intUser_id = Convert.ToInt32(intUser_id);
        //        //noticeboard.dtIssue_date = Convert.ToString(dtIssue_date);
        //        //noticeboard.dtEnd_date = Convert.ToString(dtEnd_date);
        //        //noticeboard.vchSubject = Convert.ToString(vchSubject);
        //        //noticeboard.vchNotice = Convert.ToString(vchNotice);
        //        //noticeboard.intInserted_by = Convert.ToInt32(intInserted_by);
        //        //noticeboard.InsertIP = Convert.ToString(insert_IP);
        //        //noticeboard.intAcademic_id = Convert.ToInt32(intAcademic_id);
        //        //DataSet ds = record.AddNoticeboard(command, noticeboard);
        //        DataSet ds = record.AddNoticeboard("insert", objNoticeboard);
        //        var message = Request.CreateResponse(HttpStatusCode.Created,"Record Successfully inserted");
        //        return message;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
        //    }
        //}
        //[System.Web.Http.ActionName("EditNoticeboard")]
        //[System.Web.Http.HttpPost]
        //public HttpResponseMessage EditNoticeboard(string intNotice_id, [FromBody] Noticeboard objNoticeboard)
        //{
        //    try
        //    {
        //        string NoticeId = intNotice_id;
        //        DataSet ds = record.EditNoticeboard(NoticeId, "EditNoticeboard", objNoticeboard);
        //        var message = Request.CreateResponse(HttpStatusCode.Created,"Record is updated");
        //        return message;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
        //    }
        //}

        public DataSet Get(string command, string intSchool_id, int intNotice_id, string intUserType_id)
        {
            Noticeboard noticeboard = new Noticeboard();
            noticeboard.intSchool_id = Convert.ToInt32(intSchool_id);
            noticeboard.intNotice_id = Convert.ToInt32(intNotice_id);
            noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
            DataSet ds = record.NoticeboardDetails(command, noticeboard);
            return ds;
        }

        //[System.Web.Http.HttpPost]
        ////[System.Web.Http.Route("api/ Noticeboard/AddNoticeboard")]
        //public async Task<String> AddNoticeboard(string intUserType_id, string intStandard_id, string intDepartment_id, string dtIssue_date, string dtEnd_date, string vchSubject, string vchNotice, string intInserted_by, string InsertIP, string intSchool_id, string intAcademic_id)
        //{
        //    Noticeboard noticeboard = new Noticeboard();
        //    noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
        //    noticeboard.intStandard_id = Convert.ToString(intStandard_id);
        //    noticeboard.intDepartment_id = Convert.ToString(intDepartment_id);
        //    //noticeboard.intTeacher_id = Convert.ToInt32(intTeacher_id);
        //    noticeboard.dtIssue_date = Convert.ToDateTime(dtIssue_date).ToString("dd/MM/yyyy").Replace("-", "/");
        //    noticeboard.dtEnd_date = Convert.ToDateTime(dtEnd_date).ToString("dd/MM/yyyy").Replace("-", "/");
        //    noticeboard.vchSubject = Convert.ToString(vchSubject);
        //    noticeboard.vchNotice = Convert.ToString(vchNotice);
        //    noticeboard.intInserted_by = Convert.ToInt32(intInserted_by);
        //    noticeboard.InsertIP = Convert.ToString(InsertIP);
        //    noticeboard.intSchool_id = Convert.ToInt32(intSchool_id);
        //    noticeboard.intAcademic_id = Convert.ToInt32(intAcademic_id);

        //    var ctx = HttpContext.Current;
        //    var root = ctx.Server.MapPath("~/Images");

        //    var provider = new MultipartFormDataStreamProvider(root);

        //    try
        //    {
        //        string name = "";
        //        string loacalFileName = "";
        //        noticeboard.ImageName = Convert.ToString(name);
        //        await Request.Content.
        //                 ReadAsMultipartAsync(provider);

        //        foreach (var file in provider.FileData)
        //        {
        //            name = file.Headers.ContentDisposition.FileName;
        //            if (file.Headers.ContentDisposition.FileName != "\"\"")
        //            {
        //                // Remove double quotes from string.
        //                name = name.Trim('"');

        //                noticeboard.ImageName = Convert.ToString(name);

        //                loacalFileName = file.LocalFileName;
        //                var filePath = Path.Combine(root, name);

        //                File.Move(loacalFileName, filePath);
        //            }
        //        }

        //        DataSet ds = record.AddNoticeboard("insert", noticeboard);
        //        string message = ("Record inserted Successfully..");
        //        return message;
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error:{ex.Message}";
        //    }
        //}

        //[System.Web.Http.HttpPost]
        //[System.Web.Http.Route("api/Noticeboard/EditNoticeboard")]
        //public async Task<String> EditNoticeboard(string intNotice_id, string intUserType_id, string intStandard_id, string intDepartment_id, string dtIssue_date, string dtEnd_date, string vchSubject, string vchNotice, string intInserted_by, string InsertIP)
        //{
        //    Noticeboard noticeboard = new Noticeboard();
        //    noticeboard.intNotice_id = Convert.ToInt32(intNotice_id);
        //    noticeboard.intUserType_id = Convert.ToInt32(intUserType_id);
        //    noticeboard.intStandard_id = Convert.ToString(intStandard_id);
        //    noticeboard.intDepartment_id = Convert.ToString(intDepartment_id);
        //    //noticeboard.intTeacher_id = Convert.ToInt32(intTeacher_id);
        //    noticeboard.dtIssue_date = Convert.ToDateTime(dtIssue_date).ToString("dd/MM/yyyy").Replace("-", "/");
        //    noticeboard.dtEnd_date = Convert.ToDateTime(dtEnd_date).ToString("dd/MM/yyyy").Replace("-", "/");
        //    noticeboard.vchSubject = Convert.ToString(vchSubject);
        //    noticeboard.vchNotice = Convert.ToString(vchNotice);
        //    noticeboard.intInserted_by = Convert.ToInt32(intInserted_by);
        //    noticeboard.InsertIP = Convert.ToString(InsertIP);
        //    //noticeboard.intSchool_id = Convert.ToInt32(intSchool_id);
        //    //noticeboard.intAcademic_id = Convert.ToInt32(intAcademic_id);

        //    var ctx = HttpContext.Current;
        //    var root = ctx.Server.MapPath("~/Images");

        //    var provider = new MultipartFormDataStreamProvider(root);

        //    try
        //    {
        //        string name = "";
        //        string loacalFileName = "";
        //        noticeboard.ImageName = Convert.ToString(name);
        //        await Request.Content.
        //                 ReadAsMultipartAsync(provider);

        //        foreach (var file in provider.FileData)
        //        {
        //            name = file.Headers.ContentDisposition.FileName;
        //            if (file.Headers.ContentDisposition.FileName != "\"\"")
        //            {
        //                // Remove double quotes from string.
        //                name = name.Trim('"');

        //                noticeboard.ImageName = Convert.ToString(name);

        //                loacalFileName = file.LocalFileName;
        //                var filePath = Path.Combine(root, name);

        //                File.Move(loacalFileName, filePath);
        //            }
        //        }

        //        DataSet ds = record.EditNoticeboard("insert", noticeboard);
        //        string message = ("Record updated Successfully..");
        //        return message;
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error:{ex.Message}";
        //    }
        //}

        [System.Web.Http.HttpPost]
        //[System.Web.Http.Route("api/ Noticeboard/AddNoticeboard")]
        //public async Task<String> AddNoticeboard(string intUserType_id, string intStandard_id, string intDepartment_id, string dtIssue_date, string dtEnd_date, string vchSubject, string vchNotice, string intInserted_by, string InsertIP, string intSchool_id, string intAcademic_id,string image)
        //{
        public HttpResponseMessage AddNoticeboard(Noticeboard noticeboard)
        {
            try
            {
                String imgName = "";
                if (noticeboard.image != "")
                {
                    imgName = "img" + DateTime.Now.ToShortDateString().Replace("-", "") + DateTime.Now.ToShortTimeString().Replace(":", "");

                    SaveImage(noticeboard.image, imgName);
                    imgName += ".jpg";
                }
                noticeboard.ImageName = imgName;

                String pdfName = "";
                if (noticeboard.pdf != "")
                {
                    pdfName = "pdf" + DateTime.Now.ToShortDateString().Replace("-", "") + DateTime.Now.ToShortTimeString().Replace(":", "");

                    SavePDF(noticeboard.pdf, pdfName);
                    pdfName += ".pdf";
                }
                noticeboard.PDFName = pdfName;

                DataSet ds = record.AddNoticeboard("insert", noticeboard);
                //string message = ("Record inserted Successfully..");
                var message = Request.CreateResponse(HttpStatusCode.Created, "Record inserted Successfully..");
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Noticeboard/EditNoticeboard")]
        public HttpResponseMessage EditNoticeboard(Noticeboard noticeboard)
        {
            try
            {
                if (noticeboard.ImageName == "")
                {
                    String imgName = "";
                    if (noticeboard.image != "")
                    {
                        imgName = "img" + DateTime.Now.ToShortDateString().Replace("-", "") + DateTime.Now.ToShortTimeString().Replace(":", "");

                        SaveImage(noticeboard.image, imgName);
                      imgName +=".jpg";
                    }
                    noticeboard.ImageName = imgName;
                }
                if(noticeboard.PDFName == "")
                {
                    String pdfName = "";
                    if (noticeboard.pdf != "")
                    {
                        pdfName = "pdf" + DateTime.Now.ToShortDateString().Replace("-", "") + DateTime.Now.ToShortTimeString().Replace(":", "");

                        SavePDF(noticeboard.pdf, pdfName);
                        pdfName += ".pdf";
                    }
                    noticeboard.PDFName = pdfName;
                }
                DataSet ds = record.EditNoticeboard("insert", noticeboard);
                //string message = ("Record updated Successfully..");
                var message = Request.CreateResponse(HttpStatusCode.Created, "Record updated Successfully..");
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public bool SaveImage(string ImgStr, string ImgName)
        {
            String path = HttpContext.Current.Server.MapPath("~/Images"); //Path
          
            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            string imageName = ImgName + ".jpg";

            //set the image path
            string imgPath = Path.Combine(path, imageName);

            byte[] imageBytes = Convert.FromBase64String(ImgStr);

            File.WriteAllBytes(imgPath, imageBytes);

            return true;
        }

        public bool SavePDF(string PdfStr, string PdfName)
        {
            String path = HttpContext.Current.Server.MapPath("~/PDF"); //Path

            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            string PDFName = PdfName + ".pdf";

            //set the image path
            string PDFPath = Path.Combine(path, PDFName);

            byte[] pdfBytes = Convert.FromBase64String(PdfStr);

            File.WriteAllBytes(PDFPath, pdfBytes);

            return true;
        }

        //public HttpResponseMessage Delete(string command, int intNotice_id, string intUser_id, string ip)
        //{
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage Delete(string command, int intNotice_id, string intUser_id, string ip)
        {
            try
            {
                Noticeboard noticeboard = new Noticeboard();
                noticeboard.intNotice_id = intNotice_id;
                noticeboard.intUser_id = Convert.ToInt32(intUser_id);
                noticeboard.InsertIP = ip;
                DataSet ds = record.DeleteNotice(command, noticeboard);
                var message = Request.CreateResponse(HttpStatusCode.Created, "Record Deleted Successfully..");
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }

        }
    }
}