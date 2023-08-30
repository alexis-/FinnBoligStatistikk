namespace FBS.Scrapper.Utilities
{
  using Models;
  using Models.Config;

  public static class FinnUserIdentityEx
  {
    #region Methods

    /// <summary>
    ///   Checks whether the session has expired (see
    ///   <see cref="FinnConfig.SessionIdRenewDelay" /> ) and generates a new session GUID if
    ///   necessary. Otherwise updates the session last usage timestamp to keep it valid for another
    ///   full period.
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="finnConfig"></param>
    /// <returns></returns>
    public static Guid RenewSessionId(this FinnUserIdentity identity, FinnConfig finnConfig)
    {
      if (identity.SessionIdRenewedOn + TimeSpan.FromMinutes(finnConfig.SessionIdRenewDelay) > DateTime.UtcNow)
        identity.SessionId = Guid.NewGuid();

      identity.SessionIdRenewedOn = DateTime.UtcNow;

      return identity.SessionId;
    }

    /// <summary>
    ///   Generates the Finn Mobile application's user agent based on the provided
    ///   <paramref name="identity" />.
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="finnConfig"></param>
    /// <returns></returns>
    public static string GetUserAgent(this FinnUserIdentity identity, FinnConfig finnConfig)
    {
      return finnConfig.HeaderUserAgentValue
                       .Replace(FinnConfig.UA_ANDROID_VERSION, identity.AndroidVersion.ToString())
                       .Replace(FinnConfig.UA_DEVICE_BUILD, identity.Build)
                       .Replace(FinnConfig.UA_DEVICE_MODEL, identity.Model)
                       .Replace(FinnConfig.UA_FINN_VERSION, identity.FinnVersion);
    }

    /// <summary>Randomly selects an identity from <paramref name="identities" />.</summary>
    /// <param name="identities"></param>
    /// <returns></returns>
    public static FinnUserIdentity GetRandomIdentity(this List<FinnUserIdentity> identities)
    {
      return identities[RandomInt(identities.Count)];
    }

    /// <summary>
    ///   Checks whether <see cref="ScraperGeneratedData.UserIdentities" /> has the required
    ///   number of identities specified by <see cref="ScraperConfig.NumberOfIdentities" /> and
    ///   updates it accordingly. If <see cref="ScraperConfig.ForceRecreateIdentities" /> is true,
    ///   clears the existing identities and recreates new ones from scratch.
    /// </summary>
    /// <param name="scraperGenData"></param>
    /// <param name="scraperConfig"></param>
    /// <returns></returns>
    public static bool CreateIdentitiesIfRequired(this ScraperGeneratedData scraperGenData, ScraperConfig scraperConfig)
    {
      var identities = scraperGenData.UserIdentities;

      if (scraperConfig.ForceRecreateIdentities)
        identities.Clear();

      if (identities.Count == scraperConfig.NumberOfIdentities)
        return false;

      if (identities.Count > scraperConfig.NumberOfIdentities)
        for (int i = identities.Count; i < scraperConfig.NumberOfIdentities; i--)
          identities.RemoveAt(i);

      else
        for (int i = identities.Count; i < scraperConfig.NumberOfIdentities; i++)
          identities.Add(CreateIdentity(scraperConfig));

      return true;
    }

    /// <summary>
    ///   Creates a new random identity based on one of the mobile device templates (see
    ///   <see cref="ScraperConfig.MobileDevices" />.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static FinnUserIdentity CreateIdentity(ScraperConfig config)
    {
      var device = config.MobileDevices.ElementAt(RandomInt(config.MobileDevices.Count));

      return new FinnUserIdentity(device, Guid.NewGuid());
    }

    /// <summary>Returns a random integer between 0 and <paramref name="maxExclusive" />.</summary>
    /// <param name="maxExclusive"></param>
    /// <returns></returns>
    private static int RandomInt(int maxExclusive)
    {
      return Random.Shared.Next(0, maxExclusive);
    }

    #endregion
  }
}
