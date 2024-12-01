using Microsoft.AspNetCore.Mvc;

namespace Rebels.ShortLink.Api.Controllers;

[ApiController]
[Route("/")]
public class ShortLinkController : ControllerBase
{
    /// <summary>
    /// Encodes a URL to a shortened URL.
    /// </summary>
    /// <param name="request">The URL to be shortened.</param>
    /// <returns>A shortened URL.</returns>
    [HttpPost("encode")]
    [ProducesResponseType(typeof(EncodeResponse), StatusCodes.Status200OK)]
    public IActionResult Encode([FromBody] EncodeRequest request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Decodes the id of a shortened URL to its original URL.
    /// </summary>
    /// <param name="id">The id of the shortened URL.</param>
    /// <returns>The original URL.</returns>
    [HttpGet("decode/{id}")]
    [ProducesResponseType(typeof(DecodeResponse), StatusCodes.Status200OK)]
    public IActionResult Decode(string id)
    {
        throw new NotImplementedException();
    }
}

// Do whatever you want with the items below, as long as the API's interface remains the same
public record struct EncodeRequest(string Url);
public record struct EncodeResponse(string Id, string ShortLink);
public record struct DecodeResponse(string OriginalUrl);