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
    public class ProductSubCategoryService : IProductSubCategoryService
    {
        private readonly ERPEntities context;
        public ProductSubCategoryService(ERPEntities context)
        {
            this.context = context;
        }

        public List<ProductSubCategoryModel> GetProductSubCategories()
        {
            IQueryable<ProductSubCategory> queryable = context.ProductSubCategories;
            return ObjectConverter<ProductSubCategory, ProductSubCategoryModel>.ConvertList(queryable.ToList()).ToList();
        }

        public List<ProductSubCategoryModel> GetProductSubCategories(int companyId, string type, string searchText)
        {
            IQueryable<ProductSubCategory> queryable = context.ProductSubCategories.Include(x => x.ProductCategory).Where(x => x.CompanyId == companyId && x.ProductType.Equals(type) && (x.ProductCategory.Name.Contains(searchText) || x.Name.Contains(searchText))).OrderBy(x => x.ProductCategoryId).ThenBy(x => x.OrderNo);
            return ObjectConverter<ProductSubCategory, ProductSubCategoryModel>.ConvertList(queryable.ToList()).ToList();
        }

        public ProductSubCategoryModel GetProductSubCategory(int id, string productType)
        {
            if (id <= 0)
            {
                return new ProductSubCategoryModel()
                {
                    ProductType = productType,
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]),
                    ProductSubCategoryId = id,
                    IsActive = true
                };
            }
            ProductSubCategory productSubCategory = context.ProductSubCategories.FirstOrDefault(x => x.ProductSubCategoryId == id);

            return ObjectConverter<ProductSubCategory, ProductSubCategoryModel>.Convert(productSubCategory);
        }



        public bool SaveProductSubCategory(int id, ProductSubCategoryModel model)
        {
            if (model == null)
            {
                throw new Exception("Product Sub Category data missing!");
            }

            ProductSubCategory productSubCategory = ObjectConverter<ProductSubCategoryModel, ProductSubCategory>.Convert(model);
            if (id > 0)
            {
                productSubCategory = context.ProductSubCategories.FirstOrDefault(x => x.ProductSubCategoryId == id);
                if (productSubCategory == null)
                {
                    throw new Exception("Product Sub Category not found!");
                }
                productSubCategory.ModifiedDate = DateTime.Now;
                productSubCategory.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                productSubCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productSubCategory.CreatedDate = DateTime.Now;
                productSubCategory.IsActive = true;
            }

            productSubCategory.CompanyId = model.CompanyId;
            productSubCategory.ProductType = model.ProductType;
            productSubCategory.ProductCategoryId = model.ProductCategoryId;
            productSubCategory.Name = model.Name;
            productSubCategory.BaseCommissionRate = model.BaseCommissionRate;
            productSubCategory.Remarks = model.Remarks;
            productSubCategory.OrderNo = model.OrderNo;

            context.Entry(productSubCategory).State = productSubCategory.ProductSubCategoryId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public List<SelectModel> GetProductSubCategorySelectModelsByProductCategory(int productCategoryId)
        {
            IQueryable<ProductSubCategory> queryable = context.ProductSubCategories.Where(x => x.ProductCategoryId == productCategoryId);
            return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.ProductSubCategoryId }).OrderBy(x => x.Text).ToList();
        }

        public bool DeleteProductSubCategory(int id)
        {
            ProductSubCategory productSubCategory = context.ProductSubCategories.Where(x => x.ProductSubCategoryId == id).FirstOrDefault();
            context.ProductSubCategories.Remove(productSubCategory);
            return context.SaveChanges() > 1;
        }

        public List<SelectModel> GetBasicAndAdditiveMaterialSelectModels(int companyId)
        {
            const int basicMaterial = 60;
            const int additiveMaterial = 61;
            IQueryable<ProductSubCategory> queryable = context.ProductSubCategories.Where(x => x.CompanyId == companyId && (x.ProductSubCategoryId == basicMaterial || x.ProductSubCategoryId == additiveMaterial));
            return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.ProductSubCategoryId }).ToList();
        }
    }
}
