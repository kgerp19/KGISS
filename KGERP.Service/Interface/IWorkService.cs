using KGERP.Data.CustomModel;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IWorkService
    {
        List<WorkModel> GetWorks(string searchText);
        WorkModel GetWork(int id);
        bool SaveWork(int id, WorkModel work);
        List<WorkAssignModel> GetWorkAssigns(int workId);
        bool SaveWorkAssign(int id, WorkAssignModel model);
        List<SelectModel> GetAssignMemberSelectModels(int workId);
        WorkAssignModel GetWorkAssign(int workId);
        bool DeleteMember(int workAssignId);
        bool DeleteWork(int workId);
        List<WorkCustomModel> GetManagerWorks();
        List<WorkAssignModel> GetEmployeeWorks();
        bool ChangeMemberState(WorkAssignModel model);
        List<WorkMemberModel> GetWorkMembers(string searchText);
        WorkMemberModel GetWorkMember(int id);
        bool SaveWorkMember(int id, WorkMemberModel model);
        bool DeleteWorkMember(int id);
    }
}
