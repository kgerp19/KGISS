using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using KGERP.Data.CustomModel;
using KGERP.Data.CustomViewModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Leave.ViewModels;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Service.Utility;
using KGERP.Utility;
using KGERP.ViewModel;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;
using static KGERP.Controllers.Custom_Authorization.ParentAuthorizedAttribute;

namespace KGERP.Controllers
{
    [CheckSession]
    public class LeaveApplicationController : Controller
    {
        private readonly ILeaveApplicationService leaveApplicationService;
        private readonly ILeaveCategoryService leaveCategoryService;
        private readonly IEmployeeService employeeService;
        public LeaveApplicationController(ILeaveApplicationService leaveApplicationService, ILeaveCategoryService leaveCategoryService, IEmployeeService employeeService)
        {
            this.leaveApplicationService = leaveApplicationService;
            this.leaveCategoryService = leaveCategoryService;
            this.employeeService = employeeService;
        }

        #region Leave_Application_Old
        [HttpGet]
        public async Task<ActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            int year = DateTime.Now.Year;
            if (DateTime.Now.Month < 7)
            {
                year -= 1;
            }
            if (fromDate == null)
            {
                fromDate = new DateTime(year, 7, 1);
            }
            if (toDate == null)
            {
                toDate = fromDate.Value.AddMonths(12);
            }
            LeaveApplicationVm model = new LeaveApplicationVm();
            model = await leaveApplicationService.GetLeaveApplicationByEmployee(fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(model);
        }
        [HttpPost]

