using Library_Management.Models;
using Library_Management_Domain.Entities;

public class BookService
{
    private readonly ICollection<Book> _books = new List<Book>();
    private readonly ICollection<Author> _authors = new List<Author>();
    private readonly ICollection<BookCopy> _bookCopies = new List<BookCopy>();

    private BookService()
    {
        SeedData();
    }

    private void SeedData()
    {

    }

    public void AddBook(AddBookViewModel vm)
    {
        ArgumentNullException.ThrowIfNull(vm, nameof(vm));

        var author = _authors.FirstOrDefault(a => a.Name == vm.Author);
        if (author == null)
        {
            author = new Author
            {
                Id = Guid.NewGuid(),
                Name = vm.Author,
                ProfileImageUrl = vm.AuthorProfileImageUrl,
                Books = new List<Book>()
            };
            _authors.Add(author);
        }

        var newBook = new Book
        {
            Id = Guid.NewGuid(),
            Title = vm.Title,
            ISBN = vm.ISBN,
            Description = vm.Description,
            Genre = vm.Genre,
            PublishedDate = vm.PublishedDate,
            Author = author,
            IsArchived = false // NEW: default active
        };
        _books.Add(newBook);
        author.Books.Add(newBook);

        var newBookCopy = new BookCopy
        {
            Id = Guid.NewGuid(),
            BookId = newBook.Id,
            CoverImageUrl = vm.CoverImageUrl,
            Condition = vm.Condition,
            Source = vm.Source,
            AddedDate = DateTime.Now,
            Book = newBook
        };
        _bookCopies.Add(newBookCopy);
    }

    public IEnumerable<BookListViewModel> GetBooks()
    {
        // Filter out archived books
        return _books.Where(b => !b.IsArchived).Select(b => new BookListViewModel
        {
            BookId = b.Id,
            Title = b.Title,
            ISBN = b.ISBN,
            Description = b.Description,
            Genre = b.Genre,
            PublishedDate = b.PublishedDate,
            CoverImageUrl = _bookCopies.FirstOrDefault(bi => bi.BookId == b.Id)?.CoverImageUrl,
            AuthorName = b.Author?.Name,
            AuthorProfileImageUrl = b.Author?.ProfileImageUrl,
            TotalCopies = _bookCopies.Count(bi => bi.BookId == b.Id),
            AvailableCopies = _bookCopies.Count(bi => bi.BookId == b.Id && bi.PulloutDate == null),
            Copies = _bookCopies
                .Where(c => c.BookId == b.Id)
                .Select(c => new BookCopyViewModel
                {
                    CopyId = c.Id,
                    PulloutDate = c.PulloutDate,
                    PulloutReason = c.PulloutReason
                }).ToList()
        }).ToList();
    }

    public EditBookViewModel GetBookById(Guid id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id) ?? throw new KeyNotFoundException("Book not found");
        return new EditBookViewModel
        {
            BookId = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Description = book.Description,
            Genre = book.Genre,
            PublishedDate = book.PublishedDate,
            AuthorId = book.Author?.Id,
            Author = book.Author?.Name,
            AuthorProfileImageUrl = book.Author?.ProfileImageUrl,
            CoverImageUrl = _bookCopies.FirstOrDefault(bi => bi.BookId == book.Id)?.CoverImageUrl
        };
    }

    internal void UpdateBook(EditBookViewModel vm)
    {
        ArgumentNullException.ThrowIfNull(vm, nameof(vm));

        var book = _books.FirstOrDefault(b => b.Id == vm.BookId) ?? throw new KeyNotFoundException("Book not found");

        book.Title = vm.Title;
        book.ISBN = vm.ISBN;
        book.Description = vm.Description;
        book.Genre = vm.Genre;
        book.PublishedDate = vm.PublishedDate;

        var author = _authors.FirstOrDefault(a => a.Id == vm.AuthorId);
        if (author == null)
        {
            author = new Author
            {
                Id = Guid.NewGuid(),
                Name = vm.Author,
                ProfileImageUrl = vm.AuthorProfileImageUrl,
                Books = new List<Book>()
            };
            _authors.Add(author);
        }

        if (book.Author != author)
        {
            book.Author?.Books.Remove(book);
            book.Author = author;
            author.Books.Add(book);
        }

        var bookCopy = _bookCopies.FirstOrDefault(bi => bi.BookId == vm.BookId);
        if (bookCopy != null)
        {
            bookCopy.CoverImageUrl = vm.CoverImageUrl;
        }
        else
        {
            _bookCopies.Add(new BookCopy
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                CoverImageUrl = vm.CoverImageUrl,
                AddedDate = DateTime.Now,
                Book = book
            });
        }
    }

    public void DeleteBook(Guid id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id) ?? throw new KeyNotFoundException("Book not found");
        _books.Remove(book);

        book.Author?.Books.Remove(book);
        if (book.Author != null && !book.Author.Books.Any())
        {
            _authors.Remove(book.Author);
        }

        var bookCopies = _bookCopies.Where(bi => bi.BookId == id).ToList();
        foreach (var copy in bookCopies)
        {
            _bookCopies.Remove(copy);
        }
    }

    public void AddCopies(Guid bookId, int numberOfCopies)
    {
        var book = _books.FirstOrDefault(b => b.Id == bookId) ?? throw new KeyNotFoundException("Book not found");

        for (int i = 0; i < numberOfCopies; i++)
        {
            _bookCopies.Add(new BookCopy
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                Condition = "New",
                Source = "Purchase",
                AddedDate = DateTime.Now,
                Book = book
            });
        }
    }

    // --- Pull-out functionality ---
    public void PulloutCopy(Guid copyId, string reason)
    {
        var copy = _bookCopies.FirstOrDefault(c => c.Id == copyId);
        if (copy != null)
        {
            copy.PulloutDate = DateTime.Now;
            copy.PulloutReason = reason;
        }
    }

    public Guid GetBookIdByCopy(Guid copyId)
    {
        var copy = _bookCopies.FirstOrDefault(c => c.Id == copyId);
        return copy?.BookId ?? Guid.Empty;
    }

    // --- Archive / Restore ---
    public void ArchiveBook(Guid id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book != null)
            book.IsArchived = true;
    }

    public void RestoreBook(Guid id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book != null)
            book.IsArchived = false;
    }

    // NEW: Get all books including archived
    public IEnumerable<BookListViewModel> GetAllBooks()
    {
        return _books.Select(b => new BookListViewModel
        {
            BookId = b.Id,
            Title = b.Title,
            ISBN = b.ISBN,
            Description = b.Description,
            Genre = b.Genre,
            PublishedDate = b.PublishedDate,
            CoverImageUrl = _bookCopies.FirstOrDefault(bi => bi.BookId == b.Id)?.CoverImageUrl,
            AuthorName = b.Author?.Name,
            AuthorProfileImageUrl = b.Author?.ProfileImageUrl,
            TotalCopies = _bookCopies.Count(bi => bi.BookId == b.Id),
            AvailableCopies = _bookCopies.Count(bi => bi.BookId == b.Id && bi.PulloutDate == null),
            Copies = _bookCopies
                .Where(c => c.BookId == b.Id)
                .Select(c => new BookCopyViewModel
                {
                    CopyId = c.Id,
                    PulloutDate = c.PulloutDate,
                    PulloutReason = c.PulloutReason
                }).ToList()
        }).ToList();
    }

    // Singleton
    private static BookService? _instance;
    public static BookService Instance => _instance ??= new BookService();

    public ICollection<Author> Authors => _authors;
}
