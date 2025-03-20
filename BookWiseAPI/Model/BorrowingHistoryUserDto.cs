using DataLibrary;

namespace BookWiseAPI.Model
{
    public record BorrowingHistoryUserDto(ICollection<BorrowedBookDto> BorrowedBooks,int? countOverdue)
    {
    }
}
