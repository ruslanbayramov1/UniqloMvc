using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Brands
{
    public class BrandCreateVM
    {
        [MaxLength(32, ErrorMessage = "Brand name must be less than 32 characters") ,Required]
        public string Name { get; set; }
        [Required(ErrorMessage = "Logo must be in this field")]
        public IFormFile Logo { get; set; }
    }
}
