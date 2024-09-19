using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ProductDetailController : BaseController
    {
        private readonly IProductDetailService productDetail;
        public ProductDetailController(IProductDetailService productDetail)
        {
            this.productDetail = productDetail;
        }
        // GET: ProductDetail
        
        [HttpGet]
        public ActionResult Index(int? Page_No, string type, string searchText)
        {
            searchText = searchText ?? "";
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            var product = productDetail.GetProductDetail();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(product.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult CreateOrEdit(int id)
        {
            ProductDetailViewModel vm = new ProductDetailViewModel();
            vm.ProductDetail = productDetail.GetProductDetailById(id);
            vm.Product = productDetail.ProductDetail(vm.ProductDetail.ProductId);
            return View(vm);
        }
        
        [HttpPost]
        public ActionResult CreateOrEdit(ProductDetailViewModel model)
        {
            List<ProductDetailModel> product = model.ProductDetails;
            productDetail.SaveOrEdit(product);
            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            ProductDetailViewModel vm = new ProductDetailViewModel();
            vm.ProductDetail = productDetail.GetProductDetailById(id);
            vm.Product = productDetail.ProductDetail(vm.ProductDetail.ProductId);
            return View(vm);
        }
        
        [HttpPost]
        public ActionResult Update(ProductDetailViewModel model)
        {
            ProductDetailModel product = model.ProductDetail;
            productDetail.Update(product);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public JsonResult GetProductByLcNo(string lcNo)
        {
            var product = productDetail.ProductDetailByLcNo(lcNo);
            return Json(product, JsonRequestBehavior.AllowGet);
        }
    }
}