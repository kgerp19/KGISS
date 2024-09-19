using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Utility
{
    public static class LeaveService
    {
        private static readonly ERPEntities _context = new ERPEntities();
        public static int LeaveCalculation()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int status = 0;
                    var today = DateTime.Today;
                    int year = today.Month > 6 ? today.Year + 1 : today.Year;
                    int nextYear = year + 1;
                   // var leaveCategoryList = _context.LeaveCategories.Where(x => x.IsActive && x.LeaveCategoryId != (int)LeaveTypeEnum.SickLeave).ToList();
                    var leaveCategoryList = _context.LeaveCategories.Where(x => x.IsActive).ToList();
                    var lastJoiningDate = DateTime.Today.AddMonths(-6);
                    if (leaveCategoryList == null)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                    var employeeList = (from e in _context.Employees
                                        where e.Active && e.JoiningDate.HasValue && (e.JobStatusId == (int)EmploymentStatusEnum.Permanent)
                                        select new
                                        {
                                            Id = e.Id,
                                            EmployeeId = e.EmployeeId,
                                            Gender = e.GenderId.HasValue ? (GenderTypeEnum)e.GenderId.Value : GenderTypeEnum.Others,
                                            PermanentDate = e.PermanentDate.HasValue ? e.PermanentDate : today,
                                            MaritalStatus = e.MaritalTypeId.HasValue ? (MaritalStatusEnum)e.MaritalTypeId.Value : MaritalStatusEnum.Single
                                        }).ToList();

                    foreach (var item in leaveCategoryList)
                    {
                        var totalLeave = 0;
                        var assignEmployeeList = (from x in _context.ProcessLeaves
                                                  where x.IsActive.HasValue && x.IsActive.Value && x.LeaveCategoryId == item.LeaveCategoryId
                                                  && x.Year == year
                                                  select x.EmployeeId).ToList();
                        //var nearestPermanentEmployee = (from x in _context.ProcessLeaves
                        //                                where x.IsActive.HasValue && x.IsActive.Value && x.LeaveCategoryId == item.LeaveCategoryId
                        //                                && x.Year == year
                        //                                select x.EmployeeId).ToList();
                        var notAssignEmployee = employeeList.Where(x => !assignEmployeeList.Any(y => y == x.Id)).ToList();
                        if (notAssignEmployee == null || notAssignEmployee.Count() == 0)
                        {
                            continue;
                        }
                        var lastDayOfYear = new DateTime(year, 6, 30);

                        foreach (var i in notAssignEmployee)
                        {
                            //continue;
                            //maternity leave for only married female
                            if (item.LeaveCategoryId == (int)LeaveTypeEnum.MaternityLeave && (i.Gender == GenderTypeEnum.Male || i.MaritalStatus == MaritalStatusEnum.Single))
                            {
                                continue;
                            }
                            var remainingDays = (lastDayOfYear - i.PermanentDate.Value).TotalDays + 1;
                            double leavePerDay = item.MaxDays.Value / 365.0;
                            int leaveDay = (int)Math.Floor(remainingDays * leavePerDay);
                            if (leaveDay > item.MaxDays)
                            {
                                leaveDay = item.MaxDays.Value;
                            }
                            var processLeave = new ProcessLeave()
                            {
                                Employee = i.Id,
                                EmployeeId = i.Id,
                                LeaveCategory = item.Name,
                                LeaveCategoryId = item.LeaveCategoryId,
                                IsActive = true,
                                MaxDays = leaveDay,
                                TotalLeave = leaveDay,
                                CreatedBy = "System",
                                CreatedDate = DateTime.Now,
                                LeaveBalance = leaveDay,
                                LeaveAvailed = 0,
                                LeaveYear = year.ToString(),
                                Year = year
                            };
                            _context.ProcessLeaves.Add(processLeave);
                            status += _context.SaveChanges();
                        }
                    }

                    //sick leave entry in first date of year
                    // sick leave now available after permanent in same method
                    // method off now
                    if (today.Month == 7 && today.Day == 1 && false)
                    {
                        var sickLeave = _context.LeaveCategories.FirstOrDefault(x => x.IsActive && x.LeaveCategoryId == (int)LeaveTypeEnum.SickLeave);
                        if (sickLeave == null)
                        {
                            return 0;
                        }
                        var allEmployeeList = _context.Employees.Where(x => x.Active).Select(x => x.Id).ToList();
                        var assignSickLeaveEmployeeList = _context.ProcessLeaves.Where(x => x.LeaveCategoryId == (int)LeaveTypeEnum.SickLeave).Where(x => x.Year == year)
                            .Select(x => x.EmployeeId).ToList();
                        //var unassignSickLeaveEmployeeList = allEmployeeList.Where(x=> !assignSickLeaveEmployeeList.Contains(x)).ToList();
                        var unassignSickLeaveEmployeeList = allEmployeeList.Where(x => !assignSickLeaveEmployeeList.Any(y => y == x)).ToList();
                        foreach (var item in unassignSickLeaveEmployeeList)
                        {
                            var processLeave = new ProcessLeave()
                            {
                                Employee = item,
                                EmployeeId = item,
                                LeaveCategory = sickLeave.Name,
                                LeaveCategoryId = sickLeave.LeaveCategoryId,
                                IsActive = true,
                                MaxDays = sickLeave.MaxDays.Value,
                                TotalLeave = sickLeave.MaxDays.Value,
                                CreatedBy = "System",
                                CreatedDate = DateTime.Now,
                                LeaveBalance = sickLeave.MaxDays.Value,
                                LeaveAvailed = 0,
                                LeaveYear = year.ToString(),
                                Year = year
                            };
                            _context.ProcessLeaves.Add(processLeave);
                            status += _context.SaveChanges();
                        }
                    }

                    if (today.Month == 6 && today.Day > 14 && (today.Day == 15 || today.Day % 5 == 0))
                    {
                        foreach (var item in leaveCategoryList)
                        {
                            var assignEmployeeList = (from x in _context.ProcessLeaves
                                                      where x.IsActive.HasValue && x.IsActive.Value && x.LeaveCategoryId == item.LeaveCategoryId
                                                      && x.Year == nextYear
                                                      select x.EmployeeId).ToList();

                            var notAssignEmployee = employeeList.Where(x => !assignEmployeeList.Any(y => y == x.Id)).ToList();
                            int leaveDay = item.MaxDays.Value;
                            foreach (var i in notAssignEmployee)
                            {
                                //maternity leave for only married female
                                if (item.LeaveCategoryId == (int)LeaveTypeEnum.MaternityLeave && (i.Gender == GenderTypeEnum.Male || i.MaritalStatus == MaritalStatusEnum.Single))
                                {
                                    continue;
                                }
                                var processLeave = new ProcessLeave()
                                {
                                    Employee = i.Id,
                                    EmployeeId = i.Id,
                                    LeaveCategory = item.Name,
                                    LeaveCategoryId = item.LeaveCategoryId,
                                    IsActive = true,
                                    MaxDays = leaveDay,
                                    TotalLeave = leaveDay,
                                    CreatedBy = "System",
                                    CreatedDate = DateTime.Now,
                                    LeaveBalance = leaveDay,
                                    LeaveAvailed = 0,
                                    LeaveYear = nextYear.ToString(),
                                    Year = nextYear
                                };
                                _context.ProcessLeaves.Add(processLeave);
                                status += _context.SaveChanges();
                            }
                        }
                    }
                    if (status > 0)
                    {
                        transaction.Commit();
                        return status;
                    }
                    transaction.Rollback();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return 0;
                }
            }
            return 0;
        }


        public static bool InsertSickLeave(long employeeId, DateTime joiningDate, bool isNew)
        {
            if (joiningDate == null || employeeId <= 0)
            {
                return false;
            }
            int year = joiningDate.Year;
            if (joiningDate.Month > 6)
            {
                year += 1;
            }
            var sickLeave = _context.LeaveCategories.FirstOrDefault(x => x.IsActive && x.LeaveCategoryId == (int)LeaveTypeEnum.SickLeave);
            if (sickLeave == null)
            {
                return false;
            }
            if (!isNew)
            {
                var employee = _context.Employees.FirstOrDefault(x => x.Id == employeeId && x.Active);
                if (employee == null)
                {
                    return false;
                }
            }
            var assignSickLeaveEmployeeList = _context.ProcessLeaves.FirstOrDefault(x => x.EmployeeId == employeeId && x.Year == year && x.LeaveCategoryId == (int)LeaveTypeEnum.SickLeave);
            if (assignSickLeaveEmployeeList != null)
            {
                return false;
            }
            var processLeave = new ProcessLeave()
            {
                Employee = employeeId,
                EmployeeId = employeeId,
                LeaveCategory = sickLeave.Name,
                LeaveCategoryId = sickLeave.LeaveCategoryId,
                IsActive = true,
                MaxDays = sickLeave.MaxDays.Value,
                TotalLeave = sickLeave.MaxDays.Value,
                CreatedBy = "System",
                CreatedDate = DateTime.Now,
                LeaveBalance = sickLeave.MaxDays.Value,
                LeaveAvailed = 0,
                LeaveYear = year.ToString(),
                Year = year
            };
            _context.ProcessLeaves.Add(processLeave);
            if (_context.SaveChanges() > 0)
            {
                return true;
            }

            return false;
        }
    }
}
