using BookWiseAPI.Model;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using BookWiseAPI.Services.Interfaces;
using System.Threading.Tasks.Dataflow;
using System.Linq;

namespace BookWiseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    public class LibrarianController : ControllerBase
    {
        private IBooksService _booksService;
        private readonly IAuthorUpdate _authorUpdate;
        private BooksContext _booksContext;

        public LibrarianController(BooksContext booksContext, IBooksService booksService, IAuthorUpdate authorUpdate)
        {
            _booksService = booksService;
            _authorUpdate = authorUpdate;
            _booksContext = booksContext;

        }
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            return Ok(new
            {
                User = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""),
            IsAuthenticated = User.Identity?.IsAuthenticated,
                Roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value)
            });
        }
        [HttpGet("Books")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _booksService.GetBooks();
            return Ok(books);
        }

        [HttpGet("BookByAuthors")]
        public async Task<IActionResult> BookByAuthors(AuthorDto authorDto)
        {
            var books = await _booksService.GetBooks();
            var result = await Task.Run(() => books.Where(book =>
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
            var result = await Task.Run(() => books.Where(b => b.Name.Contains(title)));
            if (result == null || !result.Any()) { return NotFound("Book not found"); }
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

        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook(BookDto bookDto)
        {
            PublishingCode pubCod = _booksContext.PublishingCodes.FirstOrDefault(a => a.Name == bookDto.TypeOfPublishingCode.Name);
            if (pubCod == null)
            {
                pubCod = new PublishingCode() { Name = bookDto.TypeOfPublishingCode.Name!};
            }
            Book book = new Book()
            {
                Name = bookDto.Title,
                Authors = bookDto.Authors.Select(x => new Author
                {
                    Name = x.Name,
                    SecondName = x.SecondName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth
                }).ToList(),
                TypeOfPublishingCode = pubCod,
                Year = bookDto.Year ?? 0,
                Country = bookDto.Country,
                City = bookDto.City,
                DaysBorrowed = bookDto.DaysBorrowed ?? 0
            };
            _booksContext.Books.Add(book);
            return await _booksContext.SaveChangesAsync() > 0 ? Ok() : BadRequest();
        }

        [HttpPatch("UpdateBook/{findTitle}")]
        public async Task<IActionResult> UpdateBook(string findTitle, BookDto bookDto)
        {
            var existingBook = await _booksContext.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Name == findTitle);
           
            if (existingBook == null)
            {
                return NotFound("Book not found.");
            }
            if (!string.IsNullOrEmpty(bookDto.Title))
            {
                existingBook.Name = bookDto.Title;
            }
           
            if (bookDto.Year!=default)
            {
                existingBook.Year = bookDto.Year?? existingBook.Year;
            }
            if (!string.IsNullOrEmpty(bookDto.Country))
            {
                existingBook.Country = bookDto.Country;
            }
            if (!string.IsNullOrEmpty(bookDto.City))
            {
                existingBook.City = bookDto.City;
            }
            if (bookDto.DaysBorrowed != default)
            {
                existingBook.DaysBorrowed = bookDto.DaysBorrowed ?? existingBook.DaysBorrowed;
            }
            return await _booksContext.SaveChangesAsync() > 0 ? Ok() : BadRequest();
        }

        [HttpPost("AddAuthor")]
        public async Task<IActionResult> AddAuthor( FulAuthorDto authorDto)
        {
            var findAuthor = await _booksContext.Authors.FirstOrDefaultAsync(a=>a.Name==authorDto.Name && a.SecondName==authorDto.SecondName && a.LastName == authorDto.LastName && a.DateOfBirth == authorDto.DateOfBirth);
            if (findAuthor == null)
            {
                Author author = new Author()
                {
                    Name = authorDto.Name,
                    LastName = authorDto.LastName,
                    SecondName = authorDto.SecondName,
                    DateOfBirth = authorDto.DateOfBirth
                };
                _booksContext.Authors.Add(author);
            }
            return await _booksContext.SaveChangesAsync() > 0 ? Ok() : BadRequest();
        }
        [HttpPatch("UpdateAuthor/{findAuthor}")]
        public async Task<IActionResult> UpdateAuthor(string findAuthor, FulAuthorDto bookDto)
        {
            var s = await _authorUpdate.Update(findAuthor,bookDto);
            return await _booksContext.SaveChangesAsync() > 0 ? Ok() : BadRequest();
        }

    }
}
