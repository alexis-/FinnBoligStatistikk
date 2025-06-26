# Finn.no API Request Headers Construction

We have reversed engineered the Android finn.no app to precisely determine how the API headers are built.

## Required Headers

A Canonical List of Required Headers: Based on our analysis, every request sent to the Finn.no API must include the following headers, constructed dynamically for each session:

    | Header | Example Value | Source |
    | :--- | :--- | :--- |
    | `Connection` | `Keep-Alive` | Static |
    | `Accept-Encoding` | `gzip` | Static |
    | `User-Agent` | `FinnApp_And/250616-85b30270 (...)` | Session (Device Profile) |
    | `FINN-Device-Info` | `Android, mobile` | Session (Device Profile) |
    | `VersionCode` | `1004586734` | Session (Device Profile) |
    | `Session-Id` | `uuid-for-this-session` | Session (Generated) |
    | `Visitor-Id` | `persistent-uuid-for-identity` | Session (User Identity) |
    | `FINN-App-Installation-Id` | `persistent-uuid-for-identity` | Session (User Identity) |
    | `Ab-Test-Device-Id` | `persistent-uuid-for-identity`| Session (User Identity) |
    | `x-nmp-os-version` | `14` | Session (Device Profile, field `AndroidVersion`) |
    | `x-nmp-device` | `SM-G998B` | Session (Device Profile, field `DeviceModel` ) |
    | `x-nmp-app-version-name` | `250616-85b30270` |	Static (per App Version, same as version name) |
    | `x-nmp-app-build-number`| `1004586734` | Static (per App Version, same as version code) |
    | `x-nmp-os-name` | `Android` | Static |
    | `x-nmp-app-brand` | `finn` | Static |
    | `x-finn-apps-adinput-version-name` | `viewings` | Static |
    | `FINN-GW-SERVICE` | `Search-Quest-RE` or `NAM2` | Endpoint-Specific |
    | `X-FINN-API-Version` | `5` | Static |
    | `Feature-Toggles` | `force-strict-mode-in-debug,debug-button,unit-test_feature_on,assert-utils-crash-on-debug,pulse-unicorn,chucker-network-interceptor,show-homescreen-transparency-dialog,force-search-results-rating-cat,disable-english-formatting,dev_enable_recommerce_upload_photos,recommerce.bet.airpods-warning,apps.android.motor-transaction.search-results-landing-page-entry-pos3,apps.android.job_marketfront.marketing_banner,apps.android.disable_davs,apps.android.adview.realestate-leisure-and-abroad-nam-2-disabled-v2,apps.android.realestate.letting.external.disable,apps.android.realestate.map.pois.disable,apps.android.tjm-process-webview-disabled,apps.android.kindly-chatbot,apps.android.adin-payment-mobilepay,apps.android.set.enable_recommerce_partial_updates_ver2,apps.android.frontpage.categories_explorer,apps.android.frontpage.content_feed_api_v0.kill_switch,apps.android.frontpage.crossbrand_itemtype_support.kill_switch,apps.android.jobapply.nativeform-v3,apps.android.jobapply.nativeform.feedbackbox,apps.android.jobapply.nativeform.recommendationcards,apps.android.mobility.ad_insertion_updated_external_flow,apps.android.mobility.tjm.adin.consent.experiment.disabled,ad_insertion_mvv_wip,apps.android.advertising.enable-gam,apps.android.advertising.tracker-killswitch,apps.android.pulse.event.sent-count-tracking-v1,apps.android.advertising.prebid-config-killswitch,apps.android.advertising.frontpage-killswitch,apps.android.advertising.adnuntius-killswitch,apps.android.advertising.content-marketing-killswitch,apps.android.advertising.content-marketing-normal-text-killswitch,apps.android.advertising.enable-gam-test-campaign,android.rc.item-creation.eid-verification,apps.android.mobility.adinsertion.eid-verification,apps.android.disable-native-recommerce-full-ad-statistics-view,apps.android.disable-native-recommerce-buy-extra-visibility-view,apps.android.job.salary_comparison,apps.android.jobapply.ai-application-letter,tjt.experiment.buy-now-placement,apps.android.set.recommerce-camera-ad-insertion-flow-off,apps.android.set.recommerce_camera_guidance_off,apps.android.set.recommerce_camera_category_attribute_predictions_off,apps.android.tcf-disable-v2,apps.android.search-location-persistence,apps.android.enable_in_app_review,apps.android.review-feedback-enable-v2,apps.android.rating-feedback-enable-v2,advertising-android-killswitch,apps.android.disable-content-card,apps.android.realestate.hybrid-realtor-profiles,apps.android.mobility-landingpage-disabled-v1,apps.android-semantic-search.dev,apps.android.recom-discovery-explore-deeplink,job.android.marketfront.ai-search-entry,apps.android.recommerce-search-result-item-new-design,apps.android.recommerce.sort-filter.callout,apps.android.brand-nav.enable-follow`| Static |
    | `X-FINN-API-Feature` | `7,9,13,15,16,17,18,21,22,24,26,28,29,30,33,31,32,36,37,34,35,40,42,43,38,44,45,48,20,49,50` | Static |
    | `Build-Type` | `release` | Static |

