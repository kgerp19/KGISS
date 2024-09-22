using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IPurchaseOrderService : IDisposable
    {
        Task<PurchaseOrderModel> GetPurchaseOrders(int companyId, DateTime? fromDate, DateTime? toDate);

        Task<PurchaseOrderModel> GetPurchaseOrder(int companyId, long purchaseOrderId);
        List<PurchaseOrderDetailModel> GetPurchaseOrderDetails(long demandId, int companyId);
        PurchaseOrderModel GetPurchaseOrderWithInclude(int purchaseOrderId);
        List<StoreDetailModel> GetQCItemList(long purchaseOrderId, int companyId);
        List<PurchaseOrderModel> GetQCPurchaseOrders(int companyId, DateTime? searchDate, string searchText);
        List<PurchaseOrderDetailModel> GetPurchaseOrderItems(long purchaseOrderId);
        long SavePurchaseOrder(long purchaseOrderId, PurchaseOrderModel purchaseOrder);
        PurchaseOrderDetailModel GetPurchaseOrderItemInfo(long demandId, int productId);
        List<MaterialReceiveDetailModel> GetPurchaseOrderItems(long purchaseOrderId, int companyId);
        string GetPurchaseOrderTemplateReportName(long purchaseOrderId);
        bool DeletePurchaseOrder(long purchaseOrderId, out string message);
        bool CancelPurchaseOrder(long purchaseOrderId, PurchaseOrderModel purchaseOrder);
        List<SelectModel> GetOpenedPurchaseByVendor(int vendorId);
    }
}
