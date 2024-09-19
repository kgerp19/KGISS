using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;


namespace KGERP.Service.Interface
{
    public interface IStoreDetailService : IDisposable
    {
        IEnumerable<StoreDetailModel> GetStoreDetails(long storeId);
        StoreDetailModel GetStoreDetail(long id, long storeId);
        bool SaveStoreDetail(long id, StoreDetailModel storeDetail, out string message);
        bool DeleteStoreDetail(long storeDetailId);

        //Task<StoreDetailModel> SaveStoreDetail(int id, StoreDetailModel model);
    }
}
