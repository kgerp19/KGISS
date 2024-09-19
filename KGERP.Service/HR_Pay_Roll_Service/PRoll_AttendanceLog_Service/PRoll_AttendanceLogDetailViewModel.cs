using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AttendanceLog_Service
{
    public class PRoll_AttendanceLogDetailViewModel
    {
        public long AttendanceLogDetail { get; set; }
        public long AttendanceLogId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string GradeName { get; set; }
        public int CompanyId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int EarlyOut { get; set; }
        public int LateIn { get; set; }
        public int LateInEarlyOut { get; set; }
        public int Absent { get; set; }
        public int OnTime { get; set; }
        public int Present { get; set; }
        public int Leave { get; set; }
        public int LeaveWithoutPay { get; set; }
        public int OnField { get; set; }
        public int Tour { get; set; }
        public int OffDay { get; set; }
        public int Holiday { get; set; }
        public int DeductedDay { get; set; }
        public int PayableDay { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}

