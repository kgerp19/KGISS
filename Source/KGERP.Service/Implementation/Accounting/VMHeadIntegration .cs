using KGERP.Service.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.Accounting
{
    public class VMHeadIntegration : BaseVM
    {
        
        public int ParentId { get; set; }
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public int OrderNo { get; set; }
        public int? LayerNo { get; set; }
        public bool IsIncomeHead { get; set; }
        public string ProductType { get; set; }

    }
    public class VMJournal : BaseVM
    {
        public string VoucherNo { get; set; }
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public string Narration { get; set; }
        public bool Finalized { get; set; } = false;
        public bool Approved { get; set; } = false;
        public string ChqName { get; set; }
        public decimal BillofExchangeValue { get; set; }
        public decimal dollarRate { get; set; }

        public int? Accounting_CostCenterFK { get; set; }
        public int? ProductCategory { get; set; }
        public int Accounting_BankOrCashParantId { get; set; }

        public int? Accounting_BankOrCashId { get; set; }
        public string BankOrCashNane { get; set; }

        public string Accounting_CostCenterName { get; set; }

        public decimal BDT_Value { get; set; }
        public decimal ForeignCurrency_Value { get; set; }
        public IEnumerable<VMJournal> DataList { get; set; }


        public decimal ForeignCurrencyAmount { get; set; } = 0;
        public decimal ConversionRateToBDT { get; set; } = 0;

        public bool isRealization { get; set; } = false;
        public bool IsVirtual { get; set; } = false;

        //public string Coce { get; set; }

        public int ERAccounting_HeadFK { get; set; }

    }

    public class VMJournalSlave : VMJournal
    {
         
        public long? VoucherDetailId { get; set; }
        public long VoucherId { get; set; }


        public VMJournal vMJournal { get; set; }

        public string Status { get; set; }
        [Required(ErrorMessage = "The Accounts Head Field Is Required")]
        public int Accounting_HeadFK { get; set; }
        [Required]
        public string AccountingHeadName { get; set; }
        public string Particular { get; set; }

        [Required]
        public int VoucherTypeId { get; set; }

        public double Debit { get; set; } = 0;
        public double Credit { get; set; } = 0;
        public decimal TotalDebit { get; set; } = 0;
        public decimal TotalCredit { get; set; } = 0;
        public IEnumerable<VMJournalSlave> DataListDetails { get; set; }
        public List<VMJournalSlave> DataListSlave { get; set; }

        public string message { get; set; }
        public string ChqNo { get; set; }
        public bool IsStock { get; set; }
        public bool IsSubmit { get; set; }
        public string ProductCategoryName { get; set; }
        public DateTime? ChqDate { get; set; }
        public bool isDocumentPayment { get; set; }
        public SelectList VoucherTypesList { get; set; } = new SelectList(new List<object>());
        public SelectList CostCenterList { get; set; } = new SelectList(new List<object>());
        public SelectList ProductCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        
    }
}
