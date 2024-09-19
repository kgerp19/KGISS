using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KGERP.Utility.Util;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using static log4net.Appender.RollingFileAppender;
using System.Globalization;
using static Humanizer.In;
using System.Runtime.InteropServices.ComTypes;
using System.IdentityModel.Protocols.WSTrust;
using static KGERP.Utility.Util.PermissionCollection.Crms;
using System.Runtime.Remoting.Contexts;
using System.Web.UI.WebControls;
using KGERP.Services.Procurement;
using System.Security.Cryptography;
using System.Security.Policy;
using Azure.Core;
using Humanizer;
using System.Data;
using System.Web.Mvc;
using KGERP.Utility;
using KGERP.Service.ServiceModel;
using System.Drawing;
using System.Data.SqlClient;
using OfficeOpenXml;
using AutoMapper.Configuration.Conventions;
using static KGERP.Utility.Util.PermissionCollection;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;

namespace KGERP.Service.Implementation
{
    public class CrmService : ICrmService
    {
        private readonly ERPEntities _context;
        public CrmService(ERPEntities context)
        {
            _context = context;
        }

        public MainMenuListVm GetAllMenu(int companyId)
        {
            var menuVm = new MainMenuListVm();
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            try
             {
                var url = _context.UrlInfoes.SingleOrDefault(q => q.UrlId == 1).Url;

                var company = _context.Companies.SingleOrDefault(q => q.CompanyId == companyId);
                menuVm.CompanyLogo = url + "Images/Logo/" + company.CompanyLogo;
                menuVm.CompanyText = company.Name;

                menuVm.CompanyId = companyId;
                var menuList = _context.CompanyMenus.Where(q => q.IsWeb && q.CompanyId == companyId && q.IsActive).Select(s =>
                             //                         join  t2 in _context.CompanyUserMenus on t1.CompanyMenuId equals t2.CompanyMenuId
                             ////where t1.IsActive && t2.UserId==userId 
                             //&& t1.CompanyId==companyId

                             new MainMenuVm
                             {
                                 MainMenuId = s.CompanyMenuId,
                                 CompanyId = s.CompanyId ?? 0,
                                 Param = s.Param,

                                 MainMenuName = s.ShortName,
                                 ControllerName = s.Controller,
                                 ActionName = s.Action,
                                 OrderNo = s.OrderNo


                             }).ToList();

                if (menuList == null) return menuVm;

                var submenuList = new List<SubMenuVm>();

                foreach (var m in menuList)
                {
                    submenuList = (from t1 in _context.CompanySubMenus
                                   join t2 in _context.CompanyUserMenus on t1.CompanySubMenuId equals t2.CompanySubMenuId
                                   where t1.IsActive && t2.UserId == userId && t2.IsActive && !t1.IsSideMenu && t1.CompanyMenuId == m.MainMenuId
                                   select new SubMenuVm
                                   {
                                       SubmenuId = t1.CompanySubMenuId,
                                       MainMenuId = t1.CompanyMenuId ?? 0,
                                       SubMenuName = t1.ShortName,
                                       ControllerName = t1.Controller,
                                       ActionName = t1.Action,
                                       Param = t1.Param,
                                       OrderNo = t1.OrderNo,
                                       CompanyId = t1.CompanyId ?? 0


                                   }).ToList();

                    var menu = new MainMenuVm
                    {
                        MainMenuId = m.MainMenuId,
                        MainMenuName = m.MainMenuName
                    };
                    var submenu = submenuList.Where(q => q.MainMenuId == m.MainMenuId)
                        .Select(s => new SubMenuVm
                        {
                            SubmenuId = s.SubmenuId,
                            SubMenuName = s.SubMenuName,
                            MainMenuId = s.MainMenuId,
                            ControllerName = s.ControllerName,
                            ActionName = s.ActionName,
                            OrderNo = s.OrderNo,
                            CompanyId = s.CompanyId
                        })
                        .ToList();

                    menu.SubMenuList.AddRange(submenu);
                    menuVm.DataList.Add(menu);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menuVm;
        }
        public async Task<CrmViewModel> GetIndex(int companyId, int userId)
        {
            int uId = 0;
            var today = DateTime.Now.Date;
            var endDay = DateTime.Now.AddMonths(1).Date;
            var model = new CrmViewModel();

            var userDetail = _context.TeamInfoes.SingleOrDefault(q => q.EmployeeId == userId && q.IsActive && q.CompanyId == companyId && !q.IsLeader && q.Name != "Existing Team");

            if (userDetail == null)
            {
                uId = 0;
            }
            else
            {
                uId = userDetail.Id;
            }

            model.TotalNumberofClient = await _context.KGRECustomers.Where(q => q.CompanyId == companyId
            && (uId == 0 || q.ResponsibleOfficerId == uId)).CountAsync();

            model.ClientStatusDataList = await Task.Run(() => (from t1 in _context.KGRECustomers
                                                               join t2 in _context.CrmServiceStatus
                                                               on t1.ServiceStatusId equals t2.StatusId
                                                               where t2.CompanyId == companyId
                                        && (userId == 179 ? (t1.ClientId > 0) : ((uId == 0) || (t1.ResponsibleOfficerId == uId)))

                                                               group t1 by new { t2.StatusName, t2.StatusId }
                                                               into g
                                                               select new ClientStatusList
                                                               {
                                                                   StatusId = g.Key.StatusId,
                                                                   StatusText = g.Key.StatusName,
                                                                   TotalNumberofClient = g.Count()

                                                               }).ToListAsync());

            var officer = _context.TeamInfoes.FirstOrDefault(e => e.EmployeeId == userId && e.CompanyId == companyId && (e.Name != "Existing Team" || e.EmployeeId == 179 || e.EmployeeId == 42189 || e.EmployeeId == 42204 || e.EmployeeId==180));

            if (officer.ManagerId != 1)
            {

                model.UpcomeingScheduleList = await Task.Run(() => (from t1 in _context.CrmSchedules
                                                                    join t2 in _context.KGRECustomers on t1.ClientId equals t2.ClientId
                                                                    join t3 in _context.TeamInfoes on t2.ResponsibleOfficerId equals t3.Id
                                                                    join t4 in _context.Employees on t3.EmployeeId equals t4.Id

                                                                    where t3.Name == officer.Name && t1.IsActive && t1.ScheduleDate >= today && t1.ScheduleDate <= endDay
                                                                    && (userId == 179 ? (t2.ClientId > 0) : ((uId == 0) || (t2.ResponsibleOfficerId == uId)))

                                                                    && t1.CompanyId == companyId
                                                                    select new CrmScheduleVm
                                                                    {
                                                                        Name = t3.Name,
                                                                        ClientId = t1.ClientId,
                                                                        ClientName = t2.FullName,
                                                                        ScheduleTime = t1.ScheduleTime,
                                                                        ScheduleDate = t1.ScheduleDate,
                                                                        Note = t1.Note,
                                                                        ScheduleType = t1.ScheduleType,
                                                                        ScheduleId = t1.ScheduleId,
                                                                        ClientMobileNo = t2.MobileNo == null ? t2.MobileNo2 : t2.MobileNo,
                                                                        IsComplete = t1.IsCompleted,
                                                                        ResponsibleOfficeName = t4.Name,
                                                                        CompanyId = t2.CompanyId ?? 0
                                                                    }).OrderBy(o => new { o.ScheduleDate, o.ScheduleTime }).ToListAsync());
            }
            else
            {
                model.UpcomeingScheduleList = await Task.Run(() => (from t1 in _context.CrmSchedules
                                                                    join t2 in _context.KGRECustomers on t1.ClientId equals t2.ClientId
                                                                    join t3 in _context.TeamInfoes on t2.ResponsibleOfficerId equals t3.Id
                                                                    join t4 in _context.Employees on t3.EmployeeId equals t4.Id

                                                                    where t1.IsActive && t1.ScheduleDate >= today && t1.ScheduleDate <= endDay
                                                                    && (userId == 179 ? (t2.ClientId > 0) : ((uId == 0) || (t2.ResponsibleOfficerId == uId)))

                                                                    && t1.CompanyId == companyId
                                                                    select new CrmScheduleVm
                                                                    {
                                                                        ClientId = t1.ClientId,
                                                                        ClientName = t2.FullName,
                                                                        ScheduleTime = t1.ScheduleTime,
                                                                        ScheduleDate = t1.ScheduleDate,
                                                                        Note = t1.Note,
                                                                        ScheduleType = t1.ScheduleType,
                                                                        ScheduleId = t1.ScheduleId,
                                                                        ClientMobileNo = t2.MobileNo == null ? t2.MobileNo2 : t2.MobileNo,
                                                                        IsComplete = t1.IsCompleted,
                                                                        ResponsibleOfficeName = t4.Name,
                                                                        CompanyId = t2.CompanyId ?? 0
                                                                    }).OrderBy(o => new { o.ScheduleDate, o.ScheduleTime }).ToListAsync());
            }


            // model.UpcomeingScheduleList = await Task.Run(() => (from t1 in _context.CrmSchedules
            //join t2 in _context.KGRECustomers on t1.ClientId equals t2.ClientId
            //join t3 in _context.TeamInfoes on t2.ResponsibleOfficerId equals t3.Id
            //join t4 in _context.Employees on t3.EmployeeId equals t4.Id

            //where t1.IsActive && t1.ScheduleDate >= today && t1.ScheduleDate <= endDay
            //&&  (userId == 179 ? (t2.ClientId > 0) : ((uId == 0) || (t2.ResponsibleOfficerId == uId)))

            //&& t1.CompanyId == companyId
            //select new CrmScheduleVm
            //{
            //    ClientId = t1.ClientId,
            //    ClientName = t2.FullName,
            //    ScheduleTime = t1.ScheduleTime,
            //    ScheduleDate = t1.ScheduleDate,
            //    Note = t1.Note,
            //    ScheduleType = t1.ScheduleType,
            //    ScheduleId = t1.ScheduleId,
            //    ClientMobileNo = t2.MobileNo == null ?                                              t2.MobileNo2 : t2.MobileNo,             
            //    IsComplete = t1.IsCompleted,
            //    ResponsibleOfficeName =                                                             t4.Name,
            //    CompanyId = t2.CompanyId ?? 0
            //}).OrderBy(o => new { o.ScheduleDate, o.ScheduleTime }).ToListAsync());




            model.TodayScheduleList = model.UpcomeingScheduleList.Where(q => q.ScheduleDate == today).ToList();

            model.UpcomeingScheduleList = model.UpcomeingScheduleList.Where(q => q.ScheduleDate != today).ToList();
            model.CountTodaySchedule = model.TodayScheduleList.Count();
            model.CountUpcommingSchedule = model.UpcomeingScheduleList.Count();

            model.CompanyId = companyId;

            return model;
        }
        public async Task<CrmListVm> GetAllClient(int companyId, int userId)
        {

            if (IsLeader(userId, companyId))
            {
                userId = 179;
            }

            var models = new CrmListVm();

            models.DataList = await Task.Run(() => (from t1 in _context.KGRECustomers
                                                    join t2 in _context.ProductCategories on t1.ProjectId equals t2.ProductCategoryId
                                                    join t3 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t3.Id
                                                    join t4 in _context.Employees on t3.EmployeeId equals t4.Id
                                                    join t5 in _context.CrmServiceStatus on t1.ServiceStatusId equals t5.StatusId
                                                    join t6 in _context.CrmChoiceAreas on t1.ChoieceAreaId equals t6.ChoiceAreaId into t6_Join
                                                    from t6 in t6_Join.DefaultIfEmpty()
                                                    where t1.CompanyId == companyId
                                                    && (t3.ManagerId == userId ? t1.ClientId > 0 : t3.EmployeeId == userId)

                                                    select new CrmVm
                                                    {
                                                        ClientId = t1.ClientId,
                                                        Name = t1.FullName,
                                                        MobileNo = t1.MobileNo,
                                                        MobileNo2 = t1.MobileNo2,
                                                        Email = t1.Email,
                                                        Email2 = t1.Email1,
                                                        StatusText = t5.StatusName,
                                                        ResponsibleOfficeName = t4.Name,
                                                        ProjectId = t1.ProjectId ?? 0,
                                                        ProjectText = t2.Name,
                                                        CompanyId = companyId,
                                                        ChoiceAreaText = t6.ChoiceAreaName
                                                    }
                                                  ).OrderByDescending(o => o.ClientId).ToListAsync());


            //  if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            //  {
            //    //  DataList = DataList.OrderBy(sortColumn + " " + sortColumnDir);
            //  }
            //  if (!string.IsNullOrEmpty(searchValue))
            //  {
            //      if(searchValue != "\0")
            //      {
            //          models.DataList =  models.DataList.Where(m => m.Name.Contains(searchValue)
            //                               || m.ResponsibleOfficeName.Contains(searchValue)
            //                               || m.MobileNo.Contains(searchValue)
            //                               || m.Email.Contains(searchValue)).ToList();
            //      }

            //  }
            // // recordsTotal = customerData.Count();
            //  var data = models.DataList.Skip(skip).Take(pageSize).ToList();
            ////  var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            //  //return Ok(jsonData);
            models.CompanyId = companyId;
            models.IsLeader = IsLeader(userId, companyId);

            return models;
        }
        public async Task<CrmListVm> GetUserClient(int companyId, int userId, int pageNumber, int pageSize, string searchText)
        {
            var models = new CrmListVm();
            var allData = await Task.Run(() => (from t1 in _context.KGRECustomers.Where(x => x.CompanyId == companyId)
                                                    join t2 in _context.ProductCategories on t1.ProjectId equals t2.ProductCategoryId into t2_Join
                                                    from t2 in t2_Join.DefaultIfEmpty()
                                                    join t3 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t3.Id
                                                    join t4 in _context.Employees on t3.EmployeeId equals t4.Id

                                                    join t5 in _context.CrmServiceStatus on t1.ServiceStatusId equals t5.StatusId into t5_Join
                                                    from t5 in t5_Join.DefaultIfEmpty()
                                                    join t6 in _context.CrmChoiceAreas on t1.ChoieceAreaId equals t6.ChoiceAreaId into t6_Join
                                                    from t6 in t6_Join.DefaultIfEmpty()
                                                        //join t7 in _context.CrmSchedules.OrderByDescending(o=>o.ScheduleDate).Take(1) on t1.ClientId equals t7.ClientId into t7_Join
                                                        //from t7 in t7_Join.DefaultIfEmpty()
                                                    where t1.CompanyId == companyId && ((userId == t3.ManagerId||userId==180) ? t1.ClientId > 0 : t4.Id == userId)
                                                //(userId == t3.ManagerId  change too ((userId == t3.ManagerId || userId == 180) aniiiii
                                                select new CrmVm
                                                    {
                                                        ClientId = t1.ClientId,
                                                        Name = t1.FullName,
                                                        MobileNo = t1.MobileNo,
                                                        MobileNo2 = t1.MobileNo2,
                                                        Email = t1.Email,
                                                        Email2 = t1.Email1,
                                                        StatusText = t5.StatusName,
                                                        ResponsibleOfficeName = t4.Name,
                                                        JobTitle = t1.Designation,
                                                        Organization = t1.DepartmentOrInstitution,
                                                        ProjectId = t1.ProjectId ?? 0,
                                                        ProjectText = t2.Name,
                                                        CompanyId = companyId,
                                                        ChoiceAreaText = t6.ChoiceAreaName,
                                                        Remarks = t1.Remarks,
                                                        CreatedDate = t1.CreatedDate,
                                                        SourceofMediaText = t1.SourceOfMedia

                                                    }
                                                  ).OrderByDescending(o => o.ClientId).AsQueryable());

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                allData = allData.Where(t =>
                    t.Name.ToLower().Contains(searchText) ||
                    t.MobileNo.ToLower().Contains(searchText) ||
                    t.MobileNo2.ToLower().Contains(searchText) ||
                    t.Email.ToLower().Contains(searchText) ||
                    t.Email2.ToLower().Contains(searchText) ||
                    t.StatusText.ToLower().Contains(searchText) ||
                    t.ResponsibleOfficeName.ToLower().Contains(searchText) ||
                    t.JobTitle.ToLower().Contains(searchText) ||
                    t.Organization.ToLower().Contains(searchText) ||
                    t.ProjectText.ToLower().Contains(searchText) ||
                    t.ChoiceAreaText.ToLower().Contains(searchText) ||
                    t.Remarks.ToLower().Contains(searchText) ||
                    t.SourceofMediaText.ToLower().Contains(searchText)
                );
            }

            int totalCount =  allData.Count();
            models.Totalcount = totalCount;
            models.CompanyId = companyId;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);            
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
            models.DataList = allData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            models.FirstSerialNumber = ((pageNumber * pageSize) - pageSize) + 1;
            // Set the current page number and page size in the CrmListVm
            models.CureentPage = pageNumber;
            models.pageSize = pageSize;
            models.totalPage = totalPages;
            return models;
        }


