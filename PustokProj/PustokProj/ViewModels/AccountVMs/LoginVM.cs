using System.ComponentModel.DataAnnotations;

namespace PustokProj.ViewModels.AccountVMs
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username field is required"), StringLength(maximumLength: 18, MinimumLength = 4, ErrorMessage = "Username length must be more than 4 characters and less than 18")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password field is required"), DataType(DataType.Password), StringLength(maximumLength: 24, MinimumLength = 8, ErrorMessage = "Password length must be more than 8 characters and less than 24")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
