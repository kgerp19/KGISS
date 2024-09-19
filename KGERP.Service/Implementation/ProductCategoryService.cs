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
    public class ProductCategoryService : IProductCategoryService
    {

        private bool disposed = false;
        private readonly ERPEntities context;

        public ProductCategoryService(ERPEntities context)
        {
            this.context = context;
        }


        public ProductCategoryModel GetProductCategory(int id, string productType)
        {
            if (id == 0)
            {
                return new ProductCategoryModel()
                {
                    ProductCategoryId = id,
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]),
                    ProductType = productType,
                    IsActive = true
                };
            }
            ProductCategory productCategory = context.ProductCategories.Find(id);
            return ObjectConverter<ProductCategory, ProductCategoryModel>.Convert(productCategory);
        }

        public bool SaveProductCategory(int id, ProductCategoryModel model)
        {
            if (model == null)
            {
                throw new Exception("Product Category data missing!");
            }

            bool exist = context.ProductCategories.Where(x => x.Name.Equals(model.Name) && x.CompanyId == model.CompanyId && x.ProductCategoryId != id).Any();

            if (exist)
            {
                throw new Exception("Product Category already exist!");
            }
            ProductCategory productCategory = ObjectConverter<ProductCategoryModel, ProductCategory>.Convert(model);
            if (id > 0)
            {
                productCategory = context.ProductCategories.FirstOrDefault(x => x.ProductCategoryId == id);
                if (productCategory == null)
                {
                    throw new Exception("Product Category not found!");
                }
                productCategory.ModifiedDate = DateTime.Now;
                productCategory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                productCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productCategory.CreatedDate = DateTime.Now;
                productCategory.IsActive = true;
            }
            productCategory.ProductType = model.ProductType;
            productCategory.CompanyId = model.CompanyId;
            productCategory.Name = model.Name;
            productCategory.CashCustomerRate = model.CashCustomerRate;
            productCategory.Remarks = model.Remarks;
            productCategory.OrderNo = model.OrderNo;

            context.Entry(productCategory).State = productCategory.ProductCategoryId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }


        public bool DeleteProductCategory(int id)
        {
            ProductCategory productCategory = context.ProductCategories.Find(id);
            if (productCategory == null)
            {
                throw new Exception("Product Category not found");
            }
            context.ProductCategories.Remove(productCategory);
            return context.SaveChanges() > 0;
        }

        public List<ProductCategoryModel> GetProductCategories(int companyId, string type, string searchText)
        {
            IQueryable<ProductCategory> queryable = context.ProductCategories.Where(x => x.CompanyId == companyId && x.ProductType.Equals(type) && x.Name.Contains(searchText)).OrderBy(x => x.OrderNo);
            return ObjectConverter<ProductCategory, ProductCategoryModel>.ConvertList(queryable.ToList()).ToList();
        }

        public List<SelectModel> GetProductCategorySelectModelByCompany(int companyId, string type)
        {
            IQueryable<ProductCategory> queryable = context.ProductCategories.Where(x => x.CompanyId == companyId && x.ProductType.Equals(type));
            return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.ProductCategoryId }).OrderBy(x => x.Text).ToList();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }


    }
}
