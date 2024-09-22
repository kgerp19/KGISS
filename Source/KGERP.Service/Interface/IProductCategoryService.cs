using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IProductCategoryService : IDisposable
    {
        List<ProductCategoryModel> GetProductCategories(int companyId, string type, string searchText);
        ProductCategoryModel GetProductCategory(int id, string type);
        bool SaveProductCategory(int id, ProductCategoryModel model);
        bool DeleteProductCategory(int id);
        List<SelectModel> GetProductCategorySelectModelByCompany(int companyId, string type);
    }
}
