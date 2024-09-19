using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class OrderMasterController : BaseController
    {
        private readonly IOrderMasterService orderMasterService;
        private readonly IEmployeeService employeeService;
        private readonly IVendorService vendorService;
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductSubCategoryService productSubCategoryService;
        private readonly IProductService productService;
        private readonly IStockInfoService stockInfoService;
        public OrderMasterController(IEmployeeService employeeService, IOrderMasterService orderMasterService, IVendorService vendorService,
           IProductCategoryService productCategoryService, IProductSubCategoryService productSubCategoryService,
           IProductService productService, IStockInfoService stockInfoService)
        {
            this.orderMasterService = orderMasterService;
            this.employeeService = employeeService;
            this.vendorService = vendorService;
            this.productCategoryService = productCategoryService;
            this.productSubCategoryService = productSubCategoryService;
            this.productService = productService;
            this.stockInfoService = stockInfoService;
        }

        //
        //[HttpGet]
        //public async Task<ActionResult> Index(int companyId, string productType, DateTime? fromDate, DateTime? toDate)
        //{
        //    if (companyId > 0)
        //    {
        //        Session["CompanyId"] = companyId;
        //    }
        //    if (fromDate == null)
        //    {
        //        fromDate = DateTime.Now.AddMonths(-1);
        //    }

        //    if (toDate == null)
        //    {
        //        toDate = DateTime.Now;
        //    }
        //    OrderMasterModel orderMasterModels = new OrderMasterModel();
        //    //if (string.IsNullOrEmpty(productType)) productType = "F";

        //    orderMasterModels =await orderMasterService.GetOrderMasters(companyId, fromDate, toDate, productType);

        //    orderMasterModels.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
        //    orderMasterModels.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

        //    return View(orderMasterModels);
        //}



        
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

            orderMasterModels = await orderMasterService.GetOrderMastersFeed(companyId, fromDate, toDate, productType, 0);

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
            model.CompanyId = model.CompanyId;
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            model.ProductType = model.ProductType;
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, productType = model.ProductType, fromDate = model.FromDate, toDate = model.ToDate });
        }

        
        [HttpGet]
        public async Task<ActionResult> FeedSalesOrder(int companyId, string productType, DateTime? fromDate, DateTime? toDate)
        {
            int Uid = 0;
            Uid = (int)Convert.ToInt64(Session["Id"]);
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

            orderMasterModels = await orderMasterService.GetOrderMastersFeed(companyId, fromDate, toDate, productType,Uid);

            orderMasterModels.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            orderMasterModels.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(orderMasterModels);
        }
        [HttpPost]
        
        public async Task<ActionResult> FeedSalesOrder(OrderMasterModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.CompanyId = model.CompanyId;
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            model.ProductType = model.ProductType;
            return RedirectToAction(nameof(FeedSalesOrder), new { companyId = model.CompanyId, productType = model.ProductType, fromDate = model.FromDate, toDate = model.ToDate });
        }


        
        [HttpGet]
        public async Task<ActionResult> FeedOrderCreate(int orderMasterId, string productType)
        {
            OrderMasterViewModel model = new OrderMasterViewModel();

            if (orderMasterId > 0)
            {
                model = await orderMasterService.GetFeedOrderMasterDetails(orderMasterId);
            }
            else
            {
                if (Session["Id"] != null && int.TryParse(Session["Id"].ToString(), out int Uid))
                {
                    model = orderMasterService.GetFeedOrderMaster(orderMasterId, Uid);
                    model.ProductType = productType;
                    model.MarketingOfficers = new List<SelectModel>();
                    model.OrderLocations = stockInfoService.GetStockInfoSelectModels(model.CompanyId);
                    model.SalePersonId = Uid;
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }

            return View(model);
        }



        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FeedOrderCreate(OrderMasterModel model)
        {
          
         long orderid = orderMasterService.FeedSaveSalesOrder(model.OrderMasterId, model);
            return RedirectToAction("FeedOrderCreate", new { OrderMasterId = orderid, productType = model.ProductType });


        }












        
        [HttpGet]
        public ActionResult Create(int orderMasterId, string productType)
        {
            OrderMasterModel model = new OrderMasterModel();
            model = orderMasterService.GetOrderMaster(orderMasterId);
            model.ProductType = productType;
            model.MarketingOfficers = new List<SelectModel>();
            model.OrderLocations = stockInfoService.GetStockInfoSelectModels(model.CompanyId);
            if (model.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                return View("CreateOrEditForKFMAL", model);
            }
            else
            {
                if (productType.Equals("R"))
                {
                    return View("RMCreate", model);
                }
                return View(model);
            }
        }



        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderMasterModel model)
        {
            bool status = false;
            string message;
            status = orderMasterService.SaveOrder(model.OrderMasterId, model, out message);
            if (status)
            {
                TempData["successMessage"] = "Order generated successfully.";
            }
            else
            {
                TempData["successMessage"] = message;
            }
            return RedirectToAction("Index", new { companyId = model.CompanyId, productType = model.ProductType });


        }

        
        [HttpGet]
        public ActionResult Edit(int orderMasterId, string productType)
        {
            OrderMasterModel model = new OrderMasterModel();

            model = orderMasterService.GetOrderMaster(orderMasterId);
            VendorModel customer = vendorService.GetVendor(model.CustomerId);
            model.ProductType = productType;
            model.Customer = customer.Name;
            model.CustomerAddress = customer.Address;
            model.CustomerPhone = customer.Phone;
            EmployeeModel employee = employeeService.GetEmployee(model.SalePersonId ?? 0);

            model.MarketingOfficers = new List<SelectModel>() { new SelectModel() { Text = employee.Name, Value = employee.Id } };
            model.OrderLocations = stockInfoService.GetStockInfoSelectModels(model.CompanyId);
            model.Products = productService.GetProductSelectModelsByCompanyAndProductType(model.CompanyId, productType);
            return View(model);

        }

        
        [HttpGet]
        public JsonResult GetCustomers()
        {
            List<SelectModel> customers = vendorService.GetCustomerSelectModel(Convert.ToInt32(Session["CompanyId"]), (int)Provider.Customer);
            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject(customers, Formatting.Indented, jss);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
        [HttpGet]
        public JsonResult GetProductCategories(string type)
        {
            List<SelectModel> productCategories = productCategoryService.GetProductCategorySelectModelByCompany(Convert.ToInt32(Session["CompanyId"]), type);
            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject(productCategories, Formatting.Indented, jss);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
        [HttpGet]
        public JsonResult GetProductSubCategories(int productCategoryId)
        {
            List<SelectModel> productSubCategories = productSubCategoryService.GetProductSubCategorySelectModelsByProductCategory(productCategoryId);
            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject(productSubCategories, Formatting.Indented, jss);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        
        [HttpGet]
        public JsonResult GetProducts(int productSubCategoryId)
        {
            List<SelectModel> products = productService.GetProductSelectModelsByProductSubCategory(productSubCategoryId);
            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject(products, Formatting.Indented, jss);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        
        [HttpGet]
        public JsonResult GetCustomerInfo(int id)
        {
            var custInfo = orderMasterService.GetCustomerInfo(id);
            return Json(custInfo, JsonRequestBehavior.AllowGet);
        }
        //dfdsf
        
        [HttpGet]
        public JsonResult GetProductUnitPrice(int id)
        {
            var productInfo = orderMasterService.GetProductUnitPrice(id);
            return Json(productInfo, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetProductConfig(int productId, int vendorId, int stockInfoId)
        {
            var productInfo = orderMasterService.GetProductConfig(productId, vendorId, stockInfoId);
            return Json(productInfo, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public JsonResult GetProductUnitPriceCustomerWise(int id, int vendorId)
        {
            var productInfo = orderMasterService.GetProductUnitPriceCustomerWise(id, vendorId);
            return Json(productInfo, JsonRequestBehavior.AllowGet);
        }


        
        [HttpGet]
        public JsonResult GetProductAvgPurchasePrice(int id)
        {
            var avgprice = orderMasterService.GetProductAvgPurchasePrice(id);
            return Json(avgprice, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetDepotCurrentStockByProduct(int productId, int companyId, int stockInfoId)
        {
            var avgprice = orderMasterService.GetDepotCurrentStockByProduct(productId, companyId, stockInfoId);
            return Json(avgprice, JsonRequestBehavior.AllowGet);
        }

        

        
        [HttpGet]
        public ActionResult EditOrder(long id)
        {
            var data = orderMasterService.GetOrderInforForEdit(id);
            return View(data);
        }

        
        [HttpGet]
        public JsonResult GetOrderDetails(long orderMasterId)
        {
            var orderDetails = orderMasterService.GetOrderDetailInforForEdit(orderMasterId);
            return Json(orderDetails, JsonRequestBehavior.AllowGet);
        }



        
        [HttpGet]
        public async Task<ActionResult> KgEcomIndex(int companyId, string productType, DateTime? fromDate, DateTime? toDate)
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
            OrderMasterModel orderMasterModels = new OrderMasterModel();
            productType = "F";
            orderMasterModels = await orderMasterService.GetOrderMasters(companyId, fromDate, toDate, productType);

            orderMasterModels.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            orderMasterModels.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(orderMasterModels);
          
        }



        
        [HttpGet]
        public ActionResult DeleteOrder(long orderMasterId, string productType)
        {
            bool result = orderMasterService.DeleteOrder(orderMasterId);
            if (result)
            {
                TempData["successMessage"] = "Order Deleted Successfully";
                return RedirectToAction("Index", new { companyId = Session["CompanyId"], productType });
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetNewOrderNo(int stockInfoId, string productType)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            string orderNo = orderMasterService.GetNewOrderNo(companyId, stockInfoId, productType);
            return Json(orderNo, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult DeleteOrderDetail(long orderMasterId, long orderDetailId)
        {
            string productType = string.Empty;
            bool result = orderMasterService.DeleteOrderDetail(orderDetailId, out productType);
            return RedirectToAction("Edit", new { orderMasterId, productType });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderEdit(OrderMasterModel model)
        {
            bool status = false;
            string message;
            status = orderMasterService.UpdateOrder(model, out message);
            if (status)
            {
                TempData["successMessage"] = "Order updated successfully.";
            }
            else
            {
                TempData["successMessage"] = message;
            }
            return RedirectToAction("Index", new { companyId = model.CompanyId, productType = model.ProductType });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderEditFeed(OrderMasterModel model)
        {
            bool status = false;
            string message;
            status = orderMasterService.UpdateOrder(model, out message);
            if (status)
            {
                TempData["successMessage"] = "Order updated successfully.";
            }
            else
            {
                TempData["successMessage"] = message;
            }
            return RedirectToAction("FeedSalesOrder", new { companyId = model.CompanyId, productType = model.ProductType });
        }
        public ActionResult Support(int companyId)
        {
            Session["CompanyId"] = companyId;
            return View();
        }


        public ActionResult SupportDeleteOrder(string orderNo)
        {
            TempData["successMessage"] = "Order not deleted";
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            bool result = orderMasterService.SupportDeleteOrderByOrderNo(companyId, orderNo);
            if (result)
            {
                TempData["successMessage"] = "Order deleted successfully";
            }
            return RedirectToAction("Support", new { companyId });
        }

        
        [HttpGet]
        public ActionResult ShortCreditFeed(int CompanyId)
        {
            VendorModel vendorModel = orderMasterService.GetfeedCustomer(CompanyId);
            return View(vendorModel);
        }
        
        [HttpGet]
        public JsonResult ShortCreditFeedPayment(int CustomerId)
        {
            VMFeedPayment FeedPayment = orderMasterService.GetPaymentShortList(CustomerId);
            return Json(FeedPayment, JsonRequestBehavior.AllowGet);
        }


        
        [HttpGet]
        public ActionResult ShortCreditList(int CompanyId)
        {
            VMFeedPayment vendorModel = orderMasterService.GetShortList(CompanyId);
            return View(vendorModel);
        }
    }
}