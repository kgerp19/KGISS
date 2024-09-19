using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.CustomModel
{
    public class AttendenceEntity
    {
        [DisplayName("Employee Id")]
        public string EmployeeId { get; set; }
        [DisplayName("Card No")]
        public string EmpCardNo { get; set; }
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Date")]
        public DateTime? AttendenceDate { get; set; }
        [DisplayName("In Time")]
        public TimeSpan? InTime { get; set; }
        [DisplayName("Out Time")]
        public TimeSpan? OutTime { get; set; }
        [DisplayName("Total Hour")]
        public TimeSpan? TotalHour { get; set; }
        [DisplayName("Shift Name")]
        public string ShiftName { get; set; }
        [DisplayName("Start Time")]
        public string StartTime { get; set; }
        [DisplayName("End Time")]
        public string EndTime { get; set; }
        [DisplayName("Status")]
        public string EmpStatus { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? ToDate { get; set; }

    }

    public class AttendanceVm
    {
        public string EmployeeId { get; set; }
        public long? ManagerId { get; set; }

        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public TimeSpan? InTime { get; set; }
        public TimeSpan? OutTime { get; set; }
        public string EmpStatus { get; set; }
        public IEnumerable<AttendanceVm> DataList { get; set; }
        public List<AttendanceVm> AttendenceList { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
    }
}