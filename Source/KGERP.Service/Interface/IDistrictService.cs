using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IDistrictService
    {

        List<SelectModel> GetCountriesSelectModels();
        List<SelectModel> GetDivisionSelectModels();
        List<SelectModel> GetDistrictSelectModels();
        Task<DistrictModel> GetDistricts();
        DistrictModel GetDistrict(int id);
        bool SaveDistrict(int districtId, DistrictModel model);
        bool DeleteDistrict(int id);
        List<SelectModel> GetDivisionByName(string name);
        List<SelectModel> GetDistrictByDivisionName(string name);
        List<SelectModel> GetDistrictByDivisionId(int? divisionId);
    }
}
