using DataLibrary;

namespace BookWiseAPI.Controllers
{
    public interface ITokenService
    {
        string CreateToken(Employee user);
    }
}
