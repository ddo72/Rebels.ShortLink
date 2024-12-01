using System.Collections.Concurrent;
using System.Text;

namespace Rebels.ShortLink.Api.Services
{
    public class ShortLinkService : IShortLinkService
    {
        private static readonly ConcurrentDictionary<string, string> UrlMappings = new ConcurrentDictionary<string, string>();
        private const string BaseUrl = "http://sh.ort/";
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random Random = new Random();
        private const int pathLength = 6;

        public (string Id, string ShortUrl) EncodeUrl(string longUrl)
        {
            if (string.IsNullOrEmpty(longUrl))
            {
                throw new ArgumentNullException(nameof(longUrl));
            }

            var existingEntry = GetEncodedUrl(longUrl);
            if (existingEntry != null)
            {
                return existingEntry.Value;
            }

            string id;
            do
            {
                id = GeneratePathId();
            } while (UrlMappings.ContainsKey(id));

            UrlMappings[id] = longUrl;

            return (id, BaseUrl + id);
        }

        public string? DecodeUrlById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (UrlMappings.TryGetValue(id, out string? longUrl))
            {
                return longUrl;
            }

            return null;
        }

        public string? DecodeUrlByShortUrl(string shortUrl)
        {
            if (string.IsNullOrEmpty(shortUrl))
            {
                throw new ArgumentNullException(nameof(shortUrl));
            }

            var id = shortUrl.Replace(BaseUrl, string.Empty);
            if (UrlMappings.TryGetValue(id, out string? longUrl))
            {
                return longUrl;
            }

            return null;
        }

        #region Private Methods
        private (string Id, string ShortUrl)? GetEncodedUrl(string longUrl)
        {
            var existingEntry = UrlMappings.FirstOrDefault(x => x.Value == longUrl);

            if (!existingEntry.Equals(default(KeyValuePair<string, string>)))
            {
                return (existingEntry.Key, BaseUrl + existingEntry.Key);
            }

            return null;
        }

        private string GeneratePathId()
        {
            var pathId = new StringBuilder(pathLength);

            for (int i = 0; i < pathLength; i++)
            {
                pathId.Append(Chars[Random.Next(Chars.Length)]);
            }

            return pathId.ToString();
        }
        #endregion
    }
}
