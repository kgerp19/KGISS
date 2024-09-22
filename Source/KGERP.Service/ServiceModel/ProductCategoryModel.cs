using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class ProductCategoryModel
    {

        public string ButtonName
        {
            get
            {
                return ProductCategoryId > 0 ? "Update" : "Save";
            }

        }
        public string ProductType { get; set; }
        public int ProductCategoryId { get; set; }

        [DisplayName("Category")]
        public string Name { get; set; }
        [DisplayName("Cash Commission")]
        public Nullable<decimal> CashCustomerRate { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Order No")]
        public int OrderNo { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        public virtual CompanyModel Company { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        public IEnumerable<ProductCategoryModel> DataList { get; set; }
        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;
    }
}
