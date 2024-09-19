using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class SubZoneModel
    {
        public int SubZoneId { get; set; }
        [DisplayName("Zone")]
        public int ZoneId { get; set; }
        [DisplayName("Territory")]
        public string Name { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }


        public virtual ZoneModel Zone { get; set; }
    }
}
