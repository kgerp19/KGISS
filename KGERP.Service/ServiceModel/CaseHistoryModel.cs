using System;

namespace KGERP.Service.ServiceModel
{
    public class CaseHistoryModel
    {
        public long CaseHistoryID { get; set; }
        public long CaseId { get; set; }
        public string ChangeHistory { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
