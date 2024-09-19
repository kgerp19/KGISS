using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
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
    public class AdminApprovalProcessSystemController : BaseController
    {
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public AdminApprovalProcessSystemController(IApproval_Service Service, ICompanyService companyService)
        {
            _Service = Service;
            _companyService = companyService;
        }


        [HttpGet]
        public async Task<ActionResult> AdminApproval(int companyId=0,int month=0,int year=0,int actionId = 0)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();

            if (actionId==101)
            {
                var id2 = Session["Id"];
                if ((long)id2 > 0)
                {
                    model = await _Service.Approvalformanagment(model);
                    model.Year = year;
                    model.Month = month;
                    model.CompanyId = companyId;
                    model.SectionEmployeeId = (long)id2;
                    // model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
                    model.YearsList = _Service.YearsListLit();
                    model.Companies = _companyService.GetAllCompanySelectModels2();
                    model.ActionId = 0;
                    model = await _Service.Approvalformanagment(model);
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
                // model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
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
        public async Task<ActionResult> AdminApproval(ApprovalSystemViewModel model2)
        {
            ApprovalSystemViewModel model = new ApprovalSystemViewModel();
            if (model2.ActionId==10)
            {
                var res = await _Service.AccStatusChange(model2);
                var id = Session["Id"];
                if ((long)id > 0)
                {
                    res.SectionEmployeeId = (long)id;
                    model = await _Service.Approvalformanagment(res);
                    model.Year = res.Year;
                    model.Month = res.Month;
                    model.CompanyId = res.CompanyId;
                    model.SectionEmployeeId = (long)id;
                    // model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
                    model.YearsList = _Service.YearsListLit();
                    model.Companies = _companyService.GetAllCompanySelectModels2();
                    model.ActionId = 0;
                }
                else
                {
                    model.SectionEmployeeId = 0;
                    model.ValidationSMS = "Session expire";
                }
                return RedirectToAction("AdminApproval",new { companyId = model.CompanyId, month=model.Month, year=model.Year, actionId=101});
            }
            else
            {
                var id = Session["Id"];
                if ((long)id > 0)
                {
                    model = await _Service.Approvalformanagment(model2);
                    model.Year = model2.Year;
                    model.Month = model2.Month;
                    model.CompanyId = model2.CompanyId;
                    model.SectionEmployeeId = (long)id;
                    // model.reportCatagoryList = await _Service.ReportcatagoryLit(companyId);
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



        [HttpPost]
        public async Task<ActionResult> AccountingApproval(ApprovalSystemViewModel model)
        {
            var res = await _Service.AccStatusChange(model);
            return RedirectToAction("AdminApproval", new { model });
        }

    }
}