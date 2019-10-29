using Simple.OData.Client;
using System;

namespace OData.Client.Manager
{
    public class ODataManager : IODataManager
    {
        public ODataManager(ODataManagerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var settings = configuration as ODataClientSettings;

            var authenticator = configuration.Authenticator;
            if (authenticator != null)
            {
                var authenticatorName = authenticator.GetType().Name;
                authenticator.OnTrace += (msg) => settings.OnTrace?.Invoke("{0}: {1}", new[] { authenticatorName, msg });
            }

            var versioningManager = configuration.VersioningManager;
            if (versioningManager != null)
            {
                var versioningManagerName = versioningManager.GetType().Name;
                versioningManager.OnTrace += (msg) => settings.OnTrace?.Invoke("{0}: {1}", new[] { versioningManagerName, msg });
            }

            var beforeRequestTemp = settings.BeforeRequestAsync;
            settings.BeforeRequest = async (requestMessage) =>
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
            };

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
    }
}
