using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IPurchaseReturnService
    {
        Task<PurchaseReturnModel> GetPurchaseReturns(int companyId, DateTime? fromDate, DateTime? toDate, string type);
        PurchaseReturnModel GetPurchaseReturn(long purchaseReturnId);
        bool SavePurchaseReturn(long purchaseReturnId, PurchaseReturnModel purchaseReturn, out string message);
        string GetNewPurchaseReturnNo(int companyId, string productType);
    }
}
