namespace FinnStatistikk.DiscoveryTool.Models;

public class UserIdentity
{
  #region Constructors

  public UserIdentity(string model, string build, int androidVersion, string finnVersion, string finnVersionCode)
  {
    VisitorId       = Guid.NewGuid();
    Model           = model;
    Build           = build;
    AndroidVersion  = androidVersion;
    FinnVersion     = finnVersion;
    FinnVersionCode = finnVersionCode;
    RenewSessionId();
  }

  #endregion

  #region Properties & Fields - Public

  public Guid   VisitorId       { get; }
  public string Model           { get; }
  public string Build           { get; }
  public int    AndroidVersion  { get; }
  public string FinnVersion     { get; }
  public string FinnVersionCode { get; }

  public Guid     SessionId            { get; private set; }
  public DateTime SessionIdLastRenewed { get; private set; }

  #endregion

  #region Methods

  public void RenewSessionId()
  {
    SessionId            = Guid.NewGuid();
    SessionIdLastRenewed = DateTime.UtcNow;
  }

  #endregion
}
