using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Data.Models
{
    [MetadataType(typeof(AttendenceApproveApplicationMetadata))]
    public partial class AttendenceApproveApplication
    {

    }

    public class AttendenceApproveApplicationMetadata
    {
        public int Id { get; set; }
        [DisplayName("Employee Id")]
        public Nullable<long> EmployeeId { get; set; }

        [DisplayName("Application To")]
        public Nullable<long> ManagerId { get; set; }
        [DisplayName("Reporting Head Status")]
        public Nullable<int> ManagerStatus { get; set; }
        [DisplayName("HR Status")]
        public Nullable<int> HrStatus { get; set; }
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

        [DisplayName("Log-In Time")]
        public TimeSpan InTime { get; set; }

        [DisplayName("Log-Out Time")]
        public TimeSpan OutTime { get; set; }


        public TimeSpan ModifiedInTime { get; set; }


        public TimeSpan ModifiedOutTime { get; set; }
    }
}
