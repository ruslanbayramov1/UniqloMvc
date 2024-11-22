using Microsoft.Build.Framework;

namespace UniqloMvc.Models;

public class Brand : BaseEntity
{
    public string Name { get; set; } = null!;
    public string LogoUrl { get; set; }= null!;
    public ICollection<Product>? Products { get; set; }
}
