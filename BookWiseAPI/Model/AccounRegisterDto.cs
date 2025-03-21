using System.ComponentModel.DataAnnotations;

namespace BookWiseAPI.Controllers
{
    public record AccounRegisterDto(
     [StringLength(20, MinimumLength = 2)] string login,
     [StringLength(50, MinimumLength = 2)] string password,
     [EmailAddress] string email
     )
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public int DocumentId { get; set; }
        public string? DocumentNumber { get; set; }
    }
}
