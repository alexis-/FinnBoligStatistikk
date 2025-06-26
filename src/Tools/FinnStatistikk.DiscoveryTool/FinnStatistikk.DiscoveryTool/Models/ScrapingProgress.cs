namespace FinnStatistikk.DiscoveryTool.Models;

/// <summary>Represents the saved state of the scraping process to allow for resuming.</summary>
public class ScrapingProgress
{
  #region Properties & Fields - Public

  /// <summary>The index of the last market that was being processed.</summary>
  public int CurrentMarketIndex { get; set; }

  /// <summary>The index of the last page that was successfully processed in the current market.</summary>
  public int CurrentPageIndex { get; set; }

  #endregion
}
