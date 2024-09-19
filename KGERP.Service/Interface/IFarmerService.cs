using KGERP.Service.ServiceModel;
using System.Linq;

namespace KGERP.Service.Interface
{
    public interface IFarmerService
    {
        IQueryable<FarmerModel> GetFarmers(int companyId, string searchValue, out int count);
        FarmerModel GetFarmer(int id);
        bool SaveFarmer(long id, FarmerModel farmer);
        bool DeleteFarmer(int id);
    }
}
