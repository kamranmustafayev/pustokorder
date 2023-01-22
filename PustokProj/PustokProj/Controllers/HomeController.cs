using PustokProj.ViewModels.HomeVMs;

namespace PustokProj.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Sliders = _context.Sliders.OrderBy(sl => sl.Queue).ToList(),
                Features = _context.Features.ToList(),
                Brands = _context.Brands.ToList(),
                FeaturedBooks = _context.Books.Include(b => b.Author).Include(b => b.Genre).Where(b => b.IsFeatured == true).ToList(),
                NewBooks = _context.Books.Include(b => b.Author).Include(b => b.Genre).Where(b => b.IsNew == true).ToList(),
                BooksOnDiscount = _context.Books.Include(b => b.Author).Include(b => b.Genre).Where(b => b.Discount > 0).ToList(),
                ClassicBooks = _context.Books.Include(b => b.Author).Include(b => b.Genre).Where(b => b.Genre.Id == 7).ToList(),
                BookImages = _context.BookImages.ToList(),
            };
            return View(homeVM);
        }
    }
}
