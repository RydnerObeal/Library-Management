public class Book
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = default!;
    public string? ISBN { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public string? Genre { get; set; } = default!;
    public DateTime? PublishedDate { get; set; } = default!;
    public Author? Author { get; set; } = default!;

    public List<BookCopy> Copies { get; set; } = new List<BookCopy>();

    public int TotalCopies => Copies.Count;

    public int AvailableCopies => Copies.Count(c => c.IsAvailable);

    public bool IsArchived { get; set; } = false;
}

public class BookCopy
{
    public Guid Id { get; set; }
    public string? CoverImageUrl { get; set; } = default!;
    public string? Condition { get; set; } = default!;
    public string? Source { get; set; } = default!;
    public DateTime? AddedDate { get; set; } = default!;
    public DateTime? PulloutDate { get; set; } = default!;
    public string? PulloutReason { get; set; } = default!;

    public bool IsAvailable { get; set; } = true;

    public Book? Book { get; set; } = default!;
    public Guid BookId { get; set; }
}

public class Author
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = default!;
    public string? Biography { get; set; } = default!;
    public DateTime? BirthDate { get; set; } = default!;
    public string? ProfileImageUrl { get; set; } = default!;
    public List<Book> Books { get; set; } = new List<Book>();
    public bool IsArchived { get; set; } = false;
}
