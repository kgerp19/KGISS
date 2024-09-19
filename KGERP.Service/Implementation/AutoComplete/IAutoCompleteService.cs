using KGERP.Service.Implementation.AutoComplete.ViewModels;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.AutoComplete
{
    public interface IAutoCompleteService
    {
        Task<IEnumerable<AutoCompleteVM>> GetAllEmployeeAutoComplete(string prefix = null);
        Task<IEnumerable<AutoCompleteVM>> TaskGetAllEmployeeAutoComplete(string prefix = null, int workspaceid = 0);
        Task<IEnumerable<AutoCompleteVM>> GetAllCompanyAutoCompleteAsync(string prefix = null);
        Task<IEnumerable<AutoCompleteVM>> GetAllProjectList(int? companyId);
        Task<IEnumerable<AutoCompleteVM>> GetAllDepartmentAutoCompleteAsync(string prefix = null);
        Task<IEnumerable<AutoCompleteVM>> GetAllGRQSProductCategoryAutoCompleteAsync(string prefix = null);
        Task<EmployeeVm> GetEmployee(int employeeId);
    }
}
