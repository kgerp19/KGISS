using KGERP.Data.CustomModel;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class CreditRecoverViewModel
    {
        public CreditRecoverModel CreditRecover { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> Customers { get; set; }
        public List<MonthlyTargetCM> MonthlyTargets { get; set; }
        public List<MonthlyTargetCM> MonthlyTargetDetails { get; set; }
    }
}