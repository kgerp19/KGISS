using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Services.Procurement;
using KGERP.Utility;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Services.Packaging
{

    public class PackagingService
    {
        private readonly ERPEntities _db;

        public PackagingService(ERPEntities db)
        {
            _db = db;
        }
        #region Common
        public List<object> CommonTremsAndConditionDropDownList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.POTremsAndConditions.Where(a => a.IsActive == true).ToList())
            {
                List.Add(new { Text = item.Key, Value = item.ID });
            }
            return List;

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
        public List<object> ShippedByListDropDownList(int companyId)
        {
            var List = new List<object>();
            List.Add(new { Text = "Air", Value = "Air" });
            List.Add(new { Text = "Ship", Value = "Ship" });
            return List;

        }

        public List<object> PackagingPurchaseOrderDropDownBySupplier(int supplierId)
        {
            var PackagingPurchaseOrderList = new List<object>();
            _db.PurchaseOrders.Where(x => x.IsActive && x.SupplierId == supplierId).Select(x => x).ToList().ForEach(x => PackagingPurchaseOrderList.Add(new
            {
                Value = x.PurchaseOrderId.ToString(),
                Text = x.PurchaseOrderNo + " Date: " + x.PurchaseDate.Value.ToLongDateString()
            }));
            return PackagingPurchaseOrderList;
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

        public async Task<VMSalesOrder> PackagingOrderMastersListGet(int companyId)
        {
            VMSalesOrder vmSalesOrder = new VMSalesOrder();
            vmSalesOrder.CompanyFK = companyId;
            vmSalesOrder.DataList = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.CompanyId == companyId && x.Status < (int)EnumPOStatus.Closed)
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
            return vmSalesOrder;
        }
        public async Task<VMSalesOrder> GetSinglOrderMasters(int orderMasterId)
        {

            var v = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                          join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                          join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                          select new VMSalesOrder
                                          {
                                              CompanyFK = t1.CompanyId,
                                              OrderMasterId = t1.OrderMasterId,
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
                                              CourierNo = t1.CourierNo,
                                              FinalDestination = t1.FinalDestination,
                                              CourierCharge = t1.CourierCharge

                                          }).FirstOrDefault());
            return v;
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

        public async Task<long> OrderMastersSubmit(long? id = 0)
        {
            long result = -1;
            OrderMaster orderMasters = await _db.OrderMasters.FindAsync(id);
            if (orderMasters != null)
            {
                if (orderMasters.Status == (int)EnumPOStatus.Draft)
                {
                    orderMasters.Status = (int)EnumPOStatus.Submitted;
                }
                else
                {
                    orderMasters.Status = (int)EnumPOStatus.Draft;

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
        #region Purchase Order Add Edit Submit Hold-UnHold Cancel-Renew Closed-Reopen Delete
        public async Task<VMPurchaseOrder> PackagingPurchaseOrderListGet(int companyId)
        {
            VMPurchaseOrder vmPurchaseOrder = new VMPurchaseOrder();
            vmPurchaseOrder.CompanyFK = companyId;
            vmPurchaseOrder.DataList = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && !x.IsOpening && x.CompanyId == companyId && x.Status < (int)EnumPOStatus.Closed && !x.IsCancel && !x.IsHold)
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
        public async Task<VMPurchaseOrder> GetSinglePackagingPurchaseOrder(int id)
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
        public async Task<long> PackagingPurchaseOrderAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK).Count() + 1;
            string poCid = @"PO-" +
                            DateTime.Now.ToString("yy") +
                            DateTime.Now.ToString("MM") +
                            DateTime.Now.ToString("dd") + "-" +

                             poMax.ToString().PadLeft(2, '0');
            PurchaseOrder Packaging_PurchaseOrder = new PurchaseOrder
            {

                PurchaseOrderNo = poCid,
                PurchaseDate = vmPurchaseOrderSlave.OrderDate,
                SupplierId = vmPurchaseOrderSlave.Common_SupplierFK,
                DeliveryDate = vmPurchaseOrderSlave.DeliveryDate,
                SupplierPaymentMethodEnumFK = vmPurchaseOrderSlave.SupplierPaymentMethodEnumFK,
                DeliveryAddress = vmPurchaseOrderSlave.DeliveryAddress,
                Remarks = vmPurchaseOrderSlave.Remarks,
                TermsAndCondition = vmPurchaseOrderSlave.TermsAndCondition,
                Status = (int)EnumPOStatus.Draft,
                PurchaseOrderStatus = EnumPOStatus.Draft.ToString(),

                CountryId = vmPurchaseOrderSlave.CountryId,
                PINo = vmPurchaseOrderSlave.PINo,
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

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PurchaseOrders.Add(Packaging_PurchaseOrder);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = Packaging_PurchaseOrder.PurchaseOrderId;
            }
            return result;
        }

        public async Task<long> PackagingPurchaseOrderOpeningAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            var poMax = _db.PurchaseOrders.Where(x => x.CompanyId == vmPurchaseOrderSlave.CompanyFK && x.IsOpening).Count() + 1;
            string poCid = @"Opening-" +
                            DateTime.Now.ToString("yy") +
                            DateTime.Now.ToString("MM") +
                            DateTime.Now.ToString("dd") + "-" +

                             poMax.ToString().PadLeft(2, '0');
            PurchaseOrder Packaging_PurchaseOrder = new PurchaseOrder
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
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = vmPurchaseOrderSlave.OrderDate.Value,
                IsActive = true
            };
            _db.PurchaseOrders.Add(Packaging_PurchaseOrder);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = Packaging_PurchaseOrder.PurchaseOrderId;
            }
            return result;
        }

        public async Task<long> PackagingPurchaseOrderEdit(VMPurchaseOrder vmPurchaseOrder)
        {
            long result = -1;
            PurchaseOrder PackagingPurchaseOrder = await _db.PurchaseOrders.FindAsync(vmPurchaseOrder.PurchaseOrderId);

            PackagingPurchaseOrder.PurchaseDate = vmPurchaseOrder.OrderDate;
            PackagingPurchaseOrder.SupplierId = vmPurchaseOrder.Common_SupplierFK;
            PackagingPurchaseOrder.DeliveryDate = vmPurchaseOrder.DeliveryDate;
            PackagingPurchaseOrder.SupplierPaymentMethodEnumFK = vmPurchaseOrder.SupplierPaymentMethodEnumFK;
            PackagingPurchaseOrder.DeliveryAddress = vmPurchaseOrder.DeliveryAddress;
            PackagingPurchaseOrder.Remarks = vmPurchaseOrder.Remarks;
            PackagingPurchaseOrder.TermsAndCondition = vmPurchaseOrder.TermsAndCondition;
            PackagingPurchaseOrder.Remarks = vmPurchaseOrder.Description;
            PackagingPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            PackagingPurchaseOrder.ModifiedDate = DateTime.Now;

            PackagingPurchaseOrder.CountryId = vmPurchaseOrder.CountryId;
            PackagingPurchaseOrder.PINo = vmPurchaseOrder.PINo;
            PackagingPurchaseOrder.LCNo = vmPurchaseOrder.LCNo;
            PackagingPurchaseOrder.LCValue = vmPurchaseOrder.LCValue;
            PackagingPurchaseOrder.InsuranceNo = vmPurchaseOrder.InsuranceNo;
            PackagingPurchaseOrder.PremiumValue = vmPurchaseOrder.PremiumValue;
            PackagingPurchaseOrder.ShippedBy = vmPurchaseOrder.ShippedBy;
            PackagingPurchaseOrder.PortOfLoading = vmPurchaseOrder.PortOfLoading;
            PackagingPurchaseOrder.FinalDestinationCountryFk = vmPurchaseOrder.FinalDestinationCountryFk;
            PackagingPurchaseOrder.PortOfDischarge = vmPurchaseOrder.PortOfDischarge;
            PackagingPurchaseOrder.FreightCharge = vmPurchaseOrder.FreightCharge;
            PackagingPurchaseOrder.OtherCharge = vmPurchaseOrder.OtherCharge;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmPurchaseOrder.ID;
            }

            return result;
        }

        public async Task<long> PackagingPurchaseOrderSubmit(long? id = 0)
        {
            long result = -1;
            PurchaseOrder PackagingPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (PackagingPurchaseOrder != null)
            {
                if (PackagingPurchaseOrder.Status == (int)EnumPOStatus.Draft)
                {
                    PackagingPurchaseOrder.Status = (int)EnumPOStatus.Submitted;
                }
                else
                {
                    PackagingPurchaseOrder.Status = (int)EnumPOStatus.Draft;

                }
                PackagingPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                PackagingPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PackagingPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }
        public async Task<long> PackagingPurchaseOrderDelete(long id)
        {
            long result = -1;
            PurchaseOrder PackagingPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (PackagingPurchaseOrder != null)
            {
                PackagingPurchaseOrder.IsActive = false;
                PackagingPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                PackagingPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PackagingPurchaseOrder.PurchaseOrderId;
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
        public async Task<long> PackagingPurchaseOrderHoldUnHold(long id)
        {
            long result = -1;
            PurchaseOrder PackagingPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (PackagingPurchaseOrder != null)
            {
                if (PackagingPurchaseOrder.IsHold)
                {
                    PackagingPurchaseOrder.IsHold = false;
                }
                else
                {
                    PackagingPurchaseOrder.IsHold = true;
                }
                PackagingPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                PackagingPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PackagingPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }
        public async Task<long> PackagingPurchaseOrderCancelRenew(long id)
        {
            long result = -1;
            PurchaseOrder PackagingPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (PackagingPurchaseOrder != null)
            {
                if (PackagingPurchaseOrder.IsCancel)
                {
                    PackagingPurchaseOrder.IsCancel = false;
                }
                else
                {
                    PackagingPurchaseOrder.IsCancel = true;
                }
                PackagingPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                PackagingPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PackagingPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }
        public async Task<long> PackagingPurchaseOrderClosedReopen(long id)
        {
            long result = -1;
            PurchaseOrder PackagingPurchaseOrder = await _db.PurchaseOrders.FindAsync(id);
            if (PackagingPurchaseOrder != null)
            {
                if (PackagingPurchaseOrder.Status == (int)EnumPOStatus.Closed)
                {
                    PackagingPurchaseOrder.Status = (int)EnumPOStatus.Draft;
                }
                else
                {
                    PackagingPurchaseOrder.Status = (int)EnumPOStatus.Closed;
                }
                PackagingPurchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                PackagingPurchaseOrder.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PackagingPurchaseOrder.PurchaseOrderId;
                }
            }
            return result;
        }

        public async Task<VMPurchaseOrder> PackagingApprovalPurchaseOrderListGet(int companyId)
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
        public async Task<VMPurchaseOrder> PackagingCancelPurchaseOrderListGet(int companyId)
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
        public async Task<VMPurchaseOrder> PackagingHoldPurchaseOrderListGet(int companyId)
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
        public async Task<VMPurchaseOrder> PackagingClosedPurchaseOrderListGet(int companyId)
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
        public async Task<VMPurchaseOrderSlave> PackagingPurchaseOrderSlaveGet(int companyId, int purchaseOrderId)
        {
            VMPurchaseOrderSlave vmPurchaseOrderSlave = new VMPurchaseOrderSlave();
            vmPurchaseOrderSlave = await Task.Run(() => (from t1 in _db.PurchaseOrders.Where(x => x.IsActive && x.PurchaseOrderId == purchaseOrderId && x.CompanyId == companyId)
                                                         join t2 in _db.Vendors on t1.SupplierId equals t2.VendorId
                                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                         select new VMPurchaseOrderSlave
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
                                                             CompanyFK = t1.CompanyId,
                                                             DeliveryAddress = t1.DeliveryAddress,
                                                             DeliveryDate = t1.DeliveryDate,
                                                             Common_SupplierFK = t1.SupplierId,
                                                             FreightCharge = t1.FreightCharge,
                                                             OtherCharge = t1.OtherCharge,
                                                             CompanyName = t3.Name,
                                                             CompanyAddress = t3.Address,
                                                             CompanyEmail = t3.Email,
                                                             CompanyPhone = t3.Phone

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

        public async Task<VMPurchaseOrderSlave> PackagingPurchaseOrderSlaveOpeningBalanceGet(int companyId)
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

        public async Task<VMPurchaseOrderSlave> GetSinglePackagingPurchaseOrderSlave(int id)
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
        public async Task<long> PackagingPurchaseOrderSlaveAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            PurchaseOrderDetail PackagingPurchaseOrderSlave = new PurchaseOrderDetail
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
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PurchaseOrderDetails.Add(PackagingPurchaseOrderSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = PackagingPurchaseOrderSlave.PurchaseOrderDetailId;
            }

            return result;
        }
        public async Task<long> PackagingPurchaseOrderSlaveOpeningAdd(VMPurchaseOrderSlave vmPurchaseOrderSlave)
        {
            long result = -1;
            PurchaseOrderDetail PackagingPurchaseOrderSlave = new PurchaseOrderDetail
            {
                PurchaseOrderId = vmPurchaseOrderSlave.PurchaseOrderId,
                ProductId = 4425,
                PurchaseQty = 1,
                PurchaseRate = vmPurchaseOrderSlave.PurchasingPrice,
                PurchaseAmount = vmPurchaseOrderSlave.PurchasingPrice,

                DemandRate = 0,
                QCRate = 0,
                PackSize = 0,

                CompanyId = vmPurchaseOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.PurchaseOrderDetails.Add(PackagingPurchaseOrderSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = PackagingPurchaseOrderSlave.PurchaseOrderDetailId;
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

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmPurchaseOrderSlave.ID;
            }

            return result;
        }
        public async Task<long> PackagingPurchaseOrderSlaveDelete(long id)
        {
            long result = -1;
            PurchaseOrderDetail PackagingPurchaseOrderSlave = await _db.PurchaseOrderDetails.FindAsync(id);
            if (PackagingPurchaseOrderSlave != null)
            {
                PackagingPurchaseOrderSlave.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = PackagingPurchaseOrderSlave.PurchaseOrderDetailId;
                }
            }
            return result;
        }

        #endregion


        #region Purchase Order Detail

        public async Task<VMSalesOrderSlave> PackagingSalesOrderDetailsGet(int companyId, int orderMasterId)
        {
            VMSalesOrderSlave vmSalesOrderSlave = new VMSalesOrderSlave();
            vmSalesOrderSlave = await Task.Run(() => (from t1 in _db.OrderMasters.Where(x => x.IsActive && x.OrderMasterId == orderMasterId && x.CompanyId == companyId)
                                                      join t2 in _db.Vendors on t1.CustomerId equals t2.VendorId
                                                      join t4 in _db.SubZones on t2.SubZoneId equals t4.SubZoneId
                                                      join t5 in _db.Zones on t4.ZoneId equals t5.ZoneId

                                                      join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                      select new VMSalesOrderSlave
                                                      {
                                                          CustomerPhone = t2.Phone,
                                                          CustomerAddress = t2.Address,
                                                          CustomerEmail = t2.Email,
                                                          ContactPerson = t2.ContactName,
                                                          CompanyFK = t1.CompanyId,
                                                          OrderMasterId = t1.OrderMasterId,
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
                                                          CustomerTypeFk = t2.CustomerTypeFK,
                                                          CustomerId = t2.VendorId,
                                                          CourierCharge = t1.CourierCharge,
                                                          FinalDestination = t1.FinalDestination,
                                                          CourierNo = t1.CourierNo

                                                      }).FirstOrDefault());

            vmSalesOrderSlave.DataListSlave = await Task.Run(() => (from t1 in _db.OrderDetails.Where(x => x.IsActive && x.OrderMasterId == orderMasterId)
                                                                    join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                    join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                    join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                    join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                    select new VMSalesOrderSlave
                                                                    {
                                                                        ProductName = t4.Name + " " + t3.ProductName,
                                                                        OrderMasterId = t1.OrderMasterId.Value,
                                                                        OrderDetailId = t1.OrderDetailId,
                                                                        Qty = t1.Qty,
                                                                        UnitPrice = t1.UnitPrice,
                                                                        UnitName = t6.Name,
                                                                        TotalAmount = t1.Amount,


                                                                        ProductCategoryId = t5.ProductCategoryId,
                                                                        ProductSubCategoryId = t4.ProductSubCategoryId,
                                                                        FProductId = t3.ProductId

                                                                    }).OrderByDescending(x => x.OrderDetailId).AsEnumerable());


            return vmSalesOrderSlave;
        }


        public async Task<VMSalesOrderSlave> PackagingSalesOrderOpeningDetailsGet(int companyId)
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
                                                                        Remarks = t0.Remarks
                                                                    }).OrderByDescending(x => x.OrderMasterId).AsEnumerable());




            return vmSalesOrderSlave;
        }

        public async Task<VMSalesOrderSlave> GetSingleOrderDetails(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.OrderDetails
                                          join t2 in _db.Products on t1.ProductId equals t2.ProductId
                                          join t3 in _db.Units on t2.UnitId equals t3.UnitId

                                          where t1.OrderDetailId == id
                                          select new VMSalesOrderSlave
                                          {

                                              ProductName = t2.ProductName,
                                              OrderMasterId = t1.OrderMasterId.Value,
                                              OrderDetailId = t1.OrderDetailId,
                                              Qty = t1.Qty,
                                              UnitPrice = t1.UnitPrice,
                                              TotalAmount = t1.Amount,
                                              UnitName = t3.Name,
                                              FProductId = t1.ProductId,
                                              CompanyFK = t1.CompanyId

                                          }).FirstOrDefault());
            return v;
        }
        public async Task<long> OrderDetailAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long result = -1;
            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                ProductId = vmSalesOrderSlave.FProductId,
                Qty = vmSalesOrderSlave.Qty,
                UnitPrice = vmSalesOrderSlave.UnitPrice,
                Amount = (vmSalesOrderSlave.Qty * vmSalesOrderSlave.UnitPrice),

                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                IsActive = true
            };
            _db.OrderDetails.Add(orderDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderDetail.OrderDetailId;
            }

            return result;
        }
        public async Task<long> OrderDetailOpeningAdd(VMSalesOrderSlave vmSalesOrderSlave)
        {
            long result = -1;
            OrderDetail orderDetail = new OrderDetail
            {
                OrderMasterId = vmSalesOrderSlave.OrderMasterId,
                ProductId = 4426,
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

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifedDate = DateTime.Now;
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
















        //public List<object> ApprovedPOList()
        //{
        //    var List = new List<object>();
        //    _db.Packaging_PurchaseOrder.Where(x => x.Active && x.Status == (int)EnumPOStatus.Submitted).Select(x => x).ToList() // x.PackagingOriginTypeEnumFK == PackagingOriginTypeEnumFK &&
        //   .ForEach(x => List.Add(new
        //   {
        //       Value = x.ID,
        //       Text = x.CID + " Date: " + x.OrderDate.ToLongDateString()
        //   }));
        //    return List;

        //}

        //public async Task<List<VMPurchaseOrder>> PackagingPurchaseOrderGet(int commonSupplierFK)
        //{
        //    var x = await Task.Run(() => (from t1 in _db.WareHouse_POReceiving
        //                                  join t2 in _db.Packaging_PurchaseOrder on t1.Packaging_PurchaseOrderFk equals t2.ID
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
                     where t1.IsActive && ((t1.Name.StartsWith(prefix)) || (t1.Code.StartsWith(prefix)))

                     select new
                     {
                         label = t1.Name,
                         val = t1.VendorId
                     }).OrderBy(x => x.label).Take(20).ToList();

            return v;
        }
        public object GetAutoCompleteCustomer(string prefix, int companyId)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == (int)Provider.Customer)
                     where t1.IsActive && ((t1.Name.StartsWith(prefix)) || (t1.Code.StartsWith(prefix)))

                     select new
                     {
                         label = t1.Name,
                         val = t1.VendorId,
                         CustomerTypeFK = t1.CustomerTypeFK
                     }).OrderBy(x => x.label).Take(20).ToList();

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
            var poMax = _db.OrderMasters.Where(x => x.CompanyId == vmSalesOrderSlave.CompanyFK).Count() + 1;
            string poCid = @"SO-" +
                            DateTime.Now.ToString("yy") +
                            DateTime.Now.ToString("MM") +
                            DateTime.Now.ToString("dd") + "-" +

                             poMax.ToString().PadLeft(2, '0');
            OrderMaster orderMaster = new OrderMaster
            {

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


                CompanyId = vmSalesOrderSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),// System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                IsActive = true
            };
            _db.OrderMasters.Add(orderMaster);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = orderMaster.OrderMasterId;
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
    }
}