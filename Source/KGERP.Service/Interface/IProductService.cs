using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Interface
{
    public interface IProductService
    {
        Task<ProductPriceModel> GetLastUpdatedProductTpPrice(int companyId,string priceType);
        Task<ProductPriceModel> GetLastUpdatedProductSalePrice(int companyId,string priceType);
        Task<ProductPriceModel> GetProductTpPrice(int companyId, int productId, string priceType);
        Task<ProductPriceModel> GetProductSalePrice(int companyId, int productId, string priceType);
        Task<int> SaveProductTpPrice(ProductPriceModel model);
        Task<int> SaveProductSalePrice(ProductPriceModel model);



        List<ProductModel> GetProducts(int companyId, string type, string searchText);
        Task<ProductModel> GetProducts(int companyId, string type);

        ProductModel GetProduct(int id, string type);
        bool SaveProduct(int id, ProductModel model, out string message);
        bool EditProduct(int id, ProductModel model, out string message);
        bool DeleteProduct(int id);
        //List<SelectModel> GetProductSelectModels(int? productSubCategoryId);
        List<SelectModel> GetProductSelectModelsByProductSubCategory(int productSubCategoryId);
        object GetProductAutoComplete(string prefix, int companyId, string productType);
        
        object GetProductAutoCompleteCatagoryWise(string prefix, int companyId, string productType);
       // List<ProductPriceModel> GetLastUpdatedProductPrice(int companyId, string priceType);
        bool SaveProductPrice(ProductPriceModel model);
        bool SaveConvertedProduct(ConvertedProductModel model);
        decimal GetBaseCommissionRate(int? productId);
        List<ProductModel> GetFinishProducts(int v, string searchText);
        object GetProductAutoComplete(string prefix, int companyId);
        ProductModel GetProductInformation(int productId);
        List<SelectModel> GetRawMterialsSelectModel(int companyId);
        object GetFinishProductAutoComplete(string prefix, int companyId);

        string ProductPackName(int id);

        List<Product> GetProductInfo();
        ProductModel GetProductUnitPrice(int pId);
        decimal GetStockckAvailableQuantity(int productId, int stockFrom);
        bool SaveProductList(List<Product> newProductList);
        decimal GetRawMaterialStockUnitPrice(int id, DateTime date, int companyId);
        decimal GetProductPoressLoss(int productId);
        List<SelectModel> GetRawmaterialByDemand(int companyId, int demandId);
        BusinessCostCustomModel GetCustomerBusinessCost(int customerId, int productId, int stockInfoId);
        List<SelectModel> GetProductbyProductSubCategorySelectModels(int productSubCategoryId);
        decimal GetProductCogsPrice(int? companyId, int? productId);
        List<SelectModel> GetProductSelectModelsByCompanyAndProductType(int companyId, string productType);
        decimal GetProductPrice(int productId);
        

        List<SelectModel> GetProductTypeSelectModels();

        List<SelectListItem> GetDDLStockInfoDepartment(int? companyId);
    }
}
