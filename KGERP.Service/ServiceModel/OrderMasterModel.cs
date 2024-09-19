using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
    public class OrderMasterModel
    {
        public int Status { get; set; }
       
        public long OrderMasterId { get; set; }
        [Required]
        [DisplayName("Order Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string ProductType { get; set; }
        [Required]
        [DisplayName("Expected Delivery Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ExpectedDeliveryDate { get; set; }
        public string OrderMonthYear { get; set; }
        public int CompanyId { get; set; }
        [DisplayName("Order No")]
        [Required]
        public string OrderNo { get; set; }
        [Required(ErrorMessage = "Select Customer")]
        [DisplayName("Customer")]
        public int CustomerId { get; set; }
        [DisplayName("Total Amount")]
        public Nullable<decimal> TotalAmount { get; set; }
        [DisplayName("Sale Person")]
        [Required(ErrorMessage = "Select a responsible marketing officer")]
        public Nullable<long> SalePersonId { get; set; }
        [DisplayName("Order Location")]
        [Required]
        public int StockInfoId { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifeidDate { get; set; }
        public bool IsCash { get; set; }
        public bool IsActive { get; set; }
        public bool IsSubmitted { get; set; }
        [DisplayName("Status")]
        public string OrderStatus { get; set; }
        [DisplayName("Grand Total")]
        public decimal GrandTotal { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }

        public virtual VendorModel Vendor { get; set; }
        public virtual IList<OrderDetailModel> OrderDetails { get; set; }
        public virtual ICollection<OrderDeliverModel> OrderDelivers { get; set; }
        public virtual CompanyModel Company { get; set; }
        //--------------------------------------Extended-------------
        public string OrderDateString { get; set; }
        [Required]
        [DisplayName("Sale Person")]
        public string SalePersonName { get; set; }
        [Required]
        public string Customer { get; set; }
        [DisplayName("Customer Address")]
        public string CustomerAddress { get; set; }
        [DisplayName("Customer Phone")]
        public string CustomerPhone { get; set; }
        [DisplayName("Challan No")]
        public string ChallanNo { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }
        public int NoOfChild { get; set; }
        public int AccountHeadId { get; set; }
        [DisplayName("Delivery Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime? DeliveryDate { get; set; }
        public long OrderDeliverId { get; set; }

        public List<SelectModel> MarketingOfficers { get; set; }
        public List<SelectModel> OrderLocations { get; set; }

        public List<SelectModel> Products { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<OrderMasterModel> DataList { get; set; }
        public bool IsSeen { get; set; }

        public EnumFeedSalesStatus EnumStatus { get { return (EnumFeedSalesStatus)this.Status; } }
        public string EnumStatusName { get { return BaseFunctionalities.GetEnumDescription(this.EnumStatus); } }
        public SelectList EnumStatusList { get { return new SelectList(BaseFunctionalities.GetEnumList<EnumFeedSalesStatus>(), "Value", "Text"); } }
    }
}
