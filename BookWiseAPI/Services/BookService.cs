using BookWiseAPI.Model;
using DataLibrary;
using Microsoft.EntityFrameworkCore;

namespace BookWiseAPI.Controllers
{
    internal class BookService : IBooksService
    {
        private readonly BooksContext _booksContext;

        public BookService(BooksContext booksContext)
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

        async Task<ICollection<ReaderDto>> IBooksService.BorrowingHistory()
        {
            var readers = await _booksContext.BorrowedBooks
         .Include(bb => bb.Reader) 
         .Include(bb => bb.Book)
         .ThenInclude(b => b.Authors) 
         .Select(bb => new ReaderDto
         {
             Login = bb.Reader.Login,
             BorrowedBooks = new List<BorrowedBookDto>
             {
                new BorrowedBookDto
                {
                    Name = bb.Book.Name, 
                    Authors = bb.Book.Authors.Select(a => new AuthorDto
                    {
                        Name = a.Name,  
                        LastName = a.LastName,  
                        SecondName = a.SecondName 
                    }).ToList(),
                    DateBorrowed = bb.DateBorrowed, 
                    DateForBorrowed = bb.DateForBorrowed,  
                    DateReturned = bb.DateReturned 
                }
             }
         }).ToListAsync();
            return readers;
        }
    }
}
