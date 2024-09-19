using System;

namespace KGERP.Service.ServiceModel
{
    public class RequisitionDeliverItemModel
    {
        public int RequisitionDeliveritemId { get; set; }
        public Nullable<int> RequisitionDeliverId { get; set; }
        public Nullable<int> ProdId { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> ExtraQty { get; set; }
        public Nullable<decimal> TotalQty { get; set; }

        public virtual RequisitionDeliverModel RequisitionDelivery { get; set; }
    }
}
