using KGERP.Data.Models;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.Implementation.ApprovalSystemService
{
    public class Approval_Service : IApproval_Service
    {
        ERPEntities context = new ERPEntities();

        public async Task<ApprovalSystemViewModel> AddApproval(ApprovalSystemViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                ReportApproval approval = new ReportApproval();
                try
                {
                    approval.ReportCategoryId = model.ReportCategoryId;
                    approval.Month = model.Month;
                    approval.Year = model.Year;
                    approval.FinalStatus = 1;
                    approval.CompanyId = model.CompanyId;
                    approval.CreatedDate = DateTime.Now;
                    approval.IsActive = true;
                    approval.IsSubmitted = false;
                    approval.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    context.ReportApprovals.Add(approval);
                    context.SaveChanges();
                    var signatory = context.Accounting_Signatory.Where(x => x.CompanyId == model.CompanyId && x.IsActive).OrderBy(c => c.OrderBy).ToList(); ;
                    List<ReportApprovalDetail> approvalDetails = new List<ReportApprovalDetail>();
                    int orderno = 0;
                    foreach (var item in signatory)
                    {
                        orderno++;
                        ReportApprovalDetail report = new ReportApprovalDetail();
                        report.ReportApprovalId = approval.ReportApprovalId;
                        report.Accounting_SignatoryId = item.SignatoryId;
                        report.ApprovalFor = item.EmployeeId;
                        report.ApprovalStatus = 1;
                        report.SignatoryOrderNo = orderno;
                        report.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        report.CreatedDate = DateTime.Now;
                        report.IsActive = true;
                        approvalDetails.Add(report);
                    }
                    context.ReportApprovalDetails.AddRange(approvalDetails);
                    context.SaveChanges();
                    scope.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }

        public async Task<ApprovalSystemViewModel> ApprovalList(int companyId, int Years, int Month)
        {
            ApprovalSystemViewModel Model = new ApprovalSystemViewModel();
            Model.CompanyId = companyId;
            Model.datalist = await Task.Run(() => (from t1 in context.ReportApprovals.Where(d => d.CompanyId == companyId && d.IsActive == true && d.Year == Years && d.Month == Month)
                                                   join t5 in context.ReportCategories on t1.ReportCategoryId equals t5.ReportCategoryId
                                                   join t6 in context.Employees on t1.CreatedBy equals t6.EmployeeId
                                                   select new ApprovalSystemViewModel
                                                   {
                                                       CompanyId = (int)t1.CompanyId,
                                                       Year = t1.Year,
                                                       Month = t1.Month,
                                                       ReportCategoryId = t1.ReportCategoryId,
                                                       ReportCategoryName = t5.Name,
                                                       FinalStatus = t1.FinalStatus,
                                                       CreatedBy = t6.Name,
                                                       CreatedDate = t1.CreatedDate.Value,
                                                       ReportApprovalId = t1.ReportApprovalId,
                                                       Issubmited = t1.IsSubmitted,
                                                       ReportName = t5.ReportName,
                                                       ReportGroup = t5.ReportGroup
                                                   }).OrderByDescending(x => x.CreatedDate).ToListAsync());

            return Model;
        }

        public async Task<ApprovalSystemViewModel> ApprovalSignetory(long id)
        {
            ApprovalSystemViewModel Model = new ApprovalSystemViewModel();
            Model.datalist = await Task.Run(() => (from t1 in context.ReportApprovals.Where(d => d.ReportApprovalId == id)
                                                   join t5 in context.ReportCategories on t1.ReportCategoryId equals t5.ReportCategoryId
                                                   join t2 in context.ReportApprovalDetails on t1.ReportApprovalId equals t2.ReportApprovalId
                                                   join t3 in context.Accounting_Signatory on t2.Accounting_SignatoryId equals t3.SignatoryId
                                                   join t4 in context.Employees on t3.EmployeeId equals t4.Id
                                                   join t6 in context.Employees on t1.CreatedBy equals t6.EmployeeId
                                                   select new ApprovalSystemViewModel
                                                   {
                                                       CompanyId = (int)t1.CompanyId,
                                                       Year = t1.Year,
                                                       Month = t1.Month,
                                                       ReportCategoryId = t1.ReportCategoryId,
                                                       ReportCategoryName = t5.Name,
                                                       FinalStatus = t1.FinalStatus,
                                                       ApprovalStatus = t2.ApprovalStatus,
                                                       ApprovalStatusName = t2.ApprovalStatus == 1 ? "....." : t2.ApprovalStatus == 2 ? "Pending" : t2.ApprovalStatus == 3 ? "Approved" : t2.ApprovalStatus == 4 ? "Rejected" : "",
                                                       EmployeeId = t3.EmployeeId,
                                                       EmployeeName = t3.SignatoryName,
                                                       CreatedBy = t6.Name,
                                                       CreatedDate = t1.CreatedDate.Value,
                                                       Approvdate = t2.ApprovedDate.Value,
                                                       //StringApprovdate = t2.ApprovedDate.Value.ToShortDateString(),
                                                       OrderNo = t2.SignatoryOrderNo,
                                                       FrowardId = t2.ForwardedId.Value
                                                   }).OrderBy(x => x.OrderNo).ToListAsync());

            return Model;

        }

        public async Task<long> AccApprovalStutasUpdate(long id)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    var res = await context.ReportApprovals.FirstOrDefaultAsync(f => f.ReportApprovalId == id);
                    res.IsSubmitted = true;
                    res.FinalStatus = 2;
                    res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    res.ModifiedDate = DateTime.Now;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                    var count = context.ReportApprovalDetails.Where(f => f.ReportApprovalId == id).Max(d => d.SignatoryOrderNo);
                    var result = context.ReportApprovalDetails.Where(f => f.SignatoryOrderNo == count && f.ReportApprovalId == id).FirstOrDefault();
                    result.ForwardedId = count;
                    result.ApprovalStatus = 3;
                    result.ApprovedDate = DateTime.Now;
                    result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    result.ModifiedDate = DateTime.Now;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                    var count2 = context.ReportApprovalDetails.Where(f => f.ReportApprovalId == id).Max(d => (d.SignatoryOrderNo-1));
                    var result2 = context.ReportApprovalDetails.Where(f => f.SignatoryOrderNo == count2 && f.ReportApprovalId == id).FirstOrDefault();
                    result2.ForwardedId = count2;
                    result2.ApprovalStatus = 2;
                    result2.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    result2.ModifiedDate = DateTime.Now;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }


        }

        public async Task<long> SalaryAccApprovalStutasUpdate(long id)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    var res = await context.PRollApprovals.FirstOrDefaultAsync(f => f.PRollApprovalId == id);
                    res.IsSubmitted = true;
                    res.FinalStatus = 2;
                    res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    res.ModifiedDate = DateTime.Now;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                    var count = context.PRollApprovalDetails.Where(f => f.PRollApprovalId == id).Max(d => d.SignatoryOrderNo);
                    var result = context.PRollApprovalDetails.Where(f => f.SignatoryOrderNo == count && f.PRollApprovalId == id).FirstOrDefault();
                    result.ForwardedId = count;
                    result.ApprovalStatus = 3;
                    result.ApprovedDate = DateTime.Now;
                    result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    result.ModifiedDate = DateTime.Now;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                    var count2 = context.PRollApprovalDetails.Where(f => f.PRollApprovalId == id).Max(d => (d.SignatoryOrderNo - 1));
                    var result2 = context.PRollApprovalDetails.Where(f => f.SignatoryOrderNo == count2 && f.PRollApprovalId == id).FirstOrDefault();
                    result2.ForwardedId = count2;
                    result2.ApprovalStatus = 2;
                    result2.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    result2.ModifiedDate = DateTime.Now;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }


        }
        public async Task<bool> CheckApproval(ApprovalSystemViewModel model)
        {
            var res = await context.ReportApprovals.FirstOrDefaultAsync(d => d.Year == model.Year && d.Month == model.Month && d.ReportCategoryId == model.ReportCategoryId && d.FinalStatus != 4 && d.IsActive == true);
            if (res == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckApprovalsalarySheet(ApprovalSystemViewModel model)
        {
            var res = await context.PRollApprovals.FirstOrDefaultAsync(d =>d.CompanyId==model.CompanyId&& d.Year == model.Year && d.Month == model.Month&& d.FinalStatus != 4 && d.IsActive == true && d.IsFestivalBonus == model.IsFestivalBonus);
            if (res == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<SelectDDLModel>> ReportcatagoryLit(int companyId)
        {
            List<SelectDDLModel> models = new List<SelectDDLModel>();
            var list = await context.ReportCategories.Where(g => g.CompanyId == companyId).ToListAsync();
            foreach (var c in list)
            {
                SelectDDLModel model = new SelectDDLModel();
                model.Text = c.Name;
                model.Value = c.ReportCategoryId;
                models.Add(model);
            }
            return models;
        }

        public List<SelectModel> YearsListLit()
        {
            List<SelectModel> models = new List<SelectModel>();
            for (var i = 2022; i < 2050; i++)
            {
                SelectModel model = new SelectModel();
                model.Text = i.ToString();
                model.Value = i;
                models.Add(model);
            }
            return models;
        }

        public async Task<long> ApprovalDelete(long id)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    var res = await context.ReportApprovals.FirstOrDefaultAsync(f => f.ReportApprovalId == id);
                    res.IsActive = false;
                    res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    res.ModifiedDate = DateTime.Now;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                    var countlist = context.ReportApprovalDetails.Where(f => f.ReportApprovalId == id).ToList();
                    foreach (var item in countlist)
                    {
                        item.IsActive = false;
                        item.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        item.ModifiedDate = DateTime.Now;
                        context.Entry(res).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    scope.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public async Task<ApprovalSystemViewModel> ApprovalforEmployeeList(long Employee, int companyId, int Years, int Month)
        {
            ApprovalSystemViewModel Model = new ApprovalSystemViewModel();
            Model.datalist = await Task.Run(() => (from t1 in context.ReportApprovalDetails.Where(d => d.ApprovalFor == Employee)
                                                   join t2 in context.ReportApprovals.Where(d => d.CompanyId == companyId && d.Month == Month && d.Year == Years && d.IsSubmitted == true) on t1.ReportApprovalId equals t2.ReportApprovalId
                                                   join t3 in context.ReportCategories on t2.ReportCategoryId equals t3.ReportCategoryId
                                                   join t4 in context.Accounting_Signatory on t1.Accounting_SignatoryId equals t4.SignatoryId
                                                   join t5 in context.Employees on t2.CreatedBy equals t5.EmployeeId

                                                   select new ApprovalSystemViewModel
                                                   {
                                                       CompanyId = (int)t2.CompanyId,
                                                       Year = t2.Year,
                                                       Month = t2.Month,
                                                       ReportCategoryId = t2.ReportCategoryId,
                                                       ReportCategoryName = t3.Name,
                                                       FinalStatus = t2.FinalStatus,
                                                       ApprovalStatus = t1.ApprovalStatus,
                                                       ApprovalStatusName = t1.ApprovalStatus == 1 ? "......" : t1.ApprovalStatus == 2 ? "Pending" : t1.ApprovalStatus == 3 ? "Approved" : t1.ApprovalStatus == 4 ? "Reject" : "",
                                                       EmployeeId = t4.EmployeeId,
                                                       EmployeeName = t4.SignatoryName,
                                                       CreatedBy = t5.Name,
                                                       CreatedDate = t2.CreatedDate.Value,
                                                       Approvdate = t1.ApprovedDate.Value,
                                                       OrderNo = t1.SignatoryOrderNo,
                                                       FrowardId = t1.ForwardedId.Value,
                                                       ApprovalFor = t1.ApprovalFor,
                                                       ReportApprovalDetalisId = t1.ReportApprovalDetail1,
                                                       ReportApprovalId = t2.ReportApprovalId,
                                                       ReportName = t3.ReportName,
                                                       ReportGroup = t3.ReportGroup
                                                   }).OrderByDescending(x => x.CreatedDate).ToListAsync());

            return Model;

        }
        public async Task<ApprovalSystemViewModel> Approvalformanagment(ApprovalSystemViewModel model)
        {
            model.datalist = await Task.Run(() => (from t1 in context.ReportApprovalDetails.Where(d => d.ApprovalFor == model.SectionEmployeeId)
                                                   join t2 in context.ReportApprovals.Where(d => d.CompanyId == model.CompanyId && d.Month == model.Month && d.Year == model.Year && d.IsSubmitted == true) on t1.ReportApprovalId equals t2.ReportApprovalId

                                                   join t3 in context.ReportCategories on t2.ReportCategoryId equals t3.ReportCategoryId
                                                   join t4 in context.Accounting_Signatory on t1.Accounting_SignatoryId equals t4.SignatoryId
                                                   join t5 in context.Employees on t2.CreatedBy equals t5.EmployeeId

                                                   select new ApprovalSystemViewModel
                                                   {
                                                       CompanyId = (int)t2.CompanyId,
                                                       Year = t2.Year,
                                                       Month = t2.Month,
                                                       ReportCategoryId = t2.ReportCategoryId,
                                                       ReportCategoryName = t3.Name,
                                                       FinalStatus = t2.FinalStatus,
                                                       ApprovalStatus = t1.ApprovalStatus,
                                                       ApprovalStatusName = t1.ApprovalStatus == 1 ? "......" : t1.ApprovalStatus == 2 ? "Pending" : t1.ApprovalStatus == 3 ? "Approved" : t1.ApprovalStatus == 4 ? "Reject" : "",
                                                       EmployeeId = t4.EmployeeId,
                                                       EmployeeName = t4.SignatoryName,
                                                       CreatedBy = t5.Name,
                                                       CreatedDate = t2.CreatedDate.Value,
                                                       Approvdate = t1.ApprovedDate.Value,
                                                       OrderNo = t1.SignatoryOrderNo,
                                                       FrowardId = t1.ForwardedId.Value,
                                                       ApprovalFor = t1.ApprovalFor,
                                                       ReportApprovalDetalisId = t1.ReportApprovalDetail1,
                                                       ReportApprovalId = t2.ReportApprovalId,
                                                       ReportName = t3.ReportName,
                                                       ReportGroup = t3.ReportGroup
                                                   }).OrderByDescending(x => x.CreatedDate).ToListAsync());

            return model;

        }
        public async Task<ApprovalSystemViewModel> AduitApprovalformanagment(ApprovalSystemViewModel model)
        {
            model.datalist = await Task.Run(() => (from t2 in context.ReportApprovals.Where(d => d.CompanyId == model.CompanyId && d.Month == model.Month && d.Year == model.Year && d.IsSubmitted == true)                                                  
                                                   join t3 in context.ReportCategories on t2.ReportCategoryId equals t3.ReportCategoryId                                                   
                                                   join t5 in context.Employees on t2.CreatedBy equals t5.EmployeeId
                                                   select new ApprovalSystemViewModel
                                                   {
                                                       CompanyId = (int)t2.CompanyId,
                                                       Year = t2.Year,
                                                       Month = t2.Month,
                                                       ReportCategoryId = t2.ReportCategoryId,
                                                       ReportCategoryName = t3.Name,
                                                       FinalStatus = t2.FinalStatus,
                                                       CreatedBy = t5.Name,
                                                       CreatedDate = t2.CreatedDate.Value,
                                                       ReportApprovalId = t2.ReportApprovalId,
                                                       ReportName = t3.ReportName,
                                                       ReportGroup = t3.ReportGroup
                                                   }).OrderByDescending(x => x.CreatedDate).ToListAsync());

            return model;

        }
        public async Task<ApprovalSystemViewModel> AccStatusChange(ApprovalSystemViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    var changeStutas = await context.ReportApprovalDetails.FirstOrDefaultAsync(x => x.ReportApprovalDetail1 == model.ReportApprovalDetalisId);
                    changeStutas.ApprovalStatus = model.ApprovalStatus;
                    changeStutas.ApprovedDate = DateTime.Now;
                    changeStutas.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    changeStutas.ModifiedDate = DateTime.Now;
                    context.Entry(changeStutas).State = EntityState.Modified;
                    context.SaveChanges();

                    if (changeStutas.ApprovalStatus != 4)
                    {
                        if (changeStutas.SignatoryOrderNo != 1)
                        {
                            int slno = changeStutas.SignatoryOrderNo - 1;
                            var Stutas = await context.ReportApprovalDetails.FirstOrDefaultAsync(x => x.SignatoryOrderNo == slno && x.ReportApprovalId == model.ReportApprovalId);
                            Stutas.ForwardedId = changeStutas.SignatoryOrderNo - 1;
                            Stutas.ApprovalStatus = 2;
                            context.Entry(Stutas).State = EntityState.Modified;
                            context.SaveChanges();
                        }

                    }
                    var res = await context.ReportApprovals.FirstOrDefaultAsync(f => f.ReportApprovalId == changeStutas.ReportApprovalId);
                    var countlist = await context.ReportApprovalDetails.Where(d => d.ReportApprovalId == res.ReportApprovalId && d.ApprovalStatus != 4).ToListAsync();
                    if (model.ApprovalStatus == 4)
                    {
                        res.FinalStatus = 4;
                        res.IsActive = false;
                        res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        res.ModifiedDate = DateTime.Now;
                        context.Entry(res).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        var count = countlist.Count();
                        var approvedcount = countlist.Where(f => f.ApprovalStatus == 3).Count();
                        if (changeStutas.SignatoryOrderNo == 1 && changeStutas.ApprovalStatus == 3 && count == approvedcount)
                        {
                            res.FinalStatus = 3;
                            res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            res.ModifiedDate = DateTime.Now;
                            context.Entry(res).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    scope.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    return model;
                }

            }
        }
        public async Task<ApprovalSystemViewModel> SalarySheetAccStatusChange(ApprovalSystemViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    var changeStutas = await context.PRollApprovalDetails.FirstOrDefaultAsync(x => x.PRollApprovalDetailId == model.ReportApprovalDetalisId);
                    changeStutas.ApprovalStatus = model.ApprovalStatus;
                    changeStutas.ApprovedDate = DateTime.Now;
                    changeStutas.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    changeStutas.ModifiedDate = DateTime.Now;
                    context.Entry(changeStutas).State = EntityState.Modified;
                    context.SaveChanges();

                    if (changeStutas.ApprovalStatus != 4)
                    {
                        if (changeStutas.SignatoryOrderNo != 1)
                        {
                            int slno = changeStutas.SignatoryOrderNo - 1;
                            var Stutas = await context.PRollApprovalDetails.FirstOrDefaultAsync(x => x.SignatoryOrderNo == slno && x.PRollApprovalId == model.ReportApprovalId);
                            Stutas.ForwardedId = changeStutas.SignatoryOrderNo - 1;
                            Stutas.ApprovalStatus = 2;
                            context.Entry(Stutas).State = EntityState.Modified;
                            context.SaveChanges();
                        }

                    }
                    var res = await context.PRollApprovals.FirstOrDefaultAsync(f => f.PRollApprovalId == changeStutas.PRollApprovalId);
                    var countlist = await context.PRollApprovalDetails.Where(d => d.PRollApprovalId == res.PRollApprovalId && d.ApprovalStatus != 4).ToListAsync();
                    if (model.ApprovalStatus == 4)
                    {
                        res.FinalStatus = 4;
                        res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        res.ModifiedDate = DateTime.Now;
                        context.Entry(res).State = EntityState.Modified;
                        context.SaveChanges();

                        PRoll_PayRoll payRoll = context.PRoll_PayRoll.Where(x => x.CompanyId == model.CompanyId
                                                     && x.Month == model.Month && x.Year == model.Year && x.IsFestivalBonus == model.IsFestivalBonus).FirstOrDefault();
                        payRoll.IsClose = false;
                        context.Entry(payRoll).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        var count = countlist.Count();
                        var approvedcount = countlist.Where(f => f.ApprovalStatus == 3).Count();
                        if (changeStutas.SignatoryOrderNo == 1 && changeStutas.ApprovalStatus == 3 && count == approvedcount)
                        {
                            res.FinalStatus = 3;
                            res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            res.ModifiedDate = DateTime.Now;
                            context.Entry(res).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    scope.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    return model;
                }

            }
        }
        public async Task<ApprovalSystemViewModel> AccountingApprovalList(ApprovalSystemViewModel model)

        {
            var list = await context.ReportApprovals.Where(d => d.CompanyId == model.CompanyId && d.IsActive == true).ToListAsync();
            if (model.ReportCategoryId != 0)
            {
                list = list.Where(d => d.ReportCategoryId == model.ReportCategoryId).ToList();
            }
            if (model.Year != 0)
            {
                list = list.Where(d => d.Year == model.Year).ToList();
            }
            if (model.Month != 0)
            {
                list = list.Where(d => d.Month == model.Month).ToList();
            }
            model.datalist = (from t1 in list
                             // join t2 in context.ReportApprovalDetails on t1.ReportApprovalId equals t2.ReportApprovalId
                              join t5 in context.ReportCategories on t1.ReportCategoryId equals t5.ReportCategoryId
                              join t6 in context.Employees on t1.CreatedBy equals t6.EmployeeId

                              select new ApprovalSystemViewModel
                              {
                                  CompanyId = (int)t1.CompanyId,
                                  Year = t1.Year,
                                  Month = t1.Month,
                                  ReportCategoryId = t1.ReportCategoryId,
                                  ReportCategoryName = t5.Name,
                                  FinalStatus = t1.FinalStatus,
                                  CreatedBy = t6.Name,
                                  CreatedDate = t1.CreatedDate.Value,
                                  ReportApprovalId = t1.ReportApprovalId,
                                  Issubmited = t1.IsSubmitted,
                                  ReportName = t5.ReportName,
                                  ReportGroup=t5.ReportGroup,
                                  //ApprovalFor=t2.ApprovalFor,
                              }).OrderByDescending(x => x.Year).ToList();

            return model;
        }

        public async Task<ApprovalSystemViewModel> SalarySheetApprovalList(int companyId)
        {
            ApprovalSystemViewModel Model = new ApprovalSystemViewModel();
            Model.CompanyId = companyId;

            var approvalList = await Task.Run(() => (from t1 in context.PRollApprovals
                                                     where t1.CompanyId == companyId && t1.IsActive == true
                                                     join t6 in context.Employees on t1.CreatedBy equals t6.EmployeeId
                                                     select new ApprovalSystemViewModel
                                                     {
                                                         CompanyId = (int)t1.CompanyId,
                                                         Year = t1.Year,
                                                         Month = t1.Month,
                                                         FinalStatus = t1.FinalStatus,
                                                         CreatedBy = t6.Name,
                                                         CreatedDate = t1.CreatedDate.Value,
                                                         ReportApprovalId = t1.PRollApprovalId,
                                                         Issubmited = t1.IsSubmitted,
                                                         PayarolllId = 0 ,
                                                         IsFestivalBonus=t1.IsFestivalBonus
                                                     }).OrderByDescending(x => x.CreatedDate).ToListAsync());

            foreach (var item in approvalList)
            {
                item.PayarolllId = context.PRoll_PayRoll.Where(f => f.IsFestivalBonus == item.IsFestivalBonus && f.CompanyId == companyId && f.Month == item.Month && f.Year == item.Year).ToList().LastOrDefault().PayRollId;
            }

            Model.datalist = approvalList;

            return Model;
        }
        public async Task<ApprovalSystemViewModel> SalarySheetList(ApprovalSystemViewModel model2)
        {
            ApprovalSystemViewModel Model = new ApprovalSystemViewModel();
            Model.CompanyId = model2.CompanyId;

            var approvalList = await Task.Run(() => (from t1 in context.PRollApprovals

                                                     where  t1.IsActive == true && t1.Month == model2.Month && t1.Year == model2.Year
                                                     join t6 in context.Employees on t1.CreatedBy equals t6.EmployeeId
                                                     join t7 in context.Companies on t1.CompanyId equals t7.CompanyId
                                                     select new ApprovalSystemViewModel
                                                     {
                                                         CompanyId = (int)t1.CompanyId,
                                                         Year = t1.Year,
                                                         CompanyName=t7.Name,
                                                         Month = t1.Month,
                                                         FinalStatus = t1.FinalStatus,
                                                         CreatedBy = t6.Name,
                                                         CreatedDate = t1.CreatedDate.Value,
                                                         ReportApprovalId = t1.PRollApprovalId,
                                                         Issubmited = t1.IsSubmitted,
                                                         PayarolllId = 0,
                                                         IsFestivalBonus= t1.IsFestivalBonus
                                                     }).OrderByDescending(x => x.CreatedDate).ToListAsync());

            




            foreach (var item in approvalList)
            {
                item.PayarolllId = context.PRoll_PayRoll.Where( f => f.CompanyId==item.CompanyId && f.Month == item.Month && f.Year == item.Year).ToList().LastOrDefault().PayRollId;
            }

            Model.datalist = approvalList;

            return Model;
        }

        public async Task<ApprovalSystemViewModel> AddSalarySheetApproval(ApprovalSystemViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                PRollApproval approval = new PRollApproval();
                try
                {
                    approval.ReportCategoryId = 0;
                    approval.Month = model.Month;
                    approval.Year = model.Year;
                    approval.FinalStatus = 1;
                    approval.CompanyId = model.CompanyId;
                    approval.CreatedDate = DateTime.Now;
                    approval.IsActive = true;
                    approval.IsSubmitted = false;
                    approval.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    if(model.IsFestivalBonus)
                    {
                        approval.IsFestivalBonus = true;
                    }
                    context.PRollApprovals.Add(approval);
                    context.SaveChanges();
                    var signatory = context.PayRoll_Signatory.Where(x => x.CompanyId == model.CompanyId && x.IsActive).OrderBy(c => c.OrderBy).ToList(); ;
                    List<PRollApprovalDetail> approvalDetails = new List<PRollApprovalDetail>();
                    int orderno = 0;
                    foreach (var item in signatory)
                    {
                        orderno++;
                        PRollApprovalDetail report = new PRollApprovalDetail();
                        report.PRollApprovalId = approval.PRollApprovalId;
                        report.Accounting_SignatoryId = item.SignatoryId;
                        report.ApprovalFor = item.EmployeeId;
                        report.ApprovalStatus = 1;
                        report.SignatoryOrderNo = orderno;
                        report.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        report.CreatedDate = DateTime.Now;
                        report.IsActive = true;
                        approvalDetails.Add(report);
                    }
                    context.PRollApprovalDetails.AddRange(approvalDetails);
                    context.SaveChanges();
                    scope.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }

        public async Task<ApprovalSystemViewModel> SalarySheetApprovalSignetory(long id)
        {
            ApprovalSystemViewModel Model = new ApprovalSystemViewModel();
            Model.datalist = await Task.Run(() => (from t1 in context.PRollApprovals.Where(d => d.PRollApprovalId == id)
                                                   join t2 in context.PRollApprovalDetails on t1.PRollApprovalId equals t2.PRollApprovalId
                                                   join t3 in context.PayRoll_Signatory on t2.Accounting_SignatoryId equals t3.SignatoryId
                                                   join t4 in context.Employees on t3.EmployeeId equals t4.Id
                                                   join t6 in context.Employees on t1.CreatedBy equals t6.EmployeeId
                                                   select new ApprovalSystemViewModel
                                                   {
                                                       CompanyId = (int)t1.CompanyId,
                                                       Year = t1.Year,
                                                       Month = t1.Month,
                                                       ReportCategoryId = t1.ReportCategoryId,
                                                       FinalStatus = t1.FinalStatus,
                                                       ApprovalStatus = t2.ApprovalStatus,
                                                       ApprovalStatusName = t2.ApprovalStatus == 1 ? "....." : t2.ApprovalStatus == 2 ? "Pending" : t2.ApprovalStatus == 3 ? "Approved" : t2.ApprovalStatus == 4 ? "Rejected" : "",
                                                       EmployeeId = t3.EmployeeId,
                                                       EmployeeName = t3.SignatoryName,
                                                       CreatedBy = t6.Name,
                                                       CreatedDate = t1.CreatedDate.Value,
                                                       Approvdate = t2.ApprovedDate.Value,
                                                       //StringApprovdate = t2.ApprovedDate.Value.ToShortDateString(),
                                                       OrderNo = t2.SignatoryOrderNo,
                                                       FrowardId = t2.ForwardedId.Value,
                                                       IsFestivalBonus = t1.IsFestivalBonus
                                                   }).OrderBy(x => x.OrderNo).ToListAsync());


            return Model;
        }

        public async Task<ApprovalSystemViewModel> SalarySheetApprovalformanagment(ApprovalSystemViewModel model)
        {
            ApprovalSystemViewModel Model = new ApprovalSystemViewModel();
          var  datalist = await Task.Run(() => (from t1 in context.PRollApprovalDetails.Where(d => d.ApprovalFor == model.SectionEmployeeId)
                                                   join t2 in context.PRollApprovals.Where(d => d.CompanyId == model.CompanyId  && d.IsSubmitted == true&& d.Month==model.Month&&d.Year==model.Year && d.IsFestivalBonus == model.IsFestivalBonus) on t1.PRollApprovalId equals t2.PRollApprovalId
                                                   join t4 in context.PayRoll_Signatory on t1.Accounting_SignatoryId equals t4.SignatoryId
                                                   join t5 in context.Employees on t2.CreatedBy equals t5.EmployeeId
                                                   select new ApprovalSystemViewModel
                                                   {
                                                       CompanyId = (int)t2.CompanyId,
                                                       Year = t2.Year,
                                                       Month = t2.Month,
                                                       ReportCategoryId = t2.ReportCategoryId,
                                                       ReportCategoryName = t2.IsFestivalBonus == false? "Salary Sheet": "Bonus Sheet",
                                                       FinalStatus = t2.FinalStatus,
                                                       ApprovalStatus = t1.ApprovalStatus,
                                                       ApprovalStatusName = t1.ApprovalStatus == 1 ? "......" : t1.ApprovalStatus == 2 ? "Pending" : t1.ApprovalStatus == 3 ? "Approved" : t1.ApprovalStatus == 4 ? "Reject" : "",
                                                       EmployeeId = t4.EmployeeId,
                                                       EmployeeName = t4.SignatoryName,
                                                       CreatedBy = t5.Name,
                                                       CreatedDate = t2.CreatedDate.Value,
                                                       Approvdate = t1.ApprovedDate.Value,
                                                       OrderNo = t1.SignatoryOrderNo,
                                                       FrowardId = t1.ForwardedId.Value,
                                                       ApprovalFor = t1.ApprovalFor,
                                                       ReportApprovalDetalisId = t1.PRollApprovalDetailId,
                                                       ReportApprovalId = t2.PRollApprovalId,
                                                       IsFestivalBonus = t2.IsFestivalBonus
                                                   }).OrderByDescending(x => x.CreatedDate).ToListAsync());
            foreach (var item in datalist)
            {
                item.PayarolllId = context.PRoll_PayRoll.Where(f => f.CompanyId == item.CompanyId && f.Month == item.Month && f.Year == item.Year && f.IsFestivalBonus == item.IsFestivalBonus).ToList().LastOrDefault().PayRollId;
            }

            Model.datalist = datalist;
            return Model;
        }
    }
}
