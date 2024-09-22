using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
   public class RentProductionModel
    {
        public int RentProductionId { get; set; }
        [Required]
        [DisplayName("Production No")]
        public string RentProductionNo { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Rental Company")]
        public Nullable<int> RentCompanyId { get; set; }
        [Required]
        [DisplayName("Production Date")]
        public System.DateTime? ProductionDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<RentProductionDetailModel> RentProductionDetails { get; set; }

        //Extented Property

        [Required]
        [DisplayName("Rent Company")]
        public string RentCompanyName { get; set; }
        [DisplayName("Company Owner")]
        public string RentCompanyOwner { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<RentProductionModel> DataList { get; set; }
    }
}
