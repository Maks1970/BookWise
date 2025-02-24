using DataLibrary;
using System.Xml.Linq;

namespace BookWise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Employee employee = new Employee()
            {
                Login = "Max",
                Password = "123",
                Email = "max@library.com"
            };
            Reader reader = new Reader()
            {
                Login = "Alinka",
                Password = "3445",
                Email = "Alina@library.com",
                Name = "Alina",
                LastName = "Vasilish",
                DocumentTypeId = 2,
                DocumentNumber = "AA123456"
            };
            Reader reader2 = new Reader()
            {
                Login = "Коля",
                Password = "3445",
                Email = "кл@library.com",
                Name = "Микола",
                LastName = "Крус",
                DocumentTypeId = 3,
                DocumentNumber = "12312"
            };

            Author author = new Author()
            {
                Name = " ",
                LastName = " ",
                SecondName = " ",
                DateOfBirth = new DateTime(1888, 3, 4),
                Books = new List<Book>() // можна додати список авторів
                {
                    new Book()
                    { 
                        Name = "Книгап зі списку",
                        PublishingCodeId = 3
                        ,
                         Year = 2025, // рік публікації
                         Country = "Україна", // країна
                         City = "Київ" // місто
                    }
                }

            };
            Book book = new Book()
            {
                Name = "Книгап окремий клас",
                Authors = new List<Author>() // можна додати список авторів
                {
                    new Author() { Name = "Ya", LastName ="Kos",SecondName=" ", DateOfBirth = new DateTime(1992,1,1) },
                    new Author() { Name = "Mosy", LastName ="Kvold",SecondName=" ", DateOfBirth = new DateTime(1999,3,3) },
                    author
                },
                PublishingCodeId = 2,
                //TypeOfPublishingCode = new PublishingCode
                //{
                //    Name = "ISBN"
                //}, // замініть на відповідне значення коду публікації
                Year = 2025, // рік публікації
                Country = "Україна", // країна
                City = "Київ" // місто
            };

           
            using var ctx = new BooksContext();
            //вщетукctx.Database.EnsureCreated();

            ctx.PublishingCodes.AddRange(
            new PublishingCode { Name = "ISBN" },
            new PublishingCode { Name = "ББК" },
            new PublishingCode { Name = "УДК" });

            ctx.DocumentTypes.AddRange(
            new DocumentType { Name = "Паспорт" },
            new DocumentType { Name = "Водійське посвідчення" },
            new DocumentType { Name = "ID-карта" });
            ctx.SaveChanges();
            ctx.Employees.Add(employee);
            ctx.Employees.Add(reader);
            ctx.Authors.Add(author);
            ctx.Books.Add(book);
            ctx.SaveChanges();
            
        }
    }
}
