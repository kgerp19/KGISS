using KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_SalaryConfiguration_Services
{
    public class PRoll_SalaryConfigurationViewModel
    {
        public long SalaryConfigurationId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Basic { get; set; }
        public decimal HouseRent { get; set; }
        public decimal Medical { get; set; }
        public decimal Transportation { get; set; }
        public decimal MobileBill { get; set; }
        public decimal Welfare_D { get; set; }
        public decimal ProvidentFund_D { get; set; }
        public decimal Rent_D { get; set; }
        public decimal TransportationAddition { get; set; }
        public decimal Tax_D { get; set; }
        public decimal Remaining { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsBonusAllowed { get; set; }
        public bool IsActive { get; set; }
        public decimal Gross { get; set; }
        public long? RollSheetId { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectListItem> DDLPayrollShit { get; set; }
        public IEnumerable<PRoll_SalaryConfigurationViewModel> SalaryConfigurationList { get; set; }
    }
}
