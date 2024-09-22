using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class EmployeeClearanceVM
    {
        public long Id { get; set; }

        public int ExitInterviewId { get; set; }
        public long ClearanceSignatoryId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public DateTime JoiningDate { get; set; }
        public DateTime? ResignDate { get; set; }

        public string Comment { get; set; } = string.Empty;
        public SignatoryStatusEnum  SignatoryStatus { get; set; }

        public int UserCompanyId { get; set; }
    }
}
