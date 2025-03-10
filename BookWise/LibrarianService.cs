using DataLibrary;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

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

        public IEnumerable<Book> GetBooks()
        {
            return _booksContext.Books
                .Include(a => a.Authors)
                .Include(c => c.TypeOfPublishingCode);
        }

        public IEnumerable<Author> GetAuthors() => _booksContext.Authors;
        public IEnumerable<Author> GetAuthorsByName(string key) => _booksContext.Authors.Where(a => a.Name.Contains(key));
        public IEnumerable<Book> GetBooksByTitle(string value) =>_books = _booksContext.Books.Where(b => b.Name.Contains(value));
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
        public void AddAuthor(string name, string lastName, string secondName, DateTime dateOfBirth) 
        {
            var d = !_booksContext.Authors.Any(a => a.Name == name && a.LastName == lastName && a.SecondName == secondName);
            if (!_booksContext.Authors.Any(a => a.Name == name && a.LastName == lastName && a.SecondName== secondName))
            {
                var author = new Author()
                {
                    Name = name,
                    LastName = lastName,
                    SecondName = secondName,
                    DateOfBirth = dateOfBirth
                };
                _booksContext.Authors.Add(author);
                _booksContext.SaveChanges();
            }
                
        }
        public Author ConsoleAddAuthor() 
        {
            //DateTime DateB;
            Console.WriteLine("Name:");
            string Name = Console.ReadLine()!;
            Console.WriteLine("LastName:");
            string LastName = Console.ReadLine()!;
            Console.WriteLine("SecondName:");
            string SecondName = Console.ReadLine()!;
            Console.WriteLine("DateOfBirth (yyyy-MM-dd):");
           
                if (DateTime.TryParse(Console.ReadLine(), out DateTime DateB))
                {
                    AddAuthor(Name, LastName, SecondName, DateB);
                var j = _booksContext.Authors.FirstOrDefault(a => a.Name == Name && a.LastName == LastName && a.DateOfBirth == DateB)!;
                return j;
                }
                else
                {
                    Console.WriteLine("Невірний формат дати.");
                }
            
            
            return null;
        }
        public void AddBooks( string title, List<Author> authors, PublishingCode publishingCode, int year, string coutry, string city, int dayBorr) 
        {
            Book book = new Book()
            {
                Name = title,
                Authors = authors,
                TypeOfPublishingCode = publishingCode,
                Year = year,
                Country = coutry,
                City = city,
                DaysBorrowed = dayBorr
            };
            _booksContext.Books.Add(book);
            _booksContext.SaveChanges();
        }
        public void AddPublishingCode(string pubCode)
        {
            var code = new PublishingCode() { Name = pubCode };
            _booksContext.PublishingCodes.Add(code);
        }
        public PublishingCode GetPublishingCode(string pubCode) =>  _booksContext.PublishingCodes.FirstOrDefault(a => a.Name == pubCode);

        public void ConsoleAddBooks()
        {
            bool yet = true;
            Console.WriteLine("Title");
            string title = Console.ReadLine();
            Console.WriteLine("Authors");
            List<Author> ListAuthors = new List<Author>();
            while (yet)
            {
                Author author = ConsoleAddAuthor();
                ListAuthors.Add(author);
                Console.WriteLine("Another author? y/n");
                yet = Console.ReadLine() == "y";
            }
            Console.WriteLine("PublishingCode");
            PublishingCode publishingCode;
            var pubCode = Console.ReadLine();
             publishingCode = GetPublishingCode(pubCode!) != null ? GetPublishingCode(pubCode!) : new PublishingCode() { Name = pubCode! };
            Console.WriteLine("Year");
            int year = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Country");
            string country = Console.ReadLine();
            Console.WriteLine("City");
            string city = Console.ReadLine();
            Console.WriteLine("DaysBorrowed");
            int daysBorr = Convert.ToInt32(Console.ReadLine());
            AddBooks(title, ListAuthors, publishingCode,year,country,city,daysBorr);
        }

        public Author GetAuthor(string firstName, string lastName)
        {
            return _booksContext.Authors
                    .FirstOrDefault(a => a.Name == firstName && a.LastName == lastName);
        }

        public void ConsoleUpdateAuthor()
        {
            Console.WriteLine("Enter author's first name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Enter author's last name:");
            string lastName = Console.ReadLine();

            var author = GetAuthor(firstName, lastName);

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
        public Book GetBook(int index) 
        {
            int idb;
            if (_books != null) { idb = _books.ToList()[index].Id; }
            else
            {
                idb = _booksContext.Books
               .ToList()[index].Id;
            }
           return _booksContext.Books
               .Include(b => b.Authors)
               .Include(b => b.TypeOfPublishingCode)
               .FirstOrDefault(b => b.Id == idb);
        } 
        public void UpdateBooks()
        {
            Book book = new Book();
            Console.WriteLine("Enter Book ID to update:");
            int index = Convert.ToInt32(Console.ReadLine()) - 1;
            book = GetBook(index);
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
                book.TypeOfPublishingCode = GetPublishingCode(pubCode!) != null ? GetPublishingCode(pubCode!) : new PublishingCode() { Name = pubCode! };
            }
            Console.WriteLine("Do you want to update authors? (y/n):");
            if (Console.ReadLine().ToLower() == "y")
            {
                book.Authors.Clear();
                bool addingAuthors = true;
                while (addingAuthors)
                {
                    Author author = ConsoleAddAuthor();
                    book.Authors.Add(author);
                    Console.WriteLine("Add another author? (y/n):");
                    addingAuthors = Console.ReadLine().ToLower() == "y";
                }
            }

            Console.WriteLine(_booksContext.SaveChanges() > 0 ? "Book updated successfully!" : "No changes were made.");
        }
        public Reader GetReader(string log)
        {
            return _booksContext.Readers
                .Include(d => d.DocumenttType)
                .Include(bor => bor.BorrowedBooks)
                    .ThenInclude(b => b.Book)
                .FirstOrDefault(r => r.Login == log);
        }

        public void EditteReader()
        {
            Console.Write("Login readers:");
            var reader = GetReader(Console.ReadLine());

            while (true)
            {
                Console.WriteLine($" What will we change?\r\nName: {reader.Name}\nLastName: {reader.LastName}\nDocumentType: {reader.DocumenttType.Name}\nBorrowedBooks: {string.Join(", ", reader.BorrowedBooks.Where(b => b.DateReturned == null).Select(b=>b.Book.Name)) }");
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
                    case "ConsoleBorrowedBooks":
                        Console.WriteLine("Return/Add");
                        switch (Console.ReadLine())
                        {
                            case "Return":
                                ConsoleReturnBorrowedBook(reader);
                                break;
                            case "Add":
                                ConsoleAddBorrowedBook(reader);
                                break;
                        }    
                        break;
                    default: return;
                }
            }
        }
        public BorrowedBook GetBorrowedBook(Reader reader, string bookName)
        {
            return reader.BorrowedBooks.FirstOrDefault(b => b.Book.Name == bookName);
        }
        public void ReturnBorrowedBook( BorrowedBook borrowedBook, DateTime date)
        {
            borrowedBook.DateReturned = date;
            _booksContext.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ConsoleReturnBorrowedBook(Reader reader)
        {
            Console.WriteLine("What book?");
            string bookName = Console.ReadLine();
            var borrowedBook = GetBorrowedBook(reader, bookName);
            if (borrowedBook != null)
            {
                Console.WriteLine("What date returned? (yyyy-MM-dd)");
                string date = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out DateTime newDob)) ReturnBorrowedBook( borrowedBook,newDob);
                Console.WriteLine($"The book '{bookName}' has been up.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Book not found in borrowed books.");
            }
        }
        public Book GetBook(string bookName) => _booksContext.Books
                .Include(d => d.TypeOfPublishingCode)
                .Include(a => a.Authors)
                .FirstOrDefault(b => b.Name == bookName);
        public bool IsBorrowed(Book borrowBook) => _booksContext.BorrowedBooks.Any(b => b.BookId == borrowBook.Id && b.DateReturned == null);
        public void AddBorrowedBook(Reader reader, Book book)
        {
            var borrowedBook = new BorrowedBook
            {
                Reader = reader,
                Book = book,
                DateBorrowed = DateTime.Now,
            };
            reader.BorrowedBooks.Add(borrowedBook);
            _booksContext.SaveChanges();
        }
        public void ConsoleAddBorrowedBook(Reader reader)
        {
            Console.WriteLine("What book?");

            string bookName = Console.ReadLine();
            var borrowBook = GetBook(bookName);
            if (borrowBook == null)
            {
                Console.WriteLine("Book not found in books.");
                return;
            }
            if (IsBorrowed(borrowBook))
            {
                Console.WriteLine("This book is already borrowed by another reader.");
                return;
            }
            AddBorrowedBook(reader,borrowBook);
            Console.WriteLine($"Book '{bookName}' has been borrowed.");
        }
        public void RemoveReader(Reader reader) 
        {
            _booksContext.Readers.Remove(reader);
            _booksContext.SaveChanges();
        }
        public void ConsoleDeleteReader(string login)
        {
            var reader = GetReader(login);
            if (reader != null)
            {
                RemoveReader(reader);
                Console.WriteLine($"Reader with login '{login}' has been deleted.");
            }
            else
            {
                Console.WriteLine($"Reader with login '{login}' not found.");
            }
        }
        //To EDIT
        public bool AddReader() => ReaderService.ConsoleRegReader(_booksContext);

        public void ConsoleDebtorsReaders()
        {
            var borrowedReaders = DebtorsReaders();
            Console.WriteLine($"{"Login",14} {"Name",14} {"LastName",14} {"DocumentNumber",14}  Email");
            foreach (var bor in borrowedReaders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{bor.Login,14} {bor.Name,14} {bor.LastName,14} {bor.DocumentNumber,14}  {bor.Email}");
                Console.ResetColor();
                Console.WriteLine($"Debtors: {string.Join(", ", bor.BorrowedBooks.Select(b => b.Book.Name))}");
            }
        }

        public IEnumerable<Reader> DebtorsReaders()
        {
            return _booksContext.Readers
            .Include(r => r.BorrowedBooks)
            .ThenInclude(b => b.Book)
            .Where(r => r.BorrowedBooks.Any(b => b.DateReturned == null));
        }

        //Everyone who took books and which books (including debtors)
        public IEnumerable<Reader> ReadersTookBooks()
        {
            return _booksContext.Readers
           .Include(r => r.BorrowedBooks)
           .ThenInclude(b => b.Book)
           .Where(r => r.BorrowedBooks.Any());
        }
        public void ConsoleReaderTookBooks()
        {
            var borrowedReaders = ReadersTookBooks();
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
        public Reader HistoryOfBorrowingReader(string login)
        {
            return _booksContext.Readers
                .Include(d => d.DocumenttType)
                .Include(bor => bor.BorrowedBooks
                .OrderBy(b => b.DateForBorrowed))
                    .ThenInclude(b => b.Book)
                    .ThenInclude(a => a.Authors)
                .FirstOrDefault(r => r.Login == login);
        }
        public int CountOverdue(Reader reader) 
        {
            return reader.BorrowedBooks.Count(b => b.DateForBorrowed < DateTime.Now && b.DateReturned == null);
        }

        public void ConsoleHistoryOfBorrowing()
        {
            Console.Write("Login readers:");
            var reader = HistoryOfBorrowingReader(Console.ReadLine());
            var countOverdue = CountOverdue(reader);
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
