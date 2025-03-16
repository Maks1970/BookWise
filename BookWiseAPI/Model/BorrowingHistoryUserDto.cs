using DataLibrary;

namespace BookWiseAPI.Model
{
    public record BorrowingHistoryUserDto(ICollection<BorrowedBookDTO> BorrowedBooks,int? countOverdue)
    {
    }
}
