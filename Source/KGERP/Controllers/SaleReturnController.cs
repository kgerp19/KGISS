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
    public class SaleReturnController : BaseController
    {
        private readonly ISaleReturnService saleReturnService;
        private readonly IOrderDeliverService orderDelicerService;
        private readonly IStockInfoService stockInfoService;
        private readonly IProductService productService;
        public SaleReturnController(ISaleReturnService saleReturnService, IOrderDeliverService orderDelicerService,
            IStockInfoService stockInfoService, IProductService productService)
        {
            this.saleReturnService = saleReturnService;
            this.orderDelicerService = orderDelicerService;
            this.stockInfoService = stockInfoService;
            this.productService = productService;
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
                fromDate = DateTime.Now.AddMonths(-1);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            SaleReturnModel model = new SaleReturnModel();
            model = await saleReturnService.GetSaleReturns(companyId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(SaleReturnModel model)
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
        public async Task<ActionResult> Create(int companyId, string productType, int saleReturnId = 0)
        {
            Session["CompanyId"] = companyId;
            SaleReturnModel vm = new SaleReturnModel();

            if (saleReturnId == 0)
            {
                vm = saleReturnService.GetSaleReturn(0, productType);
                vm.CompanyId = companyId;
                vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
                vm.Invoices = orderDelicerService.GetInvoiceSelectList(companyId);
            }
            else
            {
                vm  =await  saleReturnService.SalesReturnSlaveGet(companyId,saleReturnId, productType);
            }
            return View(vm);
        }


        [HttpPost]
        public async Task<ActionResult> SubmittedSalesReturn(SaleReturnModel model)
        {
            await saleReturnService.SubmitSaleReturnByProduct(model);

            return RedirectToAction(nameof(Create), new { companyId = model.CompanyId, productType = model.ProductType, saleReturnId = model.SaleReturnId });
        }

        
        [HttpPost]
        public ActionResult Create(SaleReturnModel vm)
        {
            long  result = -1;
            string message = string.Empty;
            vm.SaleReturnDetails = vm.SaleReturnDetails;
            result = saleReturnService.SaveSaleReturn(vm, out message);
            TempData["message"] = message;
            if (result>0)
            {
                return RedirectToAction("Create", new { companyId = vm.CompanyId, productType = vm.ProductType, saleReturnId = result });
               // return RedirectToAction("Index", new { companyId = vm.SaleReturn.CompanyId, productType = vm.SaleReturn.ProductType });
            }

            return RedirectToAction("Create", new { companyId = vm.CompanyId, productType = vm.ProductType });
        }

        
        [HttpGet]
        public PartialViewResult GetDeliveredItem(long orderDeliverId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            SaleReturnViewModel vm = new SaleReturnViewModel();
            vm.SaleReturnDetails = saleReturnService.GetDeliveredItems(orderDeliverId, companyId);
            return PartialView("~/Views/SaleReturn/_deliveredItems.cshtml", vm.SaleReturnDetails);
        }

        
        public ActionResult OldIndex(int companyId, string productType, int? Page_No, DateTime? searchDate, string searchText)
        {
            searchText = searchText ?? "";
            searchDate = searchDate ?? DateTime.Now;
            if (companyId > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            List<SaleReturnModel> saleReturnModels = saleReturnService.GetOldSaleReturns(searchDate, searchText, companyId, productType);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(saleReturnModels.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult OldCreate(int companyId, string productType)
        {
            Session["CompanyId"] = companyId;
            SaleReturnViewModel vm = new SaleReturnViewModel();
            vm.SaleReturn = saleReturnService.GetSaleReturn(0, productType);
            vm.SaleReturn.CompanyId = companyId;
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
            vm.Invoices = orderDelicerService.GetInvoiceSelectList(companyId);
            return View(vm);
        }


        
        [HttpPost]
        public ActionResult OldCreate(SaleReturnViewModel vm)
        {
            long result = -1;
            string message = string.Empty;
            foreach (var item in vm.SaleReturnDetails)
            {
                item.COGSRate = productService.GetProductCogsPrice(vm.SaleReturn.CompanyId, item.ProductId);
            }
            vm.SaleReturn.SaleReturnDetails = vm.SaleReturnDetails;
            result = saleReturnService.SaveSaleReturn(vm.SaleReturn,out message);
            if (result>0)
            {
                return RedirectToAction("OldIndex", new { companyId = vm.SaleReturn.CompanyId, productType = vm.SaleReturn.ProductType });
            }
            return View(vm);
        }

    }
}