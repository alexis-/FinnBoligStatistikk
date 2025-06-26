namespace FinnStatistikk.DiscoveryTool.Services;

using Models;

public class UserIdentityService
{
  #region Properties & Fields - Non-Public

  private readonly List<UserIdentity> _identities = new();
  private readonly Random             _random     = new();

  #endregion

  #region Constructors

  public UserIdentityService()
  {
    // Populate with a default set of device profiles.
    _identities.Add(new UserIdentity("Pixel 5", "TQ3A.230605.011", 12, "230707-6e39e03", "1003572197"));
    _identities.Add(new UserIdentity("SM-A536B/DS", "TP1A.220624.014", 13, "230707-6e39e03", "1003572197"));
    _identities.Add(new UserIdentity("Pixel 6", "TQ3A.230605.010", 12, "230707-6e39e03", "1003572197"));
  }

  #endregion

  #region Methods

  public UserIdentity GetRandomIdentity()
  {
    return _identities[_random.Next(_identities.Count)];
  }

  #endregion
}
