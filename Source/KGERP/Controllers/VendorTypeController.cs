using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class VendorTypeController : BaseController
    {
        private readonly IVendorTypeService vendorTypeService;
        public VendorTypeController(IVendorTypeService vendorTypeService)
        {
            this.vendorTypeService = vendorTypeService;
        }
        public ActionResult Index()
        {
            List<VendorTypeModel> vendorTypes = vendorTypeService.GetVendorTypes();
            return View(vendorTypes);
        }

    }
}