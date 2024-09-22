using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.KfmlInstallments;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.KFMLInstallment
{
    [CheckSession]
    public class Kfml_InstallmentController : Controller
    {
        private readonly IVoucherTypeService voucherTypeService;
        private readonly ICostHeadsService _costHeadService;
        private readonly ITeamService _teamService;
        private readonly IKfml_Installment _Installment;
        private readonly ConfigurationService _service;
        public Kfml_InstallmentController( IVoucherTypeService voucherTypeService,
            ICostHeadsService _costHeadService, ITeamService teamService, IKfml_Installment installment, ConfigurationService _service)
        {
            this.voucherTypeService = voucherTypeService;
            this._costHeadService = _costHeadService;
            _teamService = teamService;
            _Installment = installment;
            this._service = _service;
        }
        // GET: Kfml_Installment
        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            IKfml_InstallmentViewModel vm = new IKfml_InstallmentViewModel() { CompanyId = companyId };
            vm.ApplicationDate = DateTime.Now;
            vm.ApplicationDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.ProductCategoryList = voucherTypeService.GetProductCategoryGcclAndKFmal(companyId);
            vm.LstPurchaseCostHeads = await _costHeadService.GetCostHeadsByCompanyId(companyId);
            vm.BookingInstallmentType = await _costHeadService.GetCompanyBookingInstallmentType(companyId);
            vm.Employee = new SelectList(await _service.EmployeeByCompanyId(companyId), "Value", "Text");
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Index(IKfml_InstallmentViewModel model)
        {
            var result= await _Installment.AddInstallment(model);
            return RedirectToAction("ViewDetalis", new { companyId =10, bookingId=result.BookingId});
        }
        [HttpGet]
        public async Task<ActionResult> InstallmentList(int companyId)
        {
            IKfml_InstallmentViewModel kfml = new IKfml_InstallmentViewModel();              
            kfml = await _Installment.InstallmentList(companyId);
            kfml.CompanyId = companyId;
            return View(kfml);
        }
        [HttpGet]
        public async Task<ActionResult> ViewDetalis(int companyId, long bookingId)
        {
            IKfml_InstallmentViewModel kfml = new IKfml_InstallmentViewModel();
            kfml = await _Installment.CustomerBookingView(companyId,bookingId);
            kfml.CompanyId = companyId;
            return View(kfml);
        }  

        [HttpPost]
        public async Task<ActionResult> finalsubmit(IKfml_InstallmentViewModel model)
        {
            IKfml_InstallmentViewModel kfml = await _Installment.CustomerBookingView(model.CompanyId, model.BookingId);
            await _Installment.AccountingSalesPushKFML(model.CompanyId, kfml, (int)KfmalJournalEnum.SalesVoucher);

            return RedirectToAction("ViewDetalis", new { companyId = 10, bookingId = kfml.BookingId });
        }

    }
}