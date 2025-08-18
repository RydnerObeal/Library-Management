using Library_Management.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            var books = BookService.Instance.GetBooks();
            return View(books);
        }

        public IActionResult AddModal()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult Add(AddBookViewModel vm)
        {
            
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                //AuthorName = book.AuthorName
            };

            return PartialView("_DeleteBookPartial", vm);
        }


        public IActionResult DeleteConfirmed(Guid id)
        {
            var book = BookService.Instance.GetBookById(id);
            if (book == null) return NotFound();

            BookService.Instance.DeleteBook(id);
            return Ok(); 
        }

        public IActionResult Details(Guid id)
        {
            var book = BookService.Instance.GetBooks().First(b => b.BookId == id);
            return View(book);
        }

        [HttpPost]
        public IActionResult AddCopies(EditBookViewModel vm)
        {
            if (vm.NewCopies > 0)
            {
                BookService.Instance.AddCopies(vm.BookId, vm.NewCopies);
            }
            return RedirectToAction("Index");
        }

    }
}
