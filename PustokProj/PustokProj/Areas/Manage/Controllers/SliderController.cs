using Microsoft.AspNetCore.Mvc;
using PustokProj.DAL;
using PustokProj.Helpers;
using PustokProj.Models;
using PustokProj.ViewModels.SliderVMs;

namespace PustokProj.Areas.Manage.Controllers
{
    [Area("manage"), Authorize(Roles = "Superadmin,Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(SliderCreateVM sliderVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            (bool check, string msg) result = ImageHelper.Checker(sliderVM.ImageFile);
            if (!result.check)
            {
                ModelState.AddModelError("ImageFile", result.msg);
                return View();
            }
            string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/sliders", sliderVM.ImageFile);
            
            Slider slider = new Slider
            {
                Title1 = sliderVM.Title1,
                Title2 = sliderVM.Title2,
                Description = sliderVM.Description,
                ButtonText = sliderVM.ButtonText,
                ImageUrl = name,
                RedirectUrl = sliderVM.RedirectUrl,
                Queue = sliderVM.Queue
            };
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Slider found = _context.Sliders.Find(id);
            if (found is null) return View("Error404");
            SliderUpdateVM sliderVM = new SliderUpdateVM
            {
                Title1 = found.Title1,
                Title2 = found.Title2,
                Description = found.Description,
                ButtonText = found.ButtonText,
                RedirectUrl = found.RedirectUrl,
                ImageUrl = found.ImageUrl,
                Queue = found.Queue
            };
            return View(sliderVM);
        }

        [HttpPost]
        public IActionResult Update(SliderUpdateVM sliderVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            Slider found = _context.Sliders.Find(sliderVM.Id);
            if (found is null) return View("Error404");
            if (sliderVM.ImageFile is not null)
            {
                (bool check, string msg) result = ImageHelper.Checker(sliderVM.ImageFile);
                if (!result.check)
                {
                    ModelState.AddModelError("ImageFile", result.msg);
                    return View();
                }
                string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/sliders", sliderVM.ImageFile);
                ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/sliders", found.ImageUrl);
                found.ImageUrl = name;
            }
            found.Title1 = sliderVM.Title1;
            found.Title2 = sliderVM.Title2;
            found.ButtonText = sliderVM.ButtonText;
            found.Description = sliderVM.Description;
            found.RedirectUrl = sliderVM.RedirectUrl;
            found.Queue = sliderVM.Queue;
            _context.SaveChanges();
            return RedirectToAction("Index");
            
        }

        public IActionResult Delete(int id)
        {
            Slider found = _context.Sliders.Find(id);
            if (found is null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Delete(Slider slider)
        {
            Slider found = _context.Sliders.Find(slider.Id);
            if (found is null) return View("Error404");
            _context.Sliders.Remove(found);
            _context.SaveChanges();
            ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/sliders", found.ImageUrl);
            return RedirectToAction("Index");
        }
    }
}
