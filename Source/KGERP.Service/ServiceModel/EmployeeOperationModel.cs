using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class EmployeeOperationModel
    {
        public string ButtonName
        {
            get
            {
                return OperationId > 0 ? "Update" : "Save";
            }

        }
        public int OperationId { get; set; }
        [Required]
        [DisplayName("Employee Id")]

        public string EmployeeId { get; set; }
        [Required]
        public string Name { get; set; }

        [DisplayName("ActionDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ActionDate { get; set; }
        [Required]
        [DisplayName("Operation/Action Type")]
        public string EmployeeOperationType { get; set; }

        [DisplayName("Reason/Reference")]
        public string Reason { get; set; }
        public string Remarks { get; set; }

        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> FromDate { get; set; }

        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public List<SelectModel> EmployeeOperationTypes { get; set; }

    }
}
