using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AdvanceCash
{
    public class PRoll_AdvanceViewModel
    {
        public long AdvanceId { get; set; }
        public DateTime AdvanceDate { get; set; }
        public string AdvanceDateString { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string AdvanceTypeName { get; set; }
        public int CompanyId { get; set; }
        public int InstallmentTypeId { get; set; }
        public int AdvanceTypeId { get; set; }
        public int NoOfInstallment { get; set; }
        public string CompanyName { get; set; }
        public DateTime InstallmentStartDate { get; set; }
        public string InstallmentStartDateString { get; set; }
        public decimal TotalAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<SelectModelInstallmentType> InstallmentType { get; set; }
        public List<SelectModelAdvanceType> AdVanceType { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SceduleInstallment> Schedule { get; set; }
        public List<PRoll_AdvanceDetalisViewModel> models { get; set; } 
        public IEnumerable<PRoll_AdvanceViewModel> viewModels { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
    }

    public class VMAdvanceType
    {
        public int AdvanceTypeId { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public int? CompanyId { get; set; }
        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;

        public IEnumerable<VMAdvanceType> DataList { get; set; }
    }
}
