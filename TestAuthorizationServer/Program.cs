using System.Diagnostics.CodeAnalysis;

namespace TestAuthorizationServer
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddTestUsers(Config.TestUsers);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseIdentityServer();

            app.Run();
        }
    }
}
