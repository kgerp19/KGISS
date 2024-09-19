using KGERP.Data.CustomModel;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class OrderDeliverViewModel
    {
        public OrderDeliverModel OrderDeliver { get; set; }
        public OrderMasterModel OrderMaster { get; set; }
        public OrderDeliverCustomModel OrderDeliverCustomModel { get; set; }

        public List<SelectModel> OrderMasters { get; set; }
        public List<SelectModel> StockInfos { get; set; }

        public List<DeliverItemCustomModel> DeliverItems { get; set; }

        public VMOrderDeliverDetail VMOrderDeliverDetail { get; set; }
        public long OrderDeliverId { get; set; }

    }
}