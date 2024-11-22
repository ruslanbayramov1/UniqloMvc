using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Brands;

public class BrandUpdateVM
{
    [MaxLength(32, ErrorMessage = "Brand name must be less than 32 characters"), Required]
    public string Name { get; set; }
    public IFormFile? Logo { get; set; }
}
