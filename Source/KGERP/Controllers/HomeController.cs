using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    public class HomeController : Controller
    {
        ERPEntities context = new ERPEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DashboardService _dashboardService;

        public HomeController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [CheckSession]
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated && DateTime.Now > Convert.ToDateTime(MailService.Request))
            {
                return RedirectToAction("Login", "User");

            }


            //var disableBroIns = ConfigurationManager.AppSettings["DisableBrowserInspectKey"];
            //string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            //// Parse the connection string to extract the database name
            //string databaseName = GetDatabaseNameFromConnectionString(connectionString);

            // Set database name in ViewBag or ViewData
            //ViewBag.DisableBrowserInspect = disableBroIns;
            var vendor = _dashboardService.AllCount(Common.GetCompanyId());
            vendor.CompanyId = Common.GetCompanyId();
            return View(vendor);
        }

        
        private string GetDatabaseNameFromConnectionString(string connectionString)
        {
            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                return builder.InitialCatalog;
            }
            catch (ArgumentException ex)
            {
                // Handle the exception appropriately, e.g., log it
                throw new ArgumentException("Invalid connection string format", ex);
            }
        }

        [CheckSession]
        public ActionResult GetChartImage()
        {
            var key = new Chart(width: 300, height: 300)
                .AddTitle("Employee Gender Ratio")
                .AddSeries(
                chartType: "Pie",
                name: "Employee",
                xValue: new[] { "Male", "Female" },
                yValues: new[] { "1305", "118" });

            return File(key.ToWebImage().GetBytes(), "image/jpeg");
        }
        [CheckSession]
        public ActionResult GetBubbleChartImage()
        {
            var key = new Chart(width: 400, height: 300)
           .AddTitle("Employee Chart")
           .AddSeries(
           chartType: "Bubble",
           name: "Employee",
           xValue: new[] { "Ripon", "Shahadot", "Rozy", "R Awal" },
           yValues: new[] { "2", "7", "5", "3" });

            return File(key.ToWebImage().GetBytes(), "image/jpeg");
        }
        [CheckSession]
        public ActionResult About()
        {
            return View();
        }
        [CheckSession]
        public ActionResult Contact()
        {
            return View();
        }

        [CheckSession]
        public ActionResult SendMail()
        {
            var senderEmail = new MailAddress("kgerp19@gmail.com", "KG");
            var receiverEmail = new MailAddress("ripongogi@krishibidgroup.com", "Ripon");
            var ccEmail = new MailAddress("swashraf@krishibidgroup.com", "Md. Asraf");
            var password = "kfl@admin321";
            var subject = "Test Subject";
            var body = "Test Body";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = subject,
                Body = body,
            }
            )

            {
                mess.CC.Add(ccEmail);
                smtp.Send(mess);
            }

            return View();
        }

        public ActionResult DownloadUserManual()
        {
            return View();
        }
        #region// KG ERP User Manual
        public FileResult DownloadUserManualByPDF()
        {
            string filepath = Server.MapPath("/UserManual/KGERPUserManual.pdf");
            byte[] pdfByte = GetBytesFromFile(filepath);
            return File(pdfByte, "application/pdf", "KGERPHRMSUserManual.pdf");
        }

        public FileResult AssetManagementUserManual()
        {
            string filepath = Server.MapPath("/UserManual/AssetManagementUserManual.pdf");
            byte[] pdfByte = GetBytesFromFile(filepath);
            return File(pdfByte, "application/pdf", "AssetManagementUserManual.pdf");
        }

        public FileResult TaskManagementUserManual()
        {
            string filepath = Server.MapPath("/UserManual/TaskManagementUserManual.pdf");
            byte[] pdfByte = GetBytesFromFile(filepath);
            return File(pdfByte, "application/pdf", "TaskManagementUserManual.pdf");
        }

        public byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)
            FileStream fs = null;
            try
            {
                fs = System.IO.File.OpenRead(fullFilePath);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                return bytes;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
        #endregion
        public void EmailNotificationForGMDApprovalPendingList()
        {
            string body = "";
            string ConnStr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            body = "<!DOCTYPE html>";
            body += "<html> <head> <style> ";
            body += "table { border: 0px solid #ddd;   width: 800px; } th, td { text - align: left; font - size:12; border: 0px solid #ddd;  padding: 0px;}";
            body += " tr: nth-child(even){ background-color: #f2f2f2}  th {background-color: #007f3d;  border: 1px solid #ddd;    color: white;}";
            body += " h5 { color: red; } h4 { color: black; }</style></head><body>  ";
            body += "<H4>Dear Concern,</H4>";
            body += "GMD approval pending list below. Please" + "<a href=" + "http://192.168.0.7:90/user/login" + "> click here </a> for details and action.<br/>";

            #region //dsLeave
            //vHrLeaveApprovalPendingList
            string sqlLeave = "Select * from vHrLeaveApprovalPendingList2GMD";
            SqlDataAdapter sdaLeave = new SqlDataAdapter(sqlLeave, ConnStr);
            DataSet dsLeave = new DataSet();
            sdaLeave.Fill(dsLeave);
            if (dsLeave.Tables[0].Rows.Count > 0)
            {
                body += "<br/>";
                body += "<H4><br/>A. Pending List (Leave)</H4>";
                body += "<table>";
                body += "<tr>";
                body += "<th>" + "Name" + "</th>";
                body += "<th>" + "Designation" + "</th>";
                body += "<th>" + "Department" + "</th>";
                body += "<th>" + "ManagerStatus" + "</th>";
                body += "<th>" + "HrAdminStatus" + "</th>";
                body += "</tr>";
                foreach (DataRow drLeave in dsLeave.Tables[0].Rows)
                {
                    body += "<tr>";
                    body += "<td>" + drLeave[0] + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[1]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[2]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[3]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[4]) + "</td>";
                    body += "</tr>";
                }
                body += "</table>";
            }
            else
            {
                body += "<br/><H4>A.Pending List (Leave)<br/>No Available</H4>";
            }
            #endregion
            #region  //dsOnField
            //vHrOnFieldDutyApprovalPendingList
            string sqlOnField = "Select * from vHrOnFieldDutyApprovalPendingList2GMD";
            SqlDataAdapter sdaOnField = new SqlDataAdapter(sqlOnField, ConnStr);
            DataSet dsOnField = new DataSet();
            sdaOnField.Fill(dsOnField);
            if (dsOnField.Tables[0].Rows.Count > 0)
            {
                body += "<H4><br/>B. Pending List (On-field duty)</H4>";
                body += "<table>";
                body += "<tr>";
                body += "<th>" + "Name" + "</th>";
                body += "<th>" + "Designation" + "</th>";
                body += "<th>" + "Department" + "</th>";
                body += "<th>" + "ManagerStatus" + "</th>";
                body += "<th>" + "HrAdminStatus" + "</th>";
                body += "</tr>";
                foreach (DataRow drOnField in dsOnField.Tables[0].Rows)
                {
                    body += "<tr>";
                    body += "<td>" + drOnField[0] + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[1]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[2]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[3]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[4]) + "</td>";
                    body += "</tr>";
                }
                body += "</table>";
            }
            else
            {
                body += "<br/><H4>B. Pending List (On-field duty)<br/>No Available</H4><br/>";
            }
            #endregion
            #region  //dsAttModify

            //vHrAttendanceModifyApprovalPendingList
            string sqlAttModify = "Select * from vHrAttendanceModifyApprovalPendingList2GMD";
            SqlDataAdapter sdaAttModify = new SqlDataAdapter(sqlAttModify, ConnStr);
            DataSet dsAttModify = new DataSet();
            sdaAttModify.Fill(dsAttModify);
            if (dsAttModify.Tables[0].Rows.Count > 0)
            {
                body += " <H4>C. Pending List (Attendance modification)</H4>";
                body += "<table>";
                body += "<tr>";
                body += "<th>" + "Name" + "</th>";
                body += "<th>" + "Designation" + "</th>";
                body += "<th>" + "Department" + "</th>";
                body += "<th>" + "ManagerStatus" + "</th>";
                body += "<th>" + "HrAdminStatus" + "</th>";
                body += "</tr>";
                foreach (DataRow drAttModify in dsAttModify.Tables[0].Rows)
                {
                    body += "<tr>";
                    body += "<td>" + drAttModify[0] + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[1]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[2]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[3]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[4]) + "</td>";
                    body += "</tr>";
                }
                body += "</table>";
            }
            else
            {
                body += "<br/><H4>C. Pending List (Attendance modification)<br/>No Available</H4><br/>";

            }
            #endregion
            body += "</table><br/><H5>[This is system generated emial notification powered by KG ERP IT Team]<b> HelpLine: Cell no. 01700729805 & PBS no. 817<b/></H5></body></html>";
            #region //   mail settings
            MailMessage message = new MailMessage();
            message.From = new MailAddress("kgerp19@gmail.com");
            //can add more recipient
            message.To.Add(new MailAddress("aafzal@krishibidgroup.com"));
            // message.To.Add(new MailAddress("hr@krishibidgroup.com"));
            //add cc 
            string receiverEmail = "aafzal@krishibidgroup.com";
            // string receiverEmail = "hr@krishibidgroup.com";
            string senderEmail = "kgerp19@gmail.com";
            string senderName = "Krishibid Group";
            var password = "kfl@admin321";
            var fromEmail = new MailAddress(senderEmail, senderName);
            var toEmail = new MailAddress("aafzal@krishibidgroup.com");
            var ccEmail = new MailAddress("hod.it@krishibidgroup.com");

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = "ERP: GMD Approval Pending List",
                Body = body,
                IsBodyHtml = true
            }
            )

            {
                try
                {
                    mess.CC.Add(ccEmail);
                    mess.Bcc.Add("swashraf@krishibidgroup.com");
                    smtp.Send(mess);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            #endregion
        }

        public void EmailNotificationForHRApprovalPendingList()
        {
            string body = "";
            string ConnStr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            body = "<!DOCTYPE html>";
            body += "<html> <head> <style> ";
            body += "table { border: 0px solid #ddd;   width: 800px; } th, td { text - align: left; font - size:12; border: 0px solid #ddd;  padding: 0px;}";
            body += " tr: nth-child(even){ background-color: #f2f2f2}  th {background-color: #007f3d;  border: 1px solid #ddd;    color: white;}";
            body += " h5 { color: red; } h4 { color: black; }</style></head><body>  ";
            body += "<H4>Dear Concern,</H4>";
            body += "HR approval pending list below. Please" + "<a href=" + "http://192.168.0.7:90/user/login" + "> click here </a> for details and action.<br/>";

            #region //dsLeave
            //vHrLeaveApprovalPendingList
            string sqlLeave = "Select * from vHrLeaveApprovalPendingList";
            SqlDataAdapter sdaLeave = new SqlDataAdapter(sqlLeave, ConnStr);
            DataSet dsLeave = new DataSet();
            sdaLeave.Fill(dsLeave);
            if (dsLeave.Tables[0].Rows.Count > 0)
            {
                body += "<br/>";
                body += "<H4><br/>A. Pending List (Leave)</H4>";
                body += "<table>";
                body += "<tr>";
                body += "<th>" + "Name" + "</th>";
                body += "<th>" + "Designation" + "</th>";
                body += "<th>" + "Department" + "</th>";
                body += "<th>" + "ManagerStatus" + "</th>";
                body += "<th>" + "HrAdminStatus" + "</th>";
                body += "</tr>";
                foreach (DataRow drLeave in dsLeave.Tables[0].Rows)
                {
                    body += "<tr>";
                    body += "<td>" + drLeave[0] + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[1]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[2]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[3]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drLeave[4]) + "</td>";
                    body += "</tr>";
                }
                body += "</table>";
            }
            else
            {
                body += "<br/><H4>A.Pending List (Leave)<br/>No Available</H4>";
            }
            #endregion
            #region  //dsOnField
            //vHrOnFieldDutyApprovalPendingList
            string sqlOnField = "Select * from vHrOnFieldDutyApprovalPendingList";
            SqlDataAdapter sdaOnField = new SqlDataAdapter(sqlOnField, ConnStr);
            DataSet dsOnField = new DataSet();
            sdaOnField.Fill(dsOnField);
            if (dsOnField.Tables[0].Rows.Count > 0)
            {
                body += "<H4><br/>B. Pending List (On-field duty)</H4>";
                body += "<table>";
                body += "<tr>";
                body += "<th>" + "Name" + "</th>";
                body += "<th>" + "Designation" + "</th>";
                body += "<th>" + "Department" + "</th>";
                body += "<th>" + "ManagerStatus" + "</th>";
                body += "<th>" + "HrAdminStatus" + "</th>";
                body += "</tr>";
                foreach (DataRow drOnField in dsOnField.Tables[0].Rows)
                {
                    body += "<tr>";
                    body += "<td>" + drOnField[0] + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[1]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[2]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[3]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drOnField[4]) + "</td>";
                    body += "</tr>";
                }
                body += "</table>";
            }
            else
            {
                body += "<br/><H4>B. Pending List (On-field duty)<br/>No Available</H4><br/>";
            }
            #endregion
            #region  //dsAttModify

            //vHrAttendanceModifyApprovalPendingList
            string sqlAttModify = "Select * from vHrAttendanceModifyApprovalPendingList";
            SqlDataAdapter sdaAttModify = new SqlDataAdapter(sqlAttModify, ConnStr);
            DataSet dsAttModify = new DataSet();
            sdaAttModify.Fill(dsAttModify);
            if (dsAttModify.Tables[0].Rows.Count > 0)
            {
                body += " <H4>C. Pending List (Attendance modification)</H4>";
                body += "<table>";
                body += "<tr>";
                body += "<th>" + "Name" + "</th>";
                body += "<th>" + "Designation" + "</th>";
                body += "<th>" + "Department" + "</th>";
                body += "<th>" + "ManagerStatus" + "</th>";
                body += "<th>" + "HrAdminStatus" + "</th>";
                body += "</tr>";
                foreach (DataRow drAttModify in dsAttModify.Tables[0].Rows)
                {
                    body += "<tr>";
                    body += "<td>" + drAttModify[0] + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[1]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[2]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[3]) + "</td>";
                    body += "<td>" + string.Format("{0:c}", drAttModify[4]) + "</td>";
                    body += "</tr>";
                }
                body += "</table>";
            }
            else
            {
                body += "<br/><H4>C. Pending List (Attendance modification)<br/>No Available</H4><br/>";

            }
            #endregion
            body += "</table><br/><H5>[This is system generated emial notification powered by KG ERP IT Team]<b> HelpLine: Cell no. 01700729805 & PBS no. 817<b/></H5></body></html>";
            #region //   mail settings
            MailMessage message = new MailMessage();
            message.From = new MailAddress("kgerp19@gmail.com");
            //can add more recipient
            // message.To.Add(new MailAddress("swashraf@krishibidgroup.com"));
            message.To.Add(new MailAddress("hr@krishibidgroup.com"));
            //add cc 
            // string receiverEmail = "swashraf@krishibidgroup.com";
            string receiverEmail = "hr@krishibidgroup.com";
            string senderEmail = "kgerp19@gmail.com";
            string senderName = "Krishibid Group";
            var password = "kfl@admin321";
            var fromEmail = new MailAddress(senderEmail, senderName);
            var toEmail = new MailAddress("hr@krishibidgroup.com");
            var ccEmail = new MailAddress("hod.it@krishibidgroup.com");

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = "ERP: HR Approval Pending List",
                Body = body,
                IsBodyHtml = true
            }
            )

            {
                try
                {
                    mess.CC.Add(ccEmail);
                    mess.Bcc.Add("swashraf@krishibidgroup.com");
                    smtp.Send(mess);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            #endregion
        }
        public void EmployeeAnniversaryCard()
        {

            string htmlMessage = @"<!DOCTYPE html><html><head>
        <meta name=" + "viewport" + " content=" + "width = device - width, initial - scale = 1><style>" +
        ".container { position: relative;text - align: center;color: white;}" +
        ".centered {position: absolute;top: 40 %;left: 53 %;transform: translate(-50 %, -50 %);"
        + "color: red;border: 1px dotted blue;display: inline - block;max - width: 250px;padding: 10px;" +
          "word -break: break-all;}</style></head><body><h2> Dear Concern </h2>" +
        "<p> We plesed to send your Anniversary card:</p><div class=" + "container" + "> <img src = " + "cid:EmbeddedContent_AnniversaryCard" + " alt=" + "Annivarsary" + "/> </div></body></html> ";

            string receiverEmail = "swashraf@krishibidgroup.com";
            // string receiverEmail = "hr@krishibidgroup.com";
            string senderEmail = "kgerp19@gmail.com";
            string senderName = "Krishibid Group";
            var password = "kfl@admin321";
            var fromEmail = new MailAddress(senderEmail, senderName);
            var toEmail = new MailAddress("swashraf@krishibidgroup.com");
            //var ccEmail = new MailAddress("raisul.awal@krishibidgroup.com");

            var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, password)
            };

            //creating a image object   
            string path2 = Server.MapPath("~/Images/Happy_Birthday_Card.jpg");
            Bitmap bitmap = (Bitmap)Image.FromFile(path2);//load the image file
            var fileName = Path.GetFileName(path2);
            var path = Path.Combine(Server.MapPath("~/FileUpload"), fileName);//Save in temporary file

            //draw the image object using a Graphics object    
            Graphics graphicsImage = Graphics.FromImage(bitmap);

            //Set the alignment based on the coordinates       
            StringFormat stringformat = new StringFormat();
            stringformat.Alignment = StringAlignment.Center;
            stringformat.LineAlignment = StringAlignment.Center;

            StringFormat stringformat2 = new StringFormat();
            stringformat2.Alignment = StringAlignment.Center;
            stringformat2.LineAlignment = StringAlignment.Center;

            //Set the font color/format/size etc..      
            System.Drawing.Color StringColor = ColorTranslator.FromHtml("#933eea");//direct color adding    
            System.Drawing.Color StringColor2 = ColorTranslator.FromHtml("#e80c88");//customise color adding 
            System.Drawing.Color smsColor = ColorTranslator.FromHtml("#581845");//customise color adding   
            string Str_TextOnImage = "Happy \nBirth Anniversary \nto ";//Your Text On Image    
            string Str_TextOnImage2 = "Md. Ashraful Islam\n Sr. Sowftware Engineer\nIT Department, Krishibid Group";//Your Text On Image    
            string birthdaySms = "On your birthday (Date: 20 Feb)\nWe wish you good luck. We hope \nthis wonderfull day fill up your \nheart with joy and blessings.";//Your Text On Image    

            graphicsImage.DrawString(Str_TextOnImage, new Font("arial", 16,
            FontStyle.Regular), new SolidBrush(StringColor), new Point(1125, 380),
            stringformat); Response.ContentType = "image/jpeg";

            graphicsImage.DrawString(Str_TextOnImage2, new Font("Helvetica", 12,
            FontStyle.Bold), new SolidBrush(StringColor2), new Point(1125, 600),
            stringformat2); Response.ContentType = "image/jpeg";

            graphicsImage.DrawString(birthdaySms, new Font("arial", 10,
            FontStyle.Bold), new SolidBrush(smsColor), new Point(1125, 799),
            stringformat2); Response.ContentType = "image/jpeg";
            bitmap.Save(path, ImageFormat.Png);
            //bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);//Display image in Browser


            MailMessage msg = new MailMessage(senderEmail, receiverEmail);
            // Create the HTML view
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage, Encoding.UTF8, MediaTypeNames.Text.Html);
            // Create a plain text message for client that don't support HTML
            string textBody = "You must use an e-mail client that supports HTML messages";
            AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(textBody, "<[^>]+?>", string.Empty), Encoding.UTF8, MediaTypeNames.Text.Plain);
            string mediaType = MediaTypeNames.Image.Jpeg;
            LinkedResource img = new LinkedResource(path, mediaType);
            img.ContentId = "EmbeddedContent_AnniversaryCard";

            img.ContentType.MediaType = mediaType;
            img.TransferEncoding = TransferEncoding.Base64;
            img.ContentType.Name = img.ContentId;
            img.ContentLink = new Uri("cid:" + img.ContentId);
            htmlView.LinkedResources.Add(img);
            msg.AlternateViews.Add(plainView);
            msg.AlternateViews.Add(htmlView);

            msg.IsBodyHtml = true;
            msg.Subject = "Anniversary card";
            client.Send(msg);
        }

        public ActionResult HRApprovalPendingList()
        {
            return View();
        }

        //public static void SetObject( string key, object value)
        //{

        //     .SetString(key, JsonConvert.SerializeObject(value));
        //}

        //public static T GetObject<T>(this ISession session, string key)
        //{
        //    var value = session.GetString(key);
        //    return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        //}
        [CheckSession]
        //[OutputCache(Duration = 120, VaryByParam = "none")]
        public PartialViewResult MenuPartial()
        {
 //           try
 //           {
 //               List<CompanyModel> companies = new List<CompanyModel>();
                
 //               if (HttpContext.Session["UserMenu"] != null)
 //               {
 //                  string Menudata = HttpContext.Session["UserMenu"].ToString();
 //                   companies = JsonConvert.DeserializeObject<List<CompanyModel>>(Menudata);
 //               }
 //               else
 //               {
 //                   string userId = System.Web.HttpContext.Current.User.Identity.Name;

 //                   List<CompanyModel> Menus = new List<CompanyModel>();
 //                   List<CompanyModel> SubMenus = new List<CompanyModel>();
 //                   companies = context.Database.
 //                                               SqlQuery<CompanyModel>(@"select CompanyId as Id,CompanyId,
 //                                              ParentId,Name,ShortName,Controller,[Action],[Param],1 as LayerNo
 //                                              from Company
 //                                              where CompanyId in (select CompanyId from CompanyUserMenu where IsActive = 1 AND UserId={0}) 
 //                                               AND IsActive=1
 //                                              order by OrderNo", userId).ToList();

 //                   Menus = context.Database.SqlQuery<CompanyModel>(@"select CompanyMenuId as Id,CompanyId,CompanyId as ParentId,Name,ShortName,Controller,[Action],[Param],2 as LayerNo
 //                                            from CompanyMenu
 //                                            where CompanyMenuId in (select CompanyMenuId  from CompanyUserMenu where UserId={0}  and IsActive = 1) 
 //AND IsActive=1 order by OrderNo",
 //                                                userId).ToList();
 //                   SubMenus = context.Database.SqlQuery<CompanyModel>(@"select CompanySubMenuId as Id,CompanyId,
 //                                       CompanyMenuId as  ParentId,
 //                                       Name,ShortName,Controller,[Action],[Param],3 as LayerNo 
 //                                       from CompanySubMenu
 //                                       where CompanySubMenuId in 
 //                                       (select CompanySubMenuId from CompanyUserMenu where IsActive = 1 and UserId={0} )
 //                                       AND IsActive=1 And IsSideMenu = 1
 //                                       order by OrderNo", userId).ToList();

 //                   foreach (var company in companies)
 //                   {
 //                       company.Company1 = Menus.Where(e => e.ParentId == company.CompanyId).ToList();
 //                       //companyRepository.Database.SqlQuery<CompanyModel>(@"select CompanyMenuId as Id,CompanyId,CompanyId as ParentId,Name,ShortName,Controller,[Action],[Param],2 as LayerNo
 //                       //                     from CompanyMenu
 //                       //                     where CompanyMenuId in (select CompanyMenuId  from CompanyUserMenu where UserId={0} and CompanyId={1} and IsActive = 1) order by OrderNo", userId, company.CompanyId).ToList();

 //                       foreach (var submenu in company.Company1)
 //                       {
 //                           submenu.Company1 = SubMenus.Where(e => e.ParentId == submenu.Id).ToList();

 //                           //companyRepository.Database.SqlQuery<CompanyModel>(@"select CompanySubMenuId as Id,CompanyId,CompanyMenuId as  ParentId,Name,ShortName,Controller,[Action],[Param],3 as LayerNo 
 //                           //                 from CompanySubMenu
 //                           //                 where CompanySubMenuId in (select CompanySubMenuId from CompanyUserMenu where IsActive = 1 and UserId={0} and CompanyMenuId={1} ) order by OrderNo", userId, submenu.Id).ToList();


 //                       }
 //                       company.Company1.RemoveAll(e => e.Company1.Count <= 0);
 //                   }

 //                   companies.RemoveAll(e => e.Company1.Count <= 0);
 //               }
 //               HttpContext.Session.Add("UserMenu", JsonConvert.SerializeObject(companies));
 //               return PartialView("_MenuPartial", companies);
 //           }
 //           catch (DbEntityValidationException e)
 //           {
 //               foreach (var eve in e.EntityValidationErrors)
 //               {
 //                   Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
 //                   foreach (var ve in eve.ValidationErrors)
 //                   {
 //                       Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
 //                   }
 //               }
 //               throw;

 //           }

 //           catch (Exception ex)
 //           {
 //               logger.Error(ex);
 //               return PartialView(RedirectToAction("Login"));
 //           }

            return PartialView("_MenuPartial");
        }



        //Menu management By Ripon
        // [CheckSession]
        //[OutputCache(Duration = 120, VaryByParam = "none")]
        //public PartialViewResult MenuPartial()
        //{
        //    try
        //    {
        //        List<MenuModel> menus = new List<MenuModel>();
        //        string userId = Session["UserName"].ToString();
        //        var userMenus = userMenuRepository.UserMenus.Include("Menu").Include("SubMenu").Where(x => x.UserId == userId && x.SubMenu.IsActive).AsQueryable();


        //        foreach (var item in userMenus)
        //        {
        //            MenuModel menuModel = new MenuModel();
        //            menuModel.ParentId = item.Menu.ParentId;
        //            menuModel.UserMenuId = item.UserMenuId;
        //            menuModel.MenuId = item.Menu.MenuId;
        //            menuModel.MenuName = item.Menu.Name;
        //            menuModel.SubMenuId = item.SubMenuId;
        //            menuModel.SubMenuName = item.SubMenu.Name;
        //            menuModel.Controller = item.SubMenu.Controller;
        //            menuModel.Action = item.SubMenu.Action;
        //            if (!string.IsNullOrEmpty(item.SubMenu.Param))
        //            {
        //                menuModel.Link = "/" + item.SubMenu.Controller + "/" + item.SubMenu.Action + "?companyId=" + item.Menu.CompanyId.ToString() + item.SubMenu.Param.ToString();
        //            }
        //            else
        //            {
        //                menuModel.Link = "/" + item.SubMenu.Controller + "/" + item.SubMenu.Action + "?companyId=" + item.Menu.CompanyId.ToString();
        //            }

        //            menuModel.UserId = item.UserId;
        //            menuModel.IsView = item.IsView;
        //            menuModel.IsAdd = item.IsAdd;
        //            menuModel.IsUpdate = item.IsUpdate;
        //            menuModel.IsDelete = item.IsDelete;
        //            menuModel.MenuOrder = item.Menu.OrderNo;
        //            menuModel.SubMenuOrder = item.SubMenu.OrderNo;
        //            menus.Add(menuModel);
        //        }


        //        return PartialView("_MenuPartial", menus);
        //    }
        //    catch (Exception ex)
        //    {
        //        return PartialView(RedirectToAction("Login"));
        //    }
        //}

        [CheckSession]
        public ActionResult SendMailForPendingMovementListToHRandGMD()
        {
            DataTable dtGMD = new DataTable();
            DataTable dtHR = new DataTable();
            dtGMD = GetGMDMovementPendingList();
            dtHR = GetHRMovementPendingList();
            if (dtGMD.Rows.Count > 0)
            {
                ViewBag.GMDMovementPendingList = GMDMovementPendingList(dtGMD);
            }
            if (dtHR.Rows.Count > 0)
            {
                ViewBag.HRMovementPendingList = HRMovementPendingList(dtHR);
            }
            return View();
        }

        public string GMDMovementPendingList(DataTable dt)
        {
            string htmlStr = "<table width='100%' align='center' cellpadding='2' cellspacing='2' border='0' bgcolor='#F0F0F0'> ";
            //DataTable dt = new DataTable();
            //dt = GetGMDMovementPendingList();
            string style = "\"border-bottom:1pt solid #F3F3F3;background-color: #F5F5F5;\"";
            htmlStr += "<tr  style=" + style + ">" +
                   "<td>Name</td>" +
                   "<td>Designation </td>" +
                   "<td>Department </td>" +
                   "<td>ManagerStatus </td>" +
                   "<td>HrAdminStatus </td>" +
                   "<td>MovementType </td></tr>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                htmlStr += "<tr  style=" + style + ">" +
                "<td>" + dt.Rows[i]["Name"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["Designation"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["Department"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["ManagerStatus"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["HrAdminStatus"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["MovementType"].ToString() + "</td></tr>";
            }

            return htmlStr += "</table>";
        }

        private DataTable GetGMDMovementPendingList()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("Select * from vGMDMovementPendingList", con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public string HRMovementPendingList(DataTable dt)
        {
            string htmlStr = "<table width='100%' align='center' cellpadding='2' cellspacing='2' border='0' bgcolor='#F0F0F0'> ";
            //DataTable dt = new DataTable();
            //dt = GetHRMovementPendingList();
            string style = "\"border-bottom:1pt solid #F3F3F3;background-color: #F5F5F5;\"";
            htmlStr += "<tr  style=" + style + ">" +
                   "<td>Name</td>" +
                   "<td>Designation </td>" +
                   "<td>Department </td>" +
                   "<td>ManagerStatus </td>" +
                   "<td>HrAdminStatus </td>" +
                   "<td>MovementType </td></tr>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                htmlStr += "<tr  style=" + style + ">" +
                "<td>" + dt.Rows[i]["Name"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["Designation"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["Department"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["ManagerStatus"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["HrAdminStatus"].ToString() + "</td>" +
                "<td>" + dt.Rows[i]["MovementType"].ToString() + "</td></tr>";
            }

            return htmlStr += "</table>";
        }

        private DataTable GetHRMovementPendingList()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("Select * from vHRMovementPendingList", con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }


        public ActionResult CreateVoucher()
        {
            ///-------------------Feed Accounting Purpose--------------------------        
            //1. FG Production
            //2. FG Sale
            //3. FG Sale Return
            //4. FG Purchase
            //5. FG Conversion
            //6. RM Purchase
            //7. RM Sale
            //8. RM Adjustment

            //int companyId = 8;
            //DateTime beginDate = Convert.ToDateTime("01/07/2021");
            //DateTime endDate = Convert.ToDateTime("31/07/2021");
            //////262
            //List<RequisitionItem> requisitionItems = repository.RequisitionItems.Include(x => x.Requisition).Where(x => x.Requisition.CompanyId == companyId && x.IssueDate >= beginDate && x.IssueDate <= endDate).ToList();
            //foreach (var item in requisitionItems)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateFGProductionVoucher {0},{1}", companyId, item.RequisitionItemId);
            //}
            ////358
            //List<OrderDeliver> orderDelivers = repository.OrderDelivers.Where(x => x.CompanyId == companyId && x.ProductType == "F" && x.DeliveryDate >= beginDate && x.DeliveryDate <= endDate).ToList();
            //foreach (var deliver in orderDelivers)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateFGSaleVoucher {0},{1}", companyId, deliver.OrderDeliverId);
            //}
            //////5
            //List<SaleReturn> saleReturns = repository.SaleReturns.Where(x => x.CompanyId == companyId && x.ProductType == "F" && x.ReturnDate >= beginDate && x.ReturnDate <= endDate).ToList();
            //foreach (var saleReturn in saleReturns)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateFGSaleReturnVoucher {0},{1}", companyId, saleReturn.SaleReturnId);
            //}
            ////2
            //List<Store> feedPurchases = repository.Stores.Where(x => x.CompanyId == companyId && x.Type == "F" && x.VendorId > 0 && x.ReceivedDate >= beginDate && x.ReceivedDate <= endDate).ToList();
            //foreach (var feedPurchase in feedPurchases)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateFGPurchaseVoucher {0},{1}", companyId, feedPurchase.StoreId);
            //}
            ////20
            //List<ConvertedProduct> convertedProducts = repository.ConvertedProducts.Where(x => x.CompanyId == companyId && x.ApprovalDate >= beginDate && x.ApprovalDate <= endDate).ToList();
            //foreach (var convertedProduct in convertedProducts)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateFGConversionVoucher {0},{1}", companyId, convertedProduct.ConvertedProductId);
            //}
            ////130
            //List<MaterialReceive> rawMaterialPurchases = repository.MaterialReceives.Where(x => x.CompanyId == companyId && x.MaterialType == "R" && x.ReceivedDate >= beginDate && x.ReceivedDate <= endDate).ToList();
            //foreach (var rawMaterialPurchase in rawMaterialPurchases)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateRMPurchaseVoucher {0},{1}", companyId, rawMaterialPurchase.MaterialReceiveId);
            //}
            ////5
            //List<OrderDeliver> rawMaterialSales = repository.OrderDelivers.Where(x => x.CompanyId == companyId && x.ProductType == "R" && x.DeliveryDate >= beginDate && x.DeliveryDate <= endDate).ToList();
            //foreach (var rawMaterialSale in rawMaterialSales)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateRMSaleVoucher {0},{1}", companyId, rawMaterialSale.OrderDeliverId);
            //}
            ////0
            //List<StockAdjust> stockAdjusts = repository.StockAdjusts.Where(x => x.CompanyId == companyId && x.AdjustDate >= beginDate && x.AdjustDate <= endDate).ToList();
            //foreach (var stockAdjust in stockAdjusts)
            //{
            //    repository.Database.ExecuteSqlCommand("EXEC AccountingFeedCreateRMStockAdjustmentVoucher {0},{1}", companyId, stockAdjust.StockAdjustId);
            //}

            return RedirectToAction("Index");
        }


    }
}