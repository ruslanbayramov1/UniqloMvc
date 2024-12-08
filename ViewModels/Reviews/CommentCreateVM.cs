using System.ComponentModel.DataAnnotations;

namespace UniqloMvc.ViewModels.Reviews;

public class CommentCreateVM
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    public string? Text { get; set; }
    [Required]
    [Range(1, 5)]
    public int ReviewRate { get; set; } = 1;
}
