using OData.Client.Manager.Authenticators;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OData.Client.Manager.Tests.Authenticators
{
    public class JwtAuthenticatorTests
    {
        private const string uriString = "http://domain.com";
        private readonly HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString));
        private readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri(uriString) };

        [Theory]
        [InlineData("foobar", "Bearer foobar", "Request header 'Authorization' already set")]
        [InlineData("foobar", "Bearer foobar", null)]
        public async Task AuthenticateWithRequestMessage_Sucess(string token, string expectedAuthHeader, string errorMessage)
        {
            string error = null;
            var replaceAuthHeader = errorMessage == null;
            var authenticator = new JwtAuthenticator(token)
            {
                ReplaceAuthorizationHeader = replaceAuthHeader
            };
            authenticator.OnTrace += (msg) => error = msg;

            Assert.True(await authenticator.AuthenticateAsync(requestMessage));
            Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(requestMessage));

            Assert.Equal(errorMessage, error);
            Assert.NotNull(requestMessage.Headers.Authorization);
            Assert.Equal(expectedAuthHeader, requestMessage.Headers.Authorization.ToString());
        }

        [Theory]
        [InlineData("foobar", "Bearer foobar", "Request header 'Authorization' already set")]
        [InlineData("foobar", "Bearer foobar", null)]
        public async Task AuthenticateWithHttpClient_Sucess(string token, string expectedAuthHeader, string errorMessage)
        {
            string error = null;
            var replaceAuthHeader = errorMessage == null;
            var authenticator = new JwtAuthenticator(token)
            {
                ReplaceAuthorizationHeader = replaceAuthHeader
            };
            authenticator.OnTrace += (msg) => error = msg;

            Assert.True(await authenticator.AuthenticateAsync(httpClient));
            Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(httpClient));

            Assert.Equal(errorMessage, error);
            Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization);
            Assert.Equal(expectedAuthHeader, httpClient.DefaultRequestHeaders.Authorization.ToString());
        }

        [Theory]
        [InlineData(null, "token")]
        public void InstantiateWithInvalidValues_Exception(string token, string paramName)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new JwtAuthenticator(token));
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public async Task AuthenticateOnNullObject_Exception()
        {
            var versioningManager = new JwtAuthenticator("foobar");
            ArgumentNullException ex;

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(requestMessage: null));
            Assert.Equal("requestMessage", ex.ParamName);

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(httpClient: null));
            Assert.Equal("httpClient", ex.ParamName);
        }
    }
}
