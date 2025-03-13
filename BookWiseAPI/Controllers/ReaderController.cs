using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static BookWiseAPI.Controllers.AccountController;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookWiseAPI.Controllers
{
    public record BookAuthorDto(string Name, string LastName, string SecondName);
    public interface IBooksService
    {
        Task<ICollection<Book>> GetBooksDbAsync(BooksContext booksContext);
    }

    internal class Books : IBooksService
    {
        public async Task<ICollection<Book>> GetBooksDbAsync(BooksContext booksContext)
        {
            var books = await booksContext.Books
                .Where(book => !booksContext.BorrowedBooks
                    .Where(borrowed => borrowed.BookId == book.Id)
                    .Any())
                .Include(b => b.Authors)
                .Include(b => b.TypeOfPublishingCode)
                .ToListAsync();
            return books;
        }

    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Reader")]
    public class ReaderController : ControllerBase
    {
        private readonly BooksContext _booksContext;
       // private  ICollection<Book>? _books;
        private IBooksService _booksService;
        public ReaderController(BooksContext booksContext, IBooksService booksService)
        {
            _booksContext = booksContext;
            _booksService = booksService;
        }

        [HttpGet("AllBooks")]
        //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _booksService.GetBooksDbAsync(_booksContext);
            return Ok(books);
        }
        [HttpGet("BookByAuthors")]
        public async Task<IActionResult> BookByAuthors(BookAuthorDto authorDto)
        {
            var books = await _booksService.GetBooksDbAsync(_booksContext);
            var result = await Task.Run(()=> books.Where(book =>
                        book.Authors
                            .Any(a => a.Name.Contains(authorDto.Name) &&
                            a.LastName.Contains(authorDto.LastName) &&
                            a.SecondName.Contains(authorDto.SecondName))));

            return Ok(result);
        }
        [HttpGet("BookByTitle")]
        public async Task<IActionResult> BoksByTitle(string title)
        {
            var books = await _booksService.GetBooksDbAsync(_booksContext);
           // if (_books == null) _books = await _booksService.GetBooksDbAsync(_booksContext);

            var result = await Task.Run(() => books.Where(b => b.Name.Contains(title)));
            if (result == null || !result.Any()) { return NotFound("Book not found"); }
            
            //await _books.Where(b => b.Name.Contains(value));

            return Ok(result);
        }
    }
}
