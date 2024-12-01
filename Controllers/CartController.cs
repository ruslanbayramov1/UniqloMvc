using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniqloMvc.DataAccess;
using UniqloMvc.Helpers;
using UniqloMvc.ViewModels.Baskets;
using UniqloMvc.ViewModels.Products;

namespace UniqloMvc.Controllers;

public class CartController : Controller
{
    private readonly UniqloDbContext _context;
    public CartController(UniqloDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        #region Explanation
        // getting data (id, count) from cookie we store
        // calling all products, which contains basket's id in stored cookie and selecting them as ProductCartItemVM to ommit unnecessary data
        #endregion

        List<BasketCookieVM> basket = await CookieHelper.GetBasket(HttpContext);
        int[] ids = basket.Select(x => x.Id).ToArray();

        List<ProductCartItemVM> cart = await _context.Products
            .Where(x => ids.Contains(x.Id))
            .Select(x => new ProductCartItemVM
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.CoverImage,
                Price = x.Discount == 0 ?
                (float)(x.SellPrice) :
                (float)(x.SellPrice - (x.SellPrice * x.Discount / 100))
            }).ToListAsync();

        foreach (var item in cart)
        {
            item.Count = basket.First(x => x.Id == item.Id).Count;
            item.PriceTotal = item.Count * item.Price;
        }
        ViewBag.AllTotalPrice = cart.Sum(x => x.PriceTotal);
        return View(cart);
    }

    public async Task<IActionResult> RemoveBasketItem(int id)
    {
        List<BasketCookieVM> basket = await CookieHelper.GetBasket(HttpContext);
        BasketCookieVM? item = basket.FirstOrDefault(x => x.Id == id);
        if (item == null) return BadRequest();

        basket.Remove(item);

        string dataText = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket", dataText);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct()
    {
        #region Explanation
        // The Request.Form contains keys that map to arrays of data, and the data is ordered.
        // In this example, we have multiple data inputs with the field names "Id" and "Count".
        // The Request.Form uses the key names to access the data arrays. For example, if the data is:
        // { Id: 5, Count: 1 } and { Id: 8, Count: 2 },
        // it will be represented as:
        // Request.Form["Id"] = { 5, 8 }
        // Request.Form["Count"] = { 1, 2 }
        //
        // The elements at the same index in both arrays correspond to the same product.
        // For instance, the 1st item in the "Id" array corresponds to the 1st item in the "Count" array.
        //
        // In this code, we retrieve the data from the form and check if the product IDs match the IDs
        // stored in the user's basket (from a cookie). If a match is found, we update the product's count
        // in the basket with the new value from the form data.
        //
        // Finally, we save the updated basket back to the cookie with the new values.
        #endregion

        var formData = Request.Form;
            
        // Get the product IDs and counts from the form data
        int[] ids = formData["Id"].Select(x => Convert.ToInt32(x)).ToArray();
        int[] counts = formData["Count"].Select(x => Convert.ToInt32(x)).ToArray();

        // Retrieve the current basket data from the cookie
        List<BasketCookieVM> basket = await CookieHelper.GetBasket(HttpContext);
         
        int index;
        List<int> itemsToRemove = [];
        foreach (var item in basket)
        {
            index = Array.IndexOf(ids, item.Id);
            if (index >= 0)
            {
                if (counts[index] > 0)
                {
                    item.Count = counts[index];
                }
                else
                {
                    itemsToRemove.Add(basket.IndexOf(item));
                }
            }
        }

        itemsToRemove.ForEach(x => basket.RemoveAt(x));

        // Save the updated basket back to the cookie
        await CookieHelper.SetBasket(basket, HttpContext);

        return RedirectToAction(nameof(Index));
    }
}
