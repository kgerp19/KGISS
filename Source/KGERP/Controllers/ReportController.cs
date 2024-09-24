using AutoMapper.Mappers;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using KGERP.Controllers.Custom_Authorization;
using KGERP.CustomModel;
using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Implementation.EmployeeLogService;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Implementation.TaskManagment;
using KGERP.Service.Interface;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using static KGERP.Utility.Util.PermissionCollection.Crms;

namespace KGERP.Controllers
{
    [CheckSession]
    //test azim
    public class ReportController : Controller
    {
        private readonly MoneyReceiptService _moneyReceiptService;
        private readonly IStockInfoService stockInfoService;
        private readonly IPurchaseOrderService purchaseOrderService;
        private readonly ICompanyService companyService;
        private readonly IVoucherTypeService voucherTypeService;
        private readonly IOfficerAssignService officerAssignService;
        private readonly IVendorService vendorService;
        private readonly AccountingService _accountingService;
        private readonly ConfigurationService _configrationService;
        private readonly IApproval_Service _appService;
        private readonly ITeamService teamService;
        private readonly EmployeeLogServices logServices;
        private readonly ICostHeadsService _costHeadService;
        private readonly ITaskManagementEvolutionService taskManagementEvolutionService;

        private IAssetService assetService;
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        IDepartmentService departmentService = new DepartmentService();
        IDesignationService designationService = new DesignationService();
        //string password = "Sys@Root@007";
        //string admin = "Administrator";
        //string url = "http://192.168.2.2/ReportServer/?%2fErpReport/";

        //string password = "Gocorona!9";
        //string admin = "Administrator";
        //string url = "http://192.168.0.7/ReportServer_SQLEXPRESS/?%2fErpReport/";

        string password = "Sysroot@123";
        string admin = "Administrator";
        string url = "http://192.168.0.13/ReportServer/?%2fErpReport/";



        public ReportController(ERPEntities db, MoneyReceiptService moneyReceiptService, IStockInfoService stockInfoService, IPurchaseOrderService purchaseOrderService,
              ICompanyService companyService, IVoucherTypeService voucherTypeService, IOfficerAssignService officerAssignService,
              IVendorService vendorService, ProcurementService service, ConfigurationService configurationService,
              IApproval_Service _appService, ITeamService teamService, ICostHeadsService _costHeadService, IAssetService _assetService, ITaskManagementEvolutionService _taskManagementEvolutionService)
        {
            this.stockInfoService = stockInfoService;
            _moneyReceiptService = moneyReceiptService;
            this.purchaseOrderService = purchaseOrderService;
            this.companyService = companyService;
            this.voucherTypeService = voucherTypeService;
            this.officerAssignService = officerAssignService;
            this.vendorService = vendorService;
            this._appService = _appService;
            this.teamService = teamService;
            this._costHeadService = _costHeadService;
            this.logServices = logServices;
            this.assetService = _assetService;
            this.taskManagementEvolutionService = _taskManagementEvolutionService;

            _service = service;
            _accountingService = new AccountingService(db);
            _configrationService = configurationService;

        }
        ProcurementService _service;
        [HttpGet]

        public ActionResult Index()
        {
            return View();
        }


        // GET: Report
        [HttpGet]

