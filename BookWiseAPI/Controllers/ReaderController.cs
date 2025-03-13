using DataLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using static BookWiseAPI.Controllers.AccountController;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookWiseAPI.Controllers
{
    public record BookAuthorDto(string Name, string LastName, string SecondName);
    public interface IBooksService
    {
        Task<ICollection<Book>> GetBooks();
    }

    internal class BooksService : IBooksService
    {
        private readonly BooksContext _booksContext;

        public BooksService(BooksContext booksContext)
        {
            _booksContext = booksContext;
        }
        public async Task<ICollection<Book>> GetBooks()
        {
            return await _booksContext.Books
                    .Where(book => _booksContext.BorrowedBooks.All(borrowed => borrowed.BookId != book.Id)) // Спрощений запит
                    .Include(b => b.Authors)
                    .Include(b => b.TypeOfPublishingCode)
                    .ToListAsync();
        }

    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Reader")]
    public class ReaderController : ControllerBase
    {
        private  BooksContext _booksContext;
       // private  ICollection<Book>? _books;
        private IBooksService _booksService;
        public ReaderController(BooksContext booksContext, IBooksService booksService)
        {
            _booksContext = booksContext;
            _booksService = booksService;
        }

        [HttpGet("Books")]
        //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _booksService.GetBooks();
            return Ok(books);
        }
        [HttpGet("BookByAuthors")]
        public async Task<IActionResult> BookByAuthors(BookAuthorDto authorDto)
        {
            var books = await _booksService.GetBooks();
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
            var books = await _booksService.GetBooks();
           // if (_books == null) _books = await _booksService.GetBooks(_booksContext);

            var result = await Task.Run(() => books.Where(b => b.Name.Contains(title)));
            if (result == null|| !result.Any()) { return NotFound("Book not found"); }
            
            //await _books.Where(b => b.Name.Contains(value));

            return Ok(result);
        }

        [HttpGet("Authors")]
        public async Task<IActionResult> GetAuthors()
        {
            var Authors = await _booksContext.Authors.ToListAsync();
            if (Authors == null || !Authors.Any()) { return NotFound("Authors not found"); }
            return Ok(Authors);
        }
        [HttpGet("AuthorsByName")]
        public async Task<IActionResult> GetAuthorsByName(string name) => (await _booksContext.Authors.AnyAsync(a => a.Name.Contains(name)))
        ? Ok(await _booksContext.Authors.Where(a => a.Name.Contains(name)).ToListAsync())
        : NotFound("Author not found");

        [HttpGet("BorrowedBooksUser")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> BorrowedBooksByUser()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var log = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;
            
            var readerId = await _booksContext.Employees
                                   .Where(u => u.Login == log)
                                   .Select(u => u.Id)
                                   .FirstOrDefaultAsync();

            if (readerId == 0) 
            {
                return NotFound("User not found");
            }
            var borrowedBooks = await _booksContext.BorrowedBooks
                .Where( b => b.ReaderId == readerId)
                .Include(b => b.Book)
                .Include(r => r.Reader)
                .Include(a => a.Book.Authors)
                .Where(v => v.Reader != null)
                .ToListAsync();
            if (borrowedBooks == null || !borrowedBooks.Any()) { return NotFound("Book not found"); }
            return Ok(borrowedBooks);
        }
    }
}
