using IdentityModel;
using IdentityModel.Client;

namespace OData.Client.Manager.Authenticators
{
    public class OidcSettings
    {
        public OidcSettings(Uri? authUri = null)
        {
            AuthUri = authUri;
        }

        /// <summary>
        /// Gets or sets the authentication endpoint uri.
        /// </summary>
        public Uri? AuthUri { get; set; }

        /// <summary>
        /// Gets or sets the discovery policy.
        /// </summary>
        public DiscoveryPolicy? DiscoveryPolicy { get; set; }

        /// <summary>
        /// Gets or sets the grant type (default: Password).
        /// </summary>
        public string GrantType { get; set; } = OidcConstants.GrantTypes.Password;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password (when using grant type: password).
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// Gets or sets the redirect uri (when using grant type: authorization_code).
        /// </summary>
        public Uri? RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the authorization code (when using grant type: authorization_code).
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the optional PKCE parameter (when using grant type: authorization_code).
        /// </summary>
        public string? CodeVerifier { get; set; }

        /// <summary>
        /// Gets or sets an optional http client.
        /// </summary>
        public HttpClient? HttpClient { get; set; }
    }
}
