using AutoMapper;
using Azure.Core;
using Humanizer;
using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.Implementation.Leave.ViewModels;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Service.Utility;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KGERP.Service.Implementation
{
    public class LeaveApplicationService : ILeaveApplicationService
    {
        private readonly ERPEntities context;
        public LeaveApplicationService(ERPEntities context)
        {
            this.context = context;
        }

        private void UpdateOrCreateSignatory(long signatoryId, int orderBy, int employeeId, int ComapnyId)
        {
            var signatory = context.RequisitionSignatories
                                   .FirstOrDefault(x => x.EmployeeId == employeeId && x.SignatoryEmpId == signatoryId && x.IsActive && x.IntegrateWith == "LeaveApplication");


            var signatoryinformation = (from t1 in context.Employees
                                        join t2 in context.Designations on t1.DesignationId equals t2.DesignationId
                                        where t1.Id == signatoryId
                                        select new EmployeeVm
                                        {
                                            EmployeeId = t1.EmployeeId,
                                            DesignationName = t2.Name,
                                        }).FirstOrDefault();
            var ishrr = context.Employees.Where(x => x.Id == employeeId).FirstOrDefault();
            var iSHr = false;
            if (ishrr.HrAdminId == signatoryId)
            {
                iSHr = true;
            }




            if (signatory != null)
            {
                signatory.OrderBy = orderBy;
                signatory.ModifiedBy = Common.GetUserId();
                signatory.ModifiedDate = DateTime.Now;
            }
            else
            {
                var newSignatory = new RequisitionSignatory
                {
                    EmployeeId = employeeId,
                    SignatoryEmpId = signatoryId,
                    OrderBy = orderBy,
                    CreatedBy = Common.GetUserId(),
                    CreatedDate = DateTime.Now,
                    IsHRAdmin = iSHr,
                    IntegrateWith = "LeaveApplication",
                    IsActive = true,
                    DesignationName = signatoryinformation.DesignationName,
                    Limitation = 0,
                    CompanyId = ComapnyId,



                };

                context.RequisitionSignatories.Add(newSignatory);
            }

            context.SaveChanges();
            //HR order by will increase 1 by default - Syed Ruman
            UpdateHrOrderBy(employeeId);
        }
        public void UpdateHrOrderBy(int employeeId)
        {
            var maxOrderBy = context.RequisitionSignatories.Where(x => x.EmployeeId == employeeId && x.IsActive == true && x.IntegrateWith == "LeaveApplication" && x.IsHRAdmin == false).Select(x => x.OrderBy).Max();
            var hr = context.RequisitionSignatories.Where(x => x.EmployeeId == employeeId && x.IsHRAdmin == true && x.IntegrateWith == "LeaveApplication" && x.IsActive == true).FirstOrDefault();
            hr.OrderBy = maxOrderBy + 1;
            context.SaveChanges();
        }


        public bool RemoveSignatory(long signatoryId)
        {
            bool isSingleSignatory = true;
            var isLastSignatory = false;
            var obj = context.RequisitionSignatories.Where(x => x.RequisitionSignatoryId == signatoryId && x.IntegrateWith == "LeaveApplication").FirstOrDefault();
            var maxOrderBy = context.RequisitionSignatories.Where(x => x.EmployeeId == obj.EmployeeId && x.IsActive == true && x.IntegrateWith == "LeaveApplication" && x.IsHRAdmin == false).Select(x => x.OrderBy).Max();
            var sameLevelSignatories = context.RequisitionSignatories.Where(x => x.EmployeeId == obj.EmployeeId && x.OrderBy == obj.OrderBy && x.IntegrateWith == "LeaveApplication" && x.IsActive == true).ToList();
            var maxLevelSig = context.RequisitionSignatories.Where(x => x.EmployeeId == obj.EmployeeId && x.OrderBy == obj.OrderBy && obj.OrderBy == maxOrderBy && x.IsActive == true && x.IntegrateWith == "LeaveApplication").ToList();
            if(sameLevelSignatories.Count > 1)
            {
                isSingleSignatory = false;
            }
            if (maxLevelSig.Count == 1)
            {
                isLastSignatory = true;
            }
            if ((obj != null && !isSingleSignatory) || (isSingleSignatory && isLastSignatory))
            {
                obj.IsActive = false;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();
                UpdateHrOrderBy((int)obj.EmployeeId);
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool LeaveSignatoryAssign(LeaveApplicationVmm viewModel)
        {
            bool success = false;
            UpdateOrCreateSignatory((long)viewModel.ManagerId, viewModel.managerOrder, (int)viewModel.EmpId, viewModel.CompanyId);
            UpdateOrCreateSignatory((long)viewModel.HrAdminId, viewModel.hrOrder, (int)viewModel.EmpId, viewModel.CompanyId);
            if (viewModel.othersData != null)
            {
                foreach (var item in viewModel.othersData)
                {
                    UpdateOrCreateSignatory(item.SignatoryEmpId, item.OrderBy, (int)viewModel.EmpId, viewModel.CompanyId);
                }
            }

            return success;
        }


        public LeaveApplicationVm ManagerAndHrLook(long EmpId)
        {
            LeaveApplicationVm model = new LeaveApplicationVm();


            var obj = context.Employees.Where(x => x.Id == EmpId).FirstOrDefault();
            if (obj != null)
            {
                model.ManagerId = obj.ManagerId;
                model.HrAdminId = obj.HrAdminId;
                var managername = context.Employees.Where(x => x.Id == model.ManagerId).FirstOrDefault();
                var hrnamre = context.Employees.Where(x => x.Id == model.HrAdminId).FirstOrDefault();
                model.ManagerName = managername.Name;
                model.Hrname = hrnamre.Name;
            }

            var objSignatory = context.RequisitionSignatories.Where(x => x.EmployeeId == EmpId && x.SignatoryEmpId == model.ManagerId && x.IntegrateWith == "LeaveApplication").FirstOrDefault();
            if (objSignatory != null)
            {
                model.managerOrder = objSignatory.OrderBy;
            }
            var objSignatory2 = context.RequisitionSignatories.Where(x => x.EmployeeId == EmpId && x.SignatoryEmpId == model.HrAdminId && x.IntegrateWith == "LeaveApplication").FirstOrDefault();
            if (objSignatory != null)
            {
                model.hrOrder = objSignatory2.OrderBy;
            }

            var obj3 = (from t1 in context.RequisitionSignatories
                        join t2 in context.Employees on t1.EmployeeId equals t2.Id
                        join t3 in context.Designations on t2.DesignationId equals t3.DesignationId
                        join t4 in context.Employees on t1.SignatoryEmpId equals t4.Id
                        join t5 in context.Designations on t4.DesignationId equals t5.DesignationId
                        where t1.EmployeeId == EmpId && t1.IsActive && t1.IntegrateWith == "LeaveApplication"
                        select new RequisitionSignatoryVM
                        {
                            EmployeeId = t1.EmployeeId,
                            RequisitionSignatoryId = t1.RequisitionSignatoryId,
                            DesignationName = t3.Name,
                            SignatoryEmpId = t1.SignatoryEmpId,
                            SignatoryEmpDesignation = t5.Name,
                            OrderBy = t1.OrderBy,
                            SignatoryName = t4.Name

                        }).OrderByDescending(x => x.OrderBy).ToList();



            if (obj3 != null)
            {
                model.DataLIstSignatory = obj3.ToList();
            }

            return model;

        }






        public async Task<LeaveApplicationVm> GetLeaveApplicationByEmployee(DateTime? fromDate, DateTime? toDate)
        {
            long id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"].ToString());

            LeaveApplicationVm model = new LeaveApplicationVm();

            model.DataList = await Task.Run(() => (from t1 in context.LeaveApplications
                                                   join t2 in context.Employees on t1.Id equals t2.Id
                                                   join t3 in context.LeaveCategories on t1.LeaveCategoryId equals t3.LeaveCategoryId
                                                   where t1.Id == id
                                                   && (fromDate == null || t1.StartDate >= fromDate)
                                                   && (toDate == null || t1.EndDate <= toDate)
                                                   select new LeaveApplicationVm
                                                   {
                                                       LeaveApplicationId = t1.LeaveApplicationId,
                                                       EmployeeId = t2.EmployeeId,
                                                       CategoryName = t3.Name,
                                                       EmployeeName = "[" + t2.EmployeeId + "] " + t2.Name,
                                                       Reason = t1.Reason,
                                                       ApplicationDate = t1.ApplicationDate,
                                                       StartDate = t1.StartDate,
                                                       EndDate = t1.EndDate,
                                                       LeaveDays = t1.LeaveDays,
                                                       HrAdminId = t1.HrAdminId,
                                                       ManagerId = t1.ManagerId,
                                                       HrAdminStatus = t1.HrAdminStatus,
                                                       ManagerStatus = t1.ManagerStatus
                                                   }
                                                 ).OrderByDescending(o => o.LeaveApplicationId)
                                                 .AsEnumerable());
            return model;
        }

        public LeaveApplicationModel GetLeaveApplication(long id)
        {
            if (id <= 0)
            {
                return new LeaveApplicationModel()
                {
                    ManagerInfo = string.Format("[{0}] {1}", System.Web.HttpContext.Current.Session["ManagerEmployeeId"].ToString(), System.Web.HttpContext.Current.Session["ManagerName"].ToString())
                };
            }
            return ObjectConverter<LeaveApplication, LeaveApplicationModel>.Convert(context.LeaveApplications.Include(x => x.LeaveCategory).FirstOrDefault(x => x.LeaveApplicationId == id));
        }

        public LeaveApplicationModel GetLeaveApplicationByOther(long id, long empId)
        {
            if (id <= 0)
            {
                Employee employee = context.Employees.Include(m => m.Employee3).Include(x => x.Department).Include(x => x.Designation).Where(x => x.Id == empId).FirstOrDefault();
                return new LeaveApplicationModel()
                {
                    Id = employee.Id,
                    KGID = employee.EmployeeId,

                    EmployeeName = employee.Name,
                    DepartmentName = employee.Department.Name,
                    DesignationName = employee.Designation.Name,
                    ManagerInfo = string.Format("[{0}] {1}", employee.Employee3.EmployeeId, employee.Employee3.Name),
                };
            }
            return ObjectConverter<LeaveApplication, LeaveApplicationModel>.Convert(context.LeaveApplications.Include(x => x.LeaveCategory).FirstOrDefault(x => x.LeaveApplicationId == id));
        }
        public bool SaveLeaveApplication(long leaveApplicationId, LeaveApplicationModel model, out string message)
        {

            if (true)
            {
                bool checkValidity = true;
                var hrAdmintId = Common.GetHRAdminId();
                var managerId = Common.GetManagerId();
                var userId = Common.GetIntUserId();
                var employee = context.Employees.FirstOrDefault(x => x.Active && x.Id == model.Id);
                if (employee == null)
                {
                    message = "No employee found";
                    return false;
                }
                if (employee.JobStatusId == 41)
                {
                    checkValidity = false;
                }
                var employeeId = employee.Id;
                managerId = (long)employee.ManagerId;

                if (model.EndDate < model.StartDate || model.StartDate == null || model.EndDate == null)
                {
                    message = "Start date must be smallest from to date";
                    return false;
                }
                //exist leave check
                bool exist = context.LeaveApplications.Where(x => x.Id == employeeId && x.Status == (int)StatusEnum.Active && x.LeaveStatus != (int)LeaveStatusEnum.Denied && (x.StartDate <= model.StartDate && x.EndDate >= model.EndDate)).Any();
                if (exist)
                {
                    message = "You have already used this date range !";
                    return false;
                }

                //check male or female for maturnity leave and marriage leave
                //(c.LeaveCategoryId != 6 || (c.LeaveCategoryId == 6 && e.GenderId == 2 && e.MaritalTypeId == 3)
                // var processLeave = context.ProcessLeaves.FirstOrDefault(x => x.Year == model.StartDate.Value.Year && x.EmployeeId == userId && x.LeaveCategoryId == model.LeaveCategoryId);
                int fromYear = model.StartDate.Value.Year;
                int toYear = model.EndDate.Value.Year;
                if (model.StartDate.Value.Month > 6)
                {
                    fromYear += 1;
                }
                var processLeave = (from x in context.ProcessLeaves
                                    join e in context.Employees on x.EmployeeId equals e.Id
                                    where x.Year == fromYear && x.EmployeeId == employeeId && x.IsActive.Value
                                    && x.LeaveCategoryId == model.LeaveCategoryId
                                    && ((model.LeaveCategoryId != 6 || (model.LeaveCategoryId == 6 && e.GenderId == 2 && e.MaritalTypeId == 3)))
                                    select x).FirstOrDefault();

                if (processLeave == null && checkValidity)
                {
                    message = "Do not have sufficient balance";
                    return false;
                }
                //check leave coverage days in 2 fiscal year
                bool isJointYearLeave = false;
                if (model.StartDate.Value.Month < 7 && model.EndDate.Value.Month > 6)
                {
                    isJointYearLeave = true;
                    toYear += 1;
                }
                if (isJointYearLeave)
                {
                    var nextYearLeaveDays = model.EndDate.Value.Day;
                    int daysInMonth = DateTime.DaysInMonth(model.StartDate.Value.Year, model.StartDate.Value.Month);
                    var previousYearLeaveDays = (daysInMonth - model.StartDate.Value.Day) + 1;
                    if (checkValidity && previousYearLeaveDays > processLeave.LeaveBalance)
                    {
                        message = "Do not have sufficient balance";
                        return false;
                    }
                    var nextYearProcessLeave = context.ProcessLeaves.FirstOrDefault(x => x.Year == toYear && x.EmployeeId == employeeId && x.LeaveCategoryId == model.LeaveCategoryId);
                    if (nextYearProcessLeave == null || nextYearProcessLeave.LeaveBalance < nextYearLeaveDays)
                    {
                        message = $"Do not have sufficient balance for the year of {toYear}";
                        return false;
                    }
                }
                else
                {
                    if (checkValidity && model.LeaveDays > processLeave.LeaveBalance)
                    {
                        message = "Do not have sufficient balance";
                        return false;
                    }
                }
                LeaveApplication leaveApplication = new LeaveApplication();
                leaveApplication.Id = employeeId;
                leaveApplication.ManagerId = managerId;
                leaveApplication.ManagerStatus = LeaveStatusEnum.Pending.ToString();
                leaveApplication.HrAdminId = hrAdmintId;
                leaveApplication.HrAdminStatus = null;
                leaveApplication.LeaveCategoryId = model.LeaveCategoryId;
                leaveApplication.StartDate = model.StartDate ?? DateTime.Now;
                leaveApplication.EndDate = model.EndDate ?? DateTime.Now;
                leaveApplication.LeaveDays = model.LeaveDays;
                leaveApplication.Address = model.Address;
                leaveApplication.ContactName = model.ContactName;
                leaveApplication.Reason = model.Reason;
                leaveApplication.Remarks = model.Remarks;
                leaveApplication.ApplicationDate = DateTime.Now;
                leaveApplication.IP = model.IP;
                leaveApplication.Active = 1;
                leaveApplication.Status = (int)StatusEnum.Active;
                leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Pending;
                context.LeaveApplications.Add(leaveApplication);
                var result = context.SaveChanges();
                if (result > 0)
                {
                    message = "Your leave request has been successfully submitted.";
                    model.LeaveApplicationId = leaveApplication.LeaveApplicationId;
                    return true;
                }
            }
            //long id = Common.GetIntUserId();
            //message = string.Empty;
            //string body = string.Empty;
            //string subject = string.Empty;
            //bool isMailSentToEmployee = false;
            //bool isMailSentToLineManager = false;
            //if (model == null)
            //{
            //    throw new Exception("Leave Application data missing!");
            //}
            //LeaveApplication leaveApplication = ObjectConverter<LeaveApplicationModel, LeaveApplication>.Convert(model);
            //leaveApplication.Id = id;

            //bool exist = context.LeaveApplications.Where(x => x.Id == id && x.ManagerStatus.Equals("Approved") && (x.StartDate <= leaveApplication.StartDate && x.EndDate >= leaveApplication.EndDate)).Any();
            //if (exist)
            //{
            //    message = "You have already used this date range !";
            //    return false;
            //}

            //LeaveCategory leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.LeaveCategoryId == leaveApplication.LeaveCategoryId);


            //if (leaveApplicationId > 0)
            //{
            //    leaveApplication = context.LeaveApplications.Include(x => x.LeaveCategory).FirstOrDefault(x => x.LeaveApplicationId == leaveApplicationId);
            //    if (leaveApplication == null)
            //    {
            //        throw new Exception("Leave Application not found!");
            //    }
            //    leaveApplication.ManagerStatus = model.ManagerStatus;
            //}
            //else
            //{
            //    leaveApplication.ManagerStatus = "Pending";
            //}
            //int leaveYear = DateTime.Now.Month > 6 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            //ProcessLeave processLeave = context.ProcessLeaves.ToList().FirstOrDefault(x => x.Employee == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId && x.LeaveYear == leaveYear.ToString());
            //if (processLeave != null)
            //{
            //    if (processLeave.MaxDays < (leaveApplication.LeaveDays + processLeave.LeaveAvailed))
            //    {
            //        message = "Sorry! Yor have already consumed this leave";
            //        return false;
            //    }
            //}

            //else
            //{
            //    leaveCategory = context.LeaveCategories.ToList().FirstOrDefault(x => x.LeaveCategoryId == model.LeaveCategoryId);
            //    if (leaveCategory != null)
            //    {
            //        if (leaveCategory.MaxDays < model.LeaveDays)
            //        {
            //            message = "Sorry! Yor are not eligible to consume this leave";
            //            return false;
            //        }
            //    }

            //}
            //leaveApplication.Id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]);
            //leaveApplication.ManagerId = Convert.ToInt64(System.Web.HttpContext.Current.Session["ManagerId"]);
            //leaveApplication.HrAdminId = Convert.ToInt64(System.Web.HttpContext.Current.Session["HrAdminId"]);
            //leaveApplication.HrAdminStatus = model.HrAdminStatus;
            //leaveApplication.LeaveCategoryId = model.LeaveCategoryId;
            //leaveApplication.StartDate = model.StartDate ?? DateTime.Now;
            //leaveApplication.EndDate = model.EndDate ?? DateTime.Now;
            //leaveApplication.LeaveDays = model.LeaveDays;
            //leaveApplication.Address = model.Address;
            //leaveApplication.ContactName = model.ContactName;
            //leaveApplication.Reason = model.Reason;
            //leaveApplication.Remarks = model.Remarks;
            //leaveApplication.ApplicationDate = DateTime.Now;
            //leaveApplication.IP = model.IP;

            //for (DateTime date = leaveApplication.StartDate.Date; date <= leaveApplication.EndDate.Date; date += TimeSpan.FromDays(1))
            //{
            //    LeaveApplicationDetail leaveApplicationDetail = new LeaveApplicationDetail();
            //    leaveApplicationDetail.LeaveDate = date;
            //    leaveApplicationDetail.LeaveYear = date.Year.ToString();
            //    leaveApplication.LeaveApplicationDetails.Add(leaveApplicationDetail);
            //}
            //context.Entry(leaveApplication).State = leaveApplication.LeaveApplicationId == 0 ? EntityState.Added : EntityState.Modified;

            //bool result = context.SaveChanges() > 0;

            //Employee employee = context.Employees.Include(x => x.Employee3).Include(x => x.Employee2).FirstOrDefault(x => x.Id == leaveApplication.Id);

            //if (employee == null)
            //{
            //    throw new Exception();
            //}

            //// body = "Employee ID : " + employee.EmployeeId + "<br/>Name : " + employee.Name + "<br/> Leave Category : " + leaveCategory.Name + "<br/>Applied Date : " + leaveApplication.ApplicationDate.Value.ToString("dd MMM yyyy") + "<br/> Leave Date : From " + leaveApplication.StartDate.Value.ToString("dd MMM yyyy") + " to " + leaveApplication.EndDate.Value.ToString("dd MMM yyyy") + "<br/> Leave Days : " + leaveApplication.LeaveDays + "<br/> Manager Status : " + leaveApplication.ManagerStatus + "<br/>HR Status : " + leaveApplication.HrAdminStatus;
            //body = EmailBodyForLeaveApplication(employee, leaveCategory, leaveApplication);
            //subject = "Leave Application Status - [" + employee.EmployeeId + "] " + employee.Name;
            //if (result)
            //{
            //    //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
            //    //isMailSentToLineManager = MailService.SendMail(string.Empty, string.Empty, employee.Manager.Email, employee.Manager.Name, string.Empty, string.Empty, subject, body, out sendStatus);
            //}
            message = "Sorry something went wrong";
            return false;
        }
        public bool SignatoryApprovalSave(LeaveApplicationModel model)
        {
            try
            {
                long employeeId = model.Id;
                var requisitions = context.RequisitionSignatories.Where(r => r.EmployeeId == employeeId && r.IsActive == true && r.IntegrateWith == "LeaveApplication").OrderBy(x => x.OrderBy).ToList();
                int status = 0;
                foreach (var req in requisitions)
                {
                    status = (req.OrderBy != 1) ? -1 : 0;
                    var signatoryApprovalMap = new SignatoryApprovalMap
                    {
                        EmployeeId = req.SignatoryEmpId,
                        TableName = req.IntegrateWith,
                        IntregratedFromId = model.LeaveApplicationId,
                        OrderBy = req.OrderBy,
                        Status = status,
                        IsHRAdmin = req.IsHRAdmin,
                        CreatedDate = DateTime.Now,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        IsActive = true
                    };

                    context.SignatoryApprovalMaps.Add(signatoryApprovalMap);
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SignatoryAutoApprovalSave(LeaveApplicationModel model)
        {
            try
            {
                long employeeId = model.Id;
                var requisitions = context.RequisitionSignatories.Where(r => r.EmployeeId == employeeId && r.IsActive == true && r.IntegrateWith == "LeaveApplication").OrderBy(x => x.OrderBy).ToList();
                int status = 0;
                foreach (var req in requisitions)
                {
                    status = 1;
                    var signatoryApprovalMap = new SignatoryApprovalMap
                    {
                        EmployeeId = req.SignatoryEmpId,
                        TableName = req.IntegrateWith,
                        IntregratedFromId = model.LeaveApplicationId,
                        OrderBy = req.OrderBy,
                        Status = status,
                        IsHRAdmin = req.IsHRAdmin,
                        CreatedDate = DateTime.Now,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        IsActive = true
                    };

                    context.SignatoryApprovalMaps.Add(signatoryApprovalMap);
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveLeaveApplicationOld(long leaveApplicationId, LeaveApplicationModel model, out string message)
        {
            long id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"].ToString());
            message = string.Empty;
            string body = string.Empty;
            string subject = string.Empty;
            bool isMailSentToEmployee = false;
            bool isMailSentToLineManager = false;
            if (model == null)
            {
                throw new Exception("Leave Application data missing!");
            }
            LeaveApplication leaveApplication = ObjectConverter<LeaveApplicationModel, LeaveApplication>.Convert(model);
            leaveApplication.Id = id;

            bool exist = context.LeaveApplications.Where(x => x.Id == id && x.ManagerStatus.Equals("Approved") && (x.StartDate <= leaveApplication.StartDate && x.EndDate >= leaveApplication.EndDate)).Any();
            if (exist)
            {
                message = "You have already used this date range !";
                return false;
            }

            LeaveCategory leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.LeaveCategoryId == leaveApplication.LeaveCategoryId);


            if (leaveApplicationId > 0)
            {
                leaveApplication = context.LeaveApplications.Include(x => x.LeaveCategory).FirstOrDefault(x => x.LeaveApplicationId == leaveApplicationId);
                if (leaveApplication == null)
                {
                    throw new Exception("Leave Application not found!");
                }
                leaveApplication.ManagerStatus = model.ManagerStatus;
            }
            else
            {
                leaveApplication.ManagerStatus = "Pending";
            }
            int leaveYear = DateTime.Now.Month > 6 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            ProcessLeave processLeave = context.ProcessLeaves.ToList().FirstOrDefault(x => x.Employee == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId && x.LeaveYear == leaveYear.ToString());
            //if (processLeave != null)
            //{
            //    if (processLeave.MaxDays < (leaveApplication.LeaveDays + processLeave.LeaveAvailed))
            //    {
            //        message = "Sorry! Yor have already consumed this leave";
            //        return false;
            //    }
            //}

            //else
            //{
            //    leaveCategory = context.LeaveCategories.ToList().FirstOrDefault(x => x.LeaveCategoryId == model.LeaveCategoryId);
            //    if (leaveCategory != null)
            //    {
            //        if (leaveCategory.MaxDays < model.LeaveDays)
            //        {
            //            message = "Sorry! Yor are not eligible to consume this leave";
            //            return false;
            //        }
            //    }

            //}
            leaveApplication.Id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]);
            leaveApplication.ManagerId = Convert.ToInt64(System.Web.HttpContext.Current.Session["ManagerId"]);
            leaveApplication.HrAdminId = Convert.ToInt64(System.Web.HttpContext.Current.Session["HrAdminId"]);
            leaveApplication.HrAdminStatus = model.HrAdminStatus;
            leaveApplication.LeaveCategoryId = model.LeaveCategoryId;
            leaveApplication.StartDate = model.StartDate ?? DateTime.Now;
            leaveApplication.EndDate = model.EndDate ?? DateTime.Now;
            leaveApplication.LeaveDays = model.LeaveDays;
            leaveApplication.Address = model.Address;
            leaveApplication.ContactName = model.ContactName;
            leaveApplication.Reason = model.Reason;
            leaveApplication.Remarks = model.Remarks;
            leaveApplication.ApplicationDate = DateTime.Now;
            leaveApplication.IP = model.IP;

            for (DateTime date = leaveApplication.StartDate.Date; date <= leaveApplication.EndDate.Date; date += TimeSpan.FromDays(1))
            {
                LeaveApplicationDetail leaveApplicationDetail = new LeaveApplicationDetail();
                leaveApplicationDetail.LeaveDate = date;
                leaveApplicationDetail.LeaveYear = date.Year.ToString();
                leaveApplication.LeaveApplicationDetails.Add(leaveApplicationDetail);
            }
            context.Entry(leaveApplication).State = leaveApplication.LeaveApplicationId == 0 ? EntityState.Added : EntityState.Modified;

            bool result = context.SaveChanges() > 0;

            Employee employee = context.Employees.Include(x => x.Employee3).Include(x => x.Employee2).FirstOrDefault(x => x.Id == leaveApplication.Id);

            if (employee == null)
            {
                throw new Exception();
            }

            // body = "Employee ID : " + employee.EmployeeId + "<br/>Name : " + employee.Name + "<br/> Leave Category : " + leaveCategory.Name + "<br/>Applied Date : " + leaveApplication.ApplicationDate.Value.ToString("dd MMM yyyy") + "<br/> Leave Date : From " + leaveApplication.StartDate.Value.ToString("dd MMM yyyy") + " to " + leaveApplication.EndDate.Value.ToString("dd MMM yyyy") + "<br/> Leave Days : " + leaveApplication.LeaveDays + "<br/> Manager Status : " + leaveApplication.ManagerStatus + "<br/>HR Status : " + leaveApplication.HrAdminStatus;
            body = EmailBodyForLeaveApplication(employee, leaveCategory, leaveApplication);
            subject = "Leave Application Status - [" + employee.EmployeeId + "] " + employee.Name;
            if (result)
            {
                //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
                //isMailSentToLineManager = MailService.SendMail(string.Empty, string.Empty, employee.Manager.Email, employee.Manager.Name, string.Empty, string.Empty, subject, body, out sendStatus);
            }
            return result;
        }

        public List<LeaveApplicationModel> GetApprovedLeaveApplication()
        {
            if (System.Web.HttpContext.Current.Session["Id"] == System.Web.HttpContext.Current.Session["HrAdminId"])
            {
                return ObjectConverter<LeaveApplication, LeaveApplicationModel>.ConvertList(context.LeaveApplications.Include("Employee").Where(x => x.HrAdminId == Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]) && x.HrAdminStatus == "Approved").OrderByDescending(x => x.StartDate).ToList()).ToList();
            }

            return ObjectConverter<LeaveApplication, LeaveApplicationModel>.ConvertList(context.LeaveApplications.Include("Employee").Where(x => x.ManagerId == Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]) && x.ManagerStatus == "Approved").OrderByDescending(x => x.StartDate).ToList()).ToList();
        }

        public bool DeleteLeaveApplication(long id)
        {
            LeaveApplication leaveApplication = context.LeaveApplications.Where(x => x.LeaveApplicationId == id).FirstOrDefault();
            IEnumerable<LeaveApplicationDetail> leaveApplicationDetails = context.LeaveApplicationDetails.Where(x => x.LeaveApplicationId == id).ToList();

            context.LeaveApplicationDetails.RemoveRange(leaveApplicationDetails);
            if (context.SaveChanges() > 1)
            {
                context.LeaveApplications.Remove(leaveApplication);
            }
            return context.SaveChanges() > 0;
        }

        public List<LeaveApplicationModel> GetDeniedLeaveApplication()
        {
            if (System.Web.HttpContext.Current.Session["Id"] == System.Web.HttpContext.Current.Session["HrAdminId"])
            {
                return ObjectConverter<LeaveApplication, LeaveApplicationModel>.ConvertList(context.LeaveApplications.Include("Employee").Where(x => x.HrAdminId == Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]) && x.HrAdminStatus == "Denied").OrderByDescending(x => x.StartDate).ToList()).ToList();
            }

            return ObjectConverter<LeaveApplication, LeaveApplicationModel>.ConvertList(context.LeaveApplications.Include("Employee").Where(x => x.ManagerId == Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]) && x.ManagerStatus == "Denied").OrderByDescending(x => x.StartDate).ToList()).ToList();
        }

        public List<LeaveBalanceCustomModel> GetLeaveBalance()
        {
            var employeeId = Common.GetIntUserId();
            var today = DateTime.Today;
            var year = today.Month > 6 ? today.Year + 1 : today.Year;
            var data = (from x in context.ProcessLeaves
                        join c in context.LeaveCategories on x.LeaveCategoryId equals c.LeaveCategoryId
                        join e in context.Employees on x.EmployeeId equals e.Id
                        where x.Year == year && x.IsActive.HasValue && x.IsActive.Value && c.IsActive && x.EmployeeId == employeeId
                        && (c.LeaveCategoryId != 6 || (c.LeaveCategoryId == 6 && e.GenderId == 2 && e.MaritalTypeId == 3))
                        select new LeaveBalanceCustomModel()
                        {
                            Id = x.Id,
                            EmployeeId = x.EmployeeId,
                            Employee = x.Employee,
                            LeaveAvailed = x.LeaveAvailed,
                            LeaveBalance = x.LeaveBalance,
                            LeaveCategoryId = x.LeaveCategoryId,
                            LeaveCategory = x.LeaveCategory,
                            LeaveYear = x.LeaveYear,
                            Year = x.Year,
                            MaxDays = x.MaxDays,
                            TotalLeave = x.TotalLeave
                        }).AsNoTracking().ToList();
            return data;

            // return context.Database.SqlQuery<LeaveBalanceCustomModel>("exec spProcessLeave {0}", Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"])).ToList();

        }
        public List<LeaveBalanceCustomModel> GetLeaveBalance(long employeeId)
        {
            var today = DateTime.Today;
            var year = today.Month > 6 ? today.Year + 1 : today.Year;
            var data = (from x in context.ProcessLeaves
                        join c in context.LeaveCategories on x.LeaveCategoryId equals c.LeaveCategoryId
                        join e in context.Employees on x.EmployeeId equals e.Id
                        where x.Year == year && x.IsActive.HasValue && x.IsActive.Value && c.IsActive && x.EmployeeId == employeeId
                        && (c.LeaveCategoryId != 6 || (c.LeaveCategoryId == 6 && e.GenderId == 2 && e.MaritalTypeId == 3))
                        select new LeaveBalanceCustomModel()
                        {
                            Id = x.Id,
                            EmployeeId = x.EmployeeId,
                            Employee = x.Employee,
                            LeaveAvailed = x.LeaveAvailed,
                            LeaveBalance = x.LeaveBalance,
                            LeaveCategoryId = x.LeaveCategoryId,
                            LeaveCategory = x.LeaveCategory,
                            LeaveYear = x.LeaveYear,
                            Year = x.Year,
                            MaxDays = x.MaxDays,
                            TotalLeave = x.TotalLeave
                        }).AsNoTracking().ToList();
            return data;

            // return context.Database.SqlQuery<LeaveBalanceCustomModel>("exec spProcessLeave {0}", Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"])).ToList();

        }

        public List<LeaveBalanceCustomModel> GetLeaveBalanceByOther(long empId)
        {
            return context.Database.SqlQuery<LeaveBalanceCustomModel>("exec spProcessLeave {0}", empId).ToList();
        }

        public List<LeaveApplicationModel> GetManagerLeaveApprovals(string searchText)
        {

            long id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]);

            var leaveApplications = context.LeaveApplications.Include(x => x.Employee).Include(x => x.LeaveCategory)
                .Where(x => x.ManagerId == id && (x.Employee.Name.Contains(searchText) || x.Employee.EmployeeId.Contains(searchText)))
                .OrderBy(x => x.ManagerStatus == "Pending" ? 0 : 1000000)
                .ThenByDescending(x => x.LeaveApplicationId)
                .ThenByDescending(x => x.StartDate).AsQueryable()
                .Select(x => new LeaveApplicationModel
                {
                    LeaveApplicationId = x.LeaveApplicationId,
                    EmployeeId = x.Employee.EmployeeId,
                    EmployeeName = x.Employee.Name,
                    DepartmentName = x.Employee.Department.Name,
                    DesignationName = x.Employee.Designation.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    ApplicationDate = x.ApplicationDate,
                    LeaveDays = x.LeaveDays,
                    ManagerId = x.ManagerId,
                    HrAdminId = x.HrAdminId,
                    ManagerStatus = x.ManagerStatus,
                    HrAdminStatus = x.HrAdminStatus,
                    LeaveName = x.LeaveCategory.Name,
                    Reason = x.Reason
                });

            return leaveApplications.ToList();
        }

        public List<LeaveApplicationModel> GetHRLeaveApprovals(string searchText)
        {
            long id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]);
            var leaveApplications = context.LeaveApplications
                .Include(x => x.Employee.Designation)
                .Where(x => x.HrAdminId == id && !string.IsNullOrEmpty(x.HrAdminStatus) && (x.Employee.Name.Contains(searchText) || x.Employee.EmployeeId.Contains(searchText)))
                .OrderBy(x => x.HrAdminStatus == "Pending" ? 0 : 1000000)
                .ThenBy(x => string.IsNullOrEmpty(x.HrAdminStatus) ? 0 : 1000000)
                .ThenByDescending(x => x.LeaveApplicationId)
                .ThenByDescending(x => x.StartDate).AsQueryable()
                .Select(x => new LeaveApplicationModel
                {
                    LeaveApplicationId = x.LeaveApplicationId,
                    EmployeeId = x.Employee.EmployeeId,
                    EmployeeName = x.Employee.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    DepartmentName = x.Employee.Department.Name,
                    DesignationName = x.Employee.Designation.Name,
                    ApplicationDate = x.ApplicationDate,
                    LeaveDays = x.LeaveDays,
                    ManagerId = x.ManagerId,
                    HrAdminId = x.HrAdminId,
                    ManagerStatus = x.ManagerStatus,
                    HrAdminStatus = x.HrAdminStatus,
                    LeaveName = x.LeaveCategory.Name,
                    Reason = x.Reason
                });

            return leaveApplications.ToList();
        }

        public bool ChangeMangerStatus(long leaveApplicationId, string managerStatus, string comments, string ip)
        {
            var defaultMail = "default@krishibidgroup.com";
            bool isMailSentToEmployee = false;
            bool isMailSentToLineManager = false;
            bool isMailSentToHR = false;
            string body = string.Empty;
            string subject = string.Empty;

            LeaveEmailCustomModel mailModel = context.LeaveApplications.Include(x => x.Employee.Employee3).Include(x => x.Employee.Employee2).Include(x => x.LeaveCategory).Where(x => x.LeaveApplicationId == leaveApplicationId).Select(x => new
            LeaveEmailCustomModel
            {
                EmployeeId = x.Employee.EmployeeId,
                EmployeeName = x.Employee.Name,
                EmployeeEmail = x.Employee.OfficeEmail ?? defaultMail,
                ManagerEmail = x.Employee.Employee3.OfficeEmail ?? defaultMail,
                HRAdminEmail = x.Employee.Employee2.OfficeEmail ?? defaultMail,
                LeaveCategory = x.LeaveCategory.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                ApplyDate = x.ApplicationDate,
                HrName = x.Employee.Employee2.Name,
                HrStatus = x.HrAdminStatus,
                ManagerName = x.Employee.Employee3.Name,
                ManagerStatus = x.ManagerStatus,
                LeaveDays = x.LeaveDays
            }).FirstOrDefault();

            LeaveApplication leaveApplication = context.LeaveApplications.Where(x => x.LeaveApplicationId == leaveApplicationId).FirstOrDefault();
            if (leaveApplication == null)
            {
                throw new Exception("Leave Application not found!");
            }
            subject = "Leave Application Status - [" + mailModel.EmployeeId + "] " + mailModel.EmployeeName;

            if (leaveApplication.ApplicationDate < leaveApplication.StartDate)
            {
                leaveApplication.IP = ip;
                if (managerStatus.Equals("Approved"))
                {
                    leaveApplication.HrAdminStatus = managerStatus;
                }
                else
                {
                    leaveApplication.HrAdminStatus = "Denied";
                }
                leaveApplication.ManagerStatus = managerStatus;
                leaveApplication.ManagerApprovalDate = DateTime.Now;
                if (context.SaveChanges() > 0)
                {
                    if (leaveApplication.HrAdminStatus.Equals("Approved"))
                    {
                        context.Database.ExecuteSqlCommand(String.Format("spUpdateAttendanceFromLeave {0},{1}", leaveApplication.LeaveApplicationId, leaveApplication.Id));
                    }

                    //Mail is temporarylly off
                    //body = EmailBody(mailModel, leaveApplication);
                    //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, mailModel.EmployeeEmail, mailModel.EmployeeName, string.Empty, string.Empty, subject, body, out string sendStatus);
                    //isMailSentToLineManager = MailService.SendMail(string.Empty, string.Empty, mailModel.ManagerEmail, mailModel.ManagerName, string.Empty, string.Empty, subject, body, out sendStatus);
                    //isMailSentToHR = MailService.SendMail(string.Empty, string.Empty, mailModel.HRAdminEmail, mailModel.HrName, string.Empty, string.Empty, subject, body, out sendStatus);
                    return context.SaveChanges() > 0;
                }
            }
            if (managerStatus.Equals("Approved"))
            {
                leaveApplication.HrAdminStatus = "Pending";
                if ((leaveApplication.ApplicationDate - leaveApplication.EndDate).Days <= 3)
                {
                    leaveApplication.HrAdminStatus = "Approved";
                }

                if (leaveApplication.ManagerId == leaveApplication.HrAdminId || leaveApplication.ManagerId == 1 || leaveApplication.ManagerId == 2)
                {
                    leaveApplication.ManagerStatus = "Approved";
                    leaveApplication.HrAdminStatus = "Approved";
                }

            }
            else
            {
                leaveApplication.ManagerStatus = managerStatus;
                leaveApplication.HrAdminStatus = managerStatus;
            }
            leaveApplication.ManagerStatus = managerStatus;
            leaveApplication.IP = ip;
            leaveApplication.ManagerApprovalDate = DateTime.Now;//Ashraf20200218
            leaveApplication.Employee = null;
            leaveApplication.LeaveCategory = null;

            //Mail Service Temporarylly off
            //if (context.SaveChanges() > 0)
            //{
            //    body = EmailBody(mailModel, leaveApplication);
            //    isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, mailModel.EmployeeEmail, mailModel.EmployeeName, string.Empty, string.Empty, subject, body, out string sendStatus);
            //    isMailSentToLineManager = MailService.SendMail(string.Empty, string.Empty, mailModel.ManagerEmail, mailModel.ManagerName, string.Empty, string.Empty, subject, body, out sendStatus);
            //}
            if (context.SaveChanges() > 0)
            {
                if (leaveApplication.HrAdminStatus.Equals("Approved"))
                {
                    context.Database.ExecuteSqlCommand(String.Format("spUpdateAttendanceFromLeave {0},{1}", leaveApplication.LeaveApplicationId, leaveApplication.Id));
                }
            }
            return false;
        }


        public bool ChangeHRStatus(long leaveApplicationId, string hrStatus, string comments, string ip)
        {
            var defaultMail = "default@krishibidgroup.com";
            //bool isMailSentToEmployee = false;
            //bool isMailSentToLineManager = false;
            //bool isMailSentToHR = false;
            string body = string.Empty;
            string subject = string.Empty;

            LeaveEmailCustomModel mailModel = context.LeaveApplications.Include(x => x.Employee.Employee3).Include(x => x.Employee.Employee2).Include(x => x.LeaveCategory).Where(x => x.LeaveApplicationId == leaveApplicationId).Select(x => new
           LeaveEmailCustomModel
            {
                EmployeeId = x.Employee.EmployeeId,
                EmployeeName = x.Employee.Name,
                EmployeeEmail = x.Employee.OfficeEmail ?? defaultMail,
                ManagerEmail = x.Employee.Employee3.OfficeEmail ?? defaultMail,
                HRAdminEmail = x.Employee.Employee2.OfficeEmail ?? defaultMail,
                LeaveCategory = x.LeaveCategory.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                ApplyDate = x.ApplicationDate,
                HrName = x.Employee.Employee2.Name,
                HrStatus = x.HrAdminStatus,
                ManagerName = x.Employee.Employee3.Name,
                ManagerStatus = x.ManagerStatus,
                LeaveDays = x.LeaveDays
            }).FirstOrDefault();


            LeaveApplication leaveApplication = context.LeaveApplications.Where(x => x.LeaveApplicationId == leaveApplicationId).FirstOrDefault();
            if (leaveApplication == null)
            {
                throw new Exception("Leave Application not found!");
            }
            subject = "Leave Application Status - [" + mailModel.EmployeeId + "] " + mailModel.EmployeeName;


            leaveApplication.HrAdminStatus = hrStatus;
            leaveApplication.IP = ip;
            leaveApplication.HRApprovalDate = DateTime.Now;//Ashraf20200218
            leaveApplication.Employee = null;
            leaveApplication.LeaveCategory = null;

            if (context.SaveChanges() > 0)
            {
                if (hrStatus.Equals("Approved"))
                {
                    context.Database.ExecuteSqlCommand(String.Format("spUpdateAttendanceFromLeave {0},{1}", leaveApplication.LeaveApplicationId, leaveApplication.Id));
                }
                //Mail service is temporarylly off
                //body = EmailBody(mailModel, leaveApplication);
                //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, mailModel.EmployeeEmail, mailModel.EmployeeName, string.Empty, string.Empty, subject, body, out string sendStatus);
                //isMailSentToLineManager = MailService.SendMail(string.Empty, string.Empty, mailModel.ManagerEmail, mailModel.ManagerName, string.Empty, string.Empty, subject, body, out sendStatus);
                //isMailSentToHR = MailService.SendMail(string.Empty, string.Empty, mailModel.HRAdminEmail, mailModel.HrName, string.Empty, string.Empty, subject, body, out sendStatus);
            }
            return context.SaveChanges() > 0;

        }
        public bool UpdateLeaveStatus(long leaveApplicationId, LeaveStatusEnum status, string comments, string ip, ApprovarTypeEnum approvar, out string message)
        {
            LeaveApplication leaveApplication = context.LeaveApplications.Where(x => x.LeaveApplicationId == leaveApplicationId).FirstOrDefault();
            if (leaveApplication == null)
            {
                message = "Sorry! no application found.";
                return false;
            }
            var userId = Common.GetIntUserId();
            bool isJointYearLeave = false;
            int fromYear = leaveApplication.StartDate.Year;
            int toYear = leaveApplication.EndDate.Year;
            bool isFinalApproved = false;
            if (leaveApplication.StartDate.Month > 6)
            {
                fromYear += 1;
            }
            if (leaveApplication.StartDate.Month < 7 && leaveApplication.EndDate.Month > 6)
            {
                isJointYearLeave = true;
                toYear += 1;
            }
            if (approvar == ApprovarTypeEnum.Manager)
            {
                if (status == LeaveStatusEnum.Denied)
                {
                    leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Denied;
                    leaveApplication.Status = (int)StatusEnum.Inactive;
                    leaveApplication.HrAdminStatus = status.ToString();
                }
                else
                {
                    leaveApplication.HrAdminStatus = LeaveStatusEnum.Pending.ToString();
                }
                leaveApplication.ManagerStatus = status.ToString();
                leaveApplication.ManagerApprovalDate = DateTime.Now;
                leaveApplication.ManagerComment = comments;
                leaveApplication.IP = ip;

                //72 hours rules are stopped by HR Manager from 2023-12-24
                //var aplicationDays = leaveApplication.ApplicationDate.Subtract(leaveApplication.EndDate.Date).Days;
                //if (status == LeaveStatusEnum.Approved && (aplicationDays <= 3 || leaveApplication.ManagerId == leaveApplication.HrAdminId || leaveApplication.ManagerId == 1 || leaveApplication.ManagerId == 2))
                //{
                //    leaveApplication.HrAdminStatus = status.ToString();
                //    leaveApplication.HRApprovalDate = DateTime.Now;
                //    leaveApplication.HrAdminComment = "Manager approved in 72 hours after request";
                //    leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Approved;
                //    isFinalApproved = true;             
                //}

                if (status == LeaveStatusEnum.Approved && (leaveApplication.ManagerId == leaveApplication.HrAdminId || leaveApplication.ManagerId == 1 || leaveApplication.ManagerId == 2))
                {
                    leaveApplication.HrAdminStatus = status.ToString();
                    leaveApplication.HRApprovalDate = DateTime.Now;
                    leaveApplication.HrAdminComment = comments;
                    leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Approved;
                    isFinalApproved = true;
                }
            }
            if (approvar == ApprovarTypeEnum.HRAdmin)
            {
                if (status == LeaveStatusEnum.Denied)
                {
                    leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Denied;
                    leaveApplication.Status = (int)StatusEnum.Inactive;
                }
                leaveApplication.HrAdminStatus = status.ToString();
                leaveApplication.HRApprovalDate = DateTime.Now;
                leaveApplication.HrAdminComment = comments;
                if (status == LeaveStatusEnum.Approved)
                {
                    //if hradmin approved directly
                    if (string.IsNullOrEmpty(leaveApplication.ManagerStatus) || leaveApplication.ManagerStatus != leaveApplication.HrAdminStatus)
                    {
                        leaveApplication.ManagerStatus = leaveApplication.HrAdminStatus;
                        leaveApplication.ManagerApprovalDate = leaveApplication.HRApprovalDate;
                    }

                    leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Approved;
                    isFinalApproved = true;

                }

            }
            if (isFinalApproved)
            {
                if (isJointYearLeave)
                {
                    var nextYearLeaveDays = leaveApplication.EndDate.Day;
                    var previousYearLeaveDays = leaveApplication.LeaveDays - nextYearLeaveDays;
                    var nextYearProcessLeave = context.ProcessLeaves.FirstOrDefault(x => x.IsActive.HasValue && x.IsActive.Value && x.Year == toYear && x.EmployeeId == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                    if (nextYearProcessLeave != null)
                    {
                        nextYearProcessLeave.LeaveBalance -= nextYearLeaveDays;
                        nextYearProcessLeave.LeaveAvailed += nextYearLeaveDays;
                        nextYearProcessLeave.LastLeaveApplicationId = leaveApplication.LeaveApplicationId;
                    }
                    var processLeave = context.ProcessLeaves.FirstOrDefault(x => x.IsActive.HasValue && x.IsActive.Value && x.Year == fromYear && x.EmployeeId == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                    if (processLeave != null)
                    {
                        processLeave.LeaveBalance -= previousYearLeaveDays;
                        processLeave.LeaveAvailed += previousYearLeaveDays;
                        processLeave.LastLeaveApplicationId = leaveApplication.LeaveApplicationId;
                    }

                    //for probation period employee balance deduct
                    var isProbationEmployee = context.Employees.FirstOrDefault(x => x.Active && x.Id == leaveApplication.Id && x.JobStatusId == 41);
                    if (processLeave == null && isProbationEmployee != null)
                    {
                        var leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.IsActive && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                        if (leaveCategory != null)
                        {
                            processLeave = new ProcessLeave()
                            {
                                Employee = leaveApplication.Id,
                                EmployeeId = leaveApplication.Id,
                                LeaveCategory = ((LeaveTypeEnum)(leaveApplication.LeaveCategoryId)).ToString(),
                                LeaveCategoryId = leaveApplication.LeaveCategoryId,
                                IsActive = true,
                                MaxDays = (int)leaveCategory.MaxDays,
                                TotalLeave = (int)leaveCategory.MaxDays,
                                CreatedBy = "System",
                                CreatedDate = DateTime.Now,
                                LeaveBalance = (int)leaveCategory.MaxDays - leaveApplication.LeaveDays,
                                LeaveAvailed = leaveApplication.LeaveDays,
                                LeaveYear = fromYear.ToString(),
                                Year = fromYear
                            };
                            context.ProcessLeaves.Add(processLeave);
                        }

                    }

                }
                if (!isJointYearLeave)
                {

                    var processLeave = context.ProcessLeaves.FirstOrDefault(x => x.IsActive.HasValue && x.IsActive.Value && x.Year == fromYear && x.EmployeeId == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                    if (processLeave != null)
                    {
                        processLeave.LeaveBalance -= leaveApplication.LeaveDays;
                        processLeave.LeaveAvailed += leaveApplication.LeaveDays;
                        processLeave.LastLeaveApplicationId = leaveApplication.LeaveApplicationId;
                    }
                    //for probation period employee balance deduct
                    var isProbationEmployee = context.Employees.FirstOrDefault(x => x.Active && x.Id == leaveApplication.Id && x.JobStatusId == 41);
                    if (processLeave == null && isProbationEmployee != null)
                    {
                        var leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.IsActive && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                        if (leaveCategory != null)
                        {
                            processLeave = new ProcessLeave()
                            {
                                Employee = leaveApplication.Id,
                                EmployeeId = leaveApplication.Id,
                                LeaveCategory = ((LeaveTypeEnum)(leaveApplication.LeaveCategoryId)).ToString(),
                                LeaveCategoryId = leaveApplication.LeaveCategoryId,
                                IsActive = true,
                                MaxDays = (int)leaveCategory.MaxDays,
                                TotalLeave = (int)leaveCategory.MaxDays,
                                CreatedBy = "System",
                                CreatedDate = DateTime.Now,
                                LeaveBalance = (int)leaveCategory.MaxDays - leaveApplication.LeaveDays,
                                LeaveAvailed = leaveApplication.LeaveDays,
                                LeaveYear = fromYear.ToString(),
                                Year = fromYear
                            };
                            context.ProcessLeaves.Add(processLeave);
                        }
                    }
                }

            }
            var result = context.SaveChanges();
            if (result > 0)
            {
                if (isFinalApproved)
                {
                    context.Database.ExecuteSqlCommand(String.Format("spUpdateAttendanceFromLeave {0},{1}", leaveApplication.LeaveApplicationId, leaveApplication.Id));
                }
                message = $"This request has been successfully {status.ToString()}.";
                return true;
            }
            message = "Sorry! something went wrong.";
            return false;
        }

        public async Task<TeamLeaveBalanceCustomModel> GetTeamLeaveBalance(long managerId, int selectedYear)
        {
            //managerId = 1028;
            TeamLeaveBalanceCustomModel model = new TeamLeaveBalanceCustomModel();
            //List<Employee> employees = context.Employees.Where(x => x.Active && x.ManagerId == managerId).ToList();
            //foreach (var employee in employees)
            //{
            //    context.Database.SqlQuery<LeaveBalanceCustomModel>("exec spProcessLeave {0}", employee.Id).ToList();
            //}
            model.DataList = context.Database.SqlQuery<TeamLeaveBalanceCustomModel>(string.Format("exec spGetTeamLeaveBalance {0},{1}", managerId, selectedYear)).AsEnumerable();
            model.SelectedYearList = context.ProcessLeaves
                .Select(x => new SelectModel()
                {
                    Text = x.LeaveYear,
                    Value = x.LeaveYear

                }).Distinct()
                .ToList().Select(s => new SelectModel()
                {
                    Text = (Convert.ToInt32(s.Text) - 1).ToString() + " - " + s.Text,
                    Value = s.Value
                })
                .OrderByDescending(o => Convert.ToInt32(o.Value))
                .ToList();

            return model;
        }
        public async Task<IEnumerable<TeamLeaveBalanceCustomModel>> GetCompanywiseLeaveBalance(int companyId, int departmentId, int selectedYear)
        {
            //managerId = 1028;
            //  TeamLeaveBalanceCustomModel model = new TeamLeaveBalanceCustomModel();
            var data = context.Database.SqlQuery<TeamLeaveBalanceCustomModel>(string.Format("exec spGetHRLeaveBalance {0},{1},{2}", companyId, departmentId, selectedYear)).AsEnumerable();

            return data;
        }

        public IEnumerable<LeaveBalanceCustomModel> GetEmployeeLeaveBalance(string employeeId, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrEmpty(employeeId))
            {
                return new List<LeaveBalanceCustomModel>();
            }
            Employee employee = context.Employees.Where(x => x.EmployeeId.Equals(employeeId)).FirstOrDefault();
            if (employee == null)
            {
                message = "Sorry! No Employee Found";
                return new List<LeaveBalanceCustomModel>();
            }
            return context.Database.SqlQuery<LeaveBalanceCustomModel>("exec spProcessLeave {0}", employee.Id).ToList();
        }

        public IEnumerable<LeaveBalanceCustomModel> GetEmployeeLeaveBalanceByIdDateRange(string employeeId, DateTime startDate, DateTime endDate, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrEmpty(employeeId))
            {
                return new List<LeaveBalanceCustomModel>();
            }
            Employee employee = context.Employees.Where(x => x.EmployeeId.Equals(employeeId)).FirstOrDefault();
            if (employee == null)
            {
                message = "Sorry! No Employee Found";
                return new List<LeaveBalanceCustomModel>();
            }
            return context.Database.SqlQuery<LeaveBalanceCustomModel>("exec spProcessLeaveDateRange {0}", employee.Id).ToList();
        }

        public EmployeeCustomModel GetCustomEmployeeModel(string employeeId)
        {

            if (string.IsNullOrEmpty(employeeId))
            {
                return new EmployeeCustomModel();
            }
            Employee employee = context.Employees.Where(x => x.EmployeeId.Equals(employeeId)).FirstOrDefault();
            if (employee == null)
            {
                return new EmployeeCustomModel();
            }
            EmployeeCustomModel employeeCustomModel = new EmployeeCustomModel();
            employeeCustomModel.EmployeeID = employee.EmployeeId;
            employeeCustomModel.EmployeeName = employee.Name;
            return employeeCustomModel;
        }

        public string ProcessLeave()
        {
            IEnumerable<Employee> employees = context.Employees.Where(x => x.Active);
            foreach (var employee in employees)
            {
                context.Database.SqlQuery<LeaveBalanceCustomModel>("exec spProcessLeave {0},{1}", employee.Id, DateTime.Now.Year.ToString()).ToList();
            }
            string message = "Leave Balance updated successfully for " + employees.Count().ToString() + " Employees";
            return message;
        }
        public string EmailBody(LeaveEmailCustomModel mailModel, LeaveApplication leaveApplication)
        {
            string body = "";

            body = "<!DOCTYPE html>";
            body += "<html> <head> <style> ";
            body += "table { border: 0px solid #ddd;   width: 500px; } th, td { text - align: left; font - size:12; border: 0px solid #ddd;  padding: 0px;}";
            body += " tr: nth-child(even){ background-color: #f2f2f2}  th {background-color: #007f3d;text-align:right;  border: 1px solid #ddd; width: 200px;   color: white;} td {background-color: #C8E5EB;  border: 1px solid #ddd;  width: 300px;  color: black;} ";
            body += " h5 { color: red; } h4 { color: black; }</style></head><body>  ";
            body += "<H4>Dear Concern,</H4>";
            body += "Please" + "<a href=" + "http://192.168.0.7:90/user/login" + "> click here </a> for details and action of <b> Leave Application</b>";

            body += "<table>";
            body += "<tr>";
            body += "<th>" + "Employee ID :" + "</th>";
            body += "<td>" + mailModel.EmployeeId + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Name :" + "</th>";
            body += "<td>" + mailModel.EmployeeName + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Leave Category :" + "</th>";
            body += "<td>" + mailModel.LeaveCategory + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Applied Date :" + "</th>";
            body += "<td>" + mailModel.ApplyDate.Value.ToString("dd-MMM-yyyy") + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Leave Date :" + "</th>";
            body += "<td>" + "From " + mailModel.StartDate.Value.ToString("dd-MMM-yyyy") + " to " + mailModel.EndDate.Value.ToString("dd-MMM-yyyy") + "</td>";
            body += "</tr>";

            body += "<tr>";
            body += "<th>" + "Leave Days : " + "</th>";
            body += "<td>" + mailModel.LeaveDays + "</td>";
            body += "</tr>";

            body += "<tr>";
            body += "<th>" + "Manager Status : " + "</th>";
            body += "<td>" + leaveApplication.ManagerStatus + " </td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "HR Status: " + "</th>";
            body += "<td>" + leaveApplication.HrAdminStatus + "</td>";
            body += "</tr>";

            body += "</table><br/><H5>[This is system generated emial notification.<b> HelpLine:Cell: 01700729805/8 PBX no.817<b/></H5></body></html>";
            return body;
        }

        public string EmailBodyForLeaveApplication(Employee employee, LeaveCategory leaveCategory, LeaveApplication leaveApplication)
        {
            string body = "";

            body = "<!DOCTYPE html>";
            body += "<html> <head> <style> ";
            body += "table { border: 0px solid #ddd;   width: 500px; } th, td { text - align: left; font - size:12; border: 0px solid #ddd;  padding: 0px;}";
            body += " tr: nth-child(even){ background-color: #f2f2f2}  th {background-color: #007f3d;text-align:right;  border: 1px solid #ddd; width: 200px;   color: white;} td {background-color: #C8E5EB;  border: 1px solid #ddd;  width: 300px;  color: black;} ";
            body += " h5 { color: red; } h4 { color: black; }</style></head><body>  ";
            body += "<H4>Dear Concern,</H4>";
            body += "Please" + "<a href=" + "http://192.168.0.7:90/user/login" + "> click here </a> for details and action of <b> Leave Application</b>";

            body += "<table>";
            body += "<tr>";
            body += "<th>" + "Employee ID :" + "</th>";
            body += "<td>" + employee.EmployeeId + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Name :" + "</th>";
            body += "<td>" + employee.Name + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Leave Category :" + "</th>";
            body += "<td>" + leaveCategory.Name + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Applied Date :" + "</th>";
            body += "<td>" + leaveApplication.ApplicationDate.ToString("dd-MMM-yyyy") + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Leave Date :" + "</th>";
            body += "<td>" + "From " + leaveApplication.StartDate.ToString("dd-MMM-yyyy") + " to " + leaveApplication.EndDate.ToString("dd-MMM-yyyy") + "</td>";
            body += "</tr>";

            body += "<tr>";
            body += "<th>" + "Leave Days : " + "</th>";
            body += "<td>" + leaveApplication.LeaveDays + "</td>";
            body += "</tr>";

            body += "<tr>";
            body += "<th>" + "Manager Status : " + "</th>";
            body += "<td>" + leaveApplication.ManagerStatus + " </td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "HR Status: " + "</th>";
            body += "<td>" + leaveApplication.HrAdminStatus + "</td>";
            body += "</tr>";

            body += "</table><br/><H5>[This is system generated emial notification.<b> HelpLine:Cell: 01700729805/8 PBX no.817<b/></H5></body></html>";
            return body;
        }

        public List<LeaveApplicationModel> GetLeaveApplicationsByOther(string searchText, int pageSize = 10, int pageNo = 1)
        {

            string sqlQuery = @"exec spHRMSGetLeaveApplicationByOther @searchText, @pageSize, @pageNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@pageNo", pageNo),
                new SqlParameter("@searchText",searchText)
            };

            List<LeaveApplicationModel> result = context.Database.SqlQuery<LeaveApplicationModel>(sqlQuery, parameters).ToList();

            //return context.Database.SqlQuery<LeaveApplicationModel>(@"exec spHRMSGetLeaveApplicationByOther").ToList();
            return result;
        }

        public bool SaveOtherLeaveApplication(int id, LeaveApplicationModel model, long empId, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("Leave Application data missing!");
            }

            Employee employee = context.Employees.Where(x => x.Id == empId).FirstOrDefault();

            LeaveApplication leaveApplication = ObjectConverter<LeaveApplicationModel, LeaveApplication>.Convert(model);

            bool exist = context.LeaveApplications.Where(x => x.Id == id && x.ManagerStatus.Equals("Approved") && (x.StartDate <= leaveApplication.StartDate && x.EndDate >= leaveApplication.EndDate)).Any();
            if (exist)
            {
                message = "Employee had already used this date range !";
                return false;
            }

            LeaveCategory leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.LeaveCategoryId == leaveApplication.LeaveCategoryId);

            int leaveYear = DateTime.Now.Month > 6 ? DateTime.Now.Year + 1 : DateTime.Now.Year;

            ProcessLeave processLeave = context.ProcessLeaves.ToList().FirstOrDefault(x => x.Employee == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId && x.LeaveYear == leaveYear.ToString());
            //if (processLeave != null)
            //{
            //    if (processLeave.MaxDays < (leaveApplication.LeaveDays + processLeave.LeaveAvailed))
            //    {
            //        message = "Sorry! Employee already consumed this leave";
            //        return false;
            //    }
            //}

            //else
            //{
            //    leaveCategory = context.LeaveCategories.ToList().FirstOrDefault(x => x.LeaveCategoryId == model.LeaveCategoryId);
            //    if (leaveCategory != null)
            //    {
            //        if (leaveCategory.MaxDays < model.LeaveDays)
            //        {
            //            message = "Sorry! Employee is not eligible to consume this leave";
            //            return false;
            //        }
            //    }

            //}
            leaveApplication.Id = employee.Id;
            leaveApplication.ManagerId = employee.ManagerId;
            leaveApplication.ManagerStatus = "Approved";
            leaveApplication.ManagerApprovalDate = DateTime.Now;
            leaveApplication.ManagerComment = "Autometic Approval";

            leaveApplication.HrAdminId = employee.HrAdminId;
            leaveApplication.HrAdminStatus = "Approved";
            leaveApplication.HRApprovalDate = DateTime.Now;
            leaveApplication.HrAdminComment = "Autometic Approval";

            leaveApplication.LeaveCategoryId = model.LeaveCategoryId;
            leaveApplication.StartDate = model.StartDate ?? DateTime.Now;
            leaveApplication.EndDate = model.EndDate ?? DateTime.Now;
            leaveApplication.LeaveDays = model.LeaveDays;
            leaveApplication.Address = model.Address;
            leaveApplication.ContactName = model.ContactName;
            leaveApplication.Reason = model.Reason;
            leaveApplication.Remarks = model.Remarks;
            leaveApplication.AppliedBy = System.Web.HttpContext.Current.User.Identity.Name;
            leaveApplication.ApplicationDate = DateTime.Now;
            leaveApplication.IP = model.IP;

            for (DateTime date = leaveApplication.StartDate.Date; date <= leaveApplication.EndDate.Date; date += TimeSpan.FromDays(1))
            {
                LeaveApplicationDetail leaveApplicationDetail = new LeaveApplicationDetail();
                leaveApplicationDetail.LeaveDate = date;
                leaveApplicationDetail.LeaveYear = date.Year.ToString();
                leaveApplication.LeaveApplicationDetails.Add(leaveApplicationDetail);
            }
            leaveApplication.LeaveCategory = null;
            leaveApplication.Employee = null;
            context.Entry(leaveApplication).State = leaveApplication.LeaveApplicationId == 0 ? EntityState.Added : EntityState.Modified;

            int noOfRowAfftected = context.SaveChanges();
            if (noOfRowAfftected > 0)
            {
                context.Database.ExecuteSqlCommand(String.Format("spUpdateAttendanceFromLeave {0},{1}", leaveApplication.LeaveApplicationId, leaveApplication.Id));
            }
            return noOfRowAfftected > 0;
        }

        public int LeaveCalculation()
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    int status = 0;
                    var today = DateTime.Today;
                    int year = today.Month > 6 ? today.Year + 1 : today.Year;
                    int nextYear = year + 1;
                    var leaveCategoryList = context.LeaveCategories.Where(x => x.IsActive && x.LeaveCategoryId != (int)LeaveTypeEnum.SickLeave).AsNoTracking().ToList();
                    var lastJoiningDate = DateTime.Today.AddMonths(-6);
                    if (leaveCategoryList == null)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                    var employeeList = (from e in context.Employees
                                        where e.Active && e.JoiningDate.HasValue && e.JobStatusId == (int)EmploymentStatusEnum.Permanent
                                        select new
                                        {
                                            Id = e.Id,
                                            EmployeeId = e.EmployeeId,
                                            Gender = e.GenderId.HasValue ? (GenderTypeEnum)e.GenderId.Value : GenderTypeEnum.Others,
                                            PermanentDate = e.PermanentDate.HasValue ? e.PermanentDate : today,
                                            MaritalStatus = e.MaritalTypeId.HasValue ? (MaritalStatusEnum)e.MaritalTypeId.Value : MaritalStatusEnum.Single
                                        }).AsNoTracking().ToList();

                    foreach (var item in leaveCategoryList)
                    {
                        var totalLeave = 0;
                        var assignEmployeeList = (from x in context.ProcessLeaves
                                                  where x.IsActive.HasValue && x.IsActive.Value && x.LeaveCategoryId == item.LeaveCategoryId
                                                  && x.Year == year
                                                  select x.EmployeeId).ToList();

                        var notAssignEmployee = employeeList.Where(x => !assignEmployeeList.Any(y => y == x.Id)).ToList();
                        var lastDayOfYear = new DateTime(year, 6, 30);

                        foreach (var i in notAssignEmployee)
                        {
                            var remainingDays = (lastDayOfYear - i.PermanentDate.Value).TotalDays + 1;
                            double leavePerDay = item.MaxDays.Value / 365.0;
                            int leaveDay = (int)Math.Floor(remainingDays * leavePerDay);
                            if (leaveDay > item.MaxDays)
                            {
                                leaveDay = item.MaxDays.Value;
                            }
                            //continue;
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
                                LeaveYear = year.ToString(),
                                Year = year
                            };
                            context.ProcessLeaves.Add(processLeave);
                            status += context.SaveChanges();
                        }
                    }

                    //sick leave entry in first date of year
                    if (today.Month == 7 && today.Day == 1)
                    {
                        var sickLeave = context.LeaveCategories.FirstOrDefault(x => x.IsActive && x.LeaveCategoryId == (int)LeaveTypeEnum.SickLeave);
                        if (sickLeave == null)
                        {
                            return 0;
                        }
                        var allEmployeeList = context.Employees.Where(x => x.Active).Select(x => x.Id).ToList();
                        var assignSickLeaveEmployeeList = context.ProcessLeaves.Where(x => x.LeaveCategoryId == (int)LeaveTypeEnum.SickLeave).Where(x => x.Year == year)
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
                            context.ProcessLeaves.Add(processLeave);
                            status += context.SaveChanges();
                        }
                    }

                    if (today.Month == 6 && today.Day > 14 && (today.Day == 15 || today.Day % 5 == 0))
                    {
                        foreach (var item in leaveCategoryList)
                        {
                            var assignEmployeeList = (from x in context.ProcessLeaves
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
                                context.ProcessLeaves.Add(processLeave);
                                status += context.SaveChanges();
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
        #region leaveSuffixPrefix
        public List<YearlyHoliday> AllYearlyHolidays()
        {
            List<YearlyHoliday> holidays = new List<YearlyHoliday>();
            holidays = context.YearlyHolidays.ToList();
            return holidays;
        }
        public List<DateTime> AllFridaySaturdays(int year)
        {
            List<DateTime> allFridaysSaturdays = new List<DateTime>();
            DateTime currentDate = new DateTime(year, 1, 1);
            while (currentDate.Year == year)
            {
                if (currentDate.DayOfWeek == DayOfWeek.Friday || currentDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    allFridaysSaturdays.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }
            return allFridaysSaturdays;
        }
        public SuffixPrefixResult isSuffixPrefix(DateTime startingDate, DateTime endingDate, long empId)
        {
            SuffixPrefixResult result = new SuffixPrefixResult();
            bool isHolidayOrWeekend = false;
            int count = 0;
            List<YearlyHoliday> holidays = AllYearlyHolidays();
            List<DateTime> holidayDates = holidays.Where(h => h.HolidayDate.Year == 2024).Select(h => h.HolidayDate).ToList();
            List<DateTime> fridaysAndSaturdays = AllFridaySaturdays(2024);
            List<DateTime> leaves = GetLeaveDatesByEmployee(empId);
            List<DateTime> allHolidayWeekends = holidayDates.Union(fridaysAndSaturdays).Union(leaves).ToList();

            bool isPrefixHolidayOrWeekends = allHolidayWeekends.Contains(startingDate.AddDays(-1));
            bool isSuffixHolidayOrWeekends = allHolidayWeekends.Contains(endingDate.AddDays(1));
            result.Result = isPrefixHolidayOrWeekends && isSuffixHolidayOrWeekends;
            result.StartDate = startingDate.AddDays(-1).ToString();
            result.EndDate = endingDate.AddDays(1).ToString();
            return result;
        }
        public List<DateTime> GetLeaveDatesByEmployee(long empId)
        {
            List<DateTime> leaves = new List<DateTime>();

            var leaveApplications = context.LeaveApplications
                .Where(x => x.Id == empId && x.Status==1)
                .Select(y => new { y.StartDate, y.EndDate })
                .ToList();

            foreach (var application in leaveApplications)
            {
                DateTime startDate = application.StartDate;
                DateTime endDate = application.EndDate;

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    leaves.Add(date);
                }
            }
            return leaves;
        }
        public bool isUnapprovedApplication(long empId)
        {
            var res = context.LeaveApplications.Where(x => x.Id == empId && x.Active == 1).OrderByDescending(x => x.LeaveApplicationId).FirstOrDefault();
            if (res == null)
            {
                return false;
            }
            else
            {
                if ((res.LeaveStatus == (int)LeaveStatusEnum.Approved) || (res.LeaveStatus == (int)LeaveStatusEnum.Denied))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }
        #endregion
        #region New Leave Application
        public async Task<LeaveApplicationVm> GetLeaveApplicationByEmployeeNew(DateTime? fromDate, DateTime? toDate)
        {
            long id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"].ToString());
            LeaveApplicationVm model = new LeaveApplicationVm();
            List<int> levels = new List<int>();
            model.DataList = await Task.Run(() => (from t1 in context.LeaveApplications
                                                   join t2 in context.Employees on t1.Id equals t2.Id
                                                   join t3 in context.LeaveCategories on t1.LeaveCategoryId equals t3.LeaveCategoryId
                                                   where t1.Id == id
                                                   && (fromDate == null || t1.StartDate >= fromDate)
                                                   && (toDate == null || t1.EndDate <= toDate) && context.SignatoryApprovalMaps.FirstOrDefault(x => x.IntregratedFromId == t1.LeaveApplicationId && x.TableName == "LeaveApplication") != null
                                                   select new LeaveApplicationVm
                                                   {
                                                       LeaveApplicationId = t1.LeaveApplicationId,
                                                       EmployeeId = t2.EmployeeId,
                                                       CategoryName = t3.Name,
                                                       EmployeeName = "[" + t2.EmployeeId + "] " + t2.Name,
                                                       Reason = t1.Reason,
                                                       ApplicationDate = t1.ApplicationDate,
                                                       StartDate = t1.StartDate,
                                                       EndDate = t1.EndDate,
                                                       LeaveDays = t1.LeaveDays,
                                                       HrAdminId = t1.HrAdminId,
                                                       ManagerId = t1.ManagerId,
                                                       HrAdminStatus = t1.HrAdminStatus,
                                                       ManagerStatus = t1.ManagerStatus,
                                                       ApprovalLevelCount = context.SignatoryApprovalMaps.Where(x => x.EmployeeId == t2.Id).Max(x => x.OrderBy),
                                                   }).OrderByDescending(o => o.LeaveApplicationId).AsEnumerable());
            model.ApprovalLevelCount = context.SignatoryApprovalMaps.Where(x => x.EmployeeId == id && x.TableName == "LeaveApplication").Max(x => x.OrderBy);
            for (int i = 1; i <= model.DataList.Count(); i++)
            {
                var x = (from a in context.SignatoryApprovalMaps
                         where a.OrderBy == i
                         select a.Status).FirstOrDefault();
                levels.Add(x);
            }
            model.Levels = levels;
            return model;
        }
        public async Task<LeaveAllDetailVM> GetLeaveApprovalRelatedAllInfo(long SigId, DateTime? fromDate, DateTime? toDate)
        {
            LeaveAllDetailVM model = new LeaveAllDetailVM();
            model.LeaveApprovalDataList = await Task.Run(() => (
    from t1 in context.SignatoryApprovalMaps
    join t2 in context.Employees on t1.EmployeeId equals t2.Id
    join t3 in context.LeaveApplications on t1.IntregratedFromId equals t3.LeaveApplicationId
    join t4 in context.Employees on t3.Id equals t4.Id
    join t5 in context.LeaveCategories on t3.LeaveCategoryId equals t5.LeaveCategoryId
    where t1.EmployeeId == SigId && t1.IsActive
    && t1.TableName == "LeaveApplication"
    && (fromDate == null || t3.StartDate >= fromDate)
    && (toDate == null || t3.EndDate <= toDate)
    select new LeaveAllDetailVM
    {
        LeaveStatus = t3.LeaveStatus,
        LeaveApplicationID = t3.LeaveApplicationId,
        EmployeeID = t4.Id,
        SigID = SigId,
        EmployeeKGid = t4.EmployeeId,
        ApplicantName = t4.Name,
        StartDate = t3.StartDate,
        EndDate = t3.EndDate,
        ApplyDate = t3.ApplicationDate,
        Reason = t3.Reason,
        LeaveDays = t3.LeaveDays,
        LeaveCategory = t5.Name,
        CompanyID = t4.CompanyId,
        TableName = t1.TableName,
        Status = context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == t3.LeaveApplicationId && x.EmployeeId == SigId && x.TableName == "LeaveApplication").Select(x => x.Status).FirstOrDefault(),
        SignatoriesApprovalList = (from a1 in context.RequisitionSignatories.Where(z => z.EmployeeId == t3.Id)
                                   join sa in context.SignatoryApprovalMaps on a1.SignatoryEmpId equals sa.EmployeeId
                                   join e in context.Employees on a1.SignatoryEmpId equals e.Id
                                   where sa.IntregratedFromId == t3.LeaveApplicationId
                                   select new SignatoryApprovalDetails
                                   {
                                       SigAppMapID = sa.SignatoryApprovalMapId,
                                       SigIndID = a1.SignatoryEmpId,
                                       SigName = e.Name,
                                       SigDesignation = a1.DesignationName,
                                       ApprovalStatus = sa.Status,
                                       SigApprovalStatus = sa.Status == -1 ? "....." : sa.Status == 0 ? "Pending" : sa.Status == 1 ? "Approved" : sa.Status == 2 ? "Rejected" : "",
                                       OrderBy = sa.OrderBy
                                   }).OrderByDescending(y => y.OrderBy).ToList()
    }).GroupBy(x => x.LeaveApplicationID)
    .Select(g => g.FirstOrDefault())
    .OrderByDescending(x => x.LeaveApplicationID)
    .ToListAsync());


            return model;
        }
        public async Task<SignatoryApprovalDetails> GetSignatoriesApprovalInfo(long applicationId, long sigId)
        {
            SignatoryApprovalDetails model = new SignatoryApprovalDetails();
            model.SignatoryDataList = await Task.Run(() => (from t1 in context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == applicationId && x.TableName == "LeaveApplication")
                                                            join t2 in context.Employees on t1.EmployeeId equals t2.Id
                                                            join t3 in context.Designations on t2.DesignationId equals t3.DesignationId
                                                            select new SignatoryApprovalDetails
                                                            {
                                                                SigAppMapID = t1.SignatoryApprovalMapId,
                                                                SigName = t2.Name,
                                                                SigDesignation = t3.Name,
                                                                ApprovalStatus = t1.Status,
                                                                //SigApprovalStatus = t1.Status == -1 ? "....." : t1.Status == 0 ? "Pending" : t1.Status == 1 ? "Approved" : t1.Status == 2 ? "Rejected" : "",
                                                                SigApprovalStatus = t1.Status == -1 ? "....." :
                                                                                    t1.Status == 0 ? "Pending" :
                                                                                    t1.Status == 1 && t1.ModifiedBy == t2.EmployeeId ? "Approved" :
                                                                                    t1.Status == 1 && t1.ModifiedBy != t2.EmployeeId ? "-" :
                                                                                    t1.Status == 2 ? "Rejected" : "",
                                                                OrderBy = t1.OrderBy,
                                                                SigIndID = t1.EmployeeId,
                                                                UserId = sigId,
                                                                KgID = t2.EmployeeId,
                                                                ApproveDate = t1.ModifiedDate.Value
                                                            }).OrderByDescending(y => y.OrderBy).ToListAsync());
            return model;
        }
        public int DoLeaveApproval(LeaveAllDetailVM vm)
        {
            int result = -1;
            SignatoryApprovalMap dbModel = new SignatoryApprovalMap();
            dbModel = context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == vm.LeaveApplicationID && x.EmployeeId == vm.SigID && x.TableName == "LeaveApplication").FirstOrDefault();

            var leaveApplication = context.LeaveApplications.Find(dbModel.IntregratedFromId);

            bool isJointYearLeave = false;
            int fromYear = leaveApplication.StartDate.Year;
            int toYear = leaveApplication.EndDate.Year;
            bool isFinalApproved = false;
            if (leaveApplication.StartDate.Month > 6)
            {
                fromYear += 1;
            }
            if (leaveApplication.StartDate.Month < 7 && leaveApplication.EndDate.Month > 6)
            {
                isJointYearLeave = true;
                toYear += 1;
            }
            if (dbModel != null)
            {
                //var sameOrderSignatories = context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == vm.LeaveApplicationID && x.OrderBy == dbModel.OrderBy).ToList();
                var sameOrderSignatories = (from sam in context.SignatoryApprovalMaps
                                            join e in context.Employees on sam.EmployeeId equals e.Id
                                            where sam.IntregratedFromId == vm.LeaveApplicationID && sam.OrderBy == dbModel.OrderBy && sam.TableName == "LeaveApplication"
                                            select new
                                            {
                                                sam,
                                                EmpID = e.EmployeeId
                                            }).ToList();

                if (sameOrderSignatories != null)
                {
                    foreach (var sameOrder in sameOrderSignatories)
                    {
                        if (sameOrder.EmpID == Common.GetUserId() && sameOrder.sam.EmployeeId != Common.GetHRAdminId())
                        {
                            sameOrder.sam.Status = vm.ApprovalStatus;
                            sameOrder.sam.ModifiedBy = Common.GetUserId();
                            sameOrder.sam.ModifiedDate = DateTime.Now;
                        }
                        else if(sameOrder.sam.EmployeeId == Common.GetHRAdminId())
                        {
                            sameOrder.sam.Status = vm.ApprovalStatus;
                            sameOrder.sam.ModifiedBy = Common.GetUserId();
                            sameOrder.sam.ModifiedDate = DateTime.Now;
                        }
                        else
                        {
                            sameOrder.sam.Status = vm.ApprovalStatus;
                            sameOrder.sam.ModifiedBy = Common.GetUserId();
                        }
                    }
                }
                var immediateHigherSig = context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == vm.LeaveApplicationID && x.OrderBy == (dbModel.OrderBy + 1) && x.TableName == "LeaveApplication").ToList();
                //var AllSignatories = context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == vm.LeaveApplicationID).ToList();
                //if (AllSignatories != null && vm.ApprovalStatus == 2)
                //{
                //    foreach(var higherSig in AllSignatories)
                //    {
                //        higherSig.Status = 2;
                //    }
                //}
                //else 
                if (immediateHigherSig != null && vm.ApprovalStatus == (int)LeaveStatusNewEnum.Approved)
                {
                    foreach (var imSig in immediateHigherSig)
                    {
                        if(Common.GetIntUserId() == Common.GetHRAdminId() && imSig.EmployeeId == Common.GetHRAdminId())
                        {
                            imSig.Status = 1;
                        }
                        else
                        {
                            imSig.Status = 0;
                        }
                        
                    }
                }




                if (dbModel.IsHRAdmin || dbModel.EmployeeId == Common.GetHRAdminId())
                {
                    if (isJointYearLeave)
                    {
                        var nextYearLeaveDays = leaveApplication.EndDate.Day;
                        var previousYearLeaveDays = leaveApplication.LeaveDays - nextYearLeaveDays;
                        var nextYearProcessLeave = context.ProcessLeaves.FirstOrDefault(x => x.IsActive.HasValue && x.IsActive.Value && x.Year == toYear && x.EmployeeId == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                        if (nextYearProcessLeave != null)
                        {
                            nextYearProcessLeave.LeaveBalance -= nextYearLeaveDays;
                            nextYearProcessLeave.LeaveAvailed += nextYearLeaveDays;
                            nextYearProcessLeave.LastLeaveApplicationId = leaveApplication.LeaveApplicationId;
                        }
                        var processLeave = context.ProcessLeaves.FirstOrDefault(x => x.IsActive.HasValue && x.IsActive.Value && x.Year == fromYear && x.EmployeeId == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                        if (processLeave != null)
                        {
                            processLeave.LeaveBalance -= previousYearLeaveDays;
                            processLeave.LeaveAvailed += previousYearLeaveDays;
                            processLeave.LastLeaveApplicationId = leaveApplication.LeaveApplicationId;
                        }

                        //for probation period employee balance deduct
                        var isProbationEmployee = context.Employees.FirstOrDefault(x => x.Active && x.Id == leaveApplication.Id && x.JobStatusId == 41);
                        if (processLeave == null && isProbationEmployee != null)
                        {
                            var leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.IsActive && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                            if (leaveCategory != null)
                            {
                                processLeave = new ProcessLeave()
                                {
                                    Employee = leaveApplication.Id,
                                    EmployeeId = leaveApplication.Id,
                                    LeaveCategory = ((LeaveTypeEnum)(leaveApplication.LeaveCategoryId)).ToString(),
                                    LeaveCategoryId = leaveApplication.LeaveCategoryId,
                                    IsActive = true,
                                    MaxDays = (int)leaveCategory.MaxDays,
                                    TotalLeave = (int)leaveCategory.MaxDays,
                                    CreatedBy = "System",
                                    CreatedDate = DateTime.Now,
                                    LeaveBalance = (int)leaveCategory.MaxDays - leaveApplication.LeaveDays,
                                    LeaveAvailed = leaveApplication.LeaveDays,
                                    LeaveYear = fromYear.ToString(),
                                    Year = fromYear
                                };
                                context.ProcessLeaves.Add(processLeave);
                            }

                        }

                    }
                    if (!isJointYearLeave)
                    {

                        var processLeave = context.ProcessLeaves.FirstOrDefault(x => x.IsActive.HasValue && x.IsActive.Value && x.Year == fromYear && x.EmployeeId == leaveApplication.Id && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                        if (processLeave != null)
                        {
                            processLeave.LeaveBalance -= leaveApplication.LeaveDays;
                            processLeave.LeaveAvailed += leaveApplication.LeaveDays;
                            processLeave.LastLeaveApplicationId = leaveApplication.LeaveApplicationId;
                        }
                        //for probation period employee balance deduct
                        var isProbationEmployee = context.Employees.FirstOrDefault(x => x.Active && x.Id == leaveApplication.Id && x.JobStatusId == 41);
                        if (processLeave == null && isProbationEmployee != null)
                        {
                            var leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.IsActive && x.LeaveCategoryId == leaveApplication.LeaveCategoryId);
                            if (leaveCategory != null)
                            {
                                processLeave = new ProcessLeave()
                                {
                                    Employee = leaveApplication.Id,
                                    EmployeeId = leaveApplication.Id,
                                    LeaveCategory = ((LeaveTypeEnum)(leaveApplication.LeaveCategoryId)).ToString(),
                                    LeaveCategoryId = leaveApplication.LeaveCategoryId,
                                    IsActive = true,
                                    MaxDays = (int)leaveCategory.MaxDays,
                                    TotalLeave = (int)leaveCategory.MaxDays,
                                    CreatedBy = "System",
                                    CreatedDate = DateTime.Now,
                                    LeaveBalance = (int)leaveCategory.MaxDays - leaveApplication.LeaveDays,
                                    LeaveAvailed = leaveApplication.LeaveDays,
                                    LeaveYear = fromYear.ToString(),
                                    Year = fromYear
                                };
                                context.ProcessLeaves.Add(processLeave);
                            }
                        }
                    }

                    if (vm.ApprovalStatus == (int)LeaveStatusNewEnum.Denied)
                    {
                        leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Denied;
                        leaveApplication.Status = (int)StatusEnum.Inactive;

                    }
                    else
                    {
                        leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Approved;
                        leaveApplication.HrAdminStatus = "Approved";
                        leaveApplication.ManagerStatus = "Approved";
                    }

                }
                else
                {
                    if (vm.ApprovalStatus == (int)LeaveStatusNewEnum.Denied)
                    {
                        leaveApplication.LeaveStatus = (int)LeaveStatusEnum.Denied;
                        leaveApplication.Status = (int)StatusEnum.Inactive;

                    }
                }
                if (context.SaveChanges() > 0)
                {

                    if (dbModel.IsHRAdmin || dbModel.EmployeeId == Common.GetHRAdminId())
                    {

                        context.Database.ExecuteSqlCommand(String.Format("spUpdateAttendanceFromLeave {0},{1}", vm.LeaveApplicationID, leaveApplication.Id));

                    }
                }

                result = 1;
            }
            return result;
        }
        public async Task<LeaveApplicationVm> GetLeaveListByEmployee(long EmpID, DateTime? fromDate, DateTime? toDate)
        {
            LeaveApplicationVm model = new LeaveApplicationVm();

            var leaveApplicationIdsByEmp = context.LeaveApplications.Where(la => la.Id == EmpID).Join(context.SignatoryApprovalMaps, la => la.LeaveApplicationId, sam => sam.IntregratedFromId, (la, sam) => la.LeaveApplicationId).Distinct().ToList();
            //var leaveApplicationIdsByEmp = context.LeaveApplications.Where(la => la.Id == EmpID).Join(context.SignatoryApprovalMaps.Where(sam => sam.TableName == "LeaveApplication"),la => la.LeaveApplicationId,sam => sam.IntregratedFromId,(la, sam) => la.LeaveApplicationId).Distinct().ToList();

            var leaveViewModels = (from laId in leaveApplicationIdsByEmp
                                   join la in context.LeaveApplications on laId equals la.LeaveApplicationId
                                   join e in context.Employees on la.Id equals e.Id
                                   join d in context.Designations on e.DesignationId equals d.DesignationId
                                   join lc in context.LeaveCategories on la.LeaveCategoryId equals lc.LeaveCategoryId
                                   where la.Active == 1 && (fromDate == null || la.StartDate >= fromDate)
                                   && (toDate == null || la.EndDate <= toDate)
                                   select new LeaveApplicationVm
                                   {
                                       LeaveStatus = la.LeaveStatus,
                                       LeaveApplicationId = la.LeaveApplicationId,
                                       EmployeeId = e.EmployeeId,
                                       EmployeeName = e.Name,
                                       CategoryName = lc.Name,
                                       Designation = d.Name,
                                       Reason = la.Reason,
                                       LeaveDays = la.LeaveDays,
                                       StartDate = la.StartDate,
                                       EndDate = la.EndDate,
                                       ApplicationDate = la.ApplicationDate,
                                       CompanyId = e.CompanyId ?? 0,
                                       Status = ApplicationStatus(la.LeaveApplicationId),
                                       TableName = "LeaveApplication"
                                   }).OrderByDescending(x => x.LeaveApplicationId).ToList();

            model.LeaveApplications = leaveViewModels;

            return model;
        }
        public string ApplicationStatus(long LeaveApplicationId)
        {
            var status = "";
            var sigs = context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == LeaveApplicationId).Select(x => x.Status).ToList();
            int counter = 0;
            foreach (var sig in sigs)
            {
                if (sig == 1)
                {
                    counter++;
                }
                else if (sig == 2)
                {
                    status = "Rejected";
                    return status;
                }
            }
            if (counter == sigs.Count)
            {
                status = "Approved";
                return status;
            }
            else
            {
                status = "Pending";
                return status;
            }
        }

        #endregion
        #region Check New Manager Is not assigned to Signatory
        public bool CheckNewManagerAddedInSignatory()
        {
            var query = from employee in context.Employees
                        join signatory in context.RequisitionSignatories
                        on employee.Id equals signatory.EmployeeId into signatoryGroup
                        select new
                        {
                            EmpId = employee.Id,
                            EmployeeKgID = employee.EmployeeId,
                            EmployeeName = employee.Name,
                            Signatories = signatoryGroup.Select(s => s.SignatoryEmpId).ToList()
                        };
            var queryResult = query.ToList();

            return true;
        }

        #endregion
        #region bulk
        public List<ManagerApproval> GetManagerLeaveApprovalsNew(string searchText)
        {
            var managerApprovals = new List<ManagerApproval>();
            long id = Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]);

            var myEmployeesApplications = context.SignatoryApprovalMaps
                                                .Where(x => x.EmployeeId == id)
                                                .ToList();

            foreach (var signatoryApprovalMap in myEmployeesApplications)
            {
                var application = context.LeaveApplications.FirstOrDefault(x => x.LeaveApplicationId == signatoryApprovalMap.IntregratedFromId);
                var employee = context.Employees.FirstOrDefault(x => x.Id == application.Id);
                var department = context.Departments.FirstOrDefault(x => x.DepartmentId == employee.DepartmentId);
                var leaveCategory = context.LeaveCategories.FirstOrDefault(x => x.LeaveCategoryId == application.LeaveCategoryId);
                var allSignatories = context.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == application.LeaveApplicationId).ToList();
                var distinctSignatories = context.SignatoryApprovalMaps
    .Where(x => x.IntregratedFromId == application.LeaveApplicationId)
    .GroupBy(x => x.OrderBy)
    .Select(group => group.FirstOrDefault())
    .ToList();
                var maxOrderBy = allSignatories.Max(x => x.OrderBy);
                string[] sigArray = new string[maxOrderBy];
                int index = 0;
                foreach (var sig in distinctSignatories)
                {
                    if (sig.EmployeeId == id && sig.Status == 0)
                    {
                        sigArray[index] = "button";
                    }
                    else if (sig.Status == 0)
                    {
                        sigArray[index] = "Pending";
                    }
                    else if (sig.Status == 1)
                    {
                        sigArray[index] = "Approved";
                    }
                    else if (sig.Status == 2)
                    {
                        sigArray[index] = "Rejected";
                    }
                    else
                    {
                        sigArray[index] = "-";
                    }
                    index++;
                }
                if (application != null && employee != null)
                {
                    var managerApproval = new ManagerApproval
                    {
                        LeaveApplications = new List<LeaveApplication> { application },
                        SignatoryApprovalMaps = new List<SignatoryApprovalMap> { signatoryApprovalMap },
                        Employees = new List<Employee> { employee },
                        Departments = new List<Department> { department },
                        LeaveCategories = new List<LeaveCategory> { leaveCategory },
                        AllSignatories = allSignatories,
                        CurrentSignatoryID = id,
                        MaxOrderBy = maxOrderBy,
                        SigArray = sigArray
                    };
                    managerApprovals.Add(managerApproval);
                }
            }

            return managerApprovals;
        }
        #endregion
    }
}