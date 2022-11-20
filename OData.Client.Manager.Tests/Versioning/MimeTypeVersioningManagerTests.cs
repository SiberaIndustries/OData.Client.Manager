using OData.Client.Manager.Versioning;
using System.Net.Http.Headers;
using Xunit;

namespace OData.Client.Manager.Tests.Versioning
{
    public class MimeTypeVersioningManagerTests
    {
        [Theory]
        [InlineData("accept", "v", "1.1", "v=1.1")]
        [InlineData("accept", "v=1.2", "", "v=1.2")]
        [InlineData("accept", "v=1.2", " ", "v=1.2")]
        [InlineData("accept", "v=1.2", null, "v=1.2")]
        [InlineData("custom", "v", "1.3", "v=1.3")]
        [InlineData("custom", "v=1.4", "", "v=1.4")]
        [InlineData("custom", "v=1.4", " ", "v=1.4")]
        [InlineData("custom", "v=1.4", null, "v=1.4")]
        public void InstantiateAndApplyWithValidValues_Success(string header, string mime, string version, string expectedValue)
        {
            string error = null;
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://domain.com"));
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var versioningManager = new MimeTypeVersioningManager(version, mime, header);
            versioningManager.OnTrace += (msg) => error = msg;

            versioningManager.ApplyVersion(requestMessage);
            versioningManager.ApplyVersion(requestMessage);

            Assert.NotEmpty(error);
            Assert.True(requestMessage.Headers.TryGetValues(header, out IEnumerable<string> values));
            Assert.Single(values, expectedValue);
        }

        [Theory]
        [InlineData("", "foo", "header")]
        [InlineData(" ", "foo", "header")]
        [InlineData(null, "foo", "header")]
        [InlineData("foo", "", "mimeType")]
        [InlineData("foo", " ", "mimeType")]
        [InlineData("foo", null, "mimeType")]
        public void InstantiateWithInvalidValues_Exception(string header, string mime, string paramName)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new MimeTypeVersioningManager(null, mime, header));
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ApplyVersionOnNullObject_Exception()
        {
            var versioningManager = new MimeTypeVersioningManager("1.0");

            var ex = Assert.Throws<ArgumentNullException>(() => versioningManager.ApplyVersion(null));
            Assert.Equal("requestMessage", ex.ParamName);
        }
    }
}
