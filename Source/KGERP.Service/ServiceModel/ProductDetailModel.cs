using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class ProductDetailModel
    {
        public long ProductDetailsId { get; set; }
        public long StoreDetailsId { get; set; }
        [DisplayName("Product")]
        public int ProductId { get; set; }
        [DisplayName("Engine No")]
        public string EngineNo { get; set; }
        [DisplayName("Chassiss No")]
        public string ChassissNO { get; set; }
        [DisplayName("Fuel Pump Sl No")]
        public string FuelPumpSlNo { get; set; }
        [DisplayName("Bettery No")]
        public string BetteryNo { get; set; }
        [DisplayName("Rear Tyre LH")]
        public string RearTyreLH { get; set; }
        [DisplayName("Rear Tyre RH")]
        public string RearTyreRH { get; set; }
        public string RearTyreFLH { get; set; }
        public string RearTyreFRH { get; set; }
        public string Color { get; set; }
        public Nullable<int> IsDelevered { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("LC NO")]
        public string LcNo { get; set; }
        public Nullable<long> DeliveredId { get; set; }

        public virtual StoreDetailModel StoreDetail { get; set; }
        public virtual ProductModel Product { get; set; }
        public virtual OrderDeliverModel OrderDeliver { get; set; }
    }
}
