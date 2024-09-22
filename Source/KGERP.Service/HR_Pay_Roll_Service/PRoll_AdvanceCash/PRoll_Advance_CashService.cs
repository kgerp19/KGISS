using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AdvanceCash
{
    public class PRoll_Advance_CashService : IPRoll_Advance_CashService
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddAdvance(PRoll_AdvanceViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PRoll_Advance adv = new PRoll_Advance();
                    adv.CompanyId = model.CompanyId;
                    adv.EmployeeId = model.EmployeeId;
                    adv.AdvanceDate = Convert.ToDateTime(model.AdvanceDateString);
                    adv.InstallmentStartDate = Convert.ToDateTime(model.InstallmentStartDateString);
                    adv.InstallmentTypeId = model.InstallmentTypeId;
                    adv.NoOfInstallment = model.NoOfInstallment;
                    adv.TotalAmount = model.TotalAmount;
                    adv.IsActive = true;
                    adv.AdvanceTypeId = model.AdvanceTypeId;
                    adv.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    adv.CreatedDate = DateTime.Now;
                    context.PRoll_Advance.Add(adv);
                    context.SaveChanges();

                    List<PRoll_AdvanceDetail> details = new List<PRoll_AdvanceDetail>();
                    foreach (var item in model.Schedule)
                    {
                        DateTime date = Convert.ToDateTime(item.StringDate);

                        PRoll_AdvanceDetail AdvanceDetail = new PRoll_AdvanceDetail();
                        AdvanceDetail.AdvanceId = adv.AdvanceId;
                        AdvanceDetail.Amount = item.Amount;
                        AdvanceDetail.PaymenyMonth = date.Month;
                        AdvanceDetail.PaymenyYear = date.Year;
                        AdvanceDetail.IsActive = true;
                        AdvanceDetail.IsPaid = false;
                        AdvanceDetail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        AdvanceDetail.CreatedDate = DateTime.Now;
                        details.Add(AdvanceDetail);
                    }
                    context.PRoll_AdvanceDetail.AddRange(details);
                    context.SaveChanges();
                    scope.Commit();
                    return adv.AdvanceId;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }

        public async Task<int> AdvanceDelete(long id)
        {
            var result = await context.PRoll_AdvanceDetail.FirstOrDefaultAsync(f => f.AdvanceDetailId == id);
            result.IsActive = false;
            result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            result.ModifiedDate = DateTime.Now;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return (int)result.AdvanceId;
        }

        public async Task<PRoll_AdvanceViewModel> AdvanceDetalis(long id)
        {
            PRoll_AdvanceViewModel mobile = new PRoll_AdvanceViewModel();
            mobile = await Task.Run(() => (from t1 in context.PRoll_Advance
                                           join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                           join t3 in context.Employees on t1.EmployeeId equals t3.Id
                                           where t1.IsActive == true && t1.AdvanceId == id
                                           select new PRoll_AdvanceViewModel
                                           {
                                               AdvanceId = t1.AdvanceId,
                                               CompanyId = t2.CompanyId,
                                               EmployeeName = t3.Name,
                                               CompanyName = t2.Name,
                                               AdvanceDate = t1.AdvanceDate,
                                               TotalAmount = t1.TotalAmount,
                                               NoOfInstallment = t1.NoOfInstallment,
                                               InstallmentTypeId = t1.InstallmentTypeId,
                                               CreatedDate = t1.CreatedDate,
                                               CreatedBy = t1.CreatedBy
                                           }).FirstOrDefaultAsync());

            mobile.models = await Task.Run(() => (from t1 in context.PRoll_Advance
                                                  join t2 in context.PRoll_AdvanceDetail on t1.AdvanceId equals t2.AdvanceId
                                                  where t2.IsActive == true && t1.AdvanceId == id
                                                  select new PRoll_AdvanceDetalisViewModel
                                                  {
                                                      AdvanceId = t1.AdvanceId,
                                                      AdvanceDetailId = t2.AdvanceDetailId,
                                                      PaymenyMonth = t2.PaymenyMonth,
                                                      PaymenyYear = t2.PaymenyYear,
                                                      Amount = t2.Amount,
                                                      CreatedDate = t1.CreatedDate,
                                                      CreatedBy = t1.CreatedBy
                                                  }).ToListAsync());
            return mobile;
        }

        public async Task<PRoll_AdvanceViewModel> AdvanceList(int companyId)
        {
            PRoll_AdvanceViewModel mobile = new PRoll_AdvanceViewModel();
            mobile.viewModels = await Task.Run(() => (from t1 in context.PRoll_Advance
                                                      join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                      join t3 in context.Employees on t1.EmployeeId equals t3.Id
                                                      join t6 in context.Proll_AdvanceeType on t1.AdvanceTypeId equals t6.AdvanceTypeId into t6_JOIN
                                                      from t6 in t6_JOIN.DefaultIfEmpty()
                                                      where t1.IsActive == true && t1.CompanyId == companyId
                                                      select new PRoll_AdvanceViewModel
                                                      {
                                                          AdvanceId = t1.AdvanceId,
                                                          CompanyId = t2.CompanyId,
                                                          CompanyName = t2.Name,
                                                          AdvanceDate = t1.AdvanceDate,
                                                          TotalAmount = t1.TotalAmount,
                                                          NoOfInstallment = t1.NoOfInstallment,
                                                          InstallmentTypeId = t1.InstallmentTypeId,
                                                          CreatedDate = t1.CreatedDate,
                                                          CreatedBy = t1.CreatedBy,
                                                          EmployeeName = t3.EmployeeId + " - " + t3.Name,
                                                          AdvanceTypeName = t6.AdvanceName
                                                      }).OrderByDescending(f => f.AdvanceId).AsQueryable());
            return mobile;
        }

        public Task<long> UpdateAdvance(PRoll_AdvanceViewModel model)
        {
            throw new NotImplementedException();
        }

        public Task<long> UpdatPRoll_AdvanceViewModeleAdvance(PRoll_AdvanceViewModel model)
        {
            throw new NotImplementedException();
        }


        public async Task<List<SelectModelAdvanceType>> GetAdvanceTypeType(int companyId)
        {
            List<SelectModelAdvanceType> selectModelLiat = new List<SelectModelAdvanceType>();
            var v = await context.Proll_AdvanceeType.Where(e => e.IsActive == true && e.CompanyId == companyId).Select(x => new SelectModelAdvanceType()
            {
                Text = x.AdvanceName,
                Value = x.AdvanceTypeId,
            }).ToListAsync();
            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }

        public async Task<VMAdvanceType> GetAdvanceType(int companyId)
        {
            VMAdvanceType vmCommonUnit = new VMAdvanceType();
            vmCommonUnit.CompanyId = companyId;
            vmCommonUnit.DataList = await Task.Run(() => (from t1 in context.Proll_AdvanceeType
                                                          where t1.IsActive
                                                          && t1.CompanyId == companyId
                                                          select new VMAdvanceType
                                                          {
                                                              AdvanceTypeId = t1.AdvanceTypeId,
                                                              Name = t1.AdvanceName,
                                                              CompanyId = t1.CompanyId,
                                                              CreatedBy = t1.CreatedBy

                                                          }).OrderByDescending(x => x.AdvanceTypeId).AsEnumerable());
            return vmCommonUnit;
        }

        public async Task<int> AdvanceTypeAdd(VMAdvanceType vMAdvanceType)
        {
            var result = -1;
            string CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(CreateBy))
            {
                return result;
            }
            else
            {
                Proll_AdvanceeType model = new Proll_AdvanceeType
                {
                    AdvanceName = vMAdvanceType.Name,
                    CompanyId = (int)vMAdvanceType.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                context.Proll_AdvanceeType.Add(model);
                if (await context.SaveChangesAsync() > 0)
                {
                    result = model.AdvanceTypeId;
                }
            }
            
            return result;
        }

        public async Task<int> AdvanceTypeEdit(VMAdvanceType vMAdvanceType)
        {
            var result = -1;
            string CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(CreateBy))
            {
                return result;
            }
            else
            {
                Proll_AdvanceeType model = context.Proll_AdvanceeType.Find(vMAdvanceType.AdvanceTypeId);
                model.AdvanceName = vMAdvanceType.Name;
                model.ModifyBy = CreateBy;
                model.ModifyDate = DateTime.Now;

                if (await context.SaveChangesAsync() > 0)
                {
                    result = model.AdvanceTypeId;
                }
                return result;
            }
            
        }

        public async Task<int> AdvanceTypeDelete(int id)
        {
            int result = -1;
            string CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(CreateBy))
            {
                return result;
            }
            if (id != 0)
            {
                Proll_AdvanceeType model = context.Proll_AdvanceeType.Find(id);
                model.IsActive = false;
                model.ModifyBy = CreateBy;
                model.ModifyDate = DateTime.Now;
                if (await context.SaveChangesAsync() > 0)
                {
                    result = model.AdvanceTypeId;
                }
            }
            return result;
        }
    }
}
