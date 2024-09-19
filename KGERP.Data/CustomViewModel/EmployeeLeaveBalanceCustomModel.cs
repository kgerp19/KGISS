using KGERP.Data.CustomModel;
using System.Collections.Generic;

namespace KGERP.Data.CustomViewModel
{
    public class EmployeeLeaveBalanceCustomModel
    {
        public IEnumerable<LeaveBalanceCustomModel> LeaveBalanceCustomModels { get; set; }
        public EmployeeCustomModel EmployeeCustomModel { get; set; }
    }
}
