using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Data.CustomModel
{
    public class AttendenceApprovalAction
    {

        public int Id { get; set; }
        [DisplayName("Employee Id")]
        public string EmployeeId { get; set; }

        public string Name { get; set; }
        [DisplayName("Reason")]
        public string Resion { get; set; }
        [DisplayName("Application For")]
        public string ApproveFor { get; set; }
        [DisplayName("Attendence Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> AttendenceDate { get; set; }
        [DisplayName("Application Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> ApplicationDate { get; set; }
        [DisplayName("In Time")]
        public Nullable<System.TimeSpan> InTime { get; set; }
        [DisplayName("Out Time")]
        public Nullable<System.TimeSpan> OutTime { get; set; }
        [DisplayName("Modified In-Time")]
        public Nullable<System.TimeSpan> ModifiedInTime { get; set; }
        [DisplayName("Modified Out-Time")]
        public Nullable<System.TimeSpan> ModifiedOutTime { get; set; }
        [DisplayName("Manager Status")]
        public Nullable<int> ManagerStatus { get; set; }
        [DisplayName("Note")]
        public string ManagerNote { get; set; }

        public Nullable<int> HrStatus { get; set; }
        [DisplayName("Note")]
        public string HrNote { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }

        [DisplayName("From Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> FromDateForOnField { get; set; }
        [DisplayName("To Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> ToDateForOnField { get; set; }


        [DisplayName("Days")]
        public int TourDays { get; set; }
    }
}
