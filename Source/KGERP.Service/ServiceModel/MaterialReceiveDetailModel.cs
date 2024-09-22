using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class MaterialReceiveDetailModel
    {
        public long MaterialReceiveDetailId { get; set; }
        public Nullable<long> MaterialReceiveId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Deduction { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }

        public Nullable<decimal> StockInQty { get; set; }
        public Nullable<decimal> StockInRate { get; set; }
        public Nullable<decimal> BagWeight { get; set; }
        [Required]
        [DisplayName("Bag Qty")]
        public int BagQty { get; set; }

        public virtual MaterialReceiveModel MaterialReceive { get; set; }

        ///---------------Extended Properties--------------------
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal Amount { get; set; }
        public decimal POQty { get; set; }
        public decimal PurchasedQty { get; set; }
        public decimal DueQty { get; set; }
        [Required]
        [DisplayName("Bag")]
        public int BagId { get; set; }
        public string BagName { get; set; }
        public decimal StockAmount { get; set; }
        public decimal NetAmount { get; set; }
        public long DeductionAmount { get; set; }
       
        //public long PurchaseOrderDetailId {get;set;}
        public long PurchaseOrderDetailFk { get; set; } = 0;
       

    }
}
