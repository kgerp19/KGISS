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
    
    public partial class CompanyUserMenu
    {
        public long CompanyUserMenuId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public int CompanyMenuId { get; set; }
        public int CompanySubMenuId { get; set; }
        public string UserId { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
