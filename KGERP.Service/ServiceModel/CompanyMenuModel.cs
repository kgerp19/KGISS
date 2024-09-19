using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class CompanyMenuModel
    {
        public string ButtonName
        {
            get
            {
                return CompanyMenuId > 0 ? "Update" : "Create";
            }
        }
        public int CompanyMenuId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Menu")]
        public string Name { get; set; }
        public string ShortName { get; set; }
        [DisplayName("Menu Order")]
        public int OrderNo { get; set; }
        public Nullable<int> CompanyType { get; set; }
        public string MushokNo { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Param { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual CompanyModel Company { get; set; }
    }
}
