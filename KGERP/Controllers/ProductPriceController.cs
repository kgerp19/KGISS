using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ProductPriceController : BaseController
    {
        // GET: ProductPrice
        private readonly IProductService productService;

        public ProductPriceController(IProductService productService)
        {
            this.productService = productService;
        }
            
        [HttpGet]
        
        public async Task<ActionResult> UpdateProductTpPrice(int companyId, int productId,string priceType)
        {
           
            ProductPriceModel model = new ProductPriceModel();
            model = await productService.GetProductTpPrice(companyId, productId, priceType);
            return View(model);
        }
        [HttpPost]
        
        public async Task<ActionResult> UpdateProductTpPrice(ProductPriceModel model)
        {
            var vm = await productService.SaveProductTpPrice(model);
            return RedirectToAction(nameof(UpdateProductTpPrice), new { companyId = model.CompanyId, productId=model.ProductId, priceType = model.PriceType });
        }

        [HttpGet]
        
        public async Task<ActionResult> UpdateProductSalePrice(int companyId, int productId, string priceType)
        {

            ProductPriceModel model = new ProductPriceModel();
            model = await productService.GetProductSalePrice(companyId, productId, priceType);
            return View(model);
        }

        [HttpPost]
        
        public async Task<ActionResult> UpdateProductSalePrice(ProductPriceModel model)
        {
            var vm = await productService.SaveProductSalePrice(model);
            return RedirectToAction(nameof(UpdateProductSalePrice), new { companyId = model.CompanyId, productId = model.ProductId, priceType = model.PriceType });
        }
        
        [HttpGet]
        public async Task<ActionResult> ProductTpPriceIndex(int companyId, string priceType)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            ProductPriceModel productPriceModel = new ProductPriceModel();
            productPriceModel = await productService.GetLastUpdatedProductTpPrice(companyId, priceType);
            return View(productPriceModel);
        }
        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, string priceType)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            ProductPriceModel productPriceModel = new ProductPriceModel();
            productPriceModel = await productService.GetLastUpdatedProductSalePrice(companyId, priceType);
            return View(productPriceModel);
        }
        


    }
}