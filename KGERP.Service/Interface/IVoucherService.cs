using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IVoucherService : IDisposable
    {
        string GetVoucherNo(int voucherTypeId, int companyId, DateTime voucherDate);
        List<VoucherModel> GetVouchers(int companyId, string searchDate, string searchText);
        Task<VoucherModel> GetVouchersList(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId);
        Task<VoucherModel> GetDraftVouchersList(int companyId, DateTime? fromDate, DateTime? toDate,int? voucherTypeId, bool? vStatus);
        Task<VoucherModel> GetVouchersListWithDelete(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId);
        Task<VoucherModel> GetVouchersListByVoucherType(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int voucherTypeId);
        Task<VoucherModel> GetStockVouchersList(int companyId);

        Task<VoucherModel> GetVouchersList(VoucherModel voucherModel);

        Task<List<VoucherModel>> GetAllVouchersList(int companyId,int? voucherId, DateTime? fromDate, DateTime? toDate);

        VoucherModel GetVoucher(int companyId, long id);
        //VoucherModel CreateTempVoucher(VoucherModel voucher);
        bool SaveVoucher(VoucherModel Voucher, out string message);
        //VoucherModel RemoveVoucherItem(long id);
        object GetVoucherNoAutoComplete(string prefix, int companyId);
        //int InsertFeedDada(int companyId);
        //int UpdateAccGLSheet();
    }
}
