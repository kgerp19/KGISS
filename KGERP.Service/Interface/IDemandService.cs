using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IDemandService : IDisposable
    {
        Task<DemandModel> GetDemandList(int companyId, DateTime? fromDate, DateTime? toDate);
        List<DemandModel> GetDemands(int companyId, string searchDate, string searchText);
        DemandModel GetDemand(long id);
        bool SaveDemand(long id, DemandModel demand, out string message);
        List<SelectModel> GetDemandSelectModels(int companyId);
        string GetNewDemandNo(string demandDate);
        List<DemandItemModel> GetDemandItems(long demandId);
        List<DemandItemDetailModel> GetDemandItemDetails(long demandId);
    }
}
