using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class AssetSubLocationViewModel
    {
        public AssetSubLocationModel SubLocation { get; set; }
        public List<SelectModel> Location { get; set; }
    }
}