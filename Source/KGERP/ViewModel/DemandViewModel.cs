using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class DemandViewModel
    {
        public DemandModel Demand { get; set; }
        public List<DemandItemModel> DemandItems { get; set; }
        public List<DemandItemDetailModel> DemandItemDetails { get; set; }
    }
}