using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IKttlService : IDisposable
    {
        List<KttlServiceModel> GetKttlServices(string searchText);
        KttlServiceModel GetKttlService(int id);
        KttlCustomerModel GetKttlCustomer(int id);
        bool SaveKttlService(int id, KttlServiceModel model);
        bool DeleteKttlService(int id);
    }
}
