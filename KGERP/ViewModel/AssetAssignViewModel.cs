using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class AssetAssignViewModel
    {
        public AssetAssignModel AssetAssign { get; set; }
        public List<SelectModel> Asset { get; set; }
        public List<SelectModel> Company { get; set; }
        public List<SelectModel> AssetLocation { get; set; }
        public List<SelectModel> AssetSubLocation { get; set; }
        public List<SelectModel> AssetCategory { get; set; }
        public List<SelectModel> AssetType { get; set; }

    }
}