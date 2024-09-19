using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace KGERP.Service.ServiceModel
{
    public class ConvertedProductModel
    {
        public int ConvertedProductId { get; set; }
        public int CompanyId { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }
        [DisplayName("Convert From")]
        public int ConvertFrom { get; set; }
        [DisplayName("Convert To")]
        public int ConvertTo { get; set; }
        [DisplayName("Qty")]
        [Range(10.0, double.MaxValue, ErrorMessage = "Minimum Qty 10 KG")]
        public decimal ConvertedQty { get; set; }
        [DisplayName("Conversion Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ConvertedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public decimal ConvertedUnitPrice { get; set; }
        [DisplayName("Status")]
        public string ConvertStatus { get; set; }
        public virtual ProductModel Product { get; set; }
        public virtual ProductModel Product1 { get; set; }

        //------------------Extended Properties---------------
        public string FromItem { get; set; }
        public string ToItem { get; set; }
        [DisplayName("Stock Available Qty")]
        public decimal StockAvailableQty { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<ConvertedProductModel> DataList { get; set; }
        public string ProductFromName { get; set; }
        public int? ConvertToAccountHeadId { get; set; }
        public decimal ConvertFromUnitPrice { get; set; }
        public string ProductToName { get; set; }
        public int? ConvertFromAccountHeadId { get; set; }
        public string IntegratedFrom { get;  set; }
    }
}
