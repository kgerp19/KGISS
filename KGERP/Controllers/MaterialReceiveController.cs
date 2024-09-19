using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
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
    public class MaterialReceiveController : BaseController
    {

        private readonly IMaterialReceiveService materialReceiveService;
        private readonly IStockInfoService stockInfoService;
        private readonly IVendorService vendorService;
        private readonly IProductCategoryService productCategoryService;
        private readonly IPurchaseOrderService purchaseOrderService;
        private readonly IBagService bagService;
        private readonly WareHouseService _wareHouseService;

        public MaterialReceiveController(WareHouseService wareHouseService, IMaterialReceiveService materialReceiveService, IStockInfoService stockInfoService, IVendorService vendorService,
            IProductCategoryService productCategoryService, IPurchaseOrderService purchaseOrderService, IBagService bagService)
        {
            this._wareHouseService = wareHouseService;
            this.materialReceiveService = materialReceiveService;
            this.stockInfoService = stockInfoService;
            this.vendorService = vendorService;
            this.productCategoryService = productCategoryService;
            this.purchaseOrderService = purchaseOrderService;
            this.bagService = bagService;
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
            MaterialReceiveModel materialReceive = new MaterialReceiveModel();

            materialReceive = await materialReceiveService.GetMaterialReceivedList(companyId, fromDate, toDate);

            materialReceive.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            materialReceive.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(materialReceive);
        }
        [HttpPost]
        public async Task<ActionResult> Index(MaterialReceiveModel model)
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
        public async Task<ActionResult> SeedIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            SeedMaterialRcvViewModel model = new SeedMaterialRcvViewModel();
            model.companyId = companyId;
            model.MRlist = await materialReceiveService.GetMaterialRcvList(companyId, fromDate, toDate);

            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SeedIndex(SeedMaterialRcvViewModel model)
        {
            if (model.companyId > 0)
            {
                Session["CompanyId"] = model.companyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(SeedIndex), new { companyId = model.companyId, fromDate = model.FromDate, toDate = model.ToDate });
        }


        [HttpGet]
        public async Task<ActionResult> GCCLIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            GCCLMaterialRecieveVm model = new GCCLMaterialRecieveVm();

            model = await  materialReceiveService.GCCLMaterialRcvList(companyId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> GCCLIndex(GCCLMaterialRecieveVm model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(GCCLIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> KFMALIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            KFMALMaterialRecieveVm model = new KFMALMaterialRecieveVm();

            model = await materialReceiveService.KFMALMaterialRcvList(companyId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> KFMALIndex(GCCLMaterialRecieveVm model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(KFMALIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> PackagingMRList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            if (fromDate == null)
            {
                fromDate = firstDayOfMonth;
            }

            if (toDate == null)
            {
                toDate = lastDayOfMonth;
            }
            KFMALMaterialRecieveVm model = new KFMALMaterialRecieveVm();

            model = await materialReceiveService.PackagingMaterialRcvList(companyId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> PackagingMRList(GCCLMaterialRecieveVm model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(PackagingMRList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(int companyId, long materialReceiveId = 0)
        {
            VMWareHousePOReceivingSlave vmReceivingSlave = new VMWareHousePOReceivingSlave();
            vmReceivingSlave.StockInfos = stockInfoService.GetFactorySelectModels(companyId);
            vmReceivingSlave.PurchaseOrders = new List<SelectModel>();
            if (materialReceiveId > 0)
            {
                vmReceivingSlave = materialReceiveService.GetMaterialReceive(companyId, materialReceiveId);
            }
            else
            {
                vmReceivingSlave.CompanyId = companyId;
            }
            return View(vmReceivingSlave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrEdit(VMWareHousePOReceivingSlave vmPOReceivingSlave)
        {
            vmPOReceivingSlave.MaterialReceiveId  = await materialReceiveService.SaveMaterialReceive(vmPOReceivingSlave);


            if (vmPOReceivingSlave.MaterialReceiveId > 0)
            {
                TempData["message"] = "Raw material received successfully";
                return RedirectToAction("CreateOrEdit", new { companyId = vmPOReceivingSlave.CompanyId, materialReceiveId = vmPOReceivingSlave.MaterialReceiveId });
            }
            else
            {
                TempData["message"] = "Raw material received failed";
                return View(vmPOReceivingSlave);
            }


            //MaterialReceiveViewModel vm = new MaterialReceiveViewModel();
            //MaterialMsgModel result = new MaterialMsgModel();
            //vm.MaterialReceive.MaterialReceiveDetails = vm.MaterialReceiveDetails;
            //if (vm.MaterialReceive.MaterialReceiveId <= 0)
            //{
            //    result = materialReceiveService.SaveMaterialReceive(vmPOReceivingSlave);
            //}
            //else
            //{
            //    result = materialReceiveService.SaveMaterialReceive(vm.MaterialReceive.MaterialReceiveId, vmPOReceivingSlave);
            //}
            //if (result.Status)
            //{
            //    TempData["message"] = "Raw material received successfully";
            //    return RedirectToAction("CreateOrEdit", new { companyId = vm.MaterialReceive.CompanyId, materialReceiveId = result.Id });
            //}
            //else
            //{
            //    TempData["message"] = "Raw material received failed";
            //    return View(vm);
            //}

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult MaterialReceivePost(MaterialReceiveViewModel vm)
        //{
        //    MaterialMsgModel result = new MaterialMsgModel();
        //    vm.MaterialReceive.MaterialReceiveDetails = vm.MaterialReceiveDetails;
        //    if (vm.MaterialReceive.MaterialReceiveId <= 0)
        //    {
        //        result = materialReceiveService.SaveMaterialReceive(0, vm.MaterialReceive);
        //    }
        //    else
        //    {
        //        result = materialReceiveService.SaveMaterialReceive(vm.MaterialReceive.MaterialReceiveId, vm.MaterialReceive);
        //    }
        //    if (result.Status)
        //    {

        //        return Json(new { status = result.Status, id = result.Id, msg = "Raw material received successfully" });
        //    }
        //    else
        //    {
        //        return Json(new { status = result.Status, id = result.Id, msg = "Raw material received failed" });

        //    }

        //}



        
        [HttpGet]
        public PartialViewResult GetPurchaseOrderItems(long purchaseOrderId, int companyId)
        {
            VMWareHousePOReceivingSlave vm = new VMWareHousePOReceivingSlave()
            {
                // MaterialReceive = new MaterialReceiveModel() { CompanyId = companyId },
                BagWeights = bagService.GetBagWeightSelectModels(companyId),
                MaterialReceiveDetailModel = purchaseOrderService.GetPurchaseOrderItems(purchaseOrderId, companyId)
            };

            return PartialView("~/Views/MaterialReceive/_purchaseOrderItemList.cshtml", vm);
        }
        
        [HttpGet]
        public async Task<ActionResult> MateriaIssueIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            MaterialReceiveModel materialReceive = new MaterialReceiveModel();
            materialReceive = await materialReceiveService.GetMaterialIssuePendingList(companyId, fromDate, toDate);
            materialReceive.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            materialReceive.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(materialReceive);
        }
        [HttpPost]
        
        public async Task<ActionResult> MateriaIssueIndex(MaterialReceiveModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(MateriaIssueIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        
        [HttpGet]
        public async Task<ActionResult> MaterialIssueEdit(int companyId, long materialReceiveId)
        {
            MaterialReceiveViewModel vm = new MaterialReceiveViewModel();
            if (materialReceiveId > 0)
            {
                vm.VMReceivingSlave = await _wareHouseService.FeedWareHousePOReceivingSlaveGet(companyId, materialReceiveId);
            }
            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaterialIssueEdit(MaterialReceiveViewModel vm)
        {
            bool result = false;

            result = materialReceiveService.MaterialReceiveIssue(vm.MaterialReceive);

            return RedirectToAction(nameof(MaterialIssueEdit), new { companyId = vm.MaterialReceive.CompanyId, materialReceiveId = vm.MaterialReceive.MaterialReceiveId });
        }


        [HttpPost]
        public ActionResult FeedPOReceivingCancel(MaterialReceiveViewModel vm)
        {
            var maretialReceiveID = materialReceiveService.MaterialIssueCancel(vm.VMReceivingSlave);
            return RedirectToAction(nameof(MaterialIssueEdit), new { companyId = vm.VMReceivingSlave.CompanyFK.Value, materialReceiveId = vm.VMReceivingSlave.MaterialReceiveId });
        }
        
        [HttpGet]
        public ActionResult MaterialReceiveEdit(long id)
        {
            MaterialReceiveViewModel vm = new MaterialReceiveViewModel();
            vm.MaterialReceive = materialReceiveService.GetMaterialReceiveEdit(id);
            vm.StockInfos = stockInfoService.GetFactorySelectModels(vm.MaterialReceive.CompanyId);
            vm.BagWeights = bagService.GetBagWeightSelectModels(vm.MaterialReceive.CompanyId);
            vm.Vendors = vendorService.GetVendorSelectModels((int)Provider.Supplier);
            vm.PurchaseOrders = purchaseOrderService.GetOpenedPurchaseByVendor(vm.MaterialReceive.VendorId);
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult MaterialReceiveEdit(MaterialReceiveViewModel vm)
        {
            bool result = false;
            result = materialReceiveService.MaterialReceiveEdit(vm.MaterialReceive);
            return RedirectToAction("Index", new { companyId = vm.MaterialReceive.CompanyId });
        }

    }
}