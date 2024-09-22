using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class UpazilaAssignModel
    {
        public int UpazilaAssignId { get; set; }
        [DisplayName("Employee")]
        public Nullable<long> EmployeeId { get; set; }
        [DisplayName("District")]
        public Nullable<int> DistrictId { get; set; }
        public Nullable<int> UpazilaId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        //---------------------------------------------
        public string Name { get; set; }
    }
}
