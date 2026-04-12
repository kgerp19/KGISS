using KGERP.Service.ServiceModel;
using System;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IApplicationManageService
    {
        Task<long> SaveOrderCreditLimitApplication(ApplicationManageModel model, string username, long employeeId);
        Task<ApplicationManageModel> GetOrderCreditLimitApplications(int companyId, long applicationId = 0);
        Task<ApplicationManageModel> GetPendingApprovals(int companyId, long employeeId);
        Task<bool> UpdateApprovalStatus(long approvalId, int status, string comment, string username);
        Task<ApplicationManageModel> GetOrderCreditLimitManageList(int companyId, int? searchStatus, DateTime? fromDate, DateTime? toDate);
    }
}
