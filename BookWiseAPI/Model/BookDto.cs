using DataLibrary;

namespace BookWiseAPI.Model
{
    public record BookDto()
    {
        public string? Title { get; set; }
        public PublishingCode? TypeOfPublishingCode { get; set; }
        public ICollection<FulAuthorDto>? Authors { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int? DaysBorrowed { get; set; }
    };
}
