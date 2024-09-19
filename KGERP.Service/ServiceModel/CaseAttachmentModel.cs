using System;

namespace KGERP.Service.ServiceModel
{
    public class CaseAttachmentModel
    {
        public long CaseAttachmentId { get; set; }
        public Nullable<long> CaseId { get; set; }
        public string AttachFileName { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    }
}
