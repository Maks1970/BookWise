using Azure.Core;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Formats.Tar;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        //public DocumentType? DocumenttType { get; set; }
        public string? DocumentNumber { get; set; }
    }
    public record AccounLoginDto(
        [StringLength(20, MinimumLength = 2)] string login,
        [StringLength(50, MinimumLength = 2)] string password);
    public record ACcountLoginResultDto(string login, string token);
    public interface ITokenService
    {
        string CreateToken(Employee user);
    }
    internal class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            var strkey = configuration["TokenKey"];
            if (strkey == null) throw new ArgumentNullException(nameof(strkey));
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));

        }
        public string CreateToken(Employee user)
        {
            var claims = new List<Claim>
                {
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.NameId,user.Login),
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(ClaimTypes.Role, user.GetType().Name)
                };
            var signature = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var descr = new SecurityTokenDescriptor
            {
                SigningCredentials = signature,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(5)
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descr);
            return handler.WriteToken(token);
        }
    }


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
            //var ss = _booksContext.Readers.Where(x => x.Login == "pas4").ToList()[0].Name;
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
