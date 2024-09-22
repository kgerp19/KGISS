using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class KGREPaymentModel
    {
        public int PaymentId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string PaymentMethodName { get; set; }
        [DisplayName("File No")]
        public string FileNo { get; set; }
        [DisplayName("Project")]
        public string ProjectName { get; set; }
        [DisplayName("Client Name")]
        public string FullName { get; set; }
        [DisplayName("Project")]
        [Required(ErrorMessage = "Please select Project")]
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> PlotId { get; set; }
        public Nullable<int> InstallmentId { get; set; }

        [DisplayName("Transaction Date")]
        [Required(ErrorMessage = "Please input TransactionDate")]
        public Nullable<System.DateTime> TransactionDate { get; set; }

        [DisplayName("Payment For")]
        [Required(ErrorMessage = "Please select payment for")]
        public Nullable<int> PaymentFor { get; set; }
        [DisplayName("Payment For")]
        public string Payment_For { get; set; }
        [DisplayName("Amount")]
        [Required(ErrorMessage = "Please Type Amount")]
        public Nullable<double> InAmount { get; set; }
        public Nullable<int> BankId { get; set; }
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Chk No")]

        public string ChkNo { get; set; }
        [DisplayName("Chk Name")]
        public string ChkName { get; set; }
        public Nullable<System.DateTime> ChkDate { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [DisplayName("Payment Mode")]
        [Required(ErrorMessage = "Please select payment method")]
        public int PaymentModeId { get; set; }
        [DisplayName("Payment Mode")]
        public string PaymentMode { get; set; }
        [DisplayName("Money Receipt No")]

        public string MoneyReceiptNo { get; set; }
        public Nullable<double> OutAmount { get; set; }
        [DisplayName("Product Type")]
        public string ProductType { get; set; }

        [DisplayName("Transaction Type")]
        [Required(ErrorMessage = "Please Input Transaction Type")]
        public string TransactionType { get; set; }

        public List<KGREPaymentModel> DataList { get; set; }

    }
}
