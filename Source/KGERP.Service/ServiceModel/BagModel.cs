using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class BagModel
    {
        public int BagId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Bag Name")]
        public string BagName { get; set; }
        public int BagSize { get; set; }
        [DisplayName("Bag Size (gm)")]
        public decimal BagValue { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<BagModel> DataList { get; set; }
    }
}
