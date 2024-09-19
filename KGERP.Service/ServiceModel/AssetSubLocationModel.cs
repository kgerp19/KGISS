using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class AssetSubLocationModel
    {
        public int SubLocationId { get; set; }
        [DisplayName("Location")]
        public Nullable<int> LocationId { get; set; }
        [DisplayName("Serial No")]
        public string SerialNo { get; set; }
        [Required]
        [DisplayName("Sub Location")]
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual AssetLocationModel AssetLocation { get; set; }
    }
}
