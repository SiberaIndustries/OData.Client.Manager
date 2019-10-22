using Extensions.Dictionary;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OData.Client.Manager.Tests
{
    public class ODataManagerTests
    {
        private const string BaseAdress = "https://services.odata.org/V4/OData/OData.svc/";
        private static readonly Uri BaseUri = new Uri(BaseAdress);

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
            ODataManagerConfiguration config = new ODataManagerConfiguration(BaseUri);
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
    }
}
