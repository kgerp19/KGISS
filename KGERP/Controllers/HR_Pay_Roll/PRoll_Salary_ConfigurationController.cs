using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Vml.Office;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_SalaryConfiguration_Services;
using KGERP.Service.Interface;
using KGERP.Utility;
using KGERP.Utility.Interface;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class PRoll_Salary_ConfigurationController : Controller
    {
        private readonly IPRoll_SalaryConfiguration iPRoll_Salary;
        private readonly ICompanyService _companyService;
        private readonly IDropdownService _dropdownService;

        public PRoll_Salary_ConfigurationController(IPRoll_SalaryConfiguration iPRoll_Salary, ICompanyService _companyService, IDropdownService dropdownService)
        {
            this.iPRoll_Salary = iPRoll_Salary;
            this._companyService = _companyService;
            _dropdownService = dropdownService;
        }
        public async Task<ActionResult> Salary_Configuration(int companyId)
        {
            PRoll_SalaryConfigurationViewModel pRoll = new PRoll_SalaryConfigurationViewModel();
            pRoll.CompanyId = companyId;
            pRoll.Companies = _companyService.GetAllCompanySelectModels2();
            pRoll.DDLPayrollShit = _dropdownService.RenderDDL(await _companyService.GetDDLPayrollShit(companyId), true);
            return View(pRoll);
        }
        [HttpPost]
        public async Task<ActionResult> Salary_Configuration(PRoll_SalaryConfigurationViewModel pRoll)
        {
            var result = await iPRoll_Salary.AddSalaryConfiguration(pRoll);
            if (result > 0)
            {
                return RedirectToAction("Salary_ConfigurationList", new { companyId = pRoll.CompanyId });
            }
            return RedirectToAction("Salary_ConfigurationList", new { companyId = pRoll.CompanyId });
        }

        public async Task<ActionResult> Salary_ConfigurationDetalis(long id)
        {
            var res = await iPRoll_Salary.SalaryConfigurationDetalis(id);
            return View(res);
        }

        public async Task<ActionResult> Salary_ConfigurationDelete(long id)
        {
            var res = await iPRoll_Salary.DeleteSalaryConfiguration(id);
            return RedirectToAction("Salary_ConfigurationList", new { companyId = res });
        }
        public async Task<ActionResult> Salary_ConfigurationList(int companyId)
        {
            var res = await iPRoll_Salary.SalaryConfigurationList(companyId);
            return View(res);
        }
        public async Task<ActionResult> Salary_ConfigurationUpdate(long id)
        {
            var res = await iPRoll_Salary.SalaryConfigurationDetalis(id);
            res.DDLPayrollShit = _dropdownService.RenderDDL(await _companyService.GetDDLPayrollShit(res.CompanyId), true);
            res.Companies = _companyService.GetAllCompanySelectModels2();
            return View(res);
        }
        [HttpPost]
        public async Task<ActionResult> Salary_ConfigurationUpdate(PRoll_SalaryConfigurationViewModel pRoll)
        {
            var result = await iPRoll_Salary.UpdateSalaryConfiguration(pRoll);
            if (result > 0)
            {
                return RedirectToAction("Salary_ConfigurationUpdate", new { id = result });
            }
            return View(pRoll);
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployeePRollSheetIdByEmployeeId(long empId)
        {
            long? result = await _companyService.GetPayrollSheetIdByEmpId(empId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployeePRollSheetIdByCompany(int ComId)
        {
            var result = _dropdownService.RenderDDL(await _companyService.GetDDLPayrollShit(ComId), true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}