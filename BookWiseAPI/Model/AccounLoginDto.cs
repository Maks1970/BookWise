using System.ComponentModel.DataAnnotations;

namespace BookWiseAPI.Controllers
{
    public record AccounLoginDto(
        [StringLength(20, MinimumLength = 2)] string login,
        [StringLength(50, MinimumLength = 2)] string password);
}
