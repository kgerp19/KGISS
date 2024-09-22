using KGERP.Data.Models;
using KGERP.Service.Implementation.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.IncentivesDistribution
{
    public interface IIncentiveConfig
    {
        List<SelectModelType> Catagorieslist(int companyId);
        IncentiveViewModel AddChart(IncentiveViewModel model);
        bool UpdateChart(IncentiveViewModel model);
        IncentiveViewModel ChartList(int companyId);
        IncentiveDistributionDetailVm GetList(int Id);
        IncentiveDistributionDetailVm GetListForAddNewIncentive(int Id, int companyId);

        IncentiveDistributionDetailVm GetorEditIncentive(long Id);
        IncentiveDistributionChart GetCatagory(int id);
        IncentiveDistributionChart Getexit(IncentiveViewModel model);
        double GetSum(int companyId, int id);
        IncentiveDistributionChart GetBy(int id);
        bool Delete(int id);
        bool objtosave(IncentiveDistributionDetailVm model);
        bool UpdateIncentive(IncentiveDistributionDetail model);
        bool Deleteincentivebyid(long id);
        IncentiveCateGoryViewModel GetIncentiveCateGoryList(int CompanyId);
        bool AddOrUpdateCategoryIncentive(IncentiveCatagory model);
        IncentiveCateGoryViewModel GetEditItem(int id);
        bool DeleteIncentivecategory(int id);
    }
}
