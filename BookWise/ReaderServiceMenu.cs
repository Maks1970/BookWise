using DataLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookWise
{
    public class ReaderServiceMenu
    {
        private Reader userReader { get; set; }
        private BooksContext booksContext {  get; set; }
        private int readerID { get; init;}
        private List<Book> _books;

        public ReaderServiceMenu(Reader UserReader, BooksContext booksContext, int readerID)
        {
            this.userReader = UserReader;
            this.booksContext = booksContext;
            this.readerID = readerID;
        }
        public static void ShowReaderMenu()
        {
            Console.WriteLine("1. Book");
            Console.WriteLine("2. Information about authors");
            Console.WriteLine("3. View Borrowed Book");
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

        public void SearchBoks(string key, string value)
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
        public void AboutAuthors()
        {
            foreach (var author in booksContext.Authors)
            {
                Console.WriteLine($"{author.Name,-10}\t{author.LastName,-12}\t{author.SecondName,-12}\t{author.DateOfBirth}");
            }
            Console.WriteLine("What name?");
            string key = Console.ReadLine()!;
            foreach (var author in booksContext.Authors.Where(a=>a.Name.Contains(key)))
            {
                Console.WriteLine($"{author.Name,-10}\t{author.LastName,-12}\t{author.SecondName, - 12}\t {author.DateOfBirth}");
            }
        }
        public void BorrowedBooks()
        {
            var borrowedBoks = booksContext.BorrowedBooks
                .Where(b => b.ReaderId == readerID)
                .Include(b => b.Book)
                .Include(r => r.Reader)
                .Include(a => a.Book.Authors)
                .Where(v => v.Reader != null)
                .ToList();
            var expiredBooks = borrowedBoks.Where(b => b.DateForBorrowed < DateTime.Now &&  b.DateReturned == null).OrderBy(d => d.DateForBorrowed);

            var withoutExBooks = borrowedBoks
                .Where(b => !expiredBooks.Any(ex => ex.Book.Name == b.Book.Name) && b.DateReturned == null)
                 .OrderBy(d => d.DateForBorrowed);
            Console.WriteLine($"{"Titel".PadRight(30)}{"Authors".PadRight(90)}{"DateBorrowed".PadRight(15)}{"DateForReturned".PadRight(17)}{"DateReturned".PadRight(15)}");
            Console.WriteLine(new string('-', 180));
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var exBook in expiredBooks)
            {
                string authors = exBook.Book.Authors != null && exBook.Book.Authors.Any()
                ? string.Join(", ", exBook.Book.Authors.Select(a => $"{a.Name} {a.LastName}  {a.SecondName}"))
                : "Невідомий автор";
                Console.WriteLine($"{exBook.Book.Name,-30}\t{authors,-80}\t{exBook.DateBorrowed.ToShortDateString()}\t{exBook.DateForBorrowed.ToShortDateString()}\t{(exBook.DateReturned.HasValue ? exBook.DateReturned.Value.ToShortDateString() : " ")}");

            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var book in withoutExBooks)
            {
                string authors = book.Book.Authors != null && book.Book.Authors.Any()
                ? string.Join(", ", book.Book.Authors.Select(a => $"{a.Name} {a.LastName}  {a.SecondName}"))
                : "Невідомий автор";
                Console.WriteLine($"{book.Book.Name,-30}\t{authors,-80}\t{book.DateBorrowed.ToShortDateString()}\t{book.DateForBorrowed.ToShortDateString()}\t{(book.DateReturned.HasValue? book.DateReturned.Value.ToShortDateString() : " " )}");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var book in borrowedBoks.Where(b => b.DateReturned != null))
            {
                string authors = book.Book.Authors != null && book.Book.Authors.Any()
                ? string.Join(", ", book.Book.Authors.Select(a => $"{a.Name} {a.LastName}  {a.SecondName}"))
                : "Невідомий автор";
                Console.WriteLine($"{book.Book.Name,-30}\t{authors,-80}\t{book.DateBorrowed.ToShortDateString()}\t{book.DateForBorrowed.ToShortDateString()}\t{(book.DateReturned.HasValue ? book.DateReturned.Value.ToShortDateString() : " ")}");
            }
            Console.ResetColor();
        }
    }
}
