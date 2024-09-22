using System;
using System.Collections.Generic;

namespace KGERP.Data.CustomModel
{
    public class AccountHeadProcessCustomModel
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public Nullable<int> LayerNo { get; set; }
        public virtual ICollection<AccountHeadProcessCustomModel> AccountHeadProcessCustomModels { get; set; }
    }
}
