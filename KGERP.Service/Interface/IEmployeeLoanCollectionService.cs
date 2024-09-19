using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IEmployeeLoanCollectionService : IDisposable
    {
        List<EmployeeLoanCollectionModel> GetEmployeeLoanCollections(string searchText);
        EmployeeLoanCollectionModel GetEmployeeLoanCollection(int id);
        List<EmployeeLoanCollectionModel> GetEmployeeLoanCollectionEvent();
        List<EmployeeLoanCollectionModel> GetCompanyBaseCaseList(int companyId);
        List<EmployeeLoanCollectionModel> GetPrevious7DaysCaseSchedule();
        bool SaveEmployeeLoanCollection(int id, EmployeeLoanCollectionModel model);
        bool DeleteEmployeeLoanCollection(int id);
    }
}
