using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services
{
    public class FoodBillServices : IFoodBillServices
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddBill(FoodBillViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                long foodBillId = 0;
                var userId = Common.GetUserId();
                try
                {
                    var foodbillSetup = await context.PRoll_FoodBill.FirstOrDefaultAsync(x => x.CompanyId == model.CompanyId && x.Year == model.Year && x.Month == model.Month && x.IsActive);
                    if (foodbillSetup != null && model.Remarks == null)
                    {
                        return foodbillSetup.FoodBillId;
                    }

                    PRoll_FoodBill foodBill = new PRoll_FoodBill();
                    foodBill.CompanyId = model.CompanyId;
                    foodBill.Month = model.Month;
                    foodBill.Year = model.Year;
                    foodBill.LunchRate = model.LunchRate;
                    foodBill.BreakfastRate = model.BreakfastRate;
                    foodBill.Remark = model.Remarks;
                    foodBill.IsActive = true;
                    foodBill.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    foodBill.CreatedDate = DateTime.Now;
                    context.PRoll_FoodBill.Add(foodBill);
                    if (await context.SaveChangesAsync() > 0)
                    {
                        List<PRoll_FoodBillDetail> details = new List<PRoll_FoodBillDetail>();
                        if (model.FoodBillDetalies.Count() > 0)
                        {
                            foreach (var item in model.FoodBillDetalies)
                            {
                                if (item.NoOfBreakfast > 0 || item.NoOfLunch > 0)
                                {
                                    PRoll_FoodBillDetail detail = new PRoll_FoodBillDetail();
                                    detail.FoodBillId = foodBill.FoodBillId;
                                    detail.EmployeeId = item.EmployeeId;
                                    detail.NoOfLunch = item.NoOfLunch;
                                    detail.NoOfBreakfast = item.NoOfBreakfast;
                                    detail.IsActive = true;
                                    detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    detail.CreatedDate = DateTime.Now;
                                    details.Add(detail);
                                }
                            }

                        }
                        context.PRoll_FoodBillDetail.AddRange(details);

                        if (await context.SaveChangesAsync() > 0)
                        {
                            scope.Commit();
                            foodBillId = foodBill.FoodBillId;
                        }

                    }
                    return foodBillId;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<long> AddBillDetalis(FoodBillDetaliesViewModel item)
        {
            var checkemp = await context.PRoll_FoodBillDetail.FirstOrDefaultAsync(f => f.EmployeeId == item.EmployeeId && f.FoodBillId == item.FoodBillId && f.IsActive);
            if (checkemp != null)
            {
                return item.FoodBillId;
            }
            PRoll_FoodBillDetail detail = new PRoll_FoodBillDetail();
            detail.FoodBillId = item.FoodBillId;
            detail.EmployeeId = item.EmployeeId;
            detail.NoOfLunch = item.NoOfLunch;
            detail.NoOfBreakfast = item.NoOfBreakfast;
            detail.IsActive = true;
            detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            detail.CreatedDate = DateTime.Now;
            context.PRoll_FoodBillDetail.Add(detail);
            await context.SaveChangesAsync();
            return item.FoodBillId;
        }

        public async Task<FoodBillViewModel> BillList(int companyId)
        {
            //string CompanyName = "";
            //var company = context.Companies.Where(x => x.CompanyId == companyId).FirstOrDefault();
            //CompanyName = company != null ? company.Name : "All Companies";
            FoodBillViewModel food = new FoodBillViewModel();
            food.CompanyId = companyId;
            food.FoodBillList = await Task.Run(() => (from t1 in context.PRoll_FoodBill
                                                      join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                      where t1.IsActive == true && t1.CompanyId == companyId
                                                      select new FoodBillViewModel
                                                      {
                                                          FoodBillId = t1.FoodBillId,
                                                          CompanyId = t2.CompanyId,
                                                          CompanyName = t2.Name,
                                                          Remarks = t1.Remark,
                                                          Year = t1.Year,
                                                          Month = t1.Month,
                                                          CreatedDate = t1.CreatedDate,
                                                          CreatedBy = t1.CreatedBy
                                                      }).OrderByDescending(f => f.FoodBillId).AsQueryable());
            return food;
        }

        public async Task<long> Delete(long id)
        {
            var result = await context.PRoll_FoodBillDetail.FirstOrDefaultAsync(f => f.FoodBillDetailId == id);
            result.IsActive = false;
            result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            result.ModifiedDate = DateTime.Now;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return result.FoodBillId;
        }

        public async Task<FoodBillViewModel> Detalis(long id)
        {
            var result = await context.PRoll_FoodBill.FirstOrDefaultAsync(f => f.FoodBillId == id);
            FoodBillViewModel foodBill = new FoodBillViewModel();
            foodBill.CompanyId = result.CompanyId;
            foodBill.CompanyName = context.Companies.FirstOrDefault(f => f.CompanyId == result.CompanyId).Name;
            foodBill.Month = result.Month;
            foodBill.FoodBillId = result.FoodBillId;
            foodBill.Year = result.Year;
            foodBill.LunchRate = result.LunchRate;
            foodBill.BreakfastRate = result.BreakfastRate;
            foodBill.CreatedBy = result.CreatedBy;
            foodBill.StrCreatedDate = result.CreatedDate.ToShortDateString();

            foodBill.FoodBillDetalies = await Task.Run(() => (from t1 in context.PRoll_FoodBillDetail
                                                              join t2 in context.Employees on t1.EmployeeId equals t2.Id
                                                              where t1.IsActive == true && t1.FoodBillId == result.FoodBillId
                                                              select new FoodBillDetaliesViewModel
                                                              {
                                                                  FoodBillDetailId = t1.FoodBillDetailId,
                                                                  FoodBillId = t1.FoodBillId,
                                                                  EmployeeId = t1.EmployeeId,
                                                                  NoOfBreakfast = t1.NoOfBreakfast,
                                                                  NoOfLunch = t1.NoOfLunch,
                                                                  EmployeeName = t2.Name,
                                                                  EmployeeKgId = t2.EmployeeId
                                                              }).ToListAsync());
            var officeExpense = await (from fDetails in context.PRoll_FoodBillDetail
                                       where fDetails.IsActive && fDetails.EmployeeId == 0 && fDetails.FoodBillId == result.FoodBillId
                                       select new FoodBillDetaliesViewModel
                                       {
                                           FoodBillDetailId = fDetails.FoodBillDetailId,
                                           FoodBillId = fDetails.FoodBillId,
                                           EmployeeId = 0,
                                           NoOfBreakfast = fDetails.NoOfBreakfast,
                                           NoOfLunch = fDetails.NoOfLunch,
                                           EmployeeName = "Office Expense",
                                           EmployeeKgId = "Office Expense"
                                       }).FirstOrDefaultAsync();
            if (officeExpense != null)
            {
                foodBill.FoodBillDetalies.Add(officeExpense);
            }
            return foodBill;

        }

        public async Task<List<EmployeeInfoVM>> employees(int companyId)
        {
            List<EmployeeInfoVM> employees = new List<EmployeeInfoVM>();
            employees = context.Employees.Where(d => d.Active == true && d.CompanyId == companyId).ToList().Select(x => new EmployeeInfoVM()
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                Name = x.Name,
                NoOfLunch = 0,
                NoOfBreakfast = 0,
            }).OrderBy(x => x.Id).ToList();

            if (companyId > 0)
            {
                employees.Add(new EmployeeInfoVM()
                {
                    Id = 0,
                    EmployeeId = "Office Expense",
                    Name = "Office Expense",
                    NoOfLunch = 0,
                    NoOfBreakfast = 0,
                });
            }

            return employees;
        }

        public async Task<List<EmployeeInfoVM>> AllEmployeesfoConfiguration(int companyId)
        {
            List<EmployeeInfoVM> employees = new List<EmployeeInfoVM>();
            var configuredEmp = context.PRoll_SalaryConfiguration.Where(x => x.CompanyId == companyId).Select(x => x.EmployeeId).ToList();
            employees = context.Employees.Where(d => d.Active == true && !configuredEmp.Contains(d.Id) && d.CompanyId == companyId).ToList().Select(x => new EmployeeInfoVM()
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                Name = x.Name,
                NoOfLunch = 0,
                NoOfBreakfast = 0,
            }).OrderBy(x => x.Id).ToList();


            return employees;
        }

        public async Task<List<EmployeeInfoVM>> AllEmployees(int companyId)
        {
            List<EmployeeInfoVM> employees = new List<EmployeeInfoVM>();
            employees = (from t1 in context.PRoll_SalaryConfiguration
                         join t2 in context.Employees on t1.EmployeeId equals t2.Id
                         where t1.IsActive == true && t1.CompanyId == companyId
                         select new EmployeeInfoVM
                         {
                             Id = t2.Id,
                             EmployeeId = t2.EmployeeId,
                             Name = t2.Name,
                             NoOfLunch = 0,
                             NoOfBreakfast = 0,
                         }).OrderBy(x => x.Id).ToList();


            return employees;
        }
        public async Task<List<EmployeeInfoVMForFine>> AllEmployeesForFine(int companyId)
        {
            List<EmployeeInfoVMForFine> employees = new List<EmployeeInfoVMForFine>();
            employees = (from t1 in context.PRoll_SalaryConfiguration
                         join t2 in context.Employees on t1.EmployeeId equals t2.Id
                         where t1.IsActive == true && t1.CompanyId == companyId
                         select new EmployeeInfoVMForFine
                         {
                             Id = t2.Id,
                             EmployeeId = t2.EmployeeId,
                             Name = t2.Name,
                             NoOfLunch = 0,
                             NoOfBreakfast = 0
                         }).OrderBy(x => x.Id).ToList();
            return employees;
        }

        public async Task<long> UpdateBillDetalis(FoodBillDetaliesViewModel item)
        {
            PRoll_FoodBillDetail detail = await context.PRoll_FoodBillDetail.FirstOrDefaultAsync(f => f.FoodBillDetailId == item.FoodBillDetailId);

            //detail.EmployeeId = item.EmployeeId;
            detail.NoOfLunch = item.NoOfLunch;
            detail.NoOfBreakfast = item.NoOfBreakfast;
            detail.IsActive = true;
            detail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            detail.ModifiedDate = DateTime.Now;
            context.Entry(detail).State = EntityState.Modified;
            context.SaveChanges();
            return item.FoodBillId;
        }
    }
}
