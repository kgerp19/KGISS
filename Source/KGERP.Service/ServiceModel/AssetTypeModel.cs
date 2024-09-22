using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class AssetTypeModel
    {
        public int AssetTypeId { get; set; }
        public Nullable<int> AssetCategoryId { get; set; }
        public Nullable<int> BrandId { get; set; }
        [DisplayName("Product Name")]
        public string Name { get; set; }
        [DisplayName("Asset Category")]
        public string Category { get; set; }
        [DisplayName("Serial No")]
        public string SerialNo { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual AssetCategoryModel AssetCategory { get; set; }
        public virtual BrandModel Brand { get; set; }
    }
}
