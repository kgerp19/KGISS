using System;

namespace KGERP.Service.ServiceModel
{
    public class VoucherTypeModel
    {
        public int VoucherTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public bool IsBankOrCash { get; set; }
    }
}
