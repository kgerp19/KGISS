using KGERP.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class AttendaceProcessController : Controller
    {
        // GET: AttendaceProcess
        IAttendenceService attendenceService = new AttendenceService();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        public ActionResult AttendanceProcess()
        {
            return View();
        }
        
        public ActionResult PrecessAttendenceInFinalTable(DateTime AttendenceDate)
        {
            //var date = DateTime.Parse(AttendenceDate);
            var result = attendenceService.PrecessAttendenceInFinalTable(AttendenceDate);
            if (result)
            {
                //ViewBag.Process = "Process Successfull";
                ViewBag.Process = true;
            }
            else
            {
                //ViewBag.Process = "Already Up to Date";
                ViewBag.Process = false;
            }
            return View("AttendanceProcess");
        }
        //[HttpPost]
        //public ActionResult AttendanceProcess(string data)
        //{
        //    InsetData();
        //    return View();
        //}

        //public void InsetData()
        //{ 
        //    //SqlBulkCopy bulkCopy = new SqlBulkCopy(@"Data Source=DESKTOP-JSUM49E;Initial Catalog=TestDb;User ID=sa;Password=sa;", SqlBulkCopyOptions.TableLock);
        //    //bulkCopy.DestinationTableName = "AttendanceData";
        //    //bulkCopy.WriteToServer(Text2Table());   
        //    string ConString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        //    using (SqlConnection destinationConnection = new SqlConnection(ConString))
        //    {
        //        destinationConnection.Open();
        //        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
        //        {
        //            bulkCopy.DestinationTableName = "AttendanceData";
        //            try
        //            {

        //                bulkCopy.WriteToServer(Text2Table());
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }
        //        destinationConnection.Close();
        //    }
        //}

        //public DataTable Text2Table()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Clear();
        //    dt.Columns.Add("EmpCardNo");
        //    dt.Columns.Add("Date");
        //    dt.Columns.Add("PanchTime");
        //    StreamReader sr = new StreamReader(@"D:\timesheet.txt");
        //    string input;


        //    while ((input = sr.ReadLine()) != null)
        //    {
        //        string[] values = input.Split(new char[] { ' ' });
        //        DataRow dr = dt.NewRow();
        //        dr["EmpCardNo"] = values[0];
        //        dr["Date"] = values[1];
        //        dr["PanchTime"] = values[2];
        //        dt.Rows.Add(dr);
        //    }


        //    sr.Close();

        //    return dt;
        //}

        /////////////////
        
        [HttpPost]
        public ActionResult AttendanceProcess(AttendaceProcessModel file)
        {
            try
            {
                if (file.TextFile != null && file.TextFile.ContentLength > 0)
                {
                    var path = string.Empty;
                    var fileName = Path.GetFileName(file.TextFile.FileName);
                    string folder = Server.MapPath(string.Format("~/{0}/", "FileUpload"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                        path = Path.Combine(Server.MapPath("~/FileUpload"), fileName);
                        file.TextFile.SaveAs(path);
                    }
                    else
                    {
                        path = Path.Combine(Server.MapPath("~/FileUpload"), fileName);
                        file.TextFile.SaveAs(path);
                    }


                    bool status = false;
                    string fileName1 = Path.GetPathRoot(file.TextFile.FileName);
                    string extention = Path.GetExtension(file.TextFile.FileName).ToLower();
                    if (extention == ".txt" || extention == ".xls")
                    {
                        status = InsetData(path.ToString());
                    }
                    if (status == true)
                    {
                        ViewBag.status = status;
                        if ((System.IO.File.Exists(path.ToString())))
                        {
                            System.IO.File.Delete(path.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Attendance Data Upload Error: " + ex);
                return View();
            }
            return View();
        }
        
        public bool InsetData(string filePath)
        {
            //SqlBulkCopy bulkCopy = new SqlBulkCopy(@"Data Source=DESKTOP-JSUM49E;Initial Catalog=TestDb;User ID=sa;Password=sa;", SqlBulkCopyOptions.TableLock);
            //bulkCopy.DestinationTableName = "AttendanceData";
            //bulkCopy.WriteToServer(Text2Table());  
            bool status = false;
            string ConString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection destinationConnection = new SqlConnection(ConString))
            {
                destinationConnection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    bulkCopy.DestinationTableName = "RawAttendence";
                    try
                    {
                        bulkCopy.WriteToServer(Text2Table(filePath));
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Attendance Data Upload Error bulkCopy: " + ex);
                    }
                }
                destinationConnection.Close();
            }
            return status;
        }
        
        public DataTable Text2Table(string path)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Id");
            dt.Columns.Add("EmpCardNo");
            dt.Columns.Add("Date");
            dt.Columns.Add("PanchTime");
            StreamReader sr = new StreamReader(path);

            string input;

            while ((input = sr.ReadLine()) != null)
            {
                string[] values = input.Split(new char[] { ' ' });
                DataRow dr = dt.NewRow();
                dr["Id"] = values[0];
                dr["EmpCardNo"] = values[1];
                dr["Date"] = values[2];
                dr["PanchTime"] = values[3];
                dt.Rows.Add(dr);
            }
            sr.Close();
            return dt;
        }

    }
}
