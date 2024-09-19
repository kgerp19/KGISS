using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IStockInfoService : IDisposable
    {
        Task<StockInfoModel> GetStockInfos(int companyId);
        Task<int> StockInfoAdd(StockInfoModel model);
        Task<int> StockInfoEdit(StockInfoModel model);
        Task<int> StockInfoDelete(int id);
        List<SelectModel> GetStockInfoSelectModels(int companyId);
        List<SelectModel> GetFactorySelectModels(int companyId);
        List<SelectModel> GetDepoSelectModels(int companyId);
        List<SelectModel> GetKFMALDepoSelectModels(int companyId);
        List<SelectModel> GetStoreSelectModels(int companyId);
        string StocName(int stockId);
        List<SelectModel> GetALLStoreSelectModels(int companyId);
        List<SelectModel> GetALLZoneSelectModels(int companyId);
    }
}
