using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service
{
    public class MobileBillDetaliesViewModel
    {
        public long MobileBillDetailId { get; set; }
        public long MobileBillId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeKgId { get; set; }
        public string EmployeeName { get; set; }
        public decimal? Amount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public SelectList EmpList { get; set; } = new SelectList(new List<object>());
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
