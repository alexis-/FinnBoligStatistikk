// ReSharper disable ArrangeModifiersOrder

namespace FBS.Scrapper.Models
{
  using System.Diagnostics.CodeAnalysis;
  using System.Text.Json.Serialization;

  public class FinnUserIdentity
  {
    #region Constructors

    public FinnUserIdentity()
    {
      SessionId          = Guid.NewGuid();
      SessionIdRenewedOn = DateTime.UtcNow;
    }

    [SetsRequiredMembers]
    public FinnUserIdentity(MobileDeviceId deviceId, Guid visitorId)
      : this()
    {
      Build           = deviceId.Build;
      Model           = deviceId.Model;
      AndroidVersion  = deviceId.AndroidVersion;
      FinnVersion     = deviceId.FinnVersion;
      FinnVersionCode = deviceId.FinnVersionCode;
      VisitorId       = visitorId;
    }

    #endregion

    #region Properties & Fields - Public

    public required string Build           { get; init; }
    public required string Model           { get; init; }
    public required int    AndroidVersion  { get; init; }
    public required string FinnVersion     { get; init; }
    public required string FinnVersionCode { get; init; }
    public required Guid   VisitorId       { get; init; }

    [JsonIgnore]
    public required Guid SessionId { get; set; }

    [JsonIgnore]
    public required DateTime SessionIdRenewedOn { get; set; }

    #endregion
  }
}
