using System;

namespace KGERP.Service.ServiceModel
{
    public class BankBranchModel
    {
        public int BankBranchId { get; set; }
        public Nullable<int> BankId { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZIPCode { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
