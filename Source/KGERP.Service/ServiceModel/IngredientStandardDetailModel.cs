using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class IngredientStandardDetailModel
    {
        public int IngredientStandardDetailId { get; set; }
        public int IngredientStandardId { get; set; }
        [DisplayName("Attribute Name")]
        public string ColumnName { get; set; }
        [DisplayName("Attribute Value")]
        public string ColumnValue { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
