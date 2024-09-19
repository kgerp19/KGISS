using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class AssetLocationModel
    {
        public int LocationId { get; set; }
        public string SerialNo { get; set; }
        [Required]
        [DisplayName("Location")]
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual ICollection<AssetSubLocationModel> AssetSubLocations { get; set; }
    }
}
