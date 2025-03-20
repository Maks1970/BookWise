using Azure.Core;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using System.Formats.Asn1;
using System.Formats.Tar;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace BookWiseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BooksContext _booksContext;
        private readonly ITokenService _tokenService;

        public AccountController(BooksContext booksContext, ITokenService tokenService)
        {
            _booksContext = booksContext;
            _tokenService = tokenService;
        }

        [HttpPost("register/{userType}")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(string userType, AccounRegisterDto registerDto)
        {
            if(await _booksContext.Employees.AnyAsync(u=>u.Login== registerDto.login))
            {
                return BadRequest();
            }
            Employee employee = new Employee();

            if (userType == "Reader")
            {
               Reader reader = new Reader()
                {
                    Login = registerDto.login,
                    Password = registerDto.password,
                    Email = registerDto.email,
                    Name = registerDto.Name,
                    LastName = registerDto.LastName,
                    DocumentTypeId = registerDto.DocumentId,
                    DocumentNumber = registerDto.DocumentNumber
                };
                await _booksContext.Readers.AddAsync(reader);
                employee = reader ;
            }
            else if(userType == "Employee")
            {
                 employee = new Employee()
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


            var token = _tokenService.CreateToken(employee);
            await _booksContext.SaveChangesAsync();

            return Ok(new ACcountLoginResultDto(employee.Login, token));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AccounLoginDto loginDto)
        {
            var user = await _booksContext.Employees.SingleOrDefaultAsync(u => u.Login == loginDto.login);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Password != loginDto.password)
            {
                return Unauthorized();
            }
            var token = _tokenService.CreateToken(user);
            return Ok((new ACcountLoginResultDto(user.Login, token)));
        }
    }
}
