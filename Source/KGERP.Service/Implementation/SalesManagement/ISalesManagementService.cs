using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.SalesManagement
{
    public interface ISalesManagementService
    {
        Task<long> SaveSalesAchievementAsync(SalesManagementVM model);
        Task<long> SaveKGCompanySaleTarget(SalesManagementVM model);
        Task<long> UpdateAchievementAsync(SalesManagementVM model);
        Task<long> UpdateCompanySalesTargetAsync(SalesManagementVM model);
        Task<List<SalesManagementVM>> GetAllAchievementsAsync();
        Task DeleteAchievementAsync(long achievementId);
        IEnumerable<SelectListItem> GetDDLSalesAchievements();
        IEnumerable<SelectListItem> GetDDLCompany(int companyId);
        Task<SalesManagementVM> GetSaleswiseTarget(int achievementId);       
        Task<SalesManagementVM> GetMonthlyTarget(int KGCompanySaleTergetId);
 
        Task<bool> SaveMonthlyTarget(List<TargetAmountViewModel> targetAmounts);
        Task<bool> EditData(int id, decimal saleAmount, decimal saleQuantity, decimal collectionAmount);
        Task<KGSalesAchivementDetailVm> GetOfficerAssign(int KGCompanyMonthlySaleTergetId,int CompanyId);
        Task<bool> SaveAchievementDetail(SalesAchievementDetailViewModel Achivement);
        Task<KGSalesAchivementDetailVm> fixTarget(int KGCompanyMonthlySaleTergetId, int CompanyId);
        Task<bool> SaveEmTarget(List<EmployeeTargetViewModel> empTargets);
        Task<SalesManagementVM> GetCompanyWiseSalesAchievementsPerSon(int CompanyId);
        Task<KGSalesAchivementDetailVm> GetOfficerAssignPersonWise(int KGSalesTargetTergetId, int CompanyId);
        Task<SalesManagementVM> GetCompanyWiseSalesAchievements(int companyid);
        Task<SalesManagementVM> GetAchievementAsync(long achievementId);
        Task DeleteSalesTargetAsync(long salesTargetId);
        Task<SalesManagementVM> GetSalesTargetAsync(long salesTargetId);
         Task<bool> SavePersonWeeklyAchivement(KGSalesCollectedAchivementVm Model);
        Task<KGSalesCollectedAchivementVm> GetWeeklyAchivement(long CurentId);
        Task<bool> DeleteWeeklyAchivement(long CurentId, long achievementId);
        Task<bool> UpdateAchivement(KGSalesCollectedAchivementVm model);
    }
}
