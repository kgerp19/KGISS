using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IWorkAssignService
    {
        WorkAssignModel GetWorkAssign(int id);
        bool SaveWorkAssignFileList(List<WorkAssignFileModel> workAssignFileModels);
        List<WorkAssignFileModel> GetFiles(long workAssignId);
        string GetFileName(long workAssingFileId);
    }
}
