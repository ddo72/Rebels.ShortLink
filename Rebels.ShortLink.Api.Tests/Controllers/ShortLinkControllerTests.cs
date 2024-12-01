using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Rebels.ShortLink.Api.Controllers;
using Rebels.ShortLink.Api.Services;

namespace Rebels.ShortLink.Api.Tests.Controllers
{
    public class ShortLinkControllerTests
    {
        private readonly Mock<IShortLinkService> _shortLinkServiceMock;
        private readonly Mock<ILogger<ShortLinkController>> _loggerMock;
        private readonly ShortLinkController _controller;

        public ShortLinkControllerTests()
        {
            _shortLinkServiceMock = new Mock<IShortLinkService>();
            _loggerMock = new Mock<ILogger<ShortLinkController>>();
            _controller = new ShortLinkController(_shortLinkServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Encode_ShouldReturnBadRequest_WhenUrlIsInvalid()
        {
            // Arrange
            var request = new EncodeRequest(string.Empty);

            // Act
            var result = _controller.Encode(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Invalid URL");
        }

        [Fact]
        public void Encode_ShouldReturnOk_WhenUrlIsValid()
        {
            // Arrange
            var request = new EncodeRequest("https://example.com");
            _shortLinkServiceMock.Setup(s => s.EncodeUrl(request.Url)).Returns(("123", "https://short.url/1"));

            // Act
            var result = _controller.Encode(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new EncodeResponse("123", "https://short.url/1"));
        }

        [Fact]
        public void Decode_ShouldReturnNotFound_WhenShortUrlNotFound()
        {
            // Arrange
            var id = "1";
            _shortLinkServiceMock.Setup(s => s.DecodeUrlById(id)).Returns((string?)null);

            // Act
            var result = _controller.Decode(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should().Be("Short URL not found");
        }

        [Fact]
        public void Decode_ShouldReturnOk_WhenShortUrlIsFound()
        {
            // Arrange
            var id = "1";
            var longUrl = "https://example.com";
            _shortLinkServiceMock.Setup(s => s.DecodeUrlById(id)).Returns(longUrl);

            // Act
            var result = _controller.Decode(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new DecodeResponse(longUrl));
        }

        [Fact]
        public void Encode_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new EncodeRequest("https://example.com");
            _shortLinkServiceMock.Setup(s => s.EncodeUrl(request.Url)).Throws(new Exception("Internal Server Error"));

            // Act
            var result = _controller.Encode(request);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void Decode_ShouldReturnBadRequest_WhenIdIsNull()
        {
            // Act
            var result = _controller.Decode(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Invalid id");
        }

        [Fact]
        public void RedirectToOriginalUrl_ReturnsBadRequest_WhenShortUrlIsInvalid()
        {
            // Act
            var result = _controller.RedirectToOriginalUrl(string.Empty);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Invalid shortUrl");
        }

        [Fact]
        public void RedirectToOriginalUrl_ReturnsNotFound_WhenShortUrlNotFound()
        {
            // Arrange
            _shortLinkServiceMock.Setup(s => s.DecodeUrlByShortUrl("123")).Returns((string?)null);

            // Act
            var result = _controller.RedirectToOriginalUrl("123") as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be("Original URL not found for 123");
        }

        [Fact]
        public void RedirectToOriginalUrl_ReturnsRedirect_WhenShortUrlIsFound()
        {
            // Arrange
            _shortLinkServiceMock.Setup(s => s.DecodeUrlByShortUrl("http://sh.ort/123")).Returns("https://example.com");

            // Act
            var result = _controller.RedirectToOriginalUrl("http%3A%2F%2Fsh.ort%2F123");

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new DecodeResponse("https://example.com"));
        }
    }
}
