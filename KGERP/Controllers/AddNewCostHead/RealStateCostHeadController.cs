using KGERP.Service.Implementation.Realestate.CustomersBooking;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using System.Threading.Tasks;
using System.Web.Mvc;
using KGERP.Service.ServiceModel;
using KGERP.Service.Implementation.RealStateCostHeadService;

namespace KGERP.Controllers.AddNewCostHead
{
    [CheckSession]
    public class RealStateCostHeadController : BaseController
    {      
        private readonly ICompanyService _companyService;
        private readonly IGLDLCustomerService gLDLCustomerService;
        private readonly ICustomerBookingService customerBookingService;
        private readonly MoneyReceiptService _moneyReceiptService;
        private readonly RealStateCostHead_Service stateCostHead_Service;
        public RealStateCostHeadController(
            RealStateCostHead_Service stateCostHead_Service,
            ICompanyService companyService,
            MoneyReceiptService moneyReceiptService
            )
        {
            _companyService = companyService;
            _moneyReceiptService = moneyReceiptService;
            this.stateCostHead_Service = stateCostHead_Service;
        }

        [HttpGet]
        public async Task<ActionResult> RealEstateCustomerCostHead(int companyId)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel();
            vm.CompanyId = companyId;
            vm.ProjectList = await _moneyReceiptService.ProjectList(companyId); 
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> RealEstateCustomerCostHead(GLDLBookingViewModel model)
        {
            if (model.ActionId==2)
            {
                var res = stateCostHead_Service.GetCostheadsMapping(model);
            }
            if (model.ActionId == 3)
            {
                var res = stateCostHead_Service.addamount(model);
            }
            model = await stateCostHead_Service.getcosthead(model);
            model.ProjectList = await _moneyReceiptService.ProjectList((int)model.CompanyId);
            return View(model);
        }      
    }
}