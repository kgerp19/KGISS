using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.Implementation;
using KGERP.Service.ServiceModel;
using KGERP.Utility;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ApplicationManageController : Controller
    {
        private readonly IApplicationManageService _applicationManageService;
        private readonly ERPEntities db = new ERPEntities();

        // Normally injected, assuming default constructor for MVC 5 without DI container configured specifically
        public ApplicationManageController()
        {
            _applicationManageService = new ApplicationManageService(db);
        }

        private long GetEmployeeId()
        {
            try
            {
                return Convert.ToInt64(Session["Id"]);
            }
            catch
            {
                return 0; // Or handle appropriately
            }
        }

        private string GetUsername()
        {
            try
            {
                return User.Identity.Name;
            }
            catch
            {
                return "Unknown";
            }
        }

        private void PopulateDropdowns(ApplicationManageModel model)
        {
            var employees = db.Employees.Where(e => e.Active && e.CompanyId == model.CompanyId).Select(e => new { e.Id, Name = e.EmployeeId + " - " + e.Name }).ToList();
            model.Managers = new SelectList(employees, "Id", "Name", model.ManagerId);

            var vendors = db.Vendors.Where(v => v.IsActive && v.CompanyId == model.CompanyId).Select(v => new { v.VendorId, Name = v.Code + " - " + v.Name }).ToList();
            model.Applicants = new SelectList(vendors, "VendorId", "Name", model.ApplicantId);
        }

        [HttpGet]
        public async Task<ActionResult> OrderCreditLimitEntry(int companyId, long applicationId = 0)
        {
            var model = await _applicationManageService.GetOrderCreditLimitApplications(companyId, applicationId);
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> OrderCreditLimitEntry(ApplicationManageModel model)
        {
            long employeeId = GetEmployeeId();
            if (ModelState.IsValid)
            {
                bool hasSignatory = db.RequisitionSignatories.Any(x => x.IntegrateWith == "OrderCreditLimit" && x.IsActive);
                if (!hasSignatory)
                {
                    TempData["ErrorMessage"] = "Signaotry can't found Please signatory set first please";
                    return RedirectToAction("OrderCreditLimitEntry", new { companyId = model.CompanyId });
                }
                else
                {
                    try
                    {
                        long newAppId = await _applicationManageService.SaveOrderCreditLimitApplication(model, GetUsername(), employeeId);
                        TempData["Message"] = "Application Submitted Successfully";
                        return RedirectToAction("OrderCreditLimitEntry", new { companyId = model.CompanyId, applicationId = newAppId });
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Failed to submit application. " + ex.Message;
                    }
                }
            }

            // Repopulate if failed
            var listModel = await _applicationManageService.GetOrderCreditLimitApplications(model.CompanyId, 0);
            model.ApplicationList = listModel.ApplicationList;
            PopulateDropdowns(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> OrderCreditLimitApprovalList(int companyId)
        {
            long employeeId = GetEmployeeId();
            var model = await _applicationManageService.GetPendingApprovals(companyId, employeeId);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> OrderCreditLimitManageList(int companyId, int? searchStatus, string strFromDate, string strToDate)
        {
            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (string.IsNullOrEmpty(strFromDate) && string.IsNullOrEmpty(strToDate))
            {
                fromDate = DateTime.Now.AddMonths(-1);
                toDate = DateTime.Now;
                strFromDate = fromDate.Value.ToString("yyyy-MM-dd");
                strToDate = toDate.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                if (!string.IsNullOrEmpty(strFromDate))
                {
                    fromDate = Convert.ToDateTime(strFromDate);
                }
                if (!string.IsNullOrEmpty(strToDate))
                {
                    toDate = Convert.ToDateTime(strToDate);
                }
            }

            var model = await _applicationManageService.GetOrderCreditLimitManageList(companyId, searchStatus, fromDate, toDate);
            model.StrFromDate = strFromDate;
            model.StrToDate = strToDate;

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateApprovalStatus(long approvalId, int status, string comment)
        {
            try
            {
                bool result = await _applicationManageService.UpdateApprovalStatus(approvalId, status, comment, GetUsername());
                return Json(new { success = result, message = result ? "Status updated successfully" : "Failed to update status." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
