using KGERP.Data.Models;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Service.Interface
{
    public interface IDepartmentService
    {
        List<Department> GetDepartments();
        List<SelectModel> GetDepartmentSelectModels();
        List<SelectListItem> GetDepartmentSelectListModels();
    }
}
