using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class OfficerAssignViewModel
    {
        public OfficerAssignModel OfficerAssign { get; set; }
        public List<SelectModel> Zones { get; set; }
    }
}