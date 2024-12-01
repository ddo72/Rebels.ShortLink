using FluentAssertions;
using Rebels.ShortLink.Api.Services;

namespace Rebels.ShortLink.Api.Tests.Services
{
    public class ShortLinkServiceTests
    {
        private readonly ShortLinkService _shortLinkService;
        private const string BaseUrl = "http://sh.ort/";

        public ShortLinkServiceTests()
        {
            _shortLinkService = new ShortLinkService();
        }

        [Fact]
        public void EncodeUrl_ShouldReturnShortUrl()
        {
            // Arrange
            var longUrl = "https://example.com";

            // Act
            var result = _shortLinkService.EncodeUrl(longUrl);

            // Assert
            result.ShortUrl.Should().StartWith(BaseUrl);
            result.Id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void EncodeUrl_ShouldThrowArgumentNullException_WhenLongUrlIsNull()
        {
            // Act
            Action act = () => _shortLinkService.EncodeUrl(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DecodeUrl_ShouldReturnLongUrl()
        {
            // Arrange
            var longUrl = "https://example.com";
            var encodedResult = _shortLinkService.EncodeUrl(longUrl);

            // Act
            var result = _shortLinkService.DecodeUrlById(encodedResult.Id);

            // Assert
            result.Should().Be(longUrl);
        }

        [Fact]
        public void EncodeUrl_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var longUrl = "https://example.com/path?query=param&another=param";

            // Act
            var result = _shortLinkService.EncodeUrl(longUrl);

            // Assert
            result.ShortUrl.Should().StartWith(BaseUrl);
            result.Id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void DecodeUrl_ShouldReturnNull_WhenShortUrlIdDoesNotExist()
        {
            // Act
            var result = _shortLinkService.DecodeUrlById("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DecodeUrlByShortUrl_ShouldReturnLongUrl()
        {
            // Arrange
            var longUrl = "https://example.com";
            var encodedResult = _shortLinkService.EncodeUrl(longUrl);

            // Act
            var result = _shortLinkService.DecodeUrlByShortUrl(encodedResult.ShortUrl);

            // Assert
            result.Should().Be(longUrl);
        }

        [Fact]
        public void DecodeUrlByShortUrl_ShouldReturnNullForInvalidShortUrl()
        {
            // Act
            var result = _shortLinkService.DecodeUrlByShortUrl("http://sh.ort/invalidId");

            // Assert
            result.Should().BeNull();
        }
    }
}
