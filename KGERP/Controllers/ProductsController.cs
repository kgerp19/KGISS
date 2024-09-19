using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ProductsController : BaseController
    {
        private readonly IUnitService unitService;
        private readonly IProductService productService;
        private readonly IProductSubCategoryService productSubCategoryService;
        private readonly IProductCategoryService productCategoryService;
        public ProductsController(IProductService productService, IProductCategoryService productCategoryService,
        IProductSubCategoryService productSubCategoryService, IUnitService unitService)
        {
            this.productService = productService;
            this.productCategoryService = productCategoryService;
            this.productSubCategoryService = productSubCategoryService;
            this.unitService = unitService;
        }

        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, string productType)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            ProductModel products =await productService.GetProducts(companyId, productType);
           
            if (productType.Equals("R"))
            {
                ViewBag.Heading = "Raw Materials List";
            }
            if (productType.Equals("F"))
            {
                ViewBag.Heading = "Finish Product List";
            }
            return View(products);
        }

        public JsonResult GetProductUnitPrice(int id)
        {
            var productInfo = productService.GetProductUnitPrice(id);
            return Json(productInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        
        public ActionResult CreateOrEdit(int id, string productType)
        {
            ProductViewModel vm = new ProductViewModel();
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            vm.Product = productService.GetProduct(id, productType);
            if (id == 0)
            {
                vm.PackName = "";
            }
            else
            {
                vm.PackName = productService.ProductPackName(id);
            }

            vm.ProductCategories = productCategoryService.GetProductCategorySelectModelByCompany(companyId, productType);
            vm.Units = unitService.GetUnitSelectModels(companyId);
            vm.ProductSubCategories = productSubCategoryService.GetProductSubCategorySelectModelsByProductCategory(vm.Product.ProductCategoryId);
            if (companyId == 10)
            {
                return View("CreateOrEditForKFMAL", vm);
            }
            else
            {
                return View(vm);
            }

        }

        [HttpPost]
        
        //[ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(ProductViewModel vm)
        {
            bool result = false;
            string message = string.Empty;
            if (vm.Product.ProductId <= 0)
            {
                result = productService.SaveProduct(0, vm.Product, out message);
            }
            else
            {
                result = productService.EditProduct(vm.Product.ProductId, vm.Product, out message);
            }
            if (result)
            {
                TempData["message"] = "Product Saved Successfully !";
                return RedirectToAction("Index", new { companyId = vm.Product.CompanyId, productType = vm.Product.ProductType });
            }
            TempData["message"] = message;
            return View(vm);

        }
        
        [HttpGet]
        public ActionResult FeedFormulaIndex(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            List<ProductModel> products = productService.GetFinishProducts(Convert.ToInt32(Session["CompanyId"]), searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(products.ToPagedList(No_Of_Page, Size_Of_Page));

        }

        
        [HttpGet]
        public ActionResult Details(int id, string productType)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductModel productSubCategory = productService.GetProduct(id, productType);
            if (productSubCategory == null)
            {
                return HttpNotFound();
            }
            return View(productSubCategory);
        }

        [HttpPost]
        public JsonResult GetProductSelectModelsByProductSubCategory(int productSubCategoryId)
        {
            List<SelectModel> products = productService.GetProductSelectModelsByProductSubCategory(productSubCategoryId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoComplete(string prefix, string productType)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var products = productService.GetProductAutoComplete(prefix, companyId, productType);
            return Json(products);
        }

     
        [HttpPost]
        public JsonResult AutoCompleteByCategory(string prefix, string productType)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var products = productService.GetProductAutoCompleteCatagoryWise(prefix, companyId, productType);
            return Json(products);
        }


        [HttpPost]
        public JsonResult RawMaterialAutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var rawMaterials = productService.GetProductAutoComplete(prefix, companyId);
            return Json(rawMaterials);
        }

        [HttpPost]
        public JsonResult FinishProductAutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var finishProducts = productService.GetFinishProductAutoComplete(prefix, companyId);
            return Json(finishProducts);
        }

        public ActionResult Delete(int id, string productType)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductModel product = productService.GetProduct(id, productType);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string productType, int id)
        {
            bool result = productService.DeleteProduct(id);
            if (result)
            {
                return RedirectToAction("Index", new { companyId = Session["CompanyId"], productType });
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetProductInformation(int productId)
        {
            ProductModel product = productService.GetProductInformation(productId);
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStockAvailableQuantity(int productId, int stockFrom)
        {

            decimal stockckAvailableQuantity = productService.GetStockckAvailableQuantity(productId, stockFrom);
            return Json(stockckAvailableQuantity, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult CreateProductList()
        {
            int companyId = 29;
            string productType = "F";
            string searchText = "";
            List<ProductModel> products = productService.GetProducts(companyId, productType, searchText);
            List<Product> newProductList = new List<Product>();
            int count = 77;
            foreach (var product in products)
            {
                string productCode = "F" + count.ToString().PadLeft(4, '0');
                Product newProduct = new Product()
                {
                    ProductCode = productCode,
                    ShortName = "KFL " + product.ShortName,
                    ProductCategoryId = product.ProductCategoryId,
                    ProductSubCategoryId = product.ProductSubCategoryId,
                    ProductName = "KFL " + product.ProductName,
                    UnitPrice = product.UnitPrice,
                    CreditSalePrice = product.CreditSalePrice,
                    SaleCommissionRate = product.SaleCommissionRate,
                    PurchaseRate = product.PurchaseRate,
                    PurchaseCommissionRate = product.PurchaseCommissionRate,
                    UnitId = product.UnitId,
                    PackSize = product.PackSize,
                    FormulaQty = product.FormulaQty,
                    ProcessLoss = product.ProcessLoss,
                    Remarks = product.Remarks,
                    OrderNo = product.OrderNo,
                    CreatedBy = "KG3071",
                    CreatedDate = DateTime.Now,
                    PackId = product.PackId,
                    IsActive = product.IsActive
                };

                newProductList.Add(newProduct);
                count++;
            }

            bool result = productService.SaveProductList(newProductList);
            return View();
        }

        
        [HttpGet]
        public JsonResult GetRawMaterialStockUnitPrice(int id, string strDate)
        {
            DateTime date = strDate == null ? DateTime.Now : Convert.ToDateTime(strDate);
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var unitPrice = productService.GetRawMaterialStockUnitPrice(id, date, companyId).ToString("#.##");
            return Json(unitPrice, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        
        public JsonResult GetProductPoressLoss(int productId)
        {
            decimal processLoss = productService.GetProductPoressLoss(productId);
            return Json(processLoss, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetRawmaterialByDemand(int companyId, int demandId)
        {
            List<SelectModel> rawMaterials = productService.GetRawmaterialByDemand(companyId,demandId);
            return Json(rawMaterials, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductPrice(int productId)
        {
            decimal productPrice = productService.GetProductPrice(productId);
            return Json(productPrice, JsonRequestBehavior.AllowGet);
        }

    }
}
