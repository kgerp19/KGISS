using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.HR_Pay_Roll_Service.Hr_PRoll_FineDeducation
{
    public class FineDeductionServices : IFineDeductionServices
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddDeduction(FineDeductionVm model)
        {
            var obj = await context.PRoll_FineDeducation.FirstOrDefaultAsync(x => x.Month == model.Month && x.Year == model.Year && x.IsActive == true && x.CompanyId == model.CompanyId);
            if (obj != null && model.Remarks == null) return 0;
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PRoll_FineDeducation Deduction = new PRoll_FineDeducation();
                    Deduction.CompanyId = model.CompanyId;
                    Deduction.Month = model.Month;
                    Deduction.Year = model.Year;
                    Deduction.Remark = model.Remarks;
                    Deduction.IsActive = true;
                    Deduction.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    Deduction.CreatedDate = DateTime.Now;
                    context.PRoll_FineDeducation.Add(Deduction);
                    context.SaveChanges();

                    List<PRoll_FineDeducationDetail> details = new List<PRoll_FineDeducationDetail>();
                    if (model.FineDeductionDetalies.Count() > 0)
                    {
                        foreach (var item in model.FineDeductionDetalies)
                        {
                            if (item.Amount > 0)
                            {
                                PRoll_FineDeducationDetail detail = new PRoll_FineDeducationDetail();
                                detail.FineDeducationId = Deduction.FineDeducationId;
                                detail.EmployeeId = item.EmployeeId;
                                detail.Amount = item.Amount;
                                detail.IsActive = true;
                                detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                detail.CreatedDate = DateTime.Now;
                                details.Add(detail);
                            }

                        }

                    }
                    context.PRoll_FineDeducationDetail.AddRange(details);
                    context.SaveChanges();
                    scope.Commit();
                    return Deduction.FineDeducationId;

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }

        public async Task<long> AddFIneDetalis(FineDeductionDetailsViewModel item)
        {

            var obj = await context.PRoll_FineDeducationDetail.FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId && x.FineDeducationId == item.FineDeducationId && x.IsActive == true);


            if (obj == null)
            {
                PRoll_FineDeducationDetail detail = new PRoll_FineDeducationDetail();
                detail.FineDeducationId = item.FineDeducationId;
                detail.EmployeeId = item.EmployeeId;
                detail.Amount = item.Amount;
                detail.IsActive = true;
                detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                detail.CreatedDate = DateTime.Now;
                context.PRoll_FineDeducationDetail.Add(detail);
                context.SaveChanges();
                return item.FineDeducationId;
            }

            return 0;
        }

        public async Task<FineDeductionVm> Detalis(long id)
        {
            var result = await context.PRoll_FineDeducation.FirstOrDefaultAsync(f => f.FineDeducationId == id);
            FineDeductionVm fine = new FineDeductionVm();
            fine.CompanyId = result.CompanyId;
            fine.CompanyName = context.Companies.FirstOrDefault(f => f.CompanyId == result.CompanyId).Name;
            fine.Month = result.Month;
            fine.FineDeducationId = result.FineDeducationId;
            fine.Year = result.Year;
            fine.CreatedBy = result.CreatedBy;
            fine.StrCreatedDate = result.CreatedDate.ToShortDateString();
            fine.Remarks = result.Remark;

            fine.FineDeductionlDetaliesList = await Task.Run(() => (from t1 in context.PRoll_FineDeducationDetail
                                                                    join t2 in context.Employees on t1.EmployeeId equals t2.Id
                                                                    where t1.IsActive == true && t1.FineDeducationId == result.FineDeducationId
                                                                    select new FineDeductionDetailsViewModel
                                                                    {
                                                                        FineDeducationDetailId = t1.FineDeducationDetailId,
                                                                        FineDeducationId = t1.FineDeducationId,
                                                                        EmployeeId = t1.EmployeeId,
                                                                        EmployeeName = t2.Name,
                                                                        EmployeeKgId = t2.EmployeeId,
                                                                        Amount = t1.Amount
                                                                    }).ToListAsync());
            return fine;
        }

        public async Task<long> UpdateFineDetalis(FineDeductionDetailsViewModel model)
        {
            PRoll_FineDeducationDetail detail = await context.PRoll_FineDeducationDetail.FirstOrDefaultAsync(f => f.FineDeducationDetailId == model.FineDeducationDetailId);
            detail.Amount = model.Amount;
            detail.IsActive = true;
            detail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            detail.ModifiedDate = DateTime.Now;
            context.Entry(detail).State = EntityState.Modified;
            context.SaveChanges();
            return model.FineDeducationId;
        }
        public async Task<long> Delete(long id)
        {
            var result = await context.PRoll_FineDeducationDetail.FirstOrDefaultAsync(f => f.FineDeducationDetailId == id);
            result.IsActive = false;
            result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            result.ModifiedDate = DateTime.Now;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return result.FineDeducationId;
        }
        public async Task<FineDeductionVm> FineList(int companyId)
        {

            string CompanyName = "";
            var company = context.Companies.Where(x => x.CompanyId == companyId).FirstOrDefault();

            CompanyName = company != null ? company.Name : "All Companies";
            FineDeductionVm mobile = new FineDeductionVm();
            mobile.CompanyName = CompanyName;
            mobile.FIneDeductionList = await Task.Run(() => (from t1 in context.PRoll_FineDeducation
                                                             join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                             where t1.IsActive == true && t1.CompanyId == companyId
                                                             select new FineDeductionVm
                                                             {
                                                                 FineDeducationId = t1.FineDeducationId,
                                                                 CompanyId = t2.CompanyId,
                                                                 CompanyName = t2.Name,
                                                                 Year = t1.Year,
                                                                 Remarks=t1.Remark,
                                                                 Month = t1.Month,
                                                                 CreatedDate = t1.CreatedDate,
                                                                 CreatedBy = t1.CreatedBy
                                                             }).OrderByDescending(f => f.FineDeducationId).AsQueryable());
            return mobile;
        }

    }
}
