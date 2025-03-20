namespace BookWiseAPI.Model
{
    public class BorrowedBookDto : BookDto
    {
        public DateTime DateBorrowed { get; set; }
        public DateTime DateForBorrowed { get; set; }
        public DateTime? DateReturned { get; set; }
    }
}
