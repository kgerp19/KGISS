using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.TaskManagment
{
    public interface ITaskManagementEvolutionService
    {
        TaskManagementEvolutionVM TaskAdd(TaskManagementEvolutionVM task);
        Task<bool> DeleteWorkBord(long id);
        Task<long> AddWorkBoard(WorkBoard model);
        Task<TaskManagementEvolutionVM> GetAllTask(long EmployeeId);
        Task<TaskManagementEvolutionVM> GetAllTaskBywork(long EmployeeId,long? WorkbordId);
        Task<WorkSpacesMemberVM> GetWorkSpacemember(long id);
        Task<WorkSpaceVm> GetAllWorkspase(long EmployeeId);
        Task<long> AddWorkspace(WorkSpaceVm model);
        Task<WorkBoardVm> GetWorkBoard(long WorkSpaceId);
        bool WorkAssignSerFoorBorad(long[] selectedInstallments, long WorkSpaceId);
        Task<bool> DeleteWorkSpacee(long id);
        bool DeleteWorkMember(long WorkSpacesMemberId);
        Task<VMWorkLabelAndSpace> GetWorkLebel(long id);
        Task<bool> SaveAttachment(HttpPostedFileBase file, long wrkid);
        Task<bool> Deleteattachment(long Attatchmentid);
        Task<bool> DeleteaWoklabel(long lavelid);
        bool UpdateEndDateee(long workid, DateTime datePicker1);
        bool AdminMake(long WorkSpacesMemberId, bool isChecked,long workspaceid);
            Task<TaskManagementEvolutionVM> GetTaskForUpdate(int? id);
        int TaskDelete(int id);
        Task<bool> editModal(string newTitle, long buttonValue);
        bool WatchModel(long workId);
        Task<TaskManagementEvolutionVM> GetAllTaskBYajax(long WorkId, int? workboardid);
        bool WatchModelSeennUnseen(long workId);
        bool WorkAssignSer(long[] selectedInstallments, long workId);
        bool RemoveWorkAssign(int workMemberId);
        bool ChangeStaeValue(int selectedValue, long workid);
        bool ChangeDescription(string description, long workid);
        bool CommentSave(string Comment, long workid);
        bool CommentUpdate(string newText, long workQAId);
        bool DeleteComent(long workQAId);
        bool ChangeWorkLabelValue(int selectedValue, long workid);
        bool LikeGive(long WorkQid);
        Task<List<WorkComentLikVm>> LikePeople(long WorkQId);
        //Task<TaskManagementEvolutionVM> GetReport(int CompanyId);
        Task<List<SelectWorkState>> GetSelectWorkState();
        Task<TaskManagementEvolutionVM> GetWorkstate(int workboardid);
        Task<List<SelectWorkLebel>> GetSelectWorkLebel();
    }
}
