namespace FBS.Scrapper.Workers
{
  using Models;
  using Models.Config;
  using Models.Json;
  using Newtonsoft.Json;
  using Services;

  /// <summary>
  ///   Processes HTTP responses that have been queued in <see cref="HttpResponseQueue" />,
  ///   eg. search result, ad information, etc.
  /// </summary>
  public class HttpResponseProcessorWorker : BackgroundService
  {
    #region Properties & Fields - Non-Public

    private readonly ILogger<HttpResponseProcessorWorker> _logger;
    private readonly ScraperConfig                        _scraperConfig;
    private readonly HttpResponseQueue                    _responseQueue;

    #endregion

    #region Constructors

    public HttpResponseProcessorWorker(
      ILogger<HttpResponseProcessorWorker> logger,
      ScraperConfig                        scraperConfig,
      HttpResponseQueue                    responseQueue)
    {
      _logger        = logger;
      _scraperConfig = scraperConfig;
      _responseQueue = responseQueue;
    }

    #endregion

    #region Methods Impl

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
      _logger.LogDebug("Response Processing Worker - Starting loop");

      while (!ct.IsCancellationRequested)
      {
        _logger.LogTrace("Response Processing Worker - running at: {time}", DateTimeOffset.Now);

        if (_responseQueue.TryDequeue(out var httpResp))
        {
          var httpReq     = httpResp.RequestMessage!;
          var requestType = ((FinnHttpRequestMessage)httpReq).RequestType;

          _logger.LogDebug("Response Processing Worker - {status} {verb} {uri}", httpResp.StatusCode, httpReq.Method,
                           httpReq.RequestUri!.ToString());

          await ProcessResponse(httpResp, requestType).ConfigureAwait(false);
        }

        await Task.Delay(_scraperConfig.ResponseProcessingDelay, ct).ConfigureAwait(false);
      }

      _logger.LogDebug("Response Processing Worker - Exiting loop");
    }

    #endregion

    #region Methods

    private async Task ProcessResponse(HttpResponseMessage httpResp, FinnRequestType requestType)
    {
      var content = await httpResp.Content.ReadAsStringAsync();

      switch (requestType)
      {
        case FinnRequestType.Search:
          var searchRes = JsonConvert.DeserializeObject<SearchResult>(content);

          await ProcessFinnSearch(httpResp, searchRes);
          break;

        case FinnRequestType.ViewAd:
          var adInfo = JsonConvert.DeserializeObject<object>(content);

          await ProcessFinnAd(httpResp);
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(requestType), $"Invalid {nameof(FinnRequestType)} '{requestType}'.");
      }
    }

    private async Task ProcessFinnSearch(HttpResponseMessage httpResp, SearchResult? searchResult)
    {

    }

    private async Task ProcessFinnAd(HttpResponseMessage httpResp) { }

    #endregion
  }
}
