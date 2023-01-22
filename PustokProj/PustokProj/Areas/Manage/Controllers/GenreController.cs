using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProj.DAL;
using PustokProj.Models;

namespace PustokProj.Areas.Manage.Controllers
{
    [Area("manage"), Authorize(Roles = "Superadmin,Admin")]
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Genre> genres = _context.Genres.ToList();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Genre found = _context.Genres.Find(id);
            if(found == null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Update(Genre genre)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            Genre found = _context.Genres.Find(genre.Id);
            if (found == null) return View("Error404");
            found.Name = genre.Name;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Genre found = _context.Genres.Find(id);
            if (found == null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Delete(Genre genre)
        {
            Genre found = _context.Genres.Find(genre.Id);
            if (found == null) return View("Error404");
            Book checkBook = _context.Books.Include(b => b.Author).FirstOrDefault(b => b.GenreId == genre.Id);
            if (checkBook is not null) return RedirectToAction("Index");
            _context.Genres.Remove(found);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
