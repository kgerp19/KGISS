using System;

namespace KGERP.Service.ServiceModel
{
    public class KGRECommentModel
    {
        public long KGRECommentId { get; set; }
        public Nullable<long> KGREId { get; set; }
        public string KGREComments { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
