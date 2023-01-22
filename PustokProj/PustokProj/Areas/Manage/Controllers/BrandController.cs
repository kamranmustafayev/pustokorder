using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProj.DAL;
using PustokProj.Helpers;
using PustokProj.Models;
using PustokProj.ViewModels.BrandVMs;
using System;
using System.Drawing;

namespace PustokProj.Areas.Manage.Controllers
{
    [Area("manage"), Authorize(Roles = "Superadmin,Admin")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BrandController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Brand> brands = _context.Brands.ToList();
            return View(brands);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BrandCreateVM brandVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            (bool check, string msg) result = ImageHelper.Checker(brandVM.ImageFile);
            if (!result.check)
            {
                ModelState.AddModelError("ImageFile", result.msg);
                return View();
            }
            string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/brands", brandVM.ImageFile);
            Brand brand = new Brand
            {
                ImageUrl = name,
                Queue = brandVM.Queue,
            };
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Brand found = _context.Brands.Find(id);
            if (found is null) return View("Error404");
            BrandUpdateVM brandVM = new BrandUpdateVM();
            return View(brandVM);
        }

        [HttpPost]
        public IActionResult Update(BrandUpdateVM brandVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            Brand found = _context.Brands.Find(brandVM.Id);
            if (found is null) return View("Error404");
            if (brandVM.ImageFile != null)
            {
                (bool check, string msg) result = ImageHelper.Checker(brandVM.ImageFile);
                if (!result.check)
                {
                    ModelState.AddModelError("ImageFile", result.msg);
                    return View();
                }
                string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/brands", brandVM.ImageFile);
                ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/brands", found.ImageUrl);
                found.ImageUrl = name;
            }
            found.Queue = brandVM.Queue;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Brand found = _context.Brands.Find(id);
            if (found is null) return View("Error404");
            return View(found);
        }
        [HttpPost]
        public IActionResult Delete(Brand brand)
        {
            Brand found = _context.Brands.Find(brand.Id);
            if (found is null) return View("Error404");
            _context.Brands.Remove(found);
            _context.SaveChanges();
            ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/brands", found.ImageUrl);
            return RedirectToAction("Index");
        }
    }
}
