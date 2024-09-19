using System;

namespace KGERP.Service.ServiceModel
{
    public class OrderDeliverDetailModel
    {
        public bool IsActive { get; set; }
        public long OrderDeliverDetailId { get; set; }
        public Nullable<long> OrderDeliverId { get; set; }
        public int ProductId { get; set; }
        public double UnitPrice { get; set; }
        public double DeliveredQty { get; set; }
        public decimal Amount { get; set; }
        public decimal BaseCommission { get; set; }
        public decimal CashCommission { get; set; }
        public decimal CarryingRate { get; set; }
        public decimal CreditCommission { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal EBaseCommission { get; set; }
        public decimal ECarryingCommission { get; set; }
        public decimal ECashCommission { get; set; }
        public decimal COGSPrice { get; set; }
        public decimal TPPrice { get; set; }
        public decimal SaleCommissionRate { get; set; }
        public decimal AdditionPrice { get; set; }
        public string EngineNo { get; set; }
        public string ChassisNo { get; set; }
        public string BatteryNo { get; set; }
        public string RearTyreRH { get; set; }
        public string RearTyreLH { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifedDate { get; set; }

        public virtual OrderDeliverModel OrderDeliver { get; set; }
        public virtual ProductModel Product { get; set; }

        //--------------------Extended Properties------------------
        public Nullable<double> OrderQty { get; set; }
        public long OrderDetailId { get; set; }
    }
}
