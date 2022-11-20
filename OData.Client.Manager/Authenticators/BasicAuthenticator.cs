namespace OData.Client.Manager.Authenticators
{
    public class BasicAuthenticator : AuthenticatorBase
    {
        public BasicAuthenticator(string username, string password)
        {
            Header = new BasicAuthenticationHeaderValue(username, password);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private BasicAuthenticator()
        {
            throw new NotSupportedException();
        }
    }
}
