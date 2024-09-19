using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class PFDataModel
    {
        public string ButtonName
        {
            get
            {
                return PFDataId > 0 ? "Update" : "Save";
            }
        }
        [Required]
        [DisplayName("Employee Id")]
        public string EmployeeId { get; set; }
        [Required]
        public int PFDataId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<double> SelfContribution { get; set; }
        public Nullable<double> OfficeContribution { get; set; }
        public Nullable<double> SelfProfit { get; set; }
        public Nullable<double> OfficeProfit { get; set; }
        public Nullable<System.DateTime> ProfitDate { get; set; }
        public string Description { get; set; }
        public string PfMonth { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> PFCreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

    }
}
