using System.ComponentModel.DataAnnotations.Schema;

namespace UniqloMvc.Models;

public class Review : BaseEntity
{
    public int ReviewRate { get; set; }
    public string? ReviewText { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}
