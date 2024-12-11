namespace UniqloMvc.ViewModels.Commons;

public class PaginationVM
{
    public int CurrentPage { get; set; }
    public int Take { get; set; }
    public decimal PageCount { get; set; }

    public PaginationVM(int currentPage, int take, decimal productCount)
    {
        CurrentPage = currentPage;
        Take = take;
        PageCount = Math.Ceiling(productCount / Take);
    }
}
