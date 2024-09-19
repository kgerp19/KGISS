using KGERP.Services.Procurement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Models
{
    public class PasswordChangeModel
    {
        [Display(Name = "Old Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Old Password is required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "New Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Minimum 4 characters required")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Confirm password and password do not match")]
        public string ConfirmPassword { get; set; }


    }

    public class GetEmployeePasswordVM
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeePassword { get; set; }
        public DateTime? CreateDate { get; set; }  

        public List<GetEmployeePasswordVM> EmployeePasswordList { get; set; }
    }
}