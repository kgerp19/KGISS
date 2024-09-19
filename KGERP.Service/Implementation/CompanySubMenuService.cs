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
    public class CompanySubMenuService : ICompanySubMenuService
    {
        private readonly ERPEntities context;
        public CompanySubMenuService(ERPEntities context)
        {
            this.context = context;
        }

        public List<CompanySubMenuModel> GetCompanySubMenus(string searchText)
        {

            IQueryable<CompanySubMenu> queryable = context.CompanySubMenus.Include(x => x.CompanyMenu.Company).
                Where(x => (x.Company.Name.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                (x.Name.ToLower().Contains(searchText.ToLower()) || x.Controller.Contains(searchText))).OrderBy(x => x.CompanyId);
            return ObjectConverter<CompanySubMenu, CompanySubMenuModel>.ConvertList(queryable.ToList()).ToList();
        }
        public CompanySubMenuModel GetCompanySubMenu(int id)
        {
            if (id <= 0)
            {
                return new CompanySubMenuModel() { IsActive = true };
            }
            CompanySubMenu companySubMenu = context.CompanySubMenus.FirstOrDefault(x => x.CompanySubMenuId == id);
            return ObjectConverter<CompanySubMenu, CompanySubMenuModel>.Convert(companySubMenu);
        }

        public bool SaveCompanySubMenu(int id, CompanySubMenuModel model)
        {
            if (model == null)
            {
                throw new Exception("Company Sub Menu data missing!");
            }

            CompanySubMenu companySubMenu = ObjectConverter<CompanySubMenuModel, CompanySubMenu>.Convert(model);


            if (id > 0)
            {
                companySubMenu = context.CompanySubMenus.FirstOrDefault(x => x.CompanySubMenuId == id);
                if (companySubMenu == null)
                {
                    throw new Exception("Company Sub Menu not found!");
                }
                companySubMenu.ModifiedDate = DateTime.Now;
                companySubMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;

                companySubMenu.CompanyId = model.CompanyId;
                companySubMenu.CompanyMenuId = model.CompanyMenuId;
                companySubMenu.Name = model.Name;
                companySubMenu.ShortName = model.Name;
                companySubMenu.OrderNo = model.OrderNo;
                companySubMenu.Controller = model.Controller;
                companySubMenu.Action = model.Action;
                companySubMenu.Param = model.Param;
                companySubMenu.IsActive = model.IsActive;

                return context.SaveChanges() > 0;
            }

            else
            {
                companySubMenu.CompanySubMenuId = context.Database.SqlQuery<int>("exec spGetNewCompanyId").FirstOrDefault();
                companySubMenu.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                companySubMenu.CreatedDate = DateTime.Now;

                companySubMenu.CompanyId = model.CompanyId;
                companySubMenu.CompanyMenuId = model.CompanyMenuId;
                companySubMenu.Name = model.Name;
                companySubMenu.ShortName = model.Name;
                companySubMenu.OrderNo = model.OrderNo;
                companySubMenu.Controller = model.Controller;
                companySubMenu.Action = model.Action;
                companySubMenu.Param = model.Param;
                companySubMenu.IsActive = model.IsActive;

                context.CompanySubMenus.Add(companySubMenu);
                return context.SaveChanges() > 0;
            }
        }

        public List<SelectModel> GetCompanySubMenuSelectModelsByCompanyMenu(int menuId)
        {
            List<CompanySubMenu> companySubMenus = context.CompanySubMenus.Where(x => x.CompanyMenuId == menuId).ToList();

            return companySubMenus.Select(x => new SelectModel { Text = x.Name, Value = x.CompanySubMenuId }).ToList();
        }

        public bool DeleteCompanySubMenu(int? id)
        {
            CompanySubMenu companySubMenu = context.CompanySubMenus.Find(id);
            if (companySubMenu == null)
            {
                return false;
            }
            context.CompanySubMenus.Remove(companySubMenu);
            return context.SaveChanges() > 0;
        }


    }
}
