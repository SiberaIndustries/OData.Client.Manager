using IdentityModel;
using Microsoft.AspNetCore.Mvc.Testing;
using OData.Client.Manager.Authenticators;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TestAuthorizationServer;
using Xunit;

namespace OData.Client.Manager.Tests.Authenticators
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "Test case")]
    public class OidcAuthenticatorTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> factory;

        private const string uriString = "http://domain.com";
        private readonly HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString));
        private readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri(uriString) };
        private readonly OidcSettings settings = new OidcSettings(null)
        {
            DiscoveryPolicy = null,
            GrantType = null,
            ClientId = null,
            ClientSecret = null,
            Username = null,
            Password = null,
            Scope = null,
            RedirectUri = null,
            Code = null,
            CodeVerifier = null,
            HttpClient = null
        };

        public OidcAuthenticatorTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        public void Dispose()
        {
            requestMessage?.Dispose();
            httpClient?.Dispose();
        }

        [Theory]
        [InlineData("Request header 'Authorization' already set")]
        [InlineData(null)]
        public async Task AuthenticateWithRequestMessage_Sucess(string errorMessage)
        {
            var client = factory.CreateDefaultClient(new Uri("http://localhost:5000"));
            settings.HttpClient = client;
            settings.ClientId = "odata-manager";
            settings.ClientSecret = "secret";
            settings.Scope = "api1";
            settings.Username = "bob";
            settings.Password = "bob";
            settings.GrantType = OidcConstants.GrantTypes.Password;

            string error = null;
            var replaceAuthHeader = errorMessage == null;
            var authenticator = new OidcAuthenticator(settings)
            {
                ReplaceAuthorizationHeader = replaceAuthHeader
            };

            authenticator.OnTrace += (msg) => error = msg;

            Assert.True(await authenticator.AuthenticateAsync(requestMessage));
            Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(requestMessage));

            Assert.Equal(errorMessage, error);
            Assert.NotNull(requestMessage.Headers.Authorization);
            Assert.Equal(authenticator.Header, requestMessage.Headers.Authorization);
            Assert.StartsWith("Bearer ey", requestMessage.Headers.Authorization.ToString());
        }

        [Theory]
        [InlineData("Request header 'Authorization' already set")]
        [InlineData(null)]
        public async Task AuthenticateWithHttpClient_Sucess(string errorMessage)
        {
            var client = factory.CreateDefaultClient(new Uri("http://localhost:5000"));
            settings.HttpClient = client;
            settings.ClientId = "odata-manager";
            settings.ClientSecret = "secret";
            settings.Scope = "api1";
            settings.Username = "bob";
            settings.Password = "bob";
            settings.GrantType = OidcConstants.GrantTypes.Password;

            string error = null;
            var replaceAuthHeader = errorMessage == null;
            var authenticator = new OidcAuthenticator(settings)
            {
                ReplaceAuthorizationHeader = replaceAuthHeader
            };
            authenticator.OnTrace += (msg) => error = msg;

            Assert.True(await authenticator.AuthenticateAsync(httpClient));
            Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(httpClient));

            Assert.Equal(errorMessage, error);
            Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization);
            Assert.Equal(authenticator.Header, httpClient.DefaultRequestHeaders.Authorization);
            Assert.StartsWith("Bearer ey", httpClient.DefaultRequestHeaders.Authorization.ToString());
        }

        [Fact]
        public void InstantiateWithInvalidValues_Exception()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OidcAuthenticator(null));
            Assert.Equal("oidcSettings", ex.ParamName);
        }

        [Fact]
        public async Task AuthenticateOnNullObject_Exception()
        {
            var versioningManager = new OidcAuthenticator(settings);
            ArgumentNullException ex;

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(requestMessage: null));
            Assert.Equal("requestMessage", ex.ParamName);

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(httpClient: null));
            Assert.Equal("httpClient", ex.ParamName);
        }

        [Fact]
        public async Task GetTokenWithRefreshToken_Sucess()
        {
            var client = factory.CreateDefaultClient(new Uri("http://localhost:5000"));
            settings.HttpClient = client;
            settings.ClientId = "odata-manager-2";
            settings.ClientSecret = "secret";
            settings.Scope = "api1 offline_access";
            settings.Username = "alice";
            settings.Password = "alice";
            settings.GrantType = OidcConstants.GrantTypes.Password;

            string error = null;
            var authenticator = new OidcAuthenticator(settings);
            authenticator.OnTrace += (msg) => error = msg;

            Assert.True(await authenticator.AuthenticateAsync(httpClient));

            Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization);
            Assert.StartsWith("Bearer ey", httpClient.DefaultRequestHeaders.Authorization.ToString());

            var token1 = await authenticator.GetTokenAsync(default);
            Assert.NotNull(token1?.RefreshToken);

            var token2 = await authenticator.GetTokenAsync(default);
            Assert.NotNull(token2?.RefreshToken);

            Assert.NotEqual(token1.RefreshToken, token2.RefreshToken);
        }
    }
}
