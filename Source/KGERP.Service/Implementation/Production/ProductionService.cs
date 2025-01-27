using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Production;
using KGERP.Services.Procurement;
using KGERP.Services.WareHouse;
using KGERP.Utility;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IdentityModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace KGERP.Services.Production
{
    public class ProductionService
    {
        private readonly ERPEntities _db;
        private readonly AccountingService _accountingService;

        public ProductionService(ERPEntities db)
        {
            _db = db;
            _accountingService = new AccountingService(db);

        }
        #region Common
        public List<object> CommonTremsAndConditionDropDownList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.POTremsAndConditions.Where(a => a.IsActive == true).ToList())
            {
                List.Add(new { Text = item.Key, Value = item.Value });
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

        #region Purchase Order Add Edit Submit Hold-UnHold Cancel-Renew Closed-Reopen Delete
        public async Task<VMProdReference> ProdReferenceListGet(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMProdReference vmProdReference = new VMProdReference();
            vmProdReference.CompanyFK = companyId;
            vmProdReference.DataList = await Task.Run(() => (from t1 in _db.Prod_Reference.Where(x => x.IsActive && x.CompanyId == companyId)

                                                             where t1.ReferenceDate >= fromDate
                                                             && t1.ReferenceDate <= toDate
                                                             select new VMProdReference
                                                             {
                                                                 ProdReferenceId = t1.ProdReferenceId,
                                                                 ReferenceDate = t1.ReferenceDate,
                                                                 ReferenceNo = t1.ReferenceNo,
                                                                 CompanyFK = t1.CompanyId,
                                                                 IsSubmitted = t1.IsSubmitted
                                                             }).OrderByDescending(x => x.ProdReferenceId).AsEnumerable());
            return vmProdReference;
        }

        public async Task<VMProdReference> ProductionListGet(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMProdReference vmProdReference = new VMProdReference();
            vmProdReference.CompanyFK = companyId;
            vmProdReference.DataList = await Task.Run(() => (from t1 in _db.Productions.Where(x => x.IsActive && x.CompanyId == companyId)
                                                             where t1.ProductionDate >= fromDate
                                                             && t1.ProductionDate <= toDate
                                                             select new VMProdReference
                                                             {
                                                                 ProductionId = t1.ProductionId,
                                                                 ProductionDate = t1.ProductionDate,
                                                                 ProductionNo = t1.ProductionNo,
                                                                 CompanyFK = t1.CompanyId,
                                                                 IsSubmitted = t1.IsSubmitted
                                                             }).OrderByDescending(x => x.ProductionId).AsEnumerable());
            return vmProdReference;
        }

        public async Task<VMProdReference> KPLProdReferenceListGet(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMProdReference vmProdReference = new VMProdReference();
            vmProdReference.CompanyFK = companyId;
            vmProdReference.DataList = await Task.Run(() => (from t1 in _db.Prod_Reference.Where(x => x.IsActive && x.CompanyId == companyId)
                                                             join t2 in _db.OrderMasters on t1.OrderMasterId equals t2.OrderMasterId
                                                             join t3 in _db.Vendors on t2.CustomerId equals t3.VendorId
                                                             where t1.ReferenceDate >= fromDate
                                                             && t1.ReferenceDate <= toDate
                                                             select new VMProdReference
                                                             {
                                                                 ProdReferenceId = t1.ProdReferenceId,
                                                                 ReferenceDate = t1.ReferenceDate,
                                                                 ReferenceNo = t1.ReferenceNo,
                                                                 CompanyFK = t1.CompanyId,
                                                                 IsSubmitted = t1.IsSubmitted,
                                                                 CustomerBy = t3.Code + " - " + t3.Name,
                                                                 OrderMaterNo = t2.OrderNo,
                                                                 FromDate = t2.OrderDate,
                                                                 CustomerPONo = t2.CustomerPONo
                                                             }).OrderByDescending(x => x.ProdReferenceId).AsEnumerable());
            return vmProdReference;
        }

        public async Task<VMProdReference> KPLProdAuthPendingReferenceListGet(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            VMProdReference vmProdReference = new VMProdReference();
            vmProdReference.CompanyFK = companyId;
            vmProdReference.DataList = await Task.Run(() => (from t1 in _db.Prod_Reference.Where(x => x.IsActive && x.CompanyId == companyId && x.IsSubmitted == true && x.IsAuthorized == false)
                                                             join t2 in _db.OrderDetails on t1.OrderDetailId equals t2.OrderDetailId
                                                             join t4 in _db.OrderMasters on t2.OrderMasterId equals t4.OrderMasterId
                                                             join t3 in _db.Vendors on t4.CustomerId equals t3.VendorId
                                                             where t1.ReferenceDate >= fromDate
                                                             && t1.ReferenceDate <= toDate
                                                             select new VMProdReference
                                                             {
                                                                 ProdReferenceId = t1.ProdReferenceId,
                                                                 CompanyFK = t1.CompanyId,


                                                                 IsSubmitted = t1.IsSubmitted,
                                                                 IsAuthorized = t1.IsAuthorized,

                                                                 ReferenceDate = t1.ReferenceDate,
                                                                 ReferenceNo = t1.ReferenceNo,
                                                                 JobNo = t2.JobOrderNo,
                                                                 JobDate = t2.OrderDate,
                                                                 CustomerPONo = t4.CustomerPONo,
                                                                 CustomerBy = t3.Code + " - " + t3.Name

                                                             }).OrderByDescending(x => x.ProdReferenceId).AsEnumerable());
            return vmProdReference;
        }

        public async Task<VMProdReference> GetSingleProdReference(int id)
        {

            var v = await Task.Run(() => (from t1 in _db.Prod_Reference

                                          where t1.ProdReferenceId == id
                                          select new VMProdReference
                                          {
                                              ProdReferenceId = t1.ProdReferenceId,
                                              ReferenceDate = t1.ReferenceDate,
                                              ReferenceNo = t1.ReferenceNo,
                                              CompanyFK = t1.CompanyId
                                          }).FirstOrDefault());
            return v;
        }
        public async Task<int> Prod_ReferenceAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;
            var poMax = _db.Prod_Reference.Where(x => x.CompanyId == vmProdReferenceSlave.CompanyFK).Count() + 1;
            string poCid = @"PROD-" +
                            DateTime.Now.ToString("yy") +
                            DateTime.Now.ToString("MM") +
                            DateTime.Now.ToString("dd") + "-" +

                             poMax.ToString().PadLeft(2, '0');
            Prod_Reference prodReference = new Prod_Reference
            {

                ReferenceNo = poCid,
                ReferenceDate = vmProdReferenceSlave.ReferenceDate,
                HeadGLId = vmProdReferenceSlave.AdvanceHeadGLId,
                CompanyId = vmProdReferenceSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                Remarks = vmProdReferenceSlave.Remarks,


                IsActive = true
            };
            _db.Prod_Reference.Add(prodReference);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = prodReference.ProdReferenceId;
            }
            return result;
        }

        public async Task<long> ProductionAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            long result = -1;
            var poMax = _db.Productions.Where(x => x.CompanyId == vmProdReferenceSlave.CompanyFK).Count() + 1;
            string poCid = @"PROD-" +
                            DateTime.Now.ToString("yy") +
                            DateTime.Now.ToString("MM") +
                            DateTime.Now.ToString("dd") + "-" +
                            poMax.ToString().PadLeft(2, '0');

            Data.Models.Production prodReference = new Data.Models.Production
            {

                ProductionNo = poCid,
                ProductionDate = vmProdReferenceSlave.ReferenceDate,
                CompanyId = vmProdReferenceSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                Remarks = vmProdReferenceSlave.Remarks,
                IsActive = true
            };

            _db.Productions.Add(prodReference);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = prodReference.ProductionId;
            }
            return result;
        }

        public async Task<long> Prod_ReferenceEdit(VMProdReference vmProdReference)
        {
            long result = -1;
            Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(vmProdReference.ProdReferenceId);

            prodReference.ReferenceNo = vmProdReference.ReferenceNo;
            prodReference.ReferenceDate = vmProdReference.ReferenceDate;
            prodReference.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            prodReference.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = prodReference.ProdReferenceId;
            }

            return result;
        }

        public async Task<int> ProdReferenceSubmit(long? id = 0)
        {
            int result = -1;
            Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(id);
            if (prodReference != null)
            {
                if (prodReference.IsSubmitted == true)
                {
                    prodReference.IsSubmitted = false;
                }
                else
                {
                    prodReference.IsSubmitted = true;

                }
                prodReference.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                prodReference.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReference.ProdReferenceId;
                }
            }
            return result;
        }
        public async Task<int> ProdReferenceDelete(long id)
        {
            int result = -1;
            Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(id);
            if (prodReference != null)
            {
                prodReference.IsActive = false;
                prodReference.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                prodReference.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReference.ProdReferenceId;
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


        #endregion


        #region Purchase Order Detail

        public async Task<VMProdReferenceSlave> ProdReferenceSlaveGet(int companyId, int prodReferenceId)
        {
            VMProdReferenceSlave vmProdReferenceSlave = new VMProdReferenceSlave();
            vmProdReferenceSlave = await Task.Run(() => (from t1 in _db.Prod_Reference.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)

                                                         select new VMProdReferenceSlave
                                                         {
                                                             ProdReferenceId = t1.ProdReferenceId,
                                                             ReferenceNo = t1.ReferenceNo,
                                                             ReferenceDate = t1.ReferenceDate,
                                                             CompanyFK = t1.CompanyId,
                                                             CreatedBy = t1.CreatedBy,
                                                             CreatedDate = t1.CreatedDate,
                                                             IsSubmitted = t1.IsSubmitted,

                                                             Remarks = t1.Remarks
                                                         }).FirstOrDefault());

            vmProdReferenceSlave.DataListSlave = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlave.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                                       join t3 in _db.Products.Where(x => x.IsActive) on t1.FProductId equals t3.ProductId
                                                                       join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                       join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                       join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                       select new VMProdReferenceSlave
                                                                       {
                                                                           ProductName = t4.Name + " " + t3.ProductName,
                                                                           ProdReferenceId = t1.ProdReferenceId,
                                                                           ProdReferenceSlaveID = t1.ProdReferenceSlaveID,
                                                                           FProductId = t1.FProductId,
                                                                           Quantity = t1.Quantity,
                                                                           UnitName = t6.Name,
                                                                           CompanyFK = t1.CompanyId,
                                                                           CostingPrice = t1.CostingPrice,
                                                                           TotalPrice = t1.Quantity * t1.CostingPrice,
                                                                           AccountingHeadId = t5.AccountingHeadId


                                                                       }).OrderByDescending(x => x.ProdReferenceSlaveID).AsEnumerable());


            vmProdReferenceSlave.RowProductConsumeList = (from aaa in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive == true)
                                                          join fff in _db.Prod_ReferenceSlave.Where(x => x.IsActive == true)
                                                              on aaa.ProdReferenceSlaveID equals fff.ProdReferenceSlaveID
                                                          join bbb in _db.Products
                                                              on aaa.RProductId equals bbb.ProductId
                                                          join ccc in _db.ProductSubCategories
                                                              on bbb.ProductSubCategoryId equals ccc.ProductSubCategoryId
                                                          join ddd in _db.ProductCategories
                                                              on ccc.ProductCategoryId equals ddd.ProductCategoryId
                                                          join eee in _db.Units
                                                              on bbb.UnitId equals eee.UnitId

                                                          join fbbb in _db.Products
                                                              on fff.FProductId equals fbbb.ProductId
                                                          join fccc in _db.ProductSubCategories
                                                              on fbbb.ProductSubCategoryId equals fccc.ProductSubCategoryId
                                                          join fddd in _db.ProductCategories
                                                              on fccc.ProductCategoryId equals fddd.ProductCategoryId
                                                          join feee in _db.Units
                                                              on fbbb.UnitId equals feee.UnitId
                                                          join mr in _db.MaterialReceiveDetails
                                                              on bbb.ProductId equals mr.ProductId

                                                          where fff.ProdReferenceId == prodReferenceId
                                                          select new VMProdReferenceSlaveConsumption
                                                          {
                                                              ProdReferenceSlaveConsumptionID = aaa.ProdReferenceSlaveConsumptionID,
                                                              RProductName = ddd.Name + " " + ccc.Name + " " + bbb.ProductName,
                                                              FProductName = fddd.Name + " " + fccc.Name + " " + fbbb.ProductName,

                                                              UnitName = eee.Name,
                                                              FUnitName = feee.Name,

                                                              TotalConsumeQuantity = aaa.TotalConsumeQuantity,
                                                              COGS = aaa.COGS,
                                                              TotalCOGS = aaa.TotalConsumeQuantity * aaa.COGS,
                                                              AccountingHeadId = ddd.AccountingHeadId,
                                                              LotNumber=mr.LotNumber
                                                             

                                                          }).OrderBy(x => x.ProdReferenceSlaveConsumptionID).ToList();



            return vmProdReferenceSlave;
        }

        public async Task<VMProdReferenceSlave> KPLProdReferenceSlaveGet(int companyId, int prodReferenceId)
        {
            VMProdReferenceSlave vmProdReferenceSlave = new VMProdReferenceSlave();
            vmProdReferenceSlave = await Task.Run(() => (from t1 in _db.Prod_Reference.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                         join t0 in _db.OrderDetails on t1.OrderDetailId equals t0.OrderDetailId
                                                         join t2 in _db.OrderMasters on t0.OrderMasterId equals t2.OrderMasterId
                                                         join t3 in _db.Vendors on t2.CustomerId equals t3.VendorId
                                                         join t4 in _db.Employees on t1.CreatedBy.Trim().ToLower() equals t4.EmployeeId.Trim().ToLower()
                                                         join t5 in _db.Employees on t1.AuthorizedBy.Trim().ToLower() equals t5.EmployeeId.Trim().ToLower() into t5_Join
                                                         from t5 in t5_Join.DefaultIfEmpty()

                                                         select new VMProdReferenceSlave
                                                         {


                                                             ProdReferenceId = t1.ProdReferenceId,
                                                             CompanyFK = t1.CompanyId,

                                                             ReferenceNo = t1.ReferenceNo,
                                                             ReferenceDate = t1.ReferenceDate,

                                                             CustomerName = t3.Name,
                                                             CustomerPhone = t3.Phone,


                                                             JobNo = t0.JobOrderNo,
                                                             JobDate = t0.OrderDate,
                                                             OrderQty = t0.Qty,
                                                             Remarks = t0.Remarks,
                                                             ReelDirection = t0.ReelDirection,
                                                             PouchDerection = t0.PouchDerection,


                                                             OrderMasterNo = t2.OrderNo,
                                                             OrderDate = t2.OrderDate,
                                                             CustomerPONo = t2.CustomerPONo,

                                                             CreatedBy = t4.Name,
                                                             CreatedDate = t1.CreatedDate,



                                                             IsSubmitted = t1.IsSubmitted,
                                                             IsAuthorized = t1.IsAuthorized,
                                                             AuthorizedDate = t1.AuthorizedDate,
                                                             AuthorizedBy = t5.Name

                                                         }).FirstOrDefault());


            vmProdReferenceSlave.DataListSlave = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlave.Where(x => x.ProdReferenceId == prodReferenceId && x.IsActive)
                                                                       join t2 in _db.OrderDetails on t1.OrderDetailId equals t2.OrderDetailId
                                                                       join t3 in _db.Products on t1.FProductId equals t3.ProductId
                                                                       join t4 in _db.ProductSubCategories on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                       join t5 in _db.ProductCategories on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                       join t6 in _db.Units on t3.UnitId equals t6.UnitId

                                                                       select new VMProdReferenceSlave
                                                                       {
                                                                           CompanyFK = t1.CompanyId,
                                                                           ProdReferenceId = t1.ProdReferenceId,
                                                                           ProdReferenceSlaveID = t1.ProdReferenceSlaveID,
                                                                           FProductId = t1.FProductId,
                                                                           OrderDetailId = t1.OrderDetailId,
                                                                           AccountingHeadId = t5.AccountingHeadId,
                                                                           ProductName = t5.Name + " " + t4.Name + " " + t3.ProductName,
                                                                           Quantity = t1.Quantity,
                                                                           UnitName = t6.Name,
                                                                           IsVATInclude = t2.IsVATInclude,
                                                                           UnitPrice = t2.UnitPrice,
                                                                           VATPercent = t2.VATPercent,
                                                                           //CostingPrice = t2.IsVATInclude = true ? (decimal)(t2.UnitPrice - (t2.UnitPrice / 100 * (double)t2.VATPercent)) : (decimal)t2.UnitPrice,
                                                                           //TotalPrice = t1.Quantity * t1.CostingPrice,

                                                                       }).OrderByDescending(x => x.ProdReferenceSlaveID).AsEnumerable());


            return vmProdReferenceSlave;
        }

        public async Task<VMProdReferenceSlave> GetSingleProdReferenceSlave(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlave.Where(x => x.ProdReferenceSlaveID == id)
                                          join t2 in _db.Products on t1.FProductId equals t2.ProductId
                                          join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                          join t4 in _db.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId
                                          join t5 in _db.Units on t2.UnitId equals t5.UnitId
                                          where t1.IsActive == true
                                          select new VMProdReferenceSlave
                                          {
                                              ProdReferenceId = t1.ProdReferenceId,
                                              ProdReferenceSlaveID = t1.ProdReferenceSlaveID,
                                              ProductName = t4.Name + " " + t3.Name + " " + t2.ProductName,
                                              Quantity = t1.Quantity,
                                              FProductId = t1.FProductId,
                                              UnitName = t5.Name,
                                              CompanyFK = t1.CompanyId,
                                              QuantityLess = t1.QuantityLess,
                                              QuantityOver = t1.QuantityOver
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<VMProdReferenceSlave> GetSingleProductionItem(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.ProductionItems.Where(x => x.ProductionItemID == id)
                                          join t2 in _db.Products on t1.ProductId equals t2.ProductId
                                          join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                          join t4 in _db.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId
                                          join t5 in _db.Units on t2.UnitId equals t5.UnitId
                                          where t1.IsActive == true
                                          select new VMProdReferenceSlave
                                          {
                                              ProductionId = t1.ProductionId,
                                              ProductionItemId = t1.ProductionItemID,
                                              ProductName = t4.Name + " " + t3.Name + " " + t2.ProductName,
                                              Quantity = t1.Quantity,
                                              FProductId = t1.ProductId,
                                              UnitName = t5.Name
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<VMProdReferenceSlave> GetSingleProd_ReferenceSlaveConsumption(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.ProdReferenceSlaveConsumptionID == id)
                                          join t2 in _db.Products on t1.RProductId equals t2.ProductId
                                          join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                          join t4 in _db.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId
                                          join t5 in _db.Units on t2.UnitId equals t5.UnitId

                                          where t1.IsActive == true
                                          select new VMProdReferenceSlave
                                          {
                                              CostingPrice = t1.COGS,
                                              FactoryExpensesHeadGLId = t1.FactoryExpensesHeadGLId,
                                              RawProductName = t4.Name + " " + t3.Name + " " + t2.ProductName,
                                              ProdReferenceId = t1.ProdReferenceId.Value,
                                              ID = t1.ProdReferenceSlaveConsumptionID,
                                              ProdReferenceSlaveID = t1.ProdReferenceSlaveID,
                                              RProductId = t1.RProductId,
                                              Quantity = t1.TotalConsumeQuantity,
                                              FectoryExpensesAmount = t1.UnitPrice,
                                              CompanyFK = t1.CompanyId,
                                              CreatedBy = t1.CreatedBy,
                                              CreatedDate = t1.CreatedDate,
                                              UnitName = t5.Name
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<VMProdReferenceSlave> GetSingleProdReferenceSlaveExpansessConsumption(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.ProdReferenceSlaveConsumptionID == id)
                                          join t2 in _db.HeadGLs on t1.FactoryExpensesHeadGLId equals t2.Id


                                          where t1.IsActive == true
                                          select new VMProdReferenceSlave
                                          {

                                              FactoryExpensesHeadGLId = t1.FactoryExpensesHeadGLId,
                                              FactoryExpecsesHeadName = t2.AccCode + " - " + t2.AccName,
                                              ID = t1.ProdReferenceSlaveConsumptionID,
                                              ProdReferenceSlaveID = t1.ProdReferenceSlaveID,
                                              FectoryExpensesAmount = t1.UnitPrice,
                                              CompanyFK = t1.CompanyId,
                                              CreatedBy = t1.CreatedBy,
                                              CreatedDate = t1.CreatedDate
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<VMProdReferenceSlave> GetSingleProductionDetails(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.ProductionDetails.Where(x => x.ProductionDetailId == id)
                                          join t2 in _db.HeadGLs on t1.ExpensesHeadGLId equals t2.Id


                                          where t1.IsActive == true
                                          select new VMProdReferenceSlave
                                          {

                                              FactoryExpensesHeadGLId = t1.ExpensesHeadGLId,
                                              FactoryExpecsesHeadName = t2.AccCode + " - " + t2.AccName,
                                              ID = (int)t1.ProductionDetailId,
                                              ProductionId = t1.ProductionId.Value,
                                              FectoryExpensesAmount = t1.COGS,
                                              CreatedBy = t1.CreatedBy,
                                              CreatedDate = t1.CreatedDate
                                          }).FirstOrDefault());
            return v;
        }
        public async Task<int> ProdReferenceSlaveAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;
            Prod_ReferenceSlave prodReferenceSlave = new Prod_ReferenceSlave
            {
                CostingPrice = vmProdReferenceSlave.CostingPrice,
                FProductId = vmProdReferenceSlave.FProductId,
                ProdReferenceId = vmProdReferenceSlave.ProdReferenceId,
                Quantity = vmProdReferenceSlave.Quantity,
                CompanyId = vmProdReferenceSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                LotNumber=vmProdReferenceSlave.LotNumber
            };
            _db.Prod_ReferenceSlave.Add(prodReferenceSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = prodReferenceSlave.ProdReferenceSlaveID;
            }

            if (result > 0)
            {
                var bomsOfProduct = _db.FinishProductBOMs.Where(x => x.FProductFK == vmProdReferenceSlave.FProductId && x.IsActive).AsEnumerable();
                List<Prod_ReferenceSlaveConsumption> List = new List<Prod_ReferenceSlaveConsumption>();
                foreach (var bom in bomsOfProduct)
                {
                    VMProductStock vMProductStock = new VMProductStock();
                    vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetSeedRMStockByProductId {0},{1},{2}", bom.RProductFK, bom.CompanyId, bom.LotNumber ?? "xyzz").FirstOrDefault();
                    bom.RequiredQuantity = bom.CalculationUnit.Value == (int)FormulaCalculationEnum.gm ? bom.RequiredQuantity / 1000 : bom.RequiredQuantity;

                    Prod_ReferenceSlaveConsumption prod_ReferenceSlaveConsumption = new Prod_ReferenceSlaveConsumption
                    {
                        RProductId = bom.RProductFK,
                        TotalConsumeQuantity = bom.RequiredQuantity * vmProdReferenceSlave.Quantity,
                        COGS = vMProductStock.ClosingRate,
                        ProdReferenceSlaveID = result,
                        CompanyId = vmProdReferenceSlave.CompanyFK,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    List.Add(prod_ReferenceSlaveConsumption);
                }
                _db.Prod_ReferenceSlaveConsumption.AddRange(List);
                await _db.SaveChangesAsync();
            }
            #region Update Finished Goods COGS
            var consumeAmount = _db.Prod_ReferenceSlaveConsumption.Where(x => x.ProdReferenceSlaveID == result
                               && x.IsActive).Select(x => x.TotalConsumeQuantity * x.COGS).DefaultIfEmpty(0).Sum();
            if (consumeAmount > 0)
            {
                Prod_ReferenceSlave referenceSlave = _db.Prod_ReferenceSlave.Find(result);
                referenceSlave.CostingPrice = consumeAmount / referenceSlave.Quantity;
                await _db.SaveChangesAsync();

            }
            #endregion


            return result;
        }

        public async Task<int> ProdReferenceSlaveByChallanAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;
            var DataListSlavePartial = vmProdReferenceSlave.DataToList.Where(x => x.Quantity > 0).ToList();
            foreach (var item in DataListSlavePartial)
            {
                int finishProductId = _db.FinishProductBOMs.Where(x => x.RProductFK == item.RProductId).Select(x => x.FProductFK).FirstOrDefault();
                Prod_ReferenceSlave prodReferenceSlave = new Prod_ReferenceSlave
                {
                    FProductId = finishProductId,
                    ProdReferenceId = vmProdReferenceSlave.ProdReferenceId,
                    Quantity = item.Quantity,
                    CompanyId = vmProdReferenceSlave.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                _db.Prod_ReferenceSlave.Add(prodReferenceSlave);
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReferenceSlave.ProdReferenceSlaveID;
                }


                if (result > 0)
                {
                    var bomsOfProduct = _db.FinishProductBOMs.Where(x => x.FProductFK == prodReferenceSlave.FProductId).AsEnumerable();
                    List<Prod_ReferenceSlaveConsumption> List = new List<Prod_ReferenceSlaveConsumption>();
                    foreach (var bom in bomsOfProduct)
                    {
                        VMProductStock vMProductStock = new VMProductStock();
                        vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetSeedRMStockByProductId {0},{1}", bom.RProductFK, bom.CompanyId).FirstOrDefault();


                        Prod_ReferenceSlaveConsumption prod_ReferenceSlaveConsumption = new Prod_ReferenceSlaveConsumption
                        {
                            RProductId = bom.RProductFK,
                            TotalConsumeQuantity = bom.RequiredQuantity * item.Quantity,
                            ProdReferenceSlaveID = result,
                            COGS = vMProductStock.ClosingRate,
                            CompanyId = vmProdReferenceSlave.CompanyFK,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        };
                        List.Add(prod_ReferenceSlaveConsumption);
                    }
                    _db.Prod_ReferenceSlaveConsumption.AddRange(List);
                    await _db.SaveChangesAsync();
                }

                #region Update Finished Goods COGS
                var consumeAmount = _db.Prod_ReferenceSlaveConsumption.Where(x => x.ProdReferenceSlaveID == result
                                   && x.IsActive).Select(x => x.TotalConsumeQuantity * x.COGS).DefaultIfEmpty(0).Sum();
                if (consumeAmount > 0)
                {
                    Prod_ReferenceSlave referenceSlave = _db.Prod_ReferenceSlave.Find(result);
                    referenceSlave.CostingPrice = consumeAmount / referenceSlave.Quantity;
                    await _db.SaveChangesAsync();

                }
                #endregion


            }
            return result;
        }



        public async Task<int> ProdReferenceSlaveRawConsumptionEdit(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;
            Product productModel = await _db.Products.FindAsync(vmProdReferenceSlave.RProductId);

            Prod_ReferenceSlaveConsumption model = await _db.Prod_ReferenceSlaveConsumption.FindAsync(vmProdReferenceSlave.ID);

            model.RProductId = vmProdReferenceSlave.RProductId;
            model.TotalConsumeQuantity = vmProdReferenceSlave.RawConsumeQuantity;
            model.COGS = productModel.CostingPrice;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmProdReferenceSlave.ID;
            }

            return result;
        }
        public async Task<int> DeleteProdReferenceSlaveConsumption(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;
            Prod_ReferenceSlaveConsumption model = await _db.Prod_ReferenceSlaveConsumption.FindAsync(vmProdReferenceSlave.ID);
            model.IsActive = false;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.ProdReferenceSlaveConsumptionID;
            }

            return result;
        }

        public async Task<long> DeleteProductionDetail(VMProdReferenceSlave vmProdReferenceSlave)
        {
            long result = -1;
            ProductionDetail model = await _db.ProductionDetails.FindAsync(vmProdReferenceSlave.ID);
            model.IsActive = false;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.ProductionDetailId;
            }

            return result;
        }

        public async Task<int> UpdateCostingPriceProdReferenceSlave(VMProdReferenceSlave FinishDataList)
        {
            var result = -1;
            foreach (var finishDataList in FinishDataList.FinishDataListSlave)
            {
                var priviousStockHistory = _db.Database.SqlQuery<GcclFinishProductCurrentStock>("exec GCCLFinishedStockByProduct {0}, {1},{2},{3}", finishDataList.CompanyFK, finishDataList.FProductId, FinishDataList.ReferenceDate, 0).FirstOrDefault();
                Product product = _db.Products.Find(finishDataList.FProductId);
                product.CostingPrice = priviousStockHistory.AvgClosingRate;
            }
            if (await _db.SaveChangesAsync() > 0)
            {
                result = 1;
            }

            return result;
        }

        public async Task<int> UpdateCostingPriceProductionDetail(VMProdReferenceSlave FinishDataList)
        {
            var result = -1;
            foreach (var finishDataList in FinishDataList.FinishDataListSlave)
            {
                var priviousStockHistory = _db.Database.SqlQuery<GcclFinishProductCurrentStock>("exec GCCLFinishedStockByProduct {0}, {1},{2},{3}", finishDataList.CompanyFK, finishDataList.FProductId, FinishDataList.ProductionItemDate, 0).FirstOrDefault();
                Product product = _db.Products.Find(finishDataList.FProductId);
                product.CostingPrice = priviousStockHistory.AvgClosingRate;
            }
            if (await _db.SaveChangesAsync() > 0)
            {
                result = 1;
            }

            return result;
        }

        public async Task<int> SubmitProdReference(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;
            Prod_Reference model = await _db.Prod_Reference.FindAsync(vmProdReferenceSlave.ProdReferenceId);
            model.IsSubmitted = true;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;

            model.ModifiedDate = DateTime.Now;



            if (vmProdReferenceSlave.CompanyFK == (int)CompanyName.GloriousCropCareLimited)
            {

                vmProdReferenceSlave = await GCCLProdReferenceSlaveGet(model.CompanyId.Value, model.ProdReferenceId);

                List<Prod_ReferenceSlave> ProdReferenceSlaveList = new List<Prod_ReferenceSlave>();

                foreach (var item in vmProdReferenceSlave.FinishDataListSlave)
                {
                    var prodReferenceSlave = await _db.Prod_ReferenceSlave.FindAsync(item.ProdReferenceSlaveID);
                    prodReferenceSlave.CostingPrice = item.CostingPrice;
                    ProdReferenceSlaveList.Add(prodReferenceSlave);
                }
                ProdReferenceSlaveList.ForEach(x => x.ModifiedDate = DateTime.Now);
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = model.ProdReferenceId;
                    await UpdateCostingPriceProductionDetail(vmProdReferenceSlave);

                }

                string title = "Integrated Journal- Production Process: " + vmProdReferenceSlave.ReferenceNo + ". Reference Date: " + vmProdReferenceSlave.ReferenceDate.ToShortDateString();
                string description = "";


                await _accountingService.AccountingProductionPushGCCL(vmProdReferenceSlave.ReferenceDate, vmProdReferenceSlave.CompanyFK.Value, vmProdReferenceSlave, title, description, (int)GCCLJournalEnum.ProductionVoucher);

            }

            return result;
        }


        public async Task<long> SubmitProduction(VMProdReferenceSlave vmProdReferenceSlave)
        {
            long result = -1;
            Data.Models.Production model = await _db.Productions.FindAsync(vmProdReferenceSlave.ProductionId);
            model.IsSubmitted = true;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;

            model.ModifiedDate = DateTime.Now;



            if (model != null)
            {

                vmProdReferenceSlave = await ProductionReferenceGet(model.CompanyId.Value, model.ProductionId);

                List<ProductionItem> ProductionItemList = new List<ProductionItem>();

                foreach (var item in vmProdReferenceSlave.FinishDataListSlave)
                {
                    var prodReferenceSlave = await _db.ProductionItems.FindAsync(item.ProductionItemId);
                    prodReferenceSlave.CostingPrice = item.CostingPrice;
                    ProductionItemList.Add(prodReferenceSlave);
                }
                ProductionItemList.ForEach(x => x.ModifiedDate = DateTime.Now);
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = model.ProductionId;
                    //await UpdateCostingPriceProdReferenceSlave(vmProdReferenceSlave);

                }

                string title = "Production Process: " + vmProdReferenceSlave.ProductionNo + ". Reference Date: " + vmProdReferenceSlave.ProductionDate.ToShortDateString();
                string description = "";//"From Raw Materials: " + rawProductNames + ". To Finish Item: " + finishProduct.ProductName;


                await _accountingService.AccountingProductionPushISS(vmProdReferenceSlave.ProductionDate, vmProdReferenceSlave.CompanyFK.Value, vmProdReferenceSlave, title, description, (int)GCCLJournalEnum.ProductionVoucher);

            }

            return result;
        }
        public async Task<long> ProductionDetailExpensesVoucher(VMProdReferenceSlave vmProdReferenceSlave)
        {
            long result = -1;

            // Store the important ID values before potentially dropping the reference  
            var paymentHeadId = vmProdReferenceSlave.AdvanceHeadGLId;
            var productionDetailId = vmProdReferenceSlave.ID;

            // Retrieve production reference data  
            vmProdReferenceSlave = await ProductionReferenceGet(vmProdReferenceSlave.CompanyFK.Value, vmProdReferenceSlave.ProductionId,vmProdReferenceSlave.ID);

            // Restore the original values  
            vmProdReferenceSlave.AdvanceHeadGLId = paymentHeadId;
            vmProdReferenceSlave.ID = productionDetailId;


            string title = $"Production Process: {vmProdReferenceSlave.ProductionNo}. Production Date: {vmProdReferenceSlave.ProductionDate.ToShortDateString()}";
            string description = "";


            await _accountingService.AccountingProductionExpencePush(
                vmProdReferenceSlave.ProductionDate,
                vmProdReferenceSlave.CompanyFK.Value,
                vmProdReferenceSlave,
                title,
                description,
                (int)GCCLJournalEnum.ProductionVoucher
            );

            return result;
        }


        public async Task<int> DeleteProdReferenceSlave(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;
            Prod_ReferenceSlave model = await _db.Prod_ReferenceSlave.FindAsync(vmProdReferenceSlave.ProdReferenceSlaveID);
            model.IsActive = false;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.ProdReferenceSlaveID;
            }

            return result;
        }

        public async Task<int> DeleteProductionItem(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;
            ProductionItem model = await _db.ProductionItems.FindAsync(vmProdReferenceSlave.ProductionItemId);
            model.IsActive = false;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.ProductionItemID;
            }

            return result;
        }
        public async Task<int> ProdReferenceSlaveFactoryConsumptionEdit(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;

            Prod_ReferenceSlaveConsumption model = await _db.Prod_ReferenceSlaveConsumption.FindAsync(vmProdReferenceSlave.ID);

            model.UnitPrice = vmProdReferenceSlave.FectoryExpensesAmount;
            model.FactoryExpensesHeadGLId = vmProdReferenceSlave.FactoryExpensesHeadGLId;


            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmProdReferenceSlave.ID;
            }

            return result;
        }

        public async Task<int> ProductionDetailEdit(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;

            ProductionDetail model = await _db.ProductionDetails.FindAsync(vmProdReferenceSlave.ID);

            model.COGS = vmProdReferenceSlave.FectoryExpensesAmount;
            model.ExpensesHeadGLId = vmProdReferenceSlave.FactoryExpensesHeadGLId;


            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmProdReferenceSlave.ID;
            }

            return result;
        }

        public async Task<int> ProdReferenceSlaveEdit(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;
            Prod_ReferenceSlave model = await _db.Prod_ReferenceSlave.FindAsync(vmProdReferenceSlave.ProdReferenceSlaveID);

            model.FProductId = vmProdReferenceSlave.FProductId;
            model.Quantity = vmProdReferenceSlave.Quantity;
            model.QuantityOver = vmProdReferenceSlave.QuantityOver;
            model.QuantityLess = vmProdReferenceSlave.QuantityLess;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmProdReferenceSlave.ID;
            }

            return result;
        }

        public async Task<int> ProductionItemEdit(VMProdReferenceSlave vmProdReferenceSlave)
        {
            var result = -1;
            ProductionItem model = await _db.ProductionItems.FindAsync(vmProdReferenceSlave.ProductionItemId);

            model.ProductId = vmProdReferenceSlave.FProductId;
            model.Quantity = vmProdReferenceSlave.Quantity;
            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmProdReferenceSlave.ID;
            }

            return result;
        }
        public async Task<int> Prod_ReferenceSlaveDelete(long id)
        {
            int result = -1;
            Prod_ReferenceSlave prodReferenceSlave = await _db.Prod_ReferenceSlave.FindAsync(id);
            if (prodReferenceSlave != null)
            {
                prodReferenceSlave.IsActive = false;

                List<Prod_ReferenceSlaveConsumption> conList = await _db.Prod_ReferenceSlaveConsumption.Where(x => x.ProdReferenceSlaveID == prodReferenceSlave.ProdReferenceSlaveID && x.IsActive).ToListAsync();
                conList.ForEach(x => x.IsActive = false);
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReferenceSlave.ProdReferenceId;
                }
            }
            return result;
        }

        #endregion


        public object GetAutoCompleteMaterialReceives(int companyId, string prefix)
        {
            var v = (from t1 in _db.MaterialReceives.Where(x => x.CompanyId == companyId)

                     where t1.IsActive && ((t1.ReceiveNo.StartsWith(prefix)) || (t1.ChallanNo.StartsWith(prefix)))

                     select new
                     {
                         label = t1.ReceiveNo + " Date " + t1.ChallanDate,
                         val = t1.MaterialReceiveId
                     }).OrderBy(x => x.label).Take(20).ToList();

            return v;
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
                     where t1.IsActive && ((t1.Name.StartsWith(prefix)) || (t1.Code.StartsWith(prefix)))

                     select new
                     {
                         label = t1.Name,
                         val = t1.VendorId
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


        public List<VMProdReferenceSlave> GetMaterialReceiveDetailsData(int materialReceiveId)
        {

            var list = (from t1 in _db.MaterialReceiveDetails
                        join t2 in _db.MaterialReceives on t1.MaterialReceiveId equals t2.MaterialReceiveId
                        join t5 in _db.Products on t1.ProductId equals t5.ProductId
                        join t6 in _db.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                        join t7 in _db.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                        join t8 in _db.Units on t5.UnitId equals t8.UnitId
                        where t1.IsActive && t5.IsActive && t6.IsActive && t7.IsActive && //t8.IsActive &&
                                 t1.MaterialReceiveId == materialReceiveId

                        select new VMProdReferenceSlave
                        {
                            ProductName = t7.Name + " " + t6.Name + " " + t5.ProductName,
                            RProductId = t1.ProductId.Value,
                            ReceivedQuantity = t1.ReceiveQty,
                            PurchasePrice = t1.UnitPrice,
                            PriviousProcessQuantity = 0, // _db.Prod_ReferenceSlaveConsumption.Where(x => x.RProductId == t1.ProductId && x.IsActive).Select(x => x.TotalConsumeQuantity).DefaultIfEmpty(0m).Sum(),
                            UnitName = t8.Name,
                        }).ToList();



            return list;
        }

        public async Task<int> GCCLProd_ReferenceSlaveConsumptionAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;
            var product = await _db.Products.FindAsync(vmProdReferenceSlave.RProductId);
            Prod_ReferenceSlaveConsumption prodReferenceSlaveConsumption = new Prod_ReferenceSlaveConsumption
            {
                RProductId = vmProdReferenceSlave.RProductId,
                TotalConsumeQuantity = vmProdReferenceSlave.RawConsumeQuantity,
                COGS = product.CostingPrice,
                ProdReferenceId = vmProdReferenceSlave.ProdReferenceId,
                ProdReferenceSlaveID = 0,
                CompanyId = vmProdReferenceSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Prod_ReferenceSlaveConsumption.Add(prodReferenceSlaveConsumption);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = prodReferenceSlaveConsumption.ProdReferenceSlaveConsumptionID;
            }

            //if (result > 0 && !vmProdReferenceSlave.MakeFinishInventory)
            //{
            //    var bomsOfProduct = _db.FinishProductBOMs.Where(x => x.FProductFK == vmProdReferenceSlave.FProductId && x.IsActive).AsEnumerable();
            //    List<Prod_ReferenceSlaveConsumption> List = new List<Prod_ReferenceSlaveConsumption>();
            //    foreach (var bom in bomsOfProduct)
            //    {
            //        var rawProduct = _db.Products.Find(bom.RProductFK);
            //        Prod_ReferenceSlaveConsumption prod_ReferenceSlaveConsumption = new Prod_ReferenceSlaveConsumption
            //        {
            //            RProductId = bom.RProductFK,
            //            TotalConsumeQuantity = bom.RequiredQuantity * vmProdReferenceSlave.Quantity,
            //            COGS = rawProduct.CostingPrice,
            //            ProdReferenceSlaveID = result,
            //            CompanyId = vmProdReferenceSlave.CompanyFK,
            //            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
            //            CreatedDate = DateTime.Now,
            //            IsActive = true
            //        };
            //        List.Add(prod_ReferenceSlaveConsumption);
            //    }
            //    _db.Prod_ReferenceSlaveConsumption.AddRange(List);
            //    await _db.SaveChangesAsync();
            //}

            //if (prodReferenceSlave.Quantity > 0)
            //{
            //    #region Ready To GRN

            //    var consumptionCost = (from t1 in _db.FinishProductBOMs.Where(x => x.FProductFK == prodReferenceSlave.FProductId && x.IsActive)
            //                           join t2 in _db.Products on t1.RProductFK equals t2.ProductId
            //                           select t1.RequiredQuantity * t2.CostingPrice).DefaultIfEmpty(0).Sum();

            //    vmProdReferenceSlave.ProdReferenceSlaveID = prodReferenceSlave.ProdReferenceSlaveID;
            //    vmProdReferenceSlave.FProductId = prodReferenceSlave.FProductId;
            //    vmProdReferenceSlave.Quantity = prodReferenceSlave.Quantity;
            //    vmProdReferenceSlave.CostingPrice = consumptionCost;
            //    #endregion

            //    await ProductGRNEdit(vmProdReferenceSlave);
            //}

            //if (result > 0 && vmProdReferenceSlave.CompanyFK == (int)CompanyName.GloriousCropCareLimited)
            //{
            //    Prod_Reference prodReference = await _db.Prod_Reference.Where(x => x.ProdReferenceId == prodReferenceSlave.ProdReferenceId).FirstOrDefaultAsync();

            //    var totalRawCnsumeAmount = _db.Prod_ReferenceSlaveConsumption
            //                                  .Where(x => x.ProdReferenceSlaveID == prodReferenceSlave.ProdReferenceSlaveID && x.IsActive)
            //                                  .Select(x => x.TotalConsumeQuantity * x.COGS).DefaultIfEmpty(0).Sum();

            //    Product finishProduct = await _db.Products.Where(x => x.ProductId == prodReferenceSlave.FProductId).FirstOrDefaultAsync();
            //    List<string> rawProduct = (from t1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.ProdReferenceSlaveID == prodReferenceSlave.ProdReferenceSlaveID && x.IsActive)
            //                               join t2 in _db.Products on t1.RProductId equals t2.ProductId
            //                               join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
            //                               select t3.Name + " " + t2.ProductName).ToList();
            //    string rawProductNames = string.Join(",", rawProduct);

            //    string title = "Integrated Journal- Production Process: " + prodReference.ReferenceNo + ". Reference Date: " + prodReference.ReferenceDate.ToShortDateString();
            //    string description = "From Raw Materials: " + rawProductNames + ". To Finish Item: " + finishProduct.ProductName;

            //    List<AccountList> crAccountList = (from t1 in _db.Prod_ReferenceSlaveConsumption
            //                                  .Where(x => x.ProdReferenceSlaveID == prodReferenceSlave.ProdReferenceSlaveID && x.IsActive)
            //                                       join t2 in _db.Products on t1.RProductId equals t2.ProductId
            //                                       join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
            //                                       select new AccountList
            //                                       {
            //                                           AccountingHeadId = t3.AccountingHeadId,
            //                                           Value = t1.TotalConsumeQuantity * t1.COGS
            //                                       }).ToList();

            //    int drAccountHead = await (from t1 in _db.ProductSubCategories.Where(x => x.ProductSubCategoryId == finishProduct.ProductSubCategoryId)
            //                               select t1.AccountingHeadId.Value).FirstOrDefaultAsync();
            //    await _accountingService.AccountingJournalPushGCCL(prodReference.ReferenceDate, vmProdReferenceSlave.CompanyFK.Value, drAccountHead, crAccountList, totalRawCnsumeAmount, title, description, (int)JournalEnum.JournalVoucer);

            //}

            return result;
        }

        public async Task<int> GCCLProdReferenceFactoryExpensesAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;

            Prod_ReferenceSlaveConsumption prodReferenceSlaveConsumption = new Prod_ReferenceSlaveConsumption
            {

                UnitPrice = vmProdReferenceSlave.FectoryExpensesAmount,
                FactoryExpensesHeadGLId = vmProdReferenceSlave.FactoryExpensesHeadGLId,
                ProdReferenceId = vmProdReferenceSlave.ProdReferenceId,
                ProdReferenceSlaveID = 0,
                CompanyId = vmProdReferenceSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Prod_ReferenceSlaveConsumption.Add(prodReferenceSlaveConsumption);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = prodReferenceSlaveConsumption.ProdReferenceSlaveConsumptionID;
            }


            return result;
        }

        public async Task<long> ProductionReferenceExpensesAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            long result = -1;

            ProductionDetail productionDetail = new ProductionDetail
            {
                ProductionId = vmProdReferenceSlave.ProductionId,
                ExpensesHeadGLId = vmProdReferenceSlave.FactoryExpensesHeadGLId,
                COGS = vmProdReferenceSlave.FectoryExpensesAmount,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                ExpensesDate = vmProdReferenceSlave.ExpensesDate,
                IsActive = true
            };
            _db.ProductionDetails.Add(productionDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = productionDetail.ProductionDetailId;
            }


            return result;
        }
        public List<object> GCCLLCFactoryExpanceHeadGLList(int companyId)
        {
            var List = new List<object>();
            //26 07 2023 requirment from smsunnahar gccl. need to add jaibo sar advance code (id of advance is 50613100)
            foreach (var item in _db.HeadGLs.Where(x => x.CompanyId == companyId && (x.ParentId == 50625189 || x.ParentId == 50625195)).ToList())
            {
                List.Add(new { Text = item.AccCode + " -" + item.AccName, Value = item.Id });
            }
            return List;

        }
        public List<object> GCCLAdvanceHeadGLList(int companyId)
        {
            var List = new List<object>();
            foreach (var item in _db.HeadGLs.Where(x => x.CompanyId == companyId && x.ParentId == 37919).ToList())
            {
                List.Add(new { Text = item.AccCode + " -" + item.AccName, Value = item.Id });
            }
            return List;

        }

        public List<object> ISSExpensessHeadGLList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.HeadGLs
                     join t2 in _db.Head5 on t1.ParentId equals t2.Id
                     join t3 in _db.Head4 on t2.ParentId equals t3.Id
                     join t4 in _db.Head3 on t3.ParentId equals t4.Id
                     join t5 in _db.Head2 on t4.ParentId equals t5.Id
                     where (t5.AccCode == "42" || t5.AccCode == "43")
                     && t1.CompanyId == companyId
                     select new
                     {
                         Id = t1.Id,
                         AccName = t5.AccName + " - " + t1.AccCode + " - " + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.AccName, Value = item.Id });
            }
            return List;

        }



        public List<object> ISSBankOrCashHeadGLList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.HeadGLs
                     join t2 in _db.Head5 on t1.ParentId equals t2.Id
                     join t3 in _db.Head4 on t2.ParentId equals t3.Id
                     join t4 in _db.Head3 on t3.ParentId equals t4.Id
                     join t5 in _db.Head2 on t4.ParentId equals t5.Id
                     where (t4.AccCode == "1301")
                     && t1.CompanyId == companyId
                     select new
                     {
                         Id = t1.Id,
                         AccName = t5.AccName + " - " + t1.AccCode + " - " + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.AccName, Value = item.Id });
            }
            return List;

        }

        public List<object> GCCLAdvanceAndRecebableHeadGLList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.HeadGLs
                     join t2 in _db.Head5 on t1.ParentId equals t2.Id
                     join t3 in _db.Head4 on t2.ParentId equals t3.Id
                     join t4 in _db.Head3 on t3.ParentId equals t4.Id
                     where (t4.AccCode == "1302" || t4.AccCode == "1304")
                     && t1.CompanyId == companyId
                     select new
                     {
                         Id = t1.Id,
                         AccName = t1.AccCode + " -" + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.AccName, Value = item.Id });
            }
            return List;

        }
        public async Task<VMProdReferenceSlave> ProductionReferenceGet(int companyId, long productionId, long productionDetailsId = 0)
        {
            VMProdReferenceSlave vmProdReferenceSlave = new VMProdReferenceSlave();
            vmProdReferenceSlave = await Task.Run(() => (from t1 in _db.Productions.Where(x => x.IsActive && x.ProductionId == productionId && x.CompanyId == companyId)
                                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                         select new VMProdReferenceSlave
                                                         {
                                                             ProductionId = t1.ProductionId,
                                                             ProductionNo = t1.ProductionNo,
                                                             ProductionDate = t1.ProductionDate,
                                                             CompanyFK = t1.CompanyId,
                                                             CompanyName = t3.Name,
                                                             CompanyAddress = t3.Address,
                                                             CompanyEmail = t3.Email,
                                                             CompanyPhone = t3.Phone,
                                                             IsSubmitted = t1.IsSubmitted,
                                                             //TotalRawConsumedAmount = (from st1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == productionId && x.CompanyId == companyId && x.RProductId > 0)
                                                             //                          select st1.TotalConsumeQuantity * st1.COGS).DefaultIfEmpty(0).Sum(),
                                                             TotalFactoryExpensessAmount = (from st1 in _db.ProductionDetails.Where(x => x.IsActive && x.ProductionId == productionId && x.ExpensesHeadGLId > 0)
                                                                                            select st1.COGS).DefaultIfEmpty(0).Sum(),
                                                             PriviousProcessQuantity = (from st1 in _db.ProductionItems.Where(x => x.IsActive && x.ProductionId == productionId && x.ProductId > 0)
                                                                                        select st1.Quantity).DefaultIfEmpty(0).Sum(),

                                                         }).FirstOrDefault());


            var query = _db.ProductionDetails.Where(x => x.IsActive && x.ProductionId == productionId && x.IsActive);

            if (productionDetailsId > 0)
            {
                query = query.Where(x => x.ProductionDetailId == productionDetailsId);
            }

            vmProdReferenceSlave.DataListSlave = await Task.Run(() =>
                (from t1 in query
                 join t3 in _db.HeadGLs on t1.ExpensesHeadGLId equals t3.Id
                 join t4 in _db.Head5 on t3.ParentId equals t4.Id
                 select new VMProdReferenceSlave
                 {
                     ID = (int)t1.ProductionDetailId,
                     FactoryExpecsesHeadName = t3.AccCode + " - " + t3.AccName,
                     ProductionId = t1.ProductionId.Value,
                     FectoryExpensesAmount = t1.COGS,
                     FactoryExpensesHeadGLId = t1.ExpensesHeadGLId,
                     ExpensesDate = t1.ExpensesDate.Value,
                     IsVoucher = t1.IsVoucher
                 })
                 .OrderByDescending(x => x.ID)
                 .ToListAsync()
            );


            var totalCOGS = (from pd1 in _db.ProductionDetails
                             where pd1.ProductionId == productionId && pd1.IsActive
                             select pd1.COGS).DefaultIfEmpty(0).Sum();

            var totalQuantity = (from pi1 in _db.ProductionItems
                                 where pi1.ProductionId == productionId && pi1.IsActive
                                 select pi1.Quantity).DefaultIfEmpty(0).Sum();

            // Calculate Costing Price based on conditions  
            decimal costingPrice;
            if (totalCOGS > 0 && totalQuantity > 0)
            {
                costingPrice = totalCOGS / totalQuantity;
            }
            else
            {
                costingPrice = 0; // or whatever default value you wish to use  
            }


            vmProdReferenceSlave.FinishDataListSlave = await Task.Run(() => (from t1 in _db.ProductionItems.Where(x => x.IsActive && x.ProductionId == productionId && x.IsActive)
                                                                             join t3 in _db.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                                             join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                             join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                             join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                             select new VMProdReferenceSlave
                                                                             {
                                                                                 AccountingHeadId = t5.AccountingHeadId,
                                                                                 ProductionItemId = t1.ProductionItemID,
                                                                                 ProductName = t4.Name + " " + t3.ProductName,
                                                                                 ProductionId = t1.ProductionId,
                                                                                 ProductCategoryId = t5.ProductCategoryId,
                                                                                 FProductId = t1.ProductId,
                                                                                 Quantity = t1.Quantity,
                                                                                 UnitName = t6.Name,
                                                                                 ProductionItemDate = t1.ProductionItemDate.Value,
                                                                                 //PurchasePrice = t3.FormulaQty.Value,
                                                                                 //CostingPrice = t1.CostingPrice

                                                                                 //CostingPrice = Math.Round((((from st1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId && x.RProductId > 0)
                                                                                 //                             select st1.TotalConsumeQuantity * st1.COGS).DefaultIfEmpty(0m).Sum() +
                                                                                 //                          (from st1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId && x.FactoryExpensesHeadGLId > 0)
                                                                                 //                           select st1.UnitPrice).DefaultIfEmpty(0m).Sum()) /
                                                                                 //                           ((from prst1 in _db.Prod_ReferenceSlave.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                                                 //                             join prst2 in _db.Products.Where(x => x.IsActive) on prst1.FProductId equals prst2.ProductId
                                                                                 //                             select ((prst1.Quantity + prst1.QuantityOver) - prst1.QuantityLess) * prst2.FormulaQty.Value
                                                                                 //                            ).DefaultIfEmpty(0m).Sum())) * t3.FormulaQty.Value, 2),
                                                                                 CostingPrice = costingPrice

                                                                             }).OrderByDescending(x => x.ProductionId).ToListAsync());
            return vmProdReferenceSlave;
        }


        public async Task<VMProdReferenceSlave> GCCLProdReferenceSlaveGet(int companyId, long prodReferenceId)
        {
            VMProdReferenceSlave vmProdReferenceSlave = new VMProdReferenceSlave();
            vmProdReferenceSlave = await Task.Run(() => (from t1 in _db.Prod_Reference.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                         join t2 in _db.HeadGLs on t1.HeadGLId equals t2.Id

                                                         join t3 in _db.Companies on t1.CompanyId equals t3.CompanyId

                                                         select new VMProdReferenceSlave
                                                         {
                                                             ProdReferenceId = t1.ProdReferenceId,
                                                             ReferenceNo = t1.ReferenceNo,
                                                             ReferenceDate = t1.ReferenceDate,
                                                             CompanyFK = t1.CompanyId,
                                                             AdvanceHeadGLName = t2.AccCode + " - " + t2.AccName,
                                                             AdvanceHeadGLId = t1.HeadGLId,
                                                             CompanyName = t3.Name,
                                                             CompanyAddress = t3.Address,
                                                             CompanyEmail = t3.Email,
                                                             CompanyPhone = t3.Phone,
                                                             IsSubmitted = t1.IsSubmitted,
                                                             TotalRawConsumedAmount = (from st1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId && x.RProductId > 0)
                                                                                       select st1.TotalConsumeQuantity * st1.COGS).DefaultIfEmpty(0).Sum(),
                                                             TotalFactoryExpensessAmount = (from st1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId && x.FactoryExpensesHeadGLId > 0)
                                                                                            select st1.UnitPrice).DefaultIfEmpty(0).Sum(),
                                                             PriviousProcessQuantity = (from st1 in _db.Prod_ReferenceSlave.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId && x.FProductId > 0)

                                                                                        select (st1.Quantity + st1.QuantityOver) - st1.QuantityLess
                                                                                         ).DefaultIfEmpty(0m).Sum()

                                                         }).FirstOrDefault());
            vmProdReferenceSlave.DataListSlave = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                                       join t3 in _db.HeadGLs on t1.FactoryExpensesHeadGLId equals t3.Id
                                                                       join t4 in _db.Head5 on t3.ParentId equals t4.Id
                                                                       select new VMProdReferenceSlave
                                                                       {
                                                                           ID = t1.ProdReferenceSlaveConsumptionID,
                                                                           FactoryExpecsesHeadName = t3.AccCode + " - " + t3.AccName,
                                                                           ProdReferenceId = t1.ProdReferenceId.Value,
                                                                           FectoryExpensesAmount = t1.UnitPrice,
                                                                           FactoryExpensesHeadGLId = t1.FactoryExpensesHeadGLId,
                                                                           CompanyFK = t1.CompanyId
                                                                       }).OrderByDescending(x => x.ID).ToListAsync());


            vmProdReferenceSlave.RawDataListSlave = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                                          join t3 in _db.Products.Where(x => x.IsActive) on t1.RProductId equals t3.ProductId
                                                                          join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                          join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                          join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                          select new VMProdReferenceSlave
                                                                          {
                                                                              AccountingHeadId = t4.AccountingHeadId,
                                                                              ID = t1.ProdReferenceSlaveConsumptionID,
                                                                              ProductName = t5.Name + " " + t4.Name + " " + t3.ProductName,
                                                                              ProdReferenceId = t1.ProdReferenceId.Value,
                                                                              ProdReferenceSlaveID = t1.ProdReferenceSlaveID,
                                                                              RProductId = t1.RProductId,
                                                                              Quantity = t1.TotalConsumeQuantity,

                                                                              UnitName = t6.Name,
                                                                              CompanyFK = t1.CompanyId,
                                                                              CostingPrice = t1.COGS,
                                                                              TotalPrice = t1.TotalConsumeQuantity * t1.COGS,

                                                                          }).OrderByDescending(x => x.ID).ToListAsync());
            vmProdReferenceSlave.FinishDataListSlave = await Task.Run(() => (from t1 in _db.Prod_ReferenceSlave.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                                             join t3 in _db.Products.Where(x => x.IsActive) on t1.FProductId equals t3.ProductId
                                                                             join t4 in _db.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                                             join t5 in _db.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                                             join t6 in _db.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                                             select new VMProdReferenceSlave
                                                                             {
                                                                                 AccountingHeadId = t4.AccountingHeadId,
                                                                                 ID = t1.ProdReferenceSlaveID,
                                                                                 ProductName = t4.Name + " " + t3.ProductName,
                                                                                 ProdReferenceId = t1.ProdReferenceId,
                                                                                 ProdReferenceSlaveID = t1.ProdReferenceSlaveID,
                                                                                 FProductId = t1.FProductId,
                                                                                 Quantity = t1.Quantity,
                                                                                 UnitName = t6.Name,
                                                                                 CompanyFK = t1.CompanyId,
                                                                                 QuantityLess = t1.QuantityLess,
                                                                                 QuantityOver = t1.QuantityOver,

                                                                                 //PurchasePrice = t3.FormulaQty.Value,
                                                                                 //CostingPrice = t1.CostingPrice
                                                                                 CostingPrice = Math.Round((((from st1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId && x.RProductId > 0)
                                                                                                              select st1.TotalConsumeQuantity * st1.COGS).DefaultIfEmpty(0m).Sum() +
                                                                                                           (from st1 in _db.Prod_ReferenceSlaveConsumption.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId && x.FactoryExpensesHeadGLId > 0)
                                                                                                            select st1.UnitPrice).DefaultIfEmpty(0m).Sum()) /
                                                                                                            ((from prst1 in _db.Prod_ReferenceSlave.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId && x.CompanyId == companyId)
                                                                                                              join prst2 in _db.Products.Where(x => x.IsActive) on prst1.FProductId equals prst2.ProductId
                                                                                                              select ((prst1.Quantity + prst1.QuantityOver) - prst1.QuantityLess) * prst2.FormulaQty.Value
                                                                                                             ).DefaultIfEmpty(0m).Sum())) * t3.FormulaQty.Value, 2),

                                                                             }).OrderByDescending(x => x.ProdReferenceSlaveID).ToListAsync());
            return vmProdReferenceSlave;
        }

        public async Task<int> GCCLProdReferenceSlaveAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;
            Prod_ReferenceSlave prodReferenceSlave = new Prod_ReferenceSlave
            {
                CostingPrice = vmProdReferenceSlave.CostingPrice, // need reset
                FProductId = vmProdReferenceSlave.FProductId,
                ProdReferenceId = vmProdReferenceSlave.ProdReferenceId,
                Quantity = vmProdReferenceSlave.Quantity,
                QuantityOver = vmProdReferenceSlave.QuantityOver,
                QuantityLess = vmProdReferenceSlave.QuantityLess,
                CompanyId = vmProdReferenceSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Prod_ReferenceSlave.Add(prodReferenceSlave);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = prodReferenceSlave.ProdReferenceSlaveID;
            }
            return result;
        }

        public async Task<int> ProductionItemAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;
            ProductionItem productionItem = new ProductionItem
            {
                ProductId = vmProdReferenceSlave.FProductId,
                ProductionId = (int)vmProdReferenceSlave.ProductionId,
                Quantity = vmProdReferenceSlave.Quantity,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                ProductionItemDate = vmProdReferenceSlave.ProductionItemDate,
                IsActive = true
            };
            _db.ProductionItems.Add(productionItem);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = productionItem.ProductionItemID;
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




        public List<VMProdReferenceSlave> ProductOrderQuantityByOrderList(long orderId, int prodReferenceSlaveID)
        {
            if (prodReferenceSlaveID > 0)
            {
                var list = (from t1 in _db.Prod_ReferenceSlave
                            join t2 in _db.Prod_Reference on t1.ProdReferenceId equals t2.ProdReferenceId
                            join t3 in _db.Products on t1.FProductId equals t3.ProductId
                            join t4 in _db.ProductSubCategories on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                            join t5 in _db.OrderDetails on t3.ProductId equals t5.ProductId
                            where t1.IsActive
                                  && t2.IsActive
                                  && t3.IsActive
                                  && t4.IsActive
                                  && t5.IsActive
                                  && t5.OrderMasterId == orderId
                                  && t1.ProdReferenceSlaveID == prodReferenceSlaveID
                            select new VMProdReferenceSlave
                            {
                                ProductName = t4.Name + " " + t3.ProductName,
                                FProductId = t3.ProductId,

                                OrderDetailId = t5.OrderDetailId,
                                OrderQty = t5.Qty,
                                PriviousProcessQuantity = (_db.Prod_ReferenceSlave
                                                             .Where(p => p.IsActive && p.OrderDetailId == t5.OrderDetailId)
                                                             .Select(p => p.Quantity)
                                                             .DefaultIfEmpty(0m)
                                                             .Sum()),
                                ProductionQty = (double)t1.Quantity,
                                ProdReferenceSlaveID = t1.ProdReferenceSlaveID
                            }).ToList();

                return list;
            }
            else
            {
                var list = (from t1 in _db.OrderDetails
                            join t2 in _db.Products on t1.ProductId equals t2.ProductId
                            join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                            where t1.IsActive && t2.IsActive && t3.IsActive && t1.OrderMasterId == orderId
                            select new VMProdReferenceSlave
                            {
                                ProductName = t3.Name + " " + t2.ProductName,
                                FProductId = t2.ProductId,


                                OrderQty = t1.Qty,
                                PriviousProcessQuantity = (_db.Prod_ReferenceSlave
                                                             .Where(p => p.IsActive && p.OrderDetailId == t1.OrderDetailId)
                                                             .Select(p => p.Quantity)
                                                             .DefaultIfEmpty(0m)
                                                             .Sum()),
                                ProductionQty = t1.Qty - (double)(_db.Prod_ReferenceSlave
                                                             .Where(p => p.IsActive && p.OrderDetailId == t1.OrderDetailId)
                                                             .Select(p => p.Quantity)
                                                             .DefaultIfEmpty(0m)
                                                             .Sum()),
                                OrderDetailId = t1.OrderDetailId
                            }).ToList();
                return list;
            }
        }

        public List<VMProdReferenceSlave> ProductOrderQuantityByOrderDetailsIdList(long orderDetailsId, int prodReferenceSlaveID)
        {
            if (prodReferenceSlaveID > 0)
            {
                var list = (from t1 in _db.Prod_ReferenceSlave
                            join t2 in _db.Prod_Reference on t1.ProdReferenceId equals t2.ProdReferenceId
                            join t3 in _db.Products on t1.FProductId equals t3.ProductId
                            join t4 in _db.ProductSubCategories on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                            join t5 in _db.OrderDetails on t3.ProductId equals t5.ProductId
                            where t1.IsActive
                                  && t2.IsActive
                                  && t3.IsActive
                                  && t4.IsActive
                                  && t5.IsActive
                                  && t5.OrderDetailId == orderDetailsId
                                  && t1.ProdReferenceSlaveID == prodReferenceSlaveID
                            select new VMProdReferenceSlave
                            {
                                ProductName = t4.Name + " " + t3.ProductName,
                                FProductId = t3.ProductId,

                                OrderDetailId = t5.OrderDetailId,
                                OrderQty = t5.Qty,
                                PriviousProcessQuantity = (_db.Prod_ReferenceSlave
                                                             .Where(p => p.IsActive && p.OrderDetailId == t5.OrderDetailId)
                                                             .Select(p => p.Quantity)
                                                             .DefaultIfEmpty(0m)
                                                             .Sum()),
                                ProductionQty = (double)t1.Quantity,
                                ProdReferenceSlaveID = t1.ProdReferenceSlaveID
                            }).ToList();

                return list;
            }
            else
            {
                var list = (from t1 in _db.OrderDetails
                            join t2 in _db.Products on t1.ProductId equals t2.ProductId
                            join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                            where t1.IsActive && t2.IsActive && t3.IsActive && t1.OrderDetailId == orderDetailsId
                            select new VMProdReferenceSlave
                            {
                                ProductName = t3.Name + " " + t2.ProductName,
                                FProductId = t2.ProductId,


                                OrderQty = t1.Qty,
                                PriviousProcessQuantity = (_db.Prod_ReferenceSlave
                                                             .Where(p => p.IsActive && p.OrderDetailId == t1.OrderDetailId)
                                                             .Select(p => p.Quantity)
                                                             .DefaultIfEmpty(0m)
                                                             .Sum()),
                                ProductionQty = t1.Qty - (double)(_db.Prod_ReferenceSlave
                                                             .Where(p => p.IsActive && p.OrderDetailId == t1.OrderDetailId)
                                                             .Select(p => p.Quantity)
                                                             .DefaultIfEmpty(0m)
                                                             .Sum()),
                                OrderDetailId = t1.OrderDetailId
                            }).ToList();
                return list;
            }
        }





        public async Task<int> KplProd_ReferenceAdd(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;
            using (var scope = _db.Database.BeginTransaction())
            {
                var poMax = _db.Prod_Reference.Count(x => x.CompanyId == vmProdReferenceSlave.CompanyFK) + 1;
                string poCid = $"KPLPR-{DateTime.Now:yyMMdd}-{poMax:00}";

                Prod_Reference prodReference = new Prod_Reference
                {
                    ReferenceNo = poCid,
                    ReferenceDate = vmProdReferenceSlave.ReferenceDate,
                    CompanyId = vmProdReferenceSlave.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    OrderMasterId = vmProdReferenceSlave.OrderMasterId,
                    IsActive = true,
                    OrderDetailId = vmProdReferenceSlave.OrderDetailId
                };

                _db.Prod_Reference.Add(prodReference);

                if (await _db.SaveChangesAsync() > 0)
                {
                    List<Prod_ReferenceSlave> listDbSaveEntity = new List<Prod_ReferenceSlave>();
                    foreach (var item in vmProdReferenceSlave.DataToList)
                    {
                        var orderDetails = _db.OrderDetails.Find(item.OrderDetailId);
                        if (item.ProductionQty > 0)
                        {
                            listDbSaveEntity.Add(new Prod_ReferenceSlave
                            {
                                ProdReferenceId = prodReference.ProdReferenceId,
                                FProductId = item.FProductId,
                                Quantity = (decimal)item.ProductionQty,
                                CompanyId = vmProdReferenceSlave.CompanyFK,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                QuantityOver = 0,
                                QuantityLess = 0,
                                CostingPrice = Convert.ToDecimal(orderDetails.IsVATInclude == true ? orderDetails.UnitPrice - (orderDetails.UnitPrice / 100 * (double)orderDetails.VATPercent) : orderDetails.UnitPrice),
                                OrderDetailId = item.OrderDetailId
                            });
                        }

                    }
                    _db.Prod_ReferenceSlave.AddRange(listDbSaveEntity);

                    if (await _db.SaveChangesAsync() > 0)
                    {
                        result = prodReference.ProdReferenceId;

                        //var c = _db.Prod_ReferenceSlave.Where(x => x.ProdReferenceId == prodReference.ProdReferenceId && x.IsActive).ToList();
                        //foreach (var item in c)
                        //{
                        //    var v = _db.FinishProductBOMs.Where(x =


                        //}



                        scope.Commit();
                    }
                    else
                    {
                        scope.Rollback();
                    }
                }
                else
                {
                    scope.Rollback();
                }
            }


            return result;
        }

        public async Task<int> KpLProdReferenceSlaveDelete(int id)
        {
            var exist = await _db.Prod_ReferenceSlave.FirstOrDefaultAsync(x => x.ProdReferenceSlaveID == id);
            if (exist != null)
            {
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    return exist.ProdReferenceId;
                }
            }
            return 0;
        }

        public async Task<int> DeleteProdReferenceforKpl(int id, string modifiedBy)
        {
            int result = -1;
            Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(id);
            List<Prod_ReferenceSlave> prodReferenceSlave = await _db.Prod_ReferenceSlave.Where(x => x.ProdReferenceId == id).ToListAsync();

            if (prodReference != null)
            {
                prodReference.IsActive = false;
                prodReference.ModifiedBy = modifiedBy;
                prodReference.ModifiedDate = DateTime.Now;

                // Mark each Prod_ReferenceSlave as inactive
                foreach (var slave in prodReferenceSlave)
                {
                    slave.IsActive = false;
                    slave.ModifiedBy = modifiedBy;
                    slave.ModifiedDate = DateTime.Now;
                }

                // Save changes to the database
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReference.ProdReferenceId;
                }
            }

            return result;
        }

        public async Task<int> AuthSubmitProdReferenceForKpl(int id, string modifiedBy)
        {
            int result = -1;
            Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(id);

            if (prodReference != null)
            {
                prodReference.IsAuthorized = true;
                prodReference.AuthorizedBy = modifiedBy;
                prodReference.AuthorizedDate = DateTime.Now;

                _db.Entry(prodReference).State = EntityState.Modified;
                // Save changes to the database

                OrderMaster ordermaster = await _db.OrderMasters.Where(x => x.OrderMasterId == prodReference.OrderMasterId).FirstOrDefaultAsync();

                ordermaster.OrderStatus = "R";
                ordermaster.ModifiedBy = modifiedBy;
                ordermaster.ModifiedDate = DateTime.Now;

                _db.Entry(ordermaster).State = EntityState.Modified;


                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReference.ProdReferenceId;
                }
            }

            return result;
        }

        public async Task<int> SubmitProdReferenceforISS(int id)
        {
            int result = -1;
            Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(id);


            if (prodReference != null)
            {
                prodReference.IsSubmitted = true;
                prodReference.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                prodReference.ModifiedDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReference.ProdReferenceId;

                }
                if (result > 0)
                {
                    var productionVm = await Task.Run(() => ProdReferenceSlaveGet(prodReference.CompanyId.Value, prodReference.ProdReferenceId));

                    await _accountingService.AccountingPackagingPushISS(productionVm);

                }

                return result;
            }

            return result;
        }



        public async Task<long> SubmitMultiReferenceforISS()
        {

            var firstDayOfMonth = new DateTime(2024, 08, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            List<Prod_Reference> listIssueMasterInfo = _db.Prod_Reference.Where(x => x.IsActive == true &&
             x.IsSubmitted == false &&
            x.CompanyId == 20 &&
           x.ReferenceDate >= firstDayOfMonth
           && x.ReferenceDate <= lastDayOfMonth).ToList();

            foreach (var item in listIssueMasterInfo)
            {
                Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(item.ProdReferenceId);


                if (prodReference != null)
                {
                    prodReference.IsSubmitted = true;
                    prodReference.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    prodReference.ModifiedDate = DateTime.Now;
                    await _db.SaveChangesAsync();
                    var productionVm = await Task.Run(() => ProdReferenceSlaveGet(prodReference.CompanyId.Value, prodReference.ProdReferenceId));

                    await _accountingService.AccountingPackagingPushISS(productionVm);

                }

            }
            return 0;
        }


        public async Task<int> KpLProdReferenceUpdate(int id, DateTime dateData)
        {
            int result = -1;
            Prod_Reference prodReference = await _db.Prod_Reference.FindAsync(id);

            if (prodReference != null)
            {
                prodReference.ReferenceDate = dateData;
                prodReference.ModifiedDate = DateTime.Now;
                prodReference.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = prodReference.ProdReferenceId;
                }
            }

            return result;
        }

        public async Task<object> GetCustomerForOrderData(int prodReferenceId)
        {
            var query = await (from t1 in _db.Prod_Reference.Where(x => x.IsActive && x.ProdReferenceId == prodReferenceId)
                               join t2 in _db.OrderMasters.Where(x => x.IsActive) on t1.OrderMasterId equals t2.OrderMasterId
                               join t3 in _db.Vendors.Where(x => x.IsActive) on t2.CustomerId equals t3.VendorId
                               select new
                               {
                                   VendoerId = t3.VendorId,
                                   VendorName = t3.Name,
                                   OrderMasterId = t1.OrderMasterId
                               }).FirstOrDefaultAsync();

            return query;
        }


        public async Task<int> KpLProdReferenceSlaveUpdate(VMProdReferenceSlave vmProdReferenceSlave)
        {
            int result = -1;

            if (vmProdReferenceSlave.ProdReferenceSlaveID > 0)
            {
                var dbEntity = await _db.Prod_ReferenceSlave.FirstOrDefaultAsync(x => x.ProdReferenceSlaveID == vmProdReferenceSlave.ProdReferenceSlaveID);
                foreach (var item in vmProdReferenceSlave.DataToList)
                {
                    if (item.ProductionQty > 0)
                    {

                        dbEntity.Quantity = (decimal)item.ProductionQty;
                        dbEntity.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        dbEntity.ModifiedDate = DateTime.Now;
                    }

                }

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = dbEntity.ProdReferenceId;
                }

            }


            return result;
        }
    }



}