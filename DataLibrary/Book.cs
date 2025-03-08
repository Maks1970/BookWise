namespace DataLibrary
{
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
        public int DaysBorrowed { get; set; }
    }

}
