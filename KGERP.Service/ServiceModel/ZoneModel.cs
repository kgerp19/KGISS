using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class ZoneModel
    {
        public int ZoneId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}
