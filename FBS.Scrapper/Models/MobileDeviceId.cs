// ReSharper disable ArrangeModifiersOrder

namespace FBS.Scrapper.Models
{
  using System.Diagnostics.CodeAnalysis;

  public class MobileDeviceId
  {
    #region Constructors

    public MobileDeviceId() { }

    [SetsRequiredMembers]
    public MobileDeviceId(int androidVersion, string model, string build, string finnVersion, string finnVersionCode)
    {
      Build           = build;
      AndroidVersion  = androidVersion;
      Model           = model;
      FinnVersion     = finnVersion;
      FinnVersionCode = finnVersionCode;
    }

    #endregion

    #region Properties & Fields - Public

    public required string Build           { get; init; }
    public required string Model           { get; init; }
    public required int    AndroidVersion  { get; init; }
    public required string FinnVersion     { get; init; }
    public required string FinnVersionCode { get; init; }

    #endregion
  }
}
