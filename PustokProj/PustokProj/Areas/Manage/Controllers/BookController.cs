using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Versioning;
using PustokProj.DAL;
using PustokProj.Helpers;
using PustokProj.Models;
using PustokProj.ViewModels.BookVMs;
using System;
using System.Xml.Linq;

namespace PustokProj.Areas.Manage.Controllers
{
    [Area("manage"), Authorize(Roles = "Superadmin,Admin")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Book> books = _context.Books.Include(b => b.Genre).Include(b => b.Author).ToList();
            return View(books);
        }

        public IActionResult Create()
        {
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(BookCreateVM bookVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Authors = _context.Authors.ToList();
                return View();
            }
            if (bookVM.ImageFiles is not null)
            {
                if(bookVM.ImageFiles.Count > 10)
                ModelState.AddModelError("ImageFiles", "Max count of images - 10");
            }
            Book book = new Book
            {
                Name = bookVM.Name,
                Description = bookVM.Description,
                CostPrice = bookVM.CostPrice,
                SellPrice = bookVM.SellPrice,
                Discount = bookVM.Discount,
                ProductCode = bookVM.ProductCode,
                IsAvailable = bookVM.IsAvailable,
                IsFeatured = bookVM.IsFeatured,
                IsNew = bookVM.IsNew,
                AuthorId = bookVM.AuthorId,
                GenreId = bookVM.GenreId
            };
            (bool check, string msg) posterResult = ImageHelper.Checker(bookVM.PosterImageFile);
            if (!posterResult.check)
            {
                ModelState.AddModelError("PosterImage", posterResult.msg);
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Authors = _context.Authors.ToList();
                return View();
            }
            string posterName = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/books", bookVM.PosterImageFile);
            BookImage posterImg = new BookImage { ImageUrl = posterName, Book = book, IsPoster = true };
            _context.BookImages.Add(posterImg);

            (bool check, string msg) hoverResult = ImageHelper.Checker(bookVM.HoverImageFile);
            if (!hoverResult.check)
            {
                ModelState.AddModelError("HoverImage", hoverResult.msg);
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Authors = _context.Authors.ToList();
                return View();
            }
            string hoverName = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/books", bookVM.HoverImageFile);
            BookImage hoverImg = new BookImage { ImageUrl = hoverName, Book = book, IsPoster = false };
            _context.BookImages.Add(hoverImg);
            if (bookVM.ImageFiles is not null)
            {
                foreach (IFormFile image in bookVM.ImageFiles)
                {
                    (bool check, string msg) result = ImageHelper.Checker(image);
                    if (!result.check)
                    {
                        ModelState.AddModelError("ImageFiles", result.msg);
                        ViewBag.Genres = _context.Genres.ToList();
                        ViewBag.Authors = _context.Authors.ToList();
                        return View();
                    }
                    string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/books", image);
                    BookImage bookImg = new BookImage { ImageUrl = name, Book = book, IsPoster = null };
                    _context.BookImages.Add(bookImg);
                }
            }
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Book found = _context.Books.Find(id);
            if (found == null) return View("Error404");
            BookUpdateVM bookVM = new BookUpdateVM
            {
                Id = found.Id,
                Name = found.Name,
                Description = found.Description,
                CostPrice = found.CostPrice,
                SellPrice = found.SellPrice,
                Discount = found.Discount,
                GenreId = found.GenreId,
                AuthorId = found.AuthorId,
                IsAvailable = found.IsAvailable,
                IsFeatured = found.IsFeatured,
                IsNew = found.IsNew,
                ProductCode = found.ProductCode,
            };
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Images = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == id).ToList();
            return View(bookVM);
        }

        [HttpPost]
        public IActionResult Update(BookUpdateVM bookVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Images = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).ToList();
                return View();
            }
            Book found = _context.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == bookVM.Id);
            if (found == null) return View("Error404");

            if (bookVM.BookImageIds is null)
                bookVM.BookImageIds = new List<int>();

            if (found.BookImages.Count != bookVM.BookImageIds.Count)
            {
                List<BookImage> delBookImgs = found.BookImages.FindAll(bi => !bookVM.BookImageIds.Contains(bi.Id) && bi.IsPoster == null);
                foreach (BookImage delBookImg in delBookImgs)
                {
                    ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/books", delBookImg.ImageUrl);
                    found.BookImages.Remove(delBookImg);
                }
            }

            if (bookVM.PosterImageFile is not null)
            {
                (bool check, string msg) result = ImageHelper.Checker(bookVM.PosterImageFile);
                if (!result.check)
                {
                    ModelState.AddModelError("PosterImageFile", result.msg);
                    ViewBag.Genres = _context.Genres.ToList();
                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Images = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).ToList();
                    return View();
                }
                string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/books", bookVM.PosterImageFile);
                BookImage bookImage = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).FirstOrDefault(bi => bi.IsPoster == true);
                ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/books", bookImage.ImageUrl);
                bookImage.ImageUrl = name;
            }
            if (bookVM.HoverImageFile is not null)
            {
                (bool check, string msg) result = ImageHelper.Checker(bookVM.HoverImageFile);
                if (!result.check)
                {
                    ModelState.AddModelError("HoverImageFile", result.msg);
                    ViewBag.Genres = _context.Genres.ToList();
                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Images = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).ToList();
                    return View();
                }
                string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/books", bookVM.HoverImageFile);
                BookImage bookImage = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).FirstOrDefault(bi => bi.IsPoster == false);
                ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/books", bookImage.ImageUrl);
                bookImage.ImageUrl = name;
            }
            if (bookVM.ImageFiles is not null)
            {
                int bookImgCount = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).Count() + bookVM.ImageFiles.Count;
                if (bookImgCount > 12)
                {
                    ModelState.AddModelError("ImageFiles", "Maximum count of additional images - 10");
                    ViewBag.Genres = _context.Genres.ToList();
                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Images = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).ToList();
                    return View();
                }
                foreach(IFormFile image in bookVM.ImageFiles)
                {
                    (bool check, string msg) result = ImageHelper.Checker(image);
                    if (!result.check)
                    {
                        ModelState.AddModelError("ImageFiles", result.msg);
                        ViewBag.Genres = _context.Genres.ToList();
                        ViewBag.Authors = _context.Authors.ToList();
                        ViewBag.Images = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == bookVM.Id).ToList();
                        return View();
                    }
                    string name = ImageHelper.SaveFile(_env.WebRootPath, "uploads/images/books", image);
                    BookImage bookImage = new BookImage { ImageUrl = name, IsPoster = null, BookId = bookVM.Id };
                    found.BookImages.Add(bookImage);
                }
            }
            found.Name = bookVM.Name;
            found.Description = bookVM.Description;
            found.CostPrice = bookVM.CostPrice;
            found.SellPrice = bookVM.SellPrice;
            found.Discount = bookVM.Discount;
            found.GenreId = bookVM.GenreId;
            found.AuthorId = bookVM.AuthorId;
            found.IsAvailable = bookVM.IsAvailable;
            found.IsFeatured = bookVM.IsFeatured;
            found.IsNew = bookVM.IsNew;
            found.ProductCode = bookVM.ProductCode;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult RemoveImage(int id, int subId)
        {
            BookImage found = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == id).FirstOrDefault(bi => bi.Id == subId);
            if (found is null) return View("Error404");
            ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/books", found.ImageUrl);
            _context.BookImages.Remove(found);
            _context.SaveChanges();
            return RedirectToAction("Update", new { id = id });
        }

        public IActionResult Delete(int id)
        {
            Book found = _context.Books.Find(id);
            if (found == null) return View("Error404");
            return View(found);
        }

        [HttpPost]
        public IActionResult Delete(Book book)
        {
            Book found = _context.Books.Find(book.Id);
            if (found is null) return View("Error404");
            List<BookImage> bookImgs = _context.BookImages.Include(bi => bi.Book).Where(bi => bi.Book.Id == found.Id).ToList();
            foreach (BookImage image in bookImgs)
            {
                ImageHelper.DeleteFile(_env.WebRootPath, "uploads/images/books", image.ImageUrl);
            }
            _context.BookImages.RemoveRange(bookImgs);
            _context.Books.Remove(found);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
