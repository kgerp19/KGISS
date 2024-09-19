using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.ApprovalSystemService
{
    public interface IApproval_Service
    {
        Task<ApprovalSystemViewModel> AddApproval(ApprovalSystemViewModel model);
        Task<ApprovalSystemViewModel> AddSalarySheetApproval(ApprovalSystemViewModel model);
        Task<bool> CheckApproval(ApprovalSystemViewModel model);
        Task<bool> CheckApprovalsalarySheet(ApprovalSystemViewModel model);
        Task<ApprovalSystemViewModel> ApprovalList(int companyId,int Years, int Month);
        Task<ApprovalSystemViewModel> SalarySheetApprovalList(int companyId);
        Task<ApprovalSystemViewModel> SalarySheetList(ApprovalSystemViewModel model2);
        Task<ApprovalSystemViewModel> ApprovalforEmployeeList(long Employee ,int companyId,int Years, int Month);
        Task<ApprovalSystemViewModel> Approvalformanagment(ApprovalSystemViewModel model);
        Task<ApprovalSystemViewModel> SalarySheetApprovalformanagment(ApprovalSystemViewModel model);
        Task<ApprovalSystemViewModel> AduitApprovalformanagment(ApprovalSystemViewModel model);
        Task<ApprovalSystemViewModel> AccountingApprovalList(ApprovalSystemViewModel model);
        Task<ApprovalSystemViewModel> ApprovalSignetory(long id);
        Task<ApprovalSystemViewModel> SalarySheetApprovalSignetory(long id);
        Task<long> AccApprovalStutasUpdate(long id);
        Task<long> SalaryAccApprovalStutasUpdate(long id);
        Task<ApprovalSystemViewModel> AccStatusChange(ApprovalSystemViewModel model);
        Task<ApprovalSystemViewModel> SalarySheetAccStatusChange(ApprovalSystemViewModel model);
        Task<long> ApprovalDelete(long id);
        Task<List<SelectDDLModel>> ReportcatagoryLit(int companyId);
        List<SelectModel> YearsListLit();
    }
}
