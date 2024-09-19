using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Controllers
{
    [CheckSession]
    public class MemberBasePFSummaryController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ERPEntities db = new ERPEntities();
        IEmployeeService employeeService = new EmployeeService(new ERPEntities());
        IMemberBasePFSummaryService memberBasePFSummaryService = new MemberBasePFSummaryService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
       
        public ActionResult Index()
        {
            string employeeId = Session["UserName"].ToString();
            ViewBag.Employee = String.Format("[Name: {0}   Employee ID: {1}]", Session["EmployeeName"].ToString(), Session["UserName"].ToString());
            List<MemberBasePFSummaryModel> pfData = memberBasePFSummaryService.GetMemberBasePFSummaryByEmployeeId(employeeId);
            if (pfData.Count > 0)
            {
                MemberBasePFSummaryModel data = memberBasePFSummaryService.GetPFLastMonthUpdatedByEmployeeId(employeeId);
                if (data != null && data.PFDate != null)
                {
                    ViewBag.LastMonth = data.PFDate.ToString("dd-MMM-yyyy");
                }
            }
            return View(pfData);
        }

       
        public ActionResult GetPFDataByEmployeeId(string employeeId)
        {
            List<MemberBasePFSummaryModel> pfData = new List<MemberBasePFSummaryModel>();
            if (!string.IsNullOrEmpty(employeeId))
            {
                EmployeeModel model = employeeService.GetEmployeeByKGID(employeeId);
                if (model != null)
                {
                    ViewBag.Employees = String.Format("[Name: {0}   Employee ID: {1}]", model.Name, model.EmployeeId);
                }

                pfData = memberBasePFSummaryService.GetMemberBasePFSummaryByEmployeeId(employeeId);
                if (pfData.Count > 0)
                {
                    MemberBasePFSummaryModel data = memberBasePFSummaryService.GetPFLastMonthUpdatedByEmployeeId(employeeId);
                    if (data != null)
                    {
                        ViewBag.LastMonth = data.PFDate.ToString("dd-MMM-yyyy");
                    }
                }
                return View(pfData);
            }
            else
            {
                return View(pfData);
            }
        }

       
        public ActionResult GetDetialsPFDataByEmployeeId(int? Page_No, string employeeId)
        {

            List<MemberBasePFSummaryModel> pfData = new List<MemberBasePFSummaryModel>();
            if (!string.IsNullOrEmpty(employeeId))
            {
                EmployeeModel model = employeeService.GetEmployeeByKGID(employeeId);
                if (model != null)
                {
                    ViewBag.Employees = String.Format("[Name: {0}   Employee ID: {1}]", model.Name, model.EmployeeId);
                    ViewBag.EmployeeId = model.EmployeeId;
                }
                pfData = memberBasePFSummaryService.GetPFDetialsByEmployeeId(employeeId);
                int Size_Of_Page = 5000;
                int No_Of_Page = (Page_No ?? 1);
                return View(pfData.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(pfData);
            }
        }


       
        public IList<MemberBasePFSummaryModel> GetEmployeePFDetialsList()
        {
            string employeeId = Session["UserName"].ToString();
            List<MemberBasePFSummaryModel> pfData = memberBasePFSummaryService.GetPFDetialsByEmployeeId(employeeId);
            return pfData;
        }
       
        public ActionResult GetPFDetialsByEmployeeId(int? Page_No, string searchText)
        {
            string employeeId = string.Empty;
            if (!string.IsNullOrEmpty(Session["UserName"].ToString()))
            {
                ViewBag.Employee = String.Format("[Name: {0}   Employee ID: {1}]", Session["EmployeeName"].ToString(), Session["UserName"].ToString());

                employeeId = Session["UserName"].ToString();
                List<MemberBasePFSummaryModel> pfData = memberBasePFSummaryService.GetPFDetialsByEmployeeId(employeeId);
                int Size_Of_Page = 50;
                int No_Of_Page = (Page_No ?? 1);
                return View(pfData.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View();
            }
        }

        public ActionResult ExportPFDetailsToExcel()
        {
            var gv = new GridView();
            gv.DataSource = this.GetEmployeePFDetialsList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + Session["EmployeeName"].ToString() + "_PFDetails.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View();
        }

        #region // Upload Data
       
        public ActionResult UploadPFData(int? companyId)
        {
            int comId;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
                comId = (int)Session["CompanyId"];
            }


            return View();
        }

       
        [HttpPost]
        public ActionResult UploadPFData(MemberBasePFSummaryModel file)
        {
            try
            {
                string message = UploadExcelFile(file);
                ViewBag.ExcelIssues = message;
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private string UploadExcelFile(MemberBasePFSummaryModel file)
        {
            string ValidDisplayMessage = "";
            // pnlValidUpData.Visible = false;
            // pnlErrorData.Visible = false;

            if (file.ExcelFile != null && file.ExcelFile.ContentLength > 0)
            {
                OleDbConnection conn = new OleDbConnection();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter da = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                string connString = "";
                string strFileName = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                string strFileType = Path.GetExtension(file.ExcelFile.FileName).ToString().ToLower();
                var fileName = Path.GetFileName(file.ExcelFile.FileName);
                var path = Path.Combine(Server.MapPath("~/FileUpload"), fileName);

                if (strFileType == ".xls" || strFileType == ".xlsx")
                {
                    try
                    {
                        file.ExcelFile.SaveAs(path);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
                else
                {
                    return "";
                }
                if (strFileType.Trim() == ".xls")
                {
                    connString = string.Format(ConfigurationManager.ConnectionStrings["Excel03ConString"].ToString(), path);//"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if (strFileType.Trim() == ".xlsx")
                {
                    connString = string.Format(ConfigurationManager.ConnectionStrings["Excel07ConString"].ToString(), path);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }
                try
                {
                    connString = string.Format(connString, path);
                    OleDbConnection connExcel = new OleDbConnection(connString);
                    OleDbCommand cmdExcel = new OleDbCommand();
                    OleDbDataAdapter oda = new OleDbDataAdapter();
                    DataTable dt = new DataTable();
                    cmdExcel.Connection = connExcel;
                    connExcel.Open();
                    DataTable dtExcelSchema;
                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                    cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
                    oda.SelectCommand = cmdExcel;
                    oda.Fill(ds);
                    string str = ValidExcel(ds);
                    if (str.Length == 0)
                    {
                        try
                        {
                            List<MemberBasePFSummaryModel> lstSuccessPfDataHelper = new List<MemberBasePFSummaryModel>();
                            List<MemberBasePFSummaryModel> lstErrorPfDataHelper = new List<MemberBasePFSummaryModel>();
                            dt = ds.Tables[0];
                            lstSuccessPfDataHelper.Clear();
                            lstErrorPfDataHelper.Clear();
                            DataTable dtError = new DataTable();
                            int t = 0;
                            int s = 0;
                            int u = 0;

                            string dupData = "";
                            foreach (DataRow dr in dt.Rows)
                            {
                                ++t;
                                if (!string.IsNullOrEmpty(dr["Company Name"].ToString()) && !string.IsNullOrEmpty(dr["Employee Id"].ToString()) && !string.IsNullOrEmpty(dr["Self Contribution"].ToString()))
                                {
                                    string emplId = dr["Employee Id"].ToString();
                                    DateTime pfMonth = Convert.ToDateTime(dr["PF Date of Month"].ToString());
                                    int scon = Convert.ToInt32(dr["Self Contribution"].ToString());
                                    PfData objPfData = null;
                                    objPfData = db.PfDatas.FirstOrDefault(x => x.EmployeeId == emplId && x.SelfContribution == scon && x.PFCreatedDate == pfMonth);
                                    if (objPfData != null)
                                    {
                                        ++u;
                                        if (!string.IsNullOrEmpty(dupData))
                                        {
                                            dupData += "\n" + dr["Company Name"].ToString() + ", " + dr["Employee Id"].ToString();
                                        }
                                        else
                                        {
                                            dupData = dr["Company Name"].ToString() + ", " + dr["Employee Id"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        objPfData = new PfData();
                                        objPfData.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                        objPfData.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                        objPfData.ModifiedDate = DateTime.Now;
                                        objPfData.PFCreatedDate = !string.IsNullOrEmpty(dr["PF Date of Month"].ToString()) ? Convert.ToDateTime(dr["PF Date of Month"].ToString()) : (DateTime?)null;
                                        objPfData.EmployeeId = dr["Employee Id"].ToString();
                                        objPfData.CompanyName = dr["Company Name"].ToString();
                                        objPfData.SelfContribution = Convert.ToInt32(dr["Self Contribution"]);
                                        objPfData.OfficeContribution = Convert.ToInt32(dr["Office Contribution"]);

                                        try
                                        {
                                            PfData pfData = db.PfDatas.Add(objPfData);
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        if (objPfData.PFDataId > 0)
                                        {
                                            ++s;
                                            //ValidDisplayMessage = ValidDisplayMessage += "Data save successfully";
                                            //ViewBag.Success = "Inserted";
                                        }
                                        ModelState.Clear();
                                    }
                                    //db.Entry(objPfData).State = objPfData.ClientId == 0 ? EntityState.Added : EntityState.Modified;
                                }
                                else
                                {
                                    //ROCMessageBox.Show("Minimum data not fulfil in Row " + (t + 1));
                                } //adding 1 as first row of the excel is containing column names
                            }

                            if (t > 0)
                            {
                                string result = "";
                                //int validlstSuccess3G = lstSuccessPfDataHelper.Count;
                                //if (u > 0 && s > 0)
                                //{
                                //    result = u + "Updated and " + s + "Saved";
                                //}
                                //else 

                                if (s > 0)
                                {
                                    result = s + " Saved";
                                    ValidDisplayMessage = "Total number of Valid data: " + result + " Out of " + t;
                                }
                                if (u > 0)
                                {
                                    //ValidDisplayMessage += "<table><tr><td>";
                                    //ValidDisplayMessage += "\nTotal number of Dulicate data: \n" + dupData + " Out of " + t;
                                    //ValidDisplayMessage += "<table/><tr/><td/>";
                                }
                                //else
                                //{
                                //    result = u + " Updated";
                                //}
                                //ValidDisplayMessage = "Total number of Valid data: " + result + " Out of " + t;
                                // This code is block by recomendation of Ashraf.
                                // gdvValidUpData.DataSource = lstSuccess3GHelper;
                                // gdvValidUpData.DataBind();
                                // btnSave.Visible = true;
                                //Session["SuccessBTSSwap"] = lstSuccessPfDataHelper;
                                //pnlValidUpData.Visible = true;
                            }
                            //else
                            //{ //this section for 0 number of valid data
                            //    int validlstSuccess3G = lstSuccessPfDataHelper.Count;
                            //    ValidDisplayMessage = "Total number of valid data: " + validlstSuccess3G.ToString() + " Out of " + (lstSuccessPfDataHelper.Count + lstErrorPfDataHelper.Count).ToString(); ;
                            //    //btnSave.Visible = false;
                            //    Session["SuccessBTSSwap"] = lstSuccessPfDataHelper;
                            //    // pnlValidUpData.Visible = true;
                            //}
                            //if (lstErrorPfDataHelper.Count > 0)
                            //{
                            //    // grvErrorData.DataSource = lstError3GHelper;
                            //    // grvErrorData.DataBind();
                            //    // pnlErrorData.Visible = true;
                            //}
                        }

                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }
                    else
                    {
                        ValidDisplayMessage = str;
                        //ViewBag.ExcelIssues = str;
                        // ROCMessageBox.Show(str);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                try
                {
                    da.Dispose();
                    conn.Close();
                    conn.Dispose();
                    System.IO.File.Delete(path);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
            }
            return ValidDisplayMessage;
        }

        private string ValidExcel(DataSet ds)
        {
            string[] header = { "Company Name", "Employee Name", "Employee Id", "Self Contribution", "Office Contribution", "Description", "PF Date of Month" };
            StringBuilder errorMsg = new StringBuilder();
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            var query = dt.AsEnumerable().Where(r => r.Field<string>("Company Name") == null && r.Field<string>("Employee Id") == null);
            foreach (var row in query.ToList())
            {
                row.Delete();
                dt.AcceptChanges();
            }
            //
            int rowcount = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row["Company Name"].ToString())) //checking blank rows out of count otherwise sytem does not go for next step
                {
                    string completed = (string)row["Company Name"];
                    if (!string.IsNullOrEmpty(completed))
                    {
                        rowcount++;
                    }
                }
            }
            if (rowcount <= 3000)
            {
                string flag = string.Empty;
                StringBuilder errorMsgClo = new StringBuilder();

                if (dt.Columns.Count == 7)
                {
                    foreach (DataColumn c in dt.Columns)
                    {
                        string str = c.ColumnName;
                        if (!header.Contains(str))
                        {
                            errorMsgClo.Append(str + ", ");
                        }
                    }
                    if (errorMsgClo.Length > 0)
                    {
                        errorMsg.Append("Excel column are invalid, Column : " + errorMsgClo.ToString());
                    }
                }
                else
                {
                    errorMsg.Append("Excel Template formate is not correct.");
                }
            }
            else
            {
                errorMsg.Append("Please upload 300 Client at a time");
            }
            return errorMsg.ToString();
        }

        private MemberBasePFSummaryModel CreateObject(DataRow dr)
        {
            string errorMsg = string.Empty;
            MemberBasePFSummaryModel objKgReCrmModel = new MemberBasePFSummaryModel();
            try
            {
                List<string> lstProjectName = new List<string>();

                if (!string.IsNullOrEmpty(dr["Company Name"].ToString()))
                {
                    objKgReCrmModel.CompanyName = dr["Company Name"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["Employee Id"].ToString()))
                {
                    objKgReCrmModel.EmployeeId = dr["Employee Id"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["Self Contribution"].ToString()))
                {
                    objKgReCrmModel.SelfContribution = Convert.ToInt32(dr["Self Contribution"].ToString());
                }

                if (!string.IsNullOrEmpty(dr["Office Contribution"].ToString()))
                {
                    objKgReCrmModel.CompanyContribution = Convert.ToInt32(dr["Office Contribution"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["Description"].ToString()))
                {
                    objKgReCrmModel.Description = dr["Description"].ToString();
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return objKgReCrmModel;
        }

        #endregion
    }
}
