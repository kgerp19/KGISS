using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace KGERP.Service.Implementation
{
    public class OrderDeliverService : IOrderDeliverService
    {

        private readonly ERPEntities context;

        public OrderDeliverService(ERPEntities context)
        {
            this.context = context;
        }

        public List<DeliverItemCustomModel> GetDeliverItems(long orderMasterId, int stockInfoId, int companyId, string productType)
        {
            IEnumerable<OrderDetail> orderDetails = context.OrderDetails
                .Include(x => x.Product.Unit)
                .Where(x => x.OrderMasterId == orderMasterId);

            //Not use this line
           // List<StoreDetail> storeDetails = context.Database.SqlQuery<StoreDetail>("exec spGetDeliverItems {0}", stockInfoId).ToList();

            List<DeliverItemCustomModel> deliverItemCustomModels = orderDetails.Select(x =>
            new DeliverItemCustomModel
            {
                CompanyId = companyId,
                ProductId = x.ProductId,
                ProductCode = x.Product.ProductCode,
                ProductName = x.Product.ProductName,
                OrderUnit = x.Product.Unit.Name,
                OrderQty = x.Qty,
                Engine = GetEngineNo(x.ProductId),
                UnitPrice = x.UnitPrice,
                StoreAvailableQty = 0,
                ReadyToDeliver = 0,
                OrderRemainingQty = x.Qty - 0,
                OrderDetailId = x.OrderDetailId,
                DeliveredQty = context.OrderDeliverDetails.Where(p => p.ProductId == x.ProductId && p.OrderDeliver.OrderMasterId == orderMasterId).Any() ? context.OrderDeliverDetails.Where(p => p.ProductId == x.ProductId && p.OrderDeliver.OrderMasterId == orderMasterId).Sum(s => s.DeliveredQty) : 0
            }).ToList();

            foreach (var item in deliverItemCustomModels)
            {
                var product = context.Products.Find(item.ProductId);
                item.TPPrice = product.CostingPrice;
                item.OrderDetailId = item.OrderDetailId;
                item.DeliveredQty = item.DeliveredQty;
                item.DueQty = item.OrderQty - item.DeliveredQty;
                item.OrderRemainingQty = item.OrderQty - item.DeliveredQty;
                item.ReadyToDeliver = item.OrderRemainingQty ?? 0;
                item.OrderRemainingQty = item.OrderRemainingQty - item.OrderRemainingQty;
                if (companyId == (int)CompanyName.KrishibidFeedLimited || companyId == (int)CompanyName.MymensinghHatcheryAndFeedsLtd)
                {
                    item.EBaseCommission = context.Database.SqlQuery<decimal>("exec spGetEffectiveBaseCommission {0},{1}", item.ProductId, orderMasterId).FirstOrDefault();
                    item.ECarryingCommission = context.Database.SqlQuery<decimal>("exec spGetEffectiveCarryingCommission {0},{1}", item.ProductId, orderMasterId).FirstOrDefault();
                    item.ECashCommission = context.Database.SqlQuery<decimal>("exec spGetEffectiveCashCommission {0},{1}", item.ProductId, orderMasterId).FirstOrDefault();
                    item.SpecialDiscount = context.Database.SqlQuery<decimal>("exec spGetEffectiveSpecialCommission {0},{1}", item.ProductId, orderMasterId).FirstOrDefault();
                    item.AdditionPrice = context.Database.SqlQuery<decimal>("exec spGetAdditionalPrice {0},{1}", item.ProductId, orderMasterId).FirstOrDefault();
                    item.DisplayUnitPrice = item.UnitPrice + Convert.ToDouble(item.AdditionPrice);
                }
                //item.StoreAvailableQty = context.Database.SqlQuery<long>("exec spGetStoreAvailableQty {0},{1},{2}", item.ProductId, stockInfoId, orderMasterId).FirstOrDefault();
                item.StoreAvailableQty = Convert.ToDouble(context.Database.SqlQuery<FeedFinishedProductStock>("Finished_Product_Wise_StockReport {0}, {1}, {2}", DateTime.Now, companyId, item.ProductId).FirstOrDefault().ClosingQty);
            }
            return deliverItemCustomModels;
        }



        public OrderDeliverModel GetOrderDeliver(long id)
        {
            OrderDeliver orderDeliver = context.OrderDelivers.Where(x => x.OrderDeliverId == id).FirstOrDefault();
            if (orderDeliver == null)
            {
                return new OrderDeliverModel();
            }
            OrderDeliverModel model = ObjectConverter<OrderDeliver, OrderDeliverModel>.Convert(orderDeliver);
            return model;
        }

        public List<OrderDeliverModel> GetOrderDelivers(string searchText)
        {
            IQueryable<OrderDeliver> orderDelivers = context.OrderDelivers.OrderByDescending(x => x.OrderDeliverId).AsQueryable();
            return ObjectConverter<OrderDeliver, OrderDeliverModel>.ConvertList(orderDelivers.ToList()).ToList();
        }

        public OrderDeliverModel GetOrderDeliverWithInclude(long orderDeliverId)
        {
            OrderDeliver orderDeliver = context.OrderDelivers.Include("OrderMaster").Include("StockInfo").Where(x => x.OrderDeliverId == orderDeliverId).FirstOrDefault();
            if (orderDeliver == null)
            {
                return new OrderDeliverModel();
            }
            OrderDeliverModel model = ObjectConverter<OrderDeliver, OrderDeliverModel>.Convert(orderDeliver);
            return model;
        }

        public OrderDeliverModel SaveOrderDeliver(long id, OrderDeliverModel model, out string message)
        {
            OrderMaster order = context.OrderMasters
                //  .Include(i=>i.OrderMasterId)
                .Include(x => x.Vendor)
                .Where(x => x.OrderMasterId == model.OrderMasterId)
                .FirstOrDefault();
            VendorModel customer = new VendorModel { Name = order.Vendor.Name, Phone = order.Vendor.Phone, Address = order.Vendor.Address };
            message = string.Empty;
            OrderDeliver orderDeliver = ObjectConverter<OrderDeliverModel, OrderDeliver>.Convert(model);

            bool isExist = context.OrderDelivers.Include("OrderMaster").Any(x => x.CompanyId == model.CompanyId && x.InvoiceNo == model.InvoiceNo && x.OrderMaster.CustomerId == order.CustomerId);
            if (isExist)
            {
                message = "Data already exists !";
                return model;
            }
            if (id > 0)
            {
                orderDeliver = context.OrderDelivers.Where(x => x.OrderDeliverId == id).FirstOrDefault();
                if (orderDeliver == null)
                {
                    throw new Exception("Data not found!");
                }
            }

            else
            {
                orderDeliver.CreatedDate = DateTime.Now;
                orderDeliver.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                orderDeliver.IsActive = true;
                
            }

            List<OrderDeliverDetail> orderDeliverDetails = new List<OrderDeliverDetail>();
            foreach (var item in model.OrderDeliverDetails)
            {
                OrderDeliverDetail orderDeliverDetail = new OrderDeliverDetail()
                {
                    
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    ProductId = item.ProductId,
                    UnitPrice = item.UnitPrice,
                    DeliveredQty = item.DeliveredQty,
                    Amount = item.Amount,
                    BaseCommission = item.BaseCommission,
                    CashCommission = item.CashCommission,
                    CarryingRate = item.CarryingRate,
                    CreditCommission = item.CreditCommission,
                    SpecialDiscount = item.SpecialDiscount,
                    EBaseCommission = item.EBaseCommission,
                    ECarryingCommission = item.ECarryingCommission,
                    ECashCommission = item.ECashCommission,
                    COGSPrice = item.COGSPrice,// context.Database.SqlQuery<decimal>("select cast(isnull(Sum(UnitPrice * InQty)/ NULLIF(sum(InQty),0),0) as decimal(18,2)) as COGSPrice from Erp.ProductStore where ProductId = {0}", item.ProductId).FirstOrDefault(),
                    SaleCommissionRate = item.SaleCommissionRate,
                    AdditionPrice = item.AdditionPrice,
                    EngineNo = item.EngineNo,
                    ChassisNo = item.ChassisNo,
                    BatteryNo = item.BatteryNo,
                    RearTyreLH = item.RearTyreLH,
                    RearTyreRH = item.RearTyreRH,
                    OrderDetailId = item.OrderDetailId
                };
                orderDeliverDetails.Add(orderDeliverDetail);
            }

            orderDeliver.OrderMasterId = model.OrderMasterId;
            orderDeliver.StockInfoId = Convert.ToInt32(order.StockInfoId);
            orderDeliver.ChallanNo = model.ChallanNo;
            orderDeliver.VehicleNo = model.VehicleNo;
            orderDeliver.InvoiceNo = model.InvoiceNo;
            orderDeliver.TotalAmount = model.TotalAmount;
            orderDeliver.Discount = model.Discount;
            orderDeliver.DiscountRate = model.DiscountRate;
            orderDeliver.CompanyId = model.CompanyId;
            orderDeliver.DeliveryDate = model.DeliveryDate;

            orderDeliver.OrderMaster = null;
            orderDeliver.OrderDeliverDetails = null;
            orderDeliver.OrderDeliverDetails = orderDeliverDetails;
            context.OrderDelivers.Add(orderDeliver);

            try
            {
                if (context.SaveChanges() > 0)
                {

                    try
                    {
                        //delete data from OrderDeliveryPreview table 
                        int noOfOrderDeliverDetailDeteleted = context.Database.ExecuteSqlCommand("delete from Erp.OrderDeliveryPreview where OrderMasterId={0}", orderDeliver.OrderMasterId);
                        //insert data to ProductStore After Delivery
                        int noOfRowsAffectedInProductStore = context.Database.ExecuteSqlCommand("exec sp_OrderDeliver_UpdateStock {0}", orderDeliver.OrderDeliverId);
                    }
                    catch (Exception e)
                    {
                        int noOfRowsDeleted = context.Database.ExecuteSqlCommand(@"delete from Erp.OrderDeliverDetail where OrderDeliverId={0}
                                                                                   delete from Erp.OrderDeliver where OrderDeliverId={0}", orderDeliver.OrderDeliverId);
                        throw;
                    }

                    message = "Order delivered successfully.";

                    //Change Order Status
                    int result = context.Database.ExecuteSqlCommand("exec UpdateOrderMasterStatus {0}", orderDeliver.OrderMasterId);

                    //Update Customer Payment Table
                   // int noOfRowAffected = context.Database.ExecuteSqlCommand("exec spUpdateVendorDuePayment {0}", orderDeliver.OrderDeliverId);

                    //Send SMS to Customer

                    string api_key = "KEY-czvwjcex2sdevdgf390x7k20vs7eeo4t";
                    string api_secret = "YnRUb0JH38cKmI@n";
                    string request_type = "SINGLE_SMS";
                    string message_type = "TEXT | UNICODE";
                    string mobile = customer.Phone ?? "01700729807";
                    string message_body = "Customer Name : " + customer.Name + ", Invoice No: " + orderDeliver.InvoiceNo + ", Amount : " + orderDeliver.TotalAmount + " Tk";

                    string url = string.Format("https://portal.adnsms.com/api/v1/secure/send-sms?api_key={0}&api_secret={1}&request_type={2}&message_type={3}&mobile={4}&message_body={5}", api_key, api_secret, request_type, message_type, mobile, message_body);

                    Message sms = new Message
                    {
                        ReceiverName = customer.Name,
                        ReceiverMobile = mobile,
                        SendDate = DateTime.Now,
                        request_type = "request_type",
                        campaign_uid = "campaign_uid",
                        sms_uid = "sms_uid",
                        api_response_code = "200",
                        api_response_message = "SUCCESS",
                        MessageBody = message_body
                    };

                    context.Messages.Add(sms);
                    context.SaveChanges();

                    //This Code is reserved for future Use

                    //using (var client = new HttpClient())
                    //{
                    //    client.BaseAddress = new Uri(url);

                    //    //HTTP POST
                    //    HttpResponseMessage apiResult = client.PostAsync("", null).Result;
                    //    if (apiResult.IsSuccessStatusCode)
                    //    {
                    //        var smsResponse = apiResult.Content.ReadAsAsync<SMSService>().Result;

                    //        Message sms = new Message
                    //        {
                    //            ReceiverName = customer.Name,
                    //            ReceiverMobile = mobile,
                    //            SendDate = DateTime.Now,
                    //            request_type = smsResponse.request_type,
                    //            campaign_uid = smsResponse.campaign_uid,
                    //            sms_uid = smsResponse.sms_uid,
                    //            api_response_code = smsResponse.api_response_code,
                    //            api_response_message = smsResponse.api_response_message,
                    //            MessageBody = message_body
                    //        };

                    //        context.Messages.Add(sms);
                    //        context.SaveChanges();
                    //    }
                    //}


                }
            }
            catch (DbEntityValidationException e)
            {

                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return ObjectConverter<OrderDeliver, OrderDeliverModel>.Convert(orderDeliver);
        }


        public string GetNewChallanNoByCompany(int companyId, string productType)
        {
            OrderDeliver orderDeliver = context.OrderDelivers.Where(x => x.CompanyId == companyId && x.ProductType.ToLower().Equals(productType.ToLower())).OrderByDescending(x => x.OrderDeliverId).FirstOrDefault();

            if (orderDeliver == null)
            {
                if (productType.ToLower().Equals("f"))
                {
                    return "FG000001";
                }
                return "RM000001";
            }

            string prefixPart = orderDeliver.ChallanNo.Substring(0, 2);
            string numberPart = orderDeliver.ChallanNo.Substring(2, 6);
            int newNumberPart = Convert.ToInt32(numberPart) + 1;
            return prefixPart + newNumberPart.ToString().PadLeft(6, '0');
        }

        public string GetLastInvoceNoByCompany(int companyId)
        {

            OrderDeliver orderDeliver = context.OrderDelivers.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.OrderDeliverId).FirstOrDefault();

            if (orderDeliver == null)
            {
                return string.Empty;
            }
            return orderDeliver.InvoiceNo;
        }

        public string GetGeneratedInvoiceNoByStockInfo(int stockInfoId)
        {

            StockInfo stockInfo = context.StockInfoes.Where(x => x.StockInfoId == stockInfoId).First();
            OrderDeliver orderDeliver = context.OrderDelivers.Where(x => x.StockInfoId == stockInfoId).OrderByDescending(x => x.OrderDeliverId).FirstOrDefault();

            if (orderDeliver == null)
            {
                return stockInfo.Code + "000001";
            }
            if (string.IsNullOrEmpty(orderDeliver.InvoiceNo))
            {
                return stockInfo.Code + "000001";
            }
            string lastInvoiceNo = orderDeliver.InvoiceNo.Substring(3);


            int num = Convert.ToInt32(lastInvoiceNo);
            num = ++num;
            return stockInfo.Code + num.ToString().PadLeft(6, '0');
        }

        public List<OrderDeliveryPreview> SaveOrderDeliverPreview(int id, List<OrderDeliveryPreview> previews)


        {
            //context.Database.ExecuteSqlCommand(@"truncate table Erp.OrderDeliveryPreview");

            string storeName = context.Database.SqlQuery<string>(@"select Name from Erp.StockInfo where StockInfoId={0}", previews.First().StockInfoId).FirstOrDefault();

            foreach (var preview in previews)
            {
                preview.StoreName = storeName;
                context.OrderDeliveryPreviews.Add(preview);
                try
                {
                    context.SaveChanges();
                }

                catch (DbEntityValidationException exc)
                {
                    // just to ease debugging
                    foreach (var error in exc.EntityValidationErrors)
                    {
                        foreach (var errorMsg in error.ValidationErrors)
                        {
                            // logging service based on NLog
                            string message = errorMsg.ErrorMessage;
                        }
                    }

                    throw;
                }
                catch (DbUpdateException e)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"DbUpdateException error details - {e?.InnerException?.InnerException?.Message}");

                    foreach (var eve in e.Entries)
                    {
                        sb.AppendLine($"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");
                    }



                    string message = e.ToString();
                    message = sb.ToString();

                    throw;
                }


            }

            return previews;
        }

        public List<SelectModel> GetInvoiceSelectList(int companyId)
        {
            return context.OrderDelivers.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
            {
                Text = x.InvoiceNo.ToString(),
                Value = x.OrderDeliverId
            }).ToList();
        }


        public List<SelectModel> GetEngineNo(int? productId)
        {
            return context.ProductDetails.Where(x => x.ProductId == productId).ToList().Select(x => new SelectModel()
            {
                Text = x.EngineNo.ToString(),
                Value = x.EngineNo.ToString()
            }).ToList();
        }

        public ProductDetailModel GetProductDetails(string engineNo)
        {
            var productDetails = context.ProductDetails.Where(x => x.EngineNo == engineNo).FirstOrDefault();
            return ObjectConverter<ProductDetail, ProductDetailModel>.Convert(productDetails);
        }

        public object GetInvoiceNoAutoComplete(int customerId, string prefix, int companyId)
        {
            return context.OrderDelivers.Include("OrderMaster").Where(x => x.OrderMaster.CustomerId == customerId && x.CompanyId == companyId && x.InvoiceNo.Contains(prefix)).Select(x => new
            {
                label = x.InvoiceNo,
                val = x.OrderDeliverId
            }).OrderBy(x => x.label).ToList();
        }


    }
}
