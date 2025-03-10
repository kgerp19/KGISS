//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KGERP.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductSubCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductSubCategory()
        {
            this.IngredientStandards = new HashSet<IngredientStandard>();
            this.Products = new HashSet<Product>();
        }
    
        public int ProductSubCategoryId { get; set; }
        public string ProductType { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> ProductCategoryId { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> BaseCommissionRate { get; set; }
        public string Code { get; set; }
        public string Remarks { get; set; }
        public int OrderNo { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> AccountingHeadId { get; set; }
        public Nullable<int> AccountingIncomeHeadId { get; set; }
        public Nullable<int> AccountingExpenseHeadId { get; set; }
        public Nullable<int> BrandId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IngredientStandard> IngredientStandards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}
