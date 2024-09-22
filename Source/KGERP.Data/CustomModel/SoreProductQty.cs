using System.Collections.Generic;

namespace KGERP.Data.CustomModel
{
    public class SoreProductQty
    {
        public int CompanyId { get; set; }
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public IEnumerable<SoreProductQty> DataList { get; set; }
    }


    public class FeedFinishedProductStock
    {
        public int CompanyId { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductSubCategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSubCategory { get; set; }
        public string Product { get; set; }
        public decimal ClosingQty { get; set; }
        public decimal StockAmount { get; set; }
        public decimal ClosingRate { get; set; }
      
    }
}
