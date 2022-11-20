using OData.Client.Manager.Versioning;
using Xunit;

namespace OData.Client.Manager.Tests.Versioning
{
    public class QueryParamVersioningManagerTests
    {
        [Theory]
        [InlineData("http://domain.com", "v", "1.1", "http://domain.com/?v=1.1", "The already existing query parameter v gets overridden")]
        [InlineData("http://domain.com", "v", "", "http://domain.com/", null)]
        [InlineData("http://domain.com", "v", " ", "http://domain.com/", null)]
        [InlineData("http://domain.com", "v", null, "http://domain.com/", null)]
        public void InstantiateAndApplyWithValidValues_Success(string uriString, string paramName, string version, string expectedUri, string errorMessage)
        {
            string error = null;
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString));
            var versioningManager = new QueryParamVersioningManager(version, paramName);
            versioningManager.OnTrace += (msg) => error = msg;

            versioningManager.ApplyVersion(requestMessage);
            versioningManager.ApplyVersion(requestMessage);

            Assert.Equal(errorMessage, error);
            Assert.Equal(expectedUri, requestMessage.RequestUri.ToString());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void InstantiateWithInvalidValues_Exception(string paramName)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new QueryParamVersioningManager(null, paramName));
            Assert.Equal("parameterName", ex.ParamName);
        }

        [Fact]
        public void ApplyVersionOnNullObject_Exception()
        {
            var versioningManager = new QueryParamVersioningManager("1.0");

            var ex = Assert.Throws<ArgumentNullException>(() => versioningManager.ApplyVersion(null));
            Assert.Equal("requestMessage", ex.ParamName);
        }
    }
}
