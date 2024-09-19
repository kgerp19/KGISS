using KGERP.Data.Models;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.ExtendedProperties;
using KGERP.Service.ServiceModel.RealState;

namespace KGERP.Controllers.KFMLInstallment
{
    [CheckSession]
    public class Kfml_InstallmentCollactionController : Controller
    {
        private ERPEntities db = new ERPEntities();
        private readonly MoneyReceiptService _moneyReceiptService;
        IKgReCrmService kgReCrmService = new KgReCrmService();
        private readonly ICompanyService _companyService;
        private readonly AccountingService _accountingService;
        public Kfml_InstallmentCollactionController(AccountingService accountingService, MoneyReceiptService moneyReceiptService, ICompanyService companyService)
        {
            _moneyReceiptService = moneyReceiptService;
            _companyService = companyService;
            _accountingService = accountingService;
        }
        // GET: Kfml_InstallmentCollaction
        public async Task<ActionResult> Create(int companyId, int moneyReceiptId = 0)
        {
            MoneyReceiptViewModel model = new MoneyReceiptViewModel();
            var company = _companyService.GetCompany(companyId);
            if (moneyReceiptId > 0)
            {
                model = await _moneyReceiptService.KfmalMonyeReceiptDetails(moneyReceiptId);
                model.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");
                model.CompanyName = company.Name;
                model.CompanyId = companyId;
            }
            else
            {
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
        [HttpPost]
        public async Task<ActionResult> Create(MoneyReceiptViewModel model)
        {
            var res = await _moneyReceiptService.AddReceiptkfmal(model);
            return RedirectToAction(nameof(Create), new { companyId = model.CompanyId, moneyReceiptId = res.MoneyReceiptId });
        }

        [HttpGet]
        public JsonResult Getfile(int companyId, long vendorId)
        {
            var crm = _moneyReceiptService.GetGroupByFile(companyId, vendorId);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetClient(long CGId)
        {
            var crm = _moneyReceiptService.GetKFmlFileInfo(CGId);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }


    }
}