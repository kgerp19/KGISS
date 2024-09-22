using KGERP.Data.CustomModel;
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
    public class CreditRecoverService : ICreditRecoverService
    {
        private readonly ERPEntities context;
        public CreditRecoverService(ERPEntities context)
        {
            this.context = context;
        }
        public IQueryable<CreditRecoverModel> GetCreditRecovers(int companyId, string searchValue, out int count)
        {
            count = context.Database.SqlQuery<int>("select count(CreditRecoverId) from CreditRecover").First();
            return context.Database.SqlQuery<CreditRecoverModel>(@"exec spGetCreditRecoverSearch {0}", searchValue).AsQueryable();
        }

        public IQueryable<CreditRecoverModel> GetCompanyCreditRecovers(int companyId, string searchValue, out int count)
        {
            count = context.Database.SqlQuery<int>("select count(CreditRecoverId) from CreditRecover where companyId =" + companyId + "").First();
            return context.Database.SqlQuery<CreditRecoverModel>(@"exec spGetCompanyCreditRecover {0},{1}", searchValue, companyId).AsQueryable();
        }

        public CreditRecoverModel GetCreditRecover(long id)
        {
            if (id == 0)
            {
                return new CreditRecoverModel()
                {
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]),
                    IsActive = true
                };
            }
            CreditRecover creditRecover = context.CreditRecovers.Find(id);
            return ObjectConverter<CreditRecover, CreditRecoverModel>.Convert(creditRecover);
        }

        public bool SaveCreditRecover(long id, CreditRecoverModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("CreditRecover data missing!");
            }

            bool exist = context.CreditRecovers.Where(x => x.CompanyId == model.CompanyId && x.VendorId == model.VendorId && x.CreditRecoverId != id).Any();

            if (exist)
            {
                message = "This customer already exists in the credit recovery list !";
                return false;
            }
            CreditRecover creditRecover = ObjectConverter<CreditRecoverModel, CreditRecover>.Convert(model);
            if (id > 0)
            {
                creditRecover = context.CreditRecovers.FirstOrDefault(x => x.CreditRecoverId == id);
                if (creditRecover == null)
                {
                    throw new Exception("Customer not found!");
                }
                creditRecover.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                creditRecover.ModifiedDate = DateTime.Now;
            }

            else
            {
                creditRecover.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                creditRecover.CreatedDate = DateTime.Now;
            }

            creditRecover.CompanyId = model.CompanyId;
            creditRecover.VendorId = model.VendorId;
            creditRecover.Amount = model.Amount;
            creditRecover.StartDate = model.StartDate;
            creditRecover.Remarks = model.Remarks;
            creditRecover.IsActive = model.IsActive;


            context.Entry(creditRecover).State = creditRecover.CreditRecoverId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteFarmer(int id)
        {
            Farmer farmer = context.Farmers.Find(id);
            if (farmer == null)
            {
                return false;
            }
            context.Farmers.Remove(farmer);
            return context.SaveChanges() > 0;
        }

        public CreditRecoverModel GetSingleCreditRecover(int creditRecoverId)
        {

            return context.Database.SqlQuery<CreditRecoverModel>(@"select     CreditRecoverId,
                                                                              (select Name from Company where CompanyId=CreditRecover.CompanyId) as CompanyName,
                                                                              (select Name from Erp.Vendor where VendorId=CreditRecover.VendorId) as CustomerName
                                                                   from       CreditRecover
                                                                   where      CreditRecoverId={0}", creditRecoverId).FirstOrDefault();

        }


        public IQueryable<CreditRecoverDetailModel> GetCreditRecoverDetails(long creditRecoverId, string searchValue, out int count)
        {
            count = context.Database.SqlQuery<int>("select count(CreditRecoverDetailId) from CreditRecoverDetail where CreditRecoverId={0}", creditRecoverId).First();
            return context.Database.SqlQuery<CreditRecoverDetailModel>(@"exec spGetCreditRecoverDetailSearch {0},{1}", creditRecoverId, searchValue).AsQueryable();
        }

        public CreditRecoverDetailModel GetCreditRecoverDetail(long id)
        {
            if (id == 0)
            {
                return new CreditRecoverDetailModel();

            }
            CreditRecoverDetail creditRecoverDetail = context.CreditRecoverDetails.Find(id);
            return ObjectConverter<CreditRecoverDetail, CreditRecoverDetailModel>.Convert(creditRecoverDetail);
        }

        public bool SaveCreditRecoverDetail(long? creditRecoverDetailId, CreditRecoverDetailModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                message = "Credit Recover Detail data missing!";
                return false;
            }

            bool isStartDateExceedPaymentDate = context.CreditRecovers.Where(x => x.CreditRecoverId == model.CreditRecoverId).First().StartDate > model.RecoverDate;
            if (isStartDateExceedPaymentDate)
            {
                message = "Payment date always comes after start date !";
                return false;
            }
            CreditRecoverDetail creditRecoverDetail = ObjectConverter<CreditRecoverDetailModel, CreditRecoverDetail>.Convert(model);
            if (creditRecoverDetailId > 0)
            {
                creditRecoverDetail = context.CreditRecoverDetails.FirstOrDefault(x => x.CreditRecoverDetailId == creditRecoverDetailId);
                if (creditRecoverDetail == null)
                {
                    message = "Credit Recover Detail not found!";
                    return false;
                }
            }

            else
            {
                //creditRecover.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                //creditRecover.CreatedDate = DateTime.Now;
            }

            creditRecoverDetail.CreditRecoverId = model.CreditRecoverId;
            creditRecoverDetail.RecoverDate = model.RecoverDate;
            creditRecoverDetail.RecoverAmount = model.RecoverAmount;
            creditRecoverDetail.Note = model.Note;

            context.Entry(creditRecoverDetail).State = creditRecoverDetail.CreditRecoverDetailId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DetailDelete(long id)
        {
            CreditRecoverDetail creditRecoverDetail = context.CreditRecoverDetails.Find(id);
            if (creditRecoverDetail == null)
            {
                return false;
            }
            context.CreditRecoverDetails.Remove(creditRecoverDetail);
            return context.SaveChanges() > 0;
        }

        public List<MonthlyTargetCM> GetMonthlyTargetReport()
        {
            return context.Database.SqlQuery<MonthlyTargetCM>(@"exec spGetMonthlyCreditRecover").ToList();
        }

        public List<MonthlyTargetCM> GetCompanyMonthlyTargetReport(int companyId)
        {
            return context.Database.SqlQuery<MonthlyTargetCM>(@"exec spGetCompanyMonthlyCreditRecover {0}", companyId).ToList();
        }

        public List<MonthlyTargetCM> GetMonthlyTargetDetailReport(int monthNo, int yearNo)
        {
            return context.Database.SqlQuery<MonthlyTargetCM>(@"exec spGetCreditRecoverDetailReport {0},{1}", monthNo, yearNo).ToList();
        }


        public List<MonthlyTargetCM> GetCompanyMonthlyTargetDetailReport(int monthNo, int yearNo, int companyId)
        {
            return context.Database.SqlQuery<MonthlyTargetCM>(@"exec GetCompanyMonthlyTargetDetailReport {0},{1},{2}", monthNo, yearNo, companyId).ToList();
        }
    }
}
