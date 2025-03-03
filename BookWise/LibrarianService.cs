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
    internal class LibrarianService
    {
        private Employee userLibrarian { get; set; }
        private BooksContext _booksContext { get; set; }
        // private int _readerID { get; init;}
        //private List<Book> _books;

        public LibrarianService(Employee UserReader, BooksContext booksContext)
        {
            this.userLibrarian = UserReader;
            this._booksContext = booksContext;
        }

        private void ShowBooks(IQueryable<Book> books) 
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
        public void Authors()
        {
            foreach (var author in _booksContext.Authors)
            {
                Console.WriteLine($"{author.Name}\t{author.LastName,12}\t{author.SecondName,12}\t{author.DateOfBirth.Date.ToString("dd.MM.yyyy")}");
            }

        }
        public void Books()
        {
            var _books = _booksContext.Books
            .Where(book => !_booksContext.BorrowedBooks
                     .Any(borrowed => borrowed.BookId == book.Id))
            .Select(book => book)
            .Include(a => a.Authors)
            .Include(c => c.TypeOfPublishingCode);
            ShowBooks(_books);
        }
        private Author CreateAuthor() 
        {
            bool check = false;
            var author = new Author();
            Console.WriteLine("Name:");
            author.Name = Console.ReadLine()!;
            Console.WriteLine("LastName:");
            author.LastName = Console.ReadLine()!;
            Console.WriteLine("SecondName:");
            author.SecondName = Console.ReadLine()!;
            Console.WriteLine("DateOfBirth (yyyy-MM-dd):");
            //author.DateOfBirth = 
            while (check!) 
            { 
            
            }
            if (DateTime.TryParse(Console.ReadLine(), out DateTime c))
            {
                author.DateOfBirth = c;
                check = true;
            }
            else
            {
                Console.WriteLine("Невірний формат дати.");
            }
                return author;
        }

        public void AddBooks()
        {
            var book = new Book();
            bool yet = true;
            Console.WriteLine("Title");
            book.Name = Console.ReadLine();
            Console.WriteLine("Authors");
            List<Author> ListAuthors = new List<Author>();
            while (yet) 
            {
                Author author = CreateAuthor();
                var existingAuthor = _booksContext.Authors
                    .FirstOrDefault(a => a.Name == author.Name && a.LastName == author.LastName);

                if (existingAuthor != null)
                {
                    ListAuthors.Add(existingAuthor);
                }
                else
                {
                    _booksContext.Authors.Add(author);
                    //_booksContext.SaveChanges();
                    ListAuthors.Add(author);
                }

                Console.WriteLine("Another author? y/n");
                yet = Console.ReadLine() == "y";
            }
            book.Authors = ListAuthors;
            Console.WriteLine("PublishingCode");
            var pubCode = Console.ReadLine();
            var existingCode = _booksContext.PublishingCodes
                    .FirstOrDefault(a => a.Name == pubCode);

            if (existingCode != null)
            {
                book.TypeOfPublishingCode = existingCode;
                //var code = new PublishingCode() { Name = pubCode};
            }
            else
            {
                var code = new PublishingCode() { Name = pubCode! };
                book.TypeOfPublishingCode = code;
                _booksContext.PublishingCodes.Add(code);
                //_booksContext.Authors.Add(author);
                // _booksContext.SaveChanges();
            }
            // book.TypeOfPublishingCode.Name = Console.ReadLine();
            Console.WriteLine("Year");
            book.Year = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Country");
            book.Country = Console.ReadLine();
            Console.WriteLine("City");
            book.City = Console.ReadLine();
            Console.WriteLine("DaysBorrowed");
            book.DaysBorrowed = Convert.ToInt32(Console.ReadLine());
            _booksContext.Books.Add(book);
            _booksContext.SaveChanges();
        }

        public static void ShowReaderMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. Browse all books/authors");
            Console.WriteLine("2. Add books and authors");
            Console.WriteLine("3. Update books and authors");
            Console.WriteLine("4. Add/edit/delete readers");
            Console.WriteLine("5. View history and current info of all readers");
        }


    }
}
