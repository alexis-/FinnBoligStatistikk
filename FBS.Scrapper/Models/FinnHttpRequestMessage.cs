namespace FBS.Scrapper.Models
{
  using Config;

  /// <summary>
  ///   Child class of <see cref="HttpRequestMessage" /> which adds functionality to help use
  ///   Finn's API.
  /// </summary>
  public class FinnHttpRequestMessage : HttpRequestMessage
  {
    #region Constructors

    /// <inheritdoc />
    public FinnHttpRequestMessage(
      FinnConfig      finnConfig,
      HttpMethod      method,
      string          uri,
      FinnRequestType requestType) : base(method, uri)
    {
      RequestType = requestType;

      switch (requestType)
      {
        case FinnRequestType.Search:
          Headers.Add(finnConfig.HeaderGatewayServiceKey, Const.Finn.GatewayServices.SearchQuest);
          break;

        case FinnRequestType.ViewAd:
          Headers.Add(finnConfig.HeaderGatewayServiceKey, Const.Finn.GatewayServices.NAM2);
          break;

        default: throw new NotImplementedException($"{nameof(FinnRequestType)} '{requestType}' is not implemented.");
      }
    }

    #endregion

    #region Properties & Fields - Public

    /// <summary>What type of page is being requested.</summary>
    public FinnRequestType RequestType { get; }

    #endregion
  }
}
