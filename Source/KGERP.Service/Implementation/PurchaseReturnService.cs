using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class PurchaseReturnService : IPurchaseReturnService
    {
        private readonly ERPEntities context;
        public PurchaseReturnService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<PurchaseReturnModel> GetPurchaseReturns(int companyId, DateTime? fromDate, DateTime? toDate, string type = "")
        {
            PurchaseReturnModel model = new PurchaseReturnModel();
            model.CompanyId = companyId;
            model.DataList = await Task.Run(() => (from t1 in context.PurchaseReturns
                                                        join t2 in context.Vendors on t1.SupplierId equals t2.VendorId
                                                        where t1.CompanyId == companyId
                                                         && t1.ReturnDate >= fromDate
                                                         && t1.ReturnDate <= toDate
                                                         && t1.ProductType == type
                                                        select new PurchaseReturnModel
                                                        {
                                                            ProductType = t1.ProductType,
                                                            PurchaseReturnId= t1.PurchaseReturnId,
                                                            ReturnDate= t1.ReturnDate,
                                                            ReturnNo= t1.ReturnNo,
                                                            SupplierName = t2.Name
                                                        })
                                                        .OrderByDescending(o => o.ReturnDate).AsEnumerable());;
            return model;
        }
       


        public bool SavePurchaseReturn(long purchaseReturnId, PurchaseReturnModel model, out string message)
        {
            message = "Purchase return failed !";
            PurchaseReturn purchaseReturn = ObjectConverter<PurchaseReturnModel, PurchaseReturn>.Convert(model);

            bool returnExists = context.PurchaseReturns.Any(x => x.CompanyId == model.CompanyId && x.ReturnNo == model.ReturnNo && x.SupplierId == model.SupplierId);
            if (returnExists)
            {
                message = "Data already exists !";
                return !returnExists;
            }

            if (model.PurchaseReturnId > 0)
            {
                purchaseReturn = context.PurchaseReturns.Where(x => x.PurchaseReturnId == model.PurchaseReturnId).FirstOrDefault();
                if (purchaseReturn == null)
                {
                    throw new Exception("Data not found!");
                }
                purchaseReturn.ModifiedDate = DateTime.Now;
                purchaseReturn.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                purchaseReturn.Active = true;
                purchaseReturn.CreatedDate = DateTime.Now;
                purchaseReturn.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }


            purchaseReturn.CompanyId = model.CompanyId;
            purchaseReturn.SupplierId = model.SupplierId;
            purchaseReturn.ProductType = model.ProductType;
            purchaseReturn.ReturnNo = model.ReturnNo;
            purchaseReturn.ReturnDate = model.ReturnDate;
            purchaseReturn.ReturnReason = model.ReturnReason;
            purchaseReturn.StockInfoId = model.StockInfoId;
            purchaseReturn.ReturnBy = model.ReturnBy;
            context.Entry(purchaseReturn).State = purchaseReturn.PurchaseReturnId == 0 ? EntityState.Added : EntityState.Modified;
            int noOfRowsAffected = context.SaveChanges();
            if (noOfRowsAffected > 0)
            {
                message = "Purchase return completed successfully !";
                return context.Database.ExecuteSqlCommand("exec CreatePurchaseReturnProductStore {0}", purchaseReturn.PurchaseReturnId) > 0;
            }
            return noOfRowsAffected > 0;
        }

        public PurchaseReturnModel GetPurchaseReturn(long id)
        {
            if (id <= 0)
            {
                return new PurchaseReturnModel()
                {
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"])
                };
            }
            PurchaseReturn purchaseReturn = context.PurchaseReturns.Include(x => x.Vendor).Include(x => x.PurchaseReturnDetails).Where(x => x.PurchaseReturnId == id).FirstOrDefault();
            if (purchaseReturn == null)
            {
                throw new Exception("Order not found");
            }
            PurchaseReturnModel model = ObjectConverter<PurchaseReturn, PurchaseReturnModel>.Convert(purchaseReturn);
            return model;
        }

        public long GetPurchaseReturnNo()
        {
            long purchaseReturnId = 0;
            var value = context.PurchaseReturns.OrderByDescending(x => x.PurchaseReturnId).FirstOrDefault();
            if (value != null)
            {
                purchaseReturnId = value.PurchaseReturnId + 1;

            }
            else
            {
                purchaseReturnId = purchaseReturnId + 1;
            }
            return purchaseReturnId;
        }
        //public OrderMasterModel GetOrderInforForEdit(long masterId)
        //{
        //    var order = context.OrderMasters.FirstOrDefault(x => x.OrderMasterId == masterId);
        //    return ObjectConverter<OrderMaster, OrderMasterModel>.Convert(order);
        //}

        //public List<OrderDetailModel> GetOrderDetailInforForEdit(long masterId)
        //{

        //    var orderDetail = context.OrderDetails.Where(x => x.OrderMasterId == masterId).ToList();
        //    return ObjectConverter<OrderDetail, OrderDetailModel>.ConvertList(orderDetail).ToList();
        //}

        public bool DeleteOrder(long purchaseReturnId)
        {
            PurchaseReturn purchaseReturn = context.PurchaseReturns.Include(x => x.PurchaseReturnDetails).Where(x => x.PurchaseReturnId == purchaseReturnId).First();
            if (purchaseReturn == null)
            {
                return false;
            }
            if (purchaseReturn.PurchaseReturnDetails.Any())
            {
                context.PurchaseReturnDetails.RemoveRange(purchaseReturn.PurchaseReturnDetails);
                context.SaveChanges();
            }
            context.PurchaseReturns.Remove(purchaseReturn);
            return context.SaveChanges() > 0;
        }

        public string GetNewPurchaseReturnNo(int companyId, string productType)
        {
            string purchaseReturnNo = string.Empty;
            if (!context.PurchaseReturns.Where(x => x.CompanyId == companyId && x.ProductType.Equals(productType)).Any())
            {
                if (productType.Equals("R"))
                {
                    return "RM-PR" + "000001";
                }
                else
                {
                    return "FG-PR" + "000001";
                }
            }
            else
            {
                PurchaseReturn purchaseReturn = context.PurchaseReturns.Where(x => x.CompanyId == companyId && x.ProductType.Equals(productType)).ToList().LastOrDefault();
                if (purchaseReturn != null)
                {
                    purchaseReturnNo = purchaseReturn.ReturnNo;
                }
            }

            string firstPart = purchaseReturnNo.Substring(0, 5);
            int numberPart = Convert.ToInt32(purchaseReturnNo.Substring(5, 6));
            return GenerateReturnNo(firstPart, numberPart);
        }


        private string GenerateReturnNo(string firstPart, int numberPart)
        {
            numberPart = numberPart + 1;
            return firstPart + numberPart.ToString().PadLeft(6, '0');
        }
        //public bool DeleteOrderDetail(long orderDetailId, out string productType)
        //{
        //    productType = "F";
        //    OrderDetail orderDetail = context.OrderDetails.Include(x => x.Product).Where(x => x.OrderDetailId == orderDetailId).FirstOrDefault();
        //    if (orderDetail == null)
        //    {
        //        return false;
        //    }
        //    productType = orderDetail.Product.ProductType;
        //    context.OrderDetails.Remove(orderDetail);
        //    return context.SaveChanges() > 0;
        //}

        //public bool UpdateOrder(OrderMasterModel model, out string message)
        //{
        //    message = string.Empty;

        //    OrderMaster orderMaster = context.OrderMasters.Where(x => x.OrderMasterId == model.OrderMasterId).FirstOrDefault();

        //    if (orderMaster == null)
        //    {
        //        throw new Exception("Order not found!");
        //    }
        //    orderMaster.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //    orderMaster.ModifiedDate = DateTime.Now;

        //    orderMaster.OrderNo = model.OrderNo;
        //    orderMaster.OrderDate = model.OrderDate.Value;
        //    orderMaster.ExpectedDeliveryDate = model.ExpectedDeliveryDate;
        //    orderMaster.SalePersonId = model.SalePersonId;
        //    orderMaster.StockInfoId = model.StockInfoId;
        //    orderMaster.Remarks = model.Remarks;
        //    orderMaster.TotalAmount = model.OrderDetails.Sum(x => Convert.ToDecimal(x.Qty) * Convert.ToDecimal(x.UnitPrice));
        //    orderMaster.OrderDetails = null;
        //    if (context.SaveChanges() > 0)
        //    {
        //        foreach (var detail in model.OrderDetails)
        //        {
        //            // MaterialReceiveDetail materialReceiveDetail = ObjectConverter<MaterialReceiveDetailModel, MaterialReceiveDetail>.Convert(detail);
        //            OrderDetail orderDetail = context.OrderDetails.Where(x => x.OrderDetailId == detail.OrderDetailId).FirstOrDefault();

        //            orderDetail.OrderDate = orderMaster.OrderDate;
        //            orderDetail.ProductId = detail.ProductId;
        //            orderDetail.Qty = detail.Qty ?? 0;
        //            orderDetail.UnitPrice = detail.UnitPrice ?? 0;
        //            orderDetail.Amount = orderDetail.Qty * orderDetail.UnitPrice;

        //            orderDetail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //            orderDetail.ModifedDate = DateTime.Today;

        //            context.Entry(orderDetail).State = orderDetail.OrderDetailId == 0 ? EntityState.Added : EntityState.Modified;
        //            context.SaveChanges();
        //        }
        //    }
        //    message = "Order updated successfully.";
        //    return context.SaveChanges() > 0;

        //}




    }
}
