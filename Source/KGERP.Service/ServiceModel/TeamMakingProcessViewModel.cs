using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
   public class TeamMakingProcessViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Employee")]
        public long EmployeeId { get; set; } 
        public long? MemberId { get; set; }
        public long LeadId { get; set; }
        public bool IsLeader { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Required]
        [DisplayName("Member Name")]
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<SelectModelType> Employee { get; set; }
        public List<SelectModelType> Members { get; set; }
        public List<TeamMemberViewModel> TeamMember { get; set; }
        public List<vwTeamLeaderList> vwTeamLeaders { get; set; }
        public List<vwTeamInfoList> vwTeams { get; set; }
        public TeamMemberInformationViewModel TeamMemberInformationView { get; set; }

}

    public class TeamMemberViewModel
    {
        public long EmployeeId { get; set; }
        public string MobileNo { get; set; }
    }

    public class TeamMemberInformationViewModel
    {
        public long EmployeeId { get; set; }
        public string MobileNo { get; set; }
        public int Id { get; set; }
        public bool IsLeader { get; set; }
        public int LeadId { get; set; }
        public string LeaderName { get; set; }
        public string StrLeaderEmpId { get; set; }
        public string LeaderDesignation { get; set; }
        public int CompanyId { get; set; }
        public string EmployeeName { get; set; }
        public string StrEmpId { get; set; }
        public string Designation { get; set; }
        public string CompanyName { get; set; }
        
    }


}
