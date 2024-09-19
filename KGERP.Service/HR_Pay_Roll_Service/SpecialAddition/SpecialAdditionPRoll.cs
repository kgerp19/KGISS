using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Service.HR_Pay_Roll_Service.SpecialAddition;
using KGERP.Service.ServiceModel;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.HR_Pay_Roll_Service.SpecialAddition
{
    public class SpecialAdditionPRoll : ISpecialAdditionServices
    {
        ERPEntities context = new ERPEntities();

        public async Task<SpecialAdditionViewModel> SpecialAddtnList(int CompanyId)
        {
            SpecialAdditionViewModel Specialaddition = new SpecialAdditionViewModel();
            Specialaddition.CompanyId = CompanyId;
            Specialaddition.SpecialAditionList = await Task.Run(() => (from t1 in context.PRoll_SpecialAddition
                                                          join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                          where t1.IsActive == true && t2.CompanyId==CompanyId
                                                          select new SpecialAdditionViewModel
                                                          {
                                                              SpecialAdditionId = t1.SpecialAdditionId,
                                                              CompanyId = t2.CompanyId,
                                                              CompanyName = t2.Name,
                                                              Year = t1.Year,
                                                              Month = t1.Month,
                                                              CreatedDate = t1.CreatedDate,
                                                              CreatedBy = t1.CreatedBy
                                                          }).OrderByDescending(f => f.SpecialAdditionId).AsQueryable());
            return Specialaddition;
        }
        public async Task<SpecialAdditionViewModel> SpAddDetalis(long id)
        {
            var result = await context.PRoll_SpecialAddition.FirstOrDefaultAsync(f => f.SpecialAdditionId == id);
            SpecialAdditionViewModel specialAddition = new SpecialAdditionViewModel();
            specialAddition.CompanyId = result.CompanyId;
            specialAddition.CompanyName = context.Companies.FirstOrDefault(f => f.CompanyId == result.CompanyId).Name;
            specialAddition.Month = result.Month;
            specialAddition.SpecialAdditionId = result.SpecialAdditionId;
            specialAddition.Year = result.Year;
            specialAddition.CreatedBy = result.CreatedBy;
            specialAddition.StrCreatedDate = result.CreatedDate.ToShortDateString();

            specialAddition.SpecialAditionDetaliesList = await Task.Run(() => (from t1 in context.PRoll_SpecialAdditionDetail
                                                                  join t2 in context.Employees on t1.EmployeeId equals t2.Id
                                                                  where t1.IsActive == true && t1.SpecialAdditionId == result.SpecialAdditionId
                                                                  select new SpecialAdditionDetailsViewModel
                                                                  {
                                                                      SpecialAdditionDetailId = t1.SpecialAdditionDetailId,
                                                                      SpecialAdditionId = t1.SpecialAdditionId,
                                                                      EmployeeId = t1.EmployeeId,
                                                                      EmployeeName = t2.Name,
                                                                      EmployeeKgId = t2.EmployeeId,
                                                                      Amount = t1.Amount,
                                                                      Remark=t1.Remark
                                                                  }).ToListAsync());
            return specialAddition;
        }
        public async Task<long> AddSpecialAddition(SpecialAdditionDetailsViewModel item)
        {

            PRoll_SpecialAdditionDetail detail = new PRoll_SpecialAdditionDetail();
            detail.SpecialAdditionId = item.SpecialAdditionId;
            detail.EmployeeId = item.EmployeeId;
            detail.Amount = item.Amount;
            detail.Remark = item.Remark;
            detail.IsActive = true;
            detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            detail.CreatedDate = DateTime.Now;
            context.PRoll_SpecialAdditionDetail.Add(detail);
            context.SaveChanges();
            return item.SpecialAdditionId;
            //var obj = await context.PRoll_SpecialAdditionDetail.FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId && x.SpecialAdditionId == item.SpecialAdditionId && x.IsActive==true);


            //if (obj == null)
            //{

            //}
            return 0;

        }
        public async Task<long> UpdateBillDetalis(SpecialAdditionDetailsViewModel model)
        {
            PRoll_SpecialAdditionDetail detail = await context.PRoll_SpecialAdditionDetail.FirstOrDefaultAsync(f => f.SpecialAdditionDetailId == model.SpecialAdditionDetailId);
            detail.Amount = model.Amount;
            detail.Remark = model.Remark;
            detail.IsActive = true;
            detail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            detail.ModifiedDate = DateTime.Now;
            context.Entry(detail).State = EntityState.Modified;
            context.SaveChanges();
            return model.SpecialAdditionId;
        }

        public async Task<long> Delete(long id)
        {
            var result = await context.PRoll_SpecialAdditionDetail.FirstOrDefaultAsync(f => f.SpecialAdditionDetailId == id);
            result.IsActive = false;
            result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            result.ModifiedDate = DateTime.Now;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return result.SpecialAdditionId;
        }
        public async Task<long> AddSpecialAddition(SpecialAdditionViewModel model)
        {
            var obj = await context.PRoll_SpecialAddition.FirstOrDefaultAsync(x => x.Month == model.Month && x.Year == model.Year && x.IsActive == true && x.CompanyId == model.CompanyId);
            if (obj != null) return 0;
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PRoll_SpecialAddition spsdd = new PRoll_SpecialAddition();
                    spsdd.CompanyId = model.CompanyId;
                    spsdd.Month = model.Month;
                    spsdd.Year = model.Year;
                    spsdd.IsActive = true;
                    spsdd.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    spsdd.CreatedDate = DateTime.Now;
                    context.PRoll_SpecialAddition.Add(spsdd);
                    context.SaveChanges();

                    List<PRoll_SpecialAdditionDetail> details = new List<PRoll_SpecialAdditionDetail>();
                    if (model.SpecialAditionDetaliesListDetalies.Count() > 0)
                    {
                        foreach (var item in model.SpecialAditionDetaliesListDetalies)
                        {
                            if (item.Amount > 0)
                            {
                                PRoll_SpecialAdditionDetail detail = new PRoll_SpecialAdditionDetail();
                                detail.SpecialAdditionId = spsdd.SpecialAdditionId;
                                detail.EmployeeId = item.EmployeeId;
                                detail.Amount = item.Amount;
                                detail.Remark = item.Remark;
                                detail.IsActive = true;
                                detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                detail.CreatedDate = DateTime.Now;
                                details.Add(detail);
                            }

                        }

                    }
                    context.PRoll_SpecialAdditionDetail.AddRange(details);
                    context.SaveChanges();
                    scope.Commit();
                    return spsdd.SpecialAdditionId;

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }
        public List<EmployeeInfoVM> GetSpecialAdd(int companyId, int? year, int? month)
        {
            List<EmployeeInfoVM> employees = new List<EmployeeInfoVM>();

            employees = (from t1 in context.PRoll_SalaryConfiguration
                     join t2 in context.Employees on t1.EmployeeId equals t2.Id
                     where t1.CompanyId == companyId && t1.IsActive == true
                     select new EmployeeInfoVM
                     {
                         Id = t2.Id,
                         EmployeeId = t2.EmployeeId,
                         Name = t2.Name,
                         specialAddition = year.HasValue && month.HasValue ? (from m in context.PRoll_SpecialAdditionDetail
                                                                              join d in context.PRoll_SpecialAddition on m.SpecialAdditionId equals d.SpecialAdditionId
                                                                              where m.EmployeeId == t2.Id && ((year == null || month == null) || (d.Year == year && d.Month == month))
                                                                              select m.Amount).FirstOrDefault() ?? 0 : 0
                     }                   
                     ).OrderBy(x => x.Id).ToList();
            return employees;

        }

    }
}
