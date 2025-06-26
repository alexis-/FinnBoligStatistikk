namespace FinnStatistikk.DiscoveryTool.Models;

public class FinnApiSettings
{
  #region Constants & Statics

  public const string UserAgentFinnVersion    = "{FinnVersion}";
  public const string UserAgentAndroidVersion = "{AndroidVersion}";
  public const string UserAgentDeviceModel    = "{DeviceModel}";
  public const string UserAgentDeviceBuild    = "{DeviceBuild}";

  #endregion

  #region Properties & Fields - Public

  public string FinnDomainName { get; init; } = "www.finn.no";

  // Corrected Host and Base URL
  public string ApiHost    { get; init; } = "appsgw.finn.no";
  public string ApiBaseUrl { get; init; } = "https://appsgw.finn.no/";

  // Corrected URL formats
  public string MarketsApiUrl { get; init; } = "api/getallmarkets";
  public string SearchApiUrl  { get; init; } = "search/{SearchKey}?client=ANDROID&sort=PUBLISHED_DESC&page={Page}";
  public string AdViewApiUrl  { get; init; } = "adview/{Id}";

  public string HmacKeyObfuscated    { get;  init; } = "MQD1MzLjZ2ZgLwp4Zl00ATD5YJV5ATRgLzVlBTEvLmNkAzR2";
  public string HeaderGatewayKey     { get;  init; } = "FINN-GW-KEY";
  public string HeaderGatewayService { get;  init; } = "FINN-GW-SERVICE";
  public string HeaderUserAgentFormat { get; init; } =
    "FinnApp_And/{FinnVersion} (Linux; U; Android {AndroidVersion}; nb_no; {DeviceModel} Build/{DeviceBuild}) FINNNativeApp(UA spoofed for tracking) FinnApp_And";
  public string HeaderAbTestDeviceId    { get; init; } = "Ab-Test-Device-Id";
  public string HeaderApiVersion        { get; init; } = "X-FINN-API-Version";
  public string HeaderApiVersionValue   { get; init; } = "5";
  public string HeaderAppInstallationId { get; init; } = "FINN-App-Installation-Id";
  public string HeaderBuildType         { get; init; } = "Build-Type";
  public string HeaderBuildTypeValue    { get; init; } = "release";
  public string HeaderDeviceInfo        { get; init; } = "FINN-Device-Info";
  public string HeaderDeviceInfoValue   { get; init; } = "Android, mobile";
  public string HeaderSessionId         { get; init; } = "Session-Id";
  public string HeaderVersionCode       { get; init; } = "VersionCode";
  public string HeaderVisitorId         { get; init; } = "Visitor-Id";

  #endregion
}
