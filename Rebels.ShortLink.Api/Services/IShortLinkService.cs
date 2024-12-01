namespace Rebels.ShortLink.Api.Services
{
    public interface IShortLinkService
    {
        (string Id, string ShortUrl) EncodeUrl(string longUrl);
        string? DecodeUrlById(string id);
        string? DecodeUrlByShortUrl(string shortUrl);
    }
}
