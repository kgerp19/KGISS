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
    public class DropDownTypeService : IDropDownTypeService
    {
        private readonly ERPEntities context;
        public DropDownTypeService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<DropDownTypeModel> GetDropDownTypes(int companyId)
        {
            DropDownTypeModel dropDownTypeModel = new DropDownTypeModel();
            dropDownTypeModel.CompanyId = companyId;
            dropDownTypeModel.DataList = await Task.Run(() => (from t1 in context.DropDownTypes
                                                      where t1.CompanyId == companyId
                                                      select new DropDownTypeModel
                                                      {
                                                          DropDownTypeId = t1.DropDownTypeId,
                                                          Name = t1.Name,
                                                          Remarks = t1.Remarks
                                                      }).OrderBy(o => o.Name).AsEnumerable());
            return dropDownTypeModel;
        }
        public List<DropDownTypeModel> GetDropDownTypes(string searchText)
        {
            IQueryable<DropDownType> queryable = context.DropDownTypes.Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)).OrderBy(x => x.Name);
            return ObjectConverter<DropDownType, DropDownTypeModel>.ConvertList(queryable.ToList()).ToList();
        }
        public  DropDownTypeModel GetDropDownType(int id)
        {
            if (id == 0)
            {
                return new DropDownTypeModel()
                {
                    CompanyId = (int)System.Web.HttpContext.Current.Session["CompanyId"],
                    IsActive = true
                };
            }
            DropDownType dropDownType = context.DropDownTypes.Find(id);
            return ObjectConverter<DropDownType, DropDownTypeModel>.Convert(dropDownType);
        }



        public bool SaveDropDownType(int id, DropDownTypeModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("DropDownType data missing!");
            }

            bool exist = context.DropDownTypes.Where(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.DropDownTypeId != id).Any();

            if (exist)
            {
                message = "Dropdown Type data already exist";
                return false;
            }
            DropDownType dropDownType = ObjectConverter<DropDownTypeModel, DropDownType>.Convert(model);
            if (id > 0)
            {
                dropDownType = context.DropDownTypes.FirstOrDefault(x => x.DropDownTypeId == id);
                if (dropDownType == null)
                {
                    throw new Exception("DropDownType not found!");
                }
                dropDownType.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                dropDownType.ModifiedDate = DateTime.Now;
            }

            else
            {
                dropDownType.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                dropDownType.CreatedDate = DateTime.Now;
            }

            dropDownType.CompanyId = model.CompanyId;
            dropDownType.Name = model.Name;
            dropDownType.Remarks = model.Remarks;
            dropDownType.IsActive = model.IsActive;
            context.Entry(dropDownType).State = dropDownType.DropDownTypeId == 0 ? EntityState.Added : EntityState.Modified;
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


        public List<SelectModel> GetDropDownTypeSelectModels()
        {
            return context.DropDownTypes.Where(x => !string.IsNullOrEmpty(x.Name)).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DropDownTypeId
            }).OrderBy(x => x.Text).ToList();
        }


        public List<SelectModel> GetCountriesSelectModels()
        {
            return context.Countries.ToList().Select(x => new SelectModel()
            {
                Text = x.CountryName,
                Value = x.CountryId
            }).OrderBy(x => x.Text).ToList();
        }


        public List<SelectModel> GetDistrictByDivision(string name)
        {
            return context.Districts.ToList().Where(x => x.Name == name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DistrictId
            }).ToList();
        }

        public bool DeleteDropDownType(int id)
        {
            DropDownType dropDownType = context.DropDownTypes.Find(id);
            if (dropDownType == null)
            {
                throw new Exception("Data not found");
            }
            context.DropDownTypes.Remove(dropDownType);
            return context.SaveChanges() > 0;
        }

        public List<DropDownTypeModel> GetDropDownTypes(int companyId, string searchText)
        {
            IQueryable<DropDownType> queryable = context.DropDownTypes.Where(x => x.CompanyId == companyId && (x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText))).OrderBy(x => x.Name);
            return ObjectConverter<DropDownType, DropDownTypeModel>.ConvertList(queryable.ToList()).ToList();
        }

        public List<SelectModel> GetDropDownTypeSelectModelsByCompany(int companyId)
        {
            return context.DropDownTypes.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DropDownTypeId
            }).OrderBy(x => x.Text).ToList();
        }
    }
}
