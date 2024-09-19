using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class UpazilaModel
    {
        public string ButtonName
        {
            get
            {
                return UpazilaId > 0 ? "Update" : "Save";
            }
        }
        public int UpazilaId { get; set; }
        [DisplayName("Upazila")]
        public string Name { get; set; }
        [DisplayName("District")]
        public int DistrictId { get; set; }
        [DisplayName("Upazila Code")]
        public string Code { get; set; }
        [DisplayName("Factory Carrying")]
        public Nullable<decimal> FacCarryingCommission { get; set; }
        [DisplayName("Depo Carrying")]
        public Nullable<decimal> DepoCarryingCommission { get; set; }
        [DisplayName("Marketing Officer")]
        public Nullable<long> MarketingOfficerId { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual DistrictModel District { get; set; }
        public string ShortName { get; set; }
        public string DistrictName { get; set; }
        public IEnumerable<UpazilaModel> DataList { get; set; }

    }
}
