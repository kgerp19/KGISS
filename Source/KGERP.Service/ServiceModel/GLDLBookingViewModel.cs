using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{


    public class CollactionBillViewModel
    {
        public long VoucherId { get; set; }
        public string ProductName { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long? CGId { get; set; }
        public int PaymentMasterId { get; set; }
        public long BookingId { get; set; }
        public bool IsFinalized { get; set; }
        public long InstallmentId { get; set; }
        public string BookingNo { get; set; }
        public string PaymentNo { get; set; }
        public int? HeadGLId { get; set; }
        public int CollactionType { get; set; }
        public decimal TotalCost { get; set; }
        public int? AccountingIncomeHeadId { get; set; }
        public int? Accounting_BankOrCashParantId { get; set; }
        public int? Accounting_BankOrCashId { get; set; }
        public int? BankChargeHeadGLId { get; set; }
        public string BankCashHeadName { get; set; }

        public string IntegratedFrom { get; set; }
        public decimal BankCharge { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionDateString { get; set; }
        public string MoneyReceiptNo { get; set; }
        public string ReceiveLocation { get; set; }
        public string ReferenceNo { get; set; }
        public string ChequeNo { get; set; }
        public decimal PayableAmount { get; set; }
        [DisplayName("Collected Amount")]
        public decimal InAmount { get; set; }
        public decimal RestofAmount { get; set; }
        public decimal DueAmount { get; set; }
        [Range(1,9999999999999999999, ErrorMessage = "Paid Amount  is Required!! not allow zero or nagetive .")]
        public decimal? AdjustmentAmount { get; set; }
        public decimal? TotalInstallment { get; set; }
        public decimal FullAmount { get; set; }
        public decimal ReceivableAmount { get; set; }
        public string CustomerGroupName { get; set; }
        public string PaymentFromHeadGLName { get; set; }
        public string BankChargeName { get; set; }
        public BookingInstallmentSchedule bookingInstallment { get; set; }
        public List<SceduleInstallment> Schedule { get; set; }
        public List<VMPaymentMaster> DataList { get; set; }
        public List<InstallmentScheduleShortModel> ScheduleVM { get; set; }
        public List<InstallmentSchedulePayment> PaymentList { get; set; }
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
    }
    public class GLDLBookingAttachment
    {
        public string Title { get; set; }
        public long DocId { get; set; } = 0;
        public HttpPostedFileBase  File { get; set; }
        public string  Extension { get; set; }
    }
    public class GLDLBookingViewModel
    {
        public string AccCode { get; set; }
        public DateTime CancelDate { get; set; }
        public string strCancelDate { get; set; }
        public string Remarks { get; set; }
        public string InstalmentRemarks { get; set; }
        public long CGId { get; set; }
        public long? ResaleLogId { get; set; }
        public int ActionId { get; set; }
        public int FileStatus { get; set; }
        public long CostId { get; set; }
        public decimal CostAmount {get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal DownPayment { get; set; }
        public decimal PaidDownPaymentAmt { get; set; }
        public DateTime? DownPaymentDate { get; set; }
        public DateTime? BookingMoneyPaymentDate { get; set; }
        public string DownPaymentNew { get; set; }
        public string DownPaymentDateNew { get; set; }
        public string BookingMoneyPaymentDateNew { get; set; }
        public int?  AcCostCenterId { get; set; }
        public long BookingId { get; set; }
        public string BookingNo { get; set; }
        //[Required]
        [GLDLFileNoRegEx(ErrorMessage = "The field must contain both ':' and '/'.")]

        public string FileNo { get; set; }
        public string FacingName { get; set; }
        public string Project { get; set; }
        public int? ProjectId { get; set; }
        public int? VendorId { get; set; }
        public string ProjectName { get; set; }
        public string UnitName { get; set; }
        public string KGRECompanyName { get; set; }
        [Required]
        [DisplayName("Select Client")]
        public string ClientName { get; set; }
        public string OtherInformation { get; set; }
        public int? CompanyId { get; set; }
        public int ClientId { get; set; }
        public int? PrimaryClientId { get; set; }
        public int EntryBy { get; set; }
        public int? Status { get; set; }
        public int? Step { get; set; }
        public string FileTitel { get; set; }
        public List<GLDLBookingAttachment> Attachments { get; set; }
        [Required]
        [Display(Name = "Team Lead")]
        public long TeamLeadId { get; set; }

        [Required]
        [Display(Name = "Client Primary Contact No")]
        public string PrimaryContactNo { get; set; }
        [Required]
        [Display(Name = "Client Primary Contact Address")]
        public string PrimaryContactAddr { get; set; }
        public string PrimaryEmail { get; set; }
        [Required]
        [Display(Name = "Sales Person")]
        public int EmployeeId { get; set; }
        public bool IsSubmited { get; set; }
        public string FullName { get; set; }
        public string SalesPerson { get; set; }
        public string SalesPersonPhone { get; set; }
        public string SalesPersonAddress { get; set; }
        public string SalesPersonEmail { get; set; }

        public string TeamLeadName { get; set; }
        public string TeamLeadPhone { get; set; }
        public string TeamLeadAddress { get; set; }
        public string TeamLeadEmail { get; set; }

        [Required]
        [Display(Name = "Customer Group Name")]
        public string CustomerGroupName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreateBy { get; set; }
        public List<SelectModel> CostCenters { get; set; }
        public List<SelectModel> Customers { get; set; }
        [Required]
        [Display(Name = "Project")]
        public int? ProductCategoryId { get; set; }
     
        public int? ProductSubCategoryId { get; set; }
        public string BlockName { get; set; }
        [Required]
        [Display(Name = "Plot")]
        public int? ProductId { get; set; }
        public string PlotName { get; set; }
        public string PlotNo { get; set; }

        [Required]
        [Display(Name = "Plot Size")]
        public double? PlotSize { get; set; }
        [Required]
        [Display(Name = "Rate Per Katha")]
        public decimal RatePerKatha { get; set; }

        public decimal LandValue { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal InstallmentSumOfAmount { get; set; }
        public decimal? SpecialDiscountAmt { get; set; } = 0;
        public double Discount { get; set; }
        public double SharePercentage { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrandTotalAmount { get; set; }
        [Display(Name = "Booking Amount")]
        public decimal BookingMoney { get; set; }
        public decimal PaidBookingMoney { get; set; }
        [Required]
        public string Customer { get; set; }
        public decimal AdvancePercentage { get; set; }
        [Display(Name = "Rest of Amount")]
        public decimal RestofAmount { get; set; }
        public decimal TotalBookingMoney { get; set; }
        public decimal TotalBookingMoneyPaid { get; set; }
        public int BookingInstallmentTypeId { get; set; }
        public int BookingInstallmentTypeManualId { get; set; }
        public string BookingInstallmentTitle { get; set; }
        public DateTime BookingInstallmentDate { get; set; }=DateTime.Now;
        public decimal BookingInstallmenAmmount { get; set; }
        public bool OneTime { get; set; }
        public bool IsShow { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? AmandmentDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public string BookingDateString { get; set; }
        public string ApplicationDateString { get; set; }
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
        public SelectList Employee { get; set; } = new SelectList(new List<object>());
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
        public List<CutomerListForBooking> Cutomers { get; set; }
        public List<GLDLBookingViewModel> datalist { get; set; }
        public List<InstallmentScheduleShortModel> Schedule { get; set; }
        public List<SelectModelType> pRM_Relations { get; set; }
        public List<BookingHeadServiceModel> LstPurchaseCostHeads { get; set; }
        public List<SelectDDLModel> CostHeads { get; set; }
        public List<SelectModelInstallmentType> BookingInstallmentType { get; set; }
       
        public List<SelectModel> ProductCategoryList { get; set; }
        public List<SelectModel> ProductSubCategoryListt { get; set; }
        public List<SelectModel> ProductListt { get; set; }
        public SelectList ProductSubCategoryList { get; set; } = new SelectList(new List<object>());
        public List<SelectModelType> ProjectList { get; set; }
        public CustomerNominee nominee { get; set; }
        public NomineeFile NomineeFile { get; set; }
        public ApprovalInfoViewModel approval { get; set; }
        public int? HeadGLId { get;  set; }
        public decimal TotalCost { get;  set; }
        public int? AccountingIncomeHeadId { get;  set; }
        public int? Accounting_BankOrCashParantId { get;  set; }
        public int? Accounting_BankOrCashId { get;  set; }
        public string IntegratedFrom { get; set; }
        public string CostHeadName { get; set; }
        public long InstallmentId { get; set; }
        public int ProductStatus { get;  set; }
        public DateTime PostingDate { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string PStatus { get; set; }
        public decimal TolalCollaction { get; set; }
        public int? SecondProductCateId { get; set; }=0;
        public int? isSeaPalace { get; set; }
        public SelectList GroupList { get; set; } = new SelectList(new List<object>());

    }
    public class CutomerListForBooking
    {
        public long MapId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorMobile { get; set; }
        public double SharePercentage { get; set; }
        public double TotalPercentage { get; set; }
        public Decimal Customerlandvalue { get; set; }
        public List<CustomerNominee> CustomerNominee { get; set; }

    }
    public class CustomerNominee
    {
        public long ImageDocId { get; set; }
        public long NIDDocId { get; set; }
        public  string ImageDocUrl { get; set; }
        public  string NIDDocUrl { get; set; }
        public int CustomerId { get; set; }
        public int? CompanyId { get; set; }
        [Required]
        [Display(Name = "Relation")]
        public int RelationId { get; set; }
        public string RelationName { get; set; }
        public string CustomerName { get; set; }
        public long NomineeId { get; set; }
        public long ProductId { get; set; }
        public long GroupId { get; set; }
        public long CGId { get; set; }
        [Required]
        [Display(Name = "Nominee Name")]
        public string NomineeName { get; set; }
        [Required]
        [Display(Name = "Nominee Mobile")]
        public string NomineeMobile { get; set; }
        [Display(Name = "Nominee Email")]
        public string NomineeEmail { get; set; }
        [Required]
        [Display(Name = "Nominee Percentage")]
        [Range(0.1, 100, ErrorMessage = "Percentage is Required!! not allow zero or nagetive .")]
        public double NomineeSharePercentage { get; set; }
        public int? Accounting_BankOrCashParantId { get; set; }
        public int? Accounting_BankOrCashId { get; set; }
    }
    public class NomineeFile
    {
        public int? CompanyId { get; set; }
        public long CNomineeId { get; set; }
        public long CGId { get; set; }
        public long ImageDocId { get; set; }
        public long NIDDocId { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public HttpPostedFileBase NIDFile { get; set; }
    }

    public class GLDLFileNoRegExAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string fieldValue = value.ToString();
                // Regular expression to check if the field contains both ":" and "/"
                if (!Regex.IsMatch(fieldValue, @"[:/]+"))
                {
                    return new ValidationResult("The field must contain both ':' and '/'.");
                }
            }
            return ValidationResult.Success;
        }
    }


    public class ApprovalInfoViewModel
    {
        public long? EntryBy { get; set; }
        public string EntryName { get; set; }
        public string EntryDesignation { get; set; }
        public string CheckedDesignation { get; set; }
        public string ApprovedDesignation { get; set; }
        public string FinalApproverDesignation { get; set; }

        public DateTime? EntryDate { get; set; }
        public DateTime? CheckDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public DateTime? FinalApvDate { get; set; }

        public long? CheckedBy { get; set; }
        public string CheckeName { get; set; }
        public long? ApprovedBy { get; set; }
        public string ApproveName { get; set; }
        public long? FinalApproverBy { get; set; }
        public string FinalApproveName { get; set; }
        public string StepName{ get; set; }
        public string StatusName{ get; set; }
        public string EntryBySignatureFileName { get; set; }
    }
}
