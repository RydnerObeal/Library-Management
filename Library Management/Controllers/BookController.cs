using Library_Management.Models;
using Library_Management.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Library_Management.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            var books = BookService.Instance.GetBooks()
                .Where(b => !b.IsArchived)
                .ToList();

            return View(books);
        }

        public IActionResult AddModal()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult Add(AddBookViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            BookService.Instance.AddBook(vm);
            return Ok();
        }

        public IActionResult EditModal(Guid id)
        {
            var editBookViewModel = BookService.Instance.GetBookById(id);
            if (editBookViewModel == null) return NotFound();

            return PartialView("_EditBookPartial", editBookViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditBookViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            BookService.Instance.UpdateBook(vm);
            return Ok();
        }

        public IActionResult DeleteModal(Guid id)
        {
            var book = BookService.Instance.GetBookById(id);
            if (book == null) return NotFound();

            var vm = new DeleteBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorName = book.Author
            };

            return PartialView("_DeleteBookPartial", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var book = BookService.Instance.GetBookById(id);
            if (book == null) return NotFound();

            BookService.Instance.DeleteBook(id);
            return RedirectToAction("Index");
        }

        public IActionResult Details(Guid id)
        {
            var book = BookService.Instance.GetBooks().FirstOrDefault(b => b.BookId == id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost]
        public IActionResult AddCopies(EditBookViewModel vm)
        {
            if (vm.NewCopies > 0)
                BookService.Instance.AddCopies(vm.BookId, vm.NewCopies);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PulloutCopy(Guid copyId, string pulloutReason)
        {
            if (string.IsNullOrWhiteSpace(pulloutReason))
                return BadRequest("Reason is required.");

            BookService.Instance.PulloutCopy(copyId, pulloutReason);
            var bookId = BookService.Instance.GetBookIdByCopy(copyId);

            return RedirectToAction("Details", new { id = bookId });
        }

        public IActionResult Archive()
        {
            var archivedBooks = BookService.Instance.GetAllBooks()
                .Where(b => b.IsArchived)
                .ToList();

            return View(archivedBooks);
        }

        [HttpPost]
        public IActionResult RestoreBook(Guid id)
        {
            BookService.Instance.RestoreBook(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult ArchiveBook(Guid id)
        {
            BookService.Instance.ArchiveBook(id);
            return Ok();
        }
    }
}
