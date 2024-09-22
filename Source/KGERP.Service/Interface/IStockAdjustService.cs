using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IStockAdjustService : IDisposable
    {
        Task<StockAdjustModel> GetStockAdjusts(int companyId, DateTime? fromDate, DateTime? toDate);
        Task<IssueVm> GetStockIssues(int companyId, DateTime? fromDate, DateTime? toDate, int status);
        Task<IssueVm> IssueSlaveGet(int companyId, int issueId);
        Task<int> IssueAdd(IssueVm model);
        Task<int> IssueSlaveAdd(IssueVm model);
        Task<int> IssueSlaveEdit(IssueVm model);
        Task<int> IssueSlaveDelete(int id);
        Task<IssueVm> GetSingleIssueSlave(int id);
        Task<IssueVm> GetSingleIssue(int id);
        Task<int> IssueSubmit(int? id = 0);

        object GetAutoCompleteEmployee(string prefix);
        List<StockAdjustModel> GetStockAdjusts(int companyId, string searchDate, string searchText);

        bool SaveStockAdjust(int v, StockAdjustModel model);
        StockAdjustModel GetStockAdjust(int id);
        //StoreModel GetStore(long id);
        //bool SaveStore(long id, StoreModel store);
        //List<SoreProductQty> GetStoreProductQty(int companyId);
        //List<SoreProductQty> GetRMStoreProductQty();
        //List<SoreProductQty> GetEcomProductStore();
        //StoreModel GetOpenningStore(long id);
    }
}
