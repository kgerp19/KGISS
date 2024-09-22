using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class EmployeeSearchViewModel
    {
        public EmployeeModel Employee { get; set; }
        public List<EmployeeModel> Employees { get; set; }

        public List<SelectModel> Departments { get; set; }
        public List<SelectModel> Designations { get; set; }
    }
}