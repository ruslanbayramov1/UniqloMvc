using UniqloMvc.Models;

namespace UniqloMvc.ViewModels.Reviews;

public class DetailVM
{
    public Product Product { get; set; }
    public IEnumerable<CommentShowVM> Comments { get; set; }
    public ProductReviewVM ProductReview { get; set; }
}
