using BookWiseAPI.Model;

namespace BookWiseAPI.Controllers
{
    public record TakeBookDto(string Title, ICollection<FulAuthorDto> Authors);
}
