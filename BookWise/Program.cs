using DataLibrary;

namespace BookWise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            using var ctx = new BooksContext();
            ctx.Database.EnsureCreated();
        }
    }
}
