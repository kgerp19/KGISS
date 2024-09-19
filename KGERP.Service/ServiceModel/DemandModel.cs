using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class DemandModel
    {
        public long DemandId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Demand No")]
        public string DemandNo { get; set; }
        [DisplayName("Demand Date")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DemandDate { get; set; }
        [DisplayName("Remarks")]
        [Required]
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public int RequisitionType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public virtual ICollection<DemandItemModel> DemandItems { get; set; }
        public IEnumerable<DemandModel> DataList { get; set; }
    }
}
