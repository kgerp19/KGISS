using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class PaymentModel
    {
        public int PaymentId { get; set; }
        public int CompanyId { get; set; }
        public string ProductType { get; set; }
        [DisplayName("Customer")]
        public int VendorId { get; set; }
        [DisplayName("Payment Mode")]
        [Required(ErrorMessage = "Please select payment method")]
        public int PaymentModeId { get; set; }


        [DisplayName("Reference No.")]
        [Required(ErrorMessage = "Reference no is required")]
        public string ReferenceNo { get; set; }
        [DisplayName("Transaction Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime TransactionDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string PaymentMethodName { get; set; }
        public virtual VendorModel Vendor { get; set; }
        [DisplayName("Credit")]
        [Range(100, Double.MaxValue, ErrorMessage = "Minimum amount  BDT 100TK")]
        public decimal InAmount { get; set; }
        [DisplayName("Debit")]
        public Nullable<decimal> OutAmount { get; set; }

        [DisplayName("Bank")]
        public Nullable<int> BankId { get; set; }
        [DisplayName("Deposit Type")]
        public string DepositType { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("M.R No")]
        public string MoneyReceiptNo { get; set; }
        [DisplayName("Received Location")]
        public string ReceiveLocation { get; set; }

        [DisplayName("Cheque No.")]
        public string ChequeNo { get; set; }

        public bool IsActive { get; set; }
        public long? OrderMasterId { get; set; }
        public long? PurchaseOrderId { get; set; }
        //--------------------Extented Properties-----------------------------
        public string PaymentMode { get; set; }
        public string Customer { get; set; }
        public string CustomerCode { get; set; }
        public string Bank { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<PaymentModel> DataList { get; set; }
    }
}
