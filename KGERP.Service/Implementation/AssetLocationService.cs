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
    public class AssetLocationService : IAssetLocationService
    {
        private bool disposed = false;

        private readonly ERPEntities context;
        public AssetLocationService(ERPEntities context)
        {
            this.context = context;
        }
        public List<AssetLocationModel> GetLocation()
        {
            var data = context.AssetLocations.ToList();
            return ObjectConverter<AssetLocation, AssetLocationModel>.ConvertList(data).ToList();

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

        public bool SaveOrEdit(int id, AssetLocationModel asset)
        {
            if (asset == null)
            {
                throw new Exception("Product data missing!");
            }

            AssetLocation Location = ObjectConverter<AssetLocationModel, AssetLocation>.Convert(asset);
            if (id > 0)
            {
                Location = context.AssetLocations.FirstOrDefault(x => x.LocationId == id);
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
            Location.SerialNo = asset.SerialNo;
            context.Entry(Location).State = Location.LocationId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public AssetLocationModel GetAssetLocationById(int id)
        {
            AssetLocation location = new AssetLocation();
            int locationId = context.AssetLocations.OrderByDescending(x => x.LocationId).Select(x => x.LocationId).Take(1).FirstOrDefault();
            int sno = locationId + 1;
            location.SerialNo = sno.ToString().PadLeft(2, '0');


            if (id > 0)
            {
                location = context.AssetLocations.Where(x => x.LocationId == id).FirstOrDefault();
            }


            return ObjectConverter<AssetLocation, AssetLocationModel>.Convert(location);
        }

        public List<AssetSubLocationModel> GetSubLocation()
        {
            var data = context.AssetSubLocations.Include(x => x.AssetLocation).ToList();
            return ObjectConverter<AssetSubLocation, AssetSubLocationModel>.ConvertList(data).ToList();
        }


        public AssetSubLocationModel GetAssetSubLocationById(int id)
        {
            AssetSubLocation location = new AssetSubLocation();


            if (id > 0)
            {
                location = context.AssetSubLocations.Where(x => x.SubLocationId == id).FirstOrDefault();
            }


            return ObjectConverter<AssetSubLocation, AssetSubLocationModel>.Convert(location);
        }

        public List<SelectModel> GetLocationSelectModels()
        {
            return context.AssetLocations.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.LocationId
            }).ToList();
        }

        public string GetSerialNo(int locationId)
        {
            var locNo = context.AssetLocations.Where(x => x.LocationId == locationId).Select(x => x.SerialNo).FirstOrDefault();
            var loSubcNo = context.AssetSubLocations.Where(x => x.LocationId == locationId).OrderByDescending(x => x.SubLocationId).Select(x => x.SerialNo).Take(1).FirstOrDefault();
            int no = loSubcNo == null ? 0 : Convert.ToInt32(loSubcNo.Substring(2, 3));
            int nextNo = no + 1;
            string sNo = nextNo.ToString().PadLeft(3, '0');
            string serialNo = locNo + sNo;
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
    }
}
