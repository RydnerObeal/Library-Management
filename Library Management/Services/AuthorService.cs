using Library_Management_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library_Management.Services
{
    public class AuthorService
    {
        private static AuthorService? _instance;
        private readonly List<Author> _authors;

        private AuthorService()
        {
            _authors = new List<Author>();
        }

        public static AuthorService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthorService();
                }
                return _instance;
            }
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            author.IsArchived = false; 
            _authors.Add(author);
        }

        public List<Author> GetAuthors()
        {
            return _authors.Where(a => !a.IsArchived).ToList();
        }

        public Author? GetAuthorById(Guid id)
        {
            return _authors.FirstOrDefault(a => a.Id == id);
        }

        public void UpdateAuthor(Author updatedAuthor)
        {
            var existing = _authors.FirstOrDefault(a => a.Id == updatedAuthor.Id);
            if (existing != null)
            {
                existing.Name = updatedAuthor.Name;
                existing.Biography = updatedAuthor.Biography;
                existing.BirthDate = updatedAuthor.BirthDate;
                existing.ProfileImageUrl = updatedAuthor.ProfileImageUrl;
            }
        }
        public void DeleteAuthor(Guid id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author != null)
            {
                _authors.Remove(author);
            }
        }

        public void ArchiveAuthor(Guid id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author != null)
                author.IsArchived = true;
        }

        public void RestoreAuthor(Guid id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author != null)
                author.IsArchived = false;
        }

        public List<Author> GetAllAuthors()
        {
            return _authors;
        }
    }
}
