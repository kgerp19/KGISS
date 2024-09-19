using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class IngredientStandardModel
    {
        public int IngredientStandardId { get; set; }
        public int CompanyId { get; set; }
        [DisplayName("Raw Material Category")]
        public int ProductSubCategoryId { get; set; }
        [DisplayName("Raw Material")]
        public int ProductId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual ProductModel Product { get; set; }
        public virtual ProductSubCategoryModel ProductSubCategory { get; set; }
        public virtual ICollection<IngredientStandardDetailModel> IngredientStandardDetails { get; set; }

        //-------------------------Extended Properties--------------------------------------
        public string ProductSubCategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
