using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class OrderMasterViewModel
    {
        public OrderMasterModel OrderMaster { get; set; }
        public List<SelectModel> Customers { get; set; }
        public List<SelectModel> Vendors { get; set; }
    }
}