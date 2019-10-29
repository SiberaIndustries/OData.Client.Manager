using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OData.Client.Manager.Authenticators
{
    public interface IAuthenticator
    {
        /// <summary>
        /// Gets or sets the action delegate that will be executed to write trace messages.
        /// </summary>
        Action<string>? OnTrace { get; set; }

        /// <summary>
        /// Ensures the authentication of the given http request message.
        /// </summary>
        /// <param name="requestMessage">The http request message which has to be authenticated.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Indicates whether the authentication was successful or not.</returns>
        Task<bool> AuthenticateAsync(HttpRequestMessage requestMessage, CancellationToken ct = default);

        /// <summary>
        /// Ensures the authentication of http request messages of the http client.
        /// </summary>
        /// <param name="httpClient">The http client.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Indicates whether the authentication was successful or not.</returns>
        Task<bool> AuthenticateAsync(HttpClient httpClient, CancellationToken ct = default);
    }
}
