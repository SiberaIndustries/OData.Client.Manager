using OData.Client.Manager.Authenticators;
using OData.Client.Manager.Versioning;
using Simple.OData.Client;
using System;
using System.Net;
using System.Net.Http;

namespace OData.Client.Manager
{
    public class ODataManagerConfiguration : ODataClientSettings
    {
        public ODataManagerConfiguration(HttpClient httpClient, Uri? baseUri = null, ICredentials? credentials = null)
            : base(httpClient, baseUri, credentials)
        {
            UseAbsoluteReferenceUris = true;
        }

        public ODataManagerConfiguration(Uri baseUri, ICredentials? credentials = null)
            : base(baseUri, credentials)
        {
            UseAbsoluteReferenceUris = true;
        }

        public ODataManagerConfiguration()
            : base()
        {
            UseAbsoluteReferenceUris = true;
        }

        /// <summary>
        /// Gets or sets the api version manager for the base uri.
        /// </summary>
        public IVersioningManager? VersioningManager { get; set; }

        /// <summary>
        /// Gets or sets the authenticator.
        /// </summary>
        public IAuthenticator? Authenticator { get; set; }
    }
}