**Note on Consent Headers (`CMP-*`):**
The choice to set these headers to `1` (consent given) instead of `0` (consent denied) is a strategic one. While both are valid, simulating a user who has accepted all consents is more likely to grant access to the full range of API features and represents the "happy path" user, minimizing the risk of encountering feature-gated or restricted responses.

## How the Gateway Header Works

The `FINN-GW-SERVICE` header acts as a routing key for Finn.no's API gateway. It tells the gateway which specific backend service (e.g., the search service vs. the ad view service) should handle the request.

Most importantly, its value is a **critical component of the HMAC signature calculation**. The server uses this "service name" along with the request method, path, and body to recalculate the `FINN-GW-KEY` signature. If your scraper does not include this header with the correct value for the endpoint you are calling, the signature your client calculates will not match the one the server calculates, and the request will be rejected as unauthorized.

## User Agent construction

### 1. Analysis of the `HttpUserAgent` Source Code

The provided Smali code for `no.finn.android.networking.HttpUserAgent` located in `.\smali_classes8\no\finn\android\networking\HttpUserAgent.smali` reveals the precise format and construction logic for the User-Agent string.

**A. Reverse-Engineered Logic:**

The core of the logic lies in the constructor (`<init>`), which assembles the final string using `java.lang.String.format`. The format string and its arguments can be mapped as follows:

*   **Format String:**
    ```
    %s/%s (Linux; U; Android %s; %s; %s Build/%s) %s(UA spoofed for tracking) %s
    ```
*   **Arguments Mapping:**
    1.  `%s`: `androidAppId` -> For the "finn" flavor, this is the static string **`FinnApp_And`**.
    2.  `%s`: `AppVersion.getVersionName()` -> The public version name of the Finn app (e.g., **`250616-85b30270`**).
    3.  `%s`: `android.os.Build.VERSION.RELEASE` -> The version of the Android OS (e.g., **`14`**).
    4.  `%s`: `Locale.getDefault().toString().toLowerCase()` -> The device's language and country code (e.g., **`nb_no`**).
    5.  `%s`: `android.os.Build.MODEL` -> The specific model identifier of the Android device (e.g., **`SM-G998B`** for a Samsung S21 Ultra).
    6.  `%s`: `android.os.Build.ID` -> The specific build ID of the Android OS installation (e.g., **`UQ1A.240205.004`**).
    7.  `%s`: `nativeAppId` -> For the "finn" flavor, this is the static string **`FINNNativeApp`**.
    8.  `%s`: `androidAppId` (again) -> The static string **`FinnApp_And`**.

**B. Example of a Reconstructed User-Agent:**

```
FinnApp_And/250616-85b30270 (Linux; U; Android 14; nb_no; SM-G998B Build/UQ1A.240205.004) FINNNativeApp(UA spoofed for tracking) FinnApp_And
```

