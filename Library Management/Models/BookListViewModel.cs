namespace Library_Management.Models
{
    public class BookListViewModel
    {
        public Guid BookId { get; set; }
        public string? Title { get; set; } = default!;
        public string? ISBN { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? Genre { get; set; } = default!;
        public DateTime? PublishedDate { get; set; } = default!;
        public string? CoverImageUrl { get; set; } = default!;

        public string? AuthorName { get; set; } = default!;
        public string? AuthorProfileImageUrl { get; set; } = default!;
        public int TotalCopies { get; set; } = 0;
        public int AvailableCopies { get; set; } = 0;

        // NEW: List of Book Copies
        public List<BookCopyViewModel> Copies { get; set; } = new List<BookCopyViewModel>();
        public bool IsArchived { get; internal set; }
        public string Author { get; internal set; }
        public Guid Id { get; internal set; }
    }

    public class BookCopyViewModel
    {
        public Guid CopyId { get; set; }
        public DateTime? PulloutDate { get; set; }
        public string? PulloutReason { get; set; }

        // Helper to show status in UI
        public string Status => PulloutDate.HasValue ? "Pulled-out" : "Available";

        public string CoverImageUrl { get; internal set; }
    }
}
