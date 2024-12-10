namespace UniqloMvc.ViewModels.Reviews
{
    public class CommentShowVM
    {
        public string Fullname { get; set; }
        public string ProfileUrl { get; set; }
        public DateTime CommentDate { get; set; }
        public string Text { get; set; }
        public int ReviewRate { get; set; }
    }
}
