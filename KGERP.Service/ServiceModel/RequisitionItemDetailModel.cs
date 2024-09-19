using System;

namespace KGERP.Service.ServiceModel
{
    public class RequisitionItemDetailModel
    {
        public System.Guid RequistionItemDetailId { get; set; }
        public int RequisitonItemId { get; set; }
        public int RequisitionId { get; set; }
        public Nullable<int> RProductId { get; set; }
        public Nullable<decimal> RProcessLoss { get; set; }
        public Nullable<decimal> RQty { get; set; }
        public Nullable<decimal> RExtraQty { get; set; }
        public Nullable<decimal> RTotalQty { get; set; }
        public Nullable<int> FProductId { get; set; }
        public Nullable<decimal> FQty { get; set; }

        //---------------Extended Properties-----------------
        public string RProductName { get; set; }
        public Nullable<decimal> BalanceQty { get; set; }
        public decimal RUnitPrice { get; set; }
        public int? AccountingHeadId { get; set; }

    }
}
