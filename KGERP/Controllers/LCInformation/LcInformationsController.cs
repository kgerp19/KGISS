using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using KGERP.Data.Models;
using KGERP.Service.Implementation.LcInfoServices;
using KGERP.Service.Implementation.LcInfoServices.LcCommonService;
using KGERP.Service.Implementation.LcInfoServices.LCInformation;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.LCInformation
{
    [CheckSession]
    public class LcInformationsController : Controller
    {
        private readonly LcCommmonInterface lcCommmon;
        private readonly ILCInformationInterface lCInformation;
        public LcInformationsController(LcCommmonInterface _lcCommmon, ILCInformationInterface _lCInformation)
        {
            lcCommmon = _lcCommmon;
            lCInformation = _lCInformation;
        }
        // GET: LcInformations
        public ActionResult Index(int companyId)
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateLc(int companyId)
        {
            LcInfoViewModel model = new LcInfoViewModel();
            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                model.AdvanceAndLoan = lcCommmon.GetAdvanceAndLoan(companyId);

                
            }
            model.GetAllBanks = lcCommmon.GetAllBanks(companyId);           
            model.GetAllVendorList = lcCommmon.GetAllVendorList(companyId);
            model.GetAllContry = lcCommmon.GetAllContry();
            model.GetAllCurrencyList = lcCommmon.GetAllCurrencyList();
            model.LCDate = DateTime.Now;
            var company = lcCommmon.GetByCompany(companyId);
            model.CompanyNmae = company.Name;
            model.CompanyId = companyId;    
            return View(model);
        }

        [HttpGet]
        public JsonResult GetGroupListByProjectId(int countryid)
        {
            var crm = lcCommmon.GetByCountryIdPortOfCountry(countryid);
            return Json(crm, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreateLc(LcInfoViewModel model)
        { 
            var res = lCInformation.AddLC(model);
            return RedirectToAction(nameof(LcDetalis), new { lcid = res });
        }
        [HttpGet]
        public ActionResult LcDetalis(long lcid)
        {
            var res = lCInformation.LCDetails(lcid);
            return View(res);
            
        }
        [HttpGet]
        public ActionResult LcEdit(long lcid)
        {
            
            var model = lCInformation.LCDetails(lcid);
            if (model.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                model.AdvanceAndLoan = lcCommmon.GetAdvanceAndLoan(model.CompanyId);


            }
            model.GetAllBanks = lcCommmon.GetAllBanks(model.CompanyId);
            model.GetAllVendorList = lcCommmon.GetAllVendorList(model.CompanyId);
            model.GetAllContry = lcCommmon.GetAllContry();
            model.GetAllCurrencyList = lcCommmon.GetAllCurrencyList();
            model.LCDate = DateTime.Now;
            var company = lcCommmon.GetByCompany(model.CompanyId);
            model.CompanyNmae = company.Name;
            model.CompanyId = model.CompanyId;
            return View(model);
        } 
        [HttpPost]
        public ActionResult LcEdit(LcInfoViewModel model)
        {
            var res = lCInformation.UpdateLC(model);
            return RedirectToAction(nameof(LcDetalis), new { lcid = res });
        }
        [HttpPost]
        public ActionResult SubmitLC(LcInfoViewModel model)
        {
            var res = lCInformation.LCSubmit(model);
            return RedirectToAction(nameof(LcList), new { companyId = res.Result });
        }
        [HttpPost]
        public ActionResult SubmitAmendments(LcInfoViewModel model)
        {
            var res = lCInformation.LCAmendmentSubmit(model);
            return RedirectToAction(nameof(LcList), new { companyId = res.Result });
        }

        [HttpPost]
        public ActionResult SubmitAmendmentLCValue(LcInfoViewModel model)
        {
            var res = lCInformation.LCValueAmendmentSubmit(model);
            return RedirectToAction(nameof(LcList), new { companyId = res.Result });
        }

        [HttpPost]
        public ActionResult AmendmentSave(LcInfoViewModel model)
        {
            var res = lCInformation.LCAmendmentSave(model);
            return RedirectToAction(nameof(LcDetalis), new { lcid = res.Result });
        }


        [HttpPost]
        public ActionResult LcValueAmendmentSave(LcInfoViewModel model)
        {
            var res = lCInformation.LCValueAmendmentSave(model);
            return RedirectToAction(nameof(LcDetalis), new { lcid = res.Result });
        }
        [HttpPost]
        public ActionResult Delete(LcInfoViewModel model)
        {
            var res = lCInformation.LCDelete(model);
            return RedirectToAction(nameof(LcList), new { companyId = res });
        }
        [HttpGet]
        public ActionResult LcList(int companyId)
        {
            var model = lCInformation.LCList(companyId);
            var company = lcCommmon.GetByCompany(companyId);
            model.CompanyNmae = company.Name;
            model.CompanyId = companyId;
            return View(model);
        }

        [HttpGet]
        public JsonResult GetCurrencyrate(int Id)
        {
            var obj = lcCommmon.GetAllCurrencyRate(Id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet]
        public JsonResult GetAmmendmentLcValue(int amendmentId)
        {
            var obj = lCInformation.GetLcAmmendmentValue(amendmentId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteAmmendMendValue(int amendmentId)
        {
            var obj = lCInformation.DeleteAmmendMendValue(amendmentId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetLcAmendment(int amendmentId)
        {
            var obj = lCInformation.GetLCAmendment(amendmentId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DltAmmendment(int amendmentId)
        {
            var obj = lCInformation.DeleteLCAmendment(amendmentId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }



    }
}