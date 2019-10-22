using Simple.OData.Client;

namespace OData.Client.Manager
{
    public interface IODataManager
    {
        /// <summary>
        /// Gets the OData client implementation.
        /// </summary>
        IODataClient Client { get; }
    }
}
