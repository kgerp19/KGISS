using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface ICompanyUserMenuService
    {
        CompanyUserMenuModel GetCompanyUserMenu(int id);
        List<CompanyUserMenuModel> GetCompanyUserMenus(string searchText);
        bool SaveCompanyUserMenu(long id, CompanyUserMenuModel companyUserMenu, out string message);
        bool DeleteCompanyUserMenu(int? id);
    }
}
