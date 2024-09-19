using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class SaleReturnModel
    {
        public SaleReturnModel()
        {
                
        }
        public long SaleReturnId { get; set; }
        [DisplayName("Delivery Invoice")]
        public Nullable<long> OrderDeliverId { get; set; }
        public int CompanyId { get; set; }
        public string ProductType { get; set; }
        [DisplayName("Return Invoice")]
        public string SaleReturnNo { get; set; }
        [DisplayName("Customer")]
        public int CustomerId { get; set; }
        [DisplayName("Warehouse Name")]
        public int StockInfoId { get; set; }
        public string CreatedBy { get; set; }
        [DisplayName("Return Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ReturnDate { get; set; }
        public string ReturnDateText { get; set; }

        [DisplayName("Received By")]
        public long ReceivedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [Required]
        [DisplayName("Return Reason")]
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual OrderDeliverModel OrderDeliver { get; set; }
        public virtual ICollection<SaleReturnDetailModel> SaleReturnDetails { get; set; }
        public  IEnumerable<SaleReturnDetailModel> ItemList { get; set; }
        public virtual VendorModel Vendor { get; set; }

        //---------------Extended Model------------------
        
        public string ReceiverName { get; set; }
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }
        public string ProprietorName { get; set; }

        public string ProprietorAddress { get; set; }

        public string ProprietorPhone { get; set; }
        public string WareHouseName { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<SaleReturnModel> DataList { get; set; }
        public List<SelectModel> StockInfos { get; set; }
        public List<SelectModel> Invoices { get; set; }
        public long VoucherId { get; set; }
    }
}
