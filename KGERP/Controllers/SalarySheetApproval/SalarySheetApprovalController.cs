using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.SalarySheetApproval
{
    [CheckSession]
    public class SalarySheetApprovalController : Controller
    {
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public SalarySheetApprovalController(IApproval_Service Service, ICompanyService companyService)
        {
            _Service = Service;
            _companyService = companyService;
        }

        
        [HttpGet]
        public async Task<ActionResult> SalarySheetApprovalProcessing(int companyId)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();
            DateTime date = DateTime.Now;
            date = date.AddMonths(-1);
            model = await _Service.SalarySheetApprovalList(companyId);
            model.Year = date.Year;
            model.Month = date.Month;
            var company = _companyService.GetCompany(companyId);
           // model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
            model.YearsList = _Service.YearsListLit();
            model.CompanyName = company.Name;
            model.CompanyId = company.CompanyId;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SalarySheetApprovalProcessing(ApprovalSystemViewModel model2)
        {
            var result = await _Service.CheckApprovalsalarySheet(model2);
            if (result)
            {
                var res = await _Service.AddSalarySheetApproval(model2);
                return RedirectToAction(nameof(SalarySheetApprovalProcessing), new { companyId = model2.CompanyId });
            }
            else
            {
                ApprovalSystemViewModel model = new ApprovalSystemViewModel();
                DateTime date = DateTime.Now;
                date = date.AddMonths(-1);
                model = await _Service.SalarySheetApprovalList(model2.CompanyId);
                model.Year = date.Year;
                model.Month = date.Month;
                var company = _companyService.GetCompany(model2.CompanyId);
                model.YearsList = _Service.YearsListLit();
                model.CompanyName = company.Name;
                model.CompanyId = company.CompanyId;
                ModelState.AddModelError("ReportCategoryId", "The report category is already exited on the same date");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> SalarySheetAdminApproval(int companyId = 0, int actionId = 0)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();

            if (actionId == 101)
            {
                var id2 = Session["Id"];
                if ((long)id2 > 0)
                {
                    model = await _Service.SalarySheetApprovalformanagment(model);
                    model.CompanyId = companyId;
                    model.SectionEmployeeId = (long)id2;
                    model.YearsList = _Service.YearsListLit();
                    model.Companies = _companyService.GetAllCompanySelectModels2();
                    model.ActionId = 0;
         
                }
                else
                {
                    model.SectionEmployeeId = 0;
                    model.ValidationSMS = "Session expire";
                }
                return View(model);
            }

            var id = Session["Id"];
            if ((long)id > 0)
            {
                DateTime date = DateTime.Now;
                date = date.AddMonths(-1);
                model.Year = date.Year;
                model.Month = date.Month;
                model.SectionEmployeeId = (long)id;
                model.ActionId = 0;
                model.YearsList = _Service.YearsListLit();
                model.Companies = _companyService.GetAllCompanySelectModels2();
            }
            else
            {
                model.SectionEmployeeId = 0;
                model.ValidationSMS = "Session expire";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SalarySheetAdminApproval(ApprovalSystemViewModel model2)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();
            if (model2.ActionId == 10)
            {
                var res = await _Service.SalarySheetAccStatusChange(model2);
                var id = Session["Id"];
                if ((long)id > 0)
                {
                    res.SectionEmployeeId = (long)id;
                    model = await _Service.SalarySheetApprovalformanagment(res);
                    model.Year = res.Year;
                    model.Month = res.Month;
                    model.CompanyId = res.CompanyId;
                    model.SectionEmployeeId = (long)id;
                    model.YearsList = _Service.YearsListLit();
                    model.Companies = _companyService.GetAllCompanySelectModels2();
                    model.ActionId = 0;
                }
                else
                {
                    model.SectionEmployeeId = 0;
                    model.ValidationSMS = "Session expire";
                }
                return RedirectToAction("SalarySheetAdminApproval", new { companyId = model.CompanyId, actionId = 101 });
            }
            else
            {
                var id = Session["Id"];
                if ((long)id > 0)
                {
                    model = await _Service.SalarySheetApprovalformanagment(model2);
                    model.Year = model2.Year;
                    model.Month = model2.Month;
                    model.CompanyId = model2.CompanyId;
                    model.SectionEmployeeId = (long)id;
                    model.YearsList = _Service.YearsListLit();
                    model.Companies = _companyService.GetAllCompanySelectModels2();
                    model.ActionId = 0;
                }
                else
                {
                    model.SectionEmployeeId = 0;
                    model.ValidationSMS = "Session expire";
                }
                return View(model);
            }
        }

        //ani
        
        [HttpGet]
        public async Task<ActionResult> SalarySheetSubmitList()
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();
            DateTime date = DateTime.Now;
            date = date.AddMonths(-1);
            model = await _Service.SalarySheetList(model);
            model.Year = date.Year;
            model.Month = date.Month;
            //var company = _companyService.GetCompany(companyId);
            // model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
            model.YearsList = _Service.YearsListLit();
            //model.CompanyName = company.Name;
            //model.CompanyId = company.CompanyId;
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SalarySheetSubmitList(ApprovalSystemViewModel model2)
        {
            //var result = await _Service.CheckApprovalsalarySheet(model2);
            //if (result)
            //{
            //    var res = await _Service.AddSalarySheetApproval(model2);
            //    return RedirectToAction(nameof(SalarySheetSubmitList), new { companyId = model2.CompanyId });
            //}
            //else
            //{
                ApprovalSystemViewModel model = new ApprovalSystemViewModel();
                DateTime date = DateTime.Now;
                date = date.AddMonths(-1);
                model = await _Service.SalarySheetList( model2);
                model.Year = date.Year;
                model.Month = date.Month;
                var company = _companyService.GetCompany(model2.CompanyId);
                model.YearsList = _Service.YearsListLit();
                model.CompanyName = company.Name;
                model.CompanyId = company.CompanyId;
                ModelState.AddModelError("ReportCategoryId", "The report category is already exited on the same date");
                return View(model);
            //}
        }
    }
}