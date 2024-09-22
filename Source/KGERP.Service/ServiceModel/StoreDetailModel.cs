using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class StoreDetailModel
    {
        public string ButtonName
        {
            get
            {
                return StoreDetailId > 0 ? "Update" : "Create";
            }
        }
        public long StoreDetailId { get; set; }
        public Nullable<long> StoreId { get; set; }
        [DisplayName("Product")]
        public Nullable<int> ProductId { get; set; }
        public double Qty { get; set; }

        [DisplayName("Batch No")]
        public string BatchId { get; set; }

        public double RemainingQty { get; set; }

        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual ProductCategoryModel ProductCategory { get; set; }
        public virtual ProductSubCategoryModel ProductSubCategory { get; set; }
        public virtual ProductModel Product { get; set; }
        public virtual StoreModel Store { get; set; }
        public Nullable<decimal> ProductionRate { get; set; }
        [DisplayName("Bag Qty")]
        public int BagQty { get; set; }

        public decimal InvoiceValue { get; set; }
        public decimal LandedCost { get; set; }
        public decimal TotalCOGS { get; set; }
        public decimal CogsPerUnit { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
   
        public Nullable<decimal> USDPrice { get; set; }
        public Nullable<decimal> BDTPrice { get; set; }
        //--------------Extended Property----------------------------------------
        public long PurchaseQty { get; set; }
        public decimal PurchaseRate { get; set; }

        public int CompanyId { get; set; }
        public int OrderQty { get; set; }
        public decimal? OrderRate { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public string ProductCode { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
