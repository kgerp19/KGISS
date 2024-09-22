using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class StoreDetailViewModel
    {
        public StoreDetailModel StoreDetail { get; set; }
        public IEnumerable<StoreDetailModel> StoreDetails { get; set; }
        public StoreModel Store { get; set; }
        public List<SelectModel> ProductCategories { get; set; }
        public List<SelectModel> ProductSubCategories { get; set; }
        public List<SelectModel> Products { get; set; }
        public List<SelectModel> Units { get; set; }
        public List<SelectModel> Vendors { get; set; }
        public List<SelectModel> StockInfos { get; set; }
    }
}