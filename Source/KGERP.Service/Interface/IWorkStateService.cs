using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IWorkStateService
    {
        List<WorkStateModel> GetWorkStates();
        List<SelectModel> GetManagerWorkStateSelectModels();
        WorkStateModel GetWorkState(int id);
        bool SaveWorkState(int id, WorkStateModel model);
        List<SelectModel> GetMemberWorkStateSelectModels(int id);
    }
}
