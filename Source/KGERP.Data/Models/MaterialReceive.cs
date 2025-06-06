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
    
    public partial class MaterialReceive
    {
        public long MaterialReceiveId { get; set; }
        public int CompanyId { get; set; }
        public Nullable<long> PurchaseOrderId { get; set; }
        public string MaterialType { get; set; }
        public string ReceiveNo { get; set; }
        public Nullable<int> StockInfoId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<long> ReceivedBy { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public string ChallanNo { get; set; }
        public Nullable<System.DateTime> ChallanDate { get; set; }
        public Nullable<System.DateTime> UnloadingDate { get; set; }
        public string TruckNo { get; set; }
        public string DriverName { get; set; }
        public bool AllowLabourBill { get; set; }
        public decimal TruckFare { get; set; }
        public decimal LabourBill { get; set; }
        public Nullable<decimal> CandFBill { get; set; }
        public Nullable<decimal> WareHouseRentBill { get; set; }
        public Nullable<decimal> FinancialCharge { get; set; }
        public decimal ValueAdjustment { get; set; }
        public decimal TDSDiduction { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string MaterialReceiveStatus { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsSeen { get; set; }
    
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
