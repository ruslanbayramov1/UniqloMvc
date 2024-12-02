using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Auths;

public class LoginVM
{
    [Required, MaxLength(48)]
    public string Username { get; set; }

    [Required, MaxLength(32)]
    public string Password { get; set; }
}
