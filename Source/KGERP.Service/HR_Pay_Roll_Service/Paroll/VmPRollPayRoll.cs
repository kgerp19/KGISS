using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.ParollServices
{
    public partial class VmPRollPayRollDetail : VmPRollPayRoll
    {
        public long PayRollDetailId { get; set; }
        
        public long EmployeeId { get; set; }
        public int PaymentPurposeId { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
      
        public int CalculationType { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }

        public IEnumerable<VmPRollPayRollDetail> DataListDetail { get; set; }

       
    }

    public partial class VmPRollPayRoll
    {
        public long PayRollId { get; set; }
        public int CompanyId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public long PRollApprovalId { get; set; }
        public string Note { get; set; }
        public bool IsClose { get; set; }
        public bool IsTest { get; set; }
        public bool IsFestivalBonus { get; set; }
        public DateTime? FestivalDate { get; set; }
        public string FestivalDateStr { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<VmPRollPayRoll> DataList { get; set; }

    }


    public class VmSalaryDetails
    {
        public bool BankSheetFlag { get; set; }
        public bool EmpPaymentFlag { get; set; }

        public int BankBranchId { get; set; }
        public string SalaryDisbursementDate { get; set; }

        public SalaryReportTypeEnum SalaryReportType { get; set; }

        public string CompanyName { get; set; }
        public string EmployeeName { get; set; }
     
        public DateTime JoiningDate { get; set; }

        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }

        public string ServiceType { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeId { get; set; }
        public decimal GrossSalary { get; set; }

        public decimal TransportAddition { get; set; }
        public decimal SpecialAddition { get; set; }
        
        public decimal FineDeduction { get; set; }
        public decimal KMCSL { get; set; }
        public decimal RentDeduction { get; set; }
        
        public decimal BasicSalary { get; set; }
        public decimal HouseRent { get; set; }
        public decimal Medical { get; set; }
        public decimal Transportation { get; set; }
        public decimal MobileBill { get; set; }
        public decimal AbsentLateInOut { get; set; }
        public decimal Advance { get; set; }
        public decimal FoodBill { get; set; }
        public decimal Welfare { get; set; }
        public decimal Mobile { get; set; }
        public decimal Tax { get; set; }
        public decimal SelfContributionPF { get; set; }
        public decimal CompanyContributionPF { get; set; }
        public decimal CashPayment { get; set; }
        public int CompanyId { get; set; }
        public int ReportTypeId { get; set; }
        public long PayRollId { get; set; }
        public long? PRollSheetId { get; set; }
        public long Id { get; set; }
        
        public string   SheetName { get; set; }
        public decimal  TotalDeduction { get; set; }
        public decimal  NetPayment { get; set; }
        public decimal  BankPayment { get; set; }
        
        public List<VmSalaryDetails> SalaryList { get; set; }
        public int Index { get; set; }
    }
    public class PRoll_BonusVm

    {
        public long BonusId { get; set; }
        public int CompanyId { get; set; }
        public string BonusTitle { get; set; }
        public int BonusYear { get; set; }
        public int BonusMonth { get; set; }
        public bool IsClose { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public IEnumerable<PRoll_BonusVm> DataListDetail { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }



    }

    public class PRoll_AdvanceTypeVm
    {
        public int AdvanceTypeId { get; set; }
        public string AdvanceName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public int CompanyId { get; set; }
        public List<PRoll_AdvanceTypeVm> DataList { get; set; }
    }
   



}
