using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface ISaleReturnService
    {
        Task<long> SubmitSaleReturnByProduct (SaleReturnModel model);
        Task<SaleReturnModel> GetSaleReturns(int companyId, DateTime? fromDate, DateTime? toDate);
        SaleReturnModel GetSaleReturn(int id, string productType);
        List<SaleReturnDetailModel> GetDeliveredItems(long orderDeliverId, int companyId);
        Task<SaleReturnModel> SalesReturnSlaveGet(int companyId, int saleReturnId, string productType);

        long SaveSaleReturn(SaleReturnModel saleReturn,out string message);
        List<SaleReturnModel> GetOldSaleReturns(DateTime? searchDate, string searchText, int companyId, string productType);
    }
}
