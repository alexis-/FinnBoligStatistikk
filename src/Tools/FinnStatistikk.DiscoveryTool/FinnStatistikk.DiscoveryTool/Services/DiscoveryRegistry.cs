namespace FinnStatistikk.DiscoveryTool.Services;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Models;

public class DiscoveryRegistry
{
  #region Constants & Statics

  private const string RegistryDirectory = "DiscoveryRegistry";
  private const string RegistryFileName  = "structural_schema.json";

  #endregion

  #region Properties & Fields - Non-Public

  private readonly string                     _registryFilePath;
  private readonly ILogger<DiscoveryRegistry> _logger;

  private DiscoveryRegistryModel _registryData = new();

  #endregion

  #region Constructors

  public DiscoveryRegistry(ILogger<DiscoveryRegistry> logger)
  {
    _logger = logger;
    Directory.CreateDirectory(RegistryDirectory);
    _registryFilePath = Path.Combine(RegistryDirectory, RegistryFileName);
  }

  #endregion

  #region Methods

  public async Task LoadAsync()
  {
    if (!File.Exists(_registryFilePath))
    {
      _logger.LogWarning("Registry file not found. A new one will be created.");
      _registryData = new DiscoveryRegistryModel();
      return;
    }

    try
    {
      var json = await File.ReadAllTextAsync(_registryFilePath);
      _registryData = JsonSerializer.Deserialize<DiscoveryRegistryModel>(json) ?? new DiscoveryRegistryModel();
      _logger.LogInformation(
        "Successfully loaded discovery registry with {SchemaCount} schemas and {PathCount} known paths.",
        _registryData.Schemas.Count, _registryData.Paths.Count);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading registry file: {Message}. Starting with a fresh registry.", ex.Message);
      _registryData = new DiscoveryRegistryModel();
    }
  }

  public async Task SaveAsync()
  {
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json    = JsonSerializer.Serialize(_registryData, options);
    await File.WriteAllTextAsync(_registryFilePath, json);
    _logger.LogInformation("Discovery registry saved to disk.");
  }

  // --- Schema Methods ---

  public bool IsKnownSchema(string schemaName) => _registryData.Schemas.Contains(schemaName);
  public void AddSchema(string     schemaName) => _registryData.Schemas.Add(schemaName);

  // --- Path/Property Methods ---

  public bool IsKnownPath(string path) => _registryData.Paths.ContainsKey(path);

  public HashSet<string> GetKnownPropertiesForPath(string path)
  {
    return _registryData.Paths.TryGetValue(path, out var properties) ? properties : new HashSet<string>();
  }

  public void AddPropertyToPath(string path, string propertyName)
  {
    if (!_registryData.Paths.TryGetValue(path, out var properties))
    {
      properties                = new HashSet<string>();
      _registryData.Paths[path] = properties;
    }

    properties.Add(propertyName);
  }

  #endregion
}
