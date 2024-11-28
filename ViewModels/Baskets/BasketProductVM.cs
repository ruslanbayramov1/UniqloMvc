namespace UniqloMvc.ViewModels.Baskets;

public class BasketProductVM : BasketCookieVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public double SellPrice { get; set; }
    public string ImageUrl { get; set; }
}
