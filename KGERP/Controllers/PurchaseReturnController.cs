using KGERP.Service.Implementation.PurchaseReturns;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
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
    public class PurchaseReturnController : BaseController
    {
        private readonly IPurchaseReturnService purchaseReturnService;
        private readonly IEmployeeService employeeService;
        private readonly IVendorService vendorService;
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductSubCategoryService productSubCategoryService;
        private readonly IProductService productService;
        private readonly IStockInfoService stockInfoService;
        private readonly PurchaseReturnservice returnservice;
        public PurchaseReturnController(IPurchaseReturnService purchaseReturnService, IEmployeeService employeeService, IVendorService vendorService,
           IProductCategoryService productCategoryService, IProductSubCategoryService productSubCategoryService,
           IProductService productService, IStockInfoService stockInfoService, PurchaseReturnservice returnservice)
        {
            this.purchaseReturnService = purchaseReturnService;
            this.employeeService = employeeService;
            this.vendorService = vendorService;
            this.productService = productService;
            this.stockInfoService = stockInfoService;
            this.returnservice = returnservice;
        }

        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, DateTime? fromDate, DateTime? toDate, string productType)
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
            PurchaseReturnModel storeModel = new PurchaseReturnModel();
            storeModel = await purchaseReturnService.GetPurchaseReturns(companyId, fromDate, toDate, productType);
            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(storeModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> Index(PurchaseReturnModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate, productType = model.ProductType });
        }


        
        [HttpGet]
        public ActionResult Create(long purchaseReturnId)
        {
            PurchaseReturnViewModel vm = new PurchaseReturnViewModel();
            vm.PurchaseReturn = purchaseReturnService.GetPurchaseReturn(purchaseReturnId);
            vm.Stocks = stockInfoService.GetFactorySelectModels(vm.PurchaseReturn.CompanyId);
            vm.ProductTypes = productService.GetProductTypeSelectModels();
            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PurchaseReturnViewModel vm)
        {
            bool status = false;
            string message;
            status = purchaseReturnService.SavePurchaseReturn(vm.PurchaseReturn.PurchaseReturnId, vm.PurchaseReturn, out message);
            if (status)
            {
                TempData["message"] = "Purchase return completed successfully.";
            }
            else
            {
                TempData["message"] = message;
            }
            return RedirectToAction("Index", new { companyId = vm.PurchaseReturn.CompanyId, productType = vm.PurchaseReturn.ProductType });
        }

        [HttpPost]
        public JsonResult GetPurchaseReturnNoByProductType(string productType)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            string purchaseReturnNo = purchaseReturnService.GetNewPurchaseReturnNo(companyId, productType);
            return Json(purchaseReturnNo, JsonRequestBehavior.AllowGet);
        }

        //
        //[HttpGet]
        //public ActionResult Edit(int orderMasterId, string productType)
        //{
        //    OrderMasterModel model = new OrderMasterModel();
        //    model = orderMasterService.GetOrderMaster(orderMasterId);
        //    VendorModel customer = vendorService.GetVendor(model.CustomerId);
        //    model.ProductType = productType;
        //    model.Customer = customer.Name;
        //    model.CustomerAddress = customer.Address;
        //    model.CustomerPhone = customer.Phone;
        //    EmployeeModel employee = employeeService.GetEmployee(model.SalePersonId ?? 0);

        //    model.MarketingOfficers = new List<SelectModel>() { new SelectModel() { Text = employee.Name, Value = employee.Id } };
        //    model.OrderLocations = stockInfoService.GetStockInfoSelectModels(model.CompanyId);
        //    model.Products = productService.GetProductSelectModelsByCompanyAndProductType(model.CompanyId, productType);
        //    if (model.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
        //    {
        //        return View("CreateOrEditForKFMAL", model);
        //    }
        //    else
        //    {
        //        if (productType.Equals("R"))
        //        {
        //            return View("RMCreate", model);
        //        }
        //        return View(model);
        //    }



        //JA.2022

        
        [HttpGet]
        public ActionResult PurchaseReturnProcess(int companyId = 0, long purchaseReturnId = 0, string message = null)
        {
            if (purchaseReturnId == 0)
            {
                PurchaseReturnnewViewModel vm = new PurchaseReturnnewViewModel();
                vm.Stocks = stockInfoService.GetFactorySelectModels(companyId);
                vm.ProductTypes = productService.GetProductTypeSelectModels();
                vm.CompanyId = companyId;
                return View(vm);
            }
            else
            {
                PurchaseReturnnewViewModel purchase = new PurchaseReturnnewViewModel();
                purchase.CompanyId = companyId;
                purchase.message = message;
                purchase.PurchaseReturnId = purchaseReturnId;
                purchase = returnservice.PurchaseReturn(purchase);
                

                return View(purchase);
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseReturnProcess(PurchaseReturnnewViewModel model)
        {
            if (model.PurchaseReturnId != 0 && model.PurchaseReturnDetailId == 0)
            {
                var res = returnservice.SaveItemPurchaseReturn(model);
                if (res.PurchaseReturnId != 0)
                {
                    return RedirectToAction("PurchaseReturnProcess", new { companyId = model.CompanyId, purchaseReturnId = res.PurchaseReturnId, message = res.message });
                }
                return RedirectToAction("PurchaseReturnProcess", new { companyId = model.CompanyId, purchaseReturnId = 0, message = res.message });
            }

            else if (model.PurchaseReturnId != 0 && model.PurchaseReturnDetailId != 0)
            {
                var res = returnservice.UpdateItemPurchaseReturn(model);
                return RedirectToAction("PurchaseReturnProcess", new { companyId = model.CompanyId, purchaseReturnId = model.PurchaseReturnId, message = res.message });
            }
            else
            {
                var res = returnservice.SavePurchaseReturn(model);
                if (res.PurchaseReturnId != 0)
                {
                    return RedirectToAction("PurchaseReturnProcess", new { companyId = model.CompanyId, purchaseReturnId = res.PurchaseReturnId, message = res.message });
                }
                return RedirectToAction("PurchaseReturnProcess", new { companyId = model.CompanyId, purchaseReturnId = 0, message = res.message });
            }
        }

        
        [HttpPost]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
        [ValidateAntiForgeryToken]
        public ActionResult EditPurchaseReturn(PurchaseReturnnewViewModel model)
        {
            var res = returnservice.UpdatePurchaseReturn(model);
            return RedirectToAction("ReturnList", new { companyId = model.CompanyId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteItem(PurchaseReturnnewViewModel model)
        {
            var res = returnservice.ItemPurchaseReturnDelete(model);
            return RedirectToAction("PurchaseReturnProcess", new { companyId = model.CompanyId, purchaseReturnId = model.PurchaseReturnId, message = res.message });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PurchaseReturnSubmited(PurchaseReturnnewViewModel model)
        {
            var purchase = await returnservice.PurchaseReturnSubmit(model);
            return RedirectToAction("PurchaseReturnProcess", new { companyId = model.CompanyId, purchaseReturnId = model.PurchaseReturnId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePurchaseReturn(PurchaseReturnnewViewModel model)
        {
            var res = returnservice.DeletePurchaseReturnitem(model);
            return RedirectToAction("ReturnList", new { companyId = model.CompanyId });
        }

        
        [HttpGet]
        public async Task<ActionResult> ReturnList(int companyId, DateTime? fromDate, DateTime? toDate)
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
            PurchaseReturnnewViewModel vm = new PurchaseReturnnewViewModel();
            vm = await returnservice.PurchaseReturnList(companyId, fromDate, toDate);
            vm.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vm.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vm.Stocks =  stockInfoService.GetFactorySelectModels(companyId);
            vm.ProductTypes = productService.GetProductTypeSelectModels();
            vm.CompanyId = companyId;
            return View(vm);
        }
        [HttpPost]
        
        public async Task<ActionResult> ReturnList(StoreModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(ReturnList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }



    }
}