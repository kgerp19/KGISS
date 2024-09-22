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
    public class CompanyMenuService : ICompanyMenuService
    {
        private readonly ERPEntities context;
        public CompanyMenuService(ERPEntities context)
        {
            this.context = context;
        }
        public List<CompanyMenuModel> GetCompanyMenus(string searchText)
        {
            IQueryable<CompanyMenu> queryable = context.CompanyMenus.Include(x => x.Company).Where(x => x.IsActive);
            return ObjectConverter<CompanyMenu, CompanyMenuModel>.ConvertList(queryable.ToList()).ToList();
        }

        public CompanyMenuModel GetCompanyMenu(int id)
        {
            if (id <= 0)
            {
                return new CompanyMenuModel() { IsActive = true };
            }
            CompanyMenu companyMenu = context.CompanyMenus.FirstOrDefault(x => x.CompanyMenuId == id);
            return ObjectConverter<CompanyMenu, CompanyMenuModel>.Convert(companyMenu);
        }



        public bool SaveCompanyMenu(int id, CompanyMenuModel model)
        {
            if (model == null)
            {
                throw new Exception("Company Menu data missing!");
            }

            CompanyMenu companyMenu = ObjectConverter<CompanyMenuModel, CompanyMenu>.Convert(model);
            if (id > 0)
            {
                companyMenu = context.CompanyMenus.FirstOrDefault(x => x.CompanyMenuId == id);
                if (companyMenu == null)
                {
                    throw new Exception("Company Menu not found!");
                }
                companyMenu.ModifiedDate = DateTime.Now;
                companyMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;

                companyMenu.CompanyId = model.CompanyId;
                companyMenu.Name = model.Name;
                companyMenu.ShortName = model.Name;
                companyMenu.OrderNo = model.OrderNo;
                companyMenu.IsActive = model.IsActive;
                return context.SaveChanges() > 0;
            }

            else
            {
                companyMenu.CompanyMenuId = context.Database.SqlQuery<int>("exec spGetNewCompanyId").FirstOrDefault();
                companyMenu.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                companyMenu.CreatedDate = DateTime.Now;

                companyMenu.CompanyId = model.CompanyId;
                companyMenu.Name = model.Name;
                companyMenu.ShortName = model.Name;
                companyMenu.OrderNo = model.OrderNo;
                companyMenu.IsActive = model.IsActive;

                context.CompanyMenus.Add(companyMenu);
                return context.SaveChanges() > 0;
            }

        }



        public List<SelectModel> GetCompanyMenuSelectModelsByCompanyId(int? companyId)
        {
            IQueryable<CompanyMenu> queryable = context.CompanyMenus.Where(x => x.CompanyId == companyId);
            return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.CompanyMenuId }).OrderBy(x => x.Text).ToList();
        }

        public List<SelectModel> GetCompanyMenuSelectModelsByCompany(int companyId)
        {
            IQueryable<CompanyMenu> queryable = context.CompanyMenus.Where(x => x.CompanyId == companyId);
            return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.CompanyMenuId }).OrderBy(x => x.Text).ToList();
        }



        //public List<MenuModel> GetMenus(int companyId, string searchText)
        //{
        //    IQueryable<Menu> queryable = context.Menus.AsQueryable();
        //    return ObjectConverter<Menu, MenuModel>.ConvertList(queryable.ToList()).ToList();
        //}

        //public List<ProductSubCategoryModel> GetProductSubCategories()
        //{
        //    IQueryable<ProductSubCategory> queryable = context.ProductSubCategories;
        //    return ObjectConverter<ProductSubCategory, ProductSubCategoryModel>.ConvertList(queryable.ToList()).ToList();
        //}

        //public List<ProductSubCategoryModel> GetProductSubCategories(int companyId,string type, string searchText)
        //{
        //    IQueryable<ProductSubCategory> queryable = context.ProductSubCategories.Include(x => x.ProductCategory).Where(x => x.ProductCategory.CompanyId==companyId && x.ProductCategory.ProductType.Equals(type) && ( x.ProductCategory.Name.Contains(searchText) || x.Name.Contains(searchText))).OrderByDescending(x=>x.ProductSubCategoryId);
        //    return ObjectConverter<ProductSubCategory, ProductSubCategoryModel>.ConvertList(queryable.ToList()).ToList();
        //}

        //public ProductSubCategoryModel GetProductSubCategory(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return new ProductSubCategoryModel() { ProductSubCategoryId = id,IsActive=true };
        //    }
        //    ProductSubCategory productSubCategory = context.ProductSubCategories.FirstOrDefault(x => x.ProductSubCategoryId == id);

        //    return ObjectConverter<ProductSubCategory, ProductSubCategoryModel>.Convert(productSubCategory);
        //}



        //public bool SaveProductSubCategory(int id, ProductSubCategoryModel model)
        //{
        //    if (model == null)
        //    {
        //        throw new Exception("Product Sub Category data missing!");
        //    }

        //    ProductSubCategory productSubCategory = ObjectConverter<ProductSubCategoryModel, ProductSubCategory>.Convert(model);
        //    if (id > 0)
        //    {
        //        productSubCategory = context.ProductSubCategories.FirstOrDefault(x => x.ProductSubCategoryId == id);
        //        if (productSubCategory == null)
        //        {
        //            throw new Exception("Product Sub Category not found!");
        //        }
        //        productSubCategory.ModifiedDate = DateTime.Now;
        //        productSubCategory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //    }

        //    else
        //    {
        //        productSubCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //        productSubCategory.CreatedDate = DateTime.Now;
        //        productSubCategory.IsActive = true;
        //    }

        //    productSubCategory.ProductCategoryId = model.ProductCategoryId;
        //    productSubCategory.Name = model.Name;
        //    productSubCategory.BaseCommissionRate = model.BaseCommissionRate;
        //    productSubCategory.Remarks = model.Remarks;
        //    productSubCategory.OrderNo = model.OrderNo;

        //    context.Entry(productSubCategory).State = productSubCategory.ProductSubCategoryId == 0 ? EntityState.Added : EntityState.Modified;
        //    return context.SaveChanges() > 0;
        //}

        //public List<SelectModel> GetProductSubCategorySelectModelsByProductCategory(int productCategoryId)
        //{
        //    IQueryable<ProductSubCategory> queryable = context.ProductSubCategories.Where(x => x.ProductCategoryId == productCategoryId);
        //    return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.ProductSubCategoryId }).OrderBy(x=>x.Text).ToList();
        //}

        //public bool DeleteProductSubCategory(int id)
        //{
        //    ProductSubCategory productSubCategory = context.ProductSubCategories.Find(id);
        //    context.ProductSubCategories.Remove(productSubCategory);
        //    return context.SaveChanges() > 1;
        //}


    }
}
