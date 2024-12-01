namespace UniqloMvc.ViewModels.Products;

public class ProductCartItemVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public float Price { get; set; }
    public int Count { get; set; }
    public float PriceTotal { get; set; }
}
