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

        async Task<ICollection<ReaderDTO>> IBooksService.BorrowingHistory()
        {
            var readers = await _booksContext.BorrowedBooks
         .Include(bb => bb.Reader)  // Включаємо дані читача
         .Include(bb => bb.Book)  // Включаємо дані книги
         .ThenInclude(b => b.Authors)  // Включаємо авторів книги
         .Select(bb => new ReaderDTO
         {
             Login = bb.Reader.Login,  // Логін читача
             BorrowedBooks = new List<BorrowedBookDTO>
             {
                new BorrowedBookDTO
                {
                    Name = bb.Book.Name,  // Назва книги
                    Authors = bb.Book.Authors.Select(a => new AuthorDTO
                    {
                        Name = a.Name,  // Ім'я автора
                        LastName = a.LastName,  // Прізвище автора
                        SecondName = a.SecondName  // По батькові автора
                    }).ToList(),
                    DateBorrowed = bb.DateBorrowed,  // Дата позичання
                    DateForBorrowed = bb.DateForBorrowed,  // Дата, коли книга мала бути повернена
                    DateReturned = bb.DateReturned  // Дата повернення (може бути null)
                }
             }
         }).ToListAsync();
            return readers;
        }
    }
}
