using System;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class UserModel
    {
        public int UserId { get; set; }
        // public string UserName { get; set; }
        // public string Email { get; set; }
        //  public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public Nullable<System.Guid> ActivationCode { get; set; }


        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name required")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Minimum 4 characters required")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password and password do not match")]

        public bool Active { get; set; }
        public string ConfirmPassword { get; set; }

        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}
