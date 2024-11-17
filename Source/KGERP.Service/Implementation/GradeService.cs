using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class GradeService : IGradeService
    {
        ERPEntities gradeRepository = new ERPEntities();
        public List<Grade> GetGrades()
        {
            return gradeRepository.Grades.ToList();
        }

        public List<SelectModel> GetGradeSelectModels(int CompanyId)
        {
            return gradeRepository.Grades.Where(x => x.IsActive && (x.CompanyId == null || x.CompanyId == CompanyId)).ToList().Select(x => new SelectModel()
            {
                Text =x.GradeCode+" - "+ x.Name.ToString(),
                Value = x.GradeId.ToString()
            }).ToList();
        }
    }
}
