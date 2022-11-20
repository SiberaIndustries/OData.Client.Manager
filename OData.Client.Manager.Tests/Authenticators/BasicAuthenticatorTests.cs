using OData.Client.Manager.Authenticators;
using Xunit;

namespace OData.Client.Manager.Tests.Authenticators
{
    public class BasicAuthenticatorTests
    {
        private const string uriString = "http://domain.com";
        private readonly HttpRequestMessage requestMessage = new(HttpMethod.Get, new Uri(uriString));
        private readonly HttpClient httpClient = new() { BaseAddress = new Uri(uriString) };

        [Theory]
        [InlineData("foo", "bar", "Basic Zm9vOmJhcg==", "Request header 'Authorization' already set")]
        [InlineData("foo", "bar", "Basic Zm9vOmJhcg==", null)]
        public async Task AuthenticateWithRequestMessage_Sucess(string user, string pw, string expectedAuthHeader, string errorMessage)
        {
            string error = null;
            var replaceAuthHeader = errorMessage == null;
            var authenticator = new BasicAuthenticator(user, pw)
            {
                ReplaceAuthorizationHeader = replaceAuthHeader
            };
            authenticator.OnTrace += (msg) => error = msg;

            Assert.True(await authenticator.AuthenticateAsync(requestMessage));
            Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(requestMessage));

            Assert.Equal(errorMessage, error);
            Assert.NotNull(requestMessage.Headers.Authorization);
            Assert.Equal(authenticator.Header, requestMessage.Headers.Authorization);
            Assert.Equal(expectedAuthHeader, requestMessage.Headers.Authorization.ToString());
        }

        [Theory]
        [InlineData("foo", "bar", "Basic Zm9vOmJhcg==", "Request header 'Authorization' already set")]
        [InlineData("foo", "bar", "Basic Zm9vOmJhcg==", null)]
        public async Task AuthenticateWithHttpClient_Sucess(string user, string pw, string expectedAuthHeader, string errorMessage)
        {
            string error = null;
            var replaceAuthHeader = errorMessage == null;
            var authenticator = new BasicAuthenticator(user, pw)
            {
                ReplaceAuthorizationHeader = replaceAuthHeader
            };
            authenticator.OnTrace += (msg) => error = msg;

            Assert.True(await authenticator.AuthenticateAsync(httpClient));
            Assert.Equal(replaceAuthHeader, await authenticator.AuthenticateAsync(httpClient));

            Assert.Equal(errorMessage, error);
            Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization);
            Assert.Equal(authenticator.Header, httpClient.DefaultRequestHeaders.Authorization);
            Assert.Equal(expectedAuthHeader, httpClient.DefaultRequestHeaders.Authorization.ToString());
        }

        [Theory]
        [InlineData(null, "foo", "userName")]
        public void InstantiateWithInvalidValues_Exception(string user, string pw, string paramName)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new BasicAuthenticator(user, pw));
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public async Task AuthenticateOnNullObject_Exception()
        {
            var versioningManager = new BasicAuthenticator("foo", "bar");
            ArgumentNullException ex;

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(requestMessage: null));
            Assert.Equal("requestMessage", ex.ParamName);

            ex = await Assert.ThrowsAsync<ArgumentNullException>(() => versioningManager.AuthenticateAsync(httpClient: null));
            Assert.Equal("httpClient", ex.ParamName);
        }
    }
}
