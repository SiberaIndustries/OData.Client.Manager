using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace TestAuthorizationServer
{
    internal static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>(2)
        {
            new ApiScope("api1", "Api 1"),
            new ApiScope("api2", "Api 2"),
        };

        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>(2)
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        public static IEnumerable<Client> Clients => new List<Client>(4)
        {
            new Client
            {
                ClientId = "odata-manager-1",
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
            },
            new Client
            {
                ClientId = "odata-manager-3",
                ClientSecrets = { new Secret("secret".Sha512()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "api1" }
            },
            new Client
            {
                ClientId = "odata-manager-4",
                ClientSecrets = { new Secret("secret".Sha512()) },
                RedirectUris = { "http://localhost:5000" },
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = { "api1" }
            }
        };

        public static List<TestUser> TestUsers => new(2)
        {
            new TestUser { Username = "bob", Password = "bob", SubjectId = "bob", Claims = { new Claim(JwtClaimTypes.Email, "bob@mail.com") } },
            new TestUser { Username = "alice", Password = "alice", SubjectId = "alice", Claims = { new Claim(JwtClaimTypes.Email, "alice@mail.com") } },
        };
    }
}
