﻿using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookWise
{
    internal class LibrarianService
    {
        private Employee userLibrarian;
        private BooksContext _booksContext { get; set; }

        private IEnumerable<Book> _books;

        public LibrarianService(Employee UserReader, BooksContext booksContext)
        {
            this.userLibrarian = UserReader;
            this._booksContext = booksContext;
        }
        
        public IEnumerable<Book> GetBooks() => 
            _booksContext.Books
                .Include(a => a.Authors)
                .Include(c => c.TypeOfPublishingCode);
        public IEnumerable<Author> GetAuthors() => _booksContext.Authors;
        public IEnumerable<Author> GetAuthorsByName(string key) => _booksContext.Authors.Where(a => a.Name.Contains(key));
        public IEnumerable<Book> GetBooksByTitle(string value) =>
            _books = _booksContext.Books
                .Where(b => b.Name.Contains(value));
        public IEnumerable<Book> GetBooksByAuthor(string value)
        {
            var nam = value.Split(" ");
            return _books = _booksContext.Books
                 .Where(book => book.Authors
                            .Any(a => a.Name.Contains(nam[0]) &&
                            (nam.Length > 1 ? a.LastName.Contains(nam[1]) : true)
                            && (nam.Length > 2 ? a.SecondName.Contains(nam[2]) : true)));
        }

        private void ConsoleBooks(IEnumerable<Book> books)
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
        public void ConsoleShowAuthors()
        {
            foreach (var author in GetAuthors())
            {
                Console.WriteLine($"{author.Name}\t{author.LastName,12}\t{author.SecondName,12}\t{author.DateOfBirth.Date.ToString("dd.MM.yyyy")}");
            }
            Console.WriteLine("What name?");
            string key = Console.ReadLine()!;
            foreach (var author in GetAuthorsByName(key))
            {
                Console.WriteLine($"{author.Name,-10}\t{author.LastName,-12}\t{author.SecondName,-12}\t {author.DateOfBirth}");
            }

        }
        public void ConsoleShowBooks() => ConsoleBooks(GetBooks());
        public void SearchBoks(string key, string value)
        {
            switch (key)
            {
                case "a":
                    ConsoleBooks(GetBooksByAuthor(value));
                    break;
                case "b":
                    ConsoleBooks(GetBooksByTitle(value));
                    break;
            }
        }
        public void CreateAuthor(string Name, string LastName, string SecondName, DateTime DateOfBirth) 
        {
            var author = new Author()
            {
                Name = Name,
                LastName = LastName,
                SecondName = SecondName,
                DateOfBirth = DateOfBirth
            };
            _booksContext.Authors.Add(author);
            _booksContext.SaveChanges();
        }
        public void ConsoleCreateAuthor() 
        {
            DateTime DateB;
            Console.WriteLine("Name:");
            string Name = Console.ReadLine()!;
            Console.WriteLine("LastName:");
            string LastName = Console.ReadLine()!;
            Console.WriteLine("SecondName:");
            string SecondName = Console.ReadLine()!;
            Console.WriteLine("DateOfBirth (yyyy-MM-dd):");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime c))
            {
                DateB = c;
                CreateAuthor(Name, LastName, SecondName, DateB);
            }
            else
            {
                Console.WriteLine("Невірний формат дати.");
            }
        }
        public void AddBooks()
        {
            var book = new Book();
            bool yet = true;
            Console.WriteLine("Title");
            book.Name = Console.ReadLine();
            Console.WriteLine("ConsoleShowAuthors");
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
            }
            else
            {
                var code = new PublishingCode() { Name = pubCode! };
                book.TypeOfPublishingCode = code;
                _booksContext.PublishingCodes.Add(code);
            }
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
            Console.WriteLine("Added");
        }
        public void UpdateAuthor()
        {
            Console.WriteLine("Enter author's first name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Enter author's last name:");
            string lastName = Console.ReadLine();

            var author = _booksContext.Authors
                .FirstOrDefault(a => a.Name == firstName && a.LastName == lastName);

            if (author == null)
            {
                Console.WriteLine("Author not found.");
                return;
            }

            Console.WriteLine($"Updating author: {author.Name} {author.LastName}");

            Console.WriteLine("Enter new first name (or press Enter to keep current):");
            string newFirstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newFirstName))
                author.Name = newFirstName;

            Console.WriteLine("Enter new last name (or press Enter to keep current):");
            string newLastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newLastName))
                author.LastName = newLastName;

            Console.WriteLine("Enter new second name (or press Enter to keep current):");
            string newSecondName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newSecondName))
                author.SecondName = newSecondName;

            Console.WriteLine("Enter new date of birth (yyyy-MM-dd) (or press Enter to keep current):");
            string dobInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dobInput) && DateTime.TryParse(dobInput, out DateTime newDob))
                author.DateOfBirth = newDob;

            _booksContext.SaveChanges();
            Console.WriteLine("Author updated successfully!");
        }
        public void UpdateBooks()
        {
            Book book = new Book();
            Console.WriteLine("Enter Book ID to update:");
            int index = Convert.ToInt32(Console.ReadLine()) - 1;
            int idb;
            if (_books != null) { idb = _books.ToList()[index].Id; }
            else {
                idb = _booksContext.Books
               .ToList()[index].Id;
            }
            book = _booksContext.Books
           .Include(b => b.Authors)
           .Include(b => b.TypeOfPublishingCode)
           .FirstOrDefault(b => b.Id == idb);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            Console.WriteLine($"Updating book: {book.Name}");

            Console.WriteLine("Enter new Title (or press Enter to skip):");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title))
                book.Name = title;

            Console.WriteLine("Enter new Year (or press Enter to skip):");
            string yearInput = Console.ReadLine();
            if (int.TryParse(yearInput, out int year))
                book.Year = year;

            Console.WriteLine("Enter new Country (or press Enter to skip):");
            string country = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(country))
                book.Country = country;

            Console.WriteLine("Enter new City (or press Enter to skip):");
            string city = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(city))
                book.City = city;

            Console.WriteLine("Enter new DaysBorrowed (or press Enter to skip):");
            string daysInput = Console.ReadLine();
            if (int.TryParse(daysInput, out int days))
                book.DaysBorrowed = days;

            Console.WriteLine("Enter new Publishing Code (or press Enter to skip):");
            string pubCode = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(pubCode))
            {
                var existingCode = _booksContext.PublishingCodes.FirstOrDefault(c => c.Name == pubCode);
                if (existingCode != null)
                {
                    book.TypeOfPublishingCode = existingCode;
                }
                else
                {
                    var newCode = new PublishingCode { Name = pubCode };
                    _booksContext.PublishingCodes.Add(newCode);
                    book.TypeOfPublishingCode = newCode;
                }
            }
            Console.WriteLine("Do you want to update authors? (y/n):");
            if (Console.ReadLine().ToLower() == "y")
            {
                book.Authors.Clear();
                bool addingAuthors = true;
                while (addingAuthors)
                {
                    Author author = CreateAuthor();
                    var existingAuthor = _booksContext.Authors
                        .FirstOrDefault(a => a.Name == author.Name && a.LastName == author.LastName);

                    if (existingAuthor != null)
                    {
                        book.Authors.Add(existingAuthor);
                    }
                    else
                    {
                        _booksContext.Authors.Add(author);
                        book.Authors.Add(author);
                    }

                    Console.WriteLine("Add another author? (y/n):");
                    addingAuthors = Console.ReadLine().ToLower() == "y";
                }
            }

            _booksContext.SaveChanges();
            Console.WriteLine("Book updated successfully!");
        }

        public void EditteReader()
        {
            Console.Write("Login readers:");
            var reader = _booksContext.Readers
                .Include(d=>d.DocumenttType)
                .Include(bor=>bor.BorrowedBooks)
                    .ThenInclude(b=>b.Book)
                .FirstOrDefault(r => r.Login == Console.ReadLine());
                //_booksContext.Entry(reader)
                //.Collection(r => r.BorrowedBooks)
                //.Load();
            while (true)
            {
                Console.WriteLine($" What will we change?\r\nName: {reader.Name}\nLastName: {reader.LastName}\nDocumentType: {reader.DocumenttType.Name}\nBorrowedBooks: {string.Join(", ", reader.BorrowedBooks.Select(b=>b.Book.Name)) }");
                switch (Console.ReadLine())
                {
                    case "Name":
                        reader.Name = Console.ReadLine()!;
                        break;
                    case "LastName":
                        reader.LastName = Console.ReadLine()!;
                        break;
                    case "DocumentType":
                        reader.DocumenttType.Name = Console.ReadLine()!;
                        break;
                    case "BorrowedBooks":
                        Console.WriteLine("Delete/Add");
                        switch (Console.ReadLine())
                        {
                            case "Return":
                                ReturnBorrowedBook(ref reader);
                                break;
                            case "Add":
                                AddBorrowedBook(ref reader);
                                break;
                        }    
                        break;
                    default: return;
                }
            }
        }
        public void ReturnBorrowedBook(ref Reader reader)
        {
            Console.WriteLine("What book?");

            string bookName = Console.ReadLine();
            var borrowedBook = reader.BorrowedBooks.FirstOrDefault(b => b.Book.Name == bookName);

            if (borrowedBook != null)
            {
                Console.WriteLine("What date returned? (yyyy-MM-dd)");
                string date = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out DateTime newDob))
                    borrowedBook.DateReturned= newDob;
                _booksContext.SaveChanges();
                Console.WriteLine($"The book '{bookName}' has been up.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Book not found in borrowed books.");
            }
        }
        public void AddBorrowedBook(ref Reader reader)
        {
            Console.WriteLine("What book?");

            string bookName = Console.ReadLine();
            var borrowBook = _booksContext.Books
                .Include(d=>d.TypeOfPublishingCode)
                .Include(a=>a.Authors)
                .FirstOrDefault(b => b.Name == bookName);
            if (borrowBook == null)
            {
                Console.WriteLine("Book not found in books.");
                return;
            }
            if (_booksContext.BorrowedBooks.Any(b => b.BookId == borrowBook.Id && b.DateReturned == null))
            {
                Console.WriteLine("This book is already borrowed by another reader.");
                return;
            }
            var borrowedBook = new BorrowedBook
            {
                Reader = reader,
                Book = borrowBook,
                DateBorrowed = DateTime.Now,
            };
            reader.BorrowedBooks.Add(borrowedBook);
            _booksContext.SaveChanges(); // Збереження змін у базі
            Console.WriteLine($"Book '{bookName}' has been borrowed.");
        }
        public void DeleteReader(string login) 
        {
            var reader = _booksContext.Readers.FirstOrDefault(u=>u.Login==login);
            if (reader != null)
            {
                _booksContext.Readers.Remove(reader);
                _booksContext.SaveChanges();
                Console.WriteLine($"Reader with login '{login}' has been deleted.");
            }
            else
            {
                Console.WriteLine($"Reader with login '{login}' not found.");
            }
        }
        public bool AddReader() => ReaderService.RegReader(_booksContext);

        public void DebtorsReaders()
        {
            var borrowedReaders = _booksContext.Readers
            .Include(r => r.BorrowedBooks)
            .ThenInclude(b=>b.Book)
            .Where(r => r.BorrowedBooks.Any(b => b.DateReturned == null));
            Console.WriteLine($"{"Login",14} {"Name",14} {"LastName",14} {"DocumentNumber",14}  Email");
            foreach (var bor in borrowedReaders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{bor.Login,14} {bor.Name,14} {bor.LastName,14} {bor.DocumentNumber,14}  {bor.Email}");
                Console.ResetColor();
                Console.WriteLine($"Debtors: {string.Join(", ", bor.BorrowedBooks.Select(b => b.Book.Name))}");
            }
        }

        //Everyone who took books and which books (including debtors)
        public void ReaderTookBooks()
        {
            var borrowedReaders = _booksContext.Readers
           .Include(r => r.BorrowedBooks)
           .ThenInclude(b => b.Book)
           .Where(r => r.BorrowedBooks.Any());
            Console.WriteLine($"{"Login",14} {"Name",14} {"LastName",14} {"DocumentNumber",14}  Email");
            foreach (var bor in borrowedReaders)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{bor.Login,14} {bor.Name,14} {bor.LastName,14} {bor.DocumentNumber,14}  {bor.Email}");
                Console.ResetColor();
                
                Console.WriteLine($"ConsoleShowBooks:{string.Join(", ",bor.BorrowedBooks.Select(b => b.Book.Name))}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Borrowed books:{string.Join(", ", bor.BorrowedBooks.Where(b => b.DateForBorrowed > DateTime.Now && b.DateReturned == null).Select(b => b.Book.Name))}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Overdue : {string.Join(", ", bor.BorrowedBooks.Where(b => b.DateForBorrowed < DateTime.Now && b.DateReturned == null).Select(b => b.Book.Name))}");
            }
            Console.ResetColor();
        }
        public void HistoryOfBorrowing()
        {
            Console.Write("Login readers:");
            var reader = _booksContext.Readers
                .Include(d => d.DocumenttType)
                .Include(bor => bor.BorrowedBooks
                .OrderBy(b => b.DateForBorrowed))
                    .ThenInclude(b => b.Book)
                    .ThenInclude(a=>a.Authors)
                .FirstOrDefault(r => r.Login == Console.ReadLine());
            var countOverdue = reader.BorrowedBooks.Count(b =>  b.DateForBorrowed < DateTime.Now && b.DateReturned == null);
            Console.WriteLine($"Name: {reader.Name}\nLastName: {reader.LastName}\nDocumentType: {reader.DocumenttType.Name}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Overdue : {countOverdue}\n");
            Console.ResetColor();
            foreach (var book in reader.BorrowedBooks)
            {
                Console.WriteLine($"Book: {book.Book.Name}");
                Console.WriteLine($"Author: {string.Join(" ", book.Book.Authors.Select(a => $"{a.Name}{a.LastName} {a.SecondName}  "))} ");
                Console.WriteLine($"{"Date borrowed",-25} {"Date for borrowed",-25} {"Date returned",-25} ");
                Console.WriteLine($"{book.DateBorrowed,-25} {book.DateForBorrowed,-25} {book.DateReturned}");
                Console.WriteLine();
            }
        }
        public static void ShowLibrarianMenu()
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
