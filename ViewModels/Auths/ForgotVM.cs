using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Auths;

public class ForgotVM
{
    [Required, MaxLength(32)]
    public string Password { get; set; }

    [Required, MaxLength(32) ,Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}
