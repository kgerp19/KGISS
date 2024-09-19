using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class HeadGLsController : BaseController
    {
        private readonly IHeadGLService accountHeadService;
        private readonly IShareHolderService shareHolderService;
        private readonly AccountingService _accountingService;
        public HeadGLsController(IHeadGLService accountHeadService, IShareHolderService shareHolderService, ERPEntities db)
        {
            this.accountHeadService = accountHeadService;
            this.shareHolderService = shareHolderService;
            _accountingService = new AccountingService(db);
        }



        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var accountHeads = accountHeadService.GetAccountHeadAutoComplete(prefix, companyId);
            return Json(accountHeads);
        }

        [HttpPost]
        public JsonResult MemberHeadAutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var accountHeads = accountHeadService.GetMemberHeadAutoComplete(prefix, companyId);
            return Json(accountHeads);
        }


        [HttpPost]
        public JsonResult PFHeadAutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var accountHeads = accountHeadService.GetPFHeadAutoComplete(prefix, companyId);
            return Json(accountHeads);
        }
        [HttpPost]
        public JsonResult AutoCompleteHeadGLGet(string prefix, int companyId)
        {

            var accountHeads = accountHeadService.GetAccountHeadAutoComplete(prefix, companyId);
            return Json(accountHeads);
        }

        [HttpPost]
        public JsonResult AutoCompleteAllAccountsHead(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var accountHeads = accountHeadService.AllAccountsHead(prefix, companyId);
            return Json(accountHeads);
        }
        public ActionResult TreeView(int companyId)
        {
            HeadGLViewModel vm = new HeadGLViewModel();
            var company = _accountingService.GetCompanyById(companyId);
            vm.CompanyName = company.Name + " (" + company.ShortName + ")";
            vm.Head1s = accountHeadService.GetAccountHeadsTreeViewByCompany(companyId);
            return View(vm);
        }
        [HttpGet]
        
        public PartialViewResult GetSelectionWindow(int accountHeadId, int layerNo, string accCode, string accName)
        {
            AccountHeadProcessModel accountHeadProcessModel = accountHeadService.GetSelectedItem(accountHeadId, layerNo, accCode, accName);
            return PartialView("~/Views/HeadGLs/_selectionWindow.cshtml", accountHeadProcessModel);
        }

        [HttpGet]
        
        public PartialViewResult GetChildHead(int accountHeadId, int layerNo, string accCode, string accName, string status)
        {
            AccountHeadProcessModel accountHeadProcessModel = new AccountHeadProcessModel();
            if (status.Equals("create"))
            {
                accountHeadProcessModel = accountHeadService.GetAccountHeadProcessCreate(accountHeadId, layerNo, status);
            }

            if (status.Equals("update"))
            {
                accountHeadProcessModel = accountHeadService.GetAccountHeadProcessUpdate(accountHeadId, layerNo, status);
            }
            if (status.Equals("delete"))
            {
                accountHeadProcessModel = accountHeadService.GetAccountHeadProcessDelete(accountHeadId, layerNo, status);
            }
            ModelState.Clear();
            return PartialView("~/Views/HeadGLs/_childHeadCreateOrEdit.cshtml", accountHeadProcessModel);

        }

        [HttpPost]
        
        public ActionResult Create(AccountHeadProcessModel model)
        {
            bool result = accountHeadService.SaveAccountHead(model);
            if (result)
            {
                TempData["successMessage"] = "Operation Successful!";
            }
            return RedirectToAction("TreeView", new { model.CompanyId });
        }


        [HttpPost]
        public ActionResult GetTeritorySelectModelsByZone(int zoneId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            List<SelectModel> teritories = accountHeadService.GetTeritorySelectModelsByZone(companyId, zoneId);
            return Json(teritories, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        
        public async Task<ActionResult> AccountingHeadList(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            VMAccountHead vmAccountHead = new VMAccountHead();
            vmAccountHead = await accountHeadService.GetAccountingHeadList(companyId);
            return View(vmAccountHead);
        }

    }
}
