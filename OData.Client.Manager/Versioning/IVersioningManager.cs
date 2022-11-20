namespace OData.Client.Manager.Versioning
{
    public interface IVersioningManager
    {
        /// <summary>
        /// Gets or sets the action delegate that will be executed to write trace messages.
        /// </summary>
        Action<string>? OnTrace { get; set; }

        /// <summary>
        /// Applies the api version to the given http request message.
        /// </summary>
        /// <param name="requestMessage">The http request message.</param>
        /// <returns>Indicates whether the version was successfully applied or not.</returns>
        bool ApplyVersion(HttpRequestMessage requestMessage);
    }
}
