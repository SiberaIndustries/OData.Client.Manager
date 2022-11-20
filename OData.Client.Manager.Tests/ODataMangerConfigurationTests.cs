using Xunit;

namespace OData.Client.Manager.Tests
{
    public class ODataMangerConfigurationTests
    {
        [Fact]
        public void Initialize_Configuration_DefaultCtor()
        {
            var config = new ODataManagerConfiguration();

            Assert.True(config.UseAbsoluteReferenceUris);
            Assert.Null(config.BaseUri);
            Assert.Null(config.HttpClient);
            Assert.Null(config.Authenticator);
            Assert.Null(config.VersioningManager);
        }

        [Fact]
        public void Initialize_Configuration_HttpClientCtor()
        {
            var config = new ODataManagerConfiguration(new HttpClient { BaseAddress = new Uri("https://localhost:5000") });

            Assert.True(config.UseAbsoluteReferenceUris);
            Assert.NotNull(config.BaseUri);
            Assert.NotNull(config.HttpClient);
            Assert.Null(config.Authenticator);
            Assert.Null(config.VersioningManager);

            Assert.Equal("https://localhost:5000/", config.BaseUri.ToString());
            Assert.Equal(config.BaseUri.ToString(), config.HttpClient.BaseAddress.ToString());
        }

        [Fact]
        public void Initialize_Configuration_UriClientCtor()
        {
            var config = new ODataManagerConfiguration(new Uri("https://localhost:5000"));

            Assert.True(config.UseAbsoluteReferenceUris);
            Assert.NotNull(config.BaseUri);
            Assert.Null(config.HttpClient);
            Assert.Null(config.Authenticator);
            Assert.Null(config.VersioningManager);

            Assert.Equal("https://localhost:5000/", config.BaseUri.ToString());
        }

        [Fact]
        public void CtorWithOnlyOneApiEndpoint_Exception()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = new ODataManagerConfiguration(new HttpClient { BaseAddress = new Uri("http://localhost") })
                {
                    BaseUri = new Uri("http://localhost")
                };
            });

            Assert.Equal("Unable to set BaseUri when BaseAddress is specified on HttpClient.", ex.Message);
        }
    }
}
