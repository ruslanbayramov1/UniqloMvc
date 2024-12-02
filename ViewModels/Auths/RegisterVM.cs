using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Auths;

public class RegisterVM
{
    [Required, MaxLength(64)]
    public string Fullname { get; set; }

    [Required, MaxLength(64), DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; }

    [Required, MaxLength(48)]
    public string Username { get; set; }

    [Required, MaxLength(32), DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, MaxLength(32), DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}
