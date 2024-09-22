using System;

namespace KGERP.Data.CustomModel
{
    public class LeaveEmailCustomModel
    {

        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string ManagerEmail { get; set; }
        public string HRAdminEmail { get; set; }
        public string LeaveCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ApplyDate { get; set; }
        public string HrStatus { get; set; }
        public string HrName { get; set; }
        public string ManagerStatus { get; set; }
        public string ManagerName { get; set; }
        public int? LeaveDays { get; set; }
    }
}
