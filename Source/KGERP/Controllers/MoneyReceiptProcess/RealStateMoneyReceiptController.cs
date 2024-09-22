using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel.RealState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KGERP.Controllers.Custom_Authorization;
using KGERP.Data.CustomModel;
using KGERP.Utility;
using System.Security.Policy;

namespace KGERP.Controllers.MoneyReceiptProcess
{
    [CheckSession]
    public class RealStateMoneyReceiptController : Controller
    {
        private ERPEntities db = new ERPEntities();
        private readonly MoneyReceiptService _moneyReceiptService;
        IKgReCrmService kgReCrmService = new KgReCrmService();
        string password = "Sysroot@123";
        string admin = "Administrator";
        string url = "http://192.168.0.13/ReportServer/?%2fErpReport/";
        private readonly ICompanyService _companyService;
        private readonly AccountingService _accountingService;
        public RealStateMoneyReceiptController(AccountingService accountingService, MoneyReceiptService moneyReceiptService, ICompanyService companyService)
        {
            _moneyReceiptService = moneyReceiptService;
            _companyService = companyService;
            _accountingService = accountingService;
        }
        // GET: MoneyReceipt
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Create(int companyId, int moneyReceiptId = 0)
        {
            ////SendVerificationLinkEmail();
            MoneyReceiptViewModel model = new MoneyReceiptViewModel();
            var company = _companyService.GetCompany(companyId);
            if (moneyReceiptId > 0)
            {
                model = await _moneyReceiptService.MonyeReceiptDetails(moneyReceiptId);
                model.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");
                model.CompanyName = company.Name;
                model.CompanyId = companyId;
            }
            else
            {
                model.ProjectList = await _moneyReceiptService.ProjectList(companyId);
                model.MemMoneyReceiptType = await _moneyReceiptService.MoneyReceiptType(companyId);
                model.BankList = await _moneyReceiptService.BankList();
                model.PayType = _moneyReceiptService.GetPaymentMethodSelectModels();
                model.InstallmentList = new MultiSelectList(_moneyReceiptService.GetPaymentMethodSelectModels(), "Value", "Text");
                model.CompanyName = company.Name;
                model.CompanyId = companyId;
                model.ReceiptDateString = DateTime.Now.ToString("dd-MMM-yyyy");
                model.ChequeDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult GetClient(long CGId)
        {
            var crm = _moneyReceiptService.GetCline(CGId);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupListByProjectId(int companyId, long projectId)
        {
            var crm = _moneyReceiptService.GetGroupByProjectId(companyId, projectId);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ResaleGetByProjectId(int companyId, long projectId)
        {
            var crm = _moneyReceiptService.GetResaleGroupByProjectId(companyId, projectId);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult GetGroupListByProjectIdforIncentive(int companyId, long projectId)
        {
            var crm = _moneyReceiptService.GetGroupByProjectIdforIncentive(companyId, projectId);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupListByProjectIdforIncentiveNew(int companyId, long projectId)
        {
            var crm = _moneyReceiptService.GetGroupByProjectIdforIncentiveNEW(companyId, projectId);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public async Task<ActionResult> Create(MoneyReceiptViewModel model)
        {
            var res = await _moneyReceiptService.AddReceipt(model);
            return RedirectToAction(nameof(Create), new { companyId = model.CompanyId, moneyReceiptId = res.MoneyReceiptId });
        }

        [HttpPost]

        public async Task<ActionResult> PurposeUpdate(MoneyReceiptViewModel model)
        {
            var res = await _moneyReceiptService.PurposeUpdate(model);
            return RedirectToAction(nameof(Create), new { companyId = model.CompanyId, moneyReceiptId = res.MoneyReceiptId });
        }

        [HttpPost]

        public async Task<ActionResult> UpdateItem(MoneyReceiptViewModel model)
        {
            var res = await _moneyReceiptService.UpdateItem(model);
            return RedirectToAction(nameof(Create), new { companyId = model.CompanyId, moneyReceiptId = model.MoneyReceiptId });

        }


        [HttpPost]

        public async Task<ActionResult> MoneyReceiptSubmit(MoneyReceiptViewModel model)
        {
            MoneyReceiptViewModel moneyReceiptViewModel = new MoneyReceiptViewModel();
            if (model.IsExisting)
            {

                moneyReceiptViewModel = await _moneyReceiptService.MonyeReceiptDetailsForAccountingPush(model);
                moneyReceiptViewModel.Submitdate = Convert.ToDateTime(model.StringSubmidtate);
                var res = await _moneyReceiptService.MoneyReceiptStatusUpdate(moneyReceiptViewModel);
            }
            else
            {

                moneyReceiptViewModel = await _moneyReceiptService.MonyeReceiptDetailsForAccountingPush(model);
                model.Submitdate = Convert.ToDateTime(model.StringSubmidtate);
                moneyReceiptViewModel.Submitdate = Convert.ToDateTime(model.StringSubmidtate);
                var voucherId = await _accountingService.GldlKplCollectionPush(model.CompanyId, moneyReceiptViewModel);
                return RedirectToAction("ManageBankOrCash", "Vouchers", new { companyId = model.CompanyId, voucherId = voucherId });
            }


            return RedirectToAction(nameof(Create), new { companyId = model.CompanyId, moneyReceiptId = moneyReceiptViewModel.MoneyReceiptId });
        }

        [HttpPost]
        public async Task<ActionResult> MoneyReceiptSubmitCustomerCare(MoneyReceiptViewModel model)
        {
            MoneyReceiptViewModel moneyReceiptViewModel = new MoneyReceiptViewModel();
            if (model.IsExisting)
            {
                moneyReceiptViewModel = await _moneyReceiptService.MonyeReceiptDetailsForAccountingPush(model);
                var res = await _moneyReceiptService.MoneyReceiptStatusUpdate(moneyReceiptViewModel);
            }

            return RedirectToAction(nameof(Create), new { companyId = model.CompanyId, moneyReceiptId = model.MoneyReceiptId });
        }

        [HttpPost]
        public async Task<ActionResult> MoneyReceiptListfilter(MoneyReceiptViewModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }

            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(MoneyReceiptList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate, ProjectId = model.ProjectId, CGId = model.CGId });
        }


        [HttpGet]

        public async Task<ActionResult> MoneyReceiptList(int companyId, DateTime? fromDate, DateTime? toDate, int ProjectId = 0, long CGId = 0)
        {

            if (!fromDate.HasValue) fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!toDate.HasValue) toDate = fromDate.Value.AddMonths(1).AddSeconds(-1);             
            MoneyReceiptViewModel model = await _moneyReceiptService.MonyeReceiptList(companyId, fromDate, toDate, ProjectId, CGId);
            var company = _companyService.GetCompany(companyId);
            model.ProjectList = await _moneyReceiptService.ProjectList(companyId);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            model.ProjectId = ProjectId;
            model.CGId = CGId;

            if (ProjectId > 0)
            {
                model.GroupList = new SelectList(_moneyReceiptService.RSFileList(ProjectId), "Value", "Text");

            } 

            model.CompanyName = company.Name;
            model.CompanyId = companyId;
            return View(model);
        }



        [HttpGet]
        [ParentAuthorizedAttribute]
        public async Task<ActionResult> MoneyReceiptIntegratedList(int companyId)
        {
            MoneyReceiptViewModel model = await _moneyReceiptService.MoneyReceiptIntegratedList(companyId);
            var company = _companyService.GetCompany(companyId);
            model.CompanyName = company.Name;
            model.CompanyId = companyId;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteMoneyReceipt(MoneyReceiptViewModel model)
        {
            var res = await _moneyReceiptService.MoneyReceiptItemDelete(model);
            return RedirectToAction("MoneyReceiptList", new { companyId = model.CompanyId });
        }

        [HttpGet]
        public async Task<ActionResult> MoneyReceiptDetails(int companyId, long moneyReceiptId)
        {
            MoneyReceiptViewModel model = await _moneyReceiptService.MonyeReceiptDetails(moneyReceiptId);
            var company = _companyService.GetCompany(companyId);
            model.CompanyName = company.Name;
            model.CompanyId = companyId;
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> MoneyReceiptget(long moneyReceiptId)
        {
            MoneyReceiptViewModel model = await _moneyReceiptService.MonyeReceiptDetails(moneyReceiptId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> MoneyReceiptDetailsIntegration(int companyId, long moneyReceiptId)
        {
            MoneyReceiptViewModel model = new MoneyReceiptViewModel();
            var company = _companyService.GetCompany(companyId);
            model = await _moneyReceiptService.MonyeReceiptDetails(moneyReceiptId);
            model.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");
            model.CompanyName = company.Name;
            model.CompanyId = companyId;
            model.StringSubmidtate = DateTime.Now.ToString("dd-MMM-yyyy");
            return View(model);
        }

        [NonAction]
        public void SendVerificationLinkEmail()
        {
            string emailID = "mamun.spiders@gmail.com";
            var fromEmail = new MailAddress("jishan.ahamed@krishibidgroup.com", "Glorious Lands & Developments Limited");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "jishan@admin007"; // Replace with actual password
            string subject = "Your bill is successfully paid!";
            var toBcc = new MailAddress("rafiqulislam@krishibidgroup.com");
            // var ccEmail = new MailAddress("hod.it@krishibidgroup.com");

            string ccEmail = "dil.afroza@krishibidgroup.com,rafiqulislam@krishibidgroup.com";
            string body = "<br/><br/>hello";

            var smtp = new SmtpClient
            {
                Host = "mail.krishibidgroup.com",
                Port = 25,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            MailMessage msg = new MailMessage(fromEmail, toEmail);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;

            foreach (string sCC in ccEmail.Split(",".ToCharArray()))
            {
                msg.CC.Add(sCC);
            };


            smtp.Send(msg);
        }





        [HttpGet]

        public ActionResult RealStateDailyMoneyReceiptsCollectionReport(int companyId, string reportName, string noteReportName)
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

        [HttpPost]

        public ActionResult GetRealStateDailyMoneyReceiptsCollectionReport(ReportCustomModel model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = "";
            model.ReportName = "RealStateDailyMoneyReceiptsCollectionReport";
            reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&StrFromDate={3}", model.ReportName, model.ReportType, model.CompanyId, model.StrFromDate);
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






    }
}