        public ActionResult GetEmployeeReport(string employeeId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "Employee&rs:Command=Render&rs:Format=PDF&EmployeeId=" + employeeId;
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult ReportTemplate(int companyId, string reportName, string reportDescription)
        {
            var rptInfo = new ReportInfo
            {
                ReportName = reportName,
                ReportDescription = reportDescription,
                ReportURL = String.Format("../../Reports/ReportTemplate.aspx?ReportName={0}&ReportDescription={1}&Height={2}", reportName, reportDescription, 650),
                Width = 100,
                Height = 650
            };
            return View(rptInfo);
        }


        [HttpGet]

        public ActionResult GetLandReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                LandUser = assetService.LandUser(),
                ProjectList2 = assetService.Project(),
                DisputedLisr = assetService.DisputedList(),
                LandReceiver = assetService.LandReceiver(),
                Companies = assetService.Company(),
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult GetLandReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "LandSearchReport";


            if (model.CompaniesId == null)
            {
                model.CompaniesId = 0;
            }
            if (model.Project2Id == null)
            {
                model.Project2Id = 0;
            }
            if (model.LandReceiverId == null)
            {
                model.LandReceiverId = 0;
            }
            if (model.LandUserId == null)
            {
                model.LandUserId = 0;
            }
            if (model.DisputedId == null)
            {
                model.DisputedId = 0;
            }


            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ProjectId={3}&LandReceiverId={4}&LandUserId={5}&DisputedListId={6}&DagNo={7}", model.ReportName, model.ReportType, model.CompaniesId, model.Project2Id, model.LandReceiverId, model.LandUserId, model.DisputedId, model.DagNo);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }





        [HttpGet]

        public ActionResult CRReportTemplate(int companyId, string reportName, string reportDescription)
        {
            var rptInfo = new ReportInfo
            {
                ReportName = reportName,
                ReportDescription = reportDescription,
                ReportURL = String.Format("../../Reports/ReportTemplate.aspx?ReportName={0}&ReportDescription={1}&Height={2}&companyId={3}", reportName, reportDescription, 650, companyId),
                Width = 100,
                Height = 650
            };
            return View(rptInfo);
        }
        // GET: Report
        [HttpGet]

        public ActionResult GetOrderInvoiceReport(string orderMasterId)
        {
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            string reportURL;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                reportURL = url + "KFMALOrderInvoice&rs:Command=Render&rs:Format=PDF&OrderMasterId=" + orderMasterId;
            }

            else
            {
                reportURL = url + "OrderInvoice&rs:Command=Render&rs:Format=PDF&OrderMasterId=" + orderMasterId;
            }


            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetEmiReport(int emiId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "EmiReportForKFMAL&rs:Command=Render&rs:Format=PDF&EmiId=" + emiId;
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetKgeComOrderInvoiceReport(string orderMasterId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "KGeComInvoiceReport&rs:Command=Render&rs:Format=PDF&OrderMasterId=" + orderMasterId;
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetKFMALCOstingReport(int storeId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "CogsCostingReport&rs:Command=Render&rs:Format=PDF&StoreId=" + storeId;
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        //[HttpGet]
        //
        //public ActionResult GetStockReport()
        //{
        //    NetworkCredential nwc = new NetworkCredential(admin, AdminPassword);
        //    WebClient client = new WebClient();
        //    client.Credentials = nwc;
        //    string reportURL =url + "StockReport&rs:Command=Render&rs:Format=PDF";
        //    //string reportURL = url + "StockReport" ;
        //    return File(client.DownloadData(reportURL), "application/pdf");
        //}

        [HttpGet]

        public ActionResult GetStockReport(int companyId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);



            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}", reportName, companyId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetEcomStockReport()
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "KGeComCurrentStock&rs:Command=Render&rs:Format=PDF";
            //string reportURL = url + "StockReport" ;
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        public ActionResult GetRMStockReport()
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "RmStockReport&rs:Command=Render&rs:Format=PDF";
            //string reportURL = url + "StockReport" ;
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        public ActionResult GetRMDeliverReport(int requisitionId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "RMDeliveryReport&rs:Command=Render&rs:Format=PDF&RequisitionId=" + requisitionId;
            //string reportURL = url + "StockReport" ;
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        public ActionResult GetRequisitionReport(int requisitionId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = url + "RequisitionReport&rs:Command=Render&rs:Format=PDF&RequisitionId=" + requisitionId;
            //string reportURL = url + "StockReport" ;
            return File(client.DownloadData(reportURL), "application/pdf");
        }
        public ActionResult GetEmployeeClearanceReport(int id)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            //string reportURL = url + "EmployeeClearanceReport&rs:Command=Render&rs:Format=PDF&id=" + id;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&id={1}", "EmployeeClearanceReport", id);
            //string reportURL = url + "StockReport" ;
            return File(client.DownloadData(reportURL), "application/pdf");
        }
        [HttpGet]

        public ActionResult GetDeliveryInvoiceReport(long OrderDeliverId, int companyId)
        {
            string reportName = string.Empty;
            if (companyId == 8)
            {
                reportName = "DeliveryInvoice";
            }

            //else if (companyId == 29)
            //{
            //    reportName = "GloryFeedDeliveryInvoice";
            //}
            //else if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            //{
            //    reportName = "KFMALDeliveryInvoice";
            //}
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&OrderDeliverId={1}", reportName, OrderDeliverId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetFeedInvoiceReport(long orderMasterId)
        {
            string reportName = string.Empty;
            reportName = "FeedInvoice";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&OrderMasterId={1}", reportName, orderMasterId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult FeedMYIncentivePolicy(int companyId)
        {
            string reportName = string.Empty;
            if (companyId == 8)
            {
                reportName = "FeedMYIncentivePolicy";
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}", reportName, companyId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetDeliveryChallanReport(long orderDeliverId, int companyId)
        {
            string reportName = string.Empty;

            
                reportName = "ISSDeliveryChallan";
             
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&OrderDeliverId={1}", reportName, orderDeliverId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        // GET: Report
        [HttpGet]

        public ActionResult GetPreviousEmployeeReport(long id, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&Id={1}", reportName, id);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        // GET: Report
        [HttpGet]

        public ActionResult GetCustomerLedgerReport(int id, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&VendorId={1}", reportName, id);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        // GET: Report
        [HttpGet]
        //
        public ActionResult GetVoucherReport(int companyId, long voucherId, string reportName)
        {
            if (companyId == (int)CompanyName.KrishibidSeedLimited)
            {
                reportName = "KGVoucherReportSeed";

            }
            else
            {
                reportName = "KGVoucherReport";

            }

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&VoucherId={2}", reportName, companyId, voucherId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }



        // GET: Demand Report
        [HttpGet]

        public ActionResult GetDemandReport(int demandId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&DemandId={1}", reportName, demandId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GCCLPurchseOrderReport(int purchaseOrderId, int companyId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&PurchaseOrderId={2}", reportName, 24, purchaseOrderId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }
        [HttpGet]

        public ActionResult KFMALLPurchaseOrderReports(int purchaseOrderId, int companyId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&PurchaseOrderId={2}", reportName, companyId, purchaseOrderId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult PackagingPurchaseOrderReports(int purchaseOrderId, int companyId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&PurchaseOrderId={2}", reportName, companyId, purchaseOrderId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult PackagingBillReports(string reportName, int companyId, int materialReceiveId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&MaterialReceiveId={2}", reportName, companyId, materialReceiveId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }
        [HttpGet]

        public ActionResult GCCLPurchseInvoiceReport(int companyId, int materialReceiveId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&materialReceiveId={2}", reportName, companyId, materialReceiveId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]
        public ActionResult PackagingPurchseInvoiceReport(int companyId, int materialReceiveId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&materialReceiveId={2}", reportName, companyId, materialReceiveId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]
        public ActionResult PackagingDeliverInvoiceReport(int companyId, int orderDeliverId)
        {

            string reportName = "ISSDeliverInvoiceReports";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&OrderDeliverId={2}", reportName, companyId, orderDeliverId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GCCLSalesInvoiceReport(int companyId, int orderMasterId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&OrderMasterId={2}", reportName, companyId, orderMasterId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]
        public ActionResult PackagingSalesInvoiceReport(int companyId, int orderMasterId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&OrderMasterId={2}", reportName, companyId, orderMasterId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }
        [HttpGet]
        public ActionResult SalarySheetCompanyWiseReport(int CompanyId, long PayRollId, string reportType)
        {

            CompanyName companyName = (CompanyName)CompanyId;
            string companyNameString = companyName.ToString();
            string reportName = "CompanyWiseSalarySheet";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&PayRollId={2}&CompanyId={3}", reportName, reportType, PayRollId, CompanyId);
            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", companyNameString + " CompanyWiseSalarySheet.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), companyNameString + " application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult BonusSheetCompanyWiseReport(int CompanyId, long PayRollId, string reportType)
        {
            CompanyName companyName = (CompanyName)CompanyId;
            string companyNameString = companyName.ToString();
            string reportName = "CompanyWiseBonusSheet";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&PayRollId={2}&CompanyId={3}", reportName, reportType, PayRollId, CompanyId);
            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", companyNameString + "CompanyWiseBonusSheet.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }
        [HttpGet]
        public ActionResult BankAdviceSheetReport(int companyId, long payRollId, int bankBranchId, string reportType, string salaryDate, string employeeIds)
        {
            string reportName = "SalaryAdviceSheetReport";
            if (string.IsNullOrEmpty(employeeIds))
            {
                employeeIds = null;
            }
            var company = _accountingService.GetCompanyById(companyId);

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&PayRollId={2}&CompanyId={3}&EmployeeIds={4}&BankBranchId={5}&SalaryDate={6}", reportName, reportType, payRollId, companyId, employeeIds, bankBranchId, salaryDate);
            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", company.ShortName + "SalaryAdviceSheet.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf", company.ShortName + "SalaryAdviceSheet.pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult BonusAdviceSheetReport(int companyId, long payRollId, int bankBranchId, string reportType, string salaryDate, string employeeIds)
        {
            string reportName = "BonusAdviceSheetReport";
            if (string.IsNullOrEmpty(employeeIds))
            {
                employeeIds = null;
            }
            var company = _accountingService.GetCompanyById(companyId);

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&PayRollId={2}&CompanyId={3}&EmployeeIds={4}&BankBranchId={5}&SalaryDate={6}", reportName, reportType, payRollId, companyId, employeeIds, bankBranchId, salaryDate);
            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", company.ShortName + "BonusAdviceSheet.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf", company.ShortName + "BonusAdviceSheet.pdf");
            }
            return View();
        }

        [HttpGet]
        public ActionResult GCCLPRFInvoiceReport(int companyId, int DemandId, string reportName, int CustomerId, string AsOnDate)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            //var fdate = AsOnDate.ToString("dd-MM-yyyy");
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&DemandId={2}&CustomerId={3}&AsOnDate={4}", reportName, companyId, DemandId, CustomerId, AsOnDate);
            return File(client.DownloadData(reportURL), "application/pdf");
        }
        [HttpGet]
        public ActionResult GCCLProductionReport(int companyId, int prodReferenceId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&ProdReferenceId={2}", reportName, companyId, prodReferenceId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }
        // GET: Purchase Order Report
        [HttpGet]

        public ActionResult GetPurchseOrderReport(int purchaseOrderId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&PurchaseOrderId={1}", reportName, purchaseOrderId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        // GET: Purchase Order Report
        [HttpGet]

        public ActionResult GetMRRReport(long materialReceiveId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&MaterialReceiveId={1}", reportName, materialReceiveId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetChartOfAccountReport(int companyId, string reportType, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", reportName, reportType, companyId);

            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ChartOfAccount.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (reportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ChartOfAccount.doc");
            }

            return View();

        }

        [HttpGet]

        public ActionResult CompanyZoneAndTerritoryReport(int companyId, string reportType, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", reportName, reportType, companyId);

            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ChartOfAccount.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (reportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ChartOfAccount.doc");
            }

            return View();

        }
        [HttpGet]

        public ActionResult ProdReferenceGet(int companyId, string reportType, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}", reportName, reportType);

            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ProdReference.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (reportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ProdReference.doc");
            }

            return View();

        }



        // GET: General Ledger Report
        [HttpGet]

        [ParentAuthorizedAttribute]
        public ActionResult GeneralLedger(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GeneralLedgerReport(ReportCustomModel model)
        {
            string accCode = model.AccName.Substring(1, 13);
            string reportName = "";
            reportName = "ISSGeneralLedger";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.Id, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        //[ParentAuthorizedAttribute]
        public ActionResult SingleGeneralLedger(int companyId, int headGlId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            var headGl = _accountingService.GetHeadGLById(headGlId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                Id = headGlId,
                AccName = "[" + headGl.AccCode + "] " + headGl.AccName,
                CompanyId = companyId,
                CompanyName = company.Name + " (" + company.ShortName + ")",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()

            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult CustomerGeneralLedger(int companyId, int VendorTypeId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { VendorTypeId = VendorTypeId, CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);
        }
        [HttpGet]

        public ActionResult CustomerGeneralLedgerReport(ReportCustomModel model)
        {
            string reportName = "";
            if (model.CompanyId == 10)
            {
                reportName = "KfmalCustomerGeneralLedger";
            }
            else if (model.CompanyId == 24)
            {
                reportName = "GcclCustomerGeneralLedger";
            }


            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.Id, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult SupplierGeneralLedger(int companyId, int VendorTypeId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { VendorTypeId = VendorTypeId, CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);
        }
        [HttpGet]

        public ActionResult SupplierGeneralLedgerReport(ReportCustomModel model)
        {
            string reportName = "";
            if (model.CompanyId == 10)
            {
                reportName = "KfmalSupplierGeneralLedger";
            }
            else if (model.CompanyId == 24)
            {
                reportName = "GcclSupplierGeneralLedger";
            }


            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.Id, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        // GET: Shareholder General Ledger
        [HttpGet]

        public ActionResult ShareholderGeneralLedger(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);

        }

        [HttpGet]

        public ActionResult ShareholderGeneralLedgerReport(ReportCustomModel model)
        {
            string accCode = model.AccName.Substring(1, 13);
            string reportName = "";
            reportName = "ShareholderGeneralLedger";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.Id, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        // GET: General Ledger Report
        [HttpGet]

        public ActionResult GeneralBankOrCashBook(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),


            };
            cm.BankOrCashParantList = new SelectList(_accountingService.CashAndBankDropDownList(companyId), "Value", "Text");

            // if (companyId == (int)CompanyName.GloriousCropCareLimited
            //     || companyId == (int)CompanyName.KrishibidFirmLimited
            //     || companyId == (int)CompanyName.KrishibidPackagingLimited
            //     || companyId == (int)CompanyName.KrishibidSeedLimited
            //     )
            // {
            // }

            // else if (companyId == (int)CompanyName.KrishibidPoultryLimited)
            // {
            //     cm.BankOrCashParantList = new SelectList(_accountingService.KPLCashAndBankDropDownList(companyId), "Value", "Text");

            // }

            //if (companyId == (int)CompanyName.KrishibidSeedLimited)
            // {
            //     cm.BankOrCashParantList = new SelectList(_accountingService.SeedCashAndBankDropDownList(companyId), "Value", "Text");

            // }

            return View(cm);
        }

        [HttpGet]

        public ActionResult GeneralBankOrCashBookReport(ReportCustomModel model)
        {

            string reportName = "";
            reportName = "KGBankOrCashBook";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.Id, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        // GET: PF General Ledger
        [HttpGet]

        public ActionResult PFGeneralLedger(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);

        }

        [HttpGet]

        public ActionResult PFGeneralLedgerReport(ReportCustomModel model)
        {
            string accCode = model.AccName.Substring(1, 13);
            string reportName = "";
            reportName = "PFGeneralLedger";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.Id, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "PFGeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "PFGeneralLedger.doc");
            }
            return View();
        }



        // GET:Receipt & Payment Statement Report
        [HttpGet]

        public ActionResult ReceiptPaymentStatementReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetReceiptPaymentStatementReport(ReportCustomModel model)
        {

            string reportName = "";
            reportName = "ReceiptPaymentStatement";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatement.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatement.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PropertiesReceiptPaymentReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetPropertiesReceiptPaymentReport(ReportCustomModel model)
        {

            string reportName = "";
            reportName = "PropertiesReceivedPayments";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CostCenterId={5}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CostCenterId ?? 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatement.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatement.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult AccountingMovement(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                CompanyName = company.Name + " (" + company.ShortName + ")",
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult AccountingMovementReports(ReportCustomModel model)
        {
            string reportName = "ISSAccountingMovement";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&LayerNo={3}&StrFromDate={4}&StrToDate={5}&CompanyId={6}", reportName, model.ReportType, model.HeadGLId, model.LayerNo, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult FeedSalesAndCollection(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult FeedSalesAndCollectionReports(ReportCustomModel model)
        {

            string reportName = "";
            if (model.CompanyId == (int)CompanyName.KrishibidFeedLimited)
            {
                reportName = "FeedSalesAndCollectionReport";
            }
            else
            {
                reportName = model.ReportName;
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&Head4Id={5}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.Head4Id.Value);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult GetRawConsumeViaProduction(string StrFromDate, string StrToDate, int CompanyId, int FProductId)
        {
            string reportName = "FeedRMConsumeViaProductionReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&FProductId={5}", reportName, "PDF", StrFromDate, StrToDate, CompanyId, FProductId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GcclCustomerStatement(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                CompanyName = company.Name + " (" + company.ShortName + ")",
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }


        [HttpGet]

        public ActionResult GcclCustomerStatementReports(ReportCustomModel model)
        {
            string reportName = "GCCLCustomerStatement";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.HeadGLId, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GcclTrritoryReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                CompanyName = company.Name + " (" + company.ShortName + ")",
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ZoneList = _configrationService.ZoneDropDownList(companyId),
            };
            return View(cm);
        }




        [HttpGet]

        public ActionResult GcclTerritoryReports(ReportCustomModel model)
        {
            string reportName = "GcclTerritoryReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.ZoneId == null) model.ZoneId = 0;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.ZoneId, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        [HttpGet]
        public ActionResult AccountingMovementInternal(int HeadGLId, int LayerNo, string StrFromDate, string StrToDate, int CompanyId)
        {
            string reportName = "ISSAccountingMovement";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&LayerNo={3}&StrFromDate={4}&StrToDate={5}&CompanyId={6}", reportName, "PDF", HeadGLId, LayerNo, StrFromDate, StrToDate, CompanyId);

            return File(client.DownloadData(reportURL), "application/pdf");

        }

        [HttpGet]

        public ActionResult AccountingAdvancedLedger(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                CompanyName = company.Name + " (" + company.ShortName + ")",
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult AccountingAdvancedLedgerReports(ReportCustomModel model)
        {
            string reportName = "AccountingAdvancedLedger";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&LayerNo={3}&StrFromDate={4}&StrToDate={5}&CompanyId={6}", reportName, model.ReportType, model.HeadGLId, model.LayerNo, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult AccountingAdvancedLedgerReportsInternal(int AccHeadId, int LayerNo, string StrFromDate, string StrToDate, int CompanyId)
        {
            string reportName = "AccountingAdvancedLedger";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&LayerNo={3}&StrFromDate={4}&StrToDate={5}&CompanyId={6}", reportName, "PDF", AccHeadId, LayerNo, StrFromDate, StrToDate, CompanyId);

            return File(client.DownloadData(reportURL), "application/pdf");
            return View();
        }

        [HttpGet]

        public ActionResult AccountingReceivableDetails(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult AccountingReceivableDetailsReport(ReportCustomModel model)
        {

            string reportName = "";
            if (model.CompanyId == (int)CompanyName.KrishibidSeedLimited)
            {

                reportName = "AccountingReceivableDetailsSeed";
            }
            else
            {
                reportName = model.ReportName;
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult KPLCollectionStatement(int companyId, string reportName, string reportNameDetails)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                NoteReportName = reportNameDetails,
                CostCenterList = new SelectList(_accountingService.CostCenterDropDownList(companyId), "Value", "Text"),
                VoucherTypeList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text")
            };
            return View(cm);
        }


        [HttpGet]

        public ActionResult KPLCollectionStatementView(ReportCustomModel model)
        {
            string reportName = "";

            reportName = model.ReportName;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&CostCenterId={5}&VoucherTypeId={6}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.CostCenterId ?? 0, model.VoucherTypeId = 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult CollectionStatement(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult CollectionStatementView(ReportCustomModel model)
        {
            string reportName = "";

            reportName = model.ReportName;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult CustomerLedger(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString(), ReportName = reportName };
            return View(cm);
        }

        [HttpGet]

        public ActionResult CustomerLedgerReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&VendorId={5}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.VendorId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Sales Return Report
        [HttpGet]

        public ActionResult GetSalesReturnReport(int saleReturnId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&SaleReturnId={1}", reportName, saleReturnId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        // GET: Sales Return Report
        [HttpGet]

        public ActionResult GetCustomerReport(int vendorId, int vendorTypeId, string vendorType)
        {
            string reportURL = string.Empty;
            string reportName = string.Empty;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;

            if (vendorTypeId == 2)
            {
                if (vendorType.Equals("Cash"))
                {
                    reportName = "CashCustomerInformation";
                    reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&VendorId={1}", reportName, vendorId);
                }

                if (vendorType.Equals("Credit"))
                {
                    reportName = "CreditCustomerInformation";
                    reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&VendorId={1}", reportName, vendorId);
                }

            }
            if (vendorTypeId == 1)
            {
                reportName = "SupplierInformation";
                reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&VendorId={1}", reportName, vendorId);
            }

            return File(client.DownloadData(reportURL), "application/pdf");
        }

        // GET: Balance Sheet Report
        [HttpGet]

        [ParentAuthorizedAttribute]
        public ActionResult BalanceSheet(int companyId, string balanceSheetReportName, string noteReportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = balanceSheetReportName,
                NoteReportName = noteReportName

            };
            return View(rcm);
        }







        [HttpGet]

        public ActionResult GetBalanceSheetReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;
            long ReportCategoryId = 0;
            string reportURL = "";
            //if (model.CompanyId==28||model.CompanyId==8||model.CompanyId==24||model.CompanyId==20||model.CompanyId==19)
            //{
            //    reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&CostCenterId={4}&ReportCategoryId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.CostCenterId ?? 0, ReportCategoryId);
            //}
            //else
            //{
            //    reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&CostCenterId={4}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.CostCenterId ?? 0);
            //}
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&CostCenterId={4}&ReportCategoryId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.CostCenterId ?? 0, ReportCategoryId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]
        public ActionResult ApprovalBalanceSheetReport(int companyId, string reportName, int month, int years, int ReportGroup, long ReportCategoryId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            string reportURL = "";
            client.Credentials = nwc;
            string ReportType = "PDF";
            int CostCenterId = 0;
            var lastDayOfMonth = DateTime.DaysInMonth(years, month);

            string StrFromDate = "01" + "/" + month + "/" + years;
            string StrToDate = lastDayOfMonth + "/" + month + "/" + years;
            if (ReportGroup == 1)
            {
                reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&CostCenterId={4}&ReportCategoryId={5}", reportName, ReportType, companyId, StrToDate, CostCenterId, ReportCategoryId);
            }
            else if (ReportGroup == 2)
            {
                reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CostCenterId={5}&ReportCategoryId={6}", reportName, ReportType, companyId, StrFromDate, StrToDate, CostCenterId, ReportCategoryId);

            }
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult KTTLShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName
            };
            return View(rcm);
        }

        [HttpGet]

        public ActionResult NFFLShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult SODLShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult OPLShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult KTTLShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult KPLPoultryShareHolderPosition

            (int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult PoultryShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult KGeCShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult KGeCShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult SaltShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult SaltShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult FisherisShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult FisheriesShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult FEEDShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName
            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult FEEDShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PFPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult PFPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult PropertiesShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult PropertiesShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult SeedShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult SeedShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }
        [HttpGet]

        public ActionResult TradingShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult TradingShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult FirmShareHolderPosition(int companyId, string reportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName


            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult FirmShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Balance Sheet Report
        [HttpGet]

        public ActionResult StockTransfer(int companyId, string stockTransferDeliveryReportName, string stockTransferReceiveReportName, string stockTransferStockReportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                Stocks = stockInfoService.GetDepoSelectModels(companyId),
                StockTransferDelivery = stockTransferDeliveryReportName,
                StockTransferReceive = stockTransferReceiveReportName,
                StockTransferStock = stockTransferStockReportName
            };
            return View(rcm);
        }

        [HttpGet]

        public ActionResult KFMALStockTransfer(int companyId, string stockTransferDeliveryReportName, string stockTransferReceiveReportName, string stockTransferStockReportName)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                Stocks = stockInfoService.GetKFMALDepoSelectModels(companyId),
                StockTransferDelivery = stockTransferDeliveryReportName,
                StockTransferReceive = stockTransferReceiveReportName,
                StockTransferStock = stockTransferStockReportName
            };
            return View(rcm);
        }


        [HttpGet]

        public ActionResult GetStockTransferConsolidatedReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StockInfoId={3}&StrFromDate={4}&StrToDate={5}", model.ReportName, model.ReportType, model.CompanyId, model.StockId ?? 0, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GetKFMALStockTransferConsolidatedReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();

            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StockInfoId={3}&StrFromDate={4}&StrToDate={5}", model.ReportName, model.ReportType, model.CompanyId, model.StockId ?? 0, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Stock Transfer Report
        [HttpGet]

        public ActionResult GetStockTransferReport(int stockTransferId, string reportName, int companyId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&StockTransferId={1}&CompanyId={2}", reportName, stockTransferId, companyId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GetStockReceivdReport(int stockTransferId, string reportName, int companyId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&StockTransferId={1}&CompanyId={2}", reportName, stockTransferId, companyId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }



        [HttpGet]

        public ActionResult ProfitLoss(int companyId, string reportName, string noteReportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                NoteReportName = noteReportName,
                CostCenters = voucherTypeService.GetAccountingCostCenter(companyId)
            };
            return View(cm);
        }

        //---Ani---
        [HttpGet]

        public async Task<ActionResult> ShortCredditReport(int companyId)
        {

            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),


                VendorsList = await _configrationService.VendorCompanyWise(companyId)
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult ShortCredditReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = "";
            model.ReportName = "FeedShortCredit";
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&VendorId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.VendorId ?? 0);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GetProfitLossReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = "";
            long ReportCategoryId = 0;
            //if (model.CompanyId == 28 || model.CompanyId == 8 || model.CompanyId == 24 || model.CompanyId == 20 || model.CompanyId == 19)
            //{
            //    reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CostCenterId={5}&ReportCategoryId={6}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CostCenterId ?? 0, ReportCategoryId);
            //}
            //else
            //{
            //    reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CostCenterId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CostCenterId ?? 0);
            //}
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CostCenterId={5}&ReportCategoryId={6}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CostCenterId ?? 0, ReportCategoryId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }



        [HttpGet]

        public ActionResult ProductionGcclReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                CostCenters = voucherTypeService.GetAccountingCostCenter(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GcclProductionReportView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "GCCLProductionReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult PackagingProductionReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                CostCenters = voucherTypeService.GetAccountingCostCenter(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult PackagingProductionReportView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "ISSProductionReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }




        [HttpGet]

        public ActionResult SeedSalesCollectionReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                CostCenters = voucherTypeService.GetAccountingCostCenter(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult SeedSalesCollectionView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "SeedSalesCollectionStatement";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult ProfitLossService(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                CostCenters = voucherTypeService.GetAccountingCostCenter(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult ProfitLossServiceReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            long ReportCategoryId = 0;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CostCenterId={5}&ReportCategoryId={6}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CostCenterId ?? 0, ReportCategoryId);
            //string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CostCenterId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CostCenterId ?? 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        // GET: Balance Sheet Report
        [HttpGet]

        public ActionResult InventoryReport(int companyId, string reportName, string noteReportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                NoteReportName = noteReportName,
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId)
            };
            return View(cm);
        }


        // GET: Balance Sheet Report
        [HttpGet]

        public ActionResult PurchaseRegisterReport(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
            };

            return View(cm);
        }


        [HttpGet]

        public ActionResult CategoryWisePurchaseRegisterReport(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
            };

            return View(cm);
        }



        // GET: Sales Register Report
        [HttpGet]

        public ActionResult SalesRegisterReport(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel sr = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
            };

            return View(sr);
        }
        // GET: Customer List Report
        [HttpGet]

        public ActionResult CustomerListReport(int companyId, string reportName)
        {

            Session["CompanyId"] = companyId;
            ReportCustomerModel rcl = new ReportCustomerModel()
            {
                CompanyId = companyId,
                ReportName = reportName,
                ZoneFk = 0,
                ZoneList = _configrationService.GetZoneList(companyId),
                //SubZoneList = _configrationService.GetSubZoneList(companyId, 0),
                SubZoneFk = 0
            };

            return View(rcl);
        }
        [HttpGet]

        public ActionResult InventoryReportView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = "";
            if (model.CompanyId == 24 | model.CompanyId == 10)

            {
                reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&StockInfoId={5}&Common_ProductCategoryFk={6}&Common_ProductSubCategoryFk={7}&Common_ProductFK={8}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, 0, model.ProductCategoryId ?? 0, model.ProductSubCategoryId ?? 0, model.ProductId ?? 0); //, model.CostCenterId ?? 0

            }
            else
            {
                reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&Common_ProductCategoryFk={3}&Common_ProductSubCategoryFk={4}&Common_ProductFK={5}&StrFromDate={6}&StrToDate={7}", model.ReportName, model.ReportType, model.CompanyId, model.ProductCategoryId ?? 0, model.ProductSubCategoryId ?? 0, model.ProductId ?? 0, model.StrFromDate, model.StrToDate); //, model.CostCenterId ?? 0

            }

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult PurchaseRegisterReportView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;


            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductId={5}&ProductCategoryId={6}&ProductSubCategoryId={7}&VendorId={8}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductId ?? 0, model.ProductCategoryId ?? 0, model.ProductSubCategoryId ?? 0, model.VendorId ?? 0); //, model.CostCenterId ?? 0

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult CustomerListReportView(ReportCustomerModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;


            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ZoneId={3}&SubZoneId={4}", model.ReportName, model.ReportType, model.CompanyId, model.ZoneFk ?? 0, model.SubZoneFk ?? 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult SalesRegisterReportView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;


            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductId={5}&ProductCategoryId={6}&ProductSubCategoryId={7}&VendorId={8}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductId ?? 0, model.ProductCategoryId ?? 0, model.ProductSubCategoryId ?? 0, model.VendorId ?? 0); //, model.CostCenterId ?? 0

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: General Ledger Report
        [HttpGet]

        public ActionResult ProductList(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, FromDate = DateTime.Now, ToDate = DateTime.Now };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetProductListReport(ReportCustomModel model)
        {
            string reportName = "ProductList";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ProductType={3}", reportName, model.ReportType, model.CompanyId, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult KfmalProductList(int companyId, char ProductType, string ReportType)
        {
            string reportName = "KfmalProductList";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ProductType={3}", reportName, "PDF", companyId, ProductType);
            return File(client.DownloadData(reportURL), "application/pdf");
        }


        // GET: Stock Receive Report
        [HttpGet]

        public ActionResult GetFinishProductStoreReport(long storeId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&StoreId={1}", reportName, storeId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }


        // GET: Date Wise Sale Qty & Amount Report
        [HttpGet]

        public ActionResult DateWiseSaleQtyAndAmount(int companyId, string reportName, string productType)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = reportName,
                ProductType = productType,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                //ProductCategoryList = voucherTypeService.GetProductCategory(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetDateWiseSaleQtyAndAmount(ReportCustomModel model)
        {

            if (model.CompanyId == (int)CompanyName.GloriousCropCareLimited)
            {
                model.ReportName = "CustomerWisaSaleReport";
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&ProductType={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GcclDateWiseSale(int companyId)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()

            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetGcclDateWiseSale(ReportCustomModel model)
        {

            if (model.CompanyId == (int)CompanyName.GloriousCropCareLimited)
            {
                model.ReportName = "GcclCustomerWisaSaleReport";
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        // GET: Date Wise Sale Qty & Amount Report
        [HttpGet]

        public ActionResult DepotWiseSaleQtyAndAmount(int companyId, string reportName, string productType)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = reportName,
                ProductType = productType,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetDepotWiseSaleQtyAndAmount(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&ProductType={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }



        // GET: Item Wise Sale Status Report
        [HttpGet]

        public ActionResult ItemWiseSaleStatus(int companyId, string productType)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductType = productType,
                Title = productType == "F" ? "Finished Goods Item Wise Sales Report" : "Raw Material Item Wise Sales Report",

            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetItemWiseSaleStatusReport(ReportCustomModel model)
        {
            string reportName = "ItemWiseSaleStatus";
            int companyId = Convert.ToInt32(Session["companyId"]);

            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                reportName = "KFMALItemWiseSalesReport";
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductType={5}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ItemWiseSaleStatus.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ItemWiseSaleStatus.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult GcclItemWiseSale(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()


            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetGcclItemWiseSale(ReportCustomModel model)
        {
            string reportName = "GcclItemWiseSaleStatus";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ItemWiseSaleStatus.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ItemWiseSaleStatus.doc");
            }
            return View();
        }


        // GET: Depo Wise Sales  Report
        [HttpGet]

        public ActionResult DepoWiseSalesReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            cm.Stocks = stockInfoService.GetStockInfoSelectModels(companyId);
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetDepoWiseSaleStatusReport(ReportCustomModel model)
        {
            string reportName = "KFMALDepoWiseSalesReport";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&StockInfoId={5}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.StockId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ItemWiseSaleStatus.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ItemWiseSaleStatus.doc");
            }
            return View();
        }

        // GET: Monthly Sale Item Wise
        [HttpGet]

        public ActionResult MonthlySaleItemWise(int companyId)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                ProductCategoryList = voucherTypeService.EnumerableYearRange()

            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetMonthlySaleItemWiseReport(ReportCustomModel model)
        {
            string reportName = "";
            if (model.CompanyId == (int)CompanyName.GloriousCropCareLimited)
            {
                reportName = "MonthlyItemWiseSaleGccl";
            }
            else
            {
                reportName = "MonthlySaleItemWise";
            }

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&year={3}&month={4}", reportName, model.ReportType, model.CompanyId, model.Year, model.Month);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "MonthlySaleItemWise.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "MonthlySaleItemWise.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult InvoiceWisaSale(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetInvoiceWisaSaleReport(ReportCustomModel model)
        {
            string reportName = "";
            if (model.CompanyId == (int)CompanyName.GloriousCropCareLimited)
            {
                reportName = "GcclInvoiceWisaSaleReport";
            }
            else
            {
                reportName = "InvoiceWisaSaleReport";
            }

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "InvoiceWisaSaleReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "InvoiceWisaSaleReport.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult SupplierWisaSale(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetSupplierWisaSaleReport(ReportCustomModel model)
        {
            string reportName = "SupplierWisaSaleReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "InvoiceWisaSaleReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "InvoiceWisaSaleReport.doc");
            }
            return View();
        }

        // GET: Monthly Return Item Wise
        [HttpGet]

        public ActionResult MonthlyReturnItemWise(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                ProductCategoryList = voucherTypeService.EnumerableYearRange()
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetMonthlyReturnItemWiseReport(ReportCustomModel model)
        {
            string reportName = "MonthlyReturnItemWise";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&year={3}&month={4}", reportName, model.ReportType, model.CompanyId, model.Year, model.Month);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "MonthlySaleItemWise.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "MonthlySaleItemWise.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult SeedProductStockReport(int companyId, string reportName)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                Stocklist = new SelectList(_service.StockInfoesDropDownt(companyId), "Value", "Text"),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
                ReportName = reportName
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult GetSeedProductStockReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "SeedProductStockReport";
            if (model.StockId == null)
            {
                model.StockId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }

            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&Common_ProductCategoryFk={3}&StrToDate={4}&CompanyId={5}&Common_ProductSubCategoryFk={6}&Common_ProductFK={7}&StockInfoId={7}", model.ReportName, model.ReportType, model.StrFromDate, model.ProductCategoryId, model.StrToDate, model.CompanyId, model.ProductSubCategoryId, model.ProductId, model.StockId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        // GET: Monthly Return Item Wise
        [HttpGet]

        public ActionResult MonthlyPurchaseItemWise(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                ProductCategoryList = voucherTypeService.EnumerableYearRange()

            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetMonthlyPurchaseItemWiseReport(ReportCustomModel model)
        {
            string reportName = "MonthlyPurchaseItemWise";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&year={3}&month={4}", reportName, model.ReportType, model.CompanyId, model.Year, model.Month);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "MonthlySaleItemWise.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "MonthlySaleItemWise.doc");
            }
            return View();
        }

        // GET: Monthly Return Item Wise
        [HttpGet]

        public ActionResult ISSAdjustmentReport(int companyId)
        {
            DateTime fromDate;
            DateTime toDate;
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            fromDate = firstDayOfMonth;
            toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = fromDate,
                ToDate = toDate,
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),


            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetISSAdjustmentItemWiseReport(ReportCustomModel model)
        {
            string reportName = "ISSAdjustmentReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "MonthlySaleItemWise.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "MonthlySaleItemWise.doc");
            }
            return View();
        }

        // GET: Raw Material Stock Report
        [HttpGet]

        public ActionResult RMStock(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetRMStockReport(ReportCustomModel model)
        {
            string reportName = "RawMaterialStockReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}", reportName, model.ReportType, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RawMaterialStockReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RawMaterialStockReport.doc");
            }
            return View();
        }


        // GET: Finish Product Stock Report Depo Wise
        [HttpGet]

        public ActionResult StockReport(int companyId, string reportName, string productType)
        {
            string title = string.Empty;
            List<SelectModel> stockSelectModels = new List<SelectModel>();
            if (productType.ToLower().Equals("r"))
            {
                title = "Raw Material Stock Report";
                stockSelectModels = stockInfoService.GetFactorySelectModels(companyId);
            }
            else if (productType.ToLower().Equals("f"))
            {
                title = "Finish Product Stock Report";
                stockSelectModels = stockInfoService.GetALLStoreSelectModels(companyId);
            }
            else
            {

            }
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductType = productType,
                Title = title,
                Stocks = stockSelectModels
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult ChemicalStockReport(int companyId, string reportName, string productType)
        {
            string title = string.Empty;
            if (productType.ToLower().Equals("r"))
            {
                title = "Chemical Stock Report";
            }


            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductType = productType,
                Title = title,
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult SparepartsStockReport(int companyId, string reportName, string productType)
        {
            string title = string.Empty;
            if (productType.ToLower().Equals("r"))
            {
                title = "Spare Parts Stock Report";
            }


            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductType = productType,
                Title = title,
            };
            return View(cm);
        }


        [HttpGet]

        public ActionResult GetSparepartsStockReport(ReportCustomModel model)
        {

            model.ReportName = "SparepartsStockReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL;
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GetStockReportDepoWise(ReportCustomModel model)
        {
            if (model.StockId.Value.Equals(0))
            {
                model.ReportName = "StockReportAll";
            }

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL;
            if (model.StockId == 0)
            {
                reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);
            }
            else
            {
                reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&StockInfoId={5}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.StockId);
            }

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult StockReportFinishedFeed(int companyId, string reportName, string productType)
        {
            string title = string.Empty;
            List<SelectModel> stockSelectModels = new List<SelectModel>();

            stockSelectModels = stockInfoService.GetALLStoreSelectModels(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductType = productType,
                Title = "Finish Product Stock Report",
                Stocks = stockSelectModels
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult StockReportFinishedFeedView(ReportCustomModel model)
        {
            if (model.StockId.Value.Equals(0))
            {
                model.ReportName = "FinishedFeedStockReport";
            }

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL;
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);


            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult GetChemicalStockReport(ReportCustomModel model)
        {

            model.ReportName = "ChemicalStockReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL;
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: MRR Search with Date Range
        [HttpGet]

        public ActionResult MRRSearch(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetMRRSearchReport(ReportCustomModel model)
        {
            string reportName = "Feed_MRRSearchByDateRange";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "Feed_MRRSearchByDateRange.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "Feed_MRRSearchByDateRange.doc");
            }
            return View();
        }


        // GET: Daily RM consumption Report
        [HttpGet]

        public ActionResult DailyRMConsumption(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, FromDate = DateTime.Now, ToDate = DateTime.Now };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetDailyRMConsumptionReport(ReportCustomModel model)
        {
            string reportName = "Feed_RMComsumptionDaily";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&Date={2}&CompanyId={3}", reportName, model.ReportType, model.ToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "Feed_RMComsumptionDaily.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "Feed_RMComsumptionDaily.doc");
            }
            return View();
        }

        // GET: Depo Wise Sale Status Report
        [HttpGet]

        public ActionResult CommonSale(int companyId, string reportName, string title)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                Title = title,
                CompanyId = companyId,
                ReportName = reportName,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            cm.Stocks = stockInfoService.GetStockInfoSelectModels(companyId);
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetCommonSaleReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StockInfoId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", model.ReportName, model.ReportType, model.StockId ?? 0, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Report
        [HttpGet]

        public ActionResult GetStockAdjustReport(int stockAdjustId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&StockAdjustId={1}", reportName, stockAdjustId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult GlorySupportReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GeneralGlorySupportReport(ReportCustomModel model)
        {
            string reportName = "GloryFeedSupport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&VendorId={2}&StrFromDate={3}&StrToDate={4}", reportName, model.ReportType, model.VendorId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GloryFeedSupport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GloryFeedSupport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult ZoneWiseCustomer(int companyId)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportName = "ZoneWiseCustomer";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}", reportName, companyId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }


        public ActionResult ProductionSearch(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName
            };
            return View(cm);
        }


        public ActionResult Product_Wise_ProductionSearch(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "Feed_Product-wais-ProductionReport"
            };
            return View(cm);
        }


        [HttpGet]

        public ActionResult Product_Wise_ProductionSearchReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GetProductionSearchReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GetPurchaseOrderTemplateReport(long purchaseOrderId, string reportType)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportName = purchaseOrderService.GetPurchaseOrderTemplateReportName(purchaseOrderId);

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&PurchaseOrderId={2}", reportName, reportType, purchaseOrderId);


            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", reportName + ".xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (reportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", reportName + ".doc");
            }
            return View();

            //return File(client.DownloadData(reportURL), "application/msword", reportName + ".doc");

        }


        //[HttpGet]
        //
        //public ActionResult GetPurchaseOrderTemplateReport(long purchaseOrderId)
        //{

        //    NetworkCredential nwc = new NetworkCredential(admin, password);
        //    WebClient client = new WebClient();
        //    client.Credentials = nwc;
        //    string reportName = purchaseOrderService.GetPurchaseOrderTemplateReportName(purchaseOrderId);

        //    string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&PurchaseOrderId={2}", reportName, "WORD", purchaseOrderId);

        //    return File(client.DownloadData(reportURL), "application/msword", reportName + ".doc");

        //}


        public ActionResult ShareHolderSearch(int companyId, string reportName, bool all)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,

                Companies = all ? companyService.GetCompanySelectModels() : companyService.GetCompanySelectModelById(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetShareHolderReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", model.ReportName, model.ReportType, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }



        public ActionResult CustomerSearch(int companyId, string reportName, bool all)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,

                Companies = all ? companyService.GetCompanySelectModels() : companyService.GetCompanySelectModelById(companyId)
            };
            return View(cm);
        }




        [HttpGet]
        public ActionResult FarmerSearch(int CompanyId, bool all = true)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                //CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                Companies = all ? companyService.GetCompanySelectModels() : companyService.GetCompanySelectModelById(CompanyId)
            };
            return View(cm);
        }

        //public ActionResult GetFarmerSearchReport(ReportCustomModel model)
        //{
        //    NetworkCredential nwc = new NetworkCredential(admin, password);
        //    WebClient client = new WebClient();
        //    model.ReportName = "FarmerReport";
        //    client.Credentials = nwc;
        //    string reportURL = string.Format("http://192.168.0.7:98/ReportServer_SQLEXPRESS/?%2fErpReport/{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", model.ReportName, model.ReportType, model.CompanyId);

        //    if (model.ReportType.Equals(ReportType.EXCEL))
        //    {
        //        return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
        //    }
        //    if (model.ReportType.Equals(ReportType.PDF))
        //    {
        //        return File(client.DownloadData(reportURL), "application/pdf");
        //    }
        //    if (model.ReportType.Equals(ReportType.WORD))
        //    {
        //        return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
        //    }
        //    return View();
        //}
        [HttpGet]

        public ActionResult GetFarmerSearchReport(ReportCustomModel model)
        {
            model.ReportName = "FarmerReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", model.ReportName, model.ReportType, model.CompanyId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "OutstandingAmountandCollactionReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "OutstandingAmountandCollactionReport.doc");
            }
            return View();
        }









        [HttpGet]

        public ActionResult GetCustomerSearchReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format("http://192.168.0.7:98/ReportServer_SQLEXPRESS/?%2fErpReport/{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", model.ReportName, model.ReportType, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }



        public ActionResult VoucherSearch(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName
            };
            return View(cm);
        }



        [HttpGet]

        public ActionResult GetVoucherSearchReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }




        public ActionResult VoucherTypeSearch(int companyId, string reportName)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                VoucherTypes = voucherTypeService.GetVoucherTypeSelectModels(companyId),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetVoucherTypeSearchReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&VoucherTypeId={3}&StrFromDate={4}&StrToDate={5}", model.ReportName, model.ReportType, model.CompanyId, model.VoucherTypeId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        public ActionResult VoucherByVoucherNo(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = reportName
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetVoucherByVoucherNoReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&VoucherNo={2}&CompanyId={3}", model.ReportName, model.ReportType, model.VoucherNo, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Balance Sheet Report
        [HttpGet]

        public ActionResult TrailBalance(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                CompanyName = company.Name

            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetTrailBalanceReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "KGTrailBalance";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&LayerNo={5}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.Id);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Customer Wise Monthly Sales Report
        [HttpGet]

        public ActionResult CustomerWiseMonthlySales(int companyId, string productType, string reportName, string title)
        {
            Session["CompanyId"] = companyId;//Use to store CompanyId data into session to pass into report server url
            ReportCustomModel cm = new ReportCustomModel()
            {
                //CompanyId = companyId,
                //Years = companyService.GetSaleYearSelectModel(),
                //ProductType = productType,
                //ReportName = reportName,
                //Title = title
                Title = title,
                CompanyId = companyId,
                ProductType = productType,
                ReportName = reportName
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetCustomerWiseMonthlySalesReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductType={5}&VendorId={6}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductType, model.VendorId ?? 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Customer Wise Monthly Sales Report
        [HttpGet]

        public ActionResult CustomerWiseMonthlySaleYearBasis(int companyId, string reportName, string productType)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Years = companyService.GetSaleYearSelectModel(),
                ProductType = productType,
                ReportName = reportName,
                Vendors = vendorService.GetCustomerSelectModels(companyId, productType),
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetCustomerWiseMonthlySaleYearBasisReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&VendorId={3}&Year={4}&ProductType={5}", model.ReportName, model.ReportType, model.CompanyId, model.VendorId, model.Year, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GetFeedPurchaseReport(long storeId, int companyId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&StoreId={2}", reportName, companyId, storeId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        // GET: Customer Wise Monthly Sales Report
        [HttpGet]

        public ActionResult MarketingOfferWiseSale(int companyId, string reportName)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = reportName,
                Employees = officerAssignService.GetMarketingOfficersSelectModels(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetMarketingOfficerWiseSale(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&EmployeeId={3}&StrFromDate={4}&StrToDate={5}", model.ReportName, model.ReportType, model.CompanyId, model.EmployeeId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Customer Wise Monthly Sales Report
        [HttpGet]

        public ActionResult MarketingOfficerWiseCustomers(int companyId, string reportName)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = reportName,
                Employees = officerAssignService.GetMarketingOfficerSelectModelsFromOrderMaster(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetMarketingOfficerWiseCustomerReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&MarketingOfficerId={3}&StrFromDate={4}&StrToDate={5}", model.ReportName, model.ReportType, model.CompanyId, model.EmployeeId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        // GET: Zone Wise  Sales Report
        [HttpGet]

        public ActionResult DepotWiseSale(int companyId, string reportName, string productType, string reportTitle)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                Title = reportTitle,
                CompanyId = companyId,
                ReportName = reportName,
                ProductType = productType,
                Stocks = stockInfoService.GetALLStoreSelectModels(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetGetDepotWiseSaleReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ProductType={3}&StockInfoId={4}&StrFromDate={5}&StrToDate={6}", model.ReportName, model.ReportType, model.CompanyId, model.ProductType, model.StockId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        // GET: Zone Wise  Sales Report
        [HttpGet]

        public ActionResult ZoneWiseSale(int companyId, string reportName, string productType, string reportTitle)
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                Title = reportTitle,
                CompanyId = companyId,
                ReportName = reportName,
                ProductType = productType,
                Stocks = stockInfoService.GetALLZoneSelectModels(companyId)
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult GetGetZoneWiseSaleReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ProductType={3}&ZoneId={4}&StrFromDate={5}&StrToDate={6}", model.ReportName, model.ReportType, model.CompanyId, model.ProductType, model.ZoneId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }



        // GET: Customer Report Item Wise Sales Report
        [HttpGet]

        public ActionResult CustomerReportItemWiseSale(int companyId, string productType, string reportName, string title)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                Title = title,
                CompanyId = companyId,
                ProductType = productType,
                ReportName = reportName
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetCustomerReportItemWiseSale(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductType={5}&CustomerId={6}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductType, model.VendorId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Supplier Wise Purchase Report
        [HttpGet]

        public ActionResult SupplierWisePurchase(int companyId, string reportName, string title)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                Title = title,
                CompanyId = companyId,
                ReportName = reportName
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetSupplierWisePurchaseSale(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&VendorId={3}&StrFromDate={4}&StrToDate={5}", model.ReportName, model.ReportType, model.CompanyId, model.VendorId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        // GET: Report
        [HttpGet]

        public ActionResult GetPurchaseReturnReport(string purchaseReturnId)
        {
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            string reportURL;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            reportURL = string.Format(url + "ISSPurchaseReturn&rs:Command=Render&rs:Format=PDF&PurchaseReturnId=" + purchaseReturnId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        //GET: Account Cheque Info

        [HttpGet]

        public ActionResult GetActChequeInfoReport(int companyId, int actChequeInfoId, string reportName)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&actChequeInfoId={2}", reportName, companyId, actChequeInfoId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        //SeedReceiptPayment View
        [HttpGet]

        public ActionResult SeedReceiptPayment(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Companies = companyService.GetCompanySelectModels(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult PackagingReceiptPayment(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Companies = companyService.GetCompanySelectModels(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult PrintingReceiptPayment(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Companies = companyService.GetCompanySelectModels(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult SODLReceiptPayment(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Companies = companyService.GetCompanySelectModels(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult GLDLReceiptPayment(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Companies = companyService.GetCompanySelectModels(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult GCCLReceiptPayment(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Companies = companyService.GetCompanySelectModels(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult KFBLReceiptPayment(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                Companies = companyService.GetCompanySelectModels(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString()
            };
            return View(cm);
        }


        //GET: ReceiptPayment Statement Seed //SeedReceiptPaymentStatement

        [HttpGet]

        public ActionResult SeedReceiptPaymentReport(ReportCustomModel model)
        {
            var reportname = "";

            if (model.CompanyId == (int)CompanyName.KGECOM)
            {
                reportname = "KgComReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPackagingLimited)
            {
                reportname = "PackagingReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidTradingLimited)
            {
                reportname = "TradingReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.SonaliOrganicDairyLimited)
            {
                reportname = "SODLReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited)
            {
                reportname = "PrintingReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
            {
                reportname = "GLDLReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.GloriousCropCareLimited)
            {
                reportname = "GCCLReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidFoodAndBeverageLimited)
            {




























































































































































































                reportname = "KFBLReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidTradingLimited)
            {
                reportname = "TradingReceiptPaymentStatement";
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidSeedLimited)
            {
                reportname = "SeedReceiptPaymentStatement";
            }
            //hello r2
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={4}&CompanyId={1}&StrFromDate={2}&StrToDate={3}", reportname, model.CompanyId, model.StrFromDate, model.StrToDate, model.ReportType);

            if (model.CompanyId == (int)CompanyName.KrishibidSeedLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementSeed.xls");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPackagingLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementPackaging.xls");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPackagingLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementTrading.xls");
            }
            else if (model.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementGLDL.xls");
            }
            else if (model.CompanyId == (int)CompanyName.SonaliOrganicDairyLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementSODL.xls");
            }
            else if (model.CompanyId == (int)CompanyName.GloriousCropCareLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementGCCL.xls");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidFoodAndBeverageLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementKFBL.xls");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited && model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ReceiptPaymentStatementKPPL.xls");
            }

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }

            if (model.CompanyId == (int)CompanyName.KrishibidSeedLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementSeed.doc");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPackagingLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementPackaging.doc");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPackagingLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementTrading.doc");
            }
            else if (model.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementGLDL.doc");
            }
            else if (model.CompanyId == (int)CompanyName.SonaliOrganicDairyLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementSODL.doc");
            }
            else if (model.CompanyId == (int)CompanyName.GloriousCropCareLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementGCCL.doc");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidFoodAndBeverageLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementKFBL.doc");
            }
            else if (model.CompanyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited && model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ReceiptPaymentStatementKPPL.doc");
            }

            return View();
        }


        //kg3847-2022 start

        [HttpGet]

        public ActionResult DateWiseRawAttendance()
        {
            ReportCustomModel cm = new ReportCustomModel();
            cm.StrFromDate = DateTime.Now.ToShortDateString();
            return View(cm);
        }
        [HttpGet]

        public ActionResult DateWiseRawAttendanceReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "DateWiseRawAttendanceReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}", model.ReportName, model.ReportType, model.StrFromDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "DateWiseRawAttendanceReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "DateWiseRawAttendanceReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult IndividualAttendance(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult IndividualAttendanceSummaryReport(string employeeId, DateTime StrFromDate, DateTime StrToDate)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string ReportName = "EmployeeAttendanceReport";
            string ReportType = "PDF";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&EmployeeId={2}&StrFromDate={3}&StrToDate={4}", ReportName, ReportType, employeeId, StrFromDate, StrToDate);
            if (ReportType == "PDF")
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }
        [HttpGet]

        public ActionResult IndividualAttendanceReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "EmployeeAttendanceReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&EmployeeId={2}&StrFromDate={3}&StrToDate={4}", model.ReportName, model.ReportType, model.EmployeeKGId, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }




        [HttpGet]

        public ActionResult CompanyWiseSMSReport(int companyId, DateTime StrFromDate, DateTime StrToDate)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string ReportName = "CompanywiseSMSReport";
            string ReportType = "PDF";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}", ReportName, ReportType, companyId, StrFromDate, StrToDate);
            if (ReportType == "PDF")
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult VoucherList(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text"),
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult VoucherListReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&VoucherTypeId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.VmVoucherTypeId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult AdvancedAttendanceSearch(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                Departments = departmentService.GetDepartmentSelectModels(),
                Designations = designationService.GetDesignationSelectModels(),
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult AdvancedAttendanceSearchRepot(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "AdvancedAttendanceSearchReport";
            if (model.AttendanceStatusvalue != null)
            {
                if (model.AttendanceStatusvalue.Contains("&"))
                {
                    model.AttendanceStatusvalue = model.AttendanceStatusvalue.Replace("&", "%26");
                }
            }

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&EmployeeId={2}&DepartmentId={3}&DesignationId={4}&EmpStatus={5}&SalaryTag={6}&StrFromDate={7}&StrToDate={8}", model.ReportName, model.ReportType, model.EmployeeKGId, model.DepartmentId, model.DesignationId, model.AttendanceStatusvalue, model.SalaryTag, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult DateWiseSaleDetails(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text"),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
                ReportName = reportName
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult DateWiseSaleDetailsReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "DateWiseSaleDetailsReport";
            if (model.CustomerId == null)
            {
                model.CustomerId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }
            if (model.SubZoneFk == null)
            {
                model.SubZoneFk = 0;
            }
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CustomerId={5}&ProductId={6}&SubZoneId={7}&ProductCategoryId={8}&ProductSubCategoryId={9}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CustomerId, model.ProductId, model.SubZoneFk, model.ProductCategoryId, model.ProductSubCategoryId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult DateWisePartySaleDetails(int companyId, string reportName)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                SupplierList = new SelectList(_service.GetSupplier(companyId), "Value", "Text"),
                SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text"),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
                ReportName = reportName
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult DateWisePartySaleDetailsReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "DateWisePartySaleDetailsReport";
            if (model.VendorId == null)
            {
                model.VendorId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }
            if (model.SubZoneFk == null)
            {
                model.SubZoneFk = 0;
            }
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&VendorId={5}&ProductId={6}&SubZoneId={7}&ProductCategoryId={8}&ProductSubCategoryId={9}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.VendorId, model.ProductId, model.SubZoneFk, model.ProductCategoryId, model.ProductSubCategoryId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult DateWiseReturnDetails(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text"),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),

            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult DateWiseReturnDetailsReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "DateWiseReturnDetailsReport";
            if (model.CustomerId == null)
            {
                model.CustomerId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }
            if (model.SubZoneFk == null)
            {
                model.SubZoneFk = 0;
            }
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CustomerId={5}&ProductId={6}&SubZoneId={7}&ProductCategoryId={8}&ProductSubCategoryId={9}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CustomerId, model.ProductId, model.SubZoneFk, model.ProductCategoryId, model.ProductSubCategoryId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]
        public ActionResult SeedCustomerAgeing(int companyId = 0)
        {
            VmCustomerAgeing vmCustomerAgeing = new VmCustomerAgeing();
            vmCustomerAgeing.CompanyFK = companyId;
            vmCustomerAgeing.ZoneListList = new SelectList(_service.ZonesDropDownList(companyId), "Value", "Text");
            vmCustomerAgeing.TerritoryList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            return View(vmCustomerAgeing);

        }

        [HttpPost]

        public ActionResult CustomerAgeingReportView(VmCustomerAgeing model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "CustomerAgeingSeedReport";


            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&AsOnDate={3}&ZoneId={4}&SubZoneId={5}", model.ReportName, model.ReportType, model.CompanyFK.Value, model.AsOnDate, model.ZoneId ?? 0, model.SubZoneId ?? 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]
        public ActionResult SeedCustomerAgeingDetails(int CompanyId, int CustomerId, string AsOnDate, string reportName, string reportFormat)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&CustomerId={3}&AsOnDate={4}", reportName, reportFormat, CompanyId, CustomerId, AsOnDate);
            if (reportFormat == "EXCEL")
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", reportName + ".xls");
            }
            if (reportFormat == "PDF")
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }

            return null;
        }

        [HttpGet]

        public ActionResult GCCLShareHolderPosition(int companyId)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
            };
            return View(rcm);
        }

        [HttpGet]

        public ActionResult GCCLShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            model.ReportName = "GCCLShareholderPosition";
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult KSSLShareHolderPosition(int companyId)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
            };
            return View(rcm);
        }

        [HttpGet]

        public ActionResult KSSLShareHolderPositionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            model.ReportName = "KSSLShareholderPosition";
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&CompanyId={3}", model.ReportName, model.ReportType, model.StrFromDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult CollectionExpenditureStatements(int companyId)
        {
            ReportCustomModel rcm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
            };
            return View(rcm);
        }

        [HttpPost]

        public ActionResult CollectionExpenditureStatements(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            model.ReportName = "CollectionExpenditureStatements";
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&StrFromDate={2}&StrToDate={3}", model.ReportName, model.CompanyId, model.StrFromDate, model.StrToDate);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetGCCLCostOfGoodsSoldsDetails(int companyId, string strFromDate, string strToDate, long costCenterId, string reportName)
        {
            reportName = "GetGCCLCostOfGoodsSoldsDetails";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&StrFromDate={2}&StrToDate={3}&CostCenterId={4}", reportName, companyId, strFromDate, strToDate, costCenterId);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        [HttpGet]

        public ActionResult SupplierProductReport(int companyId, string reportName, int customerId)
        {
            reportName = "SupplierProductReport";
            int ProductId = 0;
            string strFromDate = "01/01/2015";
            string strToDate = DateTime.Now.ToString("dd/MM/yyyy");
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format=PDF&CompanyId={1}&CustomerId={2}&ProductId={3}&StrFromDate={4}&StrToDate={5}", reportName, companyId, customerId, ProductId, strFromDate, strToDate);
            return File(client.DownloadData(reportURL), "application/pdf");
        }

        //kg3847 End


        [HttpGet]

        public ActionResult ReturnLedgerFeed(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);
        }

        [HttpGet]

        public ActionResult ReturnLedgerFeedReport(ReportCustomModel model)
        {

            string reportName = "";
            reportName = "FeedReturnAdjustment";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PurchaseRegisterFeedReport(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
            };

            return View(cm);
        }




        [HttpGet]

        public ActionResult PurchaseRegisterFeedView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;


            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&VendorId={5}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.VendorId ?? 0); //, model.CostCenterId ?? 0

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PurchaseReturnLedgerFeed(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);
        }

        [HttpGet]

        public ActionResult PurchaseReturnLedgerFeedView(ReportCustomModel model)
        {

            string reportName = "";
            reportName = "FeedPurchaseReturnAdjustment";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult FeedPaymentStatement(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult FeedPaymentStatementView(ReportCustomModel model)
        {
            string reportName = "";

            reportName = model.ReportName;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult FeedPurchaseStatement(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult FeedPurchaseStatementView(ReportCustomModel model)
        {
            string reportName = "";

            reportName = model.ReportName;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult FeedSalesStatement(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult FeedSalesStatementView(ReportCustomModel model)
        {
            string reportName = "";

            reportName = model.ReportName;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "BankCashBookSeed.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }


        //Real Estate Report START
        [HttpGet]

        public ActionResult ProductBookingInformation(int companyId, long CGId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.CGId = CGId;
            model.ReportType = "PDF";
            string reportName = "";
            reportName = "RealEstateBookingInfoReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&CGId={3}", reportName, model.ReportType, model.CompanyId, model.CGId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult MoneyReceipts(int companyId, long moneyReceiptId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.MoneyReceiptId = moneyReceiptId;
            model.ReportType = "PDF";
            string reportName = "";
            // reportName = "RealStateMoneyReceiptsReport";
            reportName = "RealStateMoneyReceiptsReportsignature";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&MoneyReceiptId={3}", reportName, model.ReportType, model.CompanyId, model.MoneyReceiptId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public async Task<ActionResult> RealEstateCustomerTransactionHistoryReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
            };
            cm.ProjectList = await _moneyReceiptService.ProjectList(companyId);
            return View(cm);
        }
        [HttpPost]

        public ActionResult RealEstateCustomerTransactionHistoryReport(ReportCustomModel model)
        {
            string reportName = "";
            reportName = "RealEstateCustomerTransactionHistory";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&CGId={3}", reportName, model.ReportType, model.CompanyId, model.CGId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "TransactionHistory.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "TransactionHistory.doc");
            }
            return View();
        }

        [HttpGet]

        public async Task<ActionResult> RealStatePaymentStatementReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
            };
            cm.ProjectList = await _moneyReceiptService.ProjectList(companyId);
            return View(cm);
        }
        [HttpPost]

        public ActionResult RealStatePaymentStatementReport(ReportCustomModel model)
        {
            string reportName = "";
            reportName = "RealStatePaymentStatementReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&CGId={3}", reportName, model.ReportType, model.CompanyId, model.CGId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "PaymentStatement.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "PaymentStatement.doc");
            }
            return View();
        }

        [HttpGet]
        public ActionResult RealEstateProjectWaisBooking(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
                ReportName = "RealEstateProjectWaisBookingReport"
            };
            return View(cm);
        }


        [HttpPost]
        public ActionResult RealEstateProjectWaisBooking(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "RealEstateProjectWaisBookingReport";

            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CGId={5}&ProductId={6}&ProductCategoryId={7}&ProductSubCategoryId={8}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.CGId, model.ProductId, model.ProductCategoryId, model.ProductSubCategoryId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ProjectWaisBookingRepor.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ProjectWaisBookingRepor.doc");
            }
            return View();
        }


        [HttpGet]

        public async Task<ActionResult> RealStateMonthlyCollectionReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "RealStateMonthlyCollectionReport",
                ProjectList = await _moneyReceiptService.ProjectList(companyId)
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult RealStateMonthlyCollectionReport(ReportCustomModel model)
        {
            string reportName = "";
            reportName = "RealStateMonthlyCollectionReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.ProjectId == null)
            {
                model.ProjectId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProjectId={5}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProjectId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealStateMonthlyCollectionReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RealStateMonthlyCollectionReport.doc");
            }
            return View();
        }



        [HttpGet]

        public async Task<ActionResult> MoneyReceiptsReportList(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                PayType = _moneyReceiptService.GetPaymentMethodSelectModels(),
                ProjectList = await _moneyReceiptService.ProjectList(companyId),
                ReportName = "RealStateMoneyReceiptsReportListReport"
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult MoneyReceiptsReportList(ReportCustomModel model)
        {
            string reportName = "";
            reportName = "RealStateMoneyReceiptsReportListReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&PayTypeId={5}&ProjectId={6}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.PayTypeId ?? 0, model.ProjectId ?? 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealStateMonthlyCollectionReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RealStateMonthlyCollectionReport.doc");
            }
            return View();
        }
        [HttpGet]

        public async Task<ActionResult> RealEstateProjectWaisCollaction(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "RealEstateProjectWaisCollaction",
                ProjectList = await _moneyReceiptService.ProjectList(companyId)
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult RealEstateProjectWaisCollaction(ReportCustomModel model)
        {
            string reportName = "";
            reportName = "RealEstateProjectWaisCollaction";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.ProjectId == null) { model.ProjectId = 0; }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProjectId={5}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProjectId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealEstateProjectWaisCollaction.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RealEstateProjectWaisCollaction.doc");
            }
            return View();
        }

        //Real Estate Month Wise(Quarter) Collection Report
        [HttpGet]

        public async Task<ActionResult> RealEstateMonthWiseCollection(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "RealEstateMonthWiseCollection",
                //ProjectList = await _moneyReceiptService.ProjectList(companyId)
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult RealEstateMonthWiseCollection(ReportCustomModel model)
        {
            string reportName = "";
            reportName = "RealEstateMonthWiseCollection";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            DateTime fromDate = DateTime.ParseExact(model.StrFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(model.StrToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            model.StrFromDate = fromDate.ToString("MM/dd/yyyy");
            model.StrToDate = toDate.ToString("MM/dd/yyyy");
            //if (model.ProjectId == null) { model.ProjectId = 0; }
            //string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StartDate={2}&EndDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealEstateMonthWiseCollection.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RealEstateMonthWiseCollection.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult RealStateMonthlyInstallmentReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "RealStateMonthlyInstallmentReport",
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult RealStateMonthlyInstallmentReport(ReportCustomModel model)
        {
            string reportName = "";
            reportName = model.ReportName;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", " RealStateMonthlyInstallment.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", " RealStateMonthlyInstallment.doc");
            }
            return View();
        }


        //Real Estate Report START

        [HttpGet]

        public ActionResult CustReportYearlyCarryingAndIncentive(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                SupplierList = new SelectList(_service.GetSupplier(companyId), "Value", "Text"),
                ReportName = "CustomerReportYearlyCarryingAndIncentive"
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult CustReportYearlyCarryingAndIncentive(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "CustomerReportYearlyCarryingAndIncentive";

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&VendorId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.VendorId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ProjectWaisBookingRepor.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ProjectWaisBookingRepor.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult RawAttendanceReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "Attendance_Hr_Check_Report",
                HoursList = new SelectList(hours(), "Value", "Text"),
                Minutes = new SelectList(minites(), "Value", "Text"),
                FromHour = 8,
                FromMinut = 0,
                ToHour = 18,
                ToMinut = 0,
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult RawAttendanceReport(ReportCustomModel model)
        {
            //model.EmployeeId = 0;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            var fromhours = model.FromHour + ":" + model.FromMinut;
            var tohours = model.ToHour + ":" + model.ToMinut;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&StrFromTime={4}&StrToTime={5}&employeeId={6}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, fromhours, tohours, model.EmployeeId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RawAttendanceReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");

            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RawAttendanceReport.doc");
            }
            return View();

        }
        #region Attendance Report
        [HttpGet]

        public ActionResult AbsentReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "Absent_Hr_Check_Report",
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult AbsentReport(ReportCustomModel model)
        {

            //model.EmployeeId = 0;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&SalaryTag={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.SalaryTag);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RawAttendanceReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");

            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RawAttendanceReport.doc");
            }
            return View();

        }
        [HttpGet]

        public ActionResult LateReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "Late_Hr_Check_Report",
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult LateReport(ReportCustomModel model)
        {

            //model.EmployeeId = 0;
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&SalaryTag={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.SalaryTag);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RawAttendanceReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");

            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RawAttendanceReport.doc");
            }
            return View();

        }
        #endregion Attendance Report
        private List<SelectMarketingModel> hours()
        {
            List<SelectMarketingModel> hours = new List<SelectMarketingModel>();
            hours.Add(new SelectMarketingModel { Text = "12 AM", Value = 0 });
            hours.Add(new SelectMarketingModel { Text = "1 AM", Value = 1 });
            hours.Add(new SelectMarketingModel { Text = "2 AM", Value = 2 });
            hours.Add(new SelectMarketingModel { Text = "3 AM", Value = 3 });
            hours.Add(new SelectMarketingModel { Text = "4 AM", Value = 4 });
            hours.Add(new SelectMarketingModel { Text = "5 AM", Value = 5 });
            hours.Add(new SelectMarketingModel { Text = "6 AM", Value = 6 });
            hours.Add(new SelectMarketingModel { Text = "7 AM", Value = 7 });
            hours.Add(new SelectMarketingModel { Text = "8 AM", Value = 8 });
            hours.Add(new SelectMarketingModel { Text = "9 AM", Value = 9 });
            hours.Add(new SelectMarketingModel { Text = "10 AM", Value = 10 });
            hours.Add(new SelectMarketingModel { Text = "11 AM", Value = 11 });
            hours.Add(new SelectMarketingModel { Text = "12 PM", Value = 12 });
            hours.Add(new SelectMarketingModel { Text = "1 PM", Value = 13 });
            hours.Add(new SelectMarketingModel { Text = "2 PM", Value = 14 });
            hours.Add(new SelectMarketingModel { Text = "3 PM", Value = 15 });
            hours.Add(new SelectMarketingModel { Text = "4 PM", Value = 16 });
            hours.Add(new SelectMarketingModel { Text = "5 PM", Value = 17 });
            hours.Add(new SelectMarketingModel { Text = "6 PM", Value = 18 });
            hours.Add(new SelectMarketingModel { Text = "7 PM", Value = 19 });
            hours.Add(new SelectMarketingModel { Text = "8 PM", Value = 20 });
            hours.Add(new SelectMarketingModel { Text = "9 PM", Value = 21 });
            hours.Add(new SelectMarketingModel { Text = "10 PM", Value = 22 });
            hours.Add(new SelectMarketingModel { Text = "11 PM", Value = 23 });
            var List = new List<object>();

            //for (int i = 0; i < 13; i++)
            //{
            //    List.Add(new
            //    {
            //        Value = i,
            //        Text = i.ToString()
            //    });
            //}
            return hours;
        }
        private List<object> minites()
        {
            var List = new List<object>();
            for (int i = 0; i < 65; i = i + 5)
            {
                List.Add(new
                {
                    Value = i,
                    Text = i.ToString()
                });
            }
            return List;
        }

        [HttpGet]

        public ActionResult Leave_HR_Check_Report(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "Leave_Hr_Check_Report"
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult Leave_HR_Check_Report(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "Leave_HR_Check_Report.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "Leave_HR_Check_Report.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult DateWiseSaleReturnReport(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                SupplierList = new SelectList(_service.GetSupplier(companyId), "Value", "Text"),
                ReportName = "DateWiseSaleReturnReport"
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult DateWiseSaleReturnReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.VendorId == null)
            {
                model.VendorId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&CustomerId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.VendorId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "DateWiseSaleReturnReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "DateWiseSaleReturnReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult IncentivSummeryReport(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = "IncentiveSummaryReport",
                YearsList = _appService.YearsListLit()
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult IncentivSummeryReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&Month={3}&Years={4}", model.ReportName, model.ReportType, model.CompanyId, model.RMonths, model.RYears);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "IncentivSummeryReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "IncentivSummeryReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult IncentivApprovalReport(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = "IncentiveApprovalReport",
                YearsList = _appService.YearsListLit(),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId)
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult IncentivApprovalReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&Month={3}&Years={4}&ProjectId={5}", model.ReportName, model.ReportType, model.CompanyId, model.RMonths, model.RYears, model.ProjectId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "IncentivApprovalReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "IncentivApprovalReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult IncentivProvideReport(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                ReportName = "IncentiveProvideReport",
                YearsList = _appService.YearsListLit(),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId)
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult IncentivProvideReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.ProjectId == null)
            {
                model.ProjectId = 0;
            }

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&Month={3}&Years={4}&ProjectId={5}", model.ReportName, model.ReportType, model.CompanyId, model.RMonths, model.RYears, model.ProjectId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "IncentiveProvideReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "IncentiveProvideReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult IncentivGeneratedReport(int companyId, long MasterId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.CGId = MasterId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "IncentiveGeneratedReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&MasterId={3}", model.ReportName, model.ReportType, model.CompanyId, model.CGId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult IncentiveDistributationChartReport(int companyId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "IncentiveDistributationChartReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", model.ReportName, model.ReportType, model.CompanyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult IncentiveChartDetailsReport(int companyId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "IncentiveDistributationChartDetailsReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", model.ReportName, model.ReportType, model.CompanyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }
        [HttpGet]

        public async Task<ActionResult> RealEstateTeamWiseBooking(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                TeamLedarList = new SelectList(teamService.GetTeamByCompanyId(companyId), "Value", "Text"),
                ProjectList = await _moneyReceiptService.ProjectList(companyId),
                ReportName = "RealEstateTeamWiseBookingReport"
            };
            return View(cm);
        }
        [HttpPost]

        public async Task<ActionResult> RealEstateTeamWiseBooking(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.ProjectId == null)
            {
                model.ProjectId = 0;
            }
            if (model.TeamsId == null)
            {
                model.TeamsId = 0;
            }
            if (model.SalesPersonId == null)
            {
                model.SalesPersonId = 0;
            }

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ProductCategoryId={3}&TeamId={4}&StrFromDate={5}&SalePersionId={6}&StrToDate={7}", model.ReportName, model.ReportType, model.CompanyId, model.ProjectId, model.TeamsId, model.StrFromDate, model.SalesPersonId, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealEstateTeamWiseBooking.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RealEstateTeamWiseBooking.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult staticdataForGeneral(int companyid, int AccHeadId)
        {
            ReportCustomModel cm = new ReportCustomModel();
            cm.ReportType = "PDF";
            cm.StrToDate = DateTime.Now.ToString();
            string reportName = "";
            reportName = "ISSGeneralLedger";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, cm.ReportType, AccHeadId, "11/07/1995", cm.StrToDate, companyid);

            if (cm.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }
        #region Calculative Attendance
        [HttpGet]

        public ActionResult CalculativeAttendance(int companyId = 0)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1),
            };
            cm.StrFromDate = cm.FromDate.Value.ToString("yyyy-MM-dd");
            cm.StrToDate = cm.ToDate.Value.ToString("yyyy-MM-dd");
            return View(cm);
        }
        [HttpPost]

        public ActionResult CalculativeAttendance(ReportCustomModel model)
        {
            model.ReportName = "CalculativeAttendanceReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            int businessType = 0;
            businessType = (int)model.BusinessType;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&fromDate={2}&toDate={3}&salaryTag={4}&businessType={5}&companyId={6}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.SalaryTag, businessType, 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "Leave_HR_Check_Report.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "Leave_HR_Check_Report.doc");
            }
            return View();
        }

        #endregion Calculative Attendance

        [HttpGet]

        public ActionResult MonthlyAttendenceUpdateVersion(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1),
            };
            cm.StrFromDate = cm.FromDate.Value.ToString("yyyy-MM-dd");
            cm.StrToDate = cm.ToDate.Value.ToString("yyyy-MM-dd");
            return View(cm);
        }
        [HttpPost]

        public ActionResult GetMonthlyAttendenceUpdateVersion(ReportCustomModel model)
        {
            model.ReportName = "MonthlyAttendanceDepartmentWise";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&FromDate={2}&ToDate={3}&SalaryTag={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.officeid);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "Leave_HR_Check_Report.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "Leave_HR_Check_Report.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GeneralRequisitionReport(int companyId, long masterId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "GeneralRequisitionReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&MasterId={2}&CompanyId={3}", model.ReportName, model.ReportType, masterId, companyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PackagingProductionRequisitionRep(int requisitionId, int companyId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "PackagingProductionRequisitionRep";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&RequisitionId={2}&CompanyId={3}", model.ReportName, model.ReportType, requisitionId, companyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult KPLProdReferenceReport(int prodReferenceId, int companyId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "ISSProdReferenceReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&prodReferenceId={2}&CompanyId={3}", model.ReportName, model.ReportType, prodReferenceId, companyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult QualityExceptionComplaintReport(long QualityExceptionComplaintId, long QualityExceptionComplaintDetailId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "QualityExceptionComplaintReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&QualityExceptionComplaintId={2}&QualityExceptionComplaintDetailId={3}", model.ReportName, model.ReportType, QualityExceptionComplaintId, QualityExceptionComplaintDetailId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        public ActionResult AprAndAcrReport(long AnnualPerformanceId, long EmployeeId, long AnnualPerformanceDetailId, string EmployeeCode)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "AprAndAcrReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AnnualPerformanceId={2}&EmployeeId={3}&AnnualPerformanceDetailId={4}&EmployeeCode={5}", model.ReportName, model.ReportType, AnnualPerformanceId, EmployeeId, AnnualPerformanceDetailId, EmployeeCode);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PackagingIssueProductReport(long IssuedMasterId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "PackagingIssueProductReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&IssuedMasterId={2}", model.ReportName, model.ReportType, IssuedMasterId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult RecruitmentRequisitionReport(int companyId, long id)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "RecruitmentRequisitionReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&id={2}&companyId={3}", model.ReportName, model.ReportType, id, companyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }


        [HttpGet]

        public ActionResult EDocumentReport(int companyId, long masterId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "EDocumentReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&MasterId={2}&CompanyId={3}", model.ReportName, model.ReportType, masterId, companyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PurchaseReturnList(int companyId)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };
            return View(cm);
        }


        [HttpGet]

        public ActionResult PurchaseReturnListInvoice(int CompanyId, int id)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = CompanyId;
            model.Id = id;
            model.ReportType = ReportType.PDF;
            model.ReportName = "PurchaseReturnInvoicet";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;

            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&PurchaseReturnId={3}", model.ReportName, model.ReportType, model.CompanyId, model.Id);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "Leave_HR_Check_Report.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "Leave_HR_Check_Report.doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult GcclSubZoneWisaSaleReport(int companyId, string productType)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductType = productType,
                Title = productType == "F" ? "Finished Goods Item Wise Sales Report" : "Raw Material Item Wise Sales Report",
                ZoneList = _configrationService.GCClZoneDropDownList(companyId),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult GcclSubZoneWisaSaleReport(ReportCustomModel model)
        {
            if (model.SubZoneId == null)
            {
                model.SubZoneId = 0;
            }
            if (model.ZoneId == null)
            {
                model.ZoneId = 0;
            }
            if (model.VendorId == null)
            {
                model.VendorId = 0;
            }
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }
            string reportName = "GcclSubZoneWisaSaleReport";
            int companyId = Convert.ToInt32(Session["companyId"]);

            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                reportName = "GcclSubZoneWisaSaleReport";
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            //string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductType={5}&SubZoneId={6}&ZoneId={7}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductType, model.SubZoneId,model.ZoneId);
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&SubZoneId={5}&ZoneId={6}&VendorId={7}&ProductCategoryId={8}&ProductSubCategoryId={9}&ProductId={10}&ProductType={11}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.SubZoneId, model.ZoneId, model.VendorId, model.ProductCategoryId, model.ProductSubCategoryId, model.ProductId, model.ProductType);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GcclSubZoneWisaSaleReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GcclSubZoneWisaSaleReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult FeedCustomerInfoReport(int companyId, long vendorId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.ReportType = "PDF";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "FeedCustomerInfo";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&VendorId={2}&CompanyId={3}", model.ReportName, model.ReportType, vendorId, companyId);

            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult ReturnRegisterReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
                ZoneList = _configrationService.GCClZoneDropDownList(companyId),
            };
            return View(cm);
        }
        [HttpPost]

        public ActionResult ReturnRegisterReport(ReportCustomModel model)
        {

            if (model.SubZoneId == null)
            {
                model.SubZoneId = 0;
            }
            if (model.ZoneId == null)
            {
                model.ZoneId = 0;
            }
            if (model.VendorId == null)
            {
                model.VendorId = 0;
            }
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }
            string reportName = "GcclReturnRegisterReport";
            int companyId = Convert.ToInt32(Session["companyId"]);

            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                reportName = "GcclReturnRegisterReport";
            }
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&SubZoneId={5}&ZoneId={6}&VendorId={7}&ProductCategoryId={8}&ProductSubCategoryId={9}&ProductId={10}&ProductType={11}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.SubZoneId, model.ZoneId, model.VendorId, model.ProductCategoryId, model.ProductSubCategoryId, model.ProductId, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GcclReturnRegisterReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GcclReturnRegisterReport.doc");
            }
            return View();
        }
        [HttpGet]

        public ActionResult EmployeeAttendanceSearch()
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToString("01-01-2001"),
                StrToDate = DateTime.Now.ToShortDateString(),
                Departments = departmentService.GetDepartmentSelectModels(),
                Designations = designationService.GetDesignationSelectModels(),
                Grades = designationService.GetGradeSelectModels(),
                DivisionList = new SelectList(_configrationService.CommonDivisionsDropDownList(), "Value", "Text"),
                DistrictList = new SelectList(_configrationService.CommonDistrictsDropDownList(), "Value", "Text"),
                UpazilasList = new SelectList(_configrationService.CommonUpazilasDropDownList(), "Value", "Text"),
                Religions = dropDownItemService.GetDropDownItemSelectModels(9),
                BloodGroups = dropDownItemService.GetDropDownItemSelectModels(5),
                OfficeTypes = dropDownItemService.GetDropDownItemSelectModels(12),
                JobStatus = dropDownItemService.GetDropDownItemSelectModels(15),
                JobTypes = dropDownItemService.GetDropDownItemSelectModels(10),
                MaritalTypes = dropDownItemService.GetDropDownItemSelectModels(2),
                Genders = dropDownItemService.GetDropDownItemSelectModels(3),
                Companies = companyService.GetAllCompanySelectModels2(),
            };
            return View(cm);
        }


        [HttpPost]

        public ActionResult EmployeeAttendanceSearch(ReportCustomModel model)
        {
            if (model.CompanyId == null) { model.CompanyId = 0; }
            if (model.SalaryTag == null) { model.SalaryTag = 0; }
            if (model.DepartmentId == null) { model.DepartmentId = 0; }
            if (model.DesignationId == null) { model.DesignationId = 0; }
            if (model.EmployeeId == null) { model.EmployeeId = 0; }
            if (model.Common_DivisionFk == null) { model.Common_DivisionFk = 0; }
            if (model.Common_DistrictsFk == null) { model.Common_DistrictsFk = 0; }
            if (model.Common_UpazilasFk == null) { model.Common_UpazilasFk = 0; }
            if (model.ReligionId == null) { model.ReligionId = 0; }
            if (model.BloodGroupId == null) { model.BloodGroupId = 0; }
            if (model.MaritalTypeId == null) { model.MaritalTypeId = 0; }
            if (model.GenderId == null) { model.GenderId = 0; }
            if (model.OfficeTypeId == null) { model.OfficeTypeId = 0; }
            if (model.JobStatusId == null) { model.JobStatusId = 0; }
            if (model.ServiceTypeId == null) { model.ServiceTypeId = 0; }
            if (model.OfficeTypeId == null) { model.OfficeTypeId = 0; }
            if (model.StartGradeId == null) { model.StartGradeId = 0; }
            if (model.EndGradeId == null) { model.EndGradeId = 0; }
            if (model.EmployeeStatus == null) { model.EmployeeStatus = 0; }

            string reportName = "";
            reportName = "EmployeeInventoryReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}" +
                "&CompanyId={2}" +
                "&StrFromDate={3}" +
                "&StrToDate={4}" +
                "&SalaryTag={5}" +
                "&DepartmentId={6}" +
                "&DesignationId={7}" +
                "&EmployeeId={8}" +
                "&DivisionId={9}" +
                "&DistrictId={10}" +
                "&UpazilaId={11}" +
                "&BloodGroupId={12}" +
                "&ReligionId={13}" +
                "&MaritalTypeId={14}" +
                "&GenderId={15}" +
                "&JobStatusId={16}" +
                "&ServiceTypeId={17}" +
                "&OfficeTypeId={18}" +
                "&EmployeeStatus={19}" +
                "&StartGradeId={20}" +
                "&EndGradeId={21}",
                reportName,
                model.ReportType,
                model.CompanyId,
                model.StrFromDate,
                model.StrToDate,
                model.SalaryTag,
                model.DepartmentId,
                model.DesignationId,
                model.EmployeeId,
                model.Common_DivisionFk,
                model.Common_DistrictsFk,
                model.Common_UpazilasFk,
                model.BloodGroupId,
                model.ReligionId,
                model.MaritalTypeId,
                model.GenderId,
                model.JobStatusId,
                model.ServiceTypeId,
                model.OfficeTypeId,
                model.EmployeeStatus,
                  model.StartGradeId,
                  model.EndGradeId
                );

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "EmployeeInventoryReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "EmployeeInventoryReport.doc");
            }
            return View();
        }



        //[HttpGet]
        //
        //public async Task< ActionResult> EmployeeInventorySearch()
        //{
        //    ReportCustomModel cm = new ReportCustomModel()
        //    {
        //        FromDate = DateTime.Now,
        //        ToDate = DateTime.Now,
        //        StrFromDate = DateTime.Now.ToString("01-01-2001"),
        //        StrToDate = DateTime.Now.ToShortDateString(),                
        //        EmployeeLogTypes = await logServices.EmployeeLogType()
        //    };
        //    return View(cm);
        //}


        //[HttpPost]
        //
        //public ActionResult EmployeeInventorySearch(ReportCustomModel model)
        //{

        //    if (model.EmployeeStatus == null) { model.EmployeeStatus = 0; }

        //    string reportName = "";
        //    reportName = "EmployeeInventoryReport";
        //    NetworkCredential nwc = new NetworkCredential(admin, password);
        //    WebClient client = new WebClient();
        //    client.Credentials = nwc;
        //    string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}" +
        //        "&CompanyId={2}" +
        //        "&StrFromDate={3}" +
        //        "&StrToDate={4}" +
        //        "&SalaryTag={5}" +
        //        "&DepartmentId={6}" +
        //        "&DesignationId={7}" +
        //        "&EmployeeId={8}" +
        //        "&DivisionId={9}" +
        //        "&DistrictId={10}" +
        //        "&UpazilaId={11}" +
        //        "&BloodGroupId={12}" +
        //        "&ReligionId={13}" +
        //        "&MaritalTypeId={14}" +
        //        "&GenderId={15}" +
        //        "&JobStatusId={16}" +
        //        "&ServiceTypeId={17}" +
        //        "&OfficeTypeId={18}" +
        //        "&EmployeeStatus={19}",
        //        reportName,
        //        model.ReportType,
        //        model.CompanyId,
        //        model.StrFromDate,
        //        model.StrToDate,
        //        model.SalaryTag,
        //        model.DepartmentId,
        //        model.DesignationId,
        //        model.EmployeeId,
        //        model.Common_DivisionFk,
        //        model.Common_DistrictsFk,
        //        model.Common_UpazilasFk,
        //        model.BloodGroupId,
        //        model.ReligionId,
        //        model.MaritalTypeId,
        //        model.GenderId,
        //        model.JobStatusId,
        //        model.ServiceTypeId,
        //        model.OfficeTypeId,
        //        model.EmployeeStatus
        //        );

        //    if (model.ReportType.Equals(ReportType.EXCEL))
        //    {
        //        return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "EmployeeInventoryReport.xls");
        //    }
        //    if (model.ReportType.Equals(ReportType.PDF))
        //    {
        //        return File(client.DownloadData(reportURL), "application/pdf");
        //    }
        //    if (model.ReportType.Equals(ReportType.WORD))
        //    {
        //        return File(client.DownloadData(reportURL), "application/msword", "EmployeeInventoryReport.doc");
        //    }
        //    return View();
        //}


        [HttpGet]
        public ActionResult IndividualAttendanceHrCheckReport(string employeeId, string StrFromDate, string StrToDate)
        {
            string reportName = "Individual_Attendance_Hr_Check_Report";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&employeeId={2}&StrFromDate={3}&StrToDate={4}", reportName, "PDF", employeeId, StrFromDate, StrToDate);

            return File(client.DownloadData(reportURL), "application/pdf");

        }
        [HttpGet]

        public ActionResult KfmalBookingInformation(int companyId, long CGId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.CGId = CGId;
            model.ReportType = "PDF";
            string reportName = "";
            reportName = "KFMALBookingInfoReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&CGId={3}", reportName, model.ReportType, model.CompanyId, model.CGId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult Comparativestatmentdetails(int companyId, long CSId)
        {
            ReportCustomModel model = new ReportCustomModel();
            model.CompanyId = companyId;
            model.Csid = CSId;
            model.ReportType = "PDF";
            string reportName = "";
            reportName = "CSReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&CSID={3}", reportName, model.ReportType, model.CompanyId, model.Csid);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult OutstandingAmountandCollactionReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductCategoryList = voucherTypeService.GetProductCategoryKPL(companyId),
            };
            return View(cm);
        }



        [HttpPost]

        public ActionResult OutstandingAmountandCollactionReport(ReportCustomModel model)
        {
            model.ReportName = "RealEstateOutstandingandCollaction_with_File_Report";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportType = "PDF";
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&ProjectId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.ProductCategoryId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "OutstandingAmountandCollactionReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "OutstandingAmountandCollactionReport.doc");
            }
            return View();
        }


        [HttpPost]

        public ActionResult GetAdvanceDetails(ReportCustomModel model)
        {
            model.ReportName = "PayrolGetEmployeeAdvanceDetails";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportType = "PDF";
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&companyId={2}&paymentMonth={3}&paymentYear={4}", model.ReportName, model.ReportType, model.CompanyId, model.RMonths, model.RYears);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "OutstandingAmountandCollactionReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "OutstandingAmountandCollactionReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult RealEstateReturnReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductCategoryList = voucherTypeService.GetProductCategoryKPL(companyId),
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult RealEstateReturnReport(ReportCustomModel model)
        {
            model.ReportName = "RealEstate_Return_Report";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportType = "PDF";
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.RealEstateStatusId == null)
            { model.RealEstateStatusId = 0; }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&ProjectId={5}&StatusId={6}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.ProductCategoryId, model.RealEstateStatusId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealEstateReturnReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RealEstateReturnReport.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult RealEstateCostHeadsReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductCategoryList = voucherTypeService.GetProductCategoryKPL(companyId),
                CostHeadsList = new SelectList(_costHeadService.CostHeadsList(companyId), "Value", "Text")
            };
            return View(cm);
        }

        [HttpPost]

        public ActionResult RealEstateCostHeadsReport(ReportCustomModel model)
        {
            model.ReportName = "RealEstate_CostHead_and_collactionReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportType = "PDF";
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.CostHeadsId == null)
            {
                model.CostHeadsId = 0;
            }
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&ProjectId={5}&CostHeadId={6}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.ProductCategoryId, model.CostHeadsId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealEstateCostHeadReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "RealEstateCostHeadReport.doc");
            }
            return View();
        }





        //[HttpPost]
        //
        //public ActionResult ClientReport(CrmUploadListVm model)
        //{
        //    string reportName = "ClientUploadFile";
        //    reportName = "ClientUploadFile";
        //    NetworkCredential nwc = new NetworkCredential(admin, password);
        //    WebClient client = new WebClient();
        //    client.Credentials = nwc;
        //    string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", reportName, model.ReportType, model.CompanyId);

        //    if (model.ReportType.Equals(ReportType.EXCEL))
        //    {
        //        return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ClientList.xls");
        //    }
        //    if (model.ReportType.Equals(ReportType.PDF))
        //    {


        //        return File(client.DownloadData(reportURL), "application/pdf");
        //    }

        //    return View();


        //}

        [HttpGet]

        public ActionResult MultipleVoucherPrint(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                CompanyName = companyService.GetCompany(companyId).Name,
                VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text")

            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetMultipleVoucherPrint(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = "";
            model.ReportName = "KGMultipleVoucherReport";

            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&VoucherTypeId={5}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.VoucherTypeId ?? 0);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }



        [HttpGet]

        public ActionResult FeedItemWiseSale(int companyId, string productType)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductType = productType,
                Title = productType == "F" ? "Finished Goods Item Wise Sales Report" : "Raw Material Item Wise Sales Report",

            };
            return View(cm);
        }

        //ani

        [HttpGet]

        public ActionResult FeedSaleItemWiseReport(int companyId, string productType)
        {
            Session["CompanyId"] = companyId;

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductType = productType,

                Title = productType == "F" ? "Finished Goods Item Wise Sales Report" : "Raw Material Item Wise Sales Report",

            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult KFMALTructorSale(int companyId)
        {


            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }
        [HttpGet]

        public ActionResult GetKFMALTructorSale(ReportCustomModel model)
        {
            string reportName = "KFMALtractorSale";
            int companyId = Convert.ToInt32(Session["companyId"]);
            model.ProductType = "F";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;




            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&FromDate={2}&ToDate={3}&CompanyId={4}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "FeedSaleItemWise.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "FeedSaleItemWise.doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult GetFeedSaleItemWise(ReportCustomModel model)
        {
            string reportName = "FeedSaleItemWise";
            int companyId = Convert.ToInt32(Session["companyId"]);
            model.ProductType = "F";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductType={5}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "FeedSaleItemWise.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "FeedSaleItemWise.doc");
            }
            return View();
        }

        //ani
        [HttpGet]

        public ActionResult GlDLFileInformation(int companyId)
        {


            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId),
                Employees = officerAssignService.GetEmployyeKGRE(companyId),

            };
            return View(cm);
        }

        [HttpPost]
        public ActionResult GlDLFileInformationReport(ReportCustomModel model)
        {
            string reportName = "FileBokingHistory";
            int companyId = Convert.ToInt32(Session["companyId"]);
            model.ProductType = "F";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }

            if (model.EmplId == null)
            {
                model.EmplId = 0;
            }

            //string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&ProductCategoryId={5}&ProductSubCategoryId={6}&productId={7}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.ProductCategoryId,model.ProductSubCategoryId, model.ProductId);
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductCategoryId={5}&ProductSubCategoryId={6}&productId={7}&EmpId={8}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductCategoryId, model.ProductSubCategoryId, model.ProductId, model.EmplId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GlDLFileInformationReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GlDLFileInformationReport.doc");
            }
            return View();

        }

        //ani
        [HttpGet]

        public ActionResult GlDLBookingFileInformation(int companyId)
        {


            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,

                ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId),
                Employees = officerAssignService.GetEmployyeKGRE(companyId),

            };
            return View(cm);
        }

        [HttpPost]
        public ActionResult GlDLBookingFileInformationReport(ReportCustomModel model)
        {
            string reportName = "FileBokingHistorywithBookingDate";
            int companyId = Convert.ToInt32(Session["companyId"]);
            model.ProductType = "F";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            if (model.ProductCategoryId == null)
            {
                model.ProductCategoryId = 0;
            }
            if (model.ProductSubCategoryId == null)
            {
                model.ProductSubCategoryId = 0;
            }
            if (model.ProductId == null)
            {
                model.ProductId = 0;
            }

            if (model.EmplId == null)
            {
                model.EmplId = 0;
            }
            if (model.BookingDate == null)
            {
                model.BookingDate = "a";
            }
            if (model.StrToDate == null)
            {
                model.StrToDate = "a";
            }
            if (model.CreatedDate == null)
            {
                model.CreatedDate = "a";
            }
            if (model.CreatedToDate == null)
            {
                model.CreatedToDate = "a";
            }
            //string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}&StrToDate={4}&ProductCategoryId={5}&ProductSubCategoryId={6}&productId={7}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.ProductCategoryId,model.ProductSubCategoryId, model.ProductId);
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&ProductCategoryId={3}&ProductSubCategoryId={4}&productId={5}&EmpId={6}&BookingFromDate={7}&BookingToDate={8}&CreateFromDate={9}&CreateToDate={10}", reportName, model.ReportType, model.CompanyId, model.ProductCategoryId ?? 0, model.ProductSubCategoryId ?? 0, model.ProductId ?? 0, model.EmplId ?? 0, model.BookingDate, model.StrToDate, model.CreatedDate, model.CreatedToDate);


            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GlDLBookingFileInformationReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GlDLBookingFileInformationReport.doc");
            }
            return View();

        }

        [HttpGet]
        public ActionResult TotalDegreeCoount()
        {


            ReportCustomModel model = new ReportCustomModel();
            model.ReportType = ReportType.PDF;
            string reportName = "CountDegree";


            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}", reportName, model.ReportType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ItemWiseSaleStatus.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ItemWiseSaleStatus.doc");
            }
            return View();
        }




        [HttpGet]

        public ActionResult FeedItemWiseSaleReport(ReportCustomModel model)
        {

            int companyId = Convert.ToInt32(Session["companyId"]);

            string reportName = "ItemWiseSaleStatus";


            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductType={5}", reportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductType);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "ItemWiseSaleStatus.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ItemWiseSaleStatus.doc");
            }
            return View();
        }

        [HttpGet]

        [ParentAuthorizedAttribute]
        public ActionResult ShortCredit(int companyId)
        {
            Session["CompanyId"] = companyId;
            var company = _accountingService.GetCompanyById(companyId);
            ReportCustomModel cm = new ReportCustomModel() { CompanyId = companyId, CompanyName = company.Name + " (" + company.ShortName + ")", FromDate = DateTime.Now, ToDate = DateTime.Now, StrFromDate = DateTime.Now.ToShortDateString(), StrToDate = DateTime.Now.ToShortDateString() };
            return View(cm);
        }

        [HttpGet]

        public ActionResult ShortCreditReport(ReportCustomModel model)
        {
            string accCode = model.AccName.Substring(1, 13);
            string reportName = "";
            reportName = "ISSGeneralLedger";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&AccHeadId={2}&StrFromDate={3}&StrToDate={4}&CompanyId={5}", reportName, model.ReportType, model.Id, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }

        [HttpGet]

        public async Task<ActionResult> GetWorkReport(DateTime? fromDate, DateTime? toDate, int CompanyId = 1)
        {
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = 1,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                selectWorkLebel = await taskManagementEvolutionService.GetSelectWorkLebel(),
                SelectWorkState = await taskManagementEvolutionService.GetSelectWorkState(),
            };

            return View(cm);
        }

        [HttpPost]

        public ActionResult GetWorkingReport(ReportCustomModel model)
        {
            model.CompanyId = 1;
            model.ReportType = "PDF";
            string reportName = "";
            reportName = "GetWorksByDateRangeReport";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&FormStartDate={3}&ToEndDate={4}&Status={5}&WorkLabelId={6}&ManagerId={7}", reportName, model.ReportType, model.CompanyId, model.StrFromDate, model.StrToDate, model.WorkStatusId ?? 0, model.WorkLebelId ?? 0, model.ManagerId ?? 0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "GeneralLedger.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "GeneralLedger.doc");
            }
            return View();
        }
        #region leave_application_Report
        [HttpGet]

        public ActionResult NewLeaveManagement(int CompanyId, int LeaveApplicationID, string reportType, string applicationType)
        {

            CompanyName companyName = (CompanyName)CompanyId;
            string companyNameString = companyName.ToString();
            string reportName = "NewLeaveManagement";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&LeaveApplicationID={2}&CompanyId={3}&ApplicationType={4}", reportName, reportType, LeaveApplicationID, CompanyId, applicationType);
            if (reportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", companyNameString + " CompanyWiseSalarySheet.xls");
            }
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), companyNameString + " application/pdf");
            }
            return View();
        }
        #endregion



