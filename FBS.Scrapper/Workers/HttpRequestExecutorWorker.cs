namespace FBS.Scrapper.Workers
{
  using Models;
  using Models.Config;
  using Services;
  using Utilities;

  /// <summary>
  ///   Handles the execution of all HTTP requests. <see cref="FinnHttpRequestMessage" /> are
  ///   dequeued from <see cref="HttpRequestQueue" /> and <see cref="HttpResponseMessage" /> are
  ///   queued in <see cref="HttpResponseQueue" />. Delay between successive HTTP requests is
  ///   controlled by <see cref="ScraperConfig.WebRequestDelay" /> and
  ///   <see cref="ScraperConfig.WebRequestDelayRandomness" />.
  /// </summary>
  public class HttpRequestExecutorWorker : BackgroundService
  {
    #region Properties & Fields - Non-Public

    private readonly ILogger<HttpRequestExecutorWorker> _logger;
    private readonly ScraperConfig                      _scraperConfig;
    private readonly HttpRequestQueue                   _requestQueue;
    private readonly HttpResponseQueue                  _responseQueue;

    private readonly List<HttpClient> _httpClient;

    #endregion

    #region Constructors

    public HttpRequestExecutorWorker(
      ILogger<HttpRequestExecutorWorker> logger,
      ScraperGeneratedData               scraperGenData,
      ScraperConfig                      scraperConfig,
      FinnConfig                         finnConfig,
      FinnService                        finnService,
      HttpRequestQueue                   requestQueue,
      HttpResponseQueue                  responseQueue)
    {
      _logger        = logger;
      _scraperConfig = scraperConfig;
      _requestQueue  = requestQueue;
      _responseQueue = responseQueue;

      _httpClient = HttpEx.CreateHttpClient(finnConfig, finnService, scraperConfig, scraperGenData);
    }

    #endregion

    #region Methods Impl

    /// <summary>Executes the queued HTTP requests.</summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
      _logger.LogDebug("Http Request Executor - Starting loop");

      while (!ct.IsCancellationRequested)
      {
        _logger.LogTrace("Http Request Executor - running at: {time}", DateTimeOffset.Now);

        if (_requestQueue.TryDequeue(out var httpReq))
        {
          _logger.LogDebug("Http Request Executor - {verb} {uri}", httpReq.Method.Method, httpReq.RequestUri!.ToString());

          var httpResp = await _httpClient.SendAsync(httpReq, HttpCompletionOption.ResponseContentRead, ct);

          _logger.LogDebug("Http Request Executor - {status}: {verb} {uri}", httpResp.StatusCode, httpReq.Method.Method,
                           httpReq.RequestUri!.ToString());

          _responseQueue.Enqueue(httpResp);
        }

        await Task.Delay(GetIntervalUntilNextHttpRequest(), ct).ConfigureAwait(false);
      }

      _logger.LogDebug("Http Request Executor - Exiting loop");
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Returns the randomized delay to wait until the next http request should be executed.
    ///   See <see cref="ScraperConfig.WebRequestDelay" /> and
    ///   <see cref="ScraperConfig.WebRequestDelayRandomness" />.
    /// </summary>
    /// <returns>Delay in milliseconds</returns>
    private int GetIntervalUntilNextHttpRequest()
    {
      var min = _scraperConfig.WebRequestDelay;
      var max = _scraperConfig.WebRequestDelay + _scraperConfig.WebRequestDelayRandomness;

      return Random.Shared.Next(min, max);
    }

    #endregion
  }
}
