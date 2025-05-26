using BEPZA_AUDIT.ViewModel;
using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.Implementation.CurrencyCon;
using KGERP.Service.Implementation.PortCountry;
using KGERP.Service.Implementation.TaskManagment;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.RealState;
using KGERP.Services.Procurement;
using KGERP.Utility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Unit = KGERP.Data.Models.Unit;

namespace KGERP.Service.Implementation

{
    public class ConfigurationService
    {
        private readonly ERPEntities _db;

        public ConfigurationService(ERPEntities db)
        {
            _db = db;
        }
        //#region User role Menuitem
        public async Task<VMUserMenuAssignment> UserMenuAssignmentGet(VMUserMenuAssignment vmUserMenuAssignment)
        {
            VMUserMenuAssignment vmMenuAssignment = new VMUserMenuAssignment();
            vmMenuAssignment.CompanyFK = vmUserMenuAssignment.CompanyFK;
            var companySubMenus = await _db.CompanySubMenus.Where(x => x.CompanyId == vmUserMenuAssignment.CompanyFK).ToListAsync();
            var companySubMenusId = companySubMenus.Select(x => x.CompanySubMenuId).ToList();

            var companyUserMenus = await _db.CompanyUserMenus.Where(x => x.CompanyId == vmUserMenuAssignment.CompanyFK && x.UserId == vmUserMenuAssignment.UserId).ToListAsync();
            var companyUserMenus_SubMenuId = companyUserMenus.Select(x => x.CompanySubMenuId).ToList();

            var companySubMenusNotExistsOnUserMenus = companySubMenusId.Where(CompanySubMenuId => !companyUserMenus_SubMenuId.Contains(CompanySubMenuId)).ToList();

            var filteredCompanySubMenus = companySubMenus.Where(x => companySubMenusNotExistsOnUserMenus.Contains(x.CompanySubMenuId)).ToList();
            if (filteredCompanySubMenus.Count() > 0)
            {
                List<CompanyUserMenu> userMenuList = new List<CompanyUserMenu>();
                foreach (var subMenus in filteredCompanySubMenus)
                {
                    CompanyUserMenu userMenu = new CompanyUserMenu
                    {
                        CompanyMenuId = subMenus.CompanyMenuId.Value,
                        CompanySubMenuId = subMenus.CompanySubMenuId,
                        IsActive = false,
                        IsView = true,
                        CompanyId = vmUserMenuAssignment.CompanyFK,
                        UserId = vmUserMenuAssignment.UserId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now
                    };

                    userMenuList.Add(userMenu);
                }

                _db.CompanyUserMenus.AddRange(userMenuList);
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                }


            }
            vmMenuAssignment.DataList = await Task.Run(() => CompanyUserMenuDataLoad(vmUserMenuAssignment));
            vmMenuAssignment.CompanyFK = vmUserMenuAssignment.CompanyFK;
            vmMenuAssignment.UserId = vmUserMenuAssignment.UserId;
            vmMenuAssignment.CompanyList = new SelectList(CompaniesDropDownList(vmMenuAssignment.CompanyFK??0), "Value", "Text");

            return vmMenuAssignment;
        }
        //public async Task<VMUserMenuAssignment> UserMenuAssignmentGet(VMUserMenuAssignment vmUserMenuAssignment)
        //{
        //    VMUserMenuAssignment vmMenuAssignment = new VMUserMenuAssignment();
        //    vmMenuAssignment.CompanyFK = vmUserMenuAssignment.CompanyFK;
        //    var companySubMenus = await _db.CompanySubMenus.Where(x => x.CompanyId == vmUserMenuAssignment.CompanyFK).ToListAsync();
        //    var companySubMenusId = companySubMenus.Select(x => x.CompanySubMenuId).ToList();

        //    var companyUserMenus =await _db.CompanyUserMenus.Where(x => x.CompanyId == vmUserMenuAssignment.CompanyFK && x.UserId == vmUserMenuAssignment.UserId).ToListAsync();
        //    var companyUserMenus_SubMenuId = companyUserMenus.Select(x => x.CompanySubMenuId).ToList();

        //    var companySubMenusNotExistsOnUserMenus = companySubMenusId.Where(CompanySubMenuId => !companyUserMenus_SubMenuId.Contains(CompanySubMenuId)).ToList();

        //    var filteredCompanySubMenus = companySubMenus.Where(x => companySubMenusNotExistsOnUserMenus.Contains(x.CompanySubMenuId)).ToList();
        //    if (filteredCompanySubMenus.Count() > 0)
        //    {
        //        List<CompanyUserMenu> userMenuList = new List<CompanyUserMenu>();
        //        foreach (var subMenus in filteredCompanySubMenus)
        //        {
        //            CompanyUserMenu userMenu = new CompanyUserMenu
        //            {
        //                CompanyMenuId = subMenus.CompanyMenuId.Value,
        //                CompanySubMenuId = subMenus.CompanySubMenuId,
        //                IsActive = false,
        //                IsView = true,
        //                CompanyId = vmUserMenuAssignment.CompanyFK,
        //                UserId = vmUserMenuAssignment.UserId,
        //                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
        //                CreatedDate = DateTime.Now
        //            };

        //            userMenuList.Add(userMenu);
        //        }

        //        _db.CompanyUserMenus.AddRange(userMenuList);
        //        try
        //        {
        //            await _db.SaveChangesAsync();
        //        }
        //        catch(Exception ex)
        //        {
        //            var x = ex.Message;
        //        }


        //    }
        //    vmMenuAssignment.DataList = await Task.Run(() => CompanyUserMenuDataLoad(vmUserMenuAssignment));
        //    vmMenuAssignment.CompanyFK = vmUserMenuAssignment.CompanyFK;
        //    vmMenuAssignment.UserId = vmUserMenuAssignment.UserId;
        //    vmMenuAssignment.CompanyList = new SelectList(CompaniesDropDownList(), "Value", "Text");

        //    return vmMenuAssignment;
        //}



        public IEnumerable<VMUserMenuAssignment> CompanyUserMenuDataLoad(VMUserMenuAssignment vmMenuAssignment)
        {
            var v = (from t1 in _db.CompanyUserMenus
                     join t2 in _db.CompanySubMenus on t1.CompanySubMenuId equals t2.CompanySubMenuId
                     join t3 in _db.CompanyMenus on t1.CompanyMenuId equals t3.CompanyMenuId
                     join t4 in _db.Companies on t2.CompanyId equals t4.CompanyId
                     where t1.UserId == vmMenuAssignment.UserId && t1.CompanyId == vmMenuAssignment.CompanyFK
                     select new VMUserMenuAssignment
                     {
                         CompanyName = t4.Name,
                         MenuName = t3.Name,
                         SubmenuName = t2.Name,
                         Method = t2.Action + "/" + t2.Controller,

                         SubmenuID = t2.CompanySubMenuId,
                         IsActive = t1.IsActive,
                         MenuPriority = t2.OrderNo,


                         MenuID = t3.CompanyMenuId,
                         CompanyUserMenusId = t1.CompanyUserMenuId,
                         UserId = t1.UserId,
                         CompanyFK = t1.CompanyId,
                     }).OrderBy(x => x.MenuPriority).AsEnumerable();
            return v;
        }


        //public async Task<int> UserRoleMenuItemAdd(VMUserRoleMenuItem vmUserRoleMenuItem)
        //{
        //    var result = -1;
        //    User_RoleMenuItem userRoleMenuItem = new User_RoleMenuItem
        //    {
        //        IsAllowed = vmUserRoleMenuItem.IsAllowed,
        //        User_MenuItemFk = vmUserRoleMenuItem.ID,
        //        User_RoleFK = vmUserRoleMenuItem.ID,
        //        User = vmUserRoleMenuItem.User,
        //        UserID = vmUserRoleMenuItem.UserID
        //    };
        //    _db.User_RoleMenuItem.Add(userRoleMenuItem);
        //    if (await _db.SaveChangesAsync() > 0)
        //    {
        //        result = userRoleMenuItem.ID;
        //    }
        //    return result;
        //}
        public CompanyUserMenu CompanyUserMenuEdit(VMUserMenuAssignment vmUserMenuAssignment)
        {
            long result = -1;
            //to select Accountining_Chart_Two data.....
            CompanyUserMenu companyUserMenus = _db.CompanyUserMenus.Find(vmUserMenuAssignment.CompanyUserMenusId);
            companyUserMenus.IsActive = vmUserMenuAssignment.IsActive;
            companyUserMenus.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            companyUserMenus.ModifiedDate = DateTime.Now;

            if (_db.SaveChanges() > 0)
            {
                result = companyUserMenus.CompanyUserMenuId;
            }
            return companyUserMenus;
        }
        ////public async (Task<int>, Task<bool>) UserRoleMenuItemEdit(VMUserRoleMenuItem vmUserRoleMenuItem)
        ////{
        ////    var result = -1;
        ////    //to select Accountining_Chart_Two data.....
        ////    User_RoleMenuItem userRoleMenuItem = _db.User_RoleMenuItem.Find(vmUserRoleMenuItem.ID);
        ////    userRoleMenuItem.IsAllowed = vmUserRoleMenuItem.IsAllowed;
        ////    userRoleMenuItem.User = vmUserRoleMenuItem.User;

        ////    if (await _db.SaveChangesAsync() > 0)
        ////    {
        ////        result = userRoleMenuItem.ID;
        ////    }
        ////    return result, false;
        ////}
        //public async Task<int> UserRoleMenuItemDelete(int id)
        //{
        //    int result = -1;

        //    if (id != 0)
        //    {
        //        User_RoleMenuItem userRoleMenuItem = _db.User_RoleMenuItem.Find(id);
        //        userRoleMenuItem.Active = false;

        //        if (await _db.SaveChangesAsync() > 0)
        //        {
        //            result = userRoleMenuItem.ID;
        //        }
        //    }
        //    return result;
        //}
        //#endregion
        public async Task<VMUserMenu> AccountingCostCenterGet(int companyId)
        {
            VMUserMenu vmUserMenu = new VMUserMenu();
            vmUserMenu.CompanyFK = companyId;
            vmUserMenu.DataList = (from t1 in _db.Accounting_CostCenter
                                   join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                                   where t1.CompanyId == companyId && t1.IsActive
                                   select new VMUserMenu
                                   {
                                       ID = t1.CostCenterId,
                                       Name = t1.Name,
                                       CompanyName = t2.Name,
                                       CompanyFK = t1.CompanyId
                                   }).OrderByDescending(x => x.ID).AsEnumerable();
            return vmUserMenu;
        }

