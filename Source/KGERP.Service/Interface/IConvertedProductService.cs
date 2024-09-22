using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IConvertedProductService
    {
        Task<ConvertedProductModel> GetConvertedProducts(int companyId, DateTime? fromDate, DateTime? toDate);
        List<ConvertedProductModel> GetConvertedProducts(DateTime? searchDate, string searchText, int companyId);
        decimal GetStockckAvailableQuantity(int companyId, int productId, int stockFrom, string selectedDate);
        Task<ConvertedProductModel> GetConvertedProduct(int companyId,int convertedId);
        Task<int> SaveConvertedProduct(ConvertedProductModel model);
        Task<bool> ChangeConvertStatus(int convertedProductId, int companyId, string convertStatus);
        Task<ConvertedProductModel> GetConvertedProductById(int companyId, int convertedProductId);
    }
}
