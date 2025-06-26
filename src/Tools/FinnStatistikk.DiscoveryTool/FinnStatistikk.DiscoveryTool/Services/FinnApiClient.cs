namespace FinnStatistikk.DiscoveryTool.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FinnApiClient : IFinnApiClient
{
  #region Properties & Fields - Non-Public

  private readonly HttpClient             _httpClient;
  private readonly FinnApiSettings        _settings;
  private readonly int                    _requestDelayMs;
  private readonly ILogger<FinnApiClient> _logger;
  private readonly Random                 _random = new(); // Use a single Random instance

  #endregion

  #region Constructors

  public FinnApiClient(HttpClient                  httpClient,
                       IOptions<FinnApiSettings>   settings,
                       IOptions<DiscoverySettings> discoverySettings,
                       ILogger<FinnApiClient>      logger)
  {
    _httpClient     = httpClient;
    _settings       = settings.Value;
    _requestDelayMs = discoverySettings.Value.RequestDelayMs;
    _logger         = logger; // Assign logger
  }

  #endregion

  #region Methods Impl

  public async Task<JObject?> SearchAsync(string searchKey, int page)
  {
    var url = _settings.SearchApiUrl
                       .Replace("{SearchKey}", searchKey)
                       .Replace("{Page}", page.ToString());

    using var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add(_settings.HeaderGatewayService, "Search-Quest");

    return await SendRequestAsync(request);
  }

  public async Task<JObject?> GetAdViewAsync(string adId)
  {
    var url = _settings.AdViewApiUrl.Replace("{Id}", adId);

    using var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add(_settings.HeaderGatewayService, "NAM2");

    return await SendRequestAsync(request);
  }

  #endregion

  #region Methods

  private async Task<JObject?> SendRequestAsync(HttpRequestMessage request)
  {
    try
    {
      // --- JITTER LOGIC ---
      var minDelay      = (int)(_requestDelayMs * 0.80); // 80% of base delay
      var maxDelay      = (int)(_requestDelayMs * 1.20); // 120% of base delay
      var jitteredDelay = _random.Next(minDelay, maxDelay);

      _logger.LogDebug("Applying randomized delay of {JitteredDelay}ms (base: {RequestDelayMs}ms) before request to {RequestUri}",
                       jitteredDelay, _requestDelayMs, request.RequestUri);

      await Task.Delay(jitteredDelay);

      using var response = await _httpClient.SendAsync(request);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("API Error: {StatusCode} for {RequestUri}", response.StatusCode, request.RequestUri);
        return null;
      }

      var contentStream = await response.Content.ReadAsStreamAsync();

      using var streamReader = new StreamReader(contentStream);
      using var jsonReader   = new JsonTextReader(streamReader);
      return await JObject.LoadAsync(jsonReader);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Request failed for {RequestUri}: {ErrorMessage}", request.RequestUri, ex.Message);
      return null;
    }
  }

  #endregion
}
