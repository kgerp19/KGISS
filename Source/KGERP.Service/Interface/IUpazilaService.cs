using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IUpazilaService
    {
        Task<UpazilaModel> GetUpazilas();
        List<UpazilaModel> GetUpazilas(string searchText);

        UpazilaModel GetUpazila(int id);
        bool SaveUpazila(int id, UpazilaModel model);
        bool DeleteUpazila(int id);
        List<SelectModel> GetUpazilaSelectModelsByDistrict(int districtId);

        string GetUpazilaCodeByDistrict(int districtId);
        List<SelectModel> GetUpzilaSelectModels();
        List<SelectModel> GetUpzilaByDistrictName(string name);
        List<SelectModel> GetUpzilaByDistrictId(int? districtId);
    }
}
