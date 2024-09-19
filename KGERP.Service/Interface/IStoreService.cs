using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IStoreService : IDisposable
    {
        Task<StoreModel> GetStores(int companyId, DateTime? fromDate, DateTime? toDate, string type);
        Task<long> SubmitFeedPurchaseByProduct(StoreModel model);
        StoreModel GetStore(long id, string productType);
        bool SaveStore(long id, StoreModel store);
        Task<long> FeedSaveStore(long id, StoreModel model);
        bool ProductionBulkUpdate(RequisitionModel model);
        Task<SoreProductQty> GetStoreProductQty(int companyId);
        List<SoreProductQty> GetRMStoreProductQty();
        List<SoreProductQty> GetEcomProductStore();
        StoreModel GetOpenningStore(long id);
        bool  StoreUpdateAfterProduction(StoreModel store, List<RequisitionItemModel> requistionItems, out string message);
        bool SaveRMStore(long storeId, StoreModel store);
        List<StoreModel> GetFeedPurchaseList(DateTime? searchDate, string searchText, int companyId, string productType);

        Task<StoreModel> GetFeedPurchaseList(int companyId, DateTime? fromDate, DateTime? toDate, string type);
        Task<StoreModel> GetFeedPurchase(long id, string productType, int companyId);
        StoreModel GetRequisitionItemStore(int id, string productType);

        RequisitionModel FeedRequisitionPushGet(int companyId, long requisitionId);

    }
}
