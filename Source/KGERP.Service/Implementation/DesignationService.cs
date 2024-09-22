using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{
    public class DesignationService : IDesignationService
    {
        ERPEntities designationRepository = new ERPEntities();
        public List<Designation> GetDesignations()
        {
            return designationRepository.Designations.ToList();
        }

        public List<SelectListItem> GetDesignationSelectListModels()
        {
            return designationRepository.Designations.OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name.ToString(),
                Value = x.DesignationId.ToString()
            }).ToList();
        }

        public List<SelectModel> GetDesignationSelectModels()
        {
            return designationRepository.Designations.OrderBy(x => x.Name).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.DesignationId.ToString()
            }).ToList();
        }
        public List<SelectModelForGrade> GetGradeSelectModels()
        {
            return designationRepository.Grades.OrderBy(x => x.Name).ToList().Select(x => new SelectModelForGrade()
            {
                Text = x.Name.ToString(),
                Value = x.GradeId,
                Code=x.GradeCode.ToString()
            }).OrderBy(y=>y.Value).ToList();
        }
    }
}
