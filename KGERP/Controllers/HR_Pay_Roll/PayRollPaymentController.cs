using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Service.HR_Pay_Roll_Service.ParollServices;
using KGERP.Service.HR_Pay_Roll_Service.PayRoll_Payment_Purpose_service;
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
    public class PayRollPaymentController : Controller
    {
        public readonly IPaymentPurposeServices PaymentPurposeServices;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public PayRollPaymentController(IPaymentPurposeServices paymentPurposeServices, IApproval_Service Service, ICompanyService companyService)
        {
            this.PaymentPurposeServices = paymentPurposeServices;
            _Service = Service;
            _companyService = companyService;
        }

        public async Task<ActionResult> Index()
        {   PRoll_PaymentPurposeVm vm= new PRoll_PaymentPurposeVm();
            vm = await PaymentPurposeServices.GetPurpose();
            return View(vm);
        }

        public async Task<ActionResult> AddPayPaymentPurpose()
        {
            PRoll_PaymentPurposeVm vm = new PRoll_PaymentPurposeVm();
          
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> AddPayPaymentPurpose(PRoll_PaymentPurposeVm Model)
        {
            var result = await PaymentPurposeServices.AddPurpose(Model);
            if (result > 0)
            {
                return RedirectToAction("Index");
            }
            return View(Model);

            
        }

        public async Task<ActionResult> EditPayPaymentPurpose(int PaymentPurpose)

        {
            PRoll_PaymentPurposeVm model = new PRoll_PaymentPurposeVm();
            model = await PaymentPurposeServices.EditPayPurpose(PaymentPurpose);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> EditPayPaymentPurpose(PRoll_PaymentPurposeVm model)

        {
            var obj = await PaymentPurposeServices.UpdatePayPurpose(model);

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> DeletePayRollPurpose(int PaymentPurpose)
        {
            var result = await PaymentPurposeServices.DeletePurpose(PaymentPurpose);
            return RedirectToAction("Index");


        }

    }
}