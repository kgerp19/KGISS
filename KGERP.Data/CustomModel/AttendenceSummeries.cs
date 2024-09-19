using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Data.CustomModel
{
    public class AttendenceSummeries
    {
        [DisplayName("Id")]
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public int? Absent { get; set; }
        [DisplayName("Late In")]
        public int? LateIn { get; set; }
        [DisplayName("Early Out")]
        public int? EarlyOut { get; set; }
        [DisplayName("Late In & Early Out")]
        public int? LateInEarlyOut { get; set; }
        [DisplayName("Present")]
        public int? OK { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<AttendenceSummeries> DataList { get; set; }
    }
}
