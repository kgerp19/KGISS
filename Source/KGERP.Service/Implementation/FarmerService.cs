using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class FarmerService : IFarmerService
    {
        private readonly ERPEntities context;
        public FarmerService(ERPEntities context)
        {
            this.context = context;
        }



        public IQueryable<FarmerModel> GetFarmers(int companyId, string searchValue, out int count)
        {
            count = context.Database.SqlQuery<int>("select count(FarmerId) from Erp.Farmer").First();
            return context.Database.SqlQuery<FarmerModel>(@"exec spGetFarmerSearch {0},{1}", companyId, searchValue).AsQueryable();

        }

        public FarmerModel GetFarmer(int id)
        {
            if (id == 0)
            {

                return new FarmerModel()
                {
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]),
                    IsActive = true
                };
            }
            Farmer farmer = context.Farmers.Find(id);
            return ObjectConverter<Farmer, FarmerModel>.Convert(farmer);
        }



        public bool SaveFarmer(long id, FarmerModel model)
        {
            if (model == null)
            {
                throw new Exception("Farmer data missing!");
            }

            bool exist = context.Farmers.Where(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.ZoneId == model.ZoneId && x.Phone.Equals(model.Phone) && x.FarmerId != id).Any();

            if (exist)
            {
                return false;
            }
            Farmer farmer = ObjectConverter<FarmerModel, Farmer>.Convert(model);
            if (id > 0)
            {
                farmer = context.Farmers.FirstOrDefault(x => x.FarmerId == id);
                if (farmer == null)
                {
                    throw new Exception("Farmer not found!");
                }
                farmer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                farmer.ModifiedDate = DateTime.Now;
            }

            else
            {
                farmer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                farmer.CreatedDate = DateTime.Now;
            }

            farmer.CompanyId = model.CompanyId;
            farmer.ZoneId = model.ZoneId;
            farmer.OfficerId = model.OfficerId;
            farmer.Name = model.Name;
            farmer.DateofBirth = model.DateOfBirth;
            farmer.NationalId = model.NationalId;
            farmer.Phone = model.Phone;
            farmer.Spouse = model.Spouse;
            farmer.StartDate = model.StartDate;
            farmer.Address = model.Address;
            farmer.Remarks = model.Remarks;
            farmer.IsActive = model.IsActive;


            context.Entry(farmer).State = farmer.FarmerId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteFarmer(int id)
        {
            Farmer farmer = context.Farmers.Find(id);
            if (farmer != null)
            {
                farmer.IsActive=false;
                return context.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
           
          
        }

        //public bool DeleteDistrict(int id)
        //{
        //    District district = context.Districts.Find(id);
        //    if (district == null)
        //    {
        //        throw new Exception("District not found");
        //    }
        //    context.Districts.Remove(district);
        //    return context.SaveChanges() > 0;
        //}

        //public List<SelectModel> GetCountriesSelectModels()
        //{
        //    return context.Countries.ToList().Select(x => new SelectModel()
        //    {
        //        Text = x.CountryName,
        //        Value = x.CountryId
        //    }).OrderBy(x => x.Text).ToList();
        //}

        //public List<SelectModel> GetDivisionByName(string name)
        //{
        //    return context.Divisions.ToList().Where(x => x.Name == name).Select(x => new SelectModel()
        //    {
        //        Text = x.Name,
        //        Value = x.DivisionId
        //    }).ToList();
        //}
        //public List<SelectModel> GetDistrictByDivisionName(string name)
        //{
        //    var data = from d in context.Divisions
        //               join ds in context.Districts on d.DivisionId equals ds.DivisionId
        //               where d.Name == name
        //               select new SelectModel
        //               {
        //                   Text = ds.Name,
        //                   Value = ds.Name
        //               };
        //    return data.ToList();
        //}
    }
}
