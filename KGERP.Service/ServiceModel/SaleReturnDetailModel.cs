using System;

namespace KGERP.Service.ServiceModel
{
    public class SaleReturnDetailModel
    {
        public long SaleReturnDetailId { get; set; }
        public Nullable<long> SaleReturnId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> COGSRate { get; set; }
        public Nullable<decimal> BaseCommission { get; set; }
        public Nullable<decimal> CashCommission { get; set; }
        public Nullable<decimal> CarryingCommission { get; set; }
        public Nullable<decimal> SpecialDiscount { get; set; }
        public Nullable<decimal> AdditionPrice { get; set; }
        public virtual SaleReturnModel SaleReturn { get; set; }

        //------------Extended Properties---------------
        public Nullable<decimal> DeliveredQty { get; set; }
        public Nullable<decimal> COGSPrice { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> ActualRate { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}
