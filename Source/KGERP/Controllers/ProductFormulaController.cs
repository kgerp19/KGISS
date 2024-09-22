using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ProductFormulaController : BaseController
    {
        private readonly IProductFormulaService productFormulaService;
        private readonly IPFormulaDetailService pFormulaDetailService;

        private readonly IProductService productService;
        public ProductFormulaController(IProductFormulaService productFormulaService, IProductService productService,
            IPFormulaDetailService pFormulaDetailService)
        {
            this.productFormulaService = productFormulaService;
            this.productService = productService;
            this.pFormulaDetailService = pFormulaDetailService;
        }
       
        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }
            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            ProductFormulaModel productFormulaModel = new ProductFormulaModel();
            productFormulaModel = await productFormulaService.GetProductFormulas(companyId, fromDate, toDate);
            productFormulaModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            productFormulaModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(productFormulaModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(ProductFormulaModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }
        
        [HttpGet]
        public ActionResult CreateOrEdit(int productFormulaId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            ProductFormulaModel model = productFormulaService.GetProductFormula(productFormulaId);
            model.CompanyId = companyId;
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(ProductFormulaModel model)
        {
            string message = string.Empty;
            bool result = false;
            if (model.ProductFormulaId <= 0)
            {
                result = productFormulaService.SaveProductFormula(0, model, out message);
            }
            else
            {
                result = productFormulaService.SaveProductFormula(model.ProductFormulaId, model, out message);
            }
            TempData["message"] = message;

            if (result)
            {
                RedirectToAction("Index", new { companyId = model.CompanyId });
            }
            return View(model);
        }


        
        [HttpGet]
        public ActionResult ProuctFormulaDetails(int productFormulaId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            PFormulaDetailViewModel vm = new PFormulaDetailViewModel();
            vm.ProductFormula = productFormulaService.GetProductFormula(productFormulaId);
            vm.PFormulaDetail = pFormulaDetailService.GetFormulaDetail(0);
            vm.PFormulaDetail.ProductFormulaId = productFormulaId;
            vm.PFormulaDetails = pFormulaDetailService.GetFormulaDetails(productFormulaId);
            vm.RawMaterials = productService.GetRawMterialsSelectModel(companyId);
            return View(vm);
        }

        [HttpGet]
        
        public ActionResult CreateOrEditPFormulaDetail(int pFormulaDetailId, int productFormulaId)
        {
            PFormulaDetailViewModel vm = new PFormulaDetailViewModel();
            vm.PFormulaDetail = productFormulaService.GetPFormulaDetail(pFormulaDetailId);
            vm.PFormulaDetail.ProductFormulaId = productFormulaId;
            vm.RawMaterials = productService.GetRawMterialsSelectModel(Convert.ToInt32(Session["CompanyId"]));
            return PartialView("_productDetailCreateOrEdit", vm);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditPFormulaDetail(PFormulaDetailViewModel vm)
        {
            bool result = false;
            if (vm.PFormulaDetail.PFormulaDetailId <= 0)
            {
                result = productFormulaService.SavePFormulaDetail(0, vm.PFormulaDetail);
            }
            else
            {
                result = productFormulaService.SavePFormulaDetail(vm.PFormulaDetail.PFormulaDetailId, vm.PFormulaDetail);
            }
            if (result)
            {
                return RedirectToAction("ProuctFormulaDetails", new { productFormulaId = vm.PFormulaDetail.ProductFormulaId });
            }
            return View();
        }


        [HttpGet]
        
        public ActionResult Delete(int pFormulaDetailId)
        {
            ProductFormulaModel model = productFormulaService.GetProductFormula(pFormulaDetailId);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int pFormulaDetailId)
        {
            bool result = productFormulaService.DeleteProductFormula(pFormulaDetailId);
            if (result)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        
        public ActionResult DeletePFormulaDetail(int pFormulaDetailId)
        {
            PFormulaDetailModel formulaDetail = pFormulaDetailService.GetFormulaDetail(pFormulaDetailId);
            int productFormulaId = formulaDetail.ProductFormulaId ?? 0;
            bool result = pFormulaDetailService.DeletePFormulaDetail(pFormulaDetailId);
            if (result)
            {
                return RedirectToAction("ProuctFormulaDetails", new { productFormulaId });
            }
            return View();
        }

        
        [HttpGet]
        public ActionResult FromRequisitionDeliverCreateOrEdit(int productId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            ProductFormulaModel model = productFormulaService.GetProductFormulaUsingProductId(productId);
            model.CompanyId = companyId;
            return View("CreateOrEdit", model);
        }
    }
}