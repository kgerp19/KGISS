using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
    public class TeamService : ITeamService
    {
        private readonly ERPEntities context = new ERPEntities();
        public async Task<int> AddNewTeam(TeamInfo leadInfo, List<TeamInfo> members = null)
        {
            leadInfo.Id = 0;
            leadInfo.IsActive = true;
            leadInfo.IsLeader = true;
            leadInfo.LeadId = null;
            try
            {
                context.TeamInfoes.Add(leadInfo);
                await context.SaveChangesAsync();
                if (members != null && members.Count > 0)
                {
                    foreach (var item in members)
                    {
                        item.LeadId = leadInfo.Id;
                        item.Id = 0;
                    }
                }
                context.TeamInfoes.AddRange(members);
                await context.SaveChangesAsync();
                return leadInfo.Id;
            }
            catch (Exception ex)
            {

               return 0;
            }
        }

        public async Task<bool> UpdateTeamLead(int TeamLeadId,long NewTeamLeadId,int CompanyId)
        {
            var Model = await context.TeamInfoes.Where(e => e.CompanyId == CompanyId && e.Id == TeamLeadId).SingleOrDefaultAsync();

            if (Model != null)
            {
                Model.EmployeeId = NewTeamLeadId;
                try
                {
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;

                }
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> RemoveMemberFromTeam(int TeamInfoId,int companyId)
        {
           var model=  await context.TeamInfoes.Where(e => e.Id == TeamInfoId && e.CompanyId == companyId).SingleOrDefaultAsync();
            if (model == null)
            {
                return false;
            }
            else
            {
                try
                {
                    model.IsActive = false;
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                    
                }
            }
        }
        public  async Task<bool> AddMembersToTeam( int TeamLeadId,int CompanyId, List<TeamInfo> members)
        {
            try
            {
               
                if (members != null && members.Count > 0)
                {
                    foreach (var item in members)
                    {
                        item.LeadId = TeamLeadId;
                        item.Id = 0;
                        item.CompanyId = CompanyId;
                    }
                }
                context.TeamInfoes.AddRange(members);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<object> GetTeamListByCompanyId(int companyId)
        {
            var List = new List<object>();
             var vwTeamInfoList =  context.vwTeamInfoLists.Where(e => e.CompanyId == companyId && e.IsLeader == false).ToList();

            foreach (var item in vwTeamInfoList)
            {
                List.Add(new { Text = item.EmployeeName + "[" + item.StrEmpId + "]", Value = item.EmployeeId  });
            }

            return List;

        }

        public async Task<List<vwTeamLeaderList>> GetTeamLeaderListByCompanyId (int CompanyId)
        {
            var list = await context.vwTeamLeaderLists.Where(e => e.CompanyId == CompanyId).ToListAsync();
            return list;
        }
        public async Task<List<vwTeamInfoList>> GetTeamInfoByLeaderId(int LeaderId)
        {
            var list = await context.vwTeamInfoLists.Where(e => e.Id== LeaderId || e.LeadId== LeaderId).ToListAsync();
            return list;
        }

        public async Task<Employee> GetLeaderInfoBystrEmpId(string strEmpId,int CompanyId)
        {
            Employee model = new Employee();
            var info = await context.vwTeamInfoLists.Where(e => e.CompanyId == CompanyId && e.StrEmpId == strEmpId).FirstOrDefaultAsync();
            if(info!=null && info.LeadId != 0)
            {
                var x = await context.TeamInfoes.Include(o => o.Employee).SingleOrDefaultAsync(e => e.Id == info.LeadId);
                model = x.Employee;
            }
            return model;
        }
        public async Task<Employee>GetLeaderInfoByEmployeeId (long employeeId, int CompanyId)
        {
            Employee model = new Employee();
            var info = await context.vwTeamInfoLists.Where(e => e.CompanyId == CompanyId && e.EmployeeId == employeeId && e.IsLeader == false).FirstOrDefaultAsync();
            if (info != null && info.LeadId != 0)
            {
                var x = await context.TeamInfoes.Include(o => o.Employee).SingleOrDefaultAsync(e => e.Id == info.LeadId);
                model = x.Employee;
            }
            return model;
        }

        public EmployeeModel GetEmployee(long id)
        {
            var res = context.Employees.FirstOrDefault(f => f.Id == id);
            EmployeeModel model=new EmployeeModel();
            model.Name = res.Name;
            model.Id = res.Id;
            model.MobileNo = res.MobileNo;
            return model;
        }

        public Task AddMembersToTeam(long leadId, int companyId, TeamInfo teamInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddSignalMembersToTeam(TeamInfo teamInfo)
        {
            {
                try
                {

                    if (teamInfo.LeadId > 0)
                    {

                        context.TeamInfoes.Add(teamInfo);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public List<object> GetTeamByCompanyId(int CompanyId)
        {
            var List = new List<object>();
            var vwTeamInfoList = context.vwTeamInfoLists.Where(e => e.CompanyId == CompanyId && e.IsLeader == true).ToList();

            foreach (var item in vwTeamInfoList)
            {
                List.Add(new { Text = item.EmployeeName + " - [" + item.TeamTitle + "] - " + "[" + item.StrEmpId + "]", Value = item.EmployeeId });
            }

            return List;
        }

        public List<object> LeaderInfoByEmployeeId(long employeeId)
        {
            var List = new List<object>();
            var res= context.vwTeamInfoLists.FirstOrDefault(e => e.IsLeader == true && e.EmployeeId == employeeId );
            var vwTeamInfoList = context.vwTeamInfoLists.Where(e =>e.IsLeader == false&&e.LeadId==res.Id).ToList();

            foreach (var item in vwTeamInfoList)
            {
                List.Add(new { Text = item.EmployeeName + " - [" + item.StrEmpId + "]", Value = item.EmployeeId });
            }

            return List;
        }
    }
}
