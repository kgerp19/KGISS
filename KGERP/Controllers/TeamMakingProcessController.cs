using KGERP.Data.Models;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class TeamMakingProcessController : BaseController
    {
        // GET: TeamMakingProcess
        private readonly IGLDLCustomerService gLDLCustomerService;
        private readonly ITeamService teamService;
        private readonly ICompanyService companyService;
        private readonly IEmployeeService employeeService;
        public TeamMakingProcessController(
            IEmployeeService employeeService,
            ICompanyService companyService,
            IGLDLCustomerService gLDLCustomerService,
            ITeamService teamService)
        {
            this.gLDLCustomerService = gLDLCustomerService;
            this.teamService = teamService;
            this.companyService = companyService;
            this.employeeService = employeeService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> CreateTeam(int companyId)
        {
            TeamMakingProcessViewModel model = new TeamMakingProcessViewModel();
            model.CompanyId = companyId;
            model.Employee = await gLDLCustomerService.GetbyEmployee(companyId);
            model.Members = await gLDLCustomerService.GetbyMember(companyId);
            model.CompanyName = companyService.GetCompany(companyId).Name;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeam(TeamMakingProcessViewModel model)
        {
            TeamInfo teamInfo = new TeamInfo();
            teamInfo.EmployeeId = model.EmployeeId;
            teamInfo.CreatedDate = DateTime.Now;
            teamInfo.CreatedBy = User.Identity.Name;
            teamInfo.IsActive = true;
            teamInfo.CompanyId = model.CompanyId;
            teamInfo.Name = model.Name;

            List<TeamInfo> listInfo = new List<TeamInfo>();
            if (model.TeamMember != null)
            {
                foreach (var item in model.TeamMember)
                {
                    TeamInfo team = new TeamInfo();
                    team.EmployeeId = item.EmployeeId;
                    team.CreatedDate = DateTime.Now;
                    team.IsActive = true;
                    team.CreatedBy = User.Identity.Name;
                    team.CompanyId = model.CompanyId;
                    team.Name = model.Name;
                    listInfo.Add(team);
                }
            }

            var res = await teamService.AddNewTeam(teamInfo, listInfo);
            return RedirectToAction("TeamDetails", new { companyId = model.CompanyId, LeadId = res });
        }

        [HttpPost]
        public async Task<ActionResult> GetByMebmer(long id)
        {
            var Employee = teamService.GetEmployee(id);
            return Json(Employee);
        }
        [HttpGet]
        public async Task<ActionResult> TeamDetails(int companyId, int LeadId)
        {
            TeamMakingProcessViewModel model = new TeamMakingProcessViewModel();
            model.CompanyId = companyId;
            model.CompanyName = companyService.GetCompany(companyId).Name;
            model.Members = await gLDLCustomerService.GetbyMember(companyId);
            model.vwTeams = await teamService.GetTeamInfoByLeaderId(LeadId);
            var linfo = model.vwTeams.FirstOrDefault(f => f.Id == LeadId);
            TeamMemberInformationViewModel viewModel = new TeamMemberInformationViewModel();
            viewModel.LeaderName = linfo.EmployeeName;
            viewModel.LeaderDesignation = linfo.Designation;
            viewModel.StrLeaderEmpId = linfo.StrEmpId;
            viewModel.LeadId = linfo.Id;
            viewModel.CompanyId = linfo.CompanyId;
            viewModel.EmployeeId = linfo.EmployeeId;
            viewModel.CompanyName = linfo.CompanyName;
            model.TeamMemberInformationView = viewModel;
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> TeamList(int companyId)
        {
            TeamMakingProcessViewModel model = new TeamMakingProcessViewModel();
            model.CompanyId = companyId;
            model.CompanyName = companyService.GetCompany(companyId).Name;
            model.vwTeamLeaders = await teamService.GetTeamLeaderListByCompanyId(companyId);
            model.Employee = await gLDLCustomerService.GetbyEmployee(companyId);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddNewMember(TeamMakingProcessViewModel model)
        {
            TeamInfo teamInfo = new TeamInfo();
            teamInfo.EmployeeId = (long)model.EmployeeId;
            teamInfo.CreatedDate = DateTime.Now;
            teamInfo.CreatedBy = User.Identity.Name;
            teamInfo.IsActive = true;
            teamInfo.CompanyId = model.CompanyId;
            teamInfo.LeadId = (int)model.LeadId;
            var res = await teamService.AddSignalMembersToTeam(teamInfo);
            return RedirectToAction("TeamDetails", new { companyId = model.CompanyId, LeadId = model.LeadId });
        }
        [HttpPost]
        public async Task<ActionResult> DeleteMember(TeamMakingProcessViewModel model)
        {
            var res = await teamService.RemoveMemberFromTeam((int)model.MemberId, model.CompanyId);
            return RedirectToAction("TeamDetails", new { companyId = model.CompanyId, LeadId = model.LeadId });
        }

        [HttpPost]
        public async Task<ActionResult> ReplaceLeader(TeamMakingProcessViewModel model)
        {
            var res = await teamService.UpdateTeamLead(model.Id, model.EmployeeId, model.CompanyId);
            return RedirectToAction("TeamList", new { companyId = model.CompanyId });
        }

        [HttpPost]
        public async Task<ActionResult> GetLeaderInfo(long employeeId, int companyId)
        {
            var res = await teamService.GetLeaderInfoByEmployeeId(employeeId, companyId);
            TeamMemberInformationViewModel model = new TeamMemberInformationViewModel();

            model.EmployeeName = res.Name;
            model.EmployeeId = res.Id;
            model.StrEmpId = res.EmployeeId;
            model.Designation = res.Designation.Name;
            return Json(model);
        }
        [HttpGet]
        public async Task<ActionResult> GetLeaderInfolist(long employeeId)
        {
            var res = teamService.LeaderInfoByEmployeeId(employeeId);
            return Json(res,JsonRequestBehavior.AllowGet);
        }



    }
}