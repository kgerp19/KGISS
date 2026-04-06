using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]

    public class RequisitionSignatoryController : Controller
    {
        private readonly IRequisitionSignatoryService _service;
        private readonly ERPEntities _context;

        public RequisitionSignatoryController()
        {
            _context = new ERPEntities();
            _service = new RequisitionSignatoryService(_context);
        }

        // GET: RequisitionSignatory
        [HttpGet]
        public async Task<ActionResult> Index(int CompanyId,string integrateWith = null)
        {
            var model = await _service.GetAllSignatories(CompanyId, integrateWith);
            ViewBag.IntegrateWith = integrateWith;
            ViewBag.CompanyId = CompanyId;
            if (ViewBag.CompanyId<=0)
            {
                model = new RequisitionSignatoryViewModel();
            }
            return View(model);
        }

        // GET: Get Signatory Data for DataTable
        [HttpPost]
        public async Task<JsonResult> GetSignatoryData()
        {
            try
            {
                // DataTable parameters
                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];
                string draw = Request["draw"];

                // Filter parameters
                long? employeeId = string.IsNullOrEmpty(Request["employeeId"]) ? (long?)null : Convert.ToInt64(Request["employeeId"]);
                int? companyId = string.IsNullOrEmpty(Request["companyId"]) ? (int?)null : Convert.ToInt32(Request["companyId"]);
                int? departmentId = string.IsNullOrEmpty(Request["departmentId"]) ? (int?)null : Convert.ToInt32(Request["departmentId"]);
                int? designationId = string.IsNullOrEmpty(Request["designationId"]) ? (int?)null : Convert.ToInt32(Request["designationId"]);
                string integrateWith = Request["integrateWith"];
                if (String.IsNullOrEmpty(integrateWith) || (companyId is null && employeeId is null))
                {
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
                var serviceResult = await _service.GetSignatoriesForDataTable(
                    start, length, searchValue, sortColumnName, sortDirection,
                    employeeId, companyId, null, departmentId, designationId, integrateWith
                );

                // Cast to dynamic to access properties
                // Use reflection to get properties safely
                var resultType = serviceResult.GetType();

                // Try different property name variations
                var recordsTotalProp = resultType.GetProperty("recordsTotal") ?? resultType.GetProperty("RecordsTotal");
                var recordsFilteredProp = resultType.GetProperty("recordsFiltered") ?? resultType.GetProperty("RecordsFiltered");
                var dataProp = resultType.GetProperty("data") ?? resultType.GetProperty("Data");

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotalProp?.GetValue(serviceResult) ?? 0,
                    recordsFiltered = recordsFilteredProp?.GetValue(serviceResult) ?? 0,
                    data = dataProp?.GetValue(serviceResult) ?? new List<object>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Create or Edit
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(int companyid, long id = 0, string integrateWith = null)
        {
            RequisitionSignatoryViewModel model;

            if (id > 0)
            {
                model = await _service.GetSignatoryById(id, companyid);
                if (model == null)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                model = new RequisitionSignatoryViewModel
                {
                    IntegrateWith = integrateWith,
                    IsActive = true
                };
                model.ModelNames = _service.GetModelNames(companyid);
                model.Companies = _service.GetCompaniesByUser(companyid);
                model.Categories = _service.GetCategories();
            }

            return PartialView("_CreateOrEditModal", model);
        }

        // POST: Create or Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateOrEdit(RequisitionSignatoryViewModel model, string AssignmentMode,
            int? BulkCompanyId, int? BulkDepartmentId, int? BulkDesignationId,
            long? BulkSignatoryEmpId, int? BulkOrderBy)
        {
            try
            {
                // Check if this is bulk assignment mode
                if (AssignmentMode == "bulk")
                {
                    return await HandleBulkAssignment(model, BulkCompanyId, BulkDepartmentId, BulkDesignationId, BulkSignatoryEmpId, BulkOrderBy);
                }

                // Individual assignment validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                // Validate OrderBy is between 1-4
                if (model.OrderBy < 1 || model.OrderBy > 4)
                {
                    return Json(new { success = false, message = "Level must be between 1 and 4" });
                }

                // For new records, check if employee already has 4 signatories
                if (model.RequisitionSignatoryId == 0)
                {
                    var count = await _service.GetSignatoryCount(model.EmployeeId, model.IntegrateWith);
                    if (count >= 4)
                    {
                        return Json(new { success = false, message = "Employee already has 4 signatories. Cannot add more." });
                    }
                }

                var result = await _service.SaveSignatory(model);

                if (result)
                {
                    return Json(new { success = true, message = "Signatory saved successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save signatory" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper method for bulk assignment
        private async Task<JsonResult> HandleBulkAssignment(RequisitionSignatoryViewModel model,
            int? bulkCompanyId, int? bulkDepartmentId, int? bulkDesignationId,
            long? bulkSignatoryEmpId, int? bulkOrderBy)
        {
            // Validate bulk parameters
            if (!bulkCompanyId.HasValue)
            {
                return Json(new { success = false, message = "Company is required for bulk assignment" });
            }

            if (!bulkSignatoryEmpId.HasValue)
            {
                return Json(new { success = false, message = "Signatory is required" });
            }

            if (!bulkOrderBy.HasValue || bulkOrderBy < 1 || bulkOrderBy > 4)
            {
                return Json(new { success = false, message = "Level must be between 1 and 4" });
            }

            // Get employees based on filters
            var employees = _service.GetEmployeesByFilters(bulkCompanyId, null, bulkDepartmentId, bulkDesignationId);

            if (employees == null || !employees.Any())
            {
                return Json(new { success = false, message = "No employees found matching the selected filters" });
            }

            int successCount = 0;
            int skipCount = 0;
            var errors = new List<string>();
            string getSignatoryDesignation = String.Empty;

            if (bulkSignatoryEmpId is null || bulkSignatoryEmpId <= 0)
            {
                var chkmessage = $"Bulk assignment completed: {successCount} assigned, {skipCount} skipped (already have 4 signatories)";
                return Json(new { success = true, message = chkmessage });
            }
            else
            {
                getSignatoryDesignation = await _service.getSignatoryDesignation(bulkSignatoryEmpId.Value);
            }

            // Assign signatory to each employee
            foreach (var employee in employees)
            {
                try
                {
                    long employeeId = Convert.ToInt64(employee.Value);

                    // Check if employee already has 4 signatories
                    var count = await _service.GetSignatoryCount(employeeId, model.IntegrateWith);
                    if (count >= 4)
                    {
                        skipCount++;
                        continue;
                    }

                    // Create signatory record
                    var signatoryModel = new RequisitionSignatoryViewModel
                    {
                        EmployeeId = employeeId,
                        SignatoryEmpId = bulkSignatoryEmpId.Value,
                        OrderBy = bulkOrderBy.Value,
                        CompanyId = model.CompanyId,
                        DesignationNameSig = getSignatoryDesignation,
                        CategoryId = model.CategoryId,
                        Limitation = model.Limitation,
                        IntegrateWith = model.IntegrateWith,
                        IsHRAdmin = model.IsHRAdmin,
                        IsSupremeApproved = model.IsSupremeApproved,
                        IsActive = true
                    };

                    var result = await _service.SaveSignatory(signatoryModel);
                    if (result)
                    {
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error for employee {employee.Text}: {ex.Message}");
                }
            }

            var message = $"Bulk assignment completed: {successCount} assigned, {skipCount} skipped (already have 4 signatories)";
            if (errors.Any())
            {
                message += $". Errors: {string.Join("; ", errors.Take(3))}";
            }

            return Json(new { success = true, message = message });
        }

        // POST: Delete
        [HttpPost]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                var result = await _service.DeleteSignatory(id);

                if (result)
                {
                    return Json(new { success = true, message = "Signatory deleted successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete signatory" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Replace Approver Modal
        [HttpGet]
        public ActionResult ReplaceApprover(string integrateWith = null, int? companyId = null)
        {
            var model = new ReplaceApproverViewModel
            {
                IntegrateWith = integrateWith,
                CompanyId = companyId
            };
            return PartialView("_ReplaceApproverModal", model);
        }

        // POST: Replace Approver
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ReplaceApprover(ReplaceApproverViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                if (model.OldSignatoryEmpId == model.NewSignatoryEmpId)
                {
                    return Json(new { success = false, message = "Old and new signatory cannot be the same" });
                }

                var result = await _service.ReplaceApprover(model);

                if (result)
                {
                    return Json(new { success = true, message = "Approver replaced successfully for all employees" });
                }
                else
                {
                    return Json(new { success = false, message = "No records found to replace" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Cascading Dropdowns
        //[HttpPost]
        //public JsonResult GetDivisions(int companyId)
        //{
        //    var divisions = _service.GetDivisionsByCompany(companyId);
        //    return Json(divisions, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult GetDepartments(int companyId)
        {
            var departments = _service.GetDepartmentsByCompany(companyId);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSections(int departmentId)
        {
            var sections = _service.GetSectionsByDepartment(departmentId);
            return Json(sections, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDesignations(int departmentId)
        {
            var designations = _service.GetDesignationsByDepartment(departmentId);
            return Json(designations, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployees(int? companyId = null, int? divisionId = null, int? departmentId = null, int? designationId = null)
        {
            var employees = _service.GetEmployeesByFilters(companyId, divisionId, departmentId, designationId);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployeesByFilters(int? companyId = null, int? divisionId = null, int? departmentId = null, int? designationId = null)
        {
            var employees = _service.GetEmployeesByFilters(companyId, divisionId, departmentId, designationId);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        // GET: Employee Details for Replace Approver
        [HttpPost]
        public async Task<JsonResult> GetEmployeeDetails(long employeeId)
        {
            try
            {
                var details = await _service.GetEmployeeDetailsForReplace(employeeId);

                if (details == null)
                {
                    return Json(new { success = false, message = "Employee not found" });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        name = details.NewSignatoryName,
                        code = details.NewSignatoryCode,
                        company = details.NewCompanyName,
                        division = details.NewDivisionName,
                        department = details.NewDepartmentName,
                        section = details.NewSectionName,
                        designation = details.NewDesignationName,
                        appointmentDate = details.NewAppointmentDate?.ToString("dd-MMM-yyyy"),
                        status = details.NewEmployeeStatus
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Employee Autocomplete
        [HttpPost]
        public JsonResult EmployeeAutoComplete(string prefix, int? companyId = null)
        {
            try
            {
                var employees = _context.Employees
                    .Where(x => x.Active && (x.Name.Contains(prefix) || x.EmployeeId.Contains(prefix)))
                    .Where(x => !companyId.HasValue || x.CompanyId == companyId.Value)
                    .OrderBy(x => x.Name)
                    .Take(20)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Name = x.EmployeeId + " - " + x.Name,
                        EmployeeId = x.EmployeeId,
                        DesignationName = x.Designation != null ? x.Designation.Name : ""
                    })
                    .ToList();

                return Json(employees, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}