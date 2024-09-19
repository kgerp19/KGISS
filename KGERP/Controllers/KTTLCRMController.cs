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
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KTTLCRMController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IKttlCustomerService kttlCustomerService = new KttlCustomerService();
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        ICompanyService companyService = new CompanyService(new ERPEntities());
        IBankService bankService = new BankService();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string clientName, string passportNo, string searchText, string ResponsiblePerson, string SourceOfMedia,
            string MobileNo, string NidNo, string Services, string ServiceYear, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            int comId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0;
            //int comId = (int)Session["CompanyId"];
            ViewBag.ClientStatuss = dropDownItemService.GetDropDownItemSelectModels(36);
            ViewBag.Servicess = dropDownItemService.GetDropDownItemSelectModels(16);
            ViewBag.SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29);
            ViewBag.ResponsiblePersons = kttlCustomerService.GetKTTLEmployees();
            ViewBag.ServiceYears = kttlCustomerService.GetServeiceYear();

            List<KttlCustomerModel> kttlCrmModel = null;
            IQueryable<KttlCustomer> kttlCustomers = null;
            kttlCustomers = db.KttlCustomers.Where(x => x.CompanyId == comId);

            if (!string.IsNullOrWhiteSpace(ResponsiblePerson))
                kttlCustomers = kttlCustomers.Where(m => m.ResponsiblePerson == ResponsiblePerson && m.CompanyId == comId);

            if (!string.IsNullOrWhiteSpace(SourceOfMedia))
                kttlCustomers = kttlCustomers.Where(m => m.SourceOfMedia == SourceOfMedia && m.CompanyId == comId);

            if (!string.IsNullOrWhiteSpace(MobileNo))
                kttlCustomers = kttlCustomers.Where(m => m.MobileNo == MobileNo && m.CompanyId == comId);

            if (!string.IsNullOrWhiteSpace(NidNo))
                kttlCustomers = kttlCustomers.Where(m => m.NationalID == NidNo && m.CompanyId == comId);

            if (!string.IsNullOrWhiteSpace(Services))
                kttlCustomers = kttlCustomers.Where(m => m.Services == Services && m.CompanyId == comId);

            if (!string.IsNullOrWhiteSpace(ServiceYear))
            {
                int serviceYear = Convert.ToInt32(ServiceYear);
                kttlCustomers = kttlCustomers.Where(m => m.ServiceYear == serviceYear && m.CompanyId == comId);
            }

            if (!string.IsNullOrWhiteSpace(clientName))
                kttlCustomers = kttlCustomers.Where(m => m.FullName.Contains(clientName) || m.GivenName.Contains(clientName) || m.SurName.Contains(clientName) && m.CompanyId == comId);

            if (!string.IsNullOrWhiteSpace(passportNo))
                kttlCustomers = kttlCustomers.Where(m => m.PassportNo.Contains(passportNo) && m.CompanyId == comId);

            kttlCrmModel = ObjectConverter<KttlCustomer, KttlCustomerModel>.ConvertList(kttlCustomers.ToList()).ToList();

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            return View(kttlCrmModel.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult UpcomingClientSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<KttlCustomerModel> kttlModel = kttlCustomerService.GetKttlCustomerSchedule();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var kttlModeldata = kttlModel.Where(
                    x => x.FullName.Contains(searchText)
                || x.ResponsiblePerson.Contains(searchText)
                || x.Organization.Contains(searchText)
                || x.SourceOfMedia.Contains(searchText));
                return View(kttlModeldata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kttlModel.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult Previous7DaysClientSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<KttlCustomerModel> kttlModel = kttlCustomerService.Previous7DaysClientSchedule();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var kttlModeldata = kttlModel.Where(
                    x => x.FullName.Contains(searchText)
                || x.ResponsiblePerson.Contains(searchText)
                || x.Organization.Contains(searchText)
                || x.SourceOfMedia.Contains(searchText));
                return View(kttlModeldata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kttlModel.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int id, int? companyId)
        {
            if (companyId > 0 || id > 0)
            {
                Session["CompanyId"] = companyId;
                Session["Id"] = id;
            }

            int clientId = Session["Id"] != null ? Convert.ToInt32(Session["Id"]) : 0;
            KttlCustomerModel model = kttlCustomerService.GetKttlCustomer(clientId);
            model.MaritalTypes = dropDownItemService.GetDropDownItemSelectModels(2);
            model.BloodGroups = dropDownItemService.GetDropDownItemSelectModels(5);
            model.Religions = dropDownItemService.GetDropDownItemSelectModels(9);
            model.Servicess = dropDownItemService.GetDropDownItemSelectModels(16);
            model.Genders = dropDownItemService.GetDropDownItemSelectModels(21);
            model.Titles = dropDownItemService.GetDropDownItemSelectModels(22);
            model.SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29);
            model.ClientStatuss = dropDownItemService.GetDropDownItemSelectModels(36);
            model.Professions = dropDownItemService.GetDropDownItemSelectModels(64);
            model.TypeOfClients = dropDownItemService.GetDropDownItemSelectModels(65);
            model.PassportValidities = dropDownItemService.GetDropDownItemSelectModels(66);
            model.ResponsiblePersons = kttlCustomerService.GetKTTLEmployees();
            model.ServiceYears = kttlCustomerService.GetServeiceYear();
            model.Divisions = districtService.GetDivisionSelectModels();
            model.PlacesOfBirth = districtService.GetDistrictSelectModels();
            //model.Banks = bankService.GetBankSelectModels();//Added by Ashraf 20200616

            if (model.ClientId > 0)
            {
                model.Districts = districtService.GetDistrictByDivisionId(model.Division);
                model.Upazilas = upazilaService.GetUpzilaByDistrictId(model.District);
            }
            else
            {
                model.Districts = new List<SelectModel>();
                model.Upazilas = new List<SelectModel>();
            }

            ViewBag.cHistory = GetClientHistory(clientId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEdit(KttlCustomerModel model)
        {
            //int companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0;
            int companyId = 18;
            model.CompanyId = companyId;
            string redirectPage = string.Empty;
            if (model.ClientId <= 0)
            {
                KttlCustomer kttlCustomer = db.KttlCustomers.FirstOrDefault(x => x.MobileNo == model.MobileNo);
                if (kttlCustomer != null)
                {
                    TempData["errMessage"] = "Exists";
                    return RedirectToAction("CreateOrEdit");
                }
                else
                {
                    kttlCustomerService.SaveKTTLCustomerData(0, model);
                }
                redirectPage = "Index";
            }
            else
            {
                KttlCustomer kttlCustomer = db.KttlCustomers.FirstOrDefault(x => x.ClientId == model.ClientId);
                if (kttlCustomer == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                kttlCustomerService.SaveKTTLCustomerData(model.ClientId, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit";
                ViewBag.cHistory = GetClientHistory(model.ClientId);
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
                string strDateTime2 = dtstart.ToString("dd-MM-yyyy");

                string str = dt.Rows[i]["Remarks"].ToString();

                char[] spearator = { '*' };
                int count = 1;

                // Using the Method 
                string[] strlist = str.Split(spearator,
                       count, StringSplitOptions.None);
                string changehistory = "";
                foreach (string s in strlist)
                {
                    changehistory = changehistory += s + "\n";
                }


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
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: <BR>" + changehistory + " </td></tr>";
                    //htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: <BR>" + dt.Rows[i]["Remarks"].ToString() + " </td></tr>";
                }
            }

            return htmlStr += "</table>";
        }

        private DataTable GetClientHistoryByClientId(long KTTLClientId)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("KTTL_GetChangeHistory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@KTTLClientId", KTTLClientId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        
        [HttpGet]
        public ActionResult ExportKGREClientToExcel(int? Page_No, string clientName, string passportNo,
            string searchText, string ResponsiblePerson, string SourceOfMedia,
            string MobileNo, string NidNo, string Services, string ServiceYear)
        {
            var gv = new GridView();
            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (!string.IsNullOrEmpty(clientName) || !string.IsNullOrEmpty(passportNo)
                || !string.IsNullOrEmpty(ResponsiblePerson) || !string.IsNullOrEmpty(MobileNo) || !string.IsNullOrEmpty(SourceOfMedia)
                || !string.IsNullOrEmpty(NidNo) || !string.IsNullOrEmpty(Services) || !string.IsNullOrEmpty(ServiceYear))
            {
                dt = GetFilterClientList(Page_No, clientName, passportNo, searchText, ResponsiblePerson, SourceOfMedia, MobileNo, NidNo, Services, ServiceYear);
            }
            else
            {
                dt = GetClientList();
            }
            if (dt.Rows.Count > 0)
            {
                try
                {
                    string[] selectedColumns = new[] { "ClientId", "CTitle", "FullName", "SurName", "GivenName", "PresentAddress", "PermanentAddress", "DateofBirth", "PassportNo", "DateOfIssue", "DateOfExpire", "Nationality", "Telephone", "MobileNo", "Email", "Gender", "FatherName", "MotherName", "MaritalStatus", "Spouse", "Religion", "BloodGroup", "ImageUrl", "ClientStatus", "NoOfChild", "Organization", "Services", "ServicesDescription", "Remarks", "ResponsiblePerson", "NextScheduleDate", "LastMeetingDate1", "LastMeetingDate2", "SourceOfMedia", "PromotionalOffer", "NIDorBirthID", "ServiceYear", "BirthID" };
                    DataTable dt1 = new DataView(dt).ToTable(false, selectedColumns);

                    gv.DataSource = dt1;
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
                catch (Exception ex)
                {
                    logger.Error("KTTL Export: " + ex);
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public DataTable GetClientList(int? Page_No, string clientName, string passportNo, string searchText, string ResponsiblePerson, string SourceOfMedia,
            string MobileNo, string NidNo, string Services, string ServiceYear)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from vGetClientList where ( [ClientNo] like  '%" + searchText + "%' or  [ClientType] like  '%" + searchText + "%' or [ResponsibleLayer] like  '%" + searchText + "%' or [ClientStatus] like  '%" + searchText + "%')"))
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
                    using (SqlCommand cmd = new SqlCommand("select * from KTTLCustomer"))
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

        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KttlCustomer kttlCustomer = db.KttlCustomers.Find(id);
            if (kttlCustomer == null)
            {
                return HttpNotFound();
            }
            return View(kttlCustomer);
        }

        [HttpGet]
        
        public ActionResult CustomerAll(string ReportName, string ReportDescription)
        {
            int index = ReportDescription.IndexOf('?');
            string description = ReportDescription.Substring(0, index);

            var rptInfo = new ReportInfo
            {
                ReportName = ReportName,
                ReportDescription = description,
                ReportURL = String.Format("../../Reports/ReportTemplate.aspx?ReportName={0}&Height={1}", ReportName, 650),
                Width = 100,
                Height = 650
            };

            return View(rptInfo);
        }
        public DataTable GetFilterClientList(int? Page_No, string clientName, string passportNo, string searchText, string ResponsiblePerson, string SourceOfMedia,
            string MobileNo, string NidNo, string Services, string ServiceYear)
        {
            try
            {
                DataTable dt = new DataTable();
                IQueryable<KttlCustomer> _KttlCustomer = null;
                List<KttlCustomerModel> _KttlCustomerModel = null;
                _KttlCustomer = db.KttlCustomers;

                if (!string.IsNullOrWhiteSpace(ResponsiblePerson))
                    _KttlCustomer = _KttlCustomer.Where(m => m.ResponsiblePerson == ResponsiblePerson);

                if (!string.IsNullOrWhiteSpace(SourceOfMedia))
                {
                    _KttlCustomer = _KttlCustomer.Where(m => m.SourceOfMedia == SourceOfMedia);
                }

                if (!string.IsNullOrWhiteSpace(MobileNo))
                {
                    _KttlCustomer = _KttlCustomer.Where(m => m.MobileNo == MobileNo);
                }

                if (!string.IsNullOrWhiteSpace(NidNo))
                    _KttlCustomer = _KttlCustomer.Where(m => m.NationalID == NidNo);

                if (!string.IsNullOrWhiteSpace(Services))
                    _KttlCustomer = _KttlCustomer.Where(m => m.Services == Services);

                if (!string.IsNullOrWhiteSpace(ServiceYear))
                {
                    int serviceYear = Convert.ToInt32(ServiceYear);
                    _KttlCustomer = _KttlCustomer.Where(m => m.ServiceYear == serviceYear);

                }

                _KttlCustomerModel = ObjectConverter<KttlCustomer, KttlCustomerModel>.ConvertList(_KttlCustomer.ToList()).ToList();

                if (_KttlCustomerModel != null)
                {
                    dt = CreateDataTable(_KttlCustomerModel);
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        private DataTable CreateDataTable(IList<KttlCustomerModel> item)
        {
            Type type = typeof(KttlCustomerModel);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (KttlCustomerModel entity in item)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public class DisplayReport
        {
            public string Name
            {
                get;
                set;
            }
            public int Count
            {
                get;
                set;
            }
        }
        public ActionResult KTTLDashboard()
        {
            DataTable dt = GetClientStatus();
            if (dt.Rows.Count > 0)
            {
                ViewData.Model = dt.AsEnumerable();
            }

            DataTable dt2 = GetClientStatus();
            if (dt2.Rows.Count > 0)
            {
                TempData["data"] = dt2.AsEnumerable();
            }

            var query = db.KttlCustomers
               .GroupBy(p => p.ClientStatus)
               .Select(g => new DisplayReport { Name = g.Key, Count = g.Count() })
               .OrderByDescending(g => g.Count);
            ViewBag.ServiceYear = query;


            return View();
        }

        
        public ActionResult GetResponsiblePersonChartImage()
        {
            DataTable dt = GetResponsiblePerson();
            var key = (dynamic)null;
            if (dt.Rows.Count > 0)
            {
                List<string> gender = (from p in dt.AsEnumerable() select p.Field<string>("ResponsiblePerson")).Distinct().ToList();
                int countGender = gender.Count; //countGender

                List<string> gender2 = new List<string>();
                List<double> genderCount = new List<double>();

                foreach (DataRow row in dt.Rows)
                {
                    gender2.Add((string)Convert.ToString(row["ResponsiblePerson"]));
                }
                string[] outputgender2 = gender2.ToArray();

                foreach (DataRow row in dt.Rows)
                {
                    genderCount.Add((double)Convert.ToDouble(row["Total"]));
                }
                double[] outputgenderCount = genderCount.ToArray();
                if (gender.Count > 0)
                { 
                    key = new Chart(width: 360, height: 360)
                       .AddTitle("Responsible Officer's Client")
                       .AddSeries(
                       chartType: "Bar",
                       name: "Client",
                       xValue: outputgender2,
                       yValues: outputgenderCount);
                }
            }
            return File(key.ToWebImage().GetBytes(), "image/jpeg");

        }

        
        public ActionResult GetServicesChartImage()
        {
            DataTable dt = GetServices();
            var key = (dynamic)null;
            if (dt.Rows.Count > 0)
            {
                List<string> gender = (from p in dt.AsEnumerable() select p.Field<string>("Services")).Distinct().ToList();
                int countGender = gender.Count; //countGender

                List<string> gender2 = new List<string>();
                List<double> genderCount = new List<double>();

                foreach (DataRow row in dt.Rows)
                {
                    gender2.Add((string)Convert.ToString(row["Services"]));
                }
                string[] outputgender2 = gender2.ToArray();

                foreach (DataRow row in dt.Rows)
                {
                    genderCount.Add((double)Convert.ToDouble(row["Total"]));
                }
                double[] outputgenderCount = genderCount.ToArray();
                if (gender.Count > 0)
                {
                    key = new Chart(width: 360, height: 360)
                       .AddTitle("Client Services Ratio")
                       .AddSeries(
                       chartType: "Bubble",
                       name: "Client Services",
                       xValue: outputgender2,
                       yValues: outputgenderCount);
                }
            }
            return File(key.ToWebImage().GetBytes(), "image/jpeg");
        }
        public DataTable GetResponsiblePerson()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("Select  e.Name AS ResponsiblePerson, Count(*) As Total from KttlCustomer c Left Join Employee e on c.ResponsiblePerson = e.EmployeeId Group by e.Name"))
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

        public DataTable GetClientStatus()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("Select  ClientStatus, Count(*) As Total from KttlCustomer Group by ClientStatus"))
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

        public DataTable GetServices()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("Select  Services, Count(*) As Total from KttlCustomer Group by Services "))
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

        //Fetching District Data base on Division Name-Ashraf
        [HttpPost]
        public ActionResult GetDistrictByDivisionName(int divisionId)
        {
            //List<SelectModel> Districts = districtService.GetDistrictByDivisionName(name);//string name
            List<SelectModel> Districts = districtService.GetDistrictByDivisionId(divisionId);
            return Json(Districts, JsonRequestBehavior.AllowGet);
        }

        //Fetching Upzilla Data base on District Name-Ashraf
        [HttpPost]
        public ActionResult GetUpzilaByDistrictName(int disrtictId)
        {
            //List<SelectModel> Upazilas = upazilaService.GetUpzilaByDistrictName(name);// string name
            List<SelectModel> Upazilas = upazilaService.GetUpzilaByDistrictId(disrtictId);
            return Json(Upazilas, JsonRequestBehavior.AllowGet);
        }

        //Fetching District Data base on Division Name-Ashraf
        [HttpPost]
        public ActionResult GetDistrictByDivisionName2(string name)
        {
            List<SelectModel> Districts = districtService.GetDistrictByDivisionName(name);
            return Json(Districts, JsonRequestBehavior.AllowGet);
        }

        //Fetching Upzilla Data base on District Name-Ashraf
        [HttpPost]
        public ActionResult GetUpzilaByDistrictName2(string name)
        {
            List<SelectModel> Upazilas = upazilaService.GetUpzilaByDistrictName(name);
            return Json(Upazilas, JsonRequestBehavior.AllowGet);
        }

        #region

        
        [HttpGet]
        public ActionResult CustomerList(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoadCustomerDataList()
        {
            try
            {
                var list = kttlCustomerService.LoadCustomerDataList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region // Batch Upload
        
        public ActionResult UploadClientBatch(int? companyId)
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
        public ActionResult UploadClientBatch(KttlCustomerModel file)
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

        private string UploadExcelFile(KttlCustomerModel file)
        {
            int companyId2 = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0;
            if (companyId2 > 0)
            {

            }
            string ValidDisplayMessage = "";

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
                            List<KgReCrmModel> lstSuccessKGReCrmHelper = new List<KgReCrmModel>();
                            List<KgReCrmModel> lstErrorKGReCrmHelper = new List<KgReCrmModel>();
                            dt = ds.Tables[0];
                            lstSuccessKGReCrmHelper.Clear();
                            lstErrorKGReCrmHelper.Clear();
                            DataTable dtError = new DataTable();
                            int t = 0;
                            int s = 0;
                            int u = 0;

                            string dupData = "";
                            foreach (DataRow dr in dt.Rows)
                            {
                                ++t;
                                if (!string.IsNullOrEmpty(dr["Full Name"].ToString()) && !string.IsNullOrEmpty(dr["Mobile"].ToString()))
                                {
                                    string mobile = "0" + dr["Mobile"].ToString();
                                    KGRECustomer objKGRECustomer = null;
                                    objKGRECustomer = db.KGRECustomers.FirstOrDefault(x => x.MobileNo == mobile && x.CompanyId == companyId2);
                                    if (objKGRECustomer != null)
                                    {
                                        ++u;
                                        if (!string.IsNullOrEmpty(dupData))
                                        {
                                            dupData += "\n Name: " + dr["Full Name"].ToString() + ", Mobile no. 0" + dr["Mobile"].ToString();
                                        }
                                        else
                                        {
                                            dupData = "\n Name: " + dr["Full Name"].ToString() + ", Mobile no. 0" + dr["Mobile"].ToString();
                                        }

                                    }
                                    else
                                    {
                                        objKGRECustomer = new KGRECustomer();
                                        objKGRECustomer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                        objKGRECustomer.CreatedDate = DateTime.Now;
                                        objKGRECustomer.DateOfContact = DateTime.Now;
                                        objKGRECustomer.DateofBirth = !string.IsNullOrEmpty(dr["Date of Birth"].ToString()) ? Convert.ToDateTime(dr["Date of Birth"].ToString()) : (DateTime?)null;
                                        objKGRECustomer.FullName = dr["Full Name"].ToString();
                                        objKGRECustomer.CampaignName = dr["Campaign Name"].ToString();
                                        objKGRECustomer.ResponsibleOfficer = dr["Dealing Officer"].ToString();
                                        objKGRECustomer.PresentAddress = dr["Present Address"].ToString();
                                        objKGRECustomer.PermanentAddress = dr["Permanent address"].ToString();
                                        objKGRECustomer.Designation = dr["Job Title"].ToString();
                                        objKGRECustomer.DepartmentOrInstitution = dr["Organization"].ToString();
                                        objKGRECustomer.MobileNo = "0" + dr["Mobile"].ToString();
                                        objKGRECustomer.MobileNo2 = !string.IsNullOrEmpty(dr["Mobile 2"].ToString()) ? "0" + dr["Mobile 2"].ToString() : "";
                                        objKGRECustomer.Email = dr["Email"].ToString();
                                        objKGRECustomer.Email1 = dr["Email 2"].ToString();

                                        objKGRECustomer.SourceOfMedia = dr["Source of Media"].ToString();
                                        objKGRECustomer.ServicesDescription = dr["Service Detail"].ToString();
                                        objKGRECustomer.PromotionalOffer = dr["Promotional Offer"].ToString();
                                        string gender = dr["Gender"].ToString();
                                        if (!string.IsNullOrEmpty(gender))
                                        {
                                            objKGRECustomer.Gender = char.ToUpper(gender[0]) + gender.Substring(1);
                                        }
                                        //objKGRECustomer.Gender = dr["Gender"].ToString();
                                        objKGRECustomer.TypeOfInterest = dr["Interested In"].ToString();
                                        objKGRECustomer.StatusLevel = !string.IsNullOrEmpty(dr["Service Status"].ToString()) ? dr["Service Status"].ToString() : "New";
                                        if (!string.IsNullOrEmpty(dr["Project Name"].ToString().Trim()))
                                        {
                                            string pName = dr["Project Name"].ToString().Trim();
                                            KGREProject kGREProject = null;
                                            kGREProject = db.KGREProjects.Where(x => x.ProjectName == pName).FirstOrDefault();
                                            //if (!string.IsNullOrEmpty(kGREProject.ProjectName))
                                            if (kGREProject != null)
                                            {
                                                objKGRECustomer.Project = kGREProject.ProjectName;
                                                objKGRECustomer.ProjectName = kGREProject.ProjectName;
                                                objKGRECustomer.ProjectId = kGREProject.ProjectId;
                                            }
                                            else
                                            {
                                                ValidDisplayMessage += dr["Project Name"].ToString().Trim() + " Project name is not valid";
                                            }
                                        }

                                        objKGRECustomer.Remarks = dr["Remarks"].ToString();
                                        objKGRECustomer.ReferredBy = dr["Referred By"].ToString();


                                        int companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0;
                                        if (companyId > 0)
                                        {
                                            objKGRECustomer.CompanyId = companyId;
                                        }
                                        try
                                        {
                                            db.KGRECustomers.Add(objKGRECustomer);
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        if (objKGRECustomer.ClientId > 0)
                                        {
                                            ++s;
                                        }
                                        ModelState.Clear();
                                    }
                                }
                                else
                                {
                                }
                            }

                            if (t > 0)
                            {
                                string result = "";

                                if (s > 0)
                                {
                                    result = s + " Saved";
                                    ValidDisplayMessage = "Total number of Valid data: " + result + " Out of " + t + "\n";
                                }
                                if (u > 0)
                                {
                                    ValidDisplayMessage += "Total number of Dulicate data: " + u + " Out of " + t + "\n";
                                    ValidDisplayMessage += "\r" + dupData;
                                }
                            }

                        }

                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }
                    else
                    {
                        ValidDisplayMessage = str;
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
            string[] header = { "Campaign Name","Dealing Officer","Full Name","Mobile","Email","Job Title",
                                "Organization","Mobile 2","Email 2","Source of Media","Project Name","Interested In",
                                "Promotional Offer","Service Status","Service Detail","Gender","Date of Birth",
                                "Present Address","Permanent address","Remarks","Referred By"};
            StringBuilder errorMsg = new StringBuilder();
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            var query = dt.AsEnumerable().Where(r => r.Field<string>("Full Name") == null && r.Field<string>("Mobile") == null);
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
            if (rowcount <= 3000)
            {
                string flag = string.Empty;
                StringBuilder errorMsgClo = new StringBuilder();

                if (dt.Columns.Count == 21)
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


                if (!string.IsNullOrEmpty(dr["Project Name"].ToString()))
                {
                    using (ERPEntities unit = new ERPEntities())
                    {
                        KGREProject objProject = unit.KGREProjects.Where(x => x.ProjectName == dr["Project Name"].ToString()).FirstOrDefault();
                        if (objProject == null)
                        {
                            errorMsg = "This Project Name May not exist in Project list.";
                        }
                    }
                }
                else { errorMsg = "This Project Name required"; }

                if (!string.IsNullOrEmpty(dr["Mobile"].ToString()))
                {
                    objKgReCrmModel.MobileNo = dr["Mobile"].ToString();
                    using (ERPEntities unit = new ERPEntities())
                    {
                        KGRECustomer objCust = unit.KGRECustomers.Where(x => x.MobileNo == dr["Mobile"].ToString()).FirstOrDefault();
                        if (objCust == null)
                        {
                            errorMsg = "This Mobile is already Exist";
                        }
                    }
                }
                else { errorMsg = "Mobile 1 is not empty."; }


                if (!string.IsNullOrEmpty(dr["Full Name"].ToString()))
                {
                    objKgReCrmModel.FullName = dr["Full Name"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["Job Title"].ToString()))
                {
                    objKgReCrmModel.Designation = dr["Job Title"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["Organization"].ToString()))
                {
                    objKgReCrmModel.DepartmentOrInstitution = dr["Organization"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["Present Address"].ToString()))
                {
                    objKgReCrmModel.PresentAddress = dr["Present Address"].ToString();
                }

                if (!string.IsNullOrEmpty(dr["Permanent Address"].ToString()))
                {
                    objKgReCrmModel.PermanentAddress = dr["Permanent Address"].ToString();
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return objKgReCrmModel;
        }

        #endregion

        #region // Sold Service

        
        [HttpGet]
        public ActionResult CreateOrEditService(int id, int? companyId)
        {
            if (companyId > 0 || id > 0)
            {
                Session["CompanyId"] = companyId;
                Session["Id"] = id;
            }

            int clientId = Session["Id"] != null ? Convert.ToInt32(Session["Id"]) : 0;
            KttlCustomerModel model = kttlCustomerService.GetKttlCustomer(clientId);
            model.MaritalTypes = dropDownItemService.GetDropDownItemSelectModels(2);
            model.BloodGroups = dropDownItemService.GetDropDownItemSelectModels(5);
            model.Religions = dropDownItemService.GetDropDownItemSelectModels(9);
            model.Servicess = dropDownItemService.GetDropDownItemSelectModels(16);
            model.Genders = dropDownItemService.GetDropDownItemSelectModels(21);
            model.Titles = dropDownItemService.GetDropDownItemSelectModels(22);
            model.SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29);
            model.ClientStatuss = dropDownItemService.GetDropDownItemSelectModels(36);
            model.Professions = dropDownItemService.GetDropDownItemSelectModels(64);
            model.TypeOfClients = dropDownItemService.GetDropDownItemSelectModels(65);
            model.PassportValidities = dropDownItemService.GetDropDownItemSelectModels(66);
            model.ResponsiblePersons = kttlCustomerService.GetKTTLEmployees();
            model.ServiceYears = kttlCustomerService.GetServeiceYear();
            model.Divisions = districtService.GetDivisionSelectModels();
            model.PlacesOfBirth = districtService.GetDistrictSelectModels();
            //model.Banks = bankService.GetBankSelectModels();//Added by Ashraf 20200616

            if (model.ClientId > 0)
            {
                model.Districts = districtService.GetDistrictByDivisionId(model.Division);
                model.Upazilas = upazilaService.GetUpzilaByDistrictId(model.District);
            }
            else
            {
                model.Districts = new List<SelectModel>();
                model.Upazilas = new List<SelectModel>();
            }

            ViewBag.cHistory = GetClientHistory(clientId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEditService(KttlCustomerModel model)
        {
            //int companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0;
            int companyId = 18;
            model.CompanyId = companyId;
            string redirectPage = string.Empty;
            if (model.ClientId <= 0)
            {
                KttlCustomer kttlCustomer = db.KttlCustomers.FirstOrDefault(x => x.MobileNo == model.MobileNo);
                if (kttlCustomer != null)
                {
                    TempData["errMessage"] = "Exists";
                    return RedirectToAction("CreateOrEdit");
                }
                else
                {
                    kttlCustomerService.SaveKTTLCustomerData(0, model);
                }
                redirectPage = "Index";
            }
            else
            {
                KttlCustomer kttlCustomer = db.KttlCustomers.FirstOrDefault(x => x.ClientId == model.ClientId);
                if (kttlCustomer == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                kttlCustomerService.SaveKTTLCustomerData(model.ClientId, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit";
                ViewBag.cHistory = GetClientHistory(model.ClientId);
            }

            return RedirectToAction(redirectPage);
        }

        #endregion
    }
}
