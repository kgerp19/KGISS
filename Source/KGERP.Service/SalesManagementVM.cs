using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service
{
    public class SalesManagementVM
    {
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "From Date")]
        public string StrFromDate { get; set; }

        [Display(Name = "To Date")]
        public string StrToDate { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        public string AchievementTitle { get; set; }
        public long AchievementId { get; set; }
        public int? RequisitionToCompanyId { get; set; }
        public IEnumerable<SelectListItem> SalesAcheivements { get; set; }
        public IEnumerable<SelectListItem> Companies { get; set; }
        public IEnumerable<SelectListItem> Unit { get; set; }
        public int CompanyId { get; set; }
        public decimal TargetAmount { get; set; }
        public string TargetAmountStr { get; set; }
        public decimal TotalTergetedAmount { get; set; }
        public string TotalTergetedAmountStr { get; set; }
        public string CompanyName { get; set; }
        public long KGcomTargetId { get; set; }
        public long KGCompanySaleTergetId { get; set; }
        public List<SalesManagementVM> DataList { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UnitId { get; set; }
        public List<KGCompanyMonthlySaleTergetVM> DataCmpanyWise { get; set; }
    }
    public class KGMonthlySalesTargetVM
    {
        public int SelectedProductId { get; set; }
    }
    public partial class KGCompanyMonthlySaleTergetVM
    {
        public long KGCompanyMonthlySaleTergetId { get; set; }
        public long KGCompanySaleTergetId { get; set; }
        public int Month { get; set; }
        public decimal MonthlySalesTergetedAmount { get; set; }
        public decimal MonthlyCpllectionTergetedAmount { get; set; }
        public Nullable<decimal> MonthlyTergetedQuantity { get; set; }
        public string CreatedBy { get; set; }
        public string Unitname { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public bool IsActive { get; set; }
  
        public decimal TergetedAmount { get; set; }
        public string Title { get; set; }
         public decimal SalesAchievementAmount { get; set; }
         public decimal SalesAchievementQty{ get; set; }
         public decimal recovery{ get; set; }




    }
    public class TargetAmountViewModel
    {
        public int Month { get; set; }
        public decimal MonthlySalesTergetedAmount { get; set; }
        public decimal MonthlyCollectionTergetedAmount { get; set; }
        public decimal TergetedQty{ get; set; }
        public long KGCompanySaleTergetId { get; set; }
        public int UnitId { get; set; }
    }
    public class SalesAchievementDetailViewModel
    {
        public int KGSalesAchievementDetailsId { get; set; }
        public decimal SalesAchievementAmount { get; set; }
        public decimal SalesAchievementQuantity { get; set; }
        public decimal RecoveryAchievementAmount { get; set; }
    }
    public partial class KGSalesAchivementDetailVm
    {
        public int OfficerAssignId { get; set; }
        public int CompanyId { get; set; }
        public long KGSalesAchievementDetailsId { get; set; }
        public long KGCompanyMonthlySaleTergetId { get; set; }
        public long EmployeeId { get; set; }
        public decimal GrossSalleryAmount { get; set; }
        public decimal SalesAchievementAmount { get; set; }
        public decimal SalesAchievementQuantity { get; set; }
        public decimal SalesTargetQuantity { get; set; }
        public decimal SalesTargetAount { get; set; }
        public decimal RecoveryAchievementAmount { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string UnitName { get; set; }
       
        public List<KGSalesAchivementDetailVm> DataList { get; set; }
        public string Title { get; set; }
        public string KgId { get; set; }
        public string EmName { get; set; }
        public int Month { get; set; }
        public decimal MonthlySalesTergetedAmount { get; set; }
        public decimal MonthlyCollectionTergetedAmount { get; set; }
  
        public decimal Salary { get; set; }
        public decimal EmpTarget { get; set; }
        public List<OffcierAssignVm> offcierAssignsList { get; set; }

    }

    public partial class OffcierAssignVm
    {
        public string EmployeeName { get; set; }
        public long Id { get; set; }
        public string EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public decimal EmpTarget { get; set; }
        public decimal TargetQty { get; set; }
        public decimal TargetCollection { get; set; }
    }

    
        public class EmployeeTargetViewModel
        {
            public int EmployeeId { get; set; }
            public decimal EmpTarget { get; set; }
            public decimal SalaryAmount { get; set; }
            public long KGCompanyMonthlySaleTergetId { get; set; }
        public decimal TargetQty { get; set; }
        public decimal TargetCollection { get; set; }

    }

    public partial class KGSalesCollectedAchivementVm
    {
        public long KGSalesCollectedAchivementId { get; set; }
        public long KGSalesAchievementDetailsId { get; set; }
        public System.DateTime AchivementDate { get; set; }
        public decimal SalesAchievementAmount { get; set; }
        public decimal SalesAchievementQuantity { get; set; }
        public decimal RecoveryAchievementAmount { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }

        public List<KGSalesCollectedAchivementVm> Details { get; set; }
    }

}
