using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class LeaveApplicationModel
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
        #region new_added
        public List<LeaveApplication> LeaveApplications { get; set; }
        public List<LeaveApplicationModel> AllInfo { get; set; }
        public List<SignatoryApprovalMap> Signatories { get; set; }
        public HashSet<long> UniqueApplicationIds { get; set; }
        public int ApprovalLevelCount { get; set; }

        #endregion
    }
    public class ManagerApproval
    {
        public List<LeaveApplication> LeaveApplications { get; set; }
        public List<SignatoryApprovalMap> SignatoryApprovalMaps { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Department> Departments { get; set; }
        public List<LeaveCategory> LeaveCategories { get; set; }
        public List<SignatoryApprovalMap> AllSignatories { get; set; }
        public long CurrentSignatoryID { get; set; }
        public int MaxOrderBy { get; set; }
        public string[] SigArray { get; set; }
    }
    public class LeaveApplicationVmm
    {
        public long LeaveApplicationId { get; set; }
        public long Id { get; set; }
        public int managerOrder { get; set; }
        public int hrOrder { get; set; }
        public string EmployeeId { get; set; }
        public long EmpId { get; set; }
        public long? ManagerId { get; set; }
        public long? HrAdminId { get; set; }
        public string EmployeeName { get; set; }
        public string CategoryName { get; set; }
        public string AppliedBy { get; set; }

        public string Reason { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveDays { get; set; }
        public string ManagerStatus { get; set; }
        public string HrAdminStatus { get; set; }
        public string ManagerName { get; set; }
        public string Hrname { get; set; }

        public IEnumerable<LeaveApplicationVm> DataList { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public int CompanyId { get; set; }
        public List<signatry> othersData { get; set; }

    }

    public class LeaveApplicationVm
    {

        public int LeaveStatus { get; set; }

        public long LeaveApplicationId { get; set; }
        public long Id { get; set; }
        public int managerOrder { get; set; }
        public int hrOrder { get; set; }
        public string EmployeeId { get; set; }
        public long EmpId { get; set; }
        public long? ManagerId { get; set; }
        public long? HrAdminId { get; set; }
        public string EmployeeName { get; set; }
        public string CategoryName { get; set; }
        public string AppliedBy { get; set; }

        public string Reason { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveDays { get; set; }
        public string ManagerStatus { get; set; }
        public string HrAdminStatus { get; set; }
        public string ManagerName { get; set; }
        public string Hrname { get; set; }

        public IEnumerable<LeaveApplicationVm> DataList { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public int CompanyId { get; set; }
        public List<signatry> othersData { get; set; }
        public List<RequisitionSignatoryVM> DataLIstSignatory { get; set; }

        #region new_added
        public int LevelOneApproval { get; set; }
        public int LevelTwoApproval { get; set; }
        public int LevelThreeApproval { get; set; }
        public int HRApproval { get; set; }
        public int ApprovalLevelCount { get; set; }
        public List<int> Levels { get; set; }
        public string Designation { get; set; }
        public string Status { get; set; }
        //public int? CompanyID { get; set; }
        public List<LeaveApplicationVm> LeaveApplications { get; set; }
        public string TableName { get; set; }
        #endregion
    }



    public class signatry
    {
        public long SignatoryEmpId { get; set; }
        public int OrderBy { get; set; }
    }


}
