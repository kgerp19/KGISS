using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class OrderMasterService : IOrderMasterService
    {
        private readonly ERPEntities context;
        public OrderMasterService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<OrderMasterModel> GetOrderMasters(int companyId, DateTime? fromDate, DateTime? toDate, string productType)
        {
            OrderMasterModel orderMasterModel = new OrderMasterModel();
            orderMasterModel.CompanyId = companyId;
            orderMasterModel.DataList = await Task.Run(() => (from t1 in context.OrderMasters
                                                              join t2 in context.Vendors on t1.CustomerId equals t2.VendorId
                                                              join t3 in context.OrderDelivers on t1.OrderMasterId equals t3.OrderMasterId
                                                              into od
                                                              from t3 in od.DefaultIfEmpty()
                                                              where t1.CompanyId == companyId
                                                             && t1.ProductType == productType
                                                             && t1.OrderDate >= fromDate
                                                             && t1.IsActive == true
                                                             && t1.OrderDate <= toDate
                                                              select new OrderMasterModel
                                                              {
                                                                  OrderDate = t1.OrderDate,
                                                                  OrderMasterId = t1.OrderMasterId,
                                                                  ProductType = t1.ProductType,
                                                                  OrderNo = t1.OrderNo,
                                                                  OrderDeliverId = t3 != null ? t3.OrderDeliverId : 0,
                                                                  CustomerId = t1.CustomerId.HasValue ? 0 : t1.CustomerId.Value,
                                                                  Customer = t2.Name,
                                                                  Remarks = t1.Remarks,
                                                                  TotalAmount = t1.TotalAmount.HasValue ? 0 : t1.TotalAmount.Value,
                                                                  GrandTotal = t1.GrandTotal.HasValue ? 0 : t1.GrandTotal.Value,
                                                                  Status = t1.Status
                                                              }).OrderByDescending(o => o.OrderDate).AsEnumerable());

            return orderMasterModel;

        }
        public async Task<OrderMasterModel> GetOrderMastersFeed(int companyId, DateTime? fromDate, DateTime? toDate, string productType, int Uid)
        {
            OrderMasterModel orderMasterModel = new OrderMasterModel();
            orderMasterModel.CompanyId = companyId;
            orderMasterModel.DataList = await Task.Run(() => (from t1 in context.OrderMasters
                                                              join t2 in context.Vendors on t1.CustomerId equals t2.VendorId
                                                              where t1.CompanyId == companyId
                                                                && t1.ProductType == productType
                                                                && t1.OrderDate >= fromDate
                                                                && t1.IsActive == true
                                                                && t1.OrderDate <= toDate
                                                                && (Uid > 0 ? t1.SalePersonId == Uid: t1.SalePersonId > 0)
                                                              select new OrderMasterModel
                                                              {
                                                                  OrderDate = t1.OrderDate,
                                                                  OrderMasterId = t1.OrderMasterId,
                                                                  ProductType = t1.ProductType,
                                                                  OrderNo = t1.OrderNo,
                                                                  OrderStatus = t1.OrderStatus,
                                                                  CustomerId = t1.CustomerId.HasValue ? 0 : t1.CustomerId.Value,
                                                                  Customer = t2.Name,
                                                                  Remarks = t1.Remarks,
                                                                  Status = t1.Status,
                                                                  CompanyId = t1.CompanyId.Value
                                                              }).OrderByDescending(o => o.OrderMasterId).AsEnumerable());

            return orderMasterModel;

        }
        public async Task<OrderMasterModel> GetOrderDelivers(int companyId, DateTime? fromDate, DateTime? toDate, string productType)
        {
            OrderMasterModel orderMasterModel = new OrderMasterModel();
            orderMasterModel.DataList = await Task.Run(() => (from t1 in context.OrderMasters
                                                              join t3 in context.Vendors on t1.CustomerId equals t3.VendorId
                                                              join t2 in context.OrderDelivers on t1.OrderMasterId equals t2.OrderMasterId into t2_jon
                                                              from t2 in t2_jon.DefaultIfEmpty()
                                                              where t1.CompanyId == companyId
                                                              && t1.ProductType == productType
                                                              && t1.OrderDate >= fromDate
                                                              && t1.IsActive == true
                                                              && t1.OrderDate <= toDate
                                                              select new OrderMasterModel
                                                              {
                                                                  CompanyId = t1.CompanyId.Value,
                                                                  OrderMasterId = t1.OrderMasterId,
                                                                  ProductType = t1.ProductType,
                                                                  CustomerId = t1.CustomerId.Value,
                                                                  Customer = t3.Name,
                                                                  OrderNo = t1.OrderNo,
                                                                  OrderDate = t1.OrderDate,
                                                                  DeliveryDate = t2 != null ? t2.DeliveryDate : null,
                                                                  ChallanNo = t2 != null ? t2.ChallanNo : null,
                                                                  InvoiceNo = t2 != null ? t2.InvoiceNo : null,
                                                                  OrderStatus = t1.OrderStatus,
                                                                  OrderDeliverId = t2 != null ? t2.OrderDeliverId : 0,
                                                                  IsSubmitted = t2 != null ? t2.IsSubmitted : false,
                                                                  Status = t1.Status
                                                              }).OrderByDescending(x => x.OrderDate).AsEnumerable());

            return orderMasterModel;

        }
        public List<SelectModel> GetOrderMasterSelectModels()
        {
            return context.OrderMasters.ToList().Where(x => x.IsActive).Select(x => new SelectModel()
            {
                Text = x.OrderNo,
                Value = x.OrderMasterId
            }).ToList();
        }

        public bool SaveOrder(long orderMasterId, OrderMasterModel model, out string message)
        {
            message = string.Empty;
            OrderMaster orderMaster = ObjectConverter<OrderMasterModel, OrderMaster>.Convert(model);

            bool isOrderNoExist = context.OrderMasters.Any(x => x.CompanyId == model.CompanyId && x.OrderNo == model.OrderNo && x.CustomerId == model.CustomerId);
            if (isOrderNoExist)
            {
                message = "Data already exists !";
                return !isOrderNoExist;
            }

            bool isMasterDetailExist = context.OrderMasters.Where(x => x.CompanyId == model.CompanyId && x.CustomerId == model.CustomerId && x.OrderDate == model.OrderDate && x.ExpectedDeliveryDate == model.ExpectedDeliveryDate && x.Remarks == model.Remarks && x.OrderMasterId != model.OrderMasterId).Count() > 0;

            if (orderMasterId > 0)
            {
                orderMaster = context.OrderMasters.FirstOrDefault(x => x.OrderMasterId == orderMasterId);
                if (orderMaster == null)
                {
                    throw new Exception("Order not found!");
                }
                orderMaster.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                orderMaster.ModifiedDate = DateTime.Now;
            }
            else
            {
                foreach (var orderDetail in orderMaster.OrderDetails)
                {
                    orderDetail.CustomerId = model.CustomerId;
                    orderDetail.OrderDate = model.OrderDate;
                    orderDetail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    orderDetail.CreateDate = DateTime.Now;
                    orderDetail.IsActive = true;
                }
                orderMaster.OrderDate = model.OrderDate.Value;
                orderMaster.OrderMonthYear = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
                orderMaster.OrderStatus = "N";
                orderMaster.CreateDate = DateTime.Now;
                orderMaster.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            orderMaster.CompanyId = model.CompanyId;
            orderMaster.CustomerId = model.CustomerId;
            orderMaster.ProductType = model.ProductType;
            orderMaster.OrderDate = model.OrderDate.Value;
            orderMaster.ExpectedDeliveryDate = model.ExpectedDeliveryDate;
            orderMaster.OrderNo = model.OrderNo;
            orderMaster.SalePersonId = model.SalePersonId;
            orderMaster.StockInfoId = model.StockInfoId;
            orderMaster.Remarks = model.Remarks;
            orderMaster.TotalAmount = model.OrderDetails.Sum(x => Convert.ToDecimal(x.Qty) * Convert.ToDecimal(x.UnitPrice));
            orderMaster.DiscountRate = model.DiscountRate;
            orderMaster.DiscountAmount = model.DiscountAmount;
            orderMaster.GrandTotal = model.GrandTotal;
            orderMaster.IsCash = model.IsCash;
            orderMaster.IsActive = true;
            orderMaster.Vendor = null;

            context.Entry(orderMaster).State = orderMaster.OrderMasterId == 0 ? EntityState.Added : EntityState.Modified;
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            //For KGeCOM.com
            if (model.CompanyId == 28)
            {
                foreach (var item in model.OrderDetails)
                {
                    ProductStore data = new ProductStore();
                    data.ReceiveCode = model.OrderNo;
                    data.ReceiveDate = model.OrderDate.Value;
                    data.StockInfoId = 12;
                    data.ProductId = item.ProductId;
                    data.InQty = 0;
                    data.OutQty = Convert.ToDecimal(item.Qty);
                    data.UnitPrice = 0;
                    //data.ConvertedQty = 0;
                    data.Status = "F";
                    data.CompanyId = model.CompanyId;
                    data.TransactionDate = DateTime.Now;
                    context.ProductStores.Add(data);
                    context.SaveChanges();

                }

            }
            return true;
        }
        public long FeedSaveSalesOrder(long orderMasterId, OrderMasterModel model)
        {

            OrderMaster orderMaster = ObjectConverter<OrderMasterModel, OrderMaster>.Convert(model);

            bool isOrderNoExist = context.OrderMasters.Any(x => x.CompanyId == model.CompanyId && x.OrderNo == model.OrderNo && x.CustomerId == model.CustomerId);
            if (isOrderNoExist)
            {

                return model.OrderMasterId;
            }

            bool isMasterDetailExist = context.OrderMasters.Where(x => x.CompanyId == model.CompanyId && x.CustomerId == model.CustomerId && x.OrderDate == model.OrderDate && x.ExpectedDeliveryDate == model.ExpectedDeliveryDate && x.Remarks == model.Remarks && x.OrderMasterId != model.OrderMasterId).Count() > 0;

            if (orderMasterId > 0)
            {
                orderMaster = context.OrderMasters.FirstOrDefault(x => x.OrderMasterId == orderMasterId);
                if (orderMaster == null)
                {
                    throw new Exception("Order not found!");
                }
                orderMaster.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                orderMaster.ModifiedDate = DateTime.Now;

            }
            else
            {
                foreach (var orderDetail in orderMaster.OrderDetails)
                {
                    orderDetail.CustomerId = model.CustomerId;
                    orderDetail.OrderDate = model.OrderDate;
                    orderDetail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    orderDetail.CreateDate = DateTime.Now;
                    orderDetail.IsActive = true;
                }
                orderMaster.OrderDate = model.OrderDate.Value;
                orderMaster.OrderMonthYear = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
                orderMaster.OrderStatus = "N";
                orderMaster.CreateDate = DateTime.Now;
                orderMaster.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            orderMaster.CompanyId = model.CompanyId;
            orderMaster.CustomerId = model.CustomerId;
            orderMaster.ProductType = model.ProductType;

            orderMaster.ExpectedDeliveryDate = model.ExpectedDeliveryDate;
            orderMaster.OrderNo = model.OrderNo;
            orderMaster.SalePersonId = model.SalePersonId;
            orderMaster.StockInfoId = model.StockInfoId;
            orderMaster.Remarks = model.Remarks;
            orderMaster.TotalAmount = model.OrderDetails.Sum(x => Convert.ToDecimal(x.Qty) * Convert.ToDecimal(x.UnitPrice));
            orderMaster.DiscountRate = model.DiscountRate;
            orderMaster.DiscountAmount = model.DiscountAmount;
            orderMaster.GrandTotal = model.GrandTotal;
            orderMaster.IsCash = model.IsCash;
            orderMaster.IsActive = true;
            orderMaster.Vendor = null;

            context.Entry(orderMaster).State = orderMaster.OrderMasterId == 0 ? EntityState.Added : EntityState.Modified;
            try
            {
                context.SaveChanges();
                long savedId = orderMaster.OrderMasterId;
                model.OrderMasterId = savedId;
            }
            catch (Exception ex)
            {
                return model.OrderMasterId;
            }







            //For KGeCOM.com

            return model.OrderMasterId;
        }
        public OrderMasterModel GetOrderMaster(long orderMasterId)
        {
            if (orderMasterId <= 0)
            {
                return new OrderMasterModel()
                {
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"])
                };
            }


            OrderMaster orderMaster = context.OrderMasters.Include(x => x.Vendor).Include(x => x.OrderDetails).Where(x => x.OrderMasterId == orderMasterId).FirstOrDefault();
            if (orderMaster == null)
            {
                throw new Exception("Order not found");
            }
            OrderMasterModel model = ObjectConverter<OrderMaster, OrderMasterModel>.Convert(orderMaster);
            return model;
        }
        public OrderMasterViewModel GetFeedOrderMaster(long id, int Uid)
        {
            if (id <= 0)
            {
                return new OrderMasterViewModel()
                {
                    SalePersonName = context.Employees.Where(x => x.Id == Uid).Select(c => c.Name).FirstOrDefault(),
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"])
                };
            }


            OrderMaster orderMaster = context.OrderMasters.Include(x => x.Vendor).Include(x => x.OrderDetails).Where(x => x.OrderMasterId == id).FirstOrDefault();
            if (orderMaster == null)
            {
                throw new Exception("Order not found");
            }
            OrderMasterViewModel model = ObjectConverter<OrderMaster, OrderMasterViewModel>.Convert(orderMaster);
            return model;
        }

        public async Task<OrderMasterViewModel> GetFeedOrderMasterDetails(long id)
        {
            OrderMasterViewModel Models = new OrderMasterViewModel();
            Models = await Task.Run(() => (from t1 in context.OrderMasters
                                           join t3 in context.Vendors on t1.CustomerId equals t3.VendorId
                                           join t4 in context.Employees on t1.SalePersonId equals t4.Id
                                           join t5 in context.StockInfoes on t1.StockInfoId equals t5.StockInfoId
                                           where t1.OrderMasterId == id
                                           select new OrderMasterViewModel
                                           {
                                               CompanyId = t1.CompanyId.Value,
                                               OrderMasterId = t1.OrderMasterId,
                                               ProductType = t1.ProductType,
                                               CustomerId = t1.CustomerId.Value,
                                               Customer = t3.Name,
                                               OrderNo = t1.OrderNo,
                                               OrderDate = t1.OrderDate,
                                               ExpectedDeliveryDate = t1.ExpectedDeliveryDate,
                                               SalePersonName = t4.Name,
                                               Remarks = t1.Remarks,
                                               TotalAmount = t1.TotalAmount,
                                               CustomerPhone = t3.Phone,
                                               CustomerAddress = t3.Address,
                                               Locations = t5.Name,
                                               DeliveryDate = t1.ExpectedDeliveryDate

                                           }).FirstOrDefault());

            Models.DataList = await Task.Run(() => (
                from t1 in context.OrderDetails
                where t1.OrderMasterId == id
                join t4 in context.Products on t1.ProductId equals t4.ProductId into productGroup
                from t4 in productGroup.DefaultIfEmpty()
                select new OrderMasterViewModel
                {
                    ProductName = t4.ProductName,
                    Qty = t1.Qty,
                    Amount = t1.Amount,
                    UnitPrice = t1.UnitPrice,
                }
            ).AsEnumerable());


            return Models;
        }











        public VendorModel GetCustomerInfo(long custId)
        {
            var custInfo = context.Vendors.FirstOrDefault(x => x.VendorId == custId);
            return ObjectConverter<Vendor, VendorModel>.Convert(custInfo);
        }



        public long GetOrderNo()
        {
            long orderId = 0;
            var value = context.OrderMasters.OrderByDescending(x => x.OrderMasterId).FirstOrDefault();
            if (value != null)
            {
                orderId = value.OrderMasterId + 1;

            }
            else
            {
                orderId = orderId + 1;
            }
            return orderId;
        }


        public ProductModel GetProductUnitPrice(int pId)
        {
            var unitPrice = context.Products.FirstOrDefault(x => x.ProductId == pId);
            return ObjectConverter<Product, ProductModel>.Convert(unitPrice);
        }

        public VendorOfferModel GetProductConfig(int pId, int vendorId, int stockInfoId)
        {
            var producConfig = (from t1 in context.Products
                                join t2 in context.VendorOffers on t1.ProductId equals t2.ProductId
                                where t1.ProductId == pId && t2.VendorId == vendorId
                                select new VendorOfferModel
                                {
                                    UnitPrice = t1.UnitPrice ?? 0,
                                    BaseCommission = t2.BaseCommission ?? 0,
                                    CarryingCommission = stockInfoId == 2 ? t2.FactoryCarryingCommission ?? 0 : t2.CarryingCommission ?? 0,
                                    CashCommission = t2.CashCommission ?? 0,
                                    SpecialCommission = t2.SpecialCommission ?? 0,
                                    AdditionPrice = t2.AdditionPrice ?? 0,
                                    MonthlyIncentive = t2.MonthlyIncentive ?? 0,
                                    YearlyIncentive = t2.YearlyIncentive ?? 0

                                }).FirstOrDefault();


            return producConfig;
        }

        public ProductModel GetProductUnitPriceCustomerWise(int pId, int vendorId)
        {

            var vendor = context.Vendors.Where(x => x.VendorId == vendorId).FirstOrDefault();
            Product custInfo = context.Products.FirstOrDefault(x => x.ProductId == pId);
            if (vendor.CustomerType == "Credit")
            {
                custInfo.UnitPrice = custInfo.CreditSalePrice;
            }
            return ObjectConverter<Product, ProductModel>.Convert(custInfo);
        }

        public decimal GetProductAvgPurchasePrice(int pId)
        {
            decimal avgPrice = context.Database.SqlQuery<decimal>("EXEC sp_GetAvgPurchasePrice {0}", pId).FirstOrDefault();

            return avgPrice;
        }

        public DepotCurrentStockModel GetDepotCurrentStockByProduct(int productId, int companyId, int stockInfoId)
        {
            DepotCurrentStockModel stockModel = new DepotCurrentStockModel();
            stockModel = context.Database.SqlQuery<DepotCurrentStockModel>("EXEC DepotCurrentStockByProductId {0},{1},{2},{3}", productId, companyId, stockInfoId, DateTime.Now).FirstOrDefault();

            return stockModel;
        }
        public OrderMasterModel GetOrderInforForEdit(long masterId)
        {
            var order = context.OrderMasters.FirstOrDefault(x => x.OrderMasterId == masterId);
            return ObjectConverter<OrderMaster, OrderMasterModel>.Convert(order);
        }

        public List<OrderDetailModel> GetOrderDetailInforForEdit(long masterId)
        {

            var orderDetail = context.OrderDetails.Where(x => x.OrderMasterId == masterId).ToList();
            return ObjectConverter<OrderDetail, OrderDetailModel>.ConvertList(orderDetail).ToList();
        }

        public bool DeleteOrder(long orderMasterId)
        {
            OrderMaster order = context.OrderMasters.Include(x => x.OrderDetails).Where(x => x.OrderMasterId == orderMasterId).First();
            if (order == null)
            {
                return false;
            }
            if (order.OrderDetails.Any())
            {
                context.OrderDetails.RemoveRange(order.OrderDetails);
                context.SaveChanges();
            }
            context.OrderMasters.Remove(order);
            return context.SaveChanges() > 0;
        }

        public string GetNewOrderNo(int companyId, int stockInfoId, string productType)
        {
            OrderMaster order = context.OrderMasters.Where(x => x.StockInfoId == stockInfoId && x.ProductType.Equals(productType)).OrderBy(x => x.OrderNo).ToList().LastOrDefault();
            if (order == null)
            {
                StockInfo stockInfo = context.StockInfoes.Where(x => x.StockInfoId == stockInfoId).FirstOrDefault();
                if (productType.Equals("R"))
                {
                    return stockInfo.Code + "-RM" + "000001";
                }
                else
                {
                    return stockInfo.Code + "-FG" + "000001";
                }

            }

            string firstPart = order.OrderNo.Substring(0, 6);
            int numberPart = Convert.ToInt32(order.OrderNo.Substring(6, 6));

            return GenerateOrderNo(firstPart, numberPart);
        }


        private string GenerateOrderNo(string firstPart, int numberPart)
        {
            numberPart = numberPart + 1;
            return firstPart + numberPart.ToString().PadLeft(6, '0');
        }

        public bool DeleteOrderDetail(long orderDetailId, out string productType)
        {
            productType = "F";
            OrderDetail orderDetail = context.OrderDetails.Include(x => x.Product).Where(x => x.OrderDetailId == orderDetailId).FirstOrDefault();
            if (orderDetail == null)
            {
                return false;
            }
            productType = orderDetail.Product.ProductType;
            context.OrderDetails.Remove(orderDetail);
            return context.SaveChanges() > 0;
        }

        public bool UpdateOrder(OrderMasterModel model, out string message)
        {
            message = string.Empty;

            OrderMaster orderMaster = context.OrderMasters.Where(x => x.OrderMasterId == model.OrderMasterId).FirstOrDefault();

            if (orderMaster == null)
            {
                throw new Exception("Order not found!");
            }
            orderMaster.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            orderMaster.ModifiedDate = DateTime.Now;

            orderMaster.OrderNo = model.OrderNo;
            orderMaster.OrderDate = model.OrderDate.Value;
            orderMaster.ExpectedDeliveryDate = model.ExpectedDeliveryDate;
            orderMaster.SalePersonId = model.SalePersonId;
            orderMaster.StockInfoId = model.StockInfoId;
            orderMaster.Remarks = model.Remarks;
            orderMaster.TotalAmount = model.OrderDetails.Sum(x => Convert.ToDecimal(x.Qty) * Convert.ToDecimal(x.UnitPrice));
            orderMaster.OrderDetails = null;
            if (context.SaveChanges() > 0)
            {
                foreach (var detail in model.OrderDetails)
                {
                    // MaterialReceiveDetail materialReceiveDetail = ObjectConverter<MaterialReceiveDetailModel, MaterialReceiveDetail>.Convert(detail);
                    OrderDetail orderDetail = context.OrderDetails.Where(x => x.OrderDetailId == detail.OrderDetailId).FirstOrDefault();

                    orderDetail.OrderDate = orderMaster.OrderDate;
                    orderDetail.ProductId = detail.ProductId;
                    orderDetail.Qty = detail.Qty ?? 0;
                    orderDetail.UnitPrice = detail.UnitPrice ?? 0;
                    orderDetail.Amount = orderDetail.Qty * orderDetail.UnitPrice;

                    orderDetail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    orderDetail.ModifedDate = DateTime.Today;

                    context.Entry(orderDetail).State = orderDetail.OrderDetailId == 0 ? EntityState.Added : EntityState.Modified;
                    context.SaveChanges();
                }
            }
            message = "Order updated successfully.";
            return context.SaveChanges() > 0;

        }

        public bool CheckDepoOrder(long orderMasterId)
        {
            bool isDepoOrder = context.Database.SqlQuery<bool>(@"SELECT 
                                                              CASE WHEN Lower(Name)='factory' THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS IsDepoOrder  
                                                              From Erp.StockInfo 
                                                              WHERE StockInfoId = (SELECT StockInfoId From Erp.OrderMaster WHERE OrderMasterId = {0})", orderMasterId).FirstOrDefault();
            return isDepoOrder;

        }

        public bool SupportDeleteOrderByOrderNo(int companyId, string orderNo)
        {
            return context.Database.ExecuteSqlCommand("EXEC Helper_DeleteOrderAllInfoByCompanyIdAndOrderNo {0},{1}", companyId, orderNo) > 0;
        }
        public (long, bool) UpdateSeedAttrOrderDeliver(long id, bool isSeen)
        {
            long result = -1;
            //to select Accountining_Chart_Two data.....
            OrderDeliver orderDeliver = context.OrderDelivers.Find(id);
            orderDeliver.IsSeen = isSeen;
            orderDeliver.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            orderDeliver.ModifedDate = DateTime.Now;

            if (context.SaveChanges() > 0)
            {
                result = orderDeliver.OrderDeliverId;
            }
            return (result, orderDeliver.IsSeen);
        }
        public async Task<OrderMasterModel> GetChallanPrintOD(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            long userID = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"].ToString());
            var depot = context.StockInfoes.Where(x => x.CompanyId == companyId && x.DepotInchargeEmpId == userID).FirstOrDefault();
            OrderMasterModel orderMasterModel = new OrderMasterModel();
            if (userID > 0 && depot.StockInfoId > 0)
            {
                orderMasterModel.DataList = await Task.Run(() => (from t2 in context.OrderDelivers
                                                                  join t1 in context.OrderMasters on t2.OrderMasterId equals t1.OrderMasterId
                                                                  join t3 in context.Vendors on t1.CustomerId equals t3.VendorId
                                                                  join t4 in context.Employees on t1.SalePersonId equals t4.Id
                                                                  where t1.CompanyId == companyId && t1.StockInfoId == depot.StockInfoId
                                                                   && t2.DeliveryDate >= fromDate
                                                                   && t2.DeliveryDate <= toDate
                                                                  select new OrderMasterModel
                                                                  {
                                                                      IsSeen = t2.IsSeen,
                                                                      CompanyId = t1.CompanyId.Value,
                                                                      OrderMasterId = t1.OrderMasterId,
                                                                      ProductType = t1.ProductType,
                                                                      CustomerId = t1.CustomerId.Value,
                                                                      Customer = t3.Name,
                                                                      OrderNo = t1.OrderNo,
                                                                      OrderDate = t1.OrderDate,
                                                                      DeliveryDate = t2.DeliveryDate,
                                                                      ChallanNo = t2.ChallanNo,
                                                                      InvoiceNo = t2.InvoiceNo,
                                                                      OrderStatus = t1.OrderStatus,
                                                                      OrderDeliverId = t2.OrderDeliverId,
                                                                      IsSubmitted = t2.IsSubmitted
                                                                  }).OrderByDescending(x => x.OrderDate).AsEnumerable());
            }


            return orderMasterModel;

        }




        public VendorModel GetfeedCustomer(int CompanyId)
        {
            VendorModel vendorModel = new VendorModel();
            vendorModel.List = context.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == 2 && x.CompanyId == CompanyId)
                .Select(x => new VendorModel
                {
                    VendorId = x.VendorId,
                    Name = x.Name,
                })
                .ToList();

            return vendorModel;
        }


        public VMFeedPayment GetPaymentShortList(int VendorId)
        {
            VMFeedPayment vMFeedPayment = new VMFeedPayment();
            vMFeedPayment.DataListPayment = (IEnumerable<VMFeedPayment>)(from t1 in context.Payments
                                                                         join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                                         where t2.VendorId == VendorId && t1.BranchName == "Short Credit" && t1.IsActive == true

                                                                         select new VMFeedPayment
                                                                         {
                                                                             Vendorname = t2.Name
                                                ,
                                                                             VendorId = t1.VendorId,
                                                                             BranchName = t1.BranchName,
                                                                             TransactionDate = t1.TransactionDate,
                                                                             ReferenceNo = t1.ReferenceNo,
                                                                             InAmount = t1.InAmount,
                                                                             OutAmount = t1.OutAmount,



                                                                         }).ToList(); ;

            return vMFeedPayment;
        }

        public VMFeedPayment GetShortList(int Companyid)
        {
            VMFeedPayment vMFeedPayment = new VMFeedPayment();
            vMFeedPayment.DataListPayment = (IEnumerable<VMFeedPayment>)(from t1 in context.Payments
                                                                         join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                                         where t1.CompanyId==Companyid && t1.BranchName == "Short Credit" && t1.IsActive == true

                                                                         select new VMFeedPayment
                                                                         {
                                                                             Vendorname = t2.Name,
                                                                             VendorId = t1.VendorId,
                                                                             BranchName = t1.BranchName,
                                                                             TransactionDate = t1.TransactionDate,
                                                                             ReferenceNo = t1.ReferenceNo,
                                                                             InAmount = t1.InAmount,
                                                                             OutAmount = t1.OutAmount,



                                                                         }).ToList(); ;

            return vMFeedPayment;
        }

        public async Task<OrderMasterModel> GetDeliveredOrderList(int companyId, DateTime? fromDate, DateTime? toDate, string productType)
        {
            OrderMasterModel orderMasterModel = new OrderMasterModel();
            orderMasterModel.DataList = await Task.Run(() => (from t1 in context.OrderDelivers
                                                              join t2 in context.OrderMasters on t1.OrderMasterId equals t2.OrderMasterId
                                                              join t3 in context.Vendors on t2.CustomerId equals t3.VendorId

                                                              where t1.CompanyId == companyId
                                                              && t1.ProductType == productType
                                                              && t1.DeliveryDate >= fromDate
                                                              && t1.IsActive == true
                                                              && t1.DeliveryDate <= toDate
                                                              select new OrderMasterModel
                                                              {
                                                                  CompanyId = t2.CompanyId.Value,
                                                                  OrderMasterId = t2.OrderMasterId,
                                                                  ProductType = t2.ProductType,
                                                                  CustomerId = t2.CustomerId.Value,
                                                                  Customer = t3.Name,
                                                                  OrderNo = t2.OrderNo,
                                                                  OrderDate = t2.OrderDate,
                                                                  DeliveryDate = t1.DeliveryDate ,
                                                                  ChallanNo = t1.ChallanNo,
                                                                  InvoiceNo = t1.InvoiceNo,
                                                                  OrderStatus = t2.OrderStatus,
                                                                  OrderDeliverId = t1.OrderDeliverId,
                                                                  IsSubmitted = t1.IsSubmitted ,
                                                                  Status = t2.Status
                                                              }).OrderByDescending(x => x.DeliveryDate).AsEnumerable());

            return orderMasterModel;

        }

    }
}
