namespace BookWiseAPI.Model
{
    public record FulAuthorDto(string? Name, string? LastName, string? SecondName)
    {
        public DateTime DateOfBirth { get; set; }
    };
}
