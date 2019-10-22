using OData.Client.Manager.Extensions;
using System;
using Xunit;

namespace OData.Client.Manager.Tests
{
    public class UriExtensionsTests
    {
        [Theory]
        [InlineData("http://domain.com", "foo", "bar", "http://domain.com/?foo=bar")]
        [InlineData("http://domain.com", "foo", "better bar", "http://domain.com/?foo=better bar")]
        [InlineData("http://domain.com?test=value", "foo", "bar", "http://domain.com/?test=value&foo=bar")]
        [InlineData("http://domain.com", "foo", null, "http://domain.com/")]
        [InlineData("http://domain.com", "foo", "", "http://domain.com/")]
        [InlineData("http://domain.com", "foo", " ", "http://domain.com/")]
        [InlineData("http://domain.com?test=value", "foo", null, "http://domain.com/?test=value")]
        [InlineData("http://domain.com?test=value", "foo", "", "http://domain.com/?test=value")]
        [InlineData("http://domain.com?test=value", "foo", " ", "http://domain.com/?test=value")]
        public void AddParameter_ValidValues_Success(string adress, string name, string value, string expectedAdress)
        {
            // Arrange
            var request = new Uri(adress);

            // Act
            var newRequest = request.AddParameter(name, value);

            // Assert
            Assert.Equal(expectedAdress, newRequest.ToString());
        }

        [Fact]
        public void AddParameter_NullEmptyWhitespaceArguments_ArgumentNullException()
        {
            // Arrange
            Uri uri = null;
            ArgumentNullException ex;

            // Act / Assert
            ex = Assert.Throws<ArgumentNullException>(() => uri.AddParameter("foo", "bar"));
            Assert.Equal(nameof(uri), ex.ParamName);

            uri = new Uri("http://domain.com");
            ex = Assert.Throws<ArgumentNullException>(() => uri.AddParameter(null, "bar"));
            Assert.Equal("name", ex.ParamName);

            ex = Assert.Throws<ArgumentNullException>(() => uri.AddParameter(string.Empty, "bar"));
            Assert.Equal("name", ex.ParamName);

            ex = Assert.Throws<ArgumentNullException>(() => uri.AddParameter(" ", "bar"));
            Assert.Equal("name", ex.ParamName);
        }
    }
}
