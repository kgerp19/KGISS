using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation.Leave.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface ILeaveApplicationService
    {
        Task<LeaveApplicationVm> GetLeaveApplicationByEmployee(DateTime? fromDate, DateTime? toDate);
        LeaveApplicationModel GetLeaveApplication(long id);
        int LeaveCalculation();
        bool SaveLeaveApplication(long leaveApplicationId, LeaveApplicationModel leaveApplication, out string message);
        bool SaveLeaveApplicationOld(long leaveApplicationId, LeaveApplicationModel leaveApplication, out string message);
        List<LeaveBalanceCustomModel> GetLeaveBalance();
        List<LeaveBalanceCustomModel> GetLeaveBalance(long employeeId);
        List<LeaveApplicationModel> GetManagerLeaveApprovals(string searchText);
        List<LeaveApplicationModel> GetHRLeaveApprovals(string searchText);
        bool ChangeMangerStatus(long leaveApplicationId, string status, string comments, string ip);
        bool ChangeHRStatus(long leaveApplicationId, string status, string comments, string ip);
       bool UpdateLeaveStatus(long leaveApplicationId, LeaveStatusEnum status, string comments, string ip, ApprovarTypeEnum approvar, out string message);
        Task<TeamLeaveBalanceCustomModel> GetTeamLeaveBalance(long managerId, int selectedYear);
        Task<IEnumerable<TeamLeaveBalanceCustomModel>> GetCompanywiseLeaveBalance(int companyId, int departmentId, int selectedYear);
        IEnumerable<LeaveBalanceCustomModel> GetEmployeeLeaveBalance(string employeeId, out string message);
        IEnumerable<LeaveBalanceCustomModel> GetEmployeeLeaveBalanceByIdDateRange(string employeeId, DateTime startDate, DateTime endDate, out string message);
        EmployeeCustomModel GetCustomEmployeeModel(string employeeId);
        string ProcessLeave();
        LeaveApplicationModel GetLeaveApplicationByOther(long id, long empId);
        List<LeaveBalanceCustomModel> GetLeaveBalanceByOther(long id);
        bool SaveOtherLeaveApplication(int id, LeaveApplicationModel leaveApplication, long empId, out string message);
        List<LeaveApplicationModel> GetLeaveApplicationsByOther(string searchText,int pageSize=10, int pageNo=1);
        LeaveApplicationVm ManagerAndHrLook(long EmpId);
       bool LeaveSignatoryAssign(LeaveApplicationVmm viewModel);
        bool RemoveSignatory(long signatoryId);
        #region leaveSuffixPrefix
        SuffixPrefixResult isSuffixPrefix(DateTime startingDate, DateTime endingDate, long empId);
        bool isUnapprovedApplication(long empId);
        #endregion

        #region Future Removal - Code clean
        Task<LeaveApplicationVm> GetLeaveApplicationByEmployeeNew(DateTime? fromDate, DateTime? toDate);
        List<ManagerApproval> GetManagerLeaveApprovalsNew(string searchText);
        #endregion

        #region leaveSignatoryRequisitionAndApproval
        bool SignatoryApprovalSave(LeaveApplicationModel leaveApplication);
        bool SignatoryAutoApprovalSave(LeaveApplicationModel model);
        Task<LeaveAllDetailVM> GetLeaveApprovalRelatedAllInfo(long SigId, DateTime? fromDate, DateTime? toDate);
        Task<SignatoryApprovalDetails> GetSignatoriesApprovalInfo(long applicationId, long sigId);
        int DoLeaveApproval(LeaveAllDetailVM vm);
        Task<LeaveApplicationVm> GetLeaveListByEmployee(long EmpID, DateTime? fromDate, DateTime? toDate);
        #endregion
        #region check manager change and sig update
        bool CheckNewManagerAddedInSignatory();
        #endregion

    }
}
