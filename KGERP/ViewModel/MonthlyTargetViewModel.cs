using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class MonthlyTargetViewModel
    {
        public MonthlyTargetModel MonthlyTarget { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> Customers { get; set; }
        public List<SelectModel> Years { get; set; }
        public List<SelectModel> Months { get; set; }
    }
}