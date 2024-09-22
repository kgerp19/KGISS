using System;

namespace KGERP.Service.ServiceModel
{
    public class IssueDetailInfoModel
    {
        public long IssueDetailId { get; set; }
        public Nullable<long> IssueMasterId { get; set; }
        public Nullable<int> RProductId { get; set; }
        public Nullable<decimal> RMQ { get; set; }

        public virtual IssueMasterInfoModel IssueMasterInfo { get; set; }

        public string ProductName { get; set; }
    }
}
