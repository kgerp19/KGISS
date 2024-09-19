using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class VendorModel
    {
        public string ButtonName
        {
            get
            {
                return VendorId > 0 ? "Update" : "Save";
            }
        }

        [DisplayName("A/C Head")]
        public Nullable<int> HeadGLId { get; set; }
        [DisplayName("District")]
        [Required(ErrorMessage = "Please select District")]
        public Nullable<int> DistrictId { get; set; }
        [Required(ErrorMessage = "Please select Upazila")]
        [DisplayName("Upazilla")]
        public Nullable<int> UpazilaId { get; set; }
        [DisplayName("Customer Type")]
        public string CustomerType { get; set; }
        [DisplayName("Credit Commission")]
        public Nullable<decimal> CreditCommission { get; set; }
        public int VendorId { get; set; }
        public int VendorTypeId { get; set; }
        [DisplayName("Company")]
        public int CompanyId { get; set; }
        [DisplayName("Company Name")]
        public string Name { get; set; }
        [DisplayName("A/C Number")]
        public string Code { get; set; }
        [DisplayName("Proprietor Name")]
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string DistrictName { get; set; }
        public string UpazilaName { get; set; }
        public string ThanaName { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(@"(^([+]{1}[8]{2}|0088)?(01){1}[5-9]{1}\d{8})$", ErrorMessage = "Please enter valid phone number")]
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        [DisplayName("Due")]
        public decimal? PaymentDue { get; set; }

        [DisplayName("Opening Balance")]
        public decimal OpeningBalance { get; set; }
        [DisplayName("Zone")]
        public int? ZoneId { get; set; }
        [DisplayName("Sub Zone")]
        public int? SubZoneId { get; set; }

        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        [DisplayName("Carrying Commission")]
        public bool IsCarrying { get; set; }
        [DisplayName("Nominee Name")]
        public string NomineeName { get; set; }
        [DisplayName("Nominee Phone")]
        public string NomineePhone { get; set; }
        [DisplayName("Proprietor Image")]
        public string ImageUrl { get; set; }
        [DisplayName("Nominee Image")]
        public string NomineeImageUrl { get; set; }
        [DisplayName("Monthly Target")]
        public Nullable<decimal> MonthlyTarget { get; set; }

        [DisplayName("Yearly Target")]
        public Nullable<decimal> YearlyTarget { get; set; }
        public int CreditRatioFrom { get; set; }
        public int CreditRatioTo { get; set; }
        [DisplayName("Credit Limit")]
        public Nullable<decimal> CreditLimit { get; set; }
        [DisplayName("Monthly Incentive")]
        public decimal MonthlyIncentive { get; set; }
        [DisplayName("Yearly Incentive")]
        public decimal YearlyIncentive { get; set; }
        [DisplayName("Closing Time")]
        public string ClosingTime { get; set; }
        public string Condition { get; set; }
        [DisplayName("Country")]
        public Nullable<int> CountryId { get; set; }
        public string State { get; set; }
        [DisplayName("NID No")]
        public string NID { get; set; }
        public string BIN { get; set; }
        [DisplayName("No of Check")]
        public Nullable<int> NoOfCheck { get; set; }
        [DisplayName("Check No")]
        public string CheckNo { get; set; }
        public string ResponsiblePerson { get; set; }
        public virtual DistrictModel District { get; set; }
        public virtual UpazilaModel Upazila { get; set; }
        public virtual ZoneModel Zone { get; set; }
        public virtual SubZoneModel SubZone { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }
        public virtual ICollection<VendorOfferModel> VendorOffers { get; set; }
        public virtual CountryModel Country { get; set; }
        [DisplayName("Guarantor Name")]
        public string GuarantorName { get; set; }
        public string GurantorAddress { get; set; }
        [DisplayName("Guarantor Mobile No")]
        public string GurantorMobileNo { get; set; }
    public List<VendorModel> List { get; set; }


        //-------------------Additional Field for View-----------

        public VendorModel()
        {
            ImageUrl = "~/Images/VendorImage/default.png";
            NomineeImageUrl = "~/Images/VendorImage/default.png";
        }
        public HttpPostedFileBase UploadedFile { get; set; }
        public HttpPostedFileBase VendorImageUpload { get; set; }
        public HttpPostedFileBase NomineeImageUpload { get; set; }
        public string SupplierOrCustomer { get; set; }

        [DisplayName("Last Payment Date")]
        public System.DateTime? LastPaymentDate { get; set; }
        [DisplayName("Balance")]
        public decimal? Balance { get; set; }
        public string ZoneName { get; set; }
        public string CompanyName { get; set; }
        public string Message { get; set; }
        public int StockInfoId { get; set; }

        public IEnumerable<VendorModel> DataList { get; set; }
        public decimal SecurityAmount { get;  set; }
        public int CustomerStatus { get;  set; }
        public string Propietor { get;  set; }
        public string CountryName { get;  set; }
    }
    public class VendorDeedListVm
    {
        public int CompanyId { get; set; }
        public IEnumerable<VendorDeedVm> DataList { get; set; }
    }

    public class VendorDeedVm
    {
        public int VendorDeedId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int CompanyId { get; set; }
        public int MonthlyTarget { get; set; }
        public int YearlyTarget { get; set; }
        public int CreditRatioFrom { get; set; }
        public int CreditRatioTo { get; set; }
        public int CreditLimit { get; set; }
        public int Days { get; set; }
        public decimal Transport { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ClosingDateText { get; set; }

        public int ExtraCondition1 { get; set; }
        public decimal ExtraBenifite { get; set; }
        public decimal DepositRate { get; set; }

    }
  




    }
