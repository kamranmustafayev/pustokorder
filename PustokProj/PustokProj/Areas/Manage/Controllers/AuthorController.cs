using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProj.DAL;
using PustokProj.Models;

namespace PustokProj.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "Superadmin,Admin")]
	public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Author> authors = _context.Authors.ToList();
            return View(authors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Author author)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            _context.Authors.Add(author);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Author found = _context.Authors.Find(id);
            if(found == null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Update(Author author)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            Author found = _context.Authors.Find(author.Id);
            if (found == null) return View("Error404");
            found.FullName = author.FullName;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Author found = _context.Authors.Find(id);
            if (found == null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Delete(Author author)
        {
            Author found = _context.Authors.Find(author.Id);
            if (found == null) return View("Error404");
            Book checkBook = _context.Books.Include(b => b.Author).FirstOrDefault(b => b.AuthorId == author.Id);
            if (checkBook is not null) return RedirectToAction("Index");
            _context.Authors.Remove(found);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
