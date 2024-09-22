using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IEmployeeLoanService : IDisposable
    {
        List<EmployeeLoanModel> GetEmployeeLoans(string searchText);
        EmployeeLoanModel GetEmployeeLoan(int id);
        List<EmployeeLoanModel> GetEmployeeLoanEvent();
        List<EmployeeLoanModel> GetCompanyBaseCaseList(int companyId);
        List<EmployeeLoanModel> GetPrevious7DaysCaseSchedule();
        bool SaveEmployeeLoan(int id, EmployeeLoanModel model);
        bool DeleteEmployeeLoan(int id);
    }
}
