namespace FinnStatistikk.DiscoveryTool.Services;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Models;

/// <summary>Manages the persistence of the scraping progress.</summary>
public class ProgressManager
{
  #region Constants & Statics

  private const string ProgressFileName = "scraping_progress.json";

  #endregion

  #region Properties & Fields - Non-Public

  private readonly ILogger<ProgressManager> _logger;

  #endregion

  #region Constructors

  public ProgressManager(ILogger<ProgressManager> logger)
  {
    _logger = logger;
  }

  #endregion

  #region Methods

  /// <summary>Loads scraping progress from the state file, if it exists.</summary>
  /// <returns>A ScrapingProgress object, or null if no progress file is found or is invalid.</returns>
  public async Task<ScrapingProgress?> LoadProgressAsync()
  {
    if (!File.Exists(ProgressFileName))
      return null;

    try
    {
      var json     = await File.ReadAllTextAsync(ProgressFileName);
      var progress = JsonSerializer.Deserialize<ScrapingProgress>(json);
      if (progress != null)
        _logger.LogInformation(
          "Found previous scraping progress. Market index: {MarketIndex}, Page: {Page}",
          progress.CurrentMarketIndex, progress.CurrentPageIndex);
      return progress;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load scraping progress file. It will be ignored.");
      return null;
    }
  }

  /// <summary>Saves the current scraping progress to the state file.</summary>
  public async Task SaveProgressAsync(ScrapingProgress progress)
  {
    try
    {
      var options = new JsonSerializerOptions { WriteIndented = true };
      var json    = JsonSerializer.Serialize(progress, options);
      await File.WriteAllTextAsync(ProgressFileName, json);
      _logger.LogDebug("Scraping progress saved. Market index: {MarketIndex}, Page: {Page}",
                       progress.CurrentMarketIndex, progress.CurrentPageIndex);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to save scraping progress: {Message}", ex.Message);
    }
  }

  /// <summary>Deletes the progress file, typically after a successful run or when restarting.</summary>
  public void DeleteProgress()
  {
    if (File.Exists(ProgressFileName))
      try
      {
        File.Delete(ProgressFileName);
        _logger.LogInformation("Scraping progress file deleted.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to delete scraping progress file.");
      }
  }

  #endregion
}
