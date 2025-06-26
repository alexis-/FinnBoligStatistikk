namespace FinnStatistikk.DiscoveryTool.Services;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

public class DiscoveryEngine
{
  #region Properties & Fields - Non-Public

  private readonly DiscoveryRegistry        _registry;
  private readonly DiscoveryLogger          _logger;
  private readonly ILogger<DiscoveryEngine> _engineLogger;

  #endregion

  #region Constructors

  public DiscoveryEngine(DiscoveryRegistry registry, DiscoveryLogger logger, ILogger<DiscoveryEngine> engineLogger)
  {
    _registry     = registry;
    _logger       = logger;
    _engineLogger = engineLogger;
  }

  #endregion

  #region Methods

  public async Task ProcessResponseAsync(JObject rootNode, string sourceUrl)
  {
    await TraverseAsync(rootNode, "$", sourceUrl);
  }

  private async Task TraverseAsync(JToken? node, string currentPath, string sourceUrl)
  {
    if (node is null) return;

    if (node is JObject jObject)
    {
      // Special check for new schema names
      if (currentPath.EndsWith(".meta") && jObject.TryGetValue("schemaName", out var schemaNameToken))
      {
        var schemaName = schemaNameToken?.Value<string>();
        if (!string.IsNullOrEmpty(schemaName) && !_registry.IsKnownSchema(schemaName))
        {
          _engineLogger.LogInformation("[DISCOVERY] ✨ New schemaName found: '{SchemaName}' from {SourceUrl}", schemaName, sourceUrl);
          _registry.AddSchema(schemaName);

          var adNode = node.Parent?.Parent?["ad"];
          if (adNode is JObject adJObject) // Check that it's an object before logging
            await _logger.LogNewSchemaAsync(schemaName, sourceUrl, adJObject);
        }
      }

      // General check for new properties in an object
      var knownProperties = _registry.GetKnownPropertiesForPath(currentPath);

      // This logic now works perfectly because Newtonsoft already handled duplicates during parsing.
      var currentProperties = jObject.Properties().Select(p => p.Name).ToHashSet();

      var newProperties = currentProperties.Except(knownProperties).ToList();

      if (newProperties.Any())
      {
        _engineLogger.LogWarning("[DISCOVERY] ✨ New properties found at path '{Path}': {Properties}", currentPath,
                                 string.Join(", ", newProperties));

        foreach (var prop in newProperties)
          _registry.AddPropertyToPath(currentPath, prop);
        await _logger.LogNewStructureAsync(currentPath, newProperties, sourceUrl, jObject);
      }

      // Recurse into children
      foreach (var property in jObject.Properties().ToList()) // .ToList() for safe iteration
        await TraverseAsync(property.Value, $"{currentPath}.{property.Name}", sourceUrl);
    }
    else if (node is JArray jArray)
    {
      // Recurse into array elements
      foreach (var item in jArray.ToList()) // .ToList() for safe iteration
        await TraverseAsync(item, $"{currentPath}[*]", sourceUrl);
    }
  }

  #endregion
}
