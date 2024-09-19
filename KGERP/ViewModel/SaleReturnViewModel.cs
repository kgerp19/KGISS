using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class SaleReturnViewModel
    {
        public SaleReturnModel SaleReturn { get; set; }
        public List<SelectModel> StockInfos { get; set; }
        public List<SelectModel> Invoices { get; set; }
        public List<SaleReturnDetailModel> SaleReturnDetails { get; set; }

    }
}