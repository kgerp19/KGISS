namespace KGERP.Service.ServiceModel
{
    public class MessageModel
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }
        public System.DateTime SendDate { get; set; }
        public string request_type { get; set; }
        public string campaign_uid { get; set; }
        public string sms_uid { get; set; }
        public string api_response_code { get; set; }
        public string api_response_message { get; set; }
        public string MessageBody { get; set; }
    }
}
