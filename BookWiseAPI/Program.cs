
using BookWiseAPI.Controllers;
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddScoped<ITokenService, TokenService>();

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
