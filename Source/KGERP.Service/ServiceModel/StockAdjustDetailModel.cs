using System;

namespace KGERP.Service.ServiceModel
{
    public class StockAdjustDetailModel
    {
        public int StockAdjustDetailId { get; set; }
        public int StockAdjustId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ExcessQty { get; set; }
        public decimal LessQty { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ProductModel Product { get; set; }
        public virtual StockAdjustModel StockAdjust { get; set; }
    }
}
