using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class ComplainViewModel
    {
        public ComplainManagementModel Complain { get; set; }
        public List<SelectItemList> ComplainType { get; set; }
    }
}