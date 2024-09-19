namespace KGERP.Service.ServiceModel
{
    public class WorkMemberModel
    {
        public int WorkMemberId { get; set; }
        public long MemberId { get; set; }
        //-----------Extended Properties-----------
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string ImageUrl { get; set; }
    }
}
