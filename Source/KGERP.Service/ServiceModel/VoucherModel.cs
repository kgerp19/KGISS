using KGERP.Service.Implementation.Accounting;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
    public class VoucherModel
    {
        public long VoucherId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [Required]
        [DisplayName("Voucher Type")]
        public Nullable<int> VoucherTypeId { get; set; }
        [Required]
        [DisplayName("Voucher No.")]
        public string VoucherNo { get; set; }
        [Required]
        [DisplayName("Voucher Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> VoucherDate { get; set; }
        [DisplayName("Cheque No")]
        public string ChqNo { get; set; }
        [DisplayName("Cheque Name")]
        public string ChqName { get; set; }
        [DisplayName("Cheque Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ChqDate { get; set; }
        [DisplayName("Project / Cost Center")]
        public int Accounting_CostCenterFk { get; set; }
        [DisplayName("Narration")]
        public string Narration { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public bool IsActive { get; set; }
        public string VoucherTypeName { get; set; }
        public string CostCenterName { get; set; }

        public virtual VoucherTypeModel VoucherType { get; set; }
        public virtual ICollection<VoucherDetailModel> VoucherDetails { get; set; }
        public IEnumerable<VoucherModel> DataList { get; set; }

        ////-------------------Extented Properties------------------------------
        [DisplayName("Account Code")]
        public int AccountHeadId { get; set; }
        public string AccountHeadName { get; set; }
        [DisplayName("DEBIT (TAKA)")]
        public Nullable<double> DebitAmount { get; set; }
        [DisplayName("CREDIT (TAKA)")]
        public Nullable<double> CreditAmount { get; set; }
        [DisplayName("Particular")]
        public string Particular { get; set; }
        public bool IsStock { get; set; }
        public bool? IsSubmit { get; set; }
        public bool IsIntegrated { get; set; }
        public bool IsApproved { get; set; }
        public long ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public int? VmVoucherTypeId { get; set; }

        public SelectList VoucherTypesList { get; set; } = new SelectList(new List<object>());
        public List<SelectModel> VoucherTypes { get; set; }
        public VMJournalSlave vM { get; set; }
        public long ReportApprovalId { get; set; } = 0;
    }
}
