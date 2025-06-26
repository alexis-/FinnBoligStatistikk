namespace FinnStatistikk.DiscoveryTool.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json.Linq;

public class DiscoveryWorker
{
  #region Properties & Fields - Non-Public

  private readonly IFinnApiClient           _finnApiClient;
  private readonly DiscoveryEngine          _discoveryEngine;
  private readonly DiscoveryRegistry        _registry;
  private readonly ProgressManager          _progressManager;
  private readonly DiscoverySettings        _settings;
  private readonly ILogger<DiscoveryWorker> _logger;

  #endregion

  #region Constructors

  public DiscoveryWorker(IFinnApiClient              finnApiClient,
                         DiscoveryEngine             discoveryEngine,
                         DiscoveryRegistry           registry,
                         ProgressManager             progressManager,
                         IOptions<DiscoverySettings> settings,
                         ILogger<DiscoveryWorker>    logger)
  {
    _finnApiClient   = finnApiClient;
    _discoveryEngine = discoveryEngine;
    _registry        = registry;
    _progressManager = progressManager;
    _settings        = settings.Value;
    _logger          = logger;
  }

  #endregion

  #region Methods

  public async Task RunAsync(ScrapingProgress? initialProgress, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting Discovery Worker...");

    var markets = _settings.MarketsToScan;
    for (int marketIndex = 0; marketIndex < markets.Length; marketIndex++)
    {
      if (cancellationToken.IsCancellationRequested) break;

      var marketConfig = markets[marketIndex];

      // --- RESUME LOGIC: Skip entire markets that are already completed ---
      if (initialProgress != null && marketIndex < initialProgress.CurrentMarketIndex)
      {
        _logger.LogInformation("Skipping previously completed market [{MarketIndex}] {SearchKey}", marketIndex, marketConfig.SearchKey);
        continue;
      }

      _logger.LogInformation("--- Scanning Market: {SearchKey} ---", marketConfig.SearchKey);

      for (int page = 1; page <= _settings.PagesPerMarket; page++)
      {
        if (cancellationToken.IsCancellationRequested) break;

        // --- RESUME LOGIC: Skip pages in the current market that were already processed ---
        if (initialProgress != null && marketIndex == initialProgress.CurrentMarketIndex && page <= initialProgress.CurrentPageIndex)
        {
          _logger.LogInformation("Skipping previously scanned page {Page} in market {SearchKey}", page, marketConfig.SearchKey);
          continue;
        }

        _logger.LogInformation("Scanning page {Page}/{TotalPages} for market {MarketKey}...", page, _settings.PagesPerMarket,
                               marketConfig.SearchKey);

        var searchResultNode = await _finnApiClient.SearchAsync(marketConfig.SearchKey, page);

        if (searchResultNode == null)
        {
          _logger.LogWarning("Search result was null for {SearchKey} page {Page}, skipping page.", marketConfig.SearchKey, page);
          continue;
        }

        var sourceUrl = $"search/{marketConfig.SearchKey}?page={page}";

        // Process the search result itself for structural changes
        await _discoveryEngine.ProcessResponseAsync(searchResultNode, sourceUrl);

        var docs = searchResultNode["docs"] as JArray;
        if (docs == null)
        {
          _logger.LogWarning("No 'docs' array in search result, skipping ad processing for this page.");
        }

        else
        {
          _logger.LogInformation("Found {DocCount} ads on page.", docs.Count);
          foreach (var doc in docs)
          {
            if (cancellationToken.IsCancellationRequested) break;

            var adId = doc["ad_id"]?.Value<long?>()?.ToString();
            if (string.IsNullOrEmpty(adId)) continue;

            _logger.LogInformation("  -> Processing Ad ID: {AdId}", adId);

            var adViewNode = await _finnApiClient.GetAdViewAsync(adId);
            if (adViewNode != null)
              await _discoveryEngine.ProcessResponseAsync(adViewNode, $"adview/{adId}");
          }
        }


        // --- PROGRESSIVE SAVING ---
        if (cancellationToken.IsCancellationRequested) break;

        var currentProgress = new ScrapingProgress { CurrentMarketIndex = marketIndex, CurrentPageIndex = page };
        await _progressManager.SaveProgressAsync(currentProgress);
        await _registry.SaveAsync();
      }

      // We've finished a market; clear initialProgress so its page-skipping logic doesn't affect the next market.
      initialProgress = null;
    }

    if (!cancellationToken.IsCancellationRequested)
    {
      _logger.LogInformation("Discovery run finished successfully. Cleaning up progress file.");
      _progressManager.DeleteProgress();
    }
    else
    {
      _logger.LogInformation("Discovery run was cancelled. Progress has been saved for a future run.");
    }

    _logger.LogInformation("Discovery run finished.");
  }

  #endregion
}
