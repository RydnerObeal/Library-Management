namespace Library_Management.Models
{
    public class AddCopiesViewModel
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CurrentCopies { get; set; }
        public int NumberOfCopies { get; set; }
    }
}
