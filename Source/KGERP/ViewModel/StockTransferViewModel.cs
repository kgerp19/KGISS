using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class StockTransferViewModel
    {
        public StockTransferModel StockTransfer { get; set; }
        public List<StockTransferDetailModel> StockTransferDetail { get; set; }

        public List<SelectModel> StockFrom { get; set; }
        public List<SelectModel> StockTo { get; set; }
    }
}