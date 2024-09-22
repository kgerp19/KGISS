using System;

namespace KGERP.Data.CustomModel
{
    public class AccountHeadCustomModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int CompanyId { get; set; }
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public Nullable<int> OrderNo { get; set; }
    }
}
