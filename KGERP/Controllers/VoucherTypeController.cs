using KGERP.Service.Implementation;
using KGERP.Utility;

namespace KGERP.Controllers
{
    [CheckSession]
    public class VoucherTypeController : BaseController
    {
        private readonly VoucherTypeService voucherTypeService;
        public VoucherTypeController(VoucherTypeService voucherTypeService)
        {
            this.voucherTypeService = voucherTypeService;
        }
        //public ActionResult Index()
        //{
        //    List<VendorTypeModel> vendorTypes = vendorTypeService.GetVendorTypes();
        //    return View(vendorTypes);
        //}

    }
}