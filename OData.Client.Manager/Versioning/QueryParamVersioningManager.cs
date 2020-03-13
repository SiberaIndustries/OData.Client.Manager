using OData.Client.Manager.Extensions;
using System;
using System.Net.Http;

namespace OData.Client.Manager.Versioning
{
    public class QueryParamVersioningManager : IVersioningManager
    {
        private readonly string parameterName;
        private readonly string version;

        public QueryParamVersioningManager(string version, string parameterName = "api-version")
        {
            this.parameterName = string.IsNullOrWhiteSpace(parameterName)
                ? throw new ArgumentNullException(nameof(parameterName))
                : parameterName;
            this.version = version;
        }

        public Action<string>? OnTrace { get; set; }

        public bool ApplyVersion(HttpRequestMessage requestMessage)
        {
            _ = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));

#if NETSTANDARD2_0
            if (requestMessage.RequestUri.Query.Contains(parameterName))
#else
            if (requestMessage.RequestUri.Query.Contains(parameterName, StringComparison.Ordinal))
#endif
            {
                OnTrace?.Invoke($"The already existing query parameter {parameterName} gets overridden");
            }

            requestMessage.RequestUri = requestMessage.RequestUri.AddParameter(parameterName, version);
            return true;
        }
    }
}
