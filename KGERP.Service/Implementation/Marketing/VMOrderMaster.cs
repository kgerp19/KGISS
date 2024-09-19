using KGERP.Service.Configuration;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.Marketing
{
    public partial class VMOrderMaster : BaseVM
    {


        public long OrderMasterId { get; set; }
        public int? CompanyId { get; set; }
        public string ProductType { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public DateTime? OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string OrderMonthYear { get; set; }
        public string OrderMasterNo { get; set; }
        public string ChallanNo { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? InvoiceDiscountAmount { get; set; }
        public decimal? InvoiceDiscountRate { get; set; }
        public long? SalePersonId { get; set; }
        public int? StockInfoId { get; set; }
        public SelectList ProductCategoryList { get; set; } = new SelectList(new List<object>());

        public IEnumerable<VMOrderMaster> DataList { get; set; }



        //public virtual Company Company { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<EMI> EMIs { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<OrderDeliver> OrderDelivers { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        //public virtual Vendor Vendor { get; set; }
    }

    public partial class VMOrderDetail : VMOrderMaster
    {
        public long OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string JobOrderNo { get; set; }
        public string Stucture { get; set; }
        public string ProductName { get; set; }
        public string OrderMasterNo { get; set; }
        public double Qty { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public IEnumerable<VMOrderDetail> DataListDetails { get; set; }

        //public decimal SpecialBaseCommission { get; set; }

        //public decimal AvgParchaseRate { get; set; }

    }

}
