using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IUnitService
    {
        List<UnitModel> GetUnits();
        List<SelectModel> GetUnitSelectModels(int companyId);
    }
}