        [HttpGet]
        public ActionResult TaskProgressPrport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
            };
            return View(cm);
        }

        [HttpGet]
        public ActionResult SalesReport()
        {

            ReportCustomModel cm = new ReportCustomModel()
            {
                SaleTitle = _configrationService.GetActiveSalesTitles()

            };
            return View(cm);
        }
        [HttpPost]
        public ActionResult SalesReport(ReportCustomModel model)
        {

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "SalesReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&KGSalesId={2}", model.ReportName, model.ReportType, model.SaleTitleId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "SalesReport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "SalesReport.doc");
            }
            return View();

        }



        [HttpGet]
        public ActionResult KGSalesTargetReport(int SaleTitleId)
        {

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string ReportName = "KgSalesTargetReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&KGSalesId={2}", ReportName, "PDF", SaleTitleId);

            return File(client.DownloadData(reportURL), "application/pdf");
            return View();
        }


        [HttpGet]
        public ActionResult KGMonthlySalesReport(int SaleTitleId, int KgCompanySaleTergetId, int companyId)
        {

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string ReportName = "KgMonthlySalesTargetReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&KGSalesId={2}&KGCompanySaleTergetId={3}&CompanyId={4}", ReportName, "PDF", SaleTitleId, KgCompanySaleTergetId, companyId);


            return File(client.DownloadData(reportURL), "application/pdf");
            return View();
        }



        [HttpGet]
        public ActionResult KGSalesAchivementDetailReport(int SaleTitleId, int KGCompanySaleTergetId, int KgMonthlySaleTargetId, int CompanyId)
        {

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string ReportName = "KgSalesAchivementReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&KGSalesId={2}&KGCompanySaleTergetId={3}&KGCompanyMonthlySaleTergetId={4}&CompanyId={5}", ReportName, "PDF", SaleTitleId, KGCompanySaleTergetId, KgMonthlySaleTargetId, CompanyId);

            return File(client.DownloadData(reportURL), "application/pdf");
            return View();
        }



        [HttpPost]
        public ActionResult TaskProgressPrport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "TaskProgressPrport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "TaskProgressPrport.xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", "ProjectWaisBookingRepor.doc");
            }
            return View();
        }



        [HttpGet]

        public ActionResult PackagingRMStockReport(int companyId)
        {
            string title = string.Empty;

            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = "ISSRMStockReport",
                NoteReportName = "ISSRMStockSummeryReport",

                Title = title
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult GetPackagingRMStockReport(ReportCustomModel model)
        {



            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL;

            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }


        [HttpGet]

        public ActionResult StockReportFinishedISS(int companyId)
        {
            string title = string.Empty;
            List<SelectModel> stockSelectModels = new List<SelectModel>();

            stockSelectModels = stockInfoService.GetALLStoreSelectModels(companyId);
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                Title = "Finish Product Stock Report",
                Stocks = stockSelectModels
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult StockReportFinishedISSView(ReportCustomModel model)
        {
            model.ReportName = "ISSFinishedGoodsStockReport";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL;
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);


            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }
        public ActionResult BOMInvoice(int CompanyId, int OrderDetailId, string reportType)
        {

            CompanyName companyName = (CompanyName)CompanyId;
            string companyNameString = companyName.ToString();
            string reportName = "BOMInvoice";

            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&OrderDetailId={3}", reportName, reportType, CompanyId, OrderDetailId);
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), companyNameString + " application/pdf");
            }
            return View();
        }

        [HttpGet]

        public ActionResult RMRequisitionRegisterReport(int companyId, string reportName)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),
                ReportName = reportName,
                ProductCategoryList = voucherTypeService.GetProductCategory(companyId),
            };

            return View(cm);
        }

        [HttpGet]

        public ActionResult RMRequisitionRegisterReportView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;


            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}&ProductId={5}&ProductCategoryId={6}&ProductSubCategoryId={7}&VendorId={8}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId, model.ProductId ?? 0, model.ProductCategoryId ?? 0, model.ProductSubCategoryId ?? 0, model.VendorId ?? 0); //, model.CostCenterId ?? 0

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PackagingRMIssuedReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                CostCenters = voucherTypeService.GetAccountingCostCenter(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult PackagingRequisitionReport(int companyId)
        {
            Session["CompanyId"] = companyId;
            ReportCustomModel cm = new ReportCustomModel()
            {
                CompanyId = companyId,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                StrFromDate = DateTime.Now.ToShortDateString(),
                StrToDate = DateTime.Now.ToShortDateString(),

                CostCenters = voucherTypeService.GetAccountingCostCenter(companyId)
            };
            return View(cm);
        }

        [HttpGet]

        public ActionResult PackagingRMIssuedReportView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "PackagingRMIssuedReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public ActionResult PackagingRequisitionView(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            model.ReportName = "PackagingRequisitionReport";
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&StrFromDate={2}&StrToDate={3}&CompanyId={4}", model.ReportName, model.ReportType, model.StrFromDate, model.StrToDate, model.CompanyId);
            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]

        public async Task<ActionResult> KPLProductBookingDetails(int companyId)
        {
            //Session["CompanyId"] = companyId;
            companyId = 9;
            string reportName = "";
            string reportType = "PDF";
            reportName = "KPLProductBookingDetails";
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", reportName, reportType, companyId);
            if (reportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            return View();
        }

        //[HttpPost]

        //public ActionResult KPLProductBookingDetails(ReportCustomModel model)
        //{
        //    string reportName = "";
        //    model.ReportType = "PDF";
        //    reportName = "KPLProductBookingDetails";
        //    NetworkCredential nwc = new NetworkCredential(admin, password);
        //    WebClient client = new WebClient();
        //    client.Credentials = nwc;
        //    string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}", reportName, model.ReportType, model.CompanyId);

        //    //if (model.ReportType.Equals(ReportType.EXCEL))
        //    //{
        //    //    return File(client.DownloadData(reportURL), "application/vnd.ms-excel", "RealStateMonthlyCollectionReport.xls");
        //    //}
        //    if (model.ReportType.Equals(ReportType.PDF))
        //    {
        //        return File(client.DownloadData(reportURL), "application/pdf");
        //    }
        //    //if (model.ReportType.Equals(ReportType.WORD))
        //    //{
        //    //    return File(client.DownloadData(reportURL), "application/msword", "RealStateMonthlyCollectionReport.doc");
        //    //}
        //    return View();
        //}



    }
}