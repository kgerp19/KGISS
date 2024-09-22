using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.ServiceModel
{
    public class IssueMasterInfoModel
    {
        public long IssueMasterId { get; set; }
        public string IssueNo { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public Nullable<int> StoreId { get; set; }
        public string IssueBy { get; set; }
        public string ReceiveBy { get; set; }
        public Nullable<int> FPId { get; set; }
        public Nullable<decimal> FPQ { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public virtual ICollection<IssueDetailInfoModel> IssueDetailInfoes { get; set; }
        public List<SelectModel> StockInfos { get; set; }
    }
}
