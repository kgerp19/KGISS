using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service
{
    public class MobileBillServices : IMobileBillServices
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddBill(MoobileBillViewModel model)
        { var obj =await context.PRoll_MobileBill.FirstOrDefaultAsync(x=>x.Month==model.Month && x.Year==model.Year && x.IsActive==true && x.CompanyId==model.CompanyId);
            if (obj != null) return 0;
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PRoll_MobileBill mobile = new PRoll_MobileBill();
                    mobile.CompanyId = model.CompanyId;
                    mobile.Month = model.Month;
                    mobile.Year = model.Year;
                    mobile.IsActive = true;
                    mobile.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    mobile.CreatedDate = DateTime.Now;
                    context.PRoll_MobileBill.Add(mobile);
                    context.SaveChanges();

                    List<PRoll_MobileBillDetail> details = new List<PRoll_MobileBillDetail>();
                    if (model.MobileBillDetalies.Count() > 0)
                    {
                        foreach (var item in model.MobileBillDetalies)
                        {
                            if (item.Amount > 0)
                            {
                                PRoll_MobileBillDetail detail = new PRoll_MobileBillDetail();
                                detail.MobileBillId = mobile.MobileBillId;
                                detail.EmployeeId = item.EmployeeId;
                                detail.Amount = item.Amount;
                                detail.IsActive = true;
                                detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                detail.CreatedDate = DateTime.Now;
                                details.Add(detail);
                            }

                        }

                    }
                    context.PRoll_MobileBillDetail.AddRange(details);
                    context.SaveChanges();
                    scope.Commit();
                    return mobile.MobileBillId;

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }

        public async Task<long> AddBillDetalis(MobileBillDetaliesViewModel item)
        {
            var obj = await context.PRoll_MobileBillDetail.FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId && x.MobileBillId == item.MobileBillId && x.IsActive==true);


            if (obj == null)
            {
                PRoll_MobileBillDetail detail = new PRoll_MobileBillDetail();
                detail.MobileBillId = item.MobileBillId;
                detail.EmployeeId = item.EmployeeId;
                detail.Amount = item.Amount;
                detail.IsActive = true;
                detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                detail.CreatedDate = DateTime.Now;
                context.PRoll_MobileBillDetail.Add(detail);
                context.SaveChanges();
                return item.MobileBillId;
            }
          return 0;
           
        }

        public async Task<MoobileBillViewModel> BillList(int companyId)
        {
            MoobileBillViewModel mobile = new MoobileBillViewModel();
            mobile.CompanyId = companyId;
            mobile.MobileBillList = await Task.Run(() => (from t1 in context.PRoll_MobileBill
                                                      join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                      where t1.IsActive == true && t1.CompanyId == companyId
                                                          select new MoobileBillViewModel
                                                      {
                                                          MobileBillId = t1.MobileBillId,
                                                          CompanyId = t2.CompanyId,
                                                          CompanyName = t2.Name,
                                                          Year = t1.Year,
                                                          Month = t1.Month,
                                                          CreatedDate = t1.CreatedDate,
                                                          CreatedBy = t1.CreatedBy
                                                      }).OrderByDescending(f => f.MobileBillId).AsQueryable());
            return mobile;
        }

        public async Task<long> Delete(long id)
        {
            var result = await context.PRoll_MobileBillDetail.FirstOrDefaultAsync(f => f.MobileBillDetailId == id);
            result.IsActive = false;
            result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            result.ModifiedDate = DateTime.Now;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return result.MobileBillId;
        }

        public async Task<MoobileBillViewModel> Detalis(long id)
        {
            var result = await context.PRoll_MobileBill.FirstOrDefaultAsync(f => f.MobileBillId == id);
            MoobileBillViewModel mobile = new MoobileBillViewModel();
            mobile.CompanyId = result.CompanyId;
            mobile.CompanyName = context.Companies.FirstOrDefault(f => f.CompanyId == result.CompanyId).Name;
            mobile.Month = result.Month;
            mobile.MobileBillId = result.MobileBillId;
            mobile.Year = result.Year;
            mobile.CreatedBy = result.CreatedBy;
            mobile.StrCreatedDate = result.CreatedDate.ToShortDateString();

            mobile.MobileBillDetaliesList = await Task.Run(() => (from t1 in context.PRoll_MobileBillDetail
                                                              join t2 in context.Employees on t1.EmployeeId equals t2.Id
                                                              where t1.IsActive == true && t1.MobileBillId == result.MobileBillId
                                                                  select new MobileBillDetaliesViewModel
                                                              {
                                                                  MobileBillDetailId = t1.MobileBillDetailId,
                                                                  MobileBillId = t1.MobileBillId,
                                                                  EmployeeId = t1.EmployeeId,
                                                                  EmployeeName = t2.Name,
                                                                  EmployeeKgId = t2.EmployeeId,
                                                                  Amount=t1.Amount
                                                              }).ToListAsync());
            return mobile;
        }

        public async Task<List<EmployeeInfoVM>> employees(int companyId)
        {
            List<EmployeeInfoVM> employees = new List<EmployeeInfoVM>();
            employees =  context.Employees.Where(d => d.Active == true && d.CompanyId == companyId && d.OfficeTypeId == 33).ToList().Select( x => new EmployeeInfoVM()
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                Name = x.Name,
                MobileBill=  0
            }).OrderBy(x => x.Id).ToList();
            return employees;
        }

        public List<EmployeeInfoVM> GetMobileBills(int companyId, int? year, int? month)
        {
            List<EmployeeInfoVM> employees = new List<EmployeeInfoVM>();
            employees = context.Employees.Where(d => d.Active == true && d.CompanyId == companyId).Select(x => new EmployeeInfoVM()
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                Name = x.Name,
                MobileBill = year.HasValue && month.HasValue ? (from m in context.PRoll_MobileBillDetail
                                                                         join d in context.PRoll_MobileBill on m.MobileBillId equals d.MobileBillId
                                                                         where m.EmployeeId == x.Id && ((year == null || month == null) || (d.Year == year && d.Month == month))
                                                                         select m.Amount).FirstOrDefault()??0 : 0
            }).OrderBy(x => x.Id).ToList();
            return employees;
        }

        public async Task<long> UpdateBillDetalis(MobileBillDetaliesViewModel model)
        {
            PRoll_MobileBillDetail detail = await context.PRoll_MobileBillDetail.FirstOrDefaultAsync(f => f.MobileBillDetailId == model.MobileBillDetailId);
            detail.Amount = model.Amount;
            detail.IsActive = true;
            detail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            detail.ModifiedDate = DateTime.Now;
            context.Entry(detail).State = EntityState.Modified;
            context.SaveChanges();
            return model.MobileBillId;
        }
    }
}
