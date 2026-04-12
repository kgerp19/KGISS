using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
    public class ApplicationManageModel
    {
        public long ApplicationId { get; set; }

        [DisplayName("Applicant (Vendor)")]
        [Required(ErrorMessage = "Applicant is required")]
        public long ApplicantId { get; set; }
        public string ApplicantName { get; set; }

        [DisplayName("Manager (Employee)")]
        [Required(ErrorMessage = "Manager is required")]
        public long ManagerId { get; set; }
        public string ManagerName { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Start Date")]
        [Required(ErrorMessage = "Start Date is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [Required(ErrorMessage = "End Date is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DisplayName("Day Counts")]
        public int DayCounts { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }

        public string Remarks { get; set; }

        [DisplayName("Application Date")]
        public DateTime ApplicationDate { get; set; }

        public int Status { get; set; } // 0: Pending, 1: Approved, 2: Rejected
        public string StatusName
        {
            get
            {
                if (Status == 1) return "Approved";
                if (Status == 2) return "Rejected";
                return "Pending";
            }
        }

        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifedDate { get; set; }
        public bool IsSubmitted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Form Fields Dropdowns
        public SelectList Managers { get; set; }
        public SelectList Applicants { get; set; }

        // Filters
        [DisplayName("Filter by Status")]
        public int? SearchStatus { get; set; }

        [DisplayName("From Date")]
        public string StrFromDate { get; set; }

        [DisplayName("To Date")]
        public string StrToDate { get; set; }

        // Data Lists
        public IEnumerable<ApplicationManageModel> ApplicationList { get; set; }

        // Navigation properties for UI
        public List<SignatoryApprovalModel> SignatoryApprovals { get; set; } = new List<SignatoryApprovalModel>();
    }

    public class SignatoryApprovalModel
    {
        public long RequisitionSignatoryApprovalId { get; set; }
        public long RequisitionSignatoryId { get; set; }
        public long RequisitionId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public int OrderBy { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }

        public string StatusName
        {
            get
            {
                if (Status == 1) return "Approved";
                if (Status == 2) return "Rejected";
                return "Pending";
            }
        }
    }
}
