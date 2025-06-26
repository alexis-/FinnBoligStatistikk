namespace FinnStatistikk.DiscoveryTool.Models;

public class MarketScanConfig
{
  #region Properties & Fields - Public

  public string SearchKey { get; set; } = string.Empty;

  #endregion
}

public class DiscoverySettings
{
  #region Properties & Fields - Public

  public MarketScanConfig[] MarketsToScan  { get; set; } = { };
  public int                PagesPerMarket { get; set; } = 10;
  public int                RequestDelayMs { get; set; } = 1500;

  #endregion
}
