using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Auths;

public class ProfileUpdateVM
{
    [Required, MaxLength(64)]
    public string Fullname { get; set; }

    [Required, MaxLength(48)]
    public string Username { get; set; }

    [Required]
    public IFormFile ProfilePhoto { get; set; }
}
