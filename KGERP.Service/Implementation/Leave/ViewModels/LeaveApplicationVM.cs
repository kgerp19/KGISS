using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Leave.ViewModels
{
    public class LeaveApplicationVM
    {
        [DisplayName("Leave Application")]
        public long LeaveApplicationId { get; set; }
        public long Id { get; set; }
        [DisplayName("Employee ID")]
        public string EmployeeId { get; set; }
        [DisplayName("Manager")]
        public Nullable<long> ManagerId { get; set; }
        [DisplayName("HRD Admin")]
        public Nullable<long> HrAdminId { get; set; }
        public string Manager { get; set; }
        [DisplayName("Manager Name")]
        public string ManagerName { get; set; }
        [DisplayName("Leave Type")]
        public string LeaveType { get; set; }
        [DisplayName("Category")]
        [Required()]
        public int LeaveCategoryId { get; set; }
        [Required]
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime? StartDate { get; set; }

        [Required]
        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [AttributeGreaterThan("StartDate", ErrorMessage = "The End date must be on or after the Start date.")]
        public System.DateTime? EndDate { get; set; }

        [DisplayName("Days")]
        public int LeaveDays { get; set; }
        [DisplayName("Due Leave")]
        public Nullable<int> LeaveDue { get; set; }
        [Required]
        [DisplayName("Stay During Leave")]
        public string Address { get; set; }
        [DisplayName("Contact Name")]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "Reason field is required")]
        [StringLength(250, MinimumLength = 15, ErrorMessage = "Minimum length 12 characters")]
        public string Reason { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Applied By")]
        public string AppliedBy { get; set; }
        public Nullable<int> Active { get; set; }
        public string IP { get; set; }
        [DisplayName("M Approval")]
        public string ManagerStatus { get; set; }
        [DisplayName("Manager Comments")]
        public string ManagerComment { get; set; }

        [DisplayName("HR Approval")]
        public string HrAdminStatus { get; set; }
        [DisplayName("HR Comments")]
        public string HrAdminComment { get; set; }
        [Required]
        [DisplayName("Application Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ApplicationDate { get; set; }

        public Nullable<int> OperationId { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        [DisplayName("Leave Category")]
        public virtual LeaveCategoryModel LeaveCategory { get; set; }

        public string ManagerInfo { get; set; }
        //--------------------------Extended field
        public string ApprovalPerson { get; set; }
        public string LeaveName { get; set; }
        public string KGID { get; set; }
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
    }
}
