using System;

namespace KGERP.Service.ServiceModel
{
    public class EmiDetailModel
    {
        public long EmiDetailId { get; set; }
        public Nullable<int> EmiId { get; set; }
        public Nullable<System.DateTime> InstallmentDate { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<decimal> InstallmentAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<int> IsPaid { get; set; }
        public virtual EMIModel EMI { get; set; }
    }
}
