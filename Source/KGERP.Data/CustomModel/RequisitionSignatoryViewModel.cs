using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Data.CustomModel
{
    public class RequisitionSignatoryViewModel
    {
        public RequisitionSignatoryViewModel()
        {
            DataList = new List<RequisitionSignatoryViewModel>();
            Companies = new List<SelectModel>();
            Divisions = new List<SelectModel>();
            Departments = new List<SelectModel>();
            Sections = new List<SelectModel>();
            Designations = new List<SelectModel>();
            Employees = new List<SelectModel>();
            ModelNames = new List<SelectModel>();
            Categories = new List<SelectModel>();
        }

        // Primary Key
        public long RequisitionSignatoryId { get; set; }

        // Employee Information
        [DisplayName("Employee")]
        [Required(ErrorMessage = "Employee is required")]
        public long EmployeeId { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Employee Code")]
        public string EmployeeCode { get; set; }

        // Signatory Information
        [DisplayName("Signatory")]
        [Required(ErrorMessage = "Signatory is required")]
        public long SignatoryEmpId { get; set; }

        [DisplayName("Signatory Name")]
        public string SignatoryName { get; set; }

        [DisplayName("Signatory Code")]
        public string SignatoryCode { get; set; }

        [DisplayName("Designation")]
        public string DesignationName { get; set; }

        [DisplayName("DesignationSig")]
        public string DesignationNameSig { get; set; }

        // Order and Configuration
        [DisplayName("Level")]
        [Required(ErrorMessage = "Level is required")]
        [Range(1, 4, ErrorMessage = "Level must be between 1 and 4")]
        public int OrderBy { get; set; }

        [DisplayName("Is HR Admin")]
        public bool IsHRAdmin { get; set; }

        [DisplayName("Module Name")]
        [Required(ErrorMessage = "Module Name is required")]
        public string IntegrateWith { get; set; }

        [DisplayName("Limitation Amount")]
        [Required(ErrorMessage = "Limitation amount is required")]
        public decimal Limitation { get; set; }

        // Company and Department Information
        [DisplayName("Company")]
        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Division")]
        public int? DivisionId { get; set; }

        [DisplayName("Division Name")]
        public string DivisionName { get; set; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; }

        [DisplayName("Department Name")]
        public string DepartmentName { get; set; }

        [DisplayName("Section")]
        public int? SectionId { get; set; }

        [DisplayName("Section Name")]
        public string SectionName { get; set; }

        [DisplayName("Designation")]
        public int? DesignationId { get; set; }

        [DisplayName("Category")]
        public int? CategoryId { get; set; }

        [DisplayName("Category Name")]
        public string CategoryName { get; set; }

        // Audit Fields
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Status
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [DisplayName("Supreme Approved")]
        public bool IsSupremeApproved { get; set; }

        // Action Enum
        public ActionEnum ActionId { get; set; }

        // Collections for Grid and Dropdowns
        public List<RequisitionSignatoryViewModel> DataList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> Divisions { get; set; }
        public List<SelectModel> Departments { get; set; }
        public List<SelectModel> Sections { get; set; }
        public List<SelectModel> Designations { get; set; }
        public List<SelectModel> Employees { get; set; }
        public List<SelectModel> ModelNames { get; set; }
        public List<SelectModel> Categories { get; set; }

        // Filter Properties
        public int? FilterCompanyId { get; set; }
        public int? FilterDivisionId { get; set; }
        public int? FilterDepartmentId { get; set; }
        public int? FilterDesignationId { get; set; }
        public string FilterIntegrateWith { get; set; }

        // Additional Display Properties
        public string AppointmentDate { get; set; }
        public string EmployeeStatus { get; set; }
    }
}
