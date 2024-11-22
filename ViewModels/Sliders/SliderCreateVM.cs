using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Sliders
{
    public class SliderCreateVM
    {
        [MaxLength(32, ErrorMessage = "Title can not be more than 32 characters"), Required]
        public string Title { get; set; }
        [MaxLength(64, ErrorMessage = "Subtitle can not be more than 64 characters"), Required]
        public string Subtitle { get; set; }
        public string? Link { get; set; }
        [Required(ErrorMessage = "You must choose a file")]
        public IFormFile File { get; set; }
    }
}
