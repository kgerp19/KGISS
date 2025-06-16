using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using KG.Core.Services.Configuration;
using KGERP;
using KGERP.CustomModel;
using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.CommonModels.Model;
using KGERP.Service.Configuration;
using KGERP.Service.Contracts.KGERP.Requisitions.Command.RequestResponseModel;
using KGERP.Service.Contracts.KGERP.Requisitions.Queries.RequestModel;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.Procurement;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using KGERP.Utility.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KG.App.Controllers
{
    [CheckSession]
    public class ProcurementController : Controller
    {
        private readonly IOrderMasterService orderMasterService;
        private HttpContext httpContext;
        private readonly ProcurementService _service;
        private readonly IProductService _productService;
        private readonly MoneyReceiptService _moneyReceiptService;
        private readonly AccountingService _accountingService;
        private readonly IRequisitionService _requisitionService;
        private readonly IDropDownItemService _dropDownItemService;
        private readonly IDropdownService _dropdownService;
        private readonly ERPEntities _db = new ERPEntities();
        public ProcurementController(ProcurementService configurationService, IOrderMasterService orderMasterService, IProductService productService, MoneyReceiptService moneyReceiptService, AccountingService accountingService, IRequisitionService requisitionService, IDropDownItemService dropDownItemService, IDropdownService dropdownService)
        {
            this.orderMasterService = orderMasterService;
            _service = configurationService;
            _productService = productService;
            _moneyReceiptService = moneyReceiptService;
            _accountingService = accountingService;
            _requisitionService = requisitionService;
            _dropDownItemService = dropDownItemService;
            _dropdownService = dropdownService;
        }


        [HttpGet]
        public async Task<ActionResult> ProcurementPurchaseOrderSupplierOpening(int companyId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();
            vmPurchaseOrderSlave = await Task.Run(() => _service.ProcurementPurchaseOrderSlaveOpeningBalanceGet(companyId));
            vmPurchaseOrderSlave.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            return View(vmPurchaseOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> ProcurementPurchaseOrderSupplierOpening(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {

            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmPurchaseOrderSlave.PurchaseOrderId == 0)
                {
                    vmPurchaseOrderSlave.PurchaseOrderId = await _service.ProcurementPurchaseOrderOpeningAdd(vmPurchaseOrderSlave);

                }
                await _service.ProcurementPurchaseOrderSlaveOpeningAdd(vmPurchaseOrderSlave);
            }

            return RedirectToAction(nameof(ProcurementPurchaseOrderSupplierOpening), new { companyId = vmPurchaseOrderSlave.CompanyFK });
        }
        public JsonResult GetAutoCompleteSupplierGet(string prefix, int companyId)
        {
            var products = _service.GetAutoCompleteSupplier(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAutoCompleteDemandGet(string prefix, int companyId)
        {
            var products = _service.GetAutoCompleteDemandGet(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

         
        public JsonResult GetAutoCompleteOrderNoGet(string prefix, int companyId)
        {

            var products = _service.GetAutoCompleteOrderNoGet(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAutoCompleteStyleNo(int OrderMasterId)
        {

            var products = _service.GetAutoCompleteStyleNo(OrderMasterId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> GetDDLOrderWiseProduct(int OrderMasterId)
        {
            var products = _dropdownService.RenderDDL(await _dropDownItemService.GetDDLOrderWiseProduct(OrderMasterId), true);
            return Json(products, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> KPLRequisitonUpdate(ResuisitionRM model)
        {
            model.CreadedBy = Common.GetUserId();
            var products = await _requisitionService.KPLRequisitionUpdate(model);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteSCustomer(string prefix, int companyId)
        {
            var products = _service.GetAutoCompleteCustomer(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAutoCompleteSSubZone(string prefix, int companyId)
        {
            var products = _service.GetAutoSubZone(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteSCustomerBySz(int SubZoneId)
        {
            var products = _service.GetAutoCompleteCustomerBySz(SubZoneId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SingleDemandItem(int id)
        {
            var model = await _service.GetSingleDemandItem(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> KPLRequisitionIssuedDelete(long masterId)
        {
            RResult rResult = new RResult();
            rResult = await _requisitionService.KPLRequisitionIssuedDelete(masterId);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        #region Demand 
        //JA.2022 
        [HttpPost]
        public async Task<ActionResult> DeleteDemandItem(VmDemandItemService DemandOrderSlave)
        {
            if (DemandOrderSlave.ActionEum == ActionEnum.Delete)
            {
                DemandOrderSlave.DemandId = await _service.DemandItemDelete(DemandOrderSlave);
            }
            return RedirectToAction(nameof(DemandOrderSlave), new { companyId = DemandOrderSlave.CompanyFK, demandOrderId = DemandOrderSlave.DemandId });
        }

        [HttpGet]
        public async Task<ActionResult> DemandOrderSlave(int companyId = 0, int demandOrderId = 0)
        {
            VmDemandItemService vmModel = new VmDemandItemService();
            if (demandOrderId == 0)
            {
                vmModel.CompanyFK = companyId;
                vmModel.Status = (int)EnumPOStatus.Draft;
            }
            else if (demandOrderId > 0)
            {
                vmModel = await Task.Run(() => _service.DemandOrderSlaveGet(companyId, demandOrderId));

            }

            vmModel.ProductList = new SelectList(_productService.GetRawMterialsSelectModel(companyId), "Value", "Text");
            vmModel.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmModel.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            vmModel.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");
            return View(vmModel);
        }

        [HttpPost]
        public async Task<ActionResult> DemandOrderSlave(VmDemandItemService DemandOrderSlave)
        {
            if (DemandOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (DemandOrderSlave.DemandId == 0)
                {
                    DemandOrderSlave.DemandId = await _service.DemandhaseOrderAdd(DemandOrderSlave);
                }


                else if (DemandOrderSlave.DemandId > 0 && DemandOrderSlave.ItemQuantity > 0)
                {
                    DemandOrderSlave.DemandId = await _service.DemandItemAdd(DemandOrderSlave);
                }
                else if (DemandOrderSlave.GlobalDiscount > 0)
                {
                    await _service.DemandDiscountEdit(DemandOrderSlave);
                }

            }
            else if (DemandOrderSlave.ActionEum == ActionEnum.Edit)
            {
                await _service.DemandItemEdit(DemandOrderSlave);
            }
            return RedirectToAction(nameof(DemandOrderSlave), new { companyId = DemandOrderSlave.CompanyFK, demandOrderId = DemandOrderSlave.DemandId });
        }

        [HttpGet]
        public async Task<ActionResult> KFMALDemandOrderSlave(int companyId = 0, int demandOrderId = 0)
        {
            VmDemandItemService vmModel = new VmDemandItemService();
            if (demandOrderId == 0)
            {
                vmModel.CompanyFK = companyId;
                vmModel.Status = (int)EnumPOStatus.Draft;
            }
            else if (demandOrderId > 0)
            {
                vmModel = await Task.Run(() => _service.DemandOrderSlaveGet(companyId, demandOrderId));

            }

            vmModel.ProductList = new SelectList(_productService.GetRawMterialsSelectModel(companyId), "Value", "Text");
            vmModel.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmModel.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            vmModel.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");
            return View(vmModel);
        }

        [HttpPost]
        public async Task<ActionResult> KFMALDemandOrderSlave(VmDemandItemService DemandOrderSlave)
        {
            if (DemandOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (DemandOrderSlave.DemandId == 0)
                {
                    DemandOrderSlave.DemandId = await _service.DemandhaseOrderAdd(DemandOrderSlave);
                }


                else if (DemandOrderSlave.DemandId > 0 && DemandOrderSlave.ItemQuantity > 0)
                {
                    DemandOrderSlave.DemandId = await _service.DemandItemAdd(DemandOrderSlave);
                }
                else if (DemandOrderSlave.GlobalDiscount > 0)
                {
                    await _service.DemandDiscountEdit(DemandOrderSlave);
                }

            }
            else if (DemandOrderSlave.ActionEum == ActionEnum.Edit)
            {
                await _service.DemandItemEdit(DemandOrderSlave);
            }
            return RedirectToAction(nameof(KFMALDemandOrderSlave), new { companyId = DemandOrderSlave.CompanyFK, demandOrderId = DemandOrderSlave.DemandId });
        }
        [HttpPost]

        public async Task<ActionResult> RequisitionList(VmDemandService model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(RequisitionList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> RequisitionList(int companyId, DateTime? fromDate, DateTime? toDate)
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
            VmDemandService vmOrder = new VmDemandService();
            vmOrder = await _service.GetRequisitionList(companyId, fromDate, toDate);

            vmOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            vmOrder.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmOrder.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");

            vmOrder.CustomerList = new SelectList(_service.CustomerLisByCompany(companyId), "Value", "Text");
            vmOrder.PaymentByList = new SelectList(_service.feedPayType(companyId), "Value", "Text");
            vmOrder.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            return View(vmOrder);
        }

        [HttpPost]

        public async Task<ActionResult> KfmalRequisitionList(VmDemandService model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(KfmalRequisitionList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> KfmalRequisitionList(int companyId, DateTime? fromDate, DateTime? toDate)
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
            VmDemandService vmOrder = new VmDemandService();
            vmOrder = await _service.GetRequisitionList(companyId, fromDate, toDate);

            vmOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            vmOrder.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmOrder.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");

            vmOrder.CustomerList = new SelectList(_service.CustomerLisByCompany(companyId), "Value", "Text");
            vmOrder.PaymentByList = new SelectList(_service.feedPayType(companyId), "Value", "Text");
            vmOrder.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            return View(vmOrder);
        }
        //[HttpGet]
        //public async Task<ActionResult> RequisitionList(int companyId)
        //{
        //    VmDemandService vmOrder = new VmDemandService();
        //    vmOrder = await _service.GetRequisitionList(companyId);
        //    vmOrder.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
        //    vmOrder.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
        //    return View(vmOrder);
        //}



        [HttpPost]
        public async Task<ActionResult> UpdateDemand(VmDemandItemService DemandOrderSlave)
        {
            DemandOrderSlave.DemandId = await _service.UpdateDemand(DemandOrderSlave);
            return RedirectToAction(nameof(RequisitionList), new { companyId = DemandOrderSlave.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDemandMasters(VmDemandItemService DemandOrderSlave)
        {

            if (DemandOrderSlave.ActionEum == ActionEnum.Delete)
            {
                DemandOrderSlave.DemandId = await _service.DemandMastersDelete(DemandOrderSlave.DemandId);
            }
            return RedirectToAction(nameof(RequisitionList), new { companyId = DemandOrderSlave.CompanyFK });
        }



        [HttpPost]
        public async Task<JsonResult> GetSinglDemandMasters(int id)
        {
            var model = await _service.GetDemanMasters(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<ActionResult> DemandOrderUpdate(VmDemandItemService DemandOrderSlave)
        {
            DemandOrderSlave.DemandId = await _service.DemandhaseOrderUpdate(DemandOrderSlave);
            if (DemandOrderSlave.CompanyFK == 8)
            {
                return RedirectToAction(nameof(FeedDemandOrder), new { companyId = DemandOrderSlave.CompanyFK, demandOrderId = DemandOrderSlave.DemandId });
            }
            else
            {
                return RedirectToAction(nameof(DemandOrderSlave), new { companyId = DemandOrderSlave.CompanyFK, demandOrderId = DemandOrderSlave.DemandId });
            }

        }

        [HttpPost]
        public async Task<ActionResult> DemandupdateFeed(VmDemandItemService DemandOrderSlave)
        {
            DemandOrderSlave.DemandId = await _service.UpdateDemandfeed(DemandOrderSlave);
            return RedirectToAction(nameof(RequisitionList), new { companyId = DemandOrderSlave.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteOrderDetailFeed(int OrderDetailId)
        {
            var obj = await _service.DeletefeedorderDetail(OrderDetailId);
            return Json(obj);
        }


        [HttpPost]
        public async Task<ActionResult> DemandOrderUpdateforlist(VmDemandItemService DemandOrderSlave)
        {
            DemandOrderSlave.DemandId = await _service.DemandhaseOrderUpdate(DemandOrderSlave);
            return RedirectToAction(nameof(RequisitionList), new { companyId = DemandOrderSlave.CompanyFK });
        }

        [HttpGet]
        public async Task<ActionResult> FeedDemandOrder(int companyId = 0, int demandOrderId = 0)
        {
            VmDemandItemService vmModel = new VmDemandItemService();
            if (demandOrderId == 0)
            {
                vmModel.CompanyFK = companyId;
                vmModel.Status = (int)EnumPOStatus.Draft;
            }
            else if (demandOrderId > 0)
            {
                vmModel = await Task.Run(() => _service.DemandOrderSlaveGet(companyId, demandOrderId));

            }

            vmModel.CustomerList = new SelectList(_service.CustomerLisByCompanyFeed(companyId), "Value", "Text");
            vmModel.ProductList = new SelectList(_productService.GetRawMterialsSelectModel(companyId), "Value", "Text");
            vmModel.PaymentByList = new SelectList(_service.feedPayType(companyId), "Value", "Text");
            vmModel.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmModel.BankList = await _moneyReceiptService.BankList();
            vmModel.ReceiptDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            vmModel.ChequeDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            //vmModel.ZoneList = new SelectList(_service.ZonesDropDownList(companyId), "Value", "Text");
            //vmModel.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");
            return View(vmModel);
        }
        [HttpPost]
        public async Task<ActionResult> FeedDemandOrder(VmDemandItemService DemandOrderSlave)
        {
            if (DemandOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (DemandOrderSlave.DemandId == 0)
                {

                    DemandOrderSlave.DemandId = await _service.DemandhaseOrderAdd(DemandOrderSlave);
                }


                else if (DemandOrderSlave.DemandId > 0 && DemandOrderSlave.ItemQuantity > 0)
                {
                    DemandOrderSlave.DemandId = await _service.DemandItemAdd(DemandOrderSlave);
                }
                else if (DemandOrderSlave.GlobalDiscount > 0)
                {
                    await _service.DemandDiscountEdit(DemandOrderSlave);
                }

            }
            else if (DemandOrderSlave.ActionEum == ActionEnum.Edit)
            {
                await _service.DemandItemEdit(DemandOrderSlave);
            }
            return RedirectToAction(nameof(FeedDemandOrder), new { companyId = DemandOrderSlave.CompanyFK, demandOrderId = DemandOrderSlave.DemandId });
        }

        [HttpPost]
        public async Task<ActionResult> DemandOrderUpdatediscount(VmDemandItemService DemandOrderSlave)
        {
            await _service.DemandDiscountEdit(DemandOrderSlave);
            return RedirectToAction(nameof(FeedDemandOrder), new { companyId = DemandOrderSlave.CompanyFK, demandOrderId = DemandOrderSlave.DemandId });
        }

        #endregion


        #region Purchase Order
        [HttpGet]
        public async Task<ActionResult> ProcurementPurchaseOrderSlave(int companyId = 0, int purchaseOrderId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();

            if (purchaseOrderId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else if (purchaseOrderId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.ProcurementPurchaseOrderSlaveGet(companyId, purchaseOrderId));

            }
            vmPurchaseOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");
            if (companyId == (int)CompanyName.KrishibidSeedLimited)
            {
                vmPurchaseOrderSlave.LCList = new SelectList(_service.SeedLCHeadGLList(companyId), "Value", "Text");

            }
            return View(vmPurchaseOrderSlave);
        }


        [HttpPost]
        public async Task<ActionResult> ProcurementPurchaseOrderSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {

            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmPurchaseOrderSlave.PurchaseOrderId == 0)
                {
                    vmPurchaseOrderSlave.PurchaseOrderId = await _service.ProcurementPurchaseOrderAdd(vmPurchaseOrderSlave);

                }
                else
                {
                    await _service.ProcurementPurchaseOrderSlaveAdd(vmPurchaseOrderSlave);

                }
            }
            else if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.ProcurementPurchaseOrderSlaveEdit(vmPurchaseOrderSlave);
            }
            return RedirectToAction(nameof(ProcurementPurchaseOrderSlave), new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProcurementPurchaseOrderSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmPurchaseOrderSlave.PurchaseOrderId = await _service.ProcurementPurchaseOrderSlaveDelete(vmPurchaseOrderSlave.PurchaseOrderDetailId);
            }
            return RedirectToAction(nameof(ProcurementPurchaseOrderSlave), new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }

        

        public JsonResult GetTermNCondition(int id)
        {
            if (id != 0)
            {
                var list = _service.POTremsAndConditionsGet(id);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetOrderMasterPayableValue(int companyId, int orderMasterId)
        {
            if (orderMasterId > 0)
            {
                var list = _service.OrderMasterPayableValueGet(companyId, orderMasterId);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetPurchaseOrderPayableValue(int companyId, int purchaseOrderId)
        {
            if (purchaseOrderId > 0)
            {
                var list = _service.PurchaseOrderPayableValueGet(companyId, purchaseOrderId);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public async Task<JsonResult> SingleProcurementPurchaseOrderSlave(int id)
        {
            var model = await _service.GetSingleProcurementPurchaseOrderSlave(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetProductCategory()
        {
            var model = await Task.Run(() => _service.ProductCategoryGet());
            var list = model.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list);
        }


        public async Task<JsonResult> SingleProcurementPurchaseOrder(int id)
        {
            VMPurchaseOrder model = new VMPurchaseOrder();
            model = await _service.GetSingleProcurementPurchaseOrder(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //public async Task<ActionResult> ProcurementPurchaseOrderGet(int id)
        //{
        //    //if (id < 0) { return RedirectToAction("Error", "Home"); }

        //    var model = await Task.Run(() => _service.ProcurementPurchaseOrderGet(id));
        //    var list = model.Select(x => new { Value = x.ID, Text = x.CID }).ToList();
        //    return Json(list);
        //}



        [HttpGet]
        public async Task<ActionResult> ProcurementPurchaseOrderList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2); ;

            if (!toDate.HasValue) toDate = DateTime.Now;


            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await _service.ProcurementPurchaseOrderListGet(companyId, fromDate, toDate, vStatus);

            vmPurchaseOrder.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrder.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrder.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");

            vmPurchaseOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.Status = vStatus ?? -1;
            vmPurchaseOrder.UserId = System.Web.HttpContext.Current.User.Identity.Name;
            return View(vmPurchaseOrder);
        }


        [HttpPost]

        public async Task<ActionResult> ProcurementPurchaseOrderList(VMPurchaseOrder vmPurchaseOrder)
        {
            if (vmPurchaseOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmPurchaseOrder.CompanyId;
            }
            vmPurchaseOrder.FromDate = Convert.ToDateTime(vmPurchaseOrder.StrFromDate);
            vmPurchaseOrder.ToDate = Convert.ToDateTime(vmPurchaseOrder.StrToDate);

            return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyId, fromDate = vmPurchaseOrder.FromDate, toDate = vmPurchaseOrder.ToDate, vStatus = vmPurchaseOrder.Status });
        }
        //[HttpPost]
        //public async Task<ActionResult> ProcurementPurchaseOrderList(VMPurchaseOrder vmPurchaseOrder)
        //{
        //    if (vmPurchaseOrder.ActionEum == ActionEnum.Edit)
        //    {
        //        await _service.ProcurementPurchaseOrderEdit(vmPurchaseOrder);
        //    }
        //    return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        //}
        [HttpGet]
        public async Task<ActionResult> KFMALProcurementPurchaseOrderList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2); ;

            if (!toDate.HasValue) toDate = DateTime.Now;


            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await _service.KFMALProcurementPurchaseOrderListGet(companyId, fromDate, toDate, vStatus);

            vmPurchaseOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.Status = vStatus ?? -1;
            vmPurchaseOrder.UserId = System.Web.HttpContext.Current.User.Identity.Name;
            return View(vmPurchaseOrder);
        }
        [HttpPost]

        public async Task<ActionResult> KFMALProcurementPurchaseOrderList(VMPurchaseOrder vmPurchaseOrder)
        {
            if (vmPurchaseOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmPurchaseOrder.CompanyId;
            }
            vmPurchaseOrder.FromDate = Convert.ToDateTime(vmPurchaseOrder.StrFromDate);
            vmPurchaseOrder.ToDate = Convert.ToDateTime(vmPurchaseOrder.StrToDate);

            return RedirectToAction(nameof(KFMALProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyId, fromDate = vmPurchaseOrder.FromDate, toDate = vmPurchaseOrder.ToDate, vStatus = vmPurchaseOrder.Status });
        }



        [HttpGet]
        public async Task<ActionResult> PackagingPurchaseOrderList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);


            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await _service.PackagingPurchaseOrderListGet(companyId, fromDate, toDate, vStatus);

            vmPurchaseOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.Status = vStatus ?? -1;
            vmPurchaseOrder.UserId = System.Web.HttpContext.Current.User.Identity.Name;
            return View(vmPurchaseOrder);
        }
        [HttpPost]

        public async Task<ActionResult> PackagingPurchaseOrderList(VMPurchaseOrder vmPurchaseOrder)
        {
            if (vmPurchaseOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmPurchaseOrder.CompanyId;
            }
            vmPurchaseOrder.FromDate = Convert.ToDateTime(vmPurchaseOrder.StrFromDate);
            vmPurchaseOrder.ToDate = Convert.ToDateTime(vmPurchaseOrder.StrToDate);

            return RedirectToAction(nameof(PackagingPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyId, fromDate = vmPurchaseOrder.FromDate, toDate = vmPurchaseOrder.ToDate, vStatus = vmPurchaseOrder.Status });
        }










        [HttpGet]
        public async Task<ActionResult> ProcurementCancelPurchaseOrderList(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await _service.ProcurementCancelPurchaseOrderListGet(companyId);
            return View(vmPurchaseOrder);
        }
        [HttpGet]
        public async Task<ActionResult> ProcurementHoldPurchaseOrderList(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await _service.ProcurementHoldPurchaseOrderListGet(companyId);

            return View(vmPurchaseOrder);
        }

        [HttpGet]
        public async Task<ActionResult> ProcurementClosedPurchaseOrderList(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await _service.ProcurementClosedPurchaseOrderListGet(companyId);

            return View(vmPurchaseOrder);
        }
        [HttpGet]
        public async Task<ActionResult> ProcurementPurchaseOrderReport(int companyId = 0, int purchaseOrderId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();
            if (purchaseOrderId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.ProcurementPurchaseOrderSlaveGet(companyId, purchaseOrderId));
                vmPurchaseOrderSlave.TotalPriceInWord = VmCommonCurrency.NumberToWords(Convert.ToDecimal(vmPurchaseOrderSlave.DataListSlave.Select(x => x.PurchaseQuantity * x.PurchasingPrice).DefaultIfEmpty(0).Sum()) + vmPurchaseOrderSlave.FreightCharge + vmPurchaseOrderSlave.OtherCharge, CurrencyType.BDT);


            }
            return View(vmPurchaseOrderSlave);
        }
        //[HttpPost]
        //public async Task<ActionResult> FeedSubmitOrderMastersFromSlave(FeedSalesOrderModel model)
        //{
        //    model.OrderMasterId = await _service.FeedSalesOrderSubmit(model.OrderMasterId);
        //    return RedirectToAction(nameof(FeedProcurementSalesOrderSlave), new {  companyId = model.CompanyId,  productType = model.ProductType,  orderMasterId = model.OrderMasterId});
        //}


        #region PO Submit HoldUnHold CancelRenew  ClosedReopen Delete
        [HttpPost]
        public async Task<ActionResult> PackagingSubmitPO(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            vmPurchaseOrderSlave.PurchaseOrderId = await _service.PackagingPurchaseOrderSubmit(vmPurchaseOrderSlave);
            return RedirectToAction(nameof(PackagingPurchaseOrderSlave), "Procurement", new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }

        [HttpPost]
        public async Task<ActionResult> SubmitProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        {
            vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderSubmit(vmPurchaseOrder.PurchaseOrderId);
            return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> SubmitProcurementPurchaseOrderFromSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            vmPurchaseOrderSlave.PurchaseOrderId = await _service.ProcurementPurchaseOrderSubmit(vmPurchaseOrderSlave.PurchaseOrderId);
            return RedirectToAction(nameof(ProcurementPurchaseOrderSlave), "Procurement", new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }
        [HttpPost]
        public async Task<ActionResult> HoldUnHoldProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        {
            vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderHoldUnHold(vmPurchaseOrder.PurchaseOrderId);
            return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> CancelRenewProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        {
            vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderCancelRenew(vmPurchaseOrder.PurchaseOrderId);
            return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> ClosedReopenProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        {
            vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderClosedReopen(vmPurchaseOrder.PurchaseOrderId);
            return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> DeleteProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        {
            if (vmPurchaseOrder.ActionEum == ActionEnum.Delete)
            {
                //Delete
                //vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderDelete(vmPurchaseOrder.PurchaseOrderId);
                vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderDeleteProcess(vmPurchaseOrder.PurchaseOrderId);
            }
            return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        }
        //kkk

        #endregion



        #endregion
        [HttpPost]
        public async Task<ActionResult> PackagingSubmitPOList(VMPurchaseOrder vmPurchaseOrder)
        {
            vmPurchaseOrder.PurchaseOrderId = await _service.KFMALProcurementPurchaseOrderSubmit(vmPurchaseOrder.PurchaseOrderId);
            return RedirectToAction(nameof(PackagingPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        }

        public async Task<JsonResult> KFMALSingleProcurementPurchaseOrderSlave(int id)
        {
            var model = await _service.KFMALGetSingleProcurementPurchaseOrderSlave(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> KFMALSubmitProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        {
            vmPurchaseOrder.PurchaseOrderId = await _service.KFMALProcurementPurchaseOrderSubmit(vmPurchaseOrder.PurchaseOrderId);
            return RedirectToAction(nameof(KFMALProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> KFMALSubmitProcurementPurchaseOrderFromSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            vmPurchaseOrderSlave.PurchaseOrderId = await _service.KFMALProcurementPurchaseOrderSubmit(vmPurchaseOrderSlave.PurchaseOrderId);
            return RedirectToAction(nameof(KFMALProcurementPurchaseOrderSlave), "Procurement", new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }
        #region Sales  Order
        [HttpGet]
        public async Task<ActionResult> ProcurementSalesOrderSlaveCustomerOpening(int companyId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();

            vmSalesOrderSlave = await Task.Run(() => _service.ProcurementSalesOrderOpeningDetailsGet(companyId));

            return View(vmSalesOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> ProcurementSalesOrderSlaveCustomerOpening(VMSalesOrderSlave vmSalesOrderSlave)
        {

            if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmSalesOrderSlave.OrderMasterId == 0)
                {
                    vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterOpeningAdd(vmSalesOrderSlave);

                }
                await _service.OrderDetailOpeningAdd(vmSalesOrderSlave);
            }

            return RedirectToAction(nameof(ProcurementSalesOrderSlaveCustomerOpening), new { companyId = vmSalesOrderSlave.CompanyFK });
        }



        [HttpGet]
        public async Task<ActionResult> ProcurementSalesOrderDetailsReport(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            if (orderMasterId > 0)
            {
                vmSalesOrderSlave = await Task.Run(() => _service.ProcurementSalesOrderDetailsGet(companyId, orderMasterId));
                vmSalesOrderSlave.TotalPriceInWord = VmCommonCurrency.NumberToWords(Convert.ToDecimal(vmSalesOrderSlave.DataListSlave.Select(x => x.TotalAmount).DefaultIfEmpty(0).Sum()), CurrencyType.BDT);

            }
            return View(vmSalesOrderSlave);
        }

        public async Task<ActionResult> CustomerLisBySubZonetGet(int subZoneId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.CustomerLisBySubZonetGet(subZoneId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.ID, Text = x.Name}).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> OrderDelivieryListByOrderMaster(long orderMasterId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.OrderDelivieryListByOrderMaster(orderMasterId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.OrderDeliverId, Text = x.ChallanNo }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> CustomerLisByZonetGet(int zoneId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.CustomerLisByZonetGet(zoneId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SalesOrderLisByCustomerIdGet(int customerId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.SalesOrderLisByCustomerIdGet(customerId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.OrderMasterId, Text = x.OrderNo }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> ProcurementSalesOrderSlave(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();

            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else
            {
                vmSalesOrderSlave = await Task.Run(() => _service.ProcurementSalesOrderDetailsGet(companyId, orderMasterId));

            }
            vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");


            return View(vmSalesOrderSlave);
        }

        [HttpGet]
        public async Task<ActionResult> ProcurementRMSalesOrderSlave(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();

            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else
            {
                vmSalesOrderSlave = await Task.Run(() => _service.ProcurementSalesOrderDetailsGet(companyId, orderMasterId));

            }
            vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");


            return View(vmSalesOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> ProcurementSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {

            if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmSalesOrderSlave.OrderMasterId == 0)
                {
                    vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterAdd(vmSalesOrderSlave);

                }
                await _service.OrderDetailAdd(vmSalesOrderSlave);
            }
            if (vmSalesOrderSlave.PromotionalOfferId > 0)
            {
                await _service.PromtionalOfferIntegration(vmSalesOrderSlave);
            }
            else if (vmSalesOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.OrderDetailEdit(vmSalesOrderSlave);
            }
            return RedirectToAction(nameof(ProcurementSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> ProcurementRMSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {

            if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmSalesOrderSlave.OrderMasterId == 0)
                {
                    vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterRawAdd(vmSalesOrderSlave);

                }
                await _service.OrderDetailRawAdd(vmSalesOrderSlave);
            }
            if (vmSalesOrderSlave.PromotionalOfferId > 0)
            {
                await _service.PromtionalOfferIntegration(vmSalesOrderSlave);
            }
            else if (vmSalesOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.OrderDetailEdit(vmSalesOrderSlave);
            }
            return RedirectToAction(nameof(ProcurementRMSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProcurementSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrderSlave.OrderMasterId = await _service.ProcurementPurchaseOrderSlaveDelete(vmSalesOrderSlave.OrderMasterId);
            }
            return RedirectToAction(nameof(ProcurementSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        [HttpGet]
        public async Task<ActionResult> ProcurementSalesOrderList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder = await _service.ProcurementOrderMastersListGet(companyId, fromDate, toDate, vStatus);
            vmSalesOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.Status = vStatus ?? -1;
            return View(vmSalesOrder);
        }

        [HttpGet]
        public async Task<ActionResult> ProcurementRMSalesOrderList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder = await _service.ProcurementOrderMastersRMListGet(companyId, fromDate, toDate, vStatus);
            vmSalesOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.Status = vStatus ?? -1;
            return View(vmSalesOrder);
        }

        [HttpPost]
        public async Task<ActionResult> ProcurementSalesOrderList(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.ActionEum == ActionEnum.Edit)
            {
                await _service.OrderMastersEdit(vmSalesOrder);
            }
            return RedirectToAction(nameof(ProcurementSalesOrderList), new { companyId = vmSalesOrder.CompanyFK });
        }

        [HttpGet]
        public async Task<ActionResult> KFMALSalesOrderList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);



            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder = await _service.KFMALProcurementOrderMastersListGet(companyId, fromDate, toDate, vStatus);
            vmSalesOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.Status = vStatus ?? -1;
            return View(vmSalesOrder);
        }


        [HttpPost]
        public async Task<ActionResult> KFMALSalesOrderList(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.ActionEum == ActionEnum.Edit)
            {
                await _service.OrderMastersEdit(vmSalesOrder);
            }
            return RedirectToAction(nameof(KFMALSalesOrderList), new { companyId = vmSalesOrder.CompanyFK });
        }


        [HttpPost]
        public async Task<ActionResult> ProcurementSalesOrderfilter(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmSalesOrder.CompanyId;
            }

            vmSalesOrder.FromDate = Convert.ToDateTime(vmSalesOrder.StrFromDate);
            vmSalesOrder.ToDate = Convert.ToDateTime(vmSalesOrder.StrToDate);
            return RedirectToAction(nameof(ProcurementSalesOrderList), new { companyId = vmSalesOrder.CompanyId, fromDate = vmSalesOrder.FromDate, toDate = vmSalesOrder.ToDate, vStatus = vmSalesOrder.Status });
        }

        [HttpPost]
        public async Task<ActionResult> ProcurementRMSalesOrderfilter(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmSalesOrder.CompanyId;
            }

            vmSalesOrder.FromDate = Convert.ToDateTime(vmSalesOrder.StrFromDate);
            vmSalesOrder.ToDate = Convert.ToDateTime(vmSalesOrder.StrToDate);
            return RedirectToAction(nameof(ProcurementRMSalesOrderList), new { companyId = vmSalesOrder.CompanyId, fromDate = vmSalesOrder.FromDate, toDate = vmSalesOrder.ToDate, vStatus = vmSalesOrder.Status });
        }

        [HttpPost]
        public async Task<ActionResult> KfmalSalesOrderfilter(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmSalesOrder.CompanyId;
            }

            vmSalesOrder.FromDate = Convert.ToDateTime(vmSalesOrder.StrFromDate);
            vmSalesOrder.ToDate = Convert.ToDateTime(vmSalesOrder.StrToDate);
            return RedirectToAction(nameof(KFMALSalesOrderList), new { companyId = vmSalesOrder.CompanyId, fromDate = vmSalesOrder.FromDate, toDate = vmSalesOrder.ToDate, vStatus = vmSalesOrder.Status });
        }

        [HttpPost]
        public async Task<ActionResult> KfmalServiceSalesOrderfilter(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmSalesOrder.CompanyId;
            }

            vmSalesOrder.FromDate = Convert.ToDateTime(vmSalesOrder.StrFromDate);
            vmSalesOrder.ToDate = Convert.ToDateTime(vmSalesOrder.StrToDate);
            return RedirectToAction(nameof(KFMALSalesOrderServiceList), new { companyId = vmSalesOrder.CompanyId, fromDate = vmSalesOrder.FromDate, toDate = vmSalesOrder.ToDate, vStatus = vmSalesOrder.Status });
        }
        [HttpPost]
        public async Task<ActionResult> PackagingSalesOrderfilter(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmSalesOrder.CompanyId;
            }

            vmSalesOrder.FromDate = Convert.ToDateTime(vmSalesOrder.StrFromDate);
            vmSalesOrder.ToDate = Convert.ToDateTime(vmSalesOrder.StrToDate);
            return RedirectToAction(nameof(PackagingSalesOrderList), new { companyId = vmSalesOrder.CompanyId, fromDate = vmSalesOrder.FromDate, toDate = vmSalesOrder.ToDate, vStatus = vmSalesOrder.Status });
        }


        public async Task<JsonResult> SingleOrderDetails(int id)
        {
            var model = await _service.GetSingleOrderDetails(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> KPLSingleOrderDetails(int id)
        {
            var model = await _service.GetKPLSingleOrderDetails(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> KfmalOrderDetails(int id)
        {
            var model = await _service.GetKfmalOrderDetails(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CustomerRecevableAmountByCustomer(int companyId, int customerId)
        {
            var model = await _service.CustomerRecevableAmountByCustomerGet(companyId, customerId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> FeedCustomerDetailsById(int customerId)
        {
            var model = await _service.FeedCustomerDetailsById(customerId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProductStockByProduct(int companyId, int productId)
        {
            var model = _service.ProductStockByProductGet(companyId, productId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductStockByProductDeliver(int companyId, int productId,string Lotnumber)
        {
            var model = _service.ProductStockByProductGetOrderDeliver(companyId, productId, Lotnumber);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RMProductStockByProductGet(int companyId, int productId)
        {
            var model = _service.RMProductStockByProductGet(companyId, productId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        //public async Task<ActionResult> GetProductCategory()
        //{
        //    var model = await Task.Run(() => _service.ProductCategoryGet());
        //    var list = model.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
        //    return Json(list);
        //}

        [HttpGet]
        public async Task<ActionResult> GetSinglOrderMastersGet(int orderMasterId)
        {
            var model = await _service.GetSinglOrderMasters(orderMasterId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetOrderDetails(int orderDetailsId)
        {

            var model = await _service.GetDetailsBOM(orderDetailsId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }






        //#region PO Submit HoldUnHold CancelRenew  ClosedReopen Delete
        [HttpPost]
        public async Task<ActionResult> SubmitOrderMasters(VMSalesOrder vmSalesOrder)
        {
            vmSalesOrder.OrderMasterId = await _service.OrderMastersSubmit(vmSalesOrder.OrderMasterId);
            return RedirectToAction(nameof(ProcurementSalesOrderList), new { companyId = vmSalesOrder.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> SubmitOrderMastersFromSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            vmSalesOrderSlave.OrderMasterId = await _service.OrderMastersSubmit(vmSalesOrderSlave.OrderMasterId);
            return RedirectToAction(nameof(ProcurementSalesOrderSlave), "Procurement", new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> SubmitRMOrderMastersFromSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            vmSalesOrderSlave.OrderMasterId = await _service.OrderMastersSubmit(vmSalesOrderSlave.OrderMasterId);
            return RedirectToAction(nameof(ProcurementRMSalesOrderSlave), "Procurement", new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }
        [HttpPost]
        public async Task<ActionResult> GCCLSubmitOrderMastersFromSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            vmSalesOrderSlave.OrderMasterId = await _service.OrderMastersSubmit(vmSalesOrderSlave.OrderMasterId);

            return RedirectToAction(nameof(GCCLProcurementSalesOrderSlave), "Procurement", new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }
        public async Task<ActionResult> PAckegingSubmitOrderMastersFromSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            vmSalesOrderSlave.OrderMasterId = await _service.OrderMastersSubmit(vmSalesOrderSlave.OrderMasterId);

            return RedirectToAction(nameof(PackagingSalesOrderSlave), "Procurement", new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> FeedSubmitOrderMastersFeomEdit(OrderMasterModel orderMasterModel)
        {
            var i = await _service.OrderMastersSubmit(orderMasterModel.OrderMasterId);

            return RedirectToAction("Edit", "OrderMaster", new { orderMasterId = orderMasterModel.OrderMasterId, productType = orderMasterModel.ProductType });
        }
        [HttpPost]
        public async Task<ActionResult> KfmalSubmitOrderMaster(VMSalesOrderSlave vmSalesOrderSlave)
        {
            vmSalesOrderSlave.OrderMasterId = await _service.OrderMastersSubmit(vmSalesOrderSlave.OrderMasterId);

            return RedirectToAction(nameof(KFMALSalesOrderSlave), "Procurement", new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }
        [HttpPost]
        public async Task<ActionResult> SubmitRMConsumption(VMFinishProductBOM vmFinishProductBOM)
        {
            vmFinishProductBOM.OrderDetailId = await _service.OrderDetaisSubmit(vmFinishProductBOM.OrderDetailId);
            return RedirectToAction(nameof(PackagingSalesOrderBOM), new { companyId = vmFinishProductBOM.CompanyFK, orderDetailId = vmFinishProductBOM.OrderDetailId });
        }
        [HttpPost]
        public async Task<ActionResult> DeleteSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrderSlave.OrderDetailId = await _service.OrderDetailDelete(vmSalesOrderSlave.OrderDetailId);
            }
            return RedirectToAction(nameof(ProcurementSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }
        [HttpPost]
        public async Task<ActionResult> FeedDeleteSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrderSlave.OrderDetailId = await _service.OrderDetailDelete(vmSalesOrderSlave.OrderDetailId);
            }
            return RedirectToAction(nameof(FeedProcurementSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyId, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> PackagingOrderDetailDelete(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrderSlave.OrderDetailId = await _service.OrderDetailDelete(vmSalesOrderSlave.OrderDetailId);
            }
            return RedirectToAction(nameof(PackagingSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }






        [HttpPost]
        public async Task<ActionResult> KfmalDeleteSalesOrder(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrderSlave.OrderDetailId = await _service.OrderDetailDelete(vmSalesOrderSlave.OrderDetailId);
            }
            return RedirectToAction(nameof(KFMALSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }
        //[HttpPost]
        //public async Task<ActionResult> HoldUnHoldProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        //{
        //    vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderHoldUnHold(vmPurchaseOrder.PurchaseOrderId);
        //    return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        //}

        //[HttpPost]
        //public async Task<ActionResult> CancelRenewProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        //{
        //    vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderCancelRenew(vmPurchaseOrder.PurchaseOrderId);
        //    return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        //}
        //[HttpPost]
        //public async Task<ActionResult> ClosedReopenProcurementPurchaseOrder(VMPurchaseOrder vmPurchaseOrder)
        //{
        //    vmPurchaseOrder.PurchaseOrderId = await _service.ProcurementPurchaseOrderClosedReopen(vmPurchaseOrder.PurchaseOrderId);
        //    return RedirectToAction(nameof(ProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyFK });
        //}
        [HttpPost]
        public async Task<ActionResult> DeleteOrderMasters(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrder.OrderMasterId = await _service.OrderMastersDelete(vmSalesOrder.OrderMasterId);
            }
            return RedirectToAction(nameof(ProcurementSalesOrderList), new { companyId = vmSalesOrder.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteRMOrderMasters(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrder.OrderMasterId = await _service.OrderMastersDelete(vmSalesOrder.OrderMasterId);
            }
            return RedirectToAction(nameof(ProcurementRMSalesOrderList), new { companyId = vmSalesOrder.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteOrderMastersForPackagingSalesOrder(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrder.OrderMasterId = await _service.OrderMastersDelete(vmSalesOrder.OrderMasterId);
            }
            return RedirectToAction(nameof(PackagingSalesOrderList), new { companyId = vmSalesOrder.CompanyFK });
        }
        ////kkk

        [HttpGet]
        public async Task<ActionResult> PackagingSalesOrderSlave(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else
            {
                vmSalesOrderSlave = await Task.Run(() => _service.PackagingSalesOrderDetailsGet(companyId, orderMasterId));
            }
            vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
             
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            return View(vmSalesOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> PackagingSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmSalesOrderSlave.OrderMasterId == 0)
                {
                    vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterAdd(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.FProductId > 0)
                {
                    await _service.PackagingOrderDetailAdd(vmSalesOrderSlave);
                }
              
            }
            else if (vmSalesOrderSlave.ActionEum == ActionEnum.Edit)
            {
                await _service.PackagingOrderDetailEdit(vmSalesOrderSlave);
            }
            return RedirectToAction(nameof(PackagingSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePackagingSalesOrderSlave(VMSalesOrder vMSalesOrder)
        {
            vMSalesOrder.CompanyId = await _service.UpdatePackagingSalesOrderSlave(vMSalesOrder);
            return RedirectToAction(nameof(PackagingSalesOrderList), new { companyId = vMSalesOrder.CompanyFK });
        }



        [HttpGet]
        public async Task<ActionResult> PackagingSalesOrderList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            vmSalesOrder = await _service.PackagingOrderMastersListGet(companyId, fromDate, toDate);
            vmSalesOrder.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            //vmPurchaseOrder.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            //vmPurchaseOrder.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            //vmPurchaseOrder.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");



            vmSalesOrder.StrFromDate = fromDate.Value.ToShortDateString();
            vmSalesOrder.StrToDate = toDate.Value.ToShortDateString();

            return View(vmSalesOrder);
        }


        [HttpGet]
        public async Task<ActionResult> PackagingSalesOrderListBySalePerson(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            vmSalesOrder = await _service.PackagingOrderMastersListGetBySalePerson(companyId, fromDate, toDate);

            //vmPurchaseOrder.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            //vmPurchaseOrder.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            //vmPurchaseOrder.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");



            vmSalesOrder.StrFromDate = fromDate.Value.ToShortDateString();
            vmSalesOrder.StrToDate = toDate.Value.ToShortDateString();

            return View(vmSalesOrder);
        }
        [HttpPost]
        public async Task<ActionResult> PackagingSalesOrderList(VMSalesOrder vmSalesOrder)
        {
            if (vmSalesOrder.ActionEum == ActionEnum.Edit)
            {
                await _service.OrderMastersEdit(vmSalesOrder);
            }
            return RedirectToAction(nameof(PackagingSalesOrderList), new { companyId = vmSalesOrder.CompanyFK });
        }
        public async Task<ActionResult> OrderDetailList(long companyId, DateTime? fromDate, DateTime? toDate)
        {
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

            OrderDetailVM model = new OrderDetailVM();

            model = await _service.GetOrderDetailsList(companyId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            model.companyId = companyId;

            return View(model);

        }
        [HttpPost]

        public async Task<ActionResult> OrderDetailList(OrderDetailVM model)
        {
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(OrderDetailList), new { companyId = model.companyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> PackagingSalesOrderBOM(int companyId = 0, int orderDetailId = 0)
        {

            VMFinishProductBOM vmSalesOrderSlave = new VMFinishProductBOM();

            if (orderDetailId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;

            }
            else
            {
                vmSalesOrderSlave = await Task.Run(() => _service.PackagingSalesOrderDetailsGetBOM(companyId, orderDetailId));

            }
            //vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");

            return View(vmSalesOrderSlave);
        }

        [HttpPost]
        public async Task<ActionResult> PackagingSalesOrderBOM(VMFinishProductBOM vmFinishProductBOM)
        {


            if (vmFinishProductBOM.ActionEum == ActionEnum.Add)
            {
                if (vmFinishProductBOM.ID == 0)
                {
                    vmFinishProductBOM.OrderDetailId = await _service.AddFinishProductBOM(vmFinishProductBOM);

                }

            }
            else if (vmFinishProductBOM.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.FinishProductBOMDetailEdit(vmFinishProductBOM);
            }
            else if (vmFinishProductBOM.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmFinishProductBOM.OrderDetailId = await _service.FinishProductBOMDelete(vmFinishProductBOM.ID);
            }
            return RedirectToAction(nameof(PackagingSalesOrderBOM), new { companyId = vmFinishProductBOM.CompanyFK, orderDetailId = vmFinishProductBOM.OrderDetailId });


        }

        public async Task<JsonResult> GetFinishProductBOM(int id)
        {
            var model = await _service.GetFinishProductBOM(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //Order Purchase For Raw Item

        [HttpGet]
        public async Task<ActionResult> PackagingPurchaseOrderSlave(int companyId = 0, int purchaseOrderId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();

            if (purchaseOrderId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else if (purchaseOrderId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.PackagingPurchaseOrderSlaveGet(companyId, purchaseOrderId));
            }
            vmPurchaseOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.EmployeeList = new SelectList(_service.EmployeesByCompanyDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.Businessheadlist = new SelectList(_service.BusinesshedaList(companyId), "Value", "Text");

            if (companyId == (int)CompanyName.KrishibidPackagingLimited)
            {
                vmPurchaseOrderSlave.LCList = new SelectList(_service.PackagingLCHeadGLList(companyId), "Value", "Text");
            }
            return View(vmPurchaseOrderSlave);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> PackagingPurchaseOrderSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmPurchaseOrderSlave.PurchaseOrderId == 0)
                {
                    vmPurchaseOrderSlave.PurchaseOrderId = await _service.PackagingPurchaseOrderAdd(vmPurchaseOrderSlave);
                }
                else
                {
                    await _service.PackagingPurchaseOrderSlaveAdd(vmPurchaseOrderSlave);
                }
            }
            else if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.PackagingPurchaseOrderSlaveEdit(vmPurchaseOrderSlave);
            }

            return RedirectToAction(nameof(PackagingPurchaseOrderSlave), new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }

        [HttpGet]
        public ActionResult PackagingPurchaseRawItemDataList(int StyleNo, int SupplierFK = 0)
        {
            var model = new VMPurchaseOrderSlave();

            model.DataListPur = _service.PackagingPurchaseRawItemDataList(StyleNo, SupplierFK);
            return PartialView("_PackagingProductionRequisitionPartial", model);

        }
        public object GetStyleNo(int id)
        {
            ERPEntities db = new ERPEntities();

            object styleNo = null;
            styleNo = (from orderdtls in db.OrderDetails
                       select new
                       {
                           orderdtls.StyleNo

                       });

            return Json(styleNo, JsonRequestBehavior.AllowGet);
        }

        //PackagingRequisitionItem

        [HttpGet]
        public async Task<ActionResult> PackagingPurchaseRequisition(int companyId = 0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();

            return View(vmPurchaseRequisition);
        }

        [HttpPost]
        public async Task<ActionResult> PackagingPurchaseRequisition(VMPackagingPurchaseRequisition vmPackagingPurchaseRequisition, VMPurchaseOrderSlave productionRequisitionPar)
        {
            if (vmPackagingPurchaseRequisition.ActionEum == ActionEnum.Add)
            {
                if (vmPackagingPurchaseRequisition.RequisitionId == 0)
                {
                    vmPackagingPurchaseRequisition.RequisitionId = await _service.PackagingPurchaseRequisitionAdd(vmPackagingPurchaseRequisition);

                }
                await _service.PackagingPurchaseRequisitionDetailsAdd(vmPackagingPurchaseRequisition, productionRequisitionPar);
            }

            return RedirectToAction(nameof(PackagingPurchaseRequisition), new { companyId = vmPackagingPurchaseRequisition.CompanyFK, purchaseOrderId = vmPackagingPurchaseRequisition.RequisitionId });
        }

        [HttpGet]
        public async Task<ActionResult> PackagingProductionRequisition(int companyId)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();
            vmPurchaseRequisition.DDLStockDepartmetn = _productService.GetDDLStockInfoDepartment(companyId);
            vmPurchaseRequisition.CompanyFK = companyId;

            return View(vmPurchaseRequisition);
        }

        [HttpGet]
        public async Task<ActionResult> GeneralRMRequisition(int companyId, int requisitionId = 0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();
            if (requisitionId > 0)
            {
                vmPurchaseRequisition = await _requisitionService.GetRequisitionForPKLRM(companyId, requisitionId);

            }
            else
            {
                vmPurchaseRequisition.DDLStockDepartmetn = _productService.GetDDLStockInfoDepartment(companyId);
                vmPurchaseRequisition.CompanyFK = companyId;
                vmPurchaseRequisition.CompanyId = companyId;
                vmPurchaseRequisition.RequisitionId = requisitionId;
            }



            return View(vmPurchaseRequisition);
        }

        [HttpPost]
        public async Task<ActionResult> GeneralRMRequisition(VMPackagingPurchaseRequisition vmRequisition)
        {
            vmRequisition.CreatedBy = Common.GetUserId();

            if (vmRequisition.ActionEum == ActionEnum.Add)
            {
                vmRequisition.RequisitionId = await Task.Run(() => _requisitionService.PackagingGeneralRequisition(vmRequisition));
            }
            else if (vmRequisition.ActionEum == ActionEnum.Edit)
            {
                vmRequisition.RequisitionId = await Task.Run(() => _requisitionService.RequisitionItemDetailUpdate(vmRequisition.RequisitionId, vmRequisition.Qty ?? 0, vmRequisition.RProductId, vmRequisition.RequistionItemDetailId));

            }
            return RedirectToAction(nameof(GeneralRMRequisition), new { companyId = vmRequisition.CompanyFK, requisitionId = vmRequisition.RequisitionId });
        }


        [HttpPost]
        public async Task<ActionResult> GeneralRMRequisitionSubmit(VMPackagingPurchaseRequisition vmRequisition)
        {
            // vmRequisition.CreatedBy = Common.GetUserId();

            vmRequisition.RequisitionId = await Task.Run(() => _requisitionService.PackagingGeneralRequisitionSubmit(vmRequisition.RequisitionId, vmRequisition.IsSubmited));

            return RedirectToAction(nameof(GeneralRMRequisition), new { companyId = vmRequisition.CompanyFK, requisitionId = vmRequisition.RequisitionId });
        }

        [HttpPost]
        public async Task<ActionResult> RequisitionItemDetailDelete(VMPackagingPurchaseRequisition vmRequisition)
        {

            vmRequisition.RequisitionId = await Task.Run(() => _requisitionService.RequisitionItemDetailDeleteConfirm(vmRequisition.RequisitionId, vmRequisition.RequistionItemDetailId));

            return RedirectToAction(nameof(GeneralRMRequisition), new { companyId = vmRequisition.CompanyFK, requisitionId = vmRequisition.RequisitionId });
        }

        //[HttpPost]
        //public async Task<ActionResult> RequisitionItemDetailUpdate(int requisitionId,int CompanyId,decimal RQuantity, int RproductioId,Guid? RequistionItemDetailId)
        //{

        //    requisitionId = await Task.Run(() => _requisitionService.RequisitionItemDetailUpdate(requisitionId,RQuantity,RproductioId, RequistionItemDetailId));

        //    return RedirectToAction(nameof(GeneralRMRequisition), new { companyId = CompanyId, requisitionId = requisitionId });
        //}



        [HttpPost]
        public async Task<ActionResult> PackagingProductionRequisition(VMPackagingPurchaseRequisition vmPackagingPurchaseRequisition, VMPurchaseOrderSlave productionRequisitionPar)
        {

            if (vmPackagingPurchaseRequisition.ActionEum == ActionEnum.Add)
            {
                if (vmPackagingPurchaseRequisition.RequisitionId == 0)
                {
                    vmPackagingPurchaseRequisition.CreatedBy = Common.GetUserId();
                    vmPackagingPurchaseRequisition.RequisitionId = await _service.PackagingPurchaseRequisitionAdd(vmPackagingPurchaseRequisition);
                }

                await _service.PackagingPurchaseRequisitionDetailsAdd(vmPackagingPurchaseRequisition, productionRequisitionPar);


            }

            return RedirectToAction(nameof(PackagingProductionRequisitionDetails), new { companyId = vmPackagingPurchaseRequisition.CompanyFK, requisitionId = vmPackagingPurchaseRequisition.RequisitionId });
        }

        [HttpGet]
        public async Task<ActionResult> PackagingProductionRequisitionDetails(int companyId, int requisitionId)
        {
            PackagingProductionRequisitionDetailsRM model = new PackagingProductionRequisitionDetailsRM();
            model = await _requisitionService.PackagingProductionRequisitionDetails(companyId, requisitionId);
            //model.DDLProductList = await _dropDownItemService.GetDDLProductList(companyId);
            //model.GetDDLOrderNo = await _dropDownItemService.GetDDLCompanyWiseOrder(companyId);
            //model.GetDDLOrderDetailsId = _dropdownService.DefaultDDL();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> PackagingProductionRequisitionDetails(PackagingProductionRequisitionDetailsRM model)
        {
            
            if (model.RequisitionItemDetailId.HasValue)
            {
                model.RequisitionId = await Task.Run(() => _requisitionService.RequisitionItemDetailUpdate(model.RequisitionId, model.Qty ?? 0, model.ProductId, model.RequisitionItemDetailId));
            }
            //else
            //{
            //    model.RequisitionId = await Task.Run(() => _requisitionService.RequisitionItemDetailSave(model.RequisitionId, model.Qty ?? 0, model.ProductId,model.FinishProductBOMId));
            //}
           
            return RedirectToAction(nameof(PackagingProductionRequisitionDetails), new { companyId = model.CompanyId, requisitionId = model.RequisitionId });
        }

        [HttpGet]
        public async Task<ActionResult> PackagingProductionRequisitionList(int companyId)
        {

            VMPackagingProductionRequisitions vmSalesOrder = new VMPackagingProductionRequisitions();
            vmSalesOrder = await _service.PackagingProductionRequisitionList(companyId);
            vmSalesOrder.DDLStockDepartmetn = _dropdownService.RenderDDL(_productService.GetDDLStockInfoDepartment(companyId), true);
            return View(vmSalesOrder);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingProductionGeneralRequisitionList(int companyId)
        {

            VMPackagingProductionRequisitions vmSalesOrder = new VMPackagingProductionRequisitions();
            vmSalesOrder = await _service.PackagingGeneralRMRequisitionList(companyId);
            vmSalesOrder.DDLStockDepartmetn = _dropdownService.RenderDDL(_productService.GetDDLStockInfoDepartment(companyId), true);
            return View(vmSalesOrder);
        }

        [HttpPost]
        public async Task<JsonResult> GetResuisitionDataByRequisitionId(int requisitionId)
        {
            var result = await _requisitionService.GetResuisitionDataByRequisitionId(requisitionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetGeneralResuisitionDataByRequisitionId(int requisitionId)
        {
            var result = await _requisitionService.GetGeneralResuisitionDataByRequisitionId(requisitionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<JsonResult> PackagingProductionRequisitionDelete(int requisitionId)
        {
            RResult rResult = new RResult();
            rResult = await _requisitionService.PackagingProductionRequisitionDelete(requisitionId);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> UpdateProductAndQuantityInRequisitionItemDetail(int ProductId, decimal Quentity, Guid? RequistionItemDetailId)
        {
            RResult rResult = new RResult();
            rResult = await _requisitionService.UpdateProductAndQuantityInRequisitionItemDetail(ProductId, Quentity, RequistionItemDetailId);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> RequisitionSubmitied(PackagingProductionRequisitionDetailsRM vmProductionRequisition)
        {
           
            await _requisitionService.RequisitionSubmitied(vmProductionRequisition.RequisitionId);
            return RedirectToAction(nameof(PackagingProductionRequisitionDetails), new { companyId = vmProductionRequisition.CompanyId, requisitionId = vmProductionRequisition.RequisitionId });

        }

        [HttpGet]
        public async Task<ActionResult> PackagingIssueProductFromStore(int companyId = 0, long issueMasterId = 0,int RequisitionId=0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();
            vmPurchaseRequisition.IssueDate = DateTime.Now;
            vmPurchaseRequisition.CompanyFK = companyId;
            vmPurchaseRequisition.DDLStockDepartmetn = _productService.GetDDLStockInfoDepartment(companyId);
            if (issueMasterId > 0)
            {
                vmPurchaseRequisition = await Task.Run(() => _service.GetIssueDetail(companyId, issueMasterId));

            }
            if (RequisitionId>0)
            {
                vmPurchaseRequisition.RequisitionId = RequisitionId;
            }
            
            return View(vmPurchaseRequisition);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingGIssueProductFromStore(int companyId = 0, long issueMasterId = 0,int requisitionId=0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();
            vmPurchaseRequisition.IssueDate = DateTime.Now;
            vmPurchaseRequisition.CompanyFK = companyId;
            vmPurchaseRequisition.DDLStockDepartmetn = _productService.GetDDLStockInfoDepartment(companyId);
            if (issueMasterId > 0)
            {
                vmPurchaseRequisition = await Task.Run(() => _service.GetGeneralIssueList(companyId, issueMasterId));

            }
            if (requisitionId > 0)
            {
                vmPurchaseRequisition.RequisitionId = requisitionId;
            }
            return View(vmPurchaseRequisition);
        }

        [HttpGet]
        public async Task<JsonResult> KPLRequisitionIssuedDetailsDelete(VMPackagingPurchaseRequisition model)
        {
            RResult rResult = new RResult();
            rResult = await _requisitionService.KPLRequisitionIssuedDetailsDelete(model);

            return Json(rResult, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public async Task<ActionResult> PackagingIssueProductFromStore(VMPackagingPurchaseRequisition vmPackagingPurchaseRequisition)
        {

            if (vmPackagingPurchaseRequisition.ActionEum == ActionEnum.Add)
            {
                if (vmPackagingPurchaseRequisition.IssueMasterId == 0)
                {
                    vmPackagingPurchaseRequisition.IssueMasterId = await _service.PackagingIssueProductFromStore(vmPackagingPurchaseRequisition);
                    //await _service.PackagingIssueProductFromStoreDetailsAdd(vmPackagingPurchaseRequisition);
                }

            }

            return RedirectToAction(nameof(PackagingIssueProductFromStore), new { companyId = 20, issueMasterId = vmPackagingPurchaseRequisition.IssueMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> PackagingGIssueProductFromStore(VMPackagingPurchaseRequisition vmPackagingPurchaseRequisition)
        {

            if (vmPackagingPurchaseRequisition.ActionEum == ActionEnum.Add)
            {
                if (vmPackagingPurchaseRequisition.IssueMasterId == 0)
                {
                    vmPackagingPurchaseRequisition.IssueMasterId = await _service.PackagingGIssueProductFromStoreSave(vmPackagingPurchaseRequisition);
                }

            }

            return RedirectToAction(nameof(PackagingGIssueProductFromStore), new { companyId = 20, issueMasterId = vmPackagingPurchaseRequisition.IssueMasterId });
        }
        public JsonResult GetAutoCompleteOrderNoGetRequsitionId(string prefix, int companyId)
        {

            var products = _service.GetAutoCompleteOrderNoGetRequsitionId(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRequisitionNoByRequsitionId(int requisitionId, int companyId)
        {

            var products = _service.GetRequisitionNoByRequsitionId(requisitionId, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneralRequisitionNoByRequsitionId(int requisitionId, int companyId)
        {

            var products = _service.GetGeneralRequisitionNoByRequsitionId(requisitionId, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteByRequsitionId(string prefix, int companyId)
        {

            var products = _service.GetAutoCompleteByRequsitionId(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public async Task<ActionResult> RequisitionMasterDetails(RequisitionMasterDetailsRM loanMastes)
        //{
        //    RResult rResult = new RResult();
        //    loanMastes.CompanyId = 20;
        //    loanMastes.CreatedBy = Common.GetUserId();

        //    rResult = await Task.Run(() => _requisitionService.RequisitionSaveForPKL(loanMastes));
        //    ViewBag.RequisitionId = rResult.result;
        //    return Json(rResult, JsonRequestBehavior.AllowGet);
        //}



        [HttpPost]
        public JsonResult RequisitionDetailsSave(RequisitionMasterDetailsRM RequisitionItemDetailrm)
        {
            RResult rResult = new RResult();
            rResult = _requisitionService.RequisitionDetailsSaveForPKL(RequisitionItemDetailrm);
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PackagingProductionStoreDataList(int RequisitionId)
        {
            var model = new VMPackagingPurchaseRequisition();

            model.DataListPro = _service.PackagingProductionStoreDataList(RequisitionId);

            return PartialView("_PackagingProductionForStorePartial", model);


        }

        public ActionResult PackagingGProductionStoreDataList(int RequisitionId)
        {
            var model = new VMPackagingPurchaseRequisition();

            model.DataListPro = _service.PackagingGeneralProductionStoreDataList(RequisitionId);

            return PartialView("_PackagingGeneralProductionForStorePartial", model);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingIssueItemList(int companyId = 0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();

            vmPurchaseRequisition = await Task.Run(() => _service.PackagingIssueItemList(companyId));
            vmPurchaseRequisition.DDLStockDepartment = _dropdownService.RenderDDL(_productService.GetDDLStockInfoDepartment(companyId), true);

            return View(vmPurchaseRequisition);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingNotIssueItemList(int companyId = 0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();

            vmPurchaseRequisition = await Task.Run(() => _service.PackagingNotIssueItemList(companyId));
            vmPurchaseRequisition.DDLStockDepartment = _dropdownService.RenderDDL(_productService.GetDDLStockInfoDepartment(companyId), true);

            return View(vmPurchaseRequisition);
        }

        [HttpGet]
        public async Task<ActionResult> GeneralPackagingNotIssueItemList(int companyId = 0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();

            vmPurchaseRequisition = await Task.Run(() => _service.GeneralPackagingNotIssueItemList(companyId));
            vmPurchaseRequisition.DDLStockDepartment = _dropdownService.RenderDDL(_productService.GetDDLStockInfoDepartment(companyId), true);

            return View(vmPurchaseRequisition);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingGeneralIssueItemList(int companyId = 0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();

            vmPurchaseRequisition = await Task.Run(() => _service.PackagingGIssueItemList(companyId));
            vmPurchaseRequisition.DDLStockDepartment = _dropdownService.RenderDDL(_productService.GetDDLStockInfoDepartment(companyId), true);

            return View(vmPurchaseRequisition);
        }
        [HttpGet]
        public async Task<ActionResult> ProductPackagingAchknolagementList(int companyId = 0)
        {
            VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();

            vmPurchaseRequisition = await Task.Run(() => _service.PackagingUnAchknoledgedIssueItemList(companyId));
            vmPurchaseRequisition.DDLStockDepartment = _dropdownService.RenderDDL(_productService.GetDDLStockInfoDepartment(companyId), true);

            return View(vmPurchaseRequisition);
        }
        [HttpPost]
        public async Task<ActionResult> PackagingRMIssuedAchknolagement(VMPackagingPurchaseRequisition model)
        {
            model.AchknologeById = Common.GetIntUserId();
            GlobalValues.UserId = Common.GetUserId();
           await Task.Run(() => _service.PackagingRMIssuedAchknolagement(model));

           //await _service.SubmitMultiIssuePackaging(model);

            return RedirectToAction(nameof(ProductPackagingAchknolagementList), new { companyId = 20 });
        }

       

        [HttpGet]
        public async Task<ActionResult> RequisitionItemDetailDelete(Guid? id)
        {
            RResult rResult = new RResult();

            rResult = await Task.Run(() => _requisitionService.RequisitionItemDetailDelete(id));
            return Json(rResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> KPLRequisitionIssueUpdate(VMPackagingPurchaseRequisition model)
        {
            RResult rResult = new RResult();
            model.ModifiedBy = Common.GetUserId();
            rResult = await Task.Run(() => _requisitionService.KPLRequisitionIssueUpdate(model));
            return RedirectToAction(nameof(PackagingIssueItemList), new { companyId = 20 });
        }

        [HttpPost]
        public async Task<ActionResult> KPLGeneralRequisitionIssueUpdate(VMPackagingPurchaseRequisition model)
        {
            RResult rResult = new RResult();
            model.ModifiedBy = Common.GetUserId();
            rResult = await Task.Run(() => _requisitionService.KPLRequisitionIssueUpdate(model));
            return RedirectToAction(nameof(PackagingGeneralIssueItemList), new { companyId = 20 });
        }

        [HttpPost]
        public async Task<ActionResult> DeletePackagingSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmSalesOrderSlave.OrderDetailId = await _service.OrderDetailDelete(vmSalesOrderSlave.OrderDetailId);
            }
            return RedirectToAction(nameof(PackagingSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }
        [HttpPost]
        public async Task<ActionResult> SubmitPackagingOrderMastersFromSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            vmSalesOrderSlave.OrderMasterId = await _service.OrderMastersSubmit(vmSalesOrderSlave.OrderMasterId);
            return RedirectToAction(nameof(PackagingSalesOrderSlave), "Procurement", new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        #endregion



        #region Purchase Order
        [HttpGet]
        public async Task<ActionResult> GCCLProcurementPurchaseOrderSlave(int companyId = 0, int purchaseOrderId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();

            if (purchaseOrderId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else if (purchaseOrderId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.ProcurementPurchaseOrderSlaveGet(companyId, purchaseOrderId));

            }
            vmPurchaseOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.EmployeeList = new SelectList(_service.EmployeesByCompanyDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");

            if (companyId == (int)CompanyName.GloriousCropCareLimited)
            {
                vmPurchaseOrderSlave.LCList = new SelectList(_service.GCCLLCHeadGLList(companyId), "Value", "Text");
            }
            return View(vmPurchaseOrderSlave);
        }

        [HttpGet]
        public async Task<ActionResult> KFMALProcurementPurchaseOrderSlave(int companyId = 0, int purchaseOrderId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();

            if (purchaseOrderId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else if (purchaseOrderId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.KFMALProcurementPurchaseOrderSlaveGet(companyId, purchaseOrderId));

            }
            vmPurchaseOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.EmployeeList = new SelectList(_service.EmployeesByCompanyDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.Businessheadlist = new SelectList(_service.BusinesshedaList(companyId), "Value", "Text");




            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                vmPurchaseOrderSlave.LCList = new SelectList(_service.KFMALCHeadGLList(companyId), "Value", "Text");
            }
            return View(vmPurchaseOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> KFMALProcurementPurchaseOrderSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {

            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmPurchaseOrderSlave.PurchaseOrderId == 0)
                {
                    vmPurchaseOrderSlave.PurchaseOrderId = await _service.KFMALProcurementPurchaseOrderAdd(vmPurchaseOrderSlave);

                }
                else
                {
                    await _service.KFMALProcurementPurchaseOrderSlaveAdd(vmPurchaseOrderSlave);
                }

            }
            else if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.KFMALProcurementPurchaseOrderSlaveEdit(vmPurchaseOrderSlave);
            }
            return RedirectToAction(nameof(KFMALProcurementPurchaseOrderSlave), new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }


        public List<object> BusinesshedaList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.BusinessHeads.Where(x => x.IsActive && x.BusineesId_Fk == companyId).ToList())
            {
                List.Add(new { Text = item.Id, Value = item.EmployeeId });
            }
            return List;

        }


        [HttpPost]
        public async Task<ActionResult> GCCLProcurementPurchaseOrderSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {

            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmPurchaseOrderSlave.PurchaseOrderId == 0)
                {
                    vmPurchaseOrderSlave.PurchaseOrderId = await _service.ProcurementPurchaseOrderAdd(vmPurchaseOrderSlave);

                }
                else
                {
                    await _service.ProcurementPurchaseOrderSlaveAdd(vmPurchaseOrderSlave);

                }
            }
            else if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.ProcurementPurchaseOrderSlaveEdit(vmPurchaseOrderSlave);
            }
            return RedirectToAction(nameof(GCCLProcurementPurchaseOrderSlave), new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }



        #endregion

        [HttpGet]
        public async Task<ActionResult> PromtionalOfferDetail(int companyId = 0, int promtionalOfferId = 0)
        {
            VMPromtionalOfferDetail vmPromtionalOfferDetail = new VMPromtionalOfferDetail();

            if (promtionalOfferId == 0)
            {
                vmPromtionalOfferDetail.CompanyId = companyId;

            }
            else if (promtionalOfferId > 0)
            {
                vmPromtionalOfferDetail = await Task.Run(() => _service.ProcurementPromtionalOfferDetailGet(companyId, promtionalOfferId));
            }


            return View(vmPromtionalOfferDetail);
        }
        [HttpPost]
        public async Task<ActionResult> PromtionalOfferDetail(VMPromtionalOfferDetail vmPromtionalOfferDetail)
        {

            if (vmPromtionalOfferDetail.ActionEum == ActionEnum.Add)
            {
                if (vmPromtionalOfferDetail.PromtionalOfferId == 0)
                {
                    vmPromtionalOfferDetail.PromtionalOfferId = await _service.PromtionalOfferAdd(vmPromtionalOfferDetail);

                }
                await _service.PromtionalOfferDetailAdd(vmPromtionalOfferDetail);
            }
            else if (vmPromtionalOfferDetail.ActionEum == ActionEnum.Edit)
            {
                await _service.PromtionalOfferDetailUpdate(vmPromtionalOfferDetail);
            }
            return RedirectToAction(nameof(PromtionalOfferDetail), new { companyId = vmPromtionalOfferDetail.CompanyId, promtionalOfferId = vmPromtionalOfferDetail.PromtionalOfferId });
        }

        [HttpPost]
        public async Task<ActionResult> PromtionalOfferDetailsDelete(VMPromtionalOfferDetail vMPromtionalOffer)
        {
            if (vMPromtionalOffer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vMPromtionalOffer.PromtionalOfferId = await _service.PromtionalOfferDetailsDelete(vMPromtionalOffer.PromtionalOfferDetailId);

            }
            return RedirectToAction(nameof(PromtionalOfferDetail), new { companyId = vMPromtionalOffer.CompanyFK, promtionalOfferId = vMPromtionalOffer.PromtionalOfferId });
        }
        
        [HttpPost]
        public async Task<ActionResult> PromtionalOfferEdit(VMPromtionalOffer vMPromtionalOffer)
        {

            vMPromtionalOffer.PromtionalOfferId = await _service.PromtionalOfferEdit(vMPromtionalOffer.PromtionalOfferId, vMPromtionalOffer.PromoCode, vMPromtionalOffer.FromDate, vMPromtionalOffer.ToDate);
            return RedirectToAction(nameof(PromotionalOfferDetailList), new { companyId = vMPromtionalOffer.CompanyId});
        }

        [HttpPost]
        public async Task<ActionResult> PromtionalOfferDetailsSubmited(VMPromtionalOfferDetail vMPromtionalOffer)
        {
            vMPromtionalOffer.PromtionalOfferId = await _service.PromtionalOfferDetailsSubmited(vMPromtionalOffer.PromtionalOfferId);
            return RedirectToAction(nameof(PromtionalOfferDetail), new { companyId = vMPromtionalOffer.CompanyId, promtionalOfferId = vMPromtionalOffer.PromtionalOfferId });
        }


        [HttpGet]
        public async Task<ActionResult> PromotionalOfferDetailList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);


            VMPromtionalOffer vmPurchaseOrder = new VMPromtionalOffer();
            vmPurchaseOrder = await _service.GetPromotionalOfferListAsync(companyId, fromDate, toDate);
            vmPurchaseOrder.CompanyId = companyId;
            vmPurchaseOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
             vmPurchaseOrder.UserId = System.Web.HttpContext.Current.User.Identity.Name;
            return View(vmPurchaseOrder);
        }

        [HttpPost]
        public ActionResult PromotionalOfferDetailList(int CompanyId,DateTime StrFromDate,DateTime StrToDate)
        {
            return RedirectToAction(nameof(PromotionalOfferDetailList), new { companyId=CompanyId,fromDate= StrFromDate, toDate= StrToDate } );
        }

        [HttpPost]
        public async Task<ActionResult> PromtionalOfferDelete(VMPromtionalOffer vMPromtionalOffer)
        {
            vMPromtionalOffer.PromtionalOfferId = await _service.PromtionalOfferDelete(vMPromtionalOffer.PromtionalOfferId);
            return RedirectToAction(nameof(PromotionalOfferDetailList), new { companyId = vMPromtionalOffer.CompanyId });
        }
        [HttpPost]

        public ActionResult PromtionalOfferDetailList(VMPurchaseOrder vmPurchaseOrder)
        {
            if (vmPurchaseOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmPurchaseOrder.CompanyId;
            }
            vmPurchaseOrder.FromDate = Convert.ToDateTime(vmPurchaseOrder.StrFromDate);
            vmPurchaseOrder.ToDate = Convert.ToDateTime(vmPurchaseOrder.StrToDate);

            return RedirectToAction(nameof(PackagingPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyId, fromDate = vmPurchaseOrder.FromDate, toDate = vmPurchaseOrder.ToDate, vStatus = vmPurchaseOrder.Status });
        }



        [HttpGet]
        public async Task<ActionResult> GCCLProcurementSalesOrderSlave(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();

            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else
            {
                vmSalesOrderSlave = await Task.Run(() => _service.GcclProcurementSalesOrderDetailsGet(companyId, orderMasterId));
            }

            vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmSalesOrderSlave);
        }

        [HttpPost]
        public async Task<ActionResult> GCCLProcurementSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {

            if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmSalesOrderSlave.OrderMasterId == 0)
                {
                    vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterAdd(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.FProductId > 0)
                {
                    await _service.GcclOrderDetailAdd(vmSalesOrderSlave);
                }

                if (vmSalesOrderSlave.TotalAmountAfterDiscount > 0)
                {
                    await _service.OrderMasterAmountEdit(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.PromotionalOfferId > 0)
                {
                    await _service.PromtionalOfferIntegration(vmSalesOrderSlave);
                }
            }
            else if (vmSalesOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.OrderDetailEdit(vmSalesOrderSlave);
            }
            return RedirectToAction(nameof(GCCLProcurementSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }


        //ani
        [HttpGet]
        public async Task<ActionResult> KFMALSalesOrderSlave(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();

            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else
            {

                vmSalesOrderSlave = await Task.Run(() => _service.KfmalProcurementSalesOrderDetailsGet(companyId, orderMasterId));


            }
            vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");


            return View(vmSalesOrderSlave);
        }


        [HttpPost]
        public async Task<ActionResult> KFMALSalesOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmSalesOrderSlave.OrderMasterId == 0)
                {
                    vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterAdd(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.FProductId > 0)
                {
                    await _service.GcclOrderDetailAdd(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.TotalAmountAfterDiscount > 0)
                {
                    await _service.OrderMasterAmountEdit(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.PromotionalOfferId > 0)
                {
                    await _service.PromtionalOfferIntegration(vmSalesOrderSlave);
                }
            }
            else if (vmSalesOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.PackagingOrderDetailEdit(vmSalesOrderSlave);
            }
            return RedirectToAction(nameof(KFMALSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }
        [HttpGet]
        public async Task<ActionResult> FeedSalesOrderSlave(int companyId, string productType, int orderMasterId = 0)
        {
            FeedSalesOrderModel vmSalesOrderSlave = new FeedSalesOrderModel();
            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyId = companyId;
                vmSalesOrderSlave.OrderMasterId = orderMasterId;
                vmSalesOrderSlave.Status = (int)EnumFeedSalesStatus.Draft;
                vmSalesOrderSlave.ProductType = productType;
            }
            else
            {
                long userId = Common.GetIntUserId();
                vmSalesOrderSlave = await Task.Run(() => _service.FeedSalesOrderDetailsGet(companyId, orderMasterId));
                vmSalesOrderSlave.CurrentEmployeeIntId = userId;
            }
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.BankOrCashParantList = new SelectList(_accountingService.FEEDCashAndBankDropDownList(companyId), "Value", "Text");
            return View(vmSalesOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> FeedSalesOrderSlave(FeedSalesOrderModel feedSalesOrderModel)
        {

            if (feedSalesOrderModel.ActionEum == ActionEnum.Add)
            {
                if (feedSalesOrderModel.OrderMasterId == 0)
                {
                    feedSalesOrderModel.OrderMasterId = await _service.FeedOrderMasterAdd(feedSalesOrderModel);
                }
                if (feedSalesOrderModel.FProductId > 0)
                {
                    await _service.FeedOrderDetailAdd(feedSalesOrderModel);
                }
            }
            else if (feedSalesOrderModel.ActionEum == ActionEnum.Edit)
            {

                await _service.FeedOrderDetailEdit(feedSalesOrderModel);
            }
            return RedirectToAction(nameof(FeedSalesOrderSlave), new { companyId = feedSalesOrderModel.CompanyId, orderMasterId = feedSalesOrderModel.OrderMasterId });
        }

        [HttpGet]
        public async Task<ActionResult> FeedProcurementSalesOrderSlave(int companyId, string productType, int orderMasterId = 0)
        {
            FeedSalesOrderModel vmSalesOrderSlave = new FeedSalesOrderModel();
            if (Session["Id"] != null && int.TryParse(Session["Id"].ToString(), out int Uid))
            {
                vmSalesOrderSlave.ProductType = productType;
                vmSalesOrderSlave.SalePersonId = Uid;
                vmSalesOrderSlave.SalePersonName = _db.Employees.Find(Uid).Name;
            }
            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyId = companyId;
                vmSalesOrderSlave.OrderMasterId = orderMasterId;
                vmSalesOrderSlave.Status = (int)EnumFeedSalesStatus.Draft;
            }
            else
            {
                long userId = Common.GetIntUserId();
                vmSalesOrderSlave = await Task.Run(() => _service.FeedSalesOrderDetailsGet(companyId, orderMasterId));
                vmSalesOrderSlave.CurrentEmployeeIntId = userId;
            }
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.BankOrCashParantList = new SelectList(_accountingService.FEEDCashAndBankDropDownList(companyId), "Value", "Text");
            return View(vmSalesOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> FeedProcurementSalesOrderSlave(FeedSalesOrderModel feedSalesOrderModel)
        {

            if (feedSalesOrderModel.ActionEum == ActionEnum.Add)
            {
                if (feedSalesOrderModel.OrderMasterId == 0)
                {
                    feedSalesOrderModel.OrderMasterId = await _service.FeedOrderMasterAdd(feedSalesOrderModel);
                }
                if (feedSalesOrderModel.FProductId > 0)
                {
                    await _service.FeedOrderDetailAdd(feedSalesOrderModel);
                }
            }
            else if (feedSalesOrderModel.ActionEum == ActionEnum.Edit)
            {

                await _service.FeedOrderDetailEdit(feedSalesOrderModel);
            }
            return RedirectToAction(nameof(FeedProcurementSalesOrderSlave), new { companyId = feedSalesOrderModel.CompanyId, orderMasterId = feedSalesOrderModel.OrderMasterId });
        }

        [HttpGet]
        public async Task<ActionResult> GCCLProcurementSalesOrderSlaveByPRF(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();


            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else
            {

                if (companyId == (int)CompanyName.GloriousCropCareLimited)
                {
                    vmSalesOrderSlave = await Task.Run(() => _service.GcclProcurementSalesOrderDetailsGet(companyId, orderMasterId));
                }
                else
                {
                    vmSalesOrderSlave = await Task.Run(() => _service.ProcurementSalesOrderDetailsGet(companyId, orderMasterId));
                }


            }
            vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");

            if (companyId == 8)
            {
                vmSalesOrderSlave.SubZoneList = new SelectList(_service.ZonesDropDownList(companyId), "Value", "Text");
            }
            else
            {
                vmSalesOrderSlave.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            }
            vmSalesOrderSlave.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");


            return View(vmSalesOrderSlave);
        }

        [HttpGet]
        public async Task<JsonResult> GetDemandsByCustomer(int companyId, int customerId)
        {
            var model = await _service.DemandsDropDownList(customerId, companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetDemandDetailsPartial(int demandId)
        {
            var model = await _service.GetDemandItemPartial(demandId);

            return PartialView("_InvoiceTableForPRF", model);
        }
        public async Task<ActionResult> GetGcclDemandDetailPartial(int demandId)
        {
            var model = await _service.GetDemandItemPartial(demandId);

            return PartialView("_DemandDetailPartial", model);
        }

        public async Task<JsonResult> GetDemandsById(int id)
        {
            VmDemandItemService model = new VmDemandItemService();
            model = await _service.GetDemandsById(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public async Task<ActionResult> GetOfficerName(int SubzoneId)
        {

            var model = await _service.OfficerName(SubzoneId);

            return Json(model, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]

        public async Task<ActionResult> OfficerofTerritoryName(int SubzoneId)
        {

            var model = await _service.OfficerofTerritoryName(SubzoneId);

            return Json(model, JsonRequestBehavior.AllowGet);

        }




        [HttpPost]
        public async Task<ActionResult> GCCLProcurementSalesOrderSlaveByPRF(string StrOrderMaster, string ArrayOrderItems)
        {
            OrderMaster orderMaster = new OrderMaster();
            VMSalesOrderSlave vMSales = new VMSalesOrderSlave();
            List<VmDemandItemService> demandItems = new List<VmDemandItemService>();
            try
            {
                orderMaster = JsonConvert.DeserializeObject<OrderMaster>(StrOrderMaster);
                vMSales.CompanyFK = orderMaster.CompanyId;
                vMSales.OrderDate = orderMaster.OrderDate;
                vMSales.DemandId = orderMaster.DemandId;
                vMSales.ExpectedDeliveryDate = orderMaster.ExpectedDeliveryDate;
                vMSales.FinalDestination = orderMaster.FinalDestination;
                vMSales.CustomerPaymentMethodEnumFK = orderMaster.PaymentMethod;
                vMSales.CourierNo = orderMaster.CourierNo;
                vMSales.CourierCharge = orderMaster.CourierCharge;
                vMSales.PayableAmount = Convert.ToDouble(orderMaster.CurrentPayable);
                vMSales.StockInfoId = (int)(orderMaster.StockInfoId);
                vMSales.CustomerId = (int)orderMaster.CustomerId;
                vMSales.Remarks = orderMaster.Remarks;
                vMSales.TotalAmount = (double)orderMaster.TotalAmount.Value;
                demandItems = JsonConvert.DeserializeObject<List<VmDemandItemService>>(ArrayOrderItems);
                if (vMSales.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
                {
                    vMSales.OrderNo = orderMasterService.GetNewOrderNo(vMSales.CompanyFK.Value, vMSales.StockInfoId, "F");
                }

                var orderMasterID = await _service.OrderMasterAddForPRF(vMSales, demandItems);
                if (orderMasterID > 0)
                {
                    return Json(new { companyId = orderMaster.CompanyId, orderMasterId = orderMasterID, error = false, errormsg = "" });
                }
                else
                {
                    return Json(new { companyId = orderMaster.CompanyId, orderMasterId = 0, error = true, errormsg = "Could not add" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { companyId = orderMaster.CompanyId, orderMasterId = 0, error = true, errormsg = ex.Message });
            }
        }




        [HttpGet]

        public async Task<JsonResult> GetLcvalue(int Companyid)
        {
            var obj = _service.LCValue(Companyid);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public async Task<JsonResult> GetLcvalueByID(int Id)
        {

            var obj = _service.LCValueForAppend(Id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> CreateOrderByDemand(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            SelectModel models = new SelectModel();

            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            //else
            //{
            //    vmSalesOrderSlave = await Task.Run(() => _service.GcclProcurementSalesOrderDetailsGet(companyId, orderMasterId));
            //}

            return View(vmSalesOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> CreateOrderByDemand(VMSalesOrderSlave vMSalesOrder, VmDemandItemService vmModel)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vMSalesOrder.OrderMasterId == 0)
                {
                    vMSalesOrder.OrderMasterId = await _service.OrderMasterAddForPRF(vMSalesOrder, vmModel.DemandItemList);

                }
            }

            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(GCCLProcurementSalesOrderSlave), new { companyId = vMSalesOrder.CompanyFK, orderMasterId = vMSalesOrder.OrderMasterId });
        }



        [HttpGet]
        public async Task<ActionResult> KfmalCreateOrderByDemand(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            SelectModel models = new SelectModel();

            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            //else
            //{
            //    vmSalesOrderSlave = await Task.Run(() => _service.GcclProcurementSalesOrderDetailsGet(companyId, orderMasterId));
            //}

            return View(vmSalesOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> KfmalCreateOrderByDemand(VMSalesOrderSlave vMSalesOrder, VmDemandItemService vmModel)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vMSalesOrder.OrderMasterId == 0)
                {
                    vMSalesOrder.OrderMasterId = await _service.OrderMasterAddForPRF(vMSalesOrder, vmModel.DemandItemList);
                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(KFMALSalesOrderSlave), new { companyId = vMSalesOrder.CompanyFK, orderMasterId = vMSalesOrder.OrderMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateKFMALPurchaseBasic(VMPurchaseOrderSlave Model)
        {
            var data = _service.EditPurchasebasic(Model);

            return RedirectToAction(nameof(KFMALProcurementPurchaseOrderList), new { companyId = Model.CompanyId });
        }

        [HttpGet]
        public async Task<ActionResult> SaletargetEmployee(int CompanyId)
        {

            OfficerSalestargetVm vm = new OfficerSalestargetVm();
            vm.SelectModels = await _service.GetSaleEmplyee(CompanyId);
            vm.CompanyId = CompanyId;

            return View(vm);
        }
        [HttpPost]
        public ActionResult FeedSaveSalesTargets(OfficerSalestargetVm model)
        {
            bool obj = _service.SaveSalesTargets(model);


            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetEmplyeeByYear(long EmpId, int Year)
        {
            OfficerSalestargetVm vm = new OfficerSalestargetVm();
            vm.officerSalestargetVms = _service.GetTargetYear(EmpId, Year);

            return Json(vm, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditSaleTarget(long OfficerSalesTargetId, decimal newQuantity)
        {

            var obj = _service.EditSalesTaget(OfficerSalesTargetId, newQuantity);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }



        public async Task<JsonResult> KFMALSingleProcurementPurchaseOrder(int id, int companyId)
        {
            VMPurchaseOrder model = new VMPurchaseOrder();
            model = await _service.KFMALGetSingleProcurementPurchaseOrder(id);
            model.LcValueList = _service.LCValue(companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> PackagingSinglePurchaseOrder(int id, int companyId)
        {
            VMPurchaseOrder model = new VMPurchaseOrder();
            model = await _service.PackagingGetSinglePurchaseOrder(id);
            model.LcValueList = _service.LCValue(companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> KFMALProcurementPurchaseOrderUpdate(VMPurchaseOrder vmPurchaseOrder)
        {
            if (vmPurchaseOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmPurchaseOrder.CompanyId;
            }
            return RedirectToAction(nameof(KFMALProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyId, fromDate = vmPurchaseOrder.FromDate, toDate = vmPurchaseOrder.ToDate, vStatus = vmPurchaseOrder.Status });
        }
        [HttpGet]
        public async Task<ActionResult> KFMALSalesServiceOrderSlave(int companyId = 0, int orderMasterId = 0)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();


            if (orderMasterId == 0)
            {
                vmSalesOrderSlave.CompanyFK = companyId;
                vmSalesOrderSlave.IsService = true;
                vmSalesOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else
            {

                vmSalesOrderSlave = await Task.Run(() => _service.KfmalProcurementSalesOrderDetailsGet(companyId, orderMasterId));


            }
            vmSalesOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.PromoOfferList = new SelectList(_service.PromtionalOffersDropDownList(companyId), "Value", "Text");
            vmSalesOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");


            return View(vmSalesOrderSlave);
        }

        [HttpGet]
        public async Task<ActionResult> KFMALSalesOrderServiceList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2); ;

            if (!toDate.HasValue) toDate = DateTime.Now;


            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder = await _service.KFMALProcurementOrderMastersServiceListGet(companyId, fromDate, toDate, vStatus);
            vmSalesOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmSalesOrder.Status = vStatus ?? -1;
            return View(vmSalesOrder);
        }


        [HttpPost]
        public async Task<ActionResult> KFMALSalesServiceOrderSlave(VMSalesOrderSlave vmSalesOrderSlave)
        {
            if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmSalesOrderSlave.OrderMasterId == 0)
                {
                    vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterAdd(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.FProductId > 0)
                {
                    await _service.GcclOrderDetailAdd(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.TotalAmountAfterDiscount > 0)
                {
                    await _service.OrderMasterAmountEdit(vmSalesOrderSlave);
                }
                if (vmSalesOrderSlave.PromotionalOfferId > 0)
                {
                    await _service.PromtionalOfferIntegration(vmSalesOrderSlave);
                }
            }
            else if (vmSalesOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.PackagingOrderDetailEdit(vmSalesOrderSlave);
            }
            return RedirectToAction(nameof(KFMALSalesServiceOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        }

        #region KPPL Purchase Order
        [HttpGet]
        public async Task<ActionResult> KPPLProcurementPurchaseOrderSlave(int companyId = 0, int purchaseOrderId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();

            if (purchaseOrderId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else if (purchaseOrderId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.KPPLProcurementPurchaseOrderSlaveGet(companyId, purchaseOrderId));

            }
            vmPurchaseOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.EmployeeList = new SelectList(_service.EmployeesByCompanyDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.StockInfoList = new SelectList(_service.StockInfoesDropDownList(companyId), "Value", "Text");

            if (companyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited)
            {
                vmPurchaseOrderSlave.LCList = new SelectList(_service.KPPLLCHeadGLList(companyId), "Value", "Text");
            }
            return View(vmPurchaseOrderSlave);
        }

        [HttpPost]
        public async Task<ActionResult> KPPLProcurementPurchaseOrderSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {

            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmPurchaseOrderSlave.PurchaseOrderId == 0)
                {
                    vmPurchaseOrderSlave.PurchaseOrderId = await _service.KPPLProcurementPurchaseOrderAdd(vmPurchaseOrderSlave);

                }
                else
                {
                    await _service.KPPLProcurementPurchaseOrderSlaveAdd(vmPurchaseOrderSlave);

                }
            }
            else if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.KPPLProcurementPurchaseOrderSlaveEdit(vmPurchaseOrderSlave);
            }
            return RedirectToAction(nameof(KPPLProcurementPurchaseOrderSlave), new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }

        [HttpGet]
        public async Task<ActionResult> KPPLProcurementPurchaseOrderList(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2); ;

            if (!toDate.HasValue) toDate = DateTime.Now;


            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await _service.KPPLProcurementPurchaseOrderListGet(companyId, fromDate, toDate, vStatus);

            vmPurchaseOrder.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrder.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrder.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");

            vmPurchaseOrder.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmPurchaseOrder.Status = vStatus ?? -1;
            vmPurchaseOrder.UserId = System.Web.HttpContext.Current.User.Identity.Name;
            return View(vmPurchaseOrder);
        }


        [HttpPost]
        public async Task<ActionResult> KPPLProcurementPurchaseOrderList(VMPurchaseOrder vmPurchaseOrder)
        {
            if (vmPurchaseOrder.CompanyId > 0)
            {
                Session["CompanyId"] = vmPurchaseOrder.CompanyId;
            }
            vmPurchaseOrder.FromDate = Convert.ToDateTime(vmPurchaseOrder.StrFromDate);
            vmPurchaseOrder.ToDate = Convert.ToDateTime(vmPurchaseOrder.StrToDate);

            return RedirectToAction(nameof(KPPLProcurementPurchaseOrderList), new { companyId = vmPurchaseOrder.CompanyId, fromDate = vmPurchaseOrder.FromDate, toDate = vmPurchaseOrder.ToDate, vStatus = vmPurchaseOrder.Status });
        }
        #endregion


        [HttpGet]
        public async Task<ActionResult> PackagingFGPurchaseOrderSlave(int companyId = 0, int purchaseOrderId = 0)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();

            if (purchaseOrderId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.Status = (int)EnumPOStatus.Draft;
            }
            else if (purchaseOrderId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.PackagingPurchaseOrderSlaveGet(companyId, purchaseOrderId));
            }
            vmPurchaseOrderSlave.TermNCondition = new SelectList(_service.CommonTremsAndConditionDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.ShippedByList = new SelectList(_service.ShippedByListDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.CountryList = new SelectList(_service.CountriesDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.EmployeeList = new SelectList(_service.EmployeesByCompanyDropDownList(companyId), "Value", "Text");
            vmPurchaseOrderSlave.Businessheadlist = new SelectList(_service.BusinesshedaList(companyId), "Value", "Text");

            if (companyId == (int)CompanyName.KrishibidPackagingLimited)
            {
                vmPurchaseOrderSlave.LCList = new SelectList(_service.PackagingLCHeadGLList(companyId), "Value", "Text");
            }
            return View(vmPurchaseOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> PackagingFGPurchaseOrderSlave(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Add)
            {
                if (vmPurchaseOrderSlave.PurchaseOrderId == 0)
                {
                    vmPurchaseOrderSlave.PurchaseOrderId = await _service.PackagingPurchaseOrderAdd(vmPurchaseOrderSlave);
                }
                else
                {
                    await _service.PackagingPurchaseOrderSlaveAdd(vmPurchaseOrderSlave);
                }
            }
            else if (vmPurchaseOrderSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.PackagingPurchaseOrderSlaveEdit(vmPurchaseOrderSlave);
            }

            return RedirectToAction(nameof(PackagingFGPurchaseOrderSlave), new { companyId = vmPurchaseOrderSlave.CompanyFK, purchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId });
        }
    }
}
