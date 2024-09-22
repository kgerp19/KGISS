using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class RequisitionItemModel
    {
        public RequisitionItemModel()
        {

        }
        public int RequisitionItemId { get; set; }
        public Nullable<int> RequisitionId { get; set; }
        public Nullable<int> ProductFormulaId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public string RequisitionItemStatus { get; set; }
        public string IssueBy { get; set; }

        public Nullable<decimal> InputQty { get; set; }
        [Required]
        [DisplayName("Output Qty")]
        public decimal OutputQty { get; set; }
        [DisplayName("OverHead")]
        public decimal OverHead { get; set; }
        [DisplayName("Process Loss")]
        public decimal ProcessLoss { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public virtual RequisitionModel Requisition { get; set; }
        public virtual ProductModel Product { get; set; }
        public decimal ActualProcessLoss { get; set; }
        public decimal RMCost { get; set; }

        public int BagQty { get; set; }
        public Nullable<int> BagId { get; set; }
        [Required]
        [Range(5, Double.MaxValue, ErrorMessage = "Please enter valid Price")]
        public decimal BagUnitPrice { get; set; }
        public decimal RMCostRate { get; set; }
        public decimal BagRate { get; set; }
        public decimal ProductionRate { get; set; }
        // -------------Extended Properties--------------------
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [Required]
        public bool IsIssued { get; set; }
        public string BagName { get; set; }

        public int AvailableBagQty { get; set; }
        public int ConsumptionBagQty { get; set; }
        public int? AccountingHeadId { get; set; }

        public bool IsActive { get; set; }
        public decimal TPPrice { get; set; }
        public decimal CostingPrice { get;  set; }
        public DateTime? FormulaDate { get; set; }
    }
}
