using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.ComparativeStatementService.Comparative_Statement
{
    public class ComparativeStatementDetailVm
    {
        public long CSDetailID { get; set; }
        public long CSID { get; set; }
        public int SupplierID { get; set; }
        public decimal QuotedPrice { get; set; }
        public string Remarks { get; set; }
        public string VendorName { get; set; }
        public bool Recommendation { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
