using DataLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWise
{
    public class ReaderServiceMenu
    {
        Reader userReader { get; set; }
        BooksContext booksContext {  get; set; }

        public ReaderServiceMenu(Reader UserReader, BooksContext booksContext)
        {
            this.userReader = UserReader;
            this.booksContext = booksContext;
        }
        public static void ShowReaderMenu()
        {
            Console.WriteLine("1. Books");
            Console.WriteLine("2. Information about authors");
            Console.WriteLine("3. View Borrowed Books");
            Console.WriteLine("4. Take a book");
        }
        public void BorrowBook()
        {
            var booksNotBorrowed = booksContext.Books
                           .Where(book => !booksContext.BorrowedBooks
                                              .Any(borrowed => borrowed.BookId == book.Id))
                           .Select(book => book)
                           .Include(a=>a.Authors)
                           .Include(c => c.TypeOfPublishingCode)
                           .ToList();
            foreach(var book in booksNotBorrowed)
            {
                string authors = book.Authors != null && book.Authors.Any()
            ? string.Join(", ", book.Authors.Select(a => a.Name))
            : "Невідомий автор";
                Console.WriteLine($"{book.Name} - {book.Country} - {authors} - {book.City}-{book.Year } - {book.TypeOfPublishingCode.Name}");
            }
        }
    }
}
