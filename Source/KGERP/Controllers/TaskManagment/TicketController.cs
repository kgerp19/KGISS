using KGERP.Service.Implementation.TaskManagment;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.TaskManagment
{
    [CheckSession]
    public class TicketController : BaseController
    {
        // GET: Ticket
        private readonly TaskManagmentservice _taskManagmentservice;
        private readonly ICompanyService companyService;
        public TicketController(TaskManagmentservice taskManagmentservice, ICompanyService companyService)
        {
            _taskManagmentservice = taskManagmentservice;
            this.companyService = companyService;
        }
 

        
        [HttpGet]
        public ActionResult Request_Ticket(int companyId)
        {
            TicketingViewModel model = new TicketingViewModel();
            ModelState.Clear();
            model.Companies = companyService.GetAllCompanySelectModels();
            model.CompanyId= companyId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Request_Ticket(TicketingViewModel model)
        {
            if (model.TaskType==0)
            {
                ModelState.Clear();
                ModelState.AddModelError("TaskType", "Requred");
                return RedirectToAction(nameof(Request_Ticket), new { companyId = model.CompanyId });
            }
            ModelState.Clear();
            model.EmployeeId = Convert.ToInt32(Session["Id"]);
            var res =  _taskManagmentservice.RequestTicket(model);
            return RedirectToAction(nameof(RequestTicke_List), new { companyId = model.CompanyId});
        }
        
        [HttpPost]
        public ActionResult EditRequst(TicketingViewModel model)
        {

            ModelState.Clear();
            model.EmployeeId = Convert.ToInt32(Session["Id"]);
            model.CompanyIdFK = Convert.ToInt32(model.CompanyName);
            var res =  _taskManagmentservice.update(model);
            return RedirectToAction(nameof(RequestTicke_List), new { companyId = model.CompanyId});
        }


        
        [HttpGet]
        public ActionResult RequestTicke_List(int companyId)
        {
            TicketingViewModel model = new TicketingViewModel();
            int EmployeeId = Convert.ToInt32(Session["Id"]);
            model = _taskManagmentservice.RequestTicketList(companyId,EmployeeId);
            model.Companies = companyService.GetAllCompanySelectModels();
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteRequest(TicketingViewModel model)
        {
            var res = _taskManagmentservice.RequestDelete(model);
            if (res)
            {
                return RedirectToAction(nameof(RequestTicke_List), new { companyId = model.CompanyId });
            }
            return View(model);
        }
    


        [HttpGet]
        
        public async Task<ActionResult> Index(int companyId, int CompanyIdFK=0,int TaskType = 0, DateTime? fromDate=null, DateTime? toDate=null, int status=1)
        {
           
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now.AddDays(+1);
            }
            if (status == 0)
            {
                status = 1;
            }

            TicketingViewModel viewModel = new TicketingViewModel();
            viewModel = await _taskManagmentservice.GetAllList(companyId, CompanyIdFK, TaskType, fromDate, toDate,status);
            viewModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            viewModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            viewModel.Status = 1;
            
            viewModel.Companies = companyService.GetAllCompanySelectModels();
            viewModel.CompanyId = companyId;
            if (viewModel.TaskType != 0)
            {
                viewModel.TaskType = TaskType;
            }

            if (viewModel.CompanyIdFK != 0)
            {
                viewModel.CompanyIdFK = CompanyIdFK;
            }
            return View(viewModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> Index(TicketingViewModel vm)
        {
            Session["CompanyId"] = vm.CompanyId;
            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);


            return RedirectToAction(nameof(Index), new { companyId = vm.CompanyId, CompanyIdFK=vm.CompanyIdFK, TaskType = vm.TaskType, fromDate = vm.FromDate, toDate = vm.ToDate, status = vm.Status });
        }

        [HttpPost]
        public ActionResult ChangeStatus(TicketingViewModel vm)
        {
            var res = _taskManagmentservice.ChangeStatus(vm);
            Session["CompanyId"] = vm.CompanyId;
            if (res)
            {
                return RedirectToAction(nameof(Index), new { companyId = vm.CompanyId});
            }
            return RedirectToAction(nameof(Index), new { companyId = vm.CompanyId});
        }

        [HttpPost]
        public ActionResult ChangeStatusErp(TicketingViewModel vm)
        {
            var res = _taskManagmentservice.ChangeStatus(vm);
            Session["CompanyId"] = vm.CompanyId;
            if (res)
            {
                return RedirectToAction(nameof(ERP_List), new { companyId = vm.CompanyId });
            }
            return RedirectToAction(nameof(ERP_List), new { companyId = vm.CompanyId });
        }

        [HttpPost]
        public ActionResult ChangeStatusNetwork(TicketingViewModel vm)
        {
            var res = _taskManagmentservice.ChangeStatus(vm);
            Session["CompanyId"] = vm.CompanyId;
            if (res)
            {
                return RedirectToAction(nameof(Network_List), new { companyId = vm.CompanyId });
            }
            return RedirectToAction(nameof(Network_List), new { companyId = vm.CompanyId });
        }


        [HttpPost]
        public ActionResult ChangeStatusAC(TicketingViewModel vm)
        {
            var res = _taskManagmentservice.ChangeStatus(vm);
            Session["CompanyId"] = vm.CompanyId;
            if (res)
            {
                return RedirectToAction(nameof(Accounts_List), new { companyId = vm.CompanyId });
            }
            return RedirectToAction(nameof(Accounts_List), new { companyId = vm.CompanyId });
        }

        [HttpPost]
        public ActionResult ChangeStatusEng(TicketingViewModel vm)
        {
            var res = _taskManagmentservice.ChangeStatus(vm);
            Session["CompanyId"] = vm.CompanyId;
            if (res)
            {
                return RedirectToAction(nameof(Accounts_List), new { companyId = vm.CompanyId });
            }
            return RedirectToAction(nameof(Accounts_List), new { companyId = vm.CompanyId });
        }


        [HttpPost]
        public ActionResult ChangeStatusAdmin(TicketingViewModel vm)
        {
            var res = _taskManagmentservice.ChangeStatus(vm);
            Session["CompanyId"] = vm.CompanyId;
            if (res)
            {
                return RedirectToAction(nameof(Admin_List), new { companyId = vm.CompanyId });
            }
            return RedirectToAction(nameof(Admin_List), new { companyId = vm.CompanyId });
        }


        [HttpGet]
        
        public async Task<ActionResult> ERP_List(int companyId, int CompanyIdFK = 0,  DateTime? fromDate = null, DateTime? toDate = null, int status = 1)
        {

            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now.AddDays(+1);
            }
            if (status == 0)
            {
                status = 1;
            }

            TicketingViewModel viewModel = new TicketingViewModel();
            viewModel = await _taskManagmentservice.Erplist(companyId, CompanyIdFK, fromDate, toDate, status);
            viewModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            viewModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            viewModel.Status = 1;

            viewModel.Companies = companyService.GetAllCompanySelectModels();
            viewModel.CompanyId = companyId;
          
            if (viewModel.CompanyIdFK != 0)
            {
                viewModel.CompanyIdFK = CompanyIdFK;
            }
            return View(viewModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> ERP_List(TicketingViewModel vm)
        {
            Session["CompanyId"] = vm.CompanyId;
            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);
            return RedirectToAction(nameof(ERP_List), new { companyId = vm.CompanyId, CompanyIdFK = vm.CompanyIdFK,  fromDate = vm.FromDate, toDate = vm.ToDate, status = vm.Status });
        }




        [HttpGet]
        
        public async Task<ActionResult> Network_List(int companyId, int CompanyIdFK = 0, DateTime? fromDate = null, DateTime? toDate = null, int status = 1)
        {

            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now.AddDays(+1);
            }
            if (status == 0)
            {
                status = 1;
            }

            TicketingViewModel viewModel = new TicketingViewModel();
            viewModel = await _taskManagmentservice.Networklist(companyId, CompanyIdFK, fromDate, toDate, status);
            viewModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            viewModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            viewModel.Status = 1;

            viewModel.Companies = companyService.GetAllCompanySelectModels();
            viewModel.CompanyId = companyId;

            if (viewModel.CompanyIdFK != 0)
            {
                viewModel.CompanyIdFK = CompanyIdFK;
            }
            return View(viewModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> Network_List(TicketingViewModel vm)
        {
            Session["CompanyId"] = vm.CompanyId;
            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);
            return RedirectToAction(nameof(Network_List), new { companyId = vm.CompanyId, CompanyIdFK = vm.CompanyIdFK, fromDate = vm.FromDate, toDate = vm.ToDate, status = vm.Status });
        }

   

        [HttpGet]
        
        public async Task<ActionResult> Admin_List(int companyId, int CompanyIdFK = 0, DateTime? fromDate = null, DateTime? toDate = null, int status = 1)
        {

            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now.AddDays(+1);
            }
            if (status == 0)
            {
                status = 1;
            }

            TicketingViewModel viewModel = new TicketingViewModel();
            viewModel = await _taskManagmentservice.Adminlist(companyId, CompanyIdFK, fromDate, toDate, status);
            viewModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            viewModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            viewModel.Status = 1;

            viewModel.Companies = companyService.GetAllCompanySelectModels();
            viewModel.CompanyId = companyId;

            if (viewModel.CompanyIdFK != 0)
            {
                viewModel.CompanyIdFK = CompanyIdFK;
            }
            return View(viewModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> Admin_List(TicketingViewModel vm)
        {
            Session["CompanyId"] = vm.CompanyId;
            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);
            return RedirectToAction(nameof(Admin_List), new { companyId = vm.CompanyId, CompanyIdFK = vm.CompanyIdFK, fromDate = vm.FromDate, toDate = vm.ToDate, status = vm.Status });
        }



        [HttpPost]
        
        public async Task<ActionResult> Accounts_List(TicketingViewModel vm)
        {
            Session["CompanyId"] = vm.CompanyId;
            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);
            return RedirectToAction(nameof(Accounts_List), new { companyId = vm.CompanyId, CompanyIdFK = vm.CompanyIdFK, fromDate = vm.FromDate, toDate = vm.ToDate, status = vm.Status });
        }

        [HttpGet]
        
        public async Task<ActionResult> Accounts_List(int companyId, int CompanyIdFK = 0, DateTime? fromDate = null, DateTime? toDate = null, int status = 1)
        {

            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now.AddDays(+1);
            }
            if (status == 0)
            {
                status = 1;
            }

            TicketingViewModel viewModel = new TicketingViewModel();
            viewModel = await _taskManagmentservice.Adminlist(companyId, CompanyIdFK, fromDate, toDate, status);
            viewModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            viewModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            viewModel.Status = 1;

            viewModel.Companies = companyService.GetAllCompanySelectModels();
            viewModel.CompanyId = companyId;

            if (viewModel.CompanyIdFK != 0)
            {
                viewModel.CompanyIdFK = CompanyIdFK;
            }
            return View(viewModel);
        }



        [HttpGet]
        
        public async Task<ActionResult> Engineering_List(int companyId, int CompanyIdFK = 0, DateTime? fromDate = null, DateTime? toDate = null, int status = 1)
        {

            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now.AddDays(+1);
            }
            if (status == 0)
            {
                status = 1;
            }

            TicketingViewModel viewModel = new TicketingViewModel();
            viewModel = await _taskManagmentservice.Engineeringlist(companyId, CompanyIdFK, fromDate, toDate, status);
            viewModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            viewModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            viewModel.Status = 1;

            viewModel.Companies = companyService.GetAllCompanySelectModels();
            viewModel.CompanyId = companyId;

            if (viewModel.CompanyIdFK != 0)
            {
                viewModel.CompanyIdFK = CompanyIdFK;
            }
            return View(viewModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> Engineering_List(TicketingViewModel vm)
        {
            Session["CompanyId"] = vm.CompanyId;
            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);
            return RedirectToAction(nameof(Engineering_List), new { companyId = vm.CompanyId, CompanyIdFK = vm.CompanyIdFK, fromDate = vm.FromDate, toDate = vm.ToDate, status = vm.Status });
        }






    }
}