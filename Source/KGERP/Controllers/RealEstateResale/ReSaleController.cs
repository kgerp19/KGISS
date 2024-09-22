using KGERP.Service.Implementation;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.IncentiveModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.RealEstateResale
{
    [CheckSession]
    public class ReSaleController : Controller
    {
        private readonly MoneyReceiptService _moneyReceiptService;
        public ReSaleController(MoneyReceiptService _moneyReceiptService)
        {
            this._moneyReceiptService = _moneyReceiptService;
        }
        public async Task<ActionResult> Index(int companyId)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel();
            vm.CompanyId = companyId;
            vm.ProjectList = await _moneyReceiptService.ProjectList(companyId);
            
            return View(vm);
        }
    }
}