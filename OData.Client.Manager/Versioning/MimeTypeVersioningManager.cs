namespace OData.Client.Manager.Versioning
{
    public class MimeTypeVersioningManager : IVersioningManager
    {
        private readonly string header;
        private readonly string value;

        public MimeTypeVersioningManager(string version, string mimeType = "v", string header = "accept")
        {
            this.header = string.IsNullOrWhiteSpace(header)
                ? throw new ArgumentNullException(nameof(header))
                : header.Trim();

            var trimmedMimeType = string.IsNullOrWhiteSpace(mimeType)
                ? throw new ArgumentNullException(nameof(mimeType))
                : mimeType.Trim();

            value = string.IsNullOrWhiteSpace(version)
                ? trimmedMimeType
                : trimmedMimeType + '=' + version.Trim();
        }

        public Action<string>? OnTrace { get; set; }

        public bool ApplyVersion(HttpRequestMessage requestMessage)
        {
            _ = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));

            if (requestMessage.Headers.TryGetValues(header, out IEnumerable<string>? values) && values.Contains(value))
            {
                OnTrace?.Invoke($"The already existing mime type and value {value} gets not applied again");
                return true;
            }

            requestMessage.Headers.TryAddWithoutValidation(header, value);
            return true;
        }
    }
}
