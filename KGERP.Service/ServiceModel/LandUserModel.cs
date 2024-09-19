using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class LandUserModel
    {
        public int LandUserId { get; set; }
        [DisplayName("Land User Name")]
        public string LandUserName { get; set; }
        [DisplayName("Short Name")]
        public string ShortName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