        //new code Ruman
        public async Task<CrmUploadListVm> FilteringClientlist(CrmUploadListVm model, int userId, int pageNumber, int pageSize, string searchText)
        {
            var data = new CrmUploadListVm();
            List<long> teamMambers = new List<long>();
            TeamInfo teamInfo = new TeamInfo();
            if (userId == 42189 || userId == 42204 || userId == 180)
            {
                userId = 179;
            }

            if (userId != 179)
            {

                teamInfo = _context.TeamInfoes.Where(x => x.EmployeeId == userId && x.CompanyId == model.CompanyId).FirstOrDefault();
                if (teamInfo.IsLeader == true)
                {
                    teamMambers = _context.TeamInfoes.Where(x => x.LeadId == teamInfo.Id || x.Id == teamInfo.Id).Select(x => x.EmployeeId).ToList();

                }
            }



            var allData = await Task.Run(() => (from t1 in _context.KGRECustomers
                                                  join t2 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t2.Id
                                                  join t3 in _context.Employees on t2.EmployeeId equals t3.Id
                                                  join t4 in _context.ProductCategories on t1.ProjectId equals t4.ProductCategoryId into t4_Join
                                                  from t4 in t4_Join.DefaultIfEmpty()
                                                  join t5 in _context.CrmServiceStatus on t1.ServiceStatusId equals t5.StatusId into t5_Join
                                                  from t5 in t5_Join.DefaultIfEmpty()
                                                  join t6 in _context.CrmChoiceAreas on t1.ChoieceAreaId equals t6.ChoiceAreaId into t6_Join
                                                  from t6 in t6_Join.DefaultIfEmpty()
                                                  join t7 in _context.DropDownItems on t1.GenderId equals t7.DropDownItemId into t7_Join
                                                  from t7 in t7_Join.DefaultIfEmpty()
                                                  join t8 in _context.DropDownItems on t1.ReligionId equals t8.DropDownItemId into t8_Join
                                                  from t8 in t8_Join.DefaultIfEmpty()
                                                  join t9 in _context.DropDownItems on t1.TypeOfInterestId equals t9.DropDownItemId into t9_Join
                                                  from t9 in t9_Join.DefaultIfEmpty()
                                                  join t10 in _context.CrmPromotionalOffers on t1.PromotionalOfferId equals t10.OfferId into t10_Join
                                                  from t10 in t10_Join.DefaultIfEmpty()
                                                  join t11 in _context.CrmSourceMedias on t1.SourceOfMediaId equals t11.SourceMediaId into t11_Join
                                                  where t1.CompanyId == model.CompanyId
                                      && (model.ResponsibleOfficerId > 0 ? t1.ResponsibleOfficerId == model.ResponsibleOfficerId : t1.ResponsibleOfficerId > 0)
                                      && (userId == 179 ? t2.EmployeeId > 0 : teamInfo.IsLeader == false ? t2.EmployeeId == teamInfo.EmployeeId : teamMambers.Contains(t2.EmployeeId))
                                                      //|| t2.EmployeeId == userId)

                                                      //userId == 0 &&



                                                      && (model.GenderId == 0 || t1.GenderId == model.GenderId)
                                                      && (model.ProjectId == 0 || t1.ProjectId == model.ProjectId)
                                                      && (model.StatusId == 0 || t1.ServiceStatusId == model.StatusId)
                                                      && (model.TypeofInterestId == 0 || t1.TypeOfInterestId == model.TypeofInterestId)
                                                      && (model.SourceofMediaId == 0 || t1.SourceOfMediaId == model.SourceofMediaId)
                                                      && (model.PromotionalOfferId == 0 || t1.PromotionalOfferId == model.PromotionalOfferId)
                                                      && (model.ChoiceAreaId == 0 || t1.ChoieceAreaId == model.ChoiceAreaId)
                                                      && (model.ReligionId == 0 || t1.ReligionId == model.ReligionId)
                                                  select new ClientVm
                                                  {
                                                      ClientId = t1.ClientId,
                                                      Name = t1.FullName,
                                                      GenderName = t7.Name,
                                                      ReligionText = t8.Name,
                                                      MobileNo = t1.MobileNo,
                                                      CompanyId = t1.CompanyId ?? 0,
                                                      MobileNo2 = t1.MobileNo2,
                                                      Email1 = t1.Email,
                                                      Email2 = t1.Email1,
                                                      DateofBirth = t1.DateofBirth,
                                                      JobTitle = t1.Designation,
                                                      OrganizationText = t1.DepartmentOrInstitution,
                                                      PresentAddress = t1.PresentAddress,
                                                      PermanentAddress = t1.PermanentAddress,
                                                      ResponsibleOfficeName = t3.Name,
                                                      ProjectText = t4.Name,
                                                      TypeofInterestText = t9.Name,
                                                      OfferText = t10.OfferName,
                                                      SourceofMediaText = t1.SourceOfMedia,
                                                      CampaignText = t1.CampaignName,
                                                      ChoiceAreaText = t6.ChoiceAreaName,
                                                      StatusId = t1.ServiceStatusId,
                                                      StatusText = t5.StatusName,
                                                      ReferredBy = t1.ReferredBy,
                                                      DateOfContact = t1.DateOfContact,
                                                      Createdate = t1.CreatedDate,
                                                      PromotionalOfferText = t10.OfferName



                                                  }).OrderByDescending(x => x.Createdate).AsQueryable());

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                allData =  allData.Where(t =>
                    t.Name.ToLower().Contains(searchText) ||
                    t.MobileNo.ToLower().Contains(searchText) ||
                    t.MobileNo2.ToLower().Contains(searchText) ||
                    t.Email1.ToLower().Contains(searchText) ||
                    t.Email2.ToLower().Contains(searchText) ||
                    t.StatusText.ToLower().Contains(searchText) ||
                    t.ResponsibleOfficeName.ToLower().Contains(searchText) ||
                    t.JobTitle.ToLower().Contains(searchText) ||
                    //t.Organization.ToLower().Contains(searchText) ||
                    t.ProjectText.ToLower().Contains(searchText) ||
                    t.ChoiceAreaText.ToLower().Contains(searchText) ||
                    //t.Remarks.ToLower().Contains(searchText) ||
                    t.SourceofMediaText.ToLower().Contains(searchText)
                );
            }

            data.GenderId = model.GenderId;
            data.ReligionId = model.ReligionId;
            data.CompanyId = model.CompanyId;
            data.ChoiceAreaId = model.ChoiceAreaId;
            data.ResponsibleOfficerId = model.ResponsibleOfficerId;
            data.SourceofMediaId = model.SourceofMediaId;
            data.StatusId = model.StatusId;
            data.TypeofInterestId = model.TypeofInterestId;
            data.ProjectId = model.ProjectId;
            data.OfferId = model.OfferId;
            data.PromotionalOfferId = model.PromotionalOfferId;
            data.ChoiceAreaId = model.ChoiceAreaId;


            int totalDataCount = allData.Count();
            data.Totalcount = totalDataCount;
            int totalPages = (int)Math.Ceiling((double)totalDataCount / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
            data.DataList = allData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            //data.FirstSerialNumber = ((pageNumber * pageSize) - pageSize) + 1;
            data.FirstSerialNumber = ((pageNumber * pageSize) - pageSize);
            // Set the current page number and page size in the CrmUploadListVm
            data.CureentPage = pageNumber;
            data.pageSize = pageSize;
            data.totalPage = totalPages;

            return data;
        }

        //public async Task<CrmListVm> GetUserClient(int companyId, int userId, int pageNumber, int pageSize)
        //{
        //    var models = new CrmListVm();

        //    // Calculate the starting index of the current page's data.
        //    int startIndex = (pageNumber - 1) * pageSize;

        //    // Query to fetch the paginated data from the database.
        //    var query = (from t1 in _context.KGRECustomers.Where(x => x.CompanyId == companyId)
        //                 join t2 in _context.ProductCategories on t1.ProjectId equals t2.ProductCategoryId into t2_Join
        //                 from t2 in t2_Join.DefaultIfEmpty()
        //                 join t3 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t3.Id
        //                 join t4 in _context.Employees on t3.EmployeeId equals t4.Id
        //                 join t5 in _context.CrmServiceStatus on t1.ServiceStatusId equals t5.StatusId into t5_Join
        //                 from t5 in t5_Join.DefaultIfEmpty()
        //                 join t6 in _context.CrmChoiceAreas on t1.ChoieceAreaId equals t6.ChoiceAreaId into t6_Join
        //                 from t6 in t6_Join.DefaultIfEmpty()
        //                 where t1.CompanyId == companyId && (userId == t3.ManagerId ? t1.ClientId > 0 : t4.Id == userId)
        //                 select new CrmVm
        //                 {
        //                     ClientId = t1.ClientId,
        //                     Name = t1.FullName,
        //                     MobileNo = t1.MobileNo,
        //                     MobileNo2 = t1.MobileNo2,
        //                     Email = t1.Email,
        //                     Email2 = t1.Email1,
        //                     StatusText = t5.StatusName,
        //                     ResponsibleOfficeName = t4.Name,
        //                     JobTitle = t1.Designation,
        //                     Organization = t1.DepartmentOrInstitution,
        //                     ProjectId = t1.ProjectId ?? 0,
        //                     ProjectText = t2.Name,
        //                     CompanyId = companyId,
        //                     ChoiceAreaText = t6.ChoiceAreaName,
        //                     Remarks = t1.Remarks,
        //                     CreatedDate = t1.CreatedDate,
        //                     SourceofMediaText = t1.SourceOfMedia
        //                 }).OrderByDescending(o => o.ClientId);

        //    // Get the total count of records matching the query.
        //    int totalCount = await query.CountAsync();

        //    // Apply pagination to the query.
        //    var pagedQuery = query.Skip(startIndex).Take(pageSize);

        //    // Fetch the paginated data from the database.
        //    models.DataList = await pagedQuery.ToListAsync();

        //    // Set pagination-related information in the model.
        //    models.pageNumber = pageNumber;
        //    models.pageSize = pageSize;
        //    models.Totalcount = totalCount;
        //    models.CompanyId = companyId;

        //    return models;
        //}





















        public async Task<CrmVm> GetClientById(int clientId)
        {
            var model = new CrmVm();
            model = await Task.Run(() => (from t1 in _context.KGRECustomers
                                          join t2 in _context.Companies on t1.CompanyId equals t2.CompanyId
                                          join t3 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t3.Id
                                          join t4 in _context.Employees on t3.EmployeeId equals t4.Id
                                          join t5 in _context.CrmServiceStatus on t1.ServiceStatusId equals t5.StatusId into t5_Join
                                          from t5 in t5_Join.DefaultIfEmpty()
                                          join t6 in _context.CrmChoiceAreas on t1.ChoieceAreaId equals t6.ChoiceAreaId into t6_Join
                                          from t6 in t6_Join.DefaultIfEmpty()
                                          join t7 in _context.DropDownItems on t1.GenderId equals t7.DropDownItemId
                                          join t8 in _context.ProductCategories on t1.ProjectId equals t8.ProductCategoryId into t8_Join
                                          from t8 in t8_Join.DefaultIfEmpty()
                                          join t9 in _context.DropDownItems on t1.ReligionId equals t9.DropDownItemId into t9_Join
                                          from t9 in t9_Join.DefaultIfEmpty()
                                          where t1.ClientId == clientId
                                          select new CrmVm
                                          {
                                              Name = t1.FullName,
                                              MobileNo = t1.MobileNo,
                                              GenderText = t7.Name,
                                              MobileNo2 = t1.MobileNo2,
                                              Email = t1.Email,
                                              Email2 = t1.Email1,
                                              ReligionText = t9.Name,
                                              DateofBirth = t1.DateofBirth,
                                              JobTitle = t1.Designation,
                                              OrganizationText = t1.DepartmentOrInstitution,
                                              ProjectId = t1.ProjectId ?? 0,
                                              ProjectText = t8.Name,
                                              CompanyId = t1.CompanyId ?? 0,
                                              CompanyText = t2.Name,
                                              ResponsibleOfficeName = t4.Name,
                                              StatusText = t5.StatusName,
                                              ChoiceAreaText = t6.ChoiceAreaName,
                                              ClientId = t1.ClientId,
                                              CreatedDate = DateTime.Now




                                          }).FirstOrDefault());
            return model;
        }

        public async Task<CrmVm> GetClientCopyById(int clientId)
        {
            var model = new CrmVm();
            model = await Task.Run(() => (from t1 in _context.KGRECustomers
                                          where t1.ClientId == clientId
                                          select new CrmVm
                                          {
                                              Name = t1.FullName,
                                              MobileNo = t1.MobileNo,
                                              MobileNo2 = t1.MobileNo2,
                                              Email = t1.Email,
                                              Email2 = t1.Email1,
                                              DateofBirth = t1.DateofBirth,
                                              JobTitle = t1.Designation,
                                              OrganizationText = t1.DepartmentOrInstitution,
                                              ProjectId = t1.ProjectId ?? 0,
                                              CompanyId = t1.CompanyId ?? 0,
                                              ClientId = t1.ClientId

                                          }).FirstOrDefault());
            return model;
        }




