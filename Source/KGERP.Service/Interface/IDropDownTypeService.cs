using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IDropDownTypeService
    {
        List<DropDownTypeModel> GetDropDownTypes(string searchText);
        DropDownTypeModel GetDropDownType(int id);
        bool SaveDropDownType(int id, DropDownTypeModel model, out string message);
        bool DeleteDropDownType(int id);
        List<SelectModel> GetDropDownTypeSelectModels();
        Task<DropDownTypeModel> GetDropDownTypes(int companyId);
        List<SelectModel> GetDropDownTypeSelectModelsByCompany(int companyId);
    }
}
