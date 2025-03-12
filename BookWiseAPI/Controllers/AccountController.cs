using Azure.Core;
using DataLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Reflection.PortableExecutable;

namespace BookWiseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BooksContext _booksContext = new BooksContext();
        public record AccounRegisterDto(
            [StringLength (20,MinimumLength =2)] string login,
            [StringLength(50, MinimumLength = 2)] string password,
            [EmailAddress]string email
            ) 
        {
            public string? Name { get; set; }
            public string? LastName { get; set; }
            public DocumentType? DocumenttType { get; set; }
            public string? DocumentNumber { get; set; }
        }
        public record AccounLoginDto(
            [StringLength(20, MinimumLength = 2)] string login,
            [StringLength(50, MinimumLength = 2)] string password);


        [HttpPost("register/{userType}")]
        public async Task<ActionResult> Register(string userType, [FromBody] AccounRegisterDto registerDto)
        {
            if(await _booksContext.Employees.AnyAsync(u=>u.Login== registerDto.login))
            {
                return BadRequest();
            }
            //switch (userType)
            //{
            //    case "Reader":
            //        break;
            //}
            if(userType == "Reader")
            {
                Reader reader = new Reader()
                {
                    Login = registerDto.login,
                    Password = registerDto.password,
                    Email = registerDto.email,
                    Name = registerDto.Name,
                    LastName = registerDto.LastName,
                    DocumenttType = registerDto.DocumenttType,
                    DocumentNumber = registerDto.DocumentNumber
                };
                await _booksContext.Readers.AddAsync(reader);
            }
            else if(userType == "Employee")
            {
                Employee employee = new Employee()
                {
                    Login = registerDto.login,
                    Password = registerDto.password,
                    Email = registerDto.email,
                };
                await _booksContext.Employees.AddAsync(employee);
            }
            else
            {
                return BadRequest("Invalid user type.");
            }
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            await _booksContext.SaveChangesAsync();
            return Ok(new { token = "JWT_TOKEN" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AccounLoginDto loginDto)
        {
            //var ss = _booksContext.Readers.Where(x => x.Login == "pas4").ToList()[0].Name;
            var user = await _booksContext.Employees.SingleOrDefaultAsync(u => u.Login == loginDto.login);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Password != loginDto.password)
            {
                return NotFound("Wrong pass");
            }
            return Ok("User successfully");
        }
    }
}
