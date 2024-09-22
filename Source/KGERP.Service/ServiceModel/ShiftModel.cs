namespace KGERP.Service.ServiceModel
{
    public class ShiftModel
    {
        public int ShiftId { get; set; }
        public string Name { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public int PostFlag { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
