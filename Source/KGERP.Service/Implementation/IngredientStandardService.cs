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
    public class IngredientStandardService : IIngredientStandardService
    {
        private readonly ERPEntities context;
        public IngredientStandardService(ERPEntities context)
        {
            this.context = context;
        }

        public bool DeleteIngredientStandardDetail(int ingredientStandardDetailId)
        {
            IngredientStandardDetail ingredientStandardDetail = context.IngredientStandardDetails.Find(ingredientStandardDetailId);
            if (ingredientStandardDetail == null)
            {
                throw new Exception("Data not found");
            }
            context.IngredientStandardDetails.Remove(ingredientStandardDetail);
            return context.SaveChanges() > 0;
        }

        public IngredientStandardModel GetIngredientStandard(int id)
        {
            IngredientStandard ingredientStandard = context.IngredientStandards.Include(x => x.ProductSubCategory).Include(x => x.Product).Where(x => x.IngredientStandardId == id).FirstOrDefault();
            if (ingredientStandard == null)
            {
                return new IngredientStandardModel() { IsActive = true };
            }
            return ObjectConverter<IngredientStandard, IngredientStandardModel>.Convert(ingredientStandard);
        }

        public IngredientStandardDetailModel GetIngredientStandardDetail(int id)
        {
            if (id == 0)
            {
                return new IngredientStandardDetailModel();
            }
            IngredientStandardDetail ingredientStandardDetail = context.IngredientStandardDetails.Find(id);
            return ObjectConverter<IngredientStandardDetail, IngredientStandardDetailModel>.Convert(ingredientStandardDetail);
        }

        public List<IngredientStandardDetailModel> GetIngredientStandardDetails(int ingredientStandardId)
        {
            IQueryable<IngredientStandardDetail> queryable = context.IngredientStandardDetails.Where(x => x.IngredientStandardId == ingredientStandardId);
            return ObjectConverter<IngredientStandardDetail, IngredientStandardDetailModel>.ConvertList(queryable.ToList()).ToList();
        }

        public List<IngredientStandardModel> GetIngredientStandards(int companyId, string searchText)
        {
            IQueryable<IngredientStandardModel> models = context.Database.SqlQuery<IngredientStandardModel>("exec spGetIngredientStandards {0}", companyId).AsQueryable();
            return models.Where(x => x.ProductName.ToLower().Contains(searchText.ToLower()) || x.ProductCode.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)).OrderBy(x => x.ProductSubCategoryId).ThenBy(x => x.ProductName).ToList();
        }

        public bool SaveIngredientStandard(int id, IngredientStandardModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("Data missing!");
            }
            bool isExist = context.IngredientStandards.Where(x => x.ProductSubCategoryId == model.ProductSubCategoryId && x.ProductId == model.ProductId && x.IngredientStandardId != id).Any();

            if (isExist)
            {
                message = "Data already exists !";
                return !isExist;
            }
            IngredientStandard ingredientStandard = ObjectConverter<IngredientStandardModel, IngredientStandard>.Convert(model);
            if (id > 0)
            {
                ingredientStandard = context.IngredientStandards.FirstOrDefault(x => x.IngredientStandardId == id);
                if (ingredientStandard == null)
                {
                    throw new Exception("Data not found!");
                }
                ingredientStandard.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                ingredientStandard.ModifiedDate = DateTime.Now;
            }
            else
            {
                ingredientStandard.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                ingredientStandard.CreatedDate = DateTime.Now;
            }


            ingredientStandard.ProductSubCategoryId = model.ProductSubCategoryId;
            ingredientStandard.ProductId = model.ProductId;
            ingredientStandard.IsActive = model.IsActive;
            context.Entry(ingredientStandard).State = ingredientStandard.IngredientStandardId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool SaveIngredientStandardDetail(int id, IngredientStandardDetailModel model)
        {
            IngredientStandardDetail ingredientStandardDetail = ObjectConverter<IngredientStandardDetailModel, IngredientStandardDetail>.Convert(model);
            if (id > 0)
            {
                ingredientStandardDetail = context.IngredientStandardDetails.FirstOrDefault(x => x.IngredientStandardDetailId == id);
                if (ingredientStandardDetail == null)
                {
                    throw new Exception("Data not found!");
                }
                ingredientStandardDetail.ModifiedDate = DateTime.Now;
                ingredientStandardDetail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                ingredientStandardDetail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                ingredientStandardDetail.CreatedDate = DateTime.Now;
            }

            ingredientStandardDetail.IngredientStandardId = model.IngredientStandardId;
            ingredientStandardDetail.ColumnName = model.ColumnName;
            ingredientStandardDetail.ColumnValue = model.ColumnValue;
            context.Entry(ingredientStandardDetail).State = ingredientStandardDetail.IngredientStandardDetailId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }
    }
}
