using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookWise
{
    public class ReaderService
    {
        private Reader userReader;
        private BooksContext _booksContext { get; set; }
        // private int _readerID { get; init;}
        private List<Book> _books;

        public ReaderService(Reader UserReader, BooksContext booksContext)
        {
            this.userReader = UserReader;
            this._booksContext = booksContext;
        }
        public static void ShowReaderMenu()
        {
            Console.WriteLine("1. Show borrowed books");
            Console.WriteLine("2. Information about authors");
            Console.WriteLine("3. View Borrowed Book");
            Console.WriteLine("4. Take a book");
        }
        public static void RegReader (BooksContext ctx, Reader reader)
        {
            ctx.Add(reader);
            ctx.SaveChanges();
        }
        public static bool ConsoleRegReader(BooksContext ctx)
        {
            var newReader = new Reader();
            Console.WriteLine("Login");
            newReader.Login = Console.ReadLine()!;
            Console.WriteLine("Password");
            newReader.Password = Console.ReadLine()!;
            Console.WriteLine("Email");
            newReader.Email = Console.ReadLine()!;
            Console.WriteLine("Name");
            newReader.Name = Console.ReadLine()!;
            Console.WriteLine("LastName");
            newReader.LastName = Console.ReadLine()!;
            Console.WriteLine("Document Type(1/2/3)");
            newReader.DocumentTypeId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("DocumentNumber");
            newReader.DocumentNumber = Console.ReadLine()!;
            var existingEmployee = ctx.Employees.FirstOrDefault(u => u.Login == newReader.Login || u.Email == newReader.Email);
            if (existingEmployee != null)
            {
                Console.WriteLine("A user with this login or email already exists.");
                return false;
            }
            else
            {

                RegReader(ctx,newReader);
                Console.WriteLine("Registration was successful!");
                return true;
               // key = "b";
            }
        }
        public IEnumerable<Book> BorrowBook()
        {
            /*var booksNotBorrowed */
             _books = _booksContext.Books
            .Where(book => !_booksContext.BorrowedBooks
                     .Any(borrowed => borrowed.BookId == book.Id))
            .Select(book => book)
            .Include(a => a.Authors)
            .Include(c => c.TypeOfPublishingCode)
            .ToList();
            _booksContext.Entry(userReader)
                .Collection(r => r.BorrowedBooks)
                .Load();
            return _books;
        }
        public void ShowBooks(IEnumerable<Book> books)
        {
            int index = 0;
            foreach (var book in books)
            {
                string authors = book.Authors != null && book.Authors.Any()
            ? string.Join(", ", book.Authors.Select(a => $"{a.Name} {a.LastName}  {a.SecondName}"))
                : "Невідомий автор";
                Console.WriteLine($"{++index} - {book.Name} - {book.Country} - {authors} - {book.City}-{book.Year} - {book.TypeOfPublishingCode.Name}");
            }
        }

        public IEnumerable<Book> SearchBoksByAuthors(string value)
        {
            var nam = value.Split(" ");
            return _books.Where(book =>
                        book.Authors
                            .Any(a => a.Name.Contains(nam[0]) &&
                            (nam.Length > 1 ? a.LastName.Contains(nam[1]) : true)
                            && (nam.Length > 2 ? a.SecondName.Contains(nam[2]) : true)));
        }
        public IEnumerable<Book> SearchBoksByTitle(string value)=>  _books.Where(b => b.Name.Contains(value));
        
        public void ConsoleSearchBoks(string key, string value)
        {
            switch (key)
            {
                case "a":
                    ShowBooks (SearchBoksByAuthors(value));
                    break;
                case "t":
                    ShowBooks(SearchBoksByTitle(value));
                    break;
            }

        }
        public IEnumerable<Author> GetAuthors() => _booksContext.Authors;
        public IEnumerable<Author> GetAuthorsByName(string key) => _booksContext.Authors.Where(a => a.Name.Contains(key));
        public void AboutAuthors()
        {
            foreach (var author in GetAuthors())
            {
                Console.WriteLine($"{author.Name,-10}\t{author.LastName,-12}\t{author.SecondName,-12}\t{author.DateOfBirth}");
            }
            Console.WriteLine("What name?");
            string key = Console.ReadLine()!;
            foreach (var author in GetAuthorsByName(key))
            {
                Console.WriteLine($"{author.Name,-10}\t{author.LastName,-12}\t{author.SecondName, - 12}\t {author.DateOfBirth}");
            }
        }

        public IEnumerable<BorrowedBook> BorrowedBooksByUser() 
        {
            return _booksContext.BorrowedBooks
                .Where(b => b.ReaderId == userReader.Id)
                .Include(b => b.Book)
                .Include(r => r.Reader)
                .Include(a => a.Book.Authors)
                .Where(v => v.Reader != null);
        }
        public IEnumerable<BorrowedBook> ExpiredBooks() => BorrowedBooksByUser().Where(b => b.DateForBorrowed < DateTime.Now && b.DateReturned == null).OrderBy(d => d.DateForBorrowed);
        public IEnumerable<BorrowedBook> WithouExpiredBooks() => BorrowedBooksByUser().Where(b => !ExpiredBooks().Any(ex => ex.Book.Name == b.Book.Name) && b.DateReturned == null)
                 .OrderBy(d => d.DateForBorrowed);
        public void ConsoleBorrowedBooks()
        {
            var borrowedBoks = BorrowedBooksByUser();

            var expiredBooks = ExpiredBooks();

            var withoutExBooks = WithouExpiredBooks();
            Console.WriteLine($"{"Titel".PadRight(30)}{"ConsoleShowAuthors".PadRight(90)}{"DateBorrowed".PadRight(15)}{"DateForReturned".PadRight(17)}{"DateReturned".PadRight(15)}");
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


        public void TakeBook(int index)
        {
            var borBook = new BorrowedBook()
            {
                BookId = _books[index - 1].Id,
                Book = _books[index - 1],
                ReaderId = userReader.Id,
                Reader = userReader,
                DateBorrowed = DateTime.Now,
                DateForBorrowed = DateTime.Now.AddDays(_books[index - 1].DaysBorrowed)
            };
            if (userReader.BorrowedBooks == null)
            {
                userReader.BorrowedBooks = new List<BorrowedBook>();
            }
            userReader.BorrowedBooks.Add(borBook);
            _booksContext.SaveChanges();
        }
        public void ConsoleTakeBook(int index) 
        {
            if (index-1 < 0 || index-1 >= _books.Count)
            {
                Console.WriteLine("Invalid book index.");
                return;
            }
            TakeBook(index);
            Console.WriteLine( $"{_books[index - 1]} book successfully borrowed!");
        }

    }
}
