using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.KfmlInstallments
{
    public class IKfml_InstallmentViewModel
    {
        public int ClientId { get; set; }
        public int CompanyId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string ClientAddress{get; set; }
        public string BookingNo { get; set; }
        public int? AcCostCenterId { get; set; }
        public decimal ProductValue { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal? SpecialDiscountAmt { get; set; } = 0;
        public double Discount { get; set; }
        public double SharePercentage { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal InstallmentSumOfAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AdvancePercentage { get; set; }
        public decimal InterestPercentage { get; set; }
        public decimal InterestAmount { get; set; }
        [Display(Name = "Rest of Amount")]
        public decimal RestofAmount { get; set; }
        public decimal GrandTotalAmount { get; set; }
        [Display(Name = "Booking Amount")]
        public decimal BookingMoney { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? AmandmentDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public string BookingDateString { get; set; }
        public string ApplicationDateString { get; set; }
        public string FileNo { get; set; }
        public string Size { get; set; }
        public string ProductModel { get; set; }
        public string HorsePower { get; set; }
        public string UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal DeliveredQuantity { get; set; } = 1;
        public decimal COGSPrice { get; set; }
        public string Project { get; set; }
        public int? ProjectId { get; set; }
        public int ProductId { get; set; }
        public string ProductName{ get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public int ProductSubCategoryId { get; set; }
        public string ProductSubCategoryName { get; set; }
        public int BookingInstallmentTypeId { get; set; }
        public int BookingInstallmentTypeManualId { get; set; }
        public int? HeadGLId { get; set; }
        public int? AccountingIncomeHeadId { get; set; }

        public long EntryBy { get; set; }
        public long BookingId { get; set; }
        public long SoldBy { get; set; }
        public bool IsSubmitted { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string IntegratedFrom { get; set; }

        public int EmployeeId { get; set; }
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
        public SelectList Employee { get; set; } = new SelectList(new List<SelectDDLModel>());

        public List<BookingHeadServiceModel> LstPurchaseCostHeads { get; set; }
        public List<SelectDDLModel> CostHeads { get; set; }
        public List<SelectModelInstallmentType> BookingInstallmentType { get; set; }
        public List<SelectModel> ProductCategoryList { get; set; }
        public SelectList ProductSubCategoryList { get; set; } = new SelectList(new List<object>());
        public List<SelectModelType> ProjectList { get; set; }
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
        public BookingInstallmentSchedule bookingInstallment { get; set; }
        public List<SceduleInstallment> Schedule { get; set; }
        public List<VMPaymentMaster> DataList { get; set; }
        public List<InstallmentScheduleShortModel> ScheduleVM { get; set; }
        public List<InstallmentSchedulePayment> PaymentList { get; set; }
        public List<IKfml_InstallmentViewModel> kfmldatalist { get; set; }
        public int? AccountingStockHeadId { get;  set; }
    }
}
