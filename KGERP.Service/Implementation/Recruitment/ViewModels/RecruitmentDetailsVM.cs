using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Recruitment.ViewModels
{
    public class RecruitmentDetailsVM
    {

        public long RequisitionDetailId { get; set; }
        public long RecruitmentId_Fk { get; set; }

        public BusinessTypeEnum BusinessType { get; set; }
        public int BusineesId_Fk { get; set; }


        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public string JobTitle { get; set; }
        public JobNatureEnum JobNature { get; set; }
        public JobTypeEnum JobType { get; set; }
        public VacancyTypeEnum VacancyType { get; set; }
        public int NumberOfRecruitment { get; set; }

        public decimal TargetSalary { get; set; }

        public DateTime? DateOfVacancy { get; set; }
        public DateTime? ExpectedDateOfJoining { get; set; }
        public long ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string JobLocation { get; set; }
        public string JobDetails { get; set; }
        public EducationLevelEnum EducationalQualification { get; set; }

        public string Advertisement { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public StatusEnum Status { get; set; }
    }
}
