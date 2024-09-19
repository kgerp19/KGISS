using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class AssetCategoryModel
    {

        public int AssetCategoryId { get; set; }
        [DisplayName("Serial No")]
        public string SerialNo { get; set; }
        [DisplayName("Asset Category")]
        [Required]
        public string Name { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
