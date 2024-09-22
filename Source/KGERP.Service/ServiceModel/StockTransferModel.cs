using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class StockTransferModel
    {
        public int StockTransferId { get; set; }

        [Required]
        [DisplayName("Transfer From")]
        public Nullable<int> StockIdFrom { get; set; }

        [Required]
        [DisplayName("Transfer To")]
        public Nullable<int> StockIdTo { get; set; }
        [DisplayName("Challan No")]
        public string ChallanNo { get; set; }

        [Required]
        [DisplayName("Vehicle No")]
        public string VehicleNo { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Required]
        [DisplayName("Transfer Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> TransferDate { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Truck Fare")]
        public Nullable<decimal> TruckFare { get; set; }
        [DisplayName("Labour Bill")]
        public Nullable<decimal> LabourBill { get; set; }
        [Required]
        [DisplayName("Reference No")]
        public string ReferenceNo { get; set; }
        public int IsReceived { get; set; }
        public long ReceivedBy { get; set; }
        [DisplayName("Phone")]
        public string ReceiverPhone { get; set; }
        [Required]
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public virtual StockInfoModel StockInfo { get; set; }
        public virtual StockInfoModel StockInfo1 { get; set; }

        public virtual List<StockTransferDetailModel> Items { get; set; } = new List<StockTransferDetailModel>();

        //-----------------Extended Properties------------------------------
        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;
        public string StockFrom { get; set; }
        public string StockTo { get; set; }
        [DisplayName("Receive Person")]
        public string ReceivePerson { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSubmitted { get; set; }
        public int Status { get; set; }
        public List<SelectModel> StockFromList { get; set; } = new List<SelectModel>();
        public List<SelectModel> StockToList { get; set; } = new List<SelectModel>();
        public IEnumerable<StockTransferModel> DataList { get; set; }
        public int StockTransferDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TransferQty { get; set; }
    }
    public class ProductCurrentStockModel
    {
      public string StockName { get; set; }
      public string ProductCategory { get; set; }
      public string ProductSubCategory { get; set; }
      public string Product { get; set; }
      public string UnitName { get; set;}
      public decimal OpeningRate { get; set; }
      public decimal OpeningQty { get; set; }
    public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
     public decimal OpeningValue { get; set; }
    }

    public class SeedProductCurrentStockModel
    {
        public int ProductCategoryId { get; set; }
        public int ProductSubCategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductSubCategoryName { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TPPrice { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal PurchaseReturnQty { get; set; }
        public decimal SalesQuantity { get; set; }
        public decimal SaleReturnQuantity { get; set; }
        public decimal StockAdjustExcessQty { get; set; }
        public decimal StockAdjustLessQty { get; set; }
        public decimal CurrentStock { get; set; }
        
    }
}
