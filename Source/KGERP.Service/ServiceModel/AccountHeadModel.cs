using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class AccountHeadModel
    {

        public long AccountHeadId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Account Code")]
        public string AccCode { get; set; }
        [DisplayName("Account Name")]
        public string AccName { get; set; }
        [DisplayName("Parent Head")]
        public Nullable<long> ParentId { get; set; }
        [DisplayName("Leyer No")]
        public Nullable<int> TierNo { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual AccountHeadModel ParentHead { get; set; }
        public virtual ICollection<AccountHeadModel> AccountHead1 { get; set; }
    }
}
