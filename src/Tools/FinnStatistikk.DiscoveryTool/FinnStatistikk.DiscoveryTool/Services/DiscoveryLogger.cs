namespace FinnStatistikk.DiscoveryTool.Services;

using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DiscoveryLogger
{
  #region Constants & Statics

  private const string LogFileName = "discovery_log.md";

  #endregion

  #region Methods

  public async Task LogNewSchemaAsync(string schemaName, string sourceUrl, JObject adObject)
  {
    var sb = new StringBuilder();
    sb.AppendLine("---");
    sb.AppendLine($"- **Timestamp**: `{DateTime.UtcNow:O}`");
    sb.AppendLine($"- **Discovery Type**: `NEW_SCHEMA_NAME`");
    sb.AppendLine($"- **JSON Path**: `$.meta.schemaName`");
    sb.AppendLine($"- **Discovered Value**: `\"{schemaName}\"`");
    sb.AppendLine($"- **Source URL**: `{sourceUrl}`");
    sb.AppendLine($"- **Contextual JSON Snippet ($.ad)**:");
    sb.AppendLine("```json");
    sb.AppendLine(adObject.ToString(Formatting.Indented));
    sb.AppendLine("```");
    sb.AppendLine();

    await File.AppendAllTextAsync(LogFileName, sb.ToString());
  }

  public async Task LogNewStructureAsync(string path, IEnumerable<string> newProperties, string sourceUrl, JObject parentObject)
  {
    var sb = new StringBuilder();
    sb.AppendLine("---");
    sb.AppendLine($"- **Timestamp**: `{DateTime.UtcNow:O}`");
    sb.AppendLine($"- **Discovery Type**: `NEW_STRUCTURE`");
    sb.AppendLine($"- **JSON Path**: `{path}`");
    sb.AppendLine($"- **Discovered Properties**: `{string.Join(", ", newProperties.Select(p => $"\"{p}\""))}`");
    sb.AppendLine($"- **Source URL**: `{sourceUrl}`");
    sb.AppendLine($"- **Contextual JSON Snippet ({path})**:");
    sb.AppendLine("```json");
    sb.AppendLine(parentObject.ToString(Formatting.Indented));
    sb.AppendLine("```");
    sb.AppendLine();

    await File.AppendAllTextAsync(LogFileName, sb.ToString());
  }

  #endregion
}
