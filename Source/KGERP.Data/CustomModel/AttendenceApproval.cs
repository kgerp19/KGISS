using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Data.CustomModel
{
    public class AttendenceApproval
    {

        public int Id { get; set; }
        [DisplayName("Employee Id")]
        public long EmployeeId { get; set; }

        public string Name { get; set; }
        public string UserId { get; set; }

        public long ManagerId { get; set; }
        [DisplayName("Manager Status")]
        public string ManagerStatus { get; set; }
        [DisplayName("HR Status")]
        public string HrStatus { get; set; }
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
        public Nullable<System.TimeSpan> InTime { get; set; }
        public Nullable<System.TimeSpan> OutTime { get; set; }
        [DisplayName("Modified InTime")]
        public Nullable<System.TimeSpan> ModifiedInTime { get; set; }
        [DisplayName("Modified OutTime")]
        public Nullable<System.TimeSpan> ModifiedOutTime { get; set; }
        public string InStatus { get; set; }
        public string OutStatus { get; set; }
        public string HrNote { get; set; }
        public string ManagerNote { get; set; }

        [DisplayName("From Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> FromDateForOnField { get; set; }

        [DisplayName("To Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> ToDateForOnField { get; set; }

        [DisplayName("Days")]
        public int TourDays { get; set; }

        public IEnumerable<AttendenceApproval> DataList { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }

        #region New Added
        public string EmployeeKGId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicationType { get; set; }
        public string Designation { get; set; }
        public string ReasonAndLocation { get; set; }
        public int ComID { get; set; }
        public int LeaveStatus { get; set; }
        public string TableName { get; set; }
        #endregion
    }
}
