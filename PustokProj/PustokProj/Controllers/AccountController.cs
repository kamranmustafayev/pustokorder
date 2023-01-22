namespace PustokProj.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly AppDbContext _context;
		public AccountController(UserManager<AppUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_context = context;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVM loginVM)
		{
			if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
			if (!ModelState.IsValid) return View();
			AppUser found = await _userManager.FindByNameAsync(loginVM.UserName);
			if (found == null)
			{
				ModelState.AddModelError("", "Wrong username or password");
				return View();
			}
			var validatePass = await _userManager.CheckPasswordAsync(found, loginVM.Password);
			if (!validatePass)
			{
				ModelState.AddModelError("", "Wrong username or password");
				return View();
			}
			await _signInManager.SignInAsync(found, loginVM.RememberMe);
			return RedirectToAction("Index", "Home");
		}
		public IActionResult Register()
		{
			if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM registerVM)
		{
			if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
			if (!ModelState.IsValid) return View();
			AppUser found = await _userManager.FindByNameAsync(registerVM.UserName);
			if (found is not null)
			{
				ModelState.AddModelError("Username", "Username has been taken");
				return View();
			}
			found = await _userManager.FindByEmailAsync(registerVM.Email);
			if (found is not null)
			{
				ModelState.AddModelError("Email", "This email has already been registered");
				return View();
			}
			AppUser user = new AppUser { FullName = registerVM.FullName, Email = registerVM.Email, UserName = registerVM.UserName };
			var result = await _userManager.CreateAsync(user, registerVM.Password);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", result.Errors.FirstOrDefault().Description);
				return View();
			}
			result = await _userManager.AddToRoleAsync(user, "Member");
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Unknown error");
				return View();
			}
			return RedirectToAction("Login");
		}

		public async Task<IActionResult> Logout()
		{
			if (!User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
