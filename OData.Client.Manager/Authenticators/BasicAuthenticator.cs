using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OData.Client.Manager.Authenticators
{
    public class BasicAuthenticator : IAuthenticator
    {
        private readonly AuthenticationHeaderValue header;

        public BasicAuthenticator(string username, string password)
        {
            header = new BasicAuthenticationHeaderValue(username, password);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private BasicAuthenticator()
        {
            throw new NotSupportedException();
        }

        public bool ReplaceAuthorizationHeader { get; set; }

        /// <inheritdoc cref="IAuthenticator" />
        public Action<string>? OnTrace { get; set; }

        /// <inheritdoc cref="IAuthenticator" />
        public Task<bool> AuthenticateAsync(HttpRequestMessage requestMessage, CancellationToken ct = default)
        {
            if (requestMessage == null)
            {
                throw new ArgumentNullException(nameof(requestMessage));
            }

            if (!ReplaceAuthorizationHeader && requestMessage.Headers.Authorization != null)
            {
                OnTrace?.Invoke($"Request header '{nameof(requestMessage.Headers.Authorization)}' already set");
                return Task.FromResult(false);
            }

            requestMessage.Headers.Authorization = header;
            return Task.FromResult(true);
        }

        /// <inheritdoc cref="IAuthenticator" />
        public Task<bool> AuthenticateAsync(HttpClient httpClient, CancellationToken ct = default)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (!ReplaceAuthorizationHeader && httpClient.DefaultRequestHeaders.Authorization != null)
            {
                OnTrace?.Invoke($"Request header '{nameof(httpClient.DefaultRequestHeaders.Authorization)}' already set");
                return Task.FromResult(false);
            }

            httpClient.DefaultRequestHeaders.Authorization = header;
            return Task.FromResult(true);
        }
    }
}
