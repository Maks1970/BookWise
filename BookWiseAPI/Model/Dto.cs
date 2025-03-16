namespace BookWiseAPI.Model
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AuthorDTO> Authors { get; set; }
    }
    public class BorrowedBookDTO: BookDTO
    {
        public DateTime DateBorrowed { get; set; }
        public DateTime DateForBorrowed { get; set; }
        public DateTime? DateReturned { get; set; }
    }
    public class ReaderDTO
    {
        public string Login { get; set; }
        public List<BorrowedBookDTO> BorrowedBooks { get; set; }
    }
    public class AuthorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
