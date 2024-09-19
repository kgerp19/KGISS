using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class ProductPriceModel
    {
        public string ButtonName
        {
            get
            {
                return Id > 0 ? "Update" : "Save";
            }
        }
        public long Id { get; set; }
        public string PriceType { get; set; }
        [DisplayName("Product")]
        public Nullable<int> ProductId { get; set; }
        [DisplayName("Price Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> PriceUpdatedDate { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<decimal> UnitPrice { get; set; }
        [DisplayName("Sale Commission")]
        public Nullable<decimal> SaleCommissionRate { get; set; }

        [DisplayName("Purchase Price")]
        public Nullable<decimal> PurchaseRate { get; set; }
        [DisplayName("Purchase Commission")]
        public Nullable<decimal> PurchaseCommissionRate { get; set; }

        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        [DisplayName("Product")]
        public string ProductName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CompanyId { get; set; }

        [DisplayName("Credit Price")]
        public Nullable<decimal> CreditSellPrice { get; set; }

        //-------------Extended Property--------
        [DisplayName("Product Code")]
        public string ProductCode { get; set; }
        public decimal TPPrice { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<ProductPriceModel> DataList { get; set; }
    }

}
