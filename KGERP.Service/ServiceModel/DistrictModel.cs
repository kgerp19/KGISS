using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class DistrictModel
    {
        public string ButtonName
        {
            get
            {
                return DistrictId > 0 ? "Update" : "Save";
            }
        }
        public int DistrictId { get; set; }
        [DisplayName("District")]
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Nullable<long> DivisionId { get; set; }
        [DisplayName("District Code")]
        public string Code { get; set; }
        public Nullable<long> CommercialZoneId { get; set; }
        public string GovtDistrictCode { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public IEnumerable<DistrictModel> DataList { get; set; }

    }
}
