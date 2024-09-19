using DocumentFormat.OpenXml.EMMA; 
using KGERP.Service.Interface;
using KGERP.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc; 
using KGERP.Service.JobService;

namespace KGERP.Controllers.JobDescription
{
    [CheckSession]
    public class JobDescriptionController : Controller
    {
        public readonly IJobDescription _JobDescription;
       // private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public JobDescriptionController(IJobDescription iJobDescription,  ICompanyService companyService)
        {
            _JobDescription = iJobDescription;            
            _companyService = companyService;
        }
        public async Task<ActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> AddJobDescription(long employeeId)
        {
            JobDescriptionModel model = new JobDescriptionModel();            
            model = await _JobDescription.JobDescriptionList(employeeId);
            model.JobDescriptionTypeList = new SelectList(_JobDescription.GetJobDescriptionTypes(), "Value", "Text");

            return View(model);
        }
        
        [HttpPost]
        public async Task<ActionResult> AddJobDescription(JobDescriptionModel jobDescription)
        {
            var result = await _JobDescription.AddJobDescription(jobDescription);
            if (result > 0)
            {
                return RedirectToAction("AddJobDescription", new { employeeId = jobDescription.EmployeeId });
            }
            return View(jobDescription);
        }

        //public async Task<ActionResult> CashPaymentList(int companyId)
        //{
        //    var result = await pRoll_CashPayment.CashPaymentList(companyId);
        //    return View(result);
        //}

        //public async Task<ActionResult> Delete(long id)
        //{
        //    var result = await pRoll_CashPayment.Delete(id);
        //    return RedirectToAction("CashPaymentList");
        //}


    }
}