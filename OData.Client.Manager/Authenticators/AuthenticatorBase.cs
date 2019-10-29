using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OData.Client.Manager.Authenticators
{
    public abstract class AuthenticatorBase : IAuthenticator
    {
        public AuthenticationHeaderValue? Header { get; protected set; }

        public bool ReplaceAuthorizationHeader { get; set; }

        /// <inheritdoc cref="IAuthenticator" />
        public Action<string>? OnTrace { get; set; }

        /// <inheritdoc cref="IAuthenticator" />
        public virtual Task<bool> AuthenticateAsync(HttpRequestMessage requestMessage, CancellationToken ct = default)
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

            requestMessage.Headers.Authorization = Header;
            return Task.FromResult(true);
        }

        /// <inheritdoc cref="IAuthenticator" />
        public virtual Task<bool> AuthenticateAsync(HttpClient httpClient, CancellationToken ct = default)
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

            httpClient.DefaultRequestHeaders.Authorization = Header;
            return Task.FromResult(true);
        }
    }
}
