using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class FarmerViewModel
    {
        public FarmerModel Farmer { get; set; }
        public List<SelectModel> Zones { get; set; }
        public List<SelectModel> Officers { get; set; }
    }
}