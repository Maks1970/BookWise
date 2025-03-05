using DataLibrary;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace BookWise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool nenu = true;
           // Console.ResetColor();
            using var ctx = new BooksContext();
            while (true) 
            {
                Console.WriteLine("login(l) or Register(r)");
                var key = Console.ReadLine();
                
                while (key == "l")
                {
                    Console.Write("Login :");
                        var login = Console.ReadLine();
                    Console.Write("Password :");
                    var pass = Console.ReadLine();
                    var user = ctx.Employees
                        .FirstOrDefault(l => EF.Functions.Collate(l.Login, "SQL_Latin1_General_CP1_CS_AS") == login);
                    if (user != null && user.Password == pass)
                    {
                        string discriminator = ctx.Entry(user).Property("Discriminator").CurrentValue as string;
                        if (discriminator == "Employee")
                        {
                            Console.WriteLine("Signing in successful! Librarian");
                           
                            var librarian = new LibrarianService(user,ctx);
                            while (nenu)
                            {
                                LibrarianService.ShowLibrarianMenu();
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        Console.WriteLine("b/a");
                                        switch (Console.ReadLine())
                                        {
                                            case "b":
                                                librarian.Books();
                                                Console.WriteLine("Search a/b?");
                                                librarian.SearchBoks(Console.ReadLine(),Console.ReadLine());
                                                break;
                                            case "a":
                                                librarian.Authors();
                                                break;
                                        }
                                        break;
                                   
                                    case "2":
                                        bool addKey = true;
                                        while (addKey)
                                        {
                                            Console.WriteLine("Books or Autors? (b/a)(or press Enter to keep current)");
                                            switch (Console.ReadLine())
                                            {
                                                case "b":
                                                    librarian.AddBooks();
                                                    break;
                                                case "a":
                                                    librarian.AddAuthor();
                                                    break;
                                                default:
                                                    addKey = false;
                                                    break;
                                            }
                                        }
                                        break;
                                   
                                    case "3":
                                        Console.WriteLine("Books or Autors? (b/a)");
                                        switch (Console.ReadLine())
                                            {
                                                case "b":
                                                    librarian.UpdateBooks();
                                                    break;
                                                case "a":
                                                librarian.UpdateAuthor();
                                                break;
                                                default:
                                                    addKey = false;
                                                    break;
                                            }
                                        break;
                                    case "4":
                                        Console.WriteLine("Add/edit/delete?");
                                        switch (Console.ReadLine())
                                        {
                                            case "Add":
                                                while (!librarian.AddReader())
                                                {
                                                    Console.WriteLine("Try again? (y/n) ");
                                                    if (Console.ReadLine() == "n") break;
                                                }
                                                break;
                                            case "edit":
                                                librarian.EditteReader();
                                                break;
                                            case "delete":
                                                Console.WriteLine("Wat login");
                                                librarian.DeleteReader(Console.ReadLine());
                                                break;
                                        }
                                        break;
                                    case "5":
                                        Console.WriteLine("1.Only debtors and what they should");
                                        Console.WriteLine("2.Everyone who took books and which books (including debtors)");
                                        Console.WriteLine("3.History of borrowing and returning books by specific reader ( number of overdues)");
                                        switch (Console.ReadLine())
                                        {
                                            case "1":
                                                librarian.DebtorsReaders();
                                                break;
                                            case "2":
                                                librarian.ReaderTookBooks();
                                                break;
                                            case "3":
                                                librarian.HistoryOfBorrowing();
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                        if (discriminator == "Reader")
                        {
                            Console.WriteLine("Signing in successful! Reader");
                            var reader = new ReaderService(user as Reader,ctx);
                            
                            while (nenu)
                            {
                                ReaderService.ShowReaderMenu();
                                //var keyMenu = Console.ReadLine();
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        reader.BorrowBook();

                                        Console.WriteLine("Take a book?y/n");
                                        if (Console.ReadLine()=="y")
                                        {
                                            Console.WriteLine("What number?");
                                            reader.TakeBook(Convert.ToInt32(Console.ReadLine()));
                                        }
                                        Console.WriteLine("Take a search book?y/n");
                                        if (Console.ReadLine() == "y")
                                        {
                                            Console.WriteLine("SearchBoks by author(a), by title(t)(key somsth)");
                                            reader.SearchBoks(Console.ReadLine(), Console.ReadLine());
                                        }
                                            
                                        break;
                                    case "2":
                                        reader.AboutAuthors();
                                       // Console.WriteLine("SearchBoks author?(y/n)");
                                        break;
                                    case "3":
                                        reader.BorrowedBooks();
                                        break;
                                    case "4":
                                        reader.BorrowBook();
                                        Console.WriteLine("What number?");
                                        try
                                        {
                                            int bookId = Convert.ToInt32(Console.ReadLine());
                                            reader.TakeBook(bookId);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Invalid book index!");
                                        }
                                        break;
                                    default:
                                        nenu = false;
                                        break;
                                }

                            }
                        }

                        key = Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Invalid login or password");
                    }
                }
                while (key == "r")
                {
                    Console.WriteLine("Librarian(l) or Reader(r) ");
                    key= Console.ReadLine();
                    while (key == "l")
                    {
                        var newEmployee = new Employee();
                        Console.WriteLine("Login");
                        newEmployee.Login = Console.ReadLine()!;
                        Console.WriteLine("Password");
                        newEmployee.Password = Console.ReadLine()!;
                        Console.WriteLine("Email");
                        newEmployee.Email = Console.ReadLine()!;
                        var existingEmployee = ctx.Employees.FirstOrDefault(u => u.Login == newEmployee.Login || u.Email == newEmployee.Email);
                        if (existingEmployee != null)
                        {
                            Console.WriteLine("A user with this login or email already exists.");
                        }
                        else
                        {
                            
                            ctx.Employees.Add(newEmployee);
                            ctx.SaveChanges();
                            Console.WriteLine("Registration was successful!");
                            key = "b";
                        }
                    }
                    while (key == "r")
                    {
                        key = ReaderService.RegReader(ctx)? "b":"r";
                    }
                }
                Console.WriteLine();
            }
            //Console.WriteLine("Hello, World!");
            //newReader employee = new newReader()
            //{
            //    Login = "Max",
            //    Password = "123",
            //    Email = "max@library.com"
            //};
            //Reader reader = new Reader()
            //{
            //    Login = "Alinka",
            //    Password = "3445",
            //    Email = "Alina@library.com",
            //    Name = "Alina",
            //    LastName = "Vasilish",
            //    DocumentTypeId = 2,
            //    DocumentNumber = "AA123456"
            //};
            //Reader reader2 = new Reader()
            //{
            //    Login = "Коля",
            //    Password = "3445",
            //    Email = "кл@library.com",
            //    Name = "Микола",
            //    LastName = "Крус",
            //    DocumentTypeId = 3,
            //    DocumentNumber = "12312"
            //};

            //Author author = new Author()
            //{
            //    Name = " ",
            //    LastName = " ",
            //    SecondName = " ",
            //    DateOfBirth = new DateTime(1888, 3, 4),
            //    Book = new List<Book>() // можна додати список авторів
            //    {
            //        new Book()
            //        { 
            //            Name = "Книгап зі списку",
            //            PublishingCodeId = 3
            //            ,
            //             Year = 2025, // рік публікації
            //             Country = "Україна", // країна
            //             City = "Київ" // місто
            //        }
            //    }

            //};
            //Book book = new Book()
            //{
            //    Name = "Книгап окремий клас",
            //    Authors = new List<Author>() // можна додати список авторів
            //    {
            //        new Author() { Name = "Ya", LastName ="Kos",SecondName=" ", DateOfBirth = new DateTime(1992,1,1) },
            //        new Author() { Name = "Mosy", LastName ="Kvold",SecondName=" ", DateOfBirth = new DateTime(1999,3,3) },
            //        author
            //    },
            //    PublishingCodeId = 2,
            //    //TypeOfPublishingCode = new PublishingCode
            //    //{
            //    //    Name = "ISBN"
            //    //}, // замініть на відповідне значення коду публікації
            //    Year = 2025, // рік публікації
            //    Country = "Україна", // країна
            //    City = "Київ" // місто
            //};

           
           // using var ctx = new BooksContext();
            //вщетукctx.Database.EnsureCreated();

            //ctx.PublishingCodes.AddRange(
            //new PublishingCode { Name = "ISBN" },
            //new PublishingCode { Name = "ББК" },
            //new PublishingCode { Name = "УДК" });

            //ctx.DocumentTypes.AddRange(
            //new DocumentType { Name = "Паспорт" },
            //new DocumentType { Name = "Водійське посвідчення" },
            //new DocumentType { Name = "ID-карта" });
            //ctx.SaveChanges();
            //ctx.Employees.Add(employee);
            //ctx.Employees.Add(reader);
            //ctx.Authors.Add(author);
            //ctx.Book.Add(book);
            //ctx.SaveChanges();
            
        }
    }
}
