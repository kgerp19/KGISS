using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class EmiViewModel
    {
        public EMIModel EMI { get; set; }
        public List<EmiDetailModel> EmiDetail { get; set; }
        public List<SelectModel> Vendors { get; set; }
        public List<SelectModel> OrderInvoice { get; set; }

    }
}