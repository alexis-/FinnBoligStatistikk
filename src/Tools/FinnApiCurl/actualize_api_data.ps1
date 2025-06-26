<#
.SYNOPSIS
    Automates the process of fetching fresh API data from Finn.no.
.DESCRIPTION
    This script runs a series of pre-defined API calls against the Finn.no mobile API.
    It uses a helper Python script (finn_api_curl.py) to generate the required dynamic
    authentication signatures. The full verbose output of each call (request headers,
    response headers, and response body) is saved to a separate file in the 
    './api_data_latest' directory.

.PREREQUISITES
    - Python 3 must be installed and available in your system's PATH.
    - The 'finn_api_curl.py' script must be in the same directory as this script.
    
.NOTES
    Before running, you MUST update the placeholder Ad IDs below with recent, valid IDs
    from Finn.no to ensure the 'adview' and 'getProfile' calls succeed.
#>

# --- CONFIGURATION: UPDATE THESE VALUES ---
# You MUST replace these placeholder IDs with recent, valid ones from Finn.no.
# 1. Find a normal apartment/house for sale.
$realEstateAdId = "412915499"
# 2. Find a new construction project (Nybygg).
$developmentAdId = "365654846"
# 3. Find an item for sale on Torget (e.g., search for 'verktøy').
$torgetAdId = "413377821"
# --- END CONFIGURATION ---


# --- SCRIPT ---
$ErrorActionPreference = "Stop" # Exit script on first error
$outputDir = ".\api_data_latest"

# Create the output directory if it doesn't exist
if (-not (Test-Path $outputDir)) {
    Write-Host "Creating output directory: $outputDir"
    New-Item -ItemType Directory -Path $outputDir | Out-Null
}

# Helper function to execute a command
function Invoke-ApiCall {
    param(
        [string]$CallName,
        [string]$PythonScript,
        [array]$ArgumentList,
        [string]$OutputFile
    )
    
    Write-Host "--- Executing: $CallName ---" -ForegroundColor Yellow
    
    try {
        # Execute the python script with each argument passed separately
        & python $PythonScript $ArgumentList
        
        # Robust success check: ensure the file was created and is not empty
        if ((Test-Path $OutputFile) -and ((Get-Item $OutputFile).Length -gt 0)) {
            Write-Host "SUCCESS: API call '$CallName' completed. Output saved to '$OutputFile'" -ForegroundColor Green
        } else {
            Write-Host "FAILURE: API call '$CallName' ran, but the output file '$OutputFile' is missing or empty." -ForegroundColor Red
        }
    } catch {
        Write-Host "FAILURE: API call '$CallName' failed." -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)"
    }
    Write-Host "" # Newline for readability
}

# --- DEFINE AND RUN API CALLS ---

Write-Host "Starting Finn.no API data actualization..."
Write-Host "=========================================="
Write-Host "Using Real Estate Ad ID: $realEstateAdId"
Write-Host "Using Development Ad ID: $developmentAdId"
Write-Host "Using Torget Ad ID:      $torgetAdId"
Write-Host "=========================================="
Write-Host ""

$pythonScript = ".\finn_api_curl.py"

# 1. List Real Estate Ads (Page 1)
$outputFile = "$outputDir\01_search_realestate_homes_p1.txt"
$arguments = @("--url", "https://appsgw.finn.no/search/SEARCH_ID_REALESTATE_HOMES?client=ANDROID&sort=PUBLISHED_DESC&page=1&include_filters=false", "--gateway", "Search-Quest", "--output", $outputFile)
Invoke-ApiCall -CallName "Search Real Estate (Page 1)" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 2. List Real Estate Ads (Page 2)
$outputFile = "$outputDir\02_search_realestate_homes_p2.txt"
$arguments = @("--url", "https://appsgw.finn.no/search/SEARCH_ID_REALESTATE_HOMES?client=ANDROID&sort=PUBLISHED_DESC&page=2&include_filters=false", "--gateway", "Search-Quest", "--output", $outputFile)
Invoke-ApiCall -CallName "Search Real Estate (Page 2)" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 3. View Real Estate Ad Details
$outputFile = "$outputDir\03_view_realestate_home.txt"
$arguments = @("--url", "https://appsgw.finn.no/adview/$realEstateAdId", "--gateway", "NAM2", "--output", $outputFile)
Invoke-ApiCall -CallName "View Real Estate Ad" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 4. View Real Estate Development Project Ad Details
$outputFile = "$outputDir\04_view_realestate_development.txt"
$arguments = @("--url", "https://appsgw.finn.no/adview/$developmentAdId", "--gateway", "NAM2", "--output", $outputFile)
Invoke-ApiCall -CallName "View Development Project Ad" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 5. Get Proximity (Neighborhood) Info
$outputFile = "$outputDir\05_get_profile_realestate.txt"
$arguments = @("--url", "https://appsgw.finn.no/getProfile?client=AndroidApp&id=$realEstateAdId", "--gateway", "NABOLAGSPROFIL", "--output", $outputFile)
Invoke-ApiCall -CallName "Get Neighborhood Profile" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 6. View "Torget" Ad
$outputFile = "$outputDir\06_view_bap_sell.txt"
$arguments = @("--url", "https://appsgw.finn.no/adview/$torgetAdId", "--gateway", "NAM2", "--output", $outputFile)
Invoke-ApiCall -CallName "View Torget Ad" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 7. List All Finn Markets
$outputFile = "$outputDir\07_list_markets.txt"
$arguments = @("--url", "https://apps.finn.no/api/", "--output", $outputFile)
Invoke-ApiCall -CallName "List All Markets" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 8. Broadcast Call
$outputFile = "$outputDir\08_broadcast.txt"
$arguments = @("--url", "https://apps.finn.no/broadcast/", "--output", $outputFile)
Invoke-ApiCall -CallName "Broadcast Call" -PythonScript $pythonScript -ArgumentList $arguments -OutputFile $outputFile

# 9. Schibsted Tracker (Expected to fail with 404, which is handled gracefully)
Write-Host "--- Executing: Schibsted Tracker ---" -ForegroundColor Yellow
$schibstedUrl = "https://cis.schibsted.com/api/v1/identify"
$schibstedBody = @'
{
    "aaId": "022b139d-91de-4cdc-b2aa-7d91deb40220",
    "deviceId": "022b139d-91de-4cdc-b2aa-7d91deb40220",
    "doTracking": true
}
'@
$schibstedHeaders = @{
    "Content-Type" = "application/json; charset=UTF-8"
    "User-Agent"   = "Android-Pulse-Tracker/7.8.9"
}
$schibstedOutputFile = "$outputDir\09_schibsted_identify.txt"
try {
    # -UseBasicParsing is a good practice for non-interactive scripts
    Invoke-WebRequest -Uri $schibstedUrl -Method Post -Body $schibstedBody -Headers $schibstedHeaders -OutFile $schibstedOutputFile -UseBasicParsing
    Write-Host "SUCCESS: API call 'Schibsted Tracker' completed. Output saved to '$schibstedOutputFile'" -ForegroundColor Green
} catch {
    Write-Host "FAILURE (EXPECTED): API call 'Schibsted Tracker' failed." -ForegroundColor DarkYellow
    Write-Host "Error: $($_.Exception.Message)"
}
Write-Host ""

Write-Host "Script finished. All outputs should now be in the '$outputDir' directory." -ForegroundColor Cyan