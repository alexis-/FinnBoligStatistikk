namespace FBS.Scrapper.Utilities
{
  using Models.Config;
  using Services;

  /// <summary>Intercepts HTTP requests and inserts the headers required by Finn's API.</summary>
  public class HttpRequestHeaderInjector : DelegatingHandler
  {
    #region Properties & Fields - Non-Public

    private readonly FinnConfig           _finnConfig;
    private readonly FinnService          _finnService;
    private readonly ScraperGeneratedData _scraperGenData;

    #endregion

    #region Constructors

    public HttpRequestHeaderInjector(
      HttpMessageHandler   innerHandler,
      FinnConfig           finnConfig,
      FinnService          finnService,
      ScraperGeneratedData scraperGenData)
      : base(innerHandler)
    {
      _finnConfig     = finnConfig;
      _finnService    = finnService;
      _scraperGenData = scraperGenData;
    }

    #endregion

    #region Methods Impl

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      if (request.RequestUri != null && request.RequestUri.Host.EndsWith(_finnConfig.FinnDomainName))
      {
        await InjectHmacTokenHeader(request).ConfigureAwait(false);
        InjectUserIdentity(request);
      }

      return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Methods

    /// <summary>Generate and inserts the HMAC token used by Finn to validate API requests.</summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task InjectHmacTokenHeader(HttpRequestMessage request)
    {
      var token = await _finnService.CalculateHmacToken(request).ConfigureAwait(false);

      request.Headers.Add(_finnConfig.HeaderGatewayKeyKey, token);
    }

    /// <summary>
    ///   Picks a random identity from <see cref="ScraperGeneratedData.UserIdentities" /> and
    ///   populates the headers with the required fields as per Finn's mobile application and Finn's
    ///   API specifications.
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void InjectUserIdentity(HttpRequestMessage request)
    {
      // Make sure there are no cookie
      request.Headers.Remove("Cookie");

      // Get random identity from our identity pool
      var identity = _scraperGenData.UserIdentities.GetRandomIdentity();

      // Session ID
      var sessionId = identity.RenewSessionId(_finnConfig);
      request.Headers.Add(_finnConfig.HeaderSessionIdKey, sessionId.ToString());

      // User agent
      var ua = identity.GetUserAgent(_finnConfig);

      request.Headers.UserAgent.Clear();
      request.Headers.TryAddWithoutValidation("User-Agent", ua);

      if (string.Equals(request.Headers.UserAgent.ToString(), ua, StringComparison.Ordinal) == false)
        throw new InvalidOperationException($"User-Agent is incorrect. Found '{request.Headers.UserAgent}' but should be '{ua}'.");

      // Device/Visitor ID
      request.Headers.Add(_finnConfig.HeaderAbTestDeviceIdKey, identity.VisitorId.ToString());
      request.Headers.Add(_finnConfig.HeaderAppInstallationIdKey, identity.VisitorId.ToString());
      request.Headers.Add(_finnConfig.HeaderVisitorIdKey, identity.VisitorId.ToString());

      // Mobile platform & app build
      request.Headers.Add(_finnConfig.HeaderBuildTypeKey, _finnConfig.HeaderBuildTypeValue);
      request.Headers.Add(_finnConfig.HeaderDeviceInfoKey, _finnConfig.HeaderDeviceInfoValue);
      request.Headers.Add(_finnConfig.HeaderFeatureTogglesKey, _finnConfig.HeaderFeatureTogglesValue);
      request.Headers.Add(_finnConfig.HeaderVersionCodeKey, identity.FinnVersionCode);

      // API version & features
      request.Headers.Add(_finnConfig.HeaderApiVersionKey, _finnConfig.HeaderApiVersionValue);
      request.Headers.Add(_finnConfig.HeaderApiFeatureKey, _finnConfig.HeaderApiFeatureValue);
    }

    #endregion
  }
}
