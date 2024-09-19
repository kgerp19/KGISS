using System.ComponentModel.DataAnnotations;

namespace KGERP.Models
{
    public class UserLogin
    {
        [Display(Name = "Employee ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employee ID is required")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string PWD { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}