        public async Task<ActionResult> Index(LeaveApplicationVm model)
        {
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { fromDate = model.FromDate, toDate = model.ToDate });
        }
        [HttpGet]
        public ActionResult CreateOrEdit(long id)
        {
            LeaveApplicationViewModel vm = new LeaveApplicationViewModel();
            vm.LeaveApplication = leaveApplicationService.GetLeaveApplication(id);
            vm.LeaveCategories = new List<SelectModel>();
            vm.LeaveBalance = leaveApplicationService.GetLeaveBalance();
            //for probation period employee balance
            if (vm.LeaveBalance == null || vm.LeaveBalance.Count() == 0)
            {
                ERPEntities _context = new ERPEntities();
                if (id <= 0)
                {
                    id = Common.GetIntUserId();
                }
                var isProbitionEmployee = _context.Employees.FirstOrDefault(x => x.Active && x.Id == id && x.JobStatusId == 41);
                if (isProbitionEmployee != null)
                {
                    vm.LeaveBalance = new List<LeaveBalanceCustomModel>()
                    {
                        new LeaveBalanceCustomModel(){ Employee = id, EmployeeId  = id, LeaveBalance = 10, LeaveCategoryId = (int)LeaveTypeEnum.CasualLeave,
                        LeaveCategory = LeaveTypeEnum.CasualLeave.ToString(),
                        Year = DateTime.Now.Month > 6?DateTime.Now.Year+1:DateTime.Now.Year,
                        MaxDays = 10, LeaveAvailed = 0,
                         TotalLeave = 10 }
                    };
                }
            }
            if (vm.LeaveBalance != null)
            {
                vm.LeaveCategories = (from x in vm.LeaveBalance
                                      where x.LeaveBalance > 0
                                      select new SelectModel()
                                      {
                                          Text = x.LeaveCategory,
                                          Value = x.LeaveCategoryId
                                      }).ToList();
            }

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(LeaveApplicationViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            bool sigApprovalResult = false;
            vm.LeaveApplication.Id = Common.GetIntUserId();
            vm.LeaveApplication.IP = Request.UserHostAddress;
            if (vm.LeaveApplication.LeaveApplicationId <= 0)
            {
                result = leaveApplicationService.SaveLeaveApplication(0, vm.LeaveApplication, out message);
            }

            else
            {
                result = leaveApplicationService.SaveLeaveApplication(vm.LeaveApplication.LeaveApplicationId, vm.LeaveApplication, out message);
            }
            TempData["errorMessage"] = message;


            if (!result)
            {
                vm.LeaveApplication = leaveApplicationService.GetLeaveApplication(0);
                vm.LeaveCategories = leaveCategoryService.GetLeaveCategorySelectModels();
                vm.LeaveBalance = leaveApplicationService.GetLeaveBalance();
                return View("CreateOrEdit", vm);
            }
            else
            {
                TempData["successMessage"] = "Application Submitted Successfully";
                //sigApprovalResult = leaveApplicationService.SignatoryApprovalSave(vm.LeaveApplication);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ManagerLeaveApproval(int? Page_No, string searchText)
        {
            searchText = searchText == null ? "" : searchText;
            ViewBag.Title = "Manager Leave Approval";
            List<LeaveApplicationModel> leaveApplications = leaveApplicationService.GetManagerLeaveApprovals(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(leaveApplications.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        [HttpGet]
        public ActionResult ChangeMangerStatus(long leaveApplicationId, string Approved, string Denied, string comments)
        {
            //string status = string.IsNullOrEmpty(Approved) ? Denied : Approved;
            LeaveStatusEnum status = string.IsNullOrEmpty(Approved) ? LeaveStatusEnum.Denied : LeaveStatusEnum.Approved;
            string ip = Request.UserHostAddress;
            //bool result = leaveApplicationService.ChangeMangerStatus(leaveApplicationId, status, comments, ip);
            string feedbackMessage = "";
            bool result = leaveApplicationService.UpdateLeaveStatus(leaveApplicationId, status, comments, ip, ApprovarTypeEnum.Manager, out feedbackMessage);
            return RedirectToAction("ManagerLeaveApproval");
        }



        [HttpGet]
        public ActionResult HRLeaveApproval(int? Page_No, string searchText)
        {
            searchText = searchText == null ? "" : searchText;
            ViewBag.Title = "HR Admin Leave Approval";
            List<LeaveApplicationModel> leaveApplications = leaveApplicationService.GetHRLeaveApprovals(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(leaveApplications.ToPagedList(No_Of_Page, Size_Of_Page));
        }




        [HttpGet]
        public ActionResult ChangeHRStatus(long leaveApplicationId, string Approved, string Denied, string comments)
        {
            //string status = string.IsNullOrEmpty(Approved) ? Denied : Approved;
            //string ip = Request.UserHostAddress;
            //bool result = leaveApplicationService.ChangeHRStatus(leaveApplicationId, status, comments, ip);
            LeaveStatusEnum status = string.IsNullOrEmpty(Approved) ? LeaveStatusEnum.Denied : LeaveStatusEnum.Approved;
            string ip = Request.UserHostAddress;
            string feedbackMessage = "";
            bool result = leaveApplicationService.UpdateLeaveStatus(leaveApplicationId, status, comments, ip, ApprovarTypeEnum.HRAdmin, out feedbackMessage);
            return RedirectToAction("HRLeaveApproval");
        }



        public ActionResult Delete(long id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveApplicationModel leaveApplication = leaveApplicationService.GetLeaveApplication(id);
            if (leaveApplication == null)
            {
                return HttpNotFound();
            }
            return View(leaveApplication);
        }
        [HttpGet]
        public ActionResult ProcessLeave()
        {
            string result = leaveApplicationService.ProcessLeave();
            ViewBag.message = result;
            return View();
        }
        #endregion

        #region suffixPrefix
        [HttpPost]
        public ActionResult SuffixPrefixDeterminer(DateTime startDate, DateTime endDate)
        {
            long empId = Common.GetIntUserId();
            SuffixPrefixResult result = leaveApplicationService.isSuffixPrefix(startDate, endDate, empId);
            return Json(new { result });
        }
       
        [HttpPost]
        public ActionResult IsUnapprovedApplication()
        {
            long empId = Common.GetIntUserId();
            bool result = leaveApplicationService.isUnapprovedApplication(empId);
            return Json(new { result });
        }
        #endregion

        #region New Leave Apply
        [HttpGet]
        public async Task<ActionResult> IndexNew(DateTime? fromDate, DateTime? toDate)
        {
            int year = DateTime.Now.Year;
            if (DateTime.Now.Month < 7)
            {
                year -= 1;
            }
            if (fromDate == null)
            {
                fromDate = new DateTime(year, 7, 1);
            }
            if (toDate == null)
            {
                toDate = fromDate.Value.AddMonths(12);
            }
            LeaveApplicationVm model = new LeaveApplicationVm();
            model = await leaveApplicationService.GetLeaveApplicationByEmployeeNew(fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> IndexNew(LeaveApplicationVm model)
        {
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { fromDate = model.FromDate, toDate = model.ToDate });
        }
        [HttpGet]
        public ActionResult CreateOrEditNew(long id)
        {
            LeaveApplicationViewModel vm = new LeaveApplicationViewModel();
            vm.LeaveApplication = leaveApplicationService.GetLeaveApplication(id);
            vm.LeaveCategories = new List<SelectModel>();
            vm.LeaveBalance = leaveApplicationService.GetLeaveBalance();
            //for probation period employee balance
            if (vm.LeaveBalance == null || vm.LeaveBalance.Count() == 0)
            {
                ERPEntities _context = new ERPEntities();
                if (id <= 0)
                {
                    id = Common.GetIntUserId();
                }
                var isProbitionEmployee = _context.Employees.FirstOrDefault(x => x.Active && x.Id == id && x.JobStatusId == 41);
                if (isProbitionEmployee != null)
                {
                    vm.LeaveBalance = new List<LeaveBalanceCustomModel>()
                    {
                        new LeaveBalanceCustomModel(){ Employee = id, EmployeeId  = id, LeaveBalance = 10, LeaveCategoryId = (int)LeaveTypeEnum.CasualLeave,
                        LeaveCategory = LeaveTypeEnum.CasualLeave.ToString(),
                        Year = DateTime.Now.Month > 6?DateTime.Now.Year+1:DateTime.Now.Year,
                        MaxDays = 10, LeaveAvailed = 0,
                         TotalLeave = 10 }
                    };
                }
            }
            if (vm.LeaveBalance != null)
            {
                vm.LeaveCategories = (from x in vm.LeaveBalance
                                      where x.LeaveBalance > 0
                                      select new SelectModel()
                                      {
                                          Text = x.LeaveCategory,
                                          Value = x.LeaveCategoryId
                                      }).ToList();
            }

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditNew(LeaveApplicationViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            bool sigApprovalResult = false;
            vm.LeaveApplication.Id = Common.GetIntUserId();
            vm.LeaveApplication.IP = Request.UserHostAddress;
            if (vm.LeaveApplication.LeaveApplicationId <= 0 && vm.isSandwichLeave == 0)
            {
                result = leaveApplicationService.SaveLeaveApplication(0, vm.LeaveApplication, out message);
            }
            else
            {
                message = "Kindly apply appropriately";
            }

            //else
            //{
            //    result = leaveApplicationService.SaveLeaveApplication(vm.LeaveApplication.LeaveApplicationId, vm.LeaveApplication, out message);
            //}
            TempData["errorMessage"] = message;


            if (!result)
            {
                vm.LeaveApplication = leaveApplicationService.GetLeaveApplication(0);
                vm.LeaveCategories = leaveCategoryService.GetLeaveCategorySelectModels();
                vm.LeaveBalance = leaveApplicationService.GetLeaveBalance();
                return View("CreateOrEdit", vm);
            }
            else
            {
                TempData["successMessage"] = "Application Submitted Successfully";
                sigApprovalResult = leaveApplicationService.SignatoryApprovalSave(vm.LeaveApplication);
            }
            return RedirectToAction(nameof(LeaveHistoryNew));
        }
        [HttpGet]
        public ActionResult ManagerLeaveApprovalNew(int? Page_No, string searchText)
        {
            searchText = searchText == null ? "" : searchText;
            ViewBag.Title = "Manager Leave Approval";
            List<ManagerApproval> leaveApplications = leaveApplicationService.GetManagerLeaveApprovalsNew(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            //return View(leaveApplications.AllInfo.ToPagedList(No_Of_Page, Size_Of_Page));
            return View(leaveApplications.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [HttpGet]
        public async Task<ActionResult> LeaveApprovalNew(DateTime? fromDate, DateTime? toDate)
        {
            int year = DateTime.Now.Year;
            if (DateTime.Now.Month < 7)
            {
                year -= 1;
            }
            if (fromDate == null)
            {
                fromDate = new DateTime(year, 7, 1);
            }
            if (toDate == null)
            {
                toDate = fromDate.Value.AddMonths(12);
            }
            //fromDate = new DateTime(2016, 6, 1);
            long sigId = Common.GetIntUserId();
            LeaveAllDetailVM model = await leaveApplicationService.GetLeaveApprovalRelatedAllInfo(sigId,fromDate,toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> LeaveApprovalNew(LeaveAllDetailVM vm)
        {
            long sigId = Common.GetIntUserId();
            if (vm.LeaveApplicationID != 0)
            {
                var res = leaveApplicationService.DoLeaveApproval(vm);
            }

            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);
            return RedirectToAction(nameof(LeaveApprovalNew), new { fromDate = vm.FromDate, toDate = vm.ToDate });
            //return RedirectToAction(nameof(LeaveApprovalNew), new { fromDate = vm.FromDate, toDate = vm.ToDate });
        }


        [HttpGet]
        public async Task<ActionResult> LeaveHistoryNew(DateTime? fromDate, DateTime? toDate)
        {
            int year = DateTime.Now.Year;
            if (DateTime.Now.Month < 7)
            {
                year -= 1;
            }
            if (fromDate == null)
            {
                fromDate = new DateTime(year, 7, 1);
            }
            if (toDate == null)
            {
                toDate = fromDate.Value.AddMonths(12);
            }
            //fromDate = new DateTime(2016, 6, 1);
            long sigId = Common.GetIntUserId();
            LeaveApplicationVm model = await leaveApplicationService.GetLeaveListByEmployee(sigId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> LeaveHistoryFilter(LeaveApplicationVm model)
        {
            
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(LeaveHistoryNew), new { fromDate = model.FromDate, toDate = model.ToDate });
        }
        [HttpGet]
        public async Task<ActionResult> LeaveSignatories(long applicationId)
        {
            long sigId = Common.GetIntUserId();
            var res = await leaveApplicationService.GetSignatoriesApprovalInfo(applicationId,sigId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Leave_Balance
        [HttpGet]
        public ActionResult LeaveBalance()
        {
            List<LeaveBalanceCustomModel> leaveBalances = leaveApplicationService.GetLeaveBalance();
            return View(leaveBalances);
        }


        [HttpGet]
        public async Task<ActionResult> CompanywiseLeaveBalance(int selectedYear = 0)
        {
            TeamLeaveBalanceCustomModel model = new TeamLeaveBalanceCustomModel();
            if (selectedYear == 0)
            {
                model.SelectedYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            }
            else
            {
                model.SelectedYear = selectedYear;
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> CompanywiseLeaveBalance(TeamLeaveBalanceCustomModel model)
        {
            //TeamLeaveBalanceCustomModel model = new TeamLeaveBalanceCustomModel();
            if (model.SelectedYear == 0)
            {
                model.SelectedYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            }
            if (model.CompanyId == 0 && model.DepartmentId == 0)
            {
                var userIdd = Common.GetIntUserId();
                var empp = employeeService.GetEmployee(userIdd);
                model.CompanyId = (int)empp.CompanyId;
                model.CompanyName = empp.CompanyName;
            }
            model.DataList = await leaveApplicationService.GetCompanywiseLeaveBalance(model.CompanyId, model.DepartmentId, model.SelectedYear);
            return View(model);
        }



        [HttpGet]
        public async Task<ActionResult> TeamLeaveBalance(int selectedYear = 0)
        {
            TeamLeaveBalanceCustomModel model = new TeamLeaveBalanceCustomModel();
            if (selectedYear == 0)
            {
                model.SelectedYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            }
            else
            {
                model.SelectedYear = selectedYear;
            }

            model = await leaveApplicationService.GetTeamLeaveBalance(Convert.ToInt64(Session["Id"]), model.SelectedYear);
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> TeamLeaveBalance(TeamLeaveBalanceCustomModel model)
        {
            return RedirectToAction(nameof(TeamLeaveBalance), new { selectedYear = model.SelectedYear });
        }


        [HttpGet]
        public ActionResult EmployeeLeaveBalance(string employeeId)
        {
            employeeId = employeeId ?? string.Empty;
            EmployeeLeaveBalanceCustomModel cvm = new EmployeeLeaveBalanceCustomModel();
            cvm.LeaveBalanceCustomModels = leaveApplicationService.GetEmployeeLeaveBalance(employeeId, out string message);
            cvm.EmployeeCustomModel = leaveApplicationService.GetCustomEmployeeModel(employeeId);
            ViewBag.message = message;
            if (!string.IsNullOrEmpty(employeeId))
            {
                ViewBag.leaveHistory = GetLeaveHistory(employeeId);
            }
            return View(cvm);
        }
        [HttpGet]
        public ActionResult EmployeeLeaveBalanceByIdnDateRange(string employeeId, string StartDate, string EndDate)
        {

            DateTime date = DateTime.Now;
            DateTime firstDayOfThisYear = new DateTime(date.Year, date.Month, 1);
            DateTime lastOfThisMonth = firstDayOfThisYear.AddMonths(1).AddDays(-1);
            DateTime FromDate = StartDate == null ? firstDayOfThisYear : Convert.ToDateTime(StartDate);
            DateTime ToDate = EndDate == null ? lastOfThisMonth : Convert.ToDateTime(EndDate);


            ViewBag.FromDate = Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy");


            employeeId = employeeId ?? string.Empty;
            EmployeeLeaveBalanceCustomModel cvm = new EmployeeLeaveBalanceCustomModel();
            cvm.LeaveBalanceCustomModels = leaveApplicationService.GetEmployeeLeaveBalanceByIdDateRange(employeeId, FromDate, ToDate, out string message);
            cvm.EmployeeCustomModel = leaveApplicationService.GetCustomEmployeeModel(employeeId);
            ViewBag.message = message;
            return View(cvm);

        }

        #endregion

        #region Leave History by Manager search

        public string GetLeaveHistory(string employeeId)
        {
            string htmlStr = "";
            DataTable dt = new DataTable();
            dt = GetEmployeeHistoryByEmployeeId(employeeId);
            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                //Table start.
                sb.Append("<table cellpadding='5' cellspacing='0' style='width:100%; border: 1px solid #ccc;font-size: 10pt;font-family:Arial'>");
                //Adding HeaderRow.
                sb.Append("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    sb.Append("<th style='background-color: #009270; padding:5px; color:white; border: 1px;' align='center'>" + column.ColumnName + "</th>");
                }
                sb.Append("</tr>");

                //Adding DataRow.
                foreach (DataRow row in dt.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName == "Reason")
                        {
                            sb.Append("<td style='width:200px;border: 1px solid #ccc' align='left'>" + row[column.ColumnName].ToString() + "</td>");
                        }
                        else if (column.ColumnName == "Days")
                        {
                            sb.Append("<td style='width:40px;border: 1px solid #ccc' align='center'>" + row[column.ColumnName].ToString() + "</td>");
                        }
                        else
                        {
                            sb.Append("<td style='width:80px;border: 1px solid #ccc' align='center'>" + row[column.ColumnName].ToString() + "</td>");
                        }
                    }
                    sb.Append("</tr>");
                }

                return htmlStr = sb.ToString();
            }
            else
            {
                return htmlStr = "No Data Found";
            }
        }

        private DataTable GetEmployeeHistoryByEmployeeId(string employeeId)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("LeaveHistoryByEmployeeId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        #endregion

        #region Others
        public ActionResult OtherIndex(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            List<LeaveApplicationModel> leaveApplications = leaveApplicationService.GetLeaveApplicationsByOther(searchText, Size_Of_Page, No_Of_Page);
            //return View(leaveApplications);
            return View(leaveApplications.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        [HttpPost]
        public ActionResult OtherCreate(long employeeId)
        {

            //kgId = kgId ?? string.Empty;
            bool isEmployee = employeeService.CheckEmployee(employeeId);
            if (!isEmployee)
            {
                return RedirectToAction("OtherIndex");
            }
            LeaveApplicationViewModel vm = new LeaveApplicationViewModel();
            vm.LeaveApplication = leaveApplicationService.GetLeaveApplicationByOther(0, employeeId);
            vm.LeaveCategories = leaveCategoryService.GetLeaveCategorySelectModels();
            vm.LeaveBalance = leaveApplicationService.GetLeaveBalance(employeeId);
            //for probation period employee balance
            if (vm.LeaveBalance == null || vm.LeaveBalance.Count() == 0)
            {
                ERPEntities _context = new ERPEntities();
                var isProbitionEmployee = _context.Employees.FirstOrDefault(x => x.Active && x.Id == employeeId && x.JobStatusId == 41);
                if (isProbitionEmployee != null)
                {
                    vm.LeaveBalance = new List<LeaveBalanceCustomModel>()
                    {
                        new LeaveBalanceCustomModel(){ Employee = employeeId, EmployeeId  = employeeId, LeaveBalance = 10, LeaveCategoryId = (int)LeaveTypeEnum.CasualLeave,
                            LeaveCategory = LeaveTypeEnum.CasualLeave.ToString(),
                        Year = DateTime.Now.Month > 6?DateTime.Now.Year+1:DateTime.Now.Year,
                        MaxDays = 10, LeaveAvailed = 0,
                         TotalLeave = 10 }
                    };
                }
            }
            if (vm.LeaveBalance != null)
            {
                vm.LeaveCategories = (from x in vm.LeaveBalance
                                      where x.LeaveBalance > 0
                                      select new SelectModel()
                                      {
                                          Text = x.LeaveCategory,
                                          Value = x.LeaveCategoryId
                                      }).ToList();
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OthersLeaveCreate(LeaveApplicationViewModel vm)
        {
            string message = string.Empty;
            bool result = false;
            //result = leaveApplicationService.SaveOtherLeaveApplication(0, vm.LeaveApplication, vm.LeaveApplication.Id, out message);
            result = leaveApplicationService.SaveLeaveApplication(0, vm.LeaveApplication, out message);
            if(result)
            {
                leaveApplicationService.SignatoryAutoApprovalSave(vm.LeaveApplication);
            }
            TempData["errorMessage"] = message;

            if (!result)
            {
                vm.LeaveApplication = leaveApplicationService.GetLeaveApplicationByOther(0, vm.LeaveApplication.Id);
                vm.LeaveCategories = leaveCategoryService.GetLeaveCategorySelectModels();
                vm.LeaveBalance = leaveApplicationService.GetLeaveBalance(vm.LeaveApplication.Id);
                return View("OtherCreate", vm);
            }
            else
            {
                string approvalMessage = "";
                var isApproved = leaveApplicationService.UpdateLeaveStatus(vm.LeaveApplication.LeaveApplicationId, LeaveStatusEnum.Approved, "Approved by hardcopy document", "", ApprovarTypeEnum.HRAdmin, out approvalMessage);
                TempData["successMessage"] = "Application Submitted Successfully For Employee";
            }
            return RedirectToAction("OtherIndex");
        }


        [HttpGet]
        public ActionResult ApprovalOfficerAssign(int CompanyId)
        {
            LeaveApplicationVmm vm= new LeaveApplicationVmm();
            vm.CompanyId= CompanyId;

            return View(vm);
        }
        [HttpPost]
        public ActionResult ManagerAndHr(long empId)
        {
            var obj = leaveApplicationService.ManagerAndHrLook(empId);
            return Json(obj);
        }




        [HttpPost]
        public ActionResult AssignSignatory(LeaveApplicationVmm viewModel)
        {
            var obj = leaveApplicationService.LeaveSignatoryAssign(viewModel);
            return Json(new { success = true, message = "Data received successfully." });
        }



        //[HttpPost]
        //public ActionResult AssignSignatory(LeaveApplicationVmm viewModel)
        //{

        //    return Json(new { success = true });
        //}


        [HttpPost]
        public ActionResult RemoveSignatory(long requisitionSignatoryId)
        {
            var obj= leaveApplicationService.RemoveSignatory(requisitionSignatoryId);
            if (obj)
            {
                return Json(new { success = true, message = "Data removed successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Can not remove the signatory!" });
            }
        }



        #endregion
        #region Test
        [HttpGet]
        public ActionResult CheckNewManagerAddedToSignatory()
        {
            var isTrue = leaveApplicationService.CheckNewManagerAddedInSignatory();
            return View();
        }

        #endregion



        [HttpGet]
        public async Task<ActionResult> AnnualApprisalApproval(DateTime? fromDate, DateTime? toDate)
        {
            int year = DateTime.Now.Year;
            if (DateTime.Now.Month < 7)
            {
                year -= 1;
            }
            if (fromDate == null)
            {
                fromDate = new DateTime(year, 7, 1);
            }
            if (toDate == null)
            {
                toDate = fromDate.Value.AddMonths(12);
            }
            long sigId = Common.GetIntUserId();
            LeaveAllDetailVM model = await leaveApplicationService.GetLeaveApprovalRelatedAllInfo(sigId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AnnualApprisalApproval(LeaveAllDetailVM vm)
        {
            long sigId = Common.GetIntUserId();
            if (vm.LeaveApplicationID != 0)
            {
                var res = leaveApplicationService.DoLeaveApproval(vm);
            }

            vm.FromDate = Convert.ToDateTime(vm.StrFromDate);
            vm.ToDate = Convert.ToDateTime(vm.StrToDate);
            return RedirectToAction(nameof(LeaveApprovalNew), new { fromDate = vm.FromDate, toDate = vm.ToDate });
        }

    }
}