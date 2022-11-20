using Extensions.Dictionary;
using FakeItEasy;
using OData.Client.Manager.Authenticators;
using OData.Client.Manager.Versioning;
using Simple.OData.Client;
using Xunit;
using Xunit.Abstractions;

namespace OData.Client.Manager.Tests
{
    public class ODataManagerTests
    {
        private const string BaseAdress = "https://services.odata.org/V4/OData/OData.svc/";
        private static readonly Uri BaseUri = new Uri(BaseAdress);
        private readonly ITestOutputHelper output;

        public ODataManagerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CtorWithNullValues_Exception()
        {
            var ex1 = Assert.Throws<ArgumentNullException>(() => new ODataManager(configuration: null));
            Assert.Equal("configuration", ex1.ParamName);

            var ex2 = Assert.Throws<InvalidOperationException>(() => new ODataManager(apiEndpoint: null));
            Assert.Equal("Unable to create client session with no URI specified.", ex2.Message);
        }

        [Fact]
        public void ClientIsNeverNull_True()
        {
            var manager = new ODataManager(BaseUri);
            Assert.NotNull(manager.Client);
        }

        [Fact]
        public async Task UseODataClientAndOnTraceAndConverter_Success()
        {
            var trace = default(string);
            var config = new ODataManagerConfiguration(BaseUri)
            {
                Authenticator = new BasicAuthenticator("user", "pw"),
                VersioningManager = new QueryParamVersioningManager("1.0")
            };
            config.TypeCache.Converter.RegisterTypeConverter<Product>((IDictionary<string, object> dict) => dict.ToInstance<Product>());
            config.OnTrace = (format, args) => trace = string.Format(format, args);
            var manager = new ODataManager(config);

            trace = null;
            var dx = ODataDynamic.Expression;
            IEnumerable<dynamic> entities = await manager.Client
                .For(dx.Products)
                .FindEntriesAsync();

            Assert.NotEmpty(trace);
            Assert.Equal(11, entities.ToList().Count);

            trace = null;
            var entities2 = await manager.Client
                .For<Product>("Products")
                .FindEntriesAsync();

            Assert.NotEmpty(trace);
            Assert.Equal(11, entities2.ToList().Count);
        }

        [Fact]
        public async Task AuthenticateFailed_SuccessfullyTraced()
        {
            var authenticatorMock = A.Fake<IAuthenticator>(x => x.Wrapping(new BasicAuthenticator("user", "pw")));
            A.CallTo(() => authenticatorMock.AuthenticateAsync(A<HttpRequestMessage>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));

            var trace = default(string);
            var config = new ODataManagerConfiguration(BaseUri)
            {
                Authenticator = authenticatorMock
            };
            config.OnTrace += (format, args) => trace += string.Format(format, args);
            config.OnTrace += (format, args) => output.WriteLine(format, args);
            var manager = new ODataManager(config);

            authenticatorMock.OnTrace.Invoke("Triggered trace message in " + nameof(authenticatorMock));
            await manager.Client
                .For<Product>("Products")
                .Top(1)
                .FindEntriesAsync();

            Assert.Contains("Triggered trace message in " + nameof(authenticatorMock), trace);
            Assert.Contains("ODataManager: Authentication not successful", trace);
        }

        [Fact]
        public async Task ApplyVersionFailed_SuccessfullyTraced()
        {
            var versioningManagerMock = A.Fake<IVersioningManager>(x => x.Wrapping(new QueryParamVersioningManager("1.0")));
            A.CallTo(() => versioningManagerMock.ApplyVersion(A<HttpRequestMessage>.Ignored)).Returns(false);

            var trace = default(string);
            var config = new ODataManagerConfiguration(BaseUri)
            {
                VersioningManager = versioningManagerMock
            };
            config.OnTrace += (format, args) => trace += string.Format(format, args);
            config.OnTrace += (format, args) => output.WriteLine(format, args);
            var manager = new ODataManager(config);

            versioningManagerMock.OnTrace.Invoke("Triggered trace message in " + nameof(versioningManagerMock));
            await manager.Client
                .For<Product>("Products")
                .Top(1)
                .FindEntriesAsync();

            Assert.Contains("Triggered trace message in " + nameof(versioningManagerMock), trace);
            Assert.Contains("ODataManager: Applying version not successful", trace);
        }
    }
}
