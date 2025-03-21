using DataLibrary;

namespace BookWiseAPI.Controllers
{
    public interface IBorrowedBooksByUserService
    {
        Task<ICollection<BorrowedBook>> GetBorrowedBooksByUserAsync(string token);
    }
}
