using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ProductCategoriesController : BaseController
    {

        private readonly IProductCategoryService productCategoryService;
        public ProductCategoriesController(IProductCategoryService productCategoryService)
        {
            this.productCategoryService = productCategoryService;
        }

        
        public ActionResult Index(int companyId, int? Page_No, string productType, string searchText)
        {
            ViewBag.Type = productType;
            searchText = searchText ?? "";
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            List<ProductCategoryModel> productCategories = productCategoryService.GetProductCategories(companyId, productType, searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(productCategories.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int id, string productType)
        {
            ProductCategoryModel model = productCategoryService.GetProductCategory(id, productType);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(ProductCategoryModel model)
        {
            if (model.ProductCategoryId <= 0)
            {
                productCategoryService.SaveProductCategory(0, model);
            }
            else
            {
                productCategoryService.SaveProductCategory(model.ProductCategoryId, model);
            }
            return RedirectToAction("Index", "ProductCategories", new { companyId = model.CompanyId, productType = model.ProductType });
        }

        public ActionResult Delete(int id, int companyId, string type)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategoryModel productCategory = productCategoryService.GetProductCategory(id, type);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string productType)
        {
            bool result = productCategoryService.DeleteProductCategory(id);
            if (result)
            {
                return RedirectToAction("Index", new { companyId = Request.QueryString["companyId"], productType });
            }
            return View();
        }

        public ActionResult Details(int id, string type)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategoryModel productCategory = productCategoryService.GetProductCategory(id, type);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

    }
}
