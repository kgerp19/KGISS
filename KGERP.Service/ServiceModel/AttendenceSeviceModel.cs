using System;

namespace KGERP.Service.ServiceModel
{
    class AttendenceSeviceModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmpCardNo { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> AttendenceDate { get; set; }
        public Nullable<System.TimeSpan> InTime { get; set; }
        public Nullable<System.TimeSpan> OutTime { get; set; }
        public Nullable<System.TimeSpan> TotalHour { get; set; }
        public string ShiftName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string EmpStatus { get; set; }
    }
}
