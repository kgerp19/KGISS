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
    public class UpazilaAssignService : IUpazilaAssignService
    {
        private readonly ERPEntities context;

        public UpazilaAssignService(ERPEntities context)
        {
            this.context = context;
        }

        public List<UpazilaAssignModel> GetUpazilaListByDistrictAndEmployee(int districtId, long employeeId)
        {
            var result = context.Database.SqlQuery<UpazilaAssignModel>("exec spGetUpazilaAssign {0}, {1}", employeeId, districtId).ToList();
            return result;
        }

        public bool SaveUpazilaAssign(List<UpazilaAssignModel> models)
        {
            List<UpazilaAssign> upazilaAssigns = ObjectConverter<UpazilaAssignModel, UpazilaAssign>.ConvertList(models.ToList()).ToList();


            foreach (var item in upazilaAssigns)
            {
                UpazilaAssign upazilaAssign = new UpazilaAssign();
                if (item.UpazilaAssignId > 0)
                {
                    upazilaAssign = context.UpazilaAssigns.FirstOrDefault(x => x.UpazilaAssignId == item.UpazilaAssignId);
                    if (upazilaAssign == null)
                    {
                        throw new Exception("Not found!");
                    }
                    upazilaAssign.ModifiedDate = DateTime.Now;
                    upazilaAssign.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                }

                else
                {
                    upazilaAssign.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    upazilaAssign.CreatedDate = DateTime.Now;
                }

                upazilaAssign.EmployeeId = item.EmployeeId;
                upazilaAssign.DistrictId = item.DistrictId;
                upazilaAssign.UpazilaId = item.UpazilaId;
                upazilaAssign.IsActive = item.IsActive;

                context.Entry(upazilaAssign).State = upazilaAssign.UpazilaAssignId == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();


            }
            return true;
        }
    }
}
