using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Data.CustomModel
{
    public class TeamLeaveBalanceCustomModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public List<SelectModel> SelectedYearList { get; set; }
        public int SelectedYear { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public int CLAvailed { get; set; }
        public int CLDue { get; set; }
        public int SLAvailed { get; set; }
        public int SLDue { get; set; }
        public int ELAvailed { get; set; }
        public int ELDue { get; set; }
        public int MrgAvailed { get; set; }
        public int MrgDue { get; set; }
        public int MtrAvailed { get; set; }
        public int MtrDue { get; set; }
        public int SelAvailed { get; set; }
        public int SelDue { get; set; }
        public int HjAvailed { get; set; }
        public int HjDue { get; set; }
        public int LwAvailed { get; set; }
        public int LwDue { get; set; }
        public int CLASDGAvailed { get; set; }
        public int CLASDGDue { get; set; }
        public IEnumerable<TeamLeaveBalanceCustomModel> DataList { get; set; }
    }
}
