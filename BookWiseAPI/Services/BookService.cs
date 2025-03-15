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

    }
}
