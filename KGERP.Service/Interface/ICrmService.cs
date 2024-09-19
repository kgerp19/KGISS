using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static KGERP.Utility.Util.PermissionCollection.Crms;

namespace KGERP.Service.Interface
{
    public interface ICrmService
    {
        MainMenuListVm GetAllMenu(int companyId);
        Task<CrmViewModel> GetIndex(int companyId, int userId);
        //   Task<CrmListVm> GetAllClient(int companyId,string searchValue,int pageSize,int skip,string sortColumn,string sortColumnDir);
        Task<CrmListVm> GetAllClient(int companyId, int userId);

        //Task<CrmListVm> GetUserClient(int companyId, int userId);
        Task<CrmListVm> GetUserClient(int companyId,int pageNumber, int pageSize, int userId,string searchText);
     

        Task<CrmVm> GetClientById(int clientId);
        Task<CrmVm> GetClientCopyById(int clientId);
        Task<CrmVm> CopyClientSave(int clientId, int selectedCompanyId);

        Task<CrmScheduleVm> MakeSchedule(CrmScheduleVm model);
        Task<CrmSchedule> SaveTask(int scheduleId, int SelectedCompanyId);
        Task<CrmScheduleListVm> GetScheduleByClientId(int clientId, int userId, int companyId);
        Task<CrmScheduleListVm> UpdateClientById(CrmScheduleListVm model);
        Task<List<SelectVm>> AllResponsibleoffficerrr(int CompanyId);
        Task<CrmViewModel> Countingupdate(CrmViewModel model);
        Task<List<SelectVm>> GetService(int CompanyId);
        Task<CrmVm> ModifyUpload(CrmVm model);
        Task<CrmUploadListVm> GetAllCrmUpload(int companyId);
        Task<CompanyListVm> GetAllCompany();
        Task<TeamListVm> GetTeamList(int companyId);
        Task<ResponsibleOfficerListVm> GetAllResponsibleOfficer(int companyId);
        Task<ProjectListVm> GetAllproject(int companyId);
        Task<ProjectListVm> GetAllprojectForcrm(int companyId);
        Task<ResponsibleOfficerVm> SwitchResponsibleOffice(int clientId, int companyId);
        Task<ServiceStatusVm> SaveServiceStatus(ServiceStatusVm model);
        Task<ServiceStatusListVm> GetAllServiceStatus(int companyId);

        Task<ServiceStatusHistVm> GetServiceHistoryById(int KgreHistoryId);
        Task<ServiceStatusHistVm> UpdateStatusNote(ServiceStatusHistVm model);

        Task<CrmScheduleVm> UpdateScheduleNote(CrmScheduleVm model);
        Task<ServiceStatusHistVm> RemoveServiceStatusNote(ServiceStatusHistVm model);

        Task<CrmScheduleVm> RemoveClientSchedule(CrmScheduleVm model);
        Task<CrmScheduleVm> UpdateClientSchedule(CrmScheduleVm model);
        Task<CrmScheduleVm> GetClientScheduleById(int KgreHistoryId);



        Task<PromotionalOfferListVm> GetAllPromotionalOffer(int companyId);
        Task<PromotionalOfferVm> SavePromotionalOffer(PromotionalOfferVm model);

        Task<ChoiceAreaListVm> GetAllChoiceArea(int companyId);
        Task<List<SelectVm>> GetDropdownGender();
        Task<List<SelectVm>> GetDropdownReligion();
        Task<List<SelectVm>> GetDropdownDealingOfficer(int companyId);
        Task<List<SelectVm>> GetDropdownDealingOfficerForLead(int companyId, int uid);
        Task<List<SelectVm>> GetDropdownProject(int companyId);
        Task<VehicleVm> GetDetailsForUpdate(int? id);
        Task<VehicleVm> CraRequisitinAdd(VehicleVm model);
        Task<List<SelectVm>> GetDropdownServiceStatus(int companyId);
        Task<List<SelectVm>> GetDropdownTypeofInterest();
        Task<List<SelectVm>> GetDropdownSourceofMedia(int companyId);
        Task<List<SelectVm>> GetDropdownPromotionalOffer(int companyId);
        Task<List<SelectVm>> GetDropdownChoiceofArea(int companyId);
        Task<List<SelectVm>> GetDropdownCompany();

        Task<List<SelectVm>> GetUploaddate(int companyId);

        Task<ChoiceAreaVm> SaveChoiceArea(ChoiceAreaVm model);

        Task<object> GetAutoCompleteClientName(string prefix);
        Task<object> GetAutoCompleteClientMobile(string prefix);

        Task<bool> DeleteServiceStatus(int id);
        Task<bool> DeleteCarReq(int id);
        Task<ServiceStatusVm> GetServicestatusById(int id);
        Task<SelectModelVm> UpdateResofcrID(SelectModelVm Model, int uId);
        Task<SelectModelVm> UpdateServstsId(SelectModelVm Model);
        Task<SelectModelVm> UpdateCompany(SelectModelVm Model, int uId);
        Task<SelectModelVm> SwitchServiceStatus(SelectModelVm Model);
        Task<CrmVm> SaveClient(CrmVm model);
        Task<CrmVm> UpdateSectionClient(CrmVm model);
        Task<CrmVm> noteview(CrmVm model);
        Task<CrmVm> GetExistClientByID(string MobileNumber, int CompanyId);
        Task<VehicleVm> GetCarRquisitinList(int CompanyId, DateTime? Dateee);
         Task<VehicleVm> GetCarRquisitinListIndividual(int CompanyId, int Uid);
        Task<VehicleVm> GetOfficerName(int CompanyId, int Uid);
        Task<CrmVm> ReassignClient(CrmVm model);
        Task<VehicleVm> Savedrivernamsper(VehicleVm model);
        Task<CrmUploadListVm> FilteringClientlist(CrmUploadListVm model, int userId);
        Task<PromotionalOfferVm> GetPromotionalOfferById(int id);
        PermissionModelListVm GetPermissionHandle(int userId, int companyId);
        Task<PermissionModel> SavePermission(int id, bool status, int companyId, int userId);
        bool IsLeader(int userId, int companyId);
        bool Manager(int userId, int companyId);
        Task<ClientStatusListVm> GetClentStatus(int StatusId, int uId, int companyId);

        Task<List<ServiceStatusHistVm>> GetServiceStatusHistories(long clientId);
        Task<List<ServiceStatusHistVm>> GetNoteList(long clientId);
        Task<object> GetAutoCompleteEmployee(string prefix);

        Task<List<CrmVm>> UploadClientBatch(List<CrmVm> kGRECustomers, int companyId, string fileName);
        Task<List<CrmVm>> UploadClientBatchh(ExcelWorksheet worksheet, int companyId, string fileName);

        CrmUploadVm SyncClientBatch(CrmUploadVm model);

        Task<ClientBatchUplodListVm> GetCrmClientBatchUpload(int companyId);
       


        Task<CrmScheduleListVm> GetPendingScheduleList(int companyId, int uId);
        Task<CrmScheduleListVm> GetCompletedScheduleList(int companyId, int uId);

        Task<CrmListVm> FilteringClientUploadBatchList(int companyId, int uploadSerialNo, DateTime uploadDateTime, int userId);
        Task<CrmUploadListVm> FilteringClientlist(CrmUploadListVm model, int userId, int pageNumber, int pageSize, string searchText);
    }
    public interface IPermissionHandler
    {
        bool HasPermission(int userId, int permissionNo, int companyId);
        bool IsAdmin(int userId);
        bool IsSupperAdmin(int userId);
    }
}
