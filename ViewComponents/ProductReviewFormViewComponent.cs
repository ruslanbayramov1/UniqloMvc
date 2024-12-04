using Microsoft.AspNetCore.Mvc;

namespace UniqloMvc.ViewComponents;

public class ProductReviewFormViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}
