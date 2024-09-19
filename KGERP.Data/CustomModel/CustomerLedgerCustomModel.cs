using System;
using System.ComponentModel;

namespace KGERP.Data.CustomModel
{
    public class CustomerLedgerCustomModel
    {
        [DisplayName("Particulars")]
        public string TransactionType { get; set; }
        public int VendorId { get; set; }
        public string Customer { get; set; }
        [DisplayName("Customer Code")]
        public string CustomerCode { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Phone")]
        public string Phone { get; set; }
        [DisplayName("Opening Balance")]
        public decimal OpeningBalance { get; set; }
        [DisplayName("Date")]
        public DateTime Date { get; set; }
        [DisplayName("Challan No")]
        public string ChallanNo { get; set; }
        [DisplayName("Bill No")]
        public string BillNo { get; set; }
        [DisplayName("Quantity (Kg)")]
        public decimal Qty { get; set; }
        [DisplayName("Ref No")]
        public string PaymentRefNo { get; set; }
        [DisplayName("DR Amount")]
        public decimal DRAmount { get; set; }
        [DisplayName("CR Amount")]
        public decimal CRAmount { get; set; }

    }
}
