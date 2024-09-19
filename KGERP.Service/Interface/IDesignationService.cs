using KGERP.Data.Models;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Service.Interface
{
    public interface IDesignationService
    {
        List<Designation> GetDesignations();
        List<SelectModel> GetDesignationSelectModels();
        List<SelectModelForGrade> GetGradeSelectModels();
        List<SelectListItem> GetDesignationSelectListModels();
    }
}
