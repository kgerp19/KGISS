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
    public class ZoneService : IZoneService
    {
        private readonly ERPEntities context;
        public ZoneService(ERPEntities context)
        {
            this.context = context;
        }

        public List<ZoneModel> GetZones(int companyId, string searchText)
        {
            IQueryable<Zone> queryable = context.Zones.Where(x => x.CompanyId == companyId && (x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText))).OrderBy(x => x.Name);
            return ObjectConverter<Zone, ZoneModel>.ConvertList(queryable.ToList()).ToList();
        }


        public decimal GetCarryingRate(int? customerId)
        {
            return context.Database.SqlQuery<dynamic>(@"select Rate from  Erp.Zone where ZoneId = (select ZoneId from Erp.Vendor where VendorId ={0})", customerId).FirstOrDefault();
        }


        public List<SelectModel> GetZoneSelectModels(int companyId)
        {
            return context.Zones.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ZoneId
            }).OrderBy(x => x.Text).ToList();
        }

        public bool SaveZone(int id, ZoneModel model)
        {
            if (model == null)
            {
                throw new Exception("Zone data missing!");
            }

            Zone zone = ObjectConverter<ZoneModel, Zone>.Convert(model);
            if (id > 0)
            {
                zone = context.Zones.FirstOrDefault(x => x.ZoneId == id);
                if (zone == null)
                {
                    throw new Exception("Zone not found!");
                }
                zone.ModifiedDate = DateTime.Now;
                zone.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                zone.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                zone.CreatedDate = DateTime.Now;

            }
            zone.IsActive = model.IsActive;
            zone.Name = model.Name;
            zone.Code = model.Code;
            zone.Remarks = model.Remarks;

            context.Entry(zone).State = zone.ZoneId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public ZoneModel GetZone(int id)
        {
            if (id <= 0)
            {
                return new ZoneModel()
                {
                    ZoneId = id,
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]),
                    IsActive = true
                };
            }
            Zone zone = context.Zones.FirstOrDefault(x => x.ZoneId == id);

            return ObjectConverter<Zone, ZoneModel>.Convert(zone);

        }

        public List<SubZoneModel> GetSubZones(int companyId, string searchText)
        {
            IQueryable<SubZone> queryable = context.SubZones.Include(x => x.Zone).Where(x => x.Zone.CompanyId == companyId && (x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText))).OrderBy(x => x.Name);
            return ObjectConverter<SubZone, SubZoneModel>.ConvertList(queryable.ToList()).ToList();
        }

        public bool SaveSubZone(int id, SubZoneModel model)
        {
            if (model == null)
            {
                throw new Exception("Sub Zone data missing!");
            }

            SubZone subZone = ObjectConverter<SubZoneModel, SubZone>.Convert(model);
            if (id > 0)
            {
                subZone = context.SubZones.FirstOrDefault(x => x.SubZoneId == id);
                if (subZone == null)
                {
                    throw new Exception("Zone not found!");
                }
                subZone.ModifiedDate = DateTime.Now;
                subZone.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                subZone.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                subZone.CreatedDate = DateTime.Now;

            }
            subZone.IsActive = model.IsActive;
            subZone.Name = model.Name;
            subZone.ZoneId = model.ZoneId;


            context.Entry(subZone).State = subZone.SubZoneId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public SubZoneModel GetSubZone(int id)
        {
            if (id <= 0)
            {
                return new SubZoneModel()
                {
                    SubZoneId = id,

                    IsActive = true
                };
            }
            SubZone subZone = context.SubZones.FirstOrDefault(x => x.SubZoneId == id);

            return ObjectConverter<SubZone, SubZoneModel>.Convert(subZone);
        }

        public bool DeleteSubZone(int id, int companyId)
        {
            SubZone subZone = context.SubZones.Where(x => x.SubZoneId == id && x.Zone.CompanyId == companyId).FirstOrDefault();
            if (subZone != null)
                context.SubZones.Remove(subZone);
            return context.SaveChanges() > 0;
        }
        public List<SelectModel> GetSubZoneSelectModelsByZone(int zoneId)
        {
            IQueryable<SubZone> queryable = context.SubZones.Where(x => x.ZoneId == zoneId);
            return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.SubZoneId }).OrderBy(x => x.Text).ToList();
        }


        //public List<SelectModel> GetZoneSelectModels(int companyId)
        //{
        //    return context.Head4.Where(x=>x.CompanyId== companyId && x.AccCode.Contains("1304") && x.AccName.Contains("Zone")).ToList().Select(x => new SelectModel()
        //    {
        //        Text ="[" +x.AccCode.ToString()+"] " + x.AccName.ToString(),
        //        Value = x.Id
        //    }).OrderBy(x=>x.Text).ToList();
        //}
    }
}
