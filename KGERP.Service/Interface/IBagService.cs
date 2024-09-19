using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IBagService : IDisposable
    {
        Task<BagModel> GetBags(int companyId);
        List<SelectModel> GetBagWeightSelectModels(int companyId);
        BagModel GetBag(int bagId);
        decimal GetBagWeightByBagId(int bagId);
    }
}
