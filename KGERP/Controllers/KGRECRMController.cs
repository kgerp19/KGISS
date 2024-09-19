using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Implementation.Realestate.CustomersBooking;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Services.Procurement;
using KGERP.Utility;
using KGERP.ViewModel;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KGRECRMController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        IKgReCrmService kgReCrmService = new KgReCrmService();
        private readonly IVoucherTypeService voucherTypeService;
        private readonly ICostHeadsService _costHeadService;
        private IFTPService _ftpservice;
        private readonly ProcurementService service;
        private readonly IGLDLCustomerService gLDLCustomerService;
        private readonly ICustomerBookingService customerBookingService;
        private readonly IBookingInstallmentService bookingInstallmentService;

        IKGREProjectService kGREProjectService = new KGREProjectService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public KGRECRMController(IBookingInstallmentService bookingInstallmentService,ICustomerBookingService customerBookingService, IGLDLCustomerService gLDLCustomerService, IVoucherTypeService voucherTypeService, ProcurementService service, ICostHeadsService costHeadService, IFTPService ftpservice)
        {
            this.voucherTypeService = voucherTypeService;
            this.service = service;
            this._ftpservice = ftpservice;
            this.customerBookingService = customerBookingService;
            this.gLDLCustomerService = gLDLCustomerService;
            this.bookingInstallmentService = bookingInstallmentService;
            _costHeadService = costHeadService;
        }
        #region // KGRE CRM
        //GET: KGRECRM
        
        [HttpGet]
        public async Task<ActionResult> AdvanceClientSearch(int? Page_No, int? ProjectId, int companyId,
             string ResponsibleOfficer = "", string SourceOfMedia = "", string Impression = "",
             string StatusLevel = "", string MobileNo = "")
        {
            string employeeName = Session["EmployeeName"].ToString();
          


            ViewBag.SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29);
            ViewBag.PromotionalOffers = dropDownItemService.GetDropDownItemSelectModels(30);
            ViewBag.PlotFlats = dropDownItemService.GetDropDownItemSelectModels(31);
            ViewBag.Impressions = dropDownItemService.GetDropDownItemSelectModels(32);
            ViewBag.StatusLevels = dropDownItemService.GetDropDownItemSelectModels(33);
            ViewBag.KGREProjects = kgReCrmService.GetProjects(companyId);
            ViewBag.KGREChoiceAreas = dropDownItemService.GetDropDownItemSelectModels(35);
            ViewBag.ResponsiblePersons = await  kgReCrmService.GetKGREClient(companyId);

            List<KgReCrmVm> kGRECustomers = null;


            kGRECustomers = db.vwKGRECustomers
                .Where(q => q.CompanyId == companyId
                )
                .Select(s => new KgReCrmVm
                {
                    ResponsibleOfficer = s.ResponsibleOfficer,
                    CompanyId = s.CompanyId,
                    FullName = s.FullName,
                    Designation = s.Designation,
                    DepartmentOrInstitution = s.DepartmentOrInstitution,
                    SourceOfMedia = s.SourceOfMedia,
                    Impression = s.Impression,
                    StatusLevel = s.StatusLevel,
                    VmProjectName = s.VmProjectName,
                    ProjectId = s.ProjectId,
                    MobileNo = s.MobileNo == null ? "" : s.MobileNo,
                    MobileNo2 = s.MobileNo2,
                    ClientId = s.ClientId,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    ModifiedBy = s.ModifiedBy,
                    EmployeeId = s.EmployeeId

                })

                .ToList();


            if (!string.IsNullOrWhiteSpace(ResponsibleOfficer))

                kGRECustomers = kGRECustomers.Where(m => m.EmployeeId == ResponsibleOfficer && m.CompanyId == companyId).ToList();

            if (!string.IsNullOrWhiteSpace(SourceOfMedia))
                kGRECustomers = kGRECustomers.Where(m => m.SourceOfMedia == SourceOfMedia && m.CompanyId == companyId).ToList();

            if (!string.IsNullOrWhiteSpace(Impression))
                kGRECustomers = kGRECustomers.Where(m => m.Impression == Impression && m.CompanyId == companyId).ToList();

            if (!string.IsNullOrWhiteSpace(StatusLevel))
                kGRECustomers = kGRECustomers.Where(m => m.StatusLevel == StatusLevel && m.CompanyId == companyId).ToList();

            if (ProjectId != null)
                kGRECustomers = kGRECustomers.Where(m => m.ProjectId == ProjectId && m.CompanyId == companyId).ToList();

            if (!string.IsNullOrWhiteSpace(MobileNo))
                kGRECustomers = kGRECustomers.Where(m => (m.MobileNo.Contains(MobileNo.Trim())
                || m.MobileNo2.Contains(MobileNo.Trim()))
                && m.CompanyId == companyId).ToList();

            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);

            return View(kGRECustomers.Where(x => x.CompanyId == companyId)
                .OrderByDescending(x => x.ClientId)
                .ToPagedList(No_Of_Page, Size_Of_Page));

        }


        
        [HttpGet]
        public async Task<ActionResult> DailyDealingOfficer(int? Page_No, string searchText, string ResponsibleOfficer, string startDate, string endDate, int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            ViewBag.ResponsiblePersons = await kgReCrmService.GetKGREClient(companyId);

            DateTime now = DateTime.Now;
            DateTime fromDate = DateTime.Now;
            DateTime toDate = DateTime.Now;
            if (startDate == null)
            {
                fromDate = DateTime.Now;
            }
            else
            {
                //fromDate = startDate.Value; 
            }
            if (endDate == null)
            {
                toDate = DateTime.Now;
            }

            else
            {
                //toDate = endDate.Value; 
            }

            List<KgReCrmModel> kgReCrmModel = null;
            int comId = (int)Session["CompanyId"];

            kgReCrmModel = kgReCrmService.DailyDealingOfficerActivity(startDate, endDate, ResponsibleOfficer);
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult ForwardedByTM(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            string employeeName = Session["EmployeeName"].ToString();
            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            searchText = searchText ?? "";

            int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
                DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "").Where(x => (x.CreatedDate >= FromDate && x.CreatedDate <= ToDate)).ToList(); ;
            }
            else
            {
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "");
            }
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);

            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                    || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                    || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                    || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId && x.ModifiedBy == "KG3570" && x.StatusLevel != "New").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId && x.ModifiedBy == "KG3570" && x.StatusLevel != "New").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
                //return View(kgReCrmModel.Where(x => x.ResponsibleOfficer == employeeName && x.StatusLevel != "New" && x.CompanyId == comId && x.ModifiedBy == "KG3570" && x.StatusLevel != "New").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult ForwardedByTMN(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            string employeeName = Session["EmployeeName"].ToString();
            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            searchText = searchText ?? "";
            //ViewBag.FromDate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd");
            //ViewBag.ToDate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd");

            int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
                DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "").Where(x => (x.CreatedDate >= FromDate && x.CreatedDate <= ToDate)).ToList(); ;
            }
            else
            {
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "");
            }
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);


            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                    || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                    || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                    || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId && x.ModifiedBy == "KG0458" && x.StatusLevel != "New").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId && x.ModifiedBy == "KG0458" && x.StatusLevel != "New").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
                //return View(kgReCrmModel.Where(x => x.ResponsibleOfficer == employeeName && x.StatusLevel != "New" && x.CompanyId == comId && x.ModifiedBy == "KG0458" && x.StatusLevel != "New").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult NewLead(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            searchText = searchText ?? "";

            string employeeName = Session["EmployeeName"].ToString();
            string s = string.Empty;
            //List<KgReCrmModel> data = null;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
                DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
                kgReCrmModel = kgReCrmService.GetKGRENewLeadList(searchText ?? "").ToList();
                //.Where(x => (x.CreatedDate >= FromDate && x.CreatedDate <= ToDate)).ToList();
                var result = kgReCrmService.GetKGRENewLeadList(searchText ?? "").Where(entry => entry.CreatedDate >= FromDate && entry.CreatedDate <= ToDate).ToList();
            }
            else
            {
                kgReCrmModel = kgReCrmService.GetKGRENewLeadList(searchText ?? "");
            }

            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);

            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
                //return View(kgReCrmModel.Where(x => x.ResponsibleOfficer == employeeName && x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult BookingLead(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            searchText = searchText ?? "";

            string employeeName = Session["EmployeeName"].ToString();
            string s = string.Empty;
            //List<KgReCrmModel> data = null;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
                DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "").Where(x => x.StatusLevel == "Booking" && x.CompanyId == 7).ToList();
            }
            else
            {
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "").Where(x => x.StatusLevel == "Booking" && x.CompanyId == 7).ToList();
            }

            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);

            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId && x.StatusLevel == "Booking").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId && x.StatusLevel == "Booking").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
                //return View(kgReCrmModel.Where(x => x.CompanyId == comId && x.StatusLevel == "Booking").OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult LeadInfo(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            int comId = (int)Session["CompanyId"];
            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
            DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
            searchText = searchText ?? "";
            ViewBag.FromDate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd");
            ViewBag.ToDate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd");


            List<KgReCrmModel> kgReCrmModel = null;
            kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "").Where(x => (x.CreatedDate >= FromDate && x.CreatedDate <= ToDate)).ToList(); ;
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);
            return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));

        }

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            string employeeName = Session["EmployeeName"].ToString().Trim();
            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
            DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
            searchText = searchText ?? "";
            ViewBag.FromDate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd");
            ViewBag.ToDate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd");

            string s = string.Empty;
            int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "").Where(x => (x.CreatedDate >= FromDate && x.CreatedDate <= ToDate)).ToList(); ;
            }
            else
            {
                kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "");
            }
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);

            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG3570")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                //return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
                return View(kgReCrmModel.Where(x => x.ResponsibleOfficer == employeeName && x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> IndexTest(int companyId)
        {
            KgReCrmModel vmCommonSupplier = new KgReCrmModel();
            vmCommonSupplier = await Task.Run(() => kgReCrmService.GetCommonClient(companyId));
            return View(vmCommonSupplier);
        }

        
        [HttpGet]
        public ActionResult IndexNewLead(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            string employeeName = Session["EmployeeName"].ToString().Trim();
            int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            kgReCrmModel = kgReCrmService.GetKGRENewLeadList(searchText ?? "");
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);

            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                //return View(kgReCrmModel.Where(x => x.ResponsibleOfficer == employeeName && x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult UpcomingClientSchedule(int? Page_No, string StartDate, string EndDate, string searchText, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);
            int comId = (int)Session["CompanyId"];
            string employeeName = Session["EmployeeName"].ToString().Trim();
            searchText = searchText ?? string.Empty;
            List<KgReCrmModel> kgReCrmModel = kgReCrmService.GetKGREClientEvent().Where(x => x.CompanyId == comId).ToList();

            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                //return View(kgReCrmModel.Where(x => x.ResponsibleOfficer == employeeName && x.CompanyId == comId).ToPagedList(No_Of_Page, Size_Of_Page));
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult ClientFollowup(int? Page_No, string StartDate, string EndDate, string searchText, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);
            int comId = (int)Session["CompanyId"];
            string employeeName = Session["EmployeeName"].ToString().Trim();
            searchText = searchText ?? string.Empty;
            List<KgReCrmModel> kgReCrmModel = kgReCrmService.GetKGREClientFollowup().Where(x => x.CompanyId == comId).ToList();

            if (System.Web.HttpContext.Current.User.Identity.Name == "KG0198"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0193"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0041"
                   || System.Web.HttpContext.Current.User.Identity.Name == "KG0002")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                //return View(kgReCrmModel.Where(x => x.ResponsibleOfficer == employeeName && x.CompanyId == comId).ToPagedList(No_Of_Page, Size_Of_Page));
                return View(kgReCrmModel.Where(x => x.CompanyId == comId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult Previous7DaysClientSchedule(int? Page_No, string searchText, int companyId)
        {
            searchText = searchText ?? string.Empty;
            List<KgReCrmModel> kgReCrmModels = kgReCrmService.GetPrevious7DaysClientSchedule().Where(x => x.CompanyId == companyId).ToList();
            int Size_Of_Page = 3000;
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
        public ActionResult CreateOrEdit(int? id, string searchText, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            KgReCrmViewModel vm = new KgReCrmViewModel();
            vm._KgReCrmModel = kgReCrmService.GetKGRClientById(id);
            vm.Religions = dropDownItemService.GetDropDownItemSelectModels(9);
            vm.Genders = dropDownItemService.GetDropDownItemSelectModels(3);
            vm.SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29);
            vm.PromotionalOffers = dropDownItemService.GetDropDownItemSelectModels(30);
            vm.PlotFlats = dropDownItemService.GetDropDownItemSelectModels(31);
            vm.Impressions = dropDownItemService.GetDropDownItemSelectModels(32);
            vm.StatusLevels = dropDownItemService.GetDropDownItemSelectModels(33);
            vm.KGREProjects = kgReCrmService.GetProjects(companyId);
            vm.KGREChoiceAreas = dropDownItemService.GetDropDownItemSelectModels(35);
            vm.ResponsiblePersons = kgReCrmService.GetKGREClient();
            // vm.KGRECustomers = GetClientHistory(id);

            if (id > 0)
            {
                ViewBag.cHistory = GetClientHistoryOld(id);
                ViewBag.sHistory = GetServiceHistory(id);
            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult CheckMobileNo(string mobileNo, int companyId)
        {

            string result = null;


            if(mobileNo.Length<=11 && mobileNo.StartsWith("0"))
            {
                var dataList = db.KGRECustomers
                                .SingleOrDefault(q => q.CompanyId == companyId && (q.MobileNo == mobileNo.Trim()
                                || q.MobileNo2 == mobileNo.Trim())
                                );

                if (dataList == null)
                {
                    result = null;
                }
                else
                {
                    var companyName = db.Companies.SingleOrDefault(q => q.CompanyId == dataList.CompanyId)?.Name;
                    result = "Already Exist : Client Name :" + dataList.FullName + "and Company is : " + companyName;
                }



            }
            else
            {
                result = "Either mobile is greater than 11 or not strat with 0 ";
            }



            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult MobileNoValidationAndUpdate(string mobileNo, int companyId,int id)
        {

            string result = null;
            var dataList = db.KGRECustomers
                .SingleOrDefault(q => q.CompanyId == companyId &&q.ClientId!=id&& (q.MobileNo == mobileNo.Trim()
                || q.MobileNo2 == mobileNo.Trim())
                );


            if (dataList == null)
            {
                result = "null";
            }
            else
            {
                var companyName = db.Companies.SingleOrDefault(q => q.CompanyId == dataList.CompanyId)?.Name;
                result = "Already Exist : Client Name :" + dataList.FullName + "and Company is : " + companyName;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // POST: KGRECRM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEdit(KgReCrmViewModel model, FormCollection formcollection)
        {
            model._KgReCrmModel.Project = formcollection["ProjectName"];
            string redirectPage = string.Empty;
            int companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0;
            if (string.IsNullOrEmpty(model._KgReCrmModel.MobileNo)
                && string.IsNullOrEmpty(model._KgReCrmModel.Email))
            {
                TempData["errMessage"] = "me";
                return RedirectToAction("CreateOrEdit", new { companyId = companyId });
            }
            if (model._KgReCrmModel.ClientId <= 0)
            {
                KGRECustomer kGRECustomer = db.KGRECustomers
                    .FirstOrDefault(x => ((x.MobileNo == model._KgReCrmModel.MobileNo
                    || x.MobileNo2 == model._KgReCrmModel.MobileNo)
                    && x.CompanyId == companyId ));
                //KGRECustomer kGRECustomer = db.Database.SqlQuery<KGRECustomer>("Select * from KGRECustomer where MobileNo='" + model._KgReCrmModel.MobileNo + "' or Email='" + model._KgReCrmModel.Email + "'").FirstOrDefault();
                if (kGRECustomer != null)
                {
                    TempData["errMessage"] = "Exists";
                    return RedirectToAction("CreateOrEdit", new { companyId = companyId });
                }
                else
                {
                    model._KgReCrmModel.CompanyId = companyId;
                    kgReCrmService.SaveKGREClient(0, model._KgReCrmModel);
                }
                redirectPage = "Index";

            }
            else
            {
                KGRECustomer kGRECustomer = db.KGRECustomers
                    .FirstOrDefault(x => x.ClientId == model._KgReCrmModel.ClientId);
                if (kGRECustomer == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                // model._KgReCrmModel.CompanyId = companyId;
                kgReCrmService.SaveKGREClient(model._KgReCrmModel.ClientId, model._KgReCrmModel);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit";
                ViewBag.cHistory = GetClientHistory(model._KgReCrmModel.ClientId);
            }
            return RedirectToAction(redirectPage, new { companyId = companyId });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public IEnumerable<KGRECustomer> GetClientHistory(int id)
        {
            return GetClientHistoryByClientId(id);
        }
        public SqlDataReader Reader { get; set; }
        private IEnumerable<KGRECustomer> GetClientHistoryByClientId(int KGREId)
        {
            KGRECustomer aRicive = new KGRECustomer();
            List<KGRECustomer> aList = new List<KGRECustomer>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("KGRE_GetChangeHistory", con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@KGREId", KGREId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {

                        aRicive.ClientId = Convert.ToInt32(Reader["KGREId"].ToString());
                        aRicive.FullName = Reader["Name"].ToString();
                        aRicive.CreatedDate = Convert.ToDateTime(Reader["Date"].ToString());
                        aRicive.Remarks = Reader["Remarks"].ToString();
                        aRicive.PresentAddress = Reader["Flag"].ToString();
                        aList.Add(aRicive);
                    }
                }
                con.Close();
            }

            return aList;
        }
        public string GetClientHistoryOld(int? id)
        {
            string htmlStr = "<table width='100%' align='center' cellpadding='2' cellspacing='2' border='0' bgcolor='#F0F0F0'> ";
            DataTable dt = new DataTable();
            dt = GetClientHistoryByClientIdOld(id);
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

                // Taking a string 
                var str = dt.Rows[i]["Remarks"].ToString();
                var result = str.Split('*');
                //char[] spearator = { '*' };
                //int count = 1;
                char[] spearator = { '*' };
                Int32 count = 2;
                // Using the Method 
                string[] strlist = str.Split(spearator, 1, StringSplitOptions.RemoveEmptyEntries);
                string changehistory = "";
                foreach (string s in result)
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
                    htmlStr += "<tr><td class='papi'><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: " + changehistory + " </td></tr>";
                }
                else
                {
                    htmlStr += "<tr style=" + style + "><td> <span title=" + strDateTime2 + ">Changed " + NrOfDays + " ago by</span> <b> " + dt.Rows[i]["Name"].ToString() + "</b></td></tr>";
                    htmlStr += "<tr style='page-break-inside: avoid;'><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: <BR>" + changehistory + " </td></tr>";
                }
            }

            return htmlStr += "</table>";
        }
        public string GetServiceHistory(int? id)
        {
            string htmlStr = "<table width='100%' align='center' cellpadding='2' cellspacing='2' border='0' bgcolor='#F0F0F0'> ";
            DataTable dt = new DataTable();
            dt = GetServiceHistoryByClientId(id);
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

                // Taking a string 
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
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: <BR>" + changehistory + " </td></tr>";
                }
            }

            return htmlStr += "</table>";
        }
        private DataTable GetClientHistoryByClientIdOld(int? KGREId)
        {
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
        private DataTable GetServiceHistoryByClientId(int? KGREId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("KGRE_GetChangedServiceHistory", con))
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
        public ActionResult ExportKGREClientToExcel(string Impression, string ResponsibleOfficer, string SourceOfMedia, string StatusLevel)
        {
            int comId = (int)Session["CompanyId"];
            var gv = new GridView();
            string fileName = string.Empty;
            if (comId == 7)
            {
                fileName = "GLDL Lead List";
            }
            else
            {
                fileName = "KPL Lead List";
            }
            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (!string.IsNullOrEmpty(Impression) || !string.IsNullOrEmpty(ResponsibleOfficer) || !string.IsNullOrEmpty(SourceOfMedia) || !string.IsNullOrEmpty(StatusLevel))
            {
                dt = GetClientList(comId, Impression, ResponsibleOfficer, SourceOfMedia, StatusLevel);
            }
            else
            {
                dt = GetClientList(comId);
            }


            gv.DataSource = dt;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= " + fileName + ".xls");
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
        public DataTable GetClientList(int comId, string Impression, string ResponsibleOfficer, string SourceOfMedia, string StatusLevel)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("KGRE_ExportClientLeadListWithP", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyId", comId);
                        cmd.Parameters.AddWithValue("@Impression", Impression);
                        cmd.Parameters.AddWithValue("@ResponsibleOfficer", ResponsibleOfficer);
                        cmd.Parameters.AddWithValue("@SourceOfMedia", SourceOfMedia);
                        cmd.Parameters.AddWithValue("@StatusLevel", StatusLevel);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
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

        public DataTable GetClientList(int comId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("KGRE_ExportClientLeadList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyId", comId);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
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

        
        [HttpGet]
        public ActionResult ExistingClientList(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
            DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
            searchText = searchText ?? "";
            ViewBag.FromDate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd");
            ViewBag.ToDate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd");

            string s = string.Empty;
            int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            kgReCrmModel = kgReCrmService.GetKGREExistingLeadList(searchText ?? "").ToList();
            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);
            return View(kgReCrmModel.Where(x => x.CompanyId == comId).ToPagedList(No_Of_Page, Size_Of_Page));

        }
        #endregion

        #region // Batch Upload
        
        public ActionResult UploadClientBatch(int? companyId, KgreCrmBulkVM vm)
        {
            int comId;
            if (companyId > 0)
            {

                Session["CompanyId"] = companyId;
                comId = (int)Session["CompanyId"];
            }
            vm = vm == null ? new KgreCrmBulkVM() : vm;
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult UploadClientBatch(KgreCrmBulkVM file)
        {
            try
            {
                KgreCrmBulkVM vM = new KgreCrmBulkVM();
                vM = UploadExcelFile(file);
                vM.CompanyId = file.CompanyId;
                return View(vM);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private KgreCrmBulkVM UploadExcelFile(KgreCrmBulkVM file)
        {

            var ReturnVM = new KgreCrmBulkVM();
            ReturnVM.CompanyId = file.CompanyId;
            int companyId2 = file.CompanyId;
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
                var path = Path.Combine(Path.GetTempPath(), fileName);

                if (strFileType == ".xls" || strFileType == ".xlsx")
                {
                    try
                    {
                        file.ExcelFile.SaveAs(path);
                    }
                    catch (Exception ex)
                    {
                        ReturnVM.Status = "Failed!!! Cann't read file";
                        return ReturnVM;
                    }
                }
                else
                {
                    ReturnVM.Status = "Failed!!! Invalid File Type";
                    return ReturnVM;
                }
                if (strFileType.Trim() == ".xls")
                {
                    connString = string.Format(ConfigurationManager.ConnectionStrings["Excel03ConString"].ToString(), path);//"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if (strFileType.Trim() == ".xlsx")
                {
                    connString = string.Format(ConfigurationManager.ConnectionStrings["Excel07ConString"].ToString(), path);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }
                else
                {
                    ReturnVM.Status = "Failed!!! Invalid File Type";

                    return ReturnVM;
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

                    dt = ds.Tables[0];
                    DateTime defaultDate = Convert.ToDateTime("1900-01-01");
                    List<KgreCrmBulk> CustomerList = new List<KgreCrmBulk>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["Full Name"].ToString().Length > 4)
                        {
                            var objKGRECustomer = new KgreCrmBulk();
                            objKGRECustomer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            objKGRECustomer.CreatedDate = DateTime.Now;
                            objKGRECustomer.DateOfContact = DateTime.Now;
                            objKGRECustomer.DateofBirth = !string.IsNullOrEmpty(dr["Date of Birth"].ToString()) ? Convert.ToDateTime(dr["Date of Birth"].ToString()) : defaultDate;
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
                            objKGRECustomer.Gender = dr["Gender"].ToString();
                            objKGRECustomer.TypeOfInterest = dr["Interested In"].ToString();
                            objKGRECustomer.StatusLevel = !string.IsNullOrEmpty(dr["Service Status"].ToString()) ? dr["Service Status"].ToString() : "New";
                            objKGRECustomer.Project = dr["Project Name"].ToString().Trim();
                            objKGRECustomer.ProjectName = "";
                            objKGRECustomer.ProjectId = 0;
                            objKGRECustomer.Remarks = dr["Remarks"].ToString();
                            objKGRECustomer.ReferredBy = dr["Referred By"].ToString();
                            objKGRECustomer.CompanyId = file.CompanyId;
                            CustomerList.Add(objKGRECustomer);
                        }


                    }
                    Storeproc storeproc = new Storeproc();

                    string constring = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                    SqlConnection sqlConnection2 = new SqlConnection(constring);

                    //Process Customer Data
                    SqlParameter[] parameters2 = new SqlParameter[2];

                    SqlParameter desc12 = new SqlParameter(parameterName: "@CompanyId", dbType: System.Data.SqlDbType.NVarChar);
                    desc12.Value = file.CompanyId;
                    SqlParameter desc22 = new SqlParameter(parameterName: "@Dxml01", dbType: System.Data.SqlDbType.NVarChar);
                    var x = ListToXml.ToXml<List<KgreCrmBulk>>(CustomerList, "ds").Replace("d3p1:nil=\"true\" xmlns:d3p1=\"http://www.w3.org/2001/XMLSchema-instance" + "\"", "");

                    desc22.Value = x.Replace("\'", "");
                    parameters2[0] = desc12;
                    parameters2[1] = desc22;
                    var result2 = storeproc.GetDataSet(sqlConnection2, "[dbo].[KGRECustomerBulkEntry]", parameters2);
                    if (result2.Tables[0].Columns.Contains("errornumber"))
                    {
                        ReturnVM.Status = "Error occured on Insertion, Please check data";
                        return ReturnVM;
                    }
                    else
                    {
                        ReturnVM.ResponseList = DtToList.ConvertDataTable<KgreCrmBulk>(result2.Tables[0]);
                        int Inserted = ReturnVM.ResponseList.Where(e => e.EntryStatus == "0").ToList().Count();
                        int AlreadyExists = ReturnVM.ResponseList.Count - Inserted;
                        ReturnVM.Status = $"Successfully Inserted Customer {Inserted} of {ReturnVM.ResponseList.Count}. Already Exists: {AlreadyExists}";
                        return ReturnVM;
                    }
                }
                catch (Exception ex)
                {
                    ReturnVM.Status = $"Failed!!! {ex.Message}";
                    return ReturnVM;
                }
            }
            else
            {
                ReturnVM.Status = "Failed!!! File not found";
                return ReturnVM;
            }
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
        #region Export Lead
        
        [HttpGet]
        public ActionResult ExportDataToExcel(string ResponsibleOfficer, string SourceOfMedia, string StatusLevel)
        {
            var gv = new GridView();
            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (!string.IsNullOrEmpty(ResponsibleOfficer) || !string.IsNullOrEmpty(SourceOfMedia) || !string.IsNullOrEmpty(StatusLevel))
            {
                dt = GetFilterDataList(ResponsibleOfficer, SourceOfMedia, StatusLevel);
            }
            else
            {
                dt = GetFilterDataList();
            }
            if (dt.Rows.Count > 0)
            {
                string[] selectedColumns = new[] { "ClientId", "Designation", "FullName AS [Full Name]", "DepartmentOrInstitution AS [Organization]", "PresentAddress", "PermanentAddress", "DateofBirth", "DateOfContact", "LastContactDate", "Nationality", "CampaignName", "MobileNo", "MobileNo2", "Email", "Email1", "Gender", "SourceOfMedia", "PromotionalOffer", "Impression", "StatusLevel", "TypeOfInterest", "Project", "ChoieceOfArea", "ReferredBy", "ServicesDescription", "NextScheduleDate", "LastMeetingDate", "ResponsibleOfficer", "CompanyId", "FinalStatus", "Remarks", "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate" };
                DataTable dt1 = new DataView(dt).ToTable(false, selectedColumns);
                gv.DataSource = dt1;
                gv.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename= KG_CaseList.xls");
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
            else
            {
                return View();
            }
        }

        public DataTable GetFilterDataList(string ResponsibleOfficer, string SourceOfMedia, string StatusLevel)
        {
            try
            {
                DataTable dt = new DataTable();
                IQueryable<KGRECustomer> _KGRECustomer = null;
                List<KgReCrmModel> _KGRECustomerModel = null;
                _KGRECustomer = db.KGRECustomers;

                if (!string.IsNullOrWhiteSpace(ResponsibleOfficer))
                    _KGRECustomer = _KGRECustomer.Where(m => m.ResponsibleOfficer == ResponsibleOfficer);

                if (!string.IsNullOrWhiteSpace(SourceOfMedia))
                {
                    _KGRECustomer = _KGRECustomer.Where(m => m.SourceOfMedia == SourceOfMedia);
                }

                if (!string.IsNullOrWhiteSpace(StatusLevel))
                {
                    _KGRECustomer = _KGRECustomer.Where(m => m.StatusLevel == StatusLevel);
                }

                _KGRECustomerModel = ObjectConverter<KGRECustomer, KgReCrmModel>.ConvertList(_KGRECustomer.ToList()).ToList();

                if (_KGRECustomerModel != null)
                {
                    dt = CreateDataTable(_KGRECustomerModel);
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        private DataTable CreateDataTable(IList<KgReCrmModel> item)
        {
            Type type = typeof(KgReCrmModel);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (KgReCrmModel entity in item)
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

        public DataTable GetFilterDataList()
        {
            try
            {
                DataTable dt = new DataTable();
                IQueryable<KGRECustomer> _KGRECustomer = null;
                _KGRECustomer = db.KGRECustomers;
                if (_KGRECustomer != null)
                {
                    dt = (DataTable)_KGRECustomer;
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        #endregion

        #region Add Dashboard 2020-10-19

        [HttpGet]
        
        public ActionResult DetailedClientReport(string ReportName, string ReportDescription)
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

        
        public ActionResult GLDLDashboard(int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            int comId = (int)Session["CompanyId"];
            DataTable dt = GetClientStatus(comId);
            if (dt.Rows.Count > 0)
            {
                ViewData.Model = dt.AsEnumerable();
            }

            //DataTable dt2 = GetClientStatus();
            //if (dt2.Rows.Count > 0)
            //{
            //    TempData["data"] = dt2.AsEnumerable();
            //}

            //var query = db.KGRECustomers
            //   .GroupBy(p => p.StatusLevel)
            //   .Select(g => new DisplayReport { Name = g.Key, Count = g.Count() })
            //   .OrderByDescending(g => g.Count);
            //ViewBag.ServiceYear = query;


            return View();
        }

        
        public ActionResult GetResponsiblePersonChartImage(int? companyId)
        {
            DataTable dt = GetResponsiblePerson();
            var key = (dynamic)null;
            if (dt.Rows.Count > 0)
            {
                List<string> gender = (from p in dt.AsEnumerable() select p.Field<string>("ResponsibleOfficer")).Distinct().ToList();
                int countGender = gender.Count; //countGender

                List<string> gender2 = new List<string>();
                List<double> genderCount = new List<double>();

                foreach (DataRow row in dt.Rows)
                {
                    gender2.Add((string)Convert.ToString(row["ResponsibleOfficer"]));
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
                       chartType: "Pie",
                       name: "Client",
                       xField: "ResponsibleOfficer",
                       yFields: "Total",

                       xValue: outputgender2,
                       yValues: outputgenderCount);
                }
            }
            return File(key.ToWebImage().GetBytes(), "image/jpeg");

        }

        
        public ActionResult GetSourceOfMediaChartImage(int? companyId)
        {
            DataTable dt = GetSourceOfMedia();
            var key = (dynamic)null;
            if (dt.Rows.Count > 0)
            {
                List<string> gender = (from p in dt.AsEnumerable() select p.Field<string>("SourceOfMedia")).Distinct().ToList();
                int countGender = gender.Count; //countGender

                List<string> gender2 = new List<string>();
                List<double> genderCount = new List<double>();

                foreach (DataRow row in dt.Rows)
                {
                    gender2.Add((string)Convert.ToString(row["SourceOfMedia"]));
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
                       .AddTitle("Source of Media Ratio")
                       .AddSeries(
                       chartType: "column",
                       name: "Source of Media",
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
                    using (SqlCommand cmd = new SqlCommand("Select  ResponsibleOfficer, Count(*) As Total from KGRECustomer where companyId=7 Group by ResponsibleOfficer"))
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

        public DataTable GetClientStatus(int? companyId)
        {
            string sproc = string.Empty;
            if (companyId == 9)
            {
                sproc = "KGRE_GetStatusLevelKPL";
            }
            else
            {
                sproc = "KGRE_GetStatusLevel";
            }
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(sproc, con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
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

        public DataTable GetSourceOfMedia()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("Select  SourceOfMedia, Count(*) As Total from KGRECustomer  where companyId =7 Group by SourceOfMedia"))
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

        
        [HttpGet]
        public ActionResult GetStatusWiseClient(int? Page_No, string searchText, int? companyId, string StatusLevel)
        {
            string statusLevel = string.Empty;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if (!string.IsNullOrEmpty(StatusLevel))
            {
                Session["StatusLevel"] = StatusLevel;
                statusLevel = Session["StatusLevel"].ToString();

            }
            //int comId = (int)Session["CompanyId"];
            List<KgReCrmModel> kgReCrmModel = null;
            kgReCrmModel = kgReCrmService.GetKGRELeadList(searchText ?? "");
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            if (statusLevel == "Total")
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(kgReCrmModel.Where(x => x.CompanyId == companyId && x.StatusLevel == statusLevel).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));
            }

        }

        #endregion

        #region // Salse Information 

        
        [HttpGet]
        public ActionResult LoadKgReAllLeadList(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            string employeeName = Session["EmployeeName"].ToString().Trim();
            DateTime date = DateTime.Now;
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddMonths(1).AddDays(-1);
            DateTime FromDate = StartDate == null ? firstOfNextMonth : Convert.ToDateTime(StartDate);
            DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);
            searchText = searchText ?? "";
            ViewBag.FromDate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd");
            ViewBag.ToDate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd");

            string s = string.Empty;
            int comId = (int)Session["CompanyId"];
            List<KgReCrmVm> kgReCrmModel = null;

            kgReCrmModel = db.vwKGRECustomers
                .Select(s1 => new KgReCrmVm
                {
                    ResponsibleOfficer = s1.ResponsibleOfficer,
                    CompanyId = s1.CompanyId,
                    FullName = s1.FullName,
                    Designation = s1.Designation,
                    DepartmentOrInstitution = s1.DepartmentOrInstitution,
                    SourceOfMedia = s1.SourceOfMedia,
                    Impression = s1.Impression,
                    StatusLevel = s1.StatusLevel,
                    VmProjectName = s1.VmProjectName,
                    ProjectId = s1.ProjectId,
                    MobileNo = s1.MobileNo,
                    ClientId = s1.ClientId,
                    CreatedBy = s1.CreatedBy,
                    CreatedDate = s1.CreatedDate,
                    ModifiedBy = s1.ModifiedBy,
                    Remarks = s1.Remarks,
                    EmployeeId = s1.EmployeeId,
                    MobileNo2 = s1.MobileNo2

                })
                .Where(q => q.CompanyId == comId)
                .ToList();



            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                kgReCrmModel = kgReCrmModel.Where(x => (x.CreatedDate >= FromDate && x.CreatedDate <= ToDate)).ToList(); ;
            }

            int Size_Of_Page = 3000;
            int No_Of_Page = (Page_No ?? 1);


            return View(kgReCrmModel.Where(x => x.CompanyId == comId).OrderByDescending(x => x.ClientId).ToPagedList(No_Of_Page, Size_Of_Page));

        }

        
        [HttpGet]
        public ActionResult ClientPaymentList(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadKgReClientPaymentList()
        {
            try
            {
                var list = kgReCrmService.LoadBookingListPaymentInfo();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        
        [HttpGet]
        public ActionResult IndividualLeadList(int? Page_No, string searchText, string StartDate, string EndDate, int? companyId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadKgReLeadList()
        {
            try
            {
                var list = kgReCrmService.LoadBookingListInfo();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        
        [HttpGet]
        public async Task<ActionResult> DetailsBlockClient(int? id, string searchText, int companyId)
        {
            if (Request.QueryString["id"] != null)
            {
                id = Convert.ToInt32(Request.QueryString["id"].ToString());
            }
            else
            {
                id = 0;
            }

            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            KgReCrmViewModel vm = new KgReCrmViewModel();
            vm._KgReCrmModel = kgReCrmService.GetKGRClientById(id);
            if (vm._KgReCrmModel.BookingDate != null)
            {
                KGREPlotBookingModel _KGREPlotBookingModel = db.Database.SqlQuery<KGREPlotBookingModel>("KGRE_GetPlotBookingByClientId {0}", vm._KgReCrmModel.ClientId).FirstOrDefault();
                if (_KGREPlotBookingModel != null)
                {
                    vm.kGREPlotBookingModel = _KGREPlotBookingModel;
                    if (vm.kGREPlotBookingModel.PlotId > 0)
                    {
                        vm.kGREPlotModel = kGREProjectService.GetKGREPlot((int)vm.kGREPlotBookingModel.PlotId);
                    }
                }
            }

            vm.Genders = dropDownItemService.GetDropDownItemSelectModels(3);
            vm.SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29);
            vm.PromotionalOffers = dropDownItemService.GetDropDownItemSelectModels(30);
            vm.PlotFlats = dropDownItemService.GetDropDownItemSelectModels(31);
            vm.Impressions = dropDownItemService.GetDropDownItemSelectModels(32);
            vm.StatusLevels = dropDownItemService.GetDropDownItemSelectModels(33);
            vm.SalesTypes = dropDownItemService.GetDropDownItemSelectModels(63);
            vm.KGREChoiceAreas = dropDownItemService.GetDropDownItemSelectModels(35);
            vm.ResponsiblePersons = await kgReCrmService.GetKGREClient(companyId);

            vm.KGREProjects =  kgReCrmService.GetProjects(companyId);
            if (vm._KgReCrmModel.ProjectId > 0)
            {
                vm.Plots = kGREProjectService.GetKGREPlots(vm._KgReCrmModel.ProjectId);
            }
            else
            {
                vm.Plots = new List<SelectModel>();
            }
            return View(vm);
        }

        
        [HttpGet]
        public async Task<ActionResult> CreateOrEditBooking(int? id, string searchText, int companyId)
        {
            if (companyId > 0 || id > 0)
            {
                Session["CompanyId"] = companyId;
                Session["Id"] = id;
            }

            int clientId = Session["Id"] != null ? Convert.ToInt32(Session["Id"]) : 0;
            KgReCrmViewModel vm = new KgReCrmViewModel();
            vm._KgReCrmModel = kgReCrmService.GetKGRClientById(clientId);
            if (vm._KgReCrmModel.BookingDate != null)
            {
                KGREPlotBookingModel _KGREPlotBookingModel = db.Database.SqlQuery<KGREPlotBookingModel>("Select * from KGREPlotBooking where ClientId='" + vm._KgReCrmModel.ClientId + "' and BookingDate =(Select BookingDate from KGRECustomer where ClientId=" + vm._KgReCrmModel.ClientId + ")").FirstOrDefault();
                if (_KGREPlotBookingModel != null)
                {
                    vm.kGREPlotBookingModel = _KGREPlotBookingModel;
                    if (vm.kGREPlotBookingModel.PlotId > 0)
                    {
                        vm.kGREPlotModel = kGREProjectService.GetKGREPlot((int)vm.kGREPlotBookingModel.PlotId);
                    }
                }
            }

            vm.Religions = dropDownItemService.GetDropDownItemSelectModels(9);
            vm.Genders = dropDownItemService.GetDropDownItemSelectModels(3);
            vm.SourceOfMedias = dropDownItemService.GetDropDownItemSelectModels(29);
            vm.PromotionalOffers = dropDownItemService.GetDropDownItemSelectModels(30);
            vm.PlotFlats = dropDownItemService.GetDropDownItemSelectModels(31);
            vm.Impressions = dropDownItemService.GetDropDownItemSelectModels(32);
            vm.StatusLevels = dropDownItemService.GetDropDownItemSelectModels(33);
            vm.SalesTypes = dropDownItemService.GetDropDownItemSelectModels(63);
            vm.KGREChoiceAreas = dropDownItemService.GetDropDownItemSelectModels(35);
            vm.ResponsiblePersons = await kgReCrmService.GetKGREClient(companyId);

            vm.KGREProjects =  kgReCrmService.GetProjects(companyId);
            if (vm._KgReCrmModel.ProjectId > 0)
            {
                vm._KgReCrmModel.ClientId = vm._KgReCrmModel.ClientId;
                vm.KGREProjects =  kgReCrmService.GetProjects(companyId);

                vm.Plots = kGREProjectService.GetKGREPlots(vm._KgReCrmModel.ProjectId);
            }
            else
            {
                vm.Plots = new List<SelectModel>();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEditBooking(KgReCrmViewModel model, FormCollection formcollection)
        {
            string redirectPage = string.Empty;
            //int companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0;
            int companyId = 7;
            KGRECustomer kGRECustomer = db.KGRECustomers
                .FirstOrDefault(x => x.ClientId == model._KgReCrmModel.ClientId);
            if (kGRECustomer == null)
            {
                TempData["errMessage1"] = "Data not found!";
                return RedirectToAction("CreateOrEditBooking");
            }

            model._KgReCrmModel.CompanyId = companyId;
            bool u = kgReCrmService.SaveKGREClientBooking(model._KgReCrmModel.ClientId, model._KgReCrmModel);
            #region File Upload
            if (u == true && (model._KgReCrmModel.ClientId > 0))
            {
                int clientId = model._KgReCrmModel.ClientId;
                var clientData = db.KGRECustomers.Where(x => x.ClientId == clientId).FirstOrDefault();
                if (clientData != null)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            FileAttachment fileDetail = new FileAttachment()
                            {
                                AttachFileName = clientData.ClientId + "_" + fileName,
                                CompanyId = clientData.CompanyId,
                                KgreClientId = clientId,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                ModifiedDate = DateTime.Now
                            };

                            string folder = Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGRE"));
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                                var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGRE")), clientId + "_" + fileName);
                                file.SaveAs(path1);
                            }
                            else
                            {
                                var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGRE")), clientId + "_" + fileName);
                                file.SaveAs(path1);
                            }

                            db.Entry(fileDetail).State = EntityState.Added;
                            db.SaveChanges();
                        }
                    }
                }
                #endregion

                if (u)
                {
                    bool bresult = SaveKGREClientBooking1(0, model);
                    if (bresult)
                    {
                        bool installment = SaveKGREClientBookingInstallment(0, model);
                    }
                }
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "IndividualLeadList";
            }
            return RedirectToAction(redirectPage, new { companyId = companyId });
        }

        private bool SaveKGREClientBookingInstallment(int v, KgReCrmViewModel model)
        {
            bool result = false;
            KGREInstallment _KGREInstallment = new KGREInstallment();
            KGREInstallment _KGREInstallment1 = new KGREInstallment();
            if (model._KgReCrmModel.SaveStatus == false)
            {
                return result;
            }
            else
            {
                using (ERPEntities db = new ERPEntities())
                {
                    DateTime Sdt = DateTime.Now;

                    if (model._KgReCrmModel.BookingDate != null)
                    {
                        Sdt = Convert.ToDateTime(model._KgReCrmModel.BookingDate);
                    }

                    _KGREInstallment1 = db.KGREInstallments.OrderByDescending(x => x.InstallmentId).FirstOrDefault(x => x.PlotId == model.kGREPlotModel.PlotId && x.ClientId == model._KgReCrmModel.ClientId);
                    if (model.kGREPlotBookingModel.NoOfInstallment != null)
                    {
                        if (_KGREInstallment1 == null)
                        {
                            for (int i = 1; i <= model.kGREPlotBookingModel.NoOfInstallment; i++)
                            {
                                _KGREInstallment1 = db.KGREInstallments.OrderByDescending(x => x.InstallmentId).FirstOrDefault(x => x.PlotId == model.kGREPlotModel.PlotId && x.ClientId == model._KgReCrmModel.ClientId);

                                if (_KGREInstallment1 != null)
                                {
                                    Sdt = GetInstallmentDate(model, Convert.ToDateTime(_KGREInstallment1.InstallmentDate));
                                }
                                _KGREInstallment.ClientId = model._KgReCrmModel.ClientId;
                                _KGREInstallment.PlotId = model.kGREPlotModel.PlotId;
                                _KGREInstallment.ProjectId = model._KgReCrmModel.ProjectId;
                                _KGREInstallment.PaymentDate = Sdt;
                                _KGREInstallment.InstallmentDate = Sdt;
                                _KGREInstallment.InstallmentAmount = model.kGREPlotBookingModel.InstallmentAmount;
                                _KGREInstallment.InstallmentStatus = model.kGREPlotBookingModel.InstallmentStatus;
                                _KGREInstallment.Remarks = model.kGREPlotBookingModel.BookingNote;
                                _KGREInstallment.BookingId = Convert.ToInt32(model.kGREPlotBookingModel.BookingId);
                                _KGREInstallment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                _KGREInstallment.CreatedDate = DateTime.Now;
                                _KGREInstallment.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                _KGREInstallment.ModifiedDate = DateTime.Now;
                                try
                                {
                                    db.KGREInstallments.Add(_KGREInstallment);
                                    if (db.SaveChanges() > 0)
                                    {
                                        //return result = true;
                                    }
                                    else
                                    {
                                        ///return result;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("KGREInstallments Error " + ex);
                                }
                            }
                            return result = true;
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
            }
        }

        private DateTime GetInstallmentDate(KgReCrmViewModel model, DateTime bookingdate)
        {
            DateTime Sdt = DateTime.Now;
            if (bookingdate != null)
            {
                Sdt = bookingdate;
            }
            else
            {
                Sdt = Convert.ToDateTime(model._KgReCrmModel.BookingDate);
            }
            if (model.kGREPlotBookingModel.SalesTypeId == 475)
            {
                Sdt = Sdt;
            }
            if (model.kGREPlotBookingModel.SalesTypeId == 476)
            {
                Sdt = Sdt.AddMonths(1).AddDays(-1);
            }
            if (model.kGREPlotBookingModel.SalesTypeId == 477)
            {
                Sdt = Sdt.AddMonths(12).AddDays(-1);
            }
            if (model.kGREPlotBookingModel.SalesTypeId == 478)
            {
                Sdt = Sdt.AddMonths(6).AddDays(-1);
            }
            if (model.kGREPlotBookingModel.SalesTypeId == 479)
            {
                Sdt = Sdt.AddMonths(3).AddDays(-1);
            }

            return Sdt;
        }
        public bool SaveKGREClientBooking1(int id, KgReCrmViewModel model)
        {
            bool result = false;
            KGREPlotBooking _KGREPlotBooking = new KGREPlotBooking();
            using (ERPEntities db = new ERPEntities())
            {
                if (model._KgReCrmModel.ClientId > 0)
                {
                    _KGREPlotBooking = db.KGREPlotBookings.FirstOrDefault(x => x.BookingId == model.kGREPlotBookingModel.BookingId);
                    _KGREPlotBooking.PlotId = model.kGREPlotModel.PlotId;
                    _KGREPlotBooking.LandPricePerKatha = model.kGREPlotBookingModel.LandPricePerKatha;
                    _KGREPlotBooking.LandValue = Convert.ToInt32(model.kGREPlotBookingModel.LandValue);
                    _KGREPlotBooking.Discount = Convert.ToInt32(model.kGREPlotBookingModel.Discount);
                    _KGREPlotBooking.LandValueAfterDiscount = Convert.ToInt32(model.kGREPlotBookingModel.LandValueAfterDiscount);
                    _KGREPlotBooking.Additional25Percent = Convert.ToInt32(model.kGREPlotBookingModel.Additional25Percent);
                    _KGREPlotBooking.Additional15Percent = Convert.ToInt32(model.kGREPlotBookingModel.Additional15Percent);
                    _KGREPlotBooking.Additional10Percent = Convert.ToInt32(model.kGREPlotBookingModel.Additional10Percent);
                    _KGREPlotBooking.AdditionalCost = Convert.ToInt32(model.kGREPlotBookingModel.AdditionalCost);
                    _KGREPlotBooking.UtilityCost = Convert.ToInt32(model.kGREPlotBookingModel.UtilityCost);
                    _KGREPlotBooking.GrandTotal = Convert.ToInt32(model.kGREPlotBookingModel.GrandTotal);
                    _KGREPlotBooking.BookingMoney = Convert.ToInt32(model.kGREPlotBookingModel.BookingMoney);
                    _KGREPlotBooking.BookingDate = model._KgReCrmModel.BookingDate;
                    _KGREPlotBooking.RestOfAmount = Convert.ToInt32(model.kGREPlotBookingModel.RestOfAmount);
                    _KGREPlotBooking.SalesTypeId = model.kGREPlotBookingModel.SalesTypeId;
                    _KGREPlotBooking.NoOfInstallment = model.kGREPlotBookingModel.NoOfInstallment;
                    _KGREPlotBooking.InstallmentAmount = model.kGREPlotBookingModel.InstallmentAmount;
                    _KGREPlotBooking.BookingNote = model.kGREPlotBookingModel.BookingNote;

                    _KGREPlotBooking.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    _KGREPlotBooking.ModifiedDate = DateTime.Now;
                }
                else
                {
                    _KGREPlotBooking.ClientId = model._KgReCrmModel.ClientId;
                    _KGREPlotBooking.PlotId = model.kGREPlotModel.PlotId;
                    _KGREPlotBooking.LandPricePerKatha = model.kGREPlotBookingModel.LandPricePerKatha;
                    _KGREPlotBooking.LandValue = Convert.ToInt32(model.kGREPlotBookingModel.LandValue);
                    _KGREPlotBooking.Discount = Convert.ToInt32(model.kGREPlotBookingModel.Discount);
                    _KGREPlotBooking.LandValueAfterDiscount = Convert.ToInt32(model.kGREPlotBookingModel.LandValueAfterDiscount);
                    _KGREPlotBooking.Additional25Percent = Convert.ToInt32(model.kGREPlotBookingModel.Additional25Percent);
                    _KGREPlotBooking.Additional15Percent = Convert.ToInt32(model.kGREPlotBookingModel.Additional15Percent);
                    _KGREPlotBooking.Additional10Percent = Convert.ToInt32(model.kGREPlotBookingModel.Additional10Percent);
                    _KGREPlotBooking.AdditionalCost = Convert.ToInt32(model.kGREPlotBookingModel.AdditionalCost);
                    _KGREPlotBooking.UtilityCost = Convert.ToInt32(model.kGREPlotBookingModel.UtilityCost);
                    _KGREPlotBooking.GrandTotal = Convert.ToInt32(model.kGREPlotBookingModel.GrandTotal);
                    _KGREPlotBooking.BookingMoney = Convert.ToInt32(model.kGREPlotBookingModel.BookingMoney);
                    _KGREPlotBooking.BookingDate = model._KgReCrmModel.BookingDate;
                    _KGREPlotBooking.RestOfAmount = Convert.ToInt32(model.kGREPlotBookingModel.RestOfAmount);
                    _KGREPlotBooking.SalesTypeId = model.kGREPlotBookingModel.SalesTypeId;
                    _KGREPlotBooking.NoOfInstallment = model.kGREPlotBookingModel.NoOfInstallment;
                    _KGREPlotBooking.InstallmentAmount = model.kGREPlotBookingModel.InstallmentAmount;
                    _KGREPlotBooking.BookingNote = model.kGREPlotBookingModel.BookingNote;
                    _KGREPlotBooking.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    _KGREPlotBooking.CreatedDate = DateTime.Now;
                }

                db.Entry(_KGREPlotBooking).State = _KGREPlotBooking.BookingId == 0 ? EntityState.Added : EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    return result = true;
                }
                else
                {
                    return result;
                }
            }
        }
        [HttpPost]
        public ActionResult GetPlotByProjectId(int projectId)
        {
            List<SelectModel> plots = kGREProjectService.GetKGREPlots(projectId);
            return Json(plots, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFileNo(int projectId)
        {
            KgReCrmModel plots = kgReCrmService.GetFileNo(projectId);
            var result = JsonConvert.SerializeObject(plots, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPlotInformationByPlotId1(int plotId)
        {
            List<KGREProjectModel> plots = kGREProjectService.GetKGREPlotListByPlotId(plotId);
            return Json(plots, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult GetPlotInformationByPlotId(int plotId)
        {
            KGREProjectModel plots = kGREProjectService.GetKGREPlot(plotId);
            var result = JsonConvert.SerializeObject(plots, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion //End Salse Information 

        
        public ActionResult UploadClientBatchForPlot(int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            return View();
        }

        
        [HttpPost]
        public ActionResult UploadClientBatchForPlot(KgReCrmViewModel file)
        {
            try
            {
                string message = UploadExcelFileForPlot(file);
                ViewBag.ExcelIssues = message;
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private string UploadExcelFileForPlot(KgReCrmViewModel file)
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
                    string SheetName = dtExcelSchema.Rows[8]["TABLE_NAME"].ToString();
                    cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
                    oda.SelectCommand = cmdExcel;
                    oda.Fill(ds);
                    string errorData = "";
                    try
                    {
                        dt = ds.Tables[0];
                        DataTable dtError = new DataTable();
                        int t = 0;
                        int s = 0;
                        int u = 0;
                        string dupData = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            ++t;
                            if (!string.IsNullOrEmpty(dr["Booking Name"].ToString()))
                            {
                                errorData = string.Empty;
                                errorData = dr["Booking Name"].ToString() + " and " + dr["BDate"].ToString();
                                KGRECustomer objKGRECustomer = null;
                                objKGRECustomer = new KGRECustomer();
                                objKGRECustomer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                objKGRECustomer.CreatedDate = DateTime.Now;
                                objKGRECustomer.BookingDate = !string.IsNullOrEmpty(dr["BDate"].ToString()) ? Convert.ToDateTime(dr["BDate"].ToString()) : (DateTime?)null;
                                objKGRECustomer.FullName = dr["Booking Name"].ToString();
                                objKGRECustomer.RegistrationName = dr["Registration Name"].ToString();
                                objKGRECustomer.PresentAddress = dr["Address"].ToString();
                                objKGRECustomer.PermanentAddress = dr["Address"].ToString();
                                objKGRECustomer.Designation = dr["DesignationProfession"].ToString();
                                objKGRECustomer.MobileNo = dr["Contact Number"].ToString();
                                objKGRECustomer.Email = dr["Email Address"].ToString();
                                if (!string.IsNullOrEmpty(dr["BDate"].ToString()) && !string.IsNullOrEmpty(dr["RDate"].ToString()))
                                {
                                    objKGRECustomer.StatusLevel = "Registerd";
                                }
                                else if (!string.IsNullOrEmpty(dr["BDate"].ToString()))
                                {
                                    objKGRECustomer.StatusLevel = "Booked";
                                }
                                else if (!string.IsNullOrEmpty(dr["Booking Cancel"].ToString()))
                                {
                                    objKGRECustomer.StatusLevel = "Booking Cancelled";
                                }
                                if (Convert.ToInt32(dr["ProjectID"].ToString()) == 32)
                                {
                                    // objKGRECustomer.BookingNote = dr["BS Jorip"].ToString() + "-" + "KWV-" + dr["Fno"].ToString();
                                    objKGRECustomer.FileNo = dr["BS Jorip"].ToString() + "-" + "KWV-" + dr["Fno"].ToString();
                                    //objKGRECustomer.FileNo = "KWV-" + dr["Fno"].ToString();
                                    //// objKGRECustomer.FileNo = "KWV-" + dr["Fno"].ToString();
                                }
                                else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 33)
                                {
                                    objKGRECustomer.FileNo = "KVP-" + dr["Fno"].ToString();
                                }
                                else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 34)
                                {
                                    objKGRECustomer.FileNo = "KRG-" + dr["Fno"].ToString();
                                }
                                else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 38)
                                {
                                    objKGRECustomer.FileNo = "KSV-" + dr["Fno"].ToString();
                                }
                                else
                                {
                                    objKGRECustomer.FileNo = dr["Fno"].ToString();
                                }
                                objKGRECustomer.ReferredBy = dr["Reference"].ToString();
                                objKGRECustomer.ProjectId = Convert.ToInt32(dr["ProjectID"].ToString());
                                objKGRECustomer.CompanyId = 7;
                                try
                                {
                                    db.KGRECustomers.Add(objKGRECustomer);
                                    db.SaveChanges();
                                    if (objKGRECustomer.ClientId > 0)
                                    {
                                        ++s;
                                        bool a = SaveKGREClientBookingForPlot(objKGRECustomer.ClientId, dr);
                                        if (a == true)
                                        {
                                            ++u;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                                ModelState.Clear();
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
                                ValidDisplayMessage += "Total number of KGREClientBooking data: " + u + " Out of " + t + "\n";
                                ValidDisplayMessage += "\r" + dupData;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(errorData);
                        logger.Error(ex);
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

        public bool SaveKGREClientBookingForPlot(int ClientId, DataRow dr)
        {
            if (!string.IsNullOrEmpty(dr["Booking Cancel"].ToString().Trim()))
            {
                bool result = false;
                if (dr["Booking Cancel"].ToString() == "Double")
                {
                    return result;
                }
                else
                {
                    KGREPlotBooking _KGREPlotBooking = new KGREPlotBooking();
                    KGREInstallment _KGREInstallment = new KGREInstallment();
                    using (ERPEntities db = new ERPEntities())
                    {
                        _KGREPlotBooking.PlotId = 3307;
                        _KGREPlotBooking.ClientId = ClientId;
                        _KGREPlotBooking.LandValue = 420;
                        _KGREPlotBooking.Additional25Percent = 420;
                        _KGREPlotBooking.Additional10Percent = 420;
                        _KGREPlotBooking.UtilityCost = 420;
                        _KGREPlotBooking.RestOfAmount = 420;
                        _KGREPlotBooking.MutationCost = 420;
                        _KGREPlotBooking.BoundaryWall = 420;
                        _KGREPlotBooking.NameChange = 420;
                        _KGREPlotBooking.RegAmount = 420;
                        _KGREPlotBooking.Discount = 420;
                        _KGREPlotBooking.NoOfInstallment = 420;
                        _KGREPlotBooking.GrandTotal = 420;
                        _KGREPlotBooking.BookingMoney = 420;
                        _KGREPlotBooking.BookingDate = DateTime.Now;
                        _KGREPlotBooking.RegistrationDate = DateTime.Now;
                        _KGREPlotBooking.ReturnMoney = !string.IsNullOrEmpty(dr["Return Money"].ToString()) ? Convert.ToDouble(dr["Return Money"].ToString()) : 0;

                        if (Convert.ToInt32(dr["ProjectID"].ToString()) == 32)
                        {
                            //_KGREPlotBooking.BookingNote = "KWV-" + dr["Fno"].ToString();
                            //_KGREPlotBooking.FileNo = "KWV-" + dr["Fno"].ToString();
                            _KGREPlotBooking.BookingNote = dr["BS Jorip"].ToString() + "-" + "KWV-" + dr["Fno"].ToString();
                            _KGREPlotBooking.FileNo = dr["BS Jorip"].ToString() + "-" + "KWV-" + dr["Fno"].ToString();
                        }
                        else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 33)
                        {
                            _KGREPlotBooking.FileNo = "KVP-" + dr["Fno"].ToString();
                        }
                        else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 34)
                        {
                            _KGREPlotBooking.FileNo = "KRG-" + dr["Fno"].ToString();
                        }
                        else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 38)
                        {
                            _KGREPlotBooking.FileNo = "KSV-" + dr["Fno"].ToString();
                        }
                        else
                        {
                            _KGREPlotBooking.BookingNote = dr["Fno"].ToString();
                            _KGREPlotBooking.FileNo = "KWV-" + dr["Fno"].ToString();
                        }
                        _KGREPlotBooking.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        _KGREPlotBooking.CreatedDate = DateTime.Now;
                        _KGREPlotBooking.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        _KGREPlotBooking.ModifiedDate = DateTime.Now;
                        // db.Entry(_KGREPlotBooking).State = _KGREPlotBooking.BookingId == 0 ? EntityState.Added : EntityState.Modified;
                        db.KGREPlotBookings.Add(_KGREPlotBooking);
                        if (db.SaveChanges() > 0)
                        {
                            if (_KGREPlotBooking.BookingId > 0)
                            {
                                //SaveKGREClientBookingInstallmentForPlot(_KGREPlotBooking.BookingId, ClientId, dr);
                            }
                            return result = true;
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
            }
            else
            {
                bool result = false;
                KGREPlotBooking _KGREPlotBooking = new KGREPlotBooking();
                KGREInstallment _KGREInstallment = new KGREInstallment();
                using (ERPEntities db = new ERPEntities())
                {
                    if (!string.IsNullOrEmpty(dr["Plot No"].ToString().Trim()))
                    {
                        string PlotNo = dr["Plot No"].ToString().Trim();
                        int ProjectID = Convert.ToInt32(dr["ProjectID"].ToString().Trim());
                        KGREPlot _KGREPlot = null;
                        _KGREPlot = db.KGREPlots.Where(x => x.PlotNo == PlotNo && x.ProjectId == ProjectID).FirstOrDefault();
                        if (_KGREPlot != null)
                        {
                            _KGREPlotBooking.PlotId = _KGREPlot.PlotId;
                            //_KGREPlot.PlotStatus = "";
                        }
                    }

                    _KGREPlotBooking.ClientId = ClientId;
                    _KGREPlotBooking.LandValue = !string.IsNullOrEmpty(dr["Land Value"].ToString()) ? Convert.ToDouble(dr["Land Value"].ToString()) : 0;
                    _KGREPlotBooking.Additional25Percent = !string.IsNullOrEmpty(dr["Additional25"].ToString()) ? Convert.ToDouble(dr["Additional25"].ToString()) : 0;
                    _KGREPlotBooking.Additional10Percent = !string.IsNullOrEmpty(dr["Additional10"].ToString()) ? Convert.ToDouble(dr["Additional10"].ToString()) : 0;
                    _KGREPlotBooking.UtilityCost = !string.IsNullOrEmpty(dr["Utilities"].ToString()) ? Convert.ToDouble(dr["Utilities"].ToString()) : 0;
                    _KGREPlotBooking.RegAmount = !string.IsNullOrEmpty(dr["RegAmount"].ToString()) ? Convert.ToDouble(dr["RegAmount"].ToString()) : 0;
                    _KGREPlotBooking.MutationCost = !string.IsNullOrEmpty(dr["Mutation"].ToString()) ? Convert.ToDouble(dr["Mutation"].ToString()) : 0;
                    _KGREPlotBooking.BoundaryWall = !string.IsNullOrEmpty(dr["Boundary"].ToString()) ? Convert.ToDouble(dr["Boundary"].ToString()) : 0;
                    _KGREPlotBooking.NameChange = !string.IsNullOrEmpty(dr["Name Change"].ToString()) ? Convert.ToDouble(dr["Name Change"].ToString()) : 0;
                    _KGREPlotBooking.Discount = !string.IsNullOrEmpty(dr["Discount"].ToString()) ? Convert.ToDouble(dr["Discount"].ToString()) : 0;
                    _KGREPlotBooking.NoOfInstallment = !string.IsNullOrEmpty(dr["Instl"].ToString()) ? Convert.ToInt32(dr["Instl"].ToString()) : 0;
                    _KGREPlotBooking.GrandTotal = !string.IsNullOrEmpty(dr["Total Value"].ToString()) ? Convert.ToDouble(dr["Total Value"].ToString()) : 0;
                    _KGREPlotBooking.Remarks = !string.IsNullOrEmpty(dr["Remarks"].ToString()) ? dr["Remarks"].ToString() : "";
                    _KGREPlotBooking.BookingMoney = !string.IsNullOrEmpty(dr["Booking Money"].ToString()) ? Convert.ToDouble(dr["Booking Money"].ToString()) : 0;
                    _KGREPlotBooking.LandValueR = !string.IsNullOrEmpty(dr["Land ValueR"].ToString()) ? Convert.ToDouble(dr["Land ValueR"].ToString()) : 0;
                    _KGREPlotBooking.Additional25PercentR = !string.IsNullOrEmpty(dr["Additional25R"].ToString()) ? Convert.ToDouble(dr["Additional25R"].ToString()) : 0;
                    _KGREPlotBooking.Additional10PercentR = !string.IsNullOrEmpty(dr["Additional10R"].ToString()) ? Convert.ToDouble(dr["Additional10R"].ToString()) : 0;
                    _KGREPlotBooking.RegAmountR = !string.IsNullOrEmpty(dr["RegR"].ToString()) ? Convert.ToInt32(dr["RegR"].ToString()) : 0;
                    _KGREPlotBooking.UtilityCostR = !string.IsNullOrEmpty(dr["UtilitiesR"].ToString()) ? Convert.ToDouble(dr["UtilitiesR"].ToString()) : 0;
                    _KGREPlotBooking.MutationCostR = !string.IsNullOrEmpty(dr["MutationR"].ToString()) ? Convert.ToDouble(dr["MutationR"].ToString()) : 0;
                    _KGREPlotBooking.TreePlantationR = !string.IsNullOrEmpty(dr["Tree PlantationR"].ToString()) ? Convert.ToDouble(dr["Tree PlantationR"].ToString()) : 0;
                    _KGREPlotBooking.BoundaryWallR = !string.IsNullOrEmpty(dr["BoundaryR"].ToString()) ? Convert.ToDouble(dr["BoundaryR"].ToString()) : 0;
                    _KGREPlotBooking.NamePlateR = !string.IsNullOrEmpty(dr["Name PlateR"].ToString()) ? Convert.ToDouble(dr["Name PlateR"].ToString()) : 0;
                    _KGREPlotBooking.SecurityServiceR = !string.IsNullOrEmpty(dr["Security ChargeR"].ToString()) ? Convert.ToDouble(dr["Security ChargeR"].ToString()) : 0;
                    _KGREPlotBooking.NameChangeR = !string.IsNullOrEmpty(dr["Name ChangeR"].ToString()) ? Convert.ToDouble(dr["Name ChangeR"].ToString()) : 0;
                    _KGREPlotBooking.BSSurveyR = !string.IsNullOrEmpty(dr["BS SurveyR"].ToString()) ? Convert.ToDouble(dr["BS SurveyR"].ToString()) : 0;
                    _KGREPlotBooking.DiscountR = !string.IsNullOrEmpty(dr["DiscountR"].ToString()) ? Convert.ToDouble(dr["DiscountR"].ToString()) : 0;
                    _KGREPlotBooking.AddDelayFineR = !string.IsNullOrEmpty(dr["AddDelay FineR"].ToString()) ? Convert.ToDouble(dr["AddDelay FineR"].ToString()) : 0;
                    _KGREPlotBooking.GrandTotalR = !string.IsNullOrEmpty(dr["Total Received"].ToString()) ? Convert.ToDouble(dr["Total Received"].ToString()) : 0;
                    _KGREPlotBooking.ReturnMoney = !string.IsNullOrEmpty(dr["Return Money"].ToString()) ? Convert.ToDouble(dr["Return Money"].ToString()) : 0;
                    _KGREPlotBooking.ServiceCharge4or10PerR = !string.IsNullOrEmpty(dr["4or10Per Service ChargeR"].ToString()) ? Convert.ToDouble(dr["4or10Per Service ChargeR"].ToString()) : 0;
                    _KGREPlotBooking.NetReceivedR = !string.IsNullOrEmpty(dr["Net ReceivedR"].ToString()) ? Convert.ToDouble(dr["Net ReceivedR"].ToString()) : 0;
                    _KGREPlotBooking.Due = !string.IsNullOrEmpty(dr["Due"].ToString()) ? Convert.ToDouble(dr["Due"].ToString()) : 0;
                    _KGREPlotBooking.InstallmentAmount = !string.IsNullOrEmpty(dr["Per Installment"].ToString()) ? Convert.ToDouble(dr["Per Installment"].ToString()) : 0;

                    _KGREPlotBooking.BookingDate = !string.IsNullOrEmpty(dr["BDate"].ToString()) ? Convert.ToDateTime(dr["BDate"].ToString()) : (DateTime?)null;
                    _KGREPlotBooking.RegistrationDate = !string.IsNullOrEmpty(dr["RDate"].ToString()) ? Convert.ToDateTime(dr["RDate"].ToString()) : (DateTime?)null;

                    if (Convert.ToInt32(dr["ProjectID"].ToString()) == 32)
                    {
                        //_KGREPlotBooking.BookingNote = "KWV-" + dr["Fno"].ToString();
                        //_KGREPlotBooking.FileNo = "KWV-" + dr["Fno"].ToString();

                        _KGREPlotBooking.BookingNote = dr["BS Jorip"].ToString() + "-" + "KWV-" + dr["Fno"].ToString();
                        _KGREPlotBooking.FileNo = dr["BS Jorip"].ToString() + "-" + "KWV-" + dr["Fno"].ToString();
                    }
                    else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 33)
                    {
                        _KGREPlotBooking.BookingNote = "KVP-" + dr["Fno"].ToString();
                        _KGREPlotBooking.FileNo = "KVP-" + dr["Fno"].ToString();
                    }
                    else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 34)
                    {
                        _KGREPlotBooking.BookingNote = "KRG-" + dr["Fno"].ToString();
                        _KGREPlotBooking.FileNo = "KRG-" + dr["Fno"].ToString();
                    }
                    else if (Convert.ToInt32(dr["ProjectID"].ToString()) == 38)
                    {
                        _KGREPlotBooking.BookingNote = "KSV-" + dr["Fno"].ToString();
                        _KGREPlotBooking.FileNo = "KSV-" + dr["Fno"].ToString();
                    }
                    else
                    {
                        _KGREPlotBooking.BookingNote = dr["Fno"].ToString();
                        _KGREPlotBooking.FileNo = "KWV-" + dr["Fno"].ToString();
                    }
                    _KGREPlotBooking.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    _KGREPlotBooking.CreatedDate = DateTime.Now;
                    _KGREPlotBooking.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    _KGREPlotBooking.ModifiedDate = DateTime.Now;
                    // db.Entry(_KGREPlotBooking).State = _KGREPlotBooking.BookingId == 0 ? EntityState.Added : EntityState.Modified;
                    db.KGREPlotBookings.Add(_KGREPlotBooking);
                    if (db.SaveChanges() > 0)
                    {
                        if (_KGREPlotBooking.BookingId > 0)
                        {
                            SaveKGREClientBookingInstallmentForPlot(_KGREPlotBooking.BookingId, ClientId, dr);
                        }
                        return result = true;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
        }

        private bool SaveKGREClientBookingInstallmentForPlot(long bookingId, int clientId, DataRow dr)
        {
            bool result = false;
            KGREInstallment _KGREInstallment = new KGREInstallment();
            KGREInstallment _KGREInstallment1 = new KGREInstallment();
            using (ERPEntities db = new ERPEntities())
            {
                DateTime Sdt = DateTime.Now;
                if (!string.IsNullOrEmpty(dr["BDate"].ToString().Trim()))
                {
                    Sdt = Convert.ToDateTime(dr["BDate"].ToString());
                }
                int PlotId = 0;
                if (!string.IsNullOrEmpty(dr["Plot No"].ToString().Trim()))
                {
                    string PlotNo = dr["Plot No"].ToString().Trim();
                    int ProjectID = Convert.ToInt32(dr["ProjectID"].ToString().Trim());
                    KGREPlot _KGREPlot = null;
                    _KGREPlot = db.KGREPlots.Where(x => x.PlotNo == PlotNo && x.ProjectId == ProjectID).FirstOrDefault();
                    if (_KGREPlot != null)
                    {
                        PlotId = _KGREPlot.PlotId;
                    }
                }
                _KGREInstallment1 = db.KGREInstallments.OrderByDescending(x => x.InstallmentId).FirstOrDefault(x => x.PlotId == PlotId && x.ClientId == clientId);
                //_KGREPlotBooking.NoOfInstallment = !string.IsNullOrEmpty(dr["Instl"].ToString()) ? Convert.ToInt32(dr["Instl"].ToString()) : 0;

                if (!string.IsNullOrEmpty(dr["Instl"].ToString()))
                {
                    if (_KGREInstallment1 == null)
                    {
                        if (Convert.ToInt32(dr["Instl"].ToString()) > 0)
                        {
                            _KGREInstallment.ClientId = clientId;
                            _KGREInstallment.PlotId = PlotId;
                            _KGREInstallment.ProjectId = Convert.ToInt32(dr["ProjectID"].ToString());
                            _KGREInstallment.PaymentDate = Sdt;
                            _KGREInstallment.InstallmentDate = Sdt;

                            if (!string.IsNullOrEmpty(dr["BDate"].ToString()) && !string.IsNullOrEmpty(dr["RDate"].ToString()))
                            {
                                _KGREInstallment.InstallmentStatus = "Registerd";
                                _KGREInstallment.InstallmentAmount = Convert.ToDouble(dr["Total Received"].ToString());
                            }
                            else if (!string.IsNullOrEmpty(dr["BDate"].ToString()))
                            {
                                _KGREInstallment.InstallmentStatus = "Booked";
                                _KGREInstallment.InstallmentAmount = Convert.ToDouble(dr["Total Received"].ToString());
                            }
                            _KGREInstallment.Remarks = "";
                            _KGREInstallment.BookingId = Convert.ToInt32(bookingId);
                            _KGREInstallment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            _KGREInstallment.CreatedDate = DateTime.Now;
                            _KGREInstallment.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            _KGREInstallment.ModifiedDate = DateTime.Now;
                            try
                            {
                                db.KGREInstallments.Add(_KGREInstallment);
                                if (db.SaveChanges() > 0)
                                {
                                    if (!string.IsNullOrEmpty(dr["Plot No"].ToString().Trim()))
                                    {
                                        string PlotNo = dr["Plot No"].ToString().Trim();
                                        int ProjectID = Convert.ToInt32(dr["ProjectID"].ToString().Trim());
                                        KGREPlot _KGREPlot = null;
                                        _KGREPlot = db.KGREPlots.Where(x => x.PlotNo == PlotNo && x.ProjectId == ProjectID).FirstOrDefault();
                                        if (_KGREPlot != null)
                                        {
                                            if (!string.IsNullOrEmpty(dr["BDate"].ToString()) && !string.IsNullOrEmpty(dr["RDate"].ToString()))
                                            {
                                                _KGREPlot.PlotStatus = 473;
                                            }
                                            else if (!string.IsNullOrEmpty(dr["BDate"].ToString()))
                                            {
                                                _KGREPlot.PlotStatus = 471;
                                            }
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    ///return result;
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        return result = true;
                    }
                    else
                    {
                        return result;
                    }
                }
                else
                {
                    return result;
                }
            }
        }

        public FileResult Download(String fileName, String deedNo)
        {
            return File(Path.Combine(Server.MapPath("~/KGFiles/KGRE/"), fileName), System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                int guid = Convert.ToInt32(id);
                var path1 = string.Empty;
                FileAttachment fileDetail = db.FileAttachments.Find(guid);
                if (fileDetail == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }
                else
                {
                    LandNLegal aseet = db.LandNLegals.Find(fileDetail.AttachmentId);
                    path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGRE")), fileDetail.AttachFileName);
                }

                //Remove from database
                db.FileAttachments.Remove(fileDetail);
                db.SaveChanges();

                //Delete file from the file system 
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #region // Report
        [HttpGet]
        
        public ActionResult ClientPaymentInformation(string clientId, string ReportName, string ReportDescription)
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/KGRE/ClientPaymentInformationReport.aspx?clientId={0}&Height={1}", clientId, 800),
                    Width = 700,
                    Height = 950
                };

                return View(rptInfo);
            }
            else
            {
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }
        #endregion

        #region Collection 
        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = kgReCrmService.GetCustomerAutoComplete(prefix, companyId);
            return Json(customers);
        }

        [HttpPost]
        public JsonResult AutoCompleteByFileNo(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = kgReCrmService.AutoCompleteByFileNo(prefix, companyId);
            return Json(customers);
        }

        
        [HttpGet]
        public JsonResult GetCustomerPaymentInformation(int customerId)
        {
            KGREPlotBookingModel vendor = kgReCrmService.GetClientPaymentStatus(customerId);
            KgReCrmModel crm = kgReCrmService.GetKGRClientById(customerId);
            if (crm != null)
            {
                vendor.FullName = crm.FullName;
                vendor.PermanentAddress = crm.PermanentAddress;
                vendor.MobileNo = crm.MobileNo;
                if (crm.ProjectId != null)
                {
                    vendor.ProjectId = (int)crm.ProjectId;
                }
            }
            var result = JsonConvert.SerializeObject(vendor, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //#region //GLDLNEW   

        //
        //[HttpPost]
        //public JsonResult GetByProduct(int productid)
        //{
        //    var result = kgReCrmService.Getbyproduct(productid);
        //    return Json(result);
        //}

        //
        //[HttpPost]
        //public async Task<JsonResult> GetProductCostHeadsbyId(int productid, int companyId)
        //{
        //    BookingViewModel model = new BookingViewModel();
        //    model.product = kgReCrmService.Getbyproduct(productid);

        //    return Json(model);
        //}

        //[HttpGet]
        //public async Task<ActionResult> CustomerBooking(int clientId, int companyId)
        //{
        //    GLDLBookingViewModel vm = new GLDLBookingViewModel() { CompanyId = companyId };
        //    vm.ApplicationDate = DateTime.Now;
        //    var crm = kgReCrmService.GetKGRClientById(clientId);
        //    vm.CustomerGroupName = crm.FullName + " & Associated";
        //    vm.PrimaryContactAddr = crm.PresentAddress;
        //    vm.PrimaryContactNo = crm.MobileNo;
        //    vm.PrimaryEmail = crm.Email;
        //    vm.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
        //    vm.LstPurchaseCostHeads = await _costHeadService.GetCostHeadsByCompanyId(companyId);
        //    vm.BookingInstallmentType = await _costHeadService.GetBookingInstallmentType();
        //    vm.Employee = await gLDLCustomerService.GetbyEmployee(companyId);
        //    return View(vm);
        //}

        //[HttpPost]
        //public async Task<ActionResult> CustomerBooking(GLDLBookingViewModel model)
        //{
        //    model.EntryBy = Convert.ToInt32(Session["Id"]);
        //    var result = await gLDLCustomerService.CustomerBokking(model);
        //    if (result.CGId != 0)
        //    {
        //        return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<ActionResult> CustomerBookingInformation(int companyId, long CGId)
        //{
        //    var res = await customerBookingService.CustomerBookingView(companyId, CGId);
        //    res.pRM_Relations = await customerBookingService.PRMRelation();
        //    return View(res);
        //}

        //[HttpPost]
        //public async Task<ActionResult> CustomerNominee(CustomerNominee nominee)
        //{
        //    var res = await gLDLCustomerService.AddCustomerNominee(nominee);
        //    return RedirectToAction("CustomerBookingInformation", new { companyId = nominee.CompanyId, CGId = nominee.CGId });
        //}


        //[HttpPost]
        //public async Task<ActionResult> UpdateNominee(CustomerNominee nominee)
        //{
        //    var res = await gLDLCustomerService.UpdateNominee(nominee);
        //    return RedirectToAction("CustomerBookingInformation", new { companyId = nominee.CompanyId, CGId = nominee.CGId });
        //}


        //[HttpPost]
        //public async Task<ActionResult> DeleteNominee(CustomerNominee nominee)
        //{
        //    var res = await gLDLCustomerService.DeleteNominee(nominee);
        //    return RedirectToAction("CustomerBookingInformation", new { companyId = nominee.CompanyId, CGId = nominee.CGId });
        //}
        //[HttpPost]
        //public async Task<JsonResult> GetNominee(long id)
        //{
        //    var res = await gLDLCustomerService.GetByNominee(id);
        //    return Json(res);
        //}

        //[HttpPost]
        //public async Task<ActionResult> UpdateNomineefile(NomineeFile file)
        //{
        //    if (file.NIDFile == null && file.ImageFile == null)
        //    {
        //        return RedirectToAction("CustomerBookingInformation", new { companyId = file.CompanyId, CGId = file.CGId });
        //    }
        //    List<FileItem> itemlist = new List<FileItem>();
        //    if (file.ImageFile != null)
        //    {
        //        itemlist.Add(new FileItem
        //        {
        //            file = file.ImageFile,
        //            docdesc = "Nominee Photo",
        //            docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.ImageFile.FileName),
        //            docid = 0,
        //            FileCatagoryId = 2,
        //            fileext = Path.GetExtension(file.ImageFile.FileName),
        //            isactive = true,
        //            RecDate = DateTime.Now,
        //            SortOrder = 1,
        //            userid = Convert.ToInt32(Session["Id"])
        //        });
        //    }
        //    if (file.NIDFile != null)
        //    {
        //        itemlist.Add(new FileItem
        //        {
        //            file = file.NIDFile,
        //            docdesc = "Nominee NID",
        //            docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.NIDFile.FileName),
        //            docid = 0,
        //            FileCatagoryId = 2,
        //            fileext = Path.GetExtension(file.NIDFile.FileName),
        //            isactive = true,
        //            RecDate = DateTime.Now,
        //            SortOrder = 2,
        //            userid = Convert.ToInt32(Session["Id"])
        //        });
        //    }
        //    itemlist = await _ftpservice.UploadFileBulk(itemlist, file.CGId.ToString());
        //    long ImageDocId = 0;
        //    long NIDDocId = 0;
        //    if (file.ImageFile != null)
        //    {
        //        ImageDocId = itemlist.FirstOrDefault(f => f.SortOrder == 1).docid;
        //    }
        //    if (file.NIDFile != null)
        //    {
        //        NIDDocId = itemlist.FirstOrDefault(f => f.SortOrder == 2).docid;
        //    }

        //    var res = await gLDLCustomerService.FileUpdateNominee(file, ImageDocId, NIDDocId);
        //    return RedirectToAction("CustomerBookingInformation", new { companyId = file.CompanyId, CGId = file.CGId });
        //}


        //[HttpPost]
        //public async Task<JsonResult> DeleteNomineeImageFile(long docId, long nomineeId, long CGId, long companyId)
        //{
        //    var result = await _ftpservice.DeleteFile(docId);
        //    if (result)
        //    {
        //        var res = await gLDLCustomerService.UpdateNomineeImageDociId(nomineeId, docId);
        //        return Json(res);
        //    }
        //    return Json(0);
        //}

        //[HttpPost]
        //public async Task<JsonResult> DeleteNomineeNidFile(long docId, long nomineeId, long CGId, long companyId)
        //{
        //    var result = await _ftpservice.DeleteFile(docId);
        //    if (result)
        //    {
        //        var res = await gLDLCustomerService.UpdateNomineeNIDDociId(nomineeId, docId);
        //        return Json(res);
        //    }
        //    return Json(0);
        //}
        //[HttpPost]
        //public async Task<JsonResult> CGFileDelete(long docId,long CGId)
        //{
        //    var result = await _ftpservice.DeleteFile(docId);
        //    if (result)
        //    {
        //        var res = await gLDLCustomerService.DeleteCGFile(docId,CGId);
        //        return Json(res);
        //    }
        //    return Json(false);
        //}

        //[HttpPost]
        //public async Task<ActionResult> CGUloadFile(GLDLBookingViewModel model)
        //{
        //    if (model.Attachments == null)
        //    {
        //        return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
        //    }
        //    List<FileItem> itemlist = new List<FileItem>();
        //    for (int i = 0; i < model.Attachments.Count(); i++)
        //    {
        //        if (model.Attachments[i].DocId == 0)
        //        {
        //            itemlist.Add(new FileItem
        //            {
        //                file = model.Attachments[i].File,
        //                docdesc = model.Attachments[i].Title,

        //                docid = 0,
        //                FileCatagoryId = 2,
        //                fileext = Path.GetExtension(model.Attachments[i].File.FileName),
        //                docfilename = Guid.NewGuid().ToString() + Path.GetExtension(model.Attachments[i].File.FileName),
        //                isactive = true,
        //                RecDate = DateTime.Now,
        //                SortOrder = i,
        //                userid = Convert.ToInt32(Session["Id"])
        //            });
        //        }
        //    }
        //    itemlist = await _ftpservice.UploadFileBulk(itemlist, model.CGId.ToString());
        //    var result = await gLDLCustomerService.FileMapping(itemlist, model.CGId);
        //    return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
        //}

        //[HttpPost]
        //public async Task<JsonResult> InstallmentSchedule(int conmpanyId, int installmentId,int NoOfInstallment, decimal restofAmount)
        //{
        //    var date = DateTime.Now;
        //    var res = await bookingInstallmentService.GenerateInstallmentSchedule(conmpanyId, installmentId, NoOfInstallment, restofAmount, date);
        //    return Json(res);
        //}

        //[HttpGet]
        //public async Task<ActionResult> CustomerBookingList(int companyId)
        //{
        //    var res = await customerBookingService.CustomerBookingList(companyId);
        //    return View(res);
        //}
        //#endregion
    }
}