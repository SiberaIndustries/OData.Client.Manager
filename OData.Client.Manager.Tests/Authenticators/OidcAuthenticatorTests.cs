using IdentityModel;
using IdentityModel.Client;
using OData.Client.Manager.Authenticators;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OData.Client.Manager.Tests.Authenticators
{
    public sealed class OidcAuthenticatorTests : IClassFixture<OidcAuthenticatorFixture>
    {
        private readonly OidcAuthenticatorFixture fixture;
        private readonly Uri uri = new Uri("http://domain.com");
        private readonly ITestOutputHelper output;
        private readonly OidcSettings[] settingsCollection = new[]
        {
            new OidcSettings(new Uri("http://localhost:5000"))
            {
                DiscoveryPolicy = new DiscoveryPolicy { RequireHttps = false },
                GrantType = OidcConstants.GrantTypes.Password,
                ClientId = "odata-manager-1",
                ClientSecret = "secret",
                Username = "bob",
                Password = "bob",
                Scope = "api1"
            },
            new OidcSettings(new Uri("http://localhost:5000"))
            {
                DiscoveryPolicy = new DiscoveryPolicy { RequireHttps = false },
                GrantType = OidcConstants.GrantTypes.Password,
                ClientId = "odata-manager-2",
                ClientSecret = "secret",
                Username = "alice",
                Password = "alice",
                Scope = "api1 offline_access"
            },
            new OidcSettings(new Uri("http://localhost:5000"))
            {
                DiscoveryPolicy = new DiscoveryPolicy { RequireHttps = false },
                GrantType = OidcConstants.GrantTypes.ClientCredentials,
                ClientId = "odata-manager-3",
                ClientSecret = "secret",
                Scope = "api1"
            }
        };

        public OidcAuthenticatorTests(ITestOutputHelper output, OidcAuthenticatorFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("Request header 'Authorization' already set")]
        [InlineData(null)]
        public async Task AuthenticateWithRequestMessage_Sucess(string errorMessage)
        {
            using var client = fixture.Client;
            foreach (var settings in settingsCollection)
            {
                output.WriteLine(settings.ClientId);
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
                settings.HttpClient = client;

                string error = null;
                var replaceAuthHeader = errorMessage == null;
                var authenticator = new OidcAuthenticator(settings)
                {
                    ReplaceAuthorizationHeader = replaceAuthHeader
                };

                authenticator.OnTrace += (msg) => error = msg;
                authenticator.OnTrace += (msg) => output.WriteLine(msg);

                Assert.True(await authenticator.AuthenticateAsync(requestMessage), settings.ClientId + " not authenticated");
                Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(requestMessage));

                Assert.Equal(errorMessage, error);
                Assert.NotNull(requestMessage.Headers.Authorization);
                Assert.Equal(authenticator.Header, requestMessage.Headers.Authorization);
                Assert.StartsWith("Bearer ey", requestMessage.Headers.Authorization.ToString());
            }
        }

        [Theory]
        [InlineData("Request header 'Authorization' already set")]
        [InlineData(null)]
        public async Task AuthenticateWithHttpClient_Sucess(string errorMessage)
        {
            using var client = fixture.Client;
            foreach (var settings in settingsCollection)
            {
                output.WriteLine(settings.ClientId);
                using var httpClient = new HttpClient { BaseAddress = uri };
                settings.HttpClient = client;

                string error = null;
                var replaceAuthHeader = errorMessage == null;
                var authenticator = new OidcAuthenticator(settings)
                {
                    ReplaceAuthorizationHeader = replaceAuthHeader
                };

                authenticator.OnTrace += (msg) => error = msg;
                authenticator.OnTrace += (msg) => output.WriteLine(msg);

                Assert.True(await authenticator.AuthenticateAsync(httpClient), settings.ClientId + " not authenticated");
                Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(httpClient));

                Assert.Equal(errorMessage, error);
                Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization);
                Assert.Equal(authenticator.Header, httpClient.DefaultRequestHeaders.Authorization);
                Assert.StartsWith("Bearer ey", httpClient.DefaultRequestHeaders.Authorization.ToString());
            }
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
            var versioningManager = new OidcAuthenticator(new OidcSettings());
            ArgumentNullException ex;

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(requestMessage: null));
            Assert.Equal("requestMessage", ex.ParamName);

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(httpClient: null));
            Assert.Equal("httpClient", ex.ParamName);
        }

        [Fact]
        public async Task GetTokenWithRefreshToken_Sucess()
        {
            using var client = fixture.Client;
            var settings = settingsCollection[1];
            using var httpClient = new HttpClient { BaseAddress = uri };
            settings.HttpClient = client;

            string error = null;
            var authenticator = new OidcAuthenticator(settings);
            authenticator.OnTrace += (msg) => error = msg;
            authenticator.OnTrace += (msg) => output.WriteLine(msg);

            Assert.True(await authenticator.AuthenticateAsync(httpClient), settings.ClientId + " not authenticated");

            Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization);
            Assert.StartsWith("Bearer ey", httpClient.DefaultRequestHeaders.Authorization.ToString());

            var token1 = await authenticator.GetTokenAsync(default);
            Assert.NotNull(token1?.RefreshToken);

            var token2 = await authenticator.GetTokenAsync(default);
            Assert.NotNull(token2?.RefreshToken);

            Assert.NotEqual(token1.RefreshToken, token2.RefreshToken);
        }

        [Fact]
        public async Task AuthenticateByCode_SuccessfullyTraced()
        {
            using var client = fixture.Client;
            using var httpClient = new HttpClient { BaseAddress = uri };

            string error = null;
            var authenticator = new OidcAuthenticator(new OidcSettings(new Uri("http://localhost:5000"))
            {
                DiscoveryPolicy = new DiscoveryPolicy { RequireHttps = false },
                GrantType = OidcConstants.GrantTypes.AuthorizationCode,
                ClientId = "odata-manager-4",
                ClientSecret = "secret",
                Scope = "api1",
                Code = "1",
                RedirectUri = new Uri("http://localhost:42/invalidVal"),
                HttpClient = client
            });
            authenticator.OnTrace += (msg) => error = msg;
            authenticator.OnTrace += (msg) => output.WriteLine(msg);

            Assert.False(await authenticator.AuthenticateAsync(httpClient), "odata-manager-4 not authenticated");
            Assert.Contains("localToken response could not be set.", error);
        }

        [Theory]
        [InlineData(nameof(OidcSettings.ClientId), "invalidVal", "token response has errors: invalid_client")]
        [InlineData(nameof(OidcSettings.ClientSecret), "invalidVal", "token response has errors: invalid_client")]
        [InlineData(nameof(OidcSettings.Username), "invalidVal", "token response has errors: invalid_grant")]
        [InlineData(nameof(OidcSettings.Password), "invalidVal", "token response has errors: invalid_grant")]
        [InlineData(nameof(OidcSettings.Scope), "invalidVal", "token response has errors: invalid_scope")]
        [InlineData(nameof(OidcSettings.GrantType), "invalidVal", "Grant type 'invalidVal' is not supported")]
        public async Task AuthenticateWithInvalidValues_SuccessfullyTraced(string property, object value, string expectedError)
        {
            using var client = fixture.Client;
            var settings = settingsCollection[0];
            using var httpClient = new HttpClient { BaseAddress = uri };
            settings.HttpClient = client;
            settings.GetType().GetProperty(property).SetValue(settings, value);

            string error = null;
            var authenticator = new OidcAuthenticator(settings);

            authenticator.OnTrace += (msg) => error += msg;
            authenticator.OnTrace += (msg) => output.WriteLine(msg);

            Assert.False(await authenticator.AuthenticateAsync(httpClient), settings.ClientId + " is authenticated with invalid values");
            Assert.Contains(expectedError, error);
            Assert.Contains("localToken response could not be set.", error);
        }

        [Fact]
        public async Task AuthenticateWithInvalidUri_SuccessfullyTraced()
        {
            using var client = fixture.Client;
            var settings = settingsCollection[0];
            using var httpClient = new HttpClient { BaseAddress = uri };
            settings.HttpClient = client;
            settings.AuthUri = new Uri("http://localhost:42/invalidVal");

            string error = null;
            var authenticator = new OidcAuthenticator(settings);

            authenticator.OnTrace += (msg) => error += msg;
            authenticator.OnTrace += (msg) => output.WriteLine(msg);

            Assert.False(await authenticator.AuthenticateAsync(httpClient), settings.ClientId + " is authenticated with invalid values");
            Assert.Contains("discovery response has errors: Error connecting to http://localhost:42/invalidVal/.well-known/openid-configuration.", error);
            Assert.Contains("localToken response could not be set.", error);
        }
    }
}
