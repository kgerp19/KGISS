using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
  public class SupplierProductViewModel
    {
        public long Id { get; set; }
        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public string ProductName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Zone { get; set; }
        public string Code { get; set; }
        public int VendorId { get; set; }
        public int Count { get; set; }
        public string VendorName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public IEnumerable<SupplierProductViewModel> DataList { get; set; }

    }
}
