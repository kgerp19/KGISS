using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class PFormulaDetailModel
    {
        public string ButtonName
        {
            get
            {
                return PFormulaDetailId > 0 ? "Update" : "Save";
            }
        }
        public int PFormulaDetailId { get; set; }
        public Nullable<int> ProductFormulaId { get; set; }
        [DisplayName("Raw Material")]
        public Nullable<int> RProductId { get; set; }
        [DisplayName("Quantity (Kg)")]
        public decimal RQty { get; set; }
        [DisplayName("Process Loss (%)")]
        public decimal RProcessLoss { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }

        public virtual ProductFormulaModel ProductFormula { get; set; }
        public virtual ProductModel Product { get; set; }
        //----------------------------Extended Property----------------
        public string ProductName { get; set; }
    }
}
