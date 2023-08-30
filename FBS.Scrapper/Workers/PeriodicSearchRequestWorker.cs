namespace FBS.Scrapper.Workers
{
  using Models;
  using Models.Config;
  using Services;

  public class PeriodicSearchRequestWorker : BackgroundService
  {
    #region Properties & Fields - Non-Public

    private readonly ILogger<PeriodicSearchRequestWorker> _logger;
    private readonly FinnService                          _finnService;
    private readonly ScraperConfig                        _scraperConfig;

    private readonly int _delayBetweenRequests;
    private          int _marketIndex = 0;

    #endregion

    #region Constructors

    public PeriodicSearchRequestWorker(
      ILogger<PeriodicSearchRequestWorker> logger,
      FinnService                          finnService,
      ScraperConfig                        scraperConfig)
    {
      _logger        = logger;
      _finnService   = finnService;
      _scraperConfig = scraperConfig;

      _delayBetweenRequests = CalculateDelayBetweenRequests();
    }

    #endregion

    #region Methods Impl

    /// <summary>
    ///   Cycles through markets which have been configured for monitoring (see
    ///   <see cref="ScraperConfig.SearchMarkets" />), and queues HTTP request to the search service
    ///   each. Requests are queued at intervals <see cref="_delayBetweenRequests" /> which is
    ///   determined by <see cref="CalculateDelayBetweenRequests" />.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
      _logger.LogDebug("Periodic Search Worker - Starting loop");

      while (!ct.IsCancellationRequested)
      {
        _logger.LogTrace("Periodic Search Worker - running at: {time}", DateTimeOffset.Now);

        var httpReq = _finnService.Search(GetNextMarket(), 1);

        _logger.LogDebug("Periodic Search Worker - Enqueuing http request: {method} {url}",
                         httpReq.Method, httpReq.RequestUri!.ToString());

        await Task.Delay(_delayBetweenRequests, ct).ConfigureAwait(false);
      }

      _logger.LogDebug("Periodic Search Worker - Exiting loop");
    }

    #endregion

    #region Methods

    /// <summary>Gets the next market to send a search HTTP request to.</summary>
    /// <returns></returns>
    private FinnMarket GetNextMarket()
    {
      var market = _scraperConfig.SearchMarkets.ElementAt(_marketIndex);
      _marketIndex = (_marketIndex + 1) % _scraperConfig.SearchMarkets.Count;

      return market;
    }

    /// <summary>
    ///   Computes the delay between each HTTP request by diving the requested cycle duration
    ///   (see <see cref="ScraperConfig.SearchMarketsPeriod" />) by the number of requested markets to
    ///   monitor (see <see cref="ScraperConfig.SearchMarkets" />).
    /// </summary>
    /// <returns></returns>
    private int CalculateDelayBetweenRequests()
    {
      return _scraperConfig.SearchMarketsPeriod / _scraperConfig.SearchMarkets.Count;
    }

    #endregion
  }
}
