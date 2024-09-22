using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class OrderDeliverModel
    {
        public long OrderDeliverId { get; set; }
        [DisplayName("Order No")]
        public Nullable<long> OrderMasterId { get; set; }
        [DisplayName("Product Type")]
        public string ProductType { get; set; }
        [DisplayName("Stock")]
        public int StockInfoId { get; set; }
        [DisplayName("Challan No.")]
        public string ChallanNo { get; set; }
        [DisplayName("Vehicle No")]
        public string VehicleNo { get; set; }
        [DisplayName("Bill No")]
        public string InvoiceNo { get; set; }

        [DisplayName("Driver Name")]
        public string DriverName { get; set; }
        [DisplayName("Total Amount")]
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        [DisplayName("Discount Rate")]
        public decimal DiscountRate { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DeliveryDate { get; set; }

        public decimal Commission { get; set; }
        public decimal Carrying { get; set; }
        public decimal SpecialDiscount { get; set; }
        [DisplayName("Depo Invoice No")]
        public string DepoInvoiceNo { get; set; }

        public virtual ICollection<OrderDeliverDetailModel> OrderDeliverDetails { get; set; }

        public virtual OrderMasterModel OrderMaster { get; set; }
        public virtual StockInfoModel StockInfo { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifedDate { get; set; }

        public Nullable<int> CompanyId { get; set; }

        //------------------External Properties----------------
        public String OrderStatus { get; set; }
        public long OrderDetailId { get; set; }

    }
}
