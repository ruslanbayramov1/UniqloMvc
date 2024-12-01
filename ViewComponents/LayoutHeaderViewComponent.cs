using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Helpers;
using UniqloMvc.ViewModels.Baskets;

namespace UniqloMvc.ViewComponents
{
    public class LayoutHeaderViewComponent : ViewComponent
    {
        private readonly UniqloDbContext _context;

        public LayoutHeaderViewComponent(UniqloDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<BasketCookieVM> basketCookie = await CookieHelper.GetBasket(HttpContext);
            int[] ids = basketCookie.Select(x => x.Id).ToArray();
            List<BasketProductVM> basket = await _context.Products.Where(x => ids.Contains(x.Id)).Select(x => new BasketProductVM
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.CoverImage,
                SellPrice = x.Discount > 0 ? (double)(x.SellPrice - (x.SellPrice * x.Discount / 100)) : (double)x.SellPrice,
            }).ToListAsync();

            foreach (var product in basket)
            {
                product.Count = basketCookie.First(x => x.Id == product.Id).Count;
            }
            ViewBag.FullPrice = basket.Sum(x => x.Count * x.SellPrice);

            return View(basket);
        }
    }
}
