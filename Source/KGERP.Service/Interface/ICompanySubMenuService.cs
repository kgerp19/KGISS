using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface ICompanySubMenuService
    {
        CompanySubMenuModel GetCompanySubMenu(int id);
        List<CompanySubMenuModel> GetCompanySubMenus(string searchText);
        bool SaveCompanySubMenu(int id, CompanySubMenuModel companySubMenu);
        List<SelectModel> GetCompanySubMenuSelectModelsByCompanyMenu(int menuId);
        bool DeleteCompanySubMenu(int? id);
    }
}
