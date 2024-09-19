using KGERP.Data.Models;
using KGERP.Service.Implementation;
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
    public class KGREPaymentController : Controller
    {
        ERPEntities context = new ERPEntities();
        IKGREPaymentInfoService paymentService = new KGREPaymentInfoService();
        IPaymentModeService paymentModeService = new PaymentModeService();
        IBankService bankService = new BankService();
        IKgReCrmService kgReCrmService = new KgReCrmService();
        IKGREProjectService kGREProjectService = new KGREProjectService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());



        
        [HttpGet]
        public ActionResult Index(int companyId, int? Page_No, string searchDate, string searchText)
        {
            searchText = searchText ?? "";
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            List<KGREPaymentModel> payments = paymentService.GetPayments(searchDate, searchText, companyId);

            int Size_Of_Page = 10000;
            int No_Of_Page = (Page_No ?? 1);
            return View(payments.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(int id)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            KgReCrmViewModel vm = new KgReCrmViewModel();
            vm.Payment = await paymentService.GetPayment(id);
            vm.Payment.CompanyId = companyId;
            vm.Payment.ProductType = "F";
            vm.Banks = bankService.GetBankSelectModels(); 
            vm.KGREProjects =  kgReCrmService.GetProjects(companyId);
            vm.PaymentModes = paymentModeService.PaymentModes();
            vm.PaymentFors = dropDownItemService.GetDropDownItemSelectModels(68);
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult CreateOrEdit(string VendorName, string VendorId, KgReCrmViewModel vm)
        {
            string message = string.Empty;
            bool result = false;

            //vm.Payment.ClientId = string.IsNullOrEmpty(VendorId) ? 0 : Convert.ToInt32(VendorId);

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
