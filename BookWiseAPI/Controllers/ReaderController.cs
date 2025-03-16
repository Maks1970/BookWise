using BookWiseAPI.Model;
using DataLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Security.Claims;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookWiseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Reader")]
    public class ReaderController : ControllerBase
    {
        private  BooksContext _booksContext;
       // private  ICollection<Book>? _books;
        private IBooksService _booksService;
        private  IBorrowedBooksByUserService _borrowedBooksByUser;

        public ReaderController(BooksContext booksContext, IBooksService booksService, IBorrowedBooksByUserService borrowedBooksByUser)
        {
            _booksContext = booksContext;
            _booksService = booksService;
            _borrowedBooksByUser = borrowedBooksByUser;
        }

        [HttpGet("Books")]
        //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _booksService.GetBooks();

            var booksDTO = books.Select(book => new BookDTO
            {
                Id = book.Id,
                Name = book.Name,
                Authors = book.Authors.Select(author => new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name,
                    LastName = author.LastName,
                    SecondName = author.SecondName,
                    DateOfBirth = author.DateOfBirth
                }).ToList()
            }).ToList();
            return Ok(booksDTO);
        }
        [HttpGet("BookByAuthors")]
        public async Task<IActionResult> BookByAuthors(FulAuthorDto authorDto)
        {
            var books = await _booksService.GetBooks();
            var result = await Task.Run(()=> books.Where(book =>
                        book.Authors
                            .Any(a => a.Name.Contains(authorDto.Name) &&
                            a.LastName.Contains(authorDto.LastName) &&
                            a.SecondName.Contains(authorDto.SecondName))));

            var booksDTO = result.Select(book => new BookDTO
            {
                Id = book.Id,
                Name = book.Name,
                Authors = book.Authors.Select(author => new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name,
                    LastName = author.LastName,
                    SecondName = author.SecondName,
                    DateOfBirth = author.DateOfBirth
                }).ToList()
            }).ToList();
            return Ok(booksDTO);
        }
        [HttpGet("BookByTitle")]
        public async Task<IActionResult> BoksByTitle(string title)
        {
            var books = await _booksService.GetBooks();
           // if (_books == null) _books = await _booksService.GetBooks(_booksContext);

            var result = await Task.Run(() => books.Where(b => b.Name.Contains(title)));
            if (result == null|| !result.Any()) { return NotFound("Book not found"); }
            var booksDTO = result.Select(book => new BookDTO
            {
                Id = book.Id,
                Name = book.Name,
                Authors = book.Authors.Select(author => new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name,
                    LastName = author.LastName,
                    SecondName = author.SecondName,
                    DateOfBirth = author.DateOfBirth
                }).ToList()
            }).ToList();
            //await _books.Where(b => b.Name.Contains(value));

            return Ok(booksDTO);
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
        public async Task<IActionResult> BorrowedBooksByUser()
        {
            var borrowedBooks = await _borrowedBooksByUser.GetBorrowedBooksByUserAsync(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (borrowedBooks == null || !borrowedBooks.Any()) { return NotFound("Book not found"); }
            var booksDTO = borrowedBooks.Select(book => new BorrowedBookDTO
            {
                Id = book.Id,
                Name = book.Book.Name,
                Authors = book.Book.Authors.Select(author => new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name,
                    LastName = author.LastName,
                    SecondName = author.SecondName,
                    DateOfBirth = author.DateOfBirth
                }).ToList(),
                DateBorrowed = book.DateBorrowed,
                DateForBorrowed = book.DateForBorrowed,
                DateReturned = book.DateReturned
            }).ToList();
            return Ok(booksDTO);
        }
        [HttpGet("ExpiredBooks")]
        public async Task<IActionResult> ExpiredBooks()
        {
            var borrowedBooks = await _borrowedBooksByUser.GetBorrowedBooksByUserAsync(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            var expiredBooks = borrowedBooks
                .Where(b => b.DateForBorrowed < DateTime.Now && b.DateReturned == null)
                .OrderBy(d => d.DateForBorrowed)
                .ToList(); 

            if (expiredBooks == null || !expiredBooks.Any())
            {
                return NotFound("Book not found");
            }
            var booksDTO = borrowedBooks.Select(book => new BorrowedBookDTO
            {
                Id = book.Id,
                Name = book.Book.Name,
                Authors = book.Book.Authors.Select(author => new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name,
                    LastName = author.LastName,
                    SecondName = author.SecondName,
                    DateOfBirth = author.DateOfBirth
                }).ToList(),
                DateBorrowed = book.DateBorrowed,
                DateForBorrowed = book.DateForBorrowed,
                DateReturned = book.DateReturned
            }).ToList();
            return Ok(booksDTO);

        }
        [HttpGet("WithoutExpiredBooks")]
        public async Task<IActionResult> WithouExpiredBooks()
        {
            var borrowedBooks = await _borrowedBooksByUser.GetBorrowedBooksByUserAsync(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            var expiredBooks = await _booksContext.BorrowedBooks
                .Where(b => b.DateForBorrowed < DateTime.Now && b.DateReturned == null)
                .Select(b => b.Book.Name)  // Беремо тільки назви книг прострочених
                .ToListAsync();

            // Фільтруємо книги, яких немає в списку прострочених
            var filteredBooks = borrowedBooks
                .Where(b => !expiredBooks.Contains(b.Book.Name))  // Перевірка на наявність у списку прострочених
                .ToList();
            if (filteredBooks == null || !filteredBooks.Any())
            {
                return NotFound("No borrowed books found.");
            }
            var booksDTO = borrowedBooks.Select(book => new BorrowedBookDTO
            {
                Id = book.Id,
                Name = book.Book.Name,
                Authors = book.Book.Authors.Select(author => new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name,
                    LastName = author.LastName,
                    SecondName = author.SecondName,
                    DateOfBirth = author.DateOfBirth
                }).ToList(),
                DateBorrowed = book.DateBorrowed,
                DateForBorrowed = book.DateForBorrowed,
                DateReturned = book.DateReturned
            }).ToList();
            return Ok(booksDTO);
        }
        [HttpPost("TakeBook")]
        public async Task<IActionResult> TakeBook(TakeBookDto takeBook)
        {
            var borrowedBooks = await _borrowedBooksByUser.GetBorrowedBooksByUserAsync(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            var canTake = await _booksService.GetBooks();
            var result = await Task.Run(() => canTake.Where(b => b.Name.Contains(takeBook.Title)));
            if (result == null || !result.Any()) { return NotFound("Book not found"); }
            BorrowedBook borrow = new BorrowedBook
            {
                Reader = borrowedBooks.FirstOrDefault()!.Reader,
                Book = result.First(),
                DateBorrowed = DateTime.Now,
                DateForBorrowed = DateTime.Now.AddDays(result.First().DaysBorrowed),
            };
            _booksContext.BorrowedBooks.Add(borrow);
            return await _booksContext.SaveChangesAsync() > 0 ?  Ok(): BadRequest();
            
        }

    }
}
