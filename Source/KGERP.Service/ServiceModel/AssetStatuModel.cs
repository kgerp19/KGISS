using System.Collections.Generic;

namespace KGERP.Service.ServiceModel
{
    public class AssetStatuModel
    {
        public int StatusId { get; set; }
        public string Status { get; set; }
        public virtual ICollection<AssetModel> Assets { get; set; }
    }
}
