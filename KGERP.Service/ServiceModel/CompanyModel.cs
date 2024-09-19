using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class CompanyModel
    {
        public string ButtonName
        {
            get
            {
                return CompanyId > 0 ? "Update" : "Create";
            }
        }
        public int CompanyId { get; set; }
        public Nullable<int> ParentId { get; set; }
        [DisplayName("Company")]
        public string Name { get; set; }
        [DisplayName("Short Name")]
        public string ShortName { get; set; }
        [DisplayName("Order No")]
        public int OrderNo { get; set; }
        [DisplayName("Company Type")]
        public Nullable<int> CompanyType { get; set; }
        [DisplayName("Mushok No")]
        public string MushokNo { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Param { get; set; }
        [DisplayName("Company Logo")]
        public string CompanyLogo { get; set; }
        public Nullable<int> LayerNo { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Company")]
        public bool IsCompany { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }


        public virtual List<CompanyModel> Company1 { get; set; } = new List<CompanyModel>();
        public virtual CompanyModel Company2 { get; set; }

        public IEnumerable<CompanyModel> DataList { get; set; }


        //---------------Extended Properties------------------------------
        public HttpPostedFileBase CompanyLogoUpload { get; set; }
        public int Id { get; set; }

    }
}
