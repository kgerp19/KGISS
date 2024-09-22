using System;

namespace KGERP.Service.ServiceModel
{
    public class KGREHistoryModel
    {
        public long KGREHistoryID { get; set; }
        public long KGREId { get; set; }
        public string ChangeHistory { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
