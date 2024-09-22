using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services
{
    public class FoodBillDetaliesViewModel
    {
        public long FoodBillDetailId { get; set; }
        public long FoodBillId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeKgId { get; set; }
        public int NoOfLunch { get; set; }
        public int NoOfBreakfast { get; set; }
        public SelectList EmpList { get; set; } = new SelectList(new List<object>());

    }
}
