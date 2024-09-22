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
    public class CompanyUserMenuService : ICompanyUserMenuService
    {
        private readonly ERPEntities context;
        public CompanyUserMenuService(ERPEntities context)
        {
            this.context = context;
        }

        public List<CompanyUserMenuModel> GetCompanyUserMenus(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return new List<CompanyUserMenuModel>();
            }

            else
            {
                IQueryable<CompanyUserMenuModel> queryable = context.Database.SqlQuery<CompanyUserMenuModel>(@"
                                                             select 
                                                             CompanyUserMenuId,
                                                             (select Name from Company where CompanyId=CompanyUserMenu.CompanyId) as CompanyName,
                                                             (select Name from CompanyMenu where CompanyMenuId=CompanyUserMenu.CompanyMenuId) as CompanyMenuName,
                                                             (select Name from CompanySubMenu where CompanySubMenuId=CompanyUserMenu.CompanySubMenuId) as CompanySubMenuName,
                                                             UserId,IsActive
                                                              from CompanyUserMenu where UserId={0}", searchText.ToUpper()).AsQueryable();
                return queryable.Where(x => x.UserId.ToLower().Contains(searchText.ToLower())).ToList();
            }

        }
        public CompanyUserMenuModel GetCompanyUserMenu(int id)
        {
            if (id <= 0)
            {
                return new CompanyUserMenuModel() { IsActive = true };
            }
            CompanyUserMenu companyUserMenu = context.CompanyUserMenus.FirstOrDefault(x => x.CompanyUserMenuId == id);
            return ObjectConverter<CompanyUserMenu, CompanyUserMenuModel>.Convert(companyUserMenu);
        }



        public bool SaveCompanyUserMenu(long id, CompanyUserMenuModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("User Menu data missing!");
            }
            bool isMenuAlreadyAssign = context.CompanyUserMenus.Any(x => x.CompanyId == model.CompanyId && x.CompanyMenuId == model.CompanyMenuId && x.CompanySubMenuId == model.CompanySubMenuId && x.UserId.Equals(model.UserId) && x.CompanyUserMenuId != model.CompanyUserMenuId);

            if (isMenuAlreadyAssign)
            {
                message = "This menu has already assigned to user";
                return false;
            }
            CompanyUserMenu companyUserMenu = ObjectConverter<CompanyUserMenuModel, CompanyUserMenu>.Convert(model);
            if (id > 0)
            {
                companyUserMenu = context.CompanyUserMenus.FirstOrDefault(x => x.CompanyUserMenuId == id);
                if (companyUserMenu == null)
                {
                    throw new Exception("User Menu not found!");
                }
                companyUserMenu.ModifiedDate = DateTime.Now;
                companyUserMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {

                companyUserMenu.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                companyUserMenu.CreatedDate = DateTime.Now;
            }

            companyUserMenu.CompanyId = model.CompanyId;
            companyUserMenu.CompanyMenuId = model.CompanyMenuId;
            companyUserMenu.CompanySubMenuId = model.CompanySubMenuId;
            companyUserMenu.UserId = model.UserId;
            companyUserMenu.IsView = model.IsView;
            companyUserMenu.IsAdd = model.IsAdd;
            companyUserMenu.IsUpdate = model.IsUpdate;
            companyUserMenu.IsDelete = model.IsDelete;
            companyUserMenu.IsActive = model.IsActive;

            context.CompanyUserMenus.Add(companyUserMenu);
            context.Entry(companyUserMenu).State = companyUserMenu.CompanyUserMenuId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteCompanyUserMenu(int? id)
        {
            CompanyUserMenu companyUserMenu = context.CompanyUserMenus.Find(id);
            if (companyUserMenu == null)
            {
                return false;
            }
            context.CompanyUserMenus.Remove(companyUserMenu);
            return context.SaveChanges() > 0;
        }


    }
}
