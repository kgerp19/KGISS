namespace KGERP.Data.CustomModel
{
    public class LeaveBalanceCustomModel
    {
        public int Id { get; set; }
        public long EmployeeId { get; set; }
        public long Employee { get; set; }
        public int LeaveCategoryId { get; set; }
        public string LeaveCategory { get; set; }
        public int MaxDays { get; set; }
        public int TotalLeave { get; set; }
        public int LeaveBalance { get; set; }
        public int LeaveAvailed { get; set; }
        public string LeaveYear { get; set; }
        public int Year { get; set; }
    }
}
