using DataLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BookWiseAPI.Controllers.AccountController;

namespace BookWiseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderController : ControllerBase
    {
        [HttpPost("AllReader")]
        public async Task<IActionResult> Login(AccounLoginDto loginDto)
        {
            
            return Ok("User successfully");
        }
    }
}
