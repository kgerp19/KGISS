using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class VendorOfferModel
    {
        public string ButtonName
        {
            get
            {
                return VendorOfferId > 0 ? "Update" : "Save";
            }
        }
        public long VendorOfferId { get; set; }
        public int ProductCategoryId { get; set; }
        public int VendorId { get; set; }
        [DisplayName("Product")]
        public int ProductId { get; set; }
        public string ProductType { get; set; }
        [DisplayName("Base")]
        public decimal? BaseCommission { get; set; } = 0;
        [DisplayName("Unit Price")]

        public decimal? UnitPrice { get; set; }
        [DisplayName("Cash")]
        public decimal? CashCommission { get; set; } = 0;
        [DisplayName("Carrying")]
        public decimal? CarryingCommission { get; set; } = 0;
        [DisplayName("Factory Carrying")]
        public decimal? FactoryCarryingCommission { get; set; }
        [DisplayName("Special")]
        public decimal? SpecialCommission { get; set; } = 0;
        [DisplayName("Addition Price")]
        public decimal? AdditionPrice { get; set; } = 0;
        public decimal? MonthlyIncentive { get; set; } = 0;
        public decimal? YearlyIncentive { get; set; } = 0;
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual ProductModel Product { get; set; }
        public virtual VendorModel Vendor { get; set; }

        //-----------Extended Properties---------------

        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string CustomerName { get; set; }
        public bool IsAllBase { get; set; }
        public bool IsAllCash { get; set; }
        public bool IsAllCarrying { get; set; }
        public bool IsAllFactoryCarrying { get; set; }
        public bool IsAllSpecial { get; set; }
        public bool IsAllAdditionPrice { get; set; }
        public bool IsAllMonthlyIncentive { get; set; }
        public bool IsAllYearlyIncentive { get; set; }
    }
}