### 2. Decomposition and Dynamic Component Analysis

To generate a believable and diverse pool of user agents, we must analyze which components are static and which need to be dynamically varied.

*   **Static Components:** These are fixed for all requests to Finn.no.
    *   `androidAppId`: Always `"FinnApp_And"`.
    *   `nativeAppId`: Always `"FINNNativeApp"`.
    *   `Locale`: Can be fixed to `"nb_no"` for consistency.

*   **Dynamic Components (The Device Fingerprint):** These must be varied to simulate a real-world user base. Crucially, **these components are not independent.** A device model from 2024 cannot be running Android 10. The `Build.ID` is also specific to a model and OS version. This leads to the requirement of a "Device Profile".
    *   **`Finn App Version`:** This must be a realistic version number. To appear modern, the system should use a selection of recent, real version numbers.
    *   **`Android OS Version`:** Must be a plausible version for the chosen device model.
    *   **`Device Model`:** This is a key part of the fingerprint and requires a diverse pool of real-world Android device models popular in Norway.
    *   **`Build ID`:** Must be a valid build ID corresponding to the specific OS version and device model.

### Android Device Profile Catalogue

We have compiled a document containing around 80 credible device profiles, with the following fields:
- `DeviceBrand`
- `DeviceModel`
- `AndroidVersion`
- `BuildID`

Each user agent will be constructed using a random entry from our catalogue. We will need to persist this data somewhere the app can easily access. The raw data is in csv data. We could consider loading it directly in memory, or persisting it in the database.

### Finn App Version Tracking

We must programmatically fetch the latest public version string for the Finn.no Android app from a reliable third-party source.

For maximum credibility we shouldn't update every profile's user agent to new versions at the same time. Instead we should stagger the update over a period (for example, one or two months). Given the rapid pace of Finn updates (typically several updates monthly), our park of devices should ideally have a mix of versions. 

To that effect we would need to persist the app version history, and create an algorithm together with user profile's metadata that progressively "roll out" the new version to our fleet, simulating the propagation of updates in real life. Persisting this information in the database will also protect against total failure in case our method of obtaining the latest information about the apk version name and version code breaks down.

We have identified that the latest version code and version name can be easily obtained by parsing the apkpure.net webpage `https://apkpure.net/finn-no/no.finn.android`.

The HTML response can be parsed using the `AngleSharp` library to locate either:
- The html tag `<div class="ver-top-down" data-dt-apkid="b/APK/bm8uZmlubi5hbmRyb2lkXzEwMDQ1ODY3MzRfYzNkMjgyMTE" data-dt-filesize="52582877" data-dt-version="250616-85b30270" data-dt-versioncode="1004586734" dt-eid="download_button" dt-params="module_name=header_information_card&amp;model_type=1025&amp;package_name=no.finn.android&amp;position=3&amp;small_position=1" dt-imp-once="true" dt-imp-end-ignore="true" dt-send-beacon="true"><a rel="" class="" title="Download FINN.no latest version apk" href="https://apkpure.net/finn-no/no.finn.android/download"><span><i></i>Download Latest Version</span></a></div>`.
- An XPath built from the `data-dt-version` and `data-dt-versioncode` attributes

This html tag always contains information about the latest version of the app, including its version name (`250616-85b30270`) and version code (`1004586734`).


# Analysis of `GeneralHeaderInterceptor` Source Code

This class is an `okhttp3/Interceptor`, a standard component in Android networking that intercepts every outgoing request to add, remove, or modify headers before the request is sent over the network. It also processes the server's response before passing it back to the application.

Its primary role is to inject a comprehensive set of headers that create a complete and consistent "fingerprint" of the user's device, application version, session, and privacy consent choices.

**A. Core Operations:**

The interceptor performs its work in the `intercept` method, which can be broken down into two main phases:

