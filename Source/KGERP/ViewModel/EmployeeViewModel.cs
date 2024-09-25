using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.ViewModel
{
    public class EmployeeViewModel
    {
        public EmployeeModel Employee { get; set; }

        public List<SelectModel> Managers { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> Religions { get; set; }
        public List<SelectModel> BloodGroups { get; set; }
        public List<SelectModel> Countries { get; set; }
        public List<SelectModel> Districts { get; set; }
        public List<SelectModel> Divisions { get; set; }
        public List<SelectModel> Upazilas { get; set; }
        public List<SelectModel> MaritalTypes { get; set; }
        public List<SelectModel> Genders { get; set; }

        public List<SelectModel> EmployeeCategories { get; set; }
        public List<SelectModel> Departments { get; set; }
        public List<SelectModel> Designations { get; set; }
        public List<SelectModel> OfficeTypes { get; set; }
        public List<SelectModel> DisverseMethods { get; set; }
        public List<SelectModel> JobStatus { get; set; }
        public List<SelectModel> JobTypes { get; set; }
        public List<SelectModel> Banks { get; set; }
        public List<SelectModel> Shifts { get; set; }
        public List<SelectModel> SalaryGrades { get; set; }
        public List<SelectModel> Actives { get; set; }
        public List<SelectModel> BankBranches { get; set; }
        public List<SelectModel> RegionDistricts { get; set; }

        public List<SelectListItem> DDLVendor { get; set; }
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());
    }
}