        public async Task<CrmScheduleVm> MakeSchedule(CrmScheduleVm model)
        {
            TimeSpan ts = DateTime.Parse(model.ScheduleTimeText).TimeOfDay;

            var objectToSave = await _context.CrmSchedules.SingleOrDefaultAsync(s => s.ClientId == model.ClientId
            && s.ScheduleDate == model.ScheduleDate
            && s.ScheduleTime == ts
            && s.ScheduleType == model.ScheduleType
            && s.CompanyId == model.CompanyId);

            if (objectToSave == null)
            {
                objectToSave = new CrmSchedule()
                {
                    ClientId = model.ClientId,
                    CompanyId = model.CompanyId,
                    ScheduleDate = model.ScheduleDate,
                    ScheduleTime = ts,
                    ScheduleType = model.ScheduleType,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                _context.CrmSchedules.Add(objectToSave);
            }
            objectToSave.Note = model.Note;
            objectToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            objectToSave.ModifiedDate = DateTime.Now;

            _context.SaveChanges();

            model = await GetScheduleById(objectToSave.ScheduleId);


            return model;
        }
        public async Task<CrmScheduleListVm> GetScheduleByClientId(int clientId, int userId, int companyId)
        {
            long uId = Convert.ToInt64(userId);

            var model = new CrmScheduleListVm();

            var clientData = await Task.Run(() => (from t1 in _context.KGRECustomers.Where(q => q.CompanyId == companyId)
                                                   join t2 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t2.Id
                                                   join t3 in _context.Employees on t2.EmployeeId equals t3.Id
                                                   join t4 in _context.ProductCategories on t1.ProjectId equals t4.ProductCategoryId into t4_Join
                                                   from t4 in t4_Join.DefaultIfEmpty()

                                                   join t5 in _context.CrmServiceStatus on t1.ServiceStatusId equals t5.StatusId into t5_Join
                                                   from t5 in t5_Join.DefaultIfEmpty()

                                                   join t6 in _context.CrmChoiceAreas on t1.ChoieceAreaId equals t6.ChoiceAreaId into t6_Join
                                                   from t6 in t6_Join.DefaultIfEmpty()
                                                   join t7 in _context.DropDownItems on t1.GenderId equals t7.DropDownItemId into t7_Join
                                                   from t7 in t7_Join.DefaultIfEmpty()
                                                   join t8 in _context.DropDownItems on t1.ReligionId equals t8.DropDownItemId into t8_Join
                                                   from t8 in t8_Join.DefaultIfEmpty()
                                                   join t9 in _context.DropDownItems on t1.TypeOfInterestId equals t9.DropDownItemId into t9_Join
                                                   from t9 in t9_Join.DefaultIfEmpty()
                                                   join t10 in _context.CrmPromotionalOffers on t1.PromotionalOfferId equals t10.OfferId into t10_Join
                                                   from t10 in t10_Join.DefaultIfEmpty()
                                                   join t11 in _context.CrmSourceMedias on t1.SourceOfMediaId equals t11.SourceMediaId into t11_Join
                                                   from t11 in t11_Join.DefaultIfEmpty()

                                                   where t1.ClientId == clientId
                                                   && (userId == 0 || t2.EmployeeId == uId || userId == 179 || userId == 42189 || userId==42204 || userId==180)
                                                   select new CrmVm
                                                   {
                                                       ClientId = t1.ClientId,
                                                       Name = t1.FullName,
                                                       GenderText = t7.Name,
                                                       ReligionText = t8.Name,
                                                       ReligionId = t1.ReligionId ?? 0,
                                                       GenderId = t1.GenderId,
                                                       MobileNo = t1.MobileNo,
                                                       CompanyId = t1.CompanyId ?? 0,
                                                       MobileNo2 = t1.MobileNo2,
                                                       Email = t1.Email,
                                                       Email2 = t1.Email1,
                                                       DateofBirth = t1.DateofBirth,
                                                       JobTitle = t1.Designation,
                                                       OrganizationText = t1.DepartmentOrInstitution,
                                                       PresentAddress = t1.PresentAddress,
                                                       PermanentAddress = t1.PermanentAddress,
                                                       ResponsibleOfficeName = t3.Name,
                                                       ResponsibleOfficerId = t1.ResponsibleOfficerId,
                                                       ProjectId = t1.ProjectId ?? 0,
                                                       ProjectText = t4.Name,
                                                       TypeofInterestText = t9.Name,
                                                       OfferId = t1.PromotionalOfferId,
                                                       OfferText = t10.OfferName,
                                                       SourceofMediaText = t11.SourceMediaName,
                                                       SourceofMediaId = t1.SourceOfMediaId,
                                                       ChoiceAreaId = t1.ChoieceAreaId,
                                                       CampaignText = t1.CampaignName,
                                                       ChoiceAreaText = t6.ChoiceAreaName,
                                                       StatusId = t1.ServiceStatusId,
                                                       StatusText = t5.StatusName,
                                                       ReferredBy = t1.ReferredBy,
                                                       DateOfContact = t1.DateOfContact,
                                                       TypeofInterestId = t1.TypeOfInterestId,
                                                       Remarks = t1.Remarks,
                                                       CreatedDate=t1.CreatedDate

                                                   }
                                                 ).FirstOrDefaultAsync());

            if (clientData == null)
            {
                model.ClientData.CompanyId = companyId;
                model.ClientData.ClientId = clientId;
                model.HasMessage = true;
                model.MessageList.Add("Something wrong !");
                return model;
            }
            model.ClientData = clientData;
            model.ClientData.GenderList = await GetDropdownGender();
            model.ClientData.ReligionList = await GetDropdownReligion();
            model.ClientData.ChoiceofAreaList = await GetDropdownChoiceofArea(companyId);
            model.ClientData.SourceofMediaList = await GetDropdownSourceofMedia(companyId);
            model.ClientData.PromotionalOfferList = await GetDropdownPromotionalOffer(companyId);
            model.ClientData.DealingOfficerList = await GetDropdownDealingOfficer(companyId);
            model.ClientData.ProjectList = await GetDropdownProject(companyId);

            model.ClientData.TypeofInterestList = await GetDropdownTypeofInterest();


            model.ClientData.DateofBirthText = model.ClientData.DateofBirth == null ? "" : model.ClientData.DateofBirth.Value.ToShortDateString();

            long serviceClientId = Convert.ToInt64(clientId);

            model.ServiceStatusHistList = await Task.Run(() => (from t1 in _context.KGREHistories
                                                                where t1.KGREId == serviceClientId && t1.IsActive
                                                                select new ServiceStatusHistVm
                                                                {
                                                                    KgreHistoryId = t1.KGREHistoryID,
                                                                    ClientId = t1.KGREId,
                                                                    CompanyId = t1.CompanyId ?? companyId,
                                                                    HistoryText = t1.ChangeHistory

                                                                }).OrderByDescending(o => o.KgreHistoryId).ToListAsync());


            model.DataList = await Task.Run(() => (from t1 in _context.CrmSchedules
                                                   where t1.ClientId == clientId && t1.IsActive
                                                   select new CrmScheduleVm
                                                   {
                                                       ClientId = t1.ClientId,
                                                       CompanyId = t1.CompanyId,
                                                       ScheduleDate = t1.ScheduleDate,
                                                       ScheduleTime = t1.ScheduleTime,
                                                       ScheduleType = t1.ScheduleType,
                                                       Note = t1.Note,
                                                       ScheduleId = t1.ScheduleId

                                                   }).OrderByDescending(o => new { o.ScheduleDate, o.ScheduleTime }).ToListAsync());

            model.NoteList = await Task.Run(() => (from t1 in _context.KGREHistories
                                                   where t1.KGREId == clientId && t1.IsActive
                                                   select new ServiceStatusHistVm
                                                   {
                                                       ClientId = t1.KGREId,
                                                       Notes = t1.Note == null ? " " : t1.Note,
                                                       ModiifyDate = t1.CreatedDate



                                                   }).ToListAsync());

            model.NoteObjS = model.NoteList.LastOrDefault(d => d.ClientId == clientId);
            model.ClientData.ClientId = clientId;



            return model;
        }
        public async Task<CrmScheduleVm> GetScheduleById(int scheduleId)
        {
            var model = new CrmScheduleVm();
            model = await Task.Run(() => (from t1 in _context.CrmSchedules
                                          join t2 in _context.KGRECustomers on t1.ClientId equals t2.ClientId
                                          where t1.ScheduleId == scheduleId
                                          select new CrmScheduleVm
                                          {
                                              CompanyId = t1.CompanyId,
                                              ScheduleDate = t1.ScheduleDate,
                                              ScheduleId = t1.ScheduleId,
                                              ScheduleTime = t1.ScheduleTime,
                                              ScheduleType = t1.ScheduleType,
                                              ClientName = t2.FullName,
                                              Note = t1.Note,
                                              ClientId = t1.ClientId
                                          }).FirstOrDefault());
            return model;
        }

        public async Task<CrmUploadListVm> GetAllCrmUpload(int companyId)
        {
            var model = new CrmUploadListVm();
            //model.DataList = await Task.Run(() => (from t1 in _context.KGRECustomers
            //                                       where t1.CompanyId == companyId
            //                                       group t1 by new { t1.UploadSerialNo, t1.UploadDateTime } into g
            //                                       select new CrmUploadVm
            //                                       {
            //                                           LastUploadNo = g.Key.UploadSerialNo,
            //                                           UploadDateTime = g.Key.UploadDateTime
            //                                       }
            //                            ).ToListAsync());
            return model;
        }

        public async Task<CompanyListVm> GetAllCompany()
        {
            var model = new CompanyListVm();
            model.DataList = await Task.Run(() => (from t1 in _context.Companies
                                                   where t1.IsActive
                                                   select new CompanyVm
                                                   {
                                                       MobileNo = t1.Phone,
                                                       Address = t1.Address,
                                                       CompanyId = t1.CompanyId,
                                                       CompanyCode = t1.ShortName,
                                                       CompanyText = t1.Name,
                                                       OrderNo = t1.OrderNo
                                                   }).ToListAsync());
            return model;
        }


        public async Task<ResponsibleOfficerListVm> GetAllResponsibleOfficer(int companyId)
        {
            var model = new ResponsibleOfficerListVm();
            model.DataList = await Task.Run(() => (from t1 in _context.TeamInfoes
                                                   join t2 in _context.Employees on t1.EmployeeId equals t2.Id
                                                   join t3 in _context.Companies on t1.CompanyId equals t3.CompanyId
                                                   join t4 in _context.Employees on t1.EmployeeId equals t4.Id
                                                   where t1.IsActive
                                                   && t2.Active
                                                   && t3.IsActive
                                                   && t1.CompanyId == companyId
                                                   select new ResponsibleOfficerVm
                                                   {
                                                       CompanyId = t1.CompanyId,
                                                       CompanyText = t3.Name,
                                                       ResponsibleOfficerId = t1.Id,
                                                       ResponsibleOfficerName = t2.Name,
                                                       ResponsibleOfficerText = t2.Name,
                                                       TeamName = t1.Name,
                                                       LeaderName = t4.Name,
                                                       LeaderId = t1.LeadId ?? 0,
                                                       MemberType = t1.IsLeader ? "Leader" : "Officer",
                                                   }
                                                 ).ToListAsync());

            return model;
        }


        public async Task<ProjectListVm> GetAllproject(int companyId)
        {
            var model = new ProjectListVm();
            model.Datalist = await Task.Run(() => (from t1 in _context.ProductCategories
                                                   where
                                                   t1.CompanyId == companyId
                                                   && t1.IsCrm
                                                   && t1.IsActive
                                                   select new ProjectVm
                                                   {
                                                       ProjectId = t1.ProductCategoryId,
                                                       ProjectName = t1.Name

                                                   }
                                                 ).OrderBy(o => o.ProjectName).ToListAsync());

            return model;
        }


        public async Task<ProjectListVm> GetAllprojectForcrm(int companyId)
        {
            var model = new ProjectListVm();
            model.ProjectList = await Task.Run(() => (from t1 in _context.ProductCategories
                                                   where
                                                   t1.CompanyId == companyId
                                                   && t1.IsCrm
                                                   && t1.IsActive
                                                   select new ProjectVm
                                                   {
                                                       ProjectId = t1.ProductCategoryId,
                                                       ProjectName = t1.Name

                                                   }
                                                 ).OrderBy(o => o.ProjectName).ToListAsync());

            return model;
        }

        public async Task<ResponsibleOfficerVm> SwitchResponsibleOffice(int clientId, int companyId)
        {
            var model = new ResponsibleOfficerVm();
            var obj = await _context.KGRECustomers.SingleOrDefaultAsync(q => q.ClientId == clientId);
            if (obj == null)
            {
                return model;
            }
            obj.CompanyId = companyId;
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<TeamListVm> GetTeamList(int companyId)
        {
            var model = new TeamListVm();

            var dataList = await _context.TeamInfoes.Where(e => e.CompanyId == companyId).ToListAsync();

            foreach (var d in dataList.Where(q => q.IsLeader))
            {
                var td1 = new TeamVm
                {
                    TeamName = d.Name,
                    LeaderName = _context.Employees.SingleOrDefault(q => q.Id == d.EmployeeId).Name
                };
                if (d.IsLeader)
                {
                    foreach (var d1 in dataList.Where(q => q.LeadId == d.Id))
                    {
                        var offo = new OfficerVm
                        {
                            TeamName = d1.Name,

                            OfficerName = _context.Employees.SingleOrDefault(q => q.Id == d1.EmployeeId).Name,

                        };
                        td1.DataList.Add(offo);


                    }
                }
                model.DataList.Add(td1);


            }
            return model;
        }


        public async Task<ServiceStatusListVm> GetAllServiceStatus()
        {
            var model = new ServiceStatusListVm();
            model.Datalist = await Task.Run(() => (from t1 in _context.CrmServiceStatus
                                                   select new ServiceStatusVm
                                                   {
                                                       StatusId = 0,
                                                       StatusText = t1.StatusName,

                                                   }
                                                 ).ToListAsync());

            return model;
        }
        public async Task<ProjectListVm> GetAllProjects()
        {
            var model = new ProjectListVm();
            model.Datalist = await Task.Run(() => (from t1 in _context.ProductCategories
                                                   where t1.IsActive
                                                   select new ProjectVm
                                                   {
                                                       ProjectId = t1.ProductCategoryId,
                                                       ProjectName = t1.Name

                                                   }
                                                 ).ToListAsync());

            return model;
        }

        public async Task<ServiceStatusListVm> GetAllServiceStatus(int companyId)
        {
            var model = new ServiceStatusListVm();
            model.Datalist = await Task.Run(() => (from t1 in _context.CrmServiceStatus
                                                   where t1.CompanyId == companyId && !t1.IsDelete
                                                   select new ServiceStatusVm
                                                   {
                                                       StatusId = t1.StatusId,
                                                       companyId = t1.CompanyId,
                                                       StatusText = t1.StatusName
                                                   }).ToListAsync());
            model.CompanyId = companyId;
            return model;
        }


        //public async Task<ProjectVm> GetAllProject(int companyId)
        //{
        //    var model = new ProjectVm();
        //  model = await Task.Run(() => (from t1 in _context.ProductCategories
        //                                           where t1.CompanyId == companyId && t1.IsActive && t1.IsCrm
        //                                           select new ProjectVm
        //                                           {
        //                                               ProjectId = t1.ProductCategoryId,
        //                                               ProjectName=t1.Name
                                                    
        //                                           }).ToListAsync());
        //    model.CompanyId = companyId;
        //    return model;
        //}
        public async Task<List<SelectVm>> GetService(int CompanyId)
        {

            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.CrmServiceStatus
                                          where t1.CompanyId==CompanyId && !t1.IsDelete
                                          select new SelectVm
                                          {
                                              Id = t1.StatusId,
                                              Name = t1.StatusName
                                          }).ToListAsync());

            return model;
        }


        public async Task<ServiceStatusHistVm> GetServiceHistoryById(int KgreHistoryId)
        {
            var model = new ServiceStatusHistVm();
            var obj = await _context.KGREHistories.SingleOrDefaultAsync(q => q.KGREHistoryID == KgreHistoryId);
            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong!");
                return model;
            }
            else
            {
                model.CompanyId = obj.CompanyId ?? 0;
                model.HistoryText = obj.ChangeHistory;
                model.KgreHistoryId = obj.KGREHistoryID;
                model.ClientId = obj.KGREId;
            }

            return model;
        }
        public async Task<CrmScheduleVm> GetClientScheduleById(int scheduleId)
        {
            var model = new CrmScheduleVm();
            var obj = await _context.CrmSchedules.SingleOrDefaultAsync(q => q.ScheduleId == scheduleId);
            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong!");
                return model;
            }
            else
            {
                model.CompanyId = obj.CompanyId;
                model.Note = obj.Note;
                model.ScheduleId = obj.ScheduleId;
                model.ScheduleTime = obj.ScheduleTime;
                model.ScheduleType = obj.ScheduleType;
                model.ScheduleDate = obj.ScheduleDate;
                model.ClientId = obj.ClientId;
            }

