using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace DataLibrary
{

    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<PublishingCode> PublishingCodes { get; set; }
        public BooksContext() { }
        public BooksContext(DbContextOptions<BooksContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=WIN-JLM4T99KJ5L;Initial Catalog=BookWise;Integrated Security=True;Trust Server Certificate=True")
                /*.LogTo(Console.WriteLine)*/; 
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Створюємо унікальний індекс для поля Email
            modelBuilder.Entity<Employee>()
                .HasIndex(u => u.Login)
                .IsUnique();
        }
    }

}
