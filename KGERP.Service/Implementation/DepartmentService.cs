using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        ERPEntities departmentRepository = new ERPEntities();
        public List<Department> GetDepartments()
        {
            return departmentRepository.Departments.ToList();
        }



        public List<SelectModel> GetDepartmentSelectModels()
        {
            return departmentRepository.Departments.OrderBy(x => x.Name).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.DepartmentId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetDepartmentSelectListModels()
        {
            return departmentRepository.Departments.OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name.ToString(),
                Value = x.DepartmentId.ToString()
            }).ToList();
        }
    }
}