            return model;
        }
        public async Task<ServiceStatusHistVm> UpdateStatusNote(ServiceStatusHistVm model)
        {
            var obj = await _context.KGREHistories.SingleOrDefaultAsync(q => q.KGREHistoryID == model.KgreHistoryId);
            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong!");
                return model;
            }
            else
            {
                obj.ChangeHistory = model.HistoryText + ",Update By :[" + DateTime.Now + "]" + System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                model.HasMessage = true;
                model.MessageList.Add("Status Note update successfully");
            }
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<CrmScheduleVm> UpdateScheduleNote(CrmScheduleVm model)
        {
            var obj = await _context.CrmSchedules.SingleOrDefaultAsync(q => q.ScheduleId == model.ScheduleId);
            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong!");
                return model;
            }
            else
            {
                //    obj.ChangeHistory = model.HistoryText + ",Update By :[" + DateTime.Now + "]" + System.Web.HttpContext.Current.User.Identity.Name;
                //    obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                //    obj.ModifiedDate = DateTime.Now;
                //    model.HasMessage = true;
                //    model.MessageList.Add("Status Note update successfully");

                obj.ScheduleDate = model.ScheduleDate;
                obj.ScheduleTime = model.ScheduleTime;
                obj.Note = model.Note;
                model.HasMessage = true;
                model.MessageList.Add("Schedule Note update successfully");

            }
            await _context.SaveChangesAsync();

            return model;
        }



        public async Task<CrmScheduleVm> UpdateClientSchedule(CrmScheduleVm model)
        {
            var obj = await _context.CrmSchedules.SingleOrDefaultAsync(q => q.ScheduleId == model.ScheduleId);
            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong!");
                return model;
            }
            else
            {
                obj.Note = model.Note + ",Update By :[" + DateTime.Now + "]" + System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                model.HasMessage = true;
                model.MessageList.Add("Schedule update successfully");
            }
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<CrmScheduleVm> RemoveClientSchedule(CrmScheduleVm model)
        {
            var obj = await _context.CrmSchedules.SingleOrDefaultAsync(q => q.ScheduleId == model.ScheduleId);
            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong!");
                return model;
            }
            else
            {
                obj.IsActive = false;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                model.HasMessage = true;
                model.MessageList.Add("Schedule Remove successfully");
            }
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<ServiceStatusHistVm> RemoveServiceStatusNote(ServiceStatusHistVm model)
        {
            var obj = await _context.KGREHistories.SingleOrDefaultAsync(q => q.KGREHistoryID == model.KgreHistoryId);
            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong!");
                return model;
            }
            else
            {
                obj.IsActive = false;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                model.HasMessage = true;
                model.MessageList.Add("Status Note Remove successfully");
            }
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<ServiceStatusVm> SaveServiceStatus(ServiceStatusVm model)
        {
            var data = new ServiceStatusVm();
            var objToSave = _context.CrmServiceStatus.SingleOrDefault(e => e.StatusId == model.StatusId);
            if (objToSave == null)
            {
                objToSave = new CrmServiceStatu()
                {
                    StatusName = model.StatusText,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
                _context.CrmServiceStatus.Add(objToSave);

                data.HasMessage = true;
                data.MessageList.Add("Successfull added");
            }
            else
            {
                objToSave.StatusName = model.StatusText;

                objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                objToSave.CreatedDate = DateTime.Now;

                data.HasMessage = true;
                data.MessageList.Add("Successfull Updated");

            }
            objToSave.CompanyId = model.companyId;
            await _context.SaveChangesAsync();
            return data;
        }




        public async Task<ChoiceAreaListVm> GetAllChoiceArea(int companyId)
        {
            var model = new ChoiceAreaListVm();
            model.DataList = await Task.Run(() => (from t1 in _context.CrmChoiceAreas
                                                   where t1.CompanyId == companyId
                                                   select new ChoiceAreaVm
                                                   {
                                                       CompanyId = t1.CompanyId,
                                                       ChoiceAreaId = t1.ChoiceAreaId,
                                                       ChoiceAreaText = t1.ChoiceAreaName
                                                   }).OrderBy(o => o.ChoiceAreaText)
                                                   .ToListAsync());
            model.CompanyId = companyId;
            return model;
        }
        public async Task<List<SelectVm>> GetDropdownChoiceArea(int companyId)
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.CrmChoiceAreas
                                          where t1.CompanyId == companyId
                                          select new SelectVm
                                          {
                                              Id = t1.ChoiceAreaId,
                                              Name = t1.ChoiceAreaName
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;

        }
        public async Task<ChoiceAreaVm> SaveChoiceArea(ChoiceAreaVm model)
        {
            var objToSave = await _context.CrmChoiceAreas.SingleOrDefaultAsync(q => q.CompanyId == model.CompanyId
            && q.ChoiceAreaId == model.ChoiceAreaId);
            if (objToSave == null)
            {
                objToSave = new CrmChoiceArea()
                {
                    CompanyId = model.CompanyId,
                    ChoiceAreaName = model.ChoiceAreaText,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
                _context.CrmChoiceAreas.Add(objToSave);
            }
            else
            {
                objToSave.ChoiceAreaName = model.ChoiceAreaText;
                objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                objToSave.CreatedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return model;
        }
        public async Task<object> GetAutoCompleteClientName(string prefix)
        {

            var v = await Task.Run(() => (from t1 in _context.KGRECustomers
                                          join t2 in _context.Companies on t1.CompanyId equals t2.CompanyId
                                          join t3 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t3.Id
                                          join t4 in _context.Employees on t3.EmployeeId equals t4.Id
                                          where t1.FullName.Contains(prefix)
                                          select new
                                          {
                                              label = "[" + t1.FullName + "-" + t1.MobileNo + "] " + t2.ShortName + "-" + t4.Name,
                                              val = t1.ClientId
                                          }).OrderBy(x => x.label).Take(20).ToListAsync());
            return v;
        }
        public async Task<object> GetAutoCompleteClientMobile(string prefix)
        {

            var v = await Task.Run(() => (from t1 in _context.KGRECustomers
                                          join t2 in _context.Companies on t1.CompanyId equals t2.CompanyId
                                          join t3 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t3.Id
                                          join t4 in _context.Employees on t3.EmployeeId equals t4.Id
                                          where (t1.MobileNo.StartsWith(prefix) || t1.MobileNo2.StartsWith(prefix))
                                          select new
                                          {
                                              label = "[" + t1.MobileNo + "-" + t1.FullName + "] " + t2.ShortName + "-" + t4.Name,
                                              val = t1.ClientId
                                          }).OrderBy(x => x.label).Take(20).ToListAsync());
            return v;
        }

        public async Task<List<SelectVm>> GetDropdownGender()
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.DropDownItems
                                          where t1.DropDownTypeId == 3
                                          select new SelectVm
                                          {
                                              Id = t1.DropDownItemId,
                                              Name = t1.Name
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }

        public async Task<List<SelectVm>> GetUploaddate(int companyId)
        {
            var model = new List<SelectVm>();
            var data = await Task.Run(() => (from t1 in _context.KGRECustomers
                                             where t1.CompanyId == companyId
                                             && t1.UploadSerialNo != 0
                                             group t1 by t1.UploadSerialNo into g
                                             select new SelectVm
                                             {
                                                 Id = g.Key,
                                                 uploaddatetime = g.FirstOrDefault().UploadDateTime

                                             }).OrderByDescending(e => e.Id)
                                          .ToListAsync());

            foreach (var item in data)
            {
                var s = new SelectVm
                {
                    Id = item.Id,
                    Name = item.Id.ToString("00") + "-" + item.uploaddatetime.ToString("dd-mm-yyyy")
                };
                model.Add(s);
            }
            return model;
        }

        public async Task<List<SelectVm>> GetDropdownReligion()
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.DropDownItems
                                          where t1.DropDownTypeId == 9
                                          select new SelectVm
                                          {
                                              Id = t1.DropDownItemId,
                                              Name = t1.Name
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }

        public async Task<List<SelectVm>> GetDropdownDealingOfficer(int companyId)
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.TeamInfoes
                                          join t2 in _context.Employees on t1.EmployeeId equals t2.Id
                                          where t1.CompanyId == companyId && (t1.Name != "Existing Team" )
                                          select new SelectVm
                                          {
                                              Id = t1.Id,
                                              Name = t2.Name
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }











        public async Task<List<SelectVm>> GetDropdownDealingOfficerForLead(int companyId, int uid)
        {

            TeamInfo teaminfo = new TeamInfo();
            if (uid == 179||uid==180)
            {
                teaminfo = (_context.TeamInfoes.SingleOrDefault(x => x.EmployeeId == uid && x.CompanyId == companyId && (x.EmployeeId == 179||x.EmployeeId==180) && x.ManagerId == 1));


            }
            else
            {
                teaminfo = (_context.TeamInfoes.SingleOrDefault(x => x.EmployeeId == uid && x.CompanyId == companyId && x.Name != "Existing Team"));

            }


            var model = new List<SelectVm>();
            if (teaminfo.ManagerId == 1)
            {
                uid = 179;
                model = await Task.Run(() => (from t1 in _context.TeamInfoes
                                              join t2 in _context.Employees on t1.EmployeeId equals t2.Id
                                              where t1.CompanyId == companyId && ((t1.ManagerId == uid) || t1.ManagerId == 1)

                                              select new SelectVm
                                              {
                                                  Id = t1.Id,
                                                  Name = t2.Name,
                                                  TeamName = t1.Name
                                              }).OrderBy(o => o.Name).ToListAsync());



            }
            else
            {
                model = await Task.Run(() => (from t1 in _context.TeamInfoes.Where(x => x.Name == teaminfo.Name)
                                              join t2 in _context.Employees on t1.EmployeeId equals t2.Id
                                              where t1.CompanyId == companyId && (!t1.IsLeader || t1.EmployeeId == uid)
                                              select new SelectVm
                                              {
                                                  Id = t1.Id,
                                                  Name = t2.Name,
                                                  TeamName = t1.Name

                                              }).OrderBy(o => o.Name).ToListAsync());

            }

            return model;
        }






        public async Task<List<SelectVm>> GetDropdownProject(int companyId)
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.ProductCategories
                                          where t1.CompanyId == companyId && t1.IsActive
                                          && t1.IsCrm
                                          select new SelectVm
                                          {
                                              Id = t1.ProductCategoryId,
                                              Name = t1.Name
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }

      





        public async Task<List<SelectVm>> GetDropdownServiceStatus(int companyId)
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.CrmServiceStatus
                                          where t1.CompanyId == companyId && !t1.IsDelete
                                          select new SelectVm
                                          {
                                              Id = t1.StatusId,
                                              Name = t1.StatusName
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }

        public async Task<List<SelectVm>> GetDropdownTypeofInterest()
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.DropDownItems
                                          where t1.DropDownTypeId == 31
                                          select new SelectVm
                                          {
                                              Id = t1.DropDownItemId,
                                              Name = t1.Name
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }

        public async Task<List<SelectVm>> GetDropdownSourceofMedia(int companyId)
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.CrmSourceMedias
                                          where t1.CompanyId == companyId
                                          select new SelectVm
                                          {
                                              Id = t1.SourceMediaId,
                                              Name = t1.SourceMediaName
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }

        public async Task<List<SelectVm>> GetDropdownPromotionalOffer(int companyId)
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.CrmPromotionalOffers
                                          where t1.CompanyId == companyId && t1.IsActive && t1.IsOpen
                                          select new SelectVm
                                          {
                                              Id = t1.OfferId,
                                              Name = t1.OfferName
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }

        public async Task<List<SelectVm>> GetDropdownChoiceofArea(int companyId)
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.CrmChoiceAreas
                                          where t1.CompanyId == companyId
                                          select new SelectVm
                                          {
                                              Id = t1.ChoiceAreaId,
                                              Name = t1.ChoiceAreaName
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }
        public async Task<List<SelectVm>> GetDropdownCompany()
        {
            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.Companies
                                          select new SelectVm
                                          {
                                              Id = t1.CompanyId,
                                              Name = t1.Name
                                          }).OrderBy(o => o.Name).ToListAsync());
            return model;
        }
        public async Task<bool> DeleteServiceStatus(int id)
        {
            var model = new ServiceStatusVm();
            var obj = await _context.CrmServiceStatus.SingleOrDefaultAsync(e => e.StatusId == id);
            if (obj == null)
            {
                return false;
            }
            obj.IsDelete = true;
            obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            obj.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ServiceStatusVm> GetServicestatusById(int id)
        {

            var obj = await _context.CrmServiceStatus.SingleOrDefaultAsync(e => e.StatusId == id);
            return new ServiceStatusVm() { StatusId = obj.StatusId, StatusText = obj.StatusName };
        }
        public async Task<SelectModelVm> UpdateResofcrID(SelectModelVm Model, int uId)
        {
            var data = new SelectModelVm();
            var user = _context.TeamInfoes.SingleOrDefault(q => q.IsActive && q.IsLeader && q.EmployeeId == uId);
            if (user == null)
            {
                uId = 0;
            }
            else
            {
                uId = user.Id;
                try
                {

                    var obj = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == Model.Id);

                    obj.ResponsibleOfficerId = Model.CustomId;
                    obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    obj.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return data;
                }
                catch (Exception ex)
                {
                    var message = ex;
                }
            }


            return data;


        }

        public async Task<SelectModelVm> UpdateServstsId(SelectModelVm Model)
        {
            var data = new SelectModelVm();
            try
            {

                var obj = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == Model.Id);

                obj.ServiceStatusId = Model.CustomId;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return data;
            }
            catch (Exception ex)
            {
                var message = ex;
            }
            return data;
        }




        public async Task<SelectModelVm> UpdateCompany(SelectModelVm Model, int uId)
        {
            var data = new SelectModelVm();
            var user = _context.TeamInfoes.SingleOrDefault(q => q.IsActive && q.IsLeader && q.EmployeeId == uId);
            if (user == null)
            {
                uId = 0;
            }
            else
            {
                uId = user.Id;
                try
                {
                    var obj = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == Model.Id);

                    obj.CompanyId = Model.CustomId;
                    obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    obj.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return data;
                }
                catch (Exception ex)
                {
                    var message = ex;
                }

            }

            //return new ServiceStatusVm() { StatusId = obj.StatusId, StatusText = obj.StatusName };
            return data;
        }
        public async Task<SelectModelVm> SwitchServiceStatus(SelectModelVm Model)
        {
            var data = new SelectModelVm();
            try
            {
                var obj = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == Model.Id);


                var statusList = _context.CrmServiceStatus.ToList();
                var project = _context.ProductCategories.ToList();


                var serviceHistory = new KGREHistory();
                serviceHistory.CompanyId = obj.CompanyId;
                //serviceHistory.ChangeHistory = Model.Note + " Status:  <b>" + statusList.FirstOrDefault(s => s.StatusId == obj.ServiceStatusId)?.StatusName + "</b> to <b>" + statusList.FirstOrDefault(s => s.StatusId == Model.CustomId)?.StatusName + "</b> Changed By " + System.Web.HttpContext.Current.User.Identity.Name + " [ " + DateTime.Now + "]";

                if (Model.CustomeId2 != 0 && Model.CustomeId2!=obj.ProjectId)
                {
                    serviceHistory.ChangeHistory = "<b> Date:</b>  " + DateTime.Now.ToString("dd-MMM-yyyy") + "<br> <b> Status:</b> " + "<span class='text-danger'>" + statusList.FirstOrDefault(s => s.StatusId == obj.ServiceStatusId)?.StatusName + "</span> To <span class='text-success'>" + statusList.FirstOrDefault(s => s.StatusId == Model.CustomId)?.StatusName + " </span></b><br><b>Project:</b>"+"<span class='text-danger'>"+project.FirstOrDefault(s=>s.ProductCategoryId==obj.ProjectId)?.Name+"</span> To <span class='text-success'>"+project.FirstOrDefault(s=>s.ProductCategoryId==Model.CustomeId2)?.Name+"</span></b><br><b>Service Note:</b>" + Model.Note + "" + "<br> <b> Changed By: </b> " + System.Web.HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    serviceHistory.ChangeHistory = "<b> Date:</b>  " + DateTime.Now.ToString("dd-MMM-yyyy") + "<br> <b> Status:</b> " + "<span class='text-danger'>" + statusList.FirstOrDefault(s => s.StatusId == obj.ServiceStatusId)?.StatusName + "</span> To <span class='text-success'>" + statusList.FirstOrDefault(s => s.StatusId == Model.CustomId)?.StatusName + " </span></b><br><b>Project:</b>"+"<span class='text-success'>"+project.FirstOrDefault(s=>s.ProductCategoryId==obj.ProjectId)?.Name+"</span></b><br><b> Service Note:</b>   " + Model.Note + "" + "<br> <b> Changed By: </b> " + System.Web.HttpContext.Current.User.Identity.Name;

                }
                serviceHistory.KGREId = obj.ClientId;
                serviceHistory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                serviceHistory.CreatedDate = DateTime.Now;
                serviceHistory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                serviceHistory.ModifiedDate = DateTime.Now;
                serviceHistory.IsActive = true;
                serviceHistory.Note = Model.Note;

                _context.KGREHistories.Add(serviceHistory);
                if (Model.CustomeId2 != 0)
                {
                    obj.ProjectId = Model.CustomeId2; 
                }
                obj.ServiceStatusId = Model.CustomId;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();


                return data;
            }
            catch (Exception ex)
            {
                var message = ex;
            }
            return data;
        }

        public async Task<CrmVm> SaveClient(CrmVm model)
        {
            var data = new CrmVm();


            try
            {
                var objToSave = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == model.ClientId);

                if (objToSave == null)
                {

                    var mobileNoExisting = await _context.KGRECustomers.Where(q => q.CompanyId == model.CompanyId && (q.MobileNo == model.MobileNo
                    || q.MobileNo2 == model.MobileNo)).ToListAsync();


                    if (mobileNoExisting.Count() > 0)
                    {
                        data.CompanyId = model.CompanyId;
                        data.HasMessage = true;
                        data.MessageList.Add("This mobile no is already exist");
                        return data;
                    }
                    if (model.MobileNo.Length <= 11 && !model.MobileNo.StartsWith("0"))
                    {
                        data.CompanyId = model.CompanyId;
                        data.HasMessage = true;
                        data.messageE = "Exist";
                        return data;
                    }

                    objToSave = new KGRECustomer()
                    {
                        MobileNo = model.MobileNo,
                        CompanyId = model.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        DateOfContact = model.DateOfContact,

                    };
                    _context.KGRECustomers.Add(objToSave);
                }
                else
                {
                    objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    objToSave.ModifiedDate = DateTime.Now;

                }

                if (model.IsLeader)
                {
                    if (model.ResponsibleOfficerId == 0)
                    {
                        objToSave.ResponsibleOfficerId = _context.TeamInfoes.Where(q => q.CompanyId == model.CompanyId && q.EmployeeId == model.UserId).Select(x => x.Id).FirstOrDefault();
                    }
                    else
                    {
                        objToSave.ResponsibleOfficerId = model.ResponsibleOfficerId;
                    }


                }
                else
                {
                    if (model.ResponsibleOfficerId == 0)
                    {

                        objToSave.ResponsibleOfficerId = _context.TeamInfoes.SingleOrDefault(q => q.CompanyId == model.CompanyId && (q.EmployeeId == model.UserId && (q.Name != "Existing Team" || q.EmployeeId == 179))).Id;
                    }
                    else
                    {
                        objToSave.ResponsibleOfficerId = model.ResponsibleOfficerId;
                    }
                }

                if (model.GenderId == 0)
                {
                    model.GenderId = 1;
                }
                if (model.ReligionId == 0)
                {
                    model.ReligionId = 23;
                }

                objToSave.FullName = model.Name;
                objToSave.GenderId = model.GenderId;
                objToSave.ReligionId = model.ReligionId;
                objToSave.DateofBirth = model.DateofBirth;
                objToSave.MobileNo2 = model.MobileNo2;
                objToSave.Email = model.Email;
                objToSave.Email1 = model.Email2;
                objToSave.PresentAddress = model.PresentAddress;
                objToSave.PermanentAddress = model.PermanentAddress;

                objToSave.ProjectId = model.ProjectId;
                objToSave.TypeOfInterestId = model.TypeofInterestId;

                objToSave.PromotionalOfferId = model.OfferId;
                objToSave.ChoieceAreaId = model.ChoiceAreaId;
                objToSave.CampaignName = model.CampaignText;
                objToSave.ReferredBy = model.ReferredBy;
                objToSave.UploadDateTime = DateTime.Now;
                objToSave.SourceOfMediaId = model.SourceofMediaId;
                objToSave.Remarks = model.Remarks;
                objToSave.ServiceStatusId = model.StatusId;
                await _context.SaveChangesAsync();
                data.CompanyId = objToSave.CompanyId ?? 0;
                data.ClientId = objToSave.ClientId;

            }
            catch (Exception ex)
            {
                throw ex;

            }

            return data;
        }

        public async Task<PromotionalOfferVm> GetPromotionalOfferById(int id)
        {

            var obj = await _context.CrmPromotionalOffers.SingleOrDefaultAsync(e => e.OfferId == id);
            return new PromotionalOfferVm()
            {
                OfferId = obj.OfferId,
                OfferName = obj.OfferName,
                StartDate = obj.StartDate,
                IsOpen = obj.IsOpen,
                EndDate = obj.EndDate
            };
        }

        public async Task<PromotionalOfferListVm> GetAllPromotionalOffer(int companyId)
        {
            var model = new PromotionalOfferListVm();
            model.Datalist = await Task.Run(() => (from t1 in _context.CrmPromotionalOffers
                                                   where t1.CompanyId == companyId && t1.IsActive
                                                   select new PromotionalOfferVm
                                                   {
                                                       OfferId = t1.OfferId,
                                                       OfferName = t1.OfferName,
                                                       StartDate = t1.StartDate,
                                                       EndDate = t1.EndDate,
                                                       OfferDays = t1.offerDays,
                                                       IsActive = t1.IsActive,
                                                       IsOpen = t1.IsOpen,

                                                       OfferStatusText = t1.IsOpen ? "Open" : "Close"
                                                   }).ToListAsync());
            model.CompanyId = companyId;
            return model;
        }
        public async Task<PromotionalOfferVm> SavePromotionalOffer(PromotionalOfferVm model)
        {
            var objToSave = _context.CrmPromotionalOffers.SingleOrDefault(e => e.OfferId == model.OfferId);

            int offerDays = Convert.ToInt32((model.EndDate - model.StartDate).TotalDays);
            if (objToSave == null)
            {
                objToSave = new CrmPromotionalOffer()
                {
                    OfferName = model.OfferName,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsActive = true,
                    IsOpen = model.IsOpen,
                    offerDays = offerDays,
                    CompanyId = model.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now

                };
                _context.CrmPromotionalOffers.Add(objToSave);
            }
            else
            {
                objToSave.OfferName = model.OfferName;
                objToSave.StartDate = model.StartDate;
                objToSave.EndDate = model.EndDate;
                objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                objToSave.CreatedDate = DateTime.Now;
                objToSave.IsOpen = model.IsOpen;

            }
            await _context.SaveChangesAsync();
            return model;
        }


        public async Task<CrmUploadListVm> FilteringClientlist(CrmUploadListVm model, int userId)
        {
            var data = new CrmUploadListVm();
            List<long> teamMambers = new List<long>();
            TeamInfo teamInfo = new TeamInfo();
            if (userId == 42189 || userId == 42204 || userId == 180)
            {
                userId = 179;
            }

            if (userId != 179)
            {

                teamInfo = _context.TeamInfoes.Where(x => x.EmployeeId == userId && x.CompanyId == model.CompanyId).FirstOrDefault();
                if (teamInfo.IsLeader == true)
                {
                    teamMambers = _context.TeamInfoes.Where(x => x.LeadId == teamInfo.Id || x.Id == teamInfo.Id).Select(x => x.EmployeeId).ToList();

                }
            }



            data.DataList = await Task.Run(() => (from t1 in _context.KGRECustomers
                                                  join t2 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t2.Id
                                                  join t3 in _context.Employees on t2.EmployeeId equals t3.Id
                                                  join t4 in _context.ProductCategories on t1.ProjectId equals t4.ProductCategoryId into t4_Join
                                                  from t4 in t4_Join.DefaultIfEmpty()
                                                  join t5 in _context.CrmServiceStatus on t1.ServiceStatusId equals t5.StatusId into t5_Join
                                                  from t5 in t5_Join.DefaultIfEmpty()
                                                  join t6 in _context.CrmChoiceAreas on t1.ChoieceAreaId equals t6.ChoiceAreaId into t6_Join
                                                  from t6 in t6_Join.DefaultIfEmpty()
                                                  join t7 in _context.DropDownItems on t1.GenderId equals t7.DropDownItemId into t7_Join
                                                  from t7 in t7_Join.DefaultIfEmpty()
                                                  join t8 in _context.DropDownItems on t1.ReligionId equals t8.DropDownItemId into t8_Join
                                                  from t8 in t8_Join.DefaultIfEmpty()
                                                  join t9 in _context.DropDownItems on t1.TypeOfInterestId equals t9.DropDownItemId into t9_Join
                                                  from t9 in t9_Join.DefaultIfEmpty()
                                                  join t10 in _context.CrmPromotionalOffers on t1.PromotionalOfferId equals t10.OfferId into t10_Join
                                                  from t10 in t10_Join.DefaultIfEmpty()
                                                  join t11 in _context.CrmSourceMedias on t1.SourceOfMediaId equals t11.SourceMediaId into t11_Join
                                                  where t1.CompanyId == model.CompanyId
                                      && (model.ResponsibleOfficerId > 0 ? t1.ResponsibleOfficerId == model.ResponsibleOfficerId : t1.ResponsibleOfficerId > 0)
                                      && (userId == 179 ? t2.EmployeeId > 0 : teamInfo.IsLeader == false ? t2.EmployeeId == teamInfo.EmployeeId : teamMambers.Contains(t2.EmployeeId))
                                                      //|| t2.EmployeeId == userId)

                                                      //userId == 0 &&



                                                      && (model.GenderId == 0 || t1.GenderId == model.GenderId)
                                                      && (model.ProjectId == 0 || t1.ProjectId == model.ProjectId)
                                                      && (model.StatusId == 0 || t1.ServiceStatusId == model.StatusId)
                                                      && (model.TypeofInterestId == 0 || t1.TypeOfInterestId == model.TypeofInterestId)
                                                      && (model.SourceofMediaId == 0 || t1.SourceOfMediaId == model.SourceofMediaId)
                                                      && (model.PromotionalOfferId == 0 || t1.PromotionalOfferId == model.PromotionalOfferId)
                                                      && (model.ChoiceAreaId == 0 || t1.ChoieceAreaId == model.ChoiceAreaId)
                                                      && (model.ReligionId == 0 || t1.ReligionId == model.ReligionId)
                                                  select new ClientVm
                                                  {
                                                      ClientId = t1.ClientId,
                                                      Name = t1.FullName,
                                                      GenderName = t7.Name,
                                                      ReligionText = t8.Name,
                                                      MobileNo = t1.MobileNo,
                                                      CompanyId = t1.CompanyId ?? 0,
                                                      MobileNo2 = t1.MobileNo2,
                                                      Email1 = t1.Email,
                                                      Email2 = t1.Email1,
                                                      DateofBirth = t1.DateofBirth,
                                                      JobTitle = t1.Designation,
                                                      OrganizationText = t1.DepartmentOrInstitution,
                                                      PresentAddress = t1.PresentAddress,
                                                      PermanentAddress = t1.PermanentAddress,
                                                      ResponsibleOfficeName = t3.Name,
                                                      ProjectText = t4.Name,
                                                      TypeofInterestText = t9.Name,
                                                      OfferText = t10.OfferName,
                                                      SourceofMediaText = t1.SourceOfMedia,
                                                      CampaignText = t1.CampaignName,
                                                      ChoiceAreaText = t6.ChoiceAreaName,
                                                      StatusId = t1.ServiceStatusId,
                                                      StatusText = t5.StatusName,
                                                      ReferredBy = t1.ReferredBy,
                                                      DateOfContact = t1.DateOfContact,
                                                      Createdate = t1.CreatedDate,
                                                      PromotionalOfferText = t10.OfferName



                                                  }).OrderByDescending(x => x.Createdate).ToListAsync());

            data.GenderId = model.GenderId;
            data.ReligionId = model.ReligionId;
            data.CompanyId = model.CompanyId;
            data.ChoiceAreaId = model.ChoiceAreaId;
            data.ResponsibleOfficerId = model.ResponsibleOfficerId;
            data.SourceofMediaId = model.SourceofMediaId;
            data.StatusId = model.StatusId;
            data.TypeofInterestId = model.TypeofInterestId;
            data.ProjectId = model.ProjectId;
            data.OfferId = model.OfferId;
            data.PromotionalOfferId = model.PromotionalOfferId;
            data.ChoiceAreaId = model.ChoiceAreaId;


            return data;
        }



        public PermissionModelListVm GetPermissionHandle(int userId, int companyId)
        {
            var permissionList = new PermissionModelListVm();
            var parents = typeof(PermissionCollection)
                .GetNestedTypes()
                .Where(t => t.IsSealed)
                .ToArray();

            foreach (var parent in parents)
            {
                permissionList.PermissionList.Add(BuildPermission(parent, null));
            }
            var userPermissionList = _context.UserPermissions
                .Where(q => q.CompanyId == companyId && q.UserId == userId).ToList();

            foreach (var p in permissionList.PermissionList)
            {
                foreach (var c in p.Childs.ToList())
                {
                    foreach (var cd in c.Childs)
                    {
                        if (userPermissionList.SingleOrDefault(s => s.PermissionNo == cd.ID) == null)
                        {
                            cd.IsChecked = false;
                            c.IsChecked = false;
                        }
                        else
                        {
                            cd.IsChecked = true;
                        }
                    }
                }
            }
            permissionList.CompanyId = companyId;
            permissionList.UserId = userId;
            return permissionList;
        }

        private PermissionModel BuildPermission(Type type, int? parentId)
        {
            var parent = new PermissionModel();

            int id = Convert.ToInt32((type.GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute).Description);

            parent.ID = id;
            parent.Name = type.Name;
            parent.ParentID = parentId;

            parent.IsChecked = false;
            parent.IsExpanded = false;

            parent.Childs = new List<PermissionModel>();

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var child = new PermissionModel
                {
                    ID = Convert.ToInt32(field.GetValue(field)),
                    Name = field.Name,
                    ParentID = parent.ID
                };
                parent.Childs.Add(child);
            }

            var nestedChilds = type.GetNestedTypes().ToArray();

            foreach (var child in nestedChilds)
            {
                parent.Childs.Add(BuildPermission(child, parent.ID));
            }
            return parent;
        }


        public async Task<CrmScheduleListVm> UpdateClientById(CrmScheduleListVm model)
        {
            var data = new CrmScheduleListVm();
            try
            {
                var objToSave = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == model.ClientData.ClientId);
                if (objToSave == null)
                {
                    objToSave = new KGRECustomer()
                    {

                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        DateOfContact = model.ClientData.DateOfContact
                    };
                    _context.KGRECustomers.Add(objToSave);
                }
                else
                {
                    objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    objToSave.ModifiedDate = DateTime.Now;

                }
                objToSave.FullName = model.ClientData.Name;
                objToSave.GenderId = model.ClientData.GenderId;
                objToSave.ReligionId = model.ClientData.ReligionId;
                objToSave.DateofBirth = model.ClientData.DateofBirth;
                objToSave.MobileNo2 = model.ClientData.MobileNo2;
                objToSave.Email = model.ClientData.Email;
                objToSave.Email1 = model.ClientData.Email2;
                objToSave.PresentAddress = model.ClientData.PresentAddress;
                objToSave.PermanentAddress = model.ClientData.PermanentAddress;
                objToSave.ResponsibleOfficerId = model.ClientData.ResponsibleOfficerId;
                objToSave.ProjectId = model.ClientData.ProjectId;
                objToSave.TypeOfInterestId = model.ClientData.TypeofInterestId;
                objToSave.SourceOfMediaId = model.ClientData.SourceofMediaId;
                objToSave.PromotionalOfferId = model.ClientData.OfferId;
                objToSave.ChoieceAreaId = model.ClientData.ChoiceAreaId;
                objToSave.CampaignName = model.ClientData.CampaignText;
                objToSave.ReferredBy = model.ClientData.ReferredBy;
                objToSave.UploadDateTime = DateTime.Now;

                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                throw ex;

            }
            return data;
        }

        public bool IsLeader(int userId, int companyId)
        {
            var user = _context.TeamInfoes.SingleOrDefault(s => s.IsActive && s.IsLeader && s.CompanyId == companyId && s.EmployeeId == userId);
            if (user != null)
            {
                return true;
            }
            return false;
        }
        public bool Manager(int userId, int companyId)
        {
            var user = _context.TeamInfoes.SingleOrDefault(s => s.EmployeeId == userId && s.ManagerId == 1 && s.CompanyId == companyId);
            if (user != null)
            {
                return true;
            }
            return false;
        }



        public async Task<ClientStatusListVm> GetClentStatus(int StatusId, int userId, int companyId)
        {
            int uId = 0;


            var userDetail = _context.TeamInfoes.SingleOrDefault(q => q.EmployeeId == userId && q.IsActive && q.CompanyId == companyId && !q.IsLeader && q.Name != "Existing Team");

            if (userDetail == null)
            {
                uId = 0;
            }
            else
            {
                uId = userDetail.Id;
            }

            var model = new ClientStatusListVm();
            model.DataList = await Task.Run(() => (from t1 in _context.KGRECustomers.Where(e => e.CompanyId == companyId)
                                                   join t2 in _context.TeamInfoes.Where(r => r.CompanyId == companyId)
                                                   on t1.ResponsibleOfficerId equals t2.Id into t2_Join
                                                   from t2 in t2_Join.DefaultIfEmpty()
                                                   join t3 in _context.ProductCategories.Where(q => q.CompanyId == companyId) on t1.ProjectId equals t3.ProductCategoryId into t3_Join
                                                   from t3 in t3_Join.DefaultIfEmpty()
                                                   join t4 in _context.CrmServiceStatus.Where(q => q.CompanyId == companyId) on t1.ServiceStatusId equals t4.StatusId into t4_Join
                                                   from t4 in t4_Join.DefaultIfEmpty()
                                                   where t1.ServiceStatusId == StatusId && ((userId == 179||userId==180) ? (t1.ClientId > 0) : ((uId == 0) || (t1.ResponsibleOfficerId == uId)))
                                                   select new ClientStatusVm
                                                   {   
                                                       Name = t1.FullName,
                                                       MobileNo = t1.MobileNo,
                                                       ClientId = t1.ClientId,
                                                       ProjectText = t3.Name,
                                                       StatusText = t4.StatusName,
                                                       Remarks = t1.Remarks,
                                                       JobTitle = t1.Designation,
                                                       OrganizationText = t1.DepartmentOrInstitution,
                                                      CreatedDate=t1.CreatedDate,
                                                      CompanyId=companyId,
                                                      
                                                       ActionLink = "/Crms/GetClientDetailsById?clientId=" + t1.ClientId + "&companyId=" + t1.CompanyId

                                                   }).OrderByDescending(c=>c.CreatedDate).ToListAsync());
            return model;
        }


        public async Task<List<ServiceStatusHistVm>> GetServiceStatusHistories(long clientId)
        {
            var model = new List<ServiceStatusHistVm>();
            model = await Task.Run(() => (from t1 in _context.KGREHistories.Where(q => q.KGREId == clientId).OrderByDescending(o => o.KGREId)
                                          select new ServiceStatusHistVm
                                          {
                                              ClientId = t1.KGREId,
                                              HistoryText = t1.ChangeHistory,

                                          }).ToListAsync());

            return model;
        }

        public async Task<List<ServiceStatusHistVm>> GetNoteList(long clientId)
        {
            var model = new List<ServiceStatusHistVm>();
            model = await Task.Run(() => (from t1 in _context.KGREHistories.Where(q => q.KGREId == clientId)
                                          join t2 in _context.KGRECustomers on t1.KGREId equals t2.ClientId
                                          where t1.IsActive

                                          select new ServiceStatusHistVm
                                          {
                                              ClientId = t1.KGREId,
                                              Notes = t1.Note,
                                              name = t2.FullName,
                                              ModiifyDate = t1.CreatedDate,


                                          }).OrderByDescending(o => o.ModiifyDate).ToListAsync());

            return model;
        }









        public async Task<object> GetAutoCompleteEmployee(string prefix)
        {

            var v = await Task.Run(() => (from t1 in _context.Employees.Where(q => q.Active)
                                          join t2 in _context.Designations on t1.DesignationId equals t2.DesignationId
                                          where (t1.EmployeeId.Contains(prefix) || t1.Name.Contains(prefix) || t1.ShortName.Contains(prefix) || t2.Name.Contains(prefix))

                                          select new
                                          {
                                              label = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                                              val = t1.Id
                                          }).OrderBy(x => x.label).Take(100).ToList());

            return v;
        }

        public async Task<PermissionModel> SavePermission(int id, bool ststus, int companyId, int userId)
        {

            var model = new PermissionModel();
            if (id == 0 || userId == 0 || companyId == 0)
            {
                model.ID = 0;
                model.Name = "User or company is not select properly!";
                return model;
            }
            var obj = await _context.UserPermissions.SingleOrDefaultAsync(q => q.PermissionNo == id
            && q.CompanyId == companyId
            && q.UserId == userId);

            if (obj == null)
            {
                obj = new UserPermission();
                obj.PermissionNo = id;
                obj.PermissionText = "";
                obj.UserId = userId;
                obj.CompanyId = companyId;
                obj.CreateDate = DateTime.Now;
                obj.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                _context.UserPermissions.Add(obj);
                obj.IsActive = true;

            }
            else if (obj.IsActive)
            {
                obj.IsActive = false;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            await _context.SaveChangesAsync();

            model.ID = obj.PermissionNo;
            model.IsChecked = ststus;

            return model;
        }
        public async Task<List<CrmVm>> UploadClientBatchh(ExcelWorksheet worksheet, int companyId, string fileName)
        {
            var responseList = new List<CrmVm>();
            var crmUploadHistoryList = new List<CrmUploadHistory>();

            var responsibleOfficerList = await GetDropdownDealingOfficer(companyId);
            var typeOfInterestList = await GetDropdownTypeofInterest();
            var sourceOfMediaList = await GetDropdownSourceofMedia(companyId);
            var promotionalOfferList = await GetDropdownPromotionalOffer(companyId);
            var statusList = await GetDropdownServiceStatus(companyId);
            var choiceOfAreaList = await GetDropdownChoiceofArea(companyId);
            var genderList = await GetDropdownGender();
            var religionList = await GetDropdownReligion();
            var projectList = await GetDropdownProject(companyId);

            int maxUploadSerialNo = _context.CrmUploadHistories.Select(s => s.UploadSerialNo).DefaultIfEmpty(0).Max() + 1;

            var existList = await _context.KGRECustomers.Where(q => q.CompanyId == companyId).ToListAsync();

            int rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var crmUploadHistoryTable = new CrmUploadHistory();

                    bool isEmptyRow = true;
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()))
                        {
                            isEmptyRow = false;
                            break;
                        }
                    }

                    if (isEmptyRow)
                    {
                        break;
                    }

                    var exist = new List<KGRECustomer>();
                    string mobileNo = "0" + (worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty);
                    //string mobileNo2 = !string.IsNullOrEmpty(worksheet.Cells[row, 10].Value?.ToString()) ? "0" + worksheet.Cells[row, 9].Value.ToString() : string.Empty;
                    string mobileNo2 = !string.IsNullOrEmpty(worksheet.Cells[row, 10].Value?.ToString())? worksheet.Cells[row, 10].Value.ToString() : string.Empty;

                    if (!string.IsNullOrEmpty(mobileNo) && !string.IsNullOrEmpty(mobileNo2))
                    {
                        exist = existList.Where(q =>
                               ((q.MobileNo == mobileNo || q.MobileNo2 == mobileNo)
                               || (q.MobileNo == mobileNo2 || q.MobileNo2 == mobileNo2))
                               && q.CompanyId == companyId
                               ).ToList();
                    }
                    else if (!string.IsNullOrEmpty(mobileNo) && string.IsNullOrEmpty(mobileNo2))
                    {
                        exist = existList.Where(q =>
                               (q.MobileNo == mobileNo || q.MobileNo2 == mobileNo)
                               && q.CompanyId == companyId
                               ).ToList();
                    }
                    else if (string.IsNullOrEmpty(mobileNo) && !string.IsNullOrEmpty(mobileNo2))
                    {
                        exist = existList.Where(q =>
                               (q.MobileNo == mobileNo2 || q.MobileNo2 == mobileNo2)
                               && q.CompanyId == companyId
                               ).ToList();
                    }

                    crmUploadHistoryTable.FinalStatus = exist.Count > 0 ? "Error" : "Success";

                    crmUploadHistoryTable.CompanyId = companyId;
                    crmUploadHistoryTable.FileNo = fileName;
                    crmUploadHistoryTable.CampaignName = worksheet.Cells[row, 1].Value?.ToString();

                    crmUploadHistoryTable.ResponsibleOfficer = worksheet.Cells[row, 2].Value?.ToString();
                    var responsibleOfficer = responsibleOfficerList.FirstOrDefault(x => x.Name.Trim() == worksheet.Cells[row, 2].Value?.ToString().Trim());
                    crmUploadHistoryTable.ResponsibleOfficerId = responsibleOfficer?.Id ?? 0;

                    crmUploadHistoryTable.FullName = worksheet.Cells[row, 3].Value?.ToString();
                    crmUploadHistoryTable.MobileNo = mobileNo;
                    crmUploadHistoryTable.Email = worksheet.Cells[row, 5].Value?.ToString();
                    crmUploadHistoryTable.Designation = worksheet.Cells[row, 6].Value?.ToString();
                    crmUploadHistoryTable.DepartmentOrInstitution = worksheet.Cells[row, 7].Value?.ToString();
                    crmUploadHistoryTable.DateofBirth = !string.IsNullOrEmpty(worksheet.Cells[row, 8].Value?.ToString()) ? Convert.ToDateTime(worksheet.Cells[row, 8].Value.ToString()) : new DateTime(1900, 1, 1);

                    crmUploadHistoryTable.Gender = worksheet.Cells[row, 9].Value?.ToString();
                    var gender = genderList.FirstOrDefault(q => q.Name.ToUpper() == worksheet.Cells[row, 9].Value?.ToString().ToUpper());
                    crmUploadHistoryTable.GenderId = gender?.Id ?? 1;

                    crmUploadHistoryTable.MobileNo2 = mobileNo2;
                    crmUploadHistoryTable.Email1 = worksheet.Cells[row, 11].Value?.ToString();

                    crmUploadHistoryTable.SourceOfMedia = worksheet.Cells[row, 12].Value?.ToString();
                    var sourceOfMedia = sourceOfMediaList.FirstOrDefault(q => q.Name.ToUpper() == worksheet.Cells[row, 12].Value?.ToString().ToUpper());
                    crmUploadHistoryTable.SourceOfMediaId = sourceOfMedia?.Id ?? 0;

                    crmUploadHistoryTable.ProjectName = worksheet.Cells[row, 13].Value?.ToString();
                    var project = projectList.FirstOrDefault(q => q.Name.ToUpper() == worksheet.Cells[row, 13].Value?.ToString().Trim().ToUpper());
                    crmUploadHistoryTable.ProjectId = project?.Id ?? 0;

                    crmUploadHistoryTable.TypeOfInterest = worksheet.Cells[row, 14].Value?.ToString();
                    var typeOfInterest = typeOfInterestList.FirstOrDefault(q => q.Name == worksheet.Cells[row, 14].Value?.ToString());
                    crmUploadHistoryTable.TypeOfInterestId = typeOfInterest?.Id ?? 0;

                    crmUploadHistoryTable.PromotionalOffer = worksheet.Cells[row, 15].Value?.ToString();
                    var promotionalOffer = promotionalOfferList.FirstOrDefault(q => q.Name.ToUpper() == worksheet.Cells[row, 15].Value?.ToString().ToUpper());
                    crmUploadHistoryTable.PromotionalOfferId = promotionalOffer?.Id ?? 0;

                    crmUploadHistoryTable.StatusLevel = !string.IsNullOrEmpty(worksheet.Cells[row, 16].Value?.ToString()) ? worksheet.Cells[row, 16].Value.ToString() : "New";
                    var status = statusList.FirstOrDefault(q => q.Name.ToUpper() == worksheet.Cells[row, 16].Value?.ToString().ToUpper());
                    crmUploadHistoryTable.ServiceStatusId = status?.Id ?? 0;

                    crmUploadHistoryTable.PresentAddress = worksheet.Cells[row, 18].Value?.ToString();
                    crmUploadHistoryTable.PermanentAddress = worksheet.Cells[row, 19].Value?.ToString();

                    crmUploadHistoryTable.Remarks = worksheet.Cells[row, 21].Value?.ToString();
                    crmUploadHistoryTable.ReferredBy = worksheet.Cells[row, 22].Value?.ToString();

                    crmUploadHistoryTable.UploadSerialNo = maxUploadSerialNo;
                    crmUploadHistoryTable.UploadDateTime = DateTime.Now.Date;
                    crmUploadHistoryTable.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    crmUploadHistoryTable.CreatedDate = DateTime.Now;

                    crmUploadHistoryList.Add(crmUploadHistoryTable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row {row}: {ex.Message}");
                    throw;
                }
            }

            try
            {
                if (crmUploadHistoryList.Count > 0)
                {
                    _context.CrmUploadHistories.AddRange(crmUploadHistoryList);
                    if (_context.SaveChanges()>0)
                    {
                        var fResult = true;
                    }
                }
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"ValidationException: {ex.Message}");
                throw;
            }

            return GetUploadClientHistory(maxUploadSerialNo, companyId, DateTime.Now);
        }


        private List<CrmVm> GetUploadClientHistory(int uploadSerialNo, int companyId, DateTime uploadDateTime)
        {
            var uploadDate = uploadDateTime.Date;
            var dataList = (from t1 in _context.CrmUploadHistories
                            where t1.UploadSerialNo == uploadSerialNo
                            && t1.UploadDateTime == uploadDate
                            && t1.CompanyId == companyId
                            select new CrmVm
                            {
                                Name = t1.FullName,
                                MobileNo = t1.MobileNo,
                                ResponseStatus = t1.FinalStatus,
                                MobileNo2 = t1.MobileNo2,
                                ProjectText = t1.ProjectName,
                                GenderText = t1.Gender,
                                JobTitle = t1.Designation,
                                ResponsibleOfficeName = t1.ResponsibleOfficer,
                                OrganizationText = t1.DepartmentOrInstitution
                            }).ToList();

            return dataList;
        }


        public async Task<List<CrmVm>> UploadClientBatch(List<CrmVm> modelList, int companyId, string fileName)
        {
            var ResponseList = new List<CrmVm>();
            var crmObjHist = new List<CrmUploadHistory>();


            int maxUploadSerialNo = _context.CrmUploadHistories.Select(s => s.UploadSerialNo).DefaultIfEmpty(0).Max();

            maxUploadSerialNo += 1;

            var existList = await _context.KGRECustomers.Where(q => q.CompanyId == companyId).ToListAsync();



            foreach (var v in modelList)
            {
                var crm = new CrmVm();
                var objHist = new CrmUploadHistory();

                var exist = new List<KGRECustomer>();

                if (!string.IsNullOrEmpty(v.MobileNo) && !string.IsNullOrEmpty(v.MobileNo2))
                {
                    exist = existList.Where(q =>
                           ((q.MobileNo == v.MobileNo || q.MobileNo2 == v.MobileNo)
                           || (q.MobileNo == v.MobileNo2 || q.MobileNo2 == v.MobileNo2))
                           && q.CompanyId == v.CompanyId
                           ).ToList();
                }
                else if (!string.IsNullOrEmpty(v.MobileNo) && string.IsNullOrEmpty(v.MobileNo2))
                {
                    exist = existList.Where(q =>
                           (q.MobileNo == v.MobileNo || q.MobileNo2 == v.MobileNo)
                           && q.CompanyId == v.CompanyId
                           ).ToList();
                }
                else if (string.IsNullOrEmpty(v.MobileNo) && !string.IsNullOrEmpty(v.MobileNo2))
                {
                    exist = existList.Where(q =>
                           (q.MobileNo == v.MobileNo2 || q.MobileNo2 == v.MobileNo2)
                           && q.CompanyId == v.CompanyId
                           ).ToList();
                }

                crm = v;


                if (exist.Count > 0)
                {
                    objHist.FinalStatus = "Error";
                    crm.ResponseStatus = "Error";
                }
                else
                {
                    objHist.FinalStatus = "Success";
                    crm.ResponseStatus = "Success";
                }

                objHist.FileNo = fileName;
                objHist.FullName = v.Name;
                objHist.MobileNo = v.MobileNo;
                objHist.MobileNo2 = v.MobileNo2;
                objHist.DateofBirth = v.DateofBirth;
                objHist.CompanyId = v.CompanyId;
                objHist.DateOfContact = v.DateOfContact;
                objHist.CampaignName = v.CampaignText;
                objHist.ResponsibleOfficer = v.ResponsibleOfficeName;
                objHist.ResponsibleOfficerId = v.ResponsibleOfficerId;
                objHist.PresentAddress = v.PresentAddress;
                objHist.PermanentAddress = v.PermanentAddress;
                objHist.Designation = v.JobTitle;
                objHist.Email = v.Email;
                objHist.Email1 = v.Email2;
                objHist.SourceOfMedia = v.SourceofMediaText;
                objHist.SourceOfMediaId = v.SourceofMediaId;
                objHist.PromotionalOffer = v.OfferText;
                objHist.PromotionalOfferId = v.OfferId;
                objHist.Gender = v.GenderText;
                objHist.GenderId = v.GenderId;
                objHist.TypeOfInterest = v.TypeofInterestText;
                objHist.TypeOfInterestId = v.TypeofInterestId;
                objHist.ServiceStatusId = v.StatusId;
                objHist.StatusLevel = v.StatusText;
                objHist.Project = v.ProjectText;
                objHist.ProjectName = v.ProjectText;
                objHist.ProjectId = v.ProjectId;
                objHist.Remarks = v.Remarks;
                objHist.UploadSerialNo = maxUploadSerialNo;
                objHist.UploadDateTime = DateTime.Now.Date;
                objHist.ChoieceAreaId = v.ChoiceAreaId;
                objHist.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                objHist.CreatedDate = DateTime.Now;
                objHist.DepartmentOrInstitution = v.OrganizationText;
                crmObjHist.Add(objHist);
            }
            if (crmObjHist.Count > 0)
            {
                _context.CrmUploadHistories.AddRange(crmObjHist);
                await _context.SaveChangesAsync();
            }

            return ResponseList = GetUploadClientHistory(maxUploadSerialNo, companyId, DateTime.Now);
        }

        //private List<CrmVm> GetUploadClientHistory(int uploadSerialNo, int companyId, DateTime uploadDateTime)
        //{
        //    var uploadDate = uploadDateTime.Date;
        //    var dataList = new List<CrmVm>();

        //    dataList = (from t1 in _context.CrmUploadHistories

        //                where t1.UploadSerialNo == uploadSerialNo
        //                && t1.UploadDateTime == uploadDate
        //                && t1.CompanyId == companyId
        //                select new CrmVm
        //                {

        //                    Name = t1.FullName,
        //                    MobileNo = t1.MobileNo,
        //                    ResponseStatus = t1.FinalStatus,
        //                    MobileNo2 = t1.MobileNo2,
        //                    ProjectText = t1.ProjectName,
        //                    GenderText = t1.Gender,
        //                    JobTitle = t1.Designation,
        //                    OrganizationText = t1.DepartmentOrInstitution

        //                }
        //                               ).ToList();
        //    //foreach (var item in dataList)
        //    //{
        //    //  vm.crmVms2 = _context.KGRECustomers.Where(f => f.CompanyId == companyId&&f.MobileNo==item.MobileNo).ToList();

        //    //};



        //    //var obj= (from t1 in _context.KGRECustomers
        //    //          join t2 in dataList on t1.MobileNo equals t2.MobileNo
        //    //          where  t1.CompanyId == companyId
        //    //          select new CrmVm
        //    //          {

        //    //              ClientId=t1.ClientId,

        //    //          }
        //    //                           ).ToList();


        //    return dataList;
        //}

        public CrmUploadVm SyncClientBatch(CrmUploadVm model)
        {
            if (model == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something worng !!!");
                return model;
            }
            try
            {
                var clientList = _context.Database.ExecuteSqlCommand(@"EXEC CrmClientSync {0},{1},{2}", model.CompanyId, model.LastUploadNo, model.UploadDateTime.Date);

                model.HasMessage = true;
                model.MessageList.Add($"Seccessfully Sync ( {clientList / 2} )");



            }
            catch (Exception ex)
            {
                throw ex;

            }

            return model;

            //if (clientList.Count <= 0)
            //{
            //    model.HasMessage = true;
            //    model.MessageList.Add("Not Found To Sync?");
            //    return model;
            //}
            //else
            //{
            //    model.HasMessage = true;
            //    model.MessageList.Add("Successfully updated");
            //    return model;

            //}
        }

        //public async Task<ClientBatchUplodListVm> GetCrmClientBatchUpload(int companyId)
        //{
        //    var vm = new ClientBatchUplodListVm();

        //    vm.DataList = await Task.Run(() => (from t1 in _context.CrmUploadHistories.Where(f=>f.CompanyId==companyId)
        //                                        join t2 in _context.Employees on t1.CreatedBy equals t2.EmployeeId into e
        //                                        from j in e.DefaultIfEmpty()
        //                                        group new { t1,j} by new { t1.UploadSerialNo, t1.FinalStatus }
        //                                                       into g

        //                                        select new ClientBatchUplodVm
        //                                        {
        //                                            UploadSerialNo = g.Key.UploadSerialNo,
        //                                            ErrorCount = g.Where(q => q.t1.FinalStatus == "Error").Count(),
        //                                            SuccessCount = g.Where(q => q.t1.FinalStatus == "Success").Count(),
        //                                            IsSyncCount = g.Where(q => q.t1.IsSync == true).Count(),
        //                                            UploadDateTime = g.FirstOrDefault().t1.UploadDateTime,
        //                                            CreatedBy=g.FirstOrDefault().t1.CreatedBy,
        //                                            UploadName = g.FirstOrDefault().j.Name,
        //                                            FileName=g.FirstOrDefault().t1.FileNo == null ? " No File Name" : g.FirstOrDefault().t1.FileNo,
        //                                        }).OrderByDescending(o => new { o.UploadDateTime, o.UploadSerialNo }).ToListAsync());
        //    vm.CompanyId = companyId;
        //    return vm;
        //}
        public async Task<ClientBatchUplodListVm> GetCrmClientBatchUpload(int companyId)
        {
            var vm = new ClientBatchUplodListVm();

            var query = @"
        SELECT 
            t1.UploadSerialNo,
            SUM(CASE WHEN t1.FinalStatus = 'Error' THEN 1 ELSE 0 END) AS ErrorCount,
            SUM(CASE WHEN t1.FinalStatus = 'Success' THEN 1 ELSE 0 END) AS SuccessCount,
            SUM(CASE WHEN t1.IsSync = 1 THEN 1 ELSE 0 END) AS IsSyncCount,
            t1.UploadDateTime,
            t1.CreatedBy,
            COALESCE(t2.Name, 'Unknown') AS UploadName,
            COALESCE(t1.FileNo, 'No File Name') AS FileName
        FROM dbo.CrmUploadHistory t1
        LEFT JOIN Employee t2 ON t1.CreatedBy = t2.EmployeeId
        WHERE t1.CompanyId = @CompanyId
        GROUP BY t1.UploadSerialNo, t1.FinalStatus, t1.UploadDateTime, t1.CreatedBy, t2.Name, t1.FileNo
        ORDER BY t1.UploadDateTime DESC, t1.UploadSerialNo DESC;";

            var parameters = new SqlParameter("@CompanyId", companyId);
            vm.DataList = await _context.Database.SqlQuery<ClientBatchUplodVm>(query, parameters).ToListAsync();
            vm.CompanyId = companyId;
            return vm;
        }


        public async Task<CrmVm> CopyClientSave(int clientId, int selectedCompanyId)
        {
            var model = new CrmVm();
            var obj = await _context.KGRECustomers
                .SingleOrDefaultAsync(q => q.ClientId == clientId);

            if (obj == null)
            {
                model.HasMessage = true;
                model.MessageList.Add("Something wrong");
                return model;
            }

            var existingObj = await _context.KGRECustomers
                .SingleOrDefaultAsync(q => q.MobileNo == obj.MobileNo
                && q.CompanyId == selectedCompanyId);

            if (existingObj == null)
            {
                var teamList = _context.TeamInfoes.ToList();
                var empId = teamList.SingleOrDefault(q => q.Id == obj.ResponsibleOfficerId).EmployeeId;


                var responsibleOfficerId = teamList.SingleOrDefault(q => q.EmployeeId == empId
                && q.CompanyId == selectedCompanyId).Id;

                var objectToSave = new KGRECustomer()
                {
                    FullName = obj.FullName,
                    CompanyId = selectedCompanyId,
                    MobileNo = obj.MobileNo,
                    ResponsibleOfficerId = responsibleOfficerId,

                };
                _context.KGRECustomers.Add(objectToSave);
                await _context.SaveChangesAsync();

            }

            return model;
        }


        public async Task<CrmVm> UpdateSectionClient(CrmVm model)
        {

            var data = new CrmVm();
            var objToSave = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == model.ClientId);

            if (model.SectionEdit == 1)
            {
                try
                {

                    objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    objToSave.ModifiedDate = DateTime.Now;
                    if (model.GenderId == 0)
                    {
                        model.GenderId = 1;
                    }
                    if (model.ReligionId == 0)
                    {
                        model.ReligionId = 23;
                    }
                    objToSave.FullName = model.Name;
                    objToSave.GenderId = model.GenderId;
                    objToSave.ReligionId = model.ReligionId;
                    objToSave.DateofBirth = model.DateofBirth;
                    objToSave.MobileNo = model.MobileNo;
                    objToSave.MobileNo2 = model.MobileNo2;
                    objToSave.Email = model.Email;
                    objToSave.Email1 = model.Email2;
                    objToSave.DepartmentOrInstitution = model.Organization;
                    objToSave.Designation = model.JobTitle;


                    objToSave.ReferredBy = model.ReferredBy;
                    objToSave.UploadDateTime = DateTime.Now;
                    objToSave.SourceOfMediaId = model.SourceofMediaId;
                    objToSave.Remarks = model.Remarks;
                    await _context.SaveChangesAsync();
                    data.CompanyId = objToSave.CompanyId ?? 0;
                    data.ClientId = objToSave.ClientId;

                }
                catch (Exception ex)
                {
                    throw ex;

                }

            }
            else if (model.SectionEdit == 2)
            {
                try
                {
                    objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    objToSave.ModifiedDate = DateTime.Now;
                    objToSave.SourceOfMediaId = model.SourceofMediaId;
                    objToSave.CampaignName = model.CampaignText;
                    objToSave.PromotionalOfferId = model.OfferId;
                    objToSave.ChoieceAreaId = model.ChoiceAreaId;
                    await _context.SaveChangesAsync();
                    data.CompanyId = objToSave.CompanyId ?? 0;
                    data.ClientId = objToSave.ClientId;


                }
                catch (Exception ex)
                {
                    throw ex;
                }




            }
            else
            {
                try
                {
                    objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    objToSave.ModifiedDate = DateTime.Now;

                    //  objToSave.ResponsibleOfficerId = model.ResponsibleOfficerId;                  
                    objToSave.TypeOfInterestId = model.TypeofInterestId;
                    objToSave.ProjectId = model.ProjectId;
                    objToSave.ResponsibleOfficerId = model.ResponsibleOfficerId;
                    await _context.SaveChangesAsync();
                    data.CompanyId = objToSave.CompanyId ?? 0;
                    data.ClientId = objToSave.ClientId;



                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }





            return data;
        }













        public async Task<CrmSchedule> SaveTask(int scheduleId, int SelectedCompanyId)
        {


            var model = new CrmSchedule();
            var obj = await _context.CrmSchedules.SingleOrDefaultAsync(x => x.ScheduleId == scheduleId && x.CompanyId == SelectedCompanyId);


            if (obj != null)
            {
                try
                {
                    if (obj.IsCompleted == true)
                    {
                        obj.IsCompleted = false;
                    }
                    else
                    {
                        obj.IsCompleted = true;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var msg = ex;
                }


            }
            model.ScheduleId = scheduleId;
            model.CompanyId = SelectedCompanyId;
            model.IsCompleted = obj.IsCompleted;
            return model;
        }


        public async Task<CrmListVm> FilteringClientUploadBatchList(int companyId, int uploadSerialNo, DateTime uploadDateTime, int userId)
        {
            var vm = new CrmListVm();
            vm.DataList = await Task.Run(() => (from t1 in _context.CrmUploadHistories
                                                where t1.CompanyId == companyId
                                                && t1.UploadSerialNo == uploadSerialNo
                                                && t1.UploadDateTime == uploadDateTime
                                                select new CrmVm
                                                {
                                                    Name = t1.FullName,
                                                    MobileNo = t1.MobileNo,
                                                    MobileNo2 = t1.MobileNo2,
                                                    DateofBirth = t1.DateofBirth,
                                                    GenderText = t1.Gender,
                                                    CampaignText = t1.CampaignName,
                                                    ResponsibleOfficeName = t1.ResponsibleOfficer,
                                                    PermanentAddress = t1.PermanentAddress,
                                                    PresentAddress = t1.PresentAddress,
                                                    JobTitle = t1.Designation,
                                                    OrganizationText = t1.DepartmentOrInstitution,
                                                    Email = t1.Email,
                                                    Email2 = t1.Email1,
                                                    SourceofMediaText = t1.SourceOfMedia,
                                                    OfferText = t1.PromotionalOffer,
                                                    ProjectText = t1.ProjectName,
                                                    TypeofInterestText = t1.TypeOfInterest,
                                                    StatusText = t1.StatusLevel,
                                                    ReferredBy = t1.ReferredBy,
                                                    Remarks = t1.Remarks
                                                }
                                                ).ToListAsync());
            return vm;

        }

        public async Task<CrmScheduleListVm> GetPendingScheduleList(int companyId, int uId)
        {
            var dataList = new CrmScheduleListVm();
            var today = DateTime.Now.Date;
            var userDetail = _context.TeamInfoes.SingleOrDefault(q => q.EmployeeId == uId
            && q.IsActive && q.CompanyId == companyId && !q.IsLeader && q.Name!= "Existing Team");

            if (userDetail == null)
            {
                uId = 0;
            }
            else
            {
                uId = userDetail.Id;
            }

            dataList.DataList = await Task.Run(() => (from t1 in _context.CrmSchedules
                                                      join t2 in _context.KGRECustomers on t1.ClientId equals t2.ClientId

                                                      where t1.IsActive && t1.ScheduleDate < today
                                                      && t1.IsCompleted == false

                                                      && (uId == 0 || t2.ResponsibleOfficerId == uId)

                                                      && t1.CompanyId == companyId
                                                      select new CrmScheduleVm
                                                      {
                                                          ClientId = t1.ClientId,
                                                          ClientName = t2.FullName,
                                                          ScheduleTime = t1.ScheduleTime,
                                                          ScheduleDate = t1.ScheduleDate,
                                                          Note = t1.Note,
                                                          ScheduleType = t1.ScheduleType,
                                                          ScheduleId = t1.ScheduleId,
                                                          ClientMobileNo = t2.MobileNo == null ? t2.MobileNo2 : t2.MobileNo,
                                                          IsComplete = t1.IsCompleted,
                                                          CompanyId = t2.CompanyId ?? 0,
                                                          ResponsibleOfficeName=t2.ResponsibleOfficer
                                                      }).OrderByDescending(o => new { o.ScheduleDate, o.ScheduleTime }).ToListAsync());

            dataList.CompanyId = companyId;
            return dataList;
        }
        public async Task<CrmScheduleListVm> GetCompletedScheduleList(int companyId, int uId)
        {
            var dataList = new CrmScheduleListVm();
            var today = DateTime.Now.Date;
            var userDetail = _context.TeamInfoes.SingleOrDefault(q => q.EmployeeId == uId
            && q.IsActive && q.CompanyId == companyId && !q.IsLeader);

            if (userDetail == null)
            {
                uId = 0;
            }
            else
            {
                uId = userDetail.Id;
            }

            dataList.DataList = await Task.Run(() => (from t1 in _context.CrmSchedules
                                                      join t2 in _context.KGRECustomers on t1.ClientId equals t2.ClientId

                                                      where t1.IsActive
                                                      && t1.IsCompleted == true

                                                      && (uId == 0 || t2.ResponsibleOfficerId == uId)

                                                      && t1.CompanyId == companyId
                                                      select new CrmScheduleVm
                                                      {
                                                          ClientId = t1.ClientId,
                                                          ClientName = t2.FullName,
                                                          ScheduleTime = t1.ScheduleTime,
                                                          ScheduleDate = t1.ScheduleDate,
                                                          Note = t1.Note,
                                                          ScheduleType = t1.ScheduleType,
                                                          ScheduleId = t1.ScheduleId,
                                                          ClientMobileNo = t2.MobileNo == null ? t2.MobileNo2 : t2.MobileNo,
                                                          IsComplete = t1.IsCompleted,
                                                          CompanyId = t2.CompanyId ?? 0
                                                      }).OrderBy(o => new { o.ScheduleDate, o.ScheduleTime }).ToListAsync());

            dataList.CompanyId = companyId;
            return dataList;
        }

        public async Task<CrmVm> noteview(CrmVm model)
        {

            var data = new CrmVm();
            var client = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == model.ClientId);
            data.Remarks = client.Remarks;

            data.ScheduleDataList = await _context.CrmSchedules.Where(e => e.ClientId == model.ClientId && e.IsActive).ToListAsync();

            var serviceHistory = await _context.KGREHistories.OrderByDescending(x => x.CreatedDate).Where(y => y.KGREId == model.ClientId).FirstOrDefaultAsync();

            if (serviceHistory != null)
            {
                data.ServiceNote = serviceHistory.ChangeHistory;
            }

            return data;

        }


        public async Task<CrmVm> GetExistClientByID(string MobileNumber, int CompanyId)
        {
            CrmVm model = new CrmVm();
            model.Clientdata = new VMKGRECustomer();
            var client = await _context.KGRECustomers.FirstOrDefaultAsync(e => e.MobileNo == MobileNumber);
            model.Clientdata = await Task.Run(() => (from t1 in _context.KGRECustomers
                                                     join t2 in _context.CrmServiceStatus on t1.ServiceStatusId
                                                     equals t2.StatusId into x
                                                     from t2 in x.DefaultIfEmpty()
                                                     join t3 in _context.TeamInfoes on t1.ResponsibleOfficerId                          equals t3.Id into y
                                                     from t3 in y.DefaultIfEmpty()
                                                     join t4 in _context.Employees on t3.EmployeeId equals                              t4.Id into z
                                                     from t4 in z.DefaultIfEmpty()
                                                     join t5 in _context.ProductCategories on t1.ProjectId equals                                t5.ProductCategoryId into d
                                                     from t5 in d.DefaultIfEmpty()
                                                     where t1.ClientId == client.ClientId
                                                     select new VMKGRECustomer
                                                     {
                                                         ClientId = t1.ClientId,
                                                         FullName = t1.FullName,
                                                         MobileNo = t1.MobileNo,
                                                         ServiceStatusId = t1.ServiceStatusId,
                                                         ServiceStatusName = t2 == null ? "No                                               Select" : t2.StatusName,
                                                         ResponsibleOfficerId = t1.ResponsibleOfficerId,
                                                         ResponsibleOfficer = t4 == null ? "No Select" :                                    t4.Name,
                                                         CampaingName = t1.CampaignName,
                                                         projectId=t1.ProjectId,
                                                         ProjectName=t5.Name

                                                     }).FirstOrDefaultAsync());
            if(model.Clientdata.ProjectName == null)
            {
                model.Clientdata.ProjectName = "No project assigned yet.";
            }
            model.CompanyId = CompanyId;
            model.ServiceStatusList = await GetDropdownServiceStatus(CompanyId);
            model.DealingOfficerList = await GetDropdownDealingOfficer(CompanyId);
            model.ProjectList = await GetDropdownProject(CompanyId);
            return model;
        }

        public async Task<List<SelectVm>> AllResponsibleoffficerrr(int CompanyId)
        {

            var model = new List<SelectVm>();
            model = await Task.Run(() => (from t1 in _context.TeamInfoes
                                          join t2 in _context.Employees on t1.EmployeeId equals t2.Id
                                          where t1.CompanyId == CompanyId && t1.Name != "Existing Team"
                                          select new SelectVm
                                          {
                                              Id = t1.Id,
                                              Name = t2.Name
                                          }).OrderBy(o => o.Name).ToListAsync());


            return model;
        }
        public async Task<CrmViewModel> Countingupdate(CrmViewModel model)
        {

            if (model.FormDate == new DateTime(0001, 01, 01, 00, 00, 00))
            {
                var datetime = DateTime.Now;
                var twoMonthAgo = datetime.AddMonths(-2);
                model.FormDate = twoMonthAgo;
                model.startTodate = datetime;
            };


            var vm = new CrmViewModel();
            vm.TotalNumberofClient = await _context.KGRECustomers.Where(q => q.ResponsibleOfficerId== model.ResponsibleOfficerid && q.CreatedDate>=model.FormDate && q.CreatedDate<=model.startTodate).CountAsync();

            vm.ClientStatusDataList = await Task.Run(() => (from t1 in _context.KGRECustomers
                                                               join t2 in _context.CrmServiceStatus
                                                               on t1.ServiceStatusId equals t2.StatusId
            where t1.ResponsibleOfficerId==model.ResponsibleOfficerid && t1.CreatedDate
            >=model.FormDate && t1.CreatedDate<=model.startTodate
                                                               group t1 by new { t2.StatusName, t2.StatusId }
                                                               into g
                                                               select new ClientStatusList
                                                               {
                                                                   StatusId = g.Key.StatusId,
                                                                   StatusText = g.Key.StatusName,
                                                                   TotalNumberofClient = g.Count()

                                                               }).ToListAsync());



            return vm;
        }

      public async Task<CrmVm> ModifyUpload(CrmVm model)
        {
            if (model.StrFrmdate == new DateTime(0001, 01, 01, 00, 00, 00))
            {
                var datetime = DateTime.Now;
                var twoMonthAgo = datetime.AddMonths(-2);
                model.StrFrmdate = twoMonthAgo;
                model.Todate = datetime;
            };

            CrmVm vm = new CrmVm();
            



            vm.crmVms= await Task.Run(() => (from t1 in _context.KGRECustomers
                                             join t2 in _context.CrmServiceStatus
                                             on t1.ServiceStatusId equals t2.StatusId
                                             join t3 in _context.TeamInfoes
                                             on t1.ResponsibleOfficerId equals t3.Id
                                             join t4 in _context.Employees 
                                             on t3.EmployeeId equals t4.Id

                                             where t1.ResponsibleOfficerId == model.ResponsibleOfficerId &&
                                             //t1.ServiceStatusId==model.StatusId
                                             //(model.StatusId > 0 ? t1.ServiceStatusId == model.StatusId : t1.ServiceStatusId > 0)
                                                 (model.StatusId == 0 || t1.ServiceStatusId == model.StatusId)
                                             && DbFunctions.TruncateTime(t1.ModifiedDate) >= model.StrFrmdate && DbFunctions.TruncateTime(t1.ModifiedDate) <= model.Todate && t1.CompanyId==model.CompanyId
                                             select new CrmVm
                                             {
                                                 Name=t1.FullName,
                                                CreatedDate=t1.CreatedDate,
                                                ModifyDate=t1.ModifiedDate,
                                                StatusText=t2.StatusName,
                                                ResponsibleOfficeName=t4.Name,
                                                ClientId=t1.ClientId,
                                                CompanyId= (int)t1.CompanyId
 
                                             }).ToListAsync());





            return vm;
        }






        public async Task<CrmVm> ReassignClient(CrmVm model)
        {
            var data = new CrmVm();
            var objToSave = await _context.KGRECustomers.SingleOrDefaultAsync(e => e.ClientId == model.ClientId);
            objToSave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            objToSave.ModifiedDate = DateTime.Now;

            //  objToSave.ResponsibleOfficerId = model.ResponsibleOfficerId;                  
            objToSave.ServiceStatusId = model.StatusId == 0 ? objToSave.ServiceStatusId : model.StatusId;
            objToSave.ResponsibleOfficerId = model.ResponsibleOfficerId == 0 ? objToSave.ResponsibleOfficerId : model.ResponsibleOfficerId;
            objToSave.ProjectId=model.ProjectId==0?objToSave.ProjectId:model.ProjectId;
            await _context.SaveChangesAsync();
            data.CompanyId = model.CompanyId;
            data.ClientId = model.ClientId;
            return data;
        }
     

        public async Task<VehicleVm> GetCarRquisitinList(int CompanyId,DateTime? Dateee)
        {
            VehicleVm vm = new VehicleVm();
            vm.DataList = await Task.Run(() => (from t1 in _context.VehicleRequisitions
                                                join t2 in _context.TeamInfoes
                                                on t1.ResponsibleOfficerId equals t2.Id
                                                join t0 in _context.Employees 
                                                on t2.EmployeeId equals t0.Id
                                                join t3 in _context.ProductCategories
                                                on t1.PrijectId equals t3.ProductCategoryId
                                                where t1.CompanyId == CompanyId && t1.IsActive 
                                                && (Dateee == null || DbFunctions.TruncateTime(t1.Date)==DbFunctions.TruncateTime(Dateee))
                                                select new VehicleVm
                                                {
                                                    VehicleRequisitionId=t1.VehicleRequisitionId,
                                                    CompanyId = CompanyId,
                                                    OfficerName=t0.Name,
                                                    DayType = (DayTypeEnum)t1.DayId,
                                                    Time=t1.Time,
                                                    Location=t1.Location,
                                                    TotalPersion=t1.TotalPersion,
                                                    ProjectName=t3.Name,
                                                    DriverName=t1.DriverName==null?"Not Assign Yet": t1.DriverName,
                                                    ResponsibleOfficerId=t1.ResponsibleOfficerId,
                                                    Date=t1.Date,
                                                    Reason=t1.Reason
                                                }).OrderBy(x => x.ResponsibleOfficerId).ThenByDescending(x => x.Time).AsEnumerable());
            vm.CompanyId = CompanyId;


            return vm;
        }
        public async Task<VehicleVm> GetCarRquisitinListIndividual(int CompanyId,int Uid)
        {
            VehicleVm vm = new VehicleVm();
            var userDetail = new TeamInfo();
            if (Uid == 180)
            {
                 userDetail = _context.TeamInfoes.SingleOrDefault(q => q.EmployeeId == Uid && q.IsActive && q.CompanyId == CompanyId );
            }
            else
            {
                 userDetail = _context.TeamInfoes.SingleOrDefault(q => q.EmployeeId == Uid && q.IsActive && q.CompanyId == CompanyId && q.Name != "Existing Team");


            }

            vm.DataList = await Task.Run(() => (from t1 in _context.VehicleRequisitions
                                                join t2 in _context.TeamInfoes
                                                on t1.ResponsibleOfficerId equals t2.Id
                                                join t0 in _context.Employees
                                                on t2.EmployeeId equals t0.Id
                                                join t3 in _context.ProductCategories
                                                on t1.PrijectId equals t3.ProductCategoryId
                                                where t1.CompanyId == CompanyId && t1.IsActive && t2.Id== userDetail.Id
                                                select new VehicleVm
                                                {
                                                    VehicleRequisitionId=t1.VehicleRequisitionId,
                                                    CompanyId = CompanyId,
                                                    OfficerName = t0.Name,
                                                    DayType = (DayTypeEnum)t1.DayId,
                                                    Time = t1.Time,
                                                    Date=t1.Date,
                                                    Location = t1.Location,
                                                    TotalPersion = t1.TotalPersion,
                                                    ProjectName = t3.Name,
                                                    DriverName = t1.DriverName == null ? "Not Assign Yet" : t1.DriverName,
                                                    ResponsibleOfficerId = t1.ResponsibleOfficerId,
                                                    Reason=t1.Reason
                                                }).OrderBy(x => x.ResponsibleOfficerId).ThenByDescending(x => x.Time).AsEnumerable());
            vm.CompanyId = CompanyId;


            return vm;
        }

        public async Task<VehicleVm> GetOfficerName(int CompanyId, int Uid)
        {
            VehicleVm vm = new VehicleVm();
            if (Uid == 180)
            {
                vm = await Task.Run(() => (from t1 in _context.Employees
                                           join t2 in _context.TeamInfoes on t1.Id equals t2.EmployeeId

                                           where Uid == t1.Id && t2.CompanyId == CompanyId 
                                           select new VehicleVm
                                           {
                                               OfficerName = t1.Name,
                                               ResponsibleOfficerId = t2.Id
                                           }).FirstOrDefaultAsync());
            }
            else
            {
                vm = await Task.Run(() => (from t1 in _context.Employees
                                           join t2 in _context.TeamInfoes on t1.Id equals t2.EmployeeId

                                           where Uid == t1.Id && t2.CompanyId == CompanyId && t2.Name != "Existing Team"
                                           select new VehicleVm
                                           {
                                               OfficerName = t1.Name,
                                               ResponsibleOfficerId = t2.Id
                                           }).FirstOrDefaultAsync());
            }

            
            vm.CompanyId = CompanyId;
            return vm;
        }

        public async Task<VehicleVm> CraRequisitinAdd(VehicleVm model)
        {
            VehicleVm vm = new VehicleVm();
            var obj = _context.VehicleRequisitions.SingleOrDefault(x => x.VehicleRequisitionId == model.VehicleRequisitionId);
            DateTime date = model.Date; 
            string dayOfMonth = date.ToString("dddd");
            model.Date = Convert.ToDateTime(model.Strdate);
            model.DayType = (DayTypeEnum)Enum.Parse(typeof(DayTypeEnum), dayOfMonth,true); 
          

            if (obj== null)
            {
                try
                {
                    VehicleRequisition vehicleRequisition = new VehicleRequisition
                    {
                        ResponsibleOfficerId = model.ResponsibleOfficerId,
                        Time = model.Time,
                        Location = model.Location,
                        TotalPersion = model.TotalPersion,
                        PrijectId = model.PrijectId,
                        Date = model.Date,
                        DayId = (int)model.DayType,
                        Reason=model.Reason,
                        CreatedDate = DateTime.Now,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        IsActive = true,
                        CompanyId=model.CompanyId
                };
                    _context.VehicleRequisitions.Add(vehicleRequisition);
                    await _context.SaveChangesAsync();
                }catch(Exception ex)
                {
                    var message = ex;
                }

            }
            else
            {
                obj.ResponsibleOfficerId = model.ResponsibleOfficerId;
                obj.Time = model.Time;
                obj.Location = model.Location;
                obj.TotalPersion = model.TotalPersion;
                obj.PrijectId = model.PrijectId;
                obj.Date = model.Date;
                obj.DayId = (int)model.DayType;
                obj.Reason = model.Reason;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                await _context.SaveChangesAsync();
            }

            return vm;
        }
        public async Task<bool> DeleteCarReq(int id)
        {
            var model = new ServiceStatusVm();
            var obj = await _context.VehicleRequisitions.SingleOrDefaultAsync(e => e.VehicleRequisitionId == id);
            if (obj == null)
            {
                return false;
            }
            obj.IsActive = false;
            obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            obj.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<VehicleVm> GetDetailsForUpdate(int?id)
        {
            VehicleVm vm = new VehicleVm();
            vm = await Task.Run(() => (from t1 in _context.VehicleRequisitions
                                       join t2 in _context.TeamInfoes on t1.ResponsibleOfficerId equals t2.Id
                                       join t3 in _context.Employees on t2.EmployeeId equals t3.Id
                                       join t4 in _context.ProductCategories on t1.PrijectId equals t4.ProductCategoryId
                                       where id == t1.VehicleRequisitionId 
                                       select new VehicleVm
                                       {
                                           OfficerName = t3.Name,
                                           ResponsibleOfficerId = t2.Id,
                                           VehicleRequisitionId=t1.VehicleRequisitionId,
                                           Location=t1.Location,
                                           Date=t1.Date,
                                           Time=t1.Time,
                                           PrijectId=t1.PrijectId,
                                           ProjectName=t4.Name,
                                           TotalPersion=t1.TotalPersion,
                                           Reason=t1.Reason,
                                           CompanyId=t1.CompanyId
                                        
                                       }).FirstOrDefaultAsync());
          
            return vm;
        }

     public async   Task<VehicleVm> Savedrivernamsper(VehicleVm model)
        {
            VehicleVm vm = new VehicleVm();
            var data = _context.VehicleRequisitions.SingleOrDefault(x => x.VehicleRequisitionId == model.VehicleRequisitionId);

            if (data != null)
            {
                data.DriverName = model.DriverName;
                await _context.SaveChangesAsync();

            }


            return vm;

        }





    }

    public class PermissionHandler : IPermissionHandler
    {
        private readonly ERPEntities _context;
        public PermissionHandler(ERPEntities context)
        {
            _context = context;
        }
        public bool HasPermission(int userId, int permissionNo, int companyId)
        {
            var permission = _context.UserPermissions.SingleOrDefault(q => q.UserId == userId
            && q.PermissionNo == permissionNo && q.IsActive == true && q.CompanyId == companyId);

            if (permission == null)
            {
                return false;
            }
            else { return true; }

        }




        public bool IsAdmin(int userId)
        {
            throw new NotImplementedException();
        }

        public bool IsSupperAdmin(int userId)
        {
            throw new NotImplementedException();
        }








    }









}

