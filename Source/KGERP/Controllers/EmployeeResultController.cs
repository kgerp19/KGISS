using KGERP.Data.Models;
using KGERP.Service.CommonModels.Model;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.EmployeeResults;
using KGERP.Service.Interface;
using KGERP.Utility;
using KGERP.Utility.Interface;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class EmployeeResultController : BaseController
    {
        private readonly IEmployeeResultService _employeeResultService;
        private readonly IDropDownItemService _dropDownItemService;
        private readonly IDropdownService _dropdownService;
        ICompanyService _companyService = new CompanyService(new ERPEntities());
        IDepartmentService _departmentService = new DepartmentService();
        public EmployeeResultController(IEmployeeResultService employeeResultService, IDropDownItemService dropDownItemService, IDropdownService dropdownService)
        {
            _employeeResultService = employeeResultService;
            _dropDownItemService = dropDownItemService;
            _dropdownService = dropdownService;
        }

        public async Task<ActionResult> AnnualEmployeeResultCreate(long annualPerformanceId = 0)
        {
            AnnualPerformanceEmpResultVM modes = new AnnualPerformanceEmpResultVM();
            modes.AnnualPerformanceId = annualPerformanceId;
            modes.AssessmentFrom = DateTime.Now.Date;
            modes.AssessmentTo = DateTime.Now.Date;
            if (annualPerformanceId > 0)
            {
                modes = await _employeeResultService.AnnualPerformanceEmployeeResult(annualPerformanceId);
            }
            return View(modes);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AnnualEmployeeResultCreate(AnnualPerformanceEmpResultVM modes)
        {
            long annualPerId = await _employeeResultService.AnnualPerformanceEmployeeResultSaveAndUpdate(modes);
            return RedirectToAction(nameof(AnnualEmployeeResultList), new { AnnualPerId = annualPerId });
        }

        public async Task<ActionResult> AnnualEmployeeResultList(long AnnualPerId = 0)
        {
            AnnualPerformanceEmpResultVM models = new AnnualPerformanceEmpResultVM();
            long EmpId = Common.GetIntUserId();
            models = await _employeeResultService.GetAnnualPerformanceEmployeeResultList(EmpId);
            return View(models);
        }

        public async Task<ActionResult> AnnualPerformancetList(long AnnualPerId = 0)
        {
            AnnualPerformanceEmpResultVM models = new AnnualPerformanceEmpResultVM();
            long EmpId = Common.GetIntUserId();
            models = await _employeeResultService.GetAnnualPerformanceEmployeeResultList(EmpId);
            return View(models);
        }


        public async Task<ActionResult> EmployeeResultCreate(long annualPerformanceId, long empId, long annualPerformanceDetailId = 0)
        {
            EmployeeResultsVM modes = new EmployeeResultsVM();


            modes = await _employeeResultService.GetEmployeeDataByEmpId(empId, annualPerformanceId, annualPerformanceDetailId);
            modes.AnnualPerformanceId = annualPerformanceId;
            modes.AnnualPerformanceDetailId = annualPerformanceDetailId;
            modes.Companies = _companyService.GetCompanySelectModels();
            modes.DDLDepartments = _departmentService.GetDepartmentSelectModels();
            return View(modes);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteEducationById(int EduId)
        {
            RResult rResult = new RResult();
            rResult = await _employeeResultService.DeleteEducationById(EduId);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ProfileQualitiesSubmit(EmployeeResultsVM dataModel)
        {
            RResult rResult = new RResult();
            rResult = await _employeeResultService.ProfileQualitiesReportSubmit(dataModel);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> AnnualPerformanceSubmit(EmployeeResultsVM model)
        {

            var returnResult = await _employeeResultService.AnnualPerformanceSubmitAsync(model);
            return RedirectToAction(nameof(EmployeeResultCreate), new { annualPerformanceId = returnResult.AnnualPerformanceId, empId = returnResult.EmployeeId, annualPerformanceDetailId = returnResult.AnnualPerformanceDetailId });
        }

        [HttpPost]
        public async Task<JsonResult> EducationContentsSave(int dropDownTypeId, string Name)
        {
            RResult rResult = new RResult();
            rResult = await _employeeResultService.EducationContentSave(dropDownTypeId, Name);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> AnnualPerformanceEmployeeResultDelete(long id)
        {
            RResult rResult = new RResult();
            rResult = await _employeeResultService.GetAnnualPerformanceEmployeeResultDelete(id);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDDLExaminations(int DropDownItemTypeId)
        {
            var rResult = _dropdownService.RenderDDLSM(_dropDownItemService.GetDropDownItemSelectModels(DropDownItemTypeId), true);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDDLInstitutions(int DropDownItemTypeId)
        {
            var rResult = _dropdownService.RenderDDLSM(_dropDownItemService.GetDropDownItemSelectModels(DropDownItemTypeId), true);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDDLSubjects(int DropDownItemTypeId)
        {
            var rResult = _dropdownService.RenderDDLSM(_dropDownItemService.GetDropDownItemSelectModels(DropDownItemTypeId), true);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDDLYears()
        {
            var rResult = _dropdownService.RenderDDLSM(Helper.GetYears(), true);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> AnnualPerformanceDetailSaveUpdate(EmployeeResultsVM model)
        {
            RResult rResult = new RResult();
            rResult = await _employeeResultService.AnnualPerformanceDetailSaveUpdate(model);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AnnualPerformanceCheckingList(DateTime? fromDates, DateTime? toDates)
        {

            if (fromDates == null || toDates == null)
            {
                int currentYear = DateTime.Now.Year;
                fromDates = new DateTime(currentYear - 1, 7, 1);
                toDates = new DateTime(currentYear, 6, DateTime.DaysInMonth(currentYear, 6));
            }
            long sigId = Common.GetIntUserId();

            EmployeeResultsVM model = await _employeeResultService.GetAnnualPerformanceAllInfo(sigId, fromDates, toDates);
            return View(model);
        }

        [HttpPost]
        public ActionResult AnnualPerformanceCheckingListFilter(DateTime? fromDate, DateTime? toDate)
        {
            return RedirectToAction(nameof(AnnualPerformanceCheckingList), new { fromDates = fromDate, toDates = toDate });
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SignatoryApprovalSubmition(EmployeeResultsVM model)
        {
            long result = await _employeeResultService.SignatoryApprovalSubmission(model);
            return RedirectToAction(nameof(AnnualPerformanceCheckingList));
        }

        [HttpGet]
        public async Task<JsonResult> GetSignatoryApprovalList(long aprDetailsId)
        {
            var result = await _employeeResultService.GetSignatoryApprovalList(aprDetailsId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EmployeeResultDetail(long annualPerformanceId, long empId, long annualPerformanceDetailId)
        {
            EmployeeResultsVM modes = new EmployeeResultsVM();

            modes = await _employeeResultService.GetEmployeeResultAllInfo(annualPerformanceId, empId, annualPerformanceDetailId);
            modes.AnnualPerformanceId = annualPerformanceId;
            modes.AnnualPerformanceDetailId = annualPerformanceDetailId;
            modes.Companies = _companyService.GetCompanySelectModels();
            modes.DDLDepartments = _departmentService.GetDepartmentSelectModels();

            return View(modes);
        }

        public async Task<ActionResult> AnnualPerformanceDetailList(long annualPerformanceId)
        {

            long employeeid = Common.GetIntUserId();
            EmployeeResultsVM model = await _employeeResultService.GetAnnualPerformanceDetailsById(annualPerformanceId);

            return View(model);
        }

        [HttpPost]
        public ActionResult AnnualPerformanceDetailsCheckingListFilter(DateTime? fromDate, DateTime? toDate)
        {
            return RedirectToAction(nameof(AnnualPerformanceDetailList), new { fromDates = fromDate, toDates = toDate });
        }

        [HttpGet]
        public JsonResult GetEmployeeMarks(long employeeId, string employeeCode, DateTime startDate, DateTime endDate)
        {
            var result = _employeeResultService.GetEmployeeMarks(employeeId, employeeCode, startDate, endDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployeeInfo(long EmployeeId)
        {
            var result = await _employeeResultService.GetEmployeeInfo(EmployeeId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}