1.  **Request Enrichment:** Before the request is sent, the interceptor calls a master helper method, `applyAppHeaders`, which is responsible for adding a large number of headers. This function only applies headers if the request is destined for a Finn.no backend (`isAppBackend` check).

2.  **Response Processing:** After the response is received from the server, the interceptor reads specific headers (`Ab-Test-Treatments`, `FINN-Experiment`, `Feature-Toggles`) to dynamically update the application's internal A/B testing and feature flag configuration. This part of the logic is for the app's internal state and is **not critical for us to replicate**, but it confirms the importance of these headers in the request/response cycle.

The analysis will focus on the **Request Enrichment** phase, as these are the headers we must successfully replicate. This phase is further broken down into three sub-routines: `applyAppHeaders`, `applyNmpHeaders`, and `applyConsentHeaders`.

**B. Detailed Header Analysis & Breakdown:**

The following is a complete list of all headers added by the interceptor, grouped by the function that adds them. Each header's source and its implication for our scraper design is analyzed.

## **Headers from `applyAppHeaders` (Core Application Headers)**

This is the main function that adds the most critical headers.

| Header Name | Value Source | Type | Implication & Required Action |
| :--- | :--- | :--- | :--- |
| `Authorization` | `loginState.getAuthenticationHeader()` | Session-State | Added only if a user is logged in. As we will be operating anonymously, this header **should not be sent**. |
| `Ab-Test-Device-Id` | `ABTesting.getDeviceId()` | Persistent Identity | A unique ID for A/B testing. This ID must be generated once and then persisted and reused for a given `Visitor-Id`. |
| `Feature-Toggles` | `FeatureToggles.getAvailableFeatures()` | App-Version State | A string representing enabled features. We must capture a realistic value from a real device and include it. It can likely be a static value tied to the app version in our Device Profile. |
| `Build-Type` | `AppEnvironment.getBuildType()` | App-Version State | The build type of the app. For production, this will be a static string, likely `"release"`. |
| **`User-Agent`** | `userAgent.toString()` | **Device Profile** | The complete User-Agent string we previously analyzed. **Must be constructed dynamically** from the active Device Profile. |
| **`FINN-Device-Info`** | `"Android, mobile"` or `"Android, tablet"` | **Device Profile** | Must be consistent with the `Device Model` in the active Device Profile. We should default to `"Android, mobile"`. |
| `VersionCode` | `AppVersion.getVersionCode()` | App-Version State | The integer version code of the app (e.g., `241200`). This must be part of our Device Profile and must correspond to the `App Version` in the User-Agent. |
| `Session-Id` | `SessionInfo.getSessionId()` | Session-State | A unique identifier for the current user session. **A new UUID must be generated** at the start of each simulated user session. |
| `Visitor-Id` | `SessionInfo.getVisitorId()` | Persistent Identity | A unique identifier for the simulated app installation. **A new UUID must be generated once per User Identity** and then persisted and reused for the lifetime of that identity. |
| `FINN-App-Installation-Id` | `SessionInfo.getVisitorId()` | Persistent Identity | Same value as `Visitor-Id`, sent under a different header name. Must be kept in sync. |
| `x-finn-apps-adinput-version-name` | Static: `"viewings"` | Static | A hardcoded feature flag. This should be sent with every request. |
| `X-FINN-API-Version` | Static: `"5"` | Static | The version of the internal mobile API being targeted. This should be sent with every request. |
| `X-FINN-API-Feature` | `Api.apiFeatures()` | App-Version State | A string representing supported API features. Like `Feature-Toggles`, we must capture a realistic value and can likely keep it static per app version in our Device Profile. |

## **Headers from `applyNmpHeaders` (Marketplace Platform Headers)**

These headers appear to be for Schibsted's shared "Next-generation MarketPlace" (NMP) platform. They largely overlap with information in the User-Agent string and must be consistent.

