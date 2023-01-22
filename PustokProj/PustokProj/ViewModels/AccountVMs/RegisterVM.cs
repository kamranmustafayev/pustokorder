using System.ComponentModel.DataAnnotations;

namespace PustokProj.ViewModels.AccountVMs
{
	public class RegisterVM
	{
		[Required(ErrorMessage = "Full name field is required"), StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Full name length must be more than 4 characters and less than 50")]
		public string FullName { get; set; }
		[Required(ErrorMessage = "Email field is required"), DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Username field is required"), StringLength(maximumLength: 18, MinimumLength = 4, ErrorMessage = "Username length must be more than 4 characters and less than 18")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Password field is required"), StringLength(maximumLength: 24, MinimumLength = 8, ErrorMessage = "Password length must be more than 8 characters and less than 24"), DataType(DataType.Password)]
		public string Password { get; set; }
		[Required(ErrorMessage = "Repeat password field is required"), Compare("Password", ErrorMessage = "Passwords do not match."), DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
	}
}
