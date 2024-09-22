using System;

namespace KGERP.Service.ServiceModel
{
    public class DemandItemModel
    {
        public long DemandItemId { get; set; }
        public Nullable<long> DemandId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public long Qty { get; set; }

        public virtual DemandModel Demand { get; set; }

        //--------------Extended Propertes-----------
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public string CreatedBy { get; set; } =  System.Web.HttpContext.Current.User.Identity.Name;
        public System.DateTime CreatedDate { get; set; } = DateTime.Now;
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsInvoiceCreated { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> StockInfoId { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
        public decimal Discount { get; set; }
        public decimal DicountAmount { get; set; }
        public bool IsSubmitted { get; set; }
    }
}
