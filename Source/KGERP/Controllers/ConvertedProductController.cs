
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ConvertedProductController : BaseController
    {
        private readonly IConvertedProductService convertedProductService;
        private readonly IProductService productService;
        public ConvertedProductController(IConvertedProductService convertedProductService, IProductService productService)
        {
            this.productService = productService;
            this.convertedProductService = convertedProductService;
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
            ConvertedProductModel convertedProductModel = new ConvertedProductModel();
            convertedProductModel = await convertedProductService.GetConvertedProducts(companyId, fromDate, toDate);
            convertedProductModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            convertedProductModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(convertedProductModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> Index(ConvertedProductModel model)
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
        public async Task<ActionResult> CreateOrEdit(int companyId, int convertedId = 0)
        {
            ConvertedProductModel model = await convertedProductService.GetConvertedProduct(companyId, convertedId); ;
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(ConvertedProductModel model)
        {
            int result = await convertedProductService.SaveConvertedProduct(model);
          
            return RedirectToAction("CreateOrEdit", new { companyId = model.CompanyId , convertedId = result});
        }


        
        [HttpGet]
        public async Task<ActionResult> ApprovalIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            ConvertedProductModel convertedProductModel = new ConvertedProductModel();
            convertedProductModel = await convertedProductService.GetConvertedProducts(companyId, fromDate, toDate);
            convertedProductModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            convertedProductModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(convertedProductModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> ApprovalIndex(ConvertedProductModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(ApprovalIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        [HttpGet]
        public JsonResult GetStockAvailableQuantity(int companyId, int productId, int stockFrom, string selectedDate)
        {
            decimal stockckAvailableQuantity = convertedProductService.GetStockckAvailableQuantity(companyId, productId, stockFrom, selectedDate);
            return Json(stockckAvailableQuantity, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        
        public async Task<ActionResult> ChangeConvertStatus(int convertedProductId, int companyId, string convertStatus)
        {
            bool result = await convertedProductService.ChangeConvertStatus(convertedProductId, companyId, convertStatus);
            return RedirectToAction("ApprovalIndex", new { companyId });
        }
    }
}