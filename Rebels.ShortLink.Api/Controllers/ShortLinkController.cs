using Microsoft.AspNetCore.Mvc;
using Rebels.ShortLink.Api.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Rebels.ShortLink.Api.Controllers;

[ApiController]
[Route("/")]
public class ShortLinkController : ControllerBase
{
    private readonly IShortLinkService _shortLinkService;
    private readonly ILogger<ShortLinkController> _logger;

    public ShortLinkController(IShortLinkService shortLinkService, ILogger<ShortLinkController> logger)
    {
        _shortLinkService = shortLinkService;
        _logger = logger;
    }

    /// <summary>
    /// Encodes a URL to a shortened URL.
    /// </summary>
    /// <param name="request">The URL to be shortened.</param>
    /// <returns>A shortened URL.</returns>
    [HttpPost("encode")]
    [ProducesResponseType(typeof(EncodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Encode([FromBody] EncodeRequest request) // Prefer to use ActionResult<T> instead of IActionResult. For instance, ActionResult<EncodeResponse> instead of IActionResult
    {
        if (string.IsNullOrEmpty(request.Url))
        {
            _logger.LogWarning("Encode request failed: Invalid URL");
            return BadRequest("Invalid URL");
        }

        try
        {
            var response = _shortLinkService.EncodeUrl(request.Url);

            return Ok(new EncodeResponse(response.Id, response.ShortUrl));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while encoding URL");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Decodes the id of a shortened URL to its original URL.
    /// </summary>
    /// <param name="id">The id of the shortened URL.</param>
    /// <returns>The original URL.</returns>
    [HttpGet("decode/{id}")]
    [ProducesResponseType(typeof(DecodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Decode(string id) // Prefer to use ActionResult<T> instead of IActionResult. For instance, ActionResult<DecodeResponse> instead of IActionResult
    {
        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("Decode request failed: Invalid id");
            return BadRequest("Invalid id");
        }

        try
        {
            var longUrl = _shortLinkService.DecodeUrlById(id);
            if (!string.IsNullOrEmpty(longUrl))
            {
                return Ok(new DecodeResponse(longUrl));
            }

            _logger.LogWarning("Decode request failed: Short URL not found for id {Id}", id);
            return NotFound("Short URL not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while decoding URL for id {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Redirects to the original URL based on the shortened URL id.
    /// </summary>
    /// <param name="id">The id of the shortened URL.</param>
    /// <returns>A redirection to the original URL.</returns>
    [HttpGet("RedirectToOriginalUrl/{shortUrl}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RedirectToOriginalUrl(string shortUrl)
    {
        if (string.IsNullOrEmpty(shortUrl))
        {
            _logger.LogWarning("Redirect request failed: Invalid shortUrl");
            return BadRequest("Invalid shortUrl");
        }

        try
        {
            var unescapeShortUrl = Uri.UnescapeDataString(shortUrl);
            var longUrl = _shortLinkService.DecodeUrlByShortUrl(unescapeShortUrl);
            if (!string.IsNullOrEmpty(longUrl))
            {
                // Open the default web browser and navigate to the long URL
                Process.Start(new ProcessStartInfo
                {
                    FileName = longUrl,
                    UseShellExecute = true
                });

                return Ok(longUrl);
            }

            _logger.LogWarning("Redirect request failed: Short URL not found '{ShortUrl}'", shortUrl);
            return NotFound($"Short URL not found for {shortUrl}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while redirecting URL for id {ShortUrl}", shortUrl);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }
}

// Do whatever you want with the items below, as long as the API's interface remains the same
public record struct EncodeRequest([Required, Url] string Url);
public record struct EncodeResponse(string Id, string ShortLink);
public record struct DecodeResponse(string OriginalUrl);