using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class DemandItemDetailModel
    {
        public System.Guid DemandItemDetailId { get; set; }
        public long DemandItemId { get; set; }
        public Nullable<long> DemandId { get; set; }
        public Nullable<int> RProductId { get; set; }
        public decimal RQty { get; set; }
        public decimal RExtraQty { get; set; }
        public decimal RStockQty { get; set; }
        public decimal RRequiredQty { get; set; }
        public decimal RUnitPrice { get; set; }
        public decimal RTotalAmount { get; set; }

        //-----------Extended Properties----------
        public string RawMaterial { get; set; }
        [DisplayName("Code")]
        public string MaterialCode { get; set; }
        public decimal TotalQty { get; set; }

    }
}
