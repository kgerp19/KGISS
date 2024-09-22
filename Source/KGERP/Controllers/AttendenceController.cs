using DocumentFormat.OpenXml.Presentation;
using KGERP.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.ParollServices;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class AttendenceController : Controller
    {

        IAttendenceService attendenceService = new AttendenceService();
        // GET: Attendence
        
        public ActionResult GetDailyAttendebce(int? Page_No, string searchText, DateTime? AttendenceDate)
        {
            if (String.IsNullOrEmpty(searchText))
                searchText = string.Empty;
            //List<AttendenceEntity> attendence = attendenceService.GetDailyAttendence();
            DateTime date;
            if (AttendenceDate == null)
            {
                date = DateTime.Today;
            }
            else
            {
                date = Convert.ToDateTime(AttendenceDate);
            }


            List<AttendenceEntity> fileteredData = attendenceService.GetDailyAttendence(date).Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || x.EmployeeId.ToLower().Contains(searchText.ToLower()) || x.EmpStatus.ToLower().Contains(searchText.ToLower())).ToList();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(fileteredData.ToPagedList(No_Of_Page, Size_Of_Page));

            //return View(attendence);
        }

        [HttpGet]
        public async Task<ActionResult> AttendanceSummary(string fromDateString, string toDateString, int officeId = 1)
        {
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today;
    
            if (!string.IsNullOrEmpty(fromDateString))
            {
                try
                {
                    fromDate = Convert.ToDateTime(fromDateString);
                }
                catch (Exception e)
                {
                    
                }
            }
            if (!string.IsNullOrEmpty(toDateString))
            {
                try
                {
                    toDate = Convert.ToDateTime(toDateString);
                }
                catch (Exception e)
                {

                }
            }
            ViewBag.officeId = officeId;
            bool isMultipleDay = (toDate - fromDate).TotalDays > 0 ? true : false;
            List<AttendanceSummaryVM> attendanceSummaryListAll = new List<AttendanceSummaryVM>();
            ERPEntities _context = new ERPEntities();
            do
            {
                string dateString = fromDate.ToString("yyyy-MM-dd");

                List<AttendanceSummaryVM> attendanceSummaryList = _context.Database.SqlQuery<AttendanceSummaryVM>("Exec GetDaywiseAttendance {0}, {1}", dateString, officeId).ToList();
                if (attendanceSummaryList != null && attendanceSummaryList.Count() > 0)
                {
                    attendanceSummaryListAll.AddRange(attendanceSummaryList);
                }
                fromDate = fromDate.AddDays(1);
            } while (toDate >= fromDate);

            if (isMultipleDay)
            {

               return View(attendanceSummaryListAll.OrderBy(x => x.CompanyName).ThenBy(x => x.DepartmentName).ThenBy(x => x.EmployeeId).ToList());
            }
            return View(attendanceSummaryListAll);

        }


        
        public async Task<ActionResult> GetDailyAttendanceTeamWise(DateTime? fromDate, DateTime? toDate)
        {
            var managerId = Convert.ToInt64(Session["Id"].ToString());
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-1);
            }
            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            AttendanceVm model = new AttendanceVm();

            model = await attendenceService.GetDailyAttendanceTeamWise(managerId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);

        }
        [HttpPost]

        public async Task<ActionResult> GetDailyAttendanceTeamWise(AttendanceVm model)
        {
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(GetDailyAttendanceTeamWise), new { fromDate = model.FromDate, toDate = model.ToDate });
        }
        public async Task<ActionResult> GetDailyAttendanceOfTeam(DateTime? fromDate, DateTime? toDate)
        {
            var managerId = Convert.ToInt64(Session["Id"].ToString());
            if(managerId == Common.GetHRAdminId())
            {
                if (toDate == null)
                {
                    toDate = DateTime.Now;
                }
                if (fromDate == null)
                {
                    fromDate = toDate.Value.AddDays(-7);
                }

            }
            else
            {
                if (fromDate == null)
                {
                    fromDate = DateTime.Now.AddMonths(-1);
                }
                if (toDate == null)
                {
                    toDate = DateTime.Now;
                }
            }           
            AttendanceVm model = new AttendanceVm();

            model = await attendenceService.GetDailyAttendanceOfTeam(managerId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]

        public async Task<ActionResult> GetDailyAttendanceOfTeam(AttendanceVm model)
        {
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(GetDailyAttendanceOfTeam), new { fromDate = model.FromDate, toDate = model.ToDate });
        }


        // GET: Attendence/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Attendence/Create
        
        public ActionResult Create()
        {
            return View();
        }

        // POST: Attendence/Create
        
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        
        [HttpGet]
        public async Task<ActionResult> GetEmployeeAttendance(DateTime? fromDate, DateTime? toDate)
        {
            DateTime date = DateTime.Now;
            fromDate = fromDate.HasValue ? fromDate : new DateTime(date.Year, date.Month, 1);
            toDate = toDate.HasValue ? toDate : fromDate.Value.AddMonths(1).AddDays(-1);
            AttendanceVm model = new AttendanceVm();
            model = await attendenceService.GetSelfAttendance(fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(model);
        }
        [HttpPost]
        
        public async Task<ActionResult> GetEmployeeAttendance(AttendanceVm model)
        {
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(GetEmployeeAttendance), new { fromDate = model.FromDate, toDate = model.ToDate });
        }


        // GET: Attendence/Edit/5
        
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Attendence/Edit/5
        [HttpPost]
        
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Attendence/Delete/5
        
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Attendence/Delete/5
        [HttpPost]
        
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //[HttpGet]
        //
        //public ActionResult GetExecution(AttendanceVm model)
        //{
        //    DateTime date = DateTime.Now;
        //    fromDate = fromDate.HasValue ? fromDate : new DateTime(date.Year, date.Month, 1);
        //    toDate = toDate.HasValue ? toDate : fromDate.Value.AddMonths(1).AddDays(-1);
        //    AttendanceVm model = new AttendanceVm();
        //    model = await attendenceService.GetSelfAttendance(fromDate, toDate);
        //    model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
        //    model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            

        //    return View();
        //}




        
        [HttpGet]
        public async Task<ActionResult> GetExecution(DateTime? fromDate)
        {
            DateTime date = DateTime.Now;
            fromDate = fromDate.HasValue ? fromDate : new DateTime(date.Year, date.Month, 1);
            AttendanceVm model = new AttendanceVm();
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            return View(model);
        }
        public ActionResult Execution(AttendanceVm model)
        {
            var result = attendenceService.excuteattendance(model.StrFromDate);

            if (result)
            {
                TempData["SuccessMessage"] = "Attendance execution successful!";
            }

            return RedirectToAction("GetExecution");
        }



        
        [HttpGet]
        public async Task<ActionResult> GetMechineData(DateTime? fromDate)
        {
            DateTime date = DateTime.Now;
            fromDate = fromDate.HasValue ? fromDate : new DateTime(date.Year, date.Month, 1);
            AttendanceVm model = new AttendanceVm();
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            return View(model);
        }
        public ActionResult MachinedataExecution(AttendanceVm model)
        {
            var result = attendenceService.excutemachinedata(model.StrFromDate);

            if (result)
            {
                TempData["SuccessMessage"] = "Collected  Machine Data successful!";
            }

            return RedirectToAction("GetMechineData");
        }
    }
}
