using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class GeneralLedgerPostingKgEcomModel
    {
        public string ButtonName
        {
            get
            {
                return Id > 0 ? "Update" : "Save";
            }
        }

        public long Id { get; set; }
        public long OID { get; set; }
        [DisplayName("Voucher No")]
        public string VoucherNo { get; set; }
        [DisplayName("Voucher Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> VoucherDate { get; set; }

        [DisplayName("Account Code")]
        public string AccountHeadCode { get; set; }
        [DisplayName("Account Name")]
        public string AccountHeadName { get; set; }
        [DisplayName("Pay Mode")]
        public string PayMode { get; set; }
        [DisplayName("Cheque Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> ChqDate { get; set; }
        [DisplayName("Cheque No")]
        public Nullable<double> ChqNo { get; set; }
        public string Amount { get; set; }
        [DisplayName("Debit Amount")]
        public Nullable<double> DebitAmount { get; set; }
        [DisplayName("Credit Amount")]
        public Nullable<double> CreditAmount { get; set; }
        public string OPD { get; set; }
        public string Bank { get; set; }
        [DisplayName("Transaction Type")]
        public string TransactionType { get; set; }
        public string Project { get; set; }
        [DisplayName("Cheque Name")]
        public string Chq_Name { get; set; }
        public string Ap { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public List<SelectModel> PayModes { get; set; }
        public List<SelectModel> Description { get; set; }
        public List<SelectModel> Projects { get; set; }

    }
}
