// ReSharper disable ArrangeModifiersOrder
// ReSharper disable ClassNeverInstantiated.Global

namespace FBS.Scrapper.Models.Config
{
  public class FinnConfig
  {
    #region Constants & Statics

    public const string UA_ANDROID_VERSION = "{AndroidVersion}";
    public const string UA_DEVICE_BUILD    = "{DeviceBuild}";
    public const string UA_DEVICE_MODEL    = "{DeviceModel}";
    public const string UA_FINN_VERSION    = "{FinnVersion}";

    #endregion

    #region Properties & Fields - Public

    /// <summary>Finn domain name, used in injecting headers</summary>
    public string FinnDomainName { get; init; } = "finn.no";

    /// <summary>Finn search API url. Replace {Market} and {Page} with their respective values.</summary>
    public string SearchApiUrl { get; init; } =
      "/search/{Market}?client=ANDROID&sort=PUBLISHED_DESC&page={Page}&include_filters=false";

    /// <summary>Finn Ad view API url. Replace {Id} with the ad id.</summary>
    public string AdViewApiUrl { get; init; } = "/adview/{Id}";

    /// <summary>Inactivity delay in minutes before the session ID should be renewed</summary>
    public int SessionIdRenewDelay { get; init; } = 60;

    /// <summary>The obfuscated key used in Finn's APK</summary>
    public string HmacKeyObfuscated { get; init; } = "MQD1MzLjZ2ZgLwp4Zl00ATD5YJV5ATRgLzVlBTEvLmNkAzR2";

    /// <summary>Key of the header that contains the HMAC SHA-512 encrypted token</summary>
    public string HeaderGatewayKeyKey { get; init; } = "FINN-GW-KEY";

    /// <summary>
    ///   Key of the header that contains the requested Finn service gateway (eg. NAM2,
    ///   FAVORITE-MANAGEMENT, etc.)
    /// </summary>
    public string HeaderGatewayServiceKey { get; init; } = "FINN-GW-SERVICE";

    /// <summary>
    ///   Finn's mobile app User Agent "FinnApp_And/%s (Linux; U; Android %s; %s; %s Build/%s)
    ///   FINNNativeApp(UA spoofed for tracking) FinnApp_And". Replace {FinnVersion},
    ///   {AndroidVersion}, {DeviceModel} and {DeviceBuild} with their respective values.
    /// </summary>
    public string HeaderUserAgentValue { get; init; } =
      "FinnApp_And/{FinnVersion} (Linux; U; Android {AndroidVersion}; nb_no; {DeviceModel} Build/{DeviceBuild}) FINNNativeApp(UA spoofed for tracking) FinnApp_And";

    /// <summary>Finn's mobile app Accept header value. "gzip" as of 2023/07</summary>
    public string HeaderAcceptValue { get; init; } = "gzip";

    /// <summary>
    ///   GUID supposedly used to track devices for AB statistic tests. Same value as
    ///   FINN-App-Installation-Id and Visitor-Id
    /// </summary>
    public string HeaderAbTestDeviceIdKey { get; init; } = "Ab-Test-Device-Id";

    /// <summary>Finn's API version. 5 as of 07/2023</summary>
    public string HeaderApiVersionKey { get; init; } = "X-FINN-API-Version";

    /// <summary>Finn's API version. 5 as of 07/2023</summary>
    public string HeaderApiVersionValue { get; init; } = "5";

    /// <summary>Finn's API feature. Not sure what this is used for</summary>
    public string HeaderApiFeatureKey { get; init; } = "X-FINN-API-Feature";

    /// <summary>Finn's API feature. Not sure what this is used for</summary>
    public string HeaderApiFeatureValue { get; init; } =
      "7,9,13,15,16,17,18,21,22,24,26,28,29,30,33,31,32,36,37,34,35,40,42,43,38,44,45,48,20,49";

    /// <summary>Mobile app installation GUID. Same value as Ab-Test-Device-Id and Visitor-Id</summary>
    public string HeaderAppInstallationIdKey { get; init; } = "FINN-App-Installation-Id";

    /// <summary>App build release configuration. Supposedly either release or debug?</summary>
    public string HeaderBuildTypeKey { get; init; } = "Build-Type";

    /// <summary>App build release configuration. Supposedly either release or debug?</summary>
    public string HeaderBuildTypeValue { get; init; } = "release";

    /// <summary>Device type. "Android, mobile" for Android</summary>
    public string HeaderDeviceInfoKey { get; init; } = "FINN-Device-Info";

    /// <summary>Device type. "Android, mobile" for Android</summary>
    public string HeaderDeviceInfoValue { get; init; } = "Android, mobile";

    /// <summary>Device supported features</summary>
    public string HeaderFeatureTogglesKey { get; init; } = "Feature-Toggles";

    /// <summary>Device supported features</summary>
    public string HeaderFeatureTogglesValue { get; init; } =
      "force-strict-mode-in-debug,debug-button,pulse-unicorn,show-homescreen-transparency-dialog,apps.android.messaging.new-ui,force-search-results-rating-cat,apps.android.display-easter-eggs,apps.android.enable-glimr-ads,apps.android.enable-content-marketing-webview,apps.android.banner-ad-lazy-load-search-result,apps.android.native-app-feedback,apps.android.christmas_splash,apps.android.favorites.christmas_list,apps.android.bottombar.pride,apps.android.braze-content-cards,apps.android.motor-transaction.search-results-landing-page-entry-pos3,apps.android.campaign.job.saveSearch,apps.android.campaign.job.candidateProfile,apps.android.feature.motor.displayAdPosition,apps.android.disable_shipping_page,apps.android.disable_davs,apps.android.realestate_show_old_company_profile,apps.android.bap_staggered_grid_xml,apps.android.adview.realestate-nam-2-disabled-v2,apps.android.tjt-banner-disabled,apps.android.tjt-insurance,apps.android.makeofferflow.version_2.insurance,apps.android.set.sms_autofill,apps.android.set.show_adadmin_transaction_link,apps.android.job.candidateProfile.paywall.recommendations,apps.android.set.adinsertion_satisfaction_survey,transaction-journey-motor.experiments.shared,apps.android.job-profile-promo-text-experiment,apps.android.object-page-ad-load-offset,apps.android.result-page-ad-load-offset,apps.android.makeofferflow.version_2";

    /// <summary>
    ///   Session GUID. Seems to be generated randomly and updated after several hours of
    ///   inactivity. Example: 3055cc60-276a-4e07-b619-c8bb737d4f70
    /// </summary>
    public string HeaderSessionIdKey { get; init; } = "Session-Id";

    /// <summary>Android app version code. Eg 1003572197</summary>
    public string HeaderVersionCodeKey { get; init; } = "VersionCode";

    /// <summary>Mobile app Visitor GUID. Same value as Ab-Test-Device-Id and FINN-App-Installation-Id</summary>
    public string HeaderVisitorIdKey { get; init; } = "Visitor-Id";

    #endregion
  }
}
