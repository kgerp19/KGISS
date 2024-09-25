using KGERP.Service.Implementation.EmployeeLogService;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IEmployeeService : IDisposable
    {
        Task<EmployeeVm> GetEmployees(int companyId);
        Task<EmployeeVm> GetKSSLEmployees(int CompanyId);
        Task<EmployeeVm> GetEmployeesCompanyWise(int CompanyId);
        Task<EmployeeVm> GetAllEmployeesWise();
        EmployeeModel GetEmployee(long id);
        EmployeeModel GetEmployeeForKSSL(long id, int CmpanyId);
        EmployeeModel GetEmployeeForAcc(long id);
        EmployeeModel GetEmployee(string employeeId);
        bool CheckEmployee(long id);
        EmployeeModel GetEmployeeByKGID(string employeeId);
        bool SaveEmployee(long id, EmployeeModel employee);
        bool DeleteEmployee(long id);
        List<SelectModel> GetEmployeeSelectModels();
        List<SelectModel> GetEmployeeSelectModelsISS(int companyId);
        List<EmployeeModel> EmployeeSearch(string searchText);
        //EmployeeViewModel GetSignatureByID(long Id);
        List<EmployeeModel> EmployeeSearch();
        List<EmployeeModel> EmployeeSearchByCoId(int? companyid);
        List<EmployeeModel> GetProbitionPreiodEmployeeList();
        bool savesignature(EmployeeModel vm);
        bool saveAccountNumber(EmployeeModel vm);
        List<EmployeeModel> GetEmployeeEvent();
        Task<List<EmployeeModel>> GetEmployeesAsync(bool employeeType, string searchText);
        object GetEmployeeAutoComplete(string prefix);
        object GetEmployeeAutoCompleteByCompany(string prefix,int CompanyId );
        object GetEmployeeAutoCompleteOfficer(string prefix,int CompanyId);
        List<EmployeeModel> GetTeamMembers(string searchText);
        EmployeeModel GetTeamMember(long id);
        bool UpdateTeamMember(EmployeeModel model);
        List<EmployeeModel> GetEmployeeAdvanceSearch(int? departmentId, int? designationId, string searchText);
        List<EmployeeModel> GetEmployeeTodayEvent();
        long GetIdByKGID(string kgId);
        object GetEmployeeDesignationAutoComplete(string prefix);
        List<SelectModel> GetEmployeesForSmsByCompanyId(int companyId = 0,int departmentId=0);

        Task<VmPayRoll> GetMySalaery(int myCompanyId);
        Task<VmPayRollDetail> GetMySalaeryByPayrollId(long payRollId, long myEmployeeId);


        #region exitinterview
        ExitInterviewVM GetExitInterView(int id);
       IEnumerable<ExitInterviewVM> GetAllExitInterView(string employeeId);
       bool SaveExitInterView(ExitInterviewVM modelVM);

      bool UpdateExitInterView(ExitInterviewVM modelVM);

      bool DeleteExitInterView(int id);

    bool SaveClearanceSignatory(BusinessHeadModel model);
    bool SaveClearanceSignatoryMapping(int exitInterviewId);
     bool UpdateClearanceSignatory(BusinessHeadModel model);
     IEnumerable<RequisitionApprovalVM> GetAllExitInterviewApprovalList(int id);
    bool RemoveClearanceSignatory(int id);
    BusinessHeadModel GetClearanceSignatoryById(int id);
    IEnumerable<BusinessHeadModel> GetClearanceSignatory(int? companyId, int? departmentId);
    IEnumerable<EmployeeClearanceVM> GetClearanceApprovalData(DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? status);
    bool UpdateClearanceSignatoryApprovalStatus(long id, SignatoryStatusEnum status, string comment);

       #endregion exitinterview
    }
}


