using Library_Management_Domain.Entities;
using Library_Management.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Library_Management.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AuthorService _authorService = AuthorService.Instance;

        // LIST: Active authors
        public IActionResult Index()
        {
            var authors = _authorService.GetAuthors().ToList(); 
            return View(authors);
        }

        public IActionResult Details(Guid id)
        {
            var author = _authorService.GetAuthorById(id);
            if (author == null || author.IsArchived) return NotFound();
            return View(author);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _authorService.AddAuthor(author);
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        public IActionResult Edit(Guid id)
        {
            var author = _authorService.GetAuthorById(id);
            if (author == null || author.IsArchived) return NotFound();
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Author author)
        {
            if (id != author.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                _authorService.UpdateAuthor(author);
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Archive(Guid id)
        {
            _authorService.ArchiveAuthor(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(Guid id)
        {
            _authorService.RestoreAuthor(id);
            return RedirectToAction(nameof(Archive));
        }

        public IActionResult Archive()
        {
            var archivedAuthors = _authorService.GetAllAuthors()
                .Where(a => a.IsArchived)
                .ToList();
            return View(archivedAuthors);
        }
    }
}
