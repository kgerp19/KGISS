using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly IPaymentModeService paymentModeService;
        private readonly IVendorService vendorService;
        private readonly IBankService bankService;
        public PaymentController(IPaymentModeService paymentModeService, IBankService bankService, IPaymentService paymentService, IVendorService vendorService)
        {
            this.paymentService = paymentService;
            this.vendorService = vendorService;
            this.bankService = bankService;
            this.paymentModeService = paymentModeService;
        }
        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-1);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            PaymentModel paymentModel = new PaymentModel();
            
            paymentModel = await paymentService.GetPayments(companyId, fromDate, toDate);

            paymentModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            paymentModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(paymentModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(PaymentModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(int id)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            PaymentViewModel vm = new PaymentViewModel();
            vm.Payment = await paymentService.GetPayment(id);
            vm.Payment.CompanyId = companyId;
            vm.Payment.ProductType = "F";
            vm.Banks = bankService.GetBankSelectModels();
            vm.PaymentModes = paymentModeService.GetPaymentReceiveSelectModels();
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult CreateOrEdit(string VendorName, string VendorId, PaymentViewModel vm)
        {
            string message = string.Empty;
            bool result = false;

            vm.Payment.VendorId = string.IsNullOrEmpty(VendorId) ? 0 : Convert.ToInt32(VendorId);

            if (vm.Payment.PaymentId <= 0)
            {
                TempData["message"] = message;
                result = paymentService.SavePayment(0, vm.Payment, out message);
            }

            else
            {
                result = paymentService.SavePayment(vm.Payment.PaymentId, vm.Payment, out message);
            }
            if (!result)
            {
                return View();
            }
            return RedirectToAction("Index", new { companyId = vm.Payment.CompanyId });
        }
    }
}