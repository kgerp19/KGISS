using System.Collections.Generic;

namespace KGERP.Service.ServiceModel
{
    public class ColourModel
    {
        public int ColourId { get; set; }
        public string ColourName { get; set; }


        public virtual ICollection<AssetModel> Assets { get; set; }
    }
}
