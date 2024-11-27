using UniqloMvc.ViewModels.Brands;
using UniqloMvc.ViewModels.Products;

namespace UniqloMvc.ViewModels.Commons;

public class ShopVM
{
    public IEnumerable<ProductShopItemVM> Products { get; set; }
    public IEnumerable<BrandShopItemVM> Brands { get; set; }
}
