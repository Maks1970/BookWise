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
        private List<Book> _books;

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
            /*var booksNotBorrowed */
            _books = booksContext.Books
            .Where(book => !booksContext.BorrowedBooks
                     .Any(borrowed => borrowed.BookId == book.Id))
            .Select(book => book)
            .Include(a => a.Authors)
            .Include(c => c.TypeOfPublishingCode)
            .ToList();
            showBoks(_books);
        }

        private void showBoks(List<Book> books)
        {
            foreach (var book in books)
            {
                string authors = book.Authors != null && book.Authors.Any()
            ? string.Join(", ", book.Authors.Select(a => $"{a.Name} {a.LastName}  {a.SecondName}"))
                : "Невідомий автор";
                Console.WriteLine($"{book.Name} - {book.Country} - {authors} - {book.City}-{book.Year} - {book.TypeOfPublishingCode.Name}");
            }
        }

        public void Search(string key, string value)
        {
            var nam = value.Split(" ");
            switch (key)
            {
                case "a":
                    var booksAutors = _books
                        .Where(book =>
                        book.Authors
                            .Any(a => a.Name.Contains(nam[0]) && 
                            (nam.Length > 1 ? a.LastName.Contains(nam[1]) : true)
                            && (nam.Length > 2 ? a.SecondName.Contains(nam[2]):true)))
                        .ToList();
                    showBoks (booksAutors);
                    break;
                case "t":
                    var booksTitle = _books
                        .Where(b=>b.Name.Contains(value))
                        .ToList();
                    showBoks(booksTitle);
                    break;
            }
        }
    }
}
