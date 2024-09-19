using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Configuration;
using KGERP.Service.Implementation.IncentivesDistribution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.IncentiveDistribution
{
    [CheckSession]
    public class IncentiveDistributionConfigrationController : BaseController
    {
        private readonly IIncentiveConfig incentiveConfig;
        private readonly AccountingService _accountingService;
        public IncentiveDistributionConfigrationController(IIncentiveConfig incentiveConfig, AccountingService _accountingService)
        {
            this.incentiveConfig = incentiveConfig;
            this._accountingService = _accountingService;
        }
        // GET: IncentiveDistributionConfigration
        public ActionResult Index(int companyId, int id = 0)
        {
            IncentiveViewModel model = new IncentiveViewModel();
            if (id > 0)
            {
                var res = incentiveConfig.GetBy(id);
                model = incentiveConfig.ChartList(companyId);
                var company1 = _accountingService.GetCompanyById(companyId);
                model.CompanyName = company1.Name;
                model.CompanyId = companyId;
                model.Commission = res.Commission;
                model.IncentiveCatagoryId = res.IncentiveCatagoryId;
                model.Id = res.Id;
                model.Catagorieslist = incentiveConfig.Catagorieslist(companyId);
                return View(model);
            }

            model = incentiveConfig.ChartList(companyId);
            var company = _accountingService.GetCompanyById(companyId);
            model.CompanyName = company.Name;
            model.CompanyId = companyId;
            model.Catagorieslist = incentiveConfig.Catagorieslist(companyId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(IncentiveViewModel model)
        {
            if (model.Id > 0)
            {
                double sum = (double)incentiveConfig.GetSum(model.CompanyId, model.Id);
                double ressum = (double)(sum + model.Commission);
                var ext = incentiveConfig.Getexit(model);
                var res2 = incentiveConfig.GetBy(model.Id);
                if (ressum > 100)
                {
                    model = incentiveConfig.ChartList(res2.CompanyId);
                    var company1 = _accountingService.GetCompanyById(res2.CompanyId);
                    model.CompanyName = company1.Name;
                    model.CompanyId = res2.CompanyId;
                    model.Commission = res2.Commission;
                    model.IncentiveCatagoryId = res2.IncentiveCatagoryId;
                    model.Id = res2.Id;
                    model.Catagorieslist = incentiveConfig.Catagorieslist(model.CompanyId);
                    ModelState.AddModelError("Commission", "Commission more then 100%");
                    return View(model);
                }

                if (ext != null)
                {
                    model = incentiveConfig.ChartList(res2.CompanyId);
                    var company1 = _accountingService.GetCompanyById(res2.CompanyId);
                    model.CompanyName = company1.Name;
                    model.CompanyId = res2.CompanyId;
                    model.Commission = res2.Commission;
                    model.IncentiveCatagoryId = res2.IncentiveCatagoryId;
                    model.Id = res2.Id;
                    model.Catagorieslist = incentiveConfig.Catagorieslist(model.CompanyId);
                    ModelState.AddModelError("IncentiveCatagoryId", "Incentive Catagory Name is  Already Exists");
                    return View(model);
                }

                var res = incentiveConfig.UpdateChart(model);
                if (model.Id > 0)
                {
                    return RedirectToAction("Index", new { companyId = model.CompanyId });
                }
                return View(model);
            }
            else
            {
                double sum = (double)incentiveConfig.GetSum(model.CompanyId, model.Id);
                double ressum = (double)(sum + model.Commission);
                var ext = incentiveConfig.GetCatagory((int)model.IncentiveCatagoryId);
                if (ressum > 100)
                {
                    model = incentiveConfig.ChartList(model.CompanyId);
                    var company = _accountingService.GetCompanyById(model.CompanyId);
                    model.CompanyName = company.Name;
                    model.Catagorieslist = incentiveConfig.Catagorieslist(model.CompanyId);
                    ModelState.AddModelError("Commission", "Commission more then 100%");
                    return View(model);
                }

                if (ext != null)
                {
                    model = incentiveConfig.ChartList(model.CompanyId);
                    var company = _accountingService.GetCompanyById(model.CompanyId);
                    model.CompanyName = company.Name;
                    model.Catagorieslist = incentiveConfig.Catagorieslist(model.CompanyId);
                    ModelState.AddModelError("IncentiveCatagoryId", "Incentive Catagory Name is  Already Exists");
                    return View(model);
                }

                var res = incentiveConfig.AddChart(model);
                if (model.Id > 0)
                {
                    return RedirectToAction("Index", new { companyId = model.CompanyId });
                }
                return View(model);
            }
        }
        public ActionResult Delete(int id)
        {
            var res = incentiveConfig.Delete(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IncentiveDistributionDetails(int id)
        {
            IncentiveDistributionDetailVm vm = new IncentiveDistributionDetailVm();
            vm = incentiveConfig.GetList(id);
            return View(vm);
        }
        [HttpGet]
        public ActionResult AddNewIncentiveDetails(int id, int companyId)
        {
            IncentiveDistributionDetailVm vm = new IncentiveDistributionDetailVm();
            vm = incentiveConfig.GetListForAddNewIncentive(id, companyId);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public bool SaveIncentiive(IncentiveDistributionDetailVm model)
        {
            var res = incentiveConfig.objtosave(model);
            return res;
        }

        [HttpGet]
        public ActionResult EditIncentive(long id)
        {
            var res = incentiveConfig.GetorEditIncentive(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public bool EditSaveIncentive(IncentiveDistributionDetail model)
        {
            var res = incentiveConfig.UpdateIncentive(model);
            return res;
        }
        [HttpPost]
        public bool Deleteincentive(long Id)
        {
            var res = incentiveConfig.Deleteincentivebyid(Id);
            return res;

        }

        [HttpPost]
        public ActionResult AddIncentiveCatagry(IncentiveCatagory model)
        {
            var res=incentiveConfig.AddOrUpdateCategoryIncentive(model);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult IncentiveCatagryList(int CompanyId)
        {
           IncentiveCateGoryViewModel model = new IncentiveCateGoryViewModel();
            model=incentiveConfig.GetIncentiveCateGoryList(CompanyId);
            model.CompanyId = CompanyId;
            return View(model);
            
        }

        [HttpGet]
        public ActionResult IncentiveCatagryEditGet(int id)
        {
            IncentiveCateGoryViewModel model = new IncentiveCateGoryViewModel();
            model = incentiveConfig.GetEditItem(id);
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult DeleteIncentivecategory(int id)
        {
            var result = incentiveConfig.DeleteIncentivecategory(id);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
       




    }
    }