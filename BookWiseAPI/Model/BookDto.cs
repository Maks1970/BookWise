using DataLibrary;

namespace BookWiseAPI.Model
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AuthorDto> Authors { get; set; }
        public PublishingCode? TypeOfPublishingCode { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int? DaysBorrowed { get; set; }
    }
}
