using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Data.CustomModel
{
    public class ReplaceApproverViewModel
    {
        // From (Old Signatory)
        [DisplayName("From Code")]
        [Required(ErrorMessage = "Old signatory is required")]
        public long OldSignatoryEmpId { get; set; }

        [DisplayName("From Employee Name")]
        public string OldSignatoryName { get; set; }

        [DisplayName("From Employee Code")]
        public string OldSignatoryCode { get; set; }

        [DisplayName("From Company")]
        public string OldCompanyName { get; set; }

        [DisplayName("From Division")]
        public string OldDivisionName { get; set; }

        [DisplayName("From Department")]
        public string OldDepartmentName { get; set; }

        [DisplayName("From Section")]
        public string OldSectionName { get; set; }

        [DisplayName("From Designation")]
        public string OldDesignationName { get; set; }

        [DisplayName("From Appointment Date")]
        public DateTime? OldAppointmentDate { get; set; }

        [DisplayName("From Employee Status")]
        public string OldEmployeeStatus { get; set; }

        // To (New Signatory)
        [DisplayName("To Code")]
        [Required(ErrorMessage = "New signatory is required")]
        public long NewSignatoryEmpId { get; set; }

        [DisplayName("To Employee Name")]
        public string NewSignatoryName { get; set; }

        [DisplayName("To Employee Code")]
        public string NewSignatoryCode { get; set; }

        [DisplayName("To Company")]
        public string NewCompanyName { get; set; }

        [DisplayName("To Division")]
        public string NewDivisionName { get; set; }

        [DisplayName("To Department")]
        public string NewDepartmentName { get; set; }

        [DisplayName("To Section")]
        public string NewSectionName { get; set; }

        [DisplayName("To Designation")]
        public string NewDesignationName { get; set; }

        [DisplayName("To Appointment Date")]
        public DateTime? NewAppointmentDate { get; set; }

        [DisplayName("To Employee Status")]
        public string NewEmployeeStatus { get; set; }

        // Filter Context
        [DisplayName("Company")]
        public int? CompanyId { get; set; }

        [DisplayName("Module Name")]
        public string IntegrateWith { get; set; }

        // Audit
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
