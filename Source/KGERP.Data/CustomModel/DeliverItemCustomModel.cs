using KGERP.Utility;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Data.CustomModel
{
    public class DeliverItemCustomModel
    {
        public int ProductId { get; set; }
        public int StockInfoId { get; set; }


        [DisplayName("Product Code")]
        public string ProductCode { get; set; }
        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Unit")]
        public string OrderUnit { get; set; }
        [DisplayName("Qty")]
        public double OrderQty { get; set; }
        [DisplayName("Unit Price")]
        public double UnitPrice { get; set; }
        public decimal TPPrice { get; set; }
        public decimal TradePrice { get; set; }

        public double DisplayUnitPrice { get; set; }
        [DisplayName("Delivered Qty")]
        public double? DeliveredQty { get; set; }
        [DisplayName("Due Qty")]
        public double? DueQty { get; set; }
        [DisplayName("Available Qty")]
        public double StoreAvailableQty { get; set; }
        [DisplayName("Ready To Deliver")]
        public double ReadyToDeliver { get; set; }
        [DisplayName("Remaining Qty")]
        public double? OrderRemainingQty { get; set; }
        public decimal? Discount { get; set; }
        public string EngineNo { get; set; }
        public string ChassisNo { get; set; }
        public string BatteryNo { get; set; }
        public string RearTyreRH { get; set; }
        public string RearTyreLH { get; set; }
        public List<SelectModel> Engine { get; set; }


        public decimal EBaseCommission { get; set; }
        public decimal ECarryingCommission { get; set; }
        public decimal ECashCommission { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal AdditionPrice { get; set; }

        public int CompanyId { get; set; }
        public long OrderDetailId { get; set; }

    }
}
