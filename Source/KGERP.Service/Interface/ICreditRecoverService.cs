using KGERP.Data.CustomModel;
using KGERP.Service.ServiceModel;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Interface
{
    public interface ICreditRecoverService
    {
        IQueryable<CreditRecoverModel> GetCreditRecovers(int companyId, string searchValue, out int count);
        IQueryable<CreditRecoverModel> GetCompanyCreditRecovers(int companyId, string searchValue, out int count);
        CreditRecoverModel GetCreditRecover(long id);
        bool SaveCreditRecover(long creditRecoverId, CreditRecoverModel creditRecover, out string message);
        CreditRecoverModel GetSingleCreditRecover(int creditRecoverId);
        IQueryable<CreditRecoverDetailModel> GetCreditRecoverDetails(long creditRecoverId, string searchValue, out int count);
        CreditRecoverDetailModel GetCreditRecoverDetail(long id);
        bool SaveCreditRecoverDetail(long? creditRecoverId, CreditRecoverDetailModel model, out string message);
        bool DetailDelete(long id);
        List<MonthlyTargetCM> GetMonthlyTargetReport();
        List<MonthlyTargetCM> GetCompanyMonthlyTargetReport(int companyId);
        List<MonthlyTargetCM> GetCompanyMonthlyTargetDetailReport(int monthNo, int yearNo, int companyId);
        List<MonthlyTargetCM> GetMonthlyTargetDetailReport(int monthNo, int yearNo);
    }
}
