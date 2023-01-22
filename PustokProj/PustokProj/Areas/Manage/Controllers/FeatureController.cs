using Microsoft.AspNetCore.Mvc;
using PustokProj.DAL;
using PustokProj.Models;

namespace PustokProj.Areas.Manage.Controllers
{
    [Area("manage"), Authorize(Roles = "Superadmin,Admin")]
    public class FeatureController : Controller
    {
        private readonly AppDbContext _context;

        public FeatureController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Feature> features = _context.Features.ToList();
            return View(features);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Feature feature)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            _context.Features.Add(feature);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Feature found = _context.Features.Find(id);
            if (found is null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Update(Feature feature)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            Feature found = _context.Features.Find(feature.Id);
            if (found is null) return View("Error404");
            found.Title = feature.Title;
            found.Description = feature.Description;
            found.Icon = feature.Icon;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            Feature found = _context.Features.Find(id);
            if (found is null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Delete(Feature feature)
        {
            Feature found = _context.Features.Find(feature.Id);
            if (found is null) return View("Error404");
            _context.Features.Remove(found);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
