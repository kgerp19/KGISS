using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Vml.Office;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_AttendanceLog_Service;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using Remotion.Data.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class PRoll_AttendanceLogsController : Controller
    {
        // GET: PRoll_AttendanceLogs
        private readonly IPRoll_AttendanceLogService logService;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public PRoll_AttendanceLogsController(IPRoll_AttendanceLogService logService, IApproval_Service _Service, ICompanyService _companyService)
        {
            this.logService = logService;
            this._Service = _Service;
            this._companyService = _companyService;
        }
        public async Task<ActionResult> AddNew()
        {
            PRoll_AttendanceLogViewModel model = new PRoll_AttendanceLogViewModel();
            model.Year = DateTime.Now.Year;
            model.Month = DateTime.Now.Month - 1;
            model.YearsList = _Service.YearsListLit();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AddNew(PRoll_AttendanceLogViewModel model)
        {
            DateTime date = new DateTime(model.Year,model.Month,1);
            DateTime enddate =date.AddMonths(1).AddDays(-1);
            model.FromDate= date;
            model.ToDate= enddate;  
             var res= await logService.AddNew(model);
            if (res>0)
            {
                return RedirectToAction("AttendanceList", new { companyId = model.CompanyId });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> AttendanceList(int companyId)
        {
          var res= await logService.AttendanceList(companyId);
            return View(res);
        }   
        [HttpGet]
        public async Task<ActionResult> AttendanceProcess(long id, int companyId)
        {
            var res = await logService.AttendanceProcessPayroll(id);
            return RedirectToAction("AttendanceList" , new { companyId = companyId });
        }

        [HttpGet]
        public async Task<ActionResult> Delete(long id)
        {
            var res= await logService.Delete(id);
            return RedirectToAction("AttendanceList");
        }
        [HttpGet]
        public async Task<ActionResult> Details(int companyId,long id)
        {
            if (companyId == 1985) // 1985 Payroll Company on company Table
            {
                companyId = 0; // for all attandence 
            }
            var res = await logService.Details(companyId,id);
            res.StrFromDate= res.FromDate.ToShortDateString();
            res.StrToDate = res.ToDate.ToShortDateString();
            res.Companies = _companyService.GetAllCompanySelectModels2();
            res.YearsList = _Service.YearsListLit();
            return View(res);
        }
        [HttpPost]
        public async Task<ActionResult> EditDetails(PRoll_AttendanceLogViewModel model)
        {
            var res = await logService.DetailsEdit(model);
            return RedirectToAction("Details", new { companyId = model.CompanyId, id = model.AttendanceLogId });
        }  
        //[HttpPost]
        //public async Task<ActionResult> AttendanceLogsFilter(PRoll_AttendanceLogViewModel pRoll)
        //{
        //    return RedirectToAction("Details", new { companyId = pRoll.CompanyId,id= pRoll.AttendanceLogId});
        //}
    }
}