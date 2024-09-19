using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KGERP.ViewModel
{
    public class PurchaseReturnViewModel
    {
        public PurchaseReturnModel PurchaseReturn { get; set; }
        public List<SelectModel> Stocks { get; set; }
        public List<SelectModel> ProductTypes { get; set; }

    }

}