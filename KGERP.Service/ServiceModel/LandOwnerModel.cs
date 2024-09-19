using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class LandOwnerModel
    {
        public int LandOwnerId { get; set; }

        [DisplayName("Land Owner Name")]
        public string LandOwnerName { get; set; }
        [DisplayName("Short Name")]
        public string ShortName { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
