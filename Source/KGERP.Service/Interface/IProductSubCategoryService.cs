using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;


namespace KGERP.Service.Interface
{
    public interface IProductSubCategoryService
    {
        List<ProductSubCategoryModel> GetProductSubCategories(int companyId, string type, string searchText);
        ProductSubCategoryModel GetProductSubCategory(int id, string productType);
        bool SaveProductSubCategory(int id, ProductSubCategoryModel model);
        bool DeleteProductSubCategory(int id);
        List<SelectModel> GetProductSubCategorySelectModelsByProductCategory(int productCategoryId);
        List<SelectModel> GetBasicAndAdditiveMaterialSelectModels(int companyId);
    }
}