| Header Name | Value Source | Type | Implication & Required Action |
| :--- | :--- | :--- | :--- |
| `x-nmp-os-name` | Static: `"Android"` | Static | Must be sent with every request. |
| **`x-nmp-os-version`** | `Build.VERSION.RELEASE` | **Device Profile** | The Android OS version (e.g., `"14"`). Must match the OS version in the User-Agent. |
| **`x-nmp-device`** | `Build.MODEL` | **Device Profile** | The device model (e.g., `"SM-G998B"`). Must match the model in the User-Agent. |
| `x-nmp-app-brand` | `flavorTypeProvider.getFlavor()` | App-Version State | For our purposes, this will be the static string `"finn"`. |
| **`x-nmp-app-version-name`** | `AppVersion.getVersionName()` | App-Version State | The app's public version name (e.g., `"24.12"`). Must match the version in the User-Agent. |
| **`x-nmp-app-build-number`** | `AppVersion.getVersionCode()` | App-Version State | The app's integer version code. Must match the `VersionCode` header. |

## **Headers from `applyConsentHeaders` (Privacy Consent Headers)**

These headers communicate the user's consent choices for various data processing purposes, as required by GDPR.

| Header Name | Value Source | Type | Implication & Required Action |
| :--- | :--- | :--- | :--- |
| `CMP-Advertising` | `consentStore.getConsents()` | User Consent | Set to `"1"` if consent is given, `"0"` otherwise. |
| `CMP-Analytics` | `consentStore.getConsents()` | User Consent | Set to `"1"` if consent is given, `"0"` otherwise. |
| `CMP-Marketing` | `consentStore.getConsents()` | User Consent | Set to `"1"` if consent is given, `"0"` otherwise. |
| `CMP-Personalisation` | `consentStore.getConsents()` | User Consent | Set to `"1" if consent is given, `"0"` otherwise. |

**Recommendation:** To appear as a normal, fully functional user and maximize data access, our scraper should simulate a user who has accepted all privacy consents. Therefore, **all four `CMP-*` headers should be statically set to `"1"`**.

## 3. Summary of Findings & Implications for Scraper Design

This interceptor provides a complete blueprint for the headers required for a successful API request. The analysis reinforces and expands upon our `UserIdentityManager` and `DeviceProfile` concepts.

1.  **The "Device Profile" is Confirmed as Essential:** The User-Agent, `FINN-Device-Info`, and all `x-nmp-*` headers are derived from the same underlying device and app version characteristics. This confirms that our `DeviceProfile` entity is the correct abstraction. It must contain `Model`, `AndroidVersion`, `BuildId`, `AppVersionName`, and `AppVersionCode` as a consistent and realistic set.

2.  **The "User Identity" Requires More Attributes:** Our `UserIdentity` model must be expanded to store not just a `SessionId`, but also a persistent `VisitorId` and `Ab-Test-Device-Id`.
    *   `UserIdentity` Table/Entity:
        *   `UserIdentityId` (PK)
        *   `VisitorId` (UUID, generated once)
        *   `AbTestDeviceId` (UUID, generated once)
        *   `LastUsedTimestamp`
        *   `FailureCount`
        *   `IsActive`

3.  **The Session Requires its Own Context:** A short-lived "Session" object should be created for each simulated user session. It will hold:
    *   A newly generated `SessionId` (UUID).
    *   The selected `UserIdentity`.
    *   The selected `DeviceProfile`.
    *   The assigned `ManagedTorInstance`.
    This context object will be passed to the `HttpRequestExecutorWorker` to construct the final headers for all requests within that session.


This comprehensive set of headers, when combined with the previously analyzed HMAC signing, provides a complete and robust foundation for mimicking the mobile application's network traffic.

# API HMAC Signing

You will find an example implementation below:
```
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

public class SigningDelegatingHandler : DelegatingHandler
{
  public string HmacKeyObfuscated    { get;  init; } = "MQD1MzLjZ2ZgLwp4Zl00ATD5YJV5ATRgLzVlBTEvLmNkAzR2";
  public string HeaderGatewayKey     { get;  init; } = "FINN-GW-KEY";
  public string HeaderGatewayService { get;  init; } = "FINN-GW-SERVICE";
  
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
```