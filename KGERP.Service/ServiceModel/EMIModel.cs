using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class EMIModel
    {
        public long EmiId { get; set; }
        [DisplayName("EMI No")]
        public string EMINo { get; set; }
        [DisplayName("Invoice No")]
        public Nullable<long> OrderId { get; set; }
        [DisplayName("Sale Value")]
        public Nullable<decimal> SaleValue { get; set; }
        [DisplayName("DP(%)")]
        [Required]
        [Range(25, 30, ErrorMessage = "Enter number between 25 to 30")]
        public Nullable<decimal> Dp { get; set; }
        [DisplayName("Down Payment")]
        public Nullable<decimal> DpValue { get; set; }
        [DisplayName("Outstanding Principle")]
        public Nullable<decimal> OutStandingPrinciple { get; set; }
        [DisplayName("No of Installment")]
        [Required]

        public Nullable<int> NoOfInstallment { get; set; }
        [Required]
        [DisplayName("Flat Rate(%)")]
        [Range(11.5, 14, ErrorMessage = "Enter number between 11.5 to 14")]
        public Nullable<decimal> FlatRatePerYear { get; set; }
        [DisplayName("Bank Charge")]
        public Nullable<decimal> BankCharge { get; set; }
        [DisplayName("Net Outstanding")]
        public Nullable<decimal> NetOutStanding { get; set; }
        [DisplayName("Installment Amount")]
        public Nullable<decimal> InstallmentAmount { get; set; }
        [DisplayName("Customer Code")]
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Installment Start Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> InstallmentStartDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }


        public virtual OrderMasterModel OrderMaster { get; set; }
        public virtual ICollection<EmiDetailModel> EmiDetails { get; set; }
        public virtual VendorModel Vendor { get; set; }
    }
}
