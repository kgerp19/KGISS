using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class AssetTypeViewModel
    {
        public AssetTypeModel AssetType { get; set; }
        public List<SelectModel> Category { get; set; }
    }
}