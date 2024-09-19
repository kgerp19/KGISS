using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class MemberBasePFSummaryModel
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

        public int PFDataId { get; set; }
        public string EmployeeId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime PFDate { get; set; }
        [DisplayName("Self Contribution")]
        public double SelfContribution { get; set; }
        [DisplayName("Company Contribution")]
        public double CompanyContribution { get; set; }
        [DisplayName("Office Profit")]
        public double OfficeProfit { get; set; }
        [DisplayName("Self Profit")]
        public double SelfProfit { get; set; }
        public double Total { get; set; }

        public Nullable<System.DateTime> ProfitDate { get; set; }

        public string Description { get; set; }
        public string PfMonth { get; set; }
        public string CompanyName { get; set; }



        [DisplayName("Upload File")]
        public string FilePath { get; set; }
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}
