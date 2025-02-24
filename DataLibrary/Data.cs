using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace DataLibrary
{
    public class Employee
    {
        public int Id {get; set;}
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email {  get; set; }
    }
    public class Reader : Employee 
    {
        public string Name { get; set; }
        public string LastName {  get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentType DocumenttType { get; set; }
        public string DocumentNumber { get; set; }

    }
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<Book> Books { get; set; }

    }

    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Author> Authors { get; set; }
        public int PublishingCodeId { get; set; }
        public PublishingCode TypeOfPublishingCode { get; set; }
        public int Year { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
    //        тип видавничого коду(посилання на таблицю)
    //Рік
    //Країна видавництва
    //Місто видавництва

    public class PublishingCode
    {
        public int PublishingCodeId { get; set; }
        public string Name { get; set; }
    }
    public class DocumentType
    {
        public int DocumentTypeId { get; set; }
        public string Name { get; set; } // Наприклад, "Паспорт", "Водійські права"
    }

    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<PublishingCode> PublishingCodes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=WIN-JLM4T99KJ5L;Initial Catalog=BookWise;Integrated Security=True;Trust Server Certificate=True")
                .LogTo(Console.WriteLine); 
            base.OnConfiguring(optionsBuilder);
        }
    }

}
