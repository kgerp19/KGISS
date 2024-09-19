using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class PRoll_Cash_PaymentsController : Controller
    {
        public readonly IPRoll_CashPayment pRoll_CashPayment;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public PRoll_Cash_PaymentsController(IPRoll_CashPayment pRoll_CashPayment, IApproval_Service Service, ICompanyService companyService)
        {
            this.pRoll_CashPayment = pRoll_CashPayment;
            _Service = Service;
            _companyService = companyService;
        }
        public async Task<ActionResult> Index()
        {
            return View();
        }  
        public async Task<ActionResult> AddPayment(int companyId)
        {
            PRoll_CashPaymentViewModel model = new PRoll_CashPaymentViewModel();
            model.YearsList = _Service.YearsListLit();
            model.Year = DateTime.Now.Year;
            model.Month = DateTime.Now.Month - 1;
            model.CompanyId = companyId;
            //model.Companies = _companyService.GetAllCompanySelectModels2();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AddPayment(PRoll_CashPaymentViewModel pRoll)
        {
            var result = await pRoll_CashPayment.AddCashPayment(pRoll);
            if (result > 0)
            {
                return RedirectToAction("CashPaymentList" , new { companyId = pRoll.CompanyId });
            }
            return View(pRoll);
        }

        public async Task<ActionResult> CashPaymentList(int companyId)
        {
            var result = await pRoll_CashPayment.CashPaymentList(companyId);
            return View(result);
        }

        public async Task<ActionResult> Delete(long id)
        {
            var result = await pRoll_CashPayment.Delete(id);
            return RedirectToAction("CashPaymentList");
        }


    }
}