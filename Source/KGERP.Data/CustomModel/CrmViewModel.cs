using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace KGERP.Data.CustomModel
{
    public class CrmViewModel
    {
        public int TotalNumberofClient { get; set; }
        public List<ClientStatusList> ClientStatusDataList{ get; set; } = new List<ClientStatusList>();
        public List<CrmScheduleVm> TodayScheduleList { get; set; } = new List<CrmScheduleVm>();
        public List<CrmScheduleVm> UpcomeingScheduleList { get; set; } = new List<CrmScheduleVm>();
        public int CompanyId { get; set; }
        public bool Isleader { get; set; }
        public int CountTodaySchedule { get; set;}
        public int CountUpcommingSchedule { get; set;}
        public int ResponsibleOfficerid { get; set; }
        public DateTime startTodate { get; set; }
        public  DateTime FormDate { get; set; }
    }
  

    public class BaseVm
    {
        public bool HasMessage { get; set; }
        public Uri ReturnUrl => System.Web.HttpContext.Current.Request.UrlReferrer; 
        public List<string> MessageList { get; set; } = new List<string>();
    }
    public class ClientStatusList: BaseVm
    {
        public int TotalNumberofClient { get; set; }
        public string StatusText { get; set; }
        public int StatusId { get; set; }
    }

    public class MainMenuListVm
    {
        public int CompanyId { get; set; }
        public string CompanyText { get; set; }
        public string CompanyLogo { get; set; }

        public List<MainMenuVm> DataList { get; set; } = new List<MainMenuVm>();
    }
    public class MainMenuVm
    {
        public int MainMenuId { get; set; }
        public int CompanyId { get; set; }
        public string MainMenuName { get; set; }
        public int OrderNo { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Param { get; set; }
        public List<SubMenuVm> SubMenuList { get; set; } = new List<SubMenuVm>();
    }
    public class SubMenuVm
    {
        public int SubmenuId { get; set; }
        public int MainMenuId { get; set; }
        public string SubMenuName { get; set; }
        public int CompanyId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Param { get; set; }
        public int OrderNo { get; set; }
    }
    public class CrmScheduleListVm : BaseVm
    {
        public List<SelectVm> DealingOfficerList { get; set; } = new List<SelectVm>();
        public CrmVm ClientData = new CrmVm();
        public List<SelectVm> GenderList { get; set; } = new List<SelectVm>();
        public List<CrmScheduleVm> DataList { get; set; } = new List<CrmScheduleVm>();
        public List<SelectVm> ReligionList { get; set; } = new List<SelectVm>();
        public List<SelectVm> SourceofMediaList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ChoiceofAreaList { get; set; } = new List<SelectVm>();
        public List<ServiceStatusHistVm> ServiceStatusHistList { get; set; } = new List<ServiceStatusHistVm>();
        public List<ServiceStatusHistVm> NoteList { get; set; } = new List<ServiceStatusHistVm>();
        public ServiceStatusHistVm NoteObjS { get; set; }
      

        public int UserId { get; set; }
        public int CompanyId { get; set; }

    }

    public class schedule
    {
        public int ClientId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string ScheduleType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Note { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CompanyId { get; set; }
        public bool IsCompleted { get; set; }

    }


   






    public class CrmScheduleVm : BaseVm
    {
        public int ScheduleId { get; set; }
        public int ClientId { get; set; }
        public int CompanyId { get; set; }
        public string ClientName { get; set; }
        public string ClientMobileNo { get; set; }
        public string Note { get; set; }
        public string ScheduleType { get; set; }
        public string ScheduleTimeText { get; set; }
        public TimeSpan ScheduleTime { get; set; }

        public DateTime ScheduleDate { get; set; }
        public bool IsComplete { get; set; }
        public string ResponsibleOfficeName { get; set; }
        public string Name { get; set; }
    }
    public class ClientBatchUplodListVm :BaseVm
    {
        public int CompanyId { get; set; }
        public List<ClientBatchUplodVm> DataList { get; set; } = new List<ClientBatchUplodVm>();
    }
    public class ClientBatchUplodVm
    {
        public int UploadSerialNo { get; set; }
        public DateTime UploadDateTime { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public bool IsSync { get; set; }
        public int IsSyncCount { get; set; }
        public int CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public string UploadName { get; set; }
        public string FileName { get; set; }
    }
    public class CrmUploadListVm: BaseVm
    {
        public List<SelectVm> UploadDatetimeList { get; set; } = new List<SelectVm>();
        public int CompanyId { get; set; }
        public List<SelectVm> GenderList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ReligionList { get; set; } = new List<SelectVm>();
        public List<SelectVm> DealingOfficerList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ProjectList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ServiceStatusList { get; set; } = new List<SelectVm>();
        public List<SelectVm> TypeofInterestList { get; set; } = new List<SelectVm>();
        public List<SelectVm> SourceofMediaList { get; set; } = new List<SelectVm>();
        public List<SelectVm> PromotionalOfferList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ChoiceofAreaList { get; set; } = new List<SelectVm>();
        public List<ClientVm> DataList { get; set; } = new List<ClientVm>();
        public int GenderId { get; set; }
        public int ReligionId { get; set; }
        public int ResponsibleOfficerId { get; set; }
        public int ProjectId { get; set; }
        public int TypeofInterestId { get; set; }
        public int StatusId { get; set; }
        public int SourceofMediaId { get; set; }
        public int OfferId { get; set; }
        public int ChoiceAreaId { get; set; }
        public int PromotionalOfferId { get; set; }
        public int lastUploadSerialNo { get; set; }
        public bool IsLeader { get; set; }
        public string ReportType { get; set; }
        public bool ManagerId { get; set; }
        public int Uid { get; set; }

        public int Totalcount { get; set; }
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int CureentPage { get; set; }
        public int FirstSerialNumber { get; set; }
        public string searchText { get ; set; }
        //  public List<CrmUploadVm> DataList { get; set; } = new List<CrmUploadVm>();

    }
    public class ClientVm
    {
        public string Name { get; set; }
        public int ClientId { get; set; }
        public string GenderName { get; set; }
        public string ReligionText { get; set; }
        public string ResponsibleOfficeName { get; set; }
        public string MobileNo { get; set; }
        public string MobileNo2 { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public DateTime? DateofBirth { get; set; }
        public DateTime? DateOfContact { get; set; }
        public string SourceofMediaText { get; set; }
        public string ProjectText { get; set; }
        public string StatusText { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string OrganizationText { get; set; }
        public string CampaignText { get; set; }
        public string TypeofInterestText { get; set; }
        public string JobTitle { get; set; }
        public string ReferredBy { get; set; }
        public string ChoiceAreaText { get; set; }
        public string OfferText { get; set; }
        public string Remarks { get; set; }
        public int CompanyId { get; set; }
        
        public int StatusId { get; set; }
        public int PromotionalOfferId { get; set; }
        public string PromotionalOfferText { get; set; }
        public int uploadSerialno { get; set; }
       public DateTime uploadDatetime { get; set; }
        public DateTime Createdate { get; set; }

    }
    public class CrmUploadVm : BaseVm
    {
        public string Status { get; set; }
        public int LastUploadNo { get; set; }
        public int CompanyId { get; set; }
        public DateTime UploadDateTime { get; set; }
        public HttpPostedFileBase ExcelFile { get; set; }
        public List<CrmVm> ResponseList { get; set; } = new List<CrmVm>();
        public string Path { get; set; }
    }
    public class CrmListVm :BaseVm
    {
        public int CompanyId { get; set; }
        public List<CrmVm> DataList { get; set; } = new List<CrmVm>();
        public bool IsLeader { get; set; }
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int CureentPage { get; set; }
        public int Totalcount { get; set; }
        public int FirstSerialNumber { get; set; }


    }
    public class CrmVm : BaseVm
    {
        public List<string> Against { get; set; }
        public string ResponseStatus { get; set; }
        public int ClientId { get; set; }
        public string Name { get; set; }

        public int GenderId { get; set; }
        public string GenderText { get; set; }
        public int ReligionId { get; set; }
        public string ReligionText { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string DateofBirthText { get; set; }
        public string MobileNo { get; set; }
        public string MobileNo2 { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string JobTitle { get; set; }
        public string Organization { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }

        public int ResponsibleOfficerId { get; set; }
        public string ResponsibleOfficeName { get; set; }
        public int StatusId { get; set; }
        public string StatusText { get; set; }
        public int OfferId { get; set; }
        public string OfferText { get; set; }
        public string OrganizationText { get; set; }
        public int ProjectId { get; set; }
        public string ProjectText { get; set; }
        public string StatusDateText { get; set; }
        public int SourceofMediaId { get; set; }
        public string SourceofMediaText { get; set; }
        public int ChoiceAreaId { get; set; }
        public string ChoiceAreaText { get; set; }
        public int TypeofInterestId { get; set; }
        public string TypeofInterestText { get; set; }

        public List<CrmSchedule> ScheduleDataList { get; set; }
        public CrmVm SirviceDataList { get; set; }
        public string CampaignText { get; set; }
        public int CompanyId { get; set; }
        public string CompanyText { get; set; }
        public string ReferredBy { get; set; }
        public string Remarks { get; set; }
        public DateTime? DateOfContact { get; set; }
        public List<SelectVm> GenderList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ReligionList { get; set; } = new List<SelectVm>();
        public List<SelectVm> DealingOfficerList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ProjectList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ServiceStatusList { get; set; } = new List<SelectVm>();
        public List<SelectVm> TypeofInterestList { get; set; } = new List<SelectVm>();
        public List<SelectVm> SourceofMediaList { get; set; } = new List<SelectVm>();
        public List<SelectVm> PromotionalOfferList { get; set; } = new List<SelectVm>();
        public List<SelectVm> ChoiceofAreaList { get; set; } = new List<SelectVm>();
        public List<SelectVm> CompanyList { get; set; } = new List<SelectVm>();
        public List<CrmVm> crmVms { get; set; }
        public VMKGRECustomer Clientdata { get; set; }
        public List<KGRECustomer> crmVms2 { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public bool IsLeader { get; set; }
        public bool isManagwe { get; set; }
        public int UserId { get; set; }

        public int SectionEdit { get; set; }
        public string ServiceNote { get; set; }
        public string messageE { get; set; }
        public DateTime StrFrmdate { get; set; } 
        public DateTime Todate { get; set; }

    }

    public class VehicleVm
    {
        public VehicleRequisition VehicleRequisition { get; set; }
        public int VehicleRequisitionId { get; set; }
        public string OfficerName { get; set; }
        public int ResponsibleOfficerId { get; set; }
        public int DayId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime Date { get; set; }
        public string Time { get; set; }
        public int TotalPersion { get; set; }
        public int PrijectId { get; set; }
        public int CompanyId { get; set; }
        public string Reason { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string DriverName { get; set; }
        public string ProjectName { get; set; }
        public string Strdate { get; set; }
        [DisplayName("Document Type")]
        public DayTypeEnum DayType { get; set; }
        public IEnumerable<VehicleVm> DataList { get; set; }
        public List<SelectVm> ProjectList { get; set; }

    }





    public partial class VMKGRECustomer
    {

        public int ClientId { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public int ServiceStatusId { get; set; }
        public string ServiceStatusName { get; set; }
        public int ResponsibleOfficerId { get; set; }

        public string ResponsibleOfficer { get; set; }
        public string KGRECompanyName { get; set; }
        public string CampaingName { get; set; }
        public int CompanyId { get; set; }
        public int? projectId { get; set; }
        public string ProjectName { get; set; }
    }
    public class ServiceStatusHistVm : BaseVm
    {
        public long KgreHistoryId { get; set; }
        public int CompanyId { get; set; }
        public string HistoryText { get; set; }
        public long ClientId { get; set; }
        public string Notes { get; set; }
        public DateTime ModiifyDate { get; set; }
        public string name { get; set; }
    }
    public class SelectVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime uploaddatetime { get; set; }
        public string TeamName {get;set;}
        public long? managerID { get; set; }
        public int Uid { get; set; }
        public long empid { get; set; }
    }

    public class CompanyListVm
    {
        public List<CompanyVm> DataList { get; set; } = new List<CompanyVm>();
    }
    public class CompanyVm
    {
        public int CompanyId { get; set; }
        public string CompanyText { get; set; }
        public string CompanyCode { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public int OrderNo { get; set; }

    }
    public class ResponsibleOfficerListVm
    {
        public List<ResponsibleOfficerVm> DataList { get; set; } = new List<ResponsibleOfficerVm>();
    }
    public class ResponsibleOfficerVm
    {
        public int CompanyId { get; set; }
        public string CompanyText { get; set; }
        public string LeaderName { get; set; }
        public int LeaderId { get; set; }
        public string TeamName { get; set; }
        public string MemberType { get; set; }
        public string ResponsibleOfficerName { get; set; }
        public int ResponsibleOfficerId { get; set; }
        public string ResponsibleOfficerText { get; set; }

    }

    public class OfficerVm
    {

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeadId { get; set; }
        public bool IsLeader { get; set; }
        public int CompanyId { get; set; }

        public string TeamName { get; set; }
     
        public string OfficerName { get; set; }

    }


    public class TeamVm
    {
        public string TeamName { get; set; }
        public string LeaderName { get; set; }

        public List<OfficerVm> DataList { get; set; } = new List<OfficerVm>();
    }

    public class TeamListVm
    {
        public List<TeamVm> DataList { get; set; } = new List<TeamVm>();
    }

    public class ServiceStatusListVm : BaseVm
    {
        public int CompanyId { get; set; }
        public List<ServiceStatusVm> Datalist { get; set; } = new List<ServiceStatusVm>();
    }

    public class ServiceStatusVm:BaseVm
    {
        public int StatusId { get; set; }
        public string StatusText { get; set; }
        public int companyId { get; set; }
        public bool IsDeleted { get; set; }
        public string message { get; set; }

    }



    public class ProjectListVm
    {
        public List<ProjectVm> Datalist { get; set; } = new List<ProjectVm>();
        public List<ProjectVm> ProjectList { get; set; } = new List<ProjectVm>();
    }

    public class ProjectVm
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<ProjectListVm> projectlist { get; set; }
    }
    public class PromotionalOfferListVm
    {
        public int CompanyId { get; set; }
        public List<PromotionalOfferVm> Datalist { get; set; } = new List<PromotionalOfferVm>();
    }

    public class PromotionalOfferVm
    {
        public int OfferId { get; set; }
        public string OfferName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsOpen { get; set; }
        public int OfferDays { get; set; }
        public string OfferStatusText { get; set; }
        public int OfferRemainDays { get; set; }
        public int CompanyId { get; set; }
    }

    public class ChoiceAreaListVm
    {
        public int CompanyId { get; set; }
        public List<ChoiceAreaVm> DataList { get; set; } = new List<ChoiceAreaVm>();
    }
    public class ChoiceAreaVm
    {
        public int ChoiceAreaId { get; set; }
        public int CompanyId { get; set; }

        public string ChoiceAreaText { get; set; }
    }
    public class ClientStatusListVm
    {
        public List<ClientStatusVm> DataList { get; set; }
    }


    public class ClientStatusVm
    {
        public int StatusId { get; set; }
        public int ClientId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string ProjectText { get; set; }
        public string StatusText { get; set; }
        public string ActionLink { get; set; }
        public string JobTitle { get; set; }
        public string OrganizationText { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
     


    }


    public class SelectModelVm
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Note { get; set; }
        public int CustomId { get; set; }
        public int CustomeId2 { get; set; }
    }

    public class PermissionModelListVm
    {
        public int CompanyId { get; set; }
        public List<SelectVm> CompanyList { get; set; } = new List<SelectVm>();
        public string UserName { get; set; }
        public int UserId { get; set; }
        public List<PermissionModel> PermissionList { get; set; } = new List<PermissionModel>();

    }
    public class PermissionModel
    {
        public int? ParentID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        public List<PermissionModel> Childs { get; set; }

        public bool IsChecked { get; set; }

        public bool IsExpanded { get; set; }
    }


}