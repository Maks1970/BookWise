
using BookWiseAPI.Controllers;
using BookWiseAPI.Services;
using BookWiseAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static BookWiseAPI.Controllers.AccountController;

namespace BookWiseAPI
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DataLibrary.BooksContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("BookWiseDB")));
            //builder.Services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //});
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IBooksService, BookService>();
            builder.Services.AddScoped<IBorrowedBooksByUserService, BorrowedBooksByUserService>();
            builder.Services.AddScoped<IAuthorUpdate, AuthorUpdateService>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt=>
                {
                    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]))
                    };
                });

            var app = builder.Build();
            app.Map("/", async (context) =>
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("  MAXON  ");
            });            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
