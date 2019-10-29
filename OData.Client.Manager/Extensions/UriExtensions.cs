using System;

namespace OData.Client.Manager.Extensions
{
    internal static class UriExtensions
    {
        public static Uri AddParameter(this Uri uri, string? name, string? value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return uri;
            }

            var builder = new UriBuilder(uri);
            var queryString = name + '=' + value;
            if (builder.Query == null || builder.Query.Length <= 1)
            {
                builder.Query = queryString;
            }
            else if (!builder.Query.Contains(name))
            {
                builder.Query = builder.Query.Substring(1) + '&' + queryString;
            }

            return builder.Uri;
        }
    }
}
