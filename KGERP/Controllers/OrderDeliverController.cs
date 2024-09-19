using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
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
    public class OrderDeliverController : BaseController
    {
        private readonly IOrderDeliverService orderDeliverService;
        private readonly IOrderMasterService orderMasterService;
        private readonly IStockInfoService stockInfoService;
        private readonly IOrderDetailService orderDetailService;
        private readonly IProductService productService;
        private readonly IZoneService zoneService;

        private readonly WareHouseService wareHouseService;

        public OrderDeliverController(IZoneService zoneService, IProductService productService, IOrderDeliverService orderDeliverService, IOrderMasterService orderMasterService,
            IStockInfoService stockInfoService, IOrderDetailService orderDetailService, WareHouseService wareHouseService)
        {
            this.orderDeliverService = orderDeliverService;
            this.orderMasterService = orderMasterService;
            this.stockInfoService = stockInfoService;
            this.orderDetailService = orderDetailService;
            this.productService = productService;
            this.zoneService = zoneService;
            this.wareHouseService = wareHouseService;
        }


        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, string productType, DateTime? fromDate, DateTime? toDate)
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
            OrderMasterModel orderMasterModels = new OrderMasterModel();
            //if (string.IsNullOrEmpty(productType)) productType = "F";

            orderMasterModels = await orderMasterService.GetOrderDelivers(companyId, fromDate, toDate, productType);


            orderMasterModels.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            orderMasterModels.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(orderMasterModels);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(OrderMasterModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, productType = model.ProductType, fromDate = model.FromDate, toDate = model.ToDate });
        }
        public JsonResult UpdateSeedAttrOrderDeliver(int id, bool isSeen)
        {

            var model = orderMasterService.UpdateSeedAttrOrderDeliver(id, isSeen);
            return Json(new { OrderDeliverId = model.Item1, IsSeen = model.Item1 });
        }
        
        [HttpGet]
        public async Task<ActionResult> ChallanPrintIndex(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Now;
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }

            OrderMasterModel orderMasterModels = new OrderMasterModel();           
            orderMasterModels = await orderMasterService.GetChallanPrintOD(companyId, fromDate,toDate);


            orderMasterModels.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            orderMasterModels.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(orderMasterModels);
        }

        [HttpPost]
        
        public async Task<ActionResult> ChallanPrintIndex(OrderMasterModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(ChallanPrintIndex), new { companyId = model.CompanyId,  fromDate = model.FromDate, toDate = model.ToDate });
        }
       

        
        [HttpGet]
        public async Task<ActionResult> CreateOrUpdateOrderDeliver(int companyId, long orderMasterId, int customerId, string productType, int orderDeliverId = 0)
        {
            OrderDeliverViewModel vm = new OrderDeliverViewModel();

            if (orderDeliverId > 0)
            {
                vm.VMOrderDeliverDetail = await wareHouseService.FeedOrderDeliverDetailGet(companyId, orderDeliverId);
                vm.OrderDeliverId = orderDeliverId;
            }
            else
            {
                vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
                vm.OrderMaster = orderMasterService.GetOrderMaster(orderMasterId);
                vm.OrderDeliverId = orderDeliverId;
                vm.OrderDeliverCustomModel = new OrderDeliverCustomModel();

                vm.OrderDeliverCustomModel.IsDepoOrder = orderMasterService.CheckDepoOrder(vm.OrderMaster.OrderMasterId);
                vm.OrderDeliverCustomModel.ProductType = productType;
                vm.OrderDeliverCustomModel.CompanyId = companyId;
                vm.OrderDeliverCustomModel.InvoiceNo = vm.OrderMaster.OrderNo;
                vm.OrderDeliverCustomModel.OrderMasterId = vm.OrderMaster.OrderMasterId;
                vm.OrderDeliverCustomModel.CustomerId = customerId;
                vm.OrderDeliverCustomModel.OrderNo = vm.OrderMaster.OrderNo;
                vm.OrderDeliverCustomModel.ChallanNo = GenerateChallanNoByCompany(companyId, productType);
                vm.OrderDeliverCustomModel.OrderDate = vm.OrderMaster.OrderDate;
                vm.OrderDeliverCustomModel.Customer = vm.OrderMaster.Vendor.Name;
                vm.OrderDeliverCustomModel.CustomerAddress = vm.OrderMaster.Vendor.Address;
                vm.OrderDeliverCustomModel.CustomerContact = vm.OrderMaster.Vendor.Phone;
                vm.OrderDeliverCustomModel.CompanyId = companyId;
                vm.OrderDeliverCustomModel.DiscountAmount = vm.OrderMaster.DiscountAmount;

                vm.DeliverItems = orderDeliverService.GetDeliverItems(orderMasterId, vm.OrderMaster.StockInfoId, companyId, productType);
            }

            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                return View("KFMALCreateOrUpdateOrderDeliver", vm);
            }
            else
            {
                return View(vm);
            }

        }

        
        [HttpPost]
        public ActionResult CreateOrUpdateOrderDeliver(OrderDeliverViewModel vm)
        {
            if (vm.OrderDeliverCustomModel.ProductType.Equals("R"))
            {
                foreach (var item in vm.DeliverItems)
                {
                    if (item.StoreAvailableQty < item.ReadyToDeliver)
                    {
                        TempData["successMessage"] = "Raw materials sale operation failed. Some raw materials are not available in the stock. Please increase stock quantity";
                        return RedirectToAction("Index", new { companyId = vm.OrderDeliverCustomModel.CompanyId, productType = vm.OrderDeliverCustomModel.ProductType });
                    }
                }
            }
            OrderDeliverViewModel orderDeliverViewModel = new OrderDeliverViewModel();

            OrderDeliverModel orderDeliverModel = ProcessOrderDeliver(vm);
            orderDeliverModel = orderDeliverService.SaveOrderDeliver(0, orderDeliverModel, out string message);
            TempData["successMessage"] = message;

            return RedirectToAction(nameof(CreateOrUpdateOrderDeliver), new
            {
                companyId = vm.OrderDeliverCustomModel.CompanyId,
                orderMasterId = vm.OrderDeliverCustomModel.OrderMasterId,
                customerId = vm.OrderDeliverCustomModel.CustomerId,
                productType = vm.OrderDeliverCustomModel.ProductType,
                orderDeliverId = orderDeliverModel.OrderDeliverId
            });


            //return RedirectToAction("Index", new { companyId = orderDeliverModel.CompanyId, productType = orderDeliverModel.ProductType });
        }

        private OrderDeliverModel ProcessOrderDeliver(OrderDeliverViewModel vm)
        {
            //int noOfDiscrepencies = 0;
            OrderMasterModel orderMasterModel = orderMasterService.GetOrderMaster(vm.OrderDeliverCustomModel.OrderMasterId);

            OrderDeliverModel orderDeliverModel = new OrderDeliverModel()
            {
                ProductType = vm.OrderDeliverCustomModel.ProductType,
                CompanyId = vm.OrderDeliverCustomModel.CompanyId,
                OrderMasterId = vm.OrderDeliverCustomModel.OrderMasterId,
                StockInfoId = vm.OrderDeliverCustomModel.StockInfoId,
                ChallanNo = vm.OrderDeliverCustomModel.ChallanNo,
                VehicleNo = vm.OrderDeliverCustomModel.VehicleNo,
                InvoiceNo = vm.OrderDeliverCustomModel.InvoiceNo,
                DriverName = vm.OrderDeliverCustomModel.DriverName,
                DeliveryDate = vm.OrderDeliverCustomModel.DeliveryDate,
                TotalAmount = orderMasterModel.TotalAmount,
                Discount = orderMasterModel.DiscountAmount,
                DiscountRate = orderMasterModel.DiscountRate,
                DepoInvoiceNo = vm.OrderDeliverCustomModel.DepoInvoiceNo,
                IsActive = true

            };

            List<OrderDeliverDetailModel> orderDeliverDetailModels = new List<OrderDeliverDetailModel>();
            foreach (var item in vm.DeliverItems)
            {
                if (item.ReadyToDeliver > 0)
                {
                    //if (item.DueQty > item.ReadyToDeliver)
                    //    noOfDiscrepencies = noOfDiscrepencies + 1;

                    BusinessCostCustomModel businessCost = productService.GetCustomerBusinessCost(orderMasterModel.CustomerId, item.ProductId, orderMasterModel.StockInfoId);
                    OrderDeliverDetailModel orderDeliverDetailModel = new OrderDeliverDetailModel
                    {
                        ProductId = item.ProductId,
                        OrderQty = item.OrderQty,
                        UnitPrice = item.UnitPrice,
                        DeliveredQty = item.ReadyToDeliver,
                        Amount = Convert.ToDecimal(item.ReadyToDeliver * item.UnitPrice) - Convert.ToDecimal(item.ReadyToDeliver * item.UnitPrice) * orderDeliverModel.DiscountRate,
                        COGSPrice = item.TPPrice,
                        TPPrice = item.TradePrice,
                    // Feed
                        BaseCommission = businessCost.BaseCommission,
                        CashCommission = businessCost.CashCommission,
                        CarryingRate = businessCost.CarryingRate,
                        CreditCommission = businessCost.CreditCommission,
                        SpecialDiscount = businessCost.SpecialDiscount,
                        EBaseCommission = item.EBaseCommission,
                        ECarryingCommission = item.ECarryingCommission,
                        ECashCommission = item.ECashCommission,
                        AdditionPrice = item.AdditionPrice,
                        OrderDetailId = item.OrderDetailId,
                        IsActive = true,

                        //Glory Feed
                        SaleCommissionRate = businessCost.SaleCommissionRate,

                        //KFMAL
                        EngineNo = item.EngineNo,
                        ChassisNo = item.ChassisNo,
                        BatteryNo = item.BatteryNo,
                        RearTyreLH = item.RearTyreLH,
                        RearTyreRH = item.RearTyreRH,

                    };
                    orderDeliverDetailModels.Add(orderDeliverDetailModel);
                }

            }
            orderDeliverModel.OrderDeliverDetails = orderDeliverDetailModels;

            //orderDeliverModel.OrderStatus = noOfDiscrepencies == 0 ? "D" : "P";
            return orderDeliverModel;
        }

        
        [HttpGet]
        public ActionResult Create()
        {
            OrderDeliverViewModel vm = new OrderDeliverViewModel();
            vm.OrderDeliver = orderDeliverService.GetOrderDeliver(0);
            vm.OrderMasters = orderMasterService.GetOrderMasterSelectModels();
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult Create(OrderDeliverViewModel vm)
        {
            string message = string.Empty;
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            vm.OrderMasters = orderMasterService.GetOrderMasterSelectModels();
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
            vm.OrderDeliver = orderDeliverService.SaveOrderDeliver(0, vm.OrderDeliver, out message);
            vm.OrderDeliver = orderDeliverService.GetOrderDeliverWithInclude(vm.OrderDeliver.OrderDeliverId);

            return View(vm);
        }
        
        [HttpPost]
        public PartialViewResult CreateOrderDeliverPreview(OrderDeliverViewModel vm)
        {

            List<OrderDeliveryPreview> previews = ProcessOrderDeliverPreview(vm);
            List<OrderDeliveryPreview> previewItems = orderDeliverService.SaveOrderDeliverPreview(0, previews);
            if (vm.OrderDeliverCustomModel.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                return PartialView("_kfmalPreviewInvoice", previewItems);

            }
            else
            {
                return PartialView("_previewInvoice", previewItems);
            }

        }
        private List<OrderDeliveryPreview> ProcessOrderDeliverPreview(OrderDeliverViewModel vm)
        {
            List<OrderDeliveryPreview> previews = new List<OrderDeliveryPreview>();
            OrderMasterModel orderMasterModel = orderMasterService.GetOrderMaster(vm.OrderDeliverCustomModel.OrderMasterId);
            var stockName = stockInfoService.StocName(orderMasterModel.StockInfoId);

            foreach (var item in vm.DeliverItems)
            {
                BusinessCostCustomModel businessCost = productService.GetCustomerBusinessCost(vm.OrderDeliverCustomModel.CustomerId, item.ProductId, orderMasterModel.StockInfoId);

                //OrderDeliver
                OrderDeliveryPreview preview = new OrderDeliveryPreview();
                preview.CompanyId = vm.OrderDeliverCustomModel.CompanyId;
                preview.OrderMasterId = vm.OrderDeliverCustomModel.OrderMasterId;
                preview.OrderDate = orderMasterModel.OrderDate;
                preview.ChallanNo = vm.OrderDeliverCustomModel.ChallanNo;
                //preview.DeliveryDate = vm.OrderDeliverCustomModel.DeliveryDate; since deliveryDate is not binding with Model
                preview.DeliveryDate = vm.OrderDeliverCustomModel.DeliveryDate;
                preview.InvoiceNo = vm.OrderDeliverCustomModel.InvoiceNo;
                preview.InvoiceDate = vm.OrderDeliverCustomModel.DeliveryDate;
                preview.Party = vm.OrderDeliverCustomModel.Customer;
                preview.Address = vm.OrderDeliverCustomModel.CustomerAddress;
                preview.Phone = vm.OrderDeliverCustomModel.CustomerContact;
                preview.VehicleNo = vm.OrderDeliverCustomModel.VehicleNo;
                preview.DriverName = vm.OrderDeliverCustomModel.DriverName;
                preview.StockInfoId = orderMasterModel.StockInfoId;
                preview.StoreName = stockName;
                preview.DiscountAmount = orderMasterModel.DiscountAmount;

                //Order Deliver Detail

                preview.ProductId = item.ProductId;
                preview.ProductName = businessCost.ProductName;
                preview.OrderQty = item.OrderQty;
                preview.UnitPrice = item.UnitPrice;
                preview.ReadyToDeliver = item.ReadyToDeliver;
                preview.EBaseCommission = item.EBaseCommission;
                preview.ECarryingCommission = item.ECarryingCommission;
                preview.ECashCommission = item.ECashCommission;
                preview.SpecialDiscount = businessCost.SpecialDiscount;
                preview.AdditionPrice = businessCost.AdditionPrice;
                preview.COGSPrice = productService.GetProductCogsPrice(preview.CompanyId, preview.ProductId);
                previews.Add(preview);
            }

            return previews;
        }

        private string GenerateChallanNoByCompany(int companyId, string productType)
        {
            string newChallanNo = orderDeliverService.GetNewChallanNoByCompany(companyId, productType);
            return newChallanNo;
        }


        
        [HttpGet]
        public JsonResult GetInvoiceNo(int stockInfoId)
        {
            string invoiceNo = orderDeliverService.GetGeneratedInvoiceNoByStockInfo(stockInfoId);
            return Json(invoiceNo, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public JsonResult GetProductDetails(string engineNo)
        {
            ProductDetailModel productDetail = orderDeliverService.GetProductDetails(engineNo);
            return Json(productDetail, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult InvoiceNoAutoComplete(int customerId, string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var invoices = orderDeliverService.GetInvoiceNoAutoComplete(customerId, prefix, companyId);

            return Json(invoices);
        }


        //
        //[HttpGet]
        //public PartialViewResult GetOrderItems(long orderMasterId, int stockInfoId,int companyId)
        //{
        //    OrderDeliverViewModel vm = new OrderDeliverViewModel();
        //    vm.DeliverItems = orderDeliverService.GetDeliverItems(orderMasterId, stockInfoId,companyId);

        //    TempData["status"] = vm.DeliverItems.Any(x => x.StoreAvailableQty < 0);
        //    if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
        //    {
        //        return PartialView("_kfmalOrderItems", vm.DeliverItems);
        //    }
        //    else
        //    {
        //        return PartialView("_orderItems", vm.DeliverItems);
        //    }

        //}

        
        [HttpGet]
        public async Task<ActionResult> OrderDeliveredIndex(int companyId, string productType, DateTime? fromDate, DateTime? toDate)
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
            OrderMasterModel orderMasterModels = new OrderMasterModel();
            //if (string.IsNullOrEmpty(productType)) productType = "F";

            orderMasterModels = await orderMasterService.GetDeliveredOrderList(companyId, fromDate, toDate, productType);


            orderMasterModels.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            orderMasterModels.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(orderMasterModels);
        }
        [HttpPost]
        
        public async Task<ActionResult> OrderDeliveredIndex(OrderMasterModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(OrderDeliveredIndex), new { companyId = model.CompanyId, productType = model.ProductType, fromDate = model.FromDate, toDate = model.ToDate });
        }

    }
}