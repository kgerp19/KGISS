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
    
    public partial class ApprovalOrderMaster
    {
        public long Id { get; set; }
        public int SalesOrderSignatoryId { get; set; }
        public int ApprovalStatus { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public string Comments { get; set; }
        public long OrderMasterId { get; set; }
        public int Precedence { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
    }
}