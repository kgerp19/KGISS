using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class MonthlyTargetController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IMonthlyTargetService monthlyTargetService;
        private readonly IVendorService vendorService;

        public MonthlyTargetController(ICompanyService companyService, IMonthlyTargetService monthlyTargetService, IVendorService vendorService)
        {
            this.companyService = companyService;
            this.monthlyTargetService = monthlyTargetService;
            this.vendorService = vendorService;
        }

        public ActionResult Index(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<MonthlyTargetModel> monthlyTargets = monthlyTargetService.GetMonthlyTargets(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(monthlyTargets.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult CreateOrEdit(int id)
        {
            MonthlyTargetViewModel vm = new MonthlyTargetViewModel();
            vm.MonthlyTarget = monthlyTargetService.GetMonthlyTarget(id);
            vm.Companies = companyService.GetCompanySelectModels();
            vm.Years = Helper.GetYears();
            vm.Months = Helper.GetMonths();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(MonthlyTargetViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            if (vm.MonthlyTarget.MonthlyTargetId <= 0)
            {
                message = "Saved Successfully";
                result = monthlyTargetService.SaveMonthlyTarget(0, vm.MonthlyTarget, out message);

            }
            else
            {
                message = "Updated Successfully";
                result = monthlyTargetService.SaveMonthlyTarget(vm.MonthlyTarget.MonthlyTargetId, vm.MonthlyTarget, out message);
            }

            if (result)
            {
                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = message;
                return RedirectToAction("Index");
            }
        }

        
        [HttpGet]
        public ActionResult DeleteMonthlyTarget(int monthlyTargetId)
        {
            bool result = monthlyTargetService.DeleteMonthlyTarget(monthlyTargetId);
            if (result)
            {
                TempData["message"] = "Monthly Target Date Deleted Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        #region // Company wise Terget

        public ActionResult CompanyIndex(int? Page_No, string searchText, int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            searchText = searchText ?? "";
            List<MonthlyTargetModel> monthlyTargets = monthlyTargetService.GetMonthlyCompanyTargets(searchText, companyId);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(monthlyTargets.Where(x => x.CompanyId == companyId).ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult CompanyCreateOrEdit(int id, int companyId)
        {
            int VendorTypeId = 2;
            MonthlyTargetViewModel vm = new MonthlyTargetViewModel();
            vm.MonthlyTarget = monthlyTargetService.GetMonthlyTarget(id);
            vm.Companies = companyService.GetCompanySelectModels();
            vm.Customers = vendorService.GetCustomerSelectModelsByCompany(companyId, VendorTypeId);
            vm.Years = Helper.GetYears();
            vm.Months = Helper.GetMonths();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyCreateOrEdit(MonthlyTargetViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            if (vm.MonthlyTarget.MonthlyTargetId <= 0)
            {
                message = "Saved Successfully";
                result = monthlyTargetService.SaveMonthlyTarget(0, vm.MonthlyTarget, out message);
            }
            else
            {
                message = "Updated Successfully";
                result = monthlyTargetService.SaveMonthlyTarget(vm.MonthlyTarget.MonthlyTargetId, vm.MonthlyTarget, out message);
            }

            if (result)
            {
                TempData["message"] = message;
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                return RedirectToAction("CompanyIndex", new { companyId = companyid });
            }
            else
            {
                TempData["message"] = message;
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                return RedirectToAction("CompanyIndex", new { companyId = companyid });
            }
        }

        
        [HttpGet]
        public ActionResult DeleteCompanyMonthlyTarget(int monthlyTargetId)
        {
            bool result = monthlyTargetService.DeleteMonthlyTarget(monthlyTargetId);
            if (result)
            {
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                TempData["message"] = "Monthly Target Date Deleted Successfully";
                return RedirectToAction("CompanyIndex", new { companyId = companyid });

            }
            return View();
        }

        #endregion
    }
}
