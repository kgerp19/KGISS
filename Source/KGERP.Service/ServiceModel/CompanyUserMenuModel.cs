using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class CompanyUserMenuModel
    {
        public string ButtonName
        {
            get
            {
                return CompanyUserMenuId > 0 ? "Update" : "Create";
            }
        }
        public long CompanyUserMenuId { get; set; }
        [DisplayName("Company")]
        public int CompanyId { get; set; }
        [DisplayName("Menu")]
        public int CompanyMenuId { get; set; }
        [DisplayName("Sub Menu")]
        public int CompanySubMenuId { get; set; }
        [DisplayName("User ID")]
        public string UserId { get; set; }
        [DisplayName("View")]
        public bool IsView { get; set; }
        [DisplayName("Add")]
        public bool IsAdd { get; set; }
        [DisplayName("Update")]
        public bool IsUpdate { get; set; }
        [DisplayName("Delete")]
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual CompanyModel Company { get; set; }
        public virtual CompanyMenuModel CompanyMenu { get; set; }
        public virtual CompanySubMenuModel CompanySubMenu { get; set; }

        //--------------Extended Properties-----------------------------
        public string CompanyName { get; set; }
        public string CompanyMenuName { get; set; }
        public string CompanySubMenuName { get; set; }
    }
}
