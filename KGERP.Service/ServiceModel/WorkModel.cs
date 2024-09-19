using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class WorkModel
    {
        public string ButtonName
        {
            get
            {
                return WorkId > 0 ? "Update" : "Save";
            }
        }
        public int WorkId { get; set; }
        public long ManagerId { get; set; }
        [DisplayName("Work ID")]
        public string WorkNo { get; set; }
        [Required]
        [DisplayName("Work Title")]
        public string WorkTopic { get; set; }
        [DisplayName("Work Detail")]
        public string WorkDetail { get; set; }
        [DisplayName("Entry Date")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EntryDate { get; set; }

        [DisplayName("Exp. End Date")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ExpectedEndDate { get; set; }

        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Please select a status")]
        public int WorkStateId { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual WorkStateModel WorkState { get; set; }

        public virtual ICollection<WorkAssignModel> WorkAssigns { get; set; }

        //-------------------Extended Property--------------
        public string ManagerState { get; set; }
        public string MemberState { get; set; }
        public long MemberId { get; set; }
        public string Report { get; set; }
    }
}
