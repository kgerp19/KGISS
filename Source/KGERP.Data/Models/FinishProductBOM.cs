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
    
    public partial class FinishProductBOM
    {
        public int ID { get; set; }
        public int RProductFK { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal RProcessLoss { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public int CompanyId { get; set; }
        public int FProductFK { get; set; }
        public bool IsActive { get; set; }
        public decimal Consumption { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<long> OrderDetailId { get; set; }
        public decimal UnitPrice { get; set; }
        public string ORDStyle { get; set; }
        public Nullable<int> Status { get; set; }
        public decimal COGS { get; set; }
    }
}