using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PustokProj.ViewModels.BasketVMs;
using PustokProj.ViewModels.BookVMs;
using System.Security.Cryptography.X509Certificates;

namespace PustokProj.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            Book found = _context.Books.Include(b => b.Author).Include(b => b.Genre).Include(b => b.BookImages).FirstOrDefault(b => b.Id == id);
            if (found is null) return View("Error404");
            BookDetailsViewModel bookDVM = new BookDetailsViewModel();
            bookDVM.Book = found;
            bookDVM.RelatedBooks = _context.Books.Include(b => b.Author).Include(b => b.BookImages).Include(b => b.Genre).Where(b => b.Genre.Id == found.Genre.Id && b.Id != found.Id).ToList();
            return View(bookDVM);
        }

        public IActionResult ShowBasket()
        {
            string basketStr = HttpContext.Request.Cookies["BasketList"];
            List<BasketVM> basketBooks = basketStr is not null ? JsonConvert.DeserializeObject<List<BasketVM>>(basketStr) : new List<BasketVM>();
            return Json(basketBooks); 
        }
    }
}
