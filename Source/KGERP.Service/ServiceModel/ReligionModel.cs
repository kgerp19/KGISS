using System;

namespace KGERP.Service.ServiceModel
{
    public class ReligionModel
    {
        public int ReligionId { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
