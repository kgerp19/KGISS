using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IOfficerAssignService
    {
        List<OfficerAssignModel> GetOfficerAssigns(int companyId, string searchText);
        Task<OfficerAssignModel> OfficersAssign(int CompanyId);
        Task<OfficerAssignModel> OfficersAssignFoeNewsaleeslist(int CompanyId);
       bool DeleteOfficerNew (int Id);
        OfficerAssignModel GetOfficerAssign(int id);
        OfficerAssign UpdateOfficer(OfficerAssignModel model);
        OfficerAssign Assignpesron(OfficerAssignModel model);
        OfficerAssign AssignpesronSalePersn(OfficerAssignModel model);
        OfficerAssignModel UpdateMarketingOfficer(int Id,int companyId);
        bool SaveOfficerAssign(int id, OfficerAssignModel officerAssign);
        List<SelectModel> GetEmployyeKGRE(int CompanyId);
        string GetOffierName(long EmpId);
        bool DeleteOfficerAssign(int id);
        List<LongSelectModel> GetMarketingOfficersByCustomerZone(int customerId);
        List<SelectModel> GetOfficerSelectModelsByZone(int zoneId);
        List<SelectModel> GetMarketingOfficersSelectModels(int companyId);
        List<SelectModel> GetZoneList(int companyId);
        List<SelectModel> GetSubZoneList(int Id);
        List<SelectModel> GetMarketingOfficerSelectModelsFromOrderMaster(int companyId);
    }
}
