using BookWiseAPI.Model;
using BookWiseAPI.Services.Interfaces;
using DataLibrary;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookWiseAPI.Services
{
    internal class AuthorUpdateService : IAuthorUpdate
    {
        private  BooksContext _booksContext;
        public AuthorUpdateService(BooksContext booksContext)
        {
            _booksContext = booksContext;
        }
        public async Task<Author> Update(string findAuthor,FulAuthorDto authorDto)
        {
            var result = Regex.Split(findAuthor, @"(?<=[а-яА-ЯёЁіІїЇєЄґҐA-Za-z])(?=[А-ЯA-Z])");
            Author resAuthor = new Author();

            if (result.Length == 3)
            {
                resAuthor =  _booksContext.Authors
               .FirstOrDefault(a => a.Name.Contains(result[0]) && a.SecondName.Contains(result[1]) && a.LastName.Contains(result[2]));
            }
            else
            {
                // Розділення не вдалося, недостатньо елементів
                throw new ArgumentException("Invalid author format.");
            }
            if (resAuthor != null)
            {
                resAuthor.Name = authorDto.Name ?? resAuthor.Name;
                resAuthor.SecondName = authorDto.SecondName ?? resAuthor.SecondName;
                resAuthor.LastName = authorDto.LastName ?? resAuthor.LastName;
                if (authorDto.DateOfBirth != default)
                {
                    resAuthor.DateOfBirth = authorDto.DateOfBirth;
                }
            }

            //await _booksContext.SaveChangesAsync();
            return   resAuthor;
        }
    }
}
