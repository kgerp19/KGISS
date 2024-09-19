using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface ILandNLegalService : IDisposable
    {
        List<LandNLegalModel> GetLandNLegals(string searchText);
        List<SelectModel> GetLandNLegalEmployees();
        LandNLegalModel GetLandNLegal(long id);
        List<LandNLegalModel> GetLandNLegalEvent();
        List<LandNLegalModel> GetCompanyBaseCaseList(int companyId);
        List<LandNLegalModel> GetPrevious7DaysCaseSchedule();
        bool SaveLandNLegal(long id, LandNLegalModel model);
        bool DeleteLandNLegal(long id);
    }
}
