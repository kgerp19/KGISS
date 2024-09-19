using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class StoreViewModel
    {
        public StoreModel Store { get; set; }
        public List<StoreDetailModel> StoreDetails { get; set; }
        public RequisitionModel Requisition { get; set; }
        public List<SelectModel> Vendors { get; set; }
        public List<SelectModel> PurchaseOrders { get; set; }
        public List<SelectModel> StockInfos { get; set; }
        public List<RequisitionItemModel> RequistionItems { get; set; }

        public string SupplierName { get; set; }
        public string ReceivedByName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string StoreName { get; set; }
    }
}