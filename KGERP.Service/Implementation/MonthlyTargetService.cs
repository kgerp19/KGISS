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
    public class MonthlyTargetService : IMonthlyTargetService
    {
        private readonly ERPEntities context;
        public MonthlyTargetService(ERPEntities context)
        {
            this.context = context;
        }

        public List<MonthlyTargetModel> GetMonthlyTargets(string searchText)
        {
            IQueryable<MonthlyTarget> queryable = context.MonthlyTargets.Include(x => x.Company).Where(x => x.Company.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText));
            return queryable.ToList().Select(x => new MonthlyTargetModel
            {
                MonthlyTargetId = x.MonthlyTargetId,
                CompanyName = x.Company.Name,
                Year = x.Year,
                MonthName = new DateTime(x.Year, x.Month, 1).ToString("MMMM"),
                Amount = x.Amount,
                Remarks = x.Remarks,
                IsActive = x.IsActive
            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ThenBy(x => x.CompanyName).ToList();
        }

        public List<MonthlyTargetModel> GetMonthlyCompanyTargets(string searchText, int companyId)
        {
            dynamic result = context.Database.SqlQuery<MonthlyTargetModel>(@"exec spGetMonthlyCompanyTargets {0}", companyId).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ThenBy(x => x.CompanyName).ToList();
            return result;
            //IQueryable<MonthlyTarget> queryable = context.MonthlyTargets.Include(x => x.Company).Where(x => (x.CompanyId == companyId) && x.Company.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText));
            //return queryable.ToList().Select(x => new MonthlyTargetModel
            //{
            //    MonthlyTargetId = x.MonthlyTargetId,
            //    CompanyName = x.Company.Name,
            //    CompanyId = x.Company.CompanyId,
            //    //CustomerName = x.Vendor.Name,
            //    //CustomerId = x.Vendor.VendorId,
            //    //VendorType = x.Vendor.VendorTypeId,
            //    Year = x.Year,
            //    MonthName = new DateTime(x.Year, x.Month, 1).ToString("MMMM"),
            //    Amount = x.Amount,
            //    Remarks = x.Remarks,
            //    IsActive = x.IsActive
            //}).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ThenBy(x => x.CompanyName).ToList();

        }

        public MonthlyTargetModel GetMonthlyTarget(int id)
        {
            if (id <= 0)
            {
                return new MonthlyTargetModel() { MonthlyTargetId = id, IsActive = true };
            }
            MonthlyTarget monthlyTarget = context.MonthlyTargets.FirstOrDefault(x => x.MonthlyTargetId == id);

            return ObjectConverter<MonthlyTarget, MonthlyTargetModel>.Convert(monthlyTarget);
        }

        public bool SaveMonthlyTarget(int id, MonthlyTargetModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                message = "Monthly Target  data missing!";
                return false;
            }
            var exist = context.MonthlyTargets.Where(x => x.CompanyId == model.CompanyId && x.Year.Equals(model.Year) && x.Month.Equals(model.Month) && x.CustomerId == model.CustomerId).FirstOrDefault();

            if (exist != null)
            {
                message = "Monthly Target data aleray exist!";
                return false;
            }

            bool isDueExist = context.CreditRecovers.Where(x => x.CompanyId == model.CompanyId).Any();
            if (!isDueExist)
            {
                message = "Monthly Target can not be set. Please make a due against company";
                return false;
            }
            decimal dueAmount = context.CreditRecovers.Where(x => x.CompanyId == model.CompanyId).Sum(x => x.Amount);
            decimal collectionAmount = context.Database.SqlQuery<decimal>(@"select  isnull(sum(RecoverAmount),0) 
                                                                            from    CreditRecoverDetail
                                                                            where   CreditRecoverId in (select CreditRecoverId from CreditRecover where CompanyId = {0})", model.CompanyId).FirstOrDefault();

            decimal balance = dueAmount - collectionAmount;

            bool isMonthlyTargetGraterThanDue = model.Amount > balance;
            if (isMonthlyTargetGraterThanDue)
            {
                message = "Monthly Target can not exceed  due amount against the company";
                return false;
            }

            MonthlyTarget monthlyTarget = ObjectConverter<MonthlyTargetModel, MonthlyTarget>.Convert(model);
            if (id > 0)
            {
                monthlyTarget = context.MonthlyTargets.FirstOrDefault(x => x.MonthlyTargetId == id);
                if (monthlyTarget == null)
                {
                    message = "Monthly Target data not found!";
                    return false;
                }
                monthlyTarget.ModifiedDate = DateTime.Now;
                monthlyTarget.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                monthlyTarget.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                monthlyTarget.CreatedDate = DateTime.Now;

            }

            monthlyTarget.CompanyId = model.CompanyId;
            monthlyTarget.CustomerId = model.CustomerId;
            monthlyTarget.Year = model.Year;
            monthlyTarget.Month = model.Month;
            monthlyTarget.Amount = model.Amount;
            monthlyTarget.CustAmount = model.CustAmount;
            monthlyTarget.Remarks = model.Remarks;
            monthlyTarget.IsActive = model.IsActive;
            context.Entry(monthlyTarget).State = monthlyTarget.MonthlyTargetId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteMonthlyTarget(int monthlyTargetId)
        {
            MonthlyTarget monthlyTarget = context.MonthlyTargets.Where(x => x.MonthlyTargetId == monthlyTargetId).FirstOrDefault();
            if (monthlyTarget == null)
            {
                return false;
            }

            context.MonthlyTargets.Remove(monthlyTarget);
            return context.SaveChanges() > 0;
        }

        #region // Company wise

        public List<MonthlyTargetModel> GetCompanyMonthlyTargets(string searchText)
        {
            IQueryable<MonthlyTarget> queryable = context.MonthlyTargets.Include(x => x.Company).Where(x => x.Company.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText));
            return queryable.ToList().Select(x => new MonthlyTargetModel
            {
                MonthlyTargetId = x.MonthlyTargetId,
                CompanyName = x.Company.Name,
                Year = x.Year,
                MonthName = new DateTime(x.Year, x.Month, 1).ToString("MMMM"),
                Amount = x.Amount,
                Remarks = x.Remarks,
                IsActive = x.IsActive
            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ThenBy(x => x.CompanyName).ToList();
        }

        public MonthlyTargetModel GetCompanyMonthlyTarget(int id)
        {
            if (id <= 0)
            {
                return new MonthlyTargetModel() { MonthlyTargetId = id, IsActive = true };
            }
            MonthlyTarget monthlyTarget = context.MonthlyTargets.FirstOrDefault(x => x.MonthlyTargetId == id);
            return ObjectConverter<MonthlyTarget, MonthlyTargetModel>.Convert(monthlyTarget);
        }

        public bool SaveCompanyMonthlyTarget(int id, MonthlyTargetModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                message = "Monthly Target  data missing!";
                return false;
            }
            bool exist = context.MonthlyTargets.Where(x => x.CompanyId == model.CompanyId && x.Year.Equals(model.Year) && x.Month.Equals(model.Month) && x.MonthlyTargetId != id).Any();

            if (exist)
            {
                message = "Monthly Target data aleray exist!";
                return false;
            }

            bool isDueExist = context.CreditRecovers.Where(x => x.CompanyId == model.CompanyId).Any();
            if (!isDueExist)
            {
                message = "Monthly Target can not be set. Please make a due against company";
                return false;
            }
            decimal dueAmount = context.CreditRecovers.Where(x => x.CompanyId == model.CompanyId).Sum(x => x.Amount);
            decimal collectionAmount = context.Database.SqlQuery<decimal>(@"select  isnull(sum(RecoverAmount),0) 
                                                                            from    CreditRecoverDetail
                                                                            where   CreditRecoverId in (select CreditRecoverId from CreditRecover where CompanyId = {0})", model.CompanyId).FirstOrDefault();

            decimal balance = dueAmount - collectionAmount;

            bool isMonthlyTargetGraterThanDue = model.Amount > balance;
            if (isMonthlyTargetGraterThanDue)
            {
                message = "Monthly Target can not exceed  due amount against the company";
                return false;
            }

            MonthlyTarget monthlyTarget = ObjectConverter<MonthlyTargetModel, MonthlyTarget>.Convert(model);
            if (id > 0)
            {
                monthlyTarget = context.MonthlyTargets.FirstOrDefault(x => x.MonthlyTargetId == id);
                if (monthlyTarget == null)
                {
                    message = "Monthly Target data not found!";
                    return false;
                }
                monthlyTarget.ModifiedDate = DateTime.Now;
                monthlyTarget.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                monthlyTarget.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                monthlyTarget.CreatedDate = DateTime.Now;
            }

            monthlyTarget.CompanyId = model.CompanyId;
            monthlyTarget.CustomerId = model.CustomerId;
            monthlyTarget.Year = model.Year;
            monthlyTarget.Month = model.Month;
            monthlyTarget.Amount = model.Amount;
            monthlyTarget.CustAmount = model.CustAmount;
            monthlyTarget.Remarks = model.Remarks;
            monthlyTarget.IsActive = model.IsActive;
            context.Entry(monthlyTarget).State = monthlyTarget.MonthlyTargetId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteCompanyMonthlyTarget(int monthlyTargetId)
        {
            MonthlyTarget monthlyTarget = context.MonthlyTargets.Where(x => x.MonthlyTargetId == monthlyTargetId).FirstOrDefault();
            if (monthlyTarget == null)
            {
                return false;
            }

            context.MonthlyTargets.Remove(monthlyTarget);
            return context.SaveChanges() > 0;
        }

        #endregion
        public bool DeleteUpazila(int id)
        {
            Upazila upazila = context.Upazilas.Find(id);
            context.Upazilas.Remove(upazila);
            return context.SaveChanges() > 0;
        }

    }
}
