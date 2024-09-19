using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment
{
    public class PRoll_CashPaymentViewModel
    {
        public long CashPaymentId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeIdKG { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal Amount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
        public IEnumerable<PRoll_CashPaymentViewModel> CashPaymentList { get; set; }
    }
}
