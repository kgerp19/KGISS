using KGERP.Data.CustomModel;
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
    public class LeaveCategoryService : ILeaveCategoryService
    {
        ERPEntities leaveCategoryRepository = new ERPEntities();
        public List<LeaveCategoryModel> GetLeaveCategories()
        {           
            return ObjectConverter<LeaveCategory, LeaveCategoryModel>.ConvertList(leaveCategoryRepository.LeaveCategories.Where(x => x.IsActive).ToList()).ToList();
        }

        public LeaveCategoryModel GetLeaveCategory(int id)
        {
            if (id == 0)
            {
                LeaveCategory leaveCategory = leaveCategoryRepository.LeaveCategories.Where(x => x.LeaveCategoryId == id).FirstOrDefault();
                if (leaveCategory == null)
                {
                    return new LeaveCategoryModel();
                }
            }

            return ObjectConverter<LeaveCategory, LeaveCategoryModel>.Convert(leaveCategoryRepository.LeaveCategories.FirstOrDefault(x => x.LeaveCategoryId == id));

        }

        public List<SelectModel> GetLeaveCategorySelectModels()
        {
            List<LeaveTypeCustomModel> customModels = leaveCategoryRepository.Database.SqlQuery<LeaveTypeCustomModel>("exec spGetDropdownLeaveCategories {0}", System.Web.HttpContext.Current.Session["Id"]).ToList();

            return customModels.Select(x => new SelectModel { Value = x.LeaveCategoryId, Text = x.Name }).ToList();
        }

        public bool SaveLeaveCategory(int id, LeaveCategoryModel model)
        {
            if (model == null)
            {
                throw new Exception("Leave Category data missing!");
            }

            LeaveCategory leaveCategory = ObjectConverter<LeaveCategoryModel, LeaveCategory>.Convert(model);
            if (id > 0)
            {
                leaveCategory = leaveCategoryRepository.LeaveCategories.FirstOrDefault(x => x.LeaveCategoryId == id);
                if (leaveCategory == null)
                {
                    return false;
                }
            }
            else
            {
                leaveCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                leaveCategory.CreatedDate = DateTime.Now;
            }
            leaveCategory.Name = model.Name;
            leaveCategory.MaxDays = model.MaxDays;
            leaveCategory.Type = model.Type;
            leaveCategory.Sequence = model.Sequence;


            leaveCategoryRepository.Entry(leaveCategory).State = leaveCategory.LeaveCategoryId == 0 ? EntityState.Added : EntityState.Modified;
            return leaveCategoryRepository.SaveChanges() > 0;
        }
    }
}