        public async Task<int> AccountingCostCenterAdd(VMUserMenu vmUserMenu)
        {
            var result = -1;


            Accounting_CostCenter costCenter = new Accounting_CostCenter
            {

                Name = vmUserMenu.Name,

                CompanyId = vmUserMenu.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Accounting_CostCenter.Add(costCenter);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = costCenter.CostCenterId;
            }
            return result;
        }
        public async Task<int> AccountingCostCenterEdit(VMUserMenu vmUserMenu)
        {
            var result = -1;
            Accounting_CostCenter costCenter = _db.Accounting_CostCenter.Find(vmUserMenu.ID);
            costCenter.Name = vmUserMenu.Name;

            costCenter.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;


            if (await _db.SaveChangesAsync() > 0)
            {
                result = costCenter.CostCenterId;
            }
            return result;
        }
        public async Task<int> AccountingCostCenterDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Accounting_CostCenter costCenter = _db.Accounting_CostCenter.Find(id);
                costCenter.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = costCenter.CostCenterId;
                }
            }
            return result;
        }

        #region User Menu
        public async Task<VMUserMenu> UserMenuGet()
        {
            VMUserMenu vmUserMenu = new VMUserMenu();
            vmUserMenu.DataList = await Task.Run(() => UserMenuDataLoad());
            return vmUserMenu;
        }
        public async Task<VMUserMenu> UserMenuGetISS(int companyId)
        {
            VMUserMenu vmUserMenu = new VMUserMenu();
            vmUserMenu.DataList = await Task.Run(() => UserMenuDataLoadISS(companyId));
            return vmUserMenu;
        }

        public IEnumerable<VMUserMenu> UserMenuDataLoad()
        {
            var v = (from t1 in _db.CompanyMenus
                     join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                     where t1.IsActive == true
                     select new VMUserMenu
                     {
                         ID = t1.CompanyMenuId,
                         Name = t1.Name,
                         CompanyName = t2.Name,
                         LayerNo = t1.LayerNo,
                         ShortName = t1.ShortName,
                         Priority = t1.OrderNo,
                         IsActive = t1.IsActive,
                         CompanyFK = t1.CompanyId
                     }).OrderByDescending(x => x.ID).AsEnumerable();
            return v;
        }
        public IEnumerable<VMUserMenu> UserMenuDataLoadISS(int companyId)
        {
            var v = (from t1 in _db.CompanyMenus
                     join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                     where t1.IsActive == true && t1.CompanyId == companyId
                     select new VMUserMenu
                     {
                         ID = t1.CompanyMenuId,
                         Name = t1.Name,
                         CompanyName = t2.Name,
                         LayerNo = t1.LayerNo,
                         ShortName = t1.ShortName,
                         Priority = t1.OrderNo,
                         IsActive = t1.IsActive,
                         CompanyFK = t1.CompanyId
                     }).OrderByDescending(x => x.ID).AsEnumerable();
            return v;
        }


        public async Task<int> UserMenuAdd(VMUserMenu vmUserMenu)
        {
            var result = -1;


            CompanyMenu userMenu = new CompanyMenu
            {
                CompanyMenuId = _db.Database.SqlQuery<int>("exec spGetNewCompanyId").FirstOrDefault(),
                Name = vmUserMenu.Name,
                OrderNo = vmUserMenu.Priority,
                LayerNo = vmUserMenu.LayerNo,
                ShortName = vmUserMenu.Name,

                CompanyId = vmUserMenu.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.CompanyMenus.Add(userMenu);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = userMenu.CompanyMenuId;
            }
            return result;
        }
        public async Task<int> UserMenuEdit(VMUserMenu vmUserMenu)
        {
            var result = -1;
            using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    CompanyMenu userMenu = _db.CompanyMenus.Find(vmUserMenu.ID);
                    var CompanyUserMenuList = await _db.CompanyUserMenus
                       .Where(e => e.CompanyMenuId == vmUserMenu.ID
                       && e.CompanyId == userMenu.CompanyId && e.IsActive == true).ToListAsync();
                    CompanyUserMenuList.ForEach(e => e.CompanyId = vmUserMenu.CompanyFK);

                    userMenu.Name = vmUserMenu.Name;
                    userMenu.CompanyId = vmUserMenu.CompanyFK;

                    userMenu.OrderNo = vmUserMenu.Priority;
                    userMenu.LayerNo = vmUserMenu.LayerNo;
                    userMenu.ShortName = vmUserMenu.ShortName;
                    userMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    userMenu.ModifiedDate = DateTime.Now;
                    await _db.SaveChangesAsync();
                    result = userMenu.CompanyMenuId;
                    dbTran.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    dbTran.Rollback();
                    //throw;
                }
            }
            return result;
        }
        public async Task<int> UserMenuDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        CompanyMenu userMenu = _db.CompanyMenus.Find(id);
                        var CompanyUserMenuList = await _db.CompanyUserMenus
                           .Where(e => e.CompanyMenuId == userMenu.CompanyMenuId
                           && e.CompanyId == userMenu.CompanyId && e.IsActive == true).ToListAsync();
                        CompanyUserMenuList.ForEach(e => e.IsActive = false);

                        userMenu.IsActive = false;
                        userMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        userMenu.ModifiedDate = DateTime.Now;
                        await _db.SaveChangesAsync();
                        result = userMenu.CompanyMenuId;
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        //throw;
                    }
                }
            }
            return result;
        }
        #endregion

        #region User Submenu
        public async Task<VMUserSubMenu> UserSubMenuGet()
        {
            VMUserSubMenu vmUserSubMenu = new VMUserSubMenu();

            vmUserSubMenu.DataList = await Task.Run(() => UserSubMenuDataLoad());

            return vmUserSubMenu;
        }
        public async Task<VMUserSubMenu> UserSubMenuGetISS(int companyId)
        {
            VMUserSubMenu vmUserSubMenu = new VMUserSubMenu();

            vmUserSubMenu.DataList = await Task.Run(() => UserSubMenuDataLoadISS(companyId));

            return vmUserSubMenu;
        }

        public IEnumerable<VMUserSubMenu> UserSubMenuDataLoad()
        {
            var v = (from t1 in _db.CompanySubMenus
                     join t2 in _db.CompanyMenus on t1.CompanyMenuId equals t2.CompanyMenuId
                     join t3 in _db.Companies on t2.CompanyId equals t3.CompanyId

                     where t1.IsActive == true
                     select new VMUserSubMenu
                     {
                         CompanyName = t3.Name,
                         ID = t1.CompanySubMenuId,
                         Name = t1.Name,
                         Param = t1.Param,
                         CompanyFK = t1.CompanyId,
                         Controller = t1.Controller,
                         IsActive = t1.IsActive,
                         LayerNo = t1.LayerNo,
                         ShortName = t1.ShortName,
                         Action = t1.Action,
                         UserMenuName = t2.Name,
                         User_MenuFk = t2.CompanyMenuId,
                         Priority = t1.OrderNo

                     }).OrderByDescending(x => x.ID).AsEnumerable();
            return v;
        }
        public IEnumerable<VMUserSubMenu> UserSubMenuDataLoadISS(int companyId)
        {
            var v = (from t1 in _db.CompanySubMenus
                     join t2 in _db.CompanyMenus on t1.CompanyMenuId equals t2.CompanyMenuId
                     join t3 in _db.Companies on t2.CompanyId equals t3.CompanyId

                     where t1.IsActive == true && t1.CompanyId == companyId
                     select new VMUserSubMenu
                     {
                         CompanyName = t3.Name,
                         ID = t1.CompanySubMenuId,
                         Name = t1.Name,
                         Param = t1.Param,
                         CompanyFK = t1.CompanyId,
                         Controller = t1.Controller,
                         IsActive = t1.IsActive,
                         LayerNo = t1.LayerNo,
                         ShortName = t1.ShortName,
                         Action = t1.Action,
                         UserMenuName = t2.Name,
                         User_MenuFk = t2.CompanyMenuId,
                         Priority = t1.OrderNo

                     }).OrderByDescending(x => x.ID).AsEnumerable();
            return v;
        }


        public async Task<int> UserSubMenuAdd(VMUserSubMenu vmUserSubMenu)
        {
            var result = -1;

            var objectToSave = await _db.CompanySubMenus
                .SingleOrDefaultAsync(q => q.Name == vmUserSubMenu.Name
                && q.CompanyId == vmUserSubMenu.CompanyFK
                && q.CompanyMenuId == vmUserSubMenu.User_MenuFk
                );

            if (objectToSave != null)
            {

                return result = objectToSave.CompanySubMenuId;
            }


            CompanySubMenu userSubMenu = new CompanySubMenu
            {
                CompanySubMenuId = _db.Database.SqlQuery<int>("exec spGetNewCompanyId").FirstOrDefault(),
                Name = vmUserSubMenu.Name,
                CompanyId = vmUserSubMenu.CompanyFK,
                CompanyMenuId = vmUserSubMenu.User_MenuFk,
                OrderNo = vmUserSubMenu.Priority,
                Controller = vmUserSubMenu.Controller,
                Action = vmUserSubMenu.Action,
                LayerNo = vmUserSubMenu.LayerNo,
                IsActive = true,
                IsSideMenu = true,
                ShortName = vmUserSubMenu.ShortName,
                Param = vmUserSubMenu.Param,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now



            };
            _db.CompanySubMenus.Add(userSubMenu);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = userSubMenu.CompanySubMenuId;
            }
            return result;
        }
        public async Task<int> UserSubMenuEdit(VMUserSubMenu vmUserSubMenu)
        {
            var result = -1;
            using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    CompanySubMenu userSubMenu = _db.CompanySubMenus.Find(vmUserSubMenu.ID);

                    var CompanyUserMenuList = await _db.CompanyUserMenus
                        .Where(e => e.CompanyMenuId == userSubMenu.CompanyMenuId
                        && e.CompanySubMenuId == userSubMenu.CompanySubMenuId).ToListAsync();
                    CompanyUserMenuList.ForEach(e => e.CompanyMenuId = vmUserSubMenu.User_MenuFk);

                    userSubMenu.CompanyMenuId = vmUserSubMenu.User_MenuFk;
                    userSubMenu.Name = vmUserSubMenu.Name;
                    userSubMenu.OrderNo = vmUserSubMenu.Priority;
                    userSubMenu.Controller = vmUserSubMenu.Controller;
                    userSubMenu.Action = vmUserSubMenu.Action;
                    userSubMenu.LayerNo = vmUserSubMenu.LayerNo;
                    userSubMenu.ShortName = vmUserSubMenu.ShortName;
                    userSubMenu.Param = vmUserSubMenu.Param;
                    userSubMenu.CompanyId = vmUserSubMenu.CompanyFK;
                    userSubMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    userSubMenu.ModifiedDate = DateTime.Now;
                    _db.Entry(userSubMenu).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    result = userSubMenu.CompanySubMenuId;



                    dbTran.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    dbTran.Rollback();
                    //throw;
                }
            }
            return result;
        }
        //public async Task<int> UserSubMenuEdit(VMUserSubMenu vmUserSubMenu)
        //{

        //    var result = -1;
        //    //to select Accountining_Chart_Two data.....
        //    CompanySubMenu userSubMenu = _db.CompanySubMenus.Find(vmUserSubMenu.ID);
        //    userSubMenu.CompanyMenuId = vmUserSubMenu.User_MenuFk;
        //    userSubMenu.Name = vmUserSubMenu.Name;
        //    userSubMenu.OrderNo = vmUserSubMenu.Priority;
        //    userSubMenu.Controller = vmUserSubMenu.Controller;
        //    userSubMenu.Action = vmUserSubMenu.Action;
        //    userSubMenu.LayerNo = vmUserSubMenu.LayerNo;
        //    userSubMenu.ShortName = vmUserSubMenu.ShortName;
        //    userSubMenu.Param = vmUserSubMenu.Param;
        //    userSubMenu.CompanyId = vmUserSubMenu.CompanyFK;
        //    userSubMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //    userSubMenu.ModifiedDate = DateTime.Now;
        //    _db.Entry(userSubMenu).State = EntityState.Modified;
        //    if (await _db.SaveChangesAsync() > 0)
        //    {
        //        result = userSubMenu.CompanySubMenuId;
        //    }
        //    return result;
        //}

        public async Task<int> UserSubMenuDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        CompanySubMenu userSubMenu = _db.CompanySubMenus.Find(id);

                        var CompanyUserMenuList = await _db.CompanyUserMenus
                            .Where(e => e.CompanyMenuId == userSubMenu.CompanyMenuId
                            && e.CompanySubMenuId == userSubMenu.CompanySubMenuId && e.IsActive == true).ToListAsync();
                        CompanyUserMenuList.ForEach(e => e.IsActive = false);

                        userSubMenu.IsActive = false;
                        userSubMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        userSubMenu.ModifiedDate = DateTime.Now;
                        _db.Entry(userSubMenu).State = EntityState.Modified;
                        result = await _db.SaveChangesAsync();
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        //throw;
                    }
                }
            }
            return result;
        }
        #endregion

        #region Report Signatory
        public async Task<CommonReportSignatoryVM> GetReportSignatory(int companyId)
        {
            CommonReportSignatoryVM vmCommonSignatory = new CommonReportSignatoryVM();
            vmCommonSignatory.CompanyFK = companyId;
            vmCommonSignatory.DataList = await Task.Run(() => (from t1 in _db.ReportSignatories
                                                               join t2 in _db.ReportHeads on t1.ReportHeadId equals t2.ReportHeadId
                                                          where t1.IsActive==true
                                                          && t1.CompanyId == companyId
                                                          select new CommonReportSignatoryVM
                                                          {
                                                              ID = t1.ReportSignatoryId,
                                                              Name = t1.ReportPropertyName,
                                                              CompanyFK = t1.CompanyId,
                                                              ReportHeadId=t1.ReportHeadId,
                                                              ReportName=t2.ReportHeadName

                                                          }).OrderByDescending(x => x.ID).AsEnumerable());

            vmCommonSignatory.ReportList = await Task.Run(() => (from t1 in _db.ReportHeads
                                                                 where t1.IsActive == true
                                                                 && t1.CompanyId == companyId
                                                                 select new SelectListItem
                                                                 {
                                                                     Text = t1.ReportHeadName,
                                                                     Value = t1.ReportHeadId.ToString()

                                                                 }).OrderByDescending(x => x.Value).ToListAsync());
            return vmCommonSignatory;
        }

        

        public async Task<int> ReportSignatoryAdd(CommonReportSignatoryVM commonReportSignatory)
        {
            var result = -1;
            ReportSignatory commonRS = new ReportSignatory
            {
                ReportPropertyName = commonReportSignatory.Name,
                CompanyId = commonReportSignatory.CompanyFK,
                CreatedBy = (int?)Common.GetIntUserId()??0,
                ReportHeadId= commonReportSignatory.ReportHeadId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.ReportSignatories.Add(commonRS);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonRS.ReportSignatoryId;
            }
            return result;
        }

        public async Task<int> ReportSignatoryEdit(CommonReportSignatoryVM commonReportSignatory)
        {
            var result = -1;
            ReportSignatory commonRS = _db.ReportSignatories.Find(commonReportSignatory.ID);
            if (commonRS!=null)
            {
                commonRS.ReportPropertyName = commonReportSignatory.Name;
                commonRS.ReportHeadId = commonReportSignatory.ReportHeadId;
                commonRS.ModifyBy = (int)Common.GetIntUserId();
                commonRS.ModifyDate = DateTime.Now;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = commonReportSignatory.ID;
                }
                return result;
            }
            
            return result;
        }

        public async Task<int> ReportSignatoryDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                ReportSignatory commonRS = _db.ReportSignatories.Find(id);
                commonRS.IsActive = false;
                commonRS.ModifyBy = (int)Common.GetIntUserId();
                commonRS.ModifyDate = DateTime.Now;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = commonRS.ReportSignatoryId;
                }
            }
            return result;
        }
        #endregion

        #region Common Unit
        public async Task<VMCommonUnit> GetUnit(int companyId)
        {
            VMCommonUnit vmCommonUnit = new VMCommonUnit();
            vmCommonUnit.CompanyFK = companyId;
            vmCommonUnit.DataList = await Task.Run(() => (from t1 in _db.Units
                                                          where t1.IsActive
                                                          && t1.CompanyId == companyId
                                                          select new VMCommonUnit
                                                          {
                                                              ID = t1.UnitId,
                                                              Name = t1.Name,
                                                              CompanyFK = t1.CompanyId,
                                                              CreatedBy = t1.CreatedBy

                                                          }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonUnit;
        }
        public async Task<VMCommonUnit> GetSingleCommonUnit(int id)
        {

            var v = await Task.Run(() => (from t1 in _db.Units
                                          where t1.UnitId == id && t1.IsActive
                                          select new VMCommonUnit
                                          {
                                              ID = t1.UnitId,
                                              Name = t1.Name,
                                              CompanyFK = t1.CompanyId
                                          }).FirstOrDefault());
            return v;
        }
        public async Task<int> UnitAdd(VMCommonUnit vmCommonUnit)
        {
            var result = -1;
            Unit commonUnit = new Unit
            {
                Name = vmCommonUnit.Name,
                CompanyId = vmCommonUnit.CompanyFK,
                ShortName = vmCommonUnit.Name,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Units.Add(commonUnit);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonUnit.UnitId;
            }
            return result;
        }
        public async Task<int> UnitEdit(VMCommonUnit vmCommonUnit)
        {
            var result = -1;
            Unit commonUnit = _db.Units.Find(vmCommonUnit.ID);
            commonUnit.Name = vmCommonUnit.Name;
            commonUnit.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonUnit.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonUnit.UnitId;
            }
            return result;
        }
        public async Task<int> UnitDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Unit commonUnit = _db.Units.Find(id);
                commonUnit.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = commonUnit.UnitId;
                }
            }
            return result;
        }
        public object GetAutoCompleteSupplier(int companyId, string prefix)
        {
            var v = (from t1 in _db.Vendors
                     where t1.CompanyId == companyId
                     && t1.IsActive
                     && t1.VendorTypeId == (int)Provider.Supplier
                     && ((t1.Name.StartsWith(prefix)))

                     select new
                     {
                         label = t1.Name != null ? t1.Name : "",
                         val = t1.VendorId
                     }).OrderBy(x => x.label).Take(10).ToList();

            return v;
        }
        public object GetAutoCompleteSupplier(string prefix)
        {
            var v = (from t1 in _db.Vendors

                     where t1.IsActive && t1.VendorTypeId == (int)Provider.Supplier && ((t1.Name.StartsWith(prefix)))

                     select new
                     {
                         label = t1.Name != null ? t1.Name : "",
                         val = t1.VendorId
                     }).OrderBy(x => x.label).Take(10).ToList();

            return v;
        }
        public object GetAutoCompleteProductCategory(int companyId, string prefix, string productType)
        {
            var v = (from t1 in _db.ProductCategories

                     where t1.CompanyId == companyId && t1.IsActive && (productType == "" ? t1.ProductType != productType : t1.ProductType == productType) && ((t1.Name.StartsWith(prefix)))

                     select new
                     {
                         label = t1.Name != null ? t1.Name : "",
                         val = t1.ProductCategoryId
                     }).OrderBy(x => x.label).Take(10).ToList();

            return v;
        }

        public async Task<List<SelectModelType>> GetProductCategory(int companyId, string productType)
        {
            var list = await _db.ProductCategories
                .Where(s => s.CompanyId == companyId
                        && s.ProductType == productType &&
                        s.IsActive)
                .Select(e => new SelectModelType()
                {
                    Text = e.Name,
                    Value = e.ProductCategoryId,
                }).ToListAsync();

            return list;
        }


        public object GetAutoCompleteProduct(int companyId, string prefix, string productType)
        {

            var v = (from t1 in _db.Products
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                     join t4 in _db.Units on t1.UnitId equals t4.UnitId into Lt4
                     from t4 in Lt4.DefaultIfEmpty()
                     //join t5 in _db.OrderDeliverDetails on t1.ProductId equals t5.ProductId

                     where t1.CompanyId == companyId && t1.IsActive && t1.ProductType == productType && t2.IsActive && t3.IsActive  &&
                     ((t1.ProductName.Contains(prefix)) || (t2.Name.Contains(prefix)) || (t3.Name.Contains(prefix)))

                     select new
                     {
                         label = t3.Name + " " + t2.Name + " " + t1.ProductName,
                         val = t1.ProductId,
                         unit = t4.Name,
                         price = t1.CostingPrice
                     }).OrderBy(x => x.label).Take(20).ToList();

            return v;
        }




        public object GetAutoCompleteRawPackingMaterials(int companyId, string prefix)
        {
            var v = (from t1 in _db.Products
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId

                     where t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t3.IsActive &&
                     ((t1.ProductType == "R") || (t1.ProductType == "P")) &&
                     ((t1.ProductName.Contains(prefix)) || (t2.Name.Contains(prefix)) || (t3.Name.Contains(prefix)) || (t1.ShortName.Contains(prefix)))

                     select new
                     {
                         label = ((t1.ProductType == "R") ? "Raw Materials: " : (t1.ProductType == "P") ? "Packaging Materials: " : (t1.ProductType == "F") ? "Finished Goods: " : "") + t3.Name + " " + t2.Name + " " + t1.ProductName,
                         val = t1.ProductId
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }
        public object KPPLGetAutoCompleteRawPackingMaterials(int companyId, string prefix)
        {
            var v = (from t1 in _db.Products
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId

                     where t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t3.IsActive &&
                     //((t1.ProductType == "R") || (t1.ProductType == "P")) &&
                     ((t1.ProductName.Contains(prefix)) || (t2.Name.Contains(prefix)) || (t3.Name.Contains(prefix)) || (t1.ShortName.Contains(prefix)))

                     select new
                     {
                         label = ((t1.ProductType == "R") ? "Raw Materials: " : (t1.ProductType == "P") ? "Packaging Materials: " : (t1.ProductType == "F") ? "Finished Goods: " : "") + t3.Name + " " + t2.Name + " " + t1.ProductName,
                         val = t1.ProductId
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }



        public object GetAutoCompleteFinishedGoods(int companyId, string prefix)
        {
            var v = (from t1 in _db.Products
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId

                     where t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t3.IsActive &&
                     (t1.ProductType == "F") &&
                     ((t1.ProductName.Contains(prefix)) || (t2.Name.Contains(prefix)) || (t3.Name.Contains(prefix)))

                     select new
                     {
                         label = "Finished Goods: " + t3.Name + " " + t2.Name + " " + t1.ProductName,
                         val = t1.ProductId
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }

        public object GetAutoCompleteRawGoods(int companyId, string prefix)
        {
            var v = (from t1 in _db.Products
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId

                     where t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t3.IsActive &&
                     (t1.ProductType == "R") &&
                     ((t1.ProductName.Contains(prefix)) || (t2.Name.Contains(prefix)) || (t3.Name.Contains(prefix)))

                     select new
                     {
                         label = "Raw Goods: " + t3.Name + " " + t2.Name + " " + t1.ProductName,
                         val = t1.ProductId
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }


        public object GetAutoCompleteLot(int companyId, int productId)
        {
            var v = (from t1 in _db.Prod_ReferenceSlave
                     where t1.CompanyId == companyId && t1.IsActive && t1.FProductId == productId
                     select new
                     {
                         label = t1.LotNumber
                     }).OrderBy(x => x.label).Take(100).ToList();
           
            return v;
        }


        public object GetAutoCompleteLotRaw(int productId)
        {
            var v = (from t1 in _db.MaterialReceiveDetails
                     where t1.IsActive && t1.ProductId == productId
                     select new
                     {
                         value = t1.LotNumber.ToString(),
                         label = t1.LotNumber
                     }).OrderBy(x => x.label).Take(100).ToList();

            var grouped = v.GroupBy(x => string.IsNullOrEmpty(x.value) ? "xyzz" : x.value)
                           .Select(g => new
                           {
                               value = g.Key == "xyzz" ? "xyzz" : g.Key,
                               label = g.Key == "xyzz" ? "NoLot" : g.Key
                           })
                           .OrderBy(x => x.label)
                           .Take(100)
                           .ToList();

            // Insert default option at the beginning
            grouped.Insert(0, new { value = (string)null, label = "Select Lot" });

            return grouped;
        }




        public object GetAutoCompleteLotFinish(int productId)
        {
            var v = (from t1 in _db.Prod_ReferenceSlave
                     where t1.IsActive && t1.FProductId == productId
                     select new
                     {
                         value=t1.LotNumber.ToString(),
                         label = t1.LotNumber
                     }).Distinct().OrderBy(x => x.label).Take(100).ToList();

            var v2 = (from t1 in _db.MaterialReceiveDetails
                     where t1.IsActive && t1.ProductId == productId
                     select new
                     {
                         value = t1.LotNumber.ToString(),
                         label = t1.LotNumber
                     }).Distinct().OrderBy(x => x.label).Take(100).ToList();

            var v3 = v.Union(v2).ToList();


            var grouped = v3.GroupBy(x => string.IsNullOrEmpty(x.value) ? "xyzz" : x.value)
                   .Select(g => new
                   {
                       value = g.Key == "xyzz" ? "xyzz" : g.Key,
                       label = g.Key == "xyzz" ? "NoLot" : g.Key
                   })
                   .OrderBy(x => x.label)
                   .Take(100)
                   .ToList();

            return grouped;
        }




        public object GetAutoCompleteSaleperson(int companyId, string prefix)
        {
            var v = (from t1 in _db.OfficerAssigns
                     join t2 in _db.Employees on t1.EmpId equals t2.Id


                     where t1.CompanyId == companyId

                     select new
                     {
                         label = t2.Name,
                         val = t1.EmpId
                     }).OrderBy(x => x.label).ToList();

            return v;
        }

        public object AllEmployee(string prefix)
        {
            var v = (from t1 in _db.Employees
                     join t2 in _db.Designations on t1.DesignationId equals t2.DesignationId
                     where (t1.EmployeeId.Contains(prefix) || t1.Name.Contains(prefix) || t1.ShortName.Contains(prefix) || t2.Name.Contains(prefix))

                     select new
                     {
                         label = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                         val = t1.EmployeeId
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }
        public object AllEmployeebyid(string prefix)
        {
            var v = (from t1 in _db.Employees
                     join t2 in _db.Designations on t1.DesignationId equals t2.DesignationId
                     where (t1.EmployeeId.Contains(prefix) || t1.Name.Contains(prefix) || t1.ShortName.Contains(prefix) || t2.Name.Contains(prefix))

                     select new
                     {
                         label = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                         val = t1.Id
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }
        public object AllEmployeeIdByCompanyId(string prefix, int companyId)
        {
            var v = (from t1 in _db.Employees.Where(x => x.CompanyId == companyId && x.Active)
                     join t2 in _db.Designations on t1.DesignationId equals t2.DesignationId
                     where (t1.EmployeeId.Contains(prefix) || t1.Name.Contains(prefix) || t1.ShortName.Contains(prefix) || t2.Name.Contains(prefix))

                     select new
                     {
                         label = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                         val = t1.Id,
                         mob = t1.MobileNo,
                         Email = (t1.OfficeEmail == null ? t1.Email : t1.OfficeEmail),
                         Designation = t2.Name
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }
        public object AllEmployeeByCompanyId(string prefix, int companyId)
        {
            var v = (from t1 in _db.Employees.Where(x => x.CompanyId == companyId)
                     join t2 in _db.Designations on t1.DesignationId equals t2.DesignationId
                     where (t1.EmployeeId.Contains(prefix) || t1.Name.Contains(prefix) || t1.ShortName.Contains(prefix) || t2.Name.Contains(prefix))

                     select new
                     {
                         label = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                         val = t1.EmployeeId
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }
        public async Task<List<SelectDDLModel>> EmployeeByCompanyId(int companyId)
        {
            List<SelectDDLModel> result = new List<SelectDDLModel>();
            result = (from t1 in _db.Employees.Where(x => x.CompanyId == companyId)
                      join t2 in _db.Designations on t1.DesignationId equals t2.DesignationId
                      where t1.Active == true
                      select new SelectDDLModel
                      {
                          Text = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                          Value = t1.Id
                      }).OrderBy(x => x.Text).ToList();

            return result;
        }
        public object AllEmployeeforMenu(string prefix)
        {
            var v = (from t1 in _db.Employees.Where(q => q.Active)
                     join t2 in _db.Designations on t1.DesignationId equals t2.DesignationId
                     where (t1.EmployeeId.Contains(prefix) || t1.Name.Contains(prefix) || t1.ShortName.Contains(prefix) || t2.Name.Contains(prefix))

                     select new
                     {
                         label = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                         val = t1.EmployeeId
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }

        public object GetEmployeeforIncentive(string prefix)
        {
            var v = (from t1 in _db.Employees.Where(q => q.Active)
                     join t2 in _db.Designations on t1.DesignationId equals t2.DesignationId
                     where (t1.EmployeeId.Contains(prefix) || t1.Name.Contains(prefix) || t1.ShortName.Contains(prefix) || t2.Name.Contains(prefix))

                     select new
                     {
                         label = (t1.Name + "-" + t2.Name + "( " + t1.EmployeeId + " )"),
                         val = t1.Id
                     }).OrderBy(x => x.label).Take(100).ToList();

            return v;
        }


        public async Task<VMCommonProductCategory> GetSingleProductCategory(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.ProductCategories
                                          where t1.ProductCategoryId == id && t1.IsActive
                                          select new VMCommonProductCategory
                                          {
                                              ID = t1.ProductCategoryId,
                                              Name = t1.Name

                                          }).FirstOrDefault());
            return v;
        }
        public async Task<VMCommonProductSubCategory> GetSingleProductSubCategory(int id)
        {
            var v = await Task.Run(() => (from t1 in _db.ProductCategories
                                          where t1.ProductCategoryId == id && t1.IsActive
                                          select new VMCommonProductSubCategory
                                          {
                                              ID = t1.ProductCategoryId,
                                              Name = t1.Name
                                          }).FirstOrDefault());
            return v;
        }
        public async Task<List<VMCommonProductSubCategory>> CommonProductSubCategoryGet(int companyId, int categoryId)
        {

            List<VMCommonProductSubCategory> vmCommonProductSubCategoryList =
                await Task.Run(() => (_db.ProductSubCategories
                .Where(x => x.IsActive && x.ProductCategoryId == categoryId && x.CompanyId == companyId))
                .Select(x => new VMCommonProductSubCategory() { ID = x.ProductSubCategoryId, Name = x.Name })
                .ToListAsync());


            return vmCommonProductSubCategoryList;
        }
        public async Task<List<VMCommonUnit>> CompanyMenusGet(int companyId)
        {
            List<VMCommonUnit> vmCommonUnit =
                await Task.Run(() => (_db.CompanyMenus
                .Where(x => x.IsActive && x.CompanyId == companyId))
                .Select(x => new VMCommonUnit() { ID = x.CompanyMenuId, Name = x.Name })
                .ToListAsync());
            return vmCommonUnit;
        }
        public async Task<List<VMCommonProduct>> CommonProductGet(int companyId, int productSubCategoryId)
        {

            List<VMCommonProduct> vmCommonProductList =
                await Task.Run(() => (_db.Products
                .Where(x => x.IsActive && x.ProductSubCategoryId == productSubCategoryId && x.CompanyId == companyId))
                .Select(x => new VMCommonProduct() { ID = x.ProductId, Name = x.ProductName })
                .ToListAsync());


            return vmCommonProductList;
        }
        public async Task<List<VMCommonDistricts>> CommonDistrictsGet(int id)
        {

            List<VMCommonDistricts> vmCommonDistricts = await Task.Run(() => (_db.Districts.Where(x => x.IsActive && x.DivisionId == id)).Select(x => new VMCommonDistricts() { ID = x.DistrictId, Name = x.Name }).ToListAsync());

            return vmCommonDistricts;
        }
        public async Task<List<VMCommonDistricts>> CommonSubZonesGet(int id)
        {

            List<VMCommonDistricts> vmCommonDistricts = await Task.Run(() => (_db.SubZones.Where(x => x.IsActive && x.ZoneId == id)).Select(x => new VMCommonDistricts() { ID = x.SubZoneId, Name = x.Name }).ToListAsync());

            return vmCommonDistricts;
        }
        #endregion

        #region Supplier
        public async Task<VMCommonSupplier> GetSupplier(int companyId)
        {
            VMCommonSupplier vmCommonSupplier = new VMCommonSupplier();
            vmCommonSupplier.CompanyFK = companyId;
            vmCommonSupplier.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Supplier && x.CompanyId == companyId)
                                                              join t2 in _db.Countries on t1.CountryId equals t2.CountryId
                                                              where t1.IsActive == true
                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  Phone = t1.Phone,
                                                                  Country = t2.CountryName,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Common_CountriesFk = t1.CountryId.Value,
                                                                  HeadGLId = t1.HeadGLId,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  IsForeign = t1.IsForeign,
                                                                  BranchName = t1.BranchName,
                                                                  ACName = t1.ACName,
                                                                  ACNo = t1.ACNo,
                                                                  BankName = t1.BankName
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());


            return vmCommonSupplier;
        }
        public async Task<int> SupplierAdd(VMCommonSupplier vmCommonSupplier)
        {
            var result = -1;
            #region Genarate Supplier code
            int totalSupplier = _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Supplier && x.CompanyId == vmCommonSupplier.CompanyFK).Count();


            if (totalSupplier == 0)
            {
                totalSupplier = 1;
            }
            else
            {
                totalSupplier++;
            }

            var newString = totalSupplier.ToString().PadLeft(4, '0');
            #endregion
            Vendor commonSupplier = new Vendor
            {
                Code = newString,
                Name = vmCommonSupplier.Name,
                Phone = vmCommonSupplier.Phone,
                ContactName = vmCommonSupplier.ContactPerson,
                Email = vmCommonSupplier.Email,
                Address = vmCommonSupplier.Address,
                VendorTypeId = (int)Provider.Supplier,
                IsForeign = vmCommonSupplier.IsForeign,
                CompanyId = vmCommonSupplier.CompanyFK.Value,
                CountryId = vmCommonSupplier.Common_CountriesFk,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                BranchName = vmCommonSupplier.BranchName,
                ACName = vmCommonSupplier.ACName,
                ACNo = vmCommonSupplier.ACNo,
                BankName = vmCommonSupplier.BankName,
                IsActive = true,
                CreditRatioFrom = 0,
                CreditRatioTo = 0,

            };
            _db.Vendors.Add(commonSupplier);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonSupplier.VendorId;

                //31016 Account Payable Head for Seed Company

            }
            if (result > 0)
            {
                VMHeadIntegration vmHeadIntegration = new VMHeadIntegration();

                if (commonSupplier.CompanyId == (int)CompanyName.KrishibidSeedLimited)
                {
                    vmHeadIntegration = new VMHeadIntegration
                    {
                        AccName = commonSupplier.Name,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = 31016,
                        IsActive = true,

                        CompanyFK = commonSupplier.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,

                    };
                }
                if (commonSupplier.CompanyId == (int)CompanyName.GloriousCropCareLimited)
                {
                    vmHeadIntegration = new VMHeadIntegration
                    {
                        AccName = commonSupplier.Name,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = 39195,
                        IsActive = true,

                        CompanyFK = commonSupplier.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                    };
                }

                if (commonSupplier.CompanyId == (int)CompanyName.KrishibidFeedLimited)
                {
                    vmHeadIntegration = new VMHeadIntegration
                    {
                        AccName = commonSupplier.Name,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = 5283,


                        CompanyFK = commonSupplier.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                    };
                }

                if (commonSupplier.CompanyId == (int)CompanyName.KrishibidPackagingLimited)
                {
                    vmHeadIntegration = new VMHeadIntegration
                    {
                        AccName = commonSupplier.Name,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = 37310,


                        CompanyFK = commonSupplier.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                    };
                }

                HeadGL headGl = await PayableHeadIntegrationAdd(vmHeadIntegration);
                if (headGl != null)
                {
                    await VendorsCodeAndHeadGLIdEdit(commonSupplier.VendorId, headGl);
                }
            }


            return result;
        }

        public async Task<int> SEEDSupplierAdd(VMCommonSupplier vmCommonSupplier)
        {
            var result = -1;

            Vendor commonSupplier = new Vendor
            {
                Code = "",
                Name = vmCommonSupplier.Name,
                Phone = vmCommonSupplier.Phone,
                ContactName = vmCommonSupplier.ContactPerson,
                Email = vmCommonSupplier.Email,
                Address = vmCommonSupplier.Address,
                VendorTypeId = (int)Provider.Supplier,
                IsForeign = vmCommonSupplier.IsForeign,
                CompanyId = vmCommonSupplier.CompanyFK.Value,
                CountryId = vmCommonSupplier.Common_CountriesFk,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                BranchName = vmCommonSupplier.BranchName,
                ACName = vmCommonSupplier.ACName,
                ACNo = vmCommonSupplier.ACNo,
                BankName = vmCommonSupplier.BankName,
                IsActive = true,
                CreditRatioFrom = 0,
                CreditRatioTo = 0,


            };
            _db.Vendors.Add(commonSupplier);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonSupplier.VendorId;
                var head5id = await _db.Head5.Where(x => x.AccCode == "2401001001" && x.IsActive == true && x.CompanyId == vmCommonSupplier.CompanyFK).FirstOrDefaultAsync();
                VMHeadIntegration vmHeadIntegration = new VMHeadIntegration();
                vmHeadIntegration = new VMHeadIntegration
                {
                    AccName = commonSupplier.Name,
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ParentId = head5id.Id, // General Payable // Head5 Kfmal
                    IsActive = true,

                    CompanyFK = commonSupplier.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };

                HeadGL headGl = await SEEDSupplierHeadAdd(vmHeadIntegration, commonSupplier.VendorId);



            }



            return result;
        }

        public CompanyModel UserAssignedMenuGet()
        {
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            CompanyModel companyModel = new CompanyModel();
            companyModel.DataList = (from t1 in _db.CompanyUserMenus
                                     join t2 in _db.CompanySubMenus on t1.CompanySubMenuId equals t2.CompanySubMenuId
                                     join t3 in _db.CompanyMenus on t2.CompanyMenuId equals t3.CompanyMenuId
                                     where t1.IsActive && t1.UserId == userId
                                     select new CompanyModel
                                     {
                                         Id = t2.CompanySubMenuId,
                                         CompanyId = t2.CompanyId.Value,
                                         ParentId = t3.CompanyMenuId,
                                         Name = t2.Name,
                                         ShortName = t2.ShortName



                                     }).OrderByDescending(x => x.Id).AsEnumerable();
            return companyModel;
        }
        public async Task<int> KPLProjectCodeAndHeadGLIdEdit(int supplierId, HeadGL headGl, int heah5Id)
        {
            var result = -1;

            ProductCategory productCategories = _db.ProductCategories.Find(supplierId);
            productCategories.AccountingIncomeHeadId = headGl.Id;
            productCategories.AccountingHeadId = heah5Id;

            productCategories.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            productCategories.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = productCategories.ProductCategoryId;
            }
            return result;
        }
        public async Task<int> GLDLBlockCodeAndHeadGLIdEdit(int supplierId, HeadGL headGl, int heah5Id)
        {
            var result = -1;

            ProductSubCategory productCategories = _db.ProductSubCategories.Find(supplierId);
            productCategories.AccountingIncomeHeadId = headGl.Id;
            productCategories.AccountingHeadId = heah5Id;

            productCategories.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            productCategories.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = productCategories.ProductSubCategoryId;
            }
            return result;
        }
        public async Task<int> VendorsCodeAndHeadGLIdEdit(int supplierId, HeadGL headGl)
        {
            var result = -1;

            Vendor commonSupplier = _db.Vendors.Find(supplierId);
            commonSupplier.HeadGLId = headGl.Id;
            commonSupplier.Code = headGl.AccCode;

            commonSupplier.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonSupplier.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonSupplier.VendorId;
            }
            return result;
        }
        public async Task<int> VendorsCodeAndHead5IdEditFeed(int supplierId, Head5 head5)
        {
            var result = -1;

            Vendor commonSupplier = _db.Vendors.Find(supplierId);
            commonSupplier.HeadGLId = head5.Id;
            commonSupplier.Code = head5.AccCode;

            commonSupplier.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonSupplier.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonSupplier.VendorId;
            }
            return result;
        }









        public async Task<HeadGL> PayableHeadIntegrationAdd(VMHeadIntegration vmHeadIntegration)
        {
            long result = -1;

            string newAccountCode = "";
            int orderNo = 0;



            Head5 parentHead = _db.Head5.Where(x => x.Id == vmHeadIntegration.ParentId).FirstOrDefault();

            IQueryable<HeadGL> childHeads = _db.HeadGLs.Where(x => x.ParentId == vmHeadIntegration.ParentId);

            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 10);
                string childPart = lastAccCode.Substring(10, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }

            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }


            HeadGL headGL = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = newAccountCode,
                LayerNo = vmHeadIntegration.LayerNo,

                CompanyId = vmHeadIntegration.CompanyFK,
                CreateDate = vmHeadIntegration.CreatedDate,
                CreatedBy = vmHeadIntegration.CreatedBy,
                AccName = vmHeadIntegration.AccName,
                ParentId = vmHeadIntegration.ParentId,
                OrderNo = orderNo,
                Remarks = vmHeadIntegration.Remarks,
                IsActive = true

            };
            _db.HeadGLs.Add(headGL);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = headGL.Id;
            }
            return headGL;
        }
        public async Task<Head5> HEad5PayableHeadIntegrationAdd(VMHeadIntegration vmHeadIntegration)
        {
            long result = -1;

            string newAccountCode = "";
            int orderNo = 0;



            Head4 parentHead = _db.Head4.Where(x => x.Id == vmHeadIntegration.ParentId).FirstOrDefault();

            IQueryable<Head5> childHeads = _db.Head5.Where(x => x.ParentId == vmHeadIntegration.ParentId);

            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 7);
                string childPart = lastAccCode.Substring(7, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }

            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }


            Head5 head5 = new Head5
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = newAccountCode,
                LayerNo = vmHeadIntegration.LayerNo,

                CompanyId = vmHeadIntegration.CompanyFK,
                CreateDate = vmHeadIntegration.CreatedDate,
                CreatedBy = vmHeadIntegration.CreatedBy,
                AccName = vmHeadIntegration.AccName,
                ParentId = vmHeadIntegration.ParentId,
                OrderNo = orderNo,
                Remarks = vmHeadIntegration.Remarks,
                IsActive = true

            };
            _db.Head5.Add(head5);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = head5.Id;
            }
            return head5;
        }
        public async Task<HeadGL> KfmalCustomerHeadAdd(VMHeadIntegration vmHeadIntegration, int vendorId)
        {
            long result = -1;


            HeadGL headGL = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(vmHeadIntegration.ParentId),
                LayerNo = vmHeadIntegration.LayerNo,

                CompanyId = vmHeadIntegration.CompanyFK,
                CreateDate = vmHeadIntegration.CreatedDate,
                CreatedBy = vmHeadIntegration.CreatedBy,
                AccName = vmHeadIntegration.AccName,
                ParentId = vmHeadIntegration.ParentId,
                OrderNo = vmHeadIntegration.OrderNo,
                Remarks = vmHeadIntegration.Remarks,
                IsActive = true
            };
            _db.HeadGLs.Add(headGL);

            Vendor commonSupplier = _db.Vendors.Find(vendorId);
            commonSupplier.HeadGLId = headGL.Id;
            commonSupplier.Code = headGL.AccCode;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = headGL.Id;
            }
            return headGL;
        }

        public async Task<HeadGL> SEEDSupplierHeadAdd(VMHeadIntegration vmHeadIntegration, int vendorId)
        {
            long result = -1;


            HeadGL headGL = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(vmHeadIntegration.ParentId),
                LayerNo = vmHeadIntegration.LayerNo,

                CompanyId = vmHeadIntegration.CompanyFK,
                CreateDate = vmHeadIntegration.CreatedDate,
                CreatedBy = vmHeadIntegration.CreatedBy,
                AccName = vmHeadIntegration.AccName,
                ParentId = vmHeadIntegration.ParentId,
                OrderNo = vmHeadIntegration.OrderNo,
                Remarks = vmHeadIntegration.Remarks,
                IsActive = true
            };
            _db.HeadGLs.Add(headGL);

            Vendor commonSupplier = _db.Vendors.Find(vendorId);
            commonSupplier.HeadGLId = headGL.Id;
            commonSupplier.Code = headGL.AccCode;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = headGL.Id;
            }
            return headGL;
        }

        public async Task<HeadGL> IntegratedAccountsHeadEdit(string AccName, int headGLId)
        {
            long result = -1;
           


            HeadGL headGL = _db.HeadGLs.Find(headGLId);
            headGL.AccName = AccName;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = headGL.Id;
            }
            return headGL;
        }

        public async Task<HeadGL> IntegratedAccountsHeadEditCustomer(string AccName, int headGLId,int ParentId, string newAccountCode)
        {
            long result = -1;
            
            HeadGL headGL = _db.HeadGLs.Find(headGLId);
            headGL.AccName = AccName;
            headGL.ParentId = ParentId;
            headGL.AccCode = newAccountCode;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = headGL.Id;
            }
            return headGL;
        }
        public async Task<int> SupplierEdit(VMCommonSupplier vmCommonSupplier)
        {
            var result = -1;
            Vendor commonSupplier = _db.Vendors.Find(vmCommonSupplier.ID);

            commonSupplier.Name = vmCommonSupplier.Name;
            commonSupplier.Phone = vmCommonSupplier.Phone;
            commonSupplier.ContactName = vmCommonSupplier.ContactPerson;
            commonSupplier.Email = vmCommonSupplier.Email;
            commonSupplier.Address = vmCommonSupplier.Address;
            commonSupplier.IsForeign = vmCommonSupplier.IsForeign;

            commonSupplier.CountryId = vmCommonSupplier.Common_CountriesFk;
            commonSupplier.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonSupplier.ModifiedDate = DateTime.Now;
            commonSupplier.BranchName = vmCommonSupplier.BranchName;
            commonSupplier.ACName = vmCommonSupplier.ACName;
            commonSupplier.ACNo = vmCommonSupplier.ACNo;
            commonSupplier.BankName = vmCommonSupplier.BankName;
            

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonSupplier.VendorId;
            }
            await IntegratedAccountsHeadEdit(commonSupplier.Name, commonSupplier.HeadGLId.Value);
            return result;
        }
        public async Task<int> SupplierDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Vendor commonSupplier = _db.Vendors.Find(id);
                commonSupplier.IsActive = false;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = commonSupplier.VendorId;
                }
            }
            return result;
        }
        #endregion
        public async Task<VMPOTremsAndConditions> GetPOTremsAndConditions(int companyId)
        {
            VMPOTremsAndConditions vmCommonZone = new VMPOTremsAndConditions();
            vmCommonZone.CompanyFK = companyId;
            vmCommonZone.DataList = await Task.Run(() => (from t1 in _db.POTremsAndConditions
                                                          where t1.IsActive && t1.CompanyId == companyId
                                                          select new VMPOTremsAndConditions
                                                          {
                                                              ID = t1.ID,
                                                              Key = t1.Key,
                                                              Value = t1.Value,
                                                              CompanyFK = t1.CompanyId
                                                          }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonZone;
        }
        public async Task<int> POTremsAndConditionAdd(VMPOTremsAndConditions vmPOTremsAndConditions)
        {
            var result = -1;
            POTremsAndCondition poTremsAndConditions = new POTremsAndCondition
            {
                Key = vmPOTremsAndConditions.Key,
                Value = vmPOTremsAndConditions.Value,


                CompanyId = vmPOTremsAndConditions.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true

            };
            _db.POTremsAndConditions.Add(poTremsAndConditions);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = poTremsAndConditions.ID;
            }
            return result;
        }
        public async Task<int> POTremsAndConditionEdit(VMPOTremsAndConditions vmPOTremsAndConditions)
        {
            var result = -1;
            POTremsAndCondition poTremsAndCondition = _db.POTremsAndConditions.Find(vmPOTremsAndConditions.ID);

            poTremsAndCondition.Key = vmPOTremsAndConditions.Key;
            poTremsAndCondition.Value = vmPOTremsAndConditions.Value;

            poTremsAndCondition.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            poTremsAndCondition.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = poTremsAndCondition.ID;
            }
            return result;
        }
        public async Task<int> POTremsAndConditionDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                POTremsAndCondition poTremsAndCondition = _db.POTremsAndConditions.Find(id);
                poTremsAndCondition.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = poTremsAndCondition.ID;
                }
            }
            return result;
        }


        #region Zone
        public List<SelectModel> GetZoneList(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            SelectModel selectModel = new SelectModel
            {
                Text = "==Select Zone==",
                Value = 0,
            };
            selectModelLiat.Add(selectModel);

            var v = _db.Zones.Where(x => x.CompanyId == companyId).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.ZoneId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }
        public List<SelectModel> GetSubZoneList(int companyId, int? zoneId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            SelectModel selectModel = new SelectModel
            {
                Text = "==Select Sub Zone==",
                Value = 0,
            };
            selectModelLiat.Add(selectModel);

            if (zoneId.HasValue && zoneId > 0)
            {
                var v = _db.SubZones.Where(x => x.CompanyId == companyId && x.ZoneId == zoneId).ToList()
               .Select(x => new SelectModel()
               {
                   Text = x.Name,
                   Value = x.SubZoneId
               }).ToList();
                selectModelLiat.AddRange(v);
            }
            else
            {
                var v = _db.SubZones.Where(x => x.CompanyId == companyId).ToList()
                             .Select(x => new SelectModel()
                             {
                                 Text = x.Name,
                                 Value = x.SubZoneId
                             }).ToList();
                selectModelLiat.AddRange(v);
            }



            return selectModelLiat;
        }
        public async Task<VMCommonZone> GetZones(int companyId)
        {
            VMCommonZone vmCommonZone = new VMCommonZone();
            vmCommonZone.CompanyFK = companyId;
            vmCommonZone.DataList = await Task.Run(() => (from t1 in _db.Zones
                                                          where t1.IsActive && t1.CompanyId == companyId
                                                          select new VMCommonZone
                                                          {
                                                              ID = t1.ZoneId,
                                                              Name = t1.Name,
                                                              CompanyFK = t1.CompanyId,
                                                              ZoneIncharge = t1.ZoneIncharge,
                                                              Designation = t1.Designation,
                                                              Email = t1.Email,
                                                              MobileOffice = t1.MobileOffice,
                                                              MobilePersonal = t1.MobilePersonal,

                                                          }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonZone;
        }
        public async Task<int> ZoneAdd(VMCommonZone vmCommonZone)
        {
            var result = -1;
            Zone zone = new Zone
            {
                Name = vmCommonZone.Name,
                ZoneIncharge = vmCommonZone.ZoneIncharge,
                ZoneInchargeId = vmCommonZone.ZoneInchargeId,
                Designation = vmCommonZone.Designation,
                Email = vmCommonZone.Email,
                MobileOffice = vmCommonZone.MobileOffice,
                MobilePersonal = vmCommonZone.MobilePersonal,

                CompanyId = vmCommonZone.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true

            };
            _db.Zones.Add(zone);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = zone.ZoneId;


                var zone3id = await _db.Head3.Where(x => x.AccCode == "1304" && x.IsActive == true && x.CompanyId == vmCommonZone.CompanyFK).FirstOrDefaultAsync(); // Account Recevable / Head 3

                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = zone.Name,
                    LayerNo = 4,
                    Remarks = "4 Layer",
                    IsIncomeHead = false,
                    ParentId = zone3id.Id,
                    IsActive = true,

                    CompanyFK = zone.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                int head4Id = ISSZoneHead4Push(integration, zone.ZoneId);


            }
            return result;
        }
        public async Task<int> ZonesEdit(VMCommonZone vmCommonZone)
        {
            var result = -1;
            Zone zone = _db.Zones.Find(vmCommonZone.ID);
            zone.Name = vmCommonZone.Name;
            zone.ZoneIncharge = vmCommonZone.ZoneIncharge;
            zone.ZoneInchargeId = vmCommonZone.ZoneInchargeId;

            zone.Designation = vmCommonZone.Designation;
            zone.Email = vmCommonZone.Email;
            zone.MobileOffice = vmCommonZone.MobileOffice;
            zone.MobilePersonal = vmCommonZone.MobilePersonal;

            zone.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            zone.ModifiedDate = DateTime.Now;

            if (zone.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                Head4 head4 = _db.Head4.Find(zone.HeadGLId);
                head4.AccName = zone.Name;
            }

            if (await _db.SaveChangesAsync() > 0)
            {
                result = zone.ZoneId;
            }
            return result;
        }
        public async Task<int> ZonesDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Zone zone = _db.Zones.Find(id);
                zone.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = zone.ZoneId;
                }
            }
            return result;
        }

        #endregion

        #region Zone
        public async Task<VMCommonSubZone> GetSubZones(int companyId, int zoneId = 0)
        {
            VMCommonSubZone vmCommonSubZone = new VMCommonSubZone();
            vmCommonSubZone.CompanyFK = companyId;
            vmCommonSubZone.DataList = await Task.Run(() => (from t1 in _db.SubZones
                                                             join t2 in _db.Zones on t1.ZoneId equals t2.ZoneId
                                                             where t1.IsActive && t1.CompanyId == companyId
                                                             && (zoneId > 0 ? t1.ZoneId == zoneId : t1.SubZoneId > 0)
                                                             select new VMCommonSubZone
                                                             {
                                                                 ID = t1.SubZoneId,
                                                                 ZoneId = t1.ZoneId,
                                                                 Name = t1.Name,
                                                                 ZoneName = t2.Name,
                                                                 SalesOfficerName = t1.SalesOfficerName,
                                                                 SalesOfficerId = t1.SalesOfficerId,
                                                                 Designation = t1.Designation,
                                                                 Email = t1.Email,
                                                                 MobileOffice = t1.MobileOffice,
                                                                 MobilePersonal = t1.MobilePersonal,
                                                                 CompanyFK = t1.CompanyId
                                                             }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonSubZone;
        }
        public async Task<int> SubZoneAdd(VMCommonSubZone vmCommonSubZone)
        {
            var result = -1;
            SubZone subZone = new SubZone
            {
                Name = vmCommonSubZone.Name,
                SalesOfficerName = vmCommonSubZone.SalesOfficerName,
                SalesOfficerId = vmCommonSubZone.SalesOfficerId,
                Designation = vmCommonSubZone.Designation,
                Email = vmCommonSubZone.Email,
                MobileOffice = vmCommonSubZone.MobileOffice,
                MobilePersonal = vmCommonSubZone.MobilePersonal,
                ZoneId = vmCommonSubZone.ZoneId,
                CompanyId = vmCommonSubZone.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true

            };
            _db.SubZones.Add(subZone);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = subZone.SubZoneId;



                int ParentId = _db.Zones.Find(subZone.ZoneId).HeadGLId;

                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = subZone.Name,
                    LayerNo = 5,
                    Remarks = "5 Layer",
                    IsIncomeHead = false,
                    ParentId = ParentId,
                    IsActive = true,

                    CompanyFK = subZone.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                int head4Id = ISSSubZoneHead5Push(integration, subZone.SubZoneId);



            }
            return result;
        }
        public async Task<int> SubZonesEdit(VMCommonSubZone vmCommonSubZone)
        {
            var result = -1;
            SubZone subZone = _db.SubZones.Find(vmCommonSubZone.ID);
            subZone.ZoneId = vmCommonSubZone.ZoneId;
            subZone.Name = vmCommonSubZone.Name;
            subZone.SalesOfficerName = vmCommonSubZone.SalesOfficerName;
            subZone.SalesOfficerId = vmCommonSubZone.SalesOfficerId;
            subZone.Designation = vmCommonSubZone.Designation;
            subZone.Email = vmCommonSubZone.Email;
            subZone.MobilePersonal = vmCommonSubZone.MobilePersonal;
            subZone.MobileOffice = vmCommonSubZone.MobileOffice;
            subZone.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            subZone.ModifiedDate = DateTime.Now;

            if (subZone.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                Head5 head5 = _db.Head5.Find(subZone.AccountHeadId);
                head5.AccName = subZone.Name;
            }

            if (await _db.SaveChangesAsync() > 0)
            {
                result = subZone.SubZoneId;

            }
            return result;
        }
        public async Task<int> SubZonesDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                SubZone zone = _db.SubZones.Find(id);
                zone.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = zone.ZoneId;
                }
            }
            return result;
        }

        #endregion

        #region Product Category 
        public async Task<VMCommonProductCategory> GetFinishProductCategory(int companyId, string productType)
        {
            VMCommonProductCategory vmCommonProductCategory = new VMCommonProductCategory();
            vmCommonProductCategory.CompanyFK = companyId;
            vmCommonProductCategory.DataList = await Task.Run(() => (from t1 in _db.ProductCategories
                                                                     where t1.ProductType == productType &&
                                                                     t1.IsActive && t1.CompanyId == companyId && t1.IsActive
                                                                     // productCategoryId > 0 ? t1.ProductCategoryId == productCategoryId: t1.ProductCategoryId > 0
                                                                     select new VMCommonProductCategory
                                                                     {
                                                                         ID = t1.ProductCategoryId,
                                                                         Name = t1.Name,
                                                                         ProductType = t1.ProductType,
                                                                         CompanyFK = t1.CompanyId,
                                                                         CashCommission = t1.CashCustomerRate,
                                                                         Remarks = t1.Remarks,
                                                                         OrderNo = t1.OrderNo
                                                                     }).OrderBy(x => x.OrderNo).AsEnumerable());
            return vmCommonProductCategory;
        }
        public async Task<int> ProductFinishCategoryAdd(VMCommonProductCategory productCategoryModel)
        {
            var result = -1;
            ProductCategory productCategory = new ProductCategory
            {

                Name = productCategoryModel.Name,
                ProductType = productCategoryModel.ProductType,
                CashCustomerRate = productCategoryModel.CashCommission,

                Remarks = productCategoryModel.Remarks,
                CompanyId = productCategoryModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                NoOfFloors = productCategoryModel.NoOfFloors ?? "",
                FlatSizeSpecification = productCategoryModel.FlatSizeSpecification ?? "",
                IsActive = true,


            };
            _db.ProductCategories.Add(productCategory);
            if (await _db.SaveChangesAsync() > 0)
            {



                var catetegory = _db.ProductCategories.Find(productCategory.ProductCategoryId);
                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = catetegory.Name,
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ProductType = catetegory.ProductType,
                    CompanyFK = productCategory.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                int head5Id = FinishAccHeadGlPushSeed(integration, productCategory.ProductCategoryId);

                result = productCategory.ProductCategoryId;
            }
            return result;
        }

        public async Task<int> ProductRawCategoryAdd(VMCommonProductCategory productCategoryModel)
        {
            var result = -1;
            ProductCategory productCategory = new ProductCategory
            {

                Name = productCategoryModel.Name,
                ProductType = productCategoryModel.ProductType,
                CashCustomerRate = productCategoryModel.CashCommission,

                Remarks = productCategoryModel.Remarks,
                CompanyId = productCategoryModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                NoOfFloors = productCategoryModel.NoOfFloors ?? "",
                FlatSizeSpecification = productCategoryModel.FlatSizeSpecification ?? "",
                IsActive = true,


            };
            _db.ProductCategories.Add(productCategory);
            if (await _db.SaveChangesAsync() > 0)
            {



                var catetegory = _db.ProductCategories.Find(productCategory.ProductCategoryId);
                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = catetegory.Name,
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ProductType = catetegory.ProductType,
                    CompanyFK = productCategory.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                int head5Id = AccHeadGlPushSeed(integration, productCategory.ProductCategoryId);

                result = productCategory.ProductCategoryId;
            }
            return result;
        }

        public async Task<int> ProductFinishCategoryEdit(VMCommonProductCategory vmCommonProductCategory)
        {
            var result = -1;
            ProductCategory productCategory = _db.ProductCategories.Find(vmCommonProductCategory.ID);
            productCategory.Name = vmCommonProductCategory.Name;
            productCategory.CashCustomerRate = vmCommonProductCategory.CashCommission;

            productCategory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            productCategory.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = productCategory.ProductCategoryId;
            }
            return result;
        }
        public async Task<int> ProductFinishCategoryDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                ProductCategory productCategory = _db.ProductCategories.Find(id);
                productCategory.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = productCategory.ProductCategoryId;
                }
            }
            return result;
        }

        #endregion


        public List<SelectVm> GetBrandList(int companyId)
        {
            var model = new List<SelectVm>();
            model = (from t1 in _db.Brands
                     where t1.CompanyId == companyId

                     select new SelectVm
                     {
                         Id = t1.BrandId,
                         Name = t1.BrandName
                     }).OrderBy(o => o.Name).ToList();
            return model;
        }

        #region Common Product Subcategory
        public async Task<VMCommonProductSubCategory> GetProductSubCategory(int companyId, int categoryId, string productType)
        {
            VMCommonProductSubCategory vmCommonProductSubCategory = new VMCommonProductSubCategory();
            if (categoryId > 0)
            {
                vmCommonProductSubCategory = await Task.Run(() => (from t1 in _db.ProductCategories.Where(x => x.ProductCategoryId == categoryId && x.IsActive)

                                                                   select new VMCommonProductSubCategory
                                                                   {

                                                                       Common_ProductCategoryFk = t1.ProductCategoryId,
                                                                       CategoryName = t1.Name,
                                                                       BaseCommissionRate = t1.CashCustomerRate,
                                                                       CompanyFK = t1.CompanyId,
                                                                       ProductType = t1.ProductType,
                                                                       Description = t1.Address,
                                                                       Code = t1.Code
                                                                   }).FirstOrDefault());
            }

            else
            {
                vmCommonProductSubCategory.CompanyFK = companyId;
                vmCommonProductSubCategory.ProductType = productType;

            }
            vmCommonProductSubCategory.DataList = await Task.Run(() => (from t1 in _db.ProductSubCategories
                                                                        join t2 in _db.ProductCategories on t1.ProductCategoryId equals t2.ProductCategoryId
                                                                        where t1.IsActive && t2.IsActive && t1.CompanyId == companyId
                                                                        && t1.ProductType == productType
                                                                        && ((categoryId > 0) ? t1.ProductCategoryId == categoryId : t1.ProductCategoryId > 0)
                                                                        select new VMCommonProductSubCategory
                                                                        {
                                                                            Common_ProductCategoryFk = t2.ProductCategoryId,
                                                                            CategoryName = t2.Name,
                                                                            ProductType = t1.ProductType,
                                                                            CompanyFK = t1.CompanyId,
                                                                            ID = t1.ProductSubCategoryId,
                                                                            Name = t1.Name,
                                                                            Code = t1.Code,
                                                                            BaseCommissionRate = t1.BaseCommissionRate,
                                                                        }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonProductSubCategory;
        }




        public async Task<int> ProductSubCategoryAdd(VMCommonProductSubCategory vmCommonProductSubCategory)
        {
            var result = -1;
            ProductSubCategory commonProductSubCategory = new ProductSubCategory
            {
                Name = vmCommonProductSubCategory.Name,
                Code = vmCommonProductSubCategory.Code,
                ProductCategoryId = vmCommonProductSubCategory.Common_ProductCategoryFk,
                BaseCommissionRate = vmCommonProductSubCategory.BaseCommissionRate,
                ProductType = vmCommonProductSubCategory.ProductType,
                CompanyId = vmCommonProductSubCategory.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
                //BrandId=vmCommonProductSubCategory.BrandId
            };
            _db.ProductSubCategories.Add(commonProductSubCategory);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonProductSubCategory.ProductSubCategoryId;

                //if (commonProductSubCategory.CompanyId == (int)CompanyName.GloriousCropCareLimited)
                //{

                //    ProductSubCategory Subcatetegory = _db.ProductSubCategories.Find(commonProductSubCategory.ProductSubCategoryId);

                //    VMHeadIntegration integration = new VMHeadIntegration
                //    {
                //        AccName = Subcatetegory.Name,
                //        LayerNo = 6,
                //        Remarks = "6 Layer",
                //        IsIncomeHead = false,
                //        ProductType = Subcatetegory.ProductType,
                //        CompanyFK = commonProductSubCategory.CompanyId,
                //        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                //        CreatedDate = DateTime.Now
                //    };


                //    int headGl = AccHeadGlPush(integration, commonProductSubCategory);

                //    //if (headGlId != null)
                //    //{
                //    //    await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                //    //}
                //}


                //if (commonProductSubCategory.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
                //{
                //    int head5Id = BlockHead5Push(commonProductSubCategory);
                //    var catetegory = _db.ProductCategories.Find(commonProductSubCategory.ProductCategoryId);
                //    VMHeadIntegration integration = new VMHeadIntegration
                //    {
                //        AccName = catetegory.Name + " - " + commonProductSubCategory.Name,
                //        LayerNo = 6,
                //        Remarks = "GL Layer",
                //        IsIncomeHead = false,
                //        ParentId = 50602122,

                //        CompanyFK = commonProductSubCategory.CompanyId,
                //        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                //        CreatedDate = DateTime.Now,
                //    };
                //    HeadGL headGlId = await PayableHeadIntegrationAdd(integration);
                //    if (headGlId != null)
                //    {
                //        await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                //    }
                //}
                //if (commonProductSubCategory.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
                //{

                //    ProductSubCategory Subcatetegory = _db.ProductSubCategories.Find(commonProductSubCategory.ProductSubCategoryId);

                //    //Brand brnd = _db.Brands.Find(commonProductSubCategory.BrandId);
                //    VMHeadIntegration integration = new VMHeadIntegration
                //    {
                //        AccName = Subcatetegory.Name, //+ "(" + brnd.BrandName+")",
                //        LayerNo = 5,
                //        Remarks = "5 Layer",
                //        IsIncomeHead = false,
                //        ProductType = Subcatetegory.ProductType,
                //        CompanyFK = commonProductSubCategory.CompanyId,
                //        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                //        CreatedDate = DateTime.Now
                //    };


                //    int headGl = Acc5PushKFMAL(integration, commonProductSubCategory);

                //    //if (headGlId != null)
                //    //{
                //    //    await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                //    //}
                //}
            }



            return result;
        }
        public async Task<int> GCCLStockHeadGLPush(string accName, int companyId, string productType)
        {
            var result = -1;
            //List<HeadGL> headGLs = new List<HeadGL>();
            //if (productType == "P")
            //{
            //    HeadGL headGL = new HeadGL
            //    {
            //        AccCode = "",
            //        AccName = accName,
            //        CompanyId = companyId,
            //        LayerNo = 6,
            //        CreateDate = DateTime.Now,
            //        CreatedBy = "",
            //        ParentId = productType == "P" ? ,
            //        Remarks = "GL Layer",
            //        OrderNo = 0
            //    };
            //}

            //headGLs.Add(headGL);
            //_db.HeadGLs.AddRange(headGLs);
            //if (await _db.SaveChangesAsync() > 0)
            //{
            //    result = 1;
            //}



            return result;
        }
        public async Task<int> ProductSubCategoryEdit(VMCommonProductSubCategory vmCommonProductSubCategory)
        {

            var result = -1;
            ProductSubCategory commonProductSubCategory = _db.ProductSubCategories.Find(vmCommonProductSubCategory.ID);
            commonProductSubCategory.Name = vmCommonProductSubCategory.Name;
            commonProductSubCategory.Code = vmCommonProductSubCategory.Code;
            commonProductSubCategory.ProductCategoryId = vmCommonProductSubCategory.Common_ProductCategoryFk;
            commonProductSubCategory.BaseCommissionRate = vmCommonProductSubCategory.BaseCommissionRate;
            //commonProductSubCategory.BrandId = vmCommonProductSubCategory.BrandId;

            commonProductSubCategory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonProductSubCategory.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonProductSubCategory.ProductSubCategoryId;

                if (commonProductSubCategory.CompanyId == (int)CompanyName.KrishibidFeedLimited)
                {
                    //Setting up Customer Offer
                    _db.Database.ExecuteSqlCommand("exec spUpdateBaseCommission {0},{1},{2}", commonProductSubCategory.CompanyId, commonProductSubCategory.ProductSubCategoryId, commonProductSubCategory.ModifiedBy);


                }
            }
            return result;
        }
        public async Task<int> ProductSubCategoryDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                ProductSubCategory commonProductSubCategory = _db.ProductSubCategories.Find(id);
                commonProductSubCategory.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = commonProductSubCategory.ProductSubCategoryId;
                }
            }
            return result;
        }




        #endregion

        public class EnumModel
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }
        public List<object> CommonCountriesDropDownList()
        {
            var list = new List<object>();
            var v = _db.Countries.ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.CountryName, Value = x.CountryId });
            }
            return list;
        }
        public List<object> CommonDistrictsDropDownList()
        {
            var list = new List<object>();
            var v = _db.Districts.Where(a => a.IsActive).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.DistrictId });
            }
            return list;
        }
        public List<object> CommonUpazilasDropDownList()
        {
            var list = new List<object>();
            var v = _db.Upazilas.Where(a => a.IsActive == true).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.UpazilaId });
            }
            return list;
        }
        public List<object> CommonDivisionsDropDownList()
        {
            var list = new List<object>();
            var v = _db.Divisions.ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.DivisionId });
            }
            return list;
        }
        public class CustomerPaymentType
        {
            public string Text { get; set; }
            public string Value { get; set; }

        }
        public List<object> CommonCustomerPaymentType()
        {
            var list = new List<object>();

            var students = new List<CustomerPaymentType>() {
                new CustomerPaymentType(){ Text = "Credit", Value="Credit"},
                new CustomerPaymentType(){ Text = "Cash", Value="Cash"},
                new CustomerPaymentType(){ Text = "Special", Value="Special"}


            };

            foreach (var x in students)
            {
                list.Add(new { Text = x.Text, Value = x.Value });
            }
            return list;
        }

        public List<object> CommonRelationList()
        {
            var list = new List<object>();
            list.Add(new { Text = "Wife", Value = "Wife" });
            list.Add(new { Text = "Husband", Value = "Husband" });
            list.Add(new { Text = "Son", Value = "Son" });
            list.Add(new { Text = "Doughter", Value = "Doughter" });
            list.Add(new { Text = "Brother", Value = "Brother" });
            list.Add(new { Text = "Sister", Value = "Sister" });
            list.Add(new { Text = "Father", Value = "Father" });
            list.Add(new { Text = "Mother", Value = "Mother" });

            return list;
        }
        public List<object> CommonZonesDropDownList(int companyId)
        {
            var list = new List<object>();
            var v = _db.Zones.Where(x => x.IsActive && x.CompanyId == companyId).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.ZoneId });
            }
            return list;
        }
        public List<SelectModelType> GCClZoneDropDownList(int companyId)
        {
            var list = new List<SelectModelType>();
            list = _db.Zones.Where(x => x.IsActive == true && x.CompanyId == companyId && x.ZoneId > 0)
                .Select(s => new SelectModelType
                {
                    Value = s.ZoneId,
                    Text = s.Name
                }
                ).ToList();
            return list;
        }
        public List<object> CompaniesDropDownList(int companyid=0)
        {
            var list = new List<object>();
            var v = _db.Companies.Where(x=>(companyid==0 || x.CompanyId==companyid)).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.CompanyId });
            }
            return list;
        }
        public List<object> CompaniesDropDownListISS(int companyId)
        {
            var list = new List<object>();
            var v = _db.Companies.Where(x => x.IsCompany && x.CompanyId == companyId && x.IsActive).OrderBy(x => x.Name).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.CompanyId });
            }
            return list;
        }

        public List<object> CompanyMenusDropDownList()
        {
            var list = new List<object>();
            var v = _db.CompanyMenus.ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.CompanyMenuId });
            }
            return list;
        }
        public List<object> CompanyMenusDropDownListISS(int companyId)
        {
            var list = new List<object>();
            var v = _db.CompanyMenus.Where(x => x.IsActive && x.CompanyId == companyId).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.CompanyMenuId });
            }
            return list;
        }
        public List<object> CommonSubZonesDropDownList(int companyId, int zoneId = 0)
        {
            var list = new List<object>();
            var v = _db.SubZones.Where(x => x.IsActive && x.CompanyId == companyId && x.SubZoneId != 104 && x.SubZoneId != 105 && (zoneId > 0 ? x.ZoneId == zoneId : x.SubZoneId > 0)).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.SubZoneId });
            }
            return list;
        }

        public List<object> ChekdetailList()
        {
            var list = new List<object>();

            // Load the CheckDetail enum values
            foreach (var detail in Enum.GetValues(typeof(CheckDetail)).Cast<CheckDetail>())
            {
                list.Add(new { Text = detail.ToString(), Value = (int)detail });
            }

            return list;
        }



        public List<object> ChekdTypeList()
        {
            var list = new List<object>();

            // Load the CheckDetail enum values
            foreach (var detail in Enum.GetValues(typeof(CheckType)).Cast<CheckType>())
            {
                list.Add(new { Text = detail.ToString(), Value = (int)detail });
            }

            return list;
        }



        public List<object> EmployeesDropDownList(int companyId)
        {
            var list = new List<object>();
            var v = _db.Employees.Where(x => x.Active && x.CompanyId == companyId).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name + " (" + x.EmployeeId + ")", Value = x.Id });
            }
            return list;
        }

        public List<object> EmployeesByZoneId(int zoneId)
        {
            var list = new List<object>();
            var v = (from t1 in _db.Employees.Where(x => x.Active)
                     join t2 in _db.OfficerAssigns on t1.Id equals t2.EmpId
                     where t2.ZoneId == zoneId
                     select new
                     {
                         Text = t1.Name + " (" + t1.EmployeeId + ")",
                         Value = t1.Id
                     });
            foreach (var x in v)
            {
                list.Add(new { Text = x.Text, Value = x.Value });
            }
            return list;
        }
        public List<SelectModelType> ZoneDropDownList(int companyId)
        {
            var list = new List<SelectModelType>();
            list = _db.Zones.Where(x => x.IsActive == true && x.CompanyId == companyId && x.ZoneId > 0)
                .Select(s => new SelectModelType
                {
                    Value = s.HeadGLId,
                    Text = s.Name
                }
                ).ToList();
            return list;
        }
        public List<object> CommonThanaDropDownList()
        {
            var list = new List<object>();
            var v = _db.Upazilas.Where(a => a.IsActive == true).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.UpazilaId });
            }
            return list;
        }

        //public string UploadFile(IFormFile file, string folderName, string webRootPath)
        //{
        //    string fName = "";
        //    if (file != null)
        //    {
        //        //string folderName = "Product";
        //        //string webRootPath = _webHostEnvironment.WebRootPath;
        //        string newPath = Path.Combine(webRootPath, folderName);

        //        if (!Directory.Exists(newPath))
        //        {
        //            Directory.CreateDirectory(newPath);
        //        }
        //        if (file.Length > 0)
        //        {
        //            string exten = Path.GetFileName(file.FileName);
        //            fName = Guid.NewGuid() + exten.Substring(exten.IndexOf("."), exten.Length - exten.IndexOf("."));
        //            string sFileExtension = Path.GetExtension(file.FileName).ToLower();
        //            string fullPath = Path.Combine(newPath, fName);
        //            FileStream stream;
        //            using (stream = new FileStream(fullPath, FileMode.OpenOrCreate))
        //            {
        //                file.CopyTo(stream);
        //                stream.Position = 0;
        //            }


        //        }
        //    }
        //    return fName;
        //}

        #region Common Product
        public async Task<VMCommonProduct> GetProduct(int companyId, int categoryId = 0, int subCategoryId = 0, string productType = "")
        {
            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            if ((categoryId == 0 && subCategoryId > 0) || (categoryId > 0 && subCategoryId > 0))
            {
                vmCommonProduct = await (from t1 in _db.ProductSubCategories.Where(x => x.ProductSubCategoryId == subCategoryId && x.IsActive)
                                         join t2 in _db.ProductCategories.Where(x => x.IsActive) on t1.ProductCategoryId equals t2.ProductCategoryId
                                         select new VMCommonProduct
                                         {
                                             Common_ProductSubCategoryFk = t1.ProductSubCategoryId,
                                             Common_ProductCategoryFk = t1.ProductCategoryId,
                                             CategoryName = t2.Name,
                                             SubCategoryName = t1.Name,
                                             CompanyFK = t1.CompanyId,

                                         }).FirstOrDefaultAsync();
            }
            else if (categoryId > 0 && subCategoryId == 0)
            {
                vmCommonProduct = await (from t1 in _db.ProductCategories.Where(x => x.ProductCategoryId == categoryId)
                                         select new VMCommonProduct
                                         {

                                             Common_ProductCategoryFk = t1.ProductCategoryId,
                                             CategoryName = t1.Name,
                                             CompanyFK = t1.CompanyId
                                         }).FirstOrDefaultAsync();
            }
            else
            {
                vmCommonProduct.CompanyFK = companyId;

            }
            //vmCommonProduct.DataList = await (from t1 in _db.Products.Where(x => x.CompanyId == companyId && x.ProductType == productType)
            //                                  join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
            //                                  join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
            //                                  join t4 in _db.Units on t1.UnitId equals t4.UnitId
            //                                  join t5 in _db.Products on t1.PackId equals t5.ProductId into t5_Join
            //                                  from t5 in t5_Join.DefaultIfEmpty()
            //                                  join t6 in _db.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId into t6_Join
            //                                  from t6 in t6_Join.DefaultIfEmpty()


            //                                  where t1.IsActive && t2.IsActive && t3.IsActive &&
            //                                  ((categoryId > 0 && subCategoryId == 0) ? t2.ProductCategoryId == categoryId
            //                                  : (categoryId == 0 && subCategoryId > 0) ? t1.ProductSubCategoryId == subCategoryId
            //                                  : t1.ProductId > 0)
            //                                  select new VMCommonProduct
            //                                  {
            //                                      ID = t1.ProductId,
            //                                      Name = t1.ProductName,
            //                                      ShortName = t1.ShortName,
            //                                      UnitPrice = t1.UnitPrice ?? 0,
            //                                      TPPrice = t1.TPPrice,
            //                                      CreditSalePrice = t1.CreditSalePrice,
            //                                      SubCategoryName = t2.Name,
            //                                      CategoryName = t3.Name,
            //                                      UnitName = t4.Name,
            //                                      ProductType = t1.ProductType,
            //                                      Code = t1.ProductCode,

            //                                      DieSize = t1.DieSize,
            //                                      PackSize = t1.PackSize,
            //                                      ProcessLoss = t1.ProcessLoss,
            //                                      FormulaQty = t1.FormulaQty
            //                                  }).OrderByDescending(x => x.ID).ToListAsync();//.AsEnumerable()
            vmCommonProduct.DataList = await (from t1 in _db.Products
                                              where t1.CompanyId == companyId
                                                    && t1.ProductType == productType
                                                    && t1.IsActive
                                              join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                              where t2.IsActive
                                              join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                              where t3.IsActive
                                              join t4 in _db.Units on t1.UnitId equals t4.UnitId
                                              join t5 in _db.Products on t1.PackId equals t5.ProductId into t5_Join
                                              from t5 in t5_Join.DefaultIfEmpty()
                                              join t6 in _db.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId into t6_Join
                                              from t6 in t6_Join.DefaultIfEmpty()
                                              where (categoryId > 0 && subCategoryId > 0 && t2.ProductCategoryId == categoryId && t2.ProductSubCategoryId == subCategoryId) ||
                                                    (categoryId > 0 && subCategoryId == 0 && t2.ProductCategoryId == categoryId) ||
                                                    (categoryId == 0 && subCategoryId > 0 && t1.ProductSubCategoryId == subCategoryId) ||
                                                    (categoryId == 0 && subCategoryId == 0)
                                              select new VMCommonProduct
                                              {
                                                  ID = t1.ProductId,
                                                  Name = t1.ProductName,
                                                  ShortName = t1.ShortName,
                                                  UnitPrice = t1.UnitPrice ?? 0,
                                                  TPPrice = t1.TPPrice,
                                                  CreditSalePrice = t1.CreditSalePrice,
                                                  SubCategoryName = t2.Name,
                                                  CategoryName = t3.Name,
                                                  UnitName = t4.Name,
                                                  ProductType = t1.ProductType,
                                                  Code = t1.ProductCode,
                                                  DieSize = t1.DieSize,
                                                  PackSize = t1.PackSize,
                                                  ProcessLoss = t1.ProcessLoss,
                                                  FormulaQty = t1.FormulaQty
                                              }).OrderByDescending(x => x.ID).ToListAsync();//.AsEnumerable()

            return vmCommonProduct;
        }


        public async Task<List<Product>> SerCommonRawProductR(int companyId, int categoryId = 0, int subCategoryId = 0)
        {
            // Fetch the products using the LINQ query, including the ProductType filter
            var products = await (from t1 in _db.Products
                                  where t1.CompanyId == companyId
                                         // Use ProductType parameter here
                                        && t1.IsActive
                                  join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                  where t2.IsActive
                                  join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                  where t3.IsActive
                                  join t5 in _db.Products on t1.PackId equals t5.ProductId into t5_Join
                                  from t5 in t5_Join.DefaultIfEmpty()
                                  join t6 in _db.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId into t6_Join
                                  from t6 in t6_Join.DefaultIfEmpty()
                                  where (categoryId > 0 && subCategoryId > 0 && t2.ProductCategoryId == categoryId && t2.ProductSubCategoryId == subCategoryId) ||
                                        (categoryId > 0 && subCategoryId == 0 && t2.ProductCategoryId == categoryId) ||
                                        (categoryId == 0 && subCategoryId > 0 && t1.ProductSubCategoryId == subCategoryId) ||
                                        (categoryId == 0 && subCategoryId == 0)
                                  select t1) // Return the Product entity itself, not an anonymous type
                                  .ToListAsync();

            // Return the list of products
            return products;
        }






        public async Task<VMCommonProduct> kfmalGetProduct(int companyId, int categoryId = 0, int subCategoryId = 0, string productType = "")
        {
            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            if ((categoryId == 0 && subCategoryId > 0) || (categoryId > 0 && subCategoryId > 0))
            {
                vmCommonProduct = await (from t1 in _db.ProductSubCategories.Where(x => x.ProductSubCategoryId == subCategoryId)
                                         join t2 in _db.ProductCategories on t1.ProductCategoryId equals t2.ProductCategoryId
                                         select new VMCommonProduct
                                         {
                                             Common_ProductSubCategoryFk = t1.ProductSubCategoryId,
                                             Common_ProductCategoryFk = t1.ProductCategoryId,
                                             CategoryName = t2.Name,
                                             SubCategoryName = t1.Name,
                                             CompanyFK = t1.CompanyId,

                                         }).FirstOrDefaultAsync();
            }
            else if (categoryId > 0 && subCategoryId == 0)
            {
                vmCommonProduct = await (from t1 in _db.ProductCategories.Where(x => x.ProductCategoryId == categoryId)
                                         select new VMCommonProduct
                                         {
                                             Common_ProductCategoryFk = t1.ProductCategoryId,
                                             CategoryName = t1.Name,
                                             CompanyFK = t1.CompanyId
                                         }).FirstOrDefaultAsync();
            }
            else
            {
                vmCommonProduct.CompanyFK = companyId;
            }
            vmCommonProduct.DataList = await (from t1 in _db.Products.Where(x => x.CompanyId == companyId && x.ProductType == productType)
                                              join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                              join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                              join t4 in _db.Units on t1.UnitId equals t4.UnitId
                                              where t1.IsActive && t2.IsActive && t3.IsActive &&
                                              ((categoryId > 0 && subCategoryId == 0) ? t2.ProductCategoryId == categoryId
                                              : (categoryId == 0 && subCategoryId > 0) ? t1.ProductSubCategoryId == subCategoryId
                                              : t1.ProductId > 0)
                                              select new VMCommonProduct
                                              {
                                                  ID = t1.ProductId,
                                                  Name = t1.ProductName,
                                                  ShortName = t1.ShortName,
                                                  MRPPrice = t1.UnitPrice == null ? 0 : t1.UnitPrice.Value,
                                                  TPPrice = t1.TPPrice,
                                                  CreditSalePrice = t1.CreditSalePrice,
                                                  SubCategoryName = t2.Name,
                                                  CategoryName = t3.Name,
                                                  UnitName = t4.Name,
                                                  ProductType = t1.ProductType,
                                                  Code = t1.ProductCode,
                                                  DieSize = t1.DieSize,
                                                  PackSize = t1.PackSize,
                                                  ProcessLoss = t1.ProcessLoss,
                                                  FormulaQty = t1.FormulaQty
                                              }).OrderByDescending(x => x.ID).ToListAsync();

            return vmCommonProduct;
        }








        public async Task<VMCommonProduct> GetProductByBinSlaveId(int binSlaveID)
        {
            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct.DataList = await Task.Run(() => (from t1 in _db.Products
                                                             join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                                             join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                                             join t4 in _db.Units on t1.UnitId equals t4.UnitId

                                                             select new VMCommonProduct
                                                             {
                                                                 ID = t1.ProductId,
                                                                 Name = t1.ProductName,
                                                                 MRPPrice = t1.UnitPrice.Value,
                                                                 SubCategoryName = t2.Name,
                                                                 CategoryName = t3.Name,
                                                                 UnitName = t4.Name
                                                             }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonProduct;
        }
        #region RealState Product Services
        public async Task<List<SelectModelType>> GetFacingDropDown()
        {
            var list = await _db.FacingInfoes.Where(x => x.IsActive).Select(e => new SelectModelType()
            {
                Text = e.Title,
                Value = e.FacingId
            }).ToListAsync();

            return list;
        }
        public async Task<int> RealStateProductAdd(VMRealStateProduct vmCommonProduct)
        {
            var result = -1;
            #region Genarate Product No





            var JsonData = vmCommonProduct.CompanyFK == (int)CompanyName.GloriousLandsAndDevelopmentsLimited ? JsonConvert.SerializeObject(vmCommonProduct.PlotProp) :
                 vmCommonProduct.CompanyFK == (int)CompanyName.KrishibidPropertiesLimited ? JsonConvert.SerializeObject(vmCommonProduct.FlatProp) : "";
            #endregion

            Product commonProduct = new Product
            {
                Status = vmCommonProduct.Status,
                ProductCode = vmCommonProduct.ProductCode,
                ShortName = vmCommonProduct.ShortName,
                ProductName = vmCommonProduct.Name,
                UnitPrice = vmCommonProduct.MRPPrice,
                TPPrice = vmCommonProduct.TPPrice,
                CreditSalePrice = vmCommonProduct.CreditSalePrice,

                ProductCategoryId = vmCommonProduct.Common_ProductCategoryFk,
                ProductSubCategoryId = vmCommonProduct.Common_ProductSubCategoryFk,
                UnitId = vmCommonProduct.Common_UnitFk,
                Remarks = vmCommonProduct.Remarks,
                CompanyId = vmCommonProduct.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                DieSize = vmCommonProduct.DieSize,
                PackId = vmCommonProduct.PackId,
                PackSize = vmCommonProduct.PackSize,
                ProcessLoss = vmCommonProduct.ProcessLoss,

                IsActive = true,
                ProductType = vmCommonProduct.ProductType,

                OrderNo = 0,
                FacingId = vmCommonProduct.FacingId,
                JsonData = JsonData

            };
            _db.Products.Add(commonProduct);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonProduct.ProductId;
            }
            return result;
        }
        public async Task<int> RealStateProductEdit(VMRealStateProduct vmCommonProduct)
        {
            var result = -1;
            Product commonProduct = _db.Products.Find(vmCommonProduct.ID);

            var JsonData = vmCommonProduct.CompanyFK == (int)CompanyName.GloriousLandsAndDevelopmentsLimited ? JsonConvert.SerializeObject(vmCommonProduct.PlotProp) :
                 vmCommonProduct.CompanyFK == (int)CompanyName.KrishibidPropertiesLimited ? JsonConvert.SerializeObject(vmCommonProduct.FlatProp) : "";
            commonProduct.Status = vmCommonProduct.Status;

            commonProduct.ProductName = vmCommonProduct.Name;
            commonProduct.UnitPrice = vmCommonProduct.MRPPrice;
            commonProduct.TPPrice = vmCommonProduct.TPPrice;
            commonProduct.ShortName = vmCommonProduct.ShortName;
            commonProduct.CreditSalePrice = vmCommonProduct.CreditSalePrice;
            commonProduct.ProductSubCategoryId = vmCommonProduct.Common_ProductSubCategoryFk;
            commonProduct.UnitId = vmCommonProduct.Common_UnitFk;
            commonProduct.CompanyId = vmCommonProduct.CompanyFK.Value;
            commonProduct.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonProduct.ModifiedDate = DateTime.Now;
            commonProduct.Remarks = vmCommonProduct.Remarks;
            commonProduct.DieSize = vmCommonProduct.DieSize;
            commonProduct.PackId = vmCommonProduct.PackId;
            commonProduct.PackSize = vmCommonProduct.PackSize;
            commonProduct.ProcessLoss = vmCommonProduct.ProcessLoss;
            commonProduct.FacingId = vmCommonProduct.FacingId;
            commonProduct.JsonData = JsonData;
            commonProduct.ProductCode = vmCommonProduct.ProductCode;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonProduct.ProductId;
            }
            return result;
        }

        public async Task<VMRealStateProduct> GetRealStateProductForEdit(int productId)
        {
            VMRealStateProduct model = new VMRealStateProduct();
            Product product = await _db.Products.Include(e => e.ProductCategory).SingleOrDefaultAsync(o => o.ProductId == productId);
            if (product != null)
            {
                if (product.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
                {
                    model.PlotProp = JsonConvert.DeserializeObject<PlotProperties>(product.JsonData);
                    model.PlotProp = model.PlotProp == null ? new PlotProperties() : model.PlotProp;
                }
                if (product.CompanyId == (int)CompanyName.KrishibidPropertiesLimited)
                {
                    model.FlatProp = JsonConvert.DeserializeObject<FlatProperties>(product.JsonData);
                    model.FlatProp = model.FlatProp == null ? new FlatProperties() : model.FlatProp;
                }
                model.FacingDropDown = await GetFacingDropDown();
                model.UnitList = new SelectList(this.UnitDropDownList(product.CompanyId.Value), "Value", "Text", product.UnitId);
                model.ProductCategoryList = new SelectList(_db.ProductCategories.Where(e => e.CompanyId.Value == product.CompanyId.Value && e.IsActive)
                    .Select(o => new SelectModelType
                    {
                        Text = o.Name,
                        Value = o.ProductCategoryId
                    }).ToList(), "Value", "Text", product.ProductCategoryId);
                model.ProductSubCategoryList = new SelectList(_db.ProductSubCategories.Where(e => e.ProductCategoryId == product.ProductCategoryId && e.CompanyId.Value == product.CompanyId.Value && e.IsActive)
                    .Select(o => new SelectModelType
                    {
                        Text = o.Name,
                        Value = o.ProductSubCategoryId
                    }).ToList(), "Value", "Text", product.ProductSubCategoryId);
                model.Name = product.ProductName;
                model.MRPPrice = product.UnitPrice.HasValue ? product.UnitPrice.Value : 0;
                model.TPPrice = product.TPPrice;
                model.ShortName = product.ShortName;
                model.CreditSalePrice = product.CreditSalePrice;
                model.Common_ProductSubCategoryFk = product.ProductSubCategoryId;
                model.Common_UnitFk = product.UnitId;
                model.Common_ProductCategoryFk = product.ProductCategoryId;
                model.CategoryName = product.ProductCategory.Name;
                model.CompanyFK = product.CompanyId;
                model.Remarks = product.Remarks;
                model.DieSize = product.DieSize;
                model.PackId = product.PackId;
                model.PackSize = product.PackSize;
                model.ProcessLoss = product.ProcessLoss;
                model.FacingId = product.FacingId;
                model.ProductType = product.ProductType;
                model.ID = product.ProductId;
                model.ProductCode = product.ProductCode;
                model.Status = product.Status;

            }
            return model;
        }
        #endregion RealState Product Services


        public async Task<VMCommonProduct> ProductAdd(VMCommonProduct vmCommonProduct)
        {
            var result = -1;
            var checkDuplicateProduct = await _db.Products.AnyAsync(x => x.ProductCategoryId.Value == vmCommonProduct.Common_ProductCategoryFk.Value && x.ProductSubCategoryId.Value == vmCommonProduct.Common_ProductSubCategoryFk.Value && x.ProductName.ToLower() == vmCommonProduct.Name.ToLower());
            if (!checkDuplicateProduct)
            {
                #region Genarate Product No
                int lsatProduct = _db.Products.Select(x => x.ProductId).OrderByDescending(ID => ID).FirstOrDefault();
                if (lsatProduct == 0)
                {
                    lsatProduct = 1;
                }
                else
                {
                    lsatProduct++;
                }

                var productID = vmCommonProduct.ProductType + lsatProduct.ToString().PadLeft(6, '0');
                #endregion

                Product commonProduct = new Product
                {
                    ProductCode = productID,
                    ShortName = vmCommonProduct.ShortName,
                    ProductName = vmCommonProduct.Name,
                    UnitPrice = vmCommonProduct.UnitPrice,
                    TPPrice = vmCommonProduct.TPPrice,
                    CreditSalePrice = vmCommonProduct.CreditSalePrice,

                    ProductCategoryId = vmCommonProduct.Common_ProductCategoryFk,
                    ProductSubCategoryId = vmCommonProduct.Common_ProductSubCategoryFk,
                    UnitId = vmCommonProduct.Common_UnitFk,
                    Remarks = vmCommonProduct.Remarks,
                    CompanyId = vmCommonProduct.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DieSize = vmCommonProduct.DieSize,
                    PackId = vmCommonProduct.PackId,
                    PackSize = vmCommonProduct.PackSize,
                    ProcessLoss = vmCommonProduct.ProcessLoss,
                    FormulaQty = vmCommonProduct.FormulaQty,

                    IsActive = true,
                    ProductType = vmCommonProduct.ProductType,
                    OrderNo = 0

                };
                _db.Products.Add(commonProduct);
                if (await _db.SaveChangesAsync() > 0)
                {
                    if (commonProduct.CompanyId == (int)CompanyName.KrishibidFeedLimited)
                    {
                        result = commonProduct.ProductId;

                        Product product = _db.Products.Find(commonProduct.ProductId);

                        VMHeadIntegration integration = new VMHeadIntegration
                        {
                            AccName = product.ProductName,
                            LayerNo = 6,
                            Remarks = "6 Layer",
                            IsIncomeHead = false,
                            ProductType = product.ProductType,
                            CompanyFK = commonProduct.CompanyId,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            CreatedDate = DateTime.Now
                        };
                        int headGl = ProductHeadGlPush(integration, commonProduct);

                        //if (headGlId != null)
                        //{
                        //    await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                        //}
                    }
                    //if (commonProduct.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
                    //{
                    //    result = commonProduct.ProductId;

                    //    Product product = _db.Products.Find(commonProduct.ProductId);

                    //    VMHeadIntegration integration = new VMHeadIntegration
                    //    {
                    //        AccName = product.ProductName,
                    //        LayerNo = 6,
                    //        Remarks = "6 Layer",
                    //        IsIncomeHead = false,
                    //        ProductType = product.ProductType,
                    //        CompanyFK = commonProduct.CompanyId,
                    //        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    //        CreatedDate = DateTime.Now
                    //    };


                    //    int headGl = ProductHeadGlPush(integration, commonProduct);

                    //    //if (headGlId != null)
                    //    //{
                    //    //    await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                    //    //}
                    //}

                    result = commonProduct.ProductId;
                }

                vmCommonProduct.Common_ProductFk = result;
                vmCommonProduct.Common_ProductSubCategoryFk = commonProduct.ProductSubCategoryId;
                vmCommonProduct.Common_ProductCategoryFk = commonProduct.ProductCategoryId;
            }
            vmCommonProduct.ErrorStatus = 1;


            return vmCommonProduct;
        }

        public async Task<int> kfmalProductAdd(VMCommonProduct vmCommonProduct)
        {
            var result = -1;
            #region Genarate Product No
            int lsatProduct = _db.Products.Select(x => x.ProductId).OrderByDescending(ID => ID).FirstOrDefault();
            if (lsatProduct == 0)
            {
                lsatProduct = 1;
            }
            else
            {
                lsatProduct++;
            }

            var productID = vmCommonProduct.ProductType + lsatProduct.ToString().PadLeft(6, '0');
            #endregion

            Product commonProduct = new Product
            {
                ProductCode = productID,
                ShortName = vmCommonProduct.ShortName,
                ProductName = vmCommonProduct.Name,
                UnitPrice = vmCommonProduct.MRPPrice,
                TPPrice = vmCommonProduct.TPPrice,
                CreditSalePrice = vmCommonProduct.CreditSalePrice,

                ProductCategoryId = vmCommonProduct.Common_ProductCategoryFk,
                ProductSubCategoryId = vmCommonProduct.Common_ProductSubCategoryFk,
                UnitId = vmCommonProduct.Common_UnitFk,
                Remarks = vmCommonProduct.Remarks,
                CompanyId = vmCommonProduct.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                DieSize = vmCommonProduct.DieSize,
                PackId = vmCommonProduct.PackId,
                PackSize = vmCommonProduct.PackSize,
                ProcessLoss = vmCommonProduct.ProcessLoss,
                FormulaQty = vmCommonProduct.FormulaQty,

                IsActive = true,
                ProductType = vmCommonProduct.ProductType,
                OrderNo = 0,
                HcCode = vmCommonProduct.HcCode,
                Model = vmCommonProduct.Model,
                Color = vmCommonProduct.Color,
                HorsePower = vmCommonProduct.HorsePower,
                NoOfCylinder = vmCommonProduct.NoOfCylinder,
                EngineNo = vmCommonProduct.EngineNo,
                ChassisNo = vmCommonProduct.ChassisNo,
                FuelPumpSlNo = vmCommonProduct.FuelPumpSlNo,
                BatteryNo = vmCommonProduct.BatteryNo,
                ReanType = vmCommonProduct.ReanType



            };
            _db.Products.Add(commonProduct);
            if (await _db.SaveChangesAsync() > 0)
            {
                if (commonProduct.CompanyId == (int)CompanyName.KrishibidFeedLimited)
                {
                    result = commonProduct.ProductId;

                    Product product = _db.Products.Find(commonProduct.ProductId);

                    VMHeadIntegration integration = new VMHeadIntegration
                    {
                        AccName = product.ProductName,
                        LayerNo = 6,
                        Remarks = "6 Layer",
                        IsIncomeHead = false,
                        ProductType = product.ProductType,
                        CompanyFK = commonProduct.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now
                    };


                    int headGl = KFMALProductHeadGlPush(integration, commonProduct);

                    //if (headGlId != null)
                    //{
                    //    await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                    //}
                }
                if (commonProduct.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
                {
                    result = commonProduct.ProductId;

                    Product product = _db.Products.Find(commonProduct.ProductId);

                    VMHeadIntegration integration = new VMHeadIntegration
                    {
                        AccName = product.ProductName,
                        LayerNo = 6,
                        Remarks = "6 Layer",
                        IsIncomeHead = false,
                        ProductType = product.ProductType,
                        CompanyFK = commonProduct.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now
                    };


                    int headGl = KFMALProductHeadGlPush(integration, commonProduct);

                    //if (headGlId != null)
                    //{
                    //    await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                    //}
                }

                result = commonProduct.ProductId;
            }
            return result;
        }
        public async Task<int> kmaLProductEdit(VMCommonProduct vmCommonProduct)
        {
            var result = -1;
            Product commonProduct = _db.Products.Find(vmCommonProduct.ID);

            commonProduct.ProductName = vmCommonProduct.Name;
            commonProduct.UnitPrice = vmCommonProduct.MRPPrice;
            commonProduct.TPPrice = vmCommonProduct.TPPrice;
            commonProduct.ShortName = vmCommonProduct.ShortName;
            commonProduct.CreditSalePrice = vmCommonProduct.CreditSalePrice;
            commonProduct.ProductSubCategoryId = vmCommonProduct.Common_ProductSubCategoryFk;
            commonProduct.UnitId = vmCommonProduct.Common_UnitFk;
            commonProduct.CompanyId = vmCommonProduct.CompanyFK.Value;
            commonProduct.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonProduct.ModifiedDate = DateTime.Now;
            commonProduct.Remarks = vmCommonProduct.Remarks;
            commonProduct.DieSize = vmCommonProduct.DieSize;
            commonProduct.PackId = vmCommonProduct.PackId;
            commonProduct.PackSize = vmCommonProduct.PackSize;
            commonProduct.ProcessLoss = vmCommonProduct.ProcessLoss;
            commonProduct.HcCode = vmCommonProduct.HcCode;
            commonProduct.Model = vmCommonProduct.Model;
            commonProduct.Color = vmCommonProduct.Color;
            commonProduct.HorsePower = vmCommonProduct.HorsePower;
            commonProduct.NoOfCylinder = vmCommonProduct.NoOfCylinder;
            commonProduct.ChassisNo = vmCommonProduct.ChassisNo;
            commonProduct.EngineNo = vmCommonProduct.EngineNo;
            commonProduct.FuelPumpSlNo = vmCommonProduct.FuelPumpSlNo;
            commonProduct.BatteryNo = vmCommonProduct.BatteryNo;
            commonProduct.ReanType = vmCommonProduct.ReanType;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonProduct.ProductId;
            }
            return result;
        }
        public async Task<VMCommonProduct> ProductEdit(VMCommonProduct vmCommonProduct)
        {
            var result = -1;
            Product commonProduct = _db.Products.Find(vmCommonProduct.ID);

            commonProduct.ProductName = vmCommonProduct.Name;
            commonProduct.UnitPrice = vmCommonProduct.UnitPrice;
            commonProduct.PackSize = vmCommonProduct.PackSize;
            commonProduct.FormulaQty = vmCommonProduct.FormulaQty;
            commonProduct.ProductSubCategoryId = vmCommonProduct.Common_ProductSubCategoryFk;
            commonProduct.UnitId = vmCommonProduct.Common_UnitFk;
            commonProduct.CompanyId = vmCommonProduct.CompanyFK.Value;
            commonProduct.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonProduct.ModifiedDate = DateTime.Now;


            commonProduct.TPPrice = vmCommonProduct.TPPrice;
            commonProduct.ShortName = vmCommonProduct.ShortName;
            commonProduct.CreditSalePrice = vmCommonProduct.CreditSalePrice;

            commonProduct.Remarks = vmCommonProduct.Remarks;
            commonProduct.DieSize = vmCommonProduct.DieSize;
            commonProduct.PackId = vmCommonProduct.PackId;

            commonProduct.ProcessLoss = vmCommonProduct.ProcessLoss;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonProduct.ProductId;
            }
            vmCommonProduct.Common_ProductFk = result;
            vmCommonProduct.Common_ProductSubCategoryFk = commonProduct.ProductSubCategoryId;
            vmCommonProduct.Common_ProductCategoryFk = commonProduct.ProductCategoryId;

            return vmCommonProduct;
        }
        public async Task<VMCommonProduct> ProductDelete(int id)
        {
            int result = -1;
            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            if (id != 0)
            {
                Product commonProduct = _db.Products.Find(id);
                commonProduct.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = commonProduct.ProductId;
                }
                vmCommonProduct.Common_ProductFk = result;
                vmCommonProduct.Common_ProductSubCategoryFk = commonProduct.ProductSubCategoryId;
                vmCommonProduct.Common_ProductCategoryFk = commonProduct.ProductCategoryId;
                vmCommonProduct.CompanyFK = commonProduct.CompanyId;
            }
            return vmCommonProduct;
        }
        public async Task<int> kfmalProductDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                Product commonProduct = _db.Products.Find(id);
                commonProduct.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = commonProduct.ProductId;
                }
            }
            return result;
        }
        public List<object> ProductSubCategoryDropDownList()
        {
            var productSubCategoryList = new List<object>();
            var productSubCategory = _db.ProductSubCategories.Where(a => a.IsActive).ToList();
            foreach (var x in productSubCategory)
            {
                productSubCategoryList.Add(new { Text = x.Name, Value = x.ProductSubCategoryId });
            }
            return productSubCategoryList;
        }
        public List<object> ProductSubCategoryDropDownList(int companyId, int categoryId)
        {
            var productSubCategoryList = new List<object>();
            var productSubCategory = _db.ProductSubCategories.Where(a => a.IsActive && a.CompanyId == companyId && a.ProductCategoryId == categoryId).ToList();
            foreach (var x in productSubCategory)
            {
                productSubCategoryList.Add(new { Text = x.Name, Value = x.ProductSubCategoryId });
            }
            return productSubCategoryList;
        }
        public List<object> UnitDropDownList(int companyId)
        {
            var UnitList = new List<object>();
            var Units = _db.Units.Where(a => a.IsActive && a.CompanyId == companyId).ToList();
            foreach (var x in Units)
            {
                UnitList.Add(new { Text = x.Name, Value = x.UnitId });
            }
            return UnitList;
        }

        public List<object> PlotOrPlatStatusList(int companyId)
        {
            var UnitList = new List<object>();


            var Units = _db.DropDownItems.Where(a => a.DropDownTypeId == 62 && a.CompanyId == companyId).ToList();
            foreach (var x in Units)
            {
                UnitList.Add(new { Text = x.Name, Value = x.DropDownItemId });
            }
            return UnitList;
        }
        public VMCommonProduct GetRMUnitAndClosingRateByProductId(int productId,string lotNo)
        {
            var LotNumber = String.IsNullOrEmpty(lotNo) ? "xyzz" : lotNo;

            var products = (from t1 in _db.Products.Where(x => x.ProductId == productId)
                            join t4 in _db.Units on t1.UnitId equals t4.UnitId
                            select new VMCommonProduct
                            {
                                ID = t1.ProductId,
                                Name = t1.ProductName,
                                UnitName = t4.Name,
                                CompanyFK = t1.CompanyId
                            }).FirstOrDefault();
            VMProductStock vMProductStock = new VMProductStock();
            vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetSeedRMStockByProductId {0},{1},{2}", products.ID, products.CompanyFK, LotNumber).FirstOrDefault();
            products.CostingPrice = vMProductStock.ClosingRate;
            products.RemainingStockInQty = vMProductStock.ClosingQty;

            return products;
        }

        public VMCommonProduct GetRMUnitAndClosingRateByProductIdByLot(int companyId,int productId,string lotnumber)
        {
            var products = (from t1 in _db.Products.Where(x => x.ProductId == productId)
                            join t4 in _db.Units on t1.UnitId equals t4.UnitId
                            select new VMCommonProduct
                            {
                                ID = t1.ProductId,
                                Name = t1.ProductName,
                                UnitName = t4.Name,
                                CompanyFK = t1.CompanyId
                            }).FirstOrDefault();
            VMProductStock vMProductStock = new VMProductStock();
            vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetSeedRMStockByProductId {0},{1},{2}", productId, companyId, lotnumber).FirstOrDefault();
            products.CostingPrice = vMProductStock.ClosingRate;
            products.RemainingStockInQty = vMProductStock.ClosingQty;

            return products;
        }
        public VMRealStateProduct GetCommonProductByID(int id)
        {
            var v = (from t1 in _db.Products.Where(x => x.ProductId == id)
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                     join t4 in _db.Units on t1.UnitId equals t4.UnitId

                     select new VMRealStateProduct
                     {
                         ID = t1.ProductId,
                         Name = t1.ProductName,
                         UnitPrice = t1.UnitPrice ?? 0,
                         TPPrice = t1.TPPrice,
                         ShortName = t1.ShortName,
                         SubCategoryName = t2.Name,
                         CategoryName = t3.Name,
                         UnitName = t4.Name,
                         Common_ProductSubCategoryFk = t1.ProductSubCategoryId,
                         Common_UnitFk = t1.UnitId,
                         Common_ProductCategoryFk = t2.ProductCategoryId,
                         CompanyFK = t1.CompanyId,
                         CostingPrice = t1.CostingPrice,
                         PackId = t1.PackId,

                         DieSize = t1.DieSize,
                         PackSize = t1.PackSize,
                         ProcessLoss = t1.ProcessLoss,
                         FormulaQty = t1.FormulaQty,
                         LotNumbers = _db.MaterialReceiveDetails
                 .Where(m => m.ProductId == id && m.LotNumber != null && m.IsActive)
                 .Select(m => m.LotNumber)
                 .Distinct()
                 .ToList()

                     }).FirstOrDefault();
            return v;
        }

        public VMRealStateProduct GetCommonProductByProducId(int id, string LotNo, int CompanyId = 0)
        {
            var v = (from t1 in _db.Products.Where(x => x.ProductId == id)
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                     join t4 in _db.Units on t1.UnitId equals t4.UnitId

                     select new VMRealStateProduct
                     {
                         ID = t1.ProductId,
                         Name = t1.ProductName,
                         UnitPrice = t1.UnitPrice ?? 0,
                         TPPrice = t1.TPPrice,
                         ShortName = t1.ShortName,
                         SubCategoryName = t2.Name,
                         CategoryName = t3.Name,
                         UnitName = t4.Name,
                         Common_ProductSubCategoryFk = t1.ProductSubCategoryId,
                         Common_UnitFk = t1.UnitId,
                         Common_ProductCategoryFk = t2.ProductCategoryId,
                         CompanyFK = t1.CompanyId,
                         CostingPrice = t1.CostingPrice,
                         PackId = t1.PackId,

                         DieSize = t1.DieSize,
                         PackSize = t1.PackSize,
                         ProcessLoss = t1.ProcessLoss,
                         FormulaQty = t1.FormulaQty

                     }).FirstOrDefault();
            if (CompanyId > 0)
            {
                string lotValue = string.IsNullOrEmpty(LotNo) ? "xyzz" : LotNo;
                VMProductStock products = _db.Database.SqlQuery<VMProductStock>("EXEC SeedFinishedGoodsStockByProduct {0},{1},{2}", id, CompanyId, lotValue).FirstOrDefault();
                v.CostingPrice = products.ClosingRate;
                v.CurrentStock = products.ClosingQty;
            }

            return v;
        }

        public VMFinishProductBOM GetFinishProductBOMsByID(int id)
        {
            var v = (from t1 in _db.FinishProductBOMs.Where(x => x.ID == id)
                     join t2 in _db.Products on t1.RProductFK equals t2.ProductId
                     join t4 in _db.Units on t2.UnitId equals t4.UnitId

                     select new VMFinishProductBOM
                     {
                         ID = t1.ID,
                         FCEnumId = t1.CalculationUnit ?? 0,
                         RequiredQuantity = t1.RequiredQuantity,
                         RProductFK = t1.RProductFK,
                         FProductFK = t1.FProductFK,
                         RawProductName = t2.ProductName,
                         UnitName = t4.Name,
                         CompanyFK = t1.CompanyId,
                         LotNumbers=t1.LotNumber

                     }).FirstOrDefault();
            return v;
        }
        public async Task<VMFinishProductBOM> GetCommonProductByID(int companyId, int productId)
        {
            VMFinishProductBOM vmFinishProductBOM = new VMFinishProductBOM();

            vmFinishProductBOM = await Task.Run(() => (from t1 in _db.Products.Where(x => x.CompanyId == companyId && x.ProductId == productId)
                                                       join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                                       join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                                       join t4 in _db.Units on t1.UnitId equals t4.UnitId

                                                       select new VMFinishProductBOM
                                                       {
                                                           FProductFK = t1.ProductId,

                                                           Name = t1.ProductName,
                                                           MRPPrice = t1.UnitPrice.Value,
                                                           TPPrice = t1.TPPrice,
                                                           SubCategoryName = t2.Name,
                                                           CategoryName = t3.Name,
                                                           UnitName = t4.Name,
                                                           Common_ProductSubCategoryFk = t1.ProductSubCategoryId,
                                                           Common_UnitFk = t1.UnitId,
                                                           Common_ProductCategoryFk = t2.ProductCategoryId,
                                                           CompanyFK = t1.CompanyId,
                                                          

                                                       }).FirstOrDefault());

            vmFinishProductBOM.DataListProductBOM = await Task.Run(() => (from t1 in _db.FinishProductBOMs.Where(x => x.CompanyId == companyId && x.FProductFK == productId)
                                                                          join t2 in _db.Products on t1.RProductFK equals t2.ProductId
                                                                          join t3 in _db.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                                                          join t4 in _db.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId
                                                                          join t5 in _db.Units on t2.UnitId equals t5.UnitId
                                                                          where t1.IsActive == true && t2.IsActive == true && t3.IsActive == true && t4.IsActive == true
                                                                          select new VMFinishProductBOM
                                                                          {
                                                                              ID = t1.ID,
                                                                              RProductFK = t1.RProductFK,
                                                                              FProductFK = t1.FProductFK,
                                                                              FCEnumId = t1.CalculationUnit != null ? t1.CalculationUnit.Value : 0,
                                                                              FCEnumName = t1.CalculationUnit != null ? ((FormulaCalculationEnum)t1.CalculationUnit).ToString() : "N/A",
                                                                              CategoryName = t4.Name,
                                                                              SubCategoryName = t3.Name,
                                                                              Name = t2.ProductName,
                                                                              UnitName = t5.Name,
                                                                              RequiredQuantity = t1.RequiredQuantity,
                                                                              //RProcessLoss = t1.RProcessLoss,
                                                                              CompanyId = t1.CompanyId,

                                                                              Common_ProductSubCategoryFk = t2.ProductSubCategoryId,
                                                                              Common_UnitFk = t2.UnitId,
                                                                              Common_ProductCategoryFk = t2.ProductCategoryId,
                                                                              CompanyFK = t1.CompanyId,
                                                                              LotNumbers=t1.LotNumber
                                                                           

                                                                          }).ToListAsync());


            foreach (var item in vmFinishProductBOM.DataListProductBOM)
            {

                VMProductStock vMProductStock = new VMProductStock();
                vMProductStock = _db.Database.SqlQuery<VMProductStock>("EXEC GetSeedRMStockByProductId {0},{1},{2}", item.RProductFK, item.CompanyId, item.LotNumbers ?? "xyzz").FirstOrDefault();
                item.CurrentStock = vMProductStock.ClosingQty;
                //--GetRMUnitAndClosingRateByProductId-
            }


            return vmFinishProductBOM;
        }
        //public VMCommonProduct GetPOReceivingSlaveByID(int receivingId, int productId)
        //{
        //    var v = (from t1 in _db.WareHouse_POReceivingSlave
        //             where t1.WareHouse_POReceivingFk == receivingId && t1.Common_ProductFK == productId
        //             select new VMCommonProduct
        //             {
        //                 WareHouse_POReceivingSlaveFk = t1.ID,
        //                 DamageQuantity = t1.Damage,
        //                 PurchasePrice = t1.PurchasingPrice,
        //                 ReceivedQuantity = t1.ReceivedQuantity,
        //                 TotalPrice = ((t1.ReceivedQuantity - t1.Damage) * t1.PurchasingPrice),
        //                 //PreviousStock need diduct sales quantity
        //                 PreviousStock = _db.WareHouse_POReceivingSlave.Where(x => x.Common_ProductFK == t1.Common_ProductFK && x.ID != t1.ID).Select(x => x.ReceivedQuantity - x.Damage).DefaultIfEmpty(0).Sum()

        //             }).FirstOrDefault();
        //    return v;
        //}
        public VMCommonSupplier GetCommonSupplierByID(int id)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Supplier && x.VendorId == id)
                     join t2 in _db.Countries on t1.CountryId equals t2.CountryId

                     select new VMCommonSupplier
                     {
                         ID = t1.VendorId,
                         Name = t1.Name,
                         Email = t1.Email,
                         Phone = t1.Phone,
                         Country = t2.CountryName,
                         CompanyFK = t1.CompanyId,
                         Common_CountriesFk = t1.CountryId.Value,
                         BranchName = t1.BranchName,
                         ACName = t1.ACName,
                         ACNo = t1.ACNo,
                         BankName = t1.BankName,
                         ContactPerson = t1.ContactName,
                         Address = t1.Address,
                         Code = t1.Code,
                         CreatedBy = t1.CreatedBy,
                         Remarks = t1.Remarks,
                         IsForeign = t1.IsForeign
                     }).FirstOrDefault();
            return v;
        }

        public async Task<int> ProductMRPPriceEdit(VMCommonProduct vmCommonProduct)
        {
            var result = -1;
            Product commonProduct = _db.Products.Find(vmCommonProduct.ID);
            commonProduct.UnitPrice = vmCommonProduct.MRPPrice;

            commonProduct.CompanyId = vmCommonProduct.CompanyFK;
            commonProduct.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonProduct.ModifiedDate = DateTime.Now;
            commonProduct.Remarks = vmCommonProduct.Remarks;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonProduct.ProductId;
            }
            return result;
        }

        #endregion


        #region Finish Product BOM
        public async Task<int> FinishProductBOMAdd(VMFinishProductBOM vmFinishProductBOM)
        {
            var result = -1;
            FinishProductBOM finishProductBOM = new FinishProductBOM
            {
                FProductFK = vmFinishProductBOM.FProductFK,
                RequiredQuantity = vmFinishProductBOM.RequiredQuantity,
                RProductFK = vmFinishProductBOM.RProductFK,
                RProcessLoss = vmFinishProductBOM.RProcessLoss,
                CalculationUnit = vmFinishProductBOM.FCEnumId,
                CompanyId = vmFinishProductBOM.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                LotNumber=vmFinishProductBOM.LotNumbers
                

            };
            _db.FinishProductBOMs.Add(finishProductBOM);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = finishProductBOM.ID;
            }
            return result;
        }

        public async Task<int> FinishProductBOMEdit(VMFinishProductBOM vmFinishProductBOM)
        {
            var result = -1;
            FinishProductBOM finishProductBOM = _db.FinishProductBOMs.Find(vmFinishProductBOM.ID);
            finishProductBOM.FProductFK = vmFinishProductBOM.FProductFK;
            finishProductBOM.RequiredQuantity = vmFinishProductBOM.RequiredQuantity;
            finishProductBOM.RProductFK = vmFinishProductBOM.RProductFK;
            finishProductBOM.RProcessLoss = vmFinishProductBOM.RProcessLoss;
            finishProductBOM.CalculationUnit = vmFinishProductBOM.FCEnumId;
            finishProductBOM.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            finishProductBOM.ModifiedDate = DateTime.Now;
            finishProductBOM.LotNumber = vmFinishProductBOM.LotNumbers;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = finishProductBOM.ID;
            }
            return result;
        }
        public async Task<int> FinishProductBOMDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                FinishProductBOM finishProductBOM = _db.FinishProductBOMs.Find(id);

                finishProductBOM.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = finishProductBOM.ID;
                }
            }
            return result;
        }
        #endregion

        #region Common Customer
        public VMCommonSupplier GetCommonCustomerByIDFeed(int id)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Customer && x.VendorId == id)
                     join t2 in _db.Zones on t1.ZoneId equals t2.ZoneId
                     join t3 in _db.Upazilas on t1.UpazilaId equals t3.UpazilaId
                     join t4 in _db.Districts on t3.DistrictId equals t4.DistrictId

                     select new VMCommonSupplier
                     {
                         ID = t1.VendorId,
                         Name = t1.Name,
                         Email = t1.Email,
                         Phone = t1.Phone,
                         CompanyFK = t1.CompanyId,
                         CustomerTypeFk = t1.CustomerTypeFK,
                         SalesOfficerEmpId = t1.SalesOfficerEmpId,
                         ZoneId = t2.ZoneId,
                         Common_DivisionFk = t4.DivisionId,
                         Common_DistrictsFk = t3.DistrictId,
                         Common_UpazilasFk = t3.UpazilaId,
                         ContactPerson = t1.ContactName,
                         Address = t1.Address,
                         Code = t1.Code,
                         CreatedBy = t1.CreatedBy,
                         Remarks = t1.Remarks,
                         IsForeign = t1.IsForeign,
                         CreditLimit = t1.CreditLimit,
                         NID = t1.NID,
                         SecurityAmount = t1.SecurityAmount,
                         CustomerStatus = t1.CustomerStatus ?? 1,
                         Propietor = t1.Propietor,
                         PaymentType = t1.CustomerType,
                         NomineeName = t1.NomineeName,
                         NomineePhone = t1.NomineePhone,
                         BusinessAddress = t1.BusinessAddress,
                         NomineeNID = t1.NomineeNID,
                         NomineeRelation = t1.NomineeRelation,
                         FixedIncentive = t1.FixedIncentive,
                         FactoryCarrying = t1.FactoryCarrying,
                         DepotCarrying = t1.DepotCarrying,
                         IsIncentiveInInvoice = t1.IsIncentiveInInvoice,

                         IsPoultry = t1.IsPoultry,
                         IsFish = t1.IsFish,
                         IsCattle = t1.IsCattle,
                         CashCommissionPoultry = t1.CashCommissionPoultry,
                         CashCommissionFish = t1.CashCommissionFish,
                         CashCommissionCattle = t1.CashCommissionCattle,
                         CreditRatioFrom = t1.CreditRatioFrom,
                         CreditRatioTo = t1.CreditRatioTo,
                         MonthlyTarget = t1.MonthlyTarget ?? 0,
                         YearlyTarget = t1.YearlyTarget ?? 0,
                         MonthlyIncentive = t1.MonthlyIncentive,
                         YearlyIncentive = t1.YearlyIncentive,
                         FixedCommissionCattle = t1.FixedCommissionCattle,
                         FixedCommissionFish = t1.FixedCommissionFish,
                         FixedCommissionPoultry = t1.FixedCommissionPoultry

                     }).FirstOrDefault();
            return v;
        }
        public VMCommonSupplier GetCommonCustomerByID(int id)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Customer && x.VendorId == id)
                     join t2 in _db.SubZones on t1.SubZoneId equals t2.SubZoneId
                     join t3 in _db.Upazilas on t1.UpazilaId equals t3.UpazilaId
                     join t4 in _db.Districts on t3.DistrictId equals t4.DistrictId

                     select new VMCommonSupplier
                     {
                         ID = t1.VendorId,
                         Name = t1.Name,
                         Email = t1.Email,
                         Phone = t1.Phone,
                         CompanyFK = t1.CompanyId,
                         SubZoneId = t1.SubZoneId.Value,
                         CustomerTypeFk = t1.CustomerTypeFK,
                         ZoneId = t2.ZoneId,
                         Common_DivisionFk = t4.DivisionId,
                         Common_DistrictsFk = t3.DistrictId,
                         Common_UpazilasFk = t3.UpazilaId,
                         ContactPerson = t1.ContactName,
                         Address = t1.Address,
                         Code = t1.Code,
                         CreatedBy = t1.CreatedBy,
                         Remarks = t1.Remarks,
                         IsForeign = t1.IsForeign,
                         CreditLimit = t1.CreditLimit,
                         NID = t1.NID,
                         SecurityAmount = t1.SecurityAmount,
                         CustomerStatus = t1.CustomerStatus ?? 1,
                         Propietor = t1.Propietor,
                         PaymentType = t1.CustomerType,
                         CheckNo = t1.CheckNo,
                         BankName = t1.BankName,
                         BranchName = t1.BranchName,
                         CheckTypeId = t1.CheckTypeId ?? 0,
                         CheckDetailId = t1.CheckDetailId ?? 0,
                         ACName = t1.ACName,
                         ACNo = t1.ACNo


                     }).FirstOrDefault();
            return v;
        }

        public VMCommonSupplier GetCommonCustomerByIDForKSSL(int id)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.VendorTypeId == (int)Provider.Customer && x.VendorId == id)
                     join t3 in _db.Upazilas on t1.UpazilaId equals t3.UpazilaId
                     join t4 in _db.Districts on t3.DistrictId equals t4.DistrictId
                     join t5 in _db.Employees on t1.SalesOfficerEmpId equals t5.Id
                     select new VMCommonSupplier
                     {
                         ID = t1.VendorId,
                         Name = t1.Name,
                         Email = t1.Email,
                         Phone = t1.Phone,
                         CompanyFK = t1.CompanyId,
                         SalesOfficerEmpId = t1.SalesOfficerEmpId,
                         CustomerTypeFk = t1.CustomerTypeFK,
                         SalesOfficerEmpName = t5.Name,
                         Common_DivisionFk = t4.DivisionId,
                         Common_DistrictsFk = t3.DistrictId,
                         Common_UpazilasFk = t3.UpazilaId,
                         ContactPerson = t1.ContactName,
                         Address = t1.Address,
                         Code = t1.Code,
                         CreatedBy = t1.CreatedBy,
                         Remarks = t1.Remarks,
                         IsForeign = t1.IsForeign,
                         CreditLimit = t1.CreditLimit,
                         NID = t1.NID,
                         SecurityAmount = t1.SecurityAmount,
                         CustomerStatus = t1.CustomerStatus ?? 1,
                         Propietor = t1.Propietor,
                         PaymentType = t1.CustomerType

                     }).FirstOrDefault();
            return v;
        }

        public VMCommonSupplier GetRSCustomerByID(int id)
        {
            var v = (from t1 in _db.Vendors.Where(x => x.VendorId == id)
                     join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId into t2_Join
                     from t2 in t2_Join.DefaultIfEmpty()
                     join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId into t3_Join
                     from t3 in t3_Join.DefaultIfEmpty()
                     join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId into t4_Join
                     from t4 in t4_Join.DefaultIfEmpty()

                     select new VMCommonSupplier
                     {
                         ID = t1.VendorId,
                         Name = t1.Name,
                         Email = t1.Email,
                         Phone = t1.Phone,
                         CompanyFK = t1.CompanyId,
                         Common_DistrictsFk = t3 != null ? t3.DistrictId : 0,
                         Common_UpazilasFk = t2 != null ? t2.UpazilaId : 0,
                         Common_DivisionFk = t4 != null ? t4.DivisionId : 0,
                         ImageFileUrl = t1.ImageUrl,
                         ContactPerson = t1.ContactName,
                         Address = t1.Address,
                         Code = t1.Code,
                         CreatedBy = t1.CreatedBy,
                         Remarks = t1.Remarks,
                         VendorReferenceId = t1.VendorReferenceId,
                         NID = t1.NID,
                         ImageDocId = t1.docid,
                         vendorProfessionId = t1.ProfessionId,
                         DateOfBirth = t1.DateOfBirth,
                         MarriageDate = t1.MarriageDate,

                     }).FirstOrDefault();
            return v;
        }
        public async Task<VMCommonSupplier> GetCustomer(int companyId, int zoneId, int SubZoneId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                              join t5 in _db.SubZones on t1.SubZoneId equals t5.SubZoneId
                                                              join t6 in _db.Zones on t5.ZoneId equals t6.ZoneId
                                                              where ((zoneId > 0) && (SubZoneId == 0) ? t6.ZoneId == zoneId :
                                                                     (zoneId > 0) && (SubZoneId > 0) ? t5.SubZoneId == SubZoneId :
                                                              t6.ZoneId > 0)
                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  Common_DistrictsFk = t2.DistrictId,
                                                                  Common_UpazilasFk = t1.UpazilaId.Value,
                                                                  District = t3.Name,
                                                                  Upazila = t2.Name,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone,
                                                                  ZoneName = t6.Name + " " + t5.Name,
                                                                  ZoneIncharge = t6.ZoneIncharge,
                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  CustomerTypeFk = t1.CustomerTypeFK,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  PaymentType = t1.CustomerType,
                                                                  Propietor = t1.Propietor
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> GetPackagingCustomer(int companyId, int zoneId, int SubZoneId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                              //join t5 in _db.SubZones on t1.SubZoneId equals t5.SubZoneId
                                                              //join t6 in _db.Zones on t5.ZoneId equals t6.ZoneId
                                                              //where ((zoneId > 0) && (SubZoneId == 0) ? t6.ZoneId == zoneId :
                                                              //       (zoneId > 0) && (SubZoneId > 0) ? t5.SubZoneId == SubZoneId :
                                                              //t6.ZoneId > 0)
                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  Common_DistrictsFk = t2.DistrictId,
                                                                  Common_UpazilasFk = t1.UpazilaId.Value,
                                                                  District = t3.Name,
                                                                  Upazila = t2.Name,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone,
                                                                  //ZoneName = t6.Name + " " + t5.Name,
                                                                  //ZoneIncharge = t6.ZoneIncharge,
                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  CustomerTypeFk = t1.CustomerTypeFK,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  PaymentType = t1.CustomerType,
                                                                  Propietor = t1.Propietor
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> GetCustomerForKSSL(int companyId, int zoneId, int SubZoneId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from vendor in _db.Vendors
                                                              join upazila in _db.Upazilas on vendor.UpazilaId equals upazila.UpazilaId
                                                              join district in _db.Districts on upazila.DistrictId equals district.DistrictId
                                                              join division in _db.Divisions on district.DivisionId equals division.DivisionId
                                                              join employee in _db.Employees on vendor.SalesOfficerEmpId equals employee.Id
                                                              where vendor.IsActive
                                                              && vendor.VendorTypeId == (int)Provider.Customer
                                                              && vendor.CompanyId == companyId
                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = vendor.VendorId,
                                                                  Name = vendor.Name,
                                                                  Email = vendor.Email,
                                                                  ContactPerson = vendor.ContactName,
                                                                  Address = vendor.Address,
                                                                  Code = vendor.Code,
                                                                  Common_DistrictsFk = upazila.DistrictId,
                                                                  Common_UpazilasFk = vendor.UpazilaId.Value,
                                                                  District = district.Name,
                                                                  Upazila = upazila.Name,
                                                                  Country = division.Name,
                                                                  CreatedBy = vendor.CreatedBy,
                                                                  Remarks = vendor.Remarks,
                                                                  CompanyFK = vendor.CompanyId,
                                                                  Phone = vendor.Phone,
                                                                  CreditLimit = vendor.CreditLimit,
                                                                  NID = vendor.NID,
                                                                  CustomerTypeFk = vendor.CustomerTypeFK,
                                                                  SecurityAmount = vendor.SecurityAmount,
                                                                  CustomerStatus = vendor.CustomerStatus ?? 1,
                                                                  PaymentType = vendor.CustomerType,
                                                                  Propietor = vendor.Propietor,
                                                                  SalesOfficerEmpId = vendor.SalesOfficerEmpId,
                                                                  SalesOfficerEmpName = employee.Name
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> RSGetCustomer(int companyId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId into t2_Join
                                                              from t2 in t2_Join.DefaultIfEmpty()
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId into t3_Join
                                                              from t3 in t3_Join.DefaultIfEmpty()
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId into t4_Join
                                                              from t4 in t4_Join.DefaultIfEmpty()

                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  District = t3.Name,
                                                                  BusinessAddress = t1.BusinessAddress,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone,

                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  CustomerTypeFk = t1.CustomerTypeFK,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  PaymentType = t1.CustomerType,
                                                                  Propietor = t1.Propietor,
                                                                  ImageDocId = t1.docid
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> RSGetCustomerBooking(int companyId, int vendorId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.CustomerGroupInfoes
                                                              join t2 in _db.Vendors on t1.PrimaryClientId equals t2.VendorId
                                                              join t3 in _db.HeadGLs on t1.HeadGLId equals t3.Id
                                                              join t4 in _db.ProductBookingInfoes on t1.CGId equals t4.CGId
                                                              join t5 in _db.Products on t4.ProductId equals t5.ProductId
                                                              join t6 in _db.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                                              join t7 in _db.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                                                              where t1.PrimaryClientId == vendorId && t1.CompanyId == companyId
                                                              select new VMCommonSupplier
                                                              {
                                                                  CompanyFK = t1.CompanyId,
                                                                  CGId = t1.CGId,
                                                                  ID = t1.PrimaryClientId.Value,
                                                                  Name = t1.GroupName,
                                                                  ACName = t3.AccName,
                                                                  Code = t3.AccCode,
                                                                  FileNo = t4.FileNo,
                                                                  BookingNo = t4.BookingNo,
                                                                  ProductName = t7.Name + " " + t6.Name + " " + t5.ProductName
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }
        public async Task<VMCommonSupplier> RSGetCustomerBookingProductCategories(int companyId, int ProductSubCategoryId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t4 in _db.ProductBookingInfoes.Where(f => f.IsActive == true)
                                                              join t5 in _db.Products on t4.ProductId equals t5.ProductId
                                                              join t6 in _db.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                                              join t7 in _db.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                                                              join t1 in _db.CustomerGroupInfoes on t4.CGId equals t1.CGId
                                                              join t2 in _db.Vendors on t1.PrimaryClientId equals t2.VendorId
                                                              join t3 in _db.HeadGLs on t1.HeadGLId equals t3.Id
                                                              where t6.ProductSubCategoryId == ProductSubCategoryId && t1.CompanyId == companyId && t1.IsActive == true
                                                              select new VMCommonSupplier
                                                              {
                                                                  CompanyFK = t1.CompanyId,
                                                                  CGId = t1.CGId,
                                                                  ID = t1.PrimaryClientId.Value,
                                                                  Name = t1.GroupName,
                                                                  ACName = t3.AccName,
                                                                  Code = t3.AccCode,
                                                                  FileNo = t4.FileNo,
                                                                  BookingNo = t4.BookingNo,
                                                                  ProductName = t7.Name + " " + t6.Name + " " + t5.ProductName
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }
        public async Task<VMCommonSupplier> RSGetCustomerGroup(int companyId, int vendorId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.VendorReferenceId = vendorId;

            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId into t2_Join
                                                              from t2 in t2_Join.DefaultIfEmpty()
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId into t3_Join
                                                              from t3 in t3_Join.DefaultIfEmpty()
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId into t4_Join
                                                              from t4 in t4_Join.DefaultIfEmpty()
                                                              where t1.CompanyId == companyId && t1.VendorReferenceId == vendorId && t1.IsActive == true
                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  District = t3.Name,
                                                                  BusinessAddress = t1.BusinessAddress,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone,
                                                                  ImageFileUrl = t1.ImageUrl,
                                                                  ImageDocId = t1.docid,
                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  CustomerTypeFk = t1.CustomerTypeFK,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  PaymentType = t1.CustomerType,
                                                                  Propietor = t1.Propietor
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }
        public async Task<VMCommonSupplier> GetClient(int companyId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                              join t5 in _db.SubZones on t1.SubZoneId equals t5.SubZoneId
                                                              join t6 in _db.Zones on t5.ZoneId equals t6.ZoneId

                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  Common_DistrictsFk = t2.DistrictId,
                                                                  Common_UpazilasFk = t1.UpazilaId.Value,
                                                                  District = t3.Name,
                                                                  Upazila = t2.Name,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone,
                                                                  ZoneName = t6.Name + " " + t5.Name,
                                                                  ZoneIncharge = t6.ZoneIncharge,
                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  CustomerTypeFk = t1.CustomerTypeFK,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  Propietor = t1.Propietor
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> GetFeedCustomer(int companyId, int zoneId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.CompanyName = _db.Companies.Find(companyId).Name;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                              join t6 in _db.Zones on t1.ZoneId equals t6.ZoneId
                                                              join t7 in _db.HeadGLs on t1.HeadGLId equals t7.Id
                                                              join t8 in _db.Head5 on t7.ParentId equals t8.Id
                                                              join t9 in _db.Head4 on t8.ParentId equals t9.Id
                                                              join t10 in _db.VendorDeeds.Where(x => x.IsActive) on t1.VendorId equals t10.VendorId into t10_Join
                                                              from t10 in t10_Join.DefaultIfEmpty()


                                                              where (zoneId > 0 ? t6.ZoneId == zoneId : zoneId == 0)

                                                              select new VMCommonSupplier
                                                              {
                                                                  VendorDeedId = t10 != null ? t10.VendorDeedId : 0,
                                                                  Common_CountriesFk = t8.Id,
                                                                  NomineeName = t8.AccName + " " + t9.AccName,
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  Common_DistrictsFk = t2.DistrictId,
                                                                  Common_UpazilasFk = t1.UpazilaId.Value,
                                                                  District = t3.Name,
                                                                  Upazila = t2.Name,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone,
                                                                  ZoneName = t6.Name,
                                                                  ZoneIncharge = t6.ZoneIncharge,
                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  CustomerTypeFk = t1.CustomerTypeFK,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  Propietor = t1.Propietor,
                                                                  HeadGLId = t1.HeadGLId,
                                                                  VendorTypeId = t1.VendorTypeId,
                                                                  ACName = t1.ACName,
                                                                  ACNo = t1.ACNo,
                                                                  BankName = t1.BankName,
                                                                  BranchName = t1.BranchName,
                                                                  YearlyIncentive = t1.YearlyIncentive,
                                                                  MonthlyIncentive = t1.MonthlyIncentive,
                                                                  YearlyTarget = t1.YearlyTarget,
                                                                  MonthlyTarget = t1.MonthlyTarget
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> GetKfmalCustomer(int companyId, int zoneId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CompanyFK = companyId;
            vmCommonCustomer.CompanyName = _db.Companies.Find(companyId).Name;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                                              join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                              join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                              join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                              join t6 in _db.SubZones on t1.SubZoneId equals t6.SubZoneId
                                                              join t0 in _db.Zones on t6.ZoneId equals t0.ZoneId


                                                              where (zoneId > 0 ? t6.ZoneId == zoneId : zoneId == 0)

                                                              select new VMCommonSupplier
                                                              {
                                                                  ID = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactPerson = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  Common_DistrictsFk = t2.DistrictId,
                                                                  Common_UpazilasFk = t1.UpazilaId.Value,
                                                                  District = t3.Name,
                                                                  Upazila = t2.Name,
                                                                  Country = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyFK = t1.CompanyId,
                                                                  Phone = t1.Phone,
                                                                  ZoneName = t6.Name,
                                                                  ZoneIncharge = t0.ZoneIncharge,
                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  CustomerTypeFk = t1.CustomerTypeFK,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  Propietor = t1.Propietor,
                                                                  HeadGLId = t1.HeadGLId,
                                                                  VendorTypeId = t1.VendorTypeId,
                                                                  ACName = t1.ACName,
                                                                  ACNo = t1.ACNo,
                                                                  BankName = t1.BankName,
                                                                  BranchName = t1.BranchName,
                                                                  YearlyIncentive = t1.YearlyIncentive,
                                                                  MonthlyIncentive = t1.MonthlyIncentive,
                                                                  YearlyTarget = t1.YearlyTarget,
                                                                  MonthlyTarget = t1.MonthlyTarget
                                                              }).OrderByDescending(x => x.ID).AsEnumerable());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> GetCustomerByID(int customerId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.VendorId == customerId)
                                                     join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                     join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                     join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                     //join t5 in _db.SubZones on t1.SubZoneId equals t5.SubZoneId
                                                     join t6 in _db.Zones on t1.ZoneId equals t6.ZoneId
                                                     select new VMCommonSupplier
                                                     {
                                                         ID = t1.VendorId,
                                                         Name = t1.Name,
                                                         Email = t1.Email,
                                                         ContactPerson = t1.ContactName,
                                                         Address = t1.Address,
                                                         Code = t1.Code,
                                                         Common_DistrictsFk = t2.DistrictId,
                                                         Common_UpazilasFk = t1.UpazilaId.Value,
                                                         District = t3.Name,
                                                         Upazila = t2.Name,
                                                         Country = t4.Name,
                                                         CreatedBy = t1.CreatedBy,
                                                         Division = t4.Name,
                                                         Remarks = t1.Remarks,
                                                         CompanyFK = t1.CompanyId,
                                                         Phone = t1.Phone,
                                                         ZoneName = t1.Name,
                                                         ZoneIncharge = t6.ZoneIncharge,
                                                         CreditLimit = t1.CreditLimit,
                                                         NID = t1.NID,
                                                         CustomerTypeFk = t1.CustomerTypeFK,
                                                         VendorTypeId = t1.VendorTypeId
                                                     }).FirstOrDefault());



            return vmCommonCustomer;
        }

        public async Task<VMCommonSupplier> GetCustomerBuID(int customerId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.VendorId == customerId)
                                                     join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                     join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                     join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                     join t5 in _db.SubZones on t1.SubZoneId equals t5.SubZoneId
                                                     join t6 in _db.Zones on t5.ZoneId equals t6.ZoneId
                                                     select new VMCommonSupplier
                                                     {
                                                         ID = t1.VendorId,
                                                         Name = t1.Name,
                                                         Email = t1.Email,
                                                         ContactPerson = t1.ContactName,
                                                         Address = t1.Address,
                                                         Code = t1.Code,
                                                         Common_DistrictsFk = t2.DistrictId,
                                                         Common_UpazilasFk = t1.UpazilaId.Value,
                                                         District = t3.Name,
                                                         Upazila = t2.Name,
                                                         Country = t4.Name,
                                                         CreatedBy = t1.CreatedBy,
                                                         Division = t4.Name,
                                                         Remarks = t1.Remarks,
                                                         CompanyFK = t1.CompanyId,
                                                         Phone = t1.Phone,
                                                         ZoneName = t6.Name,
                                                         ZoneIncharge = t6.ZoneIncharge,
                                                         CreditLimit = t1.CreditLimit,
                                                         NID = t1.NID,
                                                         CustomerTypeFk = t1.CustomerTypeFK,
                                                         VendorTypeId = t1.VendorTypeId,
                                                         ACName = t1.ACName,
                                                         BankName = t1.BankName,
                                                         BranchName = t1.BranchName,
                                                         Imageurl = t1.ImageUrl,
                                                         ACNo = t1.ACNo,
                                                         CheckNo = t1.CheckNo,
                                                         CheckDetailId = (int)(CheckDetail)t1.CheckDetailId,
                                                         CheckTypeId = (int)(CheckType)t1.CheckTypeId,
                                                         TradeLicenceUrl = t1.TradeLicenseImageUrl,
                                                         SaleLiUrl = t1.WLImageUrl,
                                                         DelerLiUrl = t1.SDLImageUrl,
                                                         TinUrl = t1.TINImageUrl,
                                                         BankChkUrl = t1.CheckImageUrl,
                                                         SubZoneName = t5.Name

                                                     }).FirstOrDefault());



            return vmCommonCustomer;
        }
        public async Task<VMCommonSupplier> GetCustomerBuID2(int customerId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => (from t1 in _db.Vendors.Where(x => x.IsActive && x.VendorId == customerId)
                                                     join t2 in _db.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                     join t3 in _db.Districts on t2.DistrictId equals t3.DistrictId
                                                     join t4 in _db.Divisions on t3.DivisionId equals t4.DivisionId
                                                     join t6 in _db.Zones on t1.ZoneId equals t6.ZoneId
                                                     select new VMCommonSupplier
                                                     {

                                                         ID = t1.VendorId,
                                                         Name = t1.Name,
                                                         Email = t1.Email,
                                                         ContactPerson = t1.ContactName,
                                                         Address = t1.Address,
                                                         Code = t1.Code,
                                                         District = t3.Name,
                                                         Upazila = t2.Name,
                                                         CreatedBy = t1.CreatedBy,
                                                         Division = t4.Name,
                                                         Remarks = t1.Remarks,
                                                         CompanyFK = t1.CompanyId,
                                                         Phone = t1.Phone,
                                                         ZoneName = t6.Name,
                                                         CreditLimit = t1.CreditLimit,
                                                         NID = t1.NID,
                                                         CustomerTypeFk = t1.CustomerTypeFK,
                                                         VendorTypeId = t1.VendorTypeId,
                                                         ImageDocId = t1.docid,
                                                         NomineeName = t1.NomineeName,
                                                         NomineeNID = t1.NomineeNID,
                                                         NomineePhone = t1.NomineePhone,
                                                         NomineeRelation = t1.NomineeRelation,
                                                         GuarantorName = t1.GuarantorName,
                                                         GurantorAddress = t1.GurantorAddress,
                                                         GurantorMobileNo = t1.GurantorMobileNo,
                                                         CheckNo = t1.CheckNo,
                                                         MonthlyIncentive = t1.MonthlyIncentive,
                                                         MonthlyTarget = t1.MonthlyTarget ?? 0,
                                                         ClosingTime = t1.ClosingTime,
                                                         YearlyIncentive = t1.YearlyIncentive,
                                                         YearlyTarget = t1.YearlyTarget ?? 0,
                                                         SecurityAmount = t1.SecurityAmount,
                                                         CreditRatioTo = t1.CreditRatioTo,
                                                         CreditRatioFrom = t1.CreditRatioFrom,
                                                         CreditCommission = t1.CreditCommission,
                                                         Condition = t1.Condition,
                                                         NoOfCheck = t1.NoOfCheck,
                                                         PaymentType = t1.CustomerType,
                                                         Propietor = t1.Propietor





                                                     }).FirstOrDefault());


            vmCommonCustomer.Attachments = await Task.Run(() => (from t1 in _db.FileArchives.Where(x => x.FileCatagoryId == 5)
                                                                 join t2 in _db.CustomerBookingFileMappings on t1.docid equals t2.DocId
                                                                 where t2.CGId == (long)customerId
                                                                 select new GLDLBookingAttachment
                                                                 {
                                                                     DocId = t2.DocId,
                                                                     Title = t2.FileTitel,
                                                                     Extension = t1.fileext

                                                                 }).ToListAsync());



            return vmCommonCustomer;
        }
        public async Task<int> CustomerAdd(VMCommonSupplier vmCommonCustomer)
        {

            var result = -1;
            #region Genarate Supplier code
            int totalSupplier = _db.Vendors
                .Where(x => x.VendorTypeId == (int)Provider.Customer && x.CompanyId == vmCommonCustomer.CompanyFK).Count();


            if (totalSupplier == 0)
            {
                totalSupplier = 1;
            }
            else
            {
                totalSupplier++;
            }

            var newString = totalSupplier.ToString().PadLeft(4, '0');
            #endregion
            Vendor commonCustomer = new Vendor
            {
                Name = vmCommonCustomer.Name,
                Phone = vmCommonCustomer.Phone,
                Email = vmCommonCustomer.Email,
                UpazilaId = vmCommonCustomer.Common_UpazilasFk,
                ContactName = vmCommonCustomer.ContactPerson,
                VendorTypeId = (int)Provider.Customer,
                Address = vmCommonCustomer.Address,
                SubZoneId = vmCommonCustomer.SubZoneId,
                NID = vmCommonCustomer.NID,
                CreditLimit = vmCommonCustomer.CreditLimit,
                CustomerTypeFK = vmCommonCustomer.CustomerTypeFk,
                CompanyId = vmCommonCustomer.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                SecurityAmount = vmCommonCustomer.SecurityAmount,
                CustomerStatus = vmCommonCustomer.CustomerStatus,
                Propietor = vmCommonCustomer.Propietor,
                CustomerType = vmCommonCustomer.PaymentType,
                IsActive = true,
                DepotCarrying = vmCommonCustomer.DepotCarrying,
                IsIncentiveInInvoice = vmCommonCustomer.IsIncentiveInInvoice,
                FactoryCarrying = vmCommonCustomer.FactoryCarrying,
                FixedIncentive = vmCommonCustomer.FixedIncentive,

                SalesOfficerEmpId = vmCommonCustomer.SalesOfficerEmpId,
                IsForeign = false,
                NomineeName = vmCommonCustomer.NomineeName,
                NomineePhone = vmCommonCustomer.NomineePhone,
                ZoneId = vmCommonCustomer.ZoneId,
                NomineeRelation = vmCommonCustomer.NomineeRelation,
                NomineeNID = vmCommonCustomer.NomineeNID,
                BusinessAddress = vmCommonCustomer.BusinessAddress,
                docid = vmCommonCustomer.ImageDocId,
                YearlyTarget = vmCommonCustomer.YearlyTarget,
                CreditRatioFrom = vmCommonCustomer.CreditRatioFrom,
                MonthlyTarget = vmCommonCustomer.MonthlyTarget,
                CreditRatioTo = vmCommonCustomer.CreditRatioTo,
                NoOfCheck = vmCommonCustomer.NoOfCheck,
                CheckNo = vmCommonCustomer.CheckNo,
                ClosingTime = vmCommonCustomer.ClosingTime,
                GuarantorName = vmCommonCustomer.GuarantorName,
                GurantorAddress = vmCommonCustomer.GurantorAddress,
                GurantorMobileNo = vmCommonCustomer.GurantorMobileNo,
                IsPoultry = vmCommonCustomer.IsPoultry,
                IsFish = vmCommonCustomer.IsFish,
                IsCattle = vmCommonCustomer.IsCattle,
                CashCommissionPoultry = vmCommonCustomer.CashCommissionPoultry,
                CashCommissionFish = vmCommonCustomer.CashCommissionFish,
                CashCommissionCattle = vmCommonCustomer.CashCommissionCattle,
                MonthlyIncentive = vmCommonCustomer.MonthlyIncentive,
                YearlyIncentive = vmCommonCustomer.YearlyIncentive,
                FixedCommissionCattle = vmCommonCustomer.FixedCommissionCattle,
                FixedCommissionFish = vmCommonCustomer.FixedCommissionFish,
                FixedCommissionPoultry = vmCommonCustomer.FixedCommissionPoultry,
                NIDImageUrl = vmCommonCustomer.NidImage,
                ImageUrl = vmCommonCustomer.Imageurl,
                TradeLicenseImageUrl = vmCommonCustomer.TradeLicenceUrl,
                BSAMemberImageUrl = vmCommonCustomer.BSAMemUrl,
                WLImageUrl = vmCommonCustomer.SaleLiUrl,
                SDLImageUrl = vmCommonCustomer.DelerLiUrl,
                TINImageUrl = vmCommonCustomer.TinUrl,
                CheckImageUrl = vmCommonCustomer.BankChkUrl,
                CheckDetailId = vmCommonCustomer.CheckDetailId,
                CheckTypeId = vmCommonCustomer.CheckTypeId,
                ACName = vmCommonCustomer.ACName,
                ACNo = vmCommonCustomer.ACNo,
                BankName = vmCommonCustomer.BankName,
                BranchName = vmCommonCustomer.BranchName

            };
            _db.Vendors.Add(commonCustomer);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;
                //Accounts Receivable Seed Head4 Id = 38116
                int ParentId = 0;

                var subZones = _db.SubZones.Find(commonCustomer.SubZoneId);
                ParentId = subZones.AccountHeadId;

                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = commonCustomer.Name,
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ParentId = ParentId,
                    IsActive = true,

                    CompanyFK = commonCustomer.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };

                HeadGL headGlId = await PayableHeadIntegrationAdd(integration);

                if (headGlId != null)
                {
                    await VendorsCodeAndHeadGLIdEdit(commonCustomer.VendorId, headGlId);
                }




            }
            return result;
        }


        public async Task<int> CustomerKSSLAdd(VMCommonSupplier vmCommonCustomer)
        {

            var result = -1;
            #region Genarate Supplier code
            int totalSupplier = _db.Vendors
                .Where(x => x.VendorTypeId == (int)Provider.Customer && x.CompanyId == vmCommonCustomer.CompanyFK).Count();


            if (totalSupplier == 0)
            {
                totalSupplier = 1;
            }
            else
            {
                totalSupplier++;
            }

            var newString = totalSupplier.ToString().PadLeft(4, '0');
            #endregion
            Vendor commonCustomer = new Vendor
            {
                Name = vmCommonCustomer.Name,
                Phone = vmCommonCustomer.Phone,
                Email = vmCommonCustomer.Email,
                UpazilaId = vmCommonCustomer.Common_UpazilasFk,
                ContactName = vmCommonCustomer.ContactPerson,
                VendorTypeId = (int)Provider.Customer,
                Address = vmCommonCustomer.Address,
                SubZoneId = vmCommonCustomer.SubZoneId,
                NID = vmCommonCustomer.NID,
                CreditLimit = vmCommonCustomer.CreditLimit,
                CustomerTypeFK = vmCommonCustomer.CustomerTypeFk,
                CompanyId = vmCommonCustomer.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                SecurityAmount = vmCommonCustomer.SecurityAmount,
                CustomerStatus = vmCommonCustomer.CustomerStatus,
                Propietor = vmCommonCustomer.Propietor,
                CustomerType = vmCommonCustomer.PaymentType,
                IsActive = true,
                DepotCarrying = vmCommonCustomer.DepotCarrying,
                IsIncentiveInInvoice = vmCommonCustomer.IsIncentiveInInvoice,
                FactoryCarrying = vmCommonCustomer.FactoryCarrying,
                FixedIncentive = vmCommonCustomer.FixedIncentive,

                SalesOfficerEmpId = vmCommonCustomer.SalesOfficerEmpId,

                BusinessAddress = vmCommonCustomer.BusinessAddress,
                docid = vmCommonCustomer.ImageDocId,

            };
            _db.Vendors.Add(commonCustomer);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;



                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = commonCustomer.Name,
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ParentId = 45510,
                    IsActive = true,

                    CompanyFK = commonCustomer.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                HeadGL headGlId = await PayableHeadIntegrationAdd(integration);

                if (headGlId != null)
                {
                    await VendorsCodeAndHeadGLIdEdit(commonCustomer.VendorId, headGlId);
                }
            }
            return result;
        }


        public async Task<int> PackagingCustomerAdd(VMCommonSupplier vmCommonCustomer)
        {

            var result = -1;

            Vendor commonCustomer = new Vendor
            {
                Name = vmCommonCustomer.Name,
                Phone = vmCommonCustomer.Phone,
                Email = vmCommonCustomer.Email,
                UpazilaId = vmCommonCustomer.Common_UpazilasFk,
                ContactName = vmCommonCustomer.ContactPerson,
                VendorTypeId = (int)Provider.Customer,
                Address = vmCommonCustomer.Address,

                NID = vmCommonCustomer.NID,
                CreditLimit = vmCommonCustomer.CreditLimit,
                CustomerTypeFK = vmCommonCustomer.CustomerTypeFk,
                CompanyId = vmCommonCustomer.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                SecurityAmount = vmCommonCustomer.SecurityAmount,
                CustomerStatus = vmCommonCustomer.CustomerStatus,
                Propietor = vmCommonCustomer.Propietor,
                CustomerType = vmCommonCustomer.PaymentType,
                IsActive = true,


                SalesOfficerEmpId = vmCommonCustomer.SalesOfficerEmpId,
                IsForeign = false,
                NomineeName = vmCommonCustomer.NomineeName,
                NomineePhone = vmCommonCustomer.NomineePhone,
                NomineeRelation = vmCommonCustomer.NomineeRelation,
                NomineeNID = vmCommonCustomer.NomineeNID,
                BusinessAddress = vmCommonCustomer.BusinessAddress,
                docid = vmCommonCustomer.ImageDocId,
                YearlyTarget = vmCommonCustomer.YearlyTarget,
                CreditRatioFrom = vmCommonCustomer.CreditRatioFrom,
                MonthlyTarget = vmCommonCustomer.MonthlyTarget,
                CreditRatioTo = vmCommonCustomer.CreditRatioTo,
                NoOfCheck = vmCommonCustomer.NoOfCheck,
                CheckNo = vmCommonCustomer.CheckNo,
                ClosingTime = vmCommonCustomer.ClosingTime,
                GuarantorName = vmCommonCustomer.GuarantorName,
                GurantorAddress = vmCommonCustomer.GurantorAddress,
                GurantorMobileNo = vmCommonCustomer.GurantorMobileNo


            };
            _db.Vendors.Add(commonCustomer);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;
                int ParentId = 40481;

                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = commonCustomer.Name,
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ParentId = ParentId,
                    IsActive = true,
                    CompanyFK = commonCustomer.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                HeadGL headGlId = await PayableHeadIntegrationAdd(integration);
                if (headGlId != null)
                {
                    await VendorsCodeAndHeadGLIdEdit(commonCustomer.VendorId, headGlId);
                }

            }
            return result;
        }



        public async Task<bool> SetCustomerCommission(Vendor vendor)
        {






            List<VendorOffer> vendorOffers = await _db.VendorOffers.Where(x => x.VendorId == vendor.VendorId).ToListAsync();
            vendorOffers.ForEach(x =>
            {
                x.MonthlyIncentive = vendor.MonthlyIncentive;
                x.YearlyIncentive = vendor.YearlyIncentive;
                x.FactoryCarryingCommission = vendor.FactoryCarrying;
                x.CarryingCommission = vendor.DepotCarrying;
            });
            await _db.SaveChangesAsync();

            var productCategories = new int[] { 1, 2, 3 };

            foreach (var categoryId in productCategories)
            {
                var productIds = _db.Products
                    .Where(x => x.ProductCategoryId == categoryId && x.CompanyId == 8 && x.IsActive)
                    .Select(x => x.ProductId)
                    .AsEnumerable();

                var vendorOffersCashComm = await _db.VendorOffers
                    .Where(x => x.VendorId == vendor.VendorId && productIds.Contains(x.ProductId))
                    .ToListAsync();

                switch (categoryId)
                {
                    case 1:
                        vendorOffersCashComm.ForEach(x => x.CashCommission = vendor.CashCommissionCattle);
                        break;

                    case 2:
                        vendorOffersCashComm.ForEach(x => x.CashCommission = vendor.CashCommissionFish);
                        break;

                    case 3:
                        vendorOffersCashComm.ForEach(x => x.CashCommission = vendor.CashCommissionPoultry);
                        break;
                }
            }




            //var cattleproduct = _db.Products.Where(x => x.ProductCategoryId == 1 && x.IsActive).Select(x => x.ProductId).AsEnumerable();
            //var fishproduct = _db.Products.Where(x => x.ProductCategoryId == 2 && x.IsActive).Select(x => x.ProductId).AsEnumerable();
            //var poultryproduct = _db.Products.Where(x => x.ProductCategoryId == 3 && x.IsActive).Select(x => x.ProductId).AsEnumerable();

            //List<VendorOffer> vendorOffersCattle = await _db.VendorOffers.Where(x => x.VendorId == vendor.VendorId && cattleproduct.Contains(x.ProductId)).ToListAsync();
            //vendorOffersCattle.ForEach(x => x.CashCommission = vendor.CashCommissionCattle);
            //await _db.SaveChangesAsync();

            //List<VendorOffer> vendorOffersFish = await _db.VendorOffers.Where(x => x.VendorId == vendor.VendorId && fishproduct.Contains(x.ProductId)).ToListAsync();
            //vendorOffersFish.ForEach(x => x.CashCommission = vendor.CashCommissionFish);
            //await _db.SaveChangesAsync();

            //List<VendorOffer> vendorOffersPoultry = await _db.VendorOffers.Where(x => x.VendorId == vendor.VendorId && poultryproduct.Contains(x.ProductId)).ToListAsync();
            //vendorOffersPoultry.ForEach(x => x.CashCommission = vendor.CashCommissionPoultry);
            //await _db.SaveChangesAsync();





            return true;
        }

        public async Task<int> KfmalCustomerAdd(VMCommonSupplier vmCommonCustomer)
        {
            var result = -1;

            Vendor commonCustomer = new Vendor
            {
                Name = vmCommonCustomer.Name,
                Propietor = vmCommonCustomer.Propietor,
                ContactName = vmCommonCustomer.ContactPerson,
                Phone = vmCommonCustomer.Phone,
                Email = vmCommonCustomer.Email,
                DistrictId = vmCommonCustomer.Common_DistrictsFk,
                UpazilaId = vmCommonCustomer.Common_UpazilasFk,
                ZoneId = vmCommonCustomer.ZoneId,
                SubZoneId = vmCommonCustomer.SubZoneId,
                CustomerTypeFK = vmCommonCustomer.CustomerTypeFk,
                CustomerStatus = vmCommonCustomer.CustomerProductTypeFk,
                NID = vmCommonCustomer.NID,
                Address = vmCommonCustomer.Address,
                BusinessAddress = vmCommonCustomer.BusinessAddress,
                BIN = vmCommonCustomer.ETinNo,
                Condition = vmCommonCustomer.TradeLicense,
                SecurityAmount = vmCommonCustomer.SecurityAmount,
                CheckNo = vmCommonCustomer.CheckNo,
                GuarantorName = vmCommonCustomer.GuarantorName,
                GurantorMobileNo = vmCommonCustomer.GurantorMobileNo,
                GurantorAddress = vmCommonCustomer.GurantorAddress,
                NomineeName = vmCommonCustomer.NomineeName,
                NomineePhone = vmCommonCustomer.NomineePhone,
                NomineeRelation = vmCommonCustomer.NomineeRelation,
                NomineeNID = vmCommonCustomer.NomineeNID,
                CustomerType = vmCommonCustomer.PaymentType,
                VendorTypeId = (int)Provider.Customer,
                docid = vmCommonCustomer.ImageDocId,


                CompanyId = vmCommonCustomer.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
            };
            _db.Vendors.Add(commonCustomer);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;

                #region Head GL
                var subZones = _db.SubZones.Find(commonCustomer.SubZoneId);
                int ParentId = subZones.AccountHeadId;

                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = commonCustomer.Name,
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    ParentId = ParentId,
                    IsActive = true,
                    OrderNo = 0,
                    CompanyFK = commonCustomer.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                HeadGL headGlId = await KfmalCustomerHeadAdd(integration, commonCustomer.VendorId);

                #endregion


            }
            return result;
        }

        public async Task<int> RSCustomerGroupAdd(VMCommonSupplier vmCommonCustomer)
        {
            var result = -1;
            #region Genarate Supplier code
            int totalSupplier = _db.Vendors
                .Where(x => x.VendorTypeId == (int)Provider.CustomerAssociates && x.CompanyId == vmCommonCustomer.CompanyFK).Count();


            if (totalSupplier == 0)
            {
                totalSupplier = 1;
            }
            else
            {
                totalSupplier++;
            }

            var newString = totalSupplier.ToString().PadLeft(4, '0');
            #endregion
            Vendor commonCustomer = new Vendor
            {
                Name = vmCommonCustomer.Name,
                Phone = vmCommonCustomer.Phone,
                Email = vmCommonCustomer.Email,
                UpazilaId = vmCommonCustomer.Common_UpazilasFk,
                ContactName = vmCommonCustomer.ContactPerson,
                VendorTypeId = (int)Provider.CustomerAssociates,
                Address = vmCommonCustomer.Address,
                SubZoneId = vmCommonCustomer.SubZoneId,
                NID = vmCommonCustomer.NID,
                CreditLimit = vmCommonCustomer.CreditLimit,
                CustomerTypeFK = vmCommonCustomer.CustomerTypeFk,
                CompanyId = vmCommonCustomer.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                SecurityAmount = vmCommonCustomer.SecurityAmount,
                CustomerStatus = vmCommonCustomer.CustomerStatus,
                Propietor = vmCommonCustomer.Propietor,
                CustomerType = vmCommonCustomer.PaymentType,
                IsActive = true,
                CreditRatioFrom = 0,
                CreditRatioTo = 0,
                IsForeign = false,
                NomineeName = vmCommonCustomer.NomineeName,
                NomineePhone = vmCommonCustomer.NomineePhone,
                ZoneId = vmCommonCustomer.ZoneId,
                NomineeRelation = vmCommonCustomer.NomineeRelation,
                NomineeNID = vmCommonCustomer.NomineeNID,
                BusinessAddress = vmCommonCustomer.BusinessAddress,
                VendorReferenceId = vmCommonCustomer.VendorReferenceId,
                docid = vmCommonCustomer.ImageDocId
            };
            _db.Vendors.Add(commonCustomer);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;
            }
            return result;
        }



        public async Task<int> RSCustomerGroupEdit(VMCommonSupplier vmCommonCustomer)
        {
            var result = -1;
            Vendor commonCustomer = _db.Vendors.Find(vmCommonCustomer.ID);
            commonCustomer.Name = vmCommonCustomer.Name;
            commonCustomer.UpazilaId = vmCommonCustomer.Common_UpazilasFk;
            commonCustomer.DistrictId = vmCommonCustomer.Common_DistrictsFk;
            commonCustomer.Address = vmCommonCustomer.Address;
            commonCustomer.Phone = vmCommonCustomer.Phone;
            commonCustomer.SubZoneId = vmCommonCustomer.SubZoneId;
            commonCustomer.NID = vmCommonCustomer.NID;
            commonCustomer.CreditLimit = vmCommonCustomer.CreditLimit;
            commonCustomer.Email = vmCommonCustomer.Email;
            commonCustomer.Remarks = vmCommonCustomer.Remarks;
            commonCustomer.CompanyId = vmCommonCustomer.CompanyFK.Value;
            commonCustomer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonCustomer.ModifiedDate = DateTime.Now;
            commonCustomer.ContactName = vmCommonCustomer.ContactPerson;
            commonCustomer.CustomerTypeFK = vmCommonCustomer.CustomerTypeFk;
            commonCustomer.SecurityAmount = vmCommonCustomer.SecurityAmount;
            commonCustomer.CustomerStatus = vmCommonCustomer.CustomerStatus;
            commonCustomer.Propietor = vmCommonCustomer.Propietor;
            commonCustomer.NomineeName = vmCommonCustomer.NomineeName;
            commonCustomer.NomineePhone = vmCommonCustomer.NomineePhone;
            commonCustomer.ZoneId = vmCommonCustomer.ZoneId;
            commonCustomer.BusinessAddress = vmCommonCustomer.BusinessAddress;
            commonCustomer.NomineeNID = vmCommonCustomer.NomineeNID;
            commonCustomer.NomineeRelation = vmCommonCustomer.NomineeRelation;
            commonCustomer.VendorReferenceId = vmCommonCustomer.VendorReferenceId;
            commonCustomer.docid = vmCommonCustomer.ImageDocId;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;
            }

            return result;
        }
        public async Task<int> RSCustomerAdd(VMCommonSupplier vmCommonCustomer)
        {
            var result = -1;
            #region Genarate Supplier code
            int totalSupplier = _db.Vendors
                .Where(x => x.VendorTypeId == (int)Provider.Customer && x.CompanyId == vmCommonCustomer.CompanyFK).Count();


            if (totalSupplier == 0)
            {
                totalSupplier = 1;
            }
            else
            {
                totalSupplier++;
            }

            var newString = totalSupplier.ToString().PadLeft(4, '0');
            #endregion
            Vendor commonCustomer = new Vendor
            {
                Name = vmCommonCustomer.Name,
                Phone = vmCommonCustomer.Phone,
                Email = vmCommonCustomer.Email,
                UpazilaId = vmCommonCustomer.Common_UpazilasFk,
                ContactName = vmCommonCustomer.ContactPerson,
                VendorTypeId = (int)Provider.Customer,
                Address = vmCommonCustomer.Address,
                SubZoneId = vmCommonCustomer.SubZoneId,
                NID = vmCommonCustomer.NID,
                CreditLimit = vmCommonCustomer.CreditLimit,
                CustomerTypeFK = vmCommonCustomer.CustomerTypeFk,
                CompanyId = vmCommonCustomer.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                SecurityAmount = vmCommonCustomer.SecurityAmount,
                CustomerStatus = vmCommonCustomer.CustomerStatus,
                Propietor = vmCommonCustomer.Propietor,
                CustomerType = vmCommonCustomer.PaymentType,
                IsActive = true,
                CreditRatioFrom = 0,
                CreditRatioTo = 0,
                IsForeign = false,
                NomineeName = vmCommonCustomer.NomineeName,
                NomineePhone = vmCommonCustomer.NomineePhone,
                ZoneId = vmCommonCustomer.ZoneId,
                NomineeRelation = vmCommonCustomer.NomineeRelation,
                NomineeNID = vmCommonCustomer.NomineeNID,
                BusinessAddress = vmCommonCustomer.BusinessAddress,
                docid = vmCommonCustomer.ImageDocId,
                ProfessionId = vmCommonCustomer.vendorProfessionId,
                DateOfBirth = vmCommonCustomer.DateOfBirth,
                MarriageDate = vmCommonCustomer.MarriageDate
            };
            _db.Vendors.Add(commonCustomer);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;


            }
            return result;
        }
        public async Task<int> RSCustomerEdit(VMCommonSupplier vmCommonCustomer)
        {
            var result = -1;
            Vendor commonCustomer = _db.Vendors.Find(vmCommonCustomer.ID);
            commonCustomer.Name = vmCommonCustomer.Name;
            commonCustomer.UpazilaId = vmCommonCustomer.Common_UpazilasFk;
            commonCustomer.Address = vmCommonCustomer.Address;
            commonCustomer.Phone = vmCommonCustomer.Phone;
            commonCustomer.SubZoneId = vmCommonCustomer.SubZoneId;
            commonCustomer.NID = vmCommonCustomer.NID;
            commonCustomer.CreditLimit = vmCommonCustomer.CreditLimit;
            commonCustomer.Email = vmCommonCustomer.Email;
            commonCustomer.Remarks = vmCommonCustomer.Remarks;
            commonCustomer.CompanyId = vmCommonCustomer.CompanyFK.Value;
            commonCustomer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonCustomer.ModifiedDate = DateTime.Now;
            commonCustomer.ContactName = vmCommonCustomer.ContactPerson;
            commonCustomer.CustomerTypeFK = vmCommonCustomer.CustomerTypeFk;
            commonCustomer.SecurityAmount = vmCommonCustomer.SecurityAmount;
            commonCustomer.CustomerStatus = vmCommonCustomer.CustomerStatus;
            commonCustomer.Propietor = vmCommonCustomer.Propietor;
            commonCustomer.NomineeName = vmCommonCustomer.NomineeName;
            commonCustomer.NomineePhone = vmCommonCustomer.NomineePhone;
            commonCustomer.ZoneId = vmCommonCustomer.ZoneId;
            commonCustomer.BusinessAddress = vmCommonCustomer.BusinessAddress;
            commonCustomer.NomineeNID = vmCommonCustomer.NomineeNID;
            commonCustomer.NomineeRelation = vmCommonCustomer.NomineeRelation;
            commonCustomer.docid = vmCommonCustomer.ImageDocId;
            commonCustomer.ProfessionId = vmCommonCustomer.vendorProfessionId;
            commonCustomer.DateOfBirth = vmCommonCustomer.DateOfBirth;
            commonCustomer.MarriageDate = vmCommonCustomer.MarriageDate;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;
            }

            return result;
        }
        public async Task<int> CustomerEdit(VMCommonSupplier vmCommonCustomer)
        {
            var result = -1;

            int ParentId = 0;

            var subZones = _db.SubZones.Find(vmCommonCustomer.SubZoneId);
            ParentId = subZones.AccountHeadId;

            string newAccountCode = "";
            int orderNo = 0;
            Head5 parentHead = _db.Head5.Where(x => x.Id == ParentId).FirstOrDefault();

            IQueryable<HeadGL> childHeads = _db.HeadGLs.Where(x => x.ParentId == ParentId && x.IsActive);

            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 10);
                string childPart = lastAccCode.Substring(10, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }
            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }



            Vendor commonCustomer = _db.Vendors.Find(vmCommonCustomer.ID);
            commonCustomer.SalesOfficerEmpId = vmCommonCustomer.SalesOfficerEmpId;
            commonCustomer.Name = vmCommonCustomer.Name;
            commonCustomer.UpazilaId = vmCommonCustomer.Common_UpazilasFk;
            commonCustomer.Address = vmCommonCustomer.Address;
            commonCustomer.Phone = vmCommonCustomer.Phone;
            commonCustomer.SubZoneId = vmCommonCustomer.SubZoneId;
            commonCustomer.NID = vmCommonCustomer.NID;
            commonCustomer.CreditLimit = vmCommonCustomer.CreditLimit;
            commonCustomer.Email = vmCommonCustomer.Email;
            commonCustomer.Remarks = vmCommonCustomer.Remarks;
            commonCustomer.CompanyId = vmCommonCustomer.CompanyFK.Value;
            commonCustomer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonCustomer.ModifiedDate = DateTime.Now;
            commonCustomer.ContactName = vmCommonCustomer.ContactPerson;
            commonCustomer.CustomerTypeFK = vmCommonCustomer.CustomerTypeFk;
            commonCustomer.SecurityAmount = vmCommonCustomer.SecurityAmount;
            commonCustomer.CustomerStatus = vmCommonCustomer.CustomerStatus;
            commonCustomer.Propietor = vmCommonCustomer.Propietor;
            commonCustomer.NomineeName = vmCommonCustomer.NomineeName;
            commonCustomer.NomineePhone = vmCommonCustomer.NomineePhone;
            commonCustomer.ZoneId = vmCommonCustomer.ZoneId;
            commonCustomer.BusinessAddress = vmCommonCustomer.BusinessAddress;
            commonCustomer.NomineeNID = vmCommonCustomer.NomineeNID;
            commonCustomer.NomineeRelation = vmCommonCustomer.NomineeRelation;
            commonCustomer.CustomerType = vmCommonCustomer.PaymentType;
            commonCustomer.DepotCarrying = vmCommonCustomer.DepotCarrying;
            commonCustomer.IsIncentiveInInvoice = vmCommonCustomer.IsIncentiveInInvoice;
            commonCustomer.FactoryCarrying = vmCommonCustomer.FactoryCarrying;
            commonCustomer.FixedIncentive = vmCommonCustomer.FixedIncentive;
            commonCustomer.IsPoultry = vmCommonCustomer.IsPoultry;
            commonCustomer.IsFish = vmCommonCustomer.IsFish;
            commonCustomer.IsCattle = vmCommonCustomer.IsCattle;
            commonCustomer.CashCommissionPoultry = vmCommonCustomer.CashCommissionPoultry;
            commonCustomer.CashCommissionFish = vmCommonCustomer.CashCommissionFish;
            commonCustomer.CashCommissionCattle = vmCommonCustomer.CashCommissionCattle;
            commonCustomer.YearlyTarget = vmCommonCustomer.YearlyTarget;
            commonCustomer.CreditRatioFrom = vmCommonCustomer.CreditRatioFrom;
            commonCustomer.MonthlyTarget = vmCommonCustomer.MonthlyTarget;
            commonCustomer.CreditRatioTo = vmCommonCustomer.CreditRatioTo;
            commonCustomer.MonthlyIncentive = vmCommonCustomer.MonthlyIncentive;
            commonCustomer.YearlyIncentive = vmCommonCustomer.YearlyIncentive;
            commonCustomer.FixedCommissionCattle = vmCommonCustomer.FixedCommissionCattle;
            commonCustomer.FixedCommissionFish = vmCommonCustomer.FixedCommissionFish;
            commonCustomer.FixedCommissionPoultry = vmCommonCustomer.FixedCommissionPoultry;
            commonCustomer.CheckDetailId = vmCommonCustomer.CheckDetailId;
            commonCustomer.CheckTypeId = vmCommonCustomer.CheckTypeId;
            commonCustomer.CheckNo = vmCommonCustomer.CheckNo;
            commonCustomer.ACName = vmCommonCustomer.ACName;
            commonCustomer.ACNo = vmCommonCustomer.ACNo;
            commonCustomer.BranchName = vmCommonCustomer.BranchName;
            commonCustomer.BankName = vmCommonCustomer.BankName;
            commonCustomer.Code = newAccountCode;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;
                //if (commonCustomer.CompanyId == (int)CompanyName.KrishibidFeedLimited)
                //{
                //    _db.Database.ExecuteSqlCommand("exec spUpdateCustomerCommission {0},{1},{2}", commonCustomer.CompanyId, commonCustomer.VendorId, commonCustomer.CreatedBy);

                //}
            }

            await IntegratedAccountsHeadEditCustomer(commonCustomer.Name, commonCustomer.HeadGLId.Value,ParentId, newAccountCode);

            return result;
        }

        public async Task<int> KfmalCustomerEdit(VMCommonSupplier vmCommonCustomer)
        {
            var result = -1;
            Vendor commonCustomer = _db.Vendors.Find(vmCommonCustomer.ID);
            commonCustomer.Name = vmCommonCustomer.Name;
            commonCustomer.Propietor = vmCommonCustomer.Propietor;
            commonCustomer.ContactName = vmCommonCustomer.ContactPerson;
            commonCustomer.Phone = vmCommonCustomer.Phone;
            commonCustomer.Email = vmCommonCustomer.Email;
            commonCustomer.DistrictId = vmCommonCustomer.Common_DistrictsFk;
            commonCustomer.UpazilaId = vmCommonCustomer.Common_UpazilasFk;
            commonCustomer.ZoneId = vmCommonCustomer.ZoneId;
            commonCustomer.SubZoneId = vmCommonCustomer.SubZoneId;
            commonCustomer.CustomerTypeFK = vmCommonCustomer.CustomerTypeFk;
            commonCustomer.CustomerStatus = vmCommonCustomer.CustomerProductTypeFk;
            commonCustomer.NID = vmCommonCustomer.NID;
            commonCustomer.Address = vmCommonCustomer.Address;
            commonCustomer.BusinessAddress = vmCommonCustomer.BusinessAddress;
            commonCustomer.BIN = vmCommonCustomer.ETinNo;
            commonCustomer.Condition = vmCommonCustomer.TradeLicense;
            commonCustomer.SecurityAmount = vmCommonCustomer.SecurityAmount;
            commonCustomer.CheckNo = vmCommonCustomer.CheckNo;
            commonCustomer.GuarantorName = vmCommonCustomer.GuarantorName;
            commonCustomer.GurantorMobileNo = vmCommonCustomer.GurantorMobileNo;
            commonCustomer.GurantorAddress = vmCommonCustomer.GurantorAddress;
            commonCustomer.NomineeName = vmCommonCustomer.NomineeName;
            commonCustomer.NomineePhone = vmCommonCustomer.NomineePhone;
            commonCustomer.NomineeRelation = vmCommonCustomer.NomineeRelation;
            commonCustomer.NomineeNID = vmCommonCustomer.NomineeNID;
            commonCustomer.CustomerType = vmCommonCustomer.PaymentType;
            commonCustomer.VendorTypeId = (int)Provider.Customer;
            commonCustomer.docid = vmCommonCustomer.ImageDocId;


            commonCustomer.CompanyId = vmCommonCustomer.CompanyFK.Value;
            commonCustomer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            commonCustomer.CreatedDate = DateTime.Now;
            commonCustomer.IsActive = true;


            HeadGL headGL = _db.HeadGLs.Find(commonCustomer.HeadGLId);
            headGL.AccName = commonCustomer.Name;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = commonCustomer.VendorId;
            }
            await IntegratedAccountsHeadEdit(commonCustomer.Name, commonCustomer.HeadGLId.Value);

            return result;
        }
        public async Task<int> CustomerDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Vendor commonCustomer = _db.Vendors.Find(id);
                commonCustomer.IsActive = false;
                commonCustomer.ModifiedDate = DateTime.Now;
                commonCustomer.ModifiedBy = HttpContext.Current.Request.Cookies["UserNameCookie"]?.Value ?? System.Web.HttpContext.Current.User.Identity.Name;

                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    // throw;
                }
                //if (await _db.SaveChangesAsync() > 0)
                //{
                //    result = commonSupplier.VendorId;
                //}
            }
            return result;
        }

        #endregion
        #region MyRegion
        public async Task<VMCommonDivisions> GetDivisions()
        {
            VMCommonDivisions vmCommonDivisions = new VMCommonDivisions();

            vmCommonDivisions.DataList = await Task.Run(() => (from t1 in _db.Divisions
                                                               select new VMCommonDivisions
                                                               {
                                                                   ID = t1.DivisionId,
                                                                   Name = t1.Name
                                                               }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonDivisions;
        }
        public async Task<VMCommonDistricts> GetDistricts(int divisionsId = 0)
        {
            VMCommonDistricts vmCommonDistricts = new VMCommonDistricts();



            vmCommonDistricts.DataList = await Task.Run(() => (from t1 in _db.Districts
                                                               join t2 in _db.Divisions on t1.DivisionId equals t2.DivisionId
                                                               where t1.IsActive

                                                               && ((divisionsId > 0) ? t1.DivisionId == divisionsId : t1.DistrictId > 0)
                                                               select new VMCommonDistricts
                                                               {
                                                                   Common_DivisionsFk = t1.DivisionId,
                                                                   ID = t1.DistrictId,
                                                                   DivisionsName = t2.Name,

                                                                   Name = t1.Name
                                                               }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonDistricts;
        }

        public List<District> GetDistrictsDropDownList()
        {

            var list = _db.Districts.Where(a => a.IsActive).ToList();

            return list;
        }
        public async Task<VMCommonThana> GetUpazilas(int divisionsId = 0, int districtsId = 0)
        {
            VMCommonThana vmCommonThana = new VMCommonThana();

            vmCommonThana.DataList = await Task.Run(() => (from t1 in _db.Upazilas
                                                           join t2 in _db.Districts on t1.DistrictId equals t2.DistrictId
                                                           join t3 in _db.Divisions on t2.DivisionId equals t3.DivisionId

                                                           where t1.IsActive && t2.IsActive &&
                                                           ((divisionsId > 0 && districtsId == 0) ? t2.DivisionId == divisionsId
                                                           : (divisionsId == 0 && districtsId > 0) ? t1.DistrictId == districtsId
                                                           : t1.UpazilaId > 0)
                                                           select new VMCommonThana
                                                           {
                                                               ID = t1.UpazilaId,
                                                               Name = t1.Name,
                                                               Common_DistrictsFk = t2.DistrictId,
                                                               Common_DivisionsFk = t3.DivisionId,
                                                               DistictName = t2.Name,
                                                               DivisionsName = t3.Name

                                                           }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonThana;
        }
        #endregion
        public List<object> DDLDistrictListByDivisionID(int DivisionId)
        {
            var list = new List<object>();
            var vData = _db.Districts.Where(a => a.IsActive == true && a.DivisionId == DivisionId).ToList();
            foreach (var x in vData)
            {
                list.Add(new { Text = x.Name, Value = x.DistrictId });
            }
            return list;
        }
        public List<object> DDLUpazilasListByDistrictID(int DistrictId)
        {
            var list = new List<object>();
            var vData = _db.Upazilas.Where(a => a.IsActive == true && a.DistrictId == DistrictId).ToList();
            foreach (var x in vData)
            {
                list.Add(new { Text = x.Name, Value = x.UpazilaId });
            }
            return list;
        }

        public UpazilaModel UpazilasById(int upazilaId)
        {

            var vData = (from t1 in _db.Upazilas

                         where t1.IsActive == true && t1.UpazilaId == upazilaId
                         select new UpazilaModel
                         {
                             UpazilaId = t1.UpazilaId,
                             FacCarryingCommission = t1.FacCarryingCommission,
                             DepoCarryingCommission = t1.DepoCarryingCommission
                         }).FirstOrDefault();


            return vData;
        }

        public List<object> GetVendorsPaymentMethodEnum()
        {
            var PaymentTypeList = new List<object>();

            foreach (var eVal in Enum.GetValues(typeof(VendorsPaymentMethodEnum)))
            {
                PaymentTypeList.Add(new SelectListItem { Text = Enum.GetName(typeof(VendorsPaymentMethodEnum), eVal), Value = (Convert.ToInt32(eVal)).ToString() });
            }
            return PaymentTypeList;
        }



        public List<object> CommonActSignatorysDropDownList(int companyId)
        {
            var list = new List<object>();
            var v = _db.Accounting_Signatory.Where(x => x.CompanyId == companyId).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.SignatoryName, Value = x.SignatoryId });
            }
            return list;
        }
        public async Task<VMAccountingSignatory> GetAccountingSignatory(int companyId)
        {
            VMAccountingSignatory vmAccountingSignatory = new VMAccountingSignatory();
            vmAccountingSignatory.CompanyFK = companyId;
            vmAccountingSignatory.DataList = await Task.Run(() => (from t1 in _db.Accounting_Signatory
                                                                   join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                                                                   where t1.CompanyId == companyId && t1.IsActive

                                                                   select new VMAccountingSignatory
                                                                   {
                                                                       SignatoryId = t1.SignatoryId,
                                                                       SignatoryType = t1.SignatoryType,
                                                                       SignatoryName = t1.SignatoryName,
                                                                       CompanyName = t2.Name,
                                                                       OrderBy = t1.OrderBy,
                                                                       CompanyFK = t1.CompanyId,
                                                                       EmployeeId = t1.EmployeeId
                                                                   }).OrderByDescending(x => x.SignatoryId).AsEnumerable());
            return vmAccountingSignatory;
        }

        public async Task<int> AccountingSignatoryAdd(VMAccountingSignatory vmAccountingSignatory)
        {
            var result = -1;
            Accounting_Signatory commo = new Accounting_Signatory
            {

                SignatoryName = vmAccountingSignatory.SignatoryName,
                EmployeeId = vmAccountingSignatory.EmployeeId,
                SignatoryType = vmAccountingSignatory.SignatoryType,
                OrderBy = vmAccountingSignatory.OrderBy,
                CompanyId = vmAccountingSignatory.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                Priority = 1
            };
            _db.Accounting_Signatory.Add(commo);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commo.SignatoryId;
            }
            return result;
        }
        public async Task<int> AccountingSignatoryEdit(VMAccountingSignatory vmAccountingSignatory)
        {

            var result = -1;
            Accounting_Signatory accountingSignatory = _db.Accounting_Signatory.Find(vmAccountingSignatory.SignatoryId);
            accountingSignatory.CompanyId = vmAccountingSignatory.CompanyFK.Value;
            accountingSignatory.SignatoryType = vmAccountingSignatory.SignatoryType;
            accountingSignatory.OrderBy = vmAccountingSignatory.OrderBy;
            accountingSignatory.EmployeeId = vmAccountingSignatory.EmployeeId;
            accountingSignatory.SignatoryName = vmAccountingSignatory.SignatoryName;
            accountingSignatory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            accountingSignatory.ModifiedDate = DateTime.Now;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = accountingSignatory.SignatoryId;
            }
            return result;
        }
        public async Task<int> AccountingSignatoryDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                Accounting_Signatory accountingSignatory = _db.Accounting_Signatory.Find(id);
                accountingSignatory.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = accountingSignatory.SignatoryId;
                }
            }
            return result;
        }


        //New Compamy Setup
        public async Task<VMCompany> GetCompany()
        {
            VMCompany VMCompany = new VMCompany();

            VMCompany.DataList = await Task.Run(() => (from x in _db.Companies


                                                       select new VMCompany
                                                       {
                                                           ID = x.CompanyId,
                                                           Name = x.Name,
                                                           ShortName = x.ShortName,
                                                           OrderNo = x.OrderNo,
                                                           MushokNo = x.MushokNo,
                                                           IsCompany = x.IsCompany
                                                           //CompanyLogo = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Logo/" + (string.IsNullOrEmpty(x.CompanyLogo) ? "logo.png" : x.CompanyLogo),
                                                       }));
            return VMCompany;
        }

        public async Task<int> CompanyAdd(VMCompany VMCompany)
        {

            var result = -1;
            //if (!string.IsNullOrEmpty(VMCompany.CompanyLogo))
            //{
            //   commoLogo = VMCompany.CompanyLogo;
            //}
            Company commo = new Company
            {
                CompanyId = _db.Database.SqlQuery<int>("exec spGetNewCompanyId").FirstOrDefault(),
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,

                Name = VMCompany.Name,
                ShortName = VMCompany.ShortName,
                OrderNo = VMCompany.OrderNo,
                MushokNo = VMCompany.MushokNo,
                Address = VMCompany.Address,
                Phone = VMCompany.Phone,
                Fax = VMCompany.Fax,
                Email = VMCompany.Email,
                //CompanyLogo = commoLogo,
                IsCompany = VMCompany.IsCompany,
                IsActive = VMCompany.IsActive,

            };
            //SignatoryName = VMCompany.SignatoryName,
            //    SignatoryType = VMCompany.SignatoryType,
            //    CompanyId = VMCompany.CompanyFK.Value,
            //    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
            //    CreatedDate = DateTime.Now,
            //    IsActive = true,
            //    Priority = 1

            _db.Companies.Add(commo);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = commo.CompanyId;
            }
            return result;
        }
        //public async Task<int> AccountingSignatoryEdit(VMCompany VMCompany)
        //{

        //    var result = -1;
        //    Company accountingSignatory = _db.Companies.Find(VMCompany.SignatoryId);
        //    accountingSignatory.CompanyId = VMCompany.CompanyFK.Value;
        //    accountingSignatory.SignatoryType = VMCompany.SignatoryType;
        //    accountingSignatory.SignatoryName = VMCompany.SignatoryName;
        //    accountingSignatory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //    accountingSignatory.ModifiedDate = DateTime.Now;
        //    if (await _db.SaveChangesAsync() > 0)
        //    {
        //        result = accountingSignatory.SignatoryId;
        //    }
        //    return result;
        //}
        //public async Task<int> AccountingSignatoryDelete(int id)
        //{
        //    int result = -1;

        //    if (id != 0)
        //    {
        //        Company accountingSignatory = _db.Companies.Find(id);
        //        accountingSignatory.IsActive = false;

        //        if (await _db.SaveChangesAsync() > 0)
        //        {
        //            result = accountingSignatory.SignatoryId;
        //        }
        //    }
        //    return result;
        //}

        #region Bank  


        public List<object> CommonBanksDropDownList(int companyId)
        {
            var list = new List<object>();
            var v = _db.Banks.Where(x => x.CompanyId == companyId && x.IsActive == true).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.BankId });
            }
            return list;
        }

        public async Task<VMCommonBank> GetBanks(int companyId)
        {
            VMCommonBank vMCommonBank = new VMCommonBank();

            vMCommonBank.CompanyFK = companyId;

            vMCommonBank.DataList = await Task.Run(() => (from t1 in _db.Banks
                                                          where t1.IsActive && t1.CompanyId == companyId
                                                          select new VMCommonBank
                                                          {
                                                              ID = t1.BankId,
                                                              Name = t1.Name,
                                                              CompanyFK = t1.CompanyId,
                                                              ShortName = t1.ShortName
                                                          }).OrderByDescending(x => x.ID).AsEnumerable());

            return vMCommonBank;
        }

        public async Task<VMCommonShift> GetShift(int companyId)
        {
            VMCommonShift vMCommonShift = new VMCommonShift();

            vMCommonShift.CompanyFK = companyId;

            vMCommonShift.DataList = await Task.Run(() => (from t1 in _db.Shifts
                                                           where t1.IsActive && (t1.CompanyId == null || t1.CompanyId == companyId)
                                                           select new VMCommonShift
                                                           {
                                                               ID = t1.ShiftId,
                                                               Name = t1.Name,
                                                               StarAt = t1.StartAt,
                                                               EndAt = t1.EndAt,
                                                               CompanyFK = t1.CompanyId,
                                                           }).OrderByDescending(x => x.ID).AsEnumerable());

            return vMCommonShift;
        }

        public async Task<VMCommonDesignation> GetDesignation(int companyId)
        {
            VMCommonDesignation commonDesignation = new VMCommonDesignation();

            commonDesignation.CompanyFK = companyId;

            commonDesignation.DataList = await Task.Run(() => (from t1 in _db.Designations
                                                               where t1.IsActive && (t1.CompanyId == null || t1.CompanyId == companyId)
                                                               select new VMCommonDesignation
                                                               {
                                                                   ID = t1.DesignationId,
                                                                   Name = t1.Name,
                                                                   CompanyFK = t1.CompanyId,
                                                               }).OrderByDescending(x => x.ID).AsEnumerable());

            return commonDesignation;
        }


        public async Task<int> BankAdd(VMCommonBank vMCommonBank)
        {
            var result = -1;
            Bank bank = new Bank
            {
                Name = vMCommonBank.Name,
                CompanyId = vMCommonBank.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                ShortName = vMCommonBank.ShortName

            };
            _db.Banks.Add(bank);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = bank.BankId;
            }
            return result;
        }

        public async Task<int> ShiftAdd(VMCommonShift vMCommonShift)
        {
            var result = -1;
            Shift shift = new Shift
            {
                Name = vMCommonShift.Name,
                CompanyId = vMCommonShift.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                StartAt = vMCommonShift.StarAt,
                EndAt = vMCommonShift.EndAt,


            };
            _db.Shifts.Add(shift);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = shift.ShiftId;
            }
            return result;
        }

        public async Task<int> DesignationAdd(VMCommonDesignation vMCommonDesignation)
        {
            var result = -1;
            Designation designation = new Designation
            {
                Name = vMCommonDesignation.Name,
                CompanyId = vMCommonDesignation.CompanyFK,
                CreatedDate = DateTime.Now,
                IsActive = true,

            };
            _db.Designations.Add(designation);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = designation.DesignationId;
            }
            return result;
        }

        public async Task<int> BankEdit(VMCommonBank vMCommonBank)
        {
            var result = -1;
            Bank bank = _db.Banks.Find(vMCommonBank.ID);
            bank.Name = vMCommonBank.Name;
            bank.ShortName = vMCommonBank.ShortName;
            bank.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            bank.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = bank.BankId;
            }
            return result;
        }

        public async Task<int> ShiftEdit(VMCommonShift vMCommonShift)
        {
            var result = -1;
            Shift shift = _db.Shifts.Find(vMCommonShift.ID);
            shift.Name = vMCommonShift.Name;
            shift.UpdateBy = Common.GetUserId();
            shift.UpdateDate = DateTime.Now;
            shift.StartAt = vMCommonShift.StarAt;
            shift.EndAt = vMCommonShift.EndAt;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = shift.ShiftId;
            }
            return result;
        }

        public async Task<int> DesignationEdit(VMCommonDesignation vMCommonDesignation)
        {
            var result = -1;
            Designation designation = _db.Designations.Find(vMCommonDesignation.ID);
            designation.Name = vMCommonDesignation.Name;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = designation.DesignationId;
            }
            return result;
        }


        public async Task<int> BankDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Bank bank = _db.Banks.Find(id);
                bank.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = bank.BankId;
                }
            }
            return result;
        }

        public async Task<int> ShiftDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Shift shift = _db.Shifts.Find(id);
                shift.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = shift.ShiftId;
                }
            }
            return result;
        }

        public async Task<int> DesignationDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Designation designation = _db.Designations.Find(id);
                designation.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = designation.DesignationId;
                }
            }
            return result;
        }
        #endregion

        #region Bank Branch

        public List<object> CommonBankBranchsDropDownList(int companyId)
        {
            var list = new List<object>();
            var v = _db.BankBranches.Where(x => x.CompanyId == companyId).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.BankId });
            }
            return list;
        }

        public async Task<List<VMCommonBankBranch>> CommonBankGet(int companyId, int bankId)
        {

            List<VMCommonBankBranch> vMCommonBankBranches =
                await Task.Run(() => (_db.BankBranches
                .Where(x => x.IsActive && x.BankId == bankId && x.CompanyId == companyId))
                .Select(x => new VMCommonBankBranch() { ID = x.BankBranchId, Name = x.Name })
                .ToListAsync());


            return vMCommonBankBranches;
        }
        public async Task<VMCommonBankBranch> GetBankBranchs(int companyId, int bankId = 0)
        {
            VMCommonBankBranch vmCommonSubZone = new VMCommonBankBranch();
            vmCommonSubZone.CompanyFK = companyId;
            vmCommonSubZone.DataList = await Task.Run(() => (from t1 in _db.BankBranches
                                                             join t2 in _db.Banks on t1.BankId equals t2.BankId
                                                             where t1.IsActive && t1.CompanyId == companyId
                                                             //&& (bankId > 0 ? t1.BankId == bankId : t1.BankBranchId > 0)
                                                             select new VMCommonBankBranch
                                                             {
                                                                 ID = t1.BankBranchId,
                                                                 BankId = t2.BankId,
                                                                 Name = t1.Name,
                                                                 BankName = t2.Name,
                                                                 City = t1.City,
                                                                 Street = t1.Street,
                                                                 ZIPCode = t1.ZIPCode,
                                                                 CompanyFK = t1.CompanyId,
                                                                 AccountNumber = t1.AccountNumber,
                                                                 AccountName = t1.AccountName,
                                                                 AccountType = t1.AccountType
                                                             }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonSubZone;
        }

        public async Task<int> BankBranchAdd(VMCommonBankBranch vMCommonBankBranch)
        {
            var result = -1;
            BankBranch bankBranch = new BankBranch
            {

                Name = vMCommonBankBranch.Name,
                AccountName = vMCommonBankBranch.AccountName,
                AccountNumber = vMCommonBankBranch.AccountNumber,
                AccountType = vMCommonBankBranch.AccountType.Replace('_', ' '),

                City = vMCommonBankBranch.City,
                Street = vMCommonBankBranch.Street,
                ZIPCode = vMCommonBankBranch.ZIPCode,
                BankId = vMCommonBankBranch.BankId,
                CompanyId = vMCommonBankBranch.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.BankBranches.Add(bankBranch);
            if (await _db.SaveChangesAsync() > 0)
            {

                var Head5 = await _db.Head5.Where(x => x.AccCode == "1301001002" && x.CompanyId == vMCommonBankBranch.CompanyFK && x.IsActive == true).FirstOrDefaultAsync();

                var bankBranchVm = _db.BankBranches.Find(bankBranch.BankBranchId);
                var bankvm = _db.Banks.Find(bankBranch.BankId);
                VMHeadIntegration integration = new VMHeadIntegration
                {
                    AccName = bankvm.Name + "-" + bankBranch.Name + " (" + bankvm.ShortName + "-A/C: " + bankBranchVm.AccountNumber + ")",
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    ParentId = Head5.Id,
                    IsIncomeHead = false,
                    CompanyFK = bankBranch.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
                int head5Id = AccHeadGlBank(integration, bankBranchVm.BankBranchId);




                result = bankBranch.BankBranchId;
            }
            return result;
        }
        public async Task<int> BankBranchEdit(VMCommonBankBranch vMCommonBankBranch)
        {
            var result = -1;
            BankBranch bankBranch = _db.BankBranches.Find(vMCommonBankBranch.ID);
            bankBranch.BankId = vMCommonBankBranch.BankId;
            bankBranch.Name = vMCommonBankBranch.Name;
            bankBranch.AccountName = vMCommonBankBranch.AccountName;
            bankBranch.AccountNumber = vMCommonBankBranch.AccountNumber;
            bankBranch.AccountType = vMCommonBankBranch.AccountType.Replace('_', ' ');
            bankBranch.Street = vMCommonBankBranch.Street;
            bankBranch.City = vMCommonBankBranch.City;
            bankBranch.ZIPCode = vMCommonBankBranch.ZIPCode;
            bankBranch.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            bankBranch.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = bankBranch.BankBranchId;
            }
            return result;
        }
        public async Task<int> BankBranchDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                BankBranch bankBranch = _db.BankBranches.Find(id);
                bankBranch.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = bankBranch.BankBranchId;
                }
            }
            return result;
        }

        #endregion

        #region Account Cheque Info
        public VMCommonActChequeInfo GetActChequeInfoByID(int id)
        {
            var v = (from t1 in _db.Accounting_ChequeInfo.Where(x => x.ActChequeInfoId == id)
                     join t2 in _db.BankBranches on t1.BankBranchId equals t2.BankBranchId
                     join t3 in _db.Accounting_Signatory on t1.SignatoryId equals t3.SignatoryId
                     select new VMCommonActChequeInfo
                     {
                         ID = t1.ActChequeInfoId,
                         BankBranchId = t1.BankBranchId,
                         BankId = t2.BankId ?? 0,
                         AccountNo = t1.AccountNo,
                         ChequeDate = t1.ChequeDate,
                         PayTo = t1.PayTo,
                         Amount = t1.Amount,
                         SignatoryId = t1.SignatoryId ?? 0,
                         SignatoryName = t3.SignatoryName,
                         BankName = t2.Bank.Name,
                         BankBranchName = t2.Name,
                         CompanyFK = t1.CompanyId
                     }).FirstOrDefault();
            return v;
        }

        public async Task<VMCommonActChequeInfo> GetActChequeInfos(int companyId)
        {
            VMCommonActChequeInfo vMCommonActChequeInfo = new VMCommonActChequeInfo();
            vMCommonActChequeInfo.CompanyFK = companyId;
            vMCommonActChequeInfo.DataList = await Task.Run(() => (from t1 in _db.Accounting_ChequeInfo
                                                                   join t2 in _db.BankBranches on t1.BankBranchId equals t2.BankBranchId
                                                                   join t3 in _db.Accounting_Signatory on t1.SignatoryId equals t3.SignatoryId
                                                                   where t1.IsActive && t1.CompanyId == companyId
                                                                   select new VMCommonActChequeInfo
                                                                   {
                                                                       ID = t1.ActChequeInfoId,
                                                                       BankBranchId = t2.BankBranchId,
                                                                       PayTo = t1.PayTo,
                                                                       AccountNo = t1.AccountNo,
                                                                       Amount = t1.Amount,
                                                                       CompanyFK = t1.CompanyId,
                                                                       BankBranchName = t2.Name,
                                                                       BankName = t2.Bank.Name,
                                                                       SignatoryName = t3.SignatoryName
                                                                   }).OrderByDescending(x => x.ID).AsEnumerable());
            return vMCommonActChequeInfo;
        }

        public async Task<int> ActChequeInfoAdd(VMCommonActChequeInfo vMCommonActChequeInfo)
        {
            var result = -1;
            Accounting_ChequeInfo accounting_ChequeInfo = new Accounting_ChequeInfo
            {
                AccountNo = vMCommonActChequeInfo.AccountNo,
                PayTo = vMCommonActChequeInfo.PayTo,
                Amount = vMCommonActChequeInfo.Amount,
                ChequeDate = vMCommonActChequeInfo.ChequeDate,
                BankBranchId = vMCommonActChequeInfo.BankBranchId,
                SignatoryId = vMCommonActChequeInfo.SignatoryId,
                CompanyId = vMCommonActChequeInfo.CompanyFK.Value,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Accounting_ChequeInfo.Add(accounting_ChequeInfo);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = accounting_ChequeInfo.ActChequeInfoId;
            }
            return result;
        }
        public async Task<int> ActChequeInfoEdit(VMCommonActChequeInfo vMCommonActChequeInfo)
        {
            var result = -1;
            Accounting_ChequeInfo accounting_ChequeInfo = _db.Accounting_ChequeInfo
                .Find(vMCommonActChequeInfo.ID);

            accounting_ChequeInfo.BankBranchId = vMCommonActChequeInfo.BankBranchId;
            accounting_ChequeInfo.SignatoryId = vMCommonActChequeInfo.SignatoryId;
            accounting_ChequeInfo.PayTo = vMCommonActChequeInfo.PayTo;
            accounting_ChequeInfo.Amount = vMCommonActChequeInfo.Amount;

            accounting_ChequeInfo.AccountNo = vMCommonActChequeInfo.AccountNo;
            accounting_ChequeInfo.ChequeDate = vMCommonActChequeInfo.ChequeDate;

            accounting_ChequeInfo.CompanyId = vMCommonActChequeInfo.CompanyFK.Value;
            accounting_ChequeInfo.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            accounting_ChequeInfo.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = accounting_ChequeInfo.ActChequeInfoId;
            }
            return result;
        }
        public async Task<int> ActChequeInfoDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Accounting_ChequeInfo accounting_ChequeInfo = _db.Accounting_ChequeInfo.Find(id);
                accounting_ChequeInfo.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = accounting_ChequeInfo.ActChequeInfoId;
                }
            }
            return result;
        }

        #endregion


        public async Task<VMCommonProductCategory> GetProductCategoryProject(int companyId, string productType)
        {
            VMCommonProductCategory vmCommonProductCategory = new VMCommonProductCategory();
            vmCommonProductCategory.CompanyFK = companyId;
            vmCommonProductCategory.ProductType = productType;

            vmCommonProductCategory.DataList = await Task.Run(() => (from t1 in _db.ProductCategories
                                                                     where t1.ProductType == productType &&
                                                                     t1.IsActive && t1.CompanyId == companyId
                                                                     // productCategoryId > 0 ? t1.ProductCategoryId == productCategoryId: t1.ProductCategoryId > 0
                                                                     select new VMCommonProductCategory
                                                                     {
                                                                         ID = t1.ProductCategoryId,
                                                                         Name = t1.Name,
                                                                         LandSizeInKatha = (int)t1.LandSizeInKatha,
                                                                         ProductType = t1.ProductType,
                                                                         CompanyFK = t1.CompanyId,
                                                                         CashCommission = t1.CashCustomerRate,
                                                                         Remarks = t1.Remarks,
                                                                         Code = t1.Code,
                                                                         Description = t1.Address,
                                                                         IsCrm = t1.IsCrm

                                                                     }).OrderByDescending(x => x.ID).AsEnumerable());
            return vmCommonProductCategory;
        }

        public async Task<int> ProductCategoryProjectAdd(VMCommonProductCategory productCategoryModel)
        {
            var result = -1;
            int totalProject = _db.ProductCategories.Where(x => x.ProductType == productCategoryModel.ProductType && x.CompanyId == productCategoryModel.CompanyFK).Count();

            if (totalProject == 0)
            {
                totalProject = 1;
            }
            else
            {
                totalProject++;
            }
            ProductCategory productCategory = new ProductCategory
            {
                Code = productCategoryModel.Code,
                Name = productCategoryModel.Name,
                LandSizeInKatha = productCategoryModel.LandSizeInKatha,
                ProductType = productCategoryModel.ProductType,
                CashCustomerRate = productCategoryModel.CashCommission,
                Address = productCategoryModel.Description,
                Remarks = productCategoryModel.Remarks,
                CompanyId = productCategoryModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                NoOfFloors = productCategoryModel.NoOfFloors,
                FlatSizeSpecification = productCategoryModel.FlatSizeSpecification,
                TotalParking = productCategoryModel.TotalParking,
                HandOverDate = productCategoryModel.HandOverDate,
                TargetCostPerSqft = productCategoryModel.TargetCostPerSqft,
                TotalFlat = productCategoryModel.TotalFlat,
                IsActive = true

            };
            _db.ProductCategories.Add(productCategory);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = productCategory.ProductCategoryId;
                if (productCategory.CompanyId == (int)CompanyName.KrishibidPropertiesLimited)
                {
                    int head5Id = ProjectHead5Push(productCategory);

                    VMHeadIntegration integration = new VMHeadIntegration
                    {
                        AccName = productCategory.Name,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = 42868,

                        CompanyFK = productCategory.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                    };
                    HeadGL headGlId = await PayableHeadIntegrationAdd(integration);
                    if (headGlId != null)
                    {
                        await KPLProjectCodeAndHeadGLIdEdit(productCategory.ProductCategoryId, headGlId, head5Id);
                    }

                }

            }
            return result;
        }

        private int ProjectHead5Push(ProductCategory productCategory)
        {
            int result = -1;
            string newAccountCode = "";
            int orderNo = 0;
            Head4 parentHead = _db.Head4.Where(x => x.Id == 41304).FirstOrDefault();
            IQueryable<Head5> childHeads = _db.Head5.Where(x => x.ParentId == 41304);

            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 7);
                string childPart = lastAccCode.Substring(7, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }
            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }

            Head5 head5 = new Head5
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = newAccountCode,
                AccName = productCategory.Name,
                ParentId = 41304,// Propertise Accounts Receivable Head4 Id
                CompanyId = productCategory.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 5,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = orderNo,
                Remarks = ""
            };
            _db.Head5.Add(head5);
            if (_db.SaveChanges() > 0)
            {
                result = head5.Id;
            }
            return result;

        }

        private int BlockHead5Push(ProductSubCategory productSubCategory)
        {
            int result = -1;
            string newAccountCode = "";
            int orderNo = 0;
            Head4 parentHead = _db.Head4.Where(x => x.Id == 50598716).FirstOrDefault();
            IQueryable<Head5> childHeads = _db.Head5.Where(x => x.ParentId == 50598716);

            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 7);
                string childPart = lastAccCode.Substring(7, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }
            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }

            Head5 head5 = new Head5
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = newAccountCode,
                AccName = productSubCategory.Name,
                ParentId = 41304,// Propertise Accounts Receivable Head4 Id
                CompanyId = productSubCategory.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 5,
                CreateDate = DateTime.Now,

                OrderNo = orderNo,
                Remarks = ""
            };
            _db.Head5.Add(head5);
            if (_db.SaveChanges() > 0)
            {
                result = head5.Id;
            }
            return result;

        }


        private int AccHead5Push(VMHeadIntegration vmModel, int id)
        {
            int result = -1;

            //List<Head5> head5s = new List<Head5>();
            if (vmModel.ProductType == "R")
            {

                Head5 head5_1 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(4576),

                    AccName = vmModel.AccName,
                    ParentId = 4576,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };



                _db.Head5.Add(head5_1);
                var catagoryForAssets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForAssets.AccountingHeadId = head5_1.Id;
                _db.SaveChanges();

                Head5 head5_2 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(50609539), // 
                    AccName = vmModel.AccName,
                    ParentId = 50609539,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };
                _db.Head5.Add(head5_2);
                var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForIncome.AccountingIncomeHeadId = head5_2.Id;
                _db.SaveChanges();


                Head5 head5_3 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(3213), // 
                    AccName = vmModel.AccName,
                    ParentId = 3213,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    OrderNo = 0,
                    Remarks = ""
                };
                _db.Head5.Add(head5_3);
                var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForExpanse.AccountingExpenseHeadId = head5_3.Id;
                _db.SaveChanges();

            }

            if (vmModel.ProductType == "F")
            {

                Head5 head5_1 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(4579), //Stock And Store

                    AccName = vmModel.AccName,
                    ParentId = 4579,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };


                _db.Head5.Add(head5_1);
                var catagoryForAssets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForAssets.AccountingHeadId = head5_1.Id;
                _db.SaveChanges();

                Head5 head5_2 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(50609029), // Income
                    AccName = vmModel.AccName,
                    ParentId = 50609029,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };


                _db.Head5.Add(head5_2);
                var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForIncome.AccountingIncomeHeadId = head5_2.Id;
                _db.SaveChanges();


                Head5 head5_3 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(50609623), // Expenses
                    AccName = vmModel.AccName,
                    ParentId = 50609623,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    OrderNo = 0,
                    Remarks = ""
                };
                _db.Head5.Add(head5_3);
                var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForExpanse.AccountingExpenseHeadId = head5_3.Id;
                _db.SaveChanges();

            }

            if (vmModel.ProductType == "P")
            {

                Head5 head5_1 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(50606085), //Stock And Store

                    AccName = vmModel.AccName,
                    ParentId = 50606085,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };


                _db.Head5.Add(head5_1);
                var catagoryForassets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForassets.AccountingHeadId = head5_1.Id;
                _db.SaveChanges();

                Head5 head5_2 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(50613003), // Income
                    AccName = vmModel.AccName,
                    ParentId = 50613003,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };

                _db.Head5.Add(head5_2);
                var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForIncome.AccountingIncomeHeadId = head5_2.Id;
                _db.SaveChanges();


                Head5 head5_3 = new Head5
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead5AccCode(50606189), // Expenses
                    AccName = vmModel.AccName,
                    ParentId = 50606189,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 5,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    OrderNo = 0,
                    Remarks = ""
                };

                _db.Head5.Add(head5_3);
                var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForExpanse.AccountingExpenseHeadId = head5_3.Id;
                _db.SaveChanges();

            }

            return result;

        }
        private int AccHead4Push(VMHeadIntegration vmModel, int id)
        {
            int result = -1;
            if (vmModel.ProductType == "F")
            {
                Head4 head4_1 = new Head4
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead4AccCode(50615171),
                    AccName = vmModel.AccName,
                    ParentId = 50615171,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 4,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };
                _db.Head4.Add(head4_1);
                var catagoryForAssets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForAssets.AccountingHeadId = head4_1.Id;
                _db.SaveChanges();

                Head4 head4_2 = new Head4
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead4AccCode(50604524), // 
                    AccName = vmModel.AccName,
                    ParentId = 50604524,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 4,
                    CreateDate = DateTime.Now,
                    IsActive = true,

                    OrderNo = 0,
                    Remarks = ""
                };
                _db.Head4.Add(head4_2);
                var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForIncome.AccountingIncomeHeadId = head4_2.Id;
                _db.SaveChanges();


                Head4 head4_3 = new Head4
                {
                    Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                    AccCode = GenerateHead4AccCode(50604615), // 
                    AccName = vmModel.AccName,
                    ParentId = 50604615,// Propertise Accounts Receivable Head4 Id
                    CompanyId = vmModel.CompanyFK,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    LayerNo = 4,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    OrderNo = 0,
                    Remarks = ""
                };
                _db.Head4.Add(head4_3);
                var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == id);
                catagoryForExpanse.AccountingExpenseHeadId = head4_3.Id;
                _db.SaveChanges();

            }


            return result;

        }


        private int ISSZoneHead4Push(VMHeadIntegration vmModel, int zoneId)
        {
            int result = -1;
            Head4 head4 = new Head4
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHead4AccCode(vmModel.ParentId),
                AccName = vmModel.AccName,
                ParentId = vmModel.ParentId,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 4,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = vmModel.Remarks
            };
            _db.Head4.Add(head4);
            var zones = _db.Zones.SingleOrDefault(x => x.ZoneId == zoneId);
            zones.HeadGLId = head4.Id;
            _db.SaveChanges();

            return result;

        }

        private int ISSSubZoneHead5Push(VMHeadIntegration vmModel, int subzoneId)
        {
            int result = -1;
            Head5 head5 = new Head5
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHead5AccCode(vmModel.ParentId),
                AccName = vmModel.AccName,
                ParentId = vmModel.ParentId,
                Remarks = vmModel.Remarks,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = vmModel.LayerNo,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,

            };
            _db.Head5.Add(head5);
            SubZone subZone = _db.SubZones.Find(subzoneId);
            subZone.AccountHeadId = head5.Id;
            _db.SaveChanges();

            return result;

        }

        private int Acc5PushKFMAL(VMHeadIntegration vmModel, ProductSubCategory productSubCategory)
        {
            int result = -1;
            ProductCategory productCategory = _db.ProductCategories.Find(productSubCategory.ProductCategoryId);
            //List<Head5> head5s = new List<Head5>();
            Head5 head5_1 = new Head5
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHead5AccCode(productCategory.AccountingHeadId.Value),
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 5,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "5 Layer"
            };


            _db.Head5.Add(head5_1);
            var catagoryForAssets = _db.ProductSubCategories.SingleOrDefault(x => x.ProductSubCategoryId == productSubCategory.ProductSubCategoryId);
            catagoryForAssets.AccountingHeadId = head5_1.Id;
            _db.SaveChanges();
            Head5 head5_2 = new Head5
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHead5AccCode(productCategory.AccountingIncomeHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingIncomeHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 5,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "5 Layer"
            };
            _db.Head5.Add(head5_2);
            var catagoryForIncome = _db.ProductSubCategories.SingleOrDefault(x => x.ProductSubCategoryId == productSubCategory.ProductSubCategoryId);
            catagoryForIncome.AccountingIncomeHeadId = head5_2.Id;
            _db.SaveChanges();
            Head5 head5_3 = new Head5
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHead5AccCode(productCategory.AccountingExpenseHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingExpenseHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 5,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "5 Layer"
            };
            _db.Head5.Add(head5_3);
            var catagoryForExpanse = _db.ProductSubCategories.SingleOrDefault(x => x.ProductSubCategoryId == productSubCategory.ProductSubCategoryId);
            catagoryForExpanse.AccountingExpenseHeadId = head5_3.Id;
            _db.SaveChanges();
            return result;

        }























        private int AccHeadGlPush(VMHeadIntegration vmModel, ProductSubCategory productSubCategory)
        {
            int result = -1;
            ProductCategory productCategory = _db.ProductCategories.Find(productSubCategory.ProductCategoryId);
            //List<Head5> head5s = new List<Head5>();
            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingHeadId.Value),

                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_1);
            var catagoryForAssets = _db.ProductSubCategories.SingleOrDefault(x => x.ProductSubCategoryId == productSubCategory.ProductSubCategoryId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            _db.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingIncomeHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingIncomeHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_2);
            var catagoryForIncome = _db.ProductSubCategories.SingleOrDefault(x => x.ProductSubCategoryId == productSubCategory.ProductSubCategoryId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            _db.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingExpenseHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingExpenseHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = _db.ProductSubCategories.SingleOrDefault(x => x.ProductSubCategoryId == productSubCategory.ProductSubCategoryId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            _db.SaveChanges();


            return result;

        }

        private int AccHeadGlBank(VMHeadIntegration vmModel, int bankbranchId)
        {
            int result = -1;
            //ProductCategory productCategory = _db.ProductCategories.Find(productSubCategory.ProductCategoryId);
            //List<Head5> head5s = new List<Head5>();

            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(vmModel.ParentId),

                AccName = vmModel.AccName,
                ParentId = vmModel.ParentId,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };
            _db.HeadGLs.Add(headGl_1);
            var bankbranch = _db.BankBranches.SingleOrDefault(x => x.BankBranchId == bankbranchId);
            bankbranch.AccountHeadId = headGl_1.Id;
            _db.SaveChanges();

            return result;
        }



        private int AccHeadGlPushSeed(VMHeadIntegration vmModel, int productCategoryId)
        {
            int result = -1;
            var head5idStock = _db.Head5.Where(x => x.AccCode == "1305001001" && x.IsActive == true && x.CompanyId == vmModel.CompanyFK).FirstOrDefault();
            var head5idIncome = _db.Head5.Where(x => x.AccCode == "3101001002" && x.IsActive == true && x.CompanyId == vmModel.CompanyFK).FirstOrDefault();
            var head5idExpanse = _db.Head5.Where(x => x.AccCode == "4101001001" && x.IsActive == true && x.CompanyId == vmModel.CompanyFK).FirstOrDefault();


            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(head5idStock.Id),

                AccName = vmModel.AccName,
                ParentId = head5idStock.Id,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_1);
            var catagoryForAssets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            _db.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(head5idIncome.Id), // 
                AccName = vmModel.AccName,
                ParentId = head5idIncome.Id,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_2);
            var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            _db.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(head5idExpanse.Id), // 
                AccName = vmModel.AccName,
                ParentId = head5idExpanse.Id,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            _db.SaveChanges();


            return result;

        }
        private int FinishAccHeadGlPushSeed(VMHeadIntegration vmModel, int productCategoryId)
        {
            int result = -1;
            var head5idStock = _db.Head5.Where(x => x.AccCode == "1305001002" && x.IsActive == true && x.CompanyId == vmModel.CompanyFK).FirstOrDefault();
            var head5idIncome = _db.Head5.Where(x => x.AccCode == "3101001001" && x.IsActive == true && x.CompanyId == vmModel.CompanyFK).FirstOrDefault();
            var head5idExpanse = _db.Head5.Where(x => x.AccCode == "4101001002" && x.IsActive == true && x.CompanyId == vmModel.CompanyFK).FirstOrDefault();


            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(head5idStock.Id),

                AccName = vmModel.AccName,
                ParentId = head5idStock.Id,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_1);
            var catagoryForAssets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            _db.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(head5idIncome.Id), // 
                AccName = vmModel.AccName,
                ParentId = head5idIncome.Id,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_2);
            var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            _db.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(head5idExpanse.Id), // 
                AccName = vmModel.AccName,
                ParentId = head5idExpanse.Id,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            _db.SaveChanges();


            return result;

        }
        private int AccHeadGlPushKPL(VMHeadIntegration vmModel, int productCategoryId)
        {
            int result = -1;
            //ProductCategory productCategory = _db.ProductCategories.Find(productSubCategory.ProductCategoryId);
            //List<Head5> head5s = new List<Head5>();
            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(50620373),

                AccName = vmModel.AccName,
                ParentId = 50620373,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_1);
            var catagoryForAssets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            _db.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(50620339), // 
                AccName = vmModel.AccName,
                ParentId = 50620339,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_2);
            var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            _db.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(50620336), // 
                AccName = vmModel.AccName,
                ParentId = 50620336,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            _db.SaveChanges();


            return result;

        }
        private int AccHeadGlPushKFMAL(VMHeadIntegration vmModel, int productCategoryId)
        {
            int result = -1;
            //ProductCategory productCategory = _db.ProductCategories.Find(productSubCategory.ProductCategoryId);
            //List<Head5> head5s = new List<Head5>();
            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(50615180),

                AccName = vmModel.AccName,
                ParentId = 50615180,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_1);
            var catagoryForAssets = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            _db.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(50615182), // 
                AccName = vmModel.AccName,
                ParentId = 50615182,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_2);
            var catagoryForIncome = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            _db.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(50615184), // 
                AccName = vmModel.AccName,
                ParentId = 50615184,
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = _db.ProductCategories.SingleOrDefault(x => x.ProductCategoryId == productCategoryId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            _db.SaveChanges();


            return result;

        }

        private int ProductHeadGlPush(VMHeadIntegration vmModel, Product product)
        {
            int result = -1;
            ProductCategory productCategory = _db.ProductCategories.Find(product.ProductCategoryId);
            //List<Head5> head5s = new List<Head5>();
            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingHeadId.Value),

                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_1);
            var catagoryForAssets = _db.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            _db.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingIncomeHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingIncomeHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_2);
            var catagoryForIncome = _db.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            _db.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingExpenseHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingExpenseHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = _db.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            _db.SaveChanges();


            return result;

        }
        private int KFMALProductHeadGlPush(VMHeadIntegration vmModel, Product product)
        {
            int result = -1;
            ProductSubCategory productCategory = _db.ProductSubCategories.Find(product.ProductSubCategoryId);
            //List<Head5> head5s = new List<Head5>();
            HeadGL headGl_1 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingHeadId.Value),

                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_1);
            var catagoryForAssets = _db.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            _db.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingIncomeHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingIncomeHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_2);
            var catagoryForIncome = _db.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            _db.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingExpenseHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingExpenseHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            _db.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = _db.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            _db.SaveChanges();


            return result;

        }

        private string GenerateHead4AccCode(int Head3Id)
        {
            var Head4 = _db.Head3.Where(x => x.Id == Head3Id).FirstOrDefault();


            var Head5DataList = _db.Head4.Where(x => x.ParentId == Head3Id).AsEnumerable();

            string newAccountCode = "";
            if (Head5DataList.Count() > 0)
            {
                string lastAccCode = Head5DataList.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 4);
                string childPart = lastAccCode.Substring(4, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');

            }
            else
            {
                newAccountCode = Head4.AccCode + "001";

            }
            return newAccountCode;
        }


        private string GenerateHead5AccCode(int Head4Id)
        {
            var Head4 = _db.Head4.Where(x => x.Id == Head4Id).FirstOrDefault();


            var Head5DataList = _db.Head5.Where(x => x.ParentId == Head4Id).AsEnumerable();

            string newAccountCode = "";
            if (Head5DataList.Count() > 0)
            {
                string lastAccCode = Head5DataList.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 7);
                string childPart = lastAccCode.Substring(7, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');

            }
            else
            {
                newAccountCode = Head4.AccCode + "001";

            }
            return newAccountCode;
        }
        private string GenerateHeadGlAccCode(int? Head5Id)
        {
            var Head5 = _db.Head5.Where(x => x.Id == Head5Id).FirstOrDefault();


            var HeadGlDataList = _db.HeadGLs.Where(x => x.ParentId == Head5Id).AsEnumerable();

            string newAccountCode = "";
            if (HeadGlDataList.Count() > 0)
            {
                string lastAccCode = HeadGlDataList.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 10);
                string childPart = lastAccCode.Substring(10, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');

            }
            else
            {
                newAccountCode = Head5.AccCode + "001";

            }
            return newAccountCode;
        }

        public async Task<int> ProductCategoryProjectEdit(VMCommonProductCategory vmCommonProductCategory)
        {
            var result = -1;
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    ProductCategory productCategory = await _db.ProductCategories.FindAsync(vmCommonProductCategory.ID);
                    productCategory.Name = vmCommonProductCategory.Name;
                    //productCategory.Code = vmCommonProductCategory.Code;
                    productCategory.LandSizeInKatha = vmCommonProductCategory.LandSizeInKatha;
                    productCategory.CashCustomerRate = vmCommonProductCategory.CashCommission;
                    productCategory.Address = vmCommonProductCategory.Description;
                    productCategory.NoOfFloors = vmCommonProductCategory.NoOfFloors;
                    productCategory.FlatSizeSpecification = vmCommonProductCategory.FlatSizeSpecification;
                    productCategory.TotalParking = vmCommonProductCategory.TotalParking;
                    productCategory.HandOverDate = vmCommonProductCategory.HandOverDate;
                    productCategory.TargetCostPerSqft = vmCommonProductCategory.TargetCostPerSqft;
                    productCategory.TotalFlat = vmCommonProductCategory.TotalFlat;

                    productCategory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    productCategory.ModifiedDate = DateTime.Now;
                    //_db.ProductCategories.Add(productCategory);
                    _db.Entry(productCategory).State = EntityState.Modified;
                    _db.SaveChanges();
                    result = productCategory.ProductCategoryId;
                    var h5 = _db.Head5.FirstOrDefault(f => f.Id == productCategory.AccountingHeadId);
                    h5.AccName = vmCommonProductCategory.Name;
                    _db.Entry(h5).State = EntityState.Modified;
                    _db.SaveChanges();
                    var hgl = _db.HeadGLs.FirstOrDefault(f => f.Id == productCategory.AccountingIncomeHeadId);
                    hgl.AccName = vmCommonProductCategory.Name;
                    _db.Entry(hgl).State = EntityState.Modified;
                    _db.SaveChanges();
                    scope.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return result;
                }
            }
        }


        public async Task<int> ProductCategoryProjectDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                ProductCategory productCategory = _db.ProductCategories.Find(id);
                productCategory.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = productCategory.ProductCategoryId;
                }
            }
            return result;
        }

        public realStateProducts checkProduct(int companyId, int categoryId = 0, int subCategoryId = 0, string name = "")
        {
            realStateProducts model = new realStateProducts();

            model = (from t1 in _db.Products.Where(x => x.CompanyId == companyId && x.IsActive == true)
                     where t1.ProductName == name.Trim() &&
                     t1.ProductCategoryId == categoryId &&
                     t1.ProductSubCategoryId == subCategoryId
                     select new realStateProducts
                     {

                         ID = t1.ProductId,
                         Name = t1.ProductName,
                         ShortName = t1.ShortName,
                         MRPPrice = t1.UnitPrice == null ? 0 : t1.UnitPrice.Value,
                         TPPrice = t1.TPPrice,
                         ProductType = t1.ProductType,
                         Code = t1.ProductCode,
                         ProcessLoss = t1.ProcessLoss,
                         Status = t1.Status,
                         FacingId = t1.FacingId,
                         JsonData = t1.JsonData,
                         PackSize = t1.PackSize,
                         Remarks = t1.Remarks
                     }).FirstOrDefault();
            return model;
        }

        public async Task<VMrealStateProductsForList> GetPlotOrFlat(int companyId, int categoryId = 0, int subCategoryId = 0)
        {
            VMrealStateProductsForList model = new VMrealStateProductsForList() { CompanyId = companyId };
            if (companyId == 7 || companyId == 9)
            {
                model.DataList = await (from t1 in _db.Products.Where(x => x.CompanyId == companyId)
                                        join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                        join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                        join t4 in _db.Units on t1.UnitId equals t4.UnitId
                                        join t5 in _db.FacingInfoes on t1.FacingId equals t5.FacingId
                                        where t1.IsActive && t2.IsActive && t3.IsActive &&
                                        ((categoryId > 0 && subCategoryId == 0) ? t2.ProductCategoryId == categoryId
                                           : (categoryId > 0 && subCategoryId > 0) ? t1.ProductSubCategoryId == subCategoryId
                                        : t1.ProductId > 0)
                                        select new realStateProducts
                                        {

                                            ID = t1.ProductId,
                                            Name = t1.ProductName,
                                            ShortName = t1.ShortName,
                                            MRPPrice = t1.UnitPrice == null ? 0 : t1.UnitPrice.Value,
                                            TPPrice = t1.TPPrice,
                                            SubCategoryName = t2.Name,
                                            CategoryName = t3.Name,
                                            UnitName = t4.Name,
                                            ProductType = t1.ProductType,
                                            Code = t1.ProductCode,
                                            ProcessLoss = t1.ProcessLoss,
                                            Status = t1.Status,
                                            FacingId = t1.FacingId,
                                            JsonData = t1.JsonData,
                                            FacingTitle = t5.Title,
                                            PackSize = t1.PackSize,
                                            Common_ProductCategoryFk = t3.ProductCategoryId,
                                            Common_ProductSubCategoryFk = t2.ProductSubCategoryId,
                                            Remarks = t1.Remarks
                                        }).OrderByDescending(x => x.ID).ToListAsync();
            }
            else
            {
                model.DataList = await (from t1 in _db.Products.Where(x => x.CompanyId == companyId)
                                        join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                        join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                        join t4 in _db.Units on t1.UnitId equals t4.UnitId
                                        join t5 in _db.FacingInfoes on t1.FacingId equals t5.FacingId
                                        where t1.IsActive && t2.IsActive && t3.IsActive &&
                                        ((categoryId > 0 && subCategoryId == 0) ? t2.ProductCategoryId == categoryId
                                        : (categoryId == 0 && subCategoryId > 0) ? t1.ProductSubCategoryId == subCategoryId
                                        : t1.ProductId > 0)
                                        select new realStateProducts
                                        {

                                            ID = t1.ProductId,
                                            Name = t1.ProductName,
                                            ShortName = t1.ShortName,
                                            MRPPrice = t1.UnitPrice == null ? 0 : t1.UnitPrice.Value,
                                            TPPrice = t1.TPPrice,
                                            SubCategoryName = t2.Name,
                                            CategoryName = t3.Name,
                                            UnitName = t4.Name,
                                            ProductType = t1.ProductType,
                                            Code = t1.ProductCode,
                                            ProcessLoss = t1.ProcessLoss,
                                            Status = t1.Status,
                                            FacingId = t1.FacingId,
                                            JsonData = t1.JsonData,
                                            FacingTitle = t5.Title,
                                            PackSize = t1.PackSize,
                                            Common_ProductCategoryFk = t3.ProductCategoryId,
                                            Remarks = t1.Remarks
                                        }).OrderByDescending(x => x.ID).ToListAsync();
            }


            //foreach (var item in model.DataList)
            //{
            //    if (companyId==9)
            //    {
            //        item.GetList = _db.ProductSubCategories.Where(e => e.ProductCategoryId == item.Common_ProductCategoryFk && e.CompanyId.Value == companyId && e.IsActive).
            //               Select(o => new SelectModelType
            //               {
            //                   Text = o.Name,
            //                   Value = o.ProductSubCategoryId
            //               }).ToList();
            //    }
            //}

            return model;
        }
        public async Task<VMrealStateProductsForList> GetPlotOrFlatforsalepersone(int companyId, int categoryId = 0, int subCategoryId = 0)
        {
            VMrealStateProductsForList model = new VMrealStateProductsForList() { CompanyId = companyId };
            int status = 0;
            string type = "";
            if (companyId == 7)
            {
                status = 474;
                type = "P";
            }
            if (companyId == 9)
            {
                status = 1523;
                type = "F";
            }

            model.DataList = await (from t1 in _db.Products.Where(x => x.CompanyId == companyId)
                                    join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                    join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                    join t4 in _db.Units on t1.UnitId equals t4.UnitId
                                    join t5 in _db.FacingInfoes on t1.FacingId equals t5.FacingId
                                    where t1.IsActive && t2.IsActive && t3.IsActive && t1.Status == status && t1.ProductType == type &&
                                    ((categoryId > 0 && subCategoryId == 0) ? t2.ProductCategoryId == categoryId
                                    : (categoryId > 0 && subCategoryId > 0) ? t1.ProductSubCategoryId == subCategoryId
                                    : t1.ProductId > 0)
                                    select new realStateProducts
                                    {

                                        ID = t1.ProductId,
                                        Name = t1.ProductName,
                                        ShortName = t1.ShortName,
                                        MRPPrice = t1.UnitPrice == null ? 0 : t1.UnitPrice.Value,
                                        TPPrice = t1.TPPrice,
                                        SubCategoryName = t2.Name,
                                        CategoryName = t3.Name,
                                        UnitName = t4.Name,
                                        ProductType = t1.ProductType,
                                        Code = t1.ProductCode,
                                        ProcessLoss = t1.ProcessLoss,
                                        Status = t1.Status,
                                        FacingId = t1.FacingId,
                                        JsonData = t1.JsonData,
                                        FacingTitle = t5.Title,
                                        PackSize = t1.PackSize,
                                        Common_ProductCategoryFk = t3.ProductCategoryId,
                                        Remarks = t1.Remarks
                                    }).OrderByDescending(x => x.ID).ToListAsync();


            return model;
        }
        public async Task<VMrealStateProductsForList> GetPlotOrFlatView(int companyId, int ProductId = 0)
        {
            VMrealStateProductsForList model = new VMrealStateProductsForList() { CompanyId = companyId };

            model.realStateProducts = await (from t1 in _db.Products.Where(x => x.CompanyId == companyId && x.ProductId == ProductId)
                                             join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                             join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                             join t4 in _db.Units on t1.UnitId equals t4.UnitId
                                             join t5 in _db.FacingInfoes on t1.FacingId equals t5.FacingId

                                             select new realStateProducts
                                             {
                                                 ID = t1.ProductId,
                                                 Name = t1.ProductName,
                                                 ShortName = t1.ShortName,
                                                 MRPPrice = t1.UnitPrice == null ? 0 : t1.UnitPrice.Value,
                                                 TPPrice = t1.TPPrice,
                                                 SubCategoryName = t2.Name,
                                                 CategoryName = t3.Name,
                                                 UnitName = t4.Name,
                                                 ProductType = t1.ProductType,
                                                 Code = t1.ProductCode,
                                                 ProcessLoss = t1.ProcessLoss,
                                                 Status = t1.Status,
                                                 FacingId = t1.FacingId,
                                                 JsonData = t1.JsonData,
                                                 FacingTitle = t5.Title,
                                                 PackSize = t1.PackSize,
                                                 Common_ProductCategoryFk = t3.ProductCategoryId
                                             }).FirstOrDefaultAsync();

            model.GetList = _db.ProductSubCategories.Where(e => e.ProductCategoryId == model.realStateProducts.Common_ProductCategoryFk && e.CompanyId.Value == companyId && e.IsActive).
               Select(o => new SelectModelType
               {
                   Text = o.Name,
                   Value = o.ProductSubCategoryId
               }).ToList();

            return model;
        }

        public async Task<string> FacingName(int id)
        {
            var name = await _db.FacingInfoes.FirstOrDefaultAsync(r => r.FacingId == id);
            return name.Title;
        }

        public async Task<Division> SaveDivision(Division Model)
        {


            var objToSave = _db.Divisions.Where(e => e.DivisionId == Model.DivisionId).FirstOrDefault();
            try
            {

                if (objToSave == null)
                {

                    Division commonDivisin = new Division
                    {
                        Name = Model.Name,

                    };

                    _db.Divisions.Add(commonDivisin);
                    await _db.SaveChangesAsync();

                }
                else
                {
                    objToSave.Name = Model.Name;

                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var mess = ex;
            }

            return Model;
        }

        public async Task<Division> GetDivisionById(int id)
        {

            var obj = await _db.Divisions.SingleOrDefaultAsync(e => e.DivisionId == id);
            return new Division() { DivisionId = obj.DivisionId, Name = obj.Name };
        }

        [HttpPost]
        public async Task<District> SaveDistrict(District Model)
        {

            var obj = _db.Districts.OrderByDescending(x => x.Code).FirstOrDefault();
            var objToSave = _db.Districts.Where(e => e.IsActive && e.DistrictId == Model.DistrictId).FirstOrDefault();
            try
            {

                var codeee = Convert.ToInt32(obj.Code);
                var codee = codeee + 1;
                if (objToSave == null)
                {
                    District commonDistrict = new District
                    {
                        Name = Model.Name,
                        ShortName = Model.ShortName,
                        DivisionId = Model.DivisionId,
                        Code = Convert.ToString(codee),
                        IsActive = true
                    };
                    _db.Districts.Add(commonDistrict);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    objToSave.Name = Model.Name;
                    objToSave.ShortName = Model.ShortName;
                    objToSave.DivisionId = Model.DivisionId;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var mess = ex;
            }

            return Model;
        }
        public async Task<District> GetDistrictById(int id)
        {

            var obj = await _db.Districts.SingleOrDefaultAsync(e => e.DistrictId == id);
            return new District() { DistrictId = obj.DistrictId, Name = obj.Name, ShortName = obj.ShortName, DivisionId = obj.DivisionId };
        }
        public async Task<bool> DeleteDistrict(int id)
        {
            var model = new District();
            var obj = await _db.Districts.SingleOrDefaultAsync(e => e.DistrictId == id);
            if (obj == null)
            {
                return false;
            }
            obj.IsActive = false;
            await _db.SaveChangesAsync();

            return true;

        }

        public async Task<Upazila> GetUpazilaById(int id)
        {

            var obj = await _db.Upazilas.SingleOrDefaultAsync(e => e.UpazilaId == id);
            return new Upazila() { UpazilaId = obj.UpazilaId, Name = obj.Name, ShortName = obj.ShortName, DistrictId = obj.DistrictId };
        }
        public async Task<ProductCategory> GetProjectDetailsById(int id)
        {

            var obj = await _db.ProductCategories.SingleOrDefaultAsync(e => e.ProductCategoryId == id);
            return new ProductCategory() { ProductCategoryId = obj.ProductCategoryId, Name = obj.Name, Address = obj.Address, LandSizeInKatha = obj.LandSizeInKatha, NoOfFloors = obj.NoOfFloors, FlatSizeSpecification = obj.FlatSizeSpecification, TotalParking = obj.TotalParking, HandOverDate = obj.HandOverDate, TargetCostPerSqft = obj.TargetCostPerSqft, TotalFlat = obj.TotalFlat };
        }


        public async Task<bool> DeleteUpazila(int id)
        {
            var model = new Upazila();
            var obj = await _db.Upazilas.SingleOrDefaultAsync(e => e.UpazilaId == id);
            if (obj == null)
            {
                return false;
            }
            obj.IsActive = false;
            await _db.SaveChangesAsync();

            return true;
        }



        public async Task<Upazila> SaveUpazila(Upazila Model)
        {

            var obj = _db.Upazilas.OrderByDescending(x => x.Code).FirstOrDefault();
            var objToSave = _db.Upazilas.Where(e => e.IsActive && e.UpazilaId == Model.UpazilaId).FirstOrDefault();
            try
            {

                var codeee = Convert.ToInt32(obj.Code);
                var codee = codeee + 1;
                if (objToSave == null)
                {

                    Upazila commonCustomer = new Upazila
                    {
                        Name = Model.Name,
                        ShortName = Model.ShortName,
                        DistrictId = Model.DistrictId,
                        Code = Convert.ToString(codee),
                        FacCarryingCommission = Convert.ToDecimal(1.20),
                        DepoCarryingCommission = Convert.ToDecimal(1.20),
                        IsActive = true
                    };

                    _db.Upazilas.Add(commonCustomer);
                    await _db.SaveChangesAsync();

                }
                else
                {
                    objToSave.Name = Model.Name;
                    objToSave.ShortName = Model.ShortName;
                    objToSave.DistrictId = Model.DistrictId;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var mess = ex;
            }

            return Model;
        }

        public async Task<CurrencyVm> GetCurrency()
        {
            CurrencyVm model = new CurrencyVm();
            model.DataList = await (from t1 in _db.Currencies
                                    where t1.IsActive
                                    select new CurrencyVm
                                    {
                                        CurrencyId = t1.CurrencyId,
                                        Name = t1.Name,
                                        Sign = t1.Sign,
                                        CurrencyRate = t1.CurrencyRate
                                    }).ToListAsync();

            return model;
        }
        public async Task<PortOfCountryVm> GetPortOfCountry()
        {
            PortOfCountryVm model = new PortOfCountryVm();
            model.DataList = await (from t1 in _db.PortOfCountries
                                    join t2 in _db.Countries on
                                    t1.CountryId equals t2.CountryId
                                    where t1.IsActive
                                    select new PortOfCountryVm
                                    {
                                        PortOfCountryId = t1.PortOfCountryId,
                                        PortName = t1.PortName,

                                        countryName = t2.CountryName
                                    }).ToListAsync();

            return model;
        }



        public async Task<CurrencyVm> GetCurencyForUpdate(int? id)
        {
            CurrencyVm model = new CurrencyVm();
            model = await (from t1 in _db.Currencies
                           where t1.CurrencyId == id
                           select new CurrencyVm
                           {
                               CurrencyId = t1.CurrencyId,
                               Name = t1.Name,
                               Sign = t1.Sign,
                               CurrencyRate = t1.CurrencyRate
                           }).FirstOrDefaultAsync();

            return model;
        }
        public async Task<PortOfCountryVm> GetPortCountryForUpdate(int? id)
        {
            PortOfCountryVm model = new PortOfCountryVm();
            model = await (from t1 in _db.PortOfCountries
                           join t2 in _db.Countries
                           on t1.CountryId equals t2.CountryId
                           where t1.PortOfCountryId == id
                           select new PortOfCountryVm
                           {
                               PortOfCountryId = t1.PortOfCountryId,
                               PortName = t1.PortName,
                               CountryId = t1.CountryId
                           }).FirstOrDefaultAsync();
            model.Countrylist = await _db.Countries.ToListAsync();
            return model;
        }
        public async Task<bool> DeleteFile(long docid)
        {
            var file = await _db.FileArchives.SingleAsync(e => e.docid == docid);
            if (file != null)
            {

                try
                {
                    file.isactive = false;
                    await _db.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else return false;
        }



        public async Task<List<vwFTPFileInfo>> GetAllFilesByCatagory(DateTime? StartDate, DateTime? ToDate, string selectTitle)
        {
            if (StartDate == null)
            {
                StartDate = DateTime.Today.AddMonths(-2);
            }
            if (ToDate == null)
            {
                ToDate = DateTime.Today;
            }
            var query = _db.vwFTPFileInfoes
    .Where(o => o.FileCatagoryId == 10 && o.RecDate >= StartDate && o.RecDate <= ToDate);

            if (!string.IsNullOrWhiteSpace(selectTitle))
            {
                query = query.Where(o => o.docdesc == selectTitle);
            }

            var result = await query.OrderByDescending(o => o.docid).ToListAsync();
            return result;
        }

        public async Task<List<vwFTPFileInfo>> GetAllFilesByCatagoryErp(DateTime? StartDate, DateTime? ToDate)
        {
            if (StartDate == null)
            {
                StartDate = DateTime.Today.AddMonths(-1);
            }
            if (ToDate == null)
            {
                ToDate = DateTime.Today;
            }



            return await _db.vwFTPFileInfoes.Where(o => o.FileCatagoryId == 11 && o.RecDate >= StartDate && o.RecDate <= ToDate).OrderByDescending(o => o.docid).ToListAsync();
        }

        public async Task<PortOfCountryVm> GetCountry()
        {
            PortOfCountryVm model = new PortOfCountryVm();
            model.Countrylist = await _db.Countries.ToListAsync();
            return model;

        }
        public async Task<CurrencyVm> SaveOrUpdateCurrency(CurrencyVm model)
        {
            var ObjTosave = _db.Currencies.Where(t => t.CurrencyId == model.CurrencyId).FirstOrDefault();


            try
            {
                if (ObjTosave == null)
                {
                    Currency vm = new Currency()
                    {
                        Name = model.Name,
                        Sign = model.Sign,
                        CurrencyRate = model.CurrencyRate,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        IsActive = true

                    };

                    _db.Currencies.Add(vm);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    ObjTosave.Name = model.Name;
                    ObjTosave.Sign = model.Sign;
                    ObjTosave.CurrencyRate = model.CurrencyRate;
                    ObjTosave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    ObjTosave.ModifiedDate = DateTime.Now;
                    await _db.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {
                var message = ex;
            }

            return model;
        }
        public async Task<PortOfCountryVm> SaveOrUpdatePortofCountry(PortOfCountryVm model)
        {
            var ObjTosave = _db.PortOfCountries.Where(t => t.PortOfCountryId == model.PortOfCountryId).FirstOrDefault();


            try
            {
                if (ObjTosave == null)
                {
                    PortOfCountry vm = new PortOfCountry()
                    {
                        PortName = model.PortName,
                        CountryId = model.CountryId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        IsActive = true

                    };

                    _db.PortOfCountries.Add(vm);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    ObjTosave.PortName = model.PortName;
                    ObjTosave.CountryId = model.CountryId;
                    ObjTosave.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    ObjTosave.ModifiedDate = DateTime.Now;
                    await _db.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {
                var message = ex;
            }

            return model;
        }
        public bool DeleteCurrency(int Id)
        {
            var obj = _db.Currencies.Where(t => t.CurrencyId == Id).SingleOrDefault();
            obj.IsActive = false;

            if (_db.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<SelectModelType>> VendorCompanyWise(int companyId)
        {
            var list = await _db.Vendors
              .Where(s => s.CompanyId == companyId && s.VendorTypeId == 2)
              .Select(e => new SelectModelType
              {
                  Text = e.Name,
                  Value = e.VendorId // Explicitly cast to object
              }).ToListAsync();

            return list;
        }



        public List<SelectModelSaleTitle> GetActiveSalesTitles(int companyId)
        {
            var list = _db.KGSalesAchivements
                .Where(x => x.IsActive && x.CompanyId== companyId)
                .Select(s => new SelectModelSaleTitle
                {
                    Value = s.KGSalesId,
                    Text = s.Title
                })
                .ToList();

            return list;
        }


        public bool DeletePortOfCountry(int Id)
        {
            var obj = _db.PortOfCountries.Where(t => t.PortOfCountryId == Id).SingleOrDefault();
            obj.IsActive = false;
            if (_db.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }


        public List<string> GetLotNumber(int ProductId)
        {
            var list = _db.MaterialReceiveDetails
                .Where(y => y.IsActive && !string.IsNullOrEmpty(y.LotNumber) && y.LotNumber != "Null" && y.ProductId== ProductId)
                .Select(x => x.LotNumber)
                .Distinct()  // Ensures that LotNumbers are unique
                .ToList();

            return list;
        }

        //public List<string> GetLotNumberFinish(int ProductId)
        //{
        //    var list = _db.Prod_ReferenceSlave
        //        .Where(y => y.IsActive && !string.IsNullOrEmpty(y.LotNumber) && y.LotNumber != "Null" && y.FProductId == ProductId)
        //        .Select(x => x.LotNumber)
        //        .Distinct()  // Ensures that LotNumbers are unique
        //        .ToList();

        //    return list;
        //}


        public List<string> GetLotNumberFinish(int ProductId)
        {
            // Query for lot numbers from Prod_ReferenceSlave
            var referenceSlaveLotNumbers = _db.Prod_ReferenceSlave
                .Where(y => y.IsActive && !string.IsNullOrEmpty(y.LotNumber) && y.LotNumber != "Null" && y.FProductId == ProductId)
                .Select(x => x.LotNumber)
                .Distinct();

            // Query for lot numbers from MaterialReceiveDetail
            var materialReceiveDetailLotNumbers = _db.MaterialReceiveDetails
                .Where(y => y.IsActive && !string.IsNullOrEmpty(y.LotNumber) && y.LotNumber != "Null" && y.ProductId == ProductId)
                .Select(x => x.LotNumber)
                .Distinct();

            // Combine both lists using Union to avoid duplicates
            var combinedLotNumbers = referenceSlaveLotNumbers
                .Union(materialReceiveDetailLotNumbers)
                .ToList();

            return combinedLotNumbers;
        }


        #region Incentives
        public async Task<VMIncentive> GetIncentives(int companyId)
        {
            VMIncentive vmIncentive = new VMIncentive();
            vmIncentive.CompanyFK = companyId;
            vmIncentive.DataList = await Task.Run(() => (from t1 in _db.Incentives
                                                         where t1.IsActive && t1.CompanyId == companyId
                                                         // productCategoryId > 0 ? t1.ProductCategoryId == productCategoryId: t1.ProductCategoryId > 0
                                                         select new VMIncentive
                                                         {
                                                             CompanyId = t1.CompanyId,
                                                             IncentiveId = t1.IncentiveId,
                                                             IncentiveType = t1.IncentiveType,
                                                             IsActive = t1.IsActive

                                                         }).OrderByDescending(x => x.IncentiveId).AsEnumerable());
            return vmIncentive;
        }
        public async Task<int> IncentiveAdd(VMIncentive vmIncentive)
        {
            var result = -1;
            Incentive incentive = new Incentive
            {
                IncentiveType = vmIncentive.IncentiveType,
                CompanyId = vmIncentive.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true

            };
            _db.Incentives.Add(incentive);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = incentive.IncentiveId;
            }
            return result;
        }
        public async Task<int> IncentiveEdit(VMIncentive vmIncentive)
        {
            var result = -1;
            Incentive incentive = _db.Incentives.Find(vmIncentive.IncentiveId);
            incentive.IncentiveType = vmIncentive.IncentiveType;

            vmIncentive.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            vmIncentive.ModifiedDate = DateTime.Now;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = vmIncentive.IncentiveId;
            }
            return result;
        }
        public async Task<int> IncentiveDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                Incentive incentive = _db.Incentives.Find(id);
                incentive.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = incentive.IncentiveId;
                }
            }
            return result;
        }

        #endregion


        #region Incentives
        public async Task<VMIncentiveDetails> GetIncentiveDetails(int companyId, int incentiveId)
        {
            VMIncentiveDetails vmIncentiveDetails = new VMIncentiveDetails();
            vmIncentiveDetails = await Task.Run(() => (from t1 in _db.Incentives
                                                       where t1.IsActive && t1.CompanyId == companyId
                                                        && t1.IncentiveId == incentiveId
                                                       select new VMIncentiveDetails
                                                       {

                                                           CompanyId = t1.CompanyId,
                                                           IncentiveId = t1.IncentiveId,
                                                           IncentiveType = t1.IncentiveType,
                                                           IsActive = t1.IsActive
                                                       }).FirstOrDefault()); ;
            vmIncentiveDetails.DataListDetails = await Task.Run(() => (from t1 in _db.IncentiveDetails.Where(h => h.IsActive
                                                                       )
                                                                       join t2 in _db.Incentives on t1.IncentiveId equals t2.IncentiveId
                                                                       where t1.IsActive == true && t2.IsActive == true && t2.CompanyId == companyId
                                                                        && incentiveId > 0 ? t1.IncentiveId == incentiveId : t1.IncentiveId > 0
                                                                       select new VMIncentiveDetails
                                                                       {
                                                                           MinQty = t1.MinQty,
                                                                           MaxQty = t1.MaxQty,
                                                                           Rate = t1.Rate,
                                                                           IncentiveDetailId = t1.IncentiveDetailId,
                                                                           CompanyId = t2.CompanyId,
                                                                           IncentiveId = t1.IncentiveId,
                                                                           IncentiveType = t2.IncentiveType,
                                                                           IsActive = t2.IsActive
                                                                       }).OrderByDescending(x => x.IncentiveDetailId).AsEnumerable());
            return vmIncentiveDetails;
        }
        public async Task<int> IncentiveDetailsAdd(VMIncentiveDetails vmIncentiveDetails)
        {
            var result = -1;
            IncentiveDetail incentiveDetail = new IncentiveDetail
            {
                MinQty = vmIncentiveDetails.MinQty,
                MaxQty = vmIncentiveDetails.MaxQty,
                Rate = vmIncentiveDetails.Rate,
                IncentiveId = vmIncentiveDetails.IncentiveId,
                IsActive = true

            };
            _db.IncentiveDetails.Add(incentiveDetail);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = incentiveDetail.IncentiveDetailId;
            }
            return result;
        }
        public async Task<int> IncentiveDetailsEdit(VMIncentiveDetails vmIncentiveDetails)
        {
            var result = -1;
            IncentiveDetail incentiveDetail = _db.IncentiveDetails.Find(vmIncentiveDetails.IncentiveDetailId);
            incentiveDetail.MinQty = vmIncentiveDetails.MinQty;
            incentiveDetail.MaxQty = vmIncentiveDetails.MaxQty;
            incentiveDetail.Rate = vmIncentiveDetails.Rate;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = incentiveDetail.IncentiveId;
            }
            return result;
        }
        public async Task<int> IncentiveDetailsDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                IncentiveDetail incentiveDetail = _db.IncentiveDetails.Find(id);
                incentiveDetail.IsActive = false;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = incentiveDetail.IncentiveId;
                }
            }
            return result;
        }
        #endregion

        #region URLInfo
        public void SaveUrl(UrlInfo urlInfo)
        {
            if (urlInfo.UrlId == 0)
            {
                UrlInfo urldb = new UrlInfo
                {

                    Url = urlInfo.Url,
                    CompanyId = urlInfo.CompanyId,
                    UrlType = urlInfo.UrlType,
                    IsActive = true
                };
                _db.UrlInfoes.Add(urldb);
            }
            else
            {
                var existingUrl = _db.UrlInfoes.Find(urlInfo.UrlId);
                if (existingUrl != null)
                {
                    existingUrl.Url = urlInfo.Url;
                    existingUrl.UrlType = urlInfo.UrlType;
                    existingUrl.CompanyId = urlInfo.CompanyId;
                }
            }
            _db.SaveChanges();
        }
        public List<UrlViewModel> GetAllUrls()
        {
            var result = (from url in _db.UrlInfoes
                          join company in _db.Companies on url.CompanyId equals company.CompanyId
                          where url.IsActive == true
                          select new UrlViewModel
                          {
                              UrlId = url.UrlId,
                              Url = url.Url,
                              UrlType = url.UrlType,
                              CompanyId = url.CompanyId,
                              CompanyName = company.Name
                          }).ToList();

            return result;
        }

        public UrlInfo GetUrlById(int urlId)
        {
            return _db.UrlInfoes.Find(urlId);
        }

        public void DeleteUrl(int urlId)
        {
            var url = _db.UrlInfoes.Find(urlId);
            if (url != null)
            {
                url.IsActive = false;
                _db.SaveChanges();
            }
        }
        #endregion

    }



}

