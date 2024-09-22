using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class AttendenceApproveApplicationModel
    {
        public int Id { get; set; }
        [DisplayName("Employee Id")]
        public Nullable<long> EmployeeId { get; set; }

        [DisplayName("Application To")]
        public Nullable<long> ManagerId { get; set; }
        public Nullable<long> HrId { get; set; }
        [DisplayName("Reporting Head Status")]
        public Nullable<int> ManagerStatus { get; set; }
        [DisplayName("HR Status")]
        public Nullable<int> HrStatus { get; set; }
        [Required]
        [DisplayName("Reason")]
        public string Resion { get; set; }
        [DisplayName("Application For")]
        public string ApproveFor { get; set; }
        [Required]
        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> AttendenceDate { get; set; }
        [DisplayName("Application Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> ApplicationDate { get; set; }

        [DisplayName("Log-In Time")]
        public TimeSpan InTime { get; set; }

        [DisplayName("Log-Out Time")]
        public TimeSpan OutTime { get; set; }

        [DisplayName("Actual In Time")]
        public TimeSpan ModifiedInTime { get; set; }

        [DisplayName("Actual Out Time")]
        public TimeSpan ModifiedOutTime { get; set; }
        public string HrNote { get; set; }
        public string ManagerNote { get; set; }
        public Nullable<DateTime> FromDateForOnField { get; set; }
        public Nullable<DateTime> ToDateForOnField { get; set; }

        [Required]
        [DisplayName("On Field Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> OnFieldDate { get; set; }


        [DisplayName("Days")]
        public int TourDays { get; set; }
    }
}
