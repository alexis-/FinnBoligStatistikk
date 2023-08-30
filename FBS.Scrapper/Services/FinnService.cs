// ReSharper disable CommentTypo

namespace FBS.Scrapper.Services
{
  using System.Diagnostics.CodeAnalysis;
  using System.Security.Cryptography;
  using System.Text;
  using Models;
  using Models.Config;
  using Throw;
  using Utilities;

  /// <summary>Service used to mimic Finn's mobile application's behaviour</summary>
  public class FinnService
  {
    #region Constants & Statics

    private const char CR = '\r';

    #endregion

    #region Properties & Fields - Non-Public

    private readonly FinnConfig       _finnConfig;
    private readonly HttpRequestQueue _httpRequestQueue;
    private readonly byte[]           _hmacKey;

    #endregion

    #region Constructors

    public FinnService(FinnConfig finnConfig, HttpRequestQueue httpRequestQueue)
    {
      _finnConfig       = finnConfig;
      _httpRequestQueue = httpRequestQueue;

      _hmacKey = Decode(_finnConfig.HmacKeyObfuscated);
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Generates a new HTTP request targetting Finn's search service for market
    ///   <paramref name="market" /> and page <paramref name="page" />. When
    ///   <paramref name="enqueue" /> is true, request is added to the <see cref="HttpRequestQueue" />
    ///   singleton.
    /// </summary>
    /// <param name="market"></param>
    /// <param name="page"></param>
    /// <param name="enqueue"></param>
    /// <returns></returns>
    public HttpRequestMessage Search(FinnMarket market, int page, bool enqueue = true)
    {
      var url     = GetSearchUrl(market, page);
      var httpReq = new FinnHttpRequestMessage(_finnConfig, HttpMethod.Get, url, FinnRequestType.Search);

      if (enqueue)
        _httpRequestQueue.Enqueue(httpReq);

      return httpReq;
    }

    /// <summary>
    ///   Generates a new HTTP request targetting Finn's ad viewing service for ad id
    ///   <paramref name="adId" />. When <paramref name="enqueue" /> is true, request is added to the
    ///   <see cref="HttpRequestQueue" /> singleton.
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="enqueue"></param>
    /// <returns></returns>
    public HttpRequestMessage ViewAd(int adId, bool enqueue = true)
    {
      var url     = GetAdUrl(adId);
      var httpReq = new FinnHttpRequestMessage(_finnConfig, HttpMethod.Get, url, FinnRequestType.ViewAd);

      if (enqueue)
        _httpRequestQueue.Enqueue(httpReq);

      return httpReq;
    }

    /// <summary>
    ///   Computes Finn's API search url for market <paramref name="market" /> and page
    ///   <paramref name="page" />.
    /// </summary>
    /// <param name="market"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public string GetSearchUrl(FinnMarket market, int page)
    {
      return _finnConfig.SearchApiUrl
                        .Replace(Const.Scrapper.Market, market.Name)
                        .Replace(Const.Scrapper.Page, page.ToString());
    }

    /// <summary>Computes Finn's API ad viewing url for ad id <paramref name="adId" />.</summary>
    /// <param name="adId"></param>
    /// <returns></returns>
    public string GetAdUrl(int adId)
    {
      return _finnConfig.AdViewApiUrl
                        .Replace(Const.Scrapper.Id, adId.ToString());
    }

    /// <summary>
    ///   Computes the gateway HMAC SHA-512 secret key used by the mobile app to prevent
    ///   unauthorized use of the API.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<string> CalculateHmacToken(HttpRequestMessage request)
    {
      var secret      = await CalculateSecretToken(request).ConfigureAwait(false);
      var secretBytes = Encoding.UTF8.GetBytes(secret);

      using (var hashAlgorithm = new HMACSHA512(_hmacKey))
      {
        // Compute hash
        byte[] hash = hashAlgorithm.ComputeHash(secretBytes);

        // Convert result to hexadecimal string
        return Convert.ToBase64String(hash);
      }
    }

    /// <summary>
    ///   Generates the secret token that gets encrypted with HMAC SHA-512 based on the request
    ///   parameters. Example: "GET;/adview/277158255;NAM2;"
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<string> CalculateSecretToken(HttpRequestMessage request)
    {
      request.RequestUri.ThrowIfNull();

      var method = request.Method.Method.ToUpperInvariant();

      var segments = request.RequestUri.Segments.Where(
        s => string.IsNullOrWhiteSpace(s) == false && s.Length > 0);
      var path = CreatePath(segments);

      var encodedQuery = request.RequestUri.Query.Length < 1
        ? string.Empty
        : request.RequestUri.Query;

      var gateway = GetGateway(request);
      var body    = await request.GetBody().ConfigureAwait(false);

      return $"${method};{path}{encodedQuery};{gateway};{body}";
    }

    /// <summary>
    ///   Returns the requested Gateway service (see
    ///   <see cref="FinnConfig.HeaderGatewayServiceKey" />) if it exists in
    ///   <paramref name="request" />'s headers.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    private string GetGateway(HttpRequestMessage request)
    {
      request.Headers.TryGetValues(_finnConfig.HeaderGatewayServiceKey, out var gateways);

      if (gateways is null)
        return string.Empty;

      if (gateways.Count() > 1)
        throw new InvalidOperationException(
          $"HTTP request contains more than a single ${_finnConfig.HeaderGatewayServiceKey}: {string.Join(", ", gateways)}");

      return gateways.First();
    }

    /// <summary>Creates a path from the given <paramref name="segments" />, with a leading '/'.</summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    private string CreatePath(IEnumerable<string> segments)
    {
      var sb = new StringBuilder();

      foreach (var segment in segments)
      {
        sb.Append('/');
        sb.Append(segment);
      }

      return sb.ToString();
    }

    /// <summary>Deobfuscate and converts from Base 64 the given <paramref name="value" />.</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private byte[] Decode(string value)
    {
      var base64EncodedBytes = Convert.FromBase64String(ToggleObfuscate(value));

      return base64EncodedBytes;
    }

    /// <summary>
    ///   Implements Finn's mobile application obfuscation scheme. Converts from obfuscated to
    ///   deobfuscated and vice-versa depending on the input <paramref name="value" />.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "PatternIsRedundant")]
    private string ToggleObfuscate(string value)
    {
      var sb         = new StringBuilder();
      var hashLength = value.Length;

      for (int i = 0; i < hashLength; i++)
      {
        var c = value[i];

        c = c switch
        {
          >= 'a' and <= 'm' or >= 'A' and <= 'M' => (char)(c + CR),
          >= 'n' and <= 'z' or >= 'N' and <= 'Z' => (char)(c - CR),
          _ => c
        };

        sb.Append(c);
      }

      return sb.ToString();
    }

    #endregion
  }
}
