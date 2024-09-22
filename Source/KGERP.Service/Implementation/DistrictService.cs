using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class DistrictService : IDistrictService
    {
        private readonly ERPEntities context;
        public DistrictService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<DistrictModel> GetDistricts()
        {
            DistrictModel districtModel = new DistrictModel();

            districtModel.DataList = await Task.Run(() => (from t1 in context.Districts
                                                                select new DistrictModel
                                                                {
                                                                   Name = t1.Name,  
                                                                   Code = t1.Code,
                                                                   DistrictId = t1.DistrictId
                                                                }).OrderBy(o=> o.Name).AsEnumerable());
            return districtModel;
        }
       


        public List<SelectModel> GetDistrictSelectModels()
        {
            return context.Districts.Where(x => !string.IsNullOrEmpty(x.Name)).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DistrictId
            }).OrderBy(x => x.Text).ToList();
        }

        public List<SelectModel> GetDivisionSelectModels()
        {
            return context.Divisions.Where(x => !string.IsNullOrEmpty(x.Name)).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DivisionId
            }).OrderBy(x => x.Text).ToList();
        }

        public DistrictModel GetDistrict(int id)
        {
            if (id == 0)
            {
                District lastDistrict = context.Districts.OrderByDescending(x => x.Code).FirstOrDefault();
                string newCode = (Convert.ToInt32(lastDistrict.Code) + 1).ToString();
                return new DistrictModel() { Code = newCode };
            }
            District district = context.Districts.Find(id);
            return ObjectConverter<District, DistrictModel>.Convert(district);
        }

        public bool SaveDistrict(int id, DistrictModel model)
        {
            if (model == null)
            {
                throw new Exception("District data missing!");
            }

            bool exist = context.Districts.Where(x => x.Name.Equals(model.Name) && x.DistrictId != id).Any();

            if (exist)
            {
                throw new Exception("District already exist!");
            }
            District district = ObjectConverter<DistrictModel, District>.Convert(model);
            if (id > 0)
            {
                district = context.Districts.FirstOrDefault(x => x.DistrictId == id);
                if (district == null)
                {
                    throw new Exception("District not found!");
                }
            }

            else
            {

            }

            district.IsActive = true;
            district.Name = model.Name;
            district.Code = model.Code;

            context.Entry(district).State = district.DistrictId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteDistrict(int id)
        {
            District district = context.Districts.Find(id);
            if (district == null)
            {
                throw new Exception("District not found");
            }
            context.Districts.Remove(district);
            return context.SaveChanges() > 0;
        }

        public List<SelectModel> GetCountriesSelectModels()
        {
            return context.Countries.ToList().Select(x => new SelectModel()
            {
                Text = x.CountryName,
                Value = x.CountryId
            }).OrderBy(x => x.Text).ToList();
        }

        public List<SelectModel> GetDivisionByName(string name)
        {
            return context.Divisions.ToList().Where(x => x.Name == name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DivisionId
            }).ToList();
        }
        public List<SelectModel> GetDistrictByDivisionName(string name)
        {
            var data = from d in context.Divisions
                       join ds in context.Districts on d.DivisionId equals ds.DivisionId
                       where d.Name == name
                       select new SelectModel
                       {
                           Text = ds.Name,
                           Value = ds.Name
                       };
            return data.ToList();
        }

        public List<SelectModel> GetDistrictByDivisionId(int? divisionId)
        {
            return context.Districts.ToList().Where(x => x.DivisionId == divisionId).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DistrictId
            }).ToList();
        }

    }
}
