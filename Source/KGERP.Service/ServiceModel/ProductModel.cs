using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class ProductModel
    {
        public string ButtonName
        {
            get
            {
                return ProductId > 0 ? "Update" : "Create";
            }
        }
        public string ProductType { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public int ProductId { get; set; }
        [DisplayName("Code")]
        public string ProductCode { get; set; }
        [DisplayName("Category")]

        public int ProductCategoryId { get; set; }
        [DisplayName("Sub Category")]
        public int ProductSubCategoryId { get; set; }
        [DisplayName("Product")]
        public string ProductName { get; set; }

        [DisplayName("Short Name")]
        public string ShortName { get; set; }
        [DisplayName("Unit")]
        public Nullable<int> UnitId { get; set; }
        public Nullable<double> Qty { get; set; }
        [DisplayName("Formula Qty")]
        public decimal? FormulaQty { get; set; }

        [DisplayName("Pack Size")]
        public Nullable<double> PackSize { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<decimal> UnitPrice { get; set; }
        [DisplayName("TP Price")]
        public Nullable<decimal> TPPrice { get; set; }
        public Nullable<decimal> CreditSalePrice { get; set; }
        public Nullable<decimal> PurchaseRate { get; set; }
        public Nullable<decimal> PurchaseCommissionRate { get; set; }
        public decimal SaleCommissionRate { get; set; }
        [DisplayName("Process Loss (%)")]
        public decimal ProcessLoss { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Order No")]
        public int OrderNo { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        [DisplayName("Pack Name")]
        public Nullable<int> PackId { get; set; }
        [DisplayName("Engine No")]
        public string EngineNo { get; set; }
        [DisplayName("Chassis No")]
        public string ChassisNo { get; set; }
        [DisplayName("Die size")]
        public Nullable<decimal> DieSize { get; set; }

        public virtual ProductModel Product2 { get; set; }
        public virtual ProductCategoryModel ProductCategory { get; set; }
        public virtual ProductSubCategoryModel ProductSubCategory { get; set; }
        public virtual UnitModel Unit { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string UnitName { get; set; }
        public string PackName { get; set; }

        public IEnumerable<ProductModel> DataList { get; set; }
    }
}


public class DepotCurrentStockModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int StockInfoId { get; set; }
    public string StockInfoName { get; set; } 
    public decimal CurrentStock { get; set; }
}
