using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KGERP.Service.ServiceModel;
using KGERP.Data.CustomModel;
using Officervwmodel = KGERP.Data.CustomModel.Officervwmodel;
using KGERP.Service.Implementation.LcInfoServices;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.OrderApproval.ViewModels;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using System.Web.Mvc;
using KGERP.Services.WareHouse;
using System.Threading;
using KGERP.CustomModel;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Web.UI.WebControls;

namespace KGERP.Services.Procurement
{
    public class ProcurementService
    {
        private readonly ERPEntities _db;
        private readonly AccountingService _accountingService;


        public ProcurementService(ERPEntities db)
        {
            _db = db;
            _accountingService = new AccountingService(db);

        }
        #region Common
        public List<SelectItemList> CommonTremsAndConditionDropDownList(int companyId)
        {
            var data = (from a in _db.POTremsAndConditions
                        where a.IsActive && a.CompanyId == companyId
                        select new SelectItemList()
                        {
                            Value = a.ID,
                            Text = a.Key
                        }).ToList();
            return data;

        }

        public List<object> CountriesDropDownList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.Countries.ToList())
            {
                List.Add(new { Text = item.CountryName, Value = item.CountryId });
            }
            return List;

        }

        public List<object> EmployeesByCompanyDropDownList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.Employees.Where(x => x.Active && x.CompanyId == companyId).ToList())
            {
                List.Add(new { Text = item.Name, Value = item.Id });
            }
            return List;

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
        public List<object> SeedLCHeadGLList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.HeadGLs.Where(x => x.CompanyId == companyId && x.ParentId == 38055 || x.ParentId == 43634 || x.ParentId == 34822).ToList())
            {
                List.Add(new { Text = item.AccCode + " -" + item.AccName, Value = item.Id });
            }
            return List;

        }

        public List<object> GCCLLCHeadGLList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.HeadGLs.Where(x => x.CompanyId == companyId && x.ParentId == 37952).ToList())
            {
                List.Add(new { Text = item.AccCode + " -" + item.AccName, Value = item.Id });
            }
            return List;

        }
        public List<object> KPPLLCHeadGLList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.HeadGLs.Where(x => x.CompanyId == companyId && x.ParentId == 37952).ToList())
            {
                List.Add(new { Text = item.AccCode + " -" + item.AccName, Value = item.Id });
            }
            return List;

        }
        public List<object> PackagingLCHeadGLList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.HeadGLs.Where(x => x.CompanyId == companyId && x.ParentId == 40478).ToList())
            {
                List.Add(new { Text = item.AccCode + " -" + item.AccName, Value = item.Id });
            }
            return List;

        }
        public List<object> KFMALCHeadGLList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.HeadGLs.Where(x => x.CompanyId == companyId && x.ParentId == 50604109).ToList())
            {
                List.Add(new { Text = item.AccCode + " -" + item.AccName, Value = item.Id });
            }
            return List;

        }
        public List<object> ShippedByListDropDownList(int companyId)
        {
            var List = new List<object>();
            List.Add(new { Text = "Air", Value = "Air" });
            List.Add(new { Text = "Ship", Value = "Ship" });
            return List;

        }

        public List<object> ProcurementPurchaseOrderDropDownBySupplier(int supplierId)
        {
            var procurementPurchaseOrderList = new List<object>();
            _db.PurchaseOrders.Where(x => x.IsActive && x.SupplierId == supplierId).Select(x => x).ToList().ForEach(x => procurementPurchaseOrderList.Add(new
            {
                Value = x.PurchaseOrderId.ToString(),
                Text = x.PurchaseOrderNo + " Date: " + x.PurchaseDate.Value.ToLongDateString()
            }));
            return procurementPurchaseOrderList;
        }

        public List<object> ProductCategoryDropDownList()
        {
            var List = new List<object>();
            _db.ProductCategories
        .Where(x => x.IsActive).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.ProductCategoryId,
            Text = x.Name
        }));
            return List;

        }

        public object GetAutoCompleteOrderNoGet(string prefix, int companyId)
        {
            var v = (from t1 in _db.OrderMasters.Where(x => x.CompanyId == companyId)
                     where t1.IsActive && ((t1.OrderNo.StartsWith(prefix)))

                     select new
                     {
                         label = t1.OrderNo,
                         val = t1.OrderMasterId

                     }).OrderByDescending(x => x.label).ToList();

            return v;
        }

        public object GetAutoCompleteOrderNoGetRequsitionId(string prefix, int companyId)
        {
            var v = (from t1 in _db.Requisitions.Where(x => x.CompanyId == companyId)
                     join t2 in _db.OrderDetails on t1.OrderDetailsId equals t2.OrderDetailId
                     join t3 in _db.OrderMasters on t2.OrderMasterId equals t3.OrderMasterId
                     join t4 in _db.Products on t2.ProductId equals t4.ProductId
                     join t5 in _db.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                     join t6 in _db.Vendors on t3.CustomerId equals t6.VendorId


                     where t1.IsActive && ((t1.RequisitionNo.StartsWith(prefix)) || (t2.JobOrderNo.StartsWith(prefix))
                     || (t3.OrderNo.StartsWith(prefix)) || (t5.Name.StartsWith(prefix)) || (t4.ProductName.StartsWith(prefix))
                     || (t6.Name.StartsWith(prefix))
                     )

                     select new
                     {
                         label = t1.RequisitionNo + " " + t1.RequisitionDate + " " + t2.JobOrderNo + " " +
                         t5.Name + " " + t4.ProductName + " " + t2.Remarks + " " + t6.Name,
                         val = t1.RequisitionId

                     }).OrderBy(x => x.label).Take(20).ToList();

            return v;
        }

        public string GetRequisitionNoByRequsitionId(int requisitionId, int companyId)
        {
            var v = (from t1 in _db.Requisitions
                     join t2 in _db.OrderDetails on t1.OrderDetailsId equals t2.OrderDetailId
                     join t3 in _db.OrderMasters on t2.OrderMasterId equals t3.OrderMasterId
                     join t4 in _db.Products on t2.ProductId equals t4.ProductId
                     join t5 in _db.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                     join t6 in _db.Vendors on t3.CustomerId equals t6.VendorId
                     where t1.IsActive && t1.RequisitionId == requisitionId && t1.CompanyId == companyId
                     select new
                     {
                         label = t1.RequisitionNo + " " + t1.RequisitionDate + " " + t2.JobOrderNo + " " +
                         t5.Name + " " + t4.ProductName + " " + t2.Remarks + " " + t6.Name,

                     }).OrderBy(x => x.label).FirstOrDefault();

            return v.label;
        }
        public string GetGeneralRequisitionNoByRequsitionId(int requisitionId, int companyId)
        {

            var v = (from t1 in _db.Requisitions
                     where t1.IsActive && t1.RequisitionId == requisitionId && t1.CompanyId == companyId
                     select new
                     {
                         label = t1.RequisitionNo + " " + t1.RequisitionDate
                     }
                     ).FirstOrDefault();
            return v.label;
        }




        public object GetAutoCompleteByRequsitionId(string prefix, int companyId)
        {
            var v = (from t1 in _db.Requisitions.Where(x => x.CompanyId == companyId)


                     where t1.IsActive && t1.OrderDetailsId == 0 && t1.RequisitionNo.StartsWith(prefix)


                     select new
                     {
                         label = t1.RequisitionNo + " " + t1.RequisitionDate,
                         val = t1.RequisitionId

                     }).OrderBy(x => x.label).Take(20).ToList();

            return v;
        }

        public object GetAutoCompleteStyleNo(int orderMasterId)
        {
            var v = (from t1 in _db.OrderDetails.Where(x => x.OrderMasterId == orderMasterId && x.IsActive && x.Status == (int)EnumPOStatus.Submitted)
                     join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                     join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId

                     select new
                     {
                         val = t1.OrderDetailId,
                         lable = t4.Name + " " + t3.ProductName + " Job No: " + t1.JobOrderNo + " Job Date: " + t1.OrderDate + " Qty: " + t1.Qty

                     }).ToList();

            return v;


        }



        public List<object> ProductSubCategoryDropDownList(int id = 0)
        {
            var List = new List<object>();
            _db.ProductSubCategories
        .Where(x => x.IsActive).Where(x => x.ProductCategoryId == id || id <= 0).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.ProductSubCategoryId,
            Text = x.Name
        }));
            return List;

        }

        public List<object> SubZonesDropDownList(int companyId = 0)
        {
            var List = new List<object>();
            _db.SubZones
        .Where(x => x.IsActive).Where(x => x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.SubZoneId,
            Text =/* x.SalesOfficerName + " -" + */x.Name
        }));
            return List;

        }

        public List<object> SubZonesDropDownListUserWise(long userId, int companyId = 0)

        {
            var List = new List<object>();
            _db.SubZones
        .Where(x => x.IsActive).Where(x => x.CompanyId == companyId && x.SalesOfficerId==userId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.SubZoneId,
            Text =/* x.SalesOfficerName + " -" + */x.Name
        }));
            return List;

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
        public List<object> PromtionalOffersDropDownList(int companyId = 0)
        {
            var list = new List<object>();

            var currentDate = DateTime.Now.Date; // Get the current date without time  

            var offers = _db.PromtionalOffers
                .Where(offer => offer.IsActive == true &&
                                offer.CompanyId == companyId &&
                                offer.IsSubmitted == true &&
                                DbFunctions.TruncateTime(currentDate) >= DbFunctions.TruncateTime(offer.FromDate) && // Compare only dates  
                                DbFunctions.TruncateTime(currentDate) <= DbFunctions.TruncateTime(offer.ToDate)) // Compare only dates  
                .Select(offer => new
                {
                    Value = offer.PromtionalOfferId,
                    Text = offer.PromoCode
                })
                .ToList();

            foreach (var offer in offers)
            {
                list.Add(new { offer.Value, offer.Text });
            }
            return list;
        }



        public List<object> StockInfoesDropDownList(int companyId = 0)
        {
            var List = new List<object>();
            _db.StockInfoes
         .Where(x => x.IsActive && x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.StockInfoId,
            Text = x.Name
        }));
            return List;

        }
        public List<object> KFMALLCVAlue(int companyId = 0)
        {
            var List = new List<object>();
            _db.LCInfoes
         .Where(x => x.IsActive && x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.LCId,
            Text = x.LCNo
        }));
            return List;

        }
        public List<object> StockInfoesDropDownt(int companyId)
        {
            var List = new List<object>();
            _db.StockInfoes
         .Where(x => x.IsActive && x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.StockInfoId,
            Text = x.Name
        }));
            return List;

        }
        public List<object> ProductDropDownList(int id = 0)
        {
            var List = new List<object>();
            _db.Products
        .Where(x => x.IsActive).Where(x => x.ProductSubCategoryId == id || id <= 0).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.ProductId,
            Text = x.ProductName
        }));
            return List;

        }



        #endregion

        public async Task<VMSalesOrder> ProcurementOrderMastersListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder.CompanyFK = companyId;

            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.OrderMasters
                                                          .Where(x => x.IsActive
                                                          && x.ProductType == "F"
                                                          && x.CompanyId == companyId
                                                          && x.OrderDate >= fromDate && x.OrderDate <= toDate
                                                          && !x.IsOpening
                                                          && x.Status < (int)EnumOrderMasterStatus.closed)
                                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId

                                                          select new VMSalesOrder
                                                          {
                                                              OrderMasterId = t1.OrderMasterId,
                                                              CustomerId = t1.CustomerId.Value,
                                                              CommonCustomerName = t2.Name,
                                                              CreatedBy = t1.CreatedBy,
                                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                              OrderNo = t1.OrderNo,
                                                              OrderDate = t1.OrderDate,
                                                              ExpectedDeliveryDate = t1.ExpectedDeliveryDate,

                                                              Status = t1.Status,
                                                              CompanyFK = t1.CompanyId,
                                                              CourierNo = t1.CourierNo,
                                                              FinalDestination = t1.FinalDestination,
                                                              CourierCharge = t1.CourierCharge

                                                          }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());
            if (vStatus != -1 && vStatus != null)
            {
                vmSalesOrder.DataList = vmSalesOrder.DataList.Where(q => q.Status == vStatus);
            }
            return vmSalesOrder;
        }

        public async Task<IssueDetailInfoVM> PromotionalItemListGet(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            IssueDetailInfoVM detailInfoVM = new IssueDetailInfoVM();
            detailInfoVM.CompanyFK = companyId;

            detailInfoVM.DataListSlave = await (from t1 in _db.IssueMasterInfoes
                                                          .Where(x => x.IsActive
                                                          && x.CompanyId == companyId
                                                          && x.IssueDate >= fromDate && x.IssueDate <= toDate)
                                                join t2 in _db.Vendors on t1.VendorId equals t2.VendorId

                                                select new IssueDetailInfoVM
                                                {
                                                    IssueMasterId = t1.IssueMasterId,
                                                    IssueNo = t1.IssueNo,
                                                    CustomerName = t2.Name,
                                                    CreatedBy = t1.CreatedBy,
                                                    IsSubmit = t1.IsSubmit,
                                                    IssueDate = t1.IssueDate,
                                                    Remarks = t1.Remarks

                                                }).OrderByDescending(x => x.IssueMasterId).ToListAsync();

            return detailInfoVM;
        }

        public async Task<VMSalesOrder> ProcurementOrderMastersRMListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder.CompanyFK = companyId;

            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.OrderMasters
                                                          .Where(x => x.IsActive
                                                          && x.ProductType == "R"
                                                          && x.CompanyId == companyId
                                                          && x.OrderDate >= fromDate && x.OrderDate <= toDate
                                                          && !x.IsOpening
                                                          && x.Status < (int)EnumOrderMasterStatus.closed)
                                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId

                                                          select new VMSalesOrder
                                                          {
                                                              OrderMasterId = t1.OrderMasterId,
                                                              CustomerId = t1.CustomerId.Value,
                                                              CommonCustomerName = t2.Name,
                                                              CreatedBy = t1.CreatedBy,
                                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                              OrderNo = t1.OrderNo,
                                                              OrderDate = t1.OrderDate,
                                                              ExpectedDeliveryDate = t1.ExpectedDeliveryDate,

                                                              Status = t1.Status,
                                                              CompanyFK = t1.CompanyId,
                                                              CourierNo = t1.CourierNo,
                                                              FinalDestination = t1.FinalDestination,
                                                              CourierCharge = t1.CourierCharge

                                                          }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());
            if (vStatus != -1 && vStatus != null)
            {
                vmSalesOrder.DataList = vmSalesOrder.DataList.Where(q => q.Status == vStatus);
            }
            return vmSalesOrder;
        }
        public async Task<OrderDetailVM> GetOrderDetailsList(long companyId, DateTime? fromDate, DateTime? toDate)
        {
            OrderDetailVM model = new OrderDetailVM();

            var orderMasterList = _db.OrderMasters.Where(x => x.CompanyId == companyId && x.IsActive == true).Select(x => x.OrderMasterId).ToList();
            model.OrderDetailList = await Task.Run(() => (from t1 in _db.OrderDetails
                                                          join t2 in _db.OrderMasters on t1.OrderMasterId equals t2.OrderMasterId
                                                          join t3 in _db.Vendors on t2.CustomerId equals t3.VendorId
                                                          join t4 in _db.Products on t1.ProductId equals t4.ProductId
                                                          join t6 in _db.ProductSubCategories on t4.ProductSubCategoryId equals t6.ProductSubCategoryId
                                                          join t5 in _db.Units on t4.UnitId equals t5.UnitId
                                                          where (t2.CompanyId == companyId && t2.IsActive == true
                                                          && t1.IsActive == true && t1.OrderDate >= fromDate)
                                                          && (t2.CompanyId == companyId && t2.IsActive == true &&
                                                          t1.IsActive == true
                                                          && !toDate.HasValue || t1.OrderDate <= toDate)
                                                          select new OrderDetailVM
                                                          {
                                                              OrderDetailID = t1.OrderDetailId,
                                                              CompanyName = t3.Name,
                                                              ItemId = t4.ProductId,
                                                              ItemName = t6.Name + " " + t4.ProductName,
                                                              Qty = t1.Qty,
                                                              Unit = t5.Name,
                                                              InvoiceNo = t2.OrderNo,
                                                              JobOrderNo = t1.JobOrderNo,
                                                              OrderDate = t2.OrderDate,
                                                              DeliveryDate = t2.ExpectedDeliveryDate,
                                                              Remarks = t2.Remarks,
                                                              companyId = t2.CompanyId.Value,
                                                              Structure = t1.Remarks,
                                                              PouchDerection = t1.PouchDerection,
                                                              ReelDirection = t1.ReelDirection,
                                                              JobOrderDate = t1.OrderDate,
                                                              StatusId = t1.Status


                                                          }).OrderByDescending(x => x.OrderDetailID).ToList());
            return model;

        }
        public async Task<VMSalesOrder> KFMALProcurementOrderMastersListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder.CompanyFK = companyId;

            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.OrderMasters
                                                          .Where(x => x.IsActive && !x.IsService
                                                          && x.CompanyId == companyId
                                                          && x.OrderDate >= fromDate && x.OrderDate <= toDate
                                                          && !x.IsOpening
                                                          && x.Status < (int)EnumPOStatus.Closed)
                                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId

                                                          select new VMSalesOrder
                                                          {
                                                              OrderMasterId = t1.OrderMasterId,
                                                              CustomerId = t1.CustomerId.Value,
                                                              CommonCustomerName = t2.Name,
                                                              CreatedBy = t1.CreatedBy,
                                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                              OrderNo = t1.OrderNo,
                                                              OrderDate = t1.OrderDate,
                                                              ExpectedDeliveryDate = t1.ExpectedDeliveryDate,

                                                              Status = t1.Status,
                                                              CompanyFK = t1.CompanyId,
                                                              CourierNo = t1.CourierNo,
                                                              FinalDestination = t1.FinalDestination,
                                                              CourierCharge = t1.CourierCharge

                                                          }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());
            if (vStatus != -1 && vStatus != null)
            {
                vmSalesOrder.DataList = vmSalesOrder.DataList.Where(q => q.Status == vStatus);
            }
            return vmSalesOrder;
        }



        public async Task<VMSalesOrder> KFMALProcurementOrderMastersServiceListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder.CompanyFK = companyId;

            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.OrderMasters
                                                          .Where(x => x.IsActive && x.IsService
                                                          && x.CompanyId == companyId
                                                          && x.OrderDate >= fromDate && x.OrderDate <= toDate
                                                          && !x.IsOpening
                                                          && x.Status < (int)EnumPOStatus.Closed)
                                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId

                                                          select new VMSalesOrder
                                                          {
                                                              OrderMasterId = t1.OrderMasterId,
                                                              CustomerId = t1.CustomerId.Value,
                                                              CommonCustomerName = t2.Name,
                                                              CreatedBy = t1.CreatedBy,
                                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                              OrderNo = t1.OrderNo,
                                                              OrderDate = t1.OrderDate,
                                                              ExpectedDeliveryDate = t1.ExpectedDeliveryDate,

                                                              Status = t1.Status,
                                                              CompanyFK = t1.CompanyId,
                                                              CourierNo = t1.CourierNo,
                                                              FinalDestination = t1.FinalDestination,
                                                              CourierCharge = t1.CourierCharge

                                                          }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());
            if (vStatus != -1 && vStatus != null)
            {
                vmSalesOrder.DataList = vmSalesOrder.DataList.Where(q => q.Status == vStatus);
            }
            return vmSalesOrder;
        }


        public async Task<VMSalesOrder> PackagingOrderMastersListGet(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder.CompanyFK = companyId;

            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.OrderMasters
                                                          .Where(x => x.IsActive
                                                          && x.CompanyId == companyId
                                                              && x.OrderDate >= fromDate && x.OrderDate <= toDate)
                                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId

                                                          select new VMSalesOrder
                                                          {
                                                              OrderMasterId = t1.OrderMasterId,
                                                              CustomerId = t1.CustomerId.Value,
                                                              CommonCustomerName = t2.Name,
                                                              CreatedBy = t1.CreatedBy,
                                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                              OrderNo = t1.OrderNo,
                                                              OrderDate = t1.OrderDate,
                                                              ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                              CustomerPONo = t1.CustomerPONo,
                                                              Status = t1.Status,
                                                              CompanyFK = t1.CompanyId,
                                                              CourierNo = t1.CourierNo,
                                                              FinalDestination = t1.FinalDestination,
                                                              CourierCharge = t1.CourierCharge,
                                                              OrderStatus = t1.OrderStatus

                                                          }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());

            return vmSalesOrder;
        }

        public async Task<VMSalesOrder> PackagingOrderMastersListGetBySalePerson(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder.CompanyFK = companyId;
            var userid = Common.GetUserId();
            var employee = await _db.Employees.Where(x => x.EmployeeId == userid).FirstOrDefaultAsync();


            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.OrderMasters
                                                          .Where(x => x.IsActive
                                                          && x.CompanyId == companyId && x.SalePersonId == employee.Id)
                                                              //&& x.OrderDate >= fromDate && x.OrderDate <= toDate
                                                              //&& !x.IsOpening)

                                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId


                                                          select new VMSalesOrder
                                                          {
                                                              OrderMasterId = t1.OrderMasterId,
                                                              CustomerId = t1.CustomerId.Value,
                                                              CommonCustomerName = t2.Name,
                                                              CreatedBy = t1.CreatedBy,
                                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                              OrderNo = t1.OrderNo,
                                                              OrderDate = t1.OrderDate,
                                                              ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                              CustomerPONo = t1.CustomerPONo,
                                                              Status = t1.Status,
                                                              CompanyFK = t1.CompanyId,
                                                              CourierNo = t1.CourierNo,
                                                              FinalDestination = t1.FinalDestination,
                                                              CourierCharge = t1.CourierCharge,
                                                              OrderStatus = t1.OrderStatus

                                                          }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());

            return vmSalesOrder;
        }


        public async Task<VMSalesOrder> GetSinglOrderMasters(int orderMasterId)
        {

            var v = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                          join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                                          join t4 in _db.StockInfoes on t1.StockInfoId equals t4.StockInfoId into LStockInfo
                                          from t5 in LStockInfo.DefaultIfEmpty()
                                          select new VMSalesOrder
                                          {
                                              CompanyFK = t1.CompanyId,
                                              OrderMasterId = t1.OrderMasterId,
                                              OrderNoMsg = t1.OrderNo,
                                              Status = t1.Status,
                                              OrderDate = t1.OrderDate,
                                              CreatedBy = t1.CreatedBy,
                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                              ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                              CommonCustomerName = t2.Name,
                                              CompanyName = t3.Name,
                                              CompanyAddress = t3.Address,
                                              CompanyEmail = t3.Email,
                                              CompanyPhone = t3.Phone,
                                              CourierNo = t1.CourierNo,
                                              FinalDestination = t1.FinalDestination,
                                              CourierCharge = t1.CourierCharge,
                                              CustomerId = (int)t1.CustomerId,
                                              CustomerPONo = t1.CustomerPONo,
                                              StockInfoId = t1.StockInfoId.Value,
                                              Remarks = t1.Remarks,
                                              StockInfoNameMsg = t5.Name

                                          }).FirstOrDefault());
            return v;
        }

        public async Task<IssueDetailInfoVM> GetSinglIssueMastersGet(int orderMasterId)
        {

            var v = await (from t1 in _db.IssueMasterInfoes.Where(x => x.IsActive && x.IssueMasterId == orderMasterId)
                           join t2 in _db.Vendors on t1.VendorId equals t2.VendorId
                           join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                           join t4 in _db.Employees on t1.IssuedBy equals t4.Id
                           select new IssueDetailInfoVM
                           {
                               CompanyId = t1.CompanyId,
                               IssueMasterId = t1.IssueMasterId,
                               IssueNo = t1.IssueNo,
                               IssueDate = t1.IssueDate,
                               CreatedBy = t1.CreatedBy,
                               VendorId = t1.VendorId,
                               IssuedBy = t4.Id,
                               IssueBy = t4.Name,
                               CustomerName = t2.Name,
                               CompanyName = t3.Name,
                               CompanyAddress = t3.Address,
                               CompanyEmail = t3.Email,
                               CompanyPhone = t3.Phone,
                               Remarks = t1.Remarks


                           }).FirstOrDefaultAsync();
            return v;
        }
        public async Task<List<VMFinishProductBOM>> GetDetailsBOM(int orderDetailsId)
        {

            var v = await Task.Run(() => (from t1 in _db.FinishProductBOMs.Where(x => x.IsActive && x.OrderDetailId == orderDetailsId)
                                              //join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                          select new VMFinishProductBOM
                                          {
                                              CompanyFK = t1.CompanyId,
                                              ID = t1.ID,
                                              StatusId = t1.Status,
                                              CreatedBy = t1.CreatedBy,
                                              RProductFK = t1.RProductFK,
                                              RequiredQuantity = t1.RequiredQuantity,
                                              OrderDetailId = t1.OrderDetailId.Value
                                              //CompanyId=t3.CompanyId,
                                              //CompanyName = t3.Name,
                                              //CompanyAddress = t3.Address,
                                              //CompanyEmail = t3.Email,
                                              //CompanyPhone = t3.Phone,

                                          }).ToList());
            return v;
        }
        public async Task<long> GCCLOrderMastersDiscountEdit(VMSalesOrder vmSalesOrder)
        {
            long result = -1;
            OrderMaster orderMaster = await _db.OrderMasters.FindAsync(vmSalesOrder.OrderMasterId);

            orderMaster.DiscountRate = vmSalesOrder.DiscountRate;
            orderMaster.DiscountAmount = vmSalesOrder.DiscountAmount;

            orderMaster.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            orderMaster.ModifiedDate = DateTime.Now;


            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmSalesOrder.OrderMasterId;
            }

            return result;
        }
        public async Task<long> OrderMastersEdit(VMSalesOrder vmSalesOrder)
        {
            long result = -1;
            OrderMaster orderMaster = await _db.OrderMasters.FindAsync(vmSalesOrder.OrderMasterId);

            orderMaster.OrderDate = vmSalesOrder.OrderDate;
            orderMaster.CustomerId = vmSalesOrder.CustomerId;
            orderMaster.ExpectedDeliveryDate = vmSalesOrder.ExpectedDeliveryDate;
            orderMaster.PaymentMethod = vmSalesOrder.CustomerPaymentMethodEnumFK;

            orderMaster.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            orderMaster.ModifiedDate = DateTime.Now;


            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmSalesOrder.OrderMasterId;
            }

            return result;
        }

        public DateTime? ReceivedDate { get; set; }
        public async Task<bool> AddAllMappingSignatoryApproval(int orderMasterId, int companyId)
        {
            var order = await _db.OrderMasters.FindAsync(orderMasterId);

            if (order == null)
            {
                return false;
            }



            var signatories = await (from a in _db.OrderMasterSignatories
                                     join e in _db.Employees on a.EmployeeId equals e.Id
                                     join c in _db.Companies on e.CompanyId equals c.CompanyId
                                     join d in _db.Departments on e.DepartmentId equals d.DepartmentId
                                     join ds in _db.Designations on e.DesignationId equals ds.DesignationId
                                     where a.IsActive && a.CompanyId == companyId
                                     select new OrderMasterSignatoryVM()
                                     {
                                         SignatoryId = a.SalesOrderSignatoryId,
                                         Precedence = a.Precedence

                                     }).OrderBy(x => x.Precedence).ToListAsync();

            if (signatories == null)
            {
                return false;
            }
            int status = 0;

            Dictionary<int, int> precedenceMap = new Dictionary<int, int>();
            int sequentialPrecedence = 1;

            foreach (var item in signatories)
            {

                if (!precedenceMap.ContainsKey(item.Precedence))
                {

                    precedenceMap[item.Precedence] = sequentialPrecedence;
                    sequentialPrecedence++;
                }


                int mappedPrecedence = precedenceMap[item.Precedence];


                if (mappedPrecedence == 1)
                {
                    ReceivedDate = DateTime.Now;
                }

                ApprovalOrderMaster approval = new ApprovalOrderMaster()
                {
                    ApprovalStatus = (mappedPrecedence == 1) ? 0 : -1,
                    OrderMasterId = orderMasterId,
                    Comments = "",
                    IsActive = true,
                    SalesOrderSignatoryId = item.SignatoryId,
                    Precedence = mappedPrecedence,
                    ReceivedDate = ReceivedDate,
                };

                _db.ApprovalOrderMasters.Add(approval);
                status += await _db.SaveChangesAsync();
            }



            return false;
        }











        public async Task<long> OrderMastersSubmitrm(long? id = 0)
        {
            long result = -1;
            OrderMaster orderMasters = await _db.OrderMasters.FindAsync(id);



            //if (orderMasters != null)
            //{
            //    if (orderMasters.IsService && orderMasters.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            //    {
            //        #region Ready To Account Integration
            //        var AccData = await Task.Run(() => KfmalProcurementSalesOrderDetailsGet(orderMasters.CompanyId.Value, orderMasters.OrderMasterId));

            //        //VMOrderDeliverDetail AccData = await KfmalOrderDeliverForAcc(vmModel.CompanyFK.Value, Convert.ToInt32(vmModel.OrderDeliverId));
            //        await _accountingService.AccServiceSalesPushKfmal(AccData.CompanyId, AccData, (int)KfmalJournalEnum.SalesVoucher);
            //        //await _accountingService.GCCLOrderDeliverySMSPush(AccData);

            //        #endregion
            //    }
            //}







            if (orderMasters != null)
            {
                if (orderMasters.Status == (int)EnumOrderMasterStatus.Draft)
                {
                    orderMasters.Status = (int)EnumOrderMasterStatus.Approval;
                }
                else
                {
                    orderMasters.Status = (int)EnumOrderMasterStatus.Draft;

                }
                orderMasters.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                orderMasters.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = orderMasters.OrderMasterId;
                }
            }
            return result;
        }





        public async Task<long> OrderMastersSubmit(long? id = 0)
        {
            long result = -1;
            OrderMaster orderMasters = await _db.OrderMasters.FindAsync(id);



            //if (orderMasters != null)
            //{
            //    if (orderMasters.IsService && orderMasters.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            //    {
            //        #region Ready To Account Integration
            //        var AccData = await Task.Run(() => KfmalProcurementSalesOrderDetailsGet(orderMasters.CompanyId.Value, orderMasters.OrderMasterId));

            //        //VMOrderDeliverDetail AccData = await KfmalOrderDeliverForAcc(vmModel.CompanyFK.Value, Convert.ToInt32(vmModel.OrderDeliverId));
            //        await _accountingService.AccServiceSalesPushKfmal(AccData.CompanyId, AccData, (int)KfmalJournalEnum.SalesVoucher);
            //        //await _accountingService.GCCLOrderDeliverySMSPush(AccData);

            //        #endregion
            //    }
            //}







            if (orderMasters != null)
            {
                if (orderMasters.Status == (int)EnumOrderMasterStatus.Draft)
                {
                    orderMasters.Status = (int)EnumOrderMasterStatus.Submit;
                }
                else
                {
                    orderMasters.Status = (int)EnumOrderMasterStatus.Draft;

                }
                orderMasters.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                orderMasters.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = orderMasters.OrderMasterId;
                }
            }
            return result;
        }







        public async Task<long> OrderDetaisSubmit(long? id = 0)
        {
            long result = -1;
            OrderDetail orderDetails = await _db.OrderDetails.FindAsync(id);
            if (orderDetails != null)
            {
                if (orderDetails.Status == (int)EnumPOStatus.Draft)
                {
                    orderDetails.Status = (int)EnumPOStatus.Submitted;
                }
                else
                {
                    orderDetails.Status = (int)EnumPOStatus.Draft;
                }
                orderDetails.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                orderDetails.ModifedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = orderDetails.OrderDetailId;
                }
            }





            return result;
        }
        #region Purchase Order Add Edit Submit Hold-UnHold Cancel-Renew Closed-Reopen Delete
        public async Task<VMPurchaseOrder> ProcurementPurchaseOrderListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;

            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders
                                                             .Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId &&
                                                             x.PurchaseDate >= fromDate && x.PurchaseDate <= toDate
                                                             &&
                                                             !x.IsCancel && !x.IsHold &&
                                                             ((companyId == (int)CompanyName.KrishibidFeedLimited ? x.DemandId == null : 0 == 0))
                                                             )
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId


                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,
                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 CompanyFK = t1.CompanyId,
                                                                 CountryId = t1.CountryId,
                                                                 FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                                                 OtherCharge = t1.OtherCharge,
                                                                 FreightCharge = t1.FreightCharge,
                                                                 PINo = t1.PINo,
                                                                 PortOfDischarge = t1.PortOfDischarge,
                                                                 PortOfLoading = t1.PortOfLoading,
                                                                 ShippedBy = t1.ShippedBy,
                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,
                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,
                                                                 Common_SupplierFK = t1.SupplierId,
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());

            if (vStatus != -1 && vStatus != null)
            {
                vmPurchaseOrder.DataList = vmPurchaseOrder.DataList.Where(q => q.Status == vStatus);
            }


            return vmPurchaseOrder;
        }
        public async Task<VMPurchaseOrder> KFMALProcurementPurchaseOrderListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;

            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders
                                                             .Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId &&
                                                             x.PurchaseDate >= fromDate && x.PurchaseDate <= toDate
                                                             && !x.IsCancel && !x.IsHold)
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                             join t3 in _db.LCInfoes on t1.LCId equals t3.LCId into lc
                                                             from t3 in lc.DefaultIfEmpty()


                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,
                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 CompanyFK = t1.CompanyId,
                                                                 CountryId = t1.CountryId,
                                                                 FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                                                 OtherCharge = t1.OtherCharge,
                                                                 FreightCharge = t1.FreightCharge,
                                                                 PINo = t1.PINo,
                                                                 PortOfDischarge = t1.PortOfDischarge,
                                                                 PortOfLoading = t1.PortOfLoading,
                                                                 ShippedBy = t1.ShippedBy,
                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,
                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,
                                                                 Common_SupplierFK = t1.SupplierId,
                                                                 LCId = t3 != null ? (int?)t3.LCId : 0
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());

            if (vStatus != -1 && vStatus != null)
            {
                vmPurchaseOrder.DataList = vmPurchaseOrder.DataList.Where(q => q.Status == vStatus);
            }


            return vmPurchaseOrder;
        }

        public async Task<VMPurchaseOrder> PackagingPurchaseOrderListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;

            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders
                                                             .Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId &&
                                                             x.PurchaseDate >= fromDate && x.PurchaseDate <= toDate
                                                             &&
                                                             !x.IsCancel && !x.IsHold)
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                             join t3 in _db.LCInfoes on t1.LCId equals t3.LCId into t3_Join
                                                             from t3 in t3_Join.DefaultIfEmpty()


                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,
                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 CompanyFK = t1.CompanyId,
                                                                 CountryId = t1.CountryId,
                                                                 FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                                                 OtherCharge = t1.OtherCharge,
                                                                 FreightCharge = t1.FreightCharge,
                                                                 PINo = t1.PINo,
                                                                 PortOfDischarge = t1.PortOfDischarge,
                                                                 PortOfLoading = t1.PortOfLoading,
                                                                 ShippedBy = t1.ShippedBy,
                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,
                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,
                                                                 Common_SupplierFK = t1.SupplierId,
                                                                 TDSDeduction = t1.TDSDeduction,
                                                                 TotalDiscount = t1.TotalDiscount,
                                                                 LCId = t3 != null ? (int?)t3.LCId : 0
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());

            if (vStatus != -1 && vStatus != null)
            {
                vmPurchaseOrder.DataList = vmPurchaseOrder.DataList.Where(q => q.Status == vStatus);
            }


            return vmPurchaseOrder;
        }



        public async Task<VMPurchaseOrder> GetSingleProcurementPurchaseOrder(int id)
        {

            var v = await Task.Run(() => (from t1 in _db.PurchaseOrders
                                          join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                          where t1.PurchaseOrderId == id
                                          select new VMPurchaseOrder
                                          {
                                              PurchaseOrderId = t1.PurchaseOrderId,
                                              SupplierName = t2.Name,

                                              CID = t1.PurchaseOrderNo,
                                              OrderDate = t1.PurchaseDate,
                                              Description = t1.Remarks,
                                              IsHold = t1.IsHold,
                                              IsCancel = t1.IsCancel,
                                              Status = t1.Status,
                                              CompanyFK = t1.CompanyId,

                                              CountryId = t1.CountryId,
                                              FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                              OtherCharge = t1.OtherCharge,
                                              FreightCharge = t1.FreightCharge,
                                              PINo = t1.PINo,
                                              PortOfDischarge = t1.PortOfDischarge,
                                              PortOfLoading = t1.PortOfLoading,
                                              ShippedBy = t1.ShippedBy,

                                              PremiumValue = t1.PremiumValue,
                                              LCValue = t1.LCValue,
                                              LCNo = t1.LCNo,
                                              InsuranceNo = t1.InsuranceNo,
                                              SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                              TermsAndCondition = t1.TermsAndCondition,

                                              DeliveryAddress = t1.DeliveryAddress,
                                              DeliveryDate = t1.DeliveryDate,

                                              Common_SupplierFK = t1.SupplierId
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<VMPurchaseOrder> PackagingGetSinglePurchaseOrder(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.PurchaseOrders
                                          join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                          where t1.PurchaseOrderId == id
                                          select new VMPurchaseOrder
                                          {
                                              PurchaseOrderId = t1.PurchaseOrderId,
                                              SupplierName = t2.Name,

                                              CID = t1.PurchaseOrderNo,
                                              OrderDate = t1.PurchaseDate,
                                              Description = t1.Remarks,
                                              IsHold = t1.IsHold,
                                              IsCancel = t1.IsCancel,
                                              Status = t1.Status,
                                              CompanyFK = t1.CompanyId,

                                              CountryId = t1.CountryId,
                                              FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                              OtherCharge = t1.OtherCharge,
                                              FreightCharge = t1.FreightCharge,
                                              PINo = t1.PINo,
                                              PortOfDischarge = t1.PortOfDischarge,
                                              PortOfLoading = t1.PortOfLoading,
                                              ShippedBy = t1.ShippedBy,

                                              PremiumValue = t1.PremiumValue,
                                              LCValue = t1.LCValue,
                                              LCNo = t1.LCNo,
                                              LCId = (int?)t1.LCId,
                                              InsuranceNo = t1.InsuranceNo,
                                              SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                              TermsAndCondition = t1.TermsAndCondition,

                                              DeliveryAddress = t1.DeliveryAddress,
                                              DeliveryDate = t1.DeliveryDate,

                                              Common_SupplierFK = t1.SupplierId
                                          }).FirstOrDefault());

            return v;
        }


        public async Task<IssueDetailInfoVM> PromotionalItemInvoiceSingleItem(int id)
        {
            var v = await (from t1 in _db.IssueMasterInfoes
                           join t2 in _db.Vendors on t1.VendorId equals t2.VendorId
                           where t1.IssueMasterId == id
                           select new IssueDetailInfoVM
                           {
                               IssueNo = t1.IssueNo,
                               IsSubmit = t1.IsSubmit
                           }).FirstOrDefaultAsync();

            return v;
        }
        public async Task<VMPurchaseOrder> KFMALGetSingleProcurementPurchaseOrder(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.PurchaseOrders
                                          join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                          where t1.PurchaseOrderId == id
                                          select new VMPurchaseOrder
                                          {
                                              PurchaseOrderId = t1.PurchaseOrderId,
                                              SupplierName = t2.Name,

                                              CID = t1.PurchaseOrderNo,
                                              OrderDate = t1.PurchaseDate,
                                              Description = t1.Remarks,
                                              IsHold = t1.IsHold,
                                              IsCancel = t1.IsCancel,
                                              Status = t1.Status,
                                              CompanyFK = t1.CompanyId,

                                              CountryId = t1.CountryId,
                                              FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                              OtherCharge = t1.OtherCharge,
                                              FreightCharge = t1.FreightCharge,
                                              PINo = t1.PINo,
                                              PortOfDischarge = t1.PortOfDischarge,
                                              PortOfLoading = t1.PortOfLoading,
                                              ShippedBy = t1.ShippedBy,

                                              PremiumValue = t1.PremiumValue,
                                              LCValue = t1.LCValue,
                                              LCNo = t1.LCNo,
                                              LCId = (int?)t1.LCId,
                                              InsuranceNo = t1.InsuranceNo,
                                              SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                              TermsAndCondition = t1.TermsAndCondition,

                                              DeliveryAddress = t1.DeliveryAddress,
                                              DeliveryDate = t1.DeliveryDate,
                                              TDSDeduction = t1.TDSDeduction,
                                              TotalDiscount = t1.TotalDiscount,

                                              Common_SupplierFK = t1.SupplierId
                                          }).FirstOrDefault());

            return v;
        }
        public async Task<long> ProcurementPurchaseOrderAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                long result = -1;
                string poCid = "";
                if (vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK == (int)VendorsPaymentMethodEnum.LC)
                {
                    if (vmPurchaseOrderSlave.OrderNo != null)
                    {
                        poCid = vmPurchaseOrderSlave.OrderNo;
                    }
                    else
                    {
                        var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                        poCid = @"PO-" +
                                       DateTime.Now.ToString("yy") +
                                       DateTime.Now.ToString("MM") +
                                       DateTime.Now.ToString("dd") + "-" +

                                        poMax.ToString().PadLeft(2, '0');
                    }
                }
                else
                {
                    var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                    poCid = @"PO-" +
                                   DateTime.Now.ToString("yy") +
                                   DateTime.Now.ToString("MM") +
                                   DateTime.Now.ToString("dd") + "-" +

                                    poMax.ToString().PadLeft(2, '0');
                }
                try
                {


                    PurchaseOrder Procurement_PurchaseOrder = new PurchaseOrder
                    {

                        PurchaseOrderNo = poCid,
                        PurchaseDate = vmPurchaseOrderSlave.OrderDate,
                        SupplierId = vmPurchaseOrderSlave.Common_SupplierFK,
                        DeliveryDate = vmPurchaseOrderSlave.DeliveryDate,
                        SupplierPaymentMethodEnumFK = vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK,
                        DeliveryAddress = vmPurchaseOrderSlave.DeliveryAddress,
                        Remarks = vmPurchaseOrderSlave.Remarks,
                        Status = (int)EnumPOStatus.Draft,
                        PurchaseOrderStatus = EnumPOStatus.Draft.ToString(),
                        EmpId = vmPurchaseOrderSlave.EmployeeId,
                        CountryId = vmPurchaseOrderSlave.CountryId,
                        PINo = vmPurchaseOrderSlave.PINo,
                        LCHeadGLId = vmPurchaseOrderSlave.LCHeadGLId,
                        LCNo = vmPurchaseOrderSlave.LCNo,
                        LCValue = vmPurchaseOrderSlave.LCValue,
                        InsuranceNo = vmPurchaseOrderSlave.InsuranceNo,
                        PremiumValue = vmPurchaseOrderSlave.PremiumValue,
                        ShippedBy = vmPurchaseOrderSlave.ShippedBy,
                        PortOfLoading = vmPurchaseOrderSlave.PortOfLoading,
                        FinalDestinationCountryFk = vmPurchaseOrderSlave.FinalDestinationCountryFk,
                        PortOfDischarge = vmPurchaseOrderSlave.PortOfDischarge,
                        FreightCharge = vmPurchaseOrderSlave.FreightCharge,
                        OtherCharge = vmPurchaseOrderSlave.OtherCharge,
                        LCId = vmPurchaseOrderSlave.LCId,
                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    _db.PurchaseOrders.Add(Procurement_PurchaseOrder);
                    int res = _db.SaveChanges();
                    if (res > 0)
                    {
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                    }

                    PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
                    {
                        PurchaseOrderId = Procurement_PurchaseOrder.PurchaseOrderId,
                        ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                        PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                        PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                        PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),

                        DemandRate = 0,
                        QCRate = 0,
                        PackSize = 0,

                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);

                    res += _db.SaveChanges();

                    if (res > 1)
                    {

                        scope.Commit();
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                        return result;
                    }
                    else
                    {
                        scope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;

                }

                return result;



            }

        }
        public async Task<long> PackagingPurchaseOrderAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {

            using (var scope = _db.Database.BeginTransaction())
            {
                long result = -1;
                string poCid = "";


                if (vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK == (int)VendorsPaymentMethodEnum.LC)
                {
                    if (vmPurchaseOrderSlave.OrderNo != null)
                    {
                        poCid = vmPurchaseOrderSlave.OrderNo;
                    }
                    else
                    {
                        var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                        poCid = @"PO-" +
                                       DateTime.Now.ToString("yy") +
                                       DateTime.Now.ToString("MM") +
                                       DateTime.Now.ToString("dd") + "-" +

                                        poMax.ToString().PadLeft(2, '0');
                    }
                }
                else
                {
                    var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                    poCid = @"PO-" +
                                   DateTime.Now.ToString("yy") +
                                   DateTime.Now.ToString("MM") +
                                   DateTime.Now.ToString("dd") + "-" +

                                    poMax.ToString().PadLeft(2, '0');
                }
                try
                {


                    PurchaseOrder Procurement_PurchaseOrder = new PurchaseOrder
                    {

                        PurchaseOrderNo = poCid,
                        PurchaseDate = vmPurchaseOrderSlave.OrderDate,
                        SupplierId = vmPurchaseOrderSlave.Common_SupplierFK,
                        DeliveryDate = vmPurchaseOrderSlave.DeliveryDate,
                        SupplierPaymentMethodEnumFK = vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK,
                        DeliveryAddress = vmPurchaseOrderSlave.DeliveryAddress,
                        Remarks = vmPurchaseOrderSlave.Remarks,
                        Status = (int)EnumPOStatus.Draft,
                        PurchaseOrderStatus = EnumPOStatus.Draft.ToString(),
                        EmpId = vmPurchaseOrderSlave.EmployeeId,
                        CountryId = vmPurchaseOrderSlave.CountryId,
                        PINo = vmPurchaseOrderSlave.PINo,
                        LCHeadGLId = vmPurchaseOrderSlave.LCHeadGLId,
                        LCNo = vmPurchaseOrderSlave.LCNo,
                        LCValue = vmPurchaseOrderSlave.LCValue,
                        InsuranceNo = vmPurchaseOrderSlave.InsuranceNo,
                        PremiumValue = vmPurchaseOrderSlave.PremiumValue,
                        ShippedBy = vmPurchaseOrderSlave.ShippedBy,
                        PortOfLoading = vmPurchaseOrderSlave.PortOfLoading,
                        FinalDestinationCountryFk = vmPurchaseOrderSlave.FinalDestinationCountryFk,
                        PortOfDischarge = vmPurchaseOrderSlave.PortOfDischarge,
                        FreightCharge = vmPurchaseOrderSlave.FreightCharge,
                        OtherCharge = vmPurchaseOrderSlave.OtherCharge,
                        TermsAndCondition = vmPurchaseOrderSlave.TermsAndCondition,
                        LCId = vmPurchaseOrderSlave.LCId,
                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        TDSDeduction = vmPurchaseOrderSlave.TDSDeduction,
                        TotalDiscount = vmPurchaseOrderSlave.TotalDiscount,
                        IsActive = true
                    };

                    _db.PurchaseOrders.Add(Procurement_PurchaseOrder);
                    int res = _db.SaveChanges();
                    if (res > 0)
                    {
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                    }

                    PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
                    {
                        PurchaseOrderId = Procurement_PurchaseOrder.PurchaseOrderId,
                        ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                        PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                        PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                        PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),

                        DemandRate = 0,
                        QCRate = 0,
                        PackSize = 0,

                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        VATAddition = 0, //vmPurchaseOrderSlave.VATAddition,
                        IsActive = true,
                        IsVATIncluded = vmPurchaseOrderSlave.IsVATIncluded
                    };
                    _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);

                    res += _db.SaveChanges();

                    if (res > 1)
                    {

                        scope.Commit();
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                        return result;
                    }
                    else
                    {
                        scope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;

                }

                return result;



            }

        }

        public async Task<long> PromotionalItemInvoiceAdd(IssueDetailInfoVM issueDetailInfoVM)
        {

            using (var scope = _db.Database.BeginTransaction())
            {
                long result = -1;
                string poCid = "";


                var poMax = _db.IssueMasterInfoes.Count(x => x.CompanyId == issueDetailInfoVM.CompanyFK) + 1;
                poCid = $"PI-{DateTime.Now:yyMMdd}-{poMax:D2}";

                try
                {


                    IssueMasterInfo issueMasterInfo = new IssueMasterInfo
                    {

                        IssueNo = poCid,
                        IssueDate = issueDetailInfoVM.IssueDate,
                        IssuedBy = issueDetailInfoVM.IssuedBy,
                        VendorId = issueDetailInfoVM.VendorId,
                        CompanyId = (int)issueDetailInfoVM.CompanyFK,
                        Remarks = issueDetailInfoVM.Remarks,
                        IsActive = true,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        Achknolagement = false,
                        AchknologeBy = 0
                    };

                    _db.IssueMasterInfoes.Add(issueMasterInfo);
                    int res = await _db.SaveChangesAsync();
                    if (res > 0)
                    {
                        IssueDetailInfo issueDetailInfo = new IssueDetailInfo
                        {
                            IssueMasterId = issueMasterInfo.IssueMasterId,
                            RProductId = issueDetailInfoVM.RProductId,
                            RMQ = issueDetailInfoVM.RMQ,
                            CostingPrice = issueDetailInfoVM.CostingPrice,
                            IsActive = true,

                        };
                        _db.IssueDetailInfoes.Add(issueDetailInfo);

                        res += await _db.SaveChangesAsync();
                    }
                    if (res > 1)
                    {

                        scope.Commit();
                        result = issueMasterInfo.IssueMasterId;
                        return result;
                    }
                    else
                    {
                        scope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;

                }
                return result;
            }

        }
        public async Task<long> KFMALProcurementPurchaseOrderAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {

            using (var scope = _db.Database.BeginTransaction())
            {
                long result = -1;
                string poCid = "";


                if (vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK == (int)VendorsPaymentMethodEnum.LC)
                {
                    if (vmPurchaseOrderSlave.OrderNo != null)
                    {
                        poCid = vmPurchaseOrderSlave.OrderNo;
                    }
                    else
                    {
                        var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                        poCid = @"PO-" +
                                       DateTime.Now.ToString("yy") +
                                       DateTime.Now.ToString("MM") +
                                       DateTime.Now.ToString("dd") + "-" +

                                        poMax.ToString().PadLeft(2, '0');
                    }
                }
                else
                {
                    var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                    poCid = @"PO-" +
                                   DateTime.Now.ToString("yy") +
                                   DateTime.Now.ToString("MM") +
                                   DateTime.Now.ToString("dd") + "-" +

                                    poMax.ToString().PadLeft(2, '0');
                }
                try
                {


                    PurchaseOrder Procurement_PurchaseOrder = new PurchaseOrder
                    {

                        PurchaseOrderNo = poCid,
                        PurchaseDate = vmPurchaseOrderSlave.OrderDate,
                        SupplierId = vmPurchaseOrderSlave.Common_SupplierFK,
                        DeliveryDate = vmPurchaseOrderSlave.DeliveryDate,
                        SupplierPaymentMethodEnumFK = vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK,
                        DeliveryAddress = vmPurchaseOrderSlave.DeliveryAddress,
                        Remarks = vmPurchaseOrderSlave.Remarks,
                        Status = (int)EnumPOStatus.Draft,
                        PurchaseOrderStatus = EnumPOStatus.Draft.ToString(),
                        EmpId = vmPurchaseOrderSlave.EmployeeId,
                        CountryId = vmPurchaseOrderSlave.CountryId,
                        PINo = vmPurchaseOrderSlave.PINo,
                        LCHeadGLId = vmPurchaseOrderSlave.LCHeadGLId,
                        LCNo = vmPurchaseOrderSlave.LCNo,
                        LCValue = vmPurchaseOrderSlave.LCValue,
                        InsuranceNo = vmPurchaseOrderSlave.InsuranceNo,
                        PremiumValue = vmPurchaseOrderSlave.PremiumValue,
                        ShippedBy = vmPurchaseOrderSlave.ShippedBy,
                        PortOfLoading = vmPurchaseOrderSlave.PortOfLoading,
                        FinalDestinationCountryFk = vmPurchaseOrderSlave.FinalDestinationCountryFk,
                        PortOfDischarge = vmPurchaseOrderSlave.PortOfDischarge,
                        FreightCharge = vmPurchaseOrderSlave.FreightCharge,
                        OtherCharge = vmPurchaseOrderSlave.OtherCharge,
                        LCId = vmPurchaseOrderSlave.LCId,
                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    _db.PurchaseOrders.Add(Procurement_PurchaseOrder);
                    int res = _db.SaveChanges();
                    if (res > 0)
                    {
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                    }

                    PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
                    {
                        PurchaseOrderId = Procurement_PurchaseOrder.PurchaseOrderId,
                        ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                        PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                        PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                        PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),

                        DemandRate = 0,
                        QCRate = 0,
                        PackSize = 0,

                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);

                    res += _db.SaveChanges();

                    if (res > 1)
                    {

                        scope.Commit();
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                        return result;
                    }
                    else
                    {
                        scope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;

                }

                return result;



            }

        }
        public async Task<int> PromtionalOfferAdd(VMPromtionalOfferDetail vmPromtionalOfferDetail)
        {
            int result = -1;

            PromtionalOffer promtionalOffer = new PromtionalOffer
            {

                FromDate = vmPromtionalOfferDetail.FromDate,
                ToDate = vmPromtionalOfferDetail.ToDate,
                PromtionType = vmPromtionalOfferDetail.PromtionType,
                PromoCode = vmPromtionalOfferDetail.PromoCode,
                CompanyId = vmPromtionalOfferDetail.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PromtionalOffers.Add(promtionalOffer);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = promtionalOffer.PromtionalOfferId;
            }
            return result;
        }

        public async Task<int> PromtionalOfferDetailAdd(VMPromtionalOfferDetail vmPromtionalOfferDetail)
        {
            int result = -1;

            PromtionalOfferDetail promtionalOfferDetail = new PromtionalOfferDetail
            {

                ProductId = vmPromtionalOfferDetail.ProductId,
                PromtionalOfferId = vmPromtionalOfferDetail.PromtionalOfferId,
                PromoAmount = vmPromtionalOfferDetail.PromoAmount,
                PromoQuantity = vmPromtionalOfferDetail.PromoQuantity,

                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PromtionalOfferDetails.Add(promtionalOfferDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = promtionalOfferDetail.PromtionalOfferDetailId;
            }
            return result;
        }

        public async Task<int> PromtionalOfferDetailUpdate(VMPromtionalOfferDetail vmPromtionalOfferDetail)
        {
            int result = -1;
            PromtionalOfferDetail promtionalOfferDetail = await _db.PromtionalOfferDetails.FindAsync(vmPromtionalOfferDetail.PromtionalOfferDetailId);

            promtionalOfferDetail.ProductId = vmPromtionalOfferDetail.ProductId;
            promtionalOfferDetail.PromtionalOfferId = vmPromtionalOfferDetail.PromtionalOfferId;
            promtionalOfferDetail.PromoAmount = vmPromtionalOfferDetail.PromoAmount;
            promtionalOfferDetail.PromoQuantity = vmPromtionalOfferDetail.PromoQuantity;
            promtionalOfferDetail.CreatedBy = Common.GetUserId();
            promtionalOfferDetail.CreatedDate = DateTime.Now;

            _db.Entry(promtionalOfferDetail).State = EntityState.Modified;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = promtionalOfferDetail.PromtionalOfferDetailId;
            }
            return result;
        }

        public async Task<long> ProcurementPurchaseOrderOpeningAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK && x.IsOpening).Count() + 1;
            string poCid = @"Opening-" +
                            DateTime.Now.ToString("yy") +
                            DateTime.Now.ToString("MM") +
                            DateTime.Now.ToString("dd") + "-" +

                             poMax.ToString().PadLeft(2, '0');
            PurchaseOrder Procurement_PurchaseOrder = new PurchaseOrder
            {
                IsOpening = true,
                PurchaseOrderNo = poCid,
                PurchaseDate = vmPurchaseOrderSlave.OrderDate,
                SupplierId = vmPurchaseOrderSlave.Common_SupplierFK,
                DeliveryDate = DateTime.Now,
                SupplierPaymentMethodEnumFK = 1,
                DeliveryAddress = "",
                Remarks = vmPurchaseOrderSlave.Description,
                TermsAndCondition = "",
                Status = (int)EnumPOStatus.Submitted,
                PurchaseOrderStatus = EnumPOStatus.Submitted.ToString(),

                CountryId = 1,
                PINo = "",
                LCNo = "",
                LCValue = 0,
                InsuranceNo = "",
                PremiumValue = 0,
                ShippedBy = "",
                PortOfLoading = "",
                FinalDestinationCountryFk = 1,
                PortOfDischarge = "",
                FreightCharge = 0,
                OtherCharge = 0,

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = vmPurchaseOrderSlave.OrderDate.Value,
                IsActive = true
            };
            _db.PurchaseOrders.Add(Procurement_PurchaseOrder);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = Procurement_PurchaseOrder.PurchaseOrderId;
            }
            return result;
        }

        public async Task<long> ProcurementPurchaseOrderEdit(VMPurchaseOrder vmPurchaseOrder)
        {
            long result = -1;
            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(vmPurchaseOrder.PurchaseOrderId);

            procurementPurchaseOrder.PurchaseDate = vmPurchaseOrder.OrderDate;
            procurementPurchaseOrder.SupplierId = vmPurchaseOrder.Common_SupplierFK;
            procurementPurchaseOrder.DeliveryDate = vmPurchaseOrder.DeliveryDate;
            procurementPurchaseOrder.SupplierPaymentMethodEnumFK = vmPurchaseOrder.SupplierPaymentMethodEnumFK;
            procurementPurchaseOrder.DeliveryAddress = vmPurchaseOrder.DeliveryAddress;
            procurementPurchaseOrder.Remarks = vmPurchaseOrder.Remarks;
            procurementPurchaseOrder.TermsAndCondition = vmPurchaseOrder.TermsAndCondition;
            procurementPurchaseOrder.Remarks = vmPurchaseOrder.Description;
            procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            procurementPurchaseOrder.ModifiedDate = DateTime.Now;

            procurementPurchaseOrder.CountryId = vmPurchaseOrder.CountryId;
            procurementPurchaseOrder.PINo = vmPurchaseOrder.PINo;
            procurementPurchaseOrder.LCNo = vmPurchaseOrder.LCNo;
            procurementPurchaseOrder.LCValue = vmPurchaseOrder.LCValue;
            procurementPurchaseOrder.InsuranceNo = vmPurchaseOrder.InsuranceNo;
            procurementPurchaseOrder.PremiumValue = vmPurchaseOrder.PremiumValue;
            procurementPurchaseOrder.ShippedBy = vmPurchaseOrder.ShippedBy;
            procurementPurchaseOrder.PortOfLoading = vmPurchaseOrder.PortOfLoading;
            procurementPurchaseOrder.FinalDestinationCountryFk = vmPurchaseOrder.FinalDestinationCountryFk;
            procurementPurchaseOrder.PortOfDischarge = vmPurchaseOrder.PortOfDischarge;
            procurementPurchaseOrder.FreightCharge = vmPurchaseOrder.FreightCharge;
            procurementPurchaseOrder.OtherCharge = vmPurchaseOrder.OtherCharge;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmPurchaseOrder.ID;
            }

            return result;
        }

        public async Task<long> PackagingPurchaseOrderSubmit(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;


            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(vmPurchaseOrderSlave.PurchaseOrderId);
            procurementPurchaseOrder.TotalDiscount = vmPurchaseOrderSlave.TotalDiscount;
            await _db.SaveChangesAsync();

            if (procurementPurchaseOrder != null)
            {
                if (procurementPurchaseOrder.Status == (int)EnumPOStatus.Draft)
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Submitted;

                    List<PurchaseOrderDetail> ListDetail = _db.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == procurementPurchaseOrder.PurchaseOrderId
                     && x.IsActive).ToList();
                    decimal totalValue = ListDetail.Select(x => x.PurchaseQty * x.PurchaseRate).DefaultIfEmpty(0).Sum();

                    ListDetail.ForEach(x => x.ProductDiscount = (procurementPurchaseOrder.TotalDiscount / totalValue) * (x.PurchaseQty * x.PurchaseRate));
                    //await _db.SaveChangesAsync();
                }
                else
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Draft;

                }
                procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }

        public async Task<long> PromotionalItemInvoiceSubmit(IssueDetailInfoVM issueDetailInfoVM)
        {
            long result = -1;


            IssueMasterInfo issueMasterInfo = await _db.IssueMasterInfoes.FindAsync(issueDetailInfoVM.IssueMasterId);

            if (issueMasterInfo != null)
            {

                issueMasterInfo.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
                issueMasterInfo.ModifiedDate = DateTime.Now;
                issueMasterInfo.IsSubmit = true;

                IssueDetailInfoVM AccData = await PromotionalItemInvoiceGet(issueDetailInfoVM.CompanyFK.Value, (int)issueDetailInfoVM.IssueMasterId);
                var vResult = await _accountingService.AccountingPromotionalPushISS(AccData);
                if (vResult > 0)
                {
                    await _db.SaveChangesAsync();
                    result = issueMasterInfo.IssueMasterId;
                }
            }
            return result;
        }

        public async Task<long> ProcurementPurchaseOrderSubmit(long? id = 0)
        {
            long result = -1;
            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (procurementPurchaseOrder != null)
            {
                if (procurementPurchaseOrder.Status == (int)EnumPOStatus.Draft)
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Submitted;
                }
                else
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Draft;

                }
                procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }

        public async Task<long> KFMALProcurementPurchaseOrderSubmit(long? id = 0)
        {
            long result = -1;
            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (procurementPurchaseOrder != null)
            {
                if (procurementPurchaseOrder.Status == (int)EnumPOStatus.Draft)
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Submitted;
                }
                else
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Draft;

                }
                procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }

        public async Task<long> ProcurementPurchaseOrderDelete(long id)
        {
            long result = -1;
            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (procurementPurchaseOrder != null)
            {
                procurementPurchaseOrder.IsActive = false;
                procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrder.PurchaseOrderId;
                }
            }

            return result;
        }

        public async Task<long> ProcurementPurchaseOrderDeleteProcess(long id)
        {
            long result = -1;
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
                    procurementPurchaseOrder.IsActive = false;
                    procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                    _db.SaveChanges();


                    //List<PurchaseOrderDetail> purchaseOrderDetails = new List<PurchaseOrderDetail>(); 
                    var purchesorderLis = await _db.PurchaseOrderDetails.Where(f => f.PurchaseOrderId == procurementPurchaseOrder.PurchaseOrderId).ToListAsync();
                    foreach (var item in purchesorderLis)
                    {
                        //PurchaseOrderDetail purchaseOrderDetail = new PurchaseOrderDetail();
                        item.IsActive = false;
                        _db.SaveChanges();
                        //purchaseOrderDetails.Add(item);
                    }
                    //_db.PurchaseOrderDetails.RemoveRange(purchesorderLis);
                    //_db.PurchaseOrderDetails.AddRange(purchaseOrderDetails);

                    var materialReceive = await _db.MaterialReceives.FirstOrDefaultAsync(f => f.PurchaseOrderId == procurementPurchaseOrder.PurchaseOrderId);
                    materialReceive.IsActive = false;
                    materialReceive.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    materialReceive.ModifiedDate = DateTime.Now;
                    _db.SaveChanges();


                    var materialReceivedetalis = await _db.MaterialReceiveDetails.Where(f => f.MaterialReceiveId == materialReceive.MaterialReceiveId).ToListAsync();
                    foreach (var item in materialReceivedetalis)
                    {
                        item.IsActive = false;
                        _db.SaveChanges();
                    }


                    var Vmap = await _db.VoucherMaps.Where(d => d.IntegratedId == materialReceive.MaterialReceiveId && d.IntegratedFrom == "MaterialReceive").FirstOrDefaultAsync();
                    var voucher = _db.Vouchers.Where(f => f.VoucherId == Vmap.VoucherId).FirstOrDefault();
                    voucher.IsActive = false;
                    voucher.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    voucher.ModifiedDate = DateTime.Now;
                    _db.SaveChanges();

                    var voucherdetalis = await _db.VoucherDetails.Where(f => f.VoucherId == voucher.VoucherId).ToListAsync();
                    foreach (var item in voucherdetalis)
                    {
                        item.IsActive = false;
                        _db.SaveChanges();
                    }


                    scope.Commit();
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                }
            }
            return result;
        }


        public async Task<long> OrderMastersDelete(long id)
        {
            long result = -1;
            OrderMaster orderMasters = await _db.OrderMasters.FindAsync(id);
            if (orderMasters != null)
            {
                orderMasters.IsActive = false;
                orderMasters.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                orderMasters.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = orderMasters.OrderMasterId;
                }
            }

            return result;
        }

        public async Task<long> PromotionalItemMastersDelete(long id)
        {
            long result = -1;
            IssueMasterInfo issueMasterInfo = await _db.IssueMasterInfoes.FindAsync(id);
            if (issueMasterInfo != null)
            {
                issueMasterInfo.IsActive = false;
                issueMasterInfo.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
                issueMasterInfo.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = issueMasterInfo.IssueMasterId;
                }
            }

            return result;
        }
        public async Task<long> UpdatePromotionalItemMasters(IssueDetailInfoVM issueDetailInfoVM)
        {
            long result = -1;
            IssueMasterInfo issueMasterInfo = await _db.IssueMasterInfoes.FindAsync(issueDetailInfoVM.IssueMasterId);
            if (issueMasterInfo != null)
            {
                issueMasterInfo.IssueDate = issueDetailInfoVM.IssueDate;
                issueMasterInfo.IssuedBy = issueDetailInfoVM.IssuedBy;
                issueMasterInfo.VendorId = issueDetailInfoVM.VendorId;
                issueMasterInfo.Remarks = issueDetailInfoVM.Remarks;
                issueMasterInfo.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
                issueMasterInfo.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = issueMasterInfo.IssueMasterId;
                }
            }

            return result;
        }
        public async Task<long> ProcurementPurchaseOrderHoldUnHold(long id)
        {
            long result = -1;
            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (procurementPurchaseOrder != null)
            {
                if (procurementPurchaseOrder.IsHold)
                {
                    procurementPurchaseOrder.IsHold = false;
                }
                else
                {
                    procurementPurchaseOrder.IsHold = true;
                }
                procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }





        public async Task<long> ProcurementPurchaseOrderCancelRenew(long id)
        {
            long result = -1;
            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (procurementPurchaseOrder != null)
            {
                if (procurementPurchaseOrder.IsCancel)
                {
                    procurementPurchaseOrder.IsCancel = false;
                }
                else
                {
                    procurementPurchaseOrder.IsCancel = true;
                }
                procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }

        public async Task<VMFinishProductBOM> GetFinishProductBOM(int id)
        {

            var v = await Task.Run(() => (from t1 in _db.FinishProductBOMs.Where(x => x.ID == id)
                                          join t2 in _db.Products on t1.RProductFK equals t2.ProductId
                                          join t5 in _db.ProductSubCategories.Where(x => x.IsActive) on t2.ProductSubCategoryId equals t5.ProductSubCategoryId
                                          join t3 in _db.Units on t2.UnitId equals t3.UnitId
                                          select new VMFinishProductBOM
                                          {


                                              ID = t1.ID,
                                              CompanyFK = t1.CompanyId,
                                              RawProductName = t5.Name + "" + t2.ProductName,
                                              RProductFK = t1.RProductFK,
                                              RawConsumeQuantity = t1.Consumption,
                                              RequiredQuantity = t1.RequiredQuantity,
                                              SupplierId = t1.ID,
                                              UnitPrice = t1.UnitPrice,
                                              OrderDetailId = t1.OrderDetailId.Value,
                                              UnitName = t3.Name

                                          }).FirstOrDefault());
            return v;
        }



        public async Task<long> FinishProductBOMDelete(int id)
        {
            long result = -1;

            FinishProductBOM procurementOrderSlaveBOM = await _db.FinishProductBOMs.FindAsync(id);
            if (procurementOrderSlaveBOM != null)
            {
                procurementOrderSlaveBOM.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementOrderSlaveBOM.OrderDetailId.Value;
                }
            }
            return result;
        }

        public async Task<int> FinishProductBOMDetailEdit(VMFinishProductBOM vmFinishProductBOM)
        {
            var result = -1;
            FinishProductBOM model = await _db.FinishProductBOMs.FindAsync(vmFinishProductBOM.ID);

            model.CompanyId = vmFinishProductBOM.CompanyFK.Value;
            model.OrderDetailId = vmFinishProductBOM.OrderDetailId;
            model.RProductFK = vmFinishProductBOM.RProductFK;
            model.Consumption = vmFinishProductBOM.RawConsumeQuantity;
            model.RequiredQuantity = vmFinishProductBOM.RequiredQuantity;
            model.SupplierId = vmFinishProductBOM.SupplierId;
            model.UnitPrice = vmFinishProductBOM.UnitPrice;
            model.IsActive = true;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString();
            model.ORDStyle = vmFinishProductBOM.ORDStyle;


            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmFinishProductBOM.ID;
            }

            return result;
        }

        public async Task<long> ProcurementPurchaseOrderClosedReopen(long id)
        {
            long result = -1;
            PurchaseOrder procurementPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (procurementPurchaseOrder != null)
            {
                if (procurementPurchaseOrder.Status == (int)EnumPOStatus.Closed)
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Draft;
                }
                else
                {
                    procurementPurchaseOrder.Status = (int)EnumPOStatus.Closed;
                }
                procurementPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                procurementPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }

        public List<VMPurchaseOrderSlave> PackagingPurchaseRawItemDataList(int styleNo, int SupplierFK)
        {
            VMPurchaseOrderSlave vmPurchaseOrder = new VMPurchaseOrderSlave();
            var list = (from t1 in _db.FinishProductBOMs.Where(x => x.IsActive && x.OrderDetailId == styleNo)
                        join t3 in _db.Products.Where(x => x.IsActive) on t1.RProductFK equals t3.ProductId
                        join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                        join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                        select new VMPurchaseOrderSlave
                        {
                            ProductName = t4.Name + "" + t3.ProductName,
                            RequisitionQuantity = t1.RequiredQuantity,
                            RequiredQuantity = (t1.RequiredQuantity -
                            (_db.RequisitionItemDetails.Where(s => s.FinishProductBOMId == t1.ID && s.IsActive).Select(s => s.RQty.Value).DefaultIfEmpty(0).Sum())),
                            RequestedQuantity = (_db.RequisitionItemDetails.Where(s => s.FinishProductBOMId == t1.ID && s.IsActive).Select(s => s.RQty.Value).DefaultIfEmpty(0).Sum()),
                            Consumption = t1.Consumption,

                            UnitName = t6.Name,
                            Common_ProductFK = t1.RProductFK,
                            FinishProductBOMId = t1.ID,

                        }).ToList();
            return list;

        }

        public async Task<VMPurchaseOrder> ProcurementApprovalPurchaseOrderListGet(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId)
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,

                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,

                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,
                                                                 Common_SupplierFK = t1.SupplierId
                                                             }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmPurchaseOrder;
        }
        public async Task<VMPurchaseOrder> ProcurementCancelPurchaseOrderListGet(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;
            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId && x.IsCancel)
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,

                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 CompanyFK = t1.CompanyId,

                                                                 CountryId = t1.CountryId,
                                                                 FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                                                 OtherCharge = t1.OtherCharge,
                                                                 FreightCharge = t1.FreightCharge,
                                                                 PINo = t1.PINo,
                                                                 PortOfDischarge = t1.PortOfDischarge,
                                                                 PortOfLoading = t1.PortOfLoading,
                                                                 ShippedBy = t1.ShippedBy,


                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,

                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,

                                                                 Common_SupplierFK = t1.SupplierId,
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());
            return vmPurchaseOrder;
        }
        public async Task<VMPurchaseOrder> ProcurementHoldPurchaseOrderListGet(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;
            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId && x.IsHold)
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,

                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 CompanyFK = t1.CompanyId,

                                                                 CountryId = t1.CountryId,
                                                                 FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                                                 OtherCharge = t1.OtherCharge,
                                                                 FreightCharge = t1.FreightCharge,
                                                                 PINo = t1.PINo,
                                                                 PortOfDischarge = t1.PortOfDischarge,
                                                                 PortOfLoading = t1.PortOfLoading,
                                                                 ShippedBy = t1.ShippedBy,


                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,

                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,

                                                                 Common_SupplierFK = t1.SupplierId,
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());
            return vmPurchaseOrder;
        }
        public async Task<VMPurchaseOrder> ProcurementClosedPurchaseOrderListGet(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;
            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId && x.Status == (int)EnumPOStatus.Closed)
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,

                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 CompanyFK = t1.CompanyId,

                                                                 CountryId = t1.CountryId,
                                                                 FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                                                 OtherCharge = t1.OtherCharge,
                                                                 FreightCharge = t1.FreightCharge,
                                                                 PINo = t1.PINo,
                                                                 PortOfDischarge = t1.PortOfDischarge,
                                                                 PortOfLoading = t1.PortOfLoading,
                                                                 ShippedBy = t1.ShippedBy,


                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,

                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,

                                                                 Common_SupplierFK = t1.SupplierId,
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());
            return vmPurchaseOrder;
        }
        #endregion


        #region Purchase Order Detail
        public VMPOTremsAndConditions POTremsAndConditionsGet(int id)
        {
            var item = (from t1 in _db.POTremsAndConditions.Where(a => a.IsActive == true && a.ID == id)
                        select new VMPOTremsAndConditions
                        {
                            Value = t1.Value,
                            Key = t1.Key,
                            ID = t1.ID
                        }).FirstOrDefault();
            return item;
        }
        public decimal PurchaseOrderPayableValueGet(int companyId, int purchaseOrderId)
        {
            var SalesValue = (from t1 in _db.PurchaseOrderDetails.Where(a => a.IsActive == true && a.PurchaseOrderId == purchaseOrderId)
                              join t2 in _db.PurchaseOrders.Where(x => x.IsActive == true && x.CompanyId == companyId) on t1.PurchaseOrderId equals t2.PurchaseOrderId
                              select t1.PurchaseQty * t1.PurchaseRate).DefaultIfEmpty(0).Sum();
            var ReturnValue = (from t1 in _db.PurchaseReturnDetails
                               join t2 in _db.PurchaseReturns.Where(x => x.CompanyId == companyId) on t1.PurchaseReturnId equals t2.PurchaseReturnId
                               //join t3 in _db.MaterialReceives.Where(x => x.IsActive == true && x.CompanyId == companyId && x.PurchaseOrderId == purchaseOrderId) on t2.id equals t3.OrderDeliverId
                               select t1.Qty * t1.Rate).DefaultIfEmpty(0).Sum();
            var PaidValue = (from t1 in _db.Payments.Where(a => a.IsActive == true && a.PurchaseOrderId == purchaseOrderId)
                             select t1.OutAmount).DefaultIfEmpty(0).Sum();
            return SalesValue - ReturnValue.Value + PaidValue.Value;
        }
        public double OrderMasterPayableValueGet(int companyId, int orderMasterId)
        {
            var SalesValue = (from t0 in _db.OrderDeliverDetails
                              join t1 in _db.OrderDetails.Where(a => a.IsActive == true && a.OrderMasterId == orderMasterId) on t0.OrderDetailId equals t1.OrderDetailId
                              join t2 in _db.OrderMasters.Where(x => x.IsActive == true && x.CompanyId == companyId) on t1.OrderMasterId equals t2.OrderMasterId
                              select t0.DeliveredQty * (t1.UnitPrice - (double)t1.DiscountUnit)).DefaultIfEmpty(0).Sum();
            var ReturnValue = (from t1 in _db.SaleReturnDetails.Where(a => a.IsActive == true)
                               join t2 in _db.SaleReturns.Where(x => x.IsActive == true && x.CompanyId == companyId) on t1.SaleReturnId equals t2.SaleReturnId
                               join t3 in _db.OrderDelivers.Where(x => x.IsActive == true && x.CompanyId == companyId && x.OrderMasterId == orderMasterId) on t2.OrderDeliverId equals t3.OrderDeliverId
                               select t1.Qty * t1.Rate).DefaultIfEmpty(0).Sum();
            var PaidValue = (from t1 in _db.Payments.Where(a => a.IsActive == true && a.OrderMasterId == orderMasterId)
                             select t1.InAmount).DefaultIfEmpty(0).Sum();
            return SalesValue - Convert.ToDouble(ReturnValue.Value + PaidValue);
        }
        public async Task<VMPurchaseOrderSlave> ProcurementPurchaseOrderSlaveGet(int companyId, int purchaseOrderId)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();
            if (companyId <= 0 || purchaseOrderId <= 0)
            {
                return vmPurchaseOrderSlave;
            }
            vmPurchaseOrderSlave = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                         join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                                                         join t4 in _db.Employees on t1.EmpId equals t4.Id into t4_Join
                                                         from t4 in t4_Join.DefaultIfEmpty()
                                                         join t5 in _db.Designations on t4.DesignationId equals t5.DesignationId into t5_Join
                                                         from t5 in t5_Join.DefaultIfEmpty()
                                                         where t1.PurchaseOrderId == purchaseOrderId
                                                         select new VMPurchaseOrderSlave
                                                         {
                                                             PurchaseOrderId = t1.PurchaseOrderId,
                                                             SupplierName = t2.Name,
                                                             Code = t2.Code,
                                                             SupplierPropietor = t2.Propietor,
                                                             SupplierAddress = t2.Address,
                                                             SupplierMobile = t3.Phone,
                                                             EmployeeName = t4 != null ? t4.Name : "",
                                                             EmployeeMobile = t4 != null ? t4.MobileNo : "",
                                                             EmployeeDesignation = t5 != null ? t5.Name : "",
                                                             CID = t1.PurchaseOrderNo,
                                                             OrderDate = t1.PurchaseDate,
                                                             Description = t1.Remarks,
                                                             IsHold = t1.IsHold,
                                                             IsCancel = t1.IsCancel,
                                                             Status = t1.Status,
                                                             SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                             TermsAndCondition = t1.TermsAndCondition,
                                                             CompanyFK = t1.CompanyId,
                                                             DeliveryAddress = t1.DeliveryAddress,
                                                             DeliveryDate = t1.DeliveryDate,
                                                             Common_SupplierFK = t1.SupplierId,
                                                             FreightCharge = t1.FreightCharge,
                                                             OtherCharge = t1.OtherCharge,
                                                             CompanyName = t3.Name,
                                                             CompanyAddress = t3.Address,
                                                             CompanyEmail = t3.Email,
                                                             CompanyPhone = t3.Phone,
                                                             CreatedBy = t1.CreatedBy,
                                                             Companylogo = t3.CompanyLogo
                                                         }).FirstOrDefault());

            vmPurchaseOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.PurchaseOrderDetails.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                                       join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                       join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                       join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                       join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                       select new VMPurchaseOrderSlave
                                                                       {
                                                                           ProductName = t4.Name + " " + t3.ProductName,

                                                                           PurchaseOrderId = t1.PurchaseOrderId.Value,
                                                                           PurchaseOrderDetailId = t1.PurchaseOrderDetailId,
                                                                           PurchaseQuantity = t1.PurchaseQty,
                                                                           PurchasingPrice = t1.PurchaseRate,
                                                                           UnitName = t6.Name,
                                                                           PurchaseAmount = t1.PurchaseAmount,

                                                                           CompanyFK = t1.CompanyId,
                                                                           Common_ProductCategoryFK = t5.ProductCategoryId,
                                                                           Common_ProductSubCategoryFK = t4.ProductSubCategoryId,
                                                                           Common_ProductFK = t3.ProductId

                                                                       }).OrderByDescending(x => x.PurchaseOrderDetailId).AsEnumerable());



            //vmPurchaseOrderSlave.Currencyname = (from t1 in _db.PurchaseOrders
            //                                     join t2 in _db.LCInfoes on (long?)t1.LCId equals t2.LCId
            //                                     join t3 in _db.Currencies on t2.CurrenceyId equals t3.CurrencyId
            //                                     where t1.PurchaseOrderId == purchaseOrderId
            //                                     select t3.Name).FirstOrDefault();
            //vmPurchaseOrderSlave.currencysign = (from t1 in _db.PurchaseOrders
            //                                     join t2 in _db.LCInfoes on (long?)t1.LCId equals t2.LCId
            //                                     join t3 in _db.Currencies on t2.CurrenceyId equals t3.CurrencyId
            //                                     where t1.PurchaseOrderId == purchaseOrderId
            //                                     select t3.Sign).FirstOrDefault();
            //vmPurchaseOrderSlave.CurrencyRate = (from t1 in _db.PurchaseOrders
            //                                     join t2 in _db.LCInfoes on (long?)t1.LCId equals t2.LCId
            //                                     where t1.PurchaseOrderId == purchaseOrderId
            //                                     select t2.CurrenceyRate).FirstOrDefault();





            return vmPurchaseOrderSlave;
        }
        public async Task<VMPurchaseOrderSlave> KFMALProcurementPurchaseOrderSlaveGet(int companyId, int purchaseOrderId)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();

            //vmPurchaseOrderSlave.Currencyname = (from t1 in _db.PurchaseOrders
            //                                     join t2 in _db.LCInfoes on (long?)t1.LCId equals t2.LCId
            //                                     join t3 in _db.Currencies on t2.CurrenceyId equals t3.CurrencyId
            //                                     where t1.PurchaseOrderId == purchaseOrderId
            //                                     select t3.Name).FirstOrDefault();

            vmPurchaseOrderSlave = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                         join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                         join t4 in _db.Employees on t1.EmpId equals t4.Id into t4_Join
                                                         from t4 in t4_Join.DefaultIfEmpty()
                                                         join t5 in _db.Designations on t4.DesignationId equals t5.DesignationId into t5_Join
                                                         from t5 in t5_Join.DefaultIfEmpty()
                                                         join t6 in _db.LCInfoes on (long?)t1.LCId equals t6.LCId into lc
                                                         from t6 in lc.DefaultIfEmpty()
                                                         join t7 in _db.Currencies on t6.CurrenceyId equals t7.CurrencyId into curr
                                                         from t7 in curr.DefaultIfEmpty()

                                                         where t1.PurchaseOrderId == purchaseOrderId
                                                         select new VMPurchaseOrderSlave
                                                         {
                                                             PurchaseOrderId = t1.PurchaseOrderId,
                                                             SupplierName = t2.Name,
                                                             Code = t2.Code,
                                                             SupplierPropietor = t2.Propietor,
                                                             SupplierAddress = t2.Address,
                                                             SupplierMobile = t3.Phone,
                                                             EmployeeName = t4 != null ? t4.Name : "",
                                                             EmployeeMobile = t4 != null ? t4.MobileNo : "",
                                                             EmployeeDesignation = t5 != null ? t5.Name : "",
                                                             CID = t1.PurchaseOrderNo,
                                                             OrderDate = t1.PurchaseDate,
                                                             Description = t1.Remarks,
                                                             IsHold = t1.IsHold,
                                                             IsCancel = t1.IsCancel,
                                                             Status = t1.Status,
                                                             SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                             TermsAndCondition = t1.TermsAndCondition,
                                                             CompanyFK = t1.CompanyId,
                                                             DeliveryAddress = t1.DeliveryAddress,
                                                             DeliveryDate = t1.DeliveryDate,
                                                             Common_SupplierFK = t1.SupplierId,

                                                             CompanyName = t3.Name,
                                                             CompanyAddress = t3.Address,
                                                             CompanyEmail = t3.Email,
                                                             CompanyPhone = t3.Phone,
                                                             CreatedBy = t1.CreatedBy,
                                                             Companylogo = t3.CompanyLogo,

                                                             Currencyname = t7 != null ? t7.Name : "",
                                                             currencysign = t7 != null ? t7.Sign : "",
                                                             CurrencyRate = t6 != null ? t6.CurrenceyRate : 0,

                                                             LCNo = t6 != null ? t6.LCNo : "",
                                                             lcDate = t6 != null ? t6.LCDate : DateTime.Now,
                                                             LCValue = t6 != null ? t6.LCValue : 0,
                                                             LCValueInBDT = t6 != null ? t6.LCValueInBDT : 0,

                                                             CashMarginAmount = t6 != null ? t6.CashMarginAmount : 0,
                                                             CashMarginPercent = t6 != null ? t6.CashMarginPercent : 0,

                                                             BankCharge = t6 != null ? t6.BankCharge : 0,
                                                             FreightCharges = t6 != null ? t6.FreightCharges : 0,
                                                             CommissionValue = t6 != null ? t6.CommissionValue : 0,
                                                             InsuranceValue = t6 != null ? t6.InsuranceValue : 0,
                                                             OtherCharge = t6 != null ? (decimal)t6.OtherCharge : 0,

                                                         }).FirstOrDefault());

            vmPurchaseOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.PurchaseOrderDetails.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                                       join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                       join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                       join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                       join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                       select new VMPurchaseOrderSlave
                                                                       {
                                                                           ProductName = t4.Name + " " + t3.ProductName,

                                                                           PurchaseOrderId = t1.PurchaseOrderId.Value,
                                                                           PurchaseOrderDetailId = t1.PurchaseOrderDetailId,
                                                                           PurchaseQuantity = t1.PurchaseQty,
                                                                           PurchasingPrice = t1.PurchaseRate,
                                                                           UnitName = t6.Name,
                                                                           PurchaseAmount = t1.PurchaseAmount,

                                                                           CompanyFK = t1.CompanyId,
                                                                           Common_ProductCategoryFK = t5.ProductCategoryId,
                                                                           Common_ProductSubCategoryFK = t4.ProductSubCategoryId,
                                                                           Common_ProductFK = t3.ProductId

                                                                       }).OrderByDescending(x => x.PurchaseOrderDetailId).AsEnumerable());





            return vmPurchaseOrderSlave;
        }


        public async Task<VMPurchaseOrderSlave> PackagingPurchaseOrderSlaveGet(int companyId, int purchaseOrderId)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();


            vmPurchaseOrderSlave = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                         join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                         join t4 in _db.Employees on t1.EmpId equals t4.Id into t4_Join
                                                         from t4 in t4_Join.DefaultIfEmpty()
                                                         join t5 in _db.Designations on t4.DesignationId equals t5.DesignationId into t5_Join
                                                         from t5 in t5_Join.DefaultIfEmpty()
                                                         join t6 in _db.LCInfoes on (long?)t1.LCId equals t6.LCId into lc
                                                         from t6 in lc.DefaultIfEmpty()
                                                         join t7 in _db.Currencies on t6.CurrenceyId equals t7.CurrencyId into curr
                                                         from t7 in curr.DefaultIfEmpty()

                                                         where t1.PurchaseOrderId == purchaseOrderId
                                                         select new VMPurchaseOrderSlave
                                                         {
                                                             PurchaseOrderId = t1.PurchaseOrderId,
                                                             SupplierName = t2.Name,
                                                             Code = t2.Code,
                                                             SupplierPropietor = t2.Propietor,
                                                             SupplierAddress = t2.Address,
                                                             SupplierMobile = t3.Phone,
                                                             EmployeeName = t4 != null ? t4.Name : "",
                                                             EmployeeMobile = t4 != null ? t4.MobileNo : "",
                                                             EmployeeDesignation = t5 != null ? t5.Name : "",
                                                             CID = t1.PurchaseOrderNo,
                                                             OrderDate = t1.PurchaseDate,
                                                             Description = t1.Remarks,
                                                             IsHold = t1.IsHold,
                                                             IsCancel = t1.IsCancel,
                                                             Status = t1.Status,
                                                             SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                             TermsAndCondition = t1.TermsAndCondition,
                                                             CompanyFK = t1.CompanyId,
                                                             DeliveryAddress = t1.DeliveryAddress,
                                                             DeliveryDate = t1.DeliveryDate,
                                                             Common_SupplierFK = t1.SupplierId,
                                                             FreightCharge = t1.FreightCharge,
                                                             OtherCharge = t6 != null ? (decimal)t6.OtherCharge : 0,
                                                             CompanyName = t3.Name,
                                                             CompanyAddress = t3.Address,
                                                             CompanyEmail = t3.Email,
                                                             CompanyPhone = t3.Phone,
                                                             CreatedBy = t1.CreatedBy,
                                                             Companylogo = t3.CompanyLogo,
                                                             Currencyname = t7 != null ? t7.Name : "",
                                                             currencysign = t7 != null ? t7.Sign : "",
                                                             CurrencyRate = t6 != null ? t6.CurrenceyRate : 0,
                                                             LCNo = t6 != null ? t6.LCNo : "",
                                                             lcDate = t6 != null ? t6.LCDate : DateTime.Now,
                                                             LCValue = t6 != null ? t6.LCValue : 0,
                                                             BankCharge = t6 != null ? t6.BankCharge : 0,
                                                             FreightCharges = t6 != null ? t6.FreightCharges : 0,
                                                             CommissionValue = t6 != null ? t6.CommissionValue : 0,
                                                             InsuranceValue = t6 != null ? t6.InsuranceValue : 0,
                                                             AmendmentIncrase = t6 != null ? t6.AmendmentIncrase : 0,
                                                             AmendmentDiccrase = t6 != null ? t6.AmendmentDiccrase : 0,
                                                             CashMarginAmount = t6 != null ? t6.CashMarginAmount : 0,
                                                             CashMarginPercent = t6 != null ? t6.CashMarginPercent : 0,
                                                             TDSDeduction = t1.TDSDeduction,
                                                             TotalDiscount = t1.TotalDiscount


                                                         }).FirstOrDefault());

            vmPurchaseOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.PurchaseOrderDetails.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                                       join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                       join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                       join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                       join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                       select new VMPurchaseOrderSlave
                                                                       {
                                                                           ProductName = t4.Name + " " + t3.ProductName,

                                                                           PurchaseOrderId = t1.PurchaseOrderId.Value,
                                                                           PurchaseOrderDetailId = t1.PurchaseOrderDetailId,
                                                                           PurchaseQuantity = t1.PurchaseQty,
                                                                           PurchasingPrice = t1.PurchaseRate,
                                                                           UnitName = t6.Name,
                                                                           PurchaseAmount = t1.PurchaseAmount,
                                                                           VATAddition = t1.VATAddition,
                                                                           IsVATIncluded = t1.IsVATIncluded,
                                                                           CompanyFK = t1.CompanyId,
                                                                           Common_ProductCategoryFK = t5.ProductCategoryId,
                                                                           Common_ProductSubCategoryFK = t4.ProductSubCategoryId,
                                                                           Common_ProductFK = t3.ProductId,
                                                                           TotalDiscount = t1.ProductDiscount

                                                                       }).OrderByDescending(x => x.PurchaseOrderDetailId).AsEnumerable());




            return vmPurchaseOrderSlave;
        }

        public async Task<IssueDetailInfoVM> PromotionalItemInvoiceGet(int companyId, int IssueMasterId)
        {
            IssueDetailInfoVM vmIssueDetailInfoVM = new IssueDetailInfoVM();


            vmIssueDetailInfoVM = await (from t1 in _db.IssueMasterInfoes.Where(x => x.IsActive && x.IssueMasterId == IssueMasterId && x.CompanyId == companyId)
                                         join t2 in _db.Vendors on t1.VendorId equals t2.VendorId
                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                         join t4 in _db.Employees on t1.IssuedBy equals t4.Id into t4_Join
                                         from t4 in t4_Join.DefaultIfEmpty()
                                         join t5 in _db.Employees on t1.AchknologeBy equals t5.Id into t5_Join
                                         from t5 in t5_Join.DefaultIfEmpty()



                                         where t1.IssueMasterId == IssueMasterId
                                         select new IssueDetailInfoVM
                                         {
                                             IssueMasterId = t1.IssueMasterId,
                                             CustomerBy = t2.Name,
                                             Code = t2.Code,
                                             CustomerAddress = t2.Address,
                                             CustomerMobile = t3.Phone,
                                             IssueBy = t4 != null ? t4.Name : "",
                                             EmployeeMobile = t4 != null ? t4.MobileNo : "",
                                             CompanyName = t3.Name,
                                             CompanyAddress = t3.Address,
                                             CompanyEmail = t3.Email,
                                             CompanyPhone = t3.Phone,
                                             CreatedBy = t1.CreatedBy,
                                             IssueNo = t1.IssueNo,
                                             IssueDate = t1.IssueDate,
                                             CreatedDate = t1.CreatedDate,
                                             AchknologeName = t5.Name,
                                             AcknologeDate = t1.AcknologeDate,
                                             Achknolagement = t1.Achknolagement,
                                             IsSubmit = t1.IsSubmit,
                                             Remarks = t1.Remarks,
                                             CompanyFK = t1.CompanyId


                                         }).FirstOrDefaultAsync();

            vmIssueDetailInfoVM.DataListSlave = await (from t1 in _db.IssueDetailInfoes.Where(x => x.IsActive == true && x.IssueMasterId == IssueMasterId)
                                                       join t3 in _db.Products.Where(x => x.IsActive) on t1.RProductId equals t3.ProductId
                                                       join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                       join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                       join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                       select new IssueDetailInfoVM
                                                       {
                                                           ProductName = t4.Name + " " + t3.ProductName,
                                                           IssueMasterId = (int)t1.IssueMasterId,
                                                           IssueDetailId = t1.IssueDetailId,
                                                           RMQ = t1.RMQ,
                                                           CostingPrice = t1.CostingPrice,
                                                           UnitName = t6.Name,
                                                           RProductId = t3.ProductId,
                                                           AccountingHeadId = t5.AccountingHeadId

                                                       }).OrderByDescending(x => x.IssueDetailId).ToListAsync();




            return vmIssueDetailInfoVM;
        }

        public async Task<VMPromtionalOfferDetail> ProcurementPromtionalOfferDetailGet(int companyId, int promtionalOfferId)
        {
            VMPromtionalOfferDetail vmPromtionalOfferDetail = new VMPromtionalOfferDetail();
            vmPromtionalOfferDetail = await Task.Run(() => (from t1 in _db.PromtionalOffers.Where(x => x.IsActive && x.PromtionalOfferId == promtionalOfferId && x.CompanyId == companyId)

                                                            select new VMPromtionalOfferDetail
                                                            {
                                                                CompanyId = t1.CompanyId,
                                                                CreatedBy = t1.CreatedBy,
                                                                CreatedDate = t1.CreatedDate,
                                                                FromDate = t1.FromDate,
                                                                ModifiedBy = t1.ModifiedBy,
                                                                ModifiedDate = t1.ModifiedDate,
                                                                PromoCode = t1.PromoCode,
                                                                PromtionalOfferId = t1.PromtionalOfferId,
                                                                PromtionType = t1.PromtionType,
                                                                ToDate = t1.ToDate,
                                                                IsSumitted = t1.IsSubmitted


                                                            }).FirstOrDefault());

            vmPromtionalOfferDetail.DataListSlave = await Task.Run(() => (from t1 in _db.PromtionalOfferDetails.Where(x => x.IsActive && x.PromtionalOfferId == promtionalOfferId)
                                                                          join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId into t3_Join
                                                                          from t3 in t3_Join.DefaultIfEmpty()
                                                                          join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId into t4_Join
                                                                          from t4 in t4_Join.DefaultIfEmpty()
                                                                          join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId into t5_Join
                                                                          from t5 in t5_Join.DefaultIfEmpty()
                                                                          join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId into t6_Join
                                                                          from t6 in t6_Join.DefaultIfEmpty()
                                                                          select new VMPromtionalOfferDetail
                                                                          {
                                                                              PromtionalOfferDetailId = t1.PromtionalOfferDetailId,
                                                                              ProductName = t4 != null ? t4.Name + " " + t3.ProductName : "",
                                                                              UnitName = t6 != null ? t6.Name : "",
                                                                              ProductId = t3 != null ? t3.ProductId : 0,
                                                                              PromoAmount = t1.PromoAmount,
                                                                              PromoQuantity = t1.PromoQuantity
                                                                          }).OrderByDescending(x => x.PromtionalOfferDetailId).AsEnumerable());





            return vmPromtionalOfferDetail;
        }



        public async Task<VMPromtionalOffer> GetPromotionalOfferListAsync(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            var vmPromotionalOffer = new VMPromtionalOffer
            {
                DataList = await _db.PromtionalOffers
                    .Where(x => x.IsActive
                                 && x.CompanyId == companyId
                                 && (!fromDate.HasValue || x.FromDate >= fromDate.Value)
                                || (!toDate.HasValue || x.ToDate <= toDate.Value))
                    .Select(t1 => new VMPromtionalOffer
                    {
                        CompanyId = t1.CompanyId,
                        CreatedBy = t1.CreatedBy,
                        CreatedDate = t1.CreatedDate,
                        FromDate = t1.FromDate,
                        ModifiedBy = t1.ModifiedBy,
                        ModifiedDate = t1.ModifiedDate,
                        PromoCode = t1.PromoCode,
                        PromtionalOfferId = t1.PromtionalOfferId,
                        PromtionType = t1.PromtionType,
                        IsSumitted = t1.IsSubmitted,
                        ToDate = t1.ToDate
                    })
                    .OrderByDescending(c => c.PromtionalOfferId)
                    .ToListAsync() // Use ToListAsync for better performance  
            };

            return vmPromotionalOffer;
        }


        public async Task<VMPurchaseOrderSlave> ProcurementPurchaseOrderSlaveOpeningBalanceGet(int companyId)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();
            vmPurchaseOrderSlave.CompanyFK = companyId;

            vmPurchaseOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.PurchaseOrderDetails.Where(x => x.IsActive && x.CompanyId == companyId)
                                                                       join t3 in _db.PurchaseOrders.Where(x => x.IsActive && x.IsOpening) on t1.PurchaseOrderId equals t3.PurchaseOrderId
                                                                       join t2 in _db.Vendors on t3.SupplierId equals t2.VendorId
                                                                       join t4 in _db.Companies on t3.CompanyId equals t4.CompanyId
                                                                       select new VMPurchaseOrderSlave
                                                                       {
                                                                           ProcuredQuantity = t1.PurchaseQty,
                                                                           PurchasingPrice = t1.PurchaseRate,
                                                                           PurchaseAmount = t1.PurchaseAmount,
                                                                           CompanyFK = companyId,
                                                                           PurchaseOrderId = t3.PurchaseOrderId,
                                                                           SupplierName = t2.Name,
                                                                           CID = t3.PurchaseOrderNo,
                                                                           CreatedDate = t3.CreatedDate,
                                                                           Description = t3.Remarks
                                                                       }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());





            return vmPurchaseOrderSlave;
        }

        public async Task<VMPurchaseOrderSlave> GetSingleProcurementPurchaseOrderSlave(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.PurchaseOrderDetails
                                          join t2 in _db.Products on t1.ProductId equals t2.ProductId
                                          join t3 in _db.Units on t2.UnitId equals t3.UnitId

                                          where t1.PurchaseOrderDetailId == id
                                          select new VMPurchaseOrderSlave
                                          {
                                              ProductName = t2.ProductName,
                                              Common_ProductFK = t2.ProductId,
                                              PurchaseOrderId = t1.PurchaseOrderId.Value,

                                              PurchaseOrderDetailId = t1.PurchaseOrderDetailId,
                                              PurchaseQuantity = t1.PurchaseQty,
                                              PurchasingPrice = t1.PurchaseRate,
                                              PurchaseAmount = t1.PurchaseAmount,
                                              UnitName = t3.Name,
                                              CompanyFK = t1.CompanyId,


                                          }).FirstOrDefault());
            return v;
        }
        public async Task<VMPurchaseOrderSlave> KFMALGetSingleProcurementPurchaseOrderSlave(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.PurchaseOrderDetails
                                          join t2 in _db.Products on t1.ProductId equals t2.ProductId
                                          join t3 in _db.Units on t2.UnitId equals t3.UnitId

                                          where t1.PurchaseOrderDetailId == id
                                          select new VMPurchaseOrderSlave
                                          {
                                              ProductName = t2.ProductName,
                                              Common_ProductFK = t2.ProductId,
                                              PurchaseOrderId = t1.PurchaseOrderId.Value,

                                              PurchaseOrderDetailId = t1.PurchaseOrderDetailId,
                                              PurchaseQuantity = t1.PurchaseQty,
                                              PurchasingPrice = t1.PurchaseRate,
                                              PurchaseAmount = t1.PurchaseAmount,
                                              UnitName = t3.Name,
                                              CompanyFK = t1.CompanyId,
                                              VATAddition = t1.VATAddition

                                          }).FirstOrDefault());
            return v;
        }

        public async Task<IssueDetailInfoVM> SinglePromotionalItemInvoice(int id)
        {
            var v = await (from t1 in _db.IssueDetailInfoes
                           join t2 in _db.Products on t1.RProductId equals t2.ProductId
                           join t3 in _db.Units on t2.UnitId equals t3.UnitId

                           where t1.IssueDetailId == id
                           select new IssueDetailInfoVM
                           {
                               ProductName = t2.ProductName,
                               RProductId = t2.ProductId,
                               IssueMasterId = (int)t1.IssueMasterId,
                               IssueDetailId = t1.IssueDetailId,
                               RMQ = t1.RMQ,
                               CostingPrice = t1.CostingPrice,
                               UnitName = t3.Name
                           }).FirstOrDefaultAsync();
            return v;
        }
        public async Task<long> KFMALProcurementPurchaseOrderSlaveAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
            {
                PurchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId,
                ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),

                DemandRate = 0,
                QCRate = 0,
                PackSize = 0,

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = procurementPurchaseOrderSlave.PurchaseOrderDetailId;
            }

            return result;
        }
        public async Task<long> ProcurementPurchaseOrderSlaveAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
            {
                PurchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId,
                ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),

                DemandRate = 0,
                QCRate = 0,
                PackSize = 0,

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = procurementPurchaseOrderSlave.PurchaseOrderDetailId;
            }

            return result;
        }
        public async Task<long> ProcurementPurchaseOrderSlaveOpeningAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
            {
                PurchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId,
                ProductId = 3125, //GCCL
                PurchaseQty = 1,
                PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                PurchaseAmount = vmPurchaseOrderSlave.PurchasingPrice,

                DemandRate = 0,
                QCRate = 0,
                PackSize = 0,

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = procurementPurchaseOrderSlave.PurchaseOrderDetailId;
            }

            return result;
        }
        public async Task<int> PackagingPurchaseOrderSlaveEdit(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            var result = -1;
            PurchaseOrderDetail model = await _db.PurchaseOrderDetails.FindAsync(vmPurchaseOrderSlave.PurchaseOrderDetailId);

            model.ProductId = vmPurchaseOrderSlave.Common_ProductFK;
            model.PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity;
            model.PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice;
            model.PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice);
            model.VATAddition = vmPurchaseOrderSlave.VATAddition;
            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmPurchaseOrderSlave.ID;
            }

            return result;
        }

        public async Task<int> PromotionalItemInvoiceChildEdit(IssueDetailInfoVM issueDetailInfoVM)
        {
            var result = -1;
            IssueDetailInfo model = await _db.IssueDetailInfoes.FindAsync(issueDetailInfoVM.IssueDetailId);
            if (model is null) return result;

            model.RProductId = issueDetailInfoVM.RProductId;
            model.RMQ = issueDetailInfoVM.RMQ;
            model.CostingPrice = issueDetailInfoVM.CostingPrice;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)model.IssueMasterId;
            }

            return result;
        }

        public async Task<int> KFMALProcurementPurchaseOrderSlaveEdit(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            var result = -1;
            PurchaseOrderDetail model = await _db.PurchaseOrderDetails.FindAsync(vmPurchaseOrderSlave.PurchaseOrderDetailId);

            model.ProductId = vmPurchaseOrderSlave.Common_ProductFK;
            model.PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity;
            model.PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice;
            model.PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice);

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmPurchaseOrderSlave.ID;
            }

            return result;
        }
        public async Task<int> ProcurementPurchaseOrderSlaveEdit(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            var result = -1;
            PurchaseOrderDetail model = await _db.PurchaseOrderDetails.FindAsync(vmPurchaseOrderSlave.PurchaseOrderDetailId);

            model.ProductId = vmPurchaseOrderSlave.Common_ProductFK;
            model.PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity;
            model.PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice;
            model.PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice);

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmPurchaseOrderSlave.ID;
            }

            return result;
        }
        public async Task<long> ProcurementPurchaseOrderSlaveDelete(long id)
        {
            long result = -1;
            PurchaseOrderDetail procurementPurchaseOrderSlave = await _db.PurchaseOrderDetails.FindAsync(id);
            if (procurementPurchaseOrderSlave != null)
            {
                procurementPurchaseOrderSlave.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrderSlave.PurchaseOrderDetailId;
                }
            }
            return result;
        }
        public async Task<long> PromotionalItemInvoiceChildDelete(long id)
        {
            long result = -1;
            IssueDetailInfo issueDetail = await _db.IssueDetailInfoes.FindAsync(id);
            if (issueDetail != null)
            {
                issueDetail.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = (int)issueDetail.IssueMasterId;
                }
            }
            return result;
        }

        #endregion


        public async Task<int> PromtionalOfferDetailsDelete(int id)
        {
            int result = -1;
            PromtionalOfferDetail promtionalOfferDetails = await _db.PromtionalOfferDetails.FindAsync(id);
            if (promtionalOfferDetails != null)
            {
                promtionalOfferDetails.IsActive = false;
                promtionalOfferDetails.ModifiedBy = Common.GetUserId();
                promtionalOfferDetails.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = promtionalOfferDetails.PromtionalOfferId;
                }
            }
            return result;
        }

        public async Task<int> PromtionalOfferEdit(int offerId, string offerCode, DateTime fromDate, DateTime toDate)
        {
            int result = -1;
            PromtionalOffer promtionalOfferModel = await _db.PromtionalOffers.FindAsync(offerId);
            if (promtionalOfferModel != null)
            {
                promtionalOfferModel.PromoCode = offerCode;
                promtionalOfferModel.FromDate = fromDate;
                promtionalOfferModel.ToDate = toDate;
                _db.Entry(promtionalOfferModel).State = EntityState.Modified;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = promtionalOfferModel.PromtionalOfferId;
                }
            }
            return result;
        }

        public async Task<int> PromtionalOfferDetailsSubmited(int promtionalOfferId)
        {
            int result = -1;
            PromtionalOffer PromtionalOfferModel = await _db.PromtionalOffers.FindAsync(promtionalOfferId);
            if (PromtionalOfferModel != null)
            {
                PromtionalOfferModel.IsSubmitted = true;
                PromtionalOfferModel.ModifiedBy = Common.GetUserId();
                PromtionalOfferModel.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PromtionalOfferModel.PromtionalOfferId;
                }
            }
            return result;
        }

        public async Task<int> PromtionalOfferDelete(int promtionalOfferId)
        {
            int result = -1;
            PromtionalOffer PromtionalOfferModel = await _db.PromtionalOffers.FindAsync(promtionalOfferId);
            if (PromtionalOfferModel != null)
            {
                PromtionalOfferModel.IsActive = false;
                PromtionalOfferModel.ModifiedBy = Common.GetUserId();
                PromtionalOfferModel.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PromtionalOfferModel.PromtionalOfferId;
                }
            }
            return result;
        }


        public async Task<List<VMCommonCustomer>> CustomerLisBySubZonetGet(int subZoneId)
        {

            List<VMCommonCustomer> vmCommonCustomerList =
                await Task.Run(() => (_db.Vendors
                .Where(x => x.IsActive && x.SubZoneId == subZoneId))
                .Select(x => new VMCommonCustomer() { ID = x.VendorId, Name = x.Code + " -" + x.Name + " -" + x.Address })
                .ToListAsync());


            return vmCommonCustomerList;
        }

        public async Task<List<SalesTransferDetailVM>> OrderDelivieryListByOrderMaster(long orderMasterId)
        {

            List<SalesTransferDetailVM> vmOrderDeliversList =
                await Task.Run(() => (_db.OrderDelivers
                .Where(x => x.IsActive && x.OrderMasterId == orderMasterId))
                .Select(x => new SalesTransferDetailVM() { OrderDeliverId = x.OrderDeliverId, ChallanNo = x.ChallanNo })
                .ToListAsync());


            return vmOrderDeliversList;
        }

        public async Task<List<VMCommonCustomer>> CustomerLisByZonetGet(int zoneId)
        {

            List<VMCommonCustomer> vmCommonCustomerList =
                await Task.Run(() => (_db.Vendors
                .Where(x => x.IsActive && x.ZoneId == zoneId))
                .Select(x => new VMCommonCustomer() { ID = x.VendorId, Name = x.Code + " -" + x.Name })
                .ToListAsync());


            return vmCommonCustomerList;
        }
        #region Purchase Order Detail
        public async Task<List<VMSalesOrder>> SalesOrderLisByCustomerIdGet(int customerId)
        {

            List<VMSalesOrder> vmCommonCustomerList =
                await Task.Run(() => (_db.OrderMasters
                .Where(x => x.IsActive && x.CustomerId == customerId))
                .Select(x => new VMSalesOrder() { OrderMasterId = x.OrderMasterId, OrderNo = x.OrderNo + " -" + x.OrderDate })
                .ToListAsync());


            return vmCommonCustomerList;
        }
        public async Task<FeedSalesOrderModel> FeedPaymentGet(int companyId, long orderMasterId)
        {
            FeedSalesOrderModel VMPayment = new FeedSalesOrderModel();
            if (orderMasterId > 0)
            {
                VMPayment = await Task.Run(() => (from t2 in _db.PaymentMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                  join t3 in _db.HeadGLs on t2.PaymentToHeadGLId equals t3.Id into t3_join
                                                  from t3 in t3_join.DefaultIfEmpty()

                                                  select new FeedSalesOrderModel
                                                  {
                                                      PaymentNo = t2.PaymentNo,
                                                      TransactionDate = t2.TransactionDate,
                                                      PaymentToHeadGLId = t2.PaymentToHeadGLId,
                                                      PaymentToHeadGLName = t3 != null ? t3.AccCode + " - " + t3.AccName : "",
                                                      IsFinalized = t2.IsFinalized,
                                                      PaymentMasterId = t2.PaymentMasterId,
                                                      CompanyId = t2.CompanyId,
                                                      IsFeedVoucherCreated = t2.IsFeedVoucherCreated
                                                  }).FirstOrDefault());
            }
            ////iuiiu 

            if (VMPayment != null)
            {
                VMPayment.DataListPayment = await Task.Run(() => (from t1 in _db.Payments.Where(x => x.IsActive && x.CompanyId == companyId)
                                                                  join t2 in _db.PaymentMasters.Where(x => x.OrderMasterId == orderMasterId) on t1.PaymentMasterId equals t2.PaymentMasterId
                                                                  join t0 in _db.Vendors on t1.VendorId equals t0.VendorId
                                                                  join t5 in _db.HeadGLs on t1.PaymentFromHeadGLId equals t5.Id
                                                                  where t1.PaymentMasterId == VMPayment.PaymentMasterId

                                                                  select new FeedSalesOrderModel
                                                                  {
                                                                      InAmount = t1.InAmount,
                                                                      CustomerId = t1.VendorId,
                                                                      CommonCustomerName = t0.Name,
                                                                      CommonCustomerCode = t0.Code,
                                                                      PaymentFromHeadGLName = t5.AccCode + " - " + t5.AccName,
                                                                      PaymentFromHeadGLId = t1.PaymentFromHeadGLId,
                                                                      CompanyId = t1.CompanyId,
                                                                      CreatedBy = t1.CreatedBy,
                                                                      PaymentId = t1.PaymentId,
                                                                      ReferenceNo = t1.ReferenceNo
                                                                  }).OrderByDescending(x => x.PaymentId).AsEnumerable());
            }


            return VMPayment;
        }

        public async Task<long> UpdateSignatoryApproval(int signatoryApprovalId, SignatoryStatusEnum statusEnum, string comments)
        {
            var exist = _db.ApprovalOrderMasters.Find(signatoryApprovalId);
            if (exist == null)
            {
                return 0;
            }

            // Set the approval date and comments
            exist.ApprovalDate = DateTime.Now;
            exist.Comments = comments;
            exist.ApprovalStatus = (int)statusEnum;

            // Save the approval change
            if (await _db.SaveChangesAsync() > 0)
            {
                var orderMaster = _db.OrderMasters.FirstOrDefault(x => x.OrderMasterId == exist.OrderMasterId);
                if (orderMaster != null)
                {
                    // Get all active approval orders for this OrderMaster
                    var approvals = _db.ApprovalOrderMasters
                                        .Where(x => x.OrderMasterId == orderMaster.OrderMasterId && x.IsActive)
                                        .ToList();

                    // Process approval logic based on precedence
                    if (statusEnum == SignatoryStatusEnum.Approved)
                    {
                        // Get all records with the same precedence as the one that was approved
                        var precedenceGroup = approvals.Where(x => x.Precedence == exist.Precedence).ToList();
                        foreach (var item in precedenceGroup)
                        {
                            // Mark all the records of this precedence as approved
                            item.ApprovalStatus = (int)SignatoryStatusEnum.Approved;
                        }

                        // Set records with the next precedence level (precedence + 1) to Pending
                        var nextPrecedenceGroup = approvals.Where(x => x.Precedence == exist.Precedence + 1).ToList();
                        foreach (var item in nextPrecedenceGroup)
                        {
                            item.ApprovalStatus = (int)SignatoryStatusEnum.Pending;
                        }

                        var approvedPending = _db.ApprovalOrderMasters
                                       .Where(x => x.OrderMasterId == orderMaster.OrderMasterId && x.IsActive && x.ApprovalStatus <= 0)
                                       .ToList();

                        if (approvedPending.Count() == 0)
                        {
                            orderMaster.Status = (int)EnumOrderMasterStatus.Approval;
                        }


                        // Save changes for updated records
                        await _db.SaveChangesAsync();
                    }

                    // Handle rejection scenario
                    else if (statusEnum == SignatoryStatusEnum.Rejected)
                    {
                        // If rejected, deactivate all related approvals for this order
                        var appvals = _db.ApprovalOrderMasters.Where(x => x.OrderMasterId == exist.OrderMasterId && x.IsActive).ToList();
                        appvals.ForEach(x => x.IsActive = false);

                        // Set OrderMaster status to Draft
                        if (orderMaster != null)
                        {
                            orderMaster.Status = (int)EnumOrderMasterStatus.Draft;
                            await _db.SaveChangesAsync();
                        }
                    }

                    return exist.OrderMasterId;
                }
            }

            return 0;
        }




        public async Task<FeedSalesOrderModel> FeedSalesOrderDetailsGet(int companyId, long orderMasterId)
        {

            FeedSalesOrderModel vmSalesOrderSlave = new FeedSalesOrderModel();
            vmSalesOrderSlave = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId && x.CompanyId == companyId) //&& (SalePersonId > 0 ? x.SalePersonId == SalePersonId : x.SalePersonId > 0)
                                                      join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                      join t5 in _db.Zones on t2.ZoneId equals t5.ZoneId
                                                      join t6 in _db.StockInfoes on t1.StockInfoId equals t6.StockInfoId
                                                      join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                                                      join t4 in _db.Employees on t1.SalePersonId equals t4.Id
                                                      //join t7 in _db.PaymentMasters.Where(x => x.IsActive)
                                                      //on t1.OrderMasterId equals t7.OrderMasterId into t7_Join
                                                      //from t7 in t7_Join.DefaultIfEmpty()
                                                      //join t8 in _db.HeadGLs on t7.PaymentToHeadGLId equals t8.Id into t8_join
                                                      //from t8 in t8_join.DefaultIfEmpty()


                                                      select new FeedSalesOrderModel
                                                      {
                                                          StockInfoId = t1.StockInfoId,
                                                          WareHouse = t6.Name,

                                                          CreatedDate = t1.CreateDate,
                                                          CreatedBy = t1.CreatedBy,

                                                          CompanyId = t1.CompanyId ?? 0,
                                                          ComLogo = t3.CompanyLogo,
                                                          CompanyName = t3.Name,
                                                          CompanyAddress = t3.Address,
                                                          CompanyEmail = t3.Email,
                                                          CompanyPhone = t3.Phone,

                                                          CommonCustomerCode = t2.Code,
                                                          CommonCustomerName = t2.Name,
                                                          CustomerPhone = t2.Phone,
                                                          CustomerAddress = t2.Address,
                                                          CustomerEmail = t2.Email,
                                                          ZoneName = t5.Name,
                                                          ContactPerson = t2.ContactName,
                                                          OrderStatus = t1.OrderStatus,
                                                          ProductType = t1.ProductType,

                                                          OrderMasterId = t1.OrderMasterId,
                                                          Status = t1.Status,
                                                          OrderDate = t1.OrderDate,
                                                          SalesOrderNo = t1.OrderNo,
                                                          Remarks = t1.Remarks,
                                                          ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                          SalePersonName = t4.Name,

                                                          CustomerId = t2.VendorId,
                                                          HeadGLId = t2.HeadGLId,

                                                      }).FirstOrDefault()); ;

            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                                    join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                    join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                    join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                    join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                    select new FeedSalesOrderModel
                                                                    {
                                                                        ProductName = t4.Name + " " + t3.ProductName,
                                                                        ProductCategoryName = t5.Name,
                                                                        OrderMasterId = t1.OrderMasterId.Value,
                                                                        OrderDetailId = t1.OrderDetailId,
                                                                        Qty = t1.Qty,
                                                                        UnitPrice = t1.UnitPrice,
                                                                        UnitName = t6.Name,
                                                                        PackQuantity = t1.PackQuantity,
                                                                        Consumption = t1.Comsumption,
                                                                        ProductCategoryId = t5.ProductCategoryId,
                                                                        ProductSubCategoryId = t4.ProductSubCategoryId,
                                                                        FProductId = t3.ProductId,
                                                                        SpecialDiscount = t1.SpecialDiscount,
                                                                        BaseCommission = t1.EBaseCommission,
                                                                        CashCommission = t1.ECashCommission,
                                                                        CarryingCommission = t1.ECarryingCommission,
                                                                        AdditionalPrice = t1.AdditionPrice,
                                                                        MonthlyIncentiveInInvoice = t1.MonthlyIncentive,
                                                                        YearlyIncentiveInInvoice = t1.YearlyIncentive
                                                                    }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());
            if (vmSalesOrderSlave.OrderMasterId > 0)
            {
                vmSalesOrderSlave.DataListPayment = await Task.Run(() => (from t1 in _db.Payments.Where(x => x.IsActive && x.CompanyId == companyId)
                                                                          join t0 in _db.Vendors on t1.VendorId equals t0.VendorId
                                                                          join t5 in _db.HeadGLs on t1.BankId equals t5.Id into leftJoin
                                                                          from lj in leftJoin.DefaultIfEmpty()
                                                                          where t1.OrderMasterId == orderMasterId
                                                                          select new VMFeedPayment
                                                                          {
                                                                              InAmount = t1.InAmount,
                                                                              TransactionDate = t1.TransactionDate,

                                                                              BankName = (t1.BranchName == "Advance" ? t1.BranchName + " In " + lj.AccName : lj.AccName) ?? t1.BranchName, // Check if BranchName is "advance"
                                                                              MoneyReceiptNo = t1.MoneyReceiptNo,
                                                                              CustomerId = t1.VendorId,
                                                                              CommonCustomerName = t0.Name,
                                                                              CommonCustomerCode = t0.Code,
                                                                              PaymentFromHeadGLName = lj.AccCode + " - " + lj.AccName,
                                                                              PaymentFromHeadGLId = t1.PaymentFromHeadGLId,
                                                                              CompanyId = t1.CompanyId,
                                                                              CreatedBy = t1.CreatedBy,
                                                                              PaymentId = t1.PaymentId,
                                                                              ReferenceNo = t1.ReferenceNo,
                                                                              BranchName = t1.BranchName
                                                                          }).OrderByDescending(x => x.PaymentId).AsEnumerable());

                vmSalesOrderSlave.CurrentInvoiceAdvanced = vmSalesOrderSlave.DataListPayment.Where(x => x.BranchName == "Advance")
                                                                      .Select(x => x.InAmount).DefaultIfEmpty(0).Sum();


                vmSalesOrderSlave.PendingInvoiceAdvanced = await Task.Run(() => (from t1 in _db.Payments.Where(x => x.IsActive == true &&
                                                                                                                    x.CompanyId == companyId &&
                                                                                                                    x.VendorId == vmSalesOrderSlave.CustomerId &&
                                                                                                                    x.BranchName == "Advance")
                                                                                 join t2 in _db.OrderMasters
                                                                          .Where(x => x.OrderStatus != "D"
                                                                          && x.IsActive == true && x.OrderMasterId != vmSalesOrderSlave.OrderMasterId
                                                                          )
                                                                          on t1.OrderMasterId equals t2.OrderMasterId

                                                                                 select t1.InAmount).DefaultIfEmpty(0).Sum());


            }


            vmSalesOrderSlave.SignatoryApprovalList = await (from approval in _db.ApprovalOrderMasters
                                                             join a in _db.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                                                             join o in _db.OrderMasters on approval.OrderMasterId equals o.OrderMasterId
                                                             join v in _db.Vendors on o.CustomerId equals v.VendorId
                                                             join e in _db.Employees on a.EmployeeId equals e.Id
                                                             join c in _db.Companies on e.CompanyId equals c.CompanyId
                                                             join d in _db.Departments on e.DepartmentId equals d.DepartmentId
                                                             join ds in _db.Designations on e.DesignationId equals ds.DesignationId
                                                             where approval.IsActive
                                                             && approval.OrderMasterId == orderMasterId


                                                             select new OrderMasterSignatoryApprovalVM()
                                                             {
                                                                 CompanyId = o.CompanyId,
                                                                 ProductType = o.ProductType,
                                                                 Id = approval.Id,
                                                                 EmployeeId = e.EmployeeId,
                                                                 ApproverEmployeeIntId = a.EmployeeId.Value,

                                                                 EmployeeName = e.Name,
                                                                 CompanyName = c.Name,
                                                                 DepartmentName = d.Name,
                                                                 DesignationName = ds.Name,
                                                                 OrderNo = o.OrderNo,
                                                                 OrderMasterId = o.OrderMasterId,
                                                                 OrderDate = o.OrderDate,
                                                                 CoustomerName = v.Name,
                                                                 ApprovalOrderMasterSignatoryId = a.SalesOrderSignatoryId,
                                                                 ApprovalDate = approval.ApprovalDate,
                                                                 Comments = approval.Comments,
                                                                 SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus,
                                                                 IsPreviousApproved = _db.ApprovalOrderMasters.Where(x => x.IsActive)
                                                                              .Any(x => x.OrderMasterId == approval.OrderMasterId &&
                                                                                        x.Id != approval.Id && (x.ApprovalStatus != (int)SignatoryStatusEnum.Approved) &&
                                                                                        _db.OrderMasterSignatories.Where(o => o.IsActive)
                                                                                                .Any(y => y.SalesOrderSignatoryId == x.SalesOrderSignatoryId &&
                                                                                                          y.Precedence < a.Precedence &&
                                                                                                          y.IsActive))

                                                             }).AsQueryable().OrderBy(x => x.SignatoryStatus).ToListAsync();

            vmSalesOrderSlave.LedgerBalance = _db.Database.SqlQuery<decimal>("EXEC CustomerLadegerBalance {0},{1} ", vmSalesOrderSlave.CompanyId, vmSalesOrderSlave.HeadGLId).FirstOrDefault();




            return vmSalesOrderSlave;
        }
        //Starts Feed ProcurementSalesOrderDetailsGet -22 May 2022
        public async Task<VMSalesOrderSlave> FeedProcurementSalesOrderDetailsGet(int companyId, int orderMasterId)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            vmSalesOrderSlave = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId && x.CompanyId == companyId)
                                                      join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                      //join t4 in _db.SubZones on t2.SubZoneId equals t4.SubZoneId
                                                      join t5 in _db.Zones on t2.SubZoneId equals t5.ZoneId
                                                      join t6 in _db.StockInfoes on t1.StockInfoId equals t6.StockInfoId into t6_Join
                                                      from t6 in t6_Join.DefaultIfEmpty()
                                                      join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                                                      join t7 in _db.Demands on t1.DemandId equals t7.DemandId into t7_join
                                                      from t7 in t7_join.DefaultIfEmpty()

                                                      select new VMSalesOrderSlave
                                                      {
                                                          WareHouse = t6 != null ? t6.Name : "",
                                                          DemandNo = t7 == null ? "" : t7.DemandNo,
                                                          Propietor = t2.Propietor,
                                                          CreatedDate = t1.CreateDate,
                                                          ComLogo = t3.CompanyLogo,
                                                          CustomerPhone = t2.Phone,
                                                          CustomerAddress = t2.Address,
                                                          CustomerEmail = t2.Email,
                                                          ContactPerson = t2.ContactName,
                                                          CompanyFK = t1.CompanyId,
                                                          OrderMasterId = t1.OrderMasterId,
                                                          CreditLimit = t2.CreditLimit,
                                                          OrderNo = t1.OrderNo,
                                                          Status = t1.Status,
                                                          OrderDate = t1.OrderDate,
                                                          CreatedBy = t1.CreatedBy,
                                                          CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                          ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                          CommonCustomerName = t2.Name,
                                                          CompanyName = t3.Name,
                                                          CompanyAddress = t3.Address,
                                                          CompanyEmail = t3.Email,
                                                          CompanyPhone = t3.Phone,
                                                          ZoneName = t5.Name,
                                                          ZoneIncharge = t5.ZoneIncharge,
                                                          //SubZonesName = t4.Name,
                                                          //SubZoneIncharge = t4.SalesOfficerName,
                                                          //SubZoneInchargeMobile = t4.MobileOffice,
                                                          CommonCustomerCode = t2.Code,
                                                          CustomerTypeFk = t2.CustomerTypeFK,
                                                          CustomerId = t2.VendorId,
                                                          CourierCharge = t1.CourierCharge,
                                                          FinalDestination = t1.FinalDestination,
                                                          CourierNo = t1.CourierNo,
                                                          DiscountAmount = t1.DiscountAmount ?? 0,
                                                          DiscountRate = t1.DiscountRate ?? 0,
                                                          TotalAmountAfterDiscount = t1.TotalAmount ?? 0
                                                      }).FirstOrDefault());

            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                                    join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                    join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                    join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                    join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                    select new VMSalesOrderSlave
                                                                    {
                                                                        ProductName = t4.Name + " " + t3.ProductName,
                                                                        ProductCategoryName = t5.Name,
                                                                        OrderMasterId = t1.OrderMasterId.Value,
                                                                        OrderDetailId = t1.OrderDetailId,
                                                                        Qty = t1.Qty,
                                                                        UnitPrice = t1.UnitPrice,
                                                                        UnitName = t6.Name,
                                                                        TotalAmount = t1.Amount,
                                                                        PackQuantity = t1.PackQuantity,
                                                                        Consumption = t1.Comsumption,
                                                                        PromotionalOfferId = t1.PromotionalOfferId,
                                                                        ProductCategoryId = t5.ProductCategoryId,
                                                                        ProductSubCategoryId = t4.ProductSubCategoryId,
                                                                        FProductId = t3.ProductId,
                                                                        DiscountRate = t1.DiscountRate,
                                                                        ProductDiscountUnit = t1.DiscountUnit,
                                                                        ProductDiscountTotal = t1.DiscountAmount
                                                                    }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());


            return vmSalesOrderSlave;
        }
        //ENds Feed ProcurementSalesOrderDetailsGet -22 May 2022

        public async Task<VMSalesOrderSlave> ProcurementSalesOrderDetailsGet(int companyId, int orderMasterId)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            vmSalesOrderSlave = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId && x.CompanyId == companyId)
                                                      join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                      join t4 in _db.SubZones on t2.SubZoneId equals t4.SubZoneId
                                                      join t5 in _db.Zones on t4.ZoneId equals t5.ZoneId
                                                      join t6 in _db.StockInfoes on t1.StockInfoId equals t6.StockInfoId into t6_Join
                                                      from t6 in t6_Join.DefaultIfEmpty()
                                                      join t8 in _db.Employees on t1.SalePersonId equals t8.Id into t8_Join
                                                      from t8 in t8_Join.DefaultIfEmpty()
                                                      select new VMSalesOrderSlave
                                                      {
                                                          WareHouse = t6 != null ? t6.Name : "",
                                                          Propietor = t2.Propietor,
                                                          CreatedDate = t1.CreateDate,

                                                          CustomerPhone = t2.Phone,
                                                          CustomerAddress = t2.Address,
                                                          CustomerEmail = t2.Email,
                                                          ContactPerson = t2.ContactName,
                                                          CompanyFK = t1.CompanyId,
                                                          OrderMasterId = t1.OrderMasterId,
                                                          CreditLimit = t2.CreditLimit,
                                                          OrderNo = t1.OrderNo,
                                                          Status = t1.Status,
                                                          OrderDate = t1.OrderDate,
                                                          CreatedBy = t1.CreatedBy,
                                                          CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                          ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                          CommonCustomerName = t2.Name,

                                                          ZoneName = t5.Name,
                                                          ZoneIncharge = t5.ZoneIncharge,
                                                          SubZonesName = t4.Name,
                                                          SubZoneIncharge = t4.SalesOfficerName,
                                                          SubZoneInchargeMobile = t4.MobileOffice,
                                                          CommonCustomerCode = t2.Code,
                                                          CustomerTypeFk = t2.CustomerTypeFK,
                                                          CustomerId = t2.VendorId,
                                                          CourierCharge = t1.CourierCharge,
                                                          FinalDestination = t1.FinalDestination,
                                                          CourierNo = t1.CourierNo,
                                                          DiscountAmount = t1.DiscountAmount ?? 0,
                                                          DiscountRate = t1.DiscountRate ?? 0,
                                                          TotalAmountAfterDiscount = t1.TotalAmount ?? 0,
                                                          OfficerNAme = t8 != null ? t8.Name : ""




                                                      }).FirstOrDefault());





            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                                    join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                    join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                    join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                    join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                    select new VMSalesOrderSlave
                                                                    {

                                                                        OrderMasterId = t1.OrderMasterId.Value,
                                                                        OrderDetailId = t1.OrderDetailId,

                                                                        ProductName = t5.Name + " " + t4.Name + " " + t3.ProductName,
                                                                        Qty = t1.Qty,
                                                                        UnitName = t6.Name,
                                                                        UnitPrice = t1.UnitPrice,
                                                                        PackSize = t1.PackQuantity,
                                                                        TotalAmount = t1.Qty * t1.UnitPrice,
                                                                        QtyInPack = t3.FormulaQty,
                                                                        PromotionalOfferId = t1.PromotionalOfferId,

                                                                        FProductId = t3.ProductId,

                                                                        ProductDiscountUnit = t1.DiscountUnit,//Unit Discount                                                             

                                                                        SpecialDiscount = t1.SpecialBaseCommission,
                                                                        LotNumber = t1.LotNumber
                                                                        // SpecialDiscount   

                                                                    }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());

           var approvalList = await (from approval in _db.ApprovalOrderMasters
                                                                              join a in _db.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                                                                              join o in _db.OrderMasters on approval.OrderMasterId equals o.OrderMasterId
                                                                              join v in _db.Vendors on o.CustomerId equals v.VendorId
                                                                              join e in _db.Employees on a.EmployeeId equals e.Id
                                                                              join c in _db.Companies on e.CompanyId equals c.CompanyId
                                                                              join d in _db.Departments on e.DepartmentId equals d.DepartmentId
                                                                              join ds in _db.Designations on e.DesignationId equals ds.DesignationId
                                                                              where approval.IsActive
                                                                                    && approval.OrderMasterId == orderMasterId
                                                                              select new OrderMasterSignatoryApprovalVM()
                                                                              {
                                                                                  CompanyId = o.CompanyId,
                                                                                  ProductType = o.ProductType,
                                                                                  Id = approval.Id,
                                                                                  EmployeeId = e.EmployeeId,
                                                                                  ApproverEmployeeIntId = a.EmployeeId.Value,
                                                                                  EmployeeName = e.Name,
                                                                                  CompanyName = c.Name,
                                                                                  DepartmentName = d.Name,
                                                                                  DesignationName = ds.Name,
                                                                                  OrderNo = o.OrderNo,
                                                                                  OrderMasterId = o.OrderMasterId,
                                                                                  OrderDate = o.OrderDate,
                                                                                  CoustomerName = v.Name,
                                                                                  ApprovalOrderMasterSignatoryId = a.SalesOrderSignatoryId,
                                                                                  ApprovalDate = approval.ApprovalDate,
                                                                                  Comments = approval.Comments,
                                                                                  SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus,
                                                                                  IsPreviousApproved = _db.ApprovalOrderMasters.Where(x => x.IsActive)
                                                                                      .Any(x => x.OrderMasterId == approval.OrderMasterId
                                                                                            && x.Id != approval.Id
                                                                                            && (x.ApprovalStatus != (int)SignatoryStatusEnum.Approved)
                                                                                            && _db.OrderMasterSignatories.Where(o => o.IsActive)
                                                                                                .Any(y => y.SalesOrderSignatoryId == x.SalesOrderSignatoryId
                                                                                                      && y.Precedence < a.Precedence
                                                                                                      && y.IsActive))
                                                                              }).ToListAsync();

            // Now, sort in memory
            vmSalesOrderSlave.SignatoryApprovalList = approvalList.OrderBy(a => a.Precedence).ToList();









            return vmSalesOrderSlave;
        }

        public async Task<VMSalesOrderSlave> GcclProcurementSalesOrderDetailsGet(int companyId, int orderMasterId)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            vmSalesOrderSlave = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId && x.CompanyId == companyId)
                                                      join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                      join t4 in _db.SubZones on t2.SubZoneId equals t4.SubZoneId
                                                      join t5 in _db.Zones on t4.ZoneId equals t5.ZoneId
                                                      join t6 in _db.StockInfoes on t1.StockInfoId equals t6.StockInfoId into t6_Join
                                                      from t6 in t6_Join.DefaultIfEmpty()
                                                      join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                                                      join t7 in _db.Demands on t1.DemandId equals t7.DemandId into t7_join
                                                      from t7 in t7_join.DefaultIfEmpty()
                                                      join t8 in _db.Employees on t1.SalePersonId equals t8.Id into t8_Join
                                                      from t8 in t8_Join.DefaultIfEmpty()
                                                      select new VMSalesOrderSlave
                                                      {
                                                          WareHouse = t6 != null ? t6.Name : "",
                                                          DemandNo = t7 == null ? "" : t7.DemandNo,
                                                          Propietor = t2.Propietor,
                                                          CreatedDate = t1.CreateDate,
                                                          ComLogo = t3.CompanyLogo,
                                                          CustomerPhone = t2.Phone,
                                                          CustomerAddress = t2.Address,
                                                          CustomerEmail = t2.Email,
                                                          ContactPerson = t2.ContactName,
                                                          CompanyFK = t1.CompanyId,
                                                          OrderMasterId = t1.OrderMasterId,
                                                          CreditLimit = t2.CreditLimit,
                                                          OrderNo = t1.OrderNo,
                                                          Status = t1.Status,
                                                          OrderDate = t1.OrderDate,
                                                          CreatedBy = t1.CreatedBy,
                                                          CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                          ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                          CommonCustomerName = t2.Name,
                                                          CompanyName = t3.Name,
                                                          CompanyAddress = t3.Address,
                                                          CompanyEmail = t3.Email,
                                                          CompanyPhone = t3.Phone,
                                                          ZoneName = t5.Name,
                                                          ZoneIncharge = t5.ZoneIncharge,
                                                          SubZonesName = t4.Name,
                                                          SubZoneIncharge = t4.SalesOfficerName,
                                                          SubZoneInchargeMobile = t4.MobileOffice,
                                                          CommonCustomerCode = t2.Code,
                                                          CustomerTypeFk = t2.CustomerTypeFK,
                                                          CustomerId = t2.VendorId,
                                                          CourierCharge = t1.CourierCharge,
                                                          FinalDestination = t1.FinalDestination,
                                                          CourierNo = t1.CourierNo,
                                                          DiscountAmount = t1.DiscountAmount ?? 0,
                                                          DiscountRate = t1.DiscountRate ?? 0,
                                                          TotalAmountAfterDiscount = t1.TotalAmount ?? 0,
                                                          OfficerNAme = t8 != null ? t8.Name : ""



                                                      }).FirstOrDefault());

            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                                    join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                    join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                    join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                    join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                    select new VMSalesOrderSlave
                                                                    {
                                                                        ProductName = t4.Name + " " + t3.ProductName,
                                                                        ProductCategoryName = t5.Name,
                                                                        OrderMasterId = t1.OrderMasterId.Value,
                                                                        OrderDetailId = t1.OrderDetailId,
                                                                        Qty = t1.Qty,
                                                                        UnitPrice = t1.UnitPrice,
                                                                        UnitName = t6.Name,
                                                                        TotalAmount = t1.Amount,
                                                                        PackQuantity = t1.PackQuantity,
                                                                        Consumption = t1.Comsumption,
                                                                        PromotionalOfferId = t1.PromotionalOfferId,
                                                                        ProductCategoryId = t5.ProductCategoryId,
                                                                        ProductSubCategoryId = t4.ProductSubCategoryId,
                                                                        FProductId = t3.ProductId,
                                                                        ProductDiscountUnit = t1.DiscountUnit,//Unit Discount                                                               
                                                                        CashDiscountPercent = t1.DiscountRate, // Cash Discount                                                               
                                                                        SpecialDiscount = t1.SpecialBaseCommission, // SpecialDiscount   
                                                                    }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());


            return vmSalesOrderSlave;
        }

        public async Task<VMSalesOrderSlave> KfmalProcurementSalesOrderDetailsGet(int companyId, long orderMasterId)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            vmSalesOrderSlave = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId && x.CompanyId == companyId)
                                                      join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                      join t4 in _db.SubZones on t2.SubZoneId equals t4.SubZoneId
                                                      join t5 in _db.Zones on t4.ZoneId equals t5.ZoneId
                                                      join t6 in _db.StockInfoes on t1.StockInfoId equals t6.StockInfoId into t6_Join
                                                      from t6 in t6_Join.DefaultIfEmpty()
                                                      join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                                                      join t7 in _db.Demands on t1.DemandId equals t7.DemandId into t7_join
                                                      from t7 in t7_join.DefaultIfEmpty()
                                                      join t8 in _db.Employees on t1.SalePersonId equals t8.Id into t8_Join
                                                      from t8 in t8_Join.DefaultIfEmpty()
                                                      select new VMSalesOrderSlave
                                                      {
                                                          AccountingHeadId = t2.HeadGLId,
                                                          WareHouse = t6 != null ? t6.Name : "",
                                                          DemandNo = t7 == null ? "" : t7.DemandNo,
                                                          Propietor = t2.Propietor,
                                                          CreatedDate = t1.CreateDate,
                                                          ComLogo = t3.CompanyLogo,
                                                          CustomerPhone = t2.Phone,
                                                          CustomerAddress = t2.Address,
                                                          CustomerEmail = t2.Email,
                                                          ContactPerson = t2.ContactName,
                                                          CompanyFK = t1.CompanyId,
                                                          CompanyId = (int)t1.CompanyId,
                                                          OrderMasterId = t1.OrderMasterId,
                                                          CreditLimit = t2.CreditLimit,
                                                          OrderNo = t1.OrderNo,
                                                          Status = t1.Status,
                                                          OrderDate = t1.OrderDate,
                                                          CreatedBy = t1.CreatedBy,
                                                          CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                          ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                          CommonCustomerName = t2.Name,
                                                          CompanyName = t3.Name,
                                                          CompanyAddress = t3.Address,
                                                          CompanyEmail = t3.Email,
                                                          CompanyPhone = t3.Phone,
                                                          ZoneName = t5.Name,
                                                          ZoneIncharge = t5.ZoneIncharge,
                                                          SubZonesName = t4.Name,
                                                          SubZoneIncharge = t4.SalesOfficerName,
                                                          SubZoneInchargeMobile = t4.MobileOffice,
                                                          CommonCustomerCode = t2.Code,
                                                          CustomerTypeFk = t2.CustomerTypeFK,
                                                          CustomerId = t2.VendorId,
                                                          CourierCharge = t1.CourierCharge,
                                                          FinalDestination = t1.FinalDestination,
                                                          CourierNo = t1.CourierNo,
                                                          DiscountAmount = t1.DiscountAmount ?? 0,
                                                          DiscountRate = t1.DiscountRate ?? 0,
                                                          TotalAmountAfterDiscount = t1.TotalAmount ?? 0,
                                                          OfficerNAme = t8 != null ? t8.Name : "",
                                                          IntegratedFrom = "OrderMaster"



                                                      }).FirstOrDefault());

            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                                    join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                    join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                    join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                    join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                    select new VMSalesOrderSlave
                                                                    {
                                                                        ProductName = t4.Name + " " + t3.ProductName,
                                                                        ProductCategoryName = t5.Name,
                                                                        OrderMasterId = t1.OrderMasterId.Value,
                                                                        OrderDetailId = t1.OrderDetailId,
                                                                        Qty = t1.Qty,
                                                                        UnitPrice = t1.UnitPrice,
                                                                        UnitName = t6.Name,
                                                                        TotalAmount = t1.Amount,
                                                                        PackQuantity = t1.PackQuantity,
                                                                        Consumption = t1.Comsumption,
                                                                        PromotionalOfferId = t1.PromotionalOfferId,
                                                                        ProductCategoryId = t5.ProductCategoryId,
                                                                        ProductSubCategoryId = t4.ProductSubCategoryId,
                                                                        FProductId = t3.ProductId,
                                                                        AccountingIncomeHeadId = t3.AccountingIncomeHeadId,

                                                                        ProductDiscountUnit = t1.DiscountUnit,//Unit Discount                                                               
                                                                        CashDiscountPercent = t1.DiscountRate, // Cash Discount                                                               
                                                                        SpecialDiscount = t1.SpecialBaseCommission, // SpecialDiscount   
                                                                    }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());


            return vmSalesOrderSlave;
        }
        public async Task<VMSalesOrderSlave> ProcurementSalesOrderOpeningDetailsGet(int companyId)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            vmSalesOrderSlave.CompanyFK = companyId;
            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t0 in _db.OrderDetails.Where(x => x.IsActive)
                                                                    join t1 in _db.OrderMasters.Where(x => x.IsActive && x.IsOpening) on t0.OrderMasterId equals t1.OrderMasterId
                                                                    join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                                    join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                                    select new VMSalesOrderSlave
                                                                    {
                                                                        OrderMasterId = t0.OrderMasterId.Value,
                                                                        OrderNo = t1.OrderNo,
                                                                        OrderDate = t1.OrderDate,
                                                                        CommonCustomerName = t2.Name,
                                                                        Qty = t0.Qty,
                                                                        UnitPrice = t0.UnitPrice,
                                                                        TotalAmount = t0.Amount,
                                                                        Code = t2.Code,
                                                                        Remarks = t1.Remarks
                                                                    }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());




            return vmSalesOrderSlave;
        }
        public async Task<VMSalesOrder> CustomerRecevableAmountByCustomerGet(int companyId, int customerId)
        {
            if (customerId == 0) { VMSalesOrder vmOrderMaster2 = new VMSalesOrder(); return vmOrderMaster2; }
            VMSalesOrder vmOrderMaster = new VMSalesOrder();
            vmOrderMaster = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.VendorId == customerId && x.IsActive && x.CompanyId == companyId)
                                                  select new VMSalesOrder
                                                  {
                                                      CustomerAddress = t1.Address,
                                                      CommonCustomerName = t1.Name,
                                                      CommonCustomerCode = t1.Code,
                                                      CompanyFK = t1.CompanyId,
                                                      CustomerId = t1.VendorId,
                                                      CreditLimit = t1.CreditLimit,
                                                      CustomerTypeFk = t1.CustomerTypeFK,
                                                      PayableAmount = (from ts1 in _db.OrderDeliverDetails
                                                                       join ts2 in _db.OrderDetails on ts1.OrderDetailId equals ts2.OrderDetailId
                                                                       join ts3 in _db.OrderMasters.Where(x => x.CustomerId == t1.VendorId) on ts2.OrderMasterId equals ts3.OrderMasterId
                                                                       where ts2.IsActive && ts1.IsActive
                                                                       group new { ts1, ts2, ts3 } by new { ts3.OrderMasterId } into Group
                                                                       select Group.Sum(x => x.ts1.DeliveredQty * x.ts2.UnitPrice) + Group.FirstOrDefault().ts3.CourierCharge).DefaultIfEmpty(0).Sum(),

                                                      ReturnAmount = (from ts1 in _db.SaleReturnDetails
                                                                      join ts2 in _db.SaleReturns.Where(x => x.CustomerId == t1.VendorId && x.CompanyId == t1.CompanyId) on ts1.SaleReturnId equals ts2.SaleReturnId
                                                                      where ts1.IsActive && ts2.IsActive
                                                                      select ts1.Qty.Value * ts1.Rate.Value).DefaultIfEmpty(0).Sum(),

                                                      InAmount = _db.Payments.Where(x => x.VendorId == customerId)
                                                                       .Select(x => x.InAmount).DefaultIfEmpty(0).Sum()
                                                  }).FirstOrDefault());




            return vmOrderMaster;
        }


        public async Task<VMCommonSupplier> FeedCustomerDetailsById(int customerId)
        {
            VMCommonSupplier vmOrderMaster = new VMCommonSupplier();
            vmOrderMaster = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.VendorId == customerId && x.IsActive)
                                                  join t2 in _db.Employees on t1.SalesOfficerEmpId equals t2.Id into x
                                                  from t2 in x.DefaultIfEmpty()
                                                  select new VMCommonSupplier
                                                  {
                                                      Address = t1.Address,
                                                      Name = t1.Name,
                                                      Code = t1.Code,
                                                      CompanyFK = t1.CompanyId,
                                                      ID = t1.VendorId,
                                                      CreditLimit = t1.CreditLimit,
                                                      CustomerTypeFk = t1.CustomerTypeFK,
                                                      CashCommissionCattle = t1.CashCommissionCattle,
                                                      SalesOfficerEmpId = t2 != null ? t1.SalesOfficerEmpId : 0,
                                                      EmployeeName = t2 != null ? t2.Name : "",
                                                  }).FirstOrDefault());

            return vmOrderMaster;
        }
        public async Task<VMSalesOrderSlave> GetSingleOrderDetails(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.OrderDetails
                                          join t2 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t2.ProductId
                                          join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t2.ProductSubCategoryId equals t4.ProductSubCategoryId
                                          join t3 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t3.ProductCategoryId
                                          join t5 in _db.Units on t2.UnitId equals t5.UnitId
                                          join t7 in _db.OrderMasters on t1.OrderMasterId equals t7.OrderMasterId
                                          join t6 in _db.VendorOffers on t2.ProductId equals t6.ProductId into LJVendorOffer
                                          from t6 in LJVendorOffer.DefaultIfEmpty()
                                          where t1.OrderDetailId == id || t7.CustomerId.Value == t6.VendorId
                                          select new VMSalesOrderSlave
                                          {
                                              ProductName = t2.ProductName,
                                              OrderMasterId = t1.OrderMasterId.Value,
                                              OrderDetailId = t1.OrderDetailId,
                                              Qty = t1.Qty,
                                              UnitPrice = t1.UnitPrice,
                                              TotalAmount = t1.Amount,
                                              UnitName = t5.ShortName,
                                              FProductId = t1.ProductId,
                                              PackQuantity = t1.PackQuantity,
                                              Consumption = t1.Comsumption,
                                              CompanyFK = t1.CompanyId,
                                              ProductCategoryName = t3.Name,
                                              ProductSubCategoryName = t4.Name,
                                              SpecialDiscount = t1.SpecialDiscount,
                                              SpecialBaseCommission = t1.SpecialBaseCommission,
                                              DiscountUnit = t1.DiscountUnit,
                                              DiscountAmount = t1.DiscountAmount,
                                              BaseCommission = t6.BaseCommission ?? 0,
                                              CashCommission = t6.CashCommission ?? 0,
                                              AdditionPrice = t6.AdditionPrice ?? 0,
                                              MonthlyIncentive = t6.MonthlyIncentive ?? 0,
                                              YearlyIncentive = t6.YearlyIncentive ?? 0,
                                              CarryingCommission = t6.CarryingCommission ?? 0,
                                              ProductId = t2.ProductId
                                          }).FirstOrDefault());

            return v;
        }

        public async Task<VMSalesOrderSlave> GetKPLSingleOrderDetails(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.OrderDetails
                                          join t2 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t2.ProductId

                                          join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t2.ProductSubCategoryId equals t4.ProductSubCategoryId
                                          join t3 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t3.ProductCategoryId
                                          join t5 in _db.Units on t2.UnitId equals t5.UnitId
                                          join t7 in _db.OrderMasters on t1.OrderMasterId equals t7.OrderMasterId

                                          where t1.OrderDetailId == id
                                          select new VMSalesOrderSlave
                                          {
                                              Description = t1.Remarks,
                                              ProductName = t2.ProductName,
                                              OrderMasterId = t1.OrderMasterId.Value,
                                              OrderDetailId = t1.OrderDetailId,
                                              Qty = t1.Qty,
                                              UnitPrice = t1.UnitPrice,
                                              TotalAmount = t1.Amount,
                                              UnitName = t5.ShortName,
                                              FProductId = t1.ProductId,
                                              PackQuantity = t1.PackQuantity,
                                              Consumption = t1.Comsumption,
                                              CompanyFK = t1.CompanyId,
                                              ProductCategoryName = t3.Name,
                                              ProductSubCategoryName = t4.Name,
                                              SpecialDiscount = t1.SpecialDiscount,
                                              ProductId = t2.ProductId,
                                              IsVATInclude = t1.IsVATInclude,
                                              VATPercent = t1.VATPercent,
                                              TDSPercent = t1.TDSPercent,
                                              ReelDirection = t1.ReelDirection,
                                              PouchDerection = t1.PouchDerection,
                                              JobOrderDate = t1.OrderDate
                                          }).FirstOrDefault());
            return v;
        }
        public async Task<VMSalesOrderSlave> GetKfmalOrderDetails(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.OrderDetails
                                          join t2 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t2.ProductId

                                          join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t2.ProductSubCategoryId equals t4.ProductSubCategoryId
                                          join t3 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t3.ProductCategoryId
                                          join t5 in _db.Units on t2.UnitId equals t5.UnitId
                                          join t7 in _db.OrderMasters on t1.OrderMasterId equals t7.OrderMasterId
                                          join t6 in _db.VendorOffers on t2.ProductId equals t6.ProductId into t6_Join
                                          from t6 in t6_Join.DefaultIfEmpty()
                                          where t1.OrderDetailId == id// && t6.VendorId == t7.CustomerId
                                          select new VMSalesOrderSlave
                                          {
                                              ProductName = t2.ProductName,
                                              OrderMasterId = t1.OrderMasterId.Value,
                                              OrderDetailId = t1.OrderDetailId,
                                              Qty = t1.Qty,
                                              UnitPrice = t1.UnitPrice,
                                              PackQuantity = t1.PackQuantity,

                                              ProductDiscountUnit = t1.DiscountUnit,
                                              CashDiscountPercent = t1.DiscountRate,
                                              SpecialDiscount = t1.SpecialBaseCommission,

                                              TotalAmount = t1.Amount,
                                              UnitName = t5.ShortName,
                                              FProductId = t1.ProductId,

                                              Consumption = t1.Comsumption,
                                              CompanyFK = t1.CompanyId,
                                              ProductCategoryName = t3.Name,
                                              ProductSubCategoryName = t4.Name,

                                              ProductId = t2.ProductId
                                          }).FirstOrDefault());
            return v;
        }
        public VMCommonProduct ProductStockByProductGet(int companyId, int productId)
        {
            VMCommonProduct vmProduct = new VMCommonProduct();

            vmProduct = (from t1 in _db.Products.Where(x => x.IsActive && x.ProductId == productId)
                         join t2 in _db.Units on t1.UnitId equals t2.UnitId

                         select new VMCommonProduct
                         {
                             Name = t1.ProductName,
                             UnitName = t2.Name,
                             PackSize = t1.PackSize,
                             CompanyFK = t1.CompanyId,
                             FormulaQty = t1.FormulaQty,
                             UnitPrice = t1.UnitPrice ?? 0

                         }).FirstOrDefault();

            VMProductStock vmProductStock = new VMProductStock();
            vmProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC SeedFinishedGoodsStockByProduct {0},{1}", productId, companyId).FirstOrDefault();
            vmProduct.CurrentStock = vmProductStock.ClosingQty;


            return vmProduct;
        }


        public VMCommonProduct ProductStockByProductGetOrderDeliver(int companyId, int productId, string Lotnumber)
        {
            VMCommonProduct vmProduct = new VMCommonProduct();

            vmProduct = (from t1 in _db.Products.Where(x => x.IsActive && x.ProductId == productId)
                         join t2 in _db.Units on t1.UnitId equals t2.UnitId

                         select new VMCommonProduct
                         {
                             Name = t1.ProductName,
                             UnitName = t2.Name,
                             PackSize = t1.PackSize,
                             CompanyFK = t1.CompanyId,
                             FormulaQty = t1.FormulaQty,
                             UnitPrice = t1.UnitPrice ?? 0,
                             ProductType=t1.ProductType


                         }).FirstOrDefault();
            VMProductStock vmProductStock = new VMProductStock();
            if (vmProduct.ProductType == "F")
            {
         
                vmProductStock = _db.Database.SqlQuery<VMProductStock>(
                   "EXEC SeedFinishedGoodsStockByProductForDeliver {0}, {1}, {2}",
                   productId,
                   companyId,
                   string.IsNullOrEmpty(Lotnumber) ? "xyz" : Lotnumber
               ).FirstOrDefault();
            }
            else
            {
                vmProductStock = _db.Database.SqlQuery<VMProductStock>(
               "EXEC GetSeedRMStockByProductId {0}, {1}, {2}",
               productId,
               companyId,
               string.IsNullOrEmpty(Lotnumber) ? "xyz" : Lotnumber
           ).FirstOrDefault();

            }


          
            vmProduct.CurrentStock = vmProductStock.ClosingQty;


            return vmProduct;
        }

        public VMCommonProduct RMProductStockByProductGet(int companyId, int productId)
        {
            VMCommonProduct vmProduct = new VMCommonProduct();

            vmProduct = (from t1 in _db.Products.Where(x => x.IsActive && x.ProductId == productId)
                         join t2 in _db.Units on t1.UnitId equals t2.UnitId

                         select new VMCommonProduct
                         {
                             Name = t1.ProductName,
                             UnitName = t2.Name,
                             PackSize = t1.PackSize,
                             CompanyFK = t1.CompanyId,
                             FormulaQty = t1.FormulaQty,
                             UnitPrice = t1.UnitPrice ?? 0

                         }).FirstOrDefault();

            VMProductStock vmProductStock = new VMProductStock();
            vmProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetPackagingRMStockByProductId {0},{1}", productId, companyId).FirstOrDefault();
            vmProduct.CurrentStock = vmProductStock.ClosingQty;


            return vmProduct;
        }
        public async Task<long> OrderDetailAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long dateTime = DateTime.Now.Ticks;
            long result = -1;
            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                ProductId = vmSalesOrderSlave.FProductId,
                Qty = vmSalesOrderSlave.Qty,
                UnitPrice = vmSalesOrderSlave.UnitPrice,
                Amount = (vmSalesOrderSlave.Qty * vmSalesOrderSlave.UnitPrice),
                Comsumption = vmSalesOrderSlave.Consumption,
                PackQuantity = vmSalesOrderSlave.PackSize,

                DiscountUnit = vmSalesOrderSlave.ProductDiscountUnit,
                SpecialBaseCommission = vmSalesOrderSlave.SpecialDiscount,



                DiscountAmount = 0,
                DiscountRate = 0,

                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreateDate = DateTime.Now,
                StyleNo = Convert.ToString(dateTime),
                IsActive = true,
                //LotNumber=vmSalesOrderSlave.LotNumber
            };
            _db.OrderDetails.Add(orderDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderDetail.OrderDetailId;
            }
            //long order = await GCCLOrderMastersDiscountEdit(vmSalesOrderSlave);
            return result;
        }

        public async Task<long> OrderDetailRawAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long dateTime = DateTime.Now.Ticks;
            long result = -1;
            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                ProductId = vmSalesOrderSlave.FProductId,
                Qty = vmSalesOrderSlave.Qty,
                UnitPrice = vmSalesOrderSlave.UnitPrice,
                Amount = (vmSalesOrderSlave.Qty * vmSalesOrderSlave.UnitPrice),
                Comsumption = vmSalesOrderSlave.Consumption,
                PackQuantity = vmSalesOrderSlave.PackSize,

                DiscountUnit = vmSalesOrderSlave.ProductDiscountUnit,
                SpecialBaseCommission = vmSalesOrderSlave.SpecialDiscount,



                DiscountAmount = 0,
                DiscountRate = 0,

                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreateDate = DateTime.Now,
                StyleNo = Convert.ToString(dateTime),
                IsActive = true,
                //LotNumber=vmSalesOrderSlave.LotNumber
            };
            _db.OrderDetails.Add(orderDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderDetail.OrderDetailId;
            }
            //long order = await GCCLOrderMastersDiscountEdit(vmSalesOrderSlave);
            return result;
        }


        public async Task<long> FeedOrderDetailAdd(FeedSalesOrderModel model)
        {
            long dateTime = DateTime.Now.Ticks;
            long result = -1;
            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = model.OrderMasterId,
                ProductId = model.FProductId,
                Qty = model.Qty,
                UnitPrice = model.UnitPrice,
                Amount = (model.Qty * model.UnitPrice),
                EBaseCommission = model.BaseCommission,
                ECashCommission = model.CashCommission,
                ECarryingCommission = model.CarryingCommission,
                SpecialDiscount = model.SpecialDiscount,
                MonthlyIncentive = model.MonthlyIncentiveInInvoice,
                YearlyIncentive = model.YearlyIncentiveInInvoice,

                AdditionPrice = model.AdditionalPrice,

                Comsumption = 0,
                PackQuantity = 0,
                DiscountUnit = 0,
                DiscountAmount = 0,
                DiscountRate = 0,
                SpecialBaseCommission = 0,

                CompanyId = model.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreateDate = DateTime.Now,
                StyleNo = Convert.ToString(dateTime),
                IsActive = true
            };
            _db.OrderDetails.Add(orderDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderDetail.OrderDetailId;
            }
            //long order = await GCCLOrderMastersDiscountEdit(vmSalesOrderSlave);
            return result;
        }
        public async Task<long> GcclOrderDetailAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long dateTime = DateTime.Now.Ticks;
            long result = -1;
            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                ProductId = vmSalesOrderSlave.FProductId,
                Qty = vmSalesOrderSlave.Qty,
                UnitPrice = vmSalesOrderSlave.UnitPrice,
                Amount = (vmSalesOrderSlave.Qty * vmSalesOrderSlave.UnitPrice),
                Comsumption = vmSalesOrderSlave.Consumption,
                PackQuantity = vmSalesOrderSlave.PackQuantity,

                DiscountUnit = vmSalesOrderSlave.ProductDiscountUnit,
                DiscountRate = vmSalesOrderSlave.CashDiscountPercent,
                SpecialBaseCommission = vmSalesOrderSlave.SpecialDiscount,


                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreateDate = DateTime.Now,
                StyleNo = Convert.ToString(dateTime),
                IsActive = true
            };
            _db.OrderDetails.Add(orderDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderDetail.OrderDetailId;
            }
            //long order = await GCCLOrderMastersDiscountEdit(vmSalesOrderSlave);
            return result;
        }

        public async Task<long> PackagingOrderDetailAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long dateTime = DateTime.Now.Ticks;
            long result = -1;

            var firstDayOfMonth = new DateTime(vmSalesOrderSlave.JobOrderDate.Value.Year, vmSalesOrderSlave.JobOrderDate.Value.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Fetch order details for the current month.  
            var totalOrderThisMonth = _db.OrderDetails
                .Where(od => od.OrderDate >= firstDayOfMonth && od.OrderDate <= lastDayOfMonth)
                .OrderByDescending(od => od.OrderDetailId); // Ensure it's ordered   

            // Use FirstOrDefault after ordering to get the last object  
            var lastObjects = await totalOrderThisMonth.FirstOrDefaultAsync();
            DateTime date = vmSalesOrderSlave.JobOrderDate.Value;

            string month = date.ToString("MM");
            string yearLastTwoDigits = date.ToString("yy");

            string dat = month + yearLastTwoDigits;

            // Start with a default job order  
            var newJobOrder = $"JOB-{dat}-0001"; // Default to 4-digit format with zeros  

            if (lastObjects != null)
            {
                string lastJobOrderNo = lastObjects.JobOrderNo;

                // Split the JobOrderNo by '-'  
                string[] parts = lastJobOrderNo.Split('-');

                if (parts.Length == 3 && parts[0] == "JOB")
                {
                    // Extract and increment the last part  
                    if (int.TryParse(parts[2], out int orderNumber))
                    {
                        // Increment the order number  
                        orderNumber++;

                        // Format the new JobOrderNo  
                        newJobOrder = $"JOB-{dat}-{orderNumber:D4}";
                    }
                }
            }





            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                ProductId = vmSalesOrderSlave.FProductId,
                Qty = vmSalesOrderSlave.Qty,
                UnitPrice = vmSalesOrderSlave.UnitPrice,
                Amount = (vmSalesOrderSlave.Qty * vmSalesOrderSlave.UnitPrice),
                Comsumption = vmSalesOrderSlave.Consumption,
                PackQuantity = vmSalesOrderSlave.PackQuantity,
                Remarks = vmSalesOrderSlave.Description,
                DiscountUnit = vmSalesOrderSlave.ProductDiscountUnit,
                DiscountRate = vmSalesOrderSlave.CashDiscountPercent,
                SpecialBaseCommission = vmSalesOrderSlave.SpecialDiscount,
                VATPercent = vmSalesOrderSlave.VATPercent,
                TDSPercent = vmSalesOrderSlave.TDSPercent,
                IsVATInclude = vmSalesOrderSlave.IsVATInclude,
                OrderDate = vmSalesOrderSlave.JobOrderDate,
                ReelDirection = vmSalesOrderSlave.ReelDirection,
                PouchDerection = vmSalesOrderSlave.PouchDerection,
                Status = (int)EnumPOStatus.Draft,


                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreateDate = DateTime.Now,
                StyleNo = Convert.ToString(dateTime),
                JobOrderNo = newJobOrder,
                IsActive = true
            };
            _db.OrderDetails.Add(orderDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderDetail.OrderDetailId;
            }
            //long order = await GCCLOrderMastersDiscountEdit(vmSalesOrderSlave);
            return result;
        }

        public async Task<long> PromtionalOfferIntegration(VMSalesOrderSlave vmSalesOrderSlave)
        {
            var offers = _db.PromtionalOfferDetails
                .Where(x => x.IsActive && x.PromtionalOfferId == vmSalesOrderSlave.PromotionalOfferId).ToList();
            long result = -1;
            var offer = await _db.PromtionalOffers
                .SingleOrDefaultAsync(s => s.PromtionalOfferId == vmSalesOrderSlave.PromotionalOfferId);

            if (offer.PromtionType == 1)
            {
                List<OrderDetail> OrderDetailList = new List<OrderDetail>();
                foreach (var item in offers)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        PromotionalOfferId = vmSalesOrderSlave.PromotionalOfferId,
                        OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                        ProductId = item.ProductId,
                        Qty = Convert.ToDouble(item.PromoQuantity),
                        UnitPrice = 0,
                        Amount = 0,
                        Comsumption = 0,
                        PackQuantity = 0,
                        DiscountUnit = 0,
                        DiscountAmount = 0,
                        DiscountRate = 0,

                        CompanyId = vmSalesOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreateDate = DateTime.Now,

                        IsActive = true
                    };
                    OrderDetailList.Add(orderDetail);
                }

                _db.OrderDetails.AddRange(OrderDetailList);
            }
            else if (offer.PromtionType == 2)
            {
                var order = await _db.OrderMasters
                    .SingleOrDefaultAsync(s => s.OrderMasterId == vmSalesOrderSlave.OrderMasterId);
                if (order == null)
                {

                }
                else
                {
                    order.DiscountAmount = order.DiscountAmount + offers.FirstOrDefault().PromoAmount;

                }
            }

            if (await _db.SaveChangesAsync() > 0)
            {
                result = 1;
            }

            return result;
        }
        public async Task<long> OrderDetailOpeningAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long result = -1;
            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                ProductId = 1445,
                Qty = 1,
                UnitPrice = vmSalesOrderSlave.UnitPrice,
                Amount = vmSalesOrderSlave.UnitPrice,

                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = vmSalesOrderSlave.OrderDate,
                IsActive = true
            };
            _db.OrderDetails.Add(orderDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderDetail.OrderDetailId;
            }

            return result;
        }
        public async Task<int> OrderDetailEdit(VMSalesOrderSlave vmSalesOrderSlave)
        {
            var result = -1;
            OrderDetail model = await _db.OrderDetails.FindAsync(vmSalesOrderSlave.OrderDetailId);

            model.ProductId = vmSalesOrderSlave.FProductId;
            model.Qty = vmSalesOrderSlave.Qty;
            model.UnitPrice = vmSalesOrderSlave.UnitPrice;
            model.Amount = (vmSalesOrderSlave.Qty * vmSalesOrderSlave.UnitPrice);
            model.Comsumption = vmSalesOrderSlave.Consumption;
            model.PackQuantity = vmSalesOrderSlave.PackSize;
            model.DiscountUnit = vmSalesOrderSlave.ProductDiscountUnit;
            model.SpecialBaseCommission = vmSalesOrderSlave.SpecialDiscount;



            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmSalesOrderSlave.ID;
            }

            return result;
        }

        public async Task<int> PackagingOrderDetailEdit(VMSalesOrderSlave vmSalesOrderSlave)
        {
            var result = -1;
            OrderDetail model = await _db.OrderDetails.FindAsync(vmSalesOrderSlave.OrderDetailId);

            model.ProductId = vmSalesOrderSlave.FProductId;
            model.Qty = vmSalesOrderSlave.Qty;
            model.UnitPrice = vmSalesOrderSlave.UnitPrice;
            model.Amount = (vmSalesOrderSlave.Qty * vmSalesOrderSlave.UnitPrice);
            model.Comsumption = vmSalesOrderSlave.Consumption;
            model.PackQuantity = vmSalesOrderSlave.PackQuantity;

            model.DiscountUnit = vmSalesOrderSlave.ProductDiscountUnit;
            model.DiscountRate = vmSalesOrderSlave.CashDiscountPercent;
            model.SpecialBaseCommission = vmSalesOrderSlave.SpecialDiscount;
            model.VATPercent = vmSalesOrderSlave.VATPercent;
            model.TDSPercent = vmSalesOrderSlave.TDSPercent;
            model.IsVATInclude = vmSalesOrderSlave.IsVATInclude;
            model.OrderDate = vmSalesOrderSlave.JobOrderDate;
            model.ReelDirection = vmSalesOrderSlave.ReelDirection;
            model.PouchDerection = vmSalesOrderSlave.PouchDerection;
            model.Remarks = vmSalesOrderSlave.Description;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmSalesOrderSlave.ID;
            }

            return result;
        }

        public async Task<int> UpdatePackagingSalesOrderSlave(VMSalesOrder vMSalesOrder)
        {
            int result = 0;
            if (vMSalesOrder.OrderMasterId > 0)
            {
                OrderMaster orderMaster = await _db.OrderMasters.FirstAsync(x => x.OrderMasterId == vMSalesOrder.OrderMasterId);
                if (orderMaster != null)
                {
                    orderMaster.CustomerPONo = vMSalesOrder.CustomerPONo;
                    orderMaster.OrderDate = vMSalesOrder.OrderDate;
                    orderMaster.PaymentMethod = vMSalesOrder.CustomerPaymentMethodEnumFK;
                    orderMaster.CustomerId = vMSalesOrder.CustomerId;
                    orderMaster.StockInfoId = vMSalesOrder.StockInfoId;
                    orderMaster.FinalDestination = vMSalesOrder.FinalDestination;
                    orderMaster.ExpectedDeliveryDate = vMSalesOrder.ExpectedDeliveryDate;
                    orderMaster.CompanyId = vMSalesOrder.CompanyFK;
                    orderMaster.Remarks = vMSalesOrder.Remarks;
                    orderMaster.ModifiedBy = Common.GetUserId();
                    vMSalesOrder.ModifiedDate = DateTime.Now;
                }
                _db.Entry(orderMaster).State = EntityState.Modified;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = orderMaster.CompanyId.Value;
                }
            }
            return result;
        }


        public async Task<int> OrderMasterAmountEdit(VMSalesOrderSlave vmSalesOrderSlave)
        {
            var result = -1;
            OrderMaster model = await _db.OrderMasters.FindAsync(vmSalesOrderSlave.OrderMasterId);

            model.TotalAmount = vmSalesOrderSlave.TotalAmountAfterDiscount;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmSalesOrderSlave.ID;
            }

            return result;
        }
        public async Task<long> OrderDetailDelete(long id)
        {
            long result = -1;
            OrderDetail orderDetail = await _db.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                orderDetail.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = orderDetail.OrderDetailId;
                }
            }
            return result;
        }

        #endregion

        //JAKg3847


        public List<object> CustomerLisByCompany(int companyId = 0)
        {
            var List = new List<object>();

            _db.Vendors
         .Where(x => x.IsActive && x.CompanyId == companyId && x.VendorTypeId == (int)Provider.Customer).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.VendorId,
            Text = x.Name+"-"+x.Address
        }));
            return List;

        }

        public List<SelectModelType> CustomerLisByCompanyFeed(int companyId = 0)
        {
            var List = new List<SelectModelType>();

            List = (
                from t1 in _db.Vendors
                join t2 in _db.Zones on t1.ZoneId equals t2.ZoneId
                where t1.CompanyId == companyId && t1.VendorTypeId == (int)Provider.Customer
                select new SelectModelType
                {
                    Value = t1.VendorId,
                    Text = t1.Name
                }).ToList();

            return List;
        }

        public List<SelectModelType> feedPayType(int companyId = 0)
        {
            var List = new List<SelectModelType>();
            List = (from t1 in _db.HeadGLs
                    join t2 in _db.Head5 on t1.ParentId equals t2.Id
                    join t3 in _db.Head4 on t2.ParentId equals t3.Id
                    where t3.AccCode == "1301001" && t1.CompanyId == 8
                    select new SelectModelType
                    {
                        Value = t1.Id,
                        Text = t1.AccCode + "(" + t1.AccName + ")"
                    }
                       ).ToList();
            return List;
        }



        //public List<object> ApprovedPOList()
        //{
        //    var List = new List<object>();
        //    _db.Procurement_PurchaseOrder.Where(x => x.Active && x.Status == (int)EnumPOStatus.Submitted).Select(x => x).ToList() // x.ProcurementOriginTypeEnumFK == procurementOriginTypeEnumFK &&
        //   .ForEach(x => List.Add(new
        //   {
        //       Value = x.ID,
        //       Text = x.CID + " Date: " + x.OrderDate.ToLongDateString()
        //   }));
        //    return List;

        //}

        //public async Task<List<VMPurchaseOrder>> ProcurementPurchaseOrderGet(int commonSupplierFK)
        //{
        //    var x = await Task.Run(() => (from t1 in _db.WareHouse_POReceiving
        //                                  join t2 in _db.Procurement_PurchaseOrder on t1.Procurement_PurchaseOrderFk equals t2.ID
        //                                  where t1.Active && t2.Common_SupplierFK == commonSupplierFK
        //                                  group new { t1, t2 } by new { t2.CID, t2.ID } into Group
        //                                  select new VMPurchaseOrder
        //                                  {
        //                                      CID = Group.Key.CID + " Order Date: " + Group.First().t2.OrderDate.ToLongDateString(),
        //                                      ID = Group.Key.ID
        //                                  }).ToListAsync());
        //    return x;

        //}
        public object GetAutoCompletePO(string prefix)
        {
            var v = (from t1 in _db.PurchaseOrders
                     join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                     where t1.IsActive && t1.Status == (int)EnumPOStatus.Submitted && ((t1.PurchaseOrderNo.StartsWith(prefix)) || (t2.Name.StartsWith(prefix)) || (t1.PurchaseDate.ToString().StartsWith(prefix)))

                     select new
                     {
                         label = t1.PurchaseOrderNo + " Date " + t1.PurchaseDate.Value.ToLongDateString(),
                         val = t1.PurchaseOrderId
                     }).OrderBy(x => x.label).Take(20).ToList();

            return v;
        }
        public object GetAutoCompleteSupplier(string prefix, int companyId)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == (int)Provider.Supplier)
                     where t1.IsActive && ((t1.Name.Contains(prefix)) || (t1.Code.Contains(prefix)))

                     select new
                     {
                         label = "[" + t1.Code + "] " + t1.Name,
                         val = t1.VendorId
                     }).OrderBy(x => x.label).Take(150).ToList();

            return v;
        }
        public List<object> GetSupplier(int companyId)
        {
            var List = new List<object>();
            _db.Vendors
            .Where(x => x.IsActive).Where(x => x.CompanyId == companyId && x.VendorTypeId == (int)Provider.Supplier).Select(x => x).ToList()
            .ForEach(x => List.Add(new
            {
                Value = x.VendorId,
                Text = x.Code + " -" + x.Name
            }));
            return List;
        }
        public object GetAutoCompleteCustomer(string prefix, int companyId)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == (int)Provider.Customer)
                     join t2 in _db.HeadGLs on t1.HeadGLId equals t2.Id
                     where t1.IsActive && ((t1.Name.StartsWith(prefix)) || (t1.Code.StartsWith(prefix)))

                     select new
                     {
                         label = "[" + t2.AccCode + "] " + t1.Name,
                         val = t1.VendorId,
                         CustomerTypeFK = t1.CustomerTypeFK
                     }).OrderBy(x => x.label).Take(150).ToList();

            return v;
        }

        public object GetAutoCompleteCustomerBySz(int SubZoneId)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Customer && x.SubZoneId == SubZoneId)
                     join t2 in _db.HeadGLs on t1.HeadGLId equals t2.Id
                     where t1.IsActive && t1.IsActive == true

                     select new
                     {
                         label = "[" + t2.AccCode + "] " + t1.Name,
                         val = t1.VendorId,
                         CustomerTypeFK = t1.CustomerTypeFK
                     }).OrderBy(x => x.label).Take(150).ToList();

            return v;
        }



        //public async Task<VMCommonProduct> CommonProductSingle(int id)
        //{
        //    VMCommonProduct vmCommonProduct = new VMCommonProduct();

        //    vmCommonProduct = await Task.Run(() => (from t1 in _db.Common_Product.Where(x => x.ID == id && x.Active)
        //                                            join t2 in _db.Common_Unit.Where(x => x.Active) on t1.Common_UnitFk equals t2.ID
        //                                            select new VMCommonProduct
        //                                            {
        //                                                ID = t1.ID,
        //                                                Name = t1.Name,
        //                                                UnitName = t2.Name,
        //                                                Common_UnitFk = t1.Common_UnitFk,
        //                                                MRPPrice = t1.MRPPrice,
        //                                                VATPercent = t1.VATPercent,
        //                                                CurrentStock = (_db.WareHouse_ConsumptionSlave.Where(x => x.Common_ProductFK == id && x.Active).Select(x => x.ConsumeQuantity).DefaultIfEmpty(0).Sum()
        //                                                              - _db.Marketing_SalesSlave.Where(x => x.Common_ProductFK == id && x.Active).Select(x => x.Quantity).DefaultIfEmpty(0).Sum())
        //                                            }).FirstOrDefault());

        //    return vmCommonProduct;
        //}
        public object GetAutoSubZone(string prefix, int companyId)
        {
            var v = (from t1 in _db.SubZones.Where(x => x.CompanyId == companyId && x.IsActive == true)

                     where t1.IsActive && ((t1.Name.StartsWith(prefix)) || (t1.Code.StartsWith(prefix)))

                     select new
                     {
                         label = t1.Name,
                         val = t1.SubZoneId,

                     }).OrderBy(x => x.label).Take(150).ToList();

            return v;
        }

        public async Task<List<VMCommonProduct>> ProductCategoryGet()
        {
            List<VMCommonProduct> vMRSC = await Task.Run(() => (_db.ProductCategories.Where(x => x.IsActive)).Select(x => new VMCommonProduct() { ID = x.ProductCategoryId, Name = x.Name }).ToListAsync());

            return vMRSC;
        }


        public async Task<List<VMCommonProductSubCategory>> CommonProductSubCategoryGet(int id)
        {

            List<VMCommonProductSubCategory> vMRSC = await Task.Run(() => (_db.ProductSubCategories.Where(x => x.IsActive && (id <= 0 || x.ProductCategoryId == id))).Select(x => new VMCommonProductSubCategory() { ID = x.ProductSubCategoryId, Name = x.Name }).ToListAsync());


            return vMRSC;
        }
        public async Task<List<VMCommonProduct>> CommonProductGet(int id)
        {
            List<VMCommonProduct> vMRI = await Task.Run(() => (_db.Products.Where(x => x.IsActive && (id <= 0 || x.ProductSubCategoryId == id)).Select(x => new VMCommonProduct() { ID = x.ProductId, Name = x.ProductName })).ToListAsync());

            return vMRI;
        }
        public async Task<long> OrderMasterAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long result = -1;

            var poMax = _db.OrderMasters.Where(x => x.CompanyId ==
            vmSalesOrderSlave.CompanyFK
            && !x.IsOpening && x.ProductType == "F").Count() + 1;

            string poCid = "";
            var CompanyInfo = await _db.Companies.FirstOrDefaultAsync(x => x.CompanyId == vmSalesOrderSlave.CompanyFK.Value);
            string shortName = CompanyInfo.ShortName + "F";
            if (vmSalesOrderSlave.CompanyFK.Value > 0)
            {
                poCid = shortName + "#" + poMax.ToString();
            }
            else
            {
                return result;
            }
            //else
            //{
            //    poCid =
            //               @"SO-" +
            //                    DateTime.Now.ToString("yy") +
            //                    DateTime.Now.ToString("MM") +
            //                    DateTime.Now.ToString("dd") + "-" +

            //               poMax.ToString();
            //}

            OrderMaster orderMaster = new OrderMaster
            {
                CustomerPONo = vmSalesOrderSlave.CustomerPONo,
                OrderNo = poCid,
                OrderDate = vmSalesOrderSlave.OrderDate,
                CustomerId = vmSalesOrderSlave.CustomerId,
                ExpectedDeliveryDate = vmSalesOrderSlave.ExpectedDeliveryDate,
                PaymentMethod = vmSalesOrderSlave.CustomerPaymentMethodEnumFK,
                ProductType = "F",
                Status = (int)EnumPOStatus.Draft,
                CourierNo = vmSalesOrderSlave.CourierNo,
                FinalDestination = vmSalesOrderSlave.FinalDestination,
                CourierCharge = vmSalesOrderSlave.CourierCharge,
                CurrentPayable = Convert.ToDecimal(vmSalesOrderSlave.PayableAmount),
                StockInfoId = vmSalesOrderSlave.StockInfoId,
                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,// System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderStatus = "N",
                SalePersonId = vmSalesOrderSlave.SalePersonId,
                Remarks = vmSalesOrderSlave.Remarks,
                IsService = vmSalesOrderSlave.IsService,
            };
            _db.OrderMasters.Add(orderMaster);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderMaster.OrderMasterId;
            }
            return result;
        }

        public async Task<long> OrderMasterRawAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long result = -1;

            var poMax = _db.OrderMasters.Where(x => x.CompanyId ==
            vmSalesOrderSlave.CompanyFK
            && !x.IsOpening && x.ProductType == "R").Count() + 1;

            string poCid = "";
            var CompanyInfo = await _db.Companies.FirstOrDefaultAsync(x => x.CompanyId == vmSalesOrderSlave.CompanyFK.Value);
            string shortName = CompanyInfo.ShortName + "R";
            if (vmSalesOrderSlave.CompanyFK.Value > 0)
            {
                poCid = shortName + "#" + poMax.ToString();
            }
            else
            {
                return result;
            }
            //else
            //{
            //    poCid =
            //               @"SO-" +
            //                    DateTime.Now.ToString("yy") +
            //                    DateTime.Now.ToString("MM") +
            //                    DateTime.Now.ToString("dd") + "-" +

            //               poMax.ToString();
            //}

            OrderMaster orderMaster = new OrderMaster
            {
                CustomerPONo = vmSalesOrderSlave.CustomerPONo,
                OrderNo = poCid,
                OrderDate = vmSalesOrderSlave.OrderDate,
                CustomerId = vmSalesOrderSlave.CustomerId,
                ExpectedDeliveryDate = vmSalesOrderSlave.ExpectedDeliveryDate,
                PaymentMethod = vmSalesOrderSlave.CustomerPaymentMethodEnumFK,
                ProductType = "R",
                Status = (int)EnumPOStatus.Draft,
                CourierNo = vmSalesOrderSlave.CourierNo,
                FinalDestination = vmSalesOrderSlave.FinalDestination,
                CourierCharge = vmSalesOrderSlave.CourierCharge,
                CurrentPayable = Convert.ToDecimal(vmSalesOrderSlave.PayableAmount),
                StockInfoId = vmSalesOrderSlave.StockInfoId,
                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,// System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderStatus = "N",
                SalePersonId = vmSalesOrderSlave.SalePersonId,
                Remarks = vmSalesOrderSlave.Remarks,
                IsService = vmSalesOrderSlave.IsService,
            };
            _db.OrderMasters.Add(orderMaster);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderMaster.OrderMasterId;
            }
            return result;
        }

        public async Task<long> FeedOrderMasterAdd(FeedSalesOrderModel model)
        {
            long result = -1;
            OrderMaster orderMaster = new OrderMaster
            {
                OrderNo = model.SalesOrderNo,
                OrderDate = model.OrderDate,
                CustomerId = model.CustomerId,
                ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                ProductType = model.ProductType,
                Status = (int)EnumPOStatus.Draft,
                StockInfoId = model.StockInfoId,
                CompanyId = model.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CourierCharge = 0,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderStatus = "N",
                SalePersonId = model.SalePersonId,
                PaymentMethod = 0,
                CourierNo = "",
                FinalDestination = "",
                CurrentPayable = 0,
                DiscountAmount = 0,
                DiscountRate = 0,
                GrandTotal = 0,
                Remarks = model.Remarks,



            };
            _db.OrderMasters.Add(orderMaster);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderMaster.OrderMasterId;
            }
            return result;
        }



        public async Task<int> FeedOrderDetailEdit(FeedSalesOrderModel feedSalesOrderModel)
        {
            var result = -1;
            OrderDetail model = await _db.OrderDetails.FindAsync(feedSalesOrderModel.OrderDetailId);

            model.ProductId = feedSalesOrderModel.FProductId;
            model.Qty = feedSalesOrderModel.Qty;
            model.UnitPrice = feedSalesOrderModel.UnitPrice;
            model.Amount = (feedSalesOrderModel.Qty * feedSalesOrderModel.UnitPrice);
            model.Comsumption = feedSalesOrderModel.Consumption;
            model.PackQuantity = feedSalesOrderModel.PackQuantity;
            model.SpecialDiscount = feedSalesOrderModel.SpecialDiscount;
            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifedDate = DateTime.Now;
            model.ECarryingCommission = feedSalesOrderModel.CarryingCommission;
            model.EBaseCommission = feedSalesOrderModel.BaseCommission;
            model.ECashCommission = feedSalesOrderModel.CashCommission;
            model.AdditionPrice = feedSalesOrderModel.AdditionalPrice;
            model.MonthlyIncentive = feedSalesOrderModel.MonthlyIncentiveInInvoice;
            model.YearlyIncentive = feedSalesOrderModel.YearlyIncentiveInInvoice;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)feedSalesOrderModel.OrderMasterId;
            }

            return result;
        }

        public async Task<long> OrderMasterOpeningAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long result = -1;
            var poMax = _db.OrderMasters.Where(x => x.CompanyId == vmSalesOrderSlave.CompanyFK).Count() + 1;
            string poCid = @"Opening-" +
                            vmSalesOrderSlave.OrderDate.ToString("yy") +
                            vmSalesOrderSlave.OrderDate.ToString("MM") +
                            vmSalesOrderSlave.OrderDate.ToString("dd") + "-" +

                             poMax.ToString().PadLeft(2, '0');
            OrderMaster orderMaster = new OrderMaster
            {

                OrderNo = poCid,
                OrderDate = vmSalesOrderSlave.OrderDate,
                CustomerId = vmSalesOrderSlave.CustomerId,
                PaymentMethod = 1,
                ProductType = "F",
                Status = (int)EnumPOStatus.Submitted,
                CourierNo = "",
                FinalDestination = "",
                CourierCharge = 0,
                Remarks = vmSalesOrderSlave.Remarks,
                IsOpening = true,

                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),// System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = vmSalesOrderSlave.OrderDate,
                IsActive = true
            };
            _db.OrderMasters.Add(orderMaster);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderMaster.OrderMasterId;
            }
            return result;
        }

        #region Enum Model
        public class EnumOriginModel
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }
        public class EnumPOTypeModel
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        #endregion


        public async Task<bool> DeletefeedorderDetail(int orderDetailId)
        {

            OrderDetail feedOrderDetail = new OrderDetail();
            feedOrderDetail = await _db.OrderDetails.Where(x => x.OrderDetailId == orderDetailId).FirstOrDefaultAsync();
            if (feedOrderDetail != null)
            {
                feedOrderDetail.IsActive = false;
                await _db.SaveChangesAsync();
                return true;

            }
            else
            {
                return false;
            }
            return false;
        }

        public async Task<VMSalesOrderSlave> PackagingSalesOrderDetailsGet(int companyId, int orderMasterId)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            vmSalesOrderSlave = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId && x.CompanyId == companyId)
                                                      join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                      join t4 in _db.HeadGLs on t2.HeadGLId equals t4.Id
                                                      join t6 in _db.OfficerAssigns on t1.SalePersonId equals t6.EmpId into x
                                                      from t6 in x.DefaultIfEmpty()
                                                      join t7 in _db.Employees on t6.EmpId equals t7.Id into y
                                                      from t7 in y.DefaultIfEmpty()



                                                      join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                      select new VMSalesOrderSlave
                                                      {
                                                          CommonCustomerCode = t4.AccCode,

                                                          CustomerPhone = t2.Phone,
                                                          CustomerAddress = t2.Address,
                                                          CustomerEmail = t2.Email,
                                                          ContactPerson = t2.ContactName,
                                                          CompanyFK = t1.CompanyId,
                                                          OrderMasterId = t1.OrderMasterId,
                                                          CreatedDate = t1.CreateDate,
                                                          OrderNo = t1.OrderNo,
                                                          Status = t1.Status,
                                                          OrderDate = t1.OrderDate,
                                                          CreatedBy = t1.CreatedBy,
                                                          CustomerPaymentMethodEnumFK = t1.PaymentMethod,
                                                          ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                                          CommonCustomerName = t2.Name,
                                                          CompanyName = t3.Name,
                                                          CompanyAddress = t3.Address,
                                                          CompanyEmail = t3.Email,
                                                          CompanyPhone = t3.Phone,
                                                          CustomerPONo = t1.CustomerPONo,
                                                          CustomerTypeFk = t2.CustomerTypeFK,
                                                          CustomerId = t2.VendorId,
                                                          CourierCharge = t1.CourierCharge,
                                                          FinalDestination = t1.FinalDestination,
                                                          CourierNo = t1.CourierNo,
                                                          OfficerNAme = t7 != null ? t7.Name : ""




                                                      }).FirstOrDefault());

            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                                    join t3 in _db.Products on t1.ProductId equals t3.ProductId
                                                                    join t4 in _db.ProductSubCategories on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                    join t5 in _db.ProductCategories on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                    join t6 in _db.Units on t3.UnitId equals t6.UnitId
                                                                    select new VMSalesOrderSlave
                                                                    {

                                                                        OrderMasterId = t1.OrderMasterId.Value,
                                                                        OrderDetailId = t1.OrderDetailId,

                                                                        JobOrderNo = t1.JobOrderNo,
                                                                        JobOrderDate = t1.OrderDate,
                                                                        ReelDirection = t1.ReelDirection,
                                                                        PouchDerection = t1.PouchDerection,
                                                                        ProductName = t4.Name + " " + t3.ProductName,
                                                                        Qty = t1.Qty,
                                                                        TDSPercent = t1.TDSPercent,
                                                                        VATPercent = t1.VATPercent,
                                                                        VATAmount = (t1.Qty *
                                                                        (t1.IsVATInclude == true ? t1.UnitPrice / (((double)t1.VATPercent + 100) / 100) : t1.UnitPrice) // Unit Price
                                                                                    ) / 100 * (double)t1.VATPercent,
                                                                        IsVATInclude = t1.IsVATInclude,
                                                                        UnitPrice = (t1.IsVATInclude == true ? t1.UnitPrice / (((double)t1.VATPercent + 100) / 100) : t1.UnitPrice),
                                                                        UnitName = t6.Name,
                                                                        TotalAmount = t1.Amount,
                                                                        Description = t1.Remarks,

                                                                        ProductCategoryId = t5.ProductCategoryId,
                                                                        ProductSubCategoryId = t4.ProductSubCategoryId,
                                                                        FProductId = t3.ProductId,


                                                                        //ProductDiscountUnit = t1.DiscountUnit,//Unit Discount                                                               
                                                                        //CashDiscountPercent = t1.DiscountRate, // Cash Discount                                                               
                                                                        //SpecialDiscount = t1.SpecialBaseCommission, // SpecialDiscount 
                                                                    }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());


            return vmSalesOrderSlave;
        }

        public async Task<VMFinishProductBOM> PackagingSalesOrderDetailsGetBOM(int companyId, int orderDetailId)
        {
            VMFinishProductBOM vmFinishProductBOM = new VMFinishProductBOM();
            vmFinishProductBOM = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.OrderDetailId == orderDetailId && x.CompanyId == companyId)
                                                       join t4 in _db.OrderMasters on t1.OrderMasterId equals t4.OrderMasterId
                                                       join t5 in _db.Products on t1.ProductId equals t5.ProductId
                                                       join t6 in _db.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                                       join t7 in _db.Units on t5.UnitId equals t7.UnitId
                                                       join t2 in _db.Vendors on t4.CustomerId equals t2.VendorId
                                                       join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                       select new VMFinishProductBOM
                                                       {
                                                           OrderDate = t4.OrderDate,
                                                           CustomerPhone = t2.Phone,
                                                           CustomerAddress = t2.Address,
                                                           CustomerEmail = t2.Email,
                                                           ContactPerson = t2.ContactName,
                                                           CompanyFK = t1.CompanyId,
                                                           OrderMasterId = t1.OrderDetailId,
                                                           CreatedBy = t1.CreatedBy,
                                                           CommonCustomerName = t2.Name,
                                                           CompanyName = t3.Name,
                                                           CompanyAddress = t3.Address,
                                                           CompanyEmail = t3.Email,
                                                           CompanyPhone = t3.Phone,
                                                           Qty = t1.Qty,
                                                           FinishUnitPrice = t1.UnitPrice,
                                                           FinishProductName = t6.Name + " " + t5.ProductName,
                                                           OrderNo = t4.OrderNo,
                                                           UnitName = t7.Name,
                                                           JobOrderNo = t1.JobOrderNo,
                                                           StatusId = t1.Status,
                                                           OrderDetailId = t1.OrderDetailId
                                                       }).FirstOrDefault());

            vmFinishProductBOM.DataListProductBOM = await Task.Run(() => (from t1 in _db.FinishProductBOMs.Where(x => x.IsActive && x.OrderDetailId == orderDetailId)
                                                                          join t3 in _db.Products.Where(x => x.IsActive) on t1.RProductFK equals t3.ProductId
                                                                          join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                          join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                          select new VMFinishProductBOM
                                                                          {
                                                                              ID = t1.ID,
                                                                              RawProductName = t4.Name + " " + t3.ProductName,
                                                                              RequiredQuantity = t1.RequiredQuantity,
                                                                              OrderDetailId = t1.OrderDetailId.Value,
                                                                              UnitPrice = t1.UnitPrice,
                                                                              TotalPrice = t1.RequiredQuantity * t1.UnitPrice,
                                                                              RawConsumeQuantity = t1.Consumption
                                                                          }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());


            return vmFinishProductBOM;
        }

        public async Task<long> AddFinishProductBOM(VMFinishProductBOM vmFinishProductBOM)
        {

            long result = -1;
            FinishProductBOM FinishProductBOM = new FinishProductBOM
            {
                CompanyId = vmFinishProductBOM.CompanyFK.Value,
                OrderDetailId = vmFinishProductBOM.OrderDetailId,
                RProductFK = vmFinishProductBOM.RProductFK,
                Consumption = vmFinishProductBOM.RawConsumeQuantity,
                RequiredQuantity = vmFinishProductBOM.RequiredQuantity,
                SupplierId = vmFinishProductBOM.SupplierId,
                UnitPrice = vmFinishProductBOM.UnitPrice,
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                ORDStyle = vmFinishProductBOM.ORDStyle


            };
            _db.FinishProductBOMs.Add(FinishProductBOM);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmFinishProductBOM.OrderDetailId;
            }

            return result;
        }

        public async Task<long> PackagingPurchaseOrderSlaveAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
            {
                PurchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId,
                ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),
                VATAddition = vmPurchaseOrderSlave.VATAddition,

                DemandRate = 0,
                QCRate = 0,
                PackSize = 0,

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsVATIncluded = vmPurchaseOrderSlave.IsVATIncluded
            };
            _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = procurementPurchaseOrderSlave.PurchaseOrderDetailId;
            }

            return result;
        }

        public async Task<long> PromotionalItemInvoiceChildAdd(IssueDetailInfoVM issueDetailInfoVM)
        {
            long result = -1;

            IssueDetailInfo issueDetailInfo = new IssueDetailInfo
            {
                IssueMasterId = issueDetailInfoVM.IssueMasterId,
                RProductId = issueDetailInfoVM.RProductId,
                RMQ = issueDetailInfoVM.RMQ,
                CostingPrice = issueDetailInfoVM.CostingPrice,
                IsActive = true,

            };
            _db.IssueDetailInfoes.Add(issueDetailInfo);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = issueDetailInfoVM.IssueMasterId;
            }

            return result;
        }

        public async Task<long> PackagingPurchaseOrderDetailsAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            foreach (var item in vmPurchaseOrderSlave.DataListPur)
            {
                PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
                {
                    PurchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId,
                    ProductId = item.Common_ProductFK,
                    PurchaseQty = item.RequiredQuantity,
                    PurchaseRate = item.PurchasingPrice,
                    PurchaseAmount = item.PurchaseAmount,
                    FinishProductBOMId = item.FinishProductBOMId,



                    DemandRate = 0,
                    QCRate = 0,
                    PackSize = 0,

                    CompanyId = vmPurchaseOrderSlave.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = procurementPurchaseOrderSlave.PurchaseOrderDetailId;
                }

            }



            return result;
        }


        //PackagingPurchaseRequisition
        public async Task<int> PackagingPurchaseRequisitionAdd(VMPackagingPurchaseRequisition vmRequisition)
        {

            int result = -1;
            if (vmRequisition != null)
            {
                var poMax = _db.Requisitions.Where(x => x.CompanyId == 20).Count() + 1;
                string poCid = "";
                poCid = @"RQ-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                        DateTime.Now.ToString("dd") + "-" + poMax.ToString();

                Requisition PackagingRequisition = new Requisition
                {
                    RequisitionType = (int)EnumRequisitionType.PackgingRequisition,
                    RequisitionNo = poCid,
                    OrderDetailsId = vmRequisition.OrderDetailsId,
                    RequisitionDate = vmRequisition.RequisitionDate,
                    CompanyId = vmRequisition.CompanyFK,
                    FromRequisitionId = vmRequisition.FromDepartmentReqId,
                    ToRequisitionId = vmRequisition.ToDepartmentReqId,
                    CreatedBy = vmRequisition.CreatedBy,
                    CreatedDate = DateTime.Now,
                    RequisitionBy = vmRequisition.CreatedBy,
                    RequisitionStatus = "N",
                    IsActive = true,
                    IsSubmitted = false
                };
                _db.Requisitions.Add(PackagingRequisition);

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PackagingRequisition.RequisitionId;

                }

            }
            return result;
        }



        public async Task<long> PackagingPurchaseRequisitionDetailsAdd(VMPackagingPurchaseRequisition vmRequisition, VMPurchaseOrderSlave productionRequisitionPar)
        {
            long result = -1;
            var resultCount = 0;

            var dataList = productionRequisitionPar.DataListPur.Where(x => x.RequiredQuantity > 0).ToList();

            foreach (var item in dataList)
            {
                resultCount = _db.Database.ExecuteSqlCommand("EXEC spRequisitionItemDetailCreate {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                     0,
                     vmRequisition.RequisitionId,
                     item.Common_ProductFK,
                     null,
                     item.RequiredQuantity,
                     null,
                     null,
                     null,
                     null,
                     0,
                     true,
                     item.FinishProductBOMId);

                result = vmRequisition.RequisitionId;
            }

            return result;
        }

        public async Task<VMPackagingProductionRequisitions> PackagingGeneralRMRequisitionList(int companyId)
        {

            VMPackagingProductionRequisitions vmSalesOrder = new VMPackagingProductionRequisitions();
            vmSalesOrder.CompanyFK = companyId;
            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.Requisitions.Where(x => x.IsActive && x.CompanyId == companyId && x.RequisitionType == 3 && x.OrderDetailsId == 0)
                                                          join t6 in _db.StockInfoes.Where(x => x.IsActive) on t1.FromRequisitionId equals t6.StockInfoId
                                                          join t7 in _db.StockInfoes.Where(x => x.IsActive) on t1.ToRequisitionId equals t7.StockInfoId
                                                          select new VMPackagingProductionRequisitions
                                                          {
                                                              RequisitionId = t1.RequisitionId,
                                                              RequisitionNo = t1.RequisitionNo,
                                                              RequisitionDate = t1.RequisitionDate.Value,
                                                              RequisitionStatus = t1.RequisitionStatus,
                                                              CompanyId = t1.CompanyId.Value,
                                                              FromRequisitionName = t6.Name,
                                                              ToRequisitionName = t7.Name,
                                                              IsSubmitted = t1.IsSubmitted
                                                          }).OrderByDescending(x => x.RequisitionId).AsEnumerable());
            return vmSalesOrder;
        }

        public async Task<VMPackagingProductionRequisitions> PackagingProductionRequisitionList(int companyId)
        {
            VMPackagingProductionRequisitions vmSalesOrder = new VMPackagingProductionRequisitions();
            vmSalesOrder.CompanyFK = companyId;
            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.Requisitions.Where(x => x.IsActive && x.CompanyId == companyId && x.RequisitionType == 3)
                                                          join t2 in _db.OrderDetails.Where(x => x.IsActive) on t1.OrderDetailsId equals t2.OrderDetailId
                                                          join t3 in _db.OrderMasters.Where(x => x.IsActive) on t2.OrderMasterId equals t3.OrderMasterId
                                                          join t4 in _db.Products.Where(x => x.IsActive) on t2.ProductId equals t4.ProductId
                                                          join t5 in _db.ProductSubCategories.Where(x => x.IsActive) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                          join t6 in _db.StockInfoes.Where(x => x.IsActive) on t1.FromRequisitionId equals t6.StockInfoId
                                                          join t7 in _db.StockInfoes.Where(x => x.IsActive) on t1.ToRequisitionId equals t7.StockInfoId


                                                          select new VMPackagingProductionRequisitions
                                                          {
                                                              RequisitionId = t1.RequisitionId,
                                                              RequisitionNo = t1.RequisitionNo,
                                                              RequisitionDate = t1.RequisitionDate.Value,

                                                              RequisitionStatus = t1.RequisitionStatus,
                                                              CompanyId = t1.CompanyId.Value,

                                                              OrderNo = t3.OrderNo,
                                                              JobOrderNo = t2.JobOrderNo,
                                                              FromRequisitionName = t6.Name,
                                                              ToRequisitionName = t7.Name,
                                                              ProductName = t5.Name + " " + t4.ProductName,
                                                              Description = t2.Remarks,
                                                              OrderMasterId = t3.OrderMasterId,
                                                              IsSubmitted = t1.IsSubmitted

                                                          }).OrderByDescending(x => x.RequisitionId).AsEnumerable());
            return vmSalesOrder;
        }

        public List<VMPackagingPurchaseRequisition> PackagingProductionStoreDataList(int RequisitionId)
        {
            VMPackagingPurchaseRequisition vmSalesOrder = new VMPackagingPurchaseRequisition();
            //var list = (from t1 in _db.RequisitionItemDetails.Where(x => x.RequisitionId == RequisitionId)
            //                //join t2 in _db.FinishProductBOMs.Where(x => x.IsActive) on t1.FinishProductBOMId equals t2.ID
            //            join t4 in _db.Products.Where(x => x.IsActive) on t1.RProductId equals t4.ProductId
            //            join t5 in _db.ProductSubCategories.Where(x => x.IsActive) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
            //            join t6 in _db.Requisitions.Where(x => x.IsActive) on t1.RequisitionId equals t6.RequisitionId
            //            select new VMPackagingPurchaseRequisition
            //            {
            //                CompanyId = t6.CompanyId.Value,
            //                RequisitionId = t1.RequisitionItemId,
            //                RequistionItemDetailId = t1.RequistionItemDetailId,
            //                ProductId = t1.RProductId.Value,
            //                ProductName = t5.Name + " " + t4.ProductName,
            //                Qty = t1.RQty,
            //                RemainingQuantity = t1.RQty - (_db.IssueDetailInfoes.Where(x => x.RequisitionItemDetailId == t1.RequistionItemDetailId && x.IsActive == true).Select(x => x.RMQ).DefaultIfEmpty(0).Sum()),
            //                PriviousIssueQty = (_db.IssueDetailInfoes.Where(x => x.RequisitionItemDetailId == t1.RequistionItemDetailId && x.IsActive == true).Select(x => x.RMQ).DefaultIfEmpty(0).Sum())
            //            }).ToList();


            //foreach (VMPackagingPurchaseRequisition item in list)
            //{
            //    VMProductStock vMProductStock = new VMProductStock();
            //    vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetPackagingRMStockByProductId {0},{1}", item.ProductId, item.CompanyId).FirstOrDefault();


            //    item.CurrenntStock = vMProductStock.ClosingQty;

            //}
            return vmSalesOrder.DataListPro;
        }

        public List<VMPackagingPurchaseRequisition> PackagingGeneralProductionStoreDataList(int RequisitionId)
        {
            VMPackagingPurchaseRequisition vmSalesOrder = new VMPackagingPurchaseRequisition();
            //var list = (from t1 in _db.RequisitionItemDetails.Where(x => x.RequisitionId == RequisitionId)
            //            join t4 in _db.Products.Where(x => x.IsActive) on t1.RProductId equals t4.ProductId
            //            join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId

            //            select new VMPackagingPurchaseRequisition
            //            {
            //                RequisitionId = t1.RequisitionItemId,
            //                RequistionItemDetailId = t1.RequistionItemDetailId,
            //                RequisitionStatus = "Y",
            //                ProductId = t1.RProductId.Value,
            //                ProductName = t5.Name + " " + t4.ProductName,
            //                Qty = t1.RQty,

            //                RemainingQuantity = t1.RQty - (_db.IssueDetailInfoes.Where(x => x.RequisitionItemDetailId == t1.RequistionItemDetailId).Select(x => x.RMQ).DefaultIfEmpty(0).Sum()),
            //                PriviousIssueQty = (_db.IssueDetailInfoes.Where(x => x.RequisitionItemDetailId == t1.RequistionItemDetailId).Select(x => x.RMQ).DefaultIfEmpty(0).Sum())
            //            }).Distinct().ToList();
            return vmSalesOrder.DataListPro;
        }

        public async Task<long> PackagingIssueProductFromStore(VMPackagingPurchaseRequisition vmPackagingIssue)
        {
            long result = -1;
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    var poMax = _db.IssueMasterInfoes.Where(x => x.CompanyId == 20).Count() + 1;
                    string poCid = "";
                    poCid = @"IS-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                    DateTime.Now.ToString("dd") + "-" + poMax.ToString();

                    //IssueMasterInfo Procurement_Issue = new IssueMasterInfo
                    //{
                    //    RequisitionId = vmPackagingIssue.RequisitionId,
                    //    IssueNo = poCid,
                    //    IssueDate = vmPackagingIssue.IssueDate,
                    //    FromDepartmentId = vmPackagingIssue.FromDepartmentIssueId,
                    //    ToDepartmentId = vmPackagingIssue.ToDepartmentIssueId,

                    //    CreatedBy = Common.GetUserId(),
                    //    CreatedDate = DateTime.Now,
                    //    CompanyId = vmPackagingIssue.CompanyFK.Value,
                    //    IsActive = true,
                    //    IssuedBy = vmPackagingIssue.IssueById,
                    //    Achknolagement = false,
                    //    AchknologeBy = 0,
                    //    AcknologeDate = null


                    //};

                    //_db.IssueMasterInfoes.Add(Procurement_Issue);
                    if (await _db.SaveChangesAsync() > 0)
                    {
                        var dataList = vmPackagingIssue.DataListPro.Where(x => x.IssueQty > 0);
                        foreach (var item in dataList)
                        {
                            VMProductStock vMProductStock = new VMProductStock();
                            vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetPackagingRMStockByProductId {0},{1}", item.ProductId, vmPackagingIssue.CompanyFK).FirstOrDefault();


                            //IssueDetailInfo IssueDetais = new IssueDetailInfo
                            //{
                            //    IssueMasterId = Procurement_Issue.IssueMasterId,
                            //    RProductId = item.ProductId,
                            //    RMQ = item.IssueQty,
                            //    RequisitionItemDetailId = item.RequistionItemDetailId,
                            //    CostingPrice = vMProductStock.ClosingRate,
                            //    IsActive = true
                            //};
                            //_db.IssueDetailInfoes.Add(IssueDetais);
                        }

                        //if (await _db.SaveChangesAsync() > 0)
                        //{
                        //    //After Complete Issue Update Requisition Table
                        //    var RequisitionData = await _db.Requisitions.FirstOrDefaultAsync(x => x.RequisitionId == vmPackagingIssue.RequisitionId);
                        //    if (RequisitionData != null)
                        //    {
                        //        RequisitionData.RequisitionStatus = "I";
                        //        RequisitionData.DeliveredBy = Common.GetUserId();
                        //        RequisitionData.DeliveredDate = DateTime.Now;
                        //        RequisitionData.DeliveryNo = poCid;

                        //        _db.Entry(RequisitionData).State = EntityState.Modified;
                        //        if (await _db.SaveChangesAsync() > 0)
                        //        {
                        //            scope.Commit();
                        //            result = Procurement_Issue.IssueMasterId;
                        //        }
                        //    }
                        //}

                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    throw ex;
                }

            }

            return result;
        }

        public async Task<long> PackagingGIssueProductFromStoreSave(VMPackagingPurchaseRequisition vmPackagingIssue)
        {

            long result = -1;
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    var poMax = _db.IssueMasterInfoes.Where(x => x.CompanyId == 20).Count() + 1;
                    string poCid = "";
                    poCid = @"IS-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                    DateTime.Now.ToString("dd") + "-" + poMax.ToString();

                    //IssueMasterInfo Procurement_Issue = new IssueMasterInfo
                    //{
                    //    RequisitionId = vmPackagingIssue.RequisitionId,
                    //    IssueNo = poCid,
                    //    IssueDate = vmPackagingIssue.IssueDate,
                    //    FromDepartmentId = vmPackagingIssue.FromDepartmentIssueId,
                    //    ToDepartmentId = vmPackagingIssue.ToDepartmentIssueId,

                    //    CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                    //    CreatedDate = DateTime.Now,
                    //    CompanyId = vmPackagingIssue.CompanyFK.Value,
                    //    IsActive = true,
                    //    IssuedBy = vmPackagingIssue.IssueById,
                    //    Achknolagement = false,
                    //    AchknologeBy = 0,
                    //    AcknologeDate = null
                    //};
                    //_db.IssueMasterInfoes.Add(Procurement_Issue);
                    if (await _db.SaveChangesAsync() > 0)
                    {

                        var dataList = vmPackagingIssue.DataListPro.Where(x => x.IssueQty > 0);
                        foreach (var item in dataList)
                        {
                            VMProductStock vMProductStock = new VMProductStock();
                            vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetPackagingRMStockByProductId {0},{1}", item.ProductId, vmPackagingIssue.CompanyFK).FirstOrDefault();


                            //IssueDetailInfo IssueDetais = new IssueDetailInfo
                            //{
                            //    IssueMasterId = Procurement_Issue.IssueMasterId,
                            //    RProductId = item.ProductId,
                            //    RMQ = item.IssueQty,
                            //    RequisitionItemDetailId = item.RequistionItemDetailId,
                            //    CostingPrice = vMProductStock.ClosingRate,
                            //    IsActive = true
                            //};
                            //_db.IssueDetailInfoes.Add(IssueDetais);
                        }

                        //if (await _db.SaveChangesAsync() > 0)
                        //{
                        //    //After Complete Issue Update Requisition Table
                        //    var RequisitionData = await _db.Requisitions.FirstOrDefaultAsync(x => x.RequisitionId == vmPackagingIssue.RequisitionId);
                        //    if (RequisitionData != null)
                        //    {
                        //        RequisitionData.RequisitionStatus = "I";
                        //        RequisitionData.DeliveredBy = Common.GetUserId();
                        //        RequisitionData.DeliveredDate = DateTime.Now;
                        //        RequisitionData.DeliveryNo = poCid;

                        //        _db.Entry(RequisitionData).State = EntityState.Modified;
                        //        if (await _db.SaveChangesAsync() > 0)
                        //        {
                        //            scope.Commit();
                        //            result = Procurement_Issue.IssueMasterId;
                        //        }
                        //    }
                        //}

                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    throw ex;
                }

            }

            return result;
        }


        public async Task<long> PackagingIssueProductFromStoreDetailsAdd(VMPackagingPurchaseRequisition vmPackagingPurchaseRequisition)
        {
            long result = -1;

            var dataList = vmPackagingPurchaseRequisition.DataListPro.Where(x => x.IssueQty > 0);
            foreach (var item in dataList)
            {
                VMProductStock vMProductStock = new VMProductStock();
                vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetPackagingRMStockByProductId {0},{1}", item.ProductId, vmPackagingPurchaseRequisition.CompanyFK).FirstOrDefault();


                //IssueDetailInfo IssueDetais = new IssueDetailInfo
                //{
                //    IssueMasterId = vmPackagingPurchaseRequisition.IssueMasterId,
                //    RProductId = item.ProductId,
                //    RMQ = item.IssueQty,
                //    RequisitionItemDetailId = item.RequistionItemDetailId,
                //    CostingPrice = vMProductStock.ClosingRate,
                //    IsActive = true
                //};
                //_db.IssueDetailInfoes.Add(IssueDetais);
                //if (await _db.SaveChangesAsync() > 0)
                //{
                //    result = IssueDetais.IssueDetailId;
                //}

            }



            return result;
        }

        public async Task<VMPackagingPurchaseRequisition> GetIssueDetail(int companyId, long issueMasterId, CancellationToken cancellationToken = default)
        {
            VMPackagingPurchaseRequisition vmPackagingPurchaseRequisition = new VMPackagingPurchaseRequisition();
            //vmPackagingPurchaseRequisition = await Task.Run(() => (from t1 in _db.IssueMasterInfoes.Where(x => x.CompanyId == companyId && x.IssueMasterId == issueMasterId)
            //                                                       join c in _db.Companies on t1.CompanyId equals c.CompanyId
            //                                                       join t2 in _db.Requisitions on t1.RequisitionId equals t2.RequisitionId
            //                                                       join s in _db.StockInfoes on t1.ToDepartmentId equals s.StockInfoId
            //                                                       join s1 in _db.StockInfoes on t1.FromDepartmentId equals s1.StockInfoId
            //                                                       join e in _db.Employees on t1.IssuedBy equals e.Id
            //                                                       join e1 in _db.Employees on t2.RequisitionBy equals e1.EmployeeId
            //                                                       join e2 in _db.Employees on t1.AchknologeBy equals e2.Id into emp
            //                                                       from e3 in emp.DefaultIfEmpty()
            //                                                       select new VMPackagingPurchaseRequisition
            //                                                       {
            //                                                           IssueMasterId = t1.IssueMasterId,
            //                                                           RequisitionNo = t2.RequisitionNo,
            //                                                           RequisitionDate = t2.RequisitionDate.Value,
            //                                                           RequisitionBy = e1.Name,
            //                                                           IssueDate = t1.IssueDate,
            //                                                           ToDepartmentIssueName = s.Name,
            //                                                           FromDepartmentIssueName = s1.Name,
            //                                                           ToDepartmentIssueId = t1.ToDepartmentId,
            //                                                           IssueNo = t1.IssueNo,
            //                                                           IssueBy = e.Name,
            //                                                           AchknolagementIs = t1.Achknolagement == true ? "Achknowlaged" : "Pending",
            //                                                           AchknologeByName = e3.Name,
            //                                                           AcknologeDate = t1.AcknologeDate,
            //                                                           CompanyName = c.Name,
            //                                                           IntregrationFrom = "IssueMasterInfo"


            //                                                       }).FirstOrDefaultAsync(cancellationToken));

            //vmPackagingPurchaseRequisition.DataList = await Task.Run(() => (from t1 in _db.IssueDetailInfoes.Where(x => x.IssueMasterId == issueMasterId)
            //                                                                join t4 in _db.Products.Where(x => x.IsActive) on t1.RProductId equals t4.ProductId
            //                                                                join t5 in _db.ProductSubCategories.Where(x => x.IsActive) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
            //                                                                join t2 in _db.ProductCategories.Where(x => x.IsActive) on t5.ProductCategoryId equals t2.ProductCategoryId
            //                                                                join t3 in _db.RequisitionItemDetails.Where(x => x.IsActive) on t1.RequisitionItemDetailId equals t3.RequistionItemDetailId

            //                                                                select new VMPackagingPurchaseRequisition
            //                                                                {
            //                                                                    AccountingHeadId = t2.AccountingHeadId,
            //                                                                    IssueDetailsId = t1.IssueDetailId,
            //                                                                    ProductName = t5.Name + " " + t4.ProductName,
            //                                                                    IssueQty = t1.RMQ,
            //                                                                    AllocatedQuantity = t3.RQty,
            //                                                                    CostingPrice = t1.CostingPrice,
            //                                                                }).OrderByDescending(x => x.IssueDetailsId).ToList());



            return vmPackagingPurchaseRequisition;
        }

        public async Task<VMPackagingPurchaseRequisition> GetGeneralIssueList(int companyId, long issueMasterId, CancellationToken cancellationToken = default)
        {
            VMPackagingPurchaseRequisition vmPackagingPurchaseRequisition = new VMPackagingPurchaseRequisition();
            //vmPackagingPurchaseRequisition = await Task.Run(() => (from t1 in _db.IssueMasterInfoes.Where(x => x.CompanyId == companyId && x.IssueMasterId == issueMasterId)
            //                                                       join c in _db.Companies on t1.CompanyId equals c.CompanyId
            //                                                       join t2 in _db.Requisitions on t1.RequisitionId equals t2.RequisitionId
            //                                                       join s in _db.StockInfoes on t1.ToDepartmentId equals s.StockInfoId
            //                                                       join s1 in _db.StockInfoes on t1.FromDepartmentId equals s1.StockInfoId
            //                                                       join e in _db.Employees on t1.IssuedBy equals e.Id
            //                                                       join e1 in _db.Employees on t2.RequisitionBy equals e1.EmployeeId
            //                                                       join e2 in _db.Employees on t1.AchknologeBy equals e2.Id into emp
            //                                                       from e3 in emp.DefaultIfEmpty()
            //                                                       select new VMPackagingPurchaseRequisition
            //                                                       {
            //                                                           IssueMasterId = t1.IssueMasterId,
            //                                                           RequisitionNo = t2.RequisitionNo,
            //                                                           RequisitionDate = t2.RequisitionDate.Value,
            //                                                           RequisitionBy = e1.Name,
            //                                                           IssueDate = t1.IssueDate,
            //                                                           ToDepartmentIssueName = s.Name,
            //                                                           FromDepartmentIssueName = s1.Name,
            //                                                           ToDepartmentIssueId = t1.ToDepartmentId,
            //                                                           IssueNo = t1.IssueNo,
            //                                                           IssueBy = e.Name,
            //                                                           AchknolagementIs = t1.Achknolagement == true ? "Achknowlaged" : "Pending",
            //                                                           AchknologeByName = e3.Name,
            //                                                           AcknologeDate = t1.AcknologeDate,
            //                                                           CompanyName = c.Name,
            //                                                           CompanyId = t1.CompanyId,
            //                                                           IntregrationFrom = "IssueMasterInfo"


            //                                                       }).FirstOrDefaultAsync(cancellationToken));

            //vmPackagingPurchaseRequisition.DataList = await Task.Run(() => (from t1 in _db.IssueDetailInfoes.Where(x => x.IssueMasterId == issueMasterId)
            //                                                                join t4 in _db.Products.Where(x => x.IsActive) on t1.RProductId equals t4.ProductId
            //                                                                join t5 in _db.ProductSubCategories.Where(x => x.IsActive) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
            //                                                                join t2 in _db.ProductCategories.Where(x => x.IsActive) on t5.ProductCategoryId equals t2.ProductCategoryId
            //                                                                join t3 in _db.RequisitionItemDetails.Where(x => x.IsActive) on t1.RequisitionItemDetailId equals t3.RequistionItemDetailId


            //                                                                select new VMPackagingPurchaseRequisition
            //                                                                {
            //                                                                    AccountingHeadId = t2.AccountingHeadId,
            //                                                                    IssueDetailsId = t1.IssueDetailId,
            //                                                                    ProductName = t5.Name + " " + t4.ProductName,
            //                                                                    AllocatedQuantity = t3.RQty,
            //                                                                    IssueQty = t1.RMQ,
            //                                                                    CostingPrice = t1.CostingPrice
            //                                                                }).OrderByDescending(x => x.IssueDetailsId).ToList());




            return vmPackagingPurchaseRequisition;
        }



        public async Task<VMPackagingPurchaseRequisition> PackagingIssueItemList(int companyId)
        {
            VMPackagingPurchaseRequisition vmSalesOrder = new VMPackagingPurchaseRequisition();

            //vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.IssueMasterInfoes.Where(x => x.CompanyId == companyId && x.IsActive == true)
            //                                              join r in _db.Requisitions on t1.RequisitionId equals r.RequisitionId
            //                                              join e in _db.Employees on t1.IssuedBy equals e.Id
            //                                              join s in _db.StockInfoes on t1.FromDepartmentId equals s.StockInfoId
            //                                              join s1 in _db.StockInfoes on t1.ToDepartmentId equals s1.StockInfoId
            //                                              join ach in _db.Employees on t1.AchknologeBy equals ach.Id into achno
            //                                              from ach in achno.DefaultIfEmpty()
            //                                              where r.OrderDetailsId > 0 // General Requisition
            //                                              select new VMPackagingPurchaseRequisition
            //                                              {
            //                                                  IssueMasterId = t1.IssueMasterId,
            //                                                  IssueNo = t1.IssueNo,
            //                                                  IssueDate = t1.IssueDate,
            //                                                  FromDepartmentIssueId = t1.FromDepartmentId,
            //                                                  ToDepartmentIssueId = t1.ToDepartmentId,
            //                                                  FromDepartmentIssueName = s.Name,
            //                                                  ToDepartmentIssueName = s1.Name,
            //                                                  IssueBy = e.Name,
            //                                                  IssueById = t1.IssuedBy,
            //                                                  RequisitionNo = r.RequisitionNo,
            //                                                  RequisitionId = r.RequisitionId,
            //                                                  CompanyId = t1.CompanyId,
            //                                                  Achknolagement = t1.Achknolagement,
            //                                                  AchknologeBy = ach.Name,
            //                                                  AcknologeDate = t1.AcknologeDate

            //                                              }).OrderByDescending(x => x.IssueMasterId).AsEnumerable());
            return vmSalesOrder;
        }
        public async Task<VMPackagingPurchaseRequisition> PackagingNotIssueItemList(int companyId)
        {
            VMPackagingPurchaseRequisition vmSalesOrder = new VMPackagingPurchaseRequisition();

            //vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.Requisitions
            //                                              join t3 in _db.StockInfoes on t1.FromRequisitionId equals t3.StockInfoId
            //                                              join t4 in _db.StockInfoes on t1.ToRequisitionId equals t4.StockInfoId
            //                                              join t2 in _db.IssueMasterInfoes.Where(x=>x.CompanyId==companyId)
            //                                              on t1.RequisitionId equals t2.RequisitionId into joined
            //                                              from t2 in joined.DefaultIfEmpty()
            //                                              where t1.CompanyId == companyId && t2 == null && t1.IsActive
            //                                              select new VMPackagingPurchaseRequisition
            //                                              {
            //                                                  RequisitionNo=t1.RequisitionNo,
            //                                                  RequisitionId=t1.RequisitionId,
            //                                                  RequisitionDate=t1.RequisitionDate,
            //                                                  FromDepartmentReqId =(int)t1.FromRequisitionId,
            //                                                  ToDepartmentReqId =(int)t1.ToRequisitionId,
            //                                                  FromRequisitionName = t3.Name,
            //                                                  ToRequisitionName = t4.Name,

            //                                              }).OrderByDescending(x => x.RequisitionId).AsEnumerable());


            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.Requisitions.Where(x => x.IsActive && x.CompanyId == companyId && x.RequisitionType == 3 && x.RequisitionStatus == "N" && x.IsSubmitted)
                                                          join t2 in _db.OrderDetails.Where(x => x.IsActive) on t1.OrderDetailsId equals t2.OrderDetailId
                                                          join t3 in _db.OrderMasters.Where(x => x.IsActive) on t2.OrderMasterId equals t3.OrderMasterId
                                                          join t4 in _db.Products.Where(x => x.IsActive) on t2.ProductId equals t4.ProductId
                                                          join t5 in _db.ProductSubCategories.Where(x => x.IsActive) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                          join t6 in _db.StockInfoes.Where(x => x.IsActive) on t1.FromRequisitionId equals t6.StockInfoId
                                                          join t7 in _db.StockInfoes.Where(x => x.IsActive) on t1.ToRequisitionId equals t7.StockInfoId


                                                          select new VMPackagingPurchaseRequisition
                                                          {
                                                              RequisitionId = t1.RequisitionId,
                                                              RequisitionNo = t1.RequisitionNo,
                                                              RequisitionDate = t1.RequisitionDate.Value,

                                                              RequisitionStatus = t1.RequisitionStatus,
                                                              CompanyId = t1.CompanyId.Value,
                                                              IsSubmited = t1.IsSubmitted,
                                                              OrderNo = t3.OrderNo,
                                                              JobOrderNo = t2.JobOrderNo,
                                                              FromRequisitionName = t6.Name,
                                                              ToRequisitionName = t7.Name,
                                                              ProductName = t5.Name + " " + t4.ProductName,
                                                              Description = t2.Remarks,
                                                              OrderMasterId = (long)t2.OrderMasterId

                                                          }).OrderByDescending(x => x.RequisitionId).AsEnumerable());
            return vmSalesOrder;
        }

        public async Task<VMPackagingPurchaseRequisition> GeneralPackagingNotIssueItemList(int companyId)
        {

            VMPackagingPurchaseRequisition vmSalesOrder = new VMPackagingPurchaseRequisition();

            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.Requisitions.Where(x => x.IsActive && x.CompanyId == companyId && x.RequisitionType == 3 && x.RequisitionStatus == "N" && x.IsSubmitted && x.OrderDetailsId == 0)
                                                          join t6 in _db.StockInfoes.Where(x => x.IsActive) on t1.FromRequisitionId equals t6.StockInfoId
                                                          join t7 in _db.StockInfoes.Where(x => x.IsActive) on t1.ToRequisitionId equals t7.StockInfoId


                                                          select new VMPackagingPurchaseRequisition
                                                          {
                                                              RequisitionId = t1.RequisitionId,
                                                              RequisitionNo = t1.RequisitionNo,
                                                              RequisitionDate = t1.RequisitionDate.Value,

                                                              RequisitionStatus = t1.RequisitionStatus,
                                                              CompanyId = t1.CompanyId.Value,
                                                              IsSubmited = t1.IsSubmitted,
                                                              FromRequisitionName = t6.Name,
                                                              ToRequisitionName = t7.Name,

                                                          }).OrderByDescending(x => x.RequisitionId).AsEnumerable());
            return vmSalesOrder;
        }

        public async Task<VMPackagingPurchaseRequisition> PackagingGIssueItemList(int companyId)
        {
            VMPackagingPurchaseRequisition vmSalesOrder = new VMPackagingPurchaseRequisition();

            //vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.IssueMasterInfoes.Where(x => x.CompanyId == companyId && x.IsActive == true)
            //                                              join r in _db.Requisitions on t1.RequisitionId equals r.RequisitionId
            //                                              join e in _db.Employees on t1.IssuedBy equals e.Id
            //                                              join s in _db.StockInfoes on t1.FromDepartmentId equals s.StockInfoId
            //                                              join s1 in _db.StockInfoes on t1.ToDepartmentId equals s1.StockInfoId
            //                                              join ach in _db.Employees on t1.AchknologeBy equals ach.Id into achno
            //                                              from ach in achno.DefaultIfEmpty()
            //                                              where r.OrderDetailsId == 0 // General Requisition
            //                                              select new VMPackagingPurchaseRequisition
            //                                              {
            //                                                  IssueMasterId = t1.IssueMasterId,
            //                                                  IssueNo = t1.IssueNo,
            //                                                  IssueDate = t1.IssueDate,
            //                                                  FromDepartmentIssueId = t1.FromDepartmentId,
            //                                                  ToDepartmentIssueId = t1.ToDepartmentId,
            //                                                  FromDepartmentIssueName = s.Name,
            //                                                  ToDepartmentIssueName = s1.Name,
            //                                                  IssueBy = e.Name,
            //                                                  IssueById = t1.IssuedBy,
            //                                                  RequisitionNo = r.RequisitionNo,
            //                                                  RequisitionId = r.RequisitionId,
            //                                                  CompanyId = t1.CompanyId,
            //                                                  Achknolagement = t1.Achknolagement,
            //                                                  AchknologeBy = ach.Name,
            //                                                  AcknologeDate = t1.AcknologeDate

            //                                              }).OrderByDescending(x => x.IssueMasterId).AsEnumerable());
            return vmSalesOrder;
        }

        public async Task<VMPackagingPurchaseRequisition> PackagingUnAchknoledgedIssueItemList(int companyId)
        {
            VMPackagingPurchaseRequisition vmSalesOrder = new VMPackagingPurchaseRequisition();

            //vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.IssueMasterInfoes.Where(x => x.CompanyId == companyId && x.IsActive == true)
            //                                              join r in _db.Requisitions on t1.RequisitionId equals r.RequisitionId
            //                                              join e in _db.Employees on t1.IssuedBy equals e.Id
            //                                              join s in _db.StockInfoes on t1.FromDepartmentId equals s.StockInfoId
            //                                              join s1 in _db.StockInfoes on t1.ToDepartmentId equals s1.StockInfoId
            //                                              join ach in _db.Employees on t1.AchknologeBy equals ach.Id into achno
            //                                              from ach in achno.DefaultIfEmpty()
            //                                              where !t1.Achknolagement
            //                                              select new VMPackagingPurchaseRequisition
            //                                              {
            //                                                  IssueMasterId = t1.IssueMasterId,
            //                                                  IssueNo = t1.IssueNo,
            //                                                  IssueDate = t1.IssueDate,
            //                                                  FromDepartmentIssueId = t1.FromDepartmentId,
            //                                                  ToDepartmentIssueId = t1.ToDepartmentId,
            //                                                  FromDepartmentIssueName = s.Name,
            //                                                  ToDepartmentIssueName = s1.Name,
            //                                                  IssueBy = e.Name,
            //                                                  IssueById = t1.IssuedBy,
            //                                                  RequisitionNo = r.RequisitionNo,
            //                                                  RequisitionId = r.RequisitionId,
            //                                                  CompanyId = t1.CompanyId,
            //                                                  Achknolagement = t1.Achknolagement,
            //                                                  AchknologeBy = ach.Name,
            //                                                  AcknologeDate = t1.AcknologeDate

            //                                              }).OrderByDescending(x => x.IssueMasterId).AsEnumerable());
            return vmSalesOrder;
        }

        public async Task<VmDemandService> GetRequisitionList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VmDemandService vmOrder = new VmDemandService();
            vmOrder.CompanyFK = companyId;
            vmOrder.dataList = await Task.Run(() => (from t1 in _db.Demands
                                                     .Where(x => x.IsActive && x.CompanyId == companyId && x.DemandDate >= fromDate && x.DemandDate <= toDate)
                                                     join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                     select new VmDemandService
                                                     {
                                                         DemandId = t1.DemandId,
                                                         DemandNo = t1.DemandNo,
                                                         CID = t1.DemandNo,
                                                         CreatedDate = t1.CreatedDate,
                                                         CreatedBy = t1.CreatedBy,
                                                         ModifiedBy = t1.ModifiedBy,
                                                         DemandDate = t1.DemandDate.Value,
                                                         CompanyFK = t1.CompanyId,
                                                         CompanyId = t1.CompanyId.Value,
                                                         CompanyName = t2.Name,
                                                         CustomerId = t1.CustomerId.Value,
                                                         CustomerPaymentMethodEnumFK = t1.PaymentMethod.Value,
                                                         IsSubmitted = t1.IsSubmitted,
                                                         IsInvoiceCreated = t1.IsInvoiceCreated,
                                                         HeadGLId = t1.HeadGLId,
                                                         PayAmount = t1.Amount,
                                                         SalesStatus = t1.SalesStatus,
                                                         CreditStatus = t1.CreditStatus
                                                     }).OrderByDescending(x => x.DemandId).AsEnumerable());
            return vmOrder;
        }
        public async Task<long> DemandMastersDelete(long demandId)
        {
            var result = -1;
            Demand model = await _db.Demands.FindAsync(demandId);

            model.IsActive = false;
            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)demandId;
            }

            var itemlist = _db.DemandItems.Where(h => h.DemandId == demandId && h.IsActive).ToList();
            foreach (var item in itemlist)
            {
                item.IsActive = false;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
            }


            return result;
        }
        public async Task<VmDemandItemService> GetDemanMasters(int demandOrderId)
        {

            VmDemandItemService model = new VmDemandItemService();
            if (demandOrderId != 0)
            {

                var list = _db.Demands.Where(d => d.DemandId == demandOrderId).FirstOrDefault();
                var cus = _db.Vendors.Find(list.CustomerId);
                model.HeadGLId = list.HeadGLId;
                model.PayAmount = list.Amount;
                model.SalesStatus = list.SalesStatus;
                model.CreditStatus = list.CreditStatus;
                model.DemandId = list.DemandId;
                model.DemandNo = list.DemandNo;
                model.CID = list.DemandNo;
                model.CreatedDate = list.CreatedDate;
                model.CreatedBy = list.CreatedBy;
                model.ModifiedBy = list.ModifiedBy;
                model.ModifiedBy = list.ModifiedBy;
                model.DemandDate = (DateTime)list.DemandDate;
                model.CompanyName = list.CompanyId == null ? "" : _db.Companies.Find(list.CompanyId).Name;
                model.CustomerName = list.CustomerId == null ? "" : _db.Vendors.Find(list.CustomerId).Name;
                model.StockInfoName = list.StockInfoId == null ? "" : _db.StockInfoes.Find(list.StockInfoId).Name;
                model.CompanyId = list.CompanyId.Value;
                model.CompanyFK = list.CompanyId;
                model.CustomerId = list.CustomerId.Value;
                model.CustomerPaymentMethodEnumFK = list.PaymentMethod.Value;
                model.StockInfoId = list.StockInfoId.Value;
                model.GlobalDiscount = list.Discount;
                model.DiscountAmount = list.DicountAmount;
                model.IsSubmitted = list.IsSubmitted;
                model.SubZoneFk = cus.SubZoneId == null ? 0 : cus.SubZoneId;
                model.Remarks = list.Remarks;
                model.SubZoneFkName = cus.SubZoneId == null ? "" : _db.SubZones.Find(cus.SubZoneId).Name;
                return model;
            }

            return model;
        }
        public async Task<VmDemandItemService> DemandOrderSlaveGet(int companyId, int demandOrderId)
        {
            VmDemandItemService model = new VmDemandItemService();
            if (companyId != 0 && demandOrderId != 0)
            {
                var list = _db.Demands.Where(d => d.CompanyId == companyId && d.DemandId == demandOrderId).FirstOrDefault();
                model.DemandId = list.DemandId;
                model.DemandNo = list.DemandNo;
                model.CID = list.DemandNo;
                model.CreatedDate = list.CreatedDate;
                model.CreatedBy = list.CreatedBy;
                model.ModifiedBy = list.ModifiedBy;
                model.ModifiedBy = list.ModifiedBy;
                model.DemandDate = list.DemandDate.Value;
                model.CompanyName = list.CompanyId == null ? "" : _db.Companies.Find(companyId).Name;
                model.AccCode = list.HeadGLId == 0 ? "" : _db.HeadGLs.Find(list.HeadGLId).AccCode;
                model.AccName = list.HeadGLId == 0 ? "" : _db.HeadGLs.Find(list.HeadGLId).AccName;
                model.CustomerName = list.CustomerId == null ? "" : _db.Vendors.Find(list.CustomerId).Name;
                model.StockInfoName = list.StockInfoId == null ? "" : _db.StockInfoes.Find(list.StockInfoId).Name;
                model.CompanyId = list.CompanyId.Value;
                model.CompanyFK = list.CompanyId;
                model.CustomerId = list.CustomerId.Value;
                model.CustomerPaymentMethodEnumFK = list.PaymentMethod.Value;
                model.StockInfoId = list.StockInfoId.Value;
                model.GlobalDiscount = list.Discount;
                model.DiscountAmount = list.DicountAmount;
                model.IsSubmitted = list.IsSubmitted;
                model.HeadGLId = list.HeadGLId;
                model.PayAmount = list.Amount;
                model.SalesStatus = list.SalesStatus;
                model.CreditStatus = list.CreditStatus;
                model.vmDemandItems = await Task.Run(() => (from t1 in _db.DemandItems.Where(x => x.DemandId == demandOrderId && x.IsActive == true)

                                                            join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                            join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                            join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                            join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                            select new VmDemandItemService
                                                            {
                                                                DemandItemId = t1.DemandItemId,
                                                                DemandId = t1.DemandId.Value,
                                                                ProductId = t1.ProductId.Value,
                                                                ProductName = t3.ProductName,
                                                                ProductCategories = t5.Name,
                                                                ProductSubCategories = t4.Name,
                                                                ItemQuantity = t1.Qty,
                                                                UnitPrice = t1.UnitPrice,
                                                                ProductPrice = t1.ProductPrice,
                                                                UnitName = t6.Name,
                                                                ProductDiscountUnit = t1.DiscountUnit,//Unit Discount                                                               
                                                                CashDiscountPercent = t1.DiscountRate, // Cash Discount                                                               
                                                                SpecialDiscount = t1.SpecialDiscount, // SpecialDiscount                                                               
                                                                TotalAmount = t1.Qty * t1.UnitPrice,
                                                            }).OrderByDescending(x => x.DemandItemId).AsEnumerable());
                //model.GrossAmount = (decimal)model.vmDemandItems.Where(f => f.DemandId == demandOrderId).Select(f => f.TotalAmount).DefaultIfEmpty(0).Sum();
                //model.ProductDiscount = (decimal)model.vmDemandItems.Where(f => f.DemandId == demandOrderId).Select(f => f.ProductDiscountTotal).DefaultIfEmpty(0).Sum();
                //var greentotal = model.GrossAmount - model.ProductDiscount;
                //model.TotalAmountAfterDiscount = greentotal - model.DiscountAmount;
                return model;
            }

            return model;
        }




        public async Task<long> DemandhaseOrderAdd(VmDemandItemService DemandOrderSlave)
        {
            long result = -1;
            var poMax = _db.Demands.Where(x => x.CompanyId == DemandOrderSlave.CompanyFK).Count() + 1;
            string poCid = @"PRF#" +


                             poMax.ToString();

            Demand model = new Demand()
            {
                CompanyId = DemandOrderSlave.CompanyFK,
                DemandNo = poCid,
                RequisitionType = DemandOrderSlave.RequisitionType,
                CustomerId = DemandOrderSlave.CustomerId,
                StockInfoId = DemandOrderSlave.StockInfoId,
                PaymentMethod = DemandOrderSlave.CustomerPaymentMethodEnumFK,
                DemandDate = DemandOrderSlave.DemandDate,
                Remarks = DemandOrderSlave.Remarks,
                IsActive = true,
                CreatedDate = DateTime.Now,
                DicountAmount = DemandOrderSlave.DiscountAmount,
                Discount = DemandOrderSlave.DiscountAmount,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                IsSubmitted = false,
                HeadGLId = DemandOrderSlave.HeadGLId,
                SalesStatus = DemandOrderSlave.SalesStatus,
                CreditStatus = DemandOrderSlave.CreditStatus,
                Amount = DemandOrderSlave.PayAmount,


            };

            DemandItem demandItem = new DemandItem();
            demandItem.ProductId = ((int)DemandOrderSlave.ProductId);
            demandItem.Qty = DemandOrderSlave.ItemQuantity;
            demandItem.UnitPrice = DemandOrderSlave.UnitPrice;
            demandItem.ProductPrice = DemandOrderSlave.ProductPrice;
            demandItem.DiscountUnit = DemandOrderSlave.ProductDiscountUnit; // Unit Discount
            demandItem.DiscountRate = DemandOrderSlave.CashDiscountPercent; // Cash Discount
            demandItem.SpecialDiscount = DemandOrderSlave.SpecialDiscount;  // Special Discount

            demandItem.DiscountAmount = 0;//(Convert.ToDecimal(DemandOrderSlave.ItemQuantity) * DemandOrderSlave.ProductDiscountUnit),
                                          // ((Convert.ToDecimal(DemandOrderSlave.ItemQuantity) * DemandOrderSlave.ProductDiscountUnit) * 100) / Convert.ToDecimal((DemandOrderSlave.ItemQuantity * DemandOrderSlave.UnitPrice)),
            demandItem.CreatedDate = DateTime.Now;
            demandItem.IsActive = true;
            demandItem.CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString();


            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Demands.Add(model);
                    _db.SaveChanges();
                    demandItem.DemandId = model.DemandId;
                    _db.DemandItems.Add(demandItem);
                    _db.SaveChanges();
                    scope.Commit();
                    result = model.DemandId;
                    return result;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return result;
                }
            }
        }

        public async Task<long> DemandItemAdd(VmDemandItemService DemandOrderSlave)
        {
            long result = -1;
            DemandItem demandItem = new DemandItem()
            {
                DemandId = DemandOrderSlave.DemandId,
                ProductId = DemandOrderSlave.ProductId,
                Qty = DemandOrderSlave.ItemQuantity,
                UnitPrice = DemandOrderSlave.UnitPrice,
                DiscountUnit = DemandOrderSlave.ProductDiscountUnit, // Unit Discount
                DiscountRate = DemandOrderSlave.CashDiscountPercent, // Cash Discount
                SpecialDiscount = DemandOrderSlave.SpecialDiscount,  // Special Discount

                DiscountAmount = 0,//(Convert.ToDecimal(DemandOrderSlave.ItemQuantity) * DemandOrderSlave.ProductDiscountUnit),
                // ((Convert.ToDecimal(DemandOrderSlave.ItemQuantity) * DemandOrderSlave.ProductDiscountUnit) * 100) / Convert.ToDecimal((DemandOrderSlave.ItemQuantity * DemandOrderSlave.UnitPrice)),
                CreatedDate = DateTime.Now,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                IsActive = true,
                ProductPrice = DemandOrderSlave.ProductPrice,

            };
            _db.DemandItems.Add(demandItem);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = DemandOrderSlave.DemandId;
            }
            return result;
        }

        public async Task<VmDemandItemService> GetSingleDemandItem(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.DemandItems
                                          join t2 in _db.Demands.Where(x => x.IsActive) on t1.DemandId equals t2.DemandId
                                          join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                          join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                          where t1.DemandItemId == id
                                          select new VmDemandItemService
                                          {
                                              DemandItemId = t1.DemandItemId,
                                              DemandId = t1.DemandId.Value,
                                              ProductId = t1.ProductId.Value,
                                              ProductName = t3.ProductName,
                                              ProductCategories = t4.Name,
                                              ItemQuantity = t1.Qty,
                                              UnitPrice = t1.UnitPrice,
                                              ProductPrice = t1.ProductPrice,
                                              DiscountRate = t1.DiscountRate,
                                              ProductDiscountUnit = t1.DiscountUnit,
                                              CompanyFK = t2.CompanyId,
                                              CompanyId = t2.CompanyId.Value,
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<long> DemandItemEdit(VmDemandItemService DemandOrderSlave)
        {
            var result = -1;
            DemandItem demandItem = await _db.DemandItems.FindAsync(DemandOrderSlave.DemandItemId);
            demandItem.ProductId = DemandOrderSlave.ProductId;
            demandItem.Qty = DemandOrderSlave.ItemQuantity;
            demandItem.UnitPrice = DemandOrderSlave.UnitPrice;
            demandItem.DiscountUnit = DemandOrderSlave.ProductDiscountUnit;
            demandItem.DiscountAmount = (Convert.ToDecimal(DemandOrderSlave.ItemQuantity) * DemandOrderSlave.ProductDiscountUnit);
            demandItem.DiscountRate = ((Convert.ToDecimal(DemandOrderSlave.ItemQuantity) * DemandOrderSlave.ProductDiscountUnit) * 100) / Convert.ToDecimal((DemandOrderSlave.ItemQuantity * DemandOrderSlave.UnitPrice));
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)DemandOrderSlave.DemandId;
            }
            return result;
        }

        public async Task<int> DemandDiscountEdit(VmDemandItemService DemandOrderSlave)
        {
            var result = -1;
            Demand model = await _db.Demands.FindAsync(DemandOrderSlave.DemandId);

            model.Discount = DemandOrderSlave.GlobalDiscount;
            model.DicountAmount = DemandOrderSlave.DiscountAmount;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = DemandOrderSlave.ID;
            }

            return result;
        }



        public async Task<int> DemandhaseOrderUpdate(VmDemandItemService DemandOrderSlave)
        {
            var result = -1;
            Demand model = await _db.Demands.FindAsync(DemandOrderSlave.DemandId);

            if (model.IsSubmitted == true)
            {
                model.IsSubmitted = false;
            }
            else
            {
                model.IsSubmitted = true;
            }
            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)model.DemandId;
            }

            return result;
        }

        public async Task<long> UpdateDemandfeed(VmDemandItemService model)
        {
            long result = -1;
            var demand = _db.Demands.Find(model.DemandId);
            demand.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            demand.ModifiedDate = DateTime.Now;
            demand.DemandDate = model.DemandDate;
            demand.HeadGLId = model.HeadGLId;//Bank id
            demand.StockInfoId = model.StockInfoId;
            demand.CustomerId = model.CustomerId;
            demand.Amount = model.PayAmount;
            demand.SalesStatus = model.SalesStatus;
            demand.CreditStatus = model.CreditStatus;
            demand.Remarks = model.Remarks;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)model.DemandId;
            }
            return result;
        }
        public async Task<long> UpdateDemand(VmDemandItemService model)
        {
            var result = -1;
            Demand demand = await _db.Demands.FindAsync(model.DemandId);
            demand.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            demand.ModifiedDate = DateTime.Now;
            demand.DemandDate = model.DemandDate;
            demand.DemandDate = model.DemandDate;
            demand.PaymentMethod = model.CustomerPaymentMethodEnumFK;
            demand.StockInfoId = model.StockInfoId;
            demand.CustomerId = model.CustomerId;
            demand.Remarks = model.Remarks;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)model.DemandId;
            }
            return result;
        }
        public async Task<long> DemandItemDelete(VmDemandItemService DemandOrderSlave)
        {
            var result = -1;
            DemandItem demandItem = await _db.DemandItems.FindAsync(DemandOrderSlave.DemandItemId);
            demandItem.IsActive = false;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)DemandOrderSlave.DemandId;
            }
            return result;
        }


        public async Task<Officervwmodel> OfficerName(int SubzoneId)
        {
            var v = await Task.Run(() => (from t1 in _db.OfficerAssigns
                                          join t2 in _db.Employees on t1.EmpId equals t2.Id
                                          where t1.SubZoneId == SubzoneId && t1.IsActive
                                          select new Officervwmodel
                                          {
                                              EmployeeName = t2.Name,
                                              EmpId = t1.EmpId
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<Officervwmodel> OfficerofTerritoryName(int SubzoneId)
        {
            var v = await Task.Run(() => (from t1 in _db.SubZones
                                          join t2 in _db.Employees on t1.SalesOfficerId equals t2.Id
                                          where t1.SubZoneId == SubzoneId && t1.IsActive
                                          select new Officervwmodel
                                          {
                                              EmployeeName = t2.Name,
                                              EmpId = t1.SalesOfficerId ?? 0
                                          }).FirstOrDefault());
            return v;
        }




        public async Task<VmDemandItemService> GetDemandsById(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.Demands
                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                          join t3 in _db.StockInfoes on t1.StockInfoId equals t3.StockInfoId
                                          join t4 in _db.SubZones on t2.SubZoneId equals t4.SubZoneId

                                          where t1.DemandId == id
                                          select new VmDemandItemService
                                          {
                                              SubZoneFk = t2.SubZoneId,
                                              SubZoneFkName = t4.Name,
                                              DemandNo = t1.DemandNo,
                                              CID = t1.DemandNo,
                                              CreatedDate = t1.CreatedDate,
                                              CreatedBy = t1.CreatedBy,
                                              ModifiedBy = t1.ModifiedBy,
                                              DemandDate = t1.DemandDate.Value,
                                              CustomerName = t2.Name,
                                              StockInfoName = t3.Name,
                                              CompanyId = t1.CompanyId.Value,
                                              CompanyFK = t1.CompanyId,
                                              CustomerId = t1.CustomerId.Value,
                                              CustomerPaymentMethodEnumFK = t1.PaymentMethod.Value,
                                              StockInfoId = t1.StockInfoId.Value,
                                              GlobalDiscount = t1.Discount,
                                              DiscountAmount = t1.DicountAmount,
                                              IsSubmitted = t1.IsSubmitted,
                                              HeadGLId = t1.HeadGLId,
                                              PayAmount = t1.Amount,
                                              SalesStatus = t1.SalesStatus,
                                              CreditStatus = t1.CreditStatus,
                                          }).FirstOrDefault());
            return v;
        }


        //added by hridoy 07-apr-2022
        public async Task<List<DropDown>> DemandsDropDownList(int customerId, int companyId = 0)
        {
            var List = await _db.Demands.Where(e => e.CompanyId == companyId && e.CustomerId == customerId && e.IsSubmitted == true && e.IsInvoiceCreated == false && e.IsActive)
                 .Select(o => new DropDown { Value = o.DemandId.ToString(), Text = o.DemandNo }).ToListAsync();

            return List;
        }



        public object GetAutoCompleteDemandGet(string prefix, int companyId)
        {
            var v = (from t1 in _db.Demands.Where(x => x.CompanyId == companyId && x.IsInvoiceCreated == false)
                     where t1.IsActive && t1.IsSubmitted == true && ((t1.DemandNo.StartsWith(prefix)))

                     select new
                     {
                         label = t1.DemandNo,
                         val = t1.DemandId

                     }).OrderByDescending(x => x.label).ToList();

            return v;
        }


        public LcInfoViewModel LCValue(int ComapanyId)
        {
            LcInfoViewModel model = new LcInfoViewModel();
            model.DataList = (from t1 in _db.LCInfoes
                              where t1.CompanyId == ComapanyId && t1.IsActive
                              select new LcInfoViewModel
                              {
                                  LCId = t1.LCId,
                                  LCNo = t1.LCNo,
                                  LCDate = t1.LCDate,

                              }).OrderByDescending(x => x.LCDate).ToList();
            return model;
        }

        //public LcInfoViewModel KFMALLCValue(int ComapanyId)
        //{
        //    LcInfoViewModel model = new LcInfoViewModel();
        //    model = (from t1 in _db.LCInfoes
        //                      where t1.CompanyId == ComapanyId && t1.IsActive
        //                      select new LcInfoViewModel
        //                      {
        //                          LCId = t1.LCId,
        //                          LCNo = t1.LCNo,
        //                          LCDate = t1.LCDate,

        //                      }).OrderByDescending(x => x.LCDate).ToList();
        //    return model;
        //}



        public LcInfoViewModel LCValueForAppend(int Id)
        {
            LcInfoViewModel model = new LcInfoViewModel();
            model = (from t1 in _db.LCInfoes
                     join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                     join t3 in _db.BankBranches on t1.SupplierBankBranchId equals t3.BankBranchId
                     join t3_1 in _db.Banks on t3.BankId equals t3_1.BankId
                     join t0 in _db.BankBranches on t1.CompanyBankBranchId equals t0.BankBranchId
                     join t0_1 in _db.Banks on t0.BankId equals t0_1.BankId

                     join t5 in _db.Countries on t1.CountryofOriginId equals t5.CountryId
                     join t6 in _db.Currencies on t1.CurrenceyId equals t6.CurrencyId
                     where t1.LCId == Id && t1.IsActive
                     select new LcInfoViewModel
                     {
                         LCId = t1.LCId,
                         LCNo = t1.LCNo,
                         LCDate = t1.LCDate,
                         SupplierName = t2.Name,
                         CompanyBankName = t0_1.Name,
                         SupllierBankName = t3_1.Name,
                         CountryName = t5.CountryName,
                         SupplierId = t2.VendorId,
                         CurrenceyId = t1.CurrenceyId,
                         CurrenceyName = t6.Name

                     }).OrderByDescending(x => x.LCDate).FirstOrDefault();
            return model;
        }

        //Written by hridoy 
        public async Task<long> OrderMasterAddForPRF(VMSalesOrderSlave vmSalesOrderSlave, List<VmDemandItemService> demandItems)
        {
            long result = -1;
            var poMax = _db.OrderMasters.Where(x => x.CompanyId == vmSalesOrderSlave.CompanyFK && !x.IsOpening).Count() + 1;
            string poCid = "";

            if (vmSalesOrderSlave.CompanyFK.Value == (int)CompanyName.KrishibidSeedLimited)
            {
                poCid = poMax.ToString();
            }
            else if (vmSalesOrderSlave.CompanyFK.Value == (int)CompanyName.GloriousCropCareLimited)
            {
                poCid = @"SI#" + poMax.ToString();
            }
            else if (vmSalesOrderSlave.CompanyFK.Value == (int)CompanyName.KrishibidFeedLimited)
            {
                poCid = vmSalesOrderSlave.OrderNo;
            }
            else
            {
                poCid =
                           @"SO-" +
                                DateTime.Now.ToString("yy") +
                                DateTime.Now.ToString("MM") +
                                DateTime.Now.ToString("dd") + "-" +

                           poMax.ToString();
            }

            OrderMaster orderMaster = new OrderMaster
            {
                TotalAmount = (decimal)vmSalesOrderSlave.TotalAmount,
                GrandTotal = vmSalesOrderSlave.GrandTotal,
                DiscountRate = vmSalesOrderSlave.DiscountRate,
                DiscountAmount = vmSalesOrderSlave.DiscountAmount,
                OrderStatus = vmSalesOrderSlave.CompanyFK == 8 ? "N" : "",
                OrderMonthYear = vmSalesOrderSlave.OrderDate.Year.ToString() + vmSalesOrderSlave.OrderDate.Day.ToString(),
                OrderNo = poCid,
                OrderDate = vmSalesOrderSlave.OrderDate,
                CustomerId = vmSalesOrderSlave.CustomerId,
                DemandId = vmSalesOrderSlave.DemandId,
                ExpectedDeliveryDate = vmSalesOrderSlave.ExpectedDeliveryDate,
                PaymentMethod = vmSalesOrderSlave.CustomerPaymentMethodEnumFK,
                ProductType = "F",
                Status = (int)EnumPOStatus.Draft,
                CourierNo = vmSalesOrderSlave.CourierNo,
                FinalDestination = vmSalesOrderSlave.FinalDestination,
                CourierCharge = vmSalesOrderSlave.CourierCharge,
                CurrentPayable = Convert.ToDecimal(vmSalesOrderSlave.PayableAmount),
                StockInfoId = vmSalesOrderSlave.StockInfoId,
                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),// System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                Remarks = vmSalesOrderSlave.Remarks,
                IsActive = true
            };

            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.OrderMasters.Add(orderMaster);
                    if (await _db.SaveChangesAsync() > 0)
                    {
                        result = orderMaster.OrderMasterId;
                    }
                    List<OrderDetail> LstOrderDetails = new List<OrderDetail>();
                    foreach (var item in demandItems)
                    {
                        LstOrderDetails.Add(new OrderDetail
                        {
                            OrderMasterId = result,
                            DemandItemId = Convert.ToInt32(item.DemandItemId),
                            ProductId = item.ProductId,
                            Qty = item.ItemQuantity,
                            UnitPrice = item.UnitPrice,
                            Amount = (item.ItemQuantity * item.UnitPrice),
                            Comsumption = null,
                            PackQuantity = null,
                            DiscountUnit = item.ProductDiscountUnit,
                            DiscountAmount = 0,
                            DiscountRate = item.CashDiscountPercent,
                            SpecialBaseCommission = item.SpecialDiscount,
                            CompanyId = vmSalesOrderSlave.CompanyFK,
                            CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                            CreateDate = DateTime.Now,
                            StyleNo = Convert.ToString(DateTime.Now.Ticks),
                            IsActive = true
                        });
                    }
                    _db.OrderDetails.AddRange(LstOrderDetails);
                    await _db.SaveChangesAsync();
                    var demand = await _db.Demands.FindAsync(orderMaster.DemandId);
                    if (demand != null)
                    {
                        demand.IsInvoiceCreated = true;
                        await _db.SaveChangesAsync();
                    }
                    scope.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }
        //ends hridoy


        public async Task<VmDemandItemService> GetDemandItemPartial(int DemandId)
        {
            VmDemandItemService vmDemandItemService = new VmDemandItemService();

            vmDemandItemService.DemandItemList = await Task.Run(() => (from t1 in _db.DemandItems
                                                                       join t2 in _db.Demands on t1.DemandId equals t2.DemandId
                                                                       join t3 in _db.Products on t1.ProductId equals t3.ProductId
                                                                       join t4 in _db.ProductSubCategories on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                       join t5 in _db.Units on t3.UnitId equals t5.UnitId

                                                                       where t1.DemandId == DemandId && t1.IsActive && t2.IsActive
                                                                       select new VmDemandItemService
                                                                       {
                                                                           ProductName = t4.Name + " " + t3.ProductName,
                                                                           ProductId = t1.ProductId.Value,
                                                                           ItemQuantity = t1.Qty,
                                                                           UnitPrice = t1.UnitPrice,
                                                                           ProductPrice = t1.ProductPrice,
                                                                           UnitName = t5.Name,
                                                                           DemandId = t2.DemandId,
                                                                           DemandItemId = t1.DemandItemId,
                                                                           ProductDiscountUnit = t1.DiscountUnit,
                                                                           CashDiscountPercent = t1.DiscountRate,
                                                                           SpecialDiscount = t1.SpecialDiscount

                                                                       }).ToList());
            return vmDemandItemService;



        }

        public PurchaseOrder EditPurchasebasic(VMPurchaseOrderSlave Model)
        {
            var objtosave = _db.PurchaseOrders.Where(x => x.PurchaseOrderId == Model.PurchaseOrderId).FirstOrDefault();
            if (objtosave != null)
            {
                objtosave.CompanyId = Model.CompanyId;
                objtosave.DeliveryAddress = Model.DeliveryAddress;
                objtosave.LCId = Model.LCId;
                objtosave.DeliveryDate = Model.DeliveryDate;
                objtosave.PurchaseDate = Model.OrderDate;
                _db.SaveChanges();

            }
            return objtosave;
        }

        public async Task<List<SelectModelEmp>> GetSaleEmplyee(int CompanyId)
        {
            var list = await (
               from t1 in _db.Employees
               where t1.CompanyId == CompanyId
               select new SelectModelEmp
               {
                   Value = t1.Id,
                   Text = t1.Name
               }).ToListAsync();

            return list;
        }


        public bool SaveSalesTargets(OfficerSalestargetVm model)
        {
            var obj = _db.OfficerSalesTargets
                     .Where(x => x.EmployeeId == model.EmployeeId && x.Year == model.Year)
                     .FirstOrDefault();

            if (obj == null)
            {
                try
                {
                    List<OfficerSalesTarget> officerSalesList = new List<OfficerSalesTarget>();

                    foreach (var item in model.officerSalestargetVms)
                    {

                        // Assuming item contains data for multiple months
                        OfficerSalesTarget officerSalesTarget = new OfficerSalesTarget
                        {

                            Month = item.Month,
                            Year = model.Year,
                            EmployeeId = model.EmployeeId,
                            CompanyId = model.CompanyId,
                            TargetedQuantity = item.TargetedQuantity,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            IsApproved = false,
                            ApprovedBy = 0

                        };
                        officerSalesList.Add(officerSalesTarget);
                    }
                    _db.OfficerSalesTargets.AddRange(officerSalesList);
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {

                    return false;
                }
            }
            else
            {
                return false;
            }




        }


        public bool EditSalesTaget(long OfficerSalesTargetId, decimal newQuantity)
        {
            var obj = _db.OfficerSalesTargets
                     .Where(x => x.OfficerSalesTargetId == OfficerSalesTargetId).FirstOrDefault();

            if (obj != null)
            {
                obj.TargetedQuantity = newQuantity;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                _db.SaveChanges();
                return true;

            }

            return false;

        }







        public List<OfficerSalestargetVm> GetTargetYear(long EmpId, int Year)
        {
            var query = from t1 in _db.OfficerSalesTargets
                        join employee in _db.Employees on t1.EmployeeId equals employee.Id
                        where t1.EmployeeId == EmpId && t1.Year == Year
                        select new OfficerSalestargetVm
                        {

                            OfficerSalesTargetId = t1.OfficerSalesTargetId,
                            Month = t1.Month,
                            Year = t1.Year,
                            EmployeeId = t1.EmployeeId,
                            CompanyId = t1.CompanyId,
                            TargetedQuantity = t1.TargetedQuantity,
                            CreatedBy = t1.CreatedBy,
                            CreatedDate = t1.CreatedDate,
                            ModifiedBy = t1.ModifiedBy,
                            ModifiedDate = t1.ModifiedDate,
                            IsActive = t1.IsActive,
                            IsApproved = t1.IsApproved,
                            ApprovedBy = t1.ApprovedBy,
                            EmployeeName = employee.Name
                        };

            return query.ToList();
        }

        #region KPPL Purchase Order
        public async Task<VMPurchaseOrderSlave> KPPLProcurementPurchaseOrderSlaveGet(int companyId, int purchaseOrderId)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();
            vmPurchaseOrderSlave = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                         join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId
                                                         join t4 in _db.Employees on t1.EmpId equals t4.Id into t4_Join
                                                         from t4 in t4_Join.DefaultIfEmpty()
                                                         join t5 in _db.Designations on t4.DesignationId equals t5.DesignationId into t5_Join
                                                         from t5 in t5_Join.DefaultIfEmpty()
                                                         where t1.PurchaseOrderId == purchaseOrderId
                                                         select new VMPurchaseOrderSlave
                                                         {
                                                             PurchaseOrderId = t1.PurchaseOrderId,
                                                             SupplierName = t2.Name,
                                                             Code = t2.Code,
                                                             SupplierPropietor = t2.Propietor,
                                                             SupplierAddress = t2.Address,
                                                             SupplierMobile = t3.Phone,
                                                             EmployeeName = t4 != null ? t4.Name : "",
                                                             EmployeeMobile = t4 != null ? t4.MobileNo : "",
                                                             EmployeeDesignation = t5 != null ? t5.Name : "",
                                                             CID = t1.PurchaseOrderNo,
                                                             OrderDate = t1.PurchaseDate,
                                                             Description = t1.Remarks,
                                                             IsHold = t1.IsHold,
                                                             IsCancel = t1.IsCancel,
                                                             Status = t1.Status,
                                                             SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                             TermsAndCondition = t1.TermsAndCondition,
                                                             CompanyFK = t1.CompanyId,
                                                             DeliveryAddress = t1.DeliveryAddress,
                                                             DeliveryDate = t1.DeliveryDate,
                                                             Common_SupplierFK = t1.SupplierId,
                                                             FreightCharge = t1.FreightCharge,
                                                             OtherCharge = t1.OtherCharge,
                                                             CompanyName = t3.Name,
                                                             CompanyAddress = t3.Address,
                                                             CompanyEmail = t3.Email,
                                                             CompanyPhone = t3.Phone,
                                                             CreatedBy = t1.CreatedBy,
                                                             Companylogo = t3.CompanyLogo
                                                         }).FirstOrDefault());

            vmPurchaseOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.PurchaseOrderDetails.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                                       join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                       join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                       join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                       join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                       select new VMPurchaseOrderSlave
                                                                       {
                                                                           ProductName = t4.Name + " " + t3.ProductName,

                                                                           PurchaseOrderId = t1.PurchaseOrderId.Value,
                                                                           PurchaseOrderDetailId = t1.PurchaseOrderDetailId,
                                                                           PurchaseQuantity = t1.PurchaseQty,
                                                                           PurchasingPrice = t1.PurchaseRate,
                                                                           UnitName = t6.Name,
                                                                           PurchaseAmount = t1.PurchaseAmount,

                                                                           CompanyFK = t1.CompanyId,
                                                                           Common_ProductCategoryFK = t5.ProductCategoryId,
                                                                           Common_ProductSubCategoryFK = t4.ProductSubCategoryId,
                                                                           Common_ProductFK = t3.ProductId

                                                                       }).OrderByDescending(x => x.PurchaseOrderDetailId).AsEnumerable());

            return vmPurchaseOrderSlave;
        }
        public async Task<long> KPPLProcurementPurchaseOrderAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                long result = -1;
                string poCid = "";
                if (vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK == (int)VendorsPaymentMethodEnum.LC)
                {
                    if (vmPurchaseOrderSlave.OrderNo != null)
                    {
                        poCid = vmPurchaseOrderSlave.OrderNo;
                    }
                    else
                    {
                        var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                        poCid = @"PO-" +
                                       DateTime.Now.ToString("yy") +
                                       DateTime.Now.ToString("MM") +
                                       DateTime.Now.ToString("dd") + "-" +

                                        poMax.ToString().PadLeft(2, '0');
                    }
                }
                else
                {
                    var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
                    poCid = @"PO-" +
                                   DateTime.Now.ToString("yy") +
                                   DateTime.Now.ToString("MM") +
                                   DateTime.Now.ToString("dd") + "-" +

                                    poMax.ToString().PadLeft(2, '0');
                }
                try
                {


                    PurchaseOrder Procurement_PurchaseOrder = new PurchaseOrder
                    {

                        PurchaseOrderNo = poCid,
                        PurchaseDate = vmPurchaseOrderSlave.OrderDate,
                        SupplierId = vmPurchaseOrderSlave.Common_SupplierFK,
                        DeliveryDate = vmPurchaseOrderSlave.DeliveryDate,
                        SupplierPaymentMethodEnumFK = vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK,
                        DeliveryAddress = vmPurchaseOrderSlave.DeliveryAddress,
                        Remarks = vmPurchaseOrderSlave.Remarks,
                        Status = (int)EnumPOStatus.Draft,
                        PurchaseOrderStatus = EnumPOStatus.Draft.ToString(),
                        EmpId = vmPurchaseOrderSlave.EmployeeId,
                        CountryId = vmPurchaseOrderSlave.CountryId,
                        PINo = vmPurchaseOrderSlave.PINo,
                        LCHeadGLId = vmPurchaseOrderSlave.LCHeadGLId,
                        LCNo = vmPurchaseOrderSlave.LCNo,
                        LCValue = vmPurchaseOrderSlave.LCValue,
                        InsuranceNo = vmPurchaseOrderSlave.InsuranceNo,
                        PremiumValue = vmPurchaseOrderSlave.PremiumValue,
                        ShippedBy = vmPurchaseOrderSlave.ShippedBy,
                        PortOfLoading = vmPurchaseOrderSlave.PortOfLoading,
                        FinalDestinationCountryFk = vmPurchaseOrderSlave.FinalDestinationCountryFk,
                        PortOfDischarge = vmPurchaseOrderSlave.PortOfDischarge,
                        FreightCharge = vmPurchaseOrderSlave.FreightCharge,
                        OtherCharge = vmPurchaseOrderSlave.OtherCharge,
                        LCId = vmPurchaseOrderSlave.LCId,
                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    _db.PurchaseOrders.Add(Procurement_PurchaseOrder);
                    int res = _db.SaveChanges();
                    if (res > 0)
                    {
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                    }

                    PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
                    {
                        PurchaseOrderId = Procurement_PurchaseOrder.PurchaseOrderId,
                        ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                        PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                        PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                        PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),

                        DemandRate = 0,
                        QCRate = 0,
                        PackSize = 0,

                        CompanyId = vmPurchaseOrderSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);

                    res += _db.SaveChanges();

                    if (res > 1)
                    {

                        scope.Commit();
                        result = Procurement_PurchaseOrder.PurchaseOrderId;
                        return result;
                    }
                    else
                    {
                        scope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;

                }

                return result;



            }

        }
        public async Task<long> KPPLProcurementPurchaseOrderSlaveAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            PurchaseOrderDetail procurementPurchaseOrderSlave = new PurchaseOrderDetail
            {
                PurchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId,
                ProductId = vmPurchaseOrderSlave.Common_ProductFK,
                PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity,
                PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice),

                DemandRate = 0,
                QCRate = 0,
                PackSize = 0,

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PurchaseOrderDetails.Add(procurementPurchaseOrderSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = procurementPurchaseOrderSlave.PurchaseOrderDetailId;
            }

            return result;
        }
        public async Task<int> KPPLProcurementPurchaseOrderSlaveEdit(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            var result = -1;
            PurchaseOrderDetail model = await _db.PurchaseOrderDetails.FindAsync(vmPurchaseOrderSlave.PurchaseOrderDetailId);

            model.ProductId = vmPurchaseOrderSlave.Common_ProductFK;
            model.PurchaseQty = vmPurchaseOrderSlave.PurchaseQuantity;
            model.PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice;
            model.PurchaseAmount = (vmPurchaseOrderSlave.PurchaseQuantity * vmPurchaseOrderSlave.PurchasingPrice);

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmPurchaseOrderSlave.ID;
            }

            return result;
        }

        public async Task<VMPurchaseOrder> KPPLProcurementPurchaseOrderListGet(int companyId, DateTime? fromDate, DateTime? toDate, int? vStatus)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;

            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders
                                                             .Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId &&
                                                             x.PurchaseDate >= fromDate && x.PurchaseDate <= toDate
                                                             &&
                                                             !x.IsCancel && !x.IsHold &&
                                                             ((companyId == (int)CompanyName.KrishibidFeedLimited ? x.DemandId == null : 0 == 0))
                                                             )
                                                             join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId


                                                             select new VMPurchaseOrder
                                                             {
                                                                 PurchaseOrderId = t1.PurchaseOrderId,
                                                                 SupplierName = t2.Name,
                                                                 CID = t1.PurchaseOrderNo,
                                                                 OrderDate = t1.PurchaseDate,
                                                                 Description = t1.Remarks,
                                                                 IsHold = t1.IsHold,
                                                                 IsCancel = t1.IsCancel,
                                                                 Status = t1.Status,
                                                                 CompanyFK = t1.CompanyId,
                                                                 CountryId = t1.CountryId,
                                                                 FinalDestinationCountryFk = t1.FinalDestinationCountryFk,
                                                                 OtherCharge = t1.OtherCharge,
                                                                 FreightCharge = t1.FreightCharge,
                                                                 PINo = t1.PINo,
                                                                 PortOfDischarge = t1.PortOfDischarge,
                                                                 PortOfLoading = t1.PortOfLoading,
                                                                 ShippedBy = t1.ShippedBy,
                                                                 SupplierPaymentMethodEnumFK = t1.SupplierPaymentMethodEnumFK,
                                                                 TermsAndCondition = t1.TermsAndCondition,
                                                                 DeliveryAddress = t1.DeliveryAddress,
                                                                 DeliveryDate = t1.DeliveryDate,
                                                                 Common_SupplierFK = t1.SupplierId,
                                                             }).OrderByDescending(x => x.PurchaseOrderId).AsEnumerable());

            if (vStatus != -1 && vStatus != null)
            {
                vmPurchaseOrder.DataList = vmPurchaseOrder.DataList.Where(q => q.Status == vStatus);
            }


            return vmPurchaseOrder;
        }
        #endregion



        public async Task<long> PackagingRMIssuedAchknolagement(VMPackagingPurchaseRequisition model)
        {
            long result = -1;
            if (model != null)
            {
                var dbEntity = _db.IssueMasterInfoes.Find(model.IssueMasterId);

                if (dbEntity != null)
                {
                    dbEntity.Achknolagement = true;
                    dbEntity.AchknologeBy = model.AchknologeById;
                    dbEntity.AcknologeDate = DateTime.Now;

                    if (_db.SaveChanges() > 0)
                    {
                        result = dbEntity.IssueMasterId;
                    }
                }
                if (result > 0)
                {
                    VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();

                    vmPurchaseRequisition = await Task.Run(() => GetIssueDetail(dbEntity.CompanyId, dbEntity.IssueMasterId));


                    await _accountingService.AccountingRMIssuedPushPackaging(vmPurchaseRequisition.IssueDate, dbEntity.CompanyId, vmPurchaseRequisition, (int)PackagingJournalEnum.RMIssuedVoucher);

                }


            }
            return result;
        }


        public async Task<long> SubmitMultiIssuePackaging(VMPackagingPurchaseRequisition vm)
        {

            var firstDayOfMonth = new DateTime(2024, 08, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            List<IssueMasterInfo> listIssueMasterInfo = _db.IssueMasterInfoes.Where(x => x.IsActive == true &&
             x.Achknolagement == false &&
            x.CompanyId == 20 &&
           x.IssueDate >= firstDayOfMonth
           && x.IssueDate <= lastDayOfMonth).ToList();

            foreach (var item in listIssueMasterInfo)
            {
                IssueMasterInfo model = await _db.IssueMasterInfoes.FindAsync(item.IssueMasterId);
                model.Achknolagement = true;
                model.AchknologeBy = vm.AchknologeById;
                model.AcknologeDate = DateTime.Now;
                await _db.SaveChangesAsync();

                VMPackagingPurchaseRequisition vmPurchaseRequisition = new VMPackagingPurchaseRequisition();
                vmPurchaseRequisition = await Task.Run(() => GetIssueDetail(item.CompanyId, item.IssueMasterId));
                await _accountingService.AccountingRMIssuedPushPackaging(model.IssueDate, model.CompanyId, vmPurchaseRequisition, (int)PackagingJournalEnum.RMIssuedVoucher);


            }
            return 0;
        }



    }
}