using KGERP.Data.CustomModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IRequisitionSignatoryService
    {
        /// <summary>
        /// Get all signatories with optional filters
        /// </summary>
        Task<RequisitionSignatoryViewModel> GetAllSignatories(int companyId, string integrateWith = null);

        /// <summary>
        /// Get signatory by ID
        /// </summary>
        Task<RequisitionSignatoryViewModel> GetSignatoryById(long id,int companyId);

        /// <summary>
        /// Save (Create or Update) signatory
        /// </summary>
        Task<bool> SaveSignatory(RequisitionSignatoryViewModel model);

        /// <summary>
        /// Delete signatory
        /// </summary>
        Task<bool> DeleteSignatory(long id);

        /// <summary>
        /// Replace approver globally for all employees
        /// </summary>
        Task<bool> ReplaceApprover(ReplaceApproverViewModel model);

        /// <summary>
        /// Validate that an employee has exactly 4 signatories
        /// </summary>
        Task<bool> ValidateFourSignatories(long employeeId, string integrateWith);
        Task<string> getSignatoryDesignation(long signatoryId);

        /// <summary>
        /// Get count of signatories for an employee
        /// </summary>
        Task<int> GetSignatoryCount(long employeeId, string integrateWith);

        /// <summary>
        /// Get distinct module names from IntegrateWith field
        /// </summary>
        List<SelectModel> GetModelNames(int companyId);

        /// <summary>
        /// Get companies accessible by current user
        /// </summary>
        List<SelectModel> GetCompaniesByUser(int companyId);


        /// <summary>
        /// Get departments by division
        /// </summary>
        List<SelectModel> GetDepartmentsByDivision(int divisionId);

        /// <summary>
        /// Get departments by company
        /// </summary>
        List<SelectModel> GetDepartmentsByCompany(int companyId);

        /// <summary>
        /// Get sections by department
        /// </summary>
        List<SelectModel> GetSectionsByDepartment(int departmentId);

        /// <summary>
        /// Get designations by department
        /// </summary>
        List<SelectModel> GetDesignationsByDepartment(int departmentId);

        /// <summary>
        /// Get employees by filters
        /// </summary>
        List<SelectModel> GetEmployeesByFilters(int? companyId = null, int? divisionId = null, int? departmentId = null, int? designationId = null);

        /// <summary>
        /// Get employee details for Replace Approver
        /// </summary>
        Task<ReplaceApproverViewModel> GetEmployeeDetailsForReplace(long employeeId);
        /// <summary>
        /// Get categories
        /// </summary>
        List<SelectModel> GetCategories();

        /// <summary>
        /// Get signatories for DataTable with server-side processing
        /// </summary>
        Task<object> GetSignatoriesForDataTable(
            int start,
            int length,
            string searchValue,
            string sortColumn,
            string sortDirection,
            long? employeeId,
            int? companyId,
            int? divisionId,
            int? departmentId,
            int? designationId,
            string integrateWith);



    }
}
