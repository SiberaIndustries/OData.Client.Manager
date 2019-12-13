using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace TestAuthorizationServer
{
    internal static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
        {
            new ApiResource("api1", "Api 1"),
            new ApiResource("api2", "Api 1"),
        };

        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        public static IEnumerable<Client> Clients => new List<Client>
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

        public static List<TestUser> TestUsers => new List<TestUser>
        {
            new TestUser { Username = "bob", Password = "bob", SubjectId = "bob", Claims = { new Claim(JwtClaimTypes.Email, "bob@mail.com") } },
            new TestUser { Username = "alice", Password = "alice", SubjectId = "alice", Claims = { new Claim(JwtClaimTypes.Email, "alice@mail.com") } },
        };
    }
}
