using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class AdminSetUpModel
    {
        public long AdminId { get; set; }
        [DisplayName("Employee")]
        public long Id { get; set; }
        [DisplayName("Created By")]
        public string CreatededBy { get; set; }
        [DisplayName("Created Date")]
        public System.DateTime CreatedDate { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }
        [DisplayName("Modified Date")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Status")]
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
    }
}
