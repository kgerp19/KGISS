using KGERP.Data.CustomModel;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class StoreController : BaseController
    {

        private readonly IStoreService storeService;
        private readonly IStockInfoService stockInfoService;
        private readonly IVendorService vendorService;
        private readonly IProductCategoryService productCategoryService;
        private readonly IPurchaseOrderService purchaseOrderService;


        public StoreController(IStoreService storeService, IStockInfoService stockInfoService, IVendorService vendorService,
            IProductCategoryService productCategoryService, IPurchaseOrderService purchaseOrderService)
        {
            this.storeService = storeService;
            this.stockInfoService = stockInfoService;
            this.vendorService = vendorService;
            this.productCategoryService = productCategoryService;
            this.purchaseOrderService = purchaseOrderService;
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
            StoreModel storeModel = new StoreModel();
            storeModel = await storeService.GetStores(companyId, fromDate, toDate, "F");
            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(storeModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(StoreModel model)
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
        public ActionResult CreateOrEdit(long id, string type)
        {
            StoreViewModel vm = new StoreViewModel();
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
            vm.Vendors = vendorService.GetVendorSelectModels((int)Provider.Supplier);
            vm.Store = storeService.GetStore(id, type);
            vm.Store.CompanyId = companyId;
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
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(StoreViewModel vm)
        {
            vm.Store.StoreDetails = vm.StoreDetails;
            if (vm.Store.StoreId <= 0)
            {
                storeService.SaveStore(0, vm.Store);
            }
            else
            {
                storeService.SaveStore(vm.Store.StoreId, vm.Store);
            }

            return RedirectToAction("Index", new { companyId = vm.Store.CompanyId, type = vm.Store.Type });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductionBulkUpdate(RequisitionModel vm)
        {
            var v = storeService.ProductionBulkUpdate(vm);
            return RedirectToAction("CreateOrEdit", "Requisition", new { companyId = vm.CompanyId, requisitionId = vm.RequisitionId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreUpdateAfterProduction(StoreViewModel vm)
        {
            bool status = false;
            string message;
            long? requisitionId = vm.RequistionItems.First().RequisitionId;
            status = storeService.StoreUpdateAfterProduction(vm.Store, vm.RequistionItems, out message);
            if (status)
            {
                TempData["message"] = "Production Issued Successfully";
            }
            else
            {
                TempData["message"] = message;
            }

            return RedirectToAction("RequisitionIssueEdit", "Requisition", new { companyId = vm.Store.CompanyId, requisitionId = requisitionId });
        }

        
        [HttpGet]
        public async Task<ActionResult> RMOpenningIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            StoreModel storeModel = new StoreModel();

            storeModel = await storeService.GetStores(companyId, fromDate, toDate, "R");

            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(storeModel);
        }



        
        [HttpGet]
        public ActionResult RMOpenningCreateOrEdit(long id)
        {
            StoreViewModel vm = new StoreViewModel();
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            vm.StockInfos = stockInfoService.GetFactorySelectModels(companyId);
            vm.Vendors = vendorService.GetVendorSelectModels((int)Provider.Supplier);
            vm.Store = storeService.GetOpenningStore(id);
            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RMOpenningCreateOrEdit(StoreViewModel vm)
        {
            vm.Store.StoreDetails = vm.StoreDetails;
            if (vm.Store.StoreId <= 0)
            {
                storeService.SaveStore(0, vm.Store);
            }
            else
            {
                storeService.SaveStore(vm.Store.StoreId, vm.Store);
            }
            return RedirectToAction("RMOpenningIndex", new { companyId = Convert.ToInt32(Session["CompanyId"]) });
        }

        
        [HttpGet]
        public async Task<ActionResult> FinishProductOpenningIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            StoreModel storeModel = new StoreModel();

            storeModel = await storeService.GetStores(companyId, fromDate, toDate, "F");

            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(storeModel);
        }


        
        [HttpGet]
        public ActionResult FinishProductOpenningCreateOrEdit(long id)
        {
            StoreViewModel vm = new StoreViewModel();
            var companyId = Convert.ToInt32(Session["Companyid"]);
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
            vm.Vendors = vendorService.GetVendorSelectModels((int)Provider.Supplier);
            vm.Store = storeService.GetOpenningStore(id);
            return View(vm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishProductOpenningCreateOrEdit(StoreViewModel vm)
        {
            vm.Store.StoreDetails = vm.StoreDetails;

            if (vm.Store.StoreId <= 0)
            {
                storeService.SaveStore(0, vm.Store);
            }
            else
            {
                storeService.SaveStore(vm.Store.StoreId, vm.Store);
            }

            return RedirectToAction("FinishProductOpenningIndex", new { companyId = vm.Store.CompanyId });
        }



        
        [HttpGet]
        public async Task<ActionResult> GetStoreProduct(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            SoreProductQty soreProductQty = new SoreProductQty();
            soreProductQty = await storeService.GetStoreProductQty(companyId);
            return View(soreProductQty);
        }


        public ActionResult GetRMStoreProduct(int? Page_No, string searchText)
        {
            searchText = searchText == null ? "" : searchText;
            List<SoreProductQty> result = storeService.GetRMStoreProductQty().Where(x => x.ProductName.ToLower().Contains(searchText.ToLower()) || x.StoreName.ToLower().Contains(searchText.ToLower())).ToList();

            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(result.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        public ActionResult GetEcomProductStore(int? Page_No, string searchText)
        {
            searchText = searchText == null ? "" : searchText;
            List<SoreProductQty> result = storeService.GetEcomProductStore().Where(x => x.ProductName.ToLower().Contains(searchText.ToLower())).ToList();

            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(result.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        
        [HttpGet]
        public async Task<ActionResult> IndexForOpenning(int companyId, DateTime? fromDate, DateTime? toDate)
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
            StoreModel storeModel = new StoreModel();

            storeModel = await storeService.GetStores(companyId, fromDate, toDate, "F");

            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(storeModel);
        }

        
        [HttpGet]
        public async Task<ActionResult> RMStoreIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            StoreModel storeModel = new StoreModel();

            storeModel = await storeService.GetStores(companyId, fromDate, toDate, "R");

            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(storeModel);
        }



        //-----------------------------------------RM Store Controller----------------




        
        [HttpGet]
        public ActionResult RMStoreCreateOrEdit(long id)
        {
            string type = "R";
            StoreViewModel vm = new StoreViewModel();
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            vm.StockInfos = stockInfoService.GetFactorySelectModels(companyId);
            vm.Vendors = vendorService.GetVendorSelectModels((int)Provider.Supplier);
            vm.PurchaseOrders = new List<SelectModel>();// purchaseOrderService.GetPurchaseOrderSelectModels(companyId);
            vm.Store = storeService.GetStore(id, type);
            vm.Store.CompanyId = companyId;
            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RMStoreCreateOrEdit(StoreViewModel vm)
        {
            vm.Store.StoreDetails = vm.StoreDetails;
            bool result = storeService.SaveRMStore(vm.Store.StoreId, vm.Store);
            if (result)
            {
                TempData["successMessage"] = "RM Received Successfully !";
            }
            return RedirectToAction("RMStoreIndex", new { companyId = vm.Store.CompanyId });
        }
        
        [HttpGet]
        public async Task<ActionResult> FeedPurchaseIndex(int companyId, DateTime? fromDate, DateTime? toDate, string productType)
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
            StoreModel storeModel = new StoreModel();
            storeModel = await storeService.GetFeedPurchaseList(companyId, fromDate, toDate, productType);
            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(storeModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> FeedPurchaseIndex(StoreModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);

            return RedirectToAction(nameof(FeedPurchaseIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate, productType = model.Type });
        }




        
        [HttpGet]
        public async Task<ActionResult> FeedPurchaseCreate(int companyId, string productType = "F", long StoreId = 0)
        {
            Session["CompanyId"] = companyId;
            StoreModel vm = new StoreModel();

            vm = await storeService.GetFeedPurchase(StoreId, productType, companyId);


            vm.CompanyId = companyId;
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);

            return View(vm);
        }

        
        [HttpPost]
        public async Task<ActionResult> FeedPurchaseCreate(StoreModel vm)
        {
            try
            {
                var result = await storeService.FeedSaveStore(0, vm);
                if (result != 0)
                {
                    return RedirectToAction(nameof(FeedPurchaseCreate), new
                    {
                        companyId = vm.CompanyId,
                        productType = "F",
                        StoreId = result
                    });
                }
            }
            catch (Exception ex)
            {
                return View(vm);
            }

            return View(vm);
        }
        [HttpPost]
        public async Task<ActionResult> SubmittedFeedPurchase(StoreModel model)
        {
            await storeService.SubmitFeedPurchaseByProduct(model);

            return RedirectToAction(nameof(FeedPurchaseCreate), new { companyId = model.CompanyId, productType = "F", StoreId = model.StoreId });
        }

    }
}