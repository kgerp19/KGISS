using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.JobService
{
    public class JobDescriptionModel
    {
        public long JobDescriptionId { get; set; }
        public int JobDescriptionTypeId { get; set; }
        public long EmployeeId { get; set; }
        public string KGEmployeeId { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
              
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string DescriptionTypeName { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList JobDescriptionTypeList { get; set; } = new SelectList(new List<object>());
        public IEnumerable<JobDescriptionModel> DataList { get; set; }
    }
}
