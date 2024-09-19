using KGERP.Service.Implementation;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IOrderMasterService
    {
        Task<OrderMasterModel> GetOrderMasters(int companyId, DateTime? fromDate, DateTime? toDate, string productType);
        Task<OrderMasterModel> GetOrderMastersFeed(int companyId, DateTime? fromDate, DateTime? toDate, string productType,int Uid);
        bool SaveOrder(long orderMasterId, OrderMasterModel model, out string message);
        long FeedSaveSalesOrder(long orderMasterId, OrderMasterModel model);
        List<SelectModel> GetOrderMasterSelectModels();
        OrderMasterModel GetOrderMaster(long orderMasterId);
        OrderMasterViewModel GetFeedOrderMaster(long orderMasterId, int Uid);
        Task<OrderMasterViewModel> GetFeedOrderMasterDetails(long id);
        VendorModel GetCustomerInfo(long custId);
        ProductModel GetProductUnitPrice(int pId);
        decimal GetProductAvgPurchasePrice(int pId);
        DepotCurrentStockModel GetDepotCurrentStockByProduct(int productId, int companyId, int stockInfoId);
        OrderMasterModel GetOrderInforForEdit(long masterId);
        List<OrderDetailModel> GetOrderDetailInforForEdit(long masterId);
        long GetOrderNo();
        Task<OrderMasterModel> GetOrderDelivers(int companyId, DateTime? fromDate, DateTime? toDate, string productType);
        bool DeleteOrder(long orderMasterId);
        ProductModel GetProductUnitPriceCustomerWise(int pId, int vendorId);
        string GetNewOrderNo(int companyId, int stockInfoId, string productType);
        bool DeleteOrderDetail(long orderDetailId, out string productType);
        bool UpdateOrder(OrderMasterModel model, out string message);
        bool CheckDepoOrder(long orderMasterId);
        bool SupportDeleteOrderByOrderNo(int companyId, string orderNo);
        VendorOfferModel GetProductConfig(int pId, int vendorId, int stockInfoId);
        Task<OrderMasterModel> GetChallanPrintOD(int companyId, DateTime? fromDate, DateTime? toDate);
        (long, bool) UpdateSeedAttrOrderDeliver(long id, bool isSeen);
        VendorModel GetfeedCustomer(int CompanyId);
        VMFeedPayment GetPaymentShortList(int VendorId);
        VMFeedPayment GetShortList(int CompanyId);
        Task<OrderMasterModel> GetDeliveredOrderList(int companyId, DateTime? fromDate, DateTime? toDate, string productType);
    }
}
