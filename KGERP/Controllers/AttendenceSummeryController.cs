using KGERP.CustomModel;
using KGERP.Data.CustomModel;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Utility;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class AttendenceSummeryController : Controller
    {
        IAttendenceService attendenceService = new AttendenceService();
        IDepartmentService departmentService = new DepartmentService();
        IEmployeeService employeeService = new EmployeeService(new Data.Models.ERPEntities());



        //public ActionResult MonthlyAttendenceSummery()
        //{
        //    List<AttendenceEntity> attendences = null;
        //    if (TempData["attendences"]!=null)
        //    {
        //        attendences = TempData["attendences"] as List<AttendenceEntity>;
        //        return View(attendences);
        //    }
        //    return View(new List<AttendenceEntity>());
        //}
        
        public JsonResult GetDepartment()
        {
            var data = departmentService.GetDepartmentSelectModels();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetEmployee()
        {
            var data = employeeService.GetEmployeeSelectModels(); ;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> MonthlyAttendanceSummery( DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null)
            {
                fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if (toDate == null)
            {
                toDate = DateTime.Today;
            }
            AttendenceSummeries attenModel = new AttendenceSummeries();

            attenModel = await attendenceService.MonthlyAttendanceSummery( fromDate, toDate);

            attenModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            attenModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(attenModel);
        }
        [HttpPost]
        public async Task<ActionResult> MonthlyAttendanceSummery(AttendenceSummeries model)
        {
           
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(MonthlyAttendanceSummery), new { fromDate = model.FromDate, toDate = model.ToDate });
        }

        //
        //public async ActionResult MonthlyAttendanceSummery(int? Page_No, string searchText, DateTime? startDate, DateTime? endDate)
        //{

        //    DateTime now = DateTime.Now;
        //    DateTime fromDate = DateTime.Now;
        //    DateTime toDate = DateTime.Now;
        //    //List<AttendenceSummeries> result = new List<AttendenceSummeries>();
        //    if (startDate == null)
        //    {
        //        fromDate = new DateTime(now.Year, now.Month, 1);
        //    }
        //    else
        //    {
        //        fromDate = startDate.Value;
        //    }
        //    if (endDate == null)
        //    {
        //        toDate = fromDate.AddMonths(1).AddDays(-1);
        //    }

        //    else
        //    {
        //        toDate = endDate.Value;
        //    }

        //    ViewBag.FromDate = fromDate.ToShortDateString();
        //    ViewBag.ToDate = toDate.ToShortDateString();
        //    ViewBag.Search = searchText;

        //    if (!string.IsNullOrEmpty(searchText))
        //    {
        //        var result =await attendenceService.MonthlyAttendanceSummery(fromDate, toDate)
        //            .Where(x => x.EmployeeId.ToLower() == searchText.ToLower() || x.Department.ToLower()
        //            .Contains(searchText.ToLower()) || x.Designation.ToLower().Contains(searchText.ToLower()));

        //        int Size_Of_Page = 10;
        //        int No_Of_Page = (Page_No ?? 1);
        //        return View(result.ToPagedList(No_Of_Page, Size_Of_Page));

        //    }

        //    else
        //    {
        //        var result = attendenceService.MonthlyAttendanceSummery(fromDate, toDate);
        //        int Size_Of_Page = 15;
        //        int No_Of_Page = (Page_No ?? 1);
        //        return View(result.ToPagedList(No_Of_Page, Size_Of_Page));

        //    }

        //}

        
        public ActionResult GetEmployeeAttendence(DateTime? FromDate, DateTime? ToDate, int? EmployeeId, int? DepartmentId)
        {

            string empId;
            if (EmployeeId != null)
            {
                empId = attendenceService.GetEmpId(EmployeeId);
            }
            else
            {
                empId = null;
            }
            var fdate = Convert.ToDateTime(FromDate).ToString("yyyyMMdd");
            var tdate = Convert.ToDateTime(ToDate).ToString("yyyyMMdd");
            List<AttendenceEntity> attendences = attendenceService.GetEmployeeAttendence(fdate, tdate, empId, DepartmentId);
            TempData["attendences"] = attendences;

            return RedirectToAction("MonthlyAttendenceSummery");

        }





        
        //public ActionResult GetEmployeeAttendence()
        //{
        //  //  string empId = employeeId.Substring(1, 6);
        //    List<AttendenceEntity> attendences = attendenceService.GetEmployeeAttendence();
        //    TempData["attendences"] = attendences;
        //    //  var data = attendenceService.GetEmployeeAttendence(fromDate, toDate, employeeId, departmentId);
        //    return RedirectToAction("MonthlyAttendenceSummery");
        //}
    }
}