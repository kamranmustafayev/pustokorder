using Newtonsoft.Json;
using PustokProj.ViewModels.BasketVMs;
using PustokProj.ViewModels.HeaderVMs;

namespace PustokProj.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HeaderViewComponent(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Genre> genres = _context.Genres.ToList();
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            AppUser user = null;
            List<string> roles = null;
			if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				var awaitRoles = await _userManager.GetRolesAsync(user);
				roles = awaitRoles.ToList();
                List<UserBasketItem> basket = _context.UserBasketItems.Include(bi => bi.Book).Include(bi => bi.Book.BookImages).Include(bi => bi.User).Where(bi => bi.User.UserName == User.Identity.Name).ToList();
                foreach (var item in basket)
                {
                    BasketItemVM basketItem = new BasketItemVM { Book = item.Book, Count = item.Count };
                    basketItems.Add(basketItem);
                }
			}
            else
            {
				string basketItemsStr = HttpContext.Request.Cookies["BasketList"];
				List<BasketVM> basket = basketItemsStr is null ? new List<BasketVM>() : JsonConvert.DeserializeObject<List<BasketVM>>(basketItemsStr);
				foreach (BasketVM item in basket)
				{
					BasketItemVM basketItem = new BasketItemVM { Book = _context.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == item.BookId), Count = item.Count };
					basketItems.Add(basketItem);
				}
			}
            
            HeaderVM headerVM = new HeaderVM { BasketItems = basketItems, Genres = genres, Account = new AccountBlockVM { UserInfo = user, Roles = roles } };
            return View(await Task.FromResult(headerVM));
        }
    }
}
