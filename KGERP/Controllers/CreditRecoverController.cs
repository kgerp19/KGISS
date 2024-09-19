using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class CreditRecoverController : BaseController
    {
        private readonly ICreditRecoverService creditRecoverService;
        private readonly ICompanyService companyService;
        private readonly IVendorService vendorService;
        public CreditRecoverController(ICreditRecoverService creditRecoverService, ICompanyService companyService, IVendorService vendorService)
        {
            this.creditRecoverService = creditRecoverService;
            this.companyService = companyService;
            this.vendorService = vendorService;
        }

        
        public ActionResult CreditRecoverIndex(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            return View();
        }

        
        [HttpPost]
        public ActionResult CreditRecovers(int companyId)
        {
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            IQueryable<CreditRecoverModel> creditRecoverList = creditRecoverService.GetCreditRecovers(companyId, searchValue, out int count);
            int totalRows = count;
            int totalRowsAfterFiltering = creditRecoverList.Count();

            //sorting
            creditRecoverList = creditRecoverList.OrderBy(sortColumnName + " " + sortDirection);

            //paging
            creditRecoverList = creditRecoverList.Skip(start).Take(length);
            return Json(new { data = creditRecoverList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);
        }

        #region // Company wise CR
        
        public ActionResult CompanyCreditRecoverIndex(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            return View();
        }

        
        [HttpPost]
        public ActionResult CompanyCreditRecover(int companyId)
        {
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            IQueryable<CreditRecoverModel> creditRecoverList = creditRecoverService.GetCompanyCreditRecovers(companyId, searchValue, out int count);
            int totalRows = count;
            int totalRowsAfterFiltering = creditRecoverList.Count();

            //sorting
            creditRecoverList = creditRecoverList.OrderBy(sortColumnName + " " + sortDirection);

            //paging
            creditRecoverList = creditRecoverList.Skip(start).Take(length);

            return Json(new { data = creditRecoverList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);
        }


        
        [HttpGet]
        public ActionResult CompanyCreateOrEdit(long id = 0)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            CreditRecoverViewModel vm = new CreditRecoverViewModel();
            vm.CreditRecover = creditRecoverService.GetCreditRecover(id);
            vm.Companies = companyService.GetCompanySelectModels();
            if (vm.CreditRecover.CreditRecoverId == 0)
            {
                vm.Customers = new List<SelectModel>();
            }
            else
            {
                const int customer = 2;
                vm.Customers = vendorService.GetCustomerNameSelectModel(vm.CreditRecover.CompanyId, customer);
            }
            return View(vm);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CompanyCreateOrEdit(CreditRecoverViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            if (vm.CreditRecover.CreditRecoverId <= 0)
            {
                result = creditRecoverService.SaveCreditRecover(0, vm.CreditRecover, out message);
                if (result)
                {
                    message = "Data Saved Successfully";
                }

            }
            else
            {
                result = creditRecoverService.SaveCreditRecover(vm.CreditRecover.CreditRecoverId, vm.CreditRecover, out message);
                if (result)
                {
                    message = "Data Updated Successfully";
                }
            }

            return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);

        }

        #endregion


        
        [HttpGet]
        public ActionResult CreateOrEdit(long id = 0)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            CreditRecoverViewModel vm = new CreditRecoverViewModel();
            vm.CreditRecover = creditRecoverService.GetCreditRecover(id);
            vm.Companies = companyService.GetCompanySelectModels();
            if (vm.CreditRecover.CreditRecoverId == 0)
            {
                vm.Customers = new List<SelectModel>();
            }
            else
            {
                const int customer = 2;
                vm.Customers = vendorService.GetCustomerNameSelectModel(vm.CreditRecover.CompanyId, customer);
            }
            return View(vm);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(CreditRecoverViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            if (vm.CreditRecover.CreditRecoverId <= 0)
            {
                result = creditRecoverService.SaveCreditRecover(0, vm.CreditRecover, out message);
                if (result)
                {
                    message = "Data Saved Successfully";
                }

            }
            else
            {
                result = creditRecoverService.SaveCreditRecover(vm.CreditRecover.CreditRecoverId, vm.CreditRecover, out message);
                if (result)
                {
                    message = "Data Updated Successfully";
                }
            }

            return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);

        }

        
        public ActionResult CreditRecoverDetailIndex(int creditRecoverId)
        {
            CreditRecoverModel model = creditRecoverService.GetSingleCreditRecover(creditRecoverId);
            return View(model);
        }

        
        [HttpPost]
        public ActionResult CreditRecoverDetails(long creditRecoverId)
        {
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];



            IQueryable<CreditRecoverDetailModel> creditRecoverDetailList = creditRecoverService.GetCreditRecoverDetails(creditRecoverId, searchValue, out int count);
            int totalRows = count;
            int totalRowsAfterFiltering = creditRecoverDetailList.Count();


            //sorting
            creditRecoverDetailList = creditRecoverDetailList.OrderBy(sortColumnName + " " + sortDirection);

            //paging
            creditRecoverDetailList = creditRecoverDetailList.Skip(start).Take(length);


            return Json(new { data = creditRecoverDetailList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult DetailCreateOrEdit(long id, long creditRecoverId)
        {
            CreditRecoverDetailModel model = creditRecoverService.GetCreditRecoverDetail(id);
            model.CreditRecoverId = creditRecoverId;
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult DetailCreateOrEdit(CreditRecoverDetailModel model)
        {
            string message = string.Empty;
            bool result = false;
            if (model.CreditRecoverDetailId <= 0)
            {
                result = creditRecoverService.SaveCreditRecoverDetail(0, model, out message);
                if (result)
                {
                    message = "Data Saved Successfully";
                }

            }
            else
            {
                result = creditRecoverService.SaveCreditRecoverDetail(model.CreditRecoverId, model, out message);
                if (result)
                {
                    message = "Data Updated Successfully";
                }
            }

            return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult DetailDelete(long id)
        {
            bool result = creditRecoverService.DetailDelete(id);
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }

        
        public ActionResult DashboardIndex(int companyId)
        {
            CreditRecoverViewModel vm = new CreditRecoverViewModel();
            vm.MonthlyTargets = creditRecoverService.GetMonthlyTargetReport();
            return View(vm);
        }

        
        public ActionResult CreditRecoverDetailReport(int monthNo = 1, int yearNo = 1980)
        {
            CreditRecoverViewModel vm = new CreditRecoverViewModel();
            vm.MonthlyTargetDetails = creditRecoverService.GetMonthlyTargetDetailReport(monthNo, yearNo);
            return View(vm);
        }

        #region // Company wise
        
        public ActionResult CompanyDashboardIndex(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            CreditRecoverViewModel vm = new CreditRecoverViewModel();
            vm.MonthlyTargets = creditRecoverService.GetCompanyMonthlyTargetReport(companyId);
            return View(vm);
        }

        
        public ActionResult CompanyCreditRecoverDetailReport(int monthNo = 1, int yearNo = 1980)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            CreditRecoverViewModel vm = new CreditRecoverViewModel();
            vm.MonthlyTargetDetails = creditRecoverService.GetCompanyMonthlyTargetDetailReport(monthNo, yearNo, companyId);
            return View(vm);
        }

        #endregion
    }
}