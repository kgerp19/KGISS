using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Data.CustomModel
{
    public class OrderDeliverCustomModel
    {
        public long OrderMasterId { get; set; }
        [DisplayName("Order No")]
        public string OrderNo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Order Date")]
        public DateTime? OrderDate { get; set; }
        public int CustomerId { get; set; }
        [DisplayName("Customer")]
        public string Customer { get; set; }
        public string ProductType { get; set; }
        [DisplayName("Address")]
        public string CustomerAddress { get; set; }
        [DisplayName("Contact No")]
        public string CustomerContact { get; set; }
        [Required]
        [DisplayName("Delivery Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> DeliveryDate { get; set; }
        [Required(ErrorMessage = "Challen No is required !")]
        [DisplayName("Challan No")]
        public string ChallanNo { get; set; }
        [DisplayName("Bill No")]
        public string InvoiceNo { get; set; }
        [DisplayName("Discount Amount")]
        public decimal? DiscountAmount { get; set; }
        [DisplayName("Vehicle No")]
        [Required(ErrorMessage = "Vehicle Information is required !")]
        public string VehicleNo { get; set; }
        [DisplayName("Driver Name")]
        public string DriverName { get; set; }
        [DisplayName("Store")]
        [Required(ErrorMessage = "Please select a Store !")]
        public int StockInfoId { get; set; }

        public string StoreName { get; set; }

        public int CompanyId { get; set; }

        [DisplayName("Depot Invoice No")]
        public string DepoInvoiceNo { get; set; }
        public bool IsDepoOrder { get; set; }

        public long OrderDetailId { get; set; }
    }
}
