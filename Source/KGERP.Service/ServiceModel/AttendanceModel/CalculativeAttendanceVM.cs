using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel.AttendanceModel
{
    public class CalculativeAttendanceVM
    {
        public long Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set;}
        public int DesignationId { get; set; }
        public string DesignationName { get;set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int DaysInMonth { get; set; }
        public int DaysOfPayment { get; set; }
        public int TotalDays { get; set; }
        public int Weekend { get; set; }
        public int Holiday { get; set; }
        public int Leave { get; set; }
        public int LWP { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int WorkOutSite { get; set; }

        public int Late { get; set; }
        public int EarlyOut { get; set; }
        public int LateInAndEarlyOut { get; set; }

        public int PresentInHoliday { get; set; }

        public int LeaveWitoutPay { get; set; }
        public int AttendanceModified { get; set; }

        public int WorkingDay { get; set; }

        public DateTime JoiningDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public bool IsActive { get; set; }

    }
}
