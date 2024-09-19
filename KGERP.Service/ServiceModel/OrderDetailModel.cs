using System;

namespace KGERP.Service.ServiceModel
{
    public class OrderDetailModel
    {
        public long OrderDetailId { get; set; }
        public Nullable<long> OrderMasterId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> ProductSerial { get; set; }
        public Nullable<int> ProductCategoryId { get; set; }
        public Nullable<int> ProductSubCategoryId { get; set; }
        public int ProductId { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<decimal> AvgParchaseRate { get; set; }

        public virtual ProductModel Product { get; set; }
        public virtual OrderMasterModel OrderMaster { get; set; }

        //-----------------Extented Property--------------
        public Nullable<int> AvailableQty { get; set; }
        public Nullable<int> DeliveredQty { get; set; }
        public Nullable<int> RemainingQty { get; set; }
        public Nullable<double> DeliveryAmount { get; set; }
        public decimal OrderAmount { get; set; }

    }
}
