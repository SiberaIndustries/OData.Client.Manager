using System;
using System.Net.Http.Headers;

namespace OData.Client.Manager.Authenticators
{
    public class JwtAuthenticator : AuthenticatorBase
    {
        private const string Scheme = "Bearer";

        public JwtAuthenticator(string token)
        {
            Header = new AuthenticationHeaderValue(Scheme, token ?? throw new ArgumentNullException(nameof(token)));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private JwtAuthenticator()
        {
            throw new NotSupportedException();
        }
    }
}
