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
    
    public partial class Head2
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public Nullable<int> LayerNo { get; set; }
        public string Remarks { get; set; }
        public bool IsPrimaryHead2 { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Head1 Head1 { get; set; }
    }
}
