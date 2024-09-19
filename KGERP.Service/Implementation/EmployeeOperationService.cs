using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class EmployeeOperationService : IEmployeeOperationService
    {
        private bool disposed = false;

        ERPEntities context = new ERPEntities();
        public List<EmployeeOperationModel> GetEmployeeOperations(string searchText)
        {
            IQueryable<EmployeeOperation> EmployeeOperations = null;
            EmployeeOperations = context.EmployeeOperations.Where(x => x.EmployeeOperationType.Contains(searchText) || x.EmployeeId.Contains(searchText) || x.Name.Contains(searchText)).OrderBy(x => x.OperationId);
            return ObjectConverter<EmployeeOperation, EmployeeOperationModel>.ConvertList(EmployeeOperations.ToList()).ToList();
        }

        public EmployeeOperationModel GetEmployeeOperation(int id)
        {
            if (id == 0)
            {
                return new EmployeeOperationModel() { OperationId = id };
            }
            EmployeeOperation EmployeeOperation = context.EmployeeOperations.Find(id);
            return ObjectConverter<EmployeeOperation, EmployeeOperationModel>.Convert(EmployeeOperation);
        }

        public bool SaveEmployeeOperation(int id, EmployeeOperationModel model)
        {
            EmployeeOperation EmployeeOperation = ObjectConverter<EmployeeOperationModel, EmployeeOperation>.Convert(model);
            bool result = false;
            bool updateresult = false;

            string _sDate = "";
            string _eDate = "";
            if (id > 0)
            {
                EmployeeOperation = context.EmployeeOperations.FirstOrDefault(x => x.OperationId == id);
                _sDate = EmployeeOperation.FromDate.ToString();
                _eDate = EmployeeOperation.EndDate.ToString();
                if (EmployeeOperation != null)
                {
                    EmployeeOperation.ModifiedDate = DateTime.Now;
                    EmployeeOperation.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    EmployeeOperation.EmployeeId = model.EmployeeId;
                    EmployeeOperation.ActionDate = model.ActionDate;
                    EmployeeOperation.EmployeeOperationType = model.EmployeeOperationType;
                    EmployeeOperation.Remarks = model.Remarks;
                    EmployeeOperation.Name = model.Name;
                    EmployeeOperation.Reason = model.Reason;
                    EmployeeOperation.FromDate = model.FromDate;
                    EmployeeOperation.EndDate = model.EndDate;
                    updateresult = true;
                }
            }
            else
            {
                EmployeeOperation.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                EmployeeOperation.CreatedDate = DateTime.Now;
                EmployeeOperation.ModifiedDate = DateTime.Now;
                EmployeeOperation.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                EmployeeOperation.EmployeeId = model.EmployeeId;
                EmployeeOperation.ActionDate = model.ActionDate;
                EmployeeOperation.EmployeeOperationType = model.EmployeeOperationType;
                EmployeeOperation.Remarks = model.Remarks;
                EmployeeOperation.Reason = model.Reason;
                EmployeeOperation.Name = model.Name;
                EmployeeOperation.FromDate = model.FromDate;
                EmployeeOperation.EndDate = model.EndDate;
                updateresult = true;
            }
            context.Entry(EmployeeOperation).State = EmployeeOperation.OperationId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                if (EmployeeOperation.EmployeeOperationType == "Special Leave Without Pay")
                {
                    if (updateresult)
                    {
                        UpdateLeaveApplicationWioutPay(EmployeeOperation.EmployeeId, EmployeeOperation.OperationId, Convert.ToDateTime(EmployeeOperation.FromDate), Convert.ToDateTime(EmployeeOperation.EndDate), EmployeeOperation.Remarks, EmployeeOperation.Reason, _sDate, _eDate);
                    }
                    else
                    {
                        UpdateEmployeeInformation(EmployeeOperation.EmployeeId, Convert.ToDateTime(EmployeeOperation.ActionDate), Convert.ToDateTime(EmployeeOperation.FromDate), Convert.ToDateTime(EmployeeOperation.EndDate), EmployeeOperation.Remarks, EmployeeOperation.Reason, EmployeeOperation.EmployeeOperationType);
                    }
                }
                else
                {
                    if (EmployeeOperation.OperationId > 0)
                    {
                        var leaveData = context.LeaveApplications.Where(x => x.OperationId == EmployeeOperation.OperationId).FirstOrDefault();
                        if (leaveData != null)
                        {

                        }
                    }
                    UpdateEmployeeInformation(EmployeeOperation.EmployeeId, Convert.ToDateTime(EmployeeOperation.ActionDate), Convert.ToDateTime(EmployeeOperation.FromDate), Convert.ToDateTime(EmployeeOperation.EndDate), EmployeeOperation.Remarks, EmployeeOperation.Reason, EmployeeOperation.EmployeeOperationType);
                    UpdateUserInformation(EmployeeOperation.EmployeeId);
                }
                return result = true;
            }
            else
            {
                return result;
            }
        }

        private void UpdateLeaveApplicationWioutPay(string employeeId, int OperationId, DateTime sDate, DateTime eDate, string remarks, string reason, string _sDate, string _eDate)
        {
            double NrOfDays = 0;
            if (sDate != null && eDate != null)
            {
                DateTime d1 = eDate;
                DateTime d2 = sDate;
                TimeSpan t = d1 - d2;
                NrOfDays = t.TotalDays;
            }
            long empId = 0;
            long managerId = 0;
            Employee _Employee = context.Employees.Where(x => x.EmployeeId.Contains(employeeId)).FirstOrDefault();//From Employee Page
            if (_Employee != null)
            {
                empId = _Employee.Id;
                managerId = Convert.ToInt64(_Employee.ManagerId);

            }
            LeaveApplication _LeaveApplication = new LeaveApplication();
            LeaveApplication leaveApplication = new LeaveApplication();
            _LeaveApplication = context.LeaveApplications.FirstOrDefault(x => x.OperationId == OperationId);


            if (_LeaveApplication != null)
            {
                _LeaveApplication.Id = empId;
                _LeaveApplication.LeaveCategoryId = 11;
                _LeaveApplication.HrAdminId = 103;
                _LeaveApplication.LeaveDays = Convert.ToInt32(NrOfDays);
                _LeaveApplication.ManagerId = managerId;
                _LeaveApplication.ManagerApprovalDate = DateTime.Now;
                _LeaveApplication.HRApprovalDate = DateTime.Now;
                _LeaveApplication.ApplicationDate = DateTime.Now;
                _LeaveApplication.ManagerStatus = "Approved";
                _LeaveApplication.HrAdminStatus = "Approved";
                _LeaveApplication.StartDate = sDate;
                _LeaveApplication.EndDate = eDate;
                _LeaveApplication.Reason = reason;
                _LeaveApplication.Remarks = remarks;
                _LeaveApplication.OperationId = OperationId;
                context.SaveChanges();
            }
            else
            {
                leaveApplication.Id = empId;
                leaveApplication.LeaveCategoryId = 11;
                leaveApplication.HrAdminId = 103;
                leaveApplication.LeaveDays = Convert.ToInt32(NrOfDays);
                leaveApplication.ManagerId = managerId;
                leaveApplication.ManagerApprovalDate = DateTime.Now;
                leaveApplication.HRApprovalDate = DateTime.Now;
                leaveApplication.ApplicationDate = DateTime.Now;
                leaveApplication.ManagerStatus = "Approved";
                leaveApplication.HrAdminStatus = "Approved";
                leaveApplication.StartDate = sDate;
                leaveApplication.EndDate = eDate;
                leaveApplication.Reason = reason;
                leaveApplication.Remarks = remarks;
                leaveApplication.OperationId = OperationId;
                context.LeaveApplications.Add(leaveApplication);
                context.SaveChanges();
            }
        }

        private void UpdateEmployeeInformation(string employeeId, DateTime aDate, DateTime sDate, DateTime eDate, string remarks, string reason, string actionType)
        {
            long empId = 0;
            Employee _Employee = context.Employees.Where(x => x.EmployeeId.Contains(employeeId)).FirstOrDefault();//From Employee Page
            if (_Employee != null)
            {
                empId = _Employee.Id;
                if (actionType != "Special Leave Without Pay")
                {
                    _Employee.Active = false;
                }
                try
                {
                    _Employee.EndReason = reason;
                }
                catch
                {
                }
                try
                {
                    _Employee.Remarks = remarks;
                }
                catch
                {
                }

                try
                {
                    _Employee.EndDate = aDate;
                }
                catch
                {
                }
                context.SaveChanges();
            }
        }

        private void UpdateUserInformation(string employeeId)
        {
            User _Employee = context.Users.Where(x => x.UserName.Contains(employeeId)).FirstOrDefault();//From Employee Page
            if (_Employee != null)
            {
                _Employee.Active = false;
                context.SaveChanges();
            }
        }
        public bool DeleteEmployeeOperation(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public List<SelectModel> GetEmployeeOperationEmployees()
        {
            return context.Employees.Where(x => x.Active == true && x.DepartmentId == 6).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.EmployeeId.ToString()
            }).ToList();
        }

        public List<EmployeeOperationModel> GetEmployeeOperationEvent()
        {
            dynamic result = context.Database.SqlQuery<EmployeeOperationModel>("exec sp_GetUpcomingCaseEvent").ToList();
            return result;
            //DateTime dtFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //DateTime dtTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, DateTime.Now.Day);
            //IQueryable<EmployeeOperation> EmployeeOperations = context.EmployeeOperations.Where(x => x.NextDate >= dtFrom && x.NextDate <= dtTo).OrderBy(x => x.NextDate);
            ////IQueryable<EmployeeOperation> EmployeeOperations = context.EmployeeOperations.OrderByDescending(x => x.NextDate);
            //return ObjectConverter<EmployeeOperation, EmployeeOperationModel>.ConvertList(EmployeeOperations.ToList()).ToList();
        }

        public List<EmployeeOperationModel> GetPrevious7DaysOperationSchedule()
        {
            dynamic result = context.Database.SqlQuery<EmployeeOperationModel>("exec sp_LnL_OneWeekPreviousCaseSchedule").ToList();
            return result;
        }

        public List<EmployeeOperationModel> GetKGCaseList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<EmployeeOperationModel>("exec sp_6GetKGCaseList {0} ", searchText).ToList();
            return result;
        }
    }
}
