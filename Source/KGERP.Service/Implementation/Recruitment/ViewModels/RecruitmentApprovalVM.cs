using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Recruitment.ViewModels
{
    public class RecruitmentApprovalVM
    {
        public long RecruitmentId { get; set; }
        public string RecruitmentNumber { get; set; }

        public DateTime RecruitmentDate { get; set; }
        public string Remarks { get; set; }

        public int RecruitmentApprovalId { get; set; }

        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public int TotalNumberOfRecruitment { get; set; }
        public decimal TotalSalary { get; set; }

        public DateTime? ExpectedJoiningDate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public SignatoryStatusEnum SignatoryStatus { get; set; }

        public int UserCompanyId { get; set; }
        public List<RecruitmentRequisitionSummaryVM> RecruitmentRequisitionSummaryList { get; set; }
    }


    public class RecruitmentRequisitionSummaryVM
    {
        public int NumberOfRecruitment { get; set; }
        public decimal Salary { get; set; }
    }
}
