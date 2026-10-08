using BankServer.Controllers;
using BankServer.Notifications;
using BankServer.Repositories;
using BankServer.Services;

namespace BankServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilter>());
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddSingleton<IAccountNotifier, LoggingAccountNotifier>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
