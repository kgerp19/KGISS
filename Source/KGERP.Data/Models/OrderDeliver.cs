//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KGERP.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderDeliver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderDeliver()
        {
            this.OrderDeliverDetails = new HashSet<OrderDeliverDetail>();
            this.SaleReturns = new HashSet<SaleReturn>();
        }
    
        public long OrderDeliverId { get; set; }
        public Nullable<long> OrderMasterId { get; set; }
        public string ProductType { get; set; }
        public int StockInfoId { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string ChallanNo { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> TransportTypeId { get; set; }
        public string DriverName { get; set; }
        public string VehicleNo { get; set; }
        public string MobileNo { get; set; }
        public Nullable<decimal> Carrying { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> Commission { get; set; }
        public Nullable<decimal> SpecialDiscount { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> DiscountRate { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string DepoInvoiceNo { get; set; }
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifedDate { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsSeen { get; set; }
        public string Remark { get; set; }
    
        public virtual OrderMaster OrderMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDeliverDetail> OrderDeliverDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleReturn> SaleReturns { get; set; }
    }
}
