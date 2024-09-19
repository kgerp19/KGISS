using System;

namespace KGERP.Service.ServiceModel
{
    public class VoucherDetailModel
    {
        public long VoucherDetailId { get; set; }
        public Nullable<long> VoucherId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public Nullable<double> DebitAmount { get; set; }
        public Nullable<double> CreditAmount { get; set; }
        public string Particular { get; set; }
        public Nullable<DateTime> TransactionDate { get; set; }
        public virtual HeadGLModel HeadGL { get; set; }
        public virtual VoucherModel Voucher { get; set; }

        //--------------Extended-------------------------
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public long TempVoucherId { get; set; }
    }
}
