using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;


namespace KGERP.Service.Interface
{
    public interface ICompanyMenuService
    {
        List<CompanyMenuModel> GetCompanyMenus(string searchText);
        CompanyMenuModel GetCompanyMenu(int id);
        bool SaveCompanyMenu(int id, CompanyMenuModel companyMenu);
        List<SelectModel> GetCompanyMenuSelectModelsByCompanyId(int? companyId);
        List<SelectModel> GetCompanyMenuSelectModelsByCompany(int companyId);
        //ProductSubCategoryModel GetProductSubCategory(int id);
        //bool SaveProductSubCategory(int id, ProductSubCategoryModel model);
        //bool DeleteProductSubCategory(int id);
        //List<SelectModel> GetProductSubCategorySelectModelsByProductCategory(int productCategoryId);

    }
}
