using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class IngredientStandardController : BaseController
    {
        private readonly IIngredientStandardService ingredientStandardService;
        private readonly IProductSubCategoryService productSubCategoryService;
        private readonly IProductService productService;
        public IngredientStandardController(IIngredientStandardService ingredientStandardService, IProductSubCategoryService productSubCategoryService, IProductService productService)
        {
            this.ingredientStandardService = ingredientStandardService;
            this.productSubCategoryService = productSubCategoryService;
            this.productService = productService;
        }
        
        public ActionResult Index(int companyId, string searchText)
        {
            searchText = searchText ?? string.Empty;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            var result = ingredientStandardService.GetIngredientStandards(companyId, searchText).GroupBy(x => x.ProductSubCategoryName);
            return View(result);
        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int ingredientStandardId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            IngredientStandardViewModel vm = new IngredientStandardViewModel();
            vm.IngredientStandard = ingredientStandardService.GetIngredientStandard(ingredientStandardId);
            vm.ProductSubCategories = productSubCategoryService.GetBasicAndAdditiveMaterialSelectModels(companyId);
            if (ingredientStandardId == 0)
            {
                vm.Products = new List<SelectModel>();
            }
            else
            {
                vm.Products = productService.GetProductbyProductSubCategorySelectModels(vm.IngredientStandard.ProductSubCategoryId);
            }
            vm.IngredientStandard.CompanyId = companyId;
            return View(vm);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(IngredientStandardViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            if (vm.IngredientStandard.IngredientStandardId <= 0)
            {
                result = ingredientStandardService.SaveIngredientStandard(0, vm.IngredientStandard, out message);
            }
            else
            {
                result = ingredientStandardService.SaveIngredientStandard(vm.IngredientStandard.IngredientStandardId, vm.IngredientStandard, out message);
            }
            if (result)
            {
                TempData["message"] = "Data Saved Successfully";
            }
            else
            {
                TempData["message"] = message;
            }

            return RedirectToAction("Index", new { companyId = vm.IngredientStandard.CompanyId });
        }


        
        [HttpGet]
        public ActionResult IngredientStandardDetails(int ingredientStandardId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            IngredientStandardViewModel vm = new IngredientStandardViewModel();
            vm.IngredientStandardDetails = ingredientStandardService.GetIngredientStandardDetails(ingredientStandardId);
            vm.IngredientStandard = ingredientStandardService.GetIngredientStandard(ingredientStandardId);
            vm.IngredientStandardDetail = ingredientStandardService.GetIngredientStandardDetail(0);
            vm.IngredientStandardDetail.IngredientStandardId = ingredientStandardId;
            return View(vm);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateIngredientStandardDetail(IngredientStandardViewModel vm)
        {
            bool result = false;
            if (vm.IngredientStandardDetail.IngredientStandardDetailId <= 0)
            {
                result = ingredientStandardService.SaveIngredientStandardDetail(0, vm.IngredientStandardDetail);
            }
            else
            {
                result = ingredientStandardService.SaveIngredientStandardDetail(vm.IngredientStandardDetail.IngredientStandardDetailId, vm.IngredientStandardDetail);
            }
            if (result)
            {
                return RedirectToAction("IngredientStandardDetails", new { ingredientStandardId = vm.IngredientStandardDetail.IngredientStandardId });
            }
            return View();
        }


        //[HttpGet]
        //
        //public ActionResult Delete(int pFormulaDetailId)
        //{
        //    ProductFormulaModel model = productFormulaService.GetProductFormula(pFormulaDetailId);
        //    return View(model);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int pFormulaDetailId)
        //{
        //    bool result = productFormulaService.DeleteProductFormula(pFormulaDetailId);
        //    if (result)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        [HttpGet]
        
        public ActionResult DeleteIngredientStandardDetail(int ingredientStandardDetailId)
        {
            IngredientStandardDetailModel ingredientStandardDetail = ingredientStandardService.GetIngredientStandardDetail(ingredientStandardDetailId);
            int ingredientStandardId = ingredientStandardDetail.IngredientStandardId;
            bool result = ingredientStandardService.DeleteIngredientStandardDetail(ingredientStandardDetailId);
            if (result)
            {
                return RedirectToAction("IngredientStandardDetails", new { ingredientStandardId });
            }
            return View();
        }
    }
}