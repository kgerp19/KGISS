using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class ApplicationManageService : IApplicationManageService
    {
        private ERPEntities context;

        public ApplicationManageService(ERPEntities db)
        {
            this.context = db;
        }

        public async Task<long> SaveOrderCreditLimitApplication(ApplicationManageModel model, string username, long employeeId)
        {
            var applicationManage = new ApplicationManage
            {
                ApplicantId = model.ApplicantId,
                ManagerId = model.ManagerId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DayCounts = (model.EndDate - model.StartDate).Days + 1,
                Reason = model.Reason,
                Remarks = model.Remarks ?? string.Empty,
                ApplicationDate = DateTime.Now,
                Status = 0, // 0 = Pending
                IsActive = true,
                IsSubmitted = true,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            context.ApplicationManages.Add(applicationManage);
            await context.SaveChangesAsync();

            // Fetch signatories for OrderCreditLimit
            var signatories = await context.RequisitionSignatories
                .Where(x => x.IntegrateWith == "OrderCreditLimit" && x.IsActive)
                .OrderBy(x => x.OrderBy)
                .ToListAsync();

            if (signatories.Any())
            {
                var approvals = new List<RequisitionSignatoryApproval>();
                foreach (var signatory in signatories)
                {
                    approvals.Add(new RequisitionSignatoryApproval
                    {
                        RequisitionSignatoryId = signatory.RequisitionSignatoryId,
                        RequisitionId = applicationManage.ApplicationId,
                        EmployeeId = signatory.SignatoryEmpId, // Signatory person
                        OrderBy = signatory.OrderBy,
                        Status = 0, // Pending
                        IsActive = true,
                        CreatedBy = username,
                        CreatedDate = DateTime.Now
                    });
                }
                context.RequisitionSignatoryApprovals.AddRange(approvals);
                await context.SaveChangesAsync();
            }

            return applicationManage.ApplicationId;
        }

        public async Task<ApplicationManageModel> GetOrderCreditLimitApplications(int companyId, long applicationId = 0)
        {
            var model = new ApplicationManageModel();
            model.CompanyId = companyId;
            model.ApplicationId = applicationId;

            var applicationsQuery = from a in context.ApplicationManages
                                    join v in context.Vendors on a.ApplicantId equals v.VendorId
                                    join e in context.Employees on a.ManagerId equals e.Id
                                    where a.IsActive
                                    select new ApplicationManageModel
                                    {
                                        ApplicationId = a.ApplicationId,
                                        ApplicantId = a.ApplicantId,
                                        ApplicantName = v.Name,
                                        ManagerId = a.ManagerId,
                                        ManagerName = e.Name,
                                        Reason = a.Reason,
                                        ApplicationDate = a.ApplicationDate,
                                        StartDate = a.StartDate,
                                        EndDate = a.EndDate,
                                        DayCounts = a.DayCounts,
                                        Status = a.Status
                                    };

            if (applicationId > 0)
            {
                applicationsQuery = applicationsQuery.Where(x => x.ApplicationId == applicationId);
            }
            else
            {
                // Ensure list is empty if no explicit app id is provided for the entry page
                applicationsQuery = applicationsQuery.Where(x => false);
            }

            var applicationsList = await applicationsQuery.OrderByDescending(x => x.ApplicationId).ToListAsync();

            var allRequisitionIds = applicationsList.Select(x => x.ApplicationId).ToList();

            var signatoryApprovals = await (from app in context.RequisitionSignatoryApprovals
                                            join emp in context.Employees on app.EmployeeId equals emp.Id
                                            where app.IsActive && allRequisitionIds.Contains(app.RequisitionId)
                                            select new SignatoryApprovalModel
                                            {
                                                RequisitionId = app.RequisitionId,
                                                EmployeeName = emp.Name,
                                                DesignationName = context.Designations.Where(d => d.DesignationId == emp.DesignationId).Select(d => d.Name).FirstOrDefault(),
                                                OrderBy = app.OrderBy,
                                                Status = app.Status,
                                                Comment = app.Comment
                                            }).ToListAsync();

            foreach (var app in applicationsList)
            {
                app.SignatoryApprovals = signatoryApprovals.Where(x => x.RequisitionId == app.ApplicationId).OrderBy(x => x.OrderBy).ToList();
            }

            model.ApplicationList = applicationsList;
            return model;
        }

        public async Task<ApplicationManageModel> GetPendingApprovals(int companyId, long employeeId)
        {
            var model = new ApplicationManageModel();
            model.CompanyId = companyId;

            // Find all pending approvals for the current user
            var userPendingApprovals = await context.RequisitionSignatoryApprovals
                .Where(x => x.EmployeeId == employeeId && x.Status == 0 && x.IsActive)
                .ToListAsync();

            var pendingAppIdsContext = new List<long>();

            // Check if it's actually their turn. It's their turn if all previous signatories for the RequisitionId have Approved (Status 1)
            foreach (var userApp in userPendingApprovals)
            {
                var previousSteps = await context.RequisitionSignatoryApprovals
                    .Where(x => x.RequisitionId == userApp.RequisitionId && x.OrderBy < userApp.OrderBy && x.IsActive)
                    .ToListAsync();

                if (previousSteps.All(x => x.Status == 1))
                {
                    pendingAppIdsContext.Add(userApp.RequisitionId);
                }
            }

            var applicationsList = await (from a in context.ApplicationManages
                                          join v in context.Vendors on a.ApplicantId equals v.VendorId
                                          join e in context.Employees on a.ManagerId equals e.Id
                                          where a.IsActive && pendingAppIdsContext.Contains(a.ApplicationId)
                                          orderby a.ApplicationId descending
                                          select new ApplicationManageModel
                                          {
                                              ApplicationId = a.ApplicationId,
                                              ApplicantId = a.ApplicantId,
                                              ApplicantName = v.Name,
                                              ManagerId = a.ManagerId,
                                              ManagerName = e.Name,
                                              Reason = a.Reason,
                                              ApplicationDate = a.ApplicationDate,
                                              Status = a.Status
                                          }).ToListAsync();

            // Fetch the specific signatory approval ID for the user to act upon
            foreach (var app in applicationsList)
            {
                var appStep = userPendingApprovals.FirstOrDefault(x => x.RequisitionId == app.ApplicationId);
                if (appStep != null)
                {
                    app.SignatoryApprovals = new List<SignatoryApprovalModel>
                    {
                        new SignatoryApprovalModel
                        {
                            RequisitionSignatoryApprovalId = appStep.RequisitionSignatoryApprovalId
                        }
                    };
                }
            }

            model.ApplicationList = applicationsList;
            return model;
        }

        public async Task<bool> UpdateApprovalStatus(long approvalId, int status, string comment, string username)
        {
            var approvalRecord = await context.RequisitionSignatoryApprovals.FindAsync(approvalId);
            if (approvalRecord == null) return false;

            approvalRecord.Status = status;
            approvalRecord.Comment = comment;
            approvalRecord.ModifiedBy = username;
            approvalRecord.ModifiedDate = DateTime.Now;

            var applicationRecord = await context.ApplicationManages.FindAsync(approvalRecord.RequisitionId);
            if (applicationRecord != null)
            {
                if (status == 2) // Reject
                {
                    applicationRecord.Status = 2; // Reject main record
                    applicationRecord.ModifiedBy = username;
                    applicationRecord.ModifedDate = DateTime.Now;
                }
                else if (status == 1) // Approve
                {
                    // Check if there are any pending or upcoming approvals left
                    bool isLastStep = !await context.RequisitionSignatoryApprovals
                        .AnyAsync(x => x.RequisitionId == approvalRecord.RequisitionId && x.OrderBy > approvalRecord.OrderBy && x.IsActive);

                    if (isLastStep)
                    {
                        applicationRecord.Status = 1; // Approve main record
                        applicationRecord.ModifiedBy = username;
                        applicationRecord.ModifedDate = DateTime.Now;
                    }
                }
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<ApplicationManageModel> GetOrderCreditLimitManageList(int companyId, int? searchStatus, DateTime? fromDate, DateTime? toDate)
        {
            var model = new ApplicationManageModel();
            model.CompanyId = companyId;
            model.SearchStatus = searchStatus;

            var applicationsQuery = from a in context.ApplicationManages
                                    join v in context.Vendors on a.ApplicantId equals v.VendorId
                                    join e in context.Employees on a.ManagerId equals e.Id
                                    where a.IsActive
                                    select new ApplicationManageModel
                                    {
                                        ApplicationId = a.ApplicationId,
                                        ApplicantId = a.ApplicantId,
                                        ApplicantName = v.Name,
                                        ManagerId = a.ManagerId,
                                        ManagerName = e.Name,
                                        Reason = a.Reason,
                                        ApplicationDate = a.ApplicationDate,
                                        StartDate = a.StartDate,
                                        EndDate = a.EndDate,
                                        DayCounts = a.DayCounts,
                                        Status = a.Status
                                    };

            if (searchStatus.HasValue)
            {
                applicationsQuery = applicationsQuery.Where(x => x.Status == searchStatus.Value);
            }

            if (fromDate.HasValue)
            {
                applicationsQuery = applicationsQuery.Where(x => x.ApplicationDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                // Ensure toDate includes the entire day
                applicationsQuery = applicationsQuery.Where(x => x.ApplicationDate <= toDate.Value);
            }

            var applicationsList = await applicationsQuery.OrderByDescending(x => x.ApplicationId).ToListAsync();

            var allRequisitionIds = applicationsList.Select(x => x.ApplicationId).ToList();

            var signatoryApprovals = await (from app in context.RequisitionSignatoryApprovals
                                            join emp in context.Employees on app.EmployeeId equals emp.Id
                                            where app.IsActive && allRequisitionIds.Contains(app.RequisitionId)
                                            select new SignatoryApprovalModel
                                            {
                                                RequisitionId = app.RequisitionId,
                                                EmployeeName = emp.Name,
                                                DesignationName = context.Designations.Where(d => d.DesignationId == emp.DesignationId).Select(d => d.Name).FirstOrDefault(),
                                                OrderBy = app.OrderBy,
                                                Status = app.Status,
                                                Comment = app.Comment
                                            }).ToListAsync();

            foreach (var app in applicationsList)
            {
                app.SignatoryApprovals = signatoryApprovals.Where(x => x.RequisitionId == app.ApplicationId).OrderBy(x => x.OrderBy).ToList();
            }

            model.ApplicationList = applicationsList;
            return model;
        }
    }
}
