using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers.ApprovalSystem
{
    [CheckSession]
    public class ApprovalProcessSystemController : BaseController
    {
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public ApprovalProcessSystemController(IApproval_Service Service, ICompanyService companyService)
        {
            _Service = Service;
            _companyService = companyService;
        }

        
        [HttpGet]
        public async Task<ActionResult> ApprovalProcessing(int companyId)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();
            DateTime date = DateTime.Now;  
            date= date.AddMonths(-1);
            model = await _Service.ApprovalList(companyId, date.Year, date.Month);
            model.Year = date.Year;
            model.Month = date.Month;
            var company = _companyService.GetCompany(companyId);
            model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
            model.YearsList = _Service.YearsListLit();
            model.CompanyName = company.Name;
            model.CompanyId = company.CompanyId;
            return View(model);
        }   
        
        [HttpPost]
        public async Task<ActionResult> ApprovalProcessing(ApprovalSystemViewModel model2)
        {
            var result = await _Service.CheckApproval(model2);
            if (result)
            {
                var res= await _Service.AddApproval(model2);
                return RedirectToAction(nameof(ApprovalProcessing), new { companyId = model2.CompanyId });
            }
            else
            {
                ApprovalSystemViewModel model = new ApprovalSystemViewModel();
                DateTime date = DateTime.Now;
                date = date.AddMonths(-1);
                model = await _Service.ApprovalList(model2.CompanyId, date.Year, date.Month);
                model.Year = date.Year;
                model.Month = date.Month;
               
                var company = _companyService.GetCompany(model2.CompanyId);
                model.reportCatagoryList = await _Service.ReportcatagoryLit(model2.CompanyId);
                model.ReportCategoryId = model2.ReportCategoryId;
                model.YearsList = _Service.YearsListLit();
                model.CompanyName = company.Name;
                model.CompanyId = company.CompanyId;
                ModelState.AddModelError("ReportCategoryId", "The report category is already exited on the same date");
                return View(model);
            }
        }  
        
        [HttpGet]
        public async Task<ActionResult> AccountingApprovalList (int companyId)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();
            DateTime date = DateTime.Now;
            date = date.AddMonths(-1);
            var id = Session["Id"];
            //model.Year = date.Year;
            //model.Month = date.Month;
            var company = _companyService.GetCompany(companyId);
            model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
            model.YearsList = _Service.YearsListLit();
            model.CompanyName = company.Name;
            model.CompanyId = company.CompanyId;
            model.SectionEmployeeId = (long)id;
            model = await _Service.AccountingApprovalList(model);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AccountingApprovalList(ApprovalSystemViewModel model)
        {
            var id = Session["Id"];
            var company = _companyService.GetCompany(model.CompanyId);
            model.reportCatagoryList = await _Service.ReportcatagoryLit(model.CompanyId);
            model.YearsList = _Service.YearsListLit();
            model.CompanyName = company.Name;
            model.CompanyId = company.CompanyId;
            model.SectionEmployeeId = (long)id;
            model = await _Service.AccountingApprovalList(model);
            return View(model);
        }

            [HttpGet]
        public async Task<ActionResult> ApprovalSignotory(long approvalId)
        {
            var res = await _Service.ApprovalSignetory(approvalId);
            
            return Json(res, JsonRequestBehavior.AllowGet);
        }    
        [HttpGet]
        public async Task<ActionResult>SalarySignotory(long approvalId)
        {
            var res = await _Service.SalarySheetApprovalSignetory(approvalId);
            
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> ApprovalStutasUpdate(long approvalId)
        {
            long employeid = (long)Session["Id"];
            var res = await _Service.AccApprovalStutasUpdate(approvalId);
            
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> SalaryApprovalStutasUpdate(long approvalId)
        {
            long employeid = (long)Session["Id"];
            var res = await _Service.SalaryAccApprovalStutasUpdate(approvalId);

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> ApprovalDelete(long approvalId)
        {
            var res = await _Service.ApprovalDelete(approvalId);            
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> AccountingApproval(int companyId = 0, int month = 0, int year = 0, int actionId = 0)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();
            var id = Session["Id"];
            if ((long)id>0)
            {
                if (actionId==101)
                {
                    model = await _Service.ApprovalforEmployeeList((long)id, companyId, year, month);
                    model.Year = year;
                    model.Month = month;
                    model.SectionEmployeeId = (long)id;
                }
                else
                {
                    DateTime date = DateTime.Now;
                    date = date.AddMonths(-1);
                    model = await _Service.ApprovalforEmployeeList((long)id, companyId, date.Year, date.Month);
                    model.Year = date.Year;
                    model.Month = date.Month;
                    model.SectionEmployeeId = (long)id;
                }

            }
            else
            {
                model.SectionEmployeeId = 0;
                model.ValidationSMS = "Session expire";
            }
            var company = _companyService.GetCompany(companyId);
            model.CompanyName = company.Name;
            model.CompanyId = company.CompanyId;
            model.YearsList = _Service.YearsListLit();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AccountingApproval(ApprovalSystemViewModel model)
        {
            if (model.ActionId==11) {
                var res = await _Service.AccStatusChange(model);
            }
            ApprovalSystemViewModel model2 = new ApprovalSystemViewModel();
            if (model.ActionId == 100)
            {              
                var id2 = Session["Id"];
                if ((long)id2 > 0)
                {
                    DateTime date = DateTime.Now;
                    date = date.AddMonths(-1);
                    model2 = await _Service.ApprovalforEmployeeList((long)id2, model.CompanyId, model.Year, model.Month);
                    model2.Year = model.Year;
                    model2.Month = model.Month;
                    model2.SectionEmployeeId = (long)id2;
                }
                else
                {
                    model2.SectionEmployeeId = 0;
                    model2.ValidationSMS = "Session expire";
                }
                var company2 = _companyService.GetCompany(model.CompanyId);
                model2.CompanyName = company2.Name;
                model2.CompanyId = company2.CompanyId;
                model2.YearsList = _Service.YearsListLit();
                model2.ActionId = 0;
                return RedirectToAction("AccountingApproval", new { companyId=model2.CompanyId , month=model.Month, year=model.Year, actionId=101 });
            }
            
            var id = Session["Id"];
            if ((long)id > 0)
            {
                DateTime date = DateTime.Now;
                date = date.AddMonths(-1);
                model2 = await _Service.ApprovalforEmployeeList((long)id, model.CompanyId, date.Year, date.Month);
                model2.Year = date.Year;
                model2.Month = date.Month;
                model2.SectionEmployeeId = (long)id;
            }
            else
            {
                model2.SectionEmployeeId = 0;
                model2.ValidationSMS = "Session expire";
            }
            var company = _companyService.GetCompany(model.CompanyId);
            model2.CompanyName = company.Name;
            model2.CompanyId = company.CompanyId;
            model2.YearsList = _Service.YearsListLit();
            model2.ActionId = 0;
            model2.ActionId = 0;
            return RedirectToAction("AccountingApproval", new { companyId = model2.CompanyId, month = model2.Month, year = model2.Year, actionId = 0 });
        }

    }
}