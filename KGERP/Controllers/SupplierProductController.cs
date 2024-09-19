using KGERP.Service.Implementation.SupplierProducts;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class SupplierProductController : BaseController
    {
        private SupplierProductService supplierProductService;
        public SupplierProductController(SupplierProductService supplierProductService)
        {
            this.supplierProductService = supplierProductService;
        }

        // GET: SupplierProduct
        public async Task<ActionResult> Index(int companyId = 0)
        {
            SupplierProductViewModel model = new SupplierProductViewModel();
            model = await supplierProductService.SupplierProductList();
            model.CompanyId = companyId;
            return View(model);
        }

        public async Task<ActionResult> Delete(SupplierProductViewModel model)
        {
            model = await supplierProductService.DeleteSupplierProduct(model);
            return RedirectToAction("SupplierProductBOM", new { vendorId = model.VendorId, companyId = model.CompanyId });
        }
        public async Task<ActionResult> SupplierProductBOM(int vendorId=0,int companyId=0)
        {            
            SupplierProductViewModel model = new SupplierProductViewModel();        
            model = await supplierProductService.SupplierWaisProduct(vendorId);
            model.CompanyId = companyId;         
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SupplierProductViewModel model)
        {
            model = await supplierProductService.SaveSupplierProduct(model);
            return RedirectToAction("SupplierProductBOM", new { vendorId = model.VendorId, companyId =model.CompanyId});
        }
    }
}