using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
    public interface ITeamService
    {
        Task<int> AddNewTeam(TeamInfo leadInfo, List<TeamInfo> members = null);
        Task<bool> UpdateTeamLead(int TeamLeadId, long NewTeamLeadId, int CompanyId);
        Task<bool> RemoveMemberFromTeam(int TeamInfoId, int companyId);
        Task<bool> AddMembersToTeam(int TeamLeadId, int CompanyId, List<TeamInfo> members);
        List<object> GetTeamListByCompanyId(int CompanyId);
        Task<List<vwTeamLeaderList>> GetTeamLeaderListByCompanyId(int CompanyId);
        List<object> GetTeamByCompanyId(int CompanyId);
        Task<List<vwTeamInfoList>> GetTeamInfoByLeaderId(int LeaderId);
        Task<Employee> GetLeaderInfoBystrEmpId(string strEmpId, int CompanyId);
        Task<Employee> GetLeaderInfoByEmployeeId(long employeeId, int CompanyId);
        List<object> LeaderInfoByEmployeeId(long employeeId);
        EmployeeModel GetEmployee(long id);
        Task AddMembersToTeam(long leadId, int companyId, TeamInfo teamInfo);
        Task<bool> AddSignalMembersToTeam(TeamInfo teamInfo);
    }
}