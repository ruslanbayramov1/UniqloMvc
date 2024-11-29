using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniqloMvc.DataAccess;
using UniqloMvc.Helpers;
using UniqloMvc.ViewModels.Baskets;
using UniqloMvc.ViewModels.Brands;
using UniqloMvc.ViewModels.Commons;
using UniqloMvc.ViewModels.Products;

namespace UniqloMvc.Controllers
{
    public class ShopController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index(string? amount, string? sortby, params int?[] SelectedBrand)
        {
            var query = _context.Products
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (amount != null)
            {
                IEnumerable<int> prices = amount.Split('-').Select(x => Convert.ToInt32(x));
                query = query.Where(x => x.SellPrice - (x.SellPrice * x.Discount / 100) >= prices.ElementAt(0) && x.SellPrice - (x.SellPrice * x.Discount / 100) <= prices.ElementAt(1));
            }

            if (SelectedBrand.Length != 0)
            {
                IEnumerable<string> selectedBrands = SelectedBrand.Select(x => x.HasValue ? x.Value.ToString() : "");

                int length = SelectedBrand.Length;
                if (!selectedBrands.Contains("0"))
                {
                    query = query.Where(x => selectedBrands.Contains(x.BrandId.ToString()));
                }
            }

            if (sortby != null)
            {
                if (sortby == "newness")
                {
                    query = query.OrderByDescending(x => x.CreatedTime);
                }
                else if (sortby == "oldness")
                {
                    query = query.OrderBy(x => x.CreatedTime);
                }
            }

            ShopVM vm = new ShopVM();

            var brands = await _context.Products.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.AllCount = brands.Count;

            vm.Brands = _context.Brands
                .Where(x => !x.IsDeleted)
                .Select(x => new BrandShopItemVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Count = x.Products == null ? 0 : x.Products.Where(y => !y.IsDeleted).ToList().Count
                });

            vm.Products = await query
                .Select(x => new ProductShopItemVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    SellPrice = x.SellPrice,
                    Discount = x.Discount,
                    ImageUrl = x.CoverImage
                }).ToListAsync();

            // basket part
            List<BasketCookieVM> basketCookie = await CookieHelper.GetBasket(HttpContext);
            int[] ids = basketCookie.Select(x => x.Id).ToArray();
            List<BasketProductVM> basket = await _context.Products.Where(x => ids.Contains(x.Id)).Select(x => new BasketProductVM
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.CoverImage,
                SellPrice = x.Discount > 0 ? (double)(x.SellPrice - (x.SellPrice * x.Discount / 100)): (double)x.SellPrice,
            }).ToListAsync();

            foreach (var product in basket)
            {
                var basketItem = basketCookie.FirstOrDefault(y => y.Id == product.Id);
                if (basketItem != null)
                {
                    product.Count = basketItem.Count;
                }
            }
            ViewBag.Basket = basket;
            ViewBag.FullPrice = basket.Sum(x => x.Count * x.SellPrice);

            return View(vm);
        }


        public async Task<IActionResult> AddBasket(int id)
        {
            BasketCookieVM vm = new BasketCookieVM
            {
                Id = id,
                Count = 1,
            };
            
            List<BasketCookieVM> data = await CookieHelper.GetBasket(HttpContext);

            if (data.Exists(x => x.Id == id))
            {
                var item = data.FirstOrDefault(y => y.Id == id);
                item!.Count += 1;
            }
            else
            {
                data.Add(vm);
            }

            CookieOptions opt = new CookieOptions
            { 
                MaxAge = TimeSpan.FromHours(12)
            };

            string dataText = JsonSerializer.Serialize(data);
            HttpContext.Response.Cookies.Append("basket", dataText, opt);

            return RedirectToAction(nameof(Index));
        }
    }
}
