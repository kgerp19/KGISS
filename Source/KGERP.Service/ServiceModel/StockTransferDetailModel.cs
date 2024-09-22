using System;

namespace KGERP.Service.ServiceModel
{
    public class StockTransferDetailModel
    {
        public int CompanyId { get; set; }
        public int StockTransferDetailId { get; set; }
        public Nullable<int> StockTransferId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<decimal> TransferQty { get; set; }
        public Nullable<decimal> ReceivedQty { get; set; }
        public Nullable<int> IsRecieved { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<int> BagQty { get; set; }

        public virtual ProductModel Product { get; set; }
        public virtual StockTransferModel StockTransfer { get; set; }

        //Extended Properties
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> ReadyToReceiveQty { get; set; }
        public Nullable<decimal> RemainQty { get; set; }
        public bool IsItemChecked { get; set; }
        
    }
}
