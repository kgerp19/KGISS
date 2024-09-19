using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IStockTransferService
    {
        Task<StockTransferModel> GetStockTransfer(int companyId, DateTime? fromDate, DateTime? toDate);
        Task<int> StockTransferAdd(StockTransferModel model);
        Task<int> StockTransferSlaveAdd(StockTransferModel model);
        Task<int> StockTransferSlaveEdit(StockTransferModel model);
        Task<int> StockTransferSlaveDelete(int stockTransferDetailId);
        Task<StockTransferDetailModel> GetSingleStockTransferSlave(int stockTransferId);
        Task<StockTransferModel> GetStockTransferSlave(int companyId, int stockTransferId);
        Task<int> StockTransferSubmit(int stockTransferId);


        Task<StockTransferModel> GetGcclRmTransfer(int companyId, DateTime? fromDate, DateTime? toDate);
        object GetProductAutoComplete(int companyId,string prefix, string productType);
        ProductCurrentStockModel GetStockckAvailableQuantity(int companyId, int productId, int stockFrom, string selectedDate);

        List<StockTransferModel> GetStockTransferInfo(int companyId, DateTime? searchDate, string searchText);
        List<StockTransferDetailModel> GetStockTransferDetail(int companyId);
        Task<StockTransferModel> GetStockTransferById(int companyId, int stockTransferId);
        bool ConfirmStockReceive(int stockTransferDetailId, decimal receiveQty, int stockTransferId, DateTime reaciveDate, int productId, int companyId);
      //  StockTransferModel GetStockTransfer(int id);
        bool DeleteStockTransfer(int stockTransferId);
        Task<int> StockReceivedUpdate(StockTransferModel stockTransfer);
    }
}
