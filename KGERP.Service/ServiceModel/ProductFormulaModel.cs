using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class ProductFormulaModel
    {
        public string ButtonName
        {
            get
            {
                return ProductFormulaId > 0 ? "Update" : "Save";
            }
        }
        public int ProductFormulaId { get; set; }
        [DisplayName("Product")]
        public Nullable<int> FProductId { get; set; }
        [DisplayName("Product Qty")]
        public Nullable<decimal> FQty { get; set; }
        [DisplayName("Formula Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> FormulaDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public Nullable<int> CompanyId { get; set; }

        public virtual ProductModel Product { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string ProductName { get; set; }
        public IEnumerable<ProductFormulaModel> DataList { get; set; }
    }
}
