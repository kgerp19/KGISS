using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class SaleReturnService : ISaleReturnService
    {

        private readonly ERPEntities context;
        private readonly AccountingService _accountingService;

        public SaleReturnService(ERPEntities context, AccountingService accountingService)
        {
            this.context = context;
            _accountingService = accountingService;
        }
        public async Task<SaleReturnModel> SalesReturnSlaveGet(int companyId, int saleReturnId, string productype)
        {
            SaleReturnModel model = new SaleReturnModel();
            model = await Task.Run(() => (from t1 in context.SaleReturns.Where(x => x.CompanyId == companyId)
                                          join t2 in context.Vendors on t1.CustomerId equals t2.VendorId into t2_Join
                                          from t2 in t2_Join.DefaultIfEmpty()
                                          join t3 in context.OrderDelivers on t1.OrderDeliverId equals t3.OrderDeliverId into t3_join
                                          from t3 in t3_join.DefaultIfEmpty()
                                          join t4 in context.Employees on t1.ReceivedBy equals t4.Id into t4_join
                                          from t4 in t4_join.DefaultIfEmpty()
                                          join t5 in context.StockInfoes on t1.StockInfoId equals t5.StockInfoId into t5_join
                                          from t5 in t5_join.DefaultIfEmpty()
                                          join t8 in context.VoucherMaps.Where(X => X.CompanyId == companyId && X.IntegratedFrom == "SaleReturn") on t1.SaleReturnId equals t8.IntegratedId into t8_Join
                                          from t8 in t8_Join.DefaultIfEmpty()
                                          where t1.IsActive && t1.SaleReturnId == saleReturnId
                                          select new SaleReturnModel
                                          {
                                              VoucherId = t8 != null ? t8.VoucherId : 0,
                                              SaleReturnNo = t1.SaleReturnNo,
                                              ReturnDate = t1.ReturnDate,
                                              Reason = t1.Reason,
                                              ProprietorName = t2.Name,
                                              ProprietorAddress = t2.Address,
                                              ProprietorPhone = t2.Phone,
                                              InvoiceNo = t3.InvoiceNo,
                                              WareHouseName = t5.Name,
                                              ReceiverName = t4.Name,
                                              SaleReturnId = t1.SaleReturnId,
                                              IsFinalized = t1.IsFinalized,
                                              CompanyId = t1.CompanyId

                                          }).FirstOrDefault());

            model.ItemList = await Task.Run(() => (from t1 in context.SaleReturnDetails
                                                   join t2 in context.Products on t1.ProductId equals t2.ProductId into t2_product
                                                   from t2 in t2_product.DefaultIfEmpty()
                                                   join t3 in context.Units on t2.UnitId equals t3.UnitId into t3_product
                                                   from t3 in t3_product.DefaultIfEmpty()
                                                   where t1.SaleReturnId == saleReturnId
                                                   select new SaleReturnDetailModel
                                                   {
                                                       ProductName = t2.ProductName,
                                                       ProductCode = t2.ProductCode,
                                                       Unit = t3.Name,
                                                       BaseCommission = t1.BaseCommission,
                                                       CashCommission = t1.CashCommission,
                                                       CarryingCommission = t1.CarryingCommission,
                                                       SpecialDiscount = t1.SpecialDiscount,
                                                       AdditionPrice = t1.AdditionPrice,
                                                       Qty = t1.Qty,
                                                       Rate = t1.Rate
                                                   }
                                                            ).AsEnumerable());

            return model;
        }



        public async Task<SaleReturnModel> GetSaleReturns(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            SaleReturnModel model = new SaleReturnModel();
            model.DataList = await Task.Run(() => (from t1 in context.SaleReturns
                                                   join t2 in context.Vendors on t1.CustomerId equals t2.VendorId
                                                   join t3 in context.Employees on t1.ReceivedBy equals t3.Id
                                                   join t4 in context.OrderDelivers on t1.OrderDeliverId equals t4.OrderDeliverId
                                                   where t1.CompanyId == companyId
                                                   && t1.ReturnDate >= fromDate
                                                   && t1.ReturnDate <= toDate
                                                   select new SaleReturnModel
                                                   {
                                                       CompanyId = t1.CompanyId,
                                                       SaleReturnId = t1.SaleReturnId,
                                                       ReturnDate = t1.ReturnDate,
                                                       CustomerName = t2.Name,
                                                       SaleReturnNo = t1.SaleReturnNo,
                                                       IsFinalized = t1.IsFinalized,
                                                       IsActive = t1.IsActive,
                                                       Reason = t1.Reason,
                                                       ReceiverName = t3.Name,
                                                       ReceivedBy= t3.Id,
                                                       InvoiceNo = t4.InvoiceNo,
                                                       OrderDeliverId = t1.OrderDeliverId,
                                                       ProductType = t1.ProductType
                                                   }).OrderByDescending(x => x.ReturnDate)
                                         .AsEnumerable());
            return model;
        }
        public SaleReturnModel GetSaleReturn(int id, string productType)
        {
            string prefix = productType.Equals("F") ? "FG" : "RM";
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string saleReturnNo = string.Empty;

            if (id <= 0)
            {
                IQueryable<SaleReturn> saleReturns = context.SaleReturns
                    .Where(x => x.CompanyId == companyId && x.ProductType == productType && x.IsActive);
                int count = saleReturns.Count();
                if (count == 0)
                {
                    return new SaleReturnModel()
                    {
                        SaleReturnNo = GenerateSequenceNumber(0, prefix),
                        CompanyId = companyId,
                        ReturnDate = DateTime.Now,
                        ProductType = productType
                    };
                }

                saleReturns = saleReturns
                    .Where(x => x.CompanyId == companyId && x.ProductType == productType)
                    .OrderByDescending(x => x.SaleReturnId)
                    .Take(1);
                saleReturnNo = saleReturns.ToList().FirstOrDefault().SaleReturnNo;
                string numberPart = saleReturnNo.Substring(5, 6);
                int lastNumberPart = Convert.ToInt32(numberPart);
                saleReturnNo = GenerateSequenceNumber(lastNumberPart, prefix);
                return new SaleReturnModel()
                {
                    SaleReturnNo = saleReturnNo,
                    CompanyId = companyId,
                    ReturnDate = DateTime.Now,
                    ProductType = productType
                };

            }

            SaleReturn saleReturn = context.SaleReturns
                .Include(x => x.SaleReturnDetails)
                .Where(x => x.SaleReturnId == id)
                .FirstOrDefault();

            if (saleReturn == null)
            {
                throw new Exception("Sale Return data not found");
            }
            SaleReturnModel model = ObjectConverter<SaleReturn, SaleReturnModel>.Convert(saleReturn);
            return model;
        }

        private string GenerateSequenceNumber(int lastReceivedNo, string prefix)
        {
            int num = ++lastReceivedNo;
            return "SR-" + prefix + num.ToString().PadLeft(6, '0');
        }



        public List<SaleReturnDetailModel> GetDeliveredItems(long orderDeliverId, int companyId)
        {
            return context.Database.SqlQuery<SaleReturnDetailModel>("exec spGetSaleReturnItems {0},{1}", orderDeliverId, companyId).ToList();

        }

        public long SaveSaleReturn(SaleReturnModel model, out string message)
        {
            var result = -1;
            message = "Sales Return completed successfully!";
            SaleReturn saleReturn = ObjectConverter<SaleReturnModel, SaleReturn>.Convert(model);
            if (model.SaleReturnId > 0)
            {
                saleReturn = context.SaleReturns.Where(x => x.SaleReturnId == model.SaleReturnId).FirstOrDefault();
                if (saleReturn == null)
                {
                    throw new Exception("Data not found!");
                }
                saleReturn.ModifiedDate = DateTime.Now;
                saleReturn.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                saleReturn.ReceivedBy = Common.GetIntUserId();
            }

            else
            {
                saleReturn.IsActive = true;
                saleReturn.CreatedDate = DateTime.Now;
                saleReturn.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                saleReturn.ReceivedBy = Common.GetIntUserId();
            }
            List<SaleReturnDetail> returnItems = new List<SaleReturnDetail>();
            foreach (var item in saleReturn.SaleReturnDetails)
            {
                if (item.Qty > 0)
                {
                    item.IsActive = true;
                    returnItems.Add(item);
                }
            }
            if (returnItems.Count() == 0)
            {
                message = "Returned Quantity is not supplied !";
                return result;
            }
            saleReturn.SaleReturnDetails = null;
            saleReturn.SaleReturnDetails = returnItems;
            context.SaleReturns.Add(saleReturn);

            context.Entry(saleReturn).State = saleReturn.SaleReturnId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                //result = context.Database.ExecuteSqlCommand("exec spInsertSalesReturnToProductStore {0}", saleReturn.SaleReturnId);
                return saleReturn.SaleReturnId;
            }
            return result;
        }

        public List<SaleReturnModel> GetOldSaleReturns(DateTime? searchDate, string searchText, int companyId, string productType)
        {
            var date = searchDate ?? DateTime.Now.Date;
            IQueryable<SaleReturn> saleReturns = context.SaleReturns.Where(x => x.CompanyId == companyId && x.ProductType == productType && x.OrderDeliverId == null && x.ReturnDate <= date && (x.OrderDeliver.InvoiceNo.Contains(searchText) || x.SaleReturnNo.Contains(searchText))).OrderByDescending(x => x.SaleReturnId).Include(x => x.Employee).Include(x => x.OrderDeliver).AsQueryable();
            return ObjectConverter<SaleReturn, SaleReturnModel>.ConvertList(saleReturns.ToList()).ToList();
        }


        public async Task<long> SubmitSaleReturnByProduct(SaleReturnModel viewModel)
        {
            long result = -1;
            SaleReturn model = await context.SaleReturns.FindAsync(viewModel.SaleReturnId);
            model.IsFinalized = true;

            model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            if (await context.SaveChangesAsync() > 0)
            {
                result = model.SaleReturnId;
                ChangeFinishedItemCostingPrice(model.SaleReturnId, model.CompanyId, model.ReturnDate);
            }
            
            if (result > 0 && viewModel.CompanyId == (int)CompanyName.KrishibidFeedLimited)
            {
                #region Ready To Account Integration
                VMSaleReturnDetail AccData = await SalesReturnSlaveGet(viewModel.CompanyId, Convert.ToInt32(viewModel.SaleReturnId));
                await _accountingService.AccountingSalesReturnPushFeed(viewModel.CompanyId, AccData, (int)FeedJournalEnum.SalesReturnVoucher);

                #endregion
            }


            return result;
        }
        private void ChangeFinishedItemCostingPrice(long SaleReturnId, int companyId,DateTime date)
        {
            var requisitionItems = context.SaleReturnDetails.Where(x => x.SaleReturnId == SaleReturnId && x.IsActive).ToList();
            foreach (var item in requisitionItems)
            {
                var result = context.Database.SqlQuery<FeedFinishedProductStock>("Finished_Product_Wise_StockReport {0}, {1}, {2}", date, companyId, item.ProductId).FirstOrDefault();

                var product = context.Products.Find(result.ProductId);
                product.CostingPrice = result.ClosingRate;
                context.SaveChanges();

            }
        }
        public async Task<VMSaleReturnDetail> SalesReturnSlaveGet(int companyId, int saleReturnId)
        {
            VMSaleReturnDetail vmSaleReturnDetail = new VMSaleReturnDetail();
            vmSaleReturnDetail = await Task.Run(() => (from t1 in context.SaleReturns.Where(x => x.CompanyId == companyId)
                                                       join t2 in context.OrderDelivers on t1.OrderDeliverId equals t2.OrderDeliverId into t2_Join
                                                       from t2 in t2_Join.DefaultIfEmpty()
                                                       join t4 in context.OrderMasters on t2.OrderMasterId equals t4.OrderMasterId into t4_Join
                                                       from t4 in t4_Join.DefaultIfEmpty()
                                                       join t3 in context.Vendors on t1.CustomerId equals t3.VendorId
                                                       join t5 in context.Companies on t1.CompanyId equals t5.CompanyId
                                                       where t1.IsActive && t1.SaleReturnId == saleReturnId
                                                       select new VMSaleReturnDetail
                                                       {
                                                           IntegratedFrom = "SaleReturn",
                                                           SaleReturnNo = t1.SaleReturnNo,
                                                           ReturnDate = t1.ReturnDate,
                                                           Reason = t1.Reason,
                                                           ChallanNo = t2.ChallanNo,
                                                           OrderNo = t4.OrderNo,
                                                           CustomerName = t3.Name,
                                                           SaleReturnId = t1.SaleReturnId,
                                                           CompanyFK = t1.CompanyId,
                                                           CompanyName = t5.Name,
                                                           CompanyAddress = t5.Address,
                                                           CompanyEmail = t5.Email,
                                                           CompanyPhone = t5.Phone,
                                                           AccountingHeadId = t3.HeadGLId,
                                                           IsFinalized = t1.IsFinalized,
                                                           ComLogo = t5.CompanyLogo
                                                       }).FirstOrDefault());



            vmSaleReturnDetail.DataListDetail = await Task.Run(() => (from t1 in context.SaleReturnDetails
                                                                      join t2 in context.SaleReturns on t1.SaleReturnId equals t2.SaleReturnId
                                                                      join t3 in context.OrderDeliverDetails on t1.OrderDeliverDetailsId equals t3.OrderDeliverDetailId into t3_Join
                                                                      from t3 in t3_Join.DefaultIfEmpty()
                                                                      join t5 in context.Products on t1.ProductId equals t5.ProductId
                                                                      join t6 in context.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                                                      join t7 in context.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                                                                      join t8 in context.Units on t5.UnitId equals t8.UnitId
                                                                      join t9 in context.OrderDetails on t3.OrderDetailId equals t9.OrderDetailId into t9_Join
                                                                      from t9 in t9_Join.DefaultIfEmpty()

                                                                      where t2.IsActive && t1.IsActive && t5.IsActive && t6.IsActive && t7.IsActive
                                                                      && t8.IsActive && t2.CompanyId == companyId
                                                                      && t1.SaleReturnId == saleReturnId
                                                                      select new VMSaleReturnDetail
                                                                      {
                                                                          AccountingIncomeHeadId = t5.AccountingIncomeHeadId,
                                                                          AccountingHeadId = t5.AccountingHeadId,
                                                                          SaleReturnDetailId = t1.SaleReturnDetailId,
                                                                          Qty = t1.Qty,
                                                                          ProductId = t1.ProductId,
                                                                          OrderQty = t9 != null ? t9.Qty : 0,
                                                                          UnitName = t8.Name,
                                                                          DeliveredQty = t3 != null ? t3.DeliveredQty : 0,
                                                                          ProductName = t6.Name + " " + t5.ProductName,
                                                                          Rate = t1.Rate,
                                                                          AdditionPrice = t1.AdditionPrice,
                                                                          BaseCommission = t1.BaseCommission,
                                                                          CashCommission = t1.CashCommission,
                                                                          CarryingCommission = t1.CarryingCommission,
                                                                          SpecialDiscount = t1.SpecialDiscount,
                                                                          COGSRate = t1.COGSRate,
                                                                          MRPPrice = t1.Qty * t1.Rate,
                                                                          CostingPrice = t1.Qty * t1.COGSRate
                                                                      }).OrderByDescending(x => x.SaleReturnDetailId).AsEnumerable());

            return vmSaleReturnDetail;
        }


        //public List<DeliverItemCustomModel> GetDeliverItems(long orderMasterId, int stockInfoId)
        //{
        //    IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Include(x => x.Product.Unit).Where(x => x.OrderMasterId == orderMasterId);

        //    List<StoreDetail> storeDetails = context.Database.SqlQuery<StoreDetail>("exec spGetDeliverItems {0}", stockInfoId).ToList();

        //    List<DeliverItemCustomModel> deliverItemCustomModels = orderDetails.Select(x =>
        //    new DeliverItemCustomModel
        //    {

        //        ProductId = x.ProductId,
        //        ProductCode = x.Product.ProductCode,
        //        ProductName = x.Product.ProductName,
        //        OrderUnit = x.Product.Unit.Name,
        //        OrderQty = x.Qty,
        //        OrderUnitPrice = x.UnitPrice,
        //        StoreAvailableQty = 0,
        //        ReadyToDeliver = 0,
        //        OrderRemainingQty = x.Qty - 0,
        //        DeliveredQty = context.OrderDeliverDetails.Where(p => p.ProductId == x.ProductId && p.OrderDeliver.OrderMasterId == orderMasterId).Sum(s => s.DeliveredQty)
        //    }).ToList();

        //    foreach (var item in deliverItemCustomModels)
        //    {
        //        item.DeliveredQty = item.DeliveredQty ?? 0;
        //        item.DueQty = item.OrderQty - item.DeliveredQty;
        //        item.OrderRemainingQty = item.OrderQty - item.DeliveredQty;
        //        item.EBaseCommission = context.Database.SqlQuery<decimal>("exec spGetEffectiveBaseCommission {0},{1}", item.ProductId, orderMasterId).FirstOrDefault();
        //        item.ECarryingCommission = context.Database.SqlQuery<decimal>("exec spGetEffectiveCarryingCommission {0},{1}", stockInfoId, orderMasterId).FirstOrDefault();
        //        item.ECashCommission = context.Database.SqlQuery<decimal>("exec spGetEffectiveCashCommission {0},{1}", item.ProductId, orderMasterId).FirstOrDefault();
        //        item.StoreAvailableQty = context.Database.SqlQuery<long>("exec spGetStoreAvailableQty {0},{1}", item.ProductId, stockInfoId).FirstOrDefault();
        //    }
        //    return deliverItemCustomModels;
        //}



        //public OrderDeliverModel GetOrderDeliver(long id)
        //{
        //    OrderDeliver orderDeliver = context.OrderDelivers.Where(x => x.OrderDeliverId == id).FirstOrDefault();
        //    if (orderDeliver == null)
        //    {
        //        return new OrderDeliverModel();
        //    }
        //    OrderDeliverModel model = ObjectConverter<OrderDeliver, OrderDeliverModel>.Convert(orderDeliver);
        //    return model;
        //}

        //public List<OrderDeliverModel> GetOrderDelivers(string searchText)
        //{
        //    IQueryable<OrderDeliver> orderDelivers = context.OrderDelivers.OrderByDescending(x => x.OrderDeliverId).AsQueryable();
        //    return ObjectConverter<OrderDeliver, OrderDeliverModel>.ConvertList(orderDelivers.ToList()).ToList();
        //}

        //public OrderDeliverModel GetOrderDeliverWithInclude(long orderDeliverId)
        //{
        //    OrderDeliver orderDeliver = context.OrderDelivers.Include("OrderMaster").Include("StockInfo").Where(x => x.OrderDeliverId == orderDeliverId).FirstOrDefault();
        //    if (orderDeliver == null)
        //    {
        //        return new OrderDeliverModel();
        //    }
        //    OrderDeliverModel model = ObjectConverter<OrderDeliver, OrderDeliverModel>.Convert(orderDeliver);
        //    return model;
        //}

        //public OrderDeliverModel SaveOrderDeliver(long id, OrderDeliverModel model, out string message)
        //{
        //    message = string.Empty;


        //    OrderDeliver orderDeliver = ObjectConverter<OrderDeliverModel, OrderDeliver>.Convert(model);

        //    if (id > 0)
        //    {
        //        orderDeliver = context.OrderDelivers.Where(x => x.OrderDeliverId == id).FirstOrDefault();
        //        if (orderDeliver == null)
        //        {
        //            throw new Exception("Data not found!");
        //        }
        //    }

        //    else
        //    {
        //        orderDeliver.CreatedDate = DateTime.Now;
        //        orderDeliver.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //    }

        //    orderDeliver.OrderMasterId = model.OrderMasterId;
        //    orderDeliver.StockInfoId = model.StockInfoId;
        //    orderDeliver.ChallanNo = model.ChallanNo;
        //    orderDeliver.VehicleNo = model.VehicleNo;
        //    orderDeliver.InvoiceNo = model.InvoiceNo;
        //    orderDeliver.TotalAmount = model.TotalAmount;
        //    orderDeliver.Discount = model.Discount;
        //    orderDeliver.DiscountRate = model.DiscountRate;
        //    orderDeliver.CompanyId = model.CompanyId;
        //    //orderDeliver.DeliveryDate = model.DeliveryDate;
        //    orderDeliver.DeliveryDate = DateTime.Now;


        //    context.OrderDelivers.Add(orderDeliver);

        //    if (context.SaveChanges() > 0)
        //    {
        //        //Change Order Status
        //        int result = context.Database.ExecuteSqlCommand("exec UpdateOrderMasterStatus {0}", orderDeliver.OrderMasterId);

        //        //Update Customer Payment Table
        //        int noOfRowAffected = context.Database.ExecuteSqlCommand("exec spUpdateVendorDuePayment {0}", orderDeliver.OrderDeliverId);
        //    }

        //    return ObjectConverter<OrderDeliver, OrderDeliverModel>.Convert(orderDeliver);
        //}


        //public string GetLastChallanNoByCompany(int companyId)
        //{
        //    OrderDeliver orderDeliver = context.OrderDelivers.Include(x => x.StockInfo).Where(x => x.StockInfo.CompanyId == companyId).OrderByDescending(x => x.OrderDeliverId).FirstOrDefault();

        //    if (orderDeliver == null)
        //    {
        //        return string.Empty;
        //    }
        //    return orderDeliver.ChallanNo;
        //}

        //public string GetLastInvoceNoByCompany(int companyId)
        //{

        //    OrderDeliver orderDeliver = context.OrderDelivers.Include(x => x.StockInfo).Where(x => x.StockInfo.CompanyId == companyId).OrderByDescending(x => x.OrderDeliverId).FirstOrDefault();

        //    if (orderDeliver == null)
        //    {
        //        return string.Empty;
        //    }
        //    return orderDeliver.InvoiceNo;
        //}

        //public string GetGeneratedInvoiceNoByStockInfo(int stockInfoId)
        //{

        //    StockInfo stockInfo = context.StockInfoes.Where(x => x.StockInfoId == stockInfoId).First();
        //    OrderDeliver orderDeliver = context.OrderDelivers.Where(x => x.StockInfoId == stockInfoId).OrderByDescending(x => x.OrderDeliverId).FirstOrDefault();

        //    if (orderDeliver == null)
        //    {
        //        return stockInfo.Code + "000001";
        //    }
        //    if (string.IsNullOrEmpty(orderDeliver.InvoiceNo))
        //    {
        //        return stockInfo.Code + "000001";
        //    }
        //    string lastInvoiceNo = orderDeliver.InvoiceNo.Substring(3);


        //    int num = Convert.ToInt32(lastInvoiceNo);
        //    num = ++num;
        //    return stockInfo.Code + num.ToString().PadLeft(5, '0');
        //}

        //public List<OrderDeliveryPreview> SaveOrderDeliverPreview(int id, List<OrderDeliveryPreview> previews)


        //{
        //    context.Database.ExecuteSqlCommand(@"truncate table Erp.OrderDeliveryPreview");

        //    string storeName = context.Database.SqlQuery<string>(@"select Name from Erp.StockInfo where StockInfoId={0}", previews.First().StockInfoId).FirstOrDefault();

        //    foreach (var preview in previews)
        //    {
        //        preview.StoreName = storeName;
        //        context.OrderDeliveryPreviews.Add(preview);
        //        try
        //        {
        //            context.SaveChanges();
        //        }

        //        catch (DbEntityValidationException exc)
        //        {
        //            // just to ease debugging
        //            foreach (var error in exc.EntityValidationErrors)
        //            {
        //                foreach (var errorMsg in error.ValidationErrors)
        //                {
        //                    // logging service based on NLog
        //                    string message = errorMsg.ErrorMessage;
        //                }
        //            }

        //            throw;
        //        }
        //        catch (DbUpdateException e)
        //        {
        //            var sb = new StringBuilder();
        //            sb.AppendLine($"DbUpdateException error details - {e?.InnerException?.InnerException?.Message}");

        //            foreach (var eve in e.Entries)
        //            {
        //                sb.AppendLine($"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");
        //            }



        //            string message = e.ToString();
        //            message = sb.ToString();

        //            throw;
        //        }


        //    }

        //    return previews;
        //}


    }
}
