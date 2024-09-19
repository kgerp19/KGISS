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
    public class AssignAssetService : IAssignAssetService
    {
        private bool disposed = false;

        private readonly ERPEntities context;
        public AssignAssetService(ERPEntities context)
        {
            this.context = context;
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

        public List<AssetAssignModel> Index()
        {
            //List<AssetAssign> data = context.AssetAssigns.Include(x => x.AssetType).Include(x => x.AssetTrackingFinal).ToList();
            //return ObjectConverter<AssetAssign, AssetAssignModel>.ConvertList(data).ToList();

            dynamic result = context.Database.SqlQuery<AssetAssignModel>("exec sp_Asset_AssignAssetList").ToList();
            return result;
        }


        public AssetAssignModel GetAssetAssignById(int id)
        {
            if (id == 0)
            {
                AssetAssignModel asset = new AssetAssignModel();
                return asset;
            }
            else
            {
                AssetAssign data = context.AssetAssigns.Where(x => x.AssignId == id).FirstOrDefault();
                return ObjectConverter<AssetAssign, AssetAssignModel>.Convert(data);
            }

        }


        public List<SelectModel> Company()
        {
            return context.Companies.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.CompanyId
            }).ToList();
        }

        public List<SelectModel> AssetLocation()
        {
            return context.AssetLocations.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.LocationId
            }).ToList();
        }

        public List<SelectModel> AssetSubLocation(int locationId)
        {
            return context.AssetSubLocations.Where(x => x.LocationId == locationId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.SubLocationId
            }).ToList();
        }

        public List<SelectModel> AssetCategory()
        {
            return context.AssetCategories.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.AssetCategoryId
            }).ToList();
        }

        public List<SelectModel> AssetType(int categoryId)
        {
            return context.AssetTypes.Where(x => x.AssetCategoryId == categoryId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.AssetTypeId
            }).ToList();
        }



        public bool SaveOrEdit(AssetAssignModel model)
        {
            if (model == null)
            {
                throw new Exception("Asset not found!");
            }

            AssetAssign asset = ObjectConverter<AssetAssignModel, AssetAssign>.Convert(model);

            if (asset.AssignId > 0)
            {
                asset = context.AssetAssigns.FirstOrDefault(x => x.AssignId == asset.AssignId);
                if (asset == null)
                {
                    throw new Exception("Asset not found!");
                }
                asset.ModifiedDate = DateTime.Now;
                asset.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.CompanyId = model.CompanyId;
                asset.AssetLocationId = model.AssetLocationId;
                asset.AssetSubLocId = model.AssetSubLocId;
                asset.AssetCategoryId = model.AssetCategoryId;
                asset.AssetTypeId = model.AssetTypeId;
                asset.AssignTo = model.AssignTo;
                asset.Remarks = model.Remarks;
                context.SaveChanges();
            }

            else
            {
                asset.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.CreatedDate = DateTime.Now;

            }
            asset.CompanyId = model.CompanyId;
            asset.AssetLocationId = model.AssetLocationId;
            asset.AssetSubLocId = model.AssetSubLocId;
            asset.AssetCategoryId = model.AssetCategoryId;
            asset.AssetTypeId = model.AssetTypeId;
            asset.AssetId = model.AssetId;
            asset.AssignTo = model.AssignTo;
            asset.Remarks = model.Remarks;
            context.Entry(asset).State = asset.AssignId == 0 ? EntityState.Added : EntityState.Modified;
            context.SaveChanges();

            //Asset assetData = context.Assets.Where(x => x.AssetId == model.AssetId).FirstOrDefault();
            AssetTrackingFinal assetData = context.AssetTrackingFinals.Where(x => x.OID == model.AssetId).FirstOrDefault();
            assetData.IsAssigned = 1;
            return context.SaveChanges() > 0;
        }

        public List<SelectModel> SerialNo(int assetTypeId, int companyId)
        {
            return context.AssetTrackingFinals.Where(x => x.AssetTypeId == assetTypeId && x.CompanyId == companyId && x.IsAssigned == 0).ToList().Select(x => new SelectModel()
            {
                Text = x.SerialNumber,
                Value = x.OID
            }).ToList();
        }


        public List<SelectModel> SerialNoo(int assetTypeId, int companyId)
        {
            return context.Assets.Where(x => x.AssetTypeId == assetTypeId && x.CompanyId == companyId && x.IsAssigned == 0).ToList().Select(x => new SelectModel()
            {
                Text = x.SerialNO,
                Value = x.AssetId
            }).ToList();
        }
    }
}
