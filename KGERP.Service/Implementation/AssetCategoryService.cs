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
    public class AssetCategoryService : IAssetCategoryService
    {
        private readonly ERPEntities context;
        private bool disposed;
        public AssetCategoryService(ERPEntities context)
        {
            this.context = context;
        }
        public List<AssetCategoryModel> GetAssetCategory()
        {
            var data = context.AssetCategories.ToList();
            return ObjectConverter<AssetCategory, AssetCategoryModel>.ConvertList(data).ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool SaveOrEdit(int id, AssetCategoryModel asset)
        {
            if (asset == null)
            {
                throw new Exception("Product data missing!");
            }

            AssetCategory Category = ObjectConverter<AssetCategoryModel, AssetCategory>.Convert(asset);
            if (id > 0)
            {
                Category = context.AssetCategories.FirstOrDefault(x => x.AssetCategoryId == id);
                if (Category == null)
                {
                    throw new Exception("Product Category not found!");
                }
                Category.ModifiedDate = DateTime.Now;
                Category.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                Category.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                Category.CreatedDate = DateTime.Now;
            }

            Category.Name = asset.Name;
            Category.SerialNo = asset.SerialNo;
            context.Entry(Category).State = Category.AssetCategoryId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public AssetCategoryModel GetAssetCategoryById(int id)
        {
            AssetCategory category = new AssetCategory();
            int categoryId = context.AssetCategories.OrderByDescending(x => x.AssetCategoryId).Select(x => x.AssetCategoryId).Take(1).FirstOrDefault();
            int sno = categoryId + 1;
            category.SerialNo = sno.ToString().PadLeft(2, '0');


            if (id > 0)
            {
                category = context.AssetCategories.Where(x => x.AssetCategoryId == id).FirstOrDefault();
            }


            return ObjectConverter<AssetCategory, AssetCategoryModel>.Convert(category);
        }

        public string GetSerialNo(int catId)
        {
            var catNo = context.AssetCategories.Where(x => x.AssetCategoryId == catId).Select(x => x.SerialNo).FirstOrDefault();
            var assetTypeNo = context.AssetTypes.Where(x => x.AssetCategoryId == catId).OrderByDescending(x => x.AssetTypeId).Select(x => x.SerialNo).Take(1).FirstOrDefault();
            int no = assetTypeNo == null ? 0 : Convert.ToInt32(assetTypeNo.Substring(2, 3));
            int nextNo = no + 1;
            string sNo = nextNo.ToString().PadLeft(3, '0');
            string serialNo = catNo + sNo;
            return serialNo;

        }

        public bool SaveOrEditAssetSubLocation(int id, AssetSubLocationModel asset)
        {
            if (asset == null)
            {
                throw new Exception("Product data missing!");
            }

            AssetSubLocation Location = ObjectConverter<AssetSubLocationModel, AssetSubLocation>.Convert(asset);
            if (id > 0)
            {
                Location = context.AssetSubLocations.FirstOrDefault(x => x.SubLocationId == id);
                if (Location == null)
                {
                    throw new Exception("Product Category not found!");
                }
                Location.ModifiedDate = DateTime.Now;
                Location.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                Location.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                Location.CreatedDate = DateTime.Now;
            }

            Location.Name = asset.Name;
            Location.LocationId = asset.LocationId;
            Location.SerialNo = asset.SerialNo;
            context.Entry(Location).State = Location.SubLocationId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public List<AssetTypeModel> GetAssetType()
        {
            //var data = context.AssetTypes.Include(x => x.AssetCategory).ToList();
            //return ObjectConverter<AssetType, AssetTypeModel>.ConvertList(data).ToList();
            dynamic result = context.Database.SqlQuery<AssetTypeModel>("exec sp_Asset_GetAssetTypeList").ToList();
            return result;
        }

        public AssetTypeModel GetAssetTypeById(int id)
        {
            AssetType location = new AssetType();

            if (id > 0)
            {
                location = context.AssetTypes.Find(id);
            }

            return ObjectConverter<AssetType, AssetTypeModel>.Convert(location);
        }

        public List<SelectModel> GetAssetCategorySelectModels()
        {
            return context.AssetCategories.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.AssetCategoryId
            }).ToList();
        }

        public bool SaveOrEditAssetSubType(int id, AssetTypeModel asset)
        {
            if (asset == null)
            {
                throw new Exception("Product data missing!");
            }

            AssetType Location = ObjectConverter<AssetTypeModel, AssetType>.Convert(asset);
            if (id > 0)
            {
                Location = context.AssetTypes.FirstOrDefault(x => x.AssetTypeId == id);
                if (Location == null)
                {
                    throw new Exception("Product Category not found!");
                }
                Location.ModifiedDate = DateTime.Now;
                Location.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                Location.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                Location.CreatedDate = DateTime.Now;
            }

            Location.Name = asset.Name;
            Location.AssetCategoryId = asset.AssetCategoryId;
            Location.SerialNo = asset.SerialNo;
            context.Entry(Location).State = Location.AssetTypeId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

    }
}
