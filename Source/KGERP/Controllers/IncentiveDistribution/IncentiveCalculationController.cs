using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.IncentivesDistribution;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.IncentiveModels;
using KGERP.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class IncentiveCalculationController : BaseController
    {
        private readonly IncentivesCalculationService calculationService;
        private readonly MoneyReceiptService _moneyReceiptService;
        private readonly AccountingService _accountingService;
        public IncentiveCalculationController(AccountingService _accountingService, IncentivesCalculationService calculationService, MoneyReceiptService moneyReceiptService)
        {
            this.calculationService = calculationService;
            this._accountingService = _accountingService;
            _moneyReceiptService = moneyReceiptService;
        }
        [HttpGet]
        public async Task<ActionResult> Index(int companyid)
        {
            IncentiveCalculationViewModel model = new IncentiveCalculationViewModel();
            model.CompanyId = companyid;
            model.BookingDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            model.ProjectList = await _moneyReceiptService.ProjectList(companyid);
            return View(model);
        }

        [HttpGet]
        public JsonResult Get(int companyid,long Cgid)
        {
            var res = calculationService.incentives(companyid,Cgid);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Index(IncentiveCalculationViewModel model)
        {
            model.IncentiveDate = Convert.ToDateTime(model.BookingDateString);
            var res = calculationService.IncentiveCalculation(model);
            return RedirectToAction("IncentiveGenerate", new { companyId = model.CompanyId });
        }
        [HttpGet]
        public async Task<ActionResult> IncentiveGenerate(int companyId)
        {
            IncentiveCalculationViewModel model = new IncentiveCalculationViewModel();
            model.MappVm = calculationService.datalist(companyId);
            var company = _accountingService.GetCompanyById(companyId);
            model.CompanyName = company.Name;
            model.CompanyId = companyId;
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> IncentiveGenerateDetalis(long id)
        {
            var res = calculationService.IncentiveProcessDetails(id);
            return View(res);
        }





        [HttpGet]
        public async Task<ActionResult> IndexNewIncentive(int companyid)
        {
            IncentiveCalculationViewModel model = new IncentiveCalculationViewModel();
            model.CompanyId = companyid;
            model.BookingDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            model.ProjectList = await _moneyReceiptService.ProjectList(companyid);
            return View(model);
        }

        [HttpPost]
        public ActionResult ReceiveArray(int companyId, string list)
        {
            try
            {
             
                long[] array = JsonConvert.DeserializeObject<long[]>(list);
                
              var result= calculationService.IsIncentive(companyId,array);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                // Handle any exceptions if necessary
                return View(new { Error = "An error occurred while processing the selectedRows." });
            }
        }


    }
}