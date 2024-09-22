using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AttendanceLog_Service
{
    public class PRoll_AttendanceLogService : IPRoll_AttendanceLogService
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddNew(PRoll_AttendanceLogViewModel item)
        {
            PRoll_AttendanceLog model = new PRoll_AttendanceLog();
            model.Month = item.Month;
            model.Year = item.Year;
            model.IsActive = true;
            model.ToDate = Convert.ToDateTime(item.ToDate);
            model.FromDate = Convert.ToDateTime(item.FromDate);
            model.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.CreatedDate = DateTime.Now;
            context.PRoll_AttendanceLog.Add(model);
            var res = await context.SaveChangesAsync();
            if (res > 0)
            {
                return model.AttendanceLogId;
            }
            return 0;
        }

        public async Task<PRoll_AttendanceLogViewModel> AttendanceList(int companyId)
        {
            PRoll_AttendanceLogViewModel pRoll = new PRoll_AttendanceLogViewModel();
            pRoll.CompanyId = companyId;
            pRoll.dataList = await Task.Run(() => (from t1 in context.PRoll_AttendanceLog
                                                   where t1.IsActive == true
                                                   select new PRoll_AttendanceLogViewModel
                                                   {
                                                       CompanyId = companyId,
                                                       AttendanceLogId = t1.AttendanceLogId,
                                                       Year = t1.Year,
                                                       Month = t1.Month,
                                                       CreatedDate = t1.CreatedDate,
                                                       CreatedBy = t1.CreatedBy,
                                                       IsFinalize = t1.IsFinalize
                                                   }).OrderByDescending(f => f.AttendanceLogId).ToListAsync());
            return pRoll;
        }

        public async Task<long> AttendanceProcessPayroll(long id)
        {
            var res = await context.PRoll_AttendanceLog.FirstOrDefaultAsync(f => f.AttendanceLogId == id);
            try
            {

                var result = context.Database.ExecuteSqlCommand("exec usp_GetCalculativeAttendanceForPayRoll {0},{1},{2}", res.AttendanceLogId, res.FromDate, res.ToDate);
                return res.AttendanceLogId;
            }
            catch (DbEntityValidationException e)
            {
                throw;

            }
            return 0;
        }

        public async Task<long> Delete(long id)
        {
            var result = await context.PRoll_AttendanceLog.FirstOrDefaultAsync(f => f.AttendanceLogId == id);
            result.IsActive = false;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return result.AttendanceLogId;
        }

        public async Task<PRoll_AttendanceLogViewModel> Details(int companyid, long id)
        {
            string CompanyName = "";
            var company = context.Companies.Where(x => x.CompanyId == companyid).FirstOrDefault();

            CompanyName = company != null ? company.Name : "All Companies";
            DateTimeFormatInfo formatInfo = new DateTimeFormatInfo();
            PRoll_AttendanceLogViewModel pRoll = new PRoll_AttendanceLogViewModel();
            pRoll = await Task.Run(() => (from t1 in context.PRoll_AttendanceLog
                                          where t1.IsActive == true && t1.AttendanceLogId == id
                                          select new PRoll_AttendanceLogViewModel
                                          {
                                              CompanyName = CompanyName,
                                              AttendanceLogId = t1.AttendanceLogId,
                                              Year = t1.Year,
                                              Month = t1.Month,
                                              ToDate = t1.ToDate,
                                              FromDate = t1.FromDate,
                                              CreatedDate = t1.CreatedDate,
                                              CreatedBy = t1.CreatedBy,
                                              IsFinalize = t1.IsFinalize
                                          }).FirstOrDefaultAsync());
            pRoll.MonthName = formatInfo.GetMonthName(pRoll.Month);



            pRoll.detailViewModels = (from t1 in context.PRoll_AttendanceLog
                                      join t2 in context.PRoll_AttendanceLogDetail on t1.AttendanceLogId equals t2.AttendanceLogId
                                      join t3 in context.Employees on t2.EmployeeId equals t3.Id
                                      join t4 in context.Departments on t3.DepartmentId equals t4.DepartmentId into d
                                      from t4 in d.DefaultIfEmpty()
                                      join t5 in context.Designations on t3.DesignationId equals t5.DesignationId into c
                                      from t5 in c.DefaultIfEmpty()
                                      join t6 in context.Grades on t3.GradeId equals t6.GradeId into b
                                      from t6 in b.DefaultIfEmpty()
                                      where t1.AttendanceLogId == id && t1.IsActive == true && t2.IsActive == true
                                      && (companyid == 0 || t2.CompanyId == companyid)
                                      select new PRoll_AttendanceLogDetailViewModel
                                      {
                                          AttendanceLogId = t2.AttendanceLogId,
                                          AttendanceLogDetail = t2.AttendanceLogDetail,
                                          EmployeeId = t3.Id,
                                          GradeName = t6.GradeCode + " - " + t6.Name,
                                          EmployeeName = t3.EmployeeId + " - " + t3.Name,
                                          CompanyId = t2.CompanyId,
                                          DesignationName = t5.Name,
                                          DepartmentId = t4.DepartmentId,
                                          DepartmentName = t4.Name,
                                          EarlyOut = t2.EarlyOut,
                                          LateIn = t2.LateIn,
                                          LateInEarlyOut = t2.LateInEarlyOut,
                                          Absent = t2.Absent,
                                          DeductedDay = t2.DeductedDay,
                                          PayableDay = t2.PayableDay,
                                          OnTime = t2.OnTime,
                                          Present = t2.Present,
                                          Leave = t2.Leave,
                                          LeaveWithoutPay = t2.LeaveWithoutPay,
                                          Tour = t2.Tour,
                                          OffDay = t2.OffDay,
                                          OnField = t2.OnField,
                                      }).AsQueryable();
            pRoll.CompanyId = companyid;
            return pRoll;
        }

        public async Task<PRoll_AttendanceLogViewModel> DetailsEdit(PRoll_AttendanceLogViewModel model)
        {
            PRoll_AttendanceModifiedLog pRoll = new PRoll_AttendanceModifiedLog();
            int days = DateTime.DaysInMonth(model.Year, model.Month);
            var res = await context.PRoll_AttendanceLogDetail.FirstOrDefaultAsync(d => d.AttendanceLogDetail == model.detailViewModel.AttendanceLogDetail);
            pRoll.PreviousLog = "Payable Day : " + res.PayableDay.ToString() + " Deducted Day: " + res.DeductedDay.ToString();

            res.PayableDay = model.detailViewModel.PayableDay;
            res.DeductedDay = days - model.detailViewModel.PayableDay;
            res.ModifiedDate = DateTime.Now;
            res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            context.Entry(res).State = EntityState.Modified;
            context.SaveChanges();
            
            pRoll.NewLog = "Payable Day : " + res.PayableDay.ToString() + " Deducted Day: " + res.DeductedDay.ToString();
            pRoll.ModifiedDate = DateTime.Now;
            pRoll.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            pRoll.CompanyId = res.CompanyId;
            pRoll.EmployeeId = context.Employees.FirstOrDefault(d=>d.Id==res.EmployeeId).EmployeeId;
            pRoll.MonthAndYear = model.Month.ToString() + "-" + model.Year.ToString();
            context.PRoll_AttendanceModifiedLog.Add(pRoll);
            context.SaveChanges();
            return model;
        }


    }
}
