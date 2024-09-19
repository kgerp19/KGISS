using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel.RealState
{
  public  class MoneyReceiptViewModel
    {
        public long MoneyReceiptId { get; set; }
        public long MoneyReceiptDetailId { get; set; }
        public List<string> InstallmentId { get; set; }
        public MultiSelectList InstallmentList { get; set; }
        public int? HeadGLId { get; set; }
        public decimal BankCharge { get; set; }
        public int BankChargeAccHeahId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public String MoneyReceiptNo { get; set; }
        public DateTime? MoneyReceiptDate { get; set; }
        public DateTime? ChequeDate { get; set; }
        public DateTime? Postingdate { get; set; }
        public DateTime? Submitdate { get; set; }
        public string StringSubmidtate { get; set; }
        public string ClientName { get; set; }
        public string ReceivedBy { get; set; }
        public string ChequeNo { get; set; }
        public string ReceiptDateString { get; set; }
        public string ChequeDateString { get; set; }
        public long? CGId { get; set; }  
        public long BookingId { get; set; }  
        public long ClientId { get; set; }
        public int? ProjectId { get; set; }
        public List<string> Against { get; set; }

        public string AgainstString { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string CompanyName { get; set; }
        public string CollectionFrom { get; set; }
        public int CompanyId{ get; set; }
        public int? PayTypeId{ get; set; }
        public Decimal? Amount { get; set; }
        public Decimal? RefAmount { get; set; }
        
        public Decimal? TotalAmount { get; set; }
        public Decimal? PaidAmount { get; set; }
        public string ReceivedType { get; set; }
        public string PayTypeName { get; set; }
        public string ProjectName { get; set; }

        public string BookingNo { get; set; }
        public string PlotName { get; set; }
        public string PlotNo { get; set; }
        public string FileNo { get; set; }
        public string BlockName { get; set; }

        public string Particular { get; set; }
        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }
        public bool? IsShort { get; set; }
        public bool? IsSubmitted { get; set; }
        [DisplayName("Booking Amount")]
        public bool IsBookingAmount { get; set; }
        [DisplayName("DownPayment Amount")]
        public bool IsDownPayment { get; set; }
        [DisplayName("Existing Money Receipt")]
        public bool IsExisting { get; set; }
        [DisplayName("Disallow the check if the money receipt already exists. Otherwise, allow the check and provide a message to our client.")]
        public bool IsApproveSMS { get; set; }

        public List<SelectModelType> MemMoneyReceiptType { get; set; }
        public List<SelectModelType> BankList { get; set; }
        public List<SelectModelType> ProjectList { get; set; }
        public SelectList GroupList { get; set; } = new SelectList(new List<object>());
        public List<SelectModel> PayType { get; set; }
        public List<MoneyReceiptViewModel> MoneyReceiptList { get; set; }
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
        public int? Accounting_BankOrCashParantId { get; set; }
        public int? Accounting_BankOrCashId { get; set; }
        public string AccountingHeadName { get; set; }
        public int Indecator { get;  set; }
        public long CollectionFromId { get;  set; }
        public int VoucherTypeId { get;  set; }
        public string IntegratedFrom { get; set; }
        public string Message { get; set; }
        public int? CostCenterId { get; set; }
    }
}
