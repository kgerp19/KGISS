using System;

namespace KGERP.Service.ServiceModel
{
    public class CaseCommentModel
    {
        public long CaseCommentId { get; set; }
        public Nullable<long> CaseId { get; set; }
        public string CaseComments { get; set; }
        public string AttachmentFile { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public long ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    }
}
