using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface ICompanyUserMenuService
    {
        CompanyUserMenuModel GetCompanyUserMenu(int id);
        List<CompanyUserMenuModel> GetCompanyUserMenus(string searchText);
        // Filtered search: by userId, companyId, menuId, subMenuId
        List<CompanyUserMenuModel> GetCompanyUserMenus(string userId, int? companyId, int? menuId, int? subMenuId);
        bool SaveCompanyUserMenu(long id, CompanyUserMenuModel companyUserMenu, out string message);
        bool DeleteCompanyUserMenu(int? id);
    }
}
