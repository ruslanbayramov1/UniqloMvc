using System.Text.Json;
using UniqloMvc.ViewModels.Baskets;

namespace UniqloMvc.Helpers
{
    public class CookieHelper
    {
        public static async Task<List<BasketCookieVM>> GetBasket(HttpContext context)
        {
            string dataText = context.Request.Cookies["basket"] ?? "[]";
            List<BasketCookieVM> basket = JsonSerializer.Deserialize<List<BasketCookieVM>>(dataText) ?? new();
            return basket;
        }
    }
}
