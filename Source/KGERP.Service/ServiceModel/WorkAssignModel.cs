using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class WorkAssignModel
    {
        public string ButtonName
        {
            get
            {
                return WorkAssignId > 0 ? "Update" : "Save";
            }
        }
        public long WorkAssignId { get; set; }
        public int WorkId { get; set; }
        [DisplayName("Member")]
        public long MemberId { get; set; }
        public int WorkStateId { get; set; }
        public string Report { get; set; }
        public string FileName { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual WorkModel Work { get; set; }
        public virtual WorkStateModel WorkState { get; set; }

        //------------Extended Property-------------------------------

        [DisplayName("Work ID")]
        public string WorkNo { get; set; }
        [DisplayName("Work Title")]
        public string WorkTopic { get; set; }
        [DisplayName("Entry Date")]
        public Nullable<System.DateTime> EntryDate { get; set; }
        [DisplayName("Exp. End Date")]
        public Nullable<System.DateTime> ExpectedEndDate { get; set; }
        public string ManagerState { get; set; }
        public string MemberState { get; set; }
        public virtual ICollection<WorkAssignFileModel> WorkAssignFiles { get; set; }

    }
}
