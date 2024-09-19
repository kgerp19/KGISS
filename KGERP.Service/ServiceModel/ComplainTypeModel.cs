using System;
using System.Collections.Generic;

namespace KGERP.Service.ServiceModel
{
    public class ComplainTypeModel
    {
        public int ComplainTypeId { get; set; }
        public string Description { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public virtual ICollection<ComplainManagementModel> ComplainManagements { get; set; }
    }
}
