using System;

namespace KGERP.Data.CustomModel
{
    public class DeliveredProductCustomModel
    {
        public long OrderMasterId { get; set; }
        public int ProductId { get; set; }
        public string ProductBatchId { get; set; }
        public Nullable<double> OrderQty { get; set; }
        public Nullable<double> DeliveredQty { get; set; }
    }
}
