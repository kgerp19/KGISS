using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class PaymentViewModel
    {
        public PaymentModel Payment { get; set; }
        public List<SelectModel> Customers { get; set; }
        public List<SelectModel> PaymentModes { get; set; }
        public List<SelectModel> Banks { get; set; }
    }
}