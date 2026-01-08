using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Marketing;
using KGERP.Service.ServiceModel;
using KGERP.Services.Procurement;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{

    [CheckSession]
    public class WareHouseController : Controller
    {
        private HttpContext httpContext;
        private readonly WareHouseService _service;
        private readonly ConfigurationService _configurationService;
        private readonly ProcurementService _procurementService;


        public WareHouseController(WareHouseService wareHouseService, ConfigurationService configurationService, ProcurementService procurementService)
        {
            _service = wareHouseService;
            _configurationService = configurationService;
            _procurementService = procurementService;
        }


        public async Task<ActionResult> GetCommonProductSubCategory(int id)

        {
            if (id < 0) { return RedirectToAction("Error", "Home"); }

            var model = await Task.Run(() => _service.CommonProductSubCategoryGet(id));
            var list = model.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetCommonProduct(int id)
        {
            if (id < 0) { return RedirectToAction("Error", "Home"); }

            var model = await Task.Run(() => _service.CommonProductGet(id));
            var list = model.Select(x => new { Value = x.ID, Text = x.Name }).ToList();

            // _logger.LogInformation("--From: " + ControllerContext.ActionDescriptor.ActionName + "RawItemGet(id), RawItemGet Get by RawSubCategoryid).");
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> WareHousePurchaseOrder(int companyId, DateTime? fromDate, DateTime? toDate, string number = "0")
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2);

            if (!toDate.HasValue) toDate = DateTime.Now;

            var vmWareHousePOReceivingSlave = await _service.WareHousePurchaseOrderGet(companyId, fromDate, toDate);
            return View(vmWareHousePOReceivingSlave);
        }

        [HttpPost]

        public async Task<ActionResult> WareHousePurchaseOrder(VMWareHousePOReceivingSlave vmModel)
        {
            if (vmModel.CompanyId > 0)
            {
                Session["CompanyId"] = vmModel.CompanyId;
            }
            vmModel.FromDate = Convert.ToDateTime(vmModel.StrFromDate);
            vmModel.ToDate = Convert.ToDateTime(vmModel.StrToDate);

            return RedirectToAction(nameof(WareHousePurchaseOrder), new { companyId = vmModel.CompanyId, fromDate = vmModel.FromDate, toDate = vmModel.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> WareHousePOSlaveReceivingDetailsByPO(int id)
        {
            VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave();
            if (id > 0)
            {
                vmWareHousePOReceivingSlave = await _service.WareHousePOSlaveReceivingDetailsByPOGet(id);
            }
            return View(vmWareHousePOReceivingSlave);
        }
        [HttpGet]
        public async Task<ActionResult> WareHousePOReceivingSlaveReport(int companyId, int materialReceiveId = 0)
        {
            VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave();
            vmWareHousePOReceivingSlave = await _service.WareHousePOReceivingSlaveGet(companyId, materialReceiveId);

            return View(vmWareHousePOReceivingSlave);
        }
        [HttpGet]
        public async Task<ActionResult> WareHousePOReceivingSlave(int companyId, int materialReceiveId = 0)
        {
            VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave();
            SelectModel models = new SelectModel();

            if (materialReceiveId == 0)
            {
                vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave()
                {
                    ChallanDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (materialReceiveId > 0)
            {
                vmWareHousePOReceivingSlave = await _service.WareHousePOReceivingSlaveGet(companyId, materialReceiveId);
            }
            vmWareHousePOReceivingSlave.PurchaseOrders = new List<SelectModel>();
            vmWareHousePOReceivingSlave.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmWareHousePOReceivingSlave);
        }

        [HttpPost]
        public async Task<ActionResult> WareHousePOReceivingSlave(VMWareHousePOReceivingSlave vmModel, VMWareHousePOReceivingSlavePartial recvingSlavePartial)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.MaterialReceiveId == 0)
                {
                    vmModel.MaterialReceiveId = await _service.WareHousePOReceivingAdd(vmModel);
                }
                await _service.WareHousePOReceivingSlaveAdd(vmModel, recvingSlavePartial);
            }

            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitMaterialReceive(vmModel);


            }

            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(WareHousePOReceivingSlave), new { companyId = vmModel.CompanyFK, materialReceiveId = vmModel.MaterialReceiveId });
        }
        [HttpGet]
        public async Task<ActionResult> WareHousePOSalesReturnListByPO(int id = 0)
        {
            VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave();
            vmWareHousePOReceivingSlave = await _service.WareHousePOSlaveReturnDetailsByPOGet(id);

            return View(vmWareHousePOReceivingSlave);
        }
        [HttpGet]
        public async Task<ActionResult> WareHousePurchaseReturnSlaveReport(int companyId = 0, int materialReceiveId = 0)
        {
            VMWareHousePOReturnSlave vmWareHousePOReturnSlave = new VMWareHousePOReturnSlave();
            vmWareHousePOReturnSlave = await _service.WareHousePOReturnSlaveGet(companyId, materialReceiveId);

            return View(vmWareHousePOReturnSlave);
        }



        [HttpGet]
        public async Task<ActionResult> WareHousePurchaseReturnSlave(int companyId = 0, int purchaseReturnId = 0)
        {
            VMWareHousePOReturnSlave vmWareHousePOReturnSlave = new VMWareHousePOReturnSlave();
            if (purchaseReturnId == 0)
            {
                vmWareHousePOReturnSlave = new VMWareHousePOReturnSlave()
                {
                    ChallanDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (purchaseReturnId > 0)
            {
                vmWareHousePOReturnSlave = await _service.WareHousePOReturnSlaveGet(companyId, purchaseReturnId);
            }
            vmWareHousePOReturnSlave.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmWareHousePOReturnSlave);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingPurchaseReturnSlave(int companyId = 0, int purchaseReturnId = 0)
        {
            VMWareHousePOReturnSlave vmWareHousePOReturnSlave = new VMWareHousePOReturnSlave();
            if (purchaseReturnId == 0)
            {
                vmWareHousePOReturnSlave = new VMWareHousePOReturnSlave()
                {
                    ChallanDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (purchaseReturnId > 0)
            {
                vmWareHousePOReturnSlave = await _service.WareHousePOReturnSlaveGet(companyId, purchaseReturnId);
            }
            vmWareHousePOReturnSlave.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmWareHousePOReturnSlave);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingPurchaseReturnList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMWareHousePOReturnSlave model = new VMWareHousePOReturnSlave();
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            model = await _service.PackagingPurchaseReturnList(companyId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToShortDateString();
            model.StrToDate = toDate.Value.ToShortDateString();
            return View(model);
        }

        [HttpPost]
        public ActionResult PackagingPurchaseReturnList(VMWareHousePOReturnSlave vmModel)
        {
            if (vmModel.CompanyFK > 0)
            {
                Session["CompanyId"] = vmModel.CompanyFK;
            }

            vmModel.FromDate = Convert.ToDateTime(vmModel.StrFromDate);
            vmModel.ToDate = Convert.ToDateTime(vmModel.StrToDate);
            return RedirectToAction(nameof(PackagingPurchaseReturnList), new { companyId = vmModel.CompanyFK, fromDate = vmModel.FromDate, toDate = vmModel.ToDate });
        }

        [HttpPost]
        public async Task<ActionResult> WareHousePurchaseReturnSlave(VMWareHousePOReturnSlave vmModel, VMWareHousePOReturnSlavePartial vmModelList)
        {

            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.MaterialReceiveId > 0)
                {
                    vmModel.PurchaseReturnId = await _service.WareHousePOReturnAdd(vmModel);
                }

                await _service.WareHousePOReturnSlaveAdd(vmModel, vmModelList);
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitMaterialReturn(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(WareHousePurchaseReturnSlave), new { companyId = vmModel.CompanyFK, purchaseReturnId = vmModel.PurchaseReturnId });
        }

        [HttpPost]
        public async Task<ActionResult> PackagingPurchaseReturnSlave(VMWareHousePOReturnSlave vmModel, VMWareHousePOReturnSlavePartial vmModelList)
        {

            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.MaterialReceiveId > 0)
                {
                    vmModel.PurchaseReturnId = await _service.PackagingPOReturnAdd(vmModel);
                }

                await _service.PackagingPOReturnSlaveAdd(vmModel, vmModelList);
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitPackagingMaterialReturn(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(PackagingPurchaseReturnSlave), new { companyId = vmModel.CompanyFK, purchaseReturnId = vmModel.PurchaseReturnId });
        }

        [HttpGet]
        public async Task<ActionResult> KPLSalesReturnSlave(int companyId = 0, int saleReturnId = 0)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            if (saleReturnId == 0)
            {
                vmSaleReturnDetail = new VMSaleReturnDetail()
                {
                    ReturnDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (saleReturnId > 0)
            {
                vmSaleReturnDetail = await _service.KPLSalesReturnSlaveGet(companyId, saleReturnId);
            }

            vmSaleReturnDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmSaleReturnDetail);
        }

        [HttpPost]
        public async Task<ActionResult> KPLSalesReturnSlave(VMSaleReturnDetail vmModel, VMSaleReturnDetailPartial vmModelList)
        {

            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.SaleReturnId == 0)
                {
                    vmModel.SaleReturnId = await _service.WareHouseSaleReturnAdd(vmModel);
                }

                await _service.kplWareHouseSaleReturnDetailAdd(vmModel, vmModelList);
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSaleReturnByProduct(vmModel);


            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(KPLSalesReturnSlave), new { companyId = vmModel.CompanyFK, saleReturnId = vmModel.SaleReturnId });
        }


        [HttpGet]
        public async Task<ActionResult> WareHouseSalesReturnSlave(int companyId = 0, int saleReturnId = 0)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            if (saleReturnId == 0)
            {
                vmSaleReturnDetail = new VMSaleReturnDetail()
                {
                    ReturnDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (saleReturnId > 0)
            {
                vmSaleReturnDetail = await _service.WareHouseSalesReturnSlaveGet(companyId, saleReturnId);
            }

            vmSaleReturnDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmSaleReturnDetail);
        }

        [HttpPost]
        public async Task<ActionResult> WareHouseSalesReturnSlave(VMSaleReturnDetail vmModel, VMSaleReturnDetailPartial vmModelList)
        {

            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.SaleReturnId == 0)
                {
                    vmModel.SaleReturnId = await _service.WareHouseSaleReturnAdd(vmModel);
                }
                if (vmModel.SaleReturnId>0)
                {
                    await _service.WareHouseSaleReturnDetailAdd(vmModel, vmModelList);
                }

                
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSaleReturnByProduct(vmModel);


            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(WareHouseSalesReturnSlave), new { companyId = vmModel.CompanyFK, saleReturnId = vmModel.SaleReturnId });
        }

        [HttpGet]
        public async Task<ActionResult> WareHouseSalesReturnByProduct(int companyId, int saleReturnId = 0)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            if (saleReturnId == 0)
            {
                vmSaleReturnDetail = new VMSaleReturnDetail()
                {
                    ReturnDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (saleReturnId > 0)
            {
                vmSaleReturnDetail = await _service.WareHouseSalesReturnSlaveGet(companyId, saleReturnId);
            }
            vmSaleReturnDetail.SubZoneList = new SelectList(_procurementService.SubZonesDropDownList(companyId), "Value", "Text");
            vmSaleReturnDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmSaleReturnDetail);
        }

        [HttpPost]
        public async Task<ActionResult> WareHouseSalesReturnByProduct(VMSaleReturnDetail vmModel)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                var checkCogs = _service.FinishProductCogsGet(vmModel.ProductId??0,vmModel.CompanyFK,vmModel.LotNumber);
                if (checkCogs.ClosingRate<=0)
                {
                    TempData["ErrorMessage"] = "COGS Rate Not Found For This Product.Please Set COGS Rate First.";
                    return RedirectToAction(nameof(WareHouseSalesReturnByProduct), new { companyId = vmModel.CompanyFK, saleReturnId = vmModel.SaleReturnId });
                }
                else
                {
                    vmModel.COGSRate = checkCogs.ClosingRate;
                }

                if (vmModel.SaleReturnId == 0)
                {
                    vmModel.SaleReturnId = await _service.WareHouseSaleReturnAdd(vmModel);
                    if (vmModel.SaleReturnId>0)
                    {
                        await _service.WareHouseSaleReturnByProductAdd(vmModel);
                    }
                }
                //if (vmModel.CompanyFK == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
                //{
                //    await _service.KfmalSaleReturnByProductAdd(vmModel);

                //}
                else
                {
                    await _service.WareHouseSaleReturnByProductAdd(vmModel);

                }
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSaleReturnByProduct(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(WareHouseSalesReturnByProduct), new { companyId = vmModel.CompanyFK, saleReturnId = vmModel.SaleReturnId });
        }



        [HttpGet]
        public async Task<ActionResult> WareHouseSalesReturnByProductList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2); ;

            if (!toDate.HasValue) toDate = DateTime.Now;
            VMSaleReturn vmSaleReturn = new VMSaleReturn();
            vmSaleReturn = await _service.KFMALWareHouseSalesReturnGet(companyId, fromDate, toDate);


            vmSaleReturn.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmSaleReturn.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(vmSaleReturn);
        }
        [HttpPost]
        public async Task<ActionResult> WareHouseSalesReturnByProductList(VMSaleReturn vMSaleReturn)
        {
            vMSaleReturn.FromDate = Convert.ToDateTime(vMSaleReturn.StrFromDate);
            vMSaleReturn.ToDate = Convert.ToDateTime(vMSaleReturn.StrToDate);

            return RedirectToAction(nameof(WareHouseSalesReturnByProduct), new { companyId = vMSaleReturn.CompanyId, fromDate = vMSaleReturn.FromDate, toDate = vMSaleReturn.ToDate });
        }




        [HttpGet]
        public async Task<ActionResult> GCCLWareHouseSalesReturnByProduct(int companyId, int saleReturnId = 0)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            if (saleReturnId == 0)
            {
                vmSaleReturnDetail = new VMSaleReturnDetail()
                {
                    ReturnDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (saleReturnId > 0)
            {
                vmSaleReturnDetail = await _service.WareHouseSalesReturnSlaveGet(companyId, saleReturnId);
            }
            vmSaleReturnDetail.SubZoneList = new SelectList(_procurementService.SubZonesDropDownList(companyId), "Value", "Text");
            vmSaleReturnDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");


            return View(vmSaleReturnDetail);
        }
        [HttpPost]
        public async Task<ActionResult> GCCLWareHouseSalesReturnByProduct(VMSaleReturnDetail vmModel)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {

                if (vmModel.SaleReturnId == 0)
                {
                    vmModel.SaleReturnId = await _service.WareHouseSaleReturnAdd(vmModel);
                }
                await _service.WareHouseSaleReturnByProductAdd(vmModel);
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSaleReturnByProduct(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(GCCLWareHouseSalesReturnByProduct), new { companyId = vmModel.CompanyFK, saleReturnId = vmModel.SaleReturnId });
        }



        [HttpGet]
        public async Task<ActionResult> GCCLWareHouseSalesReturnSlave(int companyId = 0, int saleReturnId = 0)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            if (saleReturnId == 0)
            {
                vmSaleReturnDetail = new VMSaleReturnDetail()
                {
                    ReturnDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (saleReturnId > 0)
            {
                vmSaleReturnDetail = await _service.WareHouseSalesReturnSlaveGet(companyId, saleReturnId);
            }
            vmSaleReturnDetail.SubZoneList = new SelectList(_procurementService.SubZonesDropDownList(companyId), "Value", "Text");
            vmSaleReturnDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmSaleReturnDetail);
        }

        [HttpPost]
        public async Task<ActionResult> GCCLWareHouseSalesReturnSlave(VMSaleReturnDetail vmModel, VMSaleReturnDetailPartial vmModelList)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.SaleReturnId == 0)
                {
                    vmModel.SaleReturnId = await _service.WareHouseSaleReturnAdd(vmModel);
                }

                await _service.WareHouseSaleReturnDetailAdd(vmModel, vmModelList);
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSaleReturnByProduct(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(GCCLWareHouseSalesReturnSlave), new { companyId = vmModel.CompanyFK, saleReturnId = vmModel.SaleReturnId });
        }




        [HttpGet]
        public async Task<ActionResult> KfmalSalesReturn(int companyId = 0, int saleReturnId = 0)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            if (saleReturnId == 0)
            {
                vmSaleReturnDetail = new VMSaleReturnDetail()
                {
                    ReturnDate = DateTime.Today,
                    CompanyFK = companyId
                };
            }
            else if (saleReturnId > 0)
            {
                vmSaleReturnDetail = await _service.WareHouseSalesReturnSlaveGet(companyId, saleReturnId);
            }
            vmSaleReturnDetail.SubZoneList = new SelectList(_procurementService.SubZonesDropDownList(companyId), "Value", "Text");
            vmSaleReturnDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmSaleReturnDetail);
        }

        [HttpPost]
        public async Task<ActionResult> KfmalSalesReturn(VMSaleReturnDetail vmModel, VMSaleReturnDetailPartial vmModelList)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.SaleReturnId == 0)
                {
                    vmModel.SaleReturnId = await _service.WareHouseSaleReturnAdd(vmModel);
                }

                await _service.WareHouseSaleReturnDetailAdd(vmModel, vmModelList);
            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSaleReturnByProduct(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(GCCLWareHouseSalesReturnSlave), new { companyId = vmModel.CompanyFK, saleReturnId = vmModel.SaleReturnId });
        }




        public async Task<ActionResult> GetExistingChallanListByPO(int id)
        {
            if (id < 0) { return RedirectToAction("Error", "Home"); }

            var model = await Task.Run(() => _service.GetExistingChallanListByPOData(id));
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetExistingChallanListByOrderMasters(int id)
        {
            if (id < 0) { return RedirectToAction("Error", "Home"); }

            var model = await Task.Run(() => _service.GetExistingChallanListByOrderMaster(id));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompletePOGet(int companyId, string prefix)
        {
            var products = _service.GetAutoCompletePO(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPO(int vendorId)
        {
            var products = _service.GetPO(vendorId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }



        public JsonResult SupplierWisePoListGet(int companyId, int supplierId)
        {
            var products = _service.GetSupplierWisePoList(companyId, supplierId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteOrderMastersGet(int companyId, string prefix)
        {
            var products = _service.GetAutoCompleteOrderMasters(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FeedOrderMastersGet(int companyId, string prefix)
        {
            var products = _service.GetFeedOrderMasters(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ProcurementPurchaseOrderDetails(int id)
        {
            VMWareHousePOReceivingSlave model = new VMWareHousePOReceivingSlave();
            model = await _service.GetProcurementPurchaseOrder(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> OrderMastersGet(int id)
        {
            VMOrderMaster model = new VMOrderMaster();
            model = await _service.GetOrderMasters(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OrderMastersById(int orderMasterId, string productType)
        {
            VMOrderMaster model = new VMOrderMaster();
            model = _service.GetFeedOrderMasterById(orderMasterId, productType);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetCommonUnitByItem(int id)
        {
            VMCommonUnit model = new VMCommonUnit();
            model = await _service.GetCommonUnitByItem(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProcurementPOReturnSlaveData(int poReceivingId)
        {
            var model = new VMWareHousePOReturnSlavePartial();
            model.DataListSlavePartial = _service.GetPOReturnData(poReceivingId);
            return PartialView("_PurchaseReturnSlaveData", model);
        }

        public ActionResult ProcurementSalesOrderReturnSlaveData(int orderDeliverId)
        {
            var model = new VMSaleReturnDetailPartial();
            model.DataToList = _service.GetSalesOrderReturnData(orderDeliverId);
            return PartialView("_SalesOrderReturnSlaveData", model);
        }
        public ActionResult ProcurementPurchaseOrderSlaveData(int poId)
        {
            var model = new VMWareHousePOReceivingSlavePartial();
            if (poId > 0)
            {

                model.DataListSlavePartial = _service.GetProcurementPurchaseOrderSlaveData(poId);
            }
            return PartialView("_ProcurementPurchaseOrderSlaveData", model);
        }

        public ActionResult GetPackagingPurchaseOrderSlaveData(int poId)
        {
            var model = new VMWareHousePOReceivingSlavePartial();
            if (poId > 0)
            {
                model.DataListSlavePartial = _service.GetPackagingPurchaseOrderSlaveData(poId);
            }
            return PartialView("_PackagingPurchaseOrderSlaveData", model);
        }

        public ActionResult ProcurementPurchaseOrderSlaveData2(int poId)
        {
            var model = new VMWareHousePOReceivingSlavePartial();
            if (poId > 0)
            {
                //model = _service.GetPODetailsByID(poId);
                model.DataListSlavePartial = _service.GetProcurementPurchaseOrderSlaveData(poId);

            }
            var list = model.DataListSlavePartial.Take(1);

            return Json(list);
        }
        public ActionResult GetOrderDetailsDataPartial(int orderMasterId)
        {
            var model = new VMOrderDeliverDetailPartial();
            if (orderMasterId > 0)
            {
                model.DataToList = _service.GetOrderDetails(orderMasterId);
            }
            return PartialView("_OrderDetailsDataPartial", model);
        }

        public ActionResult GetOrderDeliveryDetailsDataPartial(long orderDeliveryId)
        {
            var model = new VMOrderDeliverDetailDataPartial();
            if (orderDeliveryId > 0)
            {
                model.DataToList = _service.GetOrderDeliveryDetails(orderDeliveryId);
            }
            return PartialView("_OrderDeliverDetailDataPartial", model);
        }


        public ActionResult GetFeedOrderDetailsPartial(int poId)
        {
            var model = new VMOrderDeliverDetailPartial();
            if (poId > 0)
            {
                model.DataToList = _service.GetFeedOrderDetails(poId);
            }
            return PartialView("_FeedOrderDetailsPartial", model);
        }

        public ActionResult GetSeedOrderDetailsDataPartial(int poId)
        {
            var model = new VMOrderDeliverDetailPartial();
            if (poId > 0)
            {

                model.DataToList = _service.GetSeedOrderDetails(poId);
                //-- vai push disi
            }
            return PartialView("_OrderDetailsDataPartial", model);
        }
        [HttpGet]
        public async Task<ActionResult> WareHouseOrderDeliverDetailReport(int companyId, int orderDeliverId = 0)
        {
            VMOrderDeliverDetail vmOrderDeliverDetail = new VMOrderDeliverDetail();
            vmOrderDeliverDetail = await _service.WareHouseOrderDeliverDetailGet(companyId, orderDeliverId);

            return View(vmOrderDeliverDetail);
        }




        [HttpGet]
        public async Task<ActionResult> WareHouseOrderDeliverDetail(int companyId, int orderDeliverId = 0)
        {
            VMOrderDeliverDetail vmOrderDeliverDetail = new VMOrderDeliverDetail();
            if (orderDeliverId == 0)
            {
                vmOrderDeliverDetail.CompanyFK = companyId;
            }
            else if (orderDeliverId > 0)
            {
                vmOrderDeliverDetail = await _service.WareHouseOrderDeliverDetailGet(companyId, orderDeliverId);
            }
            vmOrderDeliverDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmOrderDeliverDetail);
        }




        [HttpPost]
        public async Task<ActionResult> WareHouseOrderDeliverDetail(VMOrderDeliverDetail vmModel, VMOrderDeliverDetailPartial vmModelList)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.OrderDeliverId == 0)
                {
                    vmModel.OrderDeliverId = await _service.WareHouseOrderDeliversAdd(vmModel);
                }
                if (vmModel.OrderDeliverId > 0)
                {
                    await _service.WareHouseOrderDeliverDetailAdd(vmModel, vmModelList);
                }

            }
            //else if (model.ActionEum == ActionEnum.Edit)
            //{
            //    //Edit
            //    await _service.ShipmentDeliveryChallanSlaveEdit(model);
            //}
            //else if (model.ActionEum == ActionEnum.DeleteWareHouseSalesReturnList
            //{
            //    //Delete
            //    await _service.ShipmentDeliveryChallanSlaveDelete(model.ID);
            //}
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitOrderDeliver(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(WareHouseOrderDeliverDetail), new { companyId = vmModel.CompanyFK, orderDeliverId = vmModel.OrderDeliverId });
        }


        [HttpGet]
        public async Task<ActionResult> Salestransfer(int companyId, long salesTransfer = 0)
        {
            SalesTransferDetailVM SalesTransferDetail = new SalesTransferDetailVM();
            if (salesTransfer == 0)
            {
                
                SalesTransferDetail.CustomerList = new SelectList(_procurementService.CustomerLisByCompany(companyId), "Value", "Text");
            }
            else if (salesTransfer > 0)
            {
                SalesTransferDetail = await _service.WareHouseSalesTranserDetailGet(companyId, salesTransfer);
            }
            SalesTransferDetail.CompanyFK = companyId;




            return View(SalesTransferDetail);
        }

        [HttpPost]
        public async Task<ActionResult> Salestransfer(SalesTransferDetailVM vmModel, VMOrderDeliverDetailDataPartial vmModelList)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.SalesTransferId == 0)
                {
                    vmModel.SalesTransferId = await _service.WareHouseSalestransferAdd(vmModel, vmModelList);
                }
                

            }

            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSalesTranser(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(Salestransfer), new { companyId = vmModel.CompanyFK, salesTransfer = vmModel.SalesTransferId });
        }

        [HttpPost]
        public async Task<ActionResult> OrderDeliverDetailIdDelete(VMOrderDeliverDetail vMOrderDeliverDetail)
        {
            vMOrderDeliverDetail.OrderDeliverId = await _service.OrderDeliveryDelete(vMOrderDeliverDetail.OrderDeliverDetailId);
            return RedirectToAction(nameof(WareHouseOrderDeliverDetail), new { companyId = vMOrderDeliverDetail.CompanyFK, orderDeliverId = vMOrderDeliverDetail.OrderDeliverId });
        }

        [HttpPost]
        public async Task<ActionResult> SalesTransferDetailDelete(SalesTransferDetailVM salesTransferDetailVM)
        {
            salesTransferDetailVM.SalesTransferId = await _service.SalesTransferDetailDelete(salesTransferDetailVM.SalesTransferDetailsId);
            return RedirectToAction(nameof(Salestransfer), new { companyId = salesTransferDetailVM.CompanyFK, salesTransfer = salesTransferDetailVM.SalesTransferId });
        }

        [HttpPost]
        public async Task<ActionResult> OrderDeliverDetailUpdate(VMOrderDeliverDetail vMOrderDeliverDetail)
        {
            vMOrderDeliverDetail.OrderDeliverId = await _service.OrderDeliveryUpdate(vMOrderDeliverDetail.OrderDeliverDetailId, vMOrderDeliverDetail.DeliveredQty);
            return RedirectToAction(nameof(WareHouseOrderDeliverDetail), new { companyId = vMOrderDeliverDetail.CompanyFK, orderDeliverId = vMOrderDeliverDetail.OrderDeliverId });
        }

        [HttpPost]
        public async Task<ActionResult> SalesTransferDetailUpdate(SalesTransferDetailVM salesTransferDetailVM)
        {
            salesTransferDetailVM.SalesTransferId = await _service.SalesTransferDetailUpdate(salesTransferDetailVM.SalesTransferDetailsId, salesTransferDetailVM.TransferQuantity.Value);
            return RedirectToAction(nameof(Salestransfer), new { companyId = salesTransferDetailVM.CompanyFK, salesTransfer = salesTransferDetailVM.SalesTransferId });
        }

        [HttpGet]
        public async Task<ActionResult> WareHousSaleReturnSlaveReport(int companyId = 0, int saleReturnId = 0)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            vmSaleReturnDetail = await _service.WareHouseSalesReturnSlaveGet(companyId, saleReturnId);
            return View(vmSaleReturnDetail);
        }

       


        [HttpGet]
        public async Task<ActionResult> WareHouseSalesReturnList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2); ;

            if (!toDate.HasValue) toDate = DateTime.Now;
            VMSaleReturn vmSaleReturn = new VMSaleReturn();
            vmSaleReturn = await _service.WareHouseSalesReturnGet(companyId, fromDate, toDate);


            vmSaleReturn.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmSaleReturn.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(vmSaleReturn);
        }
        [HttpPost]
        public async Task<ActionResult> WareHouseSalesReturnList(VMSaleReturn vMSaleReturn)
        {
            vMSaleReturn.FromDate = Convert.ToDateTime(vMSaleReturn.StrFromDate);
            vMSaleReturn.ToDate = Convert.ToDateTime(vMSaleReturn.StrToDate);

            return RedirectToAction(nameof(WareHouseSalesReturnList), new { companyId = vMSaleReturn.CompanyId, fromDate = vMSaleReturn.FromDate, toDate = vMSaleReturn.ToDate });
        }


        [HttpGet]
        public async Task<ActionResult> KPLSalesReturnList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2); ;

            if (!toDate.HasValue) toDate = DateTime.Now;
            VMSaleReturn vmSaleReturn = new VMSaleReturn();
            vmSaleReturn = await _service.WareHouseSalesReturnGet(companyId, fromDate, toDate);


            vmSaleReturn.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmSaleReturn.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(vmSaleReturn);
        }
        [HttpPost]
        public async Task<ActionResult> KPLSalesReturnList(VMSaleReturn vMSaleReturn)
        {
            vMSaleReturn.FromDate = Convert.ToDateTime(vMSaleReturn.StrFromDate);
            vMSaleReturn.ToDate = Convert.ToDateTime(vMSaleReturn.StrToDate);

            return RedirectToAction(nameof(KPLSalesReturnList), new { companyId = vMSaleReturn.CompanyId, fromDate = vMSaleReturn.FromDate, toDate = vMSaleReturn.ToDate });
        }


        [HttpGet]
        public async Task<ActionResult> WareHouseOrderDeliverList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);
            VMOrderDeliver vmOrderDeliver = new VMOrderDeliver();
            vmOrderDeliver = await _service.WareHouseOrderDeliverGet(companyId, fromDate, toDate);


            vmOrderDeliver.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmOrderDeliver.StrToDate = toDate.Value.ToString("yyyy-MM-dd");


            return View(vmOrderDeliver);
        }

        [HttpGet]
        public async Task<ActionResult> SalestransferList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);
            SalesTransferDetailVM vmOrderDeliver = new SalesTransferDetailVM();
            vmOrderDeliver = await _service.WareHouseSalesTransferGet(companyId, fromDate, toDate);


            vmOrderDeliver.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmOrderDeliver.StrToDate = toDate.Value.ToString("yyyy-MM-dd");


            return View(vmOrderDeliver);
        }

        [HttpPost]
        public ActionResult SalestransferList(SalesTransferDetailVM vmOrderDeliver)
        {
            vmOrderDeliver.FromDate = Convert.ToDateTime(vmOrderDeliver.StrFromDate);
            vmOrderDeliver.ToDate = Convert.ToDateTime(vmOrderDeliver.StrToDate);
            return RedirectToAction(nameof(SalestransferList), new { companyId = vmOrderDeliver.CompanyFK, fromDate = vmOrderDeliver.FromDate, toDate = vmOrderDeliver.ToDate });
        }

        [HttpPost]
        public async Task<ActionResult> WareHouseOrderDeliverList(VMOrderDeliver vmOrderDeliver)
        {
            vmOrderDeliver.FromDate = Convert.ToDateTime(vmOrderDeliver.StrFromDate);
            vmOrderDeliver.ToDate = Convert.ToDateTime(vmOrderDeliver.StrToDate);
            return RedirectToAction(nameof(WareHouseOrderDeliverList), new { companyId = vmOrderDeliver.CompanyFK, fromDate = vmOrderDeliver.FromDate, toDate = vmOrderDeliver.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> KFMALOrderDeliverList(int companyId)
        {
            VMOrderDeliver vmOrderDeliver = new VMOrderDeliver();
            vmOrderDeliver = await _service.KFMALWareHouseOrderDeliverGet(companyId);
            return View(vmOrderDeliver);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingOrderDeliverList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMOrderDeliver vmOrderDeliver = new VMOrderDeliver();

            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            vmOrderDeliver = await _service.PackagingOrderDeliverGet(companyId, fromDate, toDate);
            vmOrderDeliver.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmOrderDeliver.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vmOrderDeliver.CompanyFK = companyId;
            return View(vmOrderDeliver);
        }
        [HttpPost]
        public async Task<ActionResult> PackagingOrderDeliverList(VMOrderDeliver vmOrderDeliver)
        {
            if (vmOrderDeliver.CompanyFK > 0)
            {
                Session["CompanyId"] = vmOrderDeliver.CompanyFK;
            }

            vmOrderDeliver.FromDate = Convert.ToDateTime(vmOrderDeliver.StrFromDate);
            vmOrderDeliver.ToDate = Convert.ToDateTime(vmOrderDeliver.StrToDate);
            return RedirectToAction(nameof(PackagingOrderDeliverList), new { companyId = vmOrderDeliver.CompanyFK, fromDate = vmOrderDeliver.FromDate, toDate = vmOrderDeliver.ToDate });



        }


        [HttpGet]
        public ActionResult WareHouseFinishProductInventory(int companyId)
        {
            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct.CompanyFK = companyId;
            vmCommonProduct.ProductCategoryList = new SelectList(_service.ProductCategoryDropDownList(companyId, "F"), "Value", "Text");

            return View(vmCommonProduct);
        }

        [HttpPost]
        public async Task<ActionResult> WareHouseFinishProductInventoryView(VMCommonProduct vmCommonProduct)
        {
            vmCommonProduct = await _service.WareHouseFinishProductInventoryGet(vmCommonProduct);
            return View(vmCommonProduct);

        }

        [HttpGet]
        public async Task<ActionResult> WareHouseFinishProductInventoryDetails(int companyId, int productId)
        {
            VmInventoryDetails vmInventoryDetails = new VmInventoryDetails();
            vmInventoryDetails.VMCommonProduct = new VMCommonProduct();
            vmInventoryDetails.FromDate = DateTime.Now.AddDays(-30);
            vmInventoryDetails.ToDate = DateTime.Now;
            vmInventoryDetails.ProductFK = productId;
            vmInventoryDetails.CompanyFK = companyId;
            vmInventoryDetails.VMCommonProduct = await Task.Run(() => _service.GetProductById(productId));


            return View(vmInventoryDetails);
        }
        [HttpPost]
        public async Task<ActionResult> WareHouseFinishProductInventoryDetailsView(VmInventoryDetails vmInventoryDetails)
        {
            var vmCommonSupplierLedger = await Task.Run(() => _service.GetLedgerInfoByFinishProduct(vmInventoryDetails));
            return View(vmCommonSupplierLedger);
        }

        [HttpGet]
        public async Task<ActionResult> WareHouseRawProductInventoryDetails(int companyId, int productId)
        {
            VmInventoryDetails vmInventoryDetails = new VmInventoryDetails();
            vmInventoryDetails.VMCommonProduct = new VMCommonProduct();
            vmInventoryDetails.FromDate = DateTime.Now.AddDays(-30);
            vmInventoryDetails.ToDate = DateTime.Now;
            vmInventoryDetails.ProductFK = productId;
            vmInventoryDetails.CompanyFK = companyId;
            vmInventoryDetails.VMCommonProduct = await Task.Run(() => _service.GetProductById(productId));


            return View(vmInventoryDetails);
        }
        [HttpPost]
        public async Task<ActionResult> WareHouseRawProductInventoryDetailsView(VmInventoryDetails vmInventoryDetails)
        {
            var vmCommonSupplierLedger = await Task.Run(() => _service.GetLedgerInfoByRawProduct(vmInventoryDetails));
            return View(vmCommonSupplierLedger);
        }
        [HttpGet]
        public ActionResult WareHouseRawItemInventory(int companyId)
        {
            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct.CompanyFK = companyId;

            vmCommonProduct.ProductCategoryList = new SelectList(_service.ProductCategoryDropDownList(companyId, "R"), "Value", "Text");

            return View(vmCommonProduct);
        }
        [HttpPost]
        public async Task<ActionResult> WareHouseRawItemInventoryView(VMCommonProduct vmCommonProduct)
        {
            vmCommonProduct = await _service.WareHouseRawItemInventoryGet(vmCommonProduct);
            return View(vmCommonProduct);
        }

        [HttpGet]
        public async Task<ActionResult> ProcurementSalesItemAdjustmentList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddMonths(-2);
            if (!toDate.HasValue) toDate = DateTime.Now;
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            vmStockAdjustDetail = new VMStockAdjustDetail()
            {
                CompanyFK = companyId
            };
            vmStockAdjustDetail = await _service.WareHouseOrderItemAdjustmentGet(companyId, fromDate, toDate);
            vmStockAdjustDetail.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmStockAdjustDetail.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(vmStockAdjustDetail);
        }
        [HttpPost]
        public async Task<ActionResult> ProcurementSalesItemAdjustmentList(VMStockAdjustDetail vMAdjustDetails)
        {

            vMAdjustDetails.FromDate = Convert.ToDateTime(vMAdjustDetails.StrFromDate);
            vMAdjustDetails.ToDate = Convert.ToDateTime(vMAdjustDetails.StrToDate);

            return RedirectToAction(nameof(ProcurementSalesItemAdjustmentList), new { companyId = vMAdjustDetails.CompanyId, fromDate = vMAdjustDetails.FromDate, toDate = vMAdjustDetails.ToDate });
        }

        [HttpGet]
        public async Task<ActionResult> KFMALProcurementSalesItemAdjustmentList(int companyId)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            vmStockAdjustDetail = new VMStockAdjustDetail()
            {
                CompanyFK = companyId
            };
            vmStockAdjustDetail = await _service.KfmalWareHouseOrderItemAdjustmentGet(companyId);

            return View(vmStockAdjustDetail);
        }


        [HttpGet]
        public async Task<ActionResult> ProcurementSalesItemAdjustmentReport(int companyId, int stockAdjustId = 0)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            if (stockAdjustId == 0)
            {
                vmStockAdjustDetail = new VMStockAdjustDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (stockAdjustId > 0)
            {
                vmStockAdjustDetail = await _service.WareHouseOrderItemAdjustmentDetailGet(companyId, stockAdjustId);
            }
            return View(vmStockAdjustDetail);
        }


        [HttpGet]
        public async Task<ActionResult> KFMALProcurementSalesItemAdjustmentReport(int companyId, int stockAdjustId = 0)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            if (stockAdjustId == 0)
            {
                vmStockAdjustDetail = new VMStockAdjustDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (stockAdjustId > 0)
            {
                vmStockAdjustDetail = await _service.KFMALWareHouseOrderItemAdjustmentDetailGet(companyId, stockAdjustId);
            }
            return View(vmStockAdjustDetail);
        }

        [HttpGet]
        public async Task<ActionResult> ProcurementSalesItemAdjustment(int companyId, int stockAdjustId = 0)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            if (stockAdjustId == 0)
            {
                vmStockAdjustDetail = new VMStockAdjustDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (stockAdjustId > 0)
            {
                vmStockAdjustDetail = await _service.WareHouseOrderItemAdjustmentDetailGet(companyId, stockAdjustId);
            }
            return View(vmStockAdjustDetail);
        }



        [HttpGet]
        public async Task<ActionResult> KFMALProcurementSalesItemAdjustment(int companyId, int stockAdjustId = 0)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            if (stockAdjustId == 0)
            {
                vmStockAdjustDetail = new VMStockAdjustDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (stockAdjustId > 0)
            {
                vmStockAdjustDetail = await _service.KfmalItemAdjustmentDetailGet(companyId, stockAdjustId);
            }
            //return View(vmStockAdjustDetail);
            vmStockAdjustDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmStockAdjustDetail);
        }
        [HttpPost]
        public async Task<ActionResult> KFMALProcurementSalesItemAdjustment(VMStockAdjustDetail vmStockAdjustDetail)
        {
            if (vmStockAdjustDetail.ActionEum == ActionEnum.Add)
            {
                if (vmStockAdjustDetail.StockAdjustId == 0)
                {
                    vmStockAdjustDetail.StockAdjustId = await _service.StockAdjustAdd(vmStockAdjustDetail);
                }
                await _service.StockAdjustDetailAdd(vmStockAdjustDetail);
            }
            else if (vmStockAdjustDetail.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitStockAdjusts(vmStockAdjustDetail);

            }
            return RedirectToAction(nameof(KFMALProcurementSalesItemAdjustment), new { companyId = vmStockAdjustDetail.CompanyFK, stockAdjustId = vmStockAdjustDetail.StockAdjustId });
        }


        [HttpGet]
        public async Task<ActionResult> GCCLProcurementSalesItemAdjustment(int companyId, int stockAdjustId = 0)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            if (stockAdjustId == 0)
            {
                vmStockAdjustDetail = new VMStockAdjustDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (stockAdjustId > 0)
            {
                vmStockAdjustDetail = await _service.WareHouseOrderItemAdjustmentDetailGet(companyId, stockAdjustId);
            }
            return View(vmStockAdjustDetail);
        }

        [HttpPost]
        public async Task<ActionResult> GCCLProcurementSalesItemAdjustment(VMStockAdjustDetail vmStockAdjustDetail)
        {
            if (vmStockAdjustDetail.ActionEum == ActionEnum.Add)
            {
                if (vmStockAdjustDetail.StockAdjustId == 0)
                {
                    vmStockAdjustDetail.StockAdjustId = await _service.StockAdjustAdd(vmStockAdjustDetail);
                }
                await _service.StockAdjustDetailAdd(vmStockAdjustDetail);
            }
            else if (vmStockAdjustDetail.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitStockAdjusts(vmStockAdjustDetail);

            }
            return RedirectToAction(nameof(GCCLProcurementSalesItemAdjustment), new { companyId = vmStockAdjustDetail.CompanyFK, stockAdjustId = vmStockAdjustDetail.StockAdjustId });
        }
        [HttpPost]
        public async Task<ActionResult> ProcurementSalesItemAdjustment(VMStockAdjustDetail vmStockAdjustDetail)
        {
            if (vmStockAdjustDetail.ActionEum == ActionEnum.Add)
            {
                if (vmStockAdjustDetail.StockAdjustId == 0)
                {
                    vmStockAdjustDetail.StockAdjustId = await _service.StockAdjustAdd(vmStockAdjustDetail);
                }
                await _service.StockAdjustDetailAdd(vmStockAdjustDetail);
            }
            else if (vmStockAdjustDetail.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitStockAdjusts(vmStockAdjustDetail);

            }
            return RedirectToAction(nameof(ProcurementSalesItemAdjustment), new { companyId = vmStockAdjustDetail.CompanyFK, stockAdjustId = vmStockAdjustDetail.StockAdjustId });
        }
        [HttpGet]
        public async Task<ActionResult> KFMALWareHousePOReceivingSlave(int companyId, int materialReceiveId = 0)
        {
            VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave();
            vmWareHousePOReceivingSlave.PurchaseOrders = new List<SelectModel>();
            if (materialReceiveId == 0)
            {
                vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave()
                {
                    ChallanDate = DateTime.Today,
                    CompanyFK = companyId

                };
                vmWareHousePOReceivingSlave.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            }
            else if (materialReceiveId > 0)
            {
                var mrr = await _service.MaterialReceivesGetById(materialReceiveId);
                if (mrr.SupplierPaymentMethodId == (int)VendorsPaymentMethodEnum.LC)
                {
                    vmWareHousePOReceivingSlave = await _service.KFMALLCPOReceivingSlaveGet(companyId, materialReceiveId);

                }
                else
                {
                    vmWareHousePOReceivingSlave = await _service.KFMALReceivingSlaveGet(companyId, materialReceiveId);

                }

            }
            return View(vmWareHousePOReceivingSlave);
        }

        [HttpPost]
        public async Task<ActionResult> KFMALWareHousePOReceivingSlave(VMWareHousePOReceivingSlave vmModel, VMWareHousePOReceivingSlavePartial vmModelList)

        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.MaterialReceiveId == 0)
                {
                    vmModel.MaterialReceiveId = await _service.KFMALWareHousePOReceivingAdd(vmModel);
                }
                if (vmModel.SupplierPaymentMethodEnumFK == (int)VendorsPaymentMethodEnum.LC)
                {
                    await _service.KFMALLCPOReceivingSlaveAdd(vmModel, vmModelList);

                }
                else
                {
                    await _service.KFMALPOReceivingSlaveAdd(vmModel, vmModelList);

                }
            }

            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitMaterialReceive(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(KFMALWareHousePOReceivingSlave), new { companyId = vmModel.CompanyFK, materialReceiveId = vmModel.MaterialReceiveId });
        }



        [HttpGet]
        public async Task<ActionResult> GCCLWareHousePOReceivingSlave(int companyId, int materialReceiveId = 0)
        {
            VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave();
            if (materialReceiveId == 0)
            {
                vmWareHousePOReceivingSlave = new VMWareHousePOReceivingSlave()
                {
                    ChallanDate = DateTime.Today,
                    CompanyFK = companyId,
                    StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text")
                };
            }
            else if (materialReceiveId > 0)
            {
                vmWareHousePOReceivingSlave = await _service.WareHousePOReceivingSlaveGet(companyId, materialReceiveId);
            }
            return View(vmWareHousePOReceivingSlave);
        }

        [HttpPost]
        public async Task<ActionResult> GCCLWareHousePOReceivingSlave(VMWareHousePOReceivingSlave vmModel, VMWareHousePOReceivingSlavePartial vmModelList)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.MaterialReceiveId == 0)
                {
                    vmModel.MaterialReceiveId = await _service.WareHousePOReceivingAdd(vmModel);
                }
                await _service.GCCLWareHousePOReceivingSlaveAdd(vmModel, vmModelList);
            }

            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitMaterialReceive(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(GCCLWareHousePOReceivingSlave), new { companyId = vmModel.CompanyFK, materialReceiveId = vmModel.MaterialReceiveId });
        }

        [HttpPost]
        public async Task<ActionResult> FeedPOReceivingSubmit(MaterialReceiveViewModel vm)
        {

            long maretialReceiveID = await _service.FeedSubmitMaterialReceive(vm.VMReceivingSlave.MaterialReceiveId, vm.VMReceivingSlave.CompanyFK.Value);
            return RedirectToAction("MaterialIssueEdit", "MaterialReceive", new { companyId = vm.VMReceivingSlave.CompanyFK.Value, materialReceiveId = vm.VMReceivingSlave.MaterialReceiveId });
        }
        [HttpPost]
        public async Task<ActionResult> FeedIssueSubmit(IssueVm vm)
        {

            long issueID = await _service.FeedSubmitIssue(vm.IssueId, vm.CompanyId);
            return RedirectToAction("IssueSlave", "StockAdjust", new { companyId = vm.CompanyId, issueId = vm.IssueId });
        }


        [HttpGet]
        public async Task<ActionResult> GCCLWareHouseOrderDeliverDetail(int companyId, int orderDeliverId = 0)
        {
            VMOrderDeliverDetail vmOrderDeliverDetail = new VMOrderDeliverDetail();
            if (orderDeliverId == 0)
            {
                vmOrderDeliverDetail = new VMOrderDeliverDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (orderDeliverId > 0)
            {
                vmOrderDeliverDetail = await _service.WareHouseOrderDeliverDetailGet(companyId, orderDeliverId);
            }
            vmOrderDeliverDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmOrderDeliverDetail);
        }

        [HttpPost]
        public async Task<ActionResult> GCCLWareHouseOrderDeliverDetail(VMOrderDeliverDetail vmModel, VMOrderDeliverDetailPartial vmModelList)
        {
            int? CompanyFK = vmModel.CompanyFK;
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.OrderDeliverId == 0)
                {
                    vmModel.OrderDeliverId = await _service.WareHouseOrderDeliversAdd(vmModel);
                }
                await _service.WareHouseOrderDeliverDetailAdd(vmModel, vmModelList);
            }

            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                vmModel.OrderDeliverId = await _service.SubmitOrderDeliver(vmModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(GCCLWareHouseOrderDeliverDetail), new { companyId = CompanyFK, orderDeliverId = vmModel.OrderDeliverId });
        }




        [HttpGet]
        public async Task<ActionResult> FeedOrderDeliverDetail(int companyId, int orderDeliverId = 0)
        {
            VMOrderDeliverDetail vmOrderDeliverDetail = new VMOrderDeliverDetail();
            if (orderDeliverId == 0)
            {
                vmOrderDeliverDetail = new VMOrderDeliverDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (orderDeliverId > 0)
            {
                vmOrderDeliverDetail = await _service.FeedOrderDeliverDetailGet(companyId, orderDeliverId);
            }
            vmOrderDeliverDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmOrderDeliverDetail);
        }

        [HttpPost]
        public async Task<ActionResult> FeedOrderDeliverDetail(VMOrderDeliverDetail vmModel, VMOrderDeliverDetailPartial vmModelList)
        {
            int? CompanyFK = vmModel.CompanyFK;
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.OrderDeliverId == 0)
                {
                    vmModel.OrderDeliverId = await _service.FeedOrderDeliversAdd(vmModel);
                }
                if (vmModel.OrderDeliverId > 0)
                {
                    await _service.FeedOrderDeliverDetailAdd(vmModel, vmModelList);
                }

            }

            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                vmModel.OrderDeliverId = await _service.SubmitOrderDeliver(vmModel);


            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(FeedOrderDeliverDetail), new { companyId = CompanyFK, orderDeliverId = vmModel.OrderDeliverId });
        }

        [HttpPost]
        public async Task<ActionResult> FeedOrderDeliverSubmit(VMOrderDeliverDetail vmModel)
        {
            vmModel.OrderDeliverId = await _service.SubmitOrderDeliver(vmModel);
            //vmModel.OrderDeliverId = await _service.SubmitMultiOrderDeliver(vmModel);
            return RedirectToAction("FeedOrderDeliverDetail", "WareHouse", new { companyId = vmModel.CompanyFK, orderDeliverId = vmModel.OrderDeliverId });
        }

        [HttpGet]

        public JsonResult GetPurchaseOrder(int id)
        {
            var res = _service.GetPurchaseNo(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public JsonResult GetOrderMaster(int customerId)
        {
            var res = _service.GetOrderMasterNo(customerId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOrderDetailsByOrderMaster(long orderMasterId)
        {
            var res = _service.GetOrderDetailsByOrderMaster(orderMasterId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KFMALWareHousePODetails(int id)
        {

            var res = _service.KFMALWareHousePODetails(id);


            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PackagingWareHousePODetails(int id)
        {

            var res = _service.PackagingPODetails(id);


            return Json(res, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PackagingJobOrderetails(int id)
        {

            var res = _service.PackagingJobOrderDetails(id);


            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PackagingJobOrderetailsByOrderDetailsId(int id)
        {

            var res = _service.PackagingJobOrderetailsByOrderDetailsId(id);


            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSupplierAutoComplete(int companyId, string prefix)
        {
            //int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = _service.GetSupplierAutoComplete(prefix, companyId);

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult PurchaseReturnList(int CompanyId, DateTime? fromDate, DateTime? ToDate)
        {
            if (CompanyId > 0)
            {
                Session["CompanyId"] = CompanyId;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (ToDate == null)
            {
                ToDate = DateTime.Now;
            }
            PurchaseReturnVm vm = new PurchaseReturnVm();
            vm = _service.PurchaseReturnList(CompanyId, fromDate, ToDate);
            vm.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vm.StrToDate = ToDate.Value.ToString("yyyy-MM-dd");
            vm.CompanyId = CompanyId;
            return View(vm);
        }

        [HttpPost]

        public ActionResult PurchaseReturnList(PurchaseReturnVm model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(PurchaseReturnList), new { companyId = model.CompanyId, fromDate = model.FromDate, ToDate = model.ToDate });
        }

        public ActionResult ReceiveDetail(WareHouseIssueSlaveVm model)
        {

            return View();
        }



        [HttpGet]
        public async Task<ActionResult> GcclSampleProductInvoice(int companyId, int stockAdjustId = 0)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            if (stockAdjustId == 0)
            {
                vmStockAdjustDetail = new VMStockAdjustDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (stockAdjustId > 0)
            {
                vmStockAdjustDetail = await _service.WareHouseOrderItemAdjustmentDetailGet(companyId, stockAdjustId);
            }
            return View(vmStockAdjustDetail);
        }

        [HttpPost]
        public async Task<ActionResult> GcclSampleProductInvoice(VMStockAdjustDetail vmStockAdjustDetail)
        {
            if (vmStockAdjustDetail.ActionEum == ActionEnum.Add)
            {
                if (vmStockAdjustDetail.StockAdjustId == 0)
                {
                    vmStockAdjustDetail.StockAdjustId = await _service.StockAdjustForSampleProductAdd(vmStockAdjustDetail);
                }
                await _service.StockAdjustDetailAdd(vmStockAdjustDetail);
            }
            else if (vmStockAdjustDetail.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitSampleProductStockAdjusts(vmStockAdjustDetail);

            }
            return RedirectToAction(nameof(GcclSampleProductInvoice), new { companyId = vmStockAdjustDetail.CompanyFK, stockAdjustId = vmStockAdjustDetail.StockAdjustId });
        }

        [HttpGet]
        public async Task<ActionResult> KfmalOrderDeliver(int companyId, int orderDeliverId = 0)
        {
            VMOrderDeliverDetail vmOrderDeliverDetail = new VMOrderDeliverDetail();
            if (orderDeliverId == 0)
            {
                vmOrderDeliverDetail = new VMOrderDeliverDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (orderDeliverId > 0)
            {
                vmOrderDeliverDetail = await _service.WareHouseOrderDeliverDetailGet(companyId, orderDeliverId);
            }
            vmOrderDeliverDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmOrderDeliverDetail);
        }

        [HttpPost]
        public async Task<ActionResult> KfmalOrderDeliver(VMOrderDeliverDetail vmModel, VMOrderDeliverDetailPartial vmModelList)
        {
            int? CompanyFK = vmModel.CompanyFK;
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.OrderDeliverId == 0)
                {
                    vmModel.OrderDeliverId = await _service.WareHouseOrderDeliversAdd(vmModel);
                }
                await _service.WareHouseOrderDeliverDetailAdd(vmModel, vmModelList);
            }

            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                vmModel.OrderDeliverId = await _service.SubmitOrderDeliver(vmModel);
                //vmModel.OrderDeliverId = await _service.KFMALSubmitMultiOrderDeliver(vmModel);

            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(KfmalOrderDeliver), new { companyId = CompanyFK, orderDeliverId = vmModel.OrderDeliverId });
        }


        [HttpGet]
        public async Task<ActionResult> PackagingPOReceiving(int companyId, int materialReceiveId = 0)
        {

            VMWareHousePOReceivingSlave vmReceiving = new VMWareHousePOReceivingSlave();
            vmReceiving.PurchaseOrders = new List<SelectModel>();
            if (materialReceiveId == 0)
            {
                vmReceiving = new VMWareHousePOReceivingSlave()
                {
                    ChallanDate = DateTime.Today,
                    CompanyFK = companyId
                };
                vmReceiving.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");
            }
            else if (materialReceiveId > 0)
            {
                vmReceiving = await _service.ISSPOReceivingGet(companyId, materialReceiveId);
            }
            
            return View(vmReceiving);
        }

        [HttpPost]
        public async Task<ActionResult> PackagingPOReceiving(VMWareHousePOReceivingSlave vmModel, VMWareHousePOReceivingSlavePartial vmModelList)

        {

            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.MaterialReceiveId == 0)
                {
                    vmModel.MaterialReceiveId = await _service.PackagingPOReceivingAdd(vmModel);
                }

                await _service.PackagingPOReceivingSlaveAdd(vmModel, vmModelList);

            }
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitMaterialReceive(vmModel);

                //vmModel.MaterialReceiveId = await _service.SubmitMultiMRISS();
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(PackagingPOReceiving), new { companyId = vmModel.CompanyFK, materialReceiveId = vmModel.MaterialReceiveId });
        }



        [HttpPost]
        public ActionResult GetPackagingOrderDetailsDataPartial(int poId)
        {
            var model = new VMOrderDeliverDetailPartial();
            if (poId > 0)
            {
                model.DataToList = _service.GetPackagingOrderDetails(poId);
                //-- vai push disi
            }
            return PartialView("_PackagingOrderDetailsDataPartial", model);
        }

        [HttpGet]
        public async Task<ActionResult> PackagingWareHouseOrderDeliverDetail(int companyId, int orderDeliverId = 0)
        {
            VMOrderDeliverDetail vmOrderDeliverDetail = new VMOrderDeliverDetail();
            if (orderDeliverId == 0)
            {
                vmOrderDeliverDetail.CompanyFK = companyId;
            }
            else if (orderDeliverId > 0)
            {
                vmOrderDeliverDetail = await _service.PackagingWareHouseOrderDeliverDetailGet(companyId, orderDeliverId);
            }
            vmOrderDeliverDetail.StockInfoList = new SelectList(_procurementService.StockInfoesDropDownList(companyId), "Value", "Text");

            return View(vmOrderDeliverDetail);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateOrderDeliverDetail(VMOrderDeliverDetail models)
        {
            models.OrderDeliverId = await _service.UpdateOrderDeliverDetail(models);
            return RedirectToAction(nameof(PackagingWareHouseOrderDeliverDetail), new { companyId = models.CompanyFK, orderDeliverId = models.OrderDeliverId });
        }

        [HttpPost]
        public async Task<ActionResult> PackagingWareHouseOrderDeliverDetail(VMOrderDeliverDetail vmModel, VMOrderDeliverDetailPartial vmModelList)
        {
            if (vmModel.ActionEum == ActionEnum.Add)
            {
                if (vmModel.OrderDeliverId == 0)
                {
                    vmModel.OrderDeliverId = await _service.PackagingWareHouseOrderDeliversAdd(vmModel);
                }
                await _service.PackagingWareHouseOrderDeliverDetailAdd(vmModel, vmModelList);
            }
            //else if (model.ActionEum == ActionEnum.Edit)
            //{
            //    //Edit
            //    await _service.ShipmentDeliveryChallanSlaveEdit(model);
            //}
            //else if (model.ActionEum == ActionEnum.DeleteWareHouseSalesReturnList
            //{
            //    //Delete
            //    await _service.ShipmentDeliveryChallanSlaveDelete(model.ID);
            //}
            else if (vmModel.ActionEum == ActionEnum.Finalize)
            {
                await _service.PackagingSubmitOrderDeliver(vmModel);
                //await _service.SubmitMultiOrderDeliverPack();
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(PackagingWareHouseOrderDeliverDetail), new { companyId = vmModel.CompanyFK, orderDeliverId = vmModel.OrderDeliverId });
        }

        //ani
    }

}