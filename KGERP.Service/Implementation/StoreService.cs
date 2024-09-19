using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class StoreService : IStoreService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        private readonly AccountingService _accountingService;

        public StoreService(ERPEntities context)
        {
            this.context = context;
            _accountingService = new AccountingService(context);

        }
        public async Task<StoreModel> GetStores(int companyId, DateTime? fromDate, DateTime? toDate, string type)
        {
            StoreModel storeModel = new StoreModel();
            storeModel.CompanyId = companyId;
            storeModel.DataList = await Task.Run(() => (from t1 in context.Stores
                                                        join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                        join t3 in context.PurchaseOrders on t1.PurchaseOrderId equals t3.PurchaseOrderId
                                                        into po
                                                        from t3 in po.DefaultIfEmpty()
                                                        join t4 in context.StockInfoes on t1.StockInfoId equals t4.StockInfoId
                                                        into st
                                                        from t4 in st.DefaultIfEmpty()
                                                        join t5 in context.Demands on t3.DemandId equals t5.DemandId
                                                        into od
                                                        from t5 in od.DefaultIfEmpty()
                                                        where t1.CompanyId == companyId
                                                         && t1.ReceivedDate >= fromDate
                                                         && t1.ReceivedDate <= toDate
                                                        select new StoreModel
                                                        {
                                                            StoreId = t1.StoreId,
                                                            ReceivedDate = t1.ReceivedDate,
                                                            ReceivedCode = t1.ReceivedCode,
                                                            SupplierName = t2.Name,
                                                            LcNo = t1.LcNo,
                                                            StoreName = t4.Name,
                                                            ParchaseOrderNo = t3.PurchaseOrderNo,
                                                            DemandNo = t5.DemandNo,
                                                            Remarks = t1.Remarks,
                                                            CreatedDate = t1.CreatedDate
                                                        }).OrderByDescending(o => o.ReceivedDate).AsEnumerable());

            return storeModel;

        }

        public StoreModel GetStore(long id, string type)
        {
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string receivedCode = string.Empty;
            if (id <= 0)
            {
                IQueryable<Store> stores = context.Stores.Where(x => x.CompanyId == companyId && x.Type.ToLower().Equals(type.ToLower()) && x.IsActive);
                int count = stores.Count();
                if (count == 0)
                {
                    return new StoreModel()
                    {
                        ReceivedCode = GenerateSequenceNumber(type, 0),
                        CompanyId = companyId,
                        Type = type
                    };
                }

                stores = stores.Where(x => x.CompanyId == companyId && x.Type.ToLower().Equals(type.ToLower())).OrderByDescending(x => x.StoreId).Take(1);
                receivedCode = stores.ToList().FirstOrDefault().ReceivedCode;
                long lastReceivedNo = Convert.ToInt64(receivedCode.Substring(3, 6));
                receivedCode = GenerateSequenceNumber(type, lastReceivedNo);
                return new StoreModel()
                {
                    ReceivedCode = receivedCode,
                    CompanyId = companyId,
                    Type = type
                };

            }
            Store store = context.Stores.Include(x => x.StoreDetails).Where(x => x.StoreId == id).FirstOrDefault();
            if (store == null)
            {
                throw new Exception("Store not found");
            }
            StoreModel model = ObjectConverter<Store, StoreModel>.Convert(store);
            return model;
        }
        private string GenerateSequenceNumber(string type, long lastReceivedNo)
        {
            string productType = string.Empty;
            productType = type.ToUpper().Equals("R") ? "RM-" : "FG-";
            lastReceivedNo = lastReceivedNo + 1;
            return productType + lastReceivedNo.ToString().PadLeft(6, '0');
        }
        public bool SaveStore(long id, StoreModel model)
        {
            int noOfRowsAffected = 0;
            Store store = ObjectConverter<StoreModel, Store>.Convert(model);
            if (id > 0)
            {
                store = context.Stores.Where(x => x.StoreId == id).FirstOrDefault();
                if (store == null)
                {
                    throw new Exception("Data not found!");
                }
                store.ModifiedDate = DateTime.Now;
                store.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                store.IsActive = true;
                store.CreatedDate = DateTime.Now;
                store.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            context.Stores.Add(store);

            noOfRowsAffected = context.SaveChanges();
            if (noOfRowsAffected > 0)
            {
                noOfRowsAffected = context.Database.ExecuteSqlCommand("exec sp_InsertIntoProductStoreFromStoreAndStoreDetail {0},{1}", store.StoreId, store.Type);
            }
            return noOfRowsAffected > 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public async Task<SoreProductQty> GetStoreProductQty(int companyId)
        {
            SoreProductQty model = new SoreProductQty();
            model.CompanyId = companyId;
            model.DataList = await Task.Run(() => (from t1 in context.ProductStores
                                                   join t2 in context.Products on t1.ProductId equals t2.ProductId
                                                   join t3 in context.StockInfoes on t1.StockInfoId equals t3.StockInfoId
                                                   into st
                                                   from t3 in st.DefaultIfEmpty()
                                                   join t4 in context.Companies on t1.CompanyId equals t4.CompanyId
                                                   into od
                                                   from t4 in od.DefaultIfEmpty()
                                                   where t1.CompanyId == companyId
                                                    && t1.Status == "F"
                                                   group t1 by new { t3.Name, t2.ProductName, t4.Address, t4.ShortName } into g
                                                   select new SoreProductQty
                                                   {

                                                       StoreName = g.Key.Name,
                                                       ProductName = g.Key.ProductName,
                                                       Quantity = g.Sum(s => s.InQty) - g.Sum(s1 => s1.OutQty)
                                                   }).OrderByDescending(o => o.ProductName).AsEnumerable());
            return model;
        }



        public List<SoreProductQty> GetRMStoreProductQty()
        {
            dynamic result = context.Database.SqlQuery<SoreProductQty>("sp_GetRMStoreProductQuantity").ToList();
            return result;
        }

        public List<SoreProductQty> GetEcomProductStore()
        {
            dynamic result = context.Database.SqlQuery<SoreProductQty>("sp_Ecom_Current_Stock").ToList();
            return result;
        }

        public StoreModel GetOpenningStore(long id)
        {
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string receivedCode = string.Empty;

            Store stores = context.Stores.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.StoreId).Take(1).FirstOrDefault();


            receivedCode = stores == null ? "000000000" : stores.ReceivedCode;

            long lastReceivedNo = Convert.ToInt64(receivedCode.Substring(3, 6)) + 1;

            return new StoreModel()
            {
                ReceivedCode = "OP-" + lastReceivedNo.ToString().PadLeft(6, '0'),
                CompanyId = companyId
            };


        }

        public StoreModel GetRequisitionItemStore(int id, string type)
        {
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string receivedCode = string.Empty;
            if (id <= 0)
            {
                IQueryable<Store> stores = context.Stores.Where(x => x.CompanyId == companyId && x.Type.ToLower().Equals(type.ToLower()) && x.ReceivedCode.StartsWith("FG") && x.IsActive);
                int count = stores.Count();
                if (count == 0)
                {
                    return new StoreModel()
                    {
                        ReceivedCode = GenerateSequenceNumber(type, 0),
                        CompanyId = companyId,
                        Type = type
                    };
                }

                stores = stores.OrderByDescending(x => x.StoreId).Take(1);
                receivedCode = stores.ToList().FirstOrDefault().ReceivedCode;
                long lastReceivedNo = Convert.ToInt64(receivedCode.Substring(3, 6));
                receivedCode = GenerateSequenceNumber(type, lastReceivedNo);
                return new StoreModel()
                {
                    ReceivedCode = receivedCode,
                    CompanyId = companyId,
                    Type = type
                };

            }
            Store store = context.Stores.Include(x => x.StoreDetails).Where(x => x.StoreId == id).FirstOrDefault();
            if (store == null)
            {
                throw new Exception("Store not found");
            }
            StoreModel model = ObjectConverter<Store, StoreModel>.Convert(store);
            return model;
        }
        public bool ProductionBulkUpdate(RequisitionModel model)
        {
            Requisition requisition = context.Requisitions.Find(model.RequisitionId);
            requisition.IsSubmitted = true;
            context.Entry(requisition).State = EntityState.Modified;
            context.SaveChanges();

            var AccData = FeedRequisitionPushGet(model.CompanyId.Value, model.RequisitionId);
            var v = _accountingService.AccountingProductionPushFeed(model.CompanyId.Value, AccData, (int)FeedJournalEnum.ProductionVoucher).Result;

            return true;

        }
        public bool StoreUpdateAfterProduction(StoreModel model, List<RequisitionItemModel> requistionItemModels, out string message)
        {
            message = string.Empty;
            int noOfRowsAffected = 0;
            int? requisitionId = requistionItemModels.First().RequisitionId;
            if (requistionItemModels.Count() != requistionItemModels.Where(x => x.IsIssued).Count())
            {
                message = "Production issue is not completed. Some products are not checked.";
                return false;
            }
            bool requisitionItemFlag = false;
            foreach (var item in requistionItemModels.Where(x => x.IsIssued))
            {
                if (item.ConsumptionBagQty > item.AvailableBagQty || (item.ConsumptionBagQty == 0 || item.AvailableBagQty == 0))
                {
                    message = "Production issue is not completed. Insufficent bags !.";
                    return false;
                }
                if (item.BagUnitPrice == 0)
                {
                    message = "Some bags have no rate defined. Please check before production issue";
                    return false;
                }


                var tpPrice = context.ProductPrices.Where(x => x.ProductId == item.ProductId
                             && x.PriceUpdatedDate <= model.ReceivedDate && x.PriceType == "TP").OrderByDescending(x => x.PriceUpdatedDate).Select(x => x.UnitPrice).FirstOrDefault();



                RequisitionItem requisitionItem = context.RequisitionItems.Find(item.RequisitionItemId);
                if (requisitionItem == null)
                {
                    throw new Exception("Data not found!");
                }
                requisitionItem.RequisitionItemStatus = "I";
                requisitionItem.BagUnitPrice = item.BagUnitPrice;
                requisitionItem.OutputQty = item.OutputQty;
                requisitionItem.OverHead = 0;
                requisitionItem.BagQty = item.ConsumptionBagQty;
                requisitionItem.IssueDate = model.ReceivedDate;
                requisitionItem.IssueBy = System.Web.HttpContext.Current.User.Identity.Name;
                requisitionItem.RMCostRate = requisitionItem.RMCost / item.OutputQty;
                requisitionItem.BagRate = (Convert.ToDecimal(requisitionItem.BagQty) * requisitionItem.BagUnitPrice) / item.OutputQty;
                requisitionItem.ProductionRate = Math.Round(requisitionItem.RMCostRate + requisitionItem.BagRate, 5);
                requisitionItem.TPPrice = tpPrice != null ? tpPrice.Value : 0;

                int rItem = context.SaveChanges();
                if (rItem > 0)
                {
                    requisitionItemFlag = true;
                }
            }
            if (requisitionItemFlag)
            {
                Requisition requisition = context.Requisitions.Find(model.RequisitionId);
                requisition.IsSubmitted = true;
                requisition.RequisitionStatus = "I";
                context.SaveChanges();

                // Costing Price

                foreach (var item in requistionItemModels.Where(x => x.IsIssued))
                {
                    Product product = context.Products.Find(item.ProductId);

                    var result = context.Database.SqlQuery<FeedFinishedProductStock>("Finished_Product_Wise_StockReport {0}, {1}, {2}", DateTime.Now.ToString("dd/MM/yyyy"), model.CompanyId, item.ProductId).FirstOrDefault();
                    product.CostingPrice = result.ClosingRate;
                    context.SaveChanges();
                }



                var AccData = FeedRequisitionPushGet(model.CompanyId.Value, requisitionId.Value);
                var v = _accountingService.AccountingProductionPushFeed(model.CompanyId.Value, AccData, (int)FeedJournalEnum.ProductionVoucher).Result;
            }

            //List<RequisitionItem> requisitionItems = ObjectConverter<RequisitionItemModel, RequisitionItem>.ConvertList(requistionItemModels.ToList()).ToList();

            //Store store = ObjectConverter<StoreModel, Store>.Convert(model);
            //store.Type = "F";
            //store.StockInfoId = model.StockInfoId;
            //store.CompanyId = model.CompanyId;
            //store.VendorId = null;
            //store.ReceivedDate = model.ReceivedDate;
            //store.ReceivedCode = model.ReceivedCode;
            //store.Remarks = "Finish product is inserted into Stock after Production on " + model.ReceivedDate.Value.ToString("dd MMM yyyy");
            //store.RequisitionId = requisitionId;

            //store.IsActive = true;
            //store.CreatedDate = DateTime.Now;
            //store.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            //store.IsSubmited = model.IsSubmited;

            //foreach (var item in requistionItemModels)
            //{


            //    if (context.SaveChanges() > 0)
            //    {

            //        //var productStore = context.Database.ExecuteSqlCommand("exec sp_InsertIntoProductStoreFromStoreAndStoreDetail {0},{1}", store.StoreId, store.Type) > 0;
            //        var changeStatus = ChangeRequisitionStatus(requisitionId.Value);

            //        var AccData = FeedRequisitionPushGet(model.CompanyId.Value, requisitionId.Value);
            //        var v = _accountingService.AccountingProductionPushFeed(model.CompanyId.Value, AccData, (int)SeedJournalEnum.ProductionVoucher).Result;

            //        //return productStore;
            //        //noOfRowsAffected = context.Database.ExecuteSqlCommand("exec sp_Feed_RmAndBagOutAfterProduction {0},{1},{2}", requisitionItem.RequisitionItemId, store.ReceivedCode, store.StockInfoId);
            //        //StoreDetail storeDetail = new StoreDetail();
            //        //storeDetail.ProductId = item.ProductId;
            //        //storeDetail.Qty = Convert.ToDouble(item.OutputQty);
            //        //storeDetail.UnitPrice = requisitionItem.ProductionRate;
            //        //store.StoreDetails.Add(storeDetail);
            //    }

            //}


            //context.Entry(store).State = store.StoreId == 0 ? EntityState.Added : EntityState.Modified;
            //noOfRowsAffected = context.SaveChanges();

            //if (noOfRowsAffected > 0)
            //{

            //    //int noOfItemsIssued = context.Database.SqlQuery<int>("select count(RequisitionItemId) from Erp.RequisitionItem where RequisitionId={0} and RequisitionItemStatus='I'", requisitionId).FirstOrDefault();
            //    //if (noOfItemsIssued == requistionItemModels.Count())
            //    //{
            //    //    noOfRowsAffected = context.Database.ExecuteSqlCommand("update Erp.Requisition set RequisitionStatus='I' where RequisitionId={0}", requisitionId);
            //    //}
            //    //else if (noOfItemsIssued > 0)
            //    //{
            //    //    noOfRowsAffected = context.Database.ExecuteSqlCommand("update Erp.Requisition set RequisitionStatus='P' where RequisitionId={0}", requisitionId);
            //    //}
            //    //return noOfItemsIssued > 0;
            //}

            return noOfRowsAffected > 0;

        }

        private int ChangeRequisitionStatus(int RequisitionId)
        {
            Requisition requisition = context.Requisitions.Find(RequisitionId);
            requisition.IsSubmitted = true;
            requisition.IsActive = true;
            requisition.RequisitionStatus = "I";
            context.Entry(requisition).State = EntityState.Modified;


            return requisition.RequisitionId;
        }

        private void ChangeFinishedItemCostingPrice(int requisitionId, int companyId)
        {
            var requisitionItems = context.RequisitionItems.Where(x => x.RequisitionId == requisitionId && x.IsActive).ToList();
            foreach (var item in requisitionItems)
            {
                var result = context.Database.SqlQuery<FeedFinishedProductStock>("Finished_Product_Wise_StockReport {0}, {1}, {2}", item.IssueDate, companyId, item.ProductId).FirstOrDefault();

                var product = context.Products.Find(result.ProductId);
                product.CostingPrice = result.ClosingRate;
                context.SaveChanges();
            }
        }

        public RequisitionModel FeedRequisitionPushGet(int companyId, long requisitionId)
        {
            RequisitionModel requisitionModel = new RequisitionModel();
            requisitionModel = (from t1 in context.Requisitions
                                where t1.CompanyId == companyId && t1.RequisitionId == requisitionId
                                select new RequisitionModel
                                {
                                    RequisitionId = t1.RequisitionId,
                                    RequisitionNo = t1.RequisitionNo,
                                    RequisitionDate = t1.DeliveredDate,
                                    RequisitionBy = t1.RequisitionBy,
                                    Description = t1.Description,
                                    CreatedBy = t1.CreatedBy,
                                    CreatedDate = t1.CreatedDate,
                                    RequisitionStatus = t1.RequisitionStatus,
                                    DeliveredBy = t1.DeliveredBy,
                                    DeliveredDate = t1.DeliveredDate,
                                    DeliveryNo = t1.DeliveryNo,
                                    IntegratedFrom = "Erp.Requisition"
                                }).FirstOrDefault();


            requisitionModel.RequisitionItemDataList = (from t1 in context.RequisitionItems
                                                        join t5 in context.Products on t1.ProductId equals t5.ProductId
                                                        join t6 in context.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                                        join t7 in context.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                                                        join t8 in context.Units on t5.UnitId equals t8.UnitId
                                                        join t9 in context.ProductFormulas on t5.ProductId equals t9.FProductId

                                                        where t1.RequisitionId == requisitionId

                                                        select new RequisitionItemModel
                                                        {
                                                            RequisitionItemId = t1.RequisitionItemId,
                                                            IssueBy = t1.IssueBy,
                                                            CostingPrice = t5.CostingPrice,
                                                            IssueDate = t1.IssueDate,
                                                            RequisitionItemStatus = t1.RequisitionItemStatus,
                                                            Qty = t1.Qty,
                                                            InputQty = t1.InputQty,
                                                            OutputQty = t1.OutputQty,
                                                            ProductionRate = t1.ProductionRate,
                                                            TPPrice = t1.TPPrice,
                                                            ProductName = t7.Name + " " + t6.Name + " " + t5.ProductName,
                                                            ProductId = t1.ProductId,
                                                            //AccountingExpenseHeadId = t6.AccountingExpenseHeadId,
                                                            AccountingHeadId = t5.AccountingHeadId,
                                                            FormulaDate = t9.FormulaDate,
                                                        }).OrderByDescending(x => x.RequisitionItemId).ToList();



            requisitionModel.BagDataList = (from t1 in context.RequisitionItems
                                            join t5 in context.Products on t1.BagId equals t5.ProductId
                                            join t6 in context.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                            join t7 in context.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                                            join t8 in context.Units on t5.UnitId equals t8.UnitId

                                            where t1.RequisitionId == requisitionId

                                            select new RequisitionItemModel
                                            {
                                                RequisitionItemId = t1.RequisitionItemId,
                                                IssueBy = t1.IssueBy,
                                                IssueDate = t1.IssueDate,
                                                RequisitionItemStatus = t1.RequisitionItemStatus,
                                                BagQty = t1.BagQty,
                                                BagRate = t1.BagRate,
                                                BagUnitPrice = t1.BagUnitPrice,
                                                ProductName = t7.Name + " " + t6.Name + " " + t5.ProductName,
                                                AccountingHeadId = t5.AccountingHeadId,
                                            }).OrderByDescending(x => x.RequisitionItemId).AsEnumerable();

            requisitionModel.RequisitionItemDetailDataList = (from t1 in context.RequisitionItemDetails
                                                              join t5 in context.Products on t1.RProductId equals t5.ProductId
                                                              join t6 in context.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                                              join t7 in context.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                                                              join t8 in context.Units on t5.UnitId equals t8.UnitId

                                                              where t1.RequisitionId == requisitionId

                                                              select new RequisitionItemDetailModel
                                                              {
                                                                  RequistionItemDetailId = t1.RequistionItemDetailId,
                                                                  FProductId = t1.FProductId,
                                                                  RProductName = t7.Name + " " + t6.Name + " " + t5.ProductName,
                                                                  RTotalQty = t1.RTotalQty,
                                                                  RUnitPrice = t1.RUnitPrice,
                                                                  AccountingHeadId = t5.AccountingHeadId,
                                                                  RProcessLoss = t1.RProcessLoss,
                                                                  RQty = t1.RQty,
                                                                  RExtraQty = t1.RExtraQty
                                                              }).OrderByDescending(x => x.RequistionItemDetailId).AsEnumerable();
            return requisitionModel;
        }

        public bool SaveRMStore(long id, StoreModel model)
        {
            int noOfRowsAffected = 0;
            Store store = ObjectConverter<StoreModel, Store>.Convert(model);
            if (id > 0)
            {
                store = context.Stores.Where(x => x.StoreId == id).FirstOrDefault();
                if (store == null)
                {
                    throw new Exception("Data not found!");
                }
                store.ModifiedDate = DateTime.Now;
                store.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                store.IsActive = true;
                store.CreatedDate = DateTime.Now;
                store.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            store.ReceivedDate = model.ReceivedDate;
            context.Stores.Add(store);

            context.Entry(store).State = store.StoreId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                //------Update Purchase Order Status----------
                noOfRowsAffected = context.Database.ExecuteSqlCommand("update Erp.PurchaseOrder set PurchaseOrderStatus='I' where PurchaseOrderId={0}", store.PurchaseOrderId);
            }
            return noOfRowsAffected > 0;
        }


        public List<StoreModel> GetFeedPurchaseList(DateTime? searchDate, string searchText, int companyId, string productType)
        {
            var date = searchDate ?? DateTime.Now.Date;
            IQueryable<Store> queryable = context.Stores.Include(x => x.Vendor).Where(x => x.CompanyId == companyId && x.Type == productType && x.ReceivedCode.StartsWith("FP-")).AsQueryable();
            return ObjectConverter<Store, StoreModel>.ConvertList(queryable.ToList()).ToList();
        }
        public async Task<StoreModel> GetFeedPurchaseList(int companyId, DateTime? fromDate, DateTime? toDate, string type)
        {
            StoreModel storeModel = new StoreModel();
            storeModel.CompanyId = companyId;
            storeModel.Type = type;
            storeModel.DataList = await Task.Run(() => (from t1 in context.Stores
                                                        join t2 in context.Vendors on t1.VendorId equals t2.VendorId

                                                        where t1.CompanyId == companyId
                                                         && t1.ReceivedDate >= fromDate
                                                         && t1.ReceivedDate <= toDate
                                                         && t1.Type == type
                                                         && t1.ReceivedCode.StartsWith("FP-")
                                                        select new StoreModel
                                                        {
                                                            CompanyId = t1.CompanyId,
                                                            StoreId = t1.StoreId,
                                                            ReceivedDate = t1.ReceivedDate,
                                                            ReceivedCode = t1.ReceivedCode,
                                                            SupplierName = t2.Name,
                                                            LcNo = t1.LcNo,
                                                            Type = t1.Type,
                                                            Remarks = t1.Remarks,
                                                            CreatedDate = t1.CreatedDate,
                                                            IsSubmited = t1.IsSubmited
                                                        }).OrderByDescending(o => o.ReceivedDate).AsEnumerable());

            return storeModel;
        }

        public async Task<StoreModel> GetFeedPurchase(int id, string productType)
        {

            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string receivedCode = string.Empty;
            if (id <= 0)
            {
                var stores = context.Stores
                    .Where(x => x.CompanyId == companyId
                        && x.Type == productType
                        && x.ReceivedCode.StartsWith("FP-")
                        && x.IsActive);

                int count = stores.Count();
                if (count == 0)
                {
                    return new StoreModel()
                    {
                        ReceivedCode = GenerateFeedPurchaseInvoiceNo(0),
                        CompanyId = companyId,
                        Type = productType
                    };
                }

                receivedCode = stores
                    .Where(x => x.CompanyId == companyId)
                    .OrderByDescending(x => x.StoreId)
                    .Take(1).FirstOrDefault().ReceivedCode;

                long lastReceivedNo = Convert.ToInt64(receivedCode.Substring(3, 6));
                receivedCode = GenerateFeedPurchaseInvoiceNo(lastReceivedNo);

                return new StoreModel()
                {
                    ReceivedCode = receivedCode,
                    CompanyId = companyId,
                    Type = productType
                };
            }
            var store = context.Stores
                .Include(x => x.StoreDetails)
                .Where(x => x.StoreId == id)
                .FirstOrDefault();

            if (store == null)
            {
                throw new Exception("Data not found");
            }

            StoreModel model = ObjectConverter<Store, StoreModel>.Convert(store);

            return model;
        }

        private string GenerateFeedPurchaseInvoiceNo(long lastReceivedNo)
        {
            lastReceivedNo = lastReceivedNo + 1;
            return "FP-" + lastReceivedNo.ToString().PadLeft(6, '0');
        }
        public async Task<StoreModel> GetFeedPurchase(long storeId, string productType, int companyId)
        {
            StoreModel model = new StoreModel();
            if (storeId > 0)
            {
                model = (from t1 in context.Stores
                         join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                         join t3 in context.StockInfoes on t1.StockInfoId equals t3.StockInfoId
                         join t4 in context.Employees on t1.ReceivedBy equals t4.Id

                         where t1.IsActive && t1.StoreId == storeId
                         select new StoreModel
                         {
                             StoreId = t1.StoreId,
                             ReceivedCode = t1.ReceivedCode,
                             ReceivedDate = t1.ReceivedDate,
                             ReceiverName = t4.Name,
                             ReceivedBy =   t4.Id,
                             SupplierName = t2.Name,
                             Address = t2.Address,
                             StoreName = t3.Name,
                             Remarks = t1.Remarks,
                             Phone = t2.Phone,
                             IsSubmited = t1.IsSubmited,
                              CompanyId = t1.CompanyId,
                               
                         }).FirstOrDefault();


                model.DataList = (from t1 in context.StoreDetails
                                  join t2 in context.Stores on t1.StoreId equals t2.StoreId
                                  join t3 in context.Products on t1.ProductId equals t3.ProductId
                                  join t4 in context.Products on t1.BagId equals t4.ProductId into t4_Join
                                  from t4 in t4_Join.DefaultIfEmpty()
                                  where t1.IsActive && t2.IsActive && t1.StoreId == storeId
                                  select new StoreModel
                                  {
                                      StoreId = t2.StoreId,
                                      ProductId = t1.ProductId.Value,
                                      ProductName = t3.ProductName,
                                      Qty = t1.Qty,
                                      UnitPrice = t1.UnitPrice,
                                      BagQty = t1.BagQty,
                                      BagId = t1.BagId,
                                      BagName = t4 != null? t4.ProductName:"No Define Bag"

                                  }).AsEnumerable();




            }
            else
            {


                string receivedCode = string.Empty;
                if (storeId == 0)
                {
                    var stores = await context.Stores
                        .Where(x => x.CompanyId == companyId
                        && x.Type == productType
                        && x.ReceivedCode.StartsWith("FP-")
                        && x.IsActive).ToListAsync();

                    if (stores.Count() == 0)
                    {
                        return new StoreModel()
                        {
                            ReceivedCode = GenerateFeedPurchaseInvoiceNo(0),
                            CompanyId = companyId,
                            Type = productType
                        };
                    }

                    receivedCode = stores.Where(x => x.CompanyId == companyId)
                        .OrderByDescending(x => x.StoreId)
                        .Take(1).FirstOrDefault()?.ReceivedCode;
                    long lastReceivedNo = Convert.ToInt64(receivedCode.Substring(3, 6));

                    receivedCode = GenerateFeedPurchaseInvoiceNo(lastReceivedNo);

                    return new StoreModel()
                    {
                        ReceivedCode = receivedCode,
                        CompanyId = companyId,
                        Type = productType
                    };
                }
                var store = await context.Stores
                    .Include(x => x.StoreDetails)
                    .Where(x => x.StoreId == storeId)
                    .FirstOrDefaultAsync();

                if (store == null)
                {
                    throw new Exception("Data not found");
                }
                model = ObjectConverter<Store, StoreModel>.Convert(store);

                if (model.ReceivedBy != null && model.ReceivedBy != 0)
                {
                    var y = context.Employees.Find(model.ReceivedBy);
                    model.ReceiverName = $"({y.EmployeeId}) {y.Name}";
                }
            }


            return model;
        }
        public async Task<long> FeedSaveStore(long id, StoreModel model)
        {
            int noOfRowsAffected = 0;
            Store store = ObjectConverter<StoreModel, Store>.Convert(model);
            if (id > 0)
            {
                store = await context.Stores
                    .Where(x => x.StoreId == id)
                    .FirstOrDefaultAsync();

                if (store == null)
                {
                    throw new Exception("Data not found!");
                }
                store.ModifiedDate = DateTime.Now;
                store.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                store.IsActive = true;
                store.ReceivedBy = Common.GetIntUserId();
            }

            else
            {
                store.IsActive = true;
                store.CreatedDate = DateTime.Now;
                store.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                store.ReceivedBy = Common.GetIntUserId();

            }
            context.Stores.Add(store);

            noOfRowsAffected = context.SaveChanges();
            if (noOfRowsAffected > 0)
            {
                noOfRowsAffected = context.Database.ExecuteSqlCommand("exec UpdateBagInfo {0}", store.StoreId);
            }

            return store.StoreId;
        }

        public async Task<long> SubmitFeedPurchaseByProduct(StoreModel model)
        {
            long result = -1;
            var obj = await context.Stores.FindAsync(model.StoreId);
            obj.IsSubmited = true;
            obj.IsActive = true;


            obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            obj.ModifiedDate = DateTime.Now;
            if (await context.SaveChangesAsync() > 0)
            {
                long storeId = obj.StoreId;
                result = storeId;
            }
            if (result > 0 && obj.CompanyId == (int)CompanyName.KrishibidFeedLimited)
            {
                #region Ready To Account Integration
                int companyId = obj.CompanyId ?? 8;
                VMStoreDetail AccData = await FeedPurchaseSlaveGet(companyId, Convert.ToInt32(obj.StoreId));
                await _accountingService.AccountingFeedPurchasePushFeed(companyId, AccData, (int)FeedJournalEnum.PurchaseVoucher);

                #endregion
            }


            return result;
        }
        public async Task<VMStoreDetail> FeedPurchaseSlaveGet(int? companyId, int storeId)
        {
            VMStoreDetail detailModel = new VMStoreDetail();
            detailModel = await Task.Run(() => (from t1 in context.Stores.Where(x => x.CompanyId == companyId)
                                                join t2 in context.StockInfoes on t1.StockInfoId equals t2.StockInfoId into t2_Join
                                                from t2 in t2_Join.DefaultIfEmpty()
                                                join t3 in context.Vendors on t1.VendorId equals t3.VendorId into t3_Join
                                                from t3 in t3_Join.DefaultIfEmpty()
                                                join t4 in context.Employees on t1.ReceivedBy equals t4.Id into t4_Join
                                                from t4 in t4_Join.DefaultIfEmpty()
                                                join t5 in context.Companies on t1.CompanyId equals t5.CompanyId
                                                where t1.IsActive && t1.StoreId == storeId
                                                select new VMStoreDetail
                                                {
                                                    ReceivedCode = t1.ReceivedCode,
                                                    ReceivedDate = t1.ReceivedDate,
                                                    Remarks = t1.Remarks,
                                                    StoreId = t1.StoreId,
                                                    StoreName = t2.Name,
                                                    SupplierName = t3.Name,
                                                    Address = t3.Address,
                                                    Phone = t3.Phone,
                                                    ReceiverName = t4.Name,
                                                    IsActive = t1.IsActive,
                                                    IsSubmited = t1.IsSubmited,
                                                    AccountingHeadId = t3.HeadGLId

                                                }).FirstOrDefault());

            int it = 0;

            detailModel.DataListDetail = await Task.Run(() => (from t1 in context.StoreDetails
                                                               join t2 in context.Stores on t1.StoreId equals t2.StoreId
                                                               join t3 in context.Products on t1.ProductId equals t3.ProductId
                                                               join t4 in context.ProductSubCategories on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                               join t5 in context.ProductCategories on t4.ProductCategoryId equals t5.ProductCategoryId
                                                               join t7 in context.Units on t3.UnitId equals t7.UnitId
                                                               join t8 in context.Products on t1.BagId equals t8.ProductId
                                                               where
                                                               t2.IsActive

                                                               && t2.CompanyId == companyId
                                                               && t1.StoreId == storeId
                                                               select new VMStoreDetail
                                                               {
                                                                   AccountingExpenseHeadId = t3.AccountingExpenseHeadId,
                                                                   AccountingHeadId = t3.AccountingHeadId,
                                                                   StoreDetailId = t1.StoreDetailId,
                                                                   Qty = t1.Qty,
                                                                   ProductId = t1.ProductId,
                                                                   UnitName = t7.Name,
                                                                   ProductName = t4.Name + " " + t3.ProductName,
                                                                   UnitPrice = t1.UnitPrice,
                                                                   IsActive = t1.IsActive,

                                                                   BagId = t8.ProductId,
                                                                   BagName = t8.ProductName,
                                                                   BagAccountingHeadId = t8.AccountingHeadId,
                                                                   BagQty = t1.BagQty,
                                                                   BagUnitPrice = t1.BagUnitPrice

                                                               }).OrderByDescending(x => x.StoreDetailId).AsEnumerable());

            return detailModel;
        }

    }
}
