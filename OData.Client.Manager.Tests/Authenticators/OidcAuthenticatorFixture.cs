using Microsoft.AspNetCore.Mvc.Testing;
using TestAuthorizationServer;

namespace OData.Client.Manager.Tests.Authenticators
{
    public class OidcAuthenticatorFixture : WebApplicationFactory<Startup>, IDisposable
    {
        private bool disposed = false;

        public OidcAuthenticatorFixture()
        {
            ClientOptions.BaseAddress = new Uri("http://localhost:5000");
        }

        public HttpClient Client => CreateClient();

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Client?.Dispose();
                }

                disposed = true;
                base.Dispose(disposing);
            }
        }

    }
}
