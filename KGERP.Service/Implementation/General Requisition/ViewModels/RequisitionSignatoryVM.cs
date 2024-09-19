using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.General_Requisition.ViewModels
{
    public class RequisitionSignatoryVM
    {

        public long RequisitionSignatoryId { get; set; }
        public string KGEmployeeId { get; set; }
        public long EmployeeId { get; set; }
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        [DisplayName("Order")]
        public int OrderBy { get; set; }
        [DisplayName("Up To Amount")]
        public int CompanyId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string SignatoryEmpDesignation { get; set; }
        public string SignatoryName { get; set; }
        public string DesignationName { get; set; }
        public string IntegrateWith { get; set; }
        public decimal Limitation { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public ActionEnum  ActionId { get; set; }
        public List<RequisitionSignatoryVM> DataList { get; set; }
        public List<RequisitionApprovalVM> ApprovarList { get; set; }
        public int UserCompanyId { get; set; }
        public long SignatoryEmpId { get; set; }



        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } 
        public bool IsCategorizedSignatory { get; set; }
    }
}
