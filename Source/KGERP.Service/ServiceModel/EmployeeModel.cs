using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;

namespace KGERP.Service.ServiceModel
{
    public class EmployeeModel
    {
        public string ButtonName
        {
            get
            {
                return Id > 0 ? "Update" : "Save";
            }

        }
        public Nullable<long> PRollSheetId { get; set; }
        public string BankAccountName { get; set; }

        public long Id { get; set; }
        [DisplayName("Employee ID")]
        public string EmployeeId { get; set; }
        [DisplayName("Manager")]
        public Nullable<long> ManagerId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }

        public string CompanyName { get; set; }

        [DisplayName("Card No")]
        public string CardId { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Nick Name")]
        public string ShortName { get; set; }

        [DisplayName("Present Address")]
        public string PresentAddress { get; set; }
        [DisplayName("Father's Name")]
        public string FatherName { get; set; }
        [DisplayName("Mother's Name")]
        public string MotherName { get; set; }
        [DisplayName("Spouse")]
        public string SpouseName { get; set; }
        [DisplayName("Telephone")]
        public string Telephone { get; set; }
        [DisplayName("PABX")]
        public string PABX { get; set; }
        [DisplayName("Mobile No")]
        public string MobileNo { get; set; }
        [DisplayName("Fax No")]
        public string FaxNo { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DisplayName("Personal Email")]
        public string Email { get; set; }
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string OfficeEmail { get; set; }
        [DisplayName("Permanent Address")]
        public string PermanentAddress { get; set; }
        [DisplayName("Department")]
        public Nullable<int> DepartmentId { get; set; }
        [DisplayName("Designation")]
        public Nullable<int> DesignationId { get; set; }

        [DisplayName("Job Status")]
        public Nullable<int> JobStatusId { get; set; }
        [DisplayName("Joining Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage ="Joinig date is required")]
        public Nullable<System.DateTime> JoiningDate { get; set; }


        [DisplayName("Probation End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ProbationEndDate { get; set; }

        [DisplayName("Permanent Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> PermanentDate { get; set; }

        public bool Active { get; set; }
        [DisplayName("Shift")]
        public Nullable<int> ShiftId { get; set; }
        [DisplayName("Date of Birth")]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateOfBirth { get; set; }
        [DisplayName("Date of Marriage")]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DateOfMarriage { get; set; }
        [DisplayName("Grade")]
        public Nullable<int> GradeId { get; set; }
        [DisplayName("Country")]
        public Nullable<int> CountryId { get; set; }
        [DisplayName("Gender")]
        public Nullable<int> GenderId { get; set; }
        [DisplayName("Marital Type")]
        public Nullable<int> MaritalTypeId { get; set; }
        [DisplayName("Division")]
        public Nullable<int> DivisionId { get; set; }

        public string Division { get; set; }
        [DisplayName("District")]
        public Nullable<int> DistrictId { get; set; }

        public string DistrictName { get; set; }
        public string Upzilla { get; set; }
        [DisplayName("Month")]
        public string TotalMonth { get; set; }

        [DisplayName("Upzilla")]
        public Nullable<int> UpzillaId { get; set; }

        [DisplayName("Bank")]
        public Nullable<int> BankId { get; set; }
        [DisplayName("Bank Branch")]
        public Nullable<int> BankBranchId { get; set; }

        [DisplayName("Account Number")]
        public string BankAccount { get; set; }
        [DisplayName("D. License No")]
        public string DrivingLicenseNo { get; set; }
        [DisplayName("Passport No")]
        public string PassportNo { get; set; }
        [DisplayName("Social ID")]
        public string SocialId { get; set; }
        [DisplayName("NID")]
        public string NationalId { get; set; }
        [DisplayName("TIN No")]
        public string TinNo { get; set; }

        public string Religion { get; set; }

        [DisplayName("Religion")]
        public Nullable<int> ReligionId { get; set; }
        [DisplayName("Blood Group")]
        public Nullable<int> BloodGroupId { get; set; }
        [DisplayName("Employee Category")]
        public Nullable<int> EmployeeCategoryId { get; set; }
        [DisplayName("Service Type")]
        public Nullable<int> ServiceTypeId { get; set; }
        [DisplayName("Disburse Method")]
        public Nullable<int> DisverseMethodId { get; set; }
        public Nullable<int> DesignationFlag { get; set; }

        public string ImageFileName { get; set; }
        [DisplayName("Signature File Name")]
        public string SignatureFileName { get; set; }
        [DisplayName("Office Type")]
        public string OfficeType { get; set; }
        [DisplayName("Office Type")]
        public Nullable<int> OfficeTypeId { get; set; }

        [DisplayName("Resign Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        [DisplayName("Reason")]
        public string EndReason { get; set; }

        [DisplayName("Employee Order")]
        public int EmployeeOrder { get; set; }

        [DisplayName("Salary Tag")]
        public int SalaryTag { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created Date")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [DisplayName("Modified By")]
        public string ModifedBy { get; set; }
        [DisplayName("Modified Date")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Event Date")]
        public string EventDate { get; set; }
        public string PasswordText { get; set; }
        public string EDepartment { get; set; }
        public string EDesignation { get; set; }
        public string Anniversary { get; set; }
        public string ECompany { get; set; }
        public string Grade { get; set; }
        public string GradeName { get; set; }
        public int? VendorId { get; set; }
        //public virtual BankModel Bank { get; set; }
        //public virtual BankBranchModel BankBranch { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual DepartmentModel Department { get; set; }
        public virtual DesignationModel Designation { get; set; }
        public virtual DistrictModel District { get; set; }
        public virtual DropDownItemModel DropDownItem { get; set; }
        public virtual DropDownItemModel DropDownItem1 { get; set; }
        public virtual DropDownItemModel DropDownItem2 { get; set; }
        public virtual DropDownItemModel DropDownItem3 { get; set; }
        public virtual DropDownItemModel DropDownItem4 { get; set; }
        public virtual DropDownItemModel DropDownItem5 { get; set; }
        public virtual DropDownItemModel DropDownItem6 { get; set; }
        public virtual DropDownItemModel DropDownItem7 { get; set; }
        public virtual DropDownItemModel DropDownItem8 { get; set; }
        public virtual DropDownItemModel DropDownItem9 { get; set; }
        public virtual DistrictModel RegionDistricts { get; set; }
        //public virtual EmployeeModel Employee2 { get; set; }
        public List<SelectVmAcc> DropdownForBank { get; set; }
        public virtual EmployeeModel Employee3 { get; set; }
        public virtual ShiftModel Shift { get; set; }

        public virtual ICollection<FileAttachment> FileAttachments { get; set; }
        public string ImagePath { get; set; }

        public string SignaturePath { get; set; }

        //-------------Extended Properties----------------
        public string SearchText { get; set; }
        public string StrJoiningDate { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string BloodGroupName { get; set; }
        public string EmployeeIdOfManager { get; set; }
        [DisplayName("Account Name")]
        public string AccouName { get; set; }
        [Required]
        public string ManagerName { get; set; }
        public long HrAdminId { get; set; }
        #region newAdded
        [DisplayName("Region")]
        public Nullable<int> RegionDistictId { get; set; }
        [DisplayName("Service Company")]
        public List<SelectModel> DDLServiceCompany { get; set; }
        [DisplayName("Service Company")]
        public string ServiceCompany { get; set; }
        #endregion

    }
    public class SelectVmAcc
    {
        public int Id { get; set; }
        public string value { get; set; }
     
    }
    public class EmployeeVm
    {
        public long Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string BankAccNum { get; set; }
        public string BankName {get; set; }
        public string AccName {get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string MobileNo { get; set; }
        public string OfficeEmail { get; set; }
        public string SainatureName { get; set; }
        public string GradeCode { get; set; }
        public string GradeName { get; set; }
        public string BloodGroup { get; set; }
        public string Remarks { get; set; }
        public string PABX { get; set; }
        public IEnumerable<EmployeeVm> DataList { get; set; }
    }


    public class VmPayRoll
    {
        public long PayRollId { get; set; }
        public int CompanyId { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
        public string Note { get; set; }
        public bool IsClose { get; set; }
        public bool IsTest { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyName { get; set; }
        public string GradesCode { get; set; }
        public string GradesName { get; set; }
        public string DesignationsName { get; set; }
        public string EmployeesMobileNo { get; set; }
        public string EmployeesOfficeEmail { get; set; }
        public string EmployeesPABX { get; set; }
        public DateTime? EmployeesJoiningDate { get; set; }
        public string EmployeesName { get; set; }


    
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<VmPayRoll> DataList { get; set; }
    }

    public class VmPayRollDetail : VmPayRoll
    {
        public long PayRollDetailId { get; set; }
      
        public long EmployeeId { get; set; }
        public int PaymentPurposeId { get; set; }
        public string PaymentPurposeName { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
       
        public int CalculationType { get; set; }
        public IEnumerable<VmPayRollDetail> DataListAddition { get; set; }
        public IEnumerable<VmPayRollDetail> DataListDeduction { get; set; }
        public IEnumerable<VmPayRollDetail> DataListNotDefined { get; set; }
    }
}
