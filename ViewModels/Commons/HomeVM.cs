using UniqloMvc.Models;

namespace UniqloMvc.ViewModels.Commons;

public class HomeVM
{
    public ICollection<Slider> Sliders { get; set; }
    public ICollection<Product> Products { get; set; }
}
