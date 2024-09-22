using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class SubZoneViewModel
    {
        public SubZoneModel SubZone { get; set; }
        public List<SelectModel> Zone { get; set; }
    }
}