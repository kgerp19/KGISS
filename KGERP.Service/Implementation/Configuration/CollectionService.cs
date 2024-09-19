using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation

{
    public class CollectionService
    {
        private readonly ERPEntities _db;
        public List<VmTransaction> Transaction { get; set; }
        private readonly AccountingService _accountingService;
        public CollectionService(ERPEntities db)
        {
            _db = db;
            _accountingService = new AccountingService(db);
        }



        #region Supplier
        public async Task<VMCommonSupplier> GetSupplier(int companyId)
        {
            VMCommonSupplier vmCommonSupplier = new VMCommonSupplier();
            vmCommonSupplier.CompanyFK = companyId;
            vmCommonSupplier.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Supplier && x.CompanyId == companyId)
                                                              join t2 in _db.Countries on t1.CountryId equals t2.CountryId
                                                              where t1.IsActive == true
                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  Phone = t1.Phone,
                                                                  Country = t2.CountryName,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Common_CountriesFk = t1.CountryId.Value,

                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  IsForeign = t1.IsForeign
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());


            return vmCommonSupplier;
        }
        public VMCommonSupplier GetSupplierById(int supplierId)
        {
            VMCommonSupplier vmCommonSupplier = new VMCommonSupplier();
            var vandor = _db.Vendors.Find(supplierId);
            vmCommonSupplier.Name = vandor.Name;
            //vmCommonSupplier = await Task.Run(() => (from t1 in vandor
            //                                         select new VMCommonSupplier
            //                                         {
            //                                             ID = t1.VendorId,
            //                                             Name = t1.Name,
            //                                             Email = t1.Email,
            //                                             Phone = t1.Phone,
            //                                             CompanyFK = t1.CompanyId,
            //                                             Common_CountriesFk = t1.CountryId.Value,

            //                                             ContactPerson = t1.ContactName,
            //                                             Address = t1.Address,
            //                                             Code = t1.Code,
            //                                             CreatedBy = t1.CreatedBy,
            //                                             Remarks = t1.Remarks,
            //                                             IsForeign = t1.IsForeign
            //                                         }).FirstOrDefault());


            return vmCommonSupplier;
        }
        #endregion


        public class EnumModel
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }


        #region Common Customer

        public VMCommonSupplier GetCommonCustomerByID(int id)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Customer && x.VendorId == id)
                     join t2 in _db.SubZones on t1.SubZoneId equals t2.SubZoneId
                     join t3 in _db.Upazilas on t1.UpazilaId equals t3.UpazilaId
                     join t4 in _db.Districts on t3.DistrictId equals t4.DistrictId

                     select new VMCommonSupplier
                     {
                         ID = t1.VendorId,
                         Name = t1.Name,
                         Email = t1.Email,
                         Phone = t1.Phone,
                         CompanyFK = t1.CompanyId,
                         SubZoneId = t1.SubZoneId.Value,
                         ZoneId = t2.ZoneId,
                         Common_DivisionFk = t4.DivisionId,
                         Common_DistrictsFk = t3.DistrictId,
                         Common_UpazilasFk = t3.UpazilaId,
                         ContactPerson = t1.ContactName,
                         Address = t1.Address,
                         Code = t1.Code,
                         CreatedBy = t1.CreatedBy,
                         Remarks = t1.Remarks,
                         IsForeign = t1.IsForeign
                     }).FirstOrDefault();
            return v;
        }
        public async Task<VMPaymentMaster> GetPaymentMasters(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMPaymentMaster vmPaymentMaster = new VMPaymentMaster();
            vmPaymentMaster.CompanyFK = companyId;
            vmPaymentMaster.DataList = await Task.Run(() => (from t1 in _db.PaymentMasters
                                                             .Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                             join t2 in _db.VendorTypes on t1.VendorTypeId equals t2.VendorTypeId
                                                             join t3 in _db.HeadGLs on t1.BankChargeHeadGLId equals t3.Id
                                                             join t4 in _db.HeadGLs on t1.PaymentToHeadGLId equals t4.Id into t4_JOIN
                                                             from t4 in t4_JOIN.DefaultIfEmpty()
                                                             where t1.TransactionDate >= fromDate
                                                             && t1.TransactionDate <= toDate
                                                             select new VMPaymentMaster
                                                             {
                                                                 CompanyFK = t1.CompanyId,
                                                                 PaymentMasterId = t1.PaymentMasterId,
                                                                 VendorId = t1.VendorId,
                                                                 PaymentNo = t1.PaymentNo,
                                                                 BankCharge = t1.BankCharge,
                                                                 BankChargeHeadGLId = t1.BankChargeHeadGLId,
                                                                 TransactionDate = t1.TransactionDate,
                                                                 MoneyReceiptNo = t1.MoneyReceiptNo,
                                                                 MoneyReceiptDate = t1.MoneyReceiptDate,
                                                                 PaymentToHeadGLId = t1.PaymentToHeadGLId,
                                                                 ReferenceNo = t3.Remarks,
                                                                 BankChargeHeadGLName = t3.AccName,
                                                                 PaymentToHeadGLName = t4.AccName
                                                             }).OrderByDescending(x => x.PaymentMasterId).AsEnumerable());
            return vmPaymentMaster;
        }


        public async Task<VMPaymentMaster> GetPaymentMasters(int companyId, int customerId)
        {
            VMPaymentMaster vmPaymentMaster = new VMPaymentMaster();
            vmPaymentMaster.CompanyFK = companyId;
            vmPaymentMaster.DataList = await Task.Run(() => (from t1 in _db.PaymentMasters.Where(x => x.IsActive == true && x.VendorId == customerId && x.CompanyId == companyId)
                                                                 //join t3 in _db.HeadGLs on t1.PaymentToHeadGLId equals t3.Id
                                                             select new VMPaymentMaster
                                                             {
                                                                 CompanyFK = t1.CompanyId,
                                                                 PaymentMasterId = t1.PaymentMasterId,
                                                                 VendorId = t1.VendorId,
                                                                 BankCharge = t1.BankCharge,
                                                                 MoneyReceiptNo = t1.MoneyReceiptNo,
                                                                 MoneyReceiptDate = t1.MoneyReceiptDate,
                                                                 PaymentNo = t1.PaymentNo,
                                                                 ReferenceNo = t1.ReferenceNo,
                                                                 TransactionDate = t1.TransactionDate,
                                                                 TotalAmount = _db.Payments.Where(x => x.PaymentMasterId == t1.PaymentMasterId && x.IsActive).Select(x => x.InAmount).DefaultIfEmpty(0).Sum()
                                                             }).OrderByDescending(x => x.PaymentMasterId).AsEnumerable());
            return vmPaymentMaster;
        }

        #endregion
        public async Task<VMSalesOrder> ProcurementOrderMastersListGetByCustomer(int companyId, int customerId)
        {

            VMSalesOrder vmOrderMaster = new VMSalesOrder();
            vmOrderMaster = (from t1 in _db.Vendors.Where(x => x.VendorId == customerId)
                             select new VMSalesOrder
                             {
                                 CommonCustomerName = t1.Name,
                                 InAmount = _db.Payments.Where(x => x.IsActive && x.VendorId == t1.VendorId).Select(x => x.InAmount).DefaultIfEmpty(0).Sum()

                             }).FirstOrDefault();
            vmOrderMaster.CompanyFK = companyId;
            vmOrderMaster.DataList = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.CompanyId == companyId && x.Status < (int)EnumPOStatus.Closed && x.CustomerId == customerId)

                                                           select new VMSalesOrder
                                                           {
                                                               OrderMasterId = t1.OrderMasterId,
                                                               CustomerId = t1.CustomerId.Value,
                                                               CreatedBy = t1.CreatedBy,
                                                               CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                               OrderNo = t1.OrderNo,
                                                               OrderDate = t1.OrderDate,
                                                               ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                               Status = t1.Status,
                                                               CompanyFK = t1.CompanyId,
                                                               CourierCharge = t1.CourierCharge,
                                                               TotalAmount = (_db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == t1.OrderMasterId).Select(x => x.Qty * x.UnitPrice).DefaultIfEmpty(0).Sum()),

                                                           }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());



            return vmOrderMaster;
        }

        public async Task<VMPurchaseOrder> ProcurementPurchaseOrdersListGetBySupplier(int companyId, int supplierId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder = (from t1 in _db.Vendors.Where(x => x.VendorId == supplierId)
                               select new VMPurchaseOrder
                               {
                                   SupplierName = t1.Name

                               }).FirstOrDefault();
            vmPurchaseOrder.CompanyFK = companyId;
            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && x.CompanyId == companyId && x.Status < (int)EnumPOStatus.Closed && x.SupplierId == supplierId)

                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 Common_SupplierFK = t1.SupplierId.Value,
                                                                 CreatedBy = t1.CreatedBy,
                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 DeliveryDate = t1.DeliveryDate,
                                                                 DeliveryAddress = t1.DeliveryAddress,

                                                                 CompanyFK = t1.CompanyId,
                                                                 TotalPOValue = _db.PurchaseOrderDetails.Where(x => x.IsActive && x.PurchaseOrderId == t1.PurchaseOrderId).Select(x => x.PurchaseQty * x.PurchaseRate).DefaultIfEmpty(0).Sum(),

                                                                 PayableAmount = (from ts1 in _db.MaterialReceiveDetails
                                                                                  join ts2 in _db.MaterialReceives on ts1.MaterialReceiveId equals ts2.MaterialReceiveId
                                                                                  where ts2.PurchaseOrderId == t1.PurchaseOrderId && ts2.IsActive && ts1.IsActive && !ts1.IsReturn
                                                                                  select ts1.ReceiveQty * ts1.UnitPrice).DefaultIfEmpty(0).Sum(),
                                                                 InAmount = _db.Payments.Where(x => x.VendorId == supplierId)
                                                                         .Select(x => x.InAmount).DefaultIfEmpty(0).Sum()
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());



            return vmPurchaseOrder;
        }
        public List<object> SubZonesDropDownList(int companyId = 0)
        {
            var List = new List<object>();
            _db.SubZones
        .Where(x => x.IsActive).Where(x => x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.SubZoneId,
            Text = x.SalesOfficerName + " -" + x.Name
        }));
            return List;

        }

        public async Task<VMPayment> ProcurementOrderMastersGetByID(int companyId, int paymentMasterId)
        {
            VMPayment VMPayment = new VMPayment();
            if (paymentMasterId > 0)
            {
                VMPayment = await Task.Run(() => (from t2 in _db.PaymentMasters.Where(x => x.IsActive && x.PaymentMasterId == paymentMasterId)
                                                  join t3 in _db.HeadGLs on t2.PaymentToHeadGLId equals t3.Id into t3_join
                                                  from t3 in t3_join.DefaultIfEmpty()
                                                  join t4 in _db.HeadGLs on t2.BankChargeHeadGLId equals t4.Id into t4_join
                                                  from t4 in t4_join.DefaultIfEmpty()
                                                  join t5 in _db.VendorTypes on t2.VendorTypeId equals t5.VendorTypeId



                                                  select new VMPayment
                                                  {
                                                      PaymentToHeadGLId = t2.PaymentToHeadGLId,

                                                      PaymentToHeadGLName = t3 != null ? t3.AccCode + " - " + t3.AccName : "",
                                                      BankChargeHeadGLName = t3 != null ? t4.AccCode + " - " + t4.AccName : "",
                                                      VendorTypeName = t5.Name,

                                                      BankCharge = t2.BankCharge,
                                                      BankChargeHeadGLId = t2.BankChargeHeadGLId,
                                                      IsFinalized = t2.IsFinalized,
                                                      PaymentMasterId = t2.PaymentMasterId,
                                                      PaymentNo = t2.PaymentNo,
                                                      CompanyFK = t2.CompanyId,
                                                      CompanyId = t2.CompanyId,
                                                      TransactionDate = t2.TransactionDate,
                                                      ReferenceNo = t2.ReferenceNo,



                                                      //TotalAmount =(from ts1 in _db.OrderDetails.Where(x => x.IsActive)
                                                      //              join ts2 in _db.OrderMasters.Where(x => x.CustomerId == customerId) on ts1.OrderMasterId equals ts2.OrderMasterId
                                                      //              select ts1.Qty * ts1.UnitPrice).DefaultIfEmpty(0).Sum(),

                                                      //PayableAmount = (from ts1 in _db.OrderDeliverDetails
                                                      //                 join ts2 in _db.OrderDetails on ts1.OrderDetailId equals ts2.OrderDetailId
                                                      //                 join ts3 in _db.OrderMasters.Where(x => x.CustomerId == t1.VendorId) on ts2.OrderMasterId equals ts3.OrderMasterId
                                                      //                 where ts2.IsActive && ts1.IsActive
                                                      //                 group new { ts1, ts2, ts3 } by new { ts3.OrderMasterId } into Group
                                                      //                 select Group.Sum(x => x.ts1.DeliveredQty * x.ts2.UnitPrice) + Group.FirstOrDefault().ts3.CourierCharge).DefaultIfEmpty(0).Sum(),

                                                      //ReturnAmount = (from ts1 in _db.SaleReturnDetails
                                                      //                join ts2 in _db.SaleReturns.Where(x => x.CustomerId == t1.VendorId && x.CompanyId == t1.CompanyId) on ts1.SaleReturnId equals ts2.SaleReturnId
                                                      //                where ts1.IsActive && ts2.IsActive
                                                      //                select ts1.Qty.Value * ts1.Rate.Value).DefaultIfEmpty(0).Sum(),

                                                      //InAmount = _db.Payments.Where(x => x.VendorId == t1.VendorId)
                                                      //                 .Select(x => x.InAmount).DefaultIfEmpty(0).Sum()
                                                  }).FirstOrDefault());
            }
            else
            {
                VMPayment.CompanyFK = companyId;
                //VMPayment = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.VendorId == t1.VendorId && x.IsActive && x.CompanyId == companyId)
                //                                  select new VMPayment
                //                                  {
                //                                      CustomerId = t1.VendorId,
                //                                      CompanyId = companyId,

                //                                      CommonCustomerName = t1.Name,
                //                                      CommonCustomerCode = t1.Code,
                //                                      CompanyFK = t1.CompanyId,

                //                                      //TotalAmount =(from ts1 in _db.OrderDetails.Where(x => x.IsActive)
                //                                      //              join ts2 in _db.OrderMasters.Where(x => x.CustomerId == customerId) on ts1.OrderMasterId equals ts2.OrderMasterId
                //                                      //              select ts1.Qty * ts1.UnitPrice).DefaultIfEmpty(0).Sum(),

                //                                      PayableAmount = (from ts1 in _db.OrderDeliverDetails
                //                                                       join ts2 in _db.OrderDetails on ts1.OrderDetailId equals ts2.OrderDetailId
                //                                                       join ts3 in _db.OrderMasters.Where(x => x.CustomerId == t1.VendorId) on ts2.OrderMasterId equals ts3.OrderMasterId
                //                                                       where ts2.IsActive && ts1.IsActive
                //                                                       group new { ts1, ts2, ts3 } by new { ts3.OrderMasterId } into Group
                //                                                       select Group.Sum(x => x.ts1.DeliveredQty * x.ts2.UnitPrice) + Group.FirstOrDefault().ts3.CourierCharge).DefaultIfEmpty(0).Sum(),

                //                                      ReturnAmount = (from ts1 in _db.SaleReturnDetails
                //                                                      join ts2 in _db.SaleReturns.Where(x => x.CustomerId == t1.VendorId && x.CompanyId == t1.CompanyId) on ts1.SaleReturnId equals ts2.SaleReturnId
                //                                                      where ts1.IsActive && ts2.IsActive
                //                                                      select ts1.Qty.Value * ts1.Rate.Value).DefaultIfEmpty(0).Sum(),

                //                                      InAmount = _db.Payments.Where(x => x.VendorId == t1.VendorId)
                //                                                       .Select(x => x.InAmount).DefaultIfEmpty(0).Sum()
                //                                  }).FirstOrDefault());
            }


            VMPayment.DataList = await Task.Run(() => (from t1 in _db.Payments.Where(x => x.IsActive && x.CompanyId == companyId && x.PaymentMasterId == paymentMasterId)
                                                       join t2 in _db.PaymentMasters on t1.PaymentMasterId equals t2.PaymentMasterId
                                                       join t3 in _db.OrderMasters on t1.OrderMasterId equals t3.OrderMasterId
                                                       join t0 in _db.Vendors on t1.VendorId equals t0.VendorId
                                                       join t5 in _db.HeadGLs on t1.PaymentFromHeadGLId equals t5.Id

                                                       select new VMPayment
                                                       {
                                                           TransactionDate = t1.TransactionDate,
                                                           MoneyReceiptNo = t1.MoneyReceiptNo,

                                                           InAmount = t1.InAmount,
                                                           CustomerId = t1.VendorId,
                                                           CommonCustomerName = t0.Name,
                                                           CommonCustomerCode = t0.Code,
                                                           OrderNo = t3.OrderNo,
                                                           OrderDate = t3.OrderDate,
                                                           PaymentFromHeadGLName = t5.AccCode + " - " + t5.AccName,
                                                           PaymentFromHeadGLId = t1.PaymentFromHeadGLId,
                                                           CompanyFK = t1.CompanyId,
                                                           CreatedBy = t1.CreatedBy,
                                                           PaymentId = t1.PaymentId,
                                                           ReferenceNo = t1.ReferenceNo
                                                       }).OrderByDescending(x => x.PaymentId).AsEnumerable());


            VMPayment.DataListExpenses = await Task.Run(() => (from t1 in _db.Expenses.Where(x => x.IsActive && x.CompanyId == companyId && x.PaymentMasterId == paymentMasterId)
                                                               join t2 in _db.HeadGLs on t1.ExpensesHeadGLId equals t2.Id
                                                               select new VMPayment
                                                               {
                                                                   ID = t1.ExpensesId,
                                                                   ExpensesAmount = t1.Amount,
                                                                   ExpensesHeadGLId = t1.ExpensesHeadGLId,
                                                                   ExpensesHeadGLName = t2.AccName,
                                                                   ExpensessReference = t1.ReferenceNo
                                                               }).OrderByDescending(x => x.ID).AsEnumerable());

            VMPayment.DataListIncome = await Task.Run(() => (from t1 in _db.Incomes.Where(x => x.IsActive && x.CompanyId == companyId && x.PaymentMasterId == paymentMasterId)
                                                             join t2 in _db.HeadGLs on t1.IncomeHeadGLId equals t2.Id
                                                             select new VMPayment
                                                             {
                                                                 ID = t1.IncomeId,
                                                                 OthersIncomeAmount = t1.Amount,
                                                                 OthersIncomeHeadGLId = t1.IncomeHeadGLId,
                                                                 OthersIncomeHeadGLName = t2.AccName,
                                                                 IncomeReference = t1.ReferenceNo
                                                             }).OrderByDescending(x => x.ID).AsEnumerable());



            return VMPayment;
        }


        public async Task<FeedSalesOrderModel> FeedPaymentGet(int companyId, int paymentMasterId)
        {
            FeedSalesOrderModel VMPayment = new FeedSalesOrderModel();
            //if (paymentMasterId > 0)
            //{
            //    VMPayment = await Task.Run(() => (from t2 in _db.PaymentMasters.Where(x => x.IsActive && x.PaymentMasterId == paymentMasterId)
            //                                      join t3 in _db.HeadGLs on t2.PaymentToHeadGLId equals t3.Id into t3_join
            //                                      from t3 in t3_join.DefaultIfEmpty()
                                                 
            //                                      select new FeedSalesOrderModel
            //                                      {
            //                                          PaymentNo = t2.PaymentNo,
            //                                          TransactionDate = t2.TransactionDate,
            //                                          PaymentToHeadGLId = t2.PaymentToHeadGLId,
            //                                          PaymentToHeadGLName = t3 != null ? t3.AccCode + " - " + t3.AccName : "",
            //                                          IsFinalized = t2.IsFinalized,
            //                                          PaymentMasterId = t2.PaymentMasterId,   
            //                                          CompanyId = t2.CompanyId,
                                                    
                                                     
            //                                          IsFeedVoucherCreated = t2.IsFeedVoucherCreated
            //                                      }).FirstOrDefault());
            //}
            //else
            //{
            //    VMPayment.CompanyId = companyId;

            //}


            VMPayment.DataListPayment = await Task.Run(() => (from t1 in _db.Payments.Where(x => x.IsActive && x.CompanyId == companyId && x.PaymentMasterId == paymentMasterId)
                                                     
                                                       select new FeedSalesOrderModel
                                                       {
                                                          
                                                           InAmount = t1.InAmount,
                                                           CustomerId = t1.VendorId,
                                                         
                                                           CompanyId = t1.CompanyId,
                                                           CreatedBy = t1.CreatedBy,
                                                           PaymentId = t1.PaymentId,
                                                           ReferenceNo = t1.ReferenceNo
                                                       }).OrderByDescending(x => x.PaymentId).AsEnumerable());

            return VMPayment;
        }

        public List<object> CustomerLisByUidtGet(int Uid)
        {
            var List = new List<object>();
            _db.Vendors
        .Where(x => x.IsActive).Where(x => x.SalesOfficerEmpId == Uid).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.VendorId,
            Text = x.Name
        }));
            return List;

        }

        public async Task<VMFeedPayment> FeedPaymentListGet(int companyId)
        {
            VMFeedPayment VMPayment = new VMFeedPayment();
            VMPayment.DataList = await Task.Run(() => (from t1 in _db.PaymentMasters.Where(x => x.IsActive && x.CompanyId == companyId)
                                                       join t2 in _db.HeadGLs on t1.PaymentToHeadGLId equals t2.Id
                                                       select new VMFeedPayment
                                                       {
                                                           PaymentNo = t1.PaymentNo,
                                                           TransactionDate = t1.TransactionDate,

                                                           CustomerId = t1.VendorId,
                                                           BankCharge = t1.BankCharge,
                                                           //CommonCustomerName = t0.Name,
                                                           //CommonCustomerCode = t0.Code,
                                                           PaymentToHeadGLName = t2.AccCode + " - " + t2.AccName,
                                                           CompanyFK = t1.CompanyId,
                                                           CreatedBy = t1.CreatedBy,
                                                           PaymentMasterId = t1.PaymentMasterId,
                                                           ReferenceNo = t1.ReferenceNo,
                                                           IsFinalized = t1.IsFinalized,
                                                           IsFeedVoucherCreated = t1.IsFeedVoucherCreated,
                                                       }).OrderByDescending(x => x.PaymentMasterId).AsEnumerable());





            return VMPayment;
        }
        //public async Task<VMFeedPayment> FeedPaymentGetById(int companyId,int paymentMasterId)
        //{
        //    VMFeedPayment VMPayment = new VMFeedPayment();

        //    VMPayment = (from t1 in _db.PaymentMasters
        //                 join t2 in _db.HeadGLs on t1.PaymentToHeadGLId equals t2.Id
        //                 where t1.CompanyId == companyId && t1.IsActive
        //                 select new VMFeedPayment
        //                 {
        //                     PaymentMasterId = t1.PaymentMasterId,
        //                     TransactionDate = t1.TransactionDate,
        //                     PaymentNo = t1.PaymentNo,
        //                     PaymentToHeadGLName = t2.AccName,
        //                     BankCharge = t1.BankCharge,
        //                     BankChargeHeadGLId = t1.BankChargeHeadGLId,
        //                     PaymentToHeadGLId  = t1.PaymentToHeadGLId,


        //                 }).FirstOrDefault();






        //    VMPayment.DataList = await Task.Run(() => (from t2 in _db.Payments.Where(x => x.IsActive && x.PaymentMasterId == paymentMasterId)
        //                                      join t3 in _db.HeadGLs on t2.PaymentFromHeadGLId equals t3.Id into t3_join
        //                                      from t3 in t3_join.DefaultIfEmpty()
        //                                      join t5 in _db.Vendors on t2.VendorId equals t5.VendorId



        //                                      select new VMFeedPayment
        //                                      {
        //                                          PaymentToHeadGLId = t2.PaymentFromHeadGLId,

        //                                          PaymentFromHeadGLName = t3 != null ? t3.AccCode + " - " + t3.AccName : "",

        //                                          VendorTypeName = t5.Name,
        //                                          PaymentId = t2.PaymentId,
        //                                           InAmount=t2.InAmount,
        //                                           MoneyReceiptNo=t2.MoneyReceiptNo,
        //                                          CompanyFK = t2.CompanyId,
        //                                          CompanyId = t2.CompanyId,
        //                                          TransactionDate = t2.TransactionDate,
        //                                          ReferenceNo = t2.ReferenceNo
        //                                      }).AsEnumerable());





        //    return VMPayment;
        //}

        public List<object> OrderMastersDropDownList(int companyId, int customerId)
        {
            var orderMastersList = new List<object>();
            var orderMasters = _db.OrderMasters.Where(a => a.IsActive && a.CompanyId == companyId && a.CustomerId == customerId).OrderByDescending(x => x.OrderMasterId).ToList();
            foreach (var x in orderMasters)
            {
                orderMastersList.Add(new { Text = x.OrderNo + " Date: " + x.OrderDate.ToLongDateString(), Value = x.OrderMasterId });
            }
            return orderMastersList;
        }
        public List<object> PurchaseOrdersDropDownList(int companyId, int customerId)
        {
            var PurchaseOrdersList = new List<object>();
            var orderMasters = _db.PurchaseOrders.Where(a => a.IsActive && a.CompanyId == companyId && a.SupplierId == customerId).OrderByDescending(x => x.PurchaseOrderId).ToList();
            foreach (var x in orderMasters)
            {
                PurchaseOrdersList.Add(new { Text = x.PurchaseOrderNo + " Date: " + x.PurchaseDate.Value.ToLongDateString(), Value = x.PurchaseOrderId });
            }
            return PurchaseOrdersList;
        }

        //public async Task<VMPurchaseOrder> ProcurementPurchaseOrdersGetByID(int companyId, int supplierId)
        //{
        //    VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
        //    vmPurchaseOrder = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive && x.CompanyId == companyId && x.VendorId == supplierId)
        //                                            select new VMPurchaseOrder
        //                                            {
        //                                                Common_SupplierFK = t1.VendorId,
        //                                                CompanyFK = t1.CompanyId,
        //                                                PayableAmount = (from ts1 in _db.MaterialReceiveDetails
        //                                                                 join ts2 in _db.MaterialReceives on ts1.MaterialReceiveId equals ts2.MaterialReceiveId
        //                                                                 join ts3 in _db.PurchaseOrders on ts2.PurchaseOrderId equals ts3.PurchaseOrderId

        //                                                                 where ts3.SupplierId == supplierId && ts2.IsActive && ts1.IsActive && !ts1.IsReturn
        //                                                                 select ts1.ReceiveQty * ts1.UnitPrice).DefaultIfEmpty(0).Sum(),


        //                                                InAmount = _db.Payments.Where(x => x.VendorId == supplierId)
        //                                                                 .Select(x => x.InAmount).DefaultIfEmpty(0).Sum()
        //                                            }).FirstOrDefault());

        //    vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.Payments.Where(x => x.IsActive && x.CompanyId == companyId && x.VendorId == supplierId)
        //                                                     select new VMPurchaseOrder
        //                                                     {
        //                                                         Common_SupplierFK = t1.VendorId,
        //                                                         CreatedBy = t1.CreatedBy,
        //                                                         PaymentDate = t1.TransactionDate,
        //                                                         CompanyFK = t1.CompanyId,
        //                                                         InAmount = t1.InAmount,
        //                                                         PaymentId = t1.PaymentId,
        //                                                         Remarks = t1.ReferenceNo
        //                                                     }).OrderByDescending(x => x.PaymentId).AsEnumerable());



        //    return vmPurchaseOrder;
        //}
        public async Task<VMPayment> ProcurementPurchaseOrdersGetByID(int companyId, int supplierId, int paymentMasterId)
        {
            VMPayment VMPayment = new VMPayment();
            if (paymentMasterId > 0)
            {
                VMPayment = await Task.Run(() => (from t2 in _db.PaymentMasters.Where(x => x.IsActive && x.PaymentMasterId == paymentMasterId)
                                                      //join t1 in _db.Vendors.Where(x => x.VendorId == supplierId && x.IsActive && x.CompanyId == companyId)
                                                      //on t2.VendorId equals t1.VendorId
                                                      //join t3 in _db.HeadGLs on t2.PaymentFromHeadGLId equals t3.Id
                                                      //join t4 in _db.HeadGLs on t2.PaymentToHeadGLId equals t4.Id


                                                  select new VMPayment
                                                  {
                                                      //ACName = t1.ACName,
                                                      //ACNo = t1.ACNo,
                                                      //BankName = t1.BankName,
                                                      //BranchName = t1.BankName,

                                                      PaymentFromHeadGLId = t2.PaymentToHeadGLId,
                                                      BankCharge = t2.BankCharge,
                                                      BankChargeHeadGLId = t2.BankChargeHeadGLId,
                                                      IsFinalized = t2.IsFinalized,
                                                      PaymentMasterId = t2.PaymentMasterId,
                                                      PaymentNo = t2.PaymentNo,
                                                      // CustomerId = t1.VendorId,
                                                      CompanyId = companyId,
                                                      CompanyFK = t2.CompanyId,
                                                      MoneyReceiptNo = t2.MoneyReceiptNo,
                                                      TransactionDate = t2.TransactionDate,
                                                      ReferenceNo = t2.ReferenceNo,
                                                      //CommonCustomerName = t1.Name,
                                                      //CommonCustomerCode = t1.Code,
                                                      //CompanyFK = t1.CompanyId,
                                                      //PaymentFromHeadGLId = t2.PaymentFromHeadGLId,
                                                      //PaymentToHeadGLId = t2.PaymentToHeadGLId,
                                                      //PaymentToHeadGLName = t4.AccCode + " - " + t4.AccName,
                                                      //PaymentFromHeadGLName = t3.AccCode + " - " + t3.AccName,


                                                      //PayableAmountDecimal = (from ts1 in _db.MaterialReceiveDetails
                                                      //                        join ts2 in _db.MaterialReceives on ts1.MaterialReceiveId equals ts2.MaterialReceiveId
                                                      //                        join ts3 in _db.PurchaseOrders on ts2.PurchaseOrderId equals ts3.PurchaseOrderId

                                                      //                        where ts3.SupplierId == supplierId && ts2.IsActive && ts1.IsActive && !ts1.IsReturn
                                                      //                        select ts1.ReceiveQty * ts1.UnitPrice).DefaultIfEmpty(0).Sum(),
                                                      //ReturnAmount = (from ts1 in _db.PurchaseReturnDetails
                                                      //                join ts2 in _db.PurchaseReturns.Where(x => x.SupplierId == t1.VendorId && x.CompanyId == t1.CompanyId) on ts1.PurchaseReturnId equals ts2.PurchaseReturnId
                                                      //                //where ts1.IsActive && ts2.IsActive
                                                      //                select ts1.Qty.Value * ts1.Rate.Value).DefaultIfEmpty(0).Sum(),

                                                      //InAmount = _db.Payments.Where(x => x.VendorId == supplierId)
                                                      //                 .Select(x => x.InAmount).DefaultIfEmpty(0).Sum()
                                                  }).FirstOrDefault());
            }
            else
            {
                VMPayment = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.VendorId == supplierId && x.IsActive && x.CompanyId == companyId)
                                                  select new VMPayment
                                                  {
                                                      ACName = t1.ACName,
                                                      ACNo = t1.ACNo,
                                                      BankName = t1.BankName,
                                                      BranchName = t1.BankName,
                                                      CustomerId = t1.VendorId,
                                                      CompanyId = companyId,

                                                      CommonCustomerName = t1.Name,
                                                      CommonCustomerCode = t1.Code,
                                                      CompanyFK = t1.CompanyId,

                                                      PayableAmountDecimal = (from ts1 in _db.MaterialReceiveDetails
                                                                              join ts2 in _db.MaterialReceives on ts1.MaterialReceiveId equals ts2.MaterialReceiveId
                                                                              join ts3 in _db.PurchaseOrders on ts2.PurchaseOrderId equals ts3.PurchaseOrderId

                                                                              where ts3.SupplierId == supplierId && ts2.IsActive && ts1.IsActive && !ts1.IsReturn
                                                                              select ts1.ReceiveQty * ts1.UnitPrice).DefaultIfEmpty(0).Sum(),
                                                      ReturnAmount = (from ts1 in _db.PurchaseReturnDetails
                                                                      join ts2 in _db.PurchaseReturns.Where(x => x.SupplierId == t1.VendorId && x.CompanyId == t1.CompanyId) on ts1.PurchaseReturnId equals ts2.PurchaseReturnId
                                                                      //where ts1.IsActive && ts2.IsActive
                                                                      select ts1.Qty.Value * ts1.Rate.Value).DefaultIfEmpty(0).Sum(),

                                                      InAmount = _db.Payments.Where(x => x.VendorId == supplierId)
                                                                       .Select(x => x.InAmount).DefaultIfEmpty(0).Sum()

                                                  }).FirstOrDefault());
            }


            VMPayment.DataList = await Task.Run(() => (from t1 in _db.Payments.Where(x => x.VendorId == supplierId && x.IsActive && x.CompanyId == companyId && x.PaymentMasterId == paymentMasterId)
                                                       join t2 in _db.PaymentMasters on t1.PaymentMasterId equals t2.PaymentMasterId
                                                       join t3 in _db.PurchaseOrders on t1.PurchaseOrderId equals t3.PurchaseOrderId

                                                       select new VMPayment
                                                       {
                                                           CreatedBy = t1.CreatedBy,
                                                           TransactionDate = t1.TransactionDate,
                                                           OrderNo = t3.PurchaseOrderNo,
                                                           OrderDate = t3.PurchaseDate.Value,
                                                           CompanyFK = t1.CompanyId,
                                                           CompanyId = t1.CompanyId,
                                                           OutAmount = t1.OutAmount,
                                                           PaymentId = t1.PaymentId,
                                                           PaymentToHeadGLId = t1.PaymentFromHeadGLId
                                                       }).OrderByDescending(x => x.PaymentId).AsEnumerable());



            return VMPayment;
        }
        public async Task<int> PaymentMasterAdd(VMPayment vmPayment)
        {
            if (vmPayment.CompanyFK == (int)CompanyName.GloriousCropCareLimited)
            {
                vmPayment.BankChargeHeadGLId = 39432; // GCCL Bank Charge Head Id
            }
            int result = -1;
            var vendors = _db.Vendors.Find(vmPayment.CustomerId);

            #region Payment ID
            int paymentMastersCount = _db.PaymentMasters.Where(x => x.CompanyId == vmPayment.CompanyFK && x.VendorId == vmPayment.CustomerId).Count();

            if (paymentMastersCount == 0)
            {
                paymentMastersCount = 1;
            }
            else
            {
                paymentMastersCount++;
            }

            string PaymentNo = "C-" + vendors.Code + "-" + paymentMastersCount.ToString().PadLeft(4, '0');
            #endregion

            PaymentMaster paymentMaster = new PaymentMaster
            {

                PaymentNo = PaymentNo,
                TransactionDate = vmPayment.TransactionDate,
                VendorId = vmPayment.CustomerId.Value,
                ReferenceNo = vmPayment.ReferenceNo,

                VendorTypeId = vendors.VendorTypeId,

                BankChargeHeadGLId = vmPayment.BankChargeHeadGLId,

                BankCharge = vmPayment.BankCharge,
                CompanyId = vmPayment.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                PaymentToHeadGLId = vmPayment.Accounting_BankOrCashId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PaymentMasters.Add(paymentMaster);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = paymentMaster.PaymentMasterId;
            }


            return result;
        }


        public async Task<int> FeedPaymentAdd(FeedSalesOrderModel vmPayment)
        {
            int result = -1;
            if (vmPayment.PaymentId > 0)
            {

                Payment payment1 = new Payment();
                payment1=await _db.Payments.Where(x=>x.PaymentId == vmPayment.PaymentId).FirstOrDefaultAsync();

                payment1.TransactionDate = vmPayment.TransactionDate;
                if (vmPayment.Accounting_BankOrCashId != null)
                {
                    payment1.BankId = vmPayment.Accounting_BankOrCashId;
                    if (vmPayment.BankName != "Advance")
                    {
                        payment1.BranchName = vmPayment.BankName;
                        payment1.ReceiveLocation = null;
                    }
                    else
                    {
                        payment1.BranchName = vmPayment.BankName;
                        payment1.ReceiveLocation = vmPayment.ReceiveLocation;
                    }
                    
                }
                else
                {
                    payment1.BranchName = vmPayment.BankName;
                    payment1.BankId = null;
                }
                payment1.InAmount= vmPayment.InAmount;
                payment1.ReferenceNo = vmPayment.ReferenceNo;

                await   _db.SaveChangesAsync();
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = payment1.PaymentId;
                }

                return result;

            }



           
            var vendors = _db.Vendors.Find(vmPayment.CustomerId);

            #region Payment ID
            int paymentMastersCount = _db.Payments.Where(x => x.CompanyId == vmPayment.CompanyId && x.VendorId == vmPayment.CustomerId).Count();

            if (paymentMastersCount == 0)
            {
                paymentMastersCount = 1;
            }
            else
            {
                paymentMastersCount++;
            }

            string PaymentNo = "C-" + vendors.Code + "-" + paymentMastersCount.ToString().PadLeft(4, '0');
            #endregion

            
            Payment payment = new Payment
            {
                MoneyReceiptNo = PaymentNo,
              
                InAmount = Convert.ToDecimal(vmPayment.InAmount),
                ReferenceNo = vmPayment.ReferenceNo,
                
                TransactionDate = vmPayment.TransactionDate,
                VendorId = vmPayment.CustomerId.Value,
                PaymentFromHeadGLId = vendors.HeadGLId, // Customer Head Id

                CompanyId = vmPayment.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                BankId = vmPayment.Accounting_BankOrCashId,
                BranchName= vmPayment.BankName,
                OrderMasterId = vmPayment.OrderMasterId.Value,
                PurchaseOrderId = 0,
                OutAmount = 0,
                ProductType = vmPayment.ProductType,
                ReceiveLocation=vmPayment.ReceiveLocation
            };
            _db.Payments.Add(payment);




            if (await _db.SaveChangesAsync() > 0)
            {
                result = payment.PaymentId;
            }

            return result;
        }





       
        public async Task<long> PaymentAdd(VMPayment vmPayment)
        {
            long result = -1;
            var vendor = _db.Vendors.Find(vmPayment.CustomerId);
            Payment payment = new Payment
            {
                PaymentMasterId = vmPayment.PaymentMasterId,
                InAmount = Convert.ToDecimal(vmPayment.InAmount),
                OutAmount = vmPayment.OutAmount,
                ProductType = "F",
                ReferenceNo = vmPayment.ReferenceNo,
                TransactionDate = vmPayment.MoneyReceiptDate.Value,
                MoneyReceiptNo = vmPayment.MoneyReceiptNo,
                VendorId = vmPayment.CustomerId.Value,
                OrderMasterId = vmPayment.OrderMasterId,
                PurchaseOrderId = vmPayment.PurchaseOrderId,
                CompanyId = vmPayment.CompanyFK.Value,
                PaymentFromHeadGLId = vendor.HeadGLId,
                //PaymentToHeadGLId = vmPayment.PaymentToHeadGLId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,

            };
            _db.Payments.Add(payment);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = payment.PaymentId;
            }


            return result;
        }
        public async Task<long> ExpensesAdd(VMPayment vmPayment)
        {
            long result = -1;

            Expens expens = new Expens
            {
                Amount = vmPayment.ExpensesAmount.Value,
                CompanyId = vmPayment.CompanyFK.Value,
                ExpensesHeadGLId = vmPayment.ExpensesHeadGLId,
                PaymentMasterId = vmPayment.PaymentMasterId,
                ReferenceNo = vmPayment.ExpensessReference,
                OutAmount = 0,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,

            };
            _db.Expenses.Add(expens);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = expens.ExpensesId;
            }


            return result;
        }

        public async Task<long> IncomeAdd(VMPayment vmPayment)
        {
            long result = -1;

            Income income = new Income
            {
                Amount = vmPayment.OthersIncomeAmount.Value,
                CompanyId = vmPayment.CompanyFK.Value,
                IncomeHeadGLId = vmPayment.OthersIncomeHeadGLId,
                PaymentMasterId = vmPayment.PaymentMasterId,
                ReferenceNo = vmPayment.IncomeReference,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,

            };
            _db.Incomes.Add(income);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = income.IncomeId;
            }


            return result;
        }
        public async Task<int> FinalizeCollectionFeed(FeedSalesOrderModel vmPayment)
        {
            var result = -1;
            PaymentMaster model = await _db.PaymentMasters.FindAsync(vmPayment.PaymentMasterId);
            model.IsFinalized = true;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.PaymentMasterId;
            }
            ////dfdf

            return result;
        }



        public async Task<int> SubmitCollectionFeed(FeedSalesOrderModel vmPayment)
        {
            var result = -1;
            PaymentMaster model = await _db.PaymentMasters.FindAsync(vmPayment.PaymentMasterId);
            model.IsFeedVoucherCreated = true;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.PaymentMasterId;
            }
            if (result > 0 && vmPayment.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
            {

                vmPayment = await Task.Run(() => FeedPaymentGet(vmPayment.CompanyFK.Value, model.PaymentMasterId));

                await _accountingService.CullectionPushFeed(vmPayment.CompanyFK.Value, vmPayment, (int)FeedJournalEnum.CreditVoucher);


            }

            return result;
        }


        public async Task<int> SubmitCollectionMasters(VMPayment vmPayment)
        {
            var result = -1;
            PaymentMaster model = await _db.PaymentMasters.FindAsync(vmPayment.PaymentMasterId);
            model.IsFinalized = true;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.PaymentMasterId;
            }
            if (result > 0 && vmPayment.CompanyFK == (int)CompanyName.GloriousCropCareLimited)
            {

                vmPayment = await Task.Run(() => ProcurementOrderMastersGetByID(vmPayment.CompanyFK.Value, model.PaymentMasterId)); // , model.VendorId, 

                await _accountingService.CullectionPushGCCL(vmPayment.CompanyFK.Value, vmPayment, (int)GCCLJournalEnum.CreditVoucher);


            }

            return result;
        }

        public async Task<int> SubmitPaymentMasters(VMPayment vmPayment)
        {
            var result = -1;
            PaymentMaster model = await _db.PaymentMasters.FindAsync(vmPayment.PaymentMasterId);
            model.IsFinalized = true;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.PaymentMasterId;
            }
            if (result > 0 && model.CompanyId == (int)CompanyName.GloriousCropCareLimited)
            {

                vmPayment = await Task.Run(() => ProcurementPurchaseOrdersGetByID(model.CompanyId, model.VendorId, model.PaymentMasterId));

                await _accountingService.PaymentPushGCCL(model.CompanyId, vmPayment, (int)GCCLJournalEnum.DebitVoucher);

            }

            return result;
        }

        public async Task<long> SupplierPaymentAdd(VMPurchaseOrder vmPurchaseOrder)
        {
            long result = -1;

            Payment payment = new Payment
            {

                InAmount = Convert.ToDecimal(vmPurchaseOrder.PaidAmount),
                TransactionDate = vmPurchaseOrder.PaymentDate,
                VendorId = vmPurchaseOrder.Common_SupplierFK.Value,
                ReferenceNo = vmPurchaseOrder.Remarks,
                ProductType = "R",


                CompanyId = vmPurchaseOrder.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Payments.Add(payment);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = payment.PaymentId;
            }
            return result;
        }


        public async Task<VmTransaction> GetLedgerInfoBySupplier(VmTransaction vmTransaction)
        {
            List<VmTransaction> tempList = new List<VmTransaction>();
            VmTransaction vmTransition = new VmTransaction();


            var DataList1 = (from t1 in _db.PurchaseOrders
                             join t2 in _db.Vendors
                             on t1.SupplierId equals t2.VendorId

                             where t1.SupplierId == vmTransaction.VendorFK && t1.IsActive == true && t1.Status == (int)EnumPOStatus.Submitted
                             select new VmTransaction
                             {
                                 PurchaseOrdersId = t1.PurchaseOrderId,
                                 Date = t1.PurchaseDate,
                                 Description = "PO ID : " + t1.PurchaseOrderNo,
                                 Credit = (from ts1 in _db.PurchaseOrderDetails
                                           join ts2 in _db.MaterialReceiveDetails on ts1.PurchaseOrderDetailId equals ts2.PurchaseOrderDetailFk
                                           where ts1.PurchaseOrderId == t1.PurchaseOrderId && ts1.IsActive && ts2.IsActive && !ts2.IsReturn
                                           select ts2.ReceiveQty * ts1.PurchaseRate).DefaultIfEmpty(0).Sum(),//Debit
                                 Debit = 0,
                                 Balance = 0,
                                 FirstCreateDate = t1.CreatedDate
                             }).Distinct().AsEnumerable();


            var DataList2 = (from t1 in _db.Payments

                             where t1.VendorId == vmTransaction.VendorFK && t1.IsActive == true
                             select new VmTransaction
                             {
                                 Date = t1.TransactionDate,
                                 Description = "Payment Reference : " + t1.ReferenceNo,
                                 //Debit = 0,
                                 //Credit = t1.Amount,
                                 Credit = 0,
                                 Debit = t1.InAmount,
                                 Balance = 0,
                                 FirstCreateDate = t1.CreatedDate
                             }).Distinct().AsEnumerable();
            var DataList = DataList1.Union(DataList2).OrderBy(x => x.Date).AsEnumerable();


            var previuosBalanceTable = (from t in DataList
                                        where t.Date < vmTransaction.FromDate
                                        select
                                        (
                                         //t.Debit - t.Credit
                                         t.Credit - t.Debit
                                        )).ToList();

            var countForId = previuosBalanceTable.Count();

            var previuosBalance = (previuosBalanceTable).DefaultIfEmpty(0).Sum();

            var sortedV = (from t in DataList
                           where t.Date >= vmTransaction.FromDate && t.Date <= vmTransaction.ToDate
                           select new VmTransaction
                           {
                               ID = ++countForId,
                               Date = t.Date,
                               Description = t.Description,
                               Credit = t.Credit,
                               Debit = t.Debit,
                               Balance = 0
                           }).Distinct().ToList();


            var supplier = _db.Vendors.Find(vmTransaction.VendorFK);
            var companies = _db.Companies.Find(vmTransaction.CompanyFK);



            vmTransition.Date = vmTransaction.FromDate;
            vmTransition.Name = supplier.Name;
            vmTransition.Description = "Opening Balance";
            vmTransition.Debit = 0;
            vmTransition.Credit = 0;
            vmTransition.Balance = previuosBalance;
            vmTransition.CompanyAddress = companies.Address;
            vmTransition.CompanyName = companies.Name;
            vmTransition.CompanyPhone = companies.Phone;
            vmTransition.CompanyEmail = companies.Email;
            vmTransition.FromDate = vmTransaction.FromDate;
            vmTransition.ToDate = vmTransaction.ToDate;
            tempList.Add(vmTransition);

            foreach (var x in sortedV)
            {
                x.Balance = previuosBalance += x.Credit - x.Debit;
                x.Name = supplier.Name;
                tempList.Add(x);
            }
            vmTransition.DataList = tempList;

            return vmTransition;
        }



        public async Task<VmTransaction> GetLedgerInfoByCustomer(VmTransaction vmTransaction)
        {
            List<VmTransaction> tempList = new List<VmTransaction>();
            VmTransaction vmTransition = new VmTransaction();


            var DataList1 = (from t1 in _db.OrderMasters
                             join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId

                             where t1.CustomerId == vmTransaction.VendorFK && t1.IsActive == true && t1.Status == (int)EnumPOStatus.Submitted
                             select new VmTransaction
                             {
                                 OrderMasterId = t1.OrderMasterId,
                                 Date = t1.OrderDate,
                                 Description = "Order No : " + t1.OrderNo,
                                 Credit = 0,
                                 Debit = 0,
                                 Balance = 0,
                                 CourierCharge = t1.CourierCharge,
                                 FirstCreateDate = t1.CreateDate
                             }).Distinct().ToList();
            foreach (var data in DataList1)
            {
                data.Credit = GetOrderValue(data.OrderMasterId) + Convert.ToDecimal(data.CourierCharge);
            }


            var DataList2 = (from t1 in _db.Payments
                             where t1.VendorId == vmTransaction.VendorFK && t1.IsActive == true
                             select new VmTransaction
                             {
                                 Date = t1.TransactionDate,
                                 Description = "Payment Reference : " + t1.ReferenceNo,
                                 //Debit = 0,
                                 //Credit = t1.Amount,
                                 Credit = 0,
                                 Debit = t1.InAmount,
                                 Balance = 0,
                                 FirstCreateDate = t1.CreatedDate
                             }).Distinct().ToList();
            var DataList3 = (from t1 in _db.SaleReturnDetails
                             join t2 in _db.SaleReturns on t1.SaleReturnId equals t2.SaleReturnId
                             where t2.CustomerId == vmTransaction.VendorFK && t1.IsActive == true
                             select new VmTransaction
                             {
                                 OrderMasterId = t2.SaleReturnId,
                                 Date = t2.ReturnDate,
                                 Description = "Return Reference : " + t2.SaleReturnNo,
                                 //Debit = 0,
                                 //Credit = t1.Amount,
                                 Credit = 0,
                                 Debit = 0, //t1.InAmount,
                                 Balance = 0,
                                 FirstCreateDate = t2.ReturnDate
                             }).Distinct().ToList();

            foreach (var data in DataList3)
            {
                data.Credit = GetReturnOrderValue(data.OrderMasterId); // Sales Return Id
            }

            var DataList = DataList1.Union(DataList2).Union(DataList3).OrderBy(x => x.Date).ToList();


            var previuosBalanceTable = (from t in DataList
                                        where t.Date < vmTransaction.FromDate
                                        select
                                        (
                                         //t.Debit - t.Credit
                                         t.Credit - t.Debit
                                        )).ToList();

            var countForId = previuosBalanceTable.Count();

            var previuosBalance = (previuosBalanceTable).DefaultIfEmpty(0).Sum();

            var sortedV = (from t in DataList
                           where t.Date >= vmTransaction.FromDate && t.Date <= vmTransaction.ToDate
                           select new VmTransaction
                           {
                               ID = ++countForId,
                               Date = t.Date,
                               Description = t.Description,
                               Credit = t.Credit,
                               Debit = t.Debit,
                               Balance = 0
                           }).Distinct().ToList();


            var supplier = _db.Vendors.Find(vmTransaction.VendorFK);
            var companies = _db.Companies.Find(vmTransaction.CompanyFK);



            vmTransition.Date = vmTransaction.FromDate;
            vmTransition.Name = supplier.Name;
            vmTransition.Description = "Opening Balance";
            vmTransition.Debit = 0;
            vmTransition.Credit = 0;
            vmTransition.Balance = previuosBalance;
            vmTransition.CompanyAddress = companies.Address;
            vmTransition.CompanyName = companies.Name;
            vmTransition.CompanyPhone = companies.Phone;
            vmTransition.CompanyEmail = companies.Email;
            vmTransition.FromDate = vmTransaction.FromDate;
            vmTransition.ToDate = vmTransaction.ToDate;



            tempList.Add(vmTransition);

            foreach (var x in sortedV)
            {
                x.Balance = previuosBalance += x.Credit - x.Debit;// x.Debit - x.Credit;
                x.Name = supplier.Name;
                tempList.Add(x);
            }


            vmTransition.DataList = tempList;

            return vmTransition;
        }

        private decimal GetOrderValue(long orderMasterId)
        {
            double orderValue = (from ts1 in _db.OrderDetails
                                 join ts2 in _db.OrderDeliverDetails on ts1.OrderDetailId equals ts2.OrderDetailId
                                 where ts1.OrderMasterId == orderMasterId && ts1.IsActive && ts2.IsActive && !ts2.IsReturn
                                 select ts2.DeliveredQty * ts1.UnitPrice).DefaultIfEmpty(0).Sum();
            return Convert.ToDecimal(orderValue);

        }
        private decimal GetReturnOrderValue(long saleReturnId)
        {
            decimal orderValue = (from ts0 in _db.SaleReturnDetails
                                  join ts1 in _db.OrderDeliverDetails on ts0.OrderDeliverDetailsId equals ts1.OrderDeliverDetailId
                                  where ts0.SaleReturnId == saleReturnId && ts1.IsActive && ts0.IsActive
                                  select new
                                  {
                                      ReturnQuantity = ts0.Qty,
                                      UnitPrice = ts1.UnitPrice
                                  }).AsEnumerable().Select(x => x.ReturnQuantity.Value * Convert.ToDecimal(x.UnitPrice)).DefaultIfEmpty(0).Sum();
            return orderValue;
        }


        public IEnumerable<VmCustomerAgeing> CustomerAgeingGet(VmCustomerAgeing vmCustomerAgeing)
        {
            return _db.Database.SqlQuery<VmCustomerAgeing>("[dbo].[GCCLCustomerAgeingNew] {0},{1},{2},{3}", vmCustomerAgeing.CompanyFK.Value, vmCustomerAgeing.AsOnDate, vmCustomerAgeing.ZoneId ?? 0, vmCustomerAgeing.SubZoneId ?? 0).AsEnumerable();
        }

        public async Task<VMCommonSupplier> GetCustomer(int companyId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  Common_DistrictsFk = t2.DistrictId,
                                                                  Common_UpazilasFk = t1.UpazilaId.Value,
                                                                  District = t3.Name,
                                                                  Upazila = t2.Name,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonCustomer;
        }

        public List<object> ZonesDropDownList(int companyId = 0)
        {
            var List = new List<object>();
            _db.Zones
        .Where(x => x.IsActive).Where(x => x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.ZoneId,
            Text = x.Name
        }));
            return List;

        }




        public async Task<FeedSalesOrderModel> GetPaymentEdit(int paymentId)
        {
            var obj = await _db.Payments
                                .Where(x => x.PaymentId == paymentId)
                                .Select(x => new FeedSalesOrderModel
                                {
                                   
                                    PaymentId = x.PaymentId,
                                    TransactionDate = x.TransactionDate,
                                    BankId= x.BankId,
                                    ReferenceNo=x.ReferenceNo,
                                    InAmount = x.InAmount,
                                    BranchName= x.BranchName,
                                    ReceiveLocation=x.ReceiveLocation
                                   
                                })
                                .FirstOrDefaultAsync();

            return obj;
        }

        public async Task<bool> DeletePayment(int paymentId)
        {
            var payment = await _db.Payments.SingleOrDefaultAsync(x => x.PaymentId == paymentId);

            if (payment != null)
            {
                if (payment.IsActive)
                {
                    payment.IsActive = false;
                    await _db.SaveChangesAsync();  // Save changes to the database
                    return true;  // Successfully deleted payment
                }
                else
                {
                   
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



    }
}
