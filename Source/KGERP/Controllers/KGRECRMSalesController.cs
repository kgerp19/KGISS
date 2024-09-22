using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KGRECRMSalesController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        IKgReCrmService kgReCrmService = new KgReCrmService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: KGRECRMSales 

        
        [HttpGet]
        public ActionResult AdvanceClientSearch(int? Page_No, string searchText)
        {
            List<KgReCrmModel> landNLegal = null;
            landNLegal = kgReCrmService.GetKGREClient(searchText ?? "");
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(landNLegal.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText)
        {
            List<KgReCrmModel> landNLegal = null;
            landNLegal = kgReCrmService.GetKGREClient(searchText ?? "");
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(landNLegal.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        
        [HttpGet]
        public ActionResult UpcomingClientSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<KgReCrmModel> kgReCrmModel = kgReCrmService.GetKGREClientEvent();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var kgReCrmdata = kgReCrmModel.Where(
                    x => x.FullName.Contains(searchText)
                || x.ResponsibleOfficer.Contains(searchText)
                || x.ReferredBy.Contains(searchText)
                || x.DepartmentOrInstitution.Contains(searchText)
                || x.SourceOfMedia.Contains(searchText));
                return View(kgReCrmdata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kgReCrmModel.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult Previous7DaysClientSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<KgReCrmModel> kgReCrmModels = kgReCrmService.GetPrevious7DaysClientSchedule();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var kgReCrmdata = kgReCrmModels.Where(
                     x => x.FullName.Contains(searchText)
                 || x.ResponsibleOfficer.Contains(searchText)
                 || x.ReferredBy.Contains(searchText)
                 || x.DepartmentOrInstitution.Contains(searchText)
                 || x.SourceOfMedia.Contains(searchText));
                return View(kgReCrmdata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kgReCrmModels.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(int id, int companyId)
        {
            var tables = new KgReCrmViewModel
            {
                _KgReCrmModel = kgReCrmService.GetKGRClientById(id),
                Genders = dropDownItemService.GetDropDownItemSelectModels(3),
                SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29),
                PromotionalOffers = dropDownItemService.GetDropDownItemSelectModels(30),
                PlotFlats = dropDownItemService.GetDropDownItemSelectModels(31),
                Impressions = dropDownItemService.GetDropDownItemSelectModels(32),
                StatusLevels = dropDownItemService.GetDropDownItemSelectModels(33),
                KGREProjects = dropDownItemService.GetDropDownItemSelectModels(34),
                KGREChoiceAreas = dropDownItemService.GetDropDownItemSelectModels(35),
                ResponsiblePersons =await kgReCrmService.GetKGREClient(companyId)
            };
            ViewBag.cHistory = GetClientHistory(id);
            return View(tables);
        }
        // POST: KGRECRM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEdit(KgReCrmViewModel model)
        {
            string redirectPage = string.Empty;
            if (model._KgReCrmModel.ClientId <= 0)
            {
                bool exist = false;
                // exist = db.KGRECustomers.Where(x => x.FullName == model.KgReCrmModel.FullName).Any();
                exist = db.KttlCustomers.Where(x => x.MobileNo.ToLower().Equals(model._KgReCrmModel.MobileNo.ToLower())).Any();

                if (exist)
                {
                    TempData["errMessage"] = "Exists";
                    return RedirectToAction("CreateOrEdit");
                }
                else
                {
                    kgReCrmService.SaveKGREClient(0, model._KgReCrmModel);
                }
                redirectPage = "Index";
            }
            else
            {
                KGRECustomer kGRECustomer = db.KGRECustomers.FirstOrDefault(x => x.ClientId == model._KgReCrmModel.ClientId);
                if (kGRECustomer == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }
                kgReCrmService.SaveKGREClient(model._KgReCrmModel.ClientId, model._KgReCrmModel);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit";
                ViewBag.cHistory = GetClientHistory(model._KgReCrmModel.ClientId);
            }

            return RedirectToAction(redirectPage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetClientHistory(long id)
        {
            string htmlStr = "<table width='100%' align='center' cellpadding='2' cellspacing='2' border='0' bgcolor='#F0F0F0'> ";
            DataTable dt = new DataTable();
            dt = GetClientHistoryByClientId(id);
            string style = "\"border-bottom:1pt solid #F3F3F3;background-color: #F5F5F5;\"";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Name"].ToString();
                dt.Rows[i]["Date"].ToString();
                dt.Rows[i]["Remarks"].ToString();
                dt.Rows[i]["Flag"].ToString();
                string NrOfDays = "";
                DateTime d1 = DateTime.Now;
                DateTime dtstart = Convert.ToDateTime(dt.Rows[i]["Date"]);

                // DateTime dtstart = Convert.ToDateTime(dr["EndDate"]);
                //string[] strDateTime = dtstart.ToString("dd-MM-yyyy").Split('-');
                string strDateTime2 = dtstart.ToString("dd-MM-yyyy");

                // displaying values in textboxes
                //string txtDate = strDateTime[0];
                //string txtMonth = strDateTime[1];
                //string txtYear = strDateTime[2];

                DateTime d2 = Convert.ToDateTime(dt.Rows[i]["Date"].ToString());
                TimeSpan t = d1 - d2;

                if (t.Days > 0)
                {
                    int days = t.Days;
                    NrOfDays = days + " Days".ToString();
                }
                else if (t.Hours > 0)
                {
                    int Hour = t.Hours;
                    NrOfDays = Hour + " Hour".ToString();
                }
                else if (t.Minutes > 0)
                {
                    int Minute = t.Minutes;
                    NrOfDays = Minute + " Minutes".ToString();
                }
                else if (t.Seconds > 0)
                {
                    int Second = t.Seconds;
                    NrOfDays = Second + " Second".ToString();
                }
                if (dt.Rows[i]["Flag"].ToString() == "Attachment Added")
                {
                    htmlStr += "<tr  style=" + style + "><td> <span title=" + strDateTime2 + ">Changed " + NrOfDays + " ago by</span> <b> " + dt.Rows[i]["Name"].ToString() + "</b></td></tr>";
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: " + dt.Rows[i]["Remarks"].ToString() + " </td></tr>";
                }
                else
                {
                    htmlStr += "<tr style=" + style + "><td> <span title=" + strDateTime2 + ">Changed " + NrOfDays + " ago by</span> <b> " + dt.Rows[i]["Name"].ToString() + "</b></td></tr>";
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: <BR>" + dt.Rows[i]["Remarks"].ToString() + " </td></tr>";
                }
            }

            return htmlStr += "</table>";
        }

        private DataTable GetClientHistoryByClientId(long KGREId)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("KGRE_GetChangeHistory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@KGREId", KGREId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }
        
        [HttpGet]
        public ActionResult ExportKGREClientToExcel(string searchText)
        {
            var gv = new GridView();
            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (!string.IsNullOrEmpty(searchText))
            {
                dt = GetClientList(searchText);
            }
            else
            {
                dt = GetClientList();
            }

            gv.DataSource = dt;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= KG_ClientList.xls");
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

        public DataTable GetClientList(string serachText)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from vGetClientList where ( [ClientNo] like  '%" + serachText + "%' or  [ClientType] like  '%" + serachText + "%' or [ResponsibleLayer] like  '%" + serachText + "%' or [ClientStatus] like  '%" + serachText + "%')"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public DataTable GetClientList()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from vGetClientList"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }


        
        public ActionResult UploadClientBatch()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult UploadClientBatch(KgReCrmViewModel file)
        {
            UploadExcelFile(file);
            return View();
        }


        protected void UploadExcelFile(KgReCrmViewModel file)
        {
            // string ValidDisplayMessage = "";
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
                    return;
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
                            List<KgReCrmModel> lstSuccessKGReCrmHelper = new List<KgReCrmModel>();
                            List<KgReCrmModel> lstErrorKGReCrmHelper = new List<KgReCrmModel>();
                            dt = ds.Tables[0];
                            lstSuccessKGReCrmHelper.Clear();
                            lstErrorKGReCrmHelper.Clear();
                            DataTable dtError = new DataTable();
                            int i = 0;
                            foreach (DataRow dr in dt.Rows)
                            {
                                ++i;
                                if (!string.IsNullOrEmpty(dr["Full Name"].ToString()) && !string.IsNullOrEmpty(dr["Mobile No"].ToString()))
                                {
                                    KGRECustomer objKGRECustomer = new KGRECustomer();

                                    objKGRECustomer.DateOfContact = !string.IsNullOrEmpty(dr["Date Of Contact"].ToString()) ? Convert.ToDateTime(dr["Date Of Contact"].ToString()) : (DateTime?)null;
                                    objKGRECustomer.FullName = dr["Full Name"].ToString();
                                    objKGRECustomer.Designation = dr["Designation"].ToString();
                                    objKGRECustomer.DepartmentOrInstitution = dr["Department/ Institution"].ToString();
                                    objKGRECustomer.MobileNo = dr["Mobile No"].ToString();
                                    objKGRECustomer.MobileNo2 = dr["Mobile No2"].ToString();
                                    objKGRECustomer.Email = dr["Email"].ToString();
                                    objKGRECustomer.SourceOfMedia = dr["Source of Media"].ToString();
                                    objKGRECustomer.PromotionalOffer = dr["Promotional Offer"].ToString();
                                    objKGRECustomer.Impression = dr["Impression"].ToString();
                                    objKGRECustomer.StatusLevel = dr["Status Level"].ToString();
                                    objKGRECustomer.TypeOfInterest = dr["Type of Interest"].ToString();
                                    objKGRECustomer.Project = dr["Project"].ToString();
                                    objKGRECustomer.Remarks = dr["Remarks"].ToString();
                                    objKGRECustomer.ChoieceOfArea = dr["Choice Of Area"].ToString();
                                    objKGRECustomer.ReferredBy = dr["Referred By"].ToString();
                                    objKGRECustomer.ResponsibleOfficer = dr["Responsible Officer"].ToString();

                                    using (var ctx = new ERPEntities())
                                    {
                                        ctx.KGRECustomers.Add(objKGRECustomer);
                                        ctx.SaveChanges();
                                    }
                                }
                                else
                                {
                                    //ROCMessageBox.Show("Minimum data not fulfil in Row " + (i + 1));
                                } //adding 1 as first row of the excel is containing column names
                            }
                            //if (lstSuccessKGReCrmHelper.Count > 0)
                            //{
                            //    int validlstSuccess3G = lstSuccessKGReCrmHelper.Count;
                            //    ValidDisplayMessage = "Total number of valid data: " + validlstSuccess3G.ToString() + " Out of " + (lstSuccessKGReCrmHelper.Count + lstErrorKGReCrmHelper.Count).ToString(); ;
                            //    // This code is block by recomendation of Ashraf.
                            //    // gdvValidUpData.DataSource = lstSuccess3GHelper;
                            //    // gdvValidUpData.DataBind();
                            //    // btnSave.Visible = true;
                            //    Session["SuccessBTSSwap"] = lstSuccessKGReCrmHelper;
                            //    //pnlValidUpData.Visible = true;
                            //}
                            //else
                            //{ //this section for 0 number of valid data
                            //    int validlstSuccess3G = lstSuccessKGReCrmHelper.Count;
                            //    ValidDisplayMessage = "Total number of valid data: " + validlstSuccess3G.ToString() + " Out of " + (lstSuccessKGReCrmHelper.Count + lstErrorKGReCrmHelper.Count).ToString(); ;
                            //    //btnSave.Visible = false;
                            //    Session["SuccessBTSSwap"] = lstSuccessKGReCrmHelper;
                            //    // pnlValidUpData.Visible = true;
                            //}
                            //if (lstErrorKGReCrmHelper.Count > 0)
                            //{
                            //    // grvErrorData.DataSource = lstError3GHelper;
                            //    // grvErrorData.DataBind();
                            //    // pnlErrorData.Visible = true;
                            //}
                        }
                        catch (Exception ex) { logger.Error(ex); }
                    }
                    else
                    {
                        ViewBag.ExcelIssues = str;
                        // ROCMessageBox.Show(str);
                    }
                }
                catch (Exception ex) { logger.Error(ex); }
                try
                {
                    da.Dispose();
                    conn.Close();
                    conn.Dispose();
                    System.IO.File.Delete(path);
                }
                catch (Exception ex) { logger.Error(ex); }
            }
            else
            {
            }
        }

        private string ValidExcel(DataSet ds)
        {
            string[] header = { "Date Of Contact","Full Name","Designation","Department/ Institution","Mobile No","Mobile No2","Email","Source Of Media",
                "Promotional Offer","Impression","Status Level","Type of Interest","Project","Remarks","Choice Of Area","Referred By","Responsible Officer"};
            StringBuilder errorMsg = new StringBuilder();
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            var query = dt.AsEnumerable().Where(r => r.Field<string>("Full Name") == null && r.Field<string>("Mobile No") == null);
            foreach (var row in query.ToList())
            {
                row.Delete();
                dt.AcceptChanges();
            }
            //
            int rowcount = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row["Full Name"].ToString())) //checking blank rows out of count otherwise sytem does not go for next step
                {
                    string completed = (string)row["Full Name"];
                    if (!string.IsNullOrEmpty(completed))
                    {
                        rowcount++;
                    }
                }
            }
            if (rowcount <= 300)
            {
                string flag = string.Empty;
                StringBuilder errorMsgClo = new StringBuilder();

                if (dt.Columns.Count == 17)
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

        private KgReCrmModel CreateObject(DataRow dr)
        {
            string errorMsg = string.Empty;
            KgReCrmModel objKgReCrmModel = new KgReCrmModel();
            try
            {
                List<string> lstProjectName = new List<string>();


                if (!string.IsNullOrEmpty(dr["Mobile No"].ToString()))
                {
                    objKgReCrmModel.MobileNo = dr["Mobile No"].ToString();
                    using (ERPEntities unit = new ERPEntities())
                    {
                        KGRECustomer objCust = unit.KGRECustomers.Where(x => x.MobileNo == dr["Mobile No"].ToString()).FirstOrDefault();
                        if (objCust == null)
                        {
                            errorMsg = "This Mobile No is already Exist";
                        }
                    }
                }
                else { errorMsg = "Mobile No 1 is not empty."; }

                if (!string.IsNullOrEmpty(dr["Full Name"].ToString()))
                {
                    objKgReCrmModel.FullName = dr["Full Name"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["Designation"].ToString()))
                {
                    objKgReCrmModel.Designation = dr["Designation"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["DepartmentOrInstitution"].ToString()))
                {
                    objKgReCrmModel.DepartmentOrInstitution = dr["DepartmentOrInstitution"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["PresentAddress"].ToString()))
                {
                    objKgReCrmModel.PresentAddress = dr["PresentAddress"].ToString();
                }

                if (!string.IsNullOrEmpty(dr["PermanentAddress"].ToString()))
                {
                    objKgReCrmModel.PermanentAddress = dr["PermanentAddress"].ToString();
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return objKgReCrmModel;
        }


        public ActionResult ExistingClientList()
        {
            return View();
        }
    }
}
