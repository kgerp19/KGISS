using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IEmployeeOperationService : IDisposable
    {
        List<EmployeeOperationModel> GetEmployeeOperations(string searchText);
        List<SelectModel> GetEmployeeOperationEmployees();
        EmployeeOperationModel GetEmployeeOperation(int id);
        List<EmployeeOperationModel> GetEmployeeOperationEvent();
        List<EmployeeOperationModel> GetPrevious7DaysOperationSchedule();
        bool SaveEmployeeOperation(int id, EmployeeOperationModel model);
        bool DeleteEmployeeOperation(int id);
    }
}
