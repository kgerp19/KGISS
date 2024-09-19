using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IOrderDeliverService
    {
        List<OrderDeliverModel> GetOrderDelivers(string searchText);
        OrderDeliverModel GetOrderDeliver(long id);
        OrderDeliverModel SaveOrderDeliver(long id, OrderDeliverModel orderDeliver, out string message);
        OrderDeliverModel GetOrderDeliverWithInclude(long orderDeliverId);
        List<DeliverItemCustomModel> GetDeliverItems(long orderMasterId, int stockInfoId, int companyId, string productType);
        string GetNewChallanNoByCompany(int companyId, string productType);
        string GetLastInvoceNoByCompany(int companyId);
        string GetGeneratedInvoiceNoByStockInfo(int stockInfoId);
        List<OrderDeliveryPreview> SaveOrderDeliverPreview(int id, List<OrderDeliveryPreview> previews);
        List<SelectModel> GetInvoiceSelectList(int companyId);
        ProductDetailModel GetProductDetails(string engineNo);
        object GetInvoiceNoAutoComplete(int customerId, string prefix, int companyId);
    }
}
