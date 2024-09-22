using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class PurchaseOrderDetailModel
    {
        public long PurchaseOrderDetailId { get; set; }
        public Nullable<long> PurchaseOrderId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<decimal> PurchaseQty { get; set; }
        public Nullable<decimal> DemandRate { get; set; }
        [Required]
        public decimal QCRate { get; set; }
        public Nullable<decimal> PurchaseRate { get; set; }
        public Nullable<decimal> PurchaseAmount { get; set; }
        public Nullable<int> PackSize { get; set; }
        public virtual PurchaseOrderModel PurchaseOrder { get; set; }

        //-----------Extended Properties--------------------------------------
        public string ProductCode { get; set; }
        public string RawMaterial { get; set; }
        public Nullable<decimal> PresentStock { get; set; }
        [DisplayName("Due Amount")]
        public Nullable<decimal> DueAmount { get; set; }
        public Nullable<decimal> RequiredQty { get; set; }
        public Nullable<decimal> PurchasedQty { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}
