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
    
    public partial class ReportSignatory
    {
        public int ReportSignatoryId { get; set; }
        public string ReportSignatoryName { get; set; }
        public string ReportPropertyName { get; set; }
        public string IntegrateWith { get; set; }
        public int ReportHeadId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifyBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    }
}
