using KGERP.Service.Interface;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class StoreDetailController : BaseController
    {
        private readonly IStoreService storeService;
        private readonly IStoreDetailService storeDetailService;
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductSubCategoryService productSubCategoryService;
        private readonly IUnitService unitService;

        public StoreDetailController(IUnitService unitService, IStoreService storeService, IStoreDetailService storeDetailService, IProductCategoryService productCategoryService, IProductSubCategoryService productSubCategoryService)
        {
            this.storeService = storeService;
            this.storeDetailService = storeDetailService;
            this.productCategoryService = productCategoryService;
            this.productSubCategoryService = productSubCategoryService;
            this.productSubCategoryService = productSubCategoryService;
            this.unitService = unitService;


        }

        
        [HttpGet]
        public ActionResult Index(long id, long storeId, string type)
        {
            StoreDetailViewModel vm = new StoreDetailViewModel();
            ViewBag.StoreId = storeId;
            vm.Store = storeService.GetStore(storeId, type);
            //vm.StoreDetails = storeDetailService.GetStoreDetails(storeId);

            //vm.StoreDetail = storeDetailService.GetStoreDetail(id, storeId);

            vm.ProductCategories = productCategoryService.GetProductCategorySelectModelByCompany(Convert.ToInt32(Session["CompanyId"]), type);
            vm.ProductSubCategories = new List<SelectModel>();
            vm.Products = new List<SelectModel>();
            if (id > 0)
            {
                //vm.ProductSubCategories = productSubCategoryService.GetProductSubCategorySelectModelsByProductCategory(vm.StoreDetail.ProductCategoryId);
                //vm.Products = productService.GetProductSelectModelsByProductSubCategory(vm.StoreDetail.ProductSubCategoryId);
            }
            vm.Units = unitService.GetUnitSelectModels(Convert.ToInt32(Session["CompanyId"]));
            return View(vm);
        }
        //
        //[HttpGet]
        //public ActionResult CreateOrEdit(long id,long storeId)
        //{
        //    StoreDetailViewModel vm = new StoreDetailViewModel();
        //    vm.StoreDetail = storeDetailService.GetStoreDetail(id, storeId);
        //    vm.Store = storeService.GetStore(storeId);
        //    vm.ProductCategories = productCategoryService.GetProductCategorySelectModelByCompany(Convert.ToInt32(Session["CompanyId"]));
        //    vm.ProductSubCategories = new List<SelectModel>();
        //    vm.Products = new List<SelectModel>();
        //    if (id>0)
        //    {
        //        vm.ProductSubCategories = productSubCategoryService.GetProductSubCategorySelectModelsByProductCategory(vm.StoreDetail.ProductCategoryId);
        //        vm.Products = productService.GetProductSelectModelsByProductSubCategory(vm.StoreDetail.ProductSubCategoryId);
        //    }
        //    vm.Units = unitService.GetUnitSelectModels(Convert.ToInt32(Session["CompanyId"]));
        //    return View(vm);
        //}


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult CreateOrEdit(StoreDetailViewModel vm)
        //{
        //    bool result = false;
        //    string message = string.Empty;
        //    if (vm.StoreDetail.StoreDetailId <= 0)
        //    {
        //        result = storeDetailService.SaveStoreDetail(0, vm.StoreDetail,out message);
        //    }
        //    else
        //    {
        //        result = storeDetailService.SaveStoreDetail(vm.StoreDetail.StoreDetailId, vm.StoreDetail,out message);
        //    }

        //    if (!result)
        //    {
        //        TempData["message"] = message;
        //    }
        //    return RedirectToAction("Index",new { id= 0, storeId= vm.StoreDetail.StoreId });
        //}


        //
        [HttpGet]
        public ActionResult Delete(long id, long storeId)
        {
            TempData["StoreId"] = storeId;
            var storeDetail = storeDetailService.GetStoreDetail(id, storeId);
            return View(storeDetail);
        }

        
        [HttpPost]
        public ActionResult Delete(long id)
        {
            bool result = storeDetailService.DeleteStoreDetail(id);
            return RedirectToAction("Index", new { id = 0, storeId = TempData["StoreId"].ToString() });
        }

    }
}