using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Reviews;

public class CreateReviewVM
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public string UserId { get; set; }

    public string? Message { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rate { get; set; } = 1;
}
