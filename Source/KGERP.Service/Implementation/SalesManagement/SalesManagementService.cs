using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows.Media.Media3D;

namespace KGERP.Service.Implementation.SalesManagement
{
    public class SalesManagementService : ISalesManagementService
    {
        private readonly ERPEntities _dbContext;

        public SalesManagementService(ERPEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> SaveSalesAchievementAsync(SalesManagementVM model)
        {
            try
            {
                var existingAchievement = _dbContext.KGSalesAchivements
                    .FirstOrDefault(x =>
                        x.FromDate <= model.ToDate &&
                        x.ToDate >= model.FromDate && x.IsActive == true);

                if (existingAchievement != null)
                {
                    return 0;
                }

                var sa = new KGSalesAchivement
                {
                    Title = model.AchievementTitle,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CompanyId=model.CompanyId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    CreatedBy = Common.GetUserId()
                };

                _dbContext.KGSalesAchivements.Add(sa);
                await _dbContext.SaveChangesAsync();
                return sa.KGSalesId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data in sales achievements: {ex.Message}");
                throw;
            }
        }
        public async Task<long> UpdateAchievementAsync(SalesManagementVM model)
        {
            try
            {
                var achievement = _dbContext.KGSalesAchivements.Find(model.AchievementId);
                if (achievement != null)
                {
                    // Check if there's already an achievement with overlapping date range
                    var existingAchievement = _dbContext.KGSalesAchivements
                        .FirstOrDefault(sa =>
                            sa.KGSalesId != model.AchievementId &&
                            sa.FromDate <= model.ToDate &&
                            sa.ToDate >= model.FromDate);

                    if (existingAchievement != null)
                    {
                        return 0;
                    }

                    achievement.Title = model.AchievementTitle;
                    achievement.FromDate = model.FromDate;
                    achievement.ToDate = model.ToDate;
                    achievement.ModifiedBy = Common.GetUserId();
                    achievement.ModifiedDate = DateTime.Now.ToString();
                    await _dbContext.SaveChangesAsync();
                    return achievement.KGSalesId;
                }
                else
                {
                    throw new Exception("Achievement not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating achievement: {ex.Message}");
                throw;
            }
        }


        public async Task<List<SalesManagementVM>> GetAllAchievementsAsync()
        {
            try
            {
                var sa = await _dbContext.KGSalesAchivements.Where(x => x.IsActive)
                    .OrderByDescending(a => a.KGSalesId)
                    .ToListAsync();

                var achievementModels = sa.Select(a => new SalesManagementVM
                {
                    AchievementId = a.KGSalesId,
                    AchievementTitle = a.Title,
                    FromDate = a.FromDate,
                    ToDate = a.ToDate,
                    TotalTergetedAmount =  _dbContext.KGCompanySaleTergets.Where(x => x.KGSalesId == a.KGSalesId && a.IsActive).Select(x => x.TergetedAmount).DefaultIfEmpty(0).Sum(),
                }).ToList();
                
                return achievementModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievements: {ex.Message}");
                throw;
            }
        }
        public async Task DeleteAchievementAsync(long achievementId)
        {
            var achievement = _dbContext.KGSalesAchivements.Find(achievementId);
            if (achievement != null)
            {
                achievement.IsActive = false;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Achievement not found.");
            }
        }
        public async Task<SalesManagementVM> GetAchievementAsync(long achievementId)
        {
            try
            {
                var achievement = _dbContext.KGSalesAchivements.Find(achievementId);
                if (achievement != null)
                {
                    return new SalesManagementVM
                    {
                        AchievementId = achievement.KGSalesId,
                        AchievementTitle = achievement.Title,
                        StrFromDate = achievement.FromDate.ToString("dd/MM/yyyy"),
                        StrToDate = achievement.ToDate.ToString("dd/MM/yyyy")
                    };
                }
                else
                {
                    throw new Exception("Achievement not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievement: {ex.Message}");
                throw;
            }
        }
        public IEnumerable<SelectListItem> GetDDLSalesAchievements()
        {
            return _dbContext.KGSalesAchivements.Where(x => x.IsActive == true).Select(p => new SelectListItem
            {
                Value = p.KGSalesId.ToString(),
                Text = p.Title
            }).ToList();
        }
        public IEnumerable<SelectListItem> GetDDLCompany(int companyId)
        {
            return _dbContext.Companies.Where(x => x.IsActive == true && x.CompanyId== companyId).Select(p => new SelectListItem
            {
                Value = p.CompanyId.ToString(),
                Text = p.Name
            }).ToList();
        }

        public async Task<SalesManagementVM> GetSaleswiseTarget(int achievementId)
        {
            try
            {
                SalesManagementVM achievementModels = new SalesManagementVM();

                achievementModels = (from t1 in _dbContext.KGSalesAchivements
                                     where t1.KGSalesId == achievementId && t1.IsActive
                                     select new SalesManagementVM
                                     {
                                         AchievementTitle = t1.Title,
                                         FromDate = t1.FromDate,
                                         ToDate = t1.ToDate

                                     }).FirstOrDefault();

                var sa = _dbContext.KGCompanySaleTergets.Where(x => x.IsActive && x.KGSalesId == achievementId)
                    .OrderByDescending(a => a.KGSalesId)
                    .ToList();

                achievementModels.DataList = sa.Select(a => new SalesManagementVM
                {
                    KGcomTargetId = a.KGCompanySaleTergetId,
                    AchievementId = a.KGSalesId,
                    CompanyName = _dbContext.Companies.Where(x => x.CompanyId == a.CompanyId).Select(x => x.Name).FirstOrDefault(),
                    CompanyId = a.CompanyId,
                    TargetAmount = a.TergetedAmount
                }).ToList();

                return achievementModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievements: {ex.Message}");
                throw;
            }
        }
        public async Task<long> SaveKGCompanySaleTarget(SalesManagementVM model)
        {
            try
            {
                var isExist = _dbContext.KGCompanySaleTergets.Where(x => x.KGSalesId == model.AchievementId && x.CompanyId == model.CompanyId && x.IsActive == true).FirstOrDefault();
                if (isExist == null)
                {
                    var sa = new KGCompanySaleTerget
                    {
                        KGSalesId = model.AchievementId,
                        CompanyId = model.CompanyId,
                        TergetedAmount = model.TargetAmount,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        CreatedBy = Common.GetUserId()
                    };

                    _dbContext.KGCompanySaleTergets.Add(sa);
                    await _dbContext.SaveChangesAsync();
                    return sa.KGCompanySaleTergetId;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data in sales targets: {ex.Message}");
                throw;
            }
        }
        public async Task<SalesManagementVM> GetCompanyWiseSalesAchievements(int companyid)
        {
            SalesManagementVM model = new SalesManagementVM();
            var x = (from t1 in _dbContext.KGSalesAchivements
                     join t2 in _dbContext.KGCompanySaleTergets on t1.KGSalesId equals t2.KGSalesId
                     where t2.CompanyId == companyid && t2.IsActive == true && t1.IsActive == true
                     select new SalesManagementVM
                     {
                         AchievementId = t2.KGSalesId,
                         AchievementTitle = t1.Title,
                         TargetAmount = t2.TergetedAmount,
                         KGCompanySaleTergetId = t2.KGCompanySaleTergetId,
                         FromDate = t1.FromDate,
                         ToDate = t1.ToDate
                     }).ToList();

            model.DataList = x;
            model.CompanyId = (int)companyid;
            return model;
        }


        public async Task<SalesManagementVM> GetMonthlyTarget(int KGCompanySaleTergetId)
        {

            SalesManagementVM Model = new SalesManagementVM();
            Model = await (from t1 in _dbContext.KGSalesAchivements
                           join t2 in _dbContext.KGCompanySaleTergets on t1.KGSalesId equals t2.KGSalesId
                           where t2.KGCompanySaleTergetId == KGCompanySaleTergetId && t1.IsActive == true
                           select new SalesManagementVM
                           {
                               TargetAmount = t2.TergetedAmount,
                               StartDate = t1.FromDate,
                               EndDate = t1.ToDate,
                               AchievementTitle = t1.Title,
                               CompanyId = t2.CompanyId
                           }).FirstOrDefaultAsync();

            Model.DataCmpanyWise = await (from t1 in _dbContext.KGCompanyMonthlySaleTergets
                                          join t2 in _dbContext.KGCompanySaleTergets on t1.KGCompanySaleTergetId equals t2.KGCompanySaleTergetId
                                          join t3 in _dbContext.Units on t1.UnitId equals t3.UnitId

                                          where t1.KGCompanySaleTergetId == KGCompanySaleTergetId && t1.IsActive == true
                                          select new KGCompanyMonthlySaleTergetVM
                                          {
                                              KGCompanySaleTergetId = t1.KGCompanySaleTergetId,
                                              KGCompanyMonthlySaleTergetId = t1.KGCompanyMonthlySaleTergetId,
                                              Month = t1.Month,
                                              MonthlySalesTergetedAmount = t1.MonthlySalesTergetedAmount,
                                              MonthlyCpllectionTergetedAmount = t1.MonthlyCollectionTergetedAmount,
                                              MonthlyTergetedQuantity = t1.MonthlyTergetedQuantity,
                                              Unitname = t3.Name,
                                          }).ToListAsync();




            foreach (var item in Model.DataCmpanyWise)
            {
            
                var salesDetails = await _dbContext.KGSalesAchivementDetails
                                                  .Where(y => y.KGCompanyMonthlySaleTergetId == item.KGCompanyMonthlySaleTergetId && y.IsActive)
                                                  .ToListAsync();

             
                decimal totalSaleAchivementAmount = 0;
                decimal totalSaleAchivementqty = 0;
                decimal totalCollectionAchivement = 0;
                foreach (var detail in salesDetails)
                {
                    totalSaleAchivementAmount += detail.SalesAchievementAmount;
                    totalSaleAchivementqty += detail.SalesAchievementQuantity;
                    totalCollectionAchivement += detail.RecoveryAchievementAmount;
                }

              
                item.SalesAchievementAmount = totalSaleAchivementAmount;
                item.SalesAchievementQty = totalSaleAchivementqty;
                item.recovery = totalCollectionAchivement;
            }








            Model.KGCompanySaleTergetId = KGCompanySaleTergetId;


           Model.Unit= _dbContext.Units.Where(x => x.IsActive == true && x.CompanyId==Model.CompanyId).Select(p => new SelectListItem
           {
               Value = p.UnitId.ToString(),
               Text = p.Name
           }).ToList();

            return Model;
        }
        public async Task<bool> SaveMonthlyTarget(List<TargetAmountViewModel> targetAmounts)
        {

            try
            {
                if (targetAmounts != null && targetAmounts.Count > 0)
                {
                    foreach (var item in targetAmounts)
                    {
                        KGCompanyMonthlySaleTerget kGCompanyMonthlySaleTergetVM = new KGCompanyMonthlySaleTerget();

                        kGCompanyMonthlySaleTergetVM.KGCompanySaleTergetId = item.KGCompanySaleTergetId;
                        kGCompanyMonthlySaleTergetVM.MonthlySalesTergetedAmount = item.MonthlySalesTergetedAmount;
                        kGCompanyMonthlySaleTergetVM.MonthlyTergetedQuantity = item.TergetedQty;
                        kGCompanyMonthlySaleTergetVM.MonthlyCollectionTergetedAmount = item.MonthlyCollectionTergetedAmount;
                        kGCompanyMonthlySaleTergetVM.Month = item.Month;
                        kGCompanyMonthlySaleTergetVM.CreatedBy = Common.GetUserId();
                        kGCompanyMonthlySaleTergetVM.CreatedDate = DateTime.Now;
                        kGCompanyMonthlySaleTergetVM.IsActive = true;
                        kGCompanyMonthlySaleTergetVM.UnitId = item.UnitId;

                        _dbContext.KGCompanyMonthlySaleTergets.Add(kGCompanyMonthlySaleTergetVM);
                    }

                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }


        }

        public async Task<bool> EditData(int id, decimal saleAmount, decimal saleQuantity,decimal collectionmount)
        {
            var obj = await _dbContext.KGCompanyMonthlySaleTergets.Where(c => c.KGCompanyMonthlySaleTergetId == id).FirstOrDefaultAsync();
            if (obj == null)
            {
                return false;
            }
            else
            {
                obj.MonthlySalesTergetedAmount = saleAmount;
                obj.MonthlyTergetedQuantity = saleQuantity;
                obj.MonthlyCollectionTergetedAmount = collectionmount;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                await _dbContext.SaveChangesAsync();

                return true;
            }


            return false;
        }

        public async Task<KGSalesAchivementDetailVm> GetOfficerAssign(int KGCompanyMonthlySaleTergetId, int CompanyId)
        {
            KGSalesAchivementDetailVm model = new KGSalesAchivementDetailVm();


            model = await (from t1 in _dbContext.KGSalesAchivements
                           join t2 in _dbContext.KGCompanySaleTergets on t1.KGSalesId equals t2.KGSalesId
                           join t3 in _dbContext.KGCompanyMonthlySaleTergets on t2.KGCompanySaleTergetId equals t3.KGCompanySaleTergetId
                           join t4 in _dbContext.Units on t3.UnitId equals t4.UnitId
                           where t3.KGCompanyMonthlySaleTergetId == KGCompanyMonthlySaleTergetId && t1.IsActive == true && t1.CompanyId== CompanyId
                           select new KGSalesAchivementDetailVm
                           {
                               MonthlySalesTergetedAmount = t3.MonthlySalesTergetedAmount,
                               MonthlyCollectionTergetedAmount = t3.MonthlyCollectionTergetedAmount,
                                
                               Title = t1.Title,
                               Month = t3.Month,
                               UnitName=t4.Name
                           }).FirstOrDefaultAsync();



            model.DataList = await (from t1 in _dbContext.KGSalesAchivementDetails
                                    join t2 in _dbContext.KGCompanyMonthlySaleTergets on t1.KGCompanyMonthlySaleTergetId equals t2.KGCompanyMonthlySaleTergetId
                                    join t3 in _dbContext.KGCompanySaleTergets on t2.KGCompanySaleTergetId equals t3.KGCompanySaleTergetId
                                    join t4 in _dbContext.KGSalesAchivements on t3.KGSalesId equals t4.KGSalesId
                                    join t5 in _dbContext.Employees on t1.EmployeeId equals t5.Id
                                    //join t6 in _dbContext.PRoll_SalaryConfiguration on t1.EmployeeId equals t6.EmployeeId
                                    join t7 in _dbContext.Units on t2.UnitId equals t7.UnitId
                                    where t1.KGCompanyMonthlySaleTergetId == KGCompanyMonthlySaleTergetId && t1.IsActive
                                    select new KGSalesAchivementDetailVm
                                    {
                                        KGSalesAchievementDetailsId = t1.KGSalesAchievementDetailsId,
                                        EmployeeId = t1.EmployeeId,
                                        KgId = t5.EmployeeId,
                                        EmName = t5.Name,
                                        //Salary = t6.Gross,
                                        SalesTargetQuantity = t1.SalesTargetQty,
                                        SalesTargetAount = t1.SalesTargetAmount,
                                        SalesAchievementAmount = t1.SalesAchievementAmount,
                                        SalesAchievementQuantity = t1.SalesAchievementQuantity,
                                        RecoveryAchievementAmount = t1.RecoveryAchievementAmount,
                                        UnitName=t7.Name,
                                        MonthlyCollectionTergetedAmount=t1.TargetCollectionAmount
                                        
                                    }).ToListAsync();

            return model;

        }

        public async Task<bool> SaveAchievementDetail(SalesAchievementDetailViewModel Achivement)
        {
            var obj = await _dbContext.KGSalesAchivementDetails.Where(x => x.KGSalesAchievementDetailsId == Achivement.KGSalesAchievementDetailsId).FirstOrDefaultAsync();

            if (obj == null)
            {
                return false;
            }
            else
            {
                obj.SalesAchievementAmount = Achivement.SalesAchievementAmount;
                obj.SalesAchievementQuantity = Achivement.SalesAchievementQuantity;
                obj.RecoveryAchievementAmount = Achivement.RecoveryAchievementAmount;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<KGSalesAchivementDetailVm> fixTarget(int KGCompanyMonthlySaleTergetId, int CompanyId)
        {
            KGSalesAchivementDetailVm model = new KGSalesAchivementDetailVm();


            model = await (from t1 in _dbContext.KGSalesAchivements
                           join t2 in _dbContext.KGCompanySaleTergets on t1.KGSalesId equals t2.KGSalesId
                           join t3 in _dbContext.KGCompanyMonthlySaleTergets on t2.KGCompanySaleTergetId equals t3.KGCompanySaleTergetId
                           join t4 in _dbContext.Units on t3.UnitId equals t4.UnitId
                           where t3.KGCompanyMonthlySaleTergetId == KGCompanyMonthlySaleTergetId && t1.IsActive == true
                           select new KGSalesAchivementDetailVm
                           {
                               MonthlySalesTergetedAmount = t3.MonthlySalesTergetedAmount,
                               Title = t1.Title,
                               Month = t3.Month,
                               SalesTargetQuantity = (decimal)t3.MonthlyTergetedQuantity,
                               UnitName=t4.Name,
                               MonthlyCollectionTergetedAmount=t3.MonthlyCollectionTergetedAmount

                               
                               

                           }).FirstOrDefaultAsync();





            model.offcierAssignsList = await (
                from t1 in _dbContext.OfficerAssigns
                join t2 in _dbContext.Employees on t1.EmpId equals t2.Id
                //join t3 in _dbContext.PRoll_SalaryConfiguration on t1.EmpId equals t3.EmployeeId
                where t1.CompanyId == CompanyId && t1.IsActive
                select new OffcierAssignVm
                {
                    EmployeeId = t2.EmployeeId,
                    EmployeeName = t2.Name,
                    Id = t2.Id,
                    //Salary = t3.Gross
                }).ToListAsync();


            var WithOutSavedPerson = await _dbContext.KGSalesAchivementDetails.Where(x => x.KGCompanyMonthlySaleTergetId == KGCompanyMonthlySaleTergetId && x.IsActive).Select(x => x.EmployeeId).ToListAsync();

            if (model.offcierAssignsList.Count() > 0)
            {
                model.offcierAssignsList = model.offcierAssignsList
      .Where(kg => !WithOutSavedPerson.Contains(kg.Id))
      .ToList();
            }








            //var totalsalary = model.offcierAssignsList.Sum(item => item.Salary);
            //foreach (var item in model.offcierAssignsList)
            //{
            //    if (totalsalary != 0)
            //    {
            //        var multiply = item.Salary * model.MonthlySalesTergetedAmount;
            //        item.EmpTarget = multiply / totalsalary;
            //        item.EmpTarget = Math.Round(item.EmpTarget, 2);
            //        var multyplyqty=item.Salary * model.SalesTargetQuantity;
            //        item.TargetQty = multyplyqty/ totalsalary;
            //        item.TargetQty = Math.Round(item.TargetQty, 2);
            //        var targetcollectionamount=item.Salary * model.MonthlyCollectionTergetedAmount;
            //        item.TargetCollection=targetcollectionamount/ totalsalary;
            //        item.TargetCollection= Math.Round(item.TargetCollection, 2);


            //    }
            //    else
            //    {
            //        item.EmpTarget = 0;
            //        item.TargetQty= 0;
            //    }
            //}
            return model;

        }






        public async Task<bool> SaveEmTarget(List<EmployeeTargetViewModel> empTargets)
        {
            try
            {
                foreach (var empTarget in empTargets)
                {

                    var existingRecord = await _dbContext.KGSalesAchivementDetails
                        .FirstOrDefaultAsync(x => x.KGCompanyMonthlySaleTergetId == empTarget.KGCompanyMonthlySaleTergetId && x.EmployeeId == empTarget.EmployeeId && x.IsActive);

                    if (existingRecord == null)
                    {

                        KGSalesAchivementDetail newRecord = new KGSalesAchivementDetail()
                        {
                            KGCompanyMonthlySaleTergetId = empTarget.KGCompanyMonthlySaleTergetId,
                            EmployeeId = empTarget.EmployeeId,
                            GrossSalleryAmount = empTarget.SalaryAmount,
                            SalesAchievementAmount = 0,
                            RecoveryAchievementAmount = 0,
                            SalesAchievementQuantity = 0,
                            IsActive = true,
                            CreatedBy = Common.GetUserId(),
                            CreatedDate = DateTime.Now,
                            SalesTargetAmount = empTarget.EmpTarget,
                            SalesTargetQty = empTarget.TargetQty,
                            TargetCollectionAmount=empTarget.TargetCollection,
                            CompanyId= empTarget.CompanyId


                        };

                        _dbContext.KGSalesAchivementDetails.Add(newRecord);
                    }
                    else
                    {

                        return false;
                    }
                }
                await _dbContext.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                var messsage = ex;
            }
            return true;
        }



        public async Task<SalesManagementVM> GetCompanyWiseSalesAchievementsPerSon(int CompanyId)
        {
            long userId = Common.GetIntUserId();

            SalesManagementVM model = new SalesManagementVM();
            var employee = await _dbContext.Employees.Where(y => y.Id == userId).FirstOrDefaultAsync();

            var obj = await _dbContext.OfficerAssigns.Where(z => z.EmpId == employee.Id).FirstOrDefaultAsync();
            if (obj != null)
            {



                var x = (from t1 in _dbContext.KGSalesAchivements
                         join t2 in _dbContext.KGCompanySaleTergets on t1.KGSalesId equals t2.KGSalesId
                         where t2.CompanyId == CompanyId && t2.IsActive == true && t1.IsActive == true
                         select new SalesManagementVM
                         {
                             AchievementId = t2.KGSalesId,
                             AchievementTitle = t1.Title,
                             TargetAmount = t2.TergetedAmount,
                             KGCompanySaleTergetId = t2.KGCompanySaleTergetId

                         }).ToList();
                model.DataList = x;
                model.CompanyId = CompanyId;
                return model;

            }
            else
            {
                return model;

            }



        }



        public async Task<KGSalesAchivementDetailVm> GetOfficerAssignPersonWise(int KGSalesTargetTergetId, int CompanyId)
        {
            KGSalesAchivementDetailVm model = new KGSalesAchivementDetailVm();
            long userId = Common.GetIntUserId();
            var employee = await _dbContext.Employees.Where(y => y.Id == userId).FirstOrDefaultAsync();
            ;

            model = await (from t1 in _dbContext.KGSalesAchivements
                           join t2 in _dbContext.KGCompanySaleTergets on t1.KGSalesId equals t2.KGSalesId
                           where t2.KGCompanySaleTergetId == KGSalesTargetTergetId && t1.IsActive == true
                           select new KGSalesAchivementDetailVm
                           {
                             
                               Title = t1.Title,
                           }).FirstOrDefaultAsync();



            model.DataList = await (from t1 in _dbContext.KGSalesAchivementDetails
                                    join t2 in _dbContext.KGCompanyMonthlySaleTergets on t1.KGCompanyMonthlySaleTergetId equals t2.KGCompanyMonthlySaleTergetId
                                    join t3 in _dbContext.KGCompanySaleTergets on t2.KGCompanySaleTergetId equals t3.KGCompanySaleTergetId
                                    join t4 in _dbContext.KGSalesAchivements on t3.KGSalesId equals t4.KGSalesId
                                    join t5 in _dbContext.Employees on t1.EmployeeId equals t5.Id
                                    join t6 in _dbContext.PRoll_SalaryConfiguration on t1.EmployeeId equals t6.EmployeeId
                                    where t2.KGCompanySaleTergetId == KGSalesTargetTergetId && t1.IsActive && t1.EmployeeId == employee.Id
                                    select new KGSalesAchivementDetailVm
                                    {
                                        KGSalesAchievementDetailsId = t1.KGSalesAchievementDetailsId,
                                        EmployeeId = t1.EmployeeId,
                                        KgId = t5.EmployeeId,
                                        EmName = t5.Name,
                                        Month = t2.Month,
                                        Salary = t6.Gross,
                                        SalesTargetQuantity = t1.SalesTargetQty,
                                        SalesTargetAount = t1.SalesTargetAmount,
                                        SalesAchievementAmount = t1.SalesAchievementAmount,
                                        SalesAchievementQuantity = t1.SalesAchievementQuantity,
                                        RecoveryAchievementAmount = t1.RecoveryAchievementAmount,
                                        MonthlyCollectionTergetedAmount=t1.TargetCollectionAmount

                                    }).OrderBy(o => o.KGSalesAchievementDetailsId).ToListAsync();

            return model;

        }


        public async Task<SalesManagementVM> GetSalesTargetAsync(long salesTargetId)
        {
            try
            {
                var target = await _dbContext.KGCompanySaleTergets.FindAsync(salesTargetId);
                if (target != null)
                {
                    return new SalesManagementVM
                    {
                        KGcomTargetId = target.KGCompanySaleTergetId,
                        AchievementId = target.KGSalesId,
                        CompanyId = target.CompanyId,
                        TargetAmount = target.TergetedAmount
                    };
                }
                else
                {
                    throw new Exception("Sales target not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching sales target: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteSalesTargetAsync(long salesTargetId)
        {
            var target = _dbContext.KGCompanySaleTergets.Find(salesTargetId);
            if (target != null)
            {
                target.IsActive = false;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Sales target not found.");
            }
        }
        public async Task<long> UpdateCompanySalesTargetAsync(SalesManagementVM model)
        {
            try
            {
                var st = _dbContext.KGCompanySaleTergets.Find(model.KGcomTargetId);
                if (st != null)
                {
                    st.KGSalesId = model.AchievementId;
                    st.CompanyId = model.CompanyId;
                    st.TergetedAmount = model.TargetAmount;
                    st.ModifiedBy = Common.GetUserId();
                    await _dbContext.SaveChangesAsync();
                    return st.KGCompanySaleTergetId;
                }
                else
                {
                    throw new Exception("Sales Target not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error");
            }
        }

       

        public async Task<bool> SavePersonWeeklyAchivement(KGSalesCollectedAchivementVm Model)
        {


            try
            {
                KGSalesCollectedAchivement vm = new KGSalesCollectedAchivement()
                {
                    KGSalesAchievementDetailsId = Model.KGSalesAchievementDetailsId,
                    SalesAchievementAmount = Model.SalesAchievementAmount,
                    SalesAchievementQuantity = Model.SalesAchievementQuantity,
                    RecoveryAchievementAmount = Model.RecoveryAchievementAmount,
                    AchivementDate = Model.AchivementDate,
                    IsActive = true,
                    CreatedBy = Common.GetUserId(),
                    CreatedDate = DateTime.Now

                };
                _dbContext.KGSalesCollectedAchivements.Add(vm);
                await _dbContext.SaveChangesAsync();

                var obj = await _dbContext.KGSalesAchivementDetails.Where(x => x.KGSalesAchievementDetailsId == Model.KGSalesAchievementDetailsId && x.IsActive == true).FirstOrDefaultAsync();
                if (obj != null)
                {
                    obj.SalesAchievementAmount = obj.SalesAchievementAmount + Model.SalesAchievementAmount;
                    obj.SalesAchievementQuantity = obj.SalesAchievementQuantity + Model.SalesAchievementQuantity;
                    obj.RecoveryAchievementAmount = obj.RecoveryAchievementAmount + Model.RecoveryAchievementAmount;
                    obj.ModifiedBy = Common.GetUserId();
                    obj.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    await _dbContext.SaveChangesAsync();

                }
                return true;

            }catch(Exception ex)
            {
                var message = ex;
            }




            return false;
        }
        
        public async Task<KGSalesCollectedAchivementVm> GetWeeklyAchivement(long CurentId)
        {
            KGSalesCollectedAchivementVm vm =new KGSalesCollectedAchivementVm();

           vm.Details= await (from t1 in _dbContext.KGSalesCollectedAchivements
                            where t1.KGSalesAchievementDetailsId==CurentId && t1.IsActive == true
                            select new KGSalesCollectedAchivementVm
                            {    KGSalesCollectedAchivementId = t1.KGSalesCollectedAchivementId,
                                SalesAchievementAmount = t1.SalesAchievementAmount,
                                SalesAchievementQuantity = t1.SalesAchievementQuantity,
                                RecoveryAchievementAmount = t1.RecoveryAchievementAmount,
                                AchivementDate=t1.AchivementDate
                            }).ToListAsync();
            return vm;

        }

        public async Task<bool> DeleteWeeklyAchivement(long CurentId,long achievementId)
        {
            var obj = await _dbContext.KGSalesCollectedAchivements.Where(x => x.KGSalesCollectedAchivementId == achievementId).FirstOrDefaultAsync();
            if (obj != null) 
            {
                obj.IsActive= false;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                await _dbContext.SaveChangesAsync();

                var obj2 = await _dbContext.KGSalesAchivementDetails.Where(x => x.KGSalesAchievementDetailsId == CurentId).FirstOrDefaultAsync();
                if (obj!=null)
                {
                    obj2.SalesAchievementAmount= obj2.SalesAchievementAmount-obj.SalesAchievementAmount;
                    obj2.SalesAchievementQuantity= obj2.SalesAchievementQuantity-obj.SalesAchievementQuantity;
                    obj2.RecoveryAchievementAmount= obj2.RecoveryAchievementAmount-obj.RecoveryAchievementAmount;
                    obj2.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    obj2.ModifiedBy = Common.GetUserId();
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }

                return true;

            }


            return false;

        }


        public async Task<bool> UpdateAchivement(KGSalesCollectedAchivementVm model)
        {

            var obj= await _dbContext.KGSalesCollectedAchivements.Where(x => x.KGSalesCollectedAchivementId == model.KGSalesCollectedAchivementId).FirstOrDefaultAsync();

            if (obj != null)
            {
                obj.SalesAchievementAmount = model.SalesAchievementAmount;
                obj.SalesAchievementQuantity= model.SalesAchievementQuantity;
                obj.RecoveryAchievementAmount=model.RecoveryAchievementAmount;
                obj.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                obj.ModifiedBy = Common.GetUserId();
                await _dbContext.SaveChangesAsync();
            }
            return false;
        }





    }

}
        



