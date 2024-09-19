using DocumentFormat.OpenXml.EMMA;
using KGERP;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.OrderApproval;
using KGERP.Services.Procurement;
using KGERP.Utility;
using log4net.Util.TypeConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pos.App.Controllers
{
    [CheckSession]
    //
    public class CollectionController : Controller
    {

        private HttpContext httpContext;
        private readonly CollectionService _service;
        private readonly AccountingService _accountingService;
        private readonly ConfigurationService _dropservice;
        private readonly CompanyService _comService;
        private readonly IOrderApprovalService _orderApprovalService;
        //string password = "Sys@Root@007";
        //string admin = "Administrator";
        //string url = "http://192.168.2.2/ReportServer/?%2fErpReport/";

        //string password = "Gocorona!9";
        //string admin = "Administrator";
        //string url = "http://192.168.0.7/ReportServer_SQLEXPRESS/?%2fErpReport/";

        string password = "Sysroot@123";
        string admin = "Administrator";
        string url = "http://192.168.0.13/ReportServer/?%2fErpReport/";
        public CollectionController(CompanyService companyService,CollectionService collectionService, AccountingService accountingService, ConfigurationService dropservice, IOrderApprovalService orderApprovalService)
        {
            _comService = companyService;
            _service = collectionService;
            _accountingService = accountingService;
            _dropservice = dropservice;
            _orderApprovalService = orderApprovalService;

        }
        #region Common Supplier
        
        public async Task<ActionResult> CommonSupplierList(int companyId)
        {

            VMCommonSupplier vmCommonSupplier = new VMCommonSupplier();
            vmCommonSupplier = await Task.Run(() => _service.GetSupplier(companyId));


            return View(vmCommonSupplier);
        }
        
        public async Task<ActionResult> KFMALSupplierList(int companyId)
        {

            VMCommonSupplier vmCommonSupplier = new VMCommonSupplier();
            vmCommonSupplier = await Task.Run(() => _service.GetSupplier(companyId));


            return View(vmCommonSupplier);
        }
        
        public async Task<ActionResult> OrderMasterByCustomer(int companyId, int customerId)
        {
            VMSalesOrder vmOrderMaster = new VMSalesOrder();
            vmOrderMaster = await Task.Run(() => _service.ProcurementOrderMastersListGetByCustomer(companyId, customerId));
            return View(vmOrderMaster);
        }
        [HttpGet]
        
        public async Task<ActionResult> OrderMasterByID(int companyId, int paymentMasterId = 0)
        {
            VMPayment vmPayment = new VMPayment();
            vmPayment = await Task.Run(() => _service.ProcurementOrderMastersGetByID(companyId, paymentMasterId));
            //vmPayment.OrderMusterList = new SelectList(_service.OrderMastersDropDownList(companyId, customerId), "Value", "Text");
            vmPayment.SubZoneList = new SelectList(_service.SubZonesDropDownList(companyId), "Value", "Text");

            if (companyId == (int)CompanyName.GloriousCropCareLimited)
            {
                vmPayment.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");
                vmPayment.ExpensesHeadList = new SelectList(_accountingService.GCCLLCFactoryExpanceHeadGLList(companyId), "Value", "Text");
                vmPayment.IncomeHeadList = new SelectList(_accountingService.GCCLOtherIncomeHeadGLList(companyId), "Value", "Text");


            }
            if (companyId == (int)CompanyName.KrishibidSeedLimited)
            {
                vmPayment.BankOrCashParantList = new SelectList(_accountingService.SeedCashAndBankDropDownList(companyId), "Value", "Text");

            }

            return View(vmPayment);
        }

        
        
        [HttpPost]
        public async Task<ActionResult> OrderMasterByID(VMPayment vmPayment)
        {

            if (vmPayment.ActionEum == ActionEnum.Add)
            {
                if (vmPayment.PaymentMasterId == 0)
                {
                    vmPayment.PaymentMasterId = await _service.PaymentMasterAdd(vmPayment);

                }
                if (vmPayment.OrderMasterId != null)
                {
                    await _service.PaymentAdd(vmPayment);

                }
                if (vmPayment.ExpensesHeadGLId != null)
                {
                    await _service.ExpensesAdd(vmPayment);

                }
               
                if (vmPayment.OthersIncomeHeadGLId != null)
                {
                    await _service.IncomeAdd(vmPayment);

                }
               

            }
            else if (vmPayment.ActionEum == ActionEnum.Finalize)
            {
                
                await _service.SubmitCollectionMasters(vmPayment);

            }

            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(OrderMasterByID), new { companyId = vmPayment.CompanyFK, customerId = vmPayment.CustomerId, paymentMasterId = vmPayment.PaymentMasterId });
        }

        #endregion

        #region Feed collection
        [HttpGet]
        
        public async Task<ActionResult> FeedCollection(int companyId, int paymentMasterId = 0)
        {



            VMFeedPayment vmFeedPayment = new VMFeedPayment();
            int Uid = Convert.ToInt32(Session["Id"]);
           
            
            vmFeedPayment = await Task.Run(() => _service.FeedPaymentGet(companyId, paymentMasterId));
            vmFeedPayment.CustomerList = new SelectList(_service.CustomerLisByUidtGet(Uid), "Value", "Text");
            //vmFeedPayment.ZoneList = new SelectList(_service.ZonesDropDownList(companyId), "Value", "Text");
            if (companyId == (int)CompanyName.KrishibidFeedLimited)
            {
                vmFeedPayment.BankOrCashParantList = new SelectList(_accountingService.FEEDCashAndBankDropDownList(companyId), "Value", "Text");
            }
            vmFeedPayment.CompanyFK = companyId;


            return View(vmFeedPayment);
        }


        
        [HttpPost]
        public async Task<ActionResult> FeedCollection(FeedSalesOrderModel vmPayment)
            {

            if (vmPayment.ActionEum == ActionEnum.Add)
            {
                await _service.FeedPaymentAdd(vmPayment);

            }
            else if (vmPayment.ActionEum == ActionEnum.Finalize)
            {

                //var result = await _service.FinalizeCollectionFeed(vmPayment);
                //if (result > 0)
                //{


                //}

                var approvalStatus = await _orderApprovalService.AddAllMappingSignatoryApproval((int)vmPayment.OrderMasterId.Value);

            }
            else if (vmPayment.ActionEum == ActionEnum.Acknowledgement)
            {
                await _service.SubmitCollectionFeed(vmPayment);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("FeedProcurementSalesOrderSlave", "Procurement", new { companyId = vmPayment.CompanyId, productType = "F", orderMasterId = vmPayment.OrderMasterId });

        }











        
        [HttpPost]
        public async Task<ActionResult> SubmitFeedCollection(FeedSalesOrderModel vmPayment)
        {

           if (vmPayment.ActionEum == ActionEnum.Acknowledgement)
            {

                await _service.SubmitCollectionFeed(vmPayment);

            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(FeedCollectionIndevitualGet), new { companyId = vmPayment.CompanyFK, customerId = vmPayment.CustomerId, paymentMasterId = vmPayment.PaymentMasterId });
        }
        #endregion
        [HttpGet]
        
        public async Task<ActionResult> FeedCollectionList(int companyId)
        {
            VMFeedPayment vmFeedPayment = new VMFeedPayment();
            vmFeedPayment = await Task.Run(() => _service.FeedPaymentListGet(companyId));
            return View(vmFeedPayment);
        }

        [HttpGet]
        
        public async Task<ActionResult> FeedAccCollectionList(int companyId)
        {
            VMFeedPayment vmFeedPayment = new VMFeedPayment();
            vmFeedPayment = await Task.Run(() => _service.FeedPaymentListGet(companyId));
            return View(vmFeedPayment);
        }
        [HttpGet]
        
        public async Task<ActionResult> FeedCollectionIndevitualGet(int companyId,int paymentMasterId)
        {
            VMFeedPayment vmFeedPayment = new VMFeedPayment();
            vmFeedPayment = await Task.Run(() => _service.FeedPaymentGet(companyId,paymentMasterId));
            return View(vmFeedPayment);
        }









        #region Common Customer
        public JsonResult CommonCustomerByIDGet(int id)
        {
            var model = _service.GetCommonCustomerByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //public async Task<ActionResult> CommonPaymentMastersList(int companyId)
        //{
        //    VMPaymentMaster vmPaymentMaster = new VMPaymentMaster();
        //    vmPaymentMaster = await Task.Run(() => _service.GetPaymentMasters(companyId));
        //    return View(vmPaymentMaster);
        //}

        
        [HttpGet]
        public async Task<ActionResult> CommonPaymentMastersList(int companyId, DateTime? fromDate, DateTime? toDate)
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
            VMPaymentMaster vmPaymentMaster = new VMPaymentMaster();
            vmPaymentMaster = await Task.Run(() => _service.GetPaymentMasters(companyId, fromDate, toDate));
            vmPaymentMaster.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPaymentMaster.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(vmPaymentMaster);
        }
        [HttpPost]
        
        public async Task<ActionResult> CommonPaymentMastersList(VMPaymentMaster model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(CommonPaymentMastersList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }



        public async Task<ActionResult> PaymentMasterList(int companyId,int customerId)
        {

            VMPaymentMaster vmPaymentMaster = new VMPaymentMaster();
            vmPaymentMaster = await Task.Run(() => _service.GetPaymentMasters(companyId, customerId));


            return View(vmPaymentMaster);
        }


        public async Task<ActionResult> PurchaseOrderBySupplier(int companyId, int supplierId)
        {

            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = await Task.Run(() => _service.ProcurementPurchaseOrdersListGetBySupplier(companyId, supplierId));


            return View(vmPurchaseOrder);
        }
        //[HttpGet]
        //public async Task<ActionResult> PurchaseOrdersByID(int companyId, int supplierId, int paymentMasterId = 0)
        //{
        //    VMPayment VMPayment = new VMPayment();
        //    VMPayment = await Task.Run(() => _service.ProcurementPurchaseOrdersGetByID(companyId, supplierId, paymentMasterId));
        //    VMPayment.TransactionDate = DateTime.Today;
        //    return View(VMPayment);
        //}
        //[HttpPost]
        //public async Task<ActionResult> PurchaseOrdersByID(VMPurchaseOrder vmPurchaseOrder)
        //{

        //    if (vmPurchaseOrder.ActionEum == ActionEnum.Add)
        //    {

        //        await _service.SupplierPaymentAdd(vmPurchaseOrder);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }

        //    return RedirectToAction(nameof(PurchaseOrdersByID), new { companyId = vmPurchaseOrder.CompanyFK, supplierId = vmPurchaseOrder.Common_SupplierFK, purchaseOrderId = vmPurchaseOrder.PurchaseOrderId });
        //}
        
        [HttpGet]
        public async Task<ActionResult> PurchaseOrdersByID(int companyId, int supplierId, int paymentMasterId = 0)
        {
            VMPayment vmPayment = new VMPayment();
            vmPayment = await Task.Run(() => _service.ProcurementPurchaseOrdersGetByID(companyId, supplierId, paymentMasterId));

            vmPayment.OrderMusterList = new SelectList(_service.PurchaseOrdersDropDownList(companyId, supplierId), "Value", "Text");

            if (companyId == (int)CompanyName.GloriousCropCareLimited)
            {
                vmPayment.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");

            }
            if (companyId == (int)CompanyName.KrishibidSeedLimited)
            {
                vmPayment.BankOrCashParantList = new SelectList(_accountingService.SeedCashAndBankDropDownList(companyId), "Value", "Text");

            }

            return View(vmPayment);
        }
        
        [HttpPost]
        public async Task<ActionResult> PurchaseOrdersByID(VMPayment vmPayment)
        {

            if (vmPayment.ActionEum == ActionEnum.Add)
            {
                if (vmPayment.PaymentMasterId == 0)
                {
                    vmPayment.PaymentMasterId = await _service.PaymentMasterAdd(vmPayment);

                }
                await _service.PaymentAdd(vmPayment);

            }
            else if (vmPayment.ActionEum == ActionEnum.Finalize)
            {

                await _service.SubmitPaymentMasters(vmPayment);

            }

            else
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction(nameof(PurchaseOrdersByID), new { companyId = vmPayment.CompanyFK, supplierId = vmPayment.CustomerId, paymentMasterId = vmPayment.PaymentMasterId });
        }

        #endregion
        
        [HttpGet]
        public async Task<ActionResult> POWiseSupplierLedgerOpening(int companyId, int supplierId)
        {
            VmTransaction vmTransaction = new VmTransaction();
            vmTransaction.FromDate = DateTime.Now.AddDays(-30);
            vmTransaction.ToDate = DateTime.Now;
            vmTransaction.VendorFK = supplierId;
            vmTransaction.CompanyFK = companyId;
            vmTransaction.VMCommonSupplier = await Task.Run(() => _service.GetSupplierById(supplierId));


            return View(vmTransaction);
        }
        [HttpPost]
        
        public async Task<ActionResult> POWiseSupplierLedgerOpeningView(VmTransaction vmTransaction)
        {
            var vmCommonSupplierLedger = await Task.Run(() => _service.GetLedgerInfoBySupplier(vmTransaction));
            return View(vmCommonSupplierLedger);
        }

        
        [HttpGet]
        public async Task<ActionResult> InvoiceWiseCustomerLedgerOpening(int companyId, int customerId)
        {
            VmTransaction vmTransaction = new VmTransaction();
            vmTransaction.VMCommonSupplier = new VMCommonSupplier();
            vmTransaction.FromDate = DateTime.Now.AddDays(-30);
            vmTransaction.ToDate = DateTime.Now;
            vmTransaction.VendorFK = customerId;
            vmTransaction.CompanyFK = companyId;
            vmTransaction.VMCommonSupplier = await Task.Run(() => _service.GetSupplierById(customerId));

            return View(vmTransaction);
        }
        [HttpPost]
        
        public async Task<ActionResult> InvoiceWiseCustomerLedgerOpeningView(VmTransaction vmTransaction)
        {
            var vmCommonSupplierLedger = await Task.Run(() => _service.GetLedgerInfoByCustomer(vmTransaction));
            return View(vmCommonSupplierLedger);
        }


        [HttpGet]
      
        public async Task<ActionResult> CustomerAgeing(int companyId)
        {
            VmCustomerAgeing vmCustomerAgeing = new VmCustomerAgeing();           
            vmCustomerAgeing.CompanyFK = companyId;
            vmCustomerAgeing.ZoneListList = new SelectList(_dropservice.CommonZonesDropDownList(companyId), "Value", "Text");
            vmCustomerAgeing.TerritoryList = new SelectList(_dropservice.CommonSubZonesDropDownList(companyId), "Value", "Text");
            return View(vmCustomerAgeing);


        }
        [HttpPost]
        
        public async Task<ActionResult> CustomerAgeingView(VmCustomerAgeing vmCustomerAgeing)
        {
            var company = _comService.GetCompany((int)vmCustomerAgeing.CompanyFK);
            vmCustomerAgeing.CompanyName = company.Name;
            vmCustomerAgeing.DataList =  _service.CustomerAgeingGet(vmCustomerAgeing);

            return View(vmCustomerAgeing);
        }

        [HttpGet]
        
        public async Task<ActionResult> CustomerAgeingReport(int companyId)
        {
            VmCustomerAgeing vmCustomerAgeing = new VmCustomerAgeing();
            vmCustomerAgeing.CompanyFK = companyId;
            vmCustomerAgeing.ZoneListList = new SelectList(_dropservice.CommonZonesDropDownList(companyId), "Value", "Text");
            vmCustomerAgeing.TerritoryList = new SelectList(_dropservice.CommonSubZonesDropDownList(companyId), "Value", "Text");
            return View(vmCustomerAgeing);


        }
        [HttpPost]       
        public ActionResult CustomerAgeingReportView(VmCustomerAgeing model)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            model.ReportName = "GCCLCustomerAgeing";
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&AsOnDate={3}&ZoneId={4}&SubZoneId={5}", model.ReportName, model.ReportType, model.CompanyFK.Value, model.AsOnDate, model.ZoneId ?? 0,model.SubZoneId??0);

            if (model.ReportType.Equals(ReportType.EXCEL))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", model.ReportName + ".xls");
            }
            if (model.ReportType.Equals(ReportType.PDF))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (model.ReportType.Equals(ReportType.WORD))
            {
                return File(client.DownloadData(reportURL), "application/msword", model.ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]
        public ActionResult CustomerAgeingReportViewGet(string ReportType, int CompanyFK, string AsOnDate, int? ZoneId = 0, int? SubZoneId = 0)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
           string ReportName = "GCCLCustomerAgeing";
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&AsOnDate={3}&ZoneId={4}&SubZoneId={5}", ReportName, ReportType, CompanyFK, AsOnDate, ZoneId , SubZoneId);

            if (ReportType.Equals("EXCEL"))
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", ReportName + ".xls");
            }
            if (ReportType.Equals("PDF"))
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }
            if (ReportType.Equals("WORD"))
            {
                return File(client.DownloadData(reportURL), "application/msword", ReportName + ".doc");
            }
            return View();
        }

        [HttpGet]
        
        public async Task<ActionResult> CustomerAgeingSeed(int companyId)
        {
            VmCustomerAgeing vmCustomerAgeing = new VmCustomerAgeing();
            vmCustomerAgeing.CompanyFK = companyId;
            vmCustomerAgeing.ZoneListList = new SelectList(_dropservice.CommonZonesDropDownList(companyId=21), "Value", "Text");
            vmCustomerAgeing.TerritoryList = new SelectList(_dropservice.CommonSubZonesDropDownList(companyId=21), "Value", "Text");
            return View(vmCustomerAgeing);
        }
        [HttpPost]

        public async Task<ActionResult> CustomerAgeingSeedView(VmCustomerAgeing vmCustomerAgeing)
        {
            var company = _comService.GetCompany(21);
            vmCustomerAgeing.CompanyName = company.Name;
            vmCustomerAgeing.CompanyFK = 21;
            vmCustomerAgeing.DataList = _service.CustomerAgeingGet(vmCustomerAgeing);
            return View(vmCustomerAgeing);
        }




        [HttpGet]        
        public ActionResult GCCLCustomerAgeingDetails(int companyId, int CustomerId,string AsOnDate, string reportName,string reportFormat)
        {
            NetworkCredential nwc = new NetworkCredential(admin, password);
            WebClient client = new WebClient();
            client.Credentials = nwc;
            string reportURL = string.Format(url + "{0}&rs:Command=Render&rs:Format={1}&CompanyId={2}&CustomerId={3}&AsOnDate={4}", reportName, reportFormat, companyId, CustomerId, AsOnDate);
            if (reportFormat == "EXCEL")
            {
                return File(client.DownloadData(reportURL), "application/vnd.ms-excel", reportName +".xls");
            }
            if (reportFormat == "PDF")
            {
                return File(client.DownloadData(reportURL), "application/pdf");
            }

            return null;
        }

       

        public async Task<ActionResult> CommonCustomerList(int companyId)
        {

            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetCustomer(companyId));


            return View(vmCommonCustomer);
        }



        [HttpGet]
        public async Task<JsonResult> GetPaymentDataEditFeed(int PaymentId,int companyId)
        {
            FeedSalesOrderModel feedSalesOrderModel = new FeedSalesOrderModel();
            feedSalesOrderModel = await _service.GetPaymentEdit(PaymentId);
            if (feedSalesOrderModel.BankId > 0)
            {
                feedSalesOrderModel.head5 = await Task.Run(() => _accountingService.FeedHeadGLByHead5ParentIdGetForEdit(companyId, feedSalesOrderModel.BankId));
                feedSalesOrderModel.headGl = await Task.Run(() => _accountingService.FeedHeadGLByHeadGLParentIdGetForEdit(feedSalesOrderModel.BankId));
                return Json(feedSalesOrderModel, JsonRequestBehavior.AllowGet);
            }
            return Json(feedSalesOrderModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> DeletePayment(int PaymentId)
        {
          
            var obj=await _service.DeletePayment(PaymentId);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }









        //[HttpGet]
        //public async Task<ActionResult> GCCLCustomerPayment(int companyId = 0, int customerId = 0)
        //{
        //    VMPayment vmPayment = new VMPayment();

        //    if (companyId == 0)
        //    {
        //        vmPayment.CompanyFK = companyId;

        //    }
        //    else
        //    {
        //        vmPayment = await Task.Run(() => _service.GCCLOrderMastersGetByCustomer(companyId, customerId));

        //    }
        //    vmPayment.OrderMusterList = new SelectList(_service.OrderMastersDropDownList(companyId, customerId), "Value", "Text");

        //    if (companyId == (int)CompanyName.GloriousCropCareLimited)
        //    {
        //        vmPayment.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");

        //    }
        //    if (companyId == (int)CompanyName.KrishibidSeedLimited)
        //    {
        //        vmPayment.BankOrCashParantList = new SelectList(_accountingService.SeedCashAndBankDropDownList(companyId), "Value", "Text");

        //    }

        //    return View(vmPayment);
        //}
        //[HttpPost]
        //public async Task<ActionResult> GCCLCustomerPayment(VMPayment vmPayment)
        //{

        //    if (vmSalesOrderSlave.ActionEum == ActionEnum.Add)
        //    {
        //        if (vmSalesOrderSlave.OrderMasterId == 0)
        //        {
        //            vmSalesOrderSlave.OrderMasterId = await _service.OrderMasterAdd(vmSalesOrderSlave);

        //        }

        //        if (vmSalesOrderSlave.FProductId > 0)
        //        {
        //            await _service.OrderDetailAdd(vmSalesOrderSlave);

        //        }
        //    }
        //    else if (vmSalesOrderSlave.ActionEum == ActionEnum.Finalize)
        //    {
        //        //Delete
        //        await _service.OrderDetailEdit(vmSalesOrderSlave);
        //    }
        //    return RedirectToAction(nameof(GCCLProcurementSalesOrderSlave), new { companyId = vmSalesOrderSlave.CompanyFK, orderMasterId = vmSalesOrderSlave.OrderMasterId });
        //}

    }



}
