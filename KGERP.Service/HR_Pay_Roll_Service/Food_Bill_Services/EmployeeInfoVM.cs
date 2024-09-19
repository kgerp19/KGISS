using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services
{
    public class EmployeeInfoVM
    {
        public long Id { set; get; }
        public string Name { set; get; }
        public string EmployeeId { set; get; }
        public int NoOfLunch { get; set; }
        public int NoOfBreakfast { get; set; }
        public decimal MobileBill { get; set; }
        public decimal specialAddition { get; set; }
        public string Remarks { get; set; }
    }

    public class EmployeeInfoVMForFine
    {
        public long Id { set; get; }
        public string Name { set; get; }
        public string EmployeeId { set; get; }
        public int NoOfLunch { get; set; }
        public int NoOfBreakfast { get; set; }
        public decimal FineDeductionBil { get; set; }
    }

    public class JobDescriptionTypeVM
    {
        public int JobDescriptionTypeId { get; set; }
        public string Name { get; set; }
    }
}
