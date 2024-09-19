using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.SpecialAddition
{
    public class SpecialAdditionDetailsViewModel
    {
        public long SpecialAdditionDetailId { get; set; }
        public long SpecialAdditionId { get; set; }
        public long EmployeeId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public SelectList EmpList { get; set; } = new SelectList(new List<object>());
        public int Month { get; set; }
        public int Year { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeKgId { get; set; }


    }
}
