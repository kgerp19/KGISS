using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class LandReceiverModel
    {

        public int LandReceiverId { get; set; }
        [DisplayName("Land Receiver")]
        public string LandReceiverName { get; set; }
        [DisplayName("Short Name")]
        public string ShortName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
