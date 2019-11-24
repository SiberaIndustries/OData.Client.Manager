using OData.Client.Manager.Authenticators;
using OData.Client.Manager.Versioning;
using Simple.OData.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OData.Client.Manager
{
    public class ODataManager : IODataManager
    {
        private readonly ODataClientSettings settings;
        private readonly Func<HttpRequestMessage, Task> beforeRequestTemp;
        private readonly IAuthenticator? authenticator;
        private readonly IVersioningManager? versioningManager;
        private readonly string? authenticatorName;
        private readonly string? versioningManagerName;

        public ODataManager(ODataManagerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            settings = configuration as ODataClientSettings;

            authenticator = configuration.Authenticator;
            if (authenticator != null)
            {
                authenticatorName = authenticator.GetType().Name;
                authenticator.OnTrace += TraceAuthenticatorMessage;
            }

            versioningManager = configuration.VersioningManager;
            if (versioningManager != null)
            {
                versioningManagerName = versioningManager.GetType().Name;
                versioningManager.OnTrace += TraceVersioningManagerMessage;
            }

            beforeRequestTemp = settings.BeforeRequestAsync;
            settings.BeforeRequestAsync = BeforeRequestAsync;

            Client = new ODataClient(settings);
        }

        public ODataManager(Uri apiEndpoint)
            : this(new ODataManagerConfiguration(apiEndpoint))
        {
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private ODataManager()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IODataManager" />
        public IODataClient Client { get; private set; }

        private async Task BeforeRequestAsync(HttpRequestMessage requestMessage)
        {
            if (authenticator != null && !await authenticator.AuthenticateAsync(requestMessage).ConfigureAwait(false))
            {
                settings.OnTrace?.Invoke("{0}: Authentication not successful", new[] { nameof(ODataManager) });
            }

            if (versioningManager != null && !versioningManager.ApplyVersion(requestMessage))
            {
                settings.OnTrace?.Invoke("{0}: Applying version not successful", new[] { nameof(ODataManager) });
            }

            if (beforeRequestTemp != null)
            {
                await beforeRequestTemp.Invoke(requestMessage).ConfigureAwait(false);
            }
        }

        private void TraceAuthenticatorMessage(string msg)
        {
            settings.OnTrace?.Invoke("{0}: {1}", new[] { authenticatorName, msg });
        }

        private void TraceVersioningManagerMessage(string msg)
        {
            settings.OnTrace?.Invoke("{0}: {1}", new[] { versioningManagerName, msg });
        }
    }
}
