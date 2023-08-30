// ReSharper disable ArrangeModifiersOrder
// ReSharper disable ClassNeverInstantiated.Global

namespace FBS.Scrapper.Models.Config
{
  public class ScraperConfig
  {
    #region Properties & Fields - Public

    /// <summary>
    ///   List of Android Devices to use in the user-agent header. See:
    ///   https://user-agents.net/applications/instagram-app/platforms/android
    /// </summary>
    public IReadOnlyCollection<MobileDeviceId> MobileDevices { get; init; } = new[]
    {
      //new MobileDeviceId(9, "PKQ1.190616.001", "Redmi Note 8", "230707-6e39e03", "1003572197"),
      new MobileDeviceId(10, "QKQ1.190828.002", "M1805E1OA", "230707-6e39e03", "1003572197"),
      new MobileDeviceId(12, "TQ3A.230605.011", "Pixel 5", "230707-6e39e03", "1003572197"),
      new MobileDeviceId(12, "TQ3A.230605.010", "Pixel 6", "230707-6e39e03", "1003572197"),
      new MobileDeviceId(13, "TP1A.220624.014.A536BXXU6CWE9", "SM-A536B/DS", "230707-6e39e03", "1003572197"),
    };

    /// <summary>
    ///   Number of identities to generate and alternate between when sending HTTP requests to
    ///   Finn.
    /// </summary>
    public int NumberOfIdentities { get; init; } = 100;

    /// <summary>
    ///   Toggle to true to erase previous identities and recreate them from scratch. Useful if
    ///   new device identities have been added.
    /// </summary>
    public bool ForceRecreateIdentities { get; init; } = false;

    /// <summary>Delay in milliseconds between each web request. Default: 2500ms</summary>
    public int WebRequestDelay { get; init; } = 2500;

    /// <summary>
    ///   How much randomness in milliseconds to modulate (add and subtract) to
    ///   <see cref="WebRequestDelay" />. Default: 1000ms
    /// </summary>
    public int WebRequestDelayRandomness { get; init; } = 1000;

    /// <summary>Timeout in milliseconds for web requests.</summary>
    public int WebRequestTimeout { get; init; } = 10000;

    /// <summary>Custom headers to use in the http client</summary>
    public IReadOnlyDictionary<string, string> WebRequestHeaders { get; init; } = new Dictionary<string, string>();

    /// <summary>List of all markets that should be searched and scraped</summary>
    public IReadOnlyCollection<FinnMarket> SearchMarkets { get; init; } = new[]
    {
      FinnMarket.RealestateCommon
    };

    /// <summary>
    ///   Period in milliseconds during which all markets search should be executed until the
    ///   next cycle begins. Default: 5 min.
    /// </summary>
    public int SearchMarketsPeriod { get; init; } = 5 * 60 * 1000;

    /// <summary>Delay in milliseconds between each attempt to process a response from the queue</summary>
    public int ResponseProcessingDelay { get; init; } = 200;

    #endregion
  }
}
