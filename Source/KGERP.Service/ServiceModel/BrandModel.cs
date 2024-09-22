using System.Collections.Generic;

namespace KGERP.Service.ServiceModel
{
    public class BrandModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public virtual ICollection<AssetTypeModel> AssetTypes { get; set; }
    }
}
