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
    
    public partial class RequisitionItemDetail
    {
        public System.Guid RequistionItemDetailId { get; set; }
        public int RequisitionItemId { get; set; }
        public int RequisitionId { get; set; }
        public Nullable<int> RProductId { get; set; }
        public Nullable<decimal> RProcessLoss { get; set; }
        public Nullable<decimal> RQty { get; set; }
        public Nullable<decimal> RExtraQty { get; set; }
        public Nullable<decimal> RTotalQty { get; set; }
        public Nullable<int> FProductId { get; set; }
        public Nullable<decimal> FQty { get; set; }
        public decimal RUnitPrice { get; set; }
        public bool IsActive { get; set; }
        public int FinishProductBOMId { get; set; }
    }
}