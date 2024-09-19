using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Interface
{
    public interface IKttlCustomerService : IDisposable
    {
        List<KttlCustomerModel> GetKttlCustomers(string searchText);
        List<KttlCustomerModel> GetKttlCustomerSchedule();
        List<KttlCustomerModel> Previous7DaysClientSchedule();
        List<int> GetServeiceYear();
        KttlCustomerModel GetKttlCustomer(int id); 
        bool SaveKTTLCustomerData(int id, KttlCustomerModel model);
        bool DeleteKttlCustomer(int id);
        List<SelectModel> GetKTTLEmployees();
        object LoadCustomerDataList();
        IQueryable<KttlCustomerModel> GetCustomers(int companyId, string searchValue, out int count);
        #region For GOL
        List<SelectModel> GetGOLEmployees();
        #endregion
    }
}
