using KGERP.Data.Models;
using KGERP.Service.Interface;

namespace KGERP.Service.Implementation
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly ERPEntities context;
        public OrderDetailService(ERPEntities context)
        {
            this.context = context;
        }

        //public List<DeliverItemCustomModel> GetDeliverItems(long orderDeliverId)
        //{

        //    OrderDeliver orderDeliver = OrderDeliverRepository.OrderDelivers.Where(x => x.OrderDeliverId == orderDeliverId).FirstOrDefault();


        //    List<DeliverItemCustomModel> deliverItemCustomModels = orderDetailRepository.OrderDetails.Include("Product").Where(x => x.OrderMasterId == orderDeliver.OrderMasterId).Select(x =>
        //    new DeliverItemCustomModel
        //    {

        //        ProductId = x.ProductId,
        //        ProductName = x.Product.ProductName,
        //        OrderQty = x.Qty,
        //        OrderUnitPrice = x.UnitPrice
        //    }).ToList();



        //    List<StoreDetail> storeDetails = StoreDetailRepository.StoreDetails.Include("Store").Where(x => x.Store.StockInfoId == orderDeliver.StockInfoId).ToList();

        //    foreach (var storeDetail in storeDetails)
        //    {
        //        foreach (var deliverItemCustomModel in deliverItemCustomModels)
        //        {
        //            if (deliverItemCustomModel.ProductId == storeDetail.ProductId)
        //            {
        //                deliverItemCustomModel.AvailableQty = storeDetail.Qty;
        //                deliverItemCustomModel.DeliveredQty = storeDetail.Qty;
        //                deliverItemCustomModel.RemainingQty = 0;
        //                if (deliverItemCustomModel.OrderQty > storeDetail.Qty)
        //                {
        //                    deliverItemCustomModel.DeliveredQty = deliverItemCustomModel.OrderQty;
        //                    deliverItemCustomModel.RemainingQty = deliverItemCustomModel.OrderQty - storeDetail.Qty;
        //                }
        //                deliverItemCustomModel.DeliveryAmount = deliverItemCustomModel.DeliveredQty * deliverItemCustomModel.OrderUnitPrice;
        //            }
        //        }
        //    }

        //    return deliverItemCustomModels;
        //}

        //public List<DeliverItemCustomModel> GetDeliverItems(long orderDeliverId)
        //{

        //    OrderDeliver orderDeliver = OrderDeliverRepository.OrderDelivers.Where(x => x.OrderDeliverId == orderDeliverId).FirstOrDefault();


        //    List<DeliverItemCustomModel> deliverItemCustomModels = orderDetailRepository.OrderDetails.Include("Product").Where(x => x.OrderMasterId == orderDeliver.OrderMasterId).Select(x =>
        //    new DeliverItemCustomModel
        //    {

        //        ProductId = x.ProductId,
        //        ProductName = x.Product.ProductName,
        //        OrderQty = x.Qty,
        //        OrderUnitPrice = x.UnitPrice
        //    }).ToList();



        //    List<StoreDetail> storeDetails = StoreDetailRepository.StoreDetails.Include("Store").Where(x => x.Store.StockInfoId == orderDeliver.StockInfoId).ToList();

        //    foreach (var storeDetail in storeDetails)
        //    {
        //        foreach (var deliverItemCustomModel in deliverItemCustomModels)
        //        {
        //            if (deliverItemCustomModel.ProductId== storeDetail.ProductId)
        //            {
        //                deliverItemCustomModel.AvailableQty = storeDetail.Qty;
        //                deliverItemCustomModel.DeliveredQty = storeDetail.Qty;
        //                deliverItemCustomModel.RemainingQty = 0;
        //                if (deliverItemCustomModel.OrderQty>storeDetail.Qty)
        //                {
        //                    deliverItemCustomModel.DeliveredQty = deliverItemCustomModel.OrderQty;
        //                    deliverItemCustomModel.RemainingQty = deliverItemCustomModel.OrderQty- storeDetail.Qty;
        //                }
        //                deliverItemCustomModel.DeliveryAmount = deliverItemCustomModel.DeliveredQty * deliverItemCustomModel.OrderUnitPrice;
        //            }
        //        }
        //    }

        //    return deliverItemCustomModels;
        //}

        //public List<OrderDetailModel> GetOrderDetailsByOrderMasters(long orderMasterId)
        //{
        //    IQueryable<OrderDetail> orderDetails = orderDetailRepository.OrderDetails.Include("Product").Where(x =>x.OrderMasterId== orderMasterId).AsQueryable();
        //    return ObjectConverter<OrderDetail, OrderDetailModel>.ConvertList(orderDetails.ToList()).ToList();
        //}
    }
}
