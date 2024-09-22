using KGERP.Data.Models;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IGradeService
    {
        List<Grade> GetGrades();
        List<SelectModel> GetGradeSelectModels();
    }
}
