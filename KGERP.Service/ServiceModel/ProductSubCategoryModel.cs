using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class ProductSubCategoryModel
    {
        public string ButtonName
        {
            get
            {
                return ProductSubCategoryId > 0 ? "Update" : "Create";
            }

        }
        public string ProductType { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public int ProductSubCategoryId { get; set; }
        [DisplayName("Sub Category")]
        public string Name { get; set; }
        [DisplayName("Category")]
        public Nullable<int> ProductCategoryId { get; set; }
        [DisplayName("Base Commission")]
        public Nullable<decimal> BaseCommissionRate { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Order No")]
        public int OrderNo { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public virtual ProductCategoryModel ProductCategory { get; set; }
    }
}
