using DataLibrary;
using System.Xml.Linq;

namespace BookWise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var ctx = new BooksContext();
            while (true) 
            {
                Console.WriteLine("login(l) or Register(r)");
                var key = Console.ReadLine();
                
                while (key == "l")
                {
                    Console.WriteLine("Login");
                        var login = Console.ReadLine();
                    Console.WriteLine("Password");
                    var pass = Console.ReadLine();
                    var user = ctx.Employees
                        .FirstOrDefault(l => l.Login == login);
                    if (user != null && user.Password == pass)
                    {
                        string discriminator = ctx.Entry(user).Property("Discriminator").CurrentValue as string;
                        if (discriminator == "Employee")
                        {
                            Console.WriteLine("Signing in successful! Librarian");
                        }
                        if (discriminator == "Reader")
                        {
                            Console.WriteLine("Signing in successful! Reader");
                            var reader = new ReaderServiceMenu(user as Reader,ctx);
                            bool nenu = true;
                            while (nenu)
                            {
                                ReaderServiceMenu.ShowReaderMenu();
                                //var keyMenu = Console.ReadLine();
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        reader.BorrowBook();
                                        Console.WriteLine("Search by author(a), by title(t)");
                                        reader.Search(Console.ReadLine(),Console.ReadLine());
                                        break;
                                    case "2":
                                        break;
                                    case "3":
                                        break;
                                    case "4":
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
                        var existingEmployee = ctx.Readers.FirstOrDefault(u => u.Login == newReader.Login || u.Email == newReader.Email);
                        if (existingEmployee != null)
                        {
                            Console.WriteLine("A user with this login or email already exists.");
                        }
                        else
                        {
                            ctx.Employees.Add(newReader);
                            ctx.SaveChanges();
                            Console.WriteLine("Registration was successful!");
                            key = "b";
                        }
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
            //    Books = new List<Book>() // можна додати список авторів
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
            //ctx.Books.Add(book);
            //ctx.SaveChanges();
            
        }
    }
}
