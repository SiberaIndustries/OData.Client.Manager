using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TestAuthorizationServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddTestUsers(Config.TestUsers);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }
    }
}
