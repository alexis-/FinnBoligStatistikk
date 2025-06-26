namespace FinnStatistikk.DiscoveryTool.Services;

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

public class SigningDelegatingHandler : DelegatingHandler
{
  #region Properties & Fields - Non-Public

  private readonly FinnApiSettings                   _settings;
  private readonly UserIdentityService               _identityService;
  private readonly byte[]                            _hmacKey;
  private readonly ILogger<SigningDelegatingHandler> _logger; // Add logger field

  #endregion

  #region Constructors

  public SigningDelegatingHandler(IOptions<FinnApiSettings>         settings,
                                  UserIdentityService               identityService,
                                  ILogger<SigningDelegatingHandler> logger)
  {
    _settings        = settings.Value;
    _identityService = identityService;
    _logger          = logger;
    _hmacKey         = DecodeHmacKey(_settings.HmacKeyObfuscated);
  }

  #endregion

  #region Methods Impl

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    // Use the new ApiHost property for the check
    if (request.RequestUri != null && request.RequestUri.Host.Equals(_settings.ApiHost, StringComparison.OrdinalIgnoreCase))
      await InjectHeadersAsync(request, cancellationToken);

    return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
  }

  #endregion

  #region Methods

  private async Task InjectHeadersAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var identity = _identityService.GetRandomIdentity();

    // Inject user identity headers
    request.Headers.UserAgent.Clear();
    request.Headers.TryAddWithoutValidation("User-Agent", GetUserAgent(identity));
    request.Headers.Add(_settings.HeaderSessionId, identity.SessionId.ToString());
    request.Headers.Add(_settings.HeaderVisitorId, identity.VisitorId.ToString());
    request.Headers.Add(_settings.HeaderAbTestDeviceId, identity.VisitorId.ToString());
    request.Headers.Add(_settings.HeaderAppInstallationId, identity.VisitorId.ToString());
    request.Headers.Add(_settings.HeaderVersionCode, identity.FinnVersionCode);
    request.Headers.Add(_settings.HeaderBuildType, _settings.HeaderBuildTypeValue);
    request.Headers.Add(_settings.HeaderDeviceInfo, _settings.HeaderDeviceInfoValue);
    request.Headers.Add(_settings.HeaderApiVersion, _settings.HeaderApiVersionValue);

    // Calculate and inject the HMAC token
    var hmacToken = await CalculateHmacTokenAsync(request, cancellationToken);
    request.Headers.Add(_settings.HeaderGatewayKey, hmacToken);
  }

  private async Task<string> CalculateHmacTokenAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var secret      = await CalculateSecretTokenAsync(request, cancellationToken).ConfigureAwait(false);
    var secretBytes = Encoding.UTF8.GetBytes(secret);

    using var hashAlgorithm = new HMACSHA512(_hmacKey);
    byte[]    hash          = await hashAlgorithm.ComputeHashAsync(new MemoryStream(secretBytes), cancellationToken);

    return Convert.ToBase64String(hash);
  }

  private async Task<string> CalculateSecretTokenAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var method = request.Method.Method.ToUpperInvariant();
    var path   = request.RequestUri!.AbsolutePath;
    var query  = request.RequestUri.Query;

    var gateway = request.Headers.TryGetValues(_settings.HeaderGatewayService, out var values) ? values.FirstOrDefault() : string.Empty;

    var body = string.Empty;
    if (request.Content != null)
      body = await request.Content.ReadAsStringAsync(cancellationToken);

    var secretString = $"{method};{path}{query};{gateway};{body}";

    _logger.LogDebug("[SIGNING] Secret string: {SecretString}", secretString);

    return secretString;
  }

  private string GetUserAgent(UserIdentity identity)
  {
    return _settings.HeaderUserAgentFormat
                    .Replace(FinnApiSettings.UserAgentAndroidVersion, identity.AndroidVersion.ToString())
                    .Replace(FinnApiSettings.UserAgentDeviceBuild, identity.Build)
                    .Replace(FinnApiSettings.UserAgentDeviceModel, identity.Model)
                    .Replace(FinnApiSettings.UserAgentFinnVersion, identity.FinnVersion);
  }

  private static byte[] DecodeHmacKey(string obfuscatedValue)
  {
    var deobfuscated = ToggleObfuscate(obfuscatedValue);
    return Convert.FromBase64String(deobfuscated);
  }

  private static string ToggleObfuscate(string value)
  {
    const char cr = '\r';
    var        sb = new StringBuilder(value.Length);

    foreach (var c in value)
    {
      char newChar = c switch
      {
        >= 'a' and <= 'm' or >= 'A' and <= 'M' => (char)(c + cr),
        >= 'n' and <= 'z' or >= 'N' and <= 'Z' => (char)(c - cr),
        _ => c
      };
      sb.Append(newChar);
    }

    return sb.ToString();
  }

  #endregion
}
