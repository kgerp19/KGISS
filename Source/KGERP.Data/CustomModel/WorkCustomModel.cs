using System;

namespace KGERP.Data.CustomModel
{
    public class WorkCustomModel
    {
        public int OrderNo { get; set; }
        public long ManagerId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string WorkNo { get; set; }
        public string WorkTopic { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public string WorkState { get; set; }
        public string Remarks { get; set; }
    }
}
