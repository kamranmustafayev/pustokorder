using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PustokProj.ViewModels.BasketVMs;
using PustokProj.ViewModels.CartVMs;
using System.Linq;

namespace PustokProj.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public CartController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {
                List<UserBasketItem> userBasketItems = _context.UserBasketItems.Include(bi => bi.User).Include(bi => bi.Book).Include(bi => bi.Book.BookImages).Where(bi => bi.User.UserName == User.Identity.Name).ToList();
                foreach (var item in userBasketItems)
                {
                    BasketItemVM basketItem = new BasketItemVM { Book = item.Book, Count = item.Count };
                    basketItems.Add(basketItem);
                }
            }
            else
            {
				string basketCookie = HttpContext.Request.Cookies["BasketList"];
				List<BasketVM> basketBooks = null;
				if (!string.IsNullOrEmpty(basketCookie))
				{
					basketBooks = JsonConvert.DeserializeObject<List<BasketVM>>(basketCookie);
					foreach (BasketVM item in basketBooks)
					{
						BasketItemVM bookItem = new BasketItemVM { Book = _context.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == item.BookId), Count = item.Count };
						basketItems.Add(bookItem);
					}
				}
			}
            ViewBag.BasketItems = basketItems;
			return View();
        }

        public async Task<IActionResult> Add(int bookId)
        {
            if (!_context.Books.Any(b => b.Id == bookId)) return NotFound();
            List<BasketVM> basketBooks = null;
            if (User.Identity.IsAuthenticated)
            {
                UserBasketItem basketItem = _context.UserBasketItems.Include(bi => bi.User).Include(bi => bi.Book).FirstOrDefault(bi => bi.User.UserName == User.Identity.Name && bi.Book.Id == bookId);
                if (basketItem is null)
                {
                    basketItem = new UserBasketItem { BookId = bookId, User = await _userManager.FindByNameAsync(User.Identity.Name), Count = 1 };
                    await _context.UserBasketItems.AddAsync(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
				string basketStr = HttpContext.Request.Cookies["BasketList"];
				if (basketStr is not null)
				{
					basketBooks = JsonConvert.DeserializeObject<List<BasketVM>>(basketStr);
					BasketVM bskt = basketBooks.FirstOrDefault(b => b.BookId == bookId);
					if (bskt is not null) bskt.Count++;
					else
					{
						bskt = new BasketVM { BookId = bookId, Count = 1 };
						basketBooks.Add(bskt);
					}
				}
				else
				{
					basketBooks = new List<BasketVM>();
					BasketVM bskt = new BasketVM { BookId = bookId, Count = 1 };
					basketBooks.Add(bskt);
				}
				basketStr = JsonConvert.SerializeObject(basketBooks);
				HttpContext.Response.Cookies.Append("BasketList", basketStr);
			}
            return Ok();
        }

        public IActionResult Checkout()
        {
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
			if (User.Identity.IsAuthenticated)
            {
                List<UserBasketItem> userBasket = _context.UserBasketItems.Include(bi => bi.Book).Include(bi => bi.User).Where(bi => bi.User.UserName == User.Identity.Name).ToList();
                foreach (var item in userBasket)
                {
                    BasketItemVM bookItem = new BasketItemVM { Book = _context.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == item.BookId), Count = item.Count };
                    basketItems.Add(bookItem);
                }
            }
            else
            {
				string basketCookie = HttpContext.Request.Cookies["BasketList"];
				List<BasketVM> basketBooks = null;
				if (!string.IsNullOrEmpty(basketCookie))
				{
					basketBooks = JsonConvert.DeserializeObject<List<BasketVM>>(basketCookie);
					foreach (BasketVM item in basketBooks)
					{
						BasketItemVM bookItem = new BasketItemVM { Book = _context.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == item.BookId), Count = item.Count };
						basketItems.Add(bookItem);
					}
				}
			}
            OrderCM orderCM = new OrderCM
            {
                BasketItemVMs = basketItems
            };
			return View(orderCM);
		}
		public async Task<IActionResult> Update(CartUpdateVM cartVM)
		{
            if (User.Identity.IsAuthenticated)
            {
                List<UserBasketItem> userItems = _context.UserBasketItems.Include(bi => bi.User).Include(b => b.Book).Where(bi => bi.User.UserName == User.Identity.Name).ToList();
                if (cartVM.BookId is null)
                {
                    _context.UserBasketItems.RemoveRange(userItems);
                }
                else
                {
                    List<UserBasketItem> deletedItems = _context.UserBasketItems.Where(b => !cartVM.BookId.Contains(b.BookId)).ToList();
                    _context.UserBasketItems.RemoveRange(deletedItems);
                    for (int i = 0; i < cartVM.BookId.Count; i++)
                    {
                        UserBasketItem updated = userItems.FirstOrDefault(b => b.BookId == cartVM.BookId[i]);

						if (cartVM.Count[i] > 0)
                        {
							updated.Count = cartVM.Count[i];
						}
                        else
                            _context.UserBasketItems.Remove(updated);
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
				if (cartVM.BookId is null) HttpContext.Response.Cookies.Delete("BasketList");
				else
				{
					string basketCookie = HttpContext.Request.Cookies["BasketList"];
					List<BasketVM> basket = basketCookie is null ? new List<BasketVM>() : JsonConvert.DeserializeObject<List<BasketVM>>(basketCookie);
					basket.RemoveAll(b => !cartVM.BookId.Contains(b.BookId));
					for (int i = 0; i < cartVM.BookId.Count; i++)
					{
						if (cartVM.Count[i] > 0)
							basket.FirstOrDefault(b => b.BookId == cartVM.BookId[i]).Count = cartVM.Count[i];
						else
							basket.RemoveAll(b => b.BookId == cartVM.BookId[i]);
					}
					string basketStr = JsonConvert.SerializeObject(basket);
					HttpContext.Response.Cookies.Append("BasketList", basketStr);
				}
			}
            return RedirectToAction("Index");

		}

		public IActionResult ShowBlock()
        {
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {
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
            return PartialView("_CartPartial", basketItems);
        }

        public async Task<IActionResult> DeleteFromBlock(int bookId)
        {
            BasketJsonVM json = null;
			if (User.Identity.IsAuthenticated)
            {
                List<UserBasketItem> basket = _context.UserBasketItems.Include(bi => bi.User).Include(bi => bi.Book).Where(bi => bi.User.UserName == User.Identity.Name).ToList();
                UserBasketItem deleted = basket.FirstOrDefault(bi => bi.BookId == bookId);
                if (deleted == null) return NotFound();
                deleted.Count -= 1;
                if (deleted.Count <= 0)
                    _context.UserBasketItems.Remove(deleted);
                List<BasketItemVM> basketItems = new List<BasketItemVM>();
                double total = 0;
                int count = 0;
                foreach (var item in basket)
                {
                    total += item.Book.SellPrice * (1 - item.Book.Discount / 100) * item.Count;
                    count += item.Count;
                }
                json = new BasketJsonVM { Count = count, Total = total, BookPrice = deleted.Book.SellPrice * (1 - deleted.Book.Discount / 100), BookCount = deleted.Count };
                await _context.SaveChangesAsync();
            }
            else
            {
				string basketItemsStr = HttpContext.Request.Cookies["BasketList"];
				List<BasketVM> basket = basketItemsStr is null ? new List<BasketVM>() : JsonConvert.DeserializeObject<List<BasketVM>>(basketItemsStr);
				BasketVM deleted = basket.FirstOrDefault(ba => ba.BookId == bookId);
				if (deleted == null) return NotFound();
				deleted.Count -= 1;
				if (deleted.Count <= 0)
					basket.Remove(deleted);
				string basketBooksStr = JsonConvert.SerializeObject(basket);
				HttpContext.Response.Cookies.Append("BasketList", basketBooksStr);
				List<BasketItemVM> basketItems = new List<BasketItemVM>();
				double total = 0;
				int count = 0;
				foreach (BasketVM item in basket)
				{
					Book book = _context.Books.Find(item.BookId);
					total += book.SellPrice * (1 - book.Discount / 100) * item.Count;
					count += item.Count;
				}
				Book deletedBook = _context.Books.FirstOrDefault(b => b.Id == deleted.BookId);
				json = new BasketJsonVM { Count = count, Total = total, BookCount = deleted.Count, BookPrice = deletedBook.SellPrice * (1 - deletedBook.Discount / 100) };
			}
            return Json(json);
		}

        [HttpPost]
        public async Task<IActionResult> Order(OrderCM orderCM)
        {
            AppUser user = null;
            List<UserBasketItem> userBasketItems = null;
            if (User.Identity.IsAuthenticated)
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            if (user is null)
            {
                string basketCookie = HttpContext.Request.Cookies["BasketList"];
                List<BasketVM> basketBooks = null;
                if (!string.IsNullOrEmpty(basketCookie))
                {
                    basketBooks = JsonConvert.DeserializeObject<List<BasketVM>>(basketCookie);
                    foreach (BasketVM item in basketBooks)
                    {
                        BasketItemVM bookItem = new BasketItemVM { Book = _context.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == item.BookId), Count = item.Count };
                        basketItems.Add(bookItem);
                    }
                }
            }
            else
            {
                userBasketItems = _context.UserBasketItems.Include(bi => bi.User).Include(bi => bi.Book).Where(bi => bi.User == user).ToList();
				foreach (var item in userBasketItems)
                {
                    BasketItemVM bookItem = new BasketItemVM { Book = item.Book, Count = item.Count };
                    basketItems.Add(bookItem);
                }
            }
            orderCM.BasketItemVMs = basketItems;
            if (!ModelState.IsValid)
            {
                return View("Checkout", orderCM);
            }
            if (orderCM.BasketItemVMs.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View("Checkout", orderCM);
            }
            Order order = new Order
            {
                FullName = orderCM.FullName,
                Address1 = orderCM.Address1,
                Address2 = orderCM.Address2,
                City = orderCM.City,
                Country = orderCM.Country,
                ZipCode = orderCM.ZipCode,
                Note = orderCM.Note,
                PhoneNumber = orderCM.PhoneNumber,
                Email = orderCM.Email,
                OrderedAt = DateTime.UtcNow.AddHours(4),
                UserId = user?.Id
            };
            _context.Orders.Add(order);
            foreach (var item in orderCM.BasketItemVMs)
            {
                if (_context.Books.Find(item.Book.Id) is null) return BadRequest();
                OrderItem orderItem = new OrderItem { Book = item.Book, BookName = item.Book.Name, CostPrice = item.Book.CostPrice, Discount = item.Book.Discount, SellPrice = item.Book.SellPrice, Count = item.Count, Order = order };
                _context.OrderItems.Add(orderItem);
                if (user is null) HttpContext.Response.Cookies.Delete("BasketList");
                else _context.UserBasketItems.RemoveRange(userBasketItems);
			}
            _context.SaveChanges();
            return RedirectToAction("Checkout");
        }
    }
}
