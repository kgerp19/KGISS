using KGERP.Data.CustomModel;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class LeaveApplicationViewModel
    {
        //  public List<LeaveApplication> LeaveApplications { get; set; }
        public LeaveApplicationModel LeaveApplication { get; set; }
        public List<SelectModel> LeaveCategories { get; set; }
        public List<LeaveBalanceCustomModel> LeaveBalance { get; set; }
        public string ErrorMessage { get; set; }
        public int isSandwichLeave { get; set; }
    }
}