using UniqloMvc.Models;
using UniqloMvc.ViewModels.Brands;

namespace UniqloMvc.ViewModels.Commons;

public class HomeVM
{
    public ICollection<Slider> Sliders { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<BrandHomeItemVM> Brands { get; set; }
}
