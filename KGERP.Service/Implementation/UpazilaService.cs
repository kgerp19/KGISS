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
    public class UpazilaService : IUpazilaService
    {
        private readonly ERPEntities context;
        public UpazilaService(ERPEntities context)
        {
            this.context = context;
        }

        public async Task<UpazilaModel> GetUpazilas()
        {
            UpazilaModel upazilaModel = new UpazilaModel();

            upazilaModel.DataList = await Task.Run(() => (from t1 in context.Upazilas
                                                          join t2 in context.Districts on t1.DistrictId equals t2.DistrictId
                                                        select new UpazilaModel
                                                        {
                                                            DistrictName = t2.Name,
                                                            Name= t1.Name,
                                                            Code= t1.Code,
                                                            FacCarryingCommission= t1.FacCarryingCommission,
                                                            DepoCarryingCommission= t1.DepoCarryingCommission,
                                                            IsActive=t1.IsActive,
                                                            UpazilaId= t1.UpazilaId
                                                        }).OrderBy(o => o.Name).AsEnumerable());

            return upazilaModel;
        }
        public List<UpazilaModel> GetUpazilas(string searchText)
        {
            IQueryable<Upazila> queryable = context.Upazilas.Include(x => x.District).Where(x => x.Name.Contains(searchText) || x.District.Name.Contains(searchText));
            return ObjectConverter<Upazila, UpazilaModel>.ConvertList(queryable.ToList()).ToList();
        }
        public UpazilaModel GetUpazila(int id)
        {
            if (id <= 0)
            {
                return new UpazilaModel() { UpazilaId = id };
            }
            Upazila upazila = context.Upazilas.FirstOrDefault(x => x.UpazilaId == id);

            return ObjectConverter<Upazila, UpazilaModel>.Convert(upazila);
        }

        public bool SaveUpazila(int id, UpazilaModel model)
        {
            if (model == null)
            {
                throw new Exception("Upazila data missing!");
            }

            Upazila upazila = ObjectConverter<UpazilaModel, Upazila>.Convert(model);
            if (id > 0)
            {
                upazila = context.Upazilas.FirstOrDefault(x => x.UpazilaId == id);
                if (upazila == null)
                {
                    throw new Exception("Upazila not found!");
                }
                //upazila.ModifiedDate = DateTime.Now;
                //upazila.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                //productSubCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                //productSubCategory.CreatedDate = DateTime.Now;

            }
            upazila.IsActive = true;
            upazila.DistrictId = model.DistrictId;
            upazila.Name = model.Name;
            upazila.Code = model.Code;
            upazila.FacCarryingCommission = model.FacCarryingCommission ?? 0;
            upazila.DepoCarryingCommission = model.DepoCarryingCommission ?? 0;
            upazila.MarketingOfficerId = model.MarketingOfficerId;
            context.Entry(upazila).State = upazila.UpazilaId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public List<SelectModel> GetUpazilaSelectModelsByDistrict(int districtId)
        {
            IQueryable<Upazila> queryable = context.Upazilas.Where(x => x.DistrictId == districtId);
            return queryable.ToList().Select(x => new SelectModel { Text = x.Name, Value = x.UpazilaId }).OrderBy(x => x.Text).ToList();
        }

        public bool DeleteUpazila(int id)
        {
            Upazila upazila = context.Upazilas.Find(id);
            context.Upazilas.Remove(upazila);
            return context.SaveChanges() > 0;
        }

        public string GetUpazilaCodeByDistrict(int districtId)
        {
            District district = context.Districts.Find(districtId);

            IQueryable<Upazila> queryable = context.Upazilas.Where(x => x.DistrictId == districtId);
            if (!queryable.Any())
            {
                return district.Code + "01";
            }
            Upazila lastUpazila = context.Upazilas.Where(x => x.DistrictId == districtId).OrderByDescending(x => x.Code).First();

            string newUpazilaCode = (Convert.ToInt32(lastUpazila.Code) + 1).ToString();
            return newUpazilaCode.PadLeft(4, '0');
        }

        public List<SelectModel> GetUpzilaSelectModels()
        {
            return context.Upazilas.Where(x => !string.IsNullOrEmpty(x.Name)).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DistrictId
            }).OrderBy(x => x.Text).ToList();
        }

        public List<SelectModel> GetUpzilaByDistrictName(string name)
        {
            var data = from u in context.Upazilas
                       join d in context.Districts on u.DistrictId equals d.DistrictId
                       where d.Name.Contains(name)
                       select new SelectModel { Text = u.Name, Value = u.Name };
            return data.ToList();
        }

        public List<SelectModel> GetUpzilaByDistrictId(int? districtId)
        {
            return context.Upazilas.ToList().Where(x => x.DistrictId == districtId).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.UpazilaId
            }).ToList();
        }
    }
}
