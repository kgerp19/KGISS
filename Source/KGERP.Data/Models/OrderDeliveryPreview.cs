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
    
    public partial class OrderDeliveryPreview
    {
        public long Id { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<long> OrderMasterId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> StockInfoId { get; set; }
        public string StoreName { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string ChallanNo { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string Party { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public Nullable<long> OrderDeliverId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductName { get; set; }
        public double OrderQty { get; set; }
        public double UnitPrice { get; set; }
        public double DeliveredQty { get; set; }
        public double ReadyToDeliver { get; set; }
        public decimal Amount { get; set; }
        public decimal BaseCommission { get; set; }
        public decimal CashCommission { get; set; }
        public decimal CarryingRate { get; set; }
        public decimal CreditCommission { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal EBaseCommission { get; set; }
        public decimal ECarryingCommission { get; set; }
        public decimal ECashCommission { get; set; }
        public decimal AdditionPrice { get; set; }
        public decimal COGSPrice { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}