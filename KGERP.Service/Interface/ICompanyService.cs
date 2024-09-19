using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Interface
{
    public interface ICompanyService
    {
        List<CompanyModel> GetCompanies(string searchText);
        List<SelectModel> GetCompanySelectModels();
        List<SelectModel> GetCompanySelectModelsForKSSl(int CompanyId);
        List<SelectModel> GetKGRECompnay();
        List<SelectModel> GetFilterCompanySelectModels();
        CompanyModel GetCompany(int id);
       
        bool SaveCompany(int companyId, CompanyModel model);
        List<SelectModel> GetAllCompanySelectModels();
        List<SelectModel> GetAllCompanySelectModels2();
        List<SelectModel> GetCompanySelectModelById(int companyId);
        List<SelectModel> GetSaleYearSelectModel();

        bool SaveBusinessHead(BusinessHeadModel model);
        bool UpdateBusinessHead(BusinessHeadModel model);
        bool RemoveBusinessHead(int id);
        List<BusinessHeadModel> GetAllBusinessHead();
        BusinessHeadModel GetBusinessHead(int id);

        Task<List<SelectListItem>> GetDDLPayrollShit(int ComId,CancellationToken cancellationToken=default);
        Task<long?> GetPayrollSheetIdByEmpId(long EmpId,CancellationToken cancellationToken=default);
    }
}
