using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class AssetViewModel
    {
        public AssetModel Asset { get; set; }
        public OfficeAssetModel _asset { get; set; }
        public List<SelectModel> Company { get; set; }
        public List<SelectModel> AssetLocation { get; set; }
        public List<SelectModel> AssetSubLocation { get; set; }
        public List<SelectModel> AssetCategory { get; set; }
        public List<SelectModel> AssetType { get; set; }
        public List<SelectModel> AssetStatus { get; set; }
        public List<SelectModel> Colour { get; set; }
        public List<SelectModel> Districts { get; set; }
        public List<SelectModel> Upazilas { get; set; }
        public List<SelectModel> Project { get; set; }
        public List<SelectModel> DisputedList { get; set; }
        public List<SelectModel> LandReceiver { get; set; }
        public List<SelectModel> LandUser { get; set; }
        public List<SelectModel> Departments { get; set; }
        public int CompanyId  { get; set; }
    }
}