# finn_api_curl.py
import argparse
import base64
import hashlib
import hmac
import subprocess
import sys
from urllib.parse import urlparse

# --- Static data ported from your C# project ---
HMAC_KEY_OBFUSCATED = "MQD1MzLjZ2ZgLwp4Zl00ATD5YJV5ATRgLzVlBTEvLmNkAzR2"
HEADERS = {
    "Connection": "Keep-Alive",
    "Accept-Encoding": "gzip",
    "Ab-Test-Device-Id": "7336f13b-fb7e-4743-a036-9dd2a5feb86b",
    "Feature-Toggles": "force-strict-mode-in-debug,debug-button,pulse-unicorn,show-homescreen-transparency-dialog,apps.android.messaging.new-ui,force-search-results-rating-cat,apps.android.display-easter-eggs,apps.android.enable-glimr-ads,apps.android.enable-content-marketing-webview,apps.android.banner-ad-lazy-load-search-result,apps.android.native-app-feedback,apps.android.christmas_splash,apps.android.favorites.christmas_list,apps.android.bottombar.pride,apps.android.braze-content-cards,apps.android.motor-transaction.search-results-landing-page-entry-pos3,apps.android.campaign.job.saveSearch,apps.android.campaign.job.candidateProfile,apps.android.feature.motor.displayAdPosition,apps.android.disable_shipping_page,apps.android.disable_davs,apps.android.realestate_show_old_company_profile,apps.android.bap_staggered_grid_xml,apps.android.adview.realestate-nam-2-disabled-v2,apps.android.tjt-banner-disabled,apps.android.tjt-insurance,apps.android.makeofferflow.version_2.insurance,apps.android.set.sms_autofill,apps.android.set.show_adadmin_transaction_link,apps.android.job.candidateProfile.paywall.recommendations,apps.android.set.adinsertion_satisfaction_survey,transaction-journey-motor.experiments.shared,apps.android.job-profile-promo-text-experiment,apps.android.object-page-ad-load-offset,apps.android.result-page-ad-load-offset,apps.android.makeofferflow.version_2",
    "Build-Type": "release",
    "User-Agent": "FinnApp_And/230707-6e39e03 (Linux; U; Android 9; nb_no; Redmi Note 8 Build/PKQ1.190616.001) FINNNativeApp(UA spoofed for tracking) FinnApp_And",
    "FINN-Device-Info": "Android, mobile",
    "VersionCode": "1003572197",
    "Session-Id": "c2b5d205-4f68-444c-bd05-bbae06c46e9c", # This can be a new GUID
    "Visitor-Id": "7336f13b-fb7e-4743-a036-9dd2a5feb86b", # Should be same as Ab-Test-Device-Id
    "FINN-App-Installation-Id": "7336f13b-fb7e-4743-a036-9dd2a5feb86b", # Should be same as Ab-Test-Device-Id
    "X-FINN-API-Version": "5",
    "X-FINN-API-Feature": "7,9,13,15,16,17,18,21,22,24,26,28,29,30,33,31,32,36,37,34,35,40,42,43,38,44,45,48,20,49"
}

def toggle_obfuscate(value: str) -> str:
    result = []
    for char in value:
        if 'a' <= char <= 'm' or 'A' <= char <= 'M':
            result.append(chr(ord(char) + 13))
        elif 'n' <= char <= 'z' or 'N' <= char <= 'Z':
            result.append(chr(ord(char) - 13))
        else:
            result.append(char)
    return "".join(result)

def get_hmac_key() -> bytes:
    deobfuscated = toggle_obfuscate(HMAC_KEY_OBFUSCATED)
    return base64.b64decode(deobfuscated)

def calculate_signature(method: str, path: str, query: str, gateway: str, body: str, hmac_key: bytes) -> str:
    secret_string = f"{method};{path}{query};{gateway};{body}"
    sys.stderr.write(f"[*] Secret string to sign: {secret_string}\n")
    signature = hmac.new(hmac_key, secret_string.encode('utf-8'), hashlib.sha512).digest()
    return base64.b64encode(signature).decode('utf-8')

def main():
    parser = argparse.ArgumentParser(description="Generate and execute a signed curl command for the Finn.no mobile API.")
    parser.add_argument("--url", required=True, help="The full URL to request.")
    parser.add_argument("--method", default="GET", help="The HTTP method (e.g., GET, POST).")
    parser.add_argument("--gateway", default="", help="The FINN-GW-SERVICE value (e.g., Search-Quest, NAM2).")
    parser.add_argument("--body", default="", help="The request body for POST requests.")
    parser.add_argument("--output", help="File to save the output to.")
    
    args = parser.parse_args()

    parsed_url = urlparse(args.url)
    path = parsed_url.path
    query = f"?{parsed_url.query}" if parsed_url.query else ""
    
    hmac_key = get_hmac_key()
    signature = calculate_signature(args.method.upper(), path, query, args.gateway, args.body, hmac_key)
    
    sys.stderr.write(f"[*] Calculated FINN-GW-KEY: {signature}\n\n")
    
    curl_command = ["curl", "-i", "-v", "--compressed", "-X", args.method.upper()]
    for key, value in HEADERS.items():
        curl_command.extend(["-H", f"{key}: {value}"])
    
    if args.gateway:
        curl_command.extend(["-H", f"Finn-GW-Service: {args.gateway}"])
    
    curl_command.extend(["-H", f"FINN-GW-KEY: {signature}"])
    
    if args.body:
        curl_command.extend(["-d", args.body])
        
    curl_command.append(args.url)

    if args.output:
        with open(args.output, 'w', encoding='utf-8') as f:
            # Redirect both stdout and stderr of the curl command to the file
            subprocess.run(curl_command, stdout=f, stderr=subprocess.STDOUT)
    else:
        subprocess.run(curl_command)

if __name__ == "__main__":
    main()