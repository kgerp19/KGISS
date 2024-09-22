using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.Procurement;
using KGERP.Utility;
using KGERP.ViewModel;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class PurchaseOrderController : BaseController
    {
        private readonly IPurchaseOrderService purchaseOrderService;
        private readonly IDemandService demandService;
        private readonly IVendorService vendorService;
        private readonly IProductCategoryService productCategoryService;
        private readonly IDistrictService districtService;
        private readonly IDropDownItemService dropDownItemService;
        private readonly ProcurementService _service;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IDemandService demandService,
            IVendorService vendorService, IDistrictService districtService,
            IProductCategoryService productCategoryService,
            IDropDownItemService dropDownItemService, ProcurementService configurationService)
        {
            this.demandService = demandService;
            this.vendorService = vendorService;
            _service = configurationService;
            this.productCategoryService = productCategoryService;
            this.purchaseOrderService = purchaseOrderService;
            this.districtService = districtService;
            this.dropDownItemService = dropDownItemService;
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
            PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();

            purchaseOrderModel = await purchaseOrderService.GetPurchaseOrders(companyId, fromDate, toDate);

            purchaseOrderModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            purchaseOrderModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(purchaseOrderModel);
        }

        [HttpPost]
       
        public async Task<ActionResult> Index(PurchaseOrderModel model)
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
        public async Task<ActionResult> CreateOrEdit(int companyId, long purchaseOrderId = 0)
        {
         
            PurchaseOrderViewModel vm = new PurchaseOrderViewModel();

            vm.RawMaterials = new List<SelectModel>();
            vm.ModeOfPurchases = dropDownItemService.GetDropDownItemSelectModels(57);
            vm.ProductOrigins = dropDownItemService.GetDropDownItemSelectModels(58);
          
            vm.PurchaseOrder = await purchaseOrderService.GetPurchaseOrder(companyId, purchaseOrderId);
           
            if (vm.PurchaseOrder != null)
            {
                vm.PurchaseOrder.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");   
            }
            vm.Countries = districtService.GetCountriesSelectModels();
            vm.PurchaseOrder.CompanyId = companyId;
            vm.Demands = demandService.GetDemandSelectModels(companyId);

            vm.TermAndConditionTypeList = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            string htmlString = "<h1 style=\"color: green;\">I am working now</h1>";


            //vm.PurchaseOrder.TermAndCondition = htmlString;
            return View(vm);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(PurchaseOrderViewModel vm)
        {
            long result = purchaseOrderService.SavePurchaseOrder(vm.PurchaseOrder.PurchaseOrderId, vm.PurchaseOrder);
            TempData["message"] = "Purchase order creation failed !";
            if (result>0)
            {
                TempData["message"] = "Purchase order created successfully !";
                return RedirectToAction(nameof(CreateOrEdit), new { companyId = vm.PurchaseOrder.CompanyId, PurchaseOrderId = result});
            }
            return View(vm);
        }

       
        [HttpGet]
        public async Task<ActionResult> CancelPurchaseOrder(int purchaseOrderId)
        {
            PurchaseOrderViewModel vm = new PurchaseOrderViewModel();
            vm.PurchaseOrder =await purchaseOrderService.GetPurchaseOrder(8, purchaseOrderId);
            return PartialView("_purchaseOrderCancel", vm);
        }

       
        [HttpPost]
        public ActionResult CancelPurchaseOrder(string actionName, PurchaseOrderViewModel vm)
        {
            vm.PurchaseOrder.PurchaseOrderStatus = actionName;
            bool result = purchaseOrderService.CancelPurchaseOrder(vm.PurchaseOrder.PurchaseOrderId, vm.PurchaseOrder);
            if (result)
            {
                TempData["message"] = "Purchase Order " + actionName + " Successfully !";
            }
            return RedirectToAction("Index", new { companyId = vm.PurchaseOrder.CompanyId });
        }

        //[SessionExpire]
        //[HttpGet]
        //public PartialViewResult GetPurchaseOrderItems(long demandId, int companyId)
        //{

        //    PurchaseOrderViewModel vm = new PurchaseOrderViewModel();
        //    vm.PurchaseOrderDetails = purchaseOrderService.GetPurchaseOrderDetails(demandId, companyId);
        //    return PartialView("_purchaseOrderItems", vm.PurchaseOrderDetails);
        //}

       
        [HttpGet]
        public JsonResult GetPurchaseOrderItemInfo(long demandId, int productId)
        {

            PurchaseOrderDetailModel model = purchaseOrderService.GetPurchaseOrderItemInfo(demandId, productId);

            var result = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }





       
        [HttpGet]
        public JsonResult GetPurchaseOrderInfo(int purchaseOrderId)
        {
            PurchaseOrderModel purchaseOrder = purchaseOrderService.GetPurchaseOrderWithInclude(purchaseOrderId);
            if (purchaseOrder.Vendor == null)
            {
                purchaseOrder.Vendor = new VendorModel();
                purchaseOrder.Vendor.Name = "Not Found";
            }
            var result = JsonConvert.SerializeObject(purchaseOrder, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(result, JsonRequestBehavior.AllowGet);

        }




        //[SessionExpire]
        //[HttpGet]
        //public ActionResult QCIndex(int companyId, int? Page_No, DateTime? searchDate, string searchText)
        //{
        //    searchText = searchText ?? "";
        //    searchDate = searchDate ?? DateTime.Now;
        //    if (companyId > 0)
        //    {
        //        Session["CompanyId"] = companyId;
        //    }
        //    var purchaseOrders = purchaseOrderService.GetQCPurchaseOrders(companyId, searchDate, searchText);
        //    int Size_Of_Page = 10;
        //    int No_Of_Page = (Page_No ?? 1);
        //    return View(purchaseOrders.ToPagedList(No_Of_Page, Size_Of_Page));

        //}

        //[SessionExpire]
        //[HttpGet]
        //public ActionResult QCEditPurchaseOrder(long purchaseOrderId)
        //{
        //    PurchaseOrderViewModel vm = new PurchaseOrderViewModel();
        //    vm.PurchaseOrder = purchaseOrderService.GetPurchaseOrder(purchaseOrderId);
        //    vm.PurchaseOrderDetails = purchaseOrderService.GetPurchaseOrderItems(purchaseOrderId);
        //    return View(vm);
        //}

        //[SessionExpire]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult QCEditPurchaseOrder(PurchaseOrderViewModel vm)
        //{
        //    vm.PurchaseOrder = purchaseOrderService.GetPurchaseOrder(vm.PurchaseOrder.PurchaseOrderId);
        //    vm.PurchaseOrder.PurchaseOrderDetails = null;
        //    vm.PurchaseOrder.PurchaseOrderDetails = vm.PurchaseOrderDetails;
        //    bool result = purchaseOrderService.SavePurchaseOrder(vm.PurchaseOrder.PurchaseOrderId, vm.PurchaseOrder);
        //    if (result)
        //    {
        //        TempData["successMessage"] = "QC Completed Successfully !";
        //    }
        //    return RedirectToAction("QCIndex", new { companyId = vm.PurchaseOrder.CompanyId });
        //}

       
        [HttpGet]
        public ActionResult DeletePurchaseOrder(long purchaseOrderId)
        {
            string message;
            bool result = purchaseOrderService.DeletePurchaseOrder(purchaseOrderId, out message);
            if (result)
            {
                TempData["message"] = "Purchase Order Deleted Successfully";
            }
            else
            {
                TempData["message"] = message;
            }
            return RedirectToAction("Index", new { companyId = Session["CompanyId"] });
        }


        [HttpPost]
        public JsonResult GetOpenedPurchaseByVendor(int vendorId)
        {
            List<SelectModel> purchaseOrderSelectModels = purchaseOrderService.GetOpenedPurchaseByVendor(vendorId);
            return Json(purchaseOrderSelectModels, JsonRequestBehavior.AllowGet);
        }



    }
}