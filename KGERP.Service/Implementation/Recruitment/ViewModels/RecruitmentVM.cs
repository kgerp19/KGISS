using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KGERP.Service.Implementation.Recruitment.ViewModels
{
    public class RecruitmentVM
    {
        public int Id { get; set; }


        public BusinessTypeEnum BusinessType { get; set; }


        [DisplayName(displayName:"Recruitment Name")]
        public string RequisitionNumber { get; set; }

        [DisplayName("Business Name")]
        [Required(ErrorMessage = "This field is required")]
        public int BusineesId_Fk { get; set; }


        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        
        [DisplayName("Requisition Date")]
        public DateTime RequisitionDate { get; set; }
        public string Remarks { get; set; }
        public StatusEnum Status { get; set; }


        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long? ProjectId { get; set; }
        //public BusinessTypeEnum BusinessType { get; set; }
        //public long BusinessId_Fk { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public RecruitmentStatusEnum RecruitmentStatus { get; set; }
        public List<RecruitmentDetailsVM> RecruitmentDetailsList { get; set; }
        public List<RecruitmentVM> DataList { get; set; }

        public List<RequisitionApprovalVM> RequisitionApprovalLists { get; set; }
        public ActionEnum ActionEnum { get; set; }





        public int RequisitionDetailId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public string JobTitle { get; set; }
        public JobNatureEnum JobNature { get; set; }
        public JobTypeEnum JobType { get; set; }
        public VacancyTypeEnum VacancyType { get; set; }
        public int NumberOfRecruitment { get; set; }

        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal TargetSalary { get; set; }

        public DateTime? DateOfVacancy { get; set; }
       // [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Expected Joining Date")]
        [Required]
              
        public DateTime? ExpectedDateOfJoining { get; set; }
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string JobLocation { get; set; }
        public string JobDetails { get; set; }
        public EducationLevelEnum EducationalQualification { get; set; }
        public string Advertisement { get; set; }

        public string FeedbackMessage { get; set; }

        public int UserCompanyId { get; set; }
    }
}
