using DataLibrary;

namespace BookWiseAPI.Controllers
{
    public interface IBooksService
    {
        Task<ICollection<Book>> GetBooks();
    }
}
