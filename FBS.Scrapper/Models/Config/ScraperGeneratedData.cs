// ReSharper disable ArrangeModifiersOrder
// ReSharper disable ClassNeverInstantiated.Global

namespace FBS.Scrapper.Models.Config
{
  public class ScraperGeneratedData
  {
    #region Constructors

    public ScraperGeneratedData() { }

    #endregion

    #region Properties & Fields - Public

    /// <summary>A list of generated identities. Loaded at application's startup.</summary>
    public List<FinnUserIdentity> UserIdentities { get; init; } = new();

    #endregion
  }
}
