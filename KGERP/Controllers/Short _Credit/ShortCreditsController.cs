using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Short__CreditService;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.Short__Credit
{
    [CheckSession]
    public class ShortCreditsController : Controller
    {
        private readonly Ishort__CreditService __CreditService;
        private readonly IVendorService vendorService;
        private readonly AccountingService _accountingService;

        public ShortCreditsController(Ishort__CreditService __CreditService, IVendorService vendorService, AccountingService accountingService)
        {
            this.__CreditService = __CreditService;
            this.vendorService = vendorService;
            _accountingService = accountingService;

        }

        [HttpPost]
        public JsonResult AutoComplete(object prefix)
        {
            if (prefix is string prefixString)
            {
                var customers = vendorService.GetCustomerAutoCompleteForShortcredit(prefixString, 8);
                return Json(customers);
            }
            //else if (prefix is int prefixInt)
            //{
            //    var customers = vendorService.GetCustomerAutoCompleteForShortcreditInt(prefixInt.ToString(), 8);
            //    return Json(customers);
            //}

            // Handle other types or return an empty result if desired
            return Json(null);
        }

        public async Task<ActionResult> Index(int companyId, long sCreditCollectionId = 0)
        {
            ShortCreditViewModel model = new ShortCreditViewModel();
            if (sCreditCollectionId > 0)
            {
                model = await Task.Run(() => __CreditService.GetShortCreditCollectedAmount(companyId, sCreditCollectionId));

            }
            else
            {
                model.CompanyId = companyId;
                model.BankOrCashParantList = new SelectList(_accountingService.FEEDCashAndBankDropDownList(companyId), "Value", "Text");
            }




            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ShortCreditViewModel model)
        {
            var result = await __CreditService.AddPayment(model);
            TempData["message"] = result.Message;
            return RedirectToAction("Index", new { companyId = model.CompanyId, sCreditCollectionId = result.ShortCreditCollectionId });
        }

        [HttpPost]
        public async Task<ActionResult> SCreditSubmit(ShortCreditViewModel model)
        {
            var result = await __CreditService.SubmitCollectionMasters(model);
          
            return RedirectToAction("Index", new { companyId = model.CompanyId });
        }

        
        [HttpGet]
        public JsonResult ShortCreditList(int VendorId)
        {
            VMFeedPayment vendorModel = __CreditService.GetShortList(VendorId);
            return Json(vendorModel,JsonRequestBehavior.AllowGet);
        }





    }
}