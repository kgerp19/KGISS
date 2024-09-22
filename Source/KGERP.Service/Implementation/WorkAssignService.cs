//using KGERP.Data.Models;
//using KGERP.Service.Interface;
//using KGERP.Service.ServiceModel;
//using KGERP.Utility;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;

//namespace KGERP.Service.Implementation
//{
//    public class WorkAssignService : IWorkAssignService
//    {
//        private readonly ERPEntities context;
//        public WorkAssignService(ERPEntities context)
//        {
//            this.context = context;
//        }



//        public WorkAssignModel GetWorkAssign(int id)
//        {
//            if (id == 0)
//            {
//                return new WorkAssignModel();
//            }
//            WorkAssign workAssign = context.WorkAssigns.Include(x => x.Work.WorkState).Where(x => x.WorkAssignId == id).FirstOrDefault();
//            return ObjectConverter<WorkAssign, WorkAssignModel>.Convert(workAssign);
//        }

//        public List<WorkAssignModel> GetWorkAssigns(int workId)
//        {
//            IQueryable<WorkAssign> queryable = context.WorkAssigns.Where(x => x.WorkId == workId);
//            return ObjectConverter<WorkAssign, WorkAssignModel>.ConvertList(queryable.ToList()).ToList();
//        }

//        public bool SaveWorkAssignFileList(List<WorkAssignFileModel> workAssignFileModels)
//        {
//            List<WorkAssignFile> workAssignFiles = ObjectConverter<WorkAssignFileModel, WorkAssignFile>.ConvertList(workAssignFileModels).ToList();
//            context.WorkAssignFiles.AddRange(workAssignFiles);
//            return context.SaveChanges() > 1;
//        }
//        public List<WorkAssignFileModel> GetFiles(long workAssignId)
//        {
//            IQueryable<WorkAssignFile> queryable = context.WorkAssignFiles.Where(x => x.WorkAssignId == workAssignId);
//            return ObjectConverter<WorkAssignFile, WorkAssignFileModel>.ConvertList(queryable.ToList()).ToList();
//        }

//        public string GetFileName(long workAssingFileId)
//        {
//            return context.WorkAssignFiles.Where(x => x.WorkAssignFileId == workAssingFileId).First().FileName;
//        }
//        //public List<SelectModel> GetDistrictSelectModels()
//        //{
//        //    return context.Districts.Where(x=>!string.IsNullOrEmpty(x.Name)).ToList().Select(x => new SelectModel()
//        //    {
//        //        Text = x.Name,
//        //        Value = x.DistrictId
//        //    }).OrderBy(x=>x.Text).ToList();
//        //}

//        //public DistrictModel GetDistrict(int id)
//        //{
//        //    if (id == 0)
//        //    {
//        //        District lastDistrict = context.Districts.OrderByDescending(x => x.Code).FirstOrDefault();
//        //        string newCode = (Convert.ToInt32(lastDistrict.Code) + 1).ToString();
//        //        return new DistrictModel() { Code=newCode};
//        //    }
//        //    District district = context.Districts.Find(id);
//        //    return ObjectConverter<District, DistrictModel>.Convert(district);
//        //}

//        //public bool SaveDistrict(int id, DistrictModel model)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new Exception("District data missing!");
//        //    }

//        //    bool exist = context.Districts.Where(x => x.Name.Equals(model.Name) &&  x.DistrictId != id).Any();

//        //    if (exist)
//        //    {
//        //        throw new Exception("District already exist!");
//        //    }
//        //    District district = ObjectConverter<DistrictModel, District>.Convert(model);
//        //    if (id > 0)
//        //    {
//        //        district = context.Districts.FirstOrDefault(x => x.DistrictId == id);
//        //        if (district == null)
//        //        {
//        //            throw new Exception("District not found!");
//        //        }
//        //    }

//        //    else
//        //    {

//        //    }

//        //    district.IsActive = true;
//        //    district.Name = model.Name;
//        //    district.Code = model.Code;

//        //    context.Entry(district).State = district.DistrictId == 0 ? EntityState.Added : EntityState.Modified;
//        //    return context.SaveChanges() > 0;
//        //}

//        //public bool DeleteDistrict(int id)
//        //{
//        //    District district = context.Districts.Find(id);
//        //    if (district == null)
//        //    {
//        //        throw new Exception("District not found");
//        //    }
//        //    context.Districts.Remove(district);
//        //    return context.SaveChanges() > 0;
//        //}

//        //public List<SelectModel> GetCountriesSelectModels()
//        //{
//        //    return context.Countries.ToList().Select(x => new SelectModel()
//        //    {
//        //        Text = x.CountryName,
//        //        Value = x.CountryId
//        //    }).OrderBy(x => x.Text).ToList();
//        //}
//    }
//}
