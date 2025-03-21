using DataLibrary;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace BookWiseAPI.Controllers
{
    public class BorrowedBooksByUserService : IBorrowedBooksByUserService
    {
        private readonly BooksContext _booksContext;

        public BorrowedBooksByUserService(BooksContext booksContext)
        {
            _booksContext = booksContext;
        }

        public async Task<ICollection<BorrowedBook>> GetBorrowedBooksByUserAsync(string token)
        {
            var log = new JwtSecurityTokenHandler().ReadJwtToken(token)?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;
            if (string.IsNullOrEmpty(log)) return default;

            var readerId = await _booksContext.Employees
                .Where(u => u.Login == log)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (readerId == 0) return default;

            var borrowedBooks = await _booksContext.BorrowedBooks
                .Where(b => b.ReaderId == readerId)
                .Include(b => b.Book)
                .Include(r => r.Reader)
                .Include(a => a.Book.Authors)
                .Where(v => v.Reader != null)
                .ToListAsync();

            return borrowedBooks;
        }
    }
}
