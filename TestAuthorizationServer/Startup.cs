using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;

namespace TestAuthorizationServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(GetClients())
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddTestUsers(GetTestUsers());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }

        private static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
        {
            new ApiResource("api1", "Api 1"),
            new ApiResource("api2", "Api 1"),
        };

        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        public static IEnumerable<Client> GetClients() => new List<Client>
        {
            new Client
            {
                ClientId = "odata-manager",
                ClientSecrets = { new Secret("secret".Sha512()) },
                RedirectUris = { "http://localhost:5000" },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "api1" }
            },
            new Client
            {
                ClientId = "odata-manager-2",
                ClientSecrets = { new Secret("secret".Sha512()) },
                RedirectUris = { "http://localhost:5000" },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "api1" },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 1
            }
        };

        public static List<TestUser> GetTestUsers() => new List<TestUser>
        {
            new TestUser { Username = "bob", Password = "bob", SubjectId = "bob", Claims = { new Claim(JwtClaimTypes.Email, "bob@mail.com") } },
            new TestUser { Username = "alice", Password = "alice", SubjectId = "alice", Claims = { new Claim(JwtClaimTypes.Email, "alice@mail.com") } },
        };
    